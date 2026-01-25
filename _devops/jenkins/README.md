# Jenkins CI/CD Kurulumu

Bu dokuman, LivestockTrading projesi icin Jenkins CI/CD pipeline kurulumunu aciklar.

## Pipeline Dosyalari

| Dosya | Amac | Trigger |
|-------|------|---------|
| `Jenkinsfile.ci` | Build & Test | PR'lar ve tum push'lar |
| `Jenkinsfile.dev` | DEV Deployment | dev branch push |
| `Jenkinsfile.prod` | PROD Deployment | main branch push (onay gerekli) |

---

## 1. Jenkins Gereksinimleri (Linux Sunucu)

### .NET SDK 8.0 Kurulumu

```bash
# Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Dogrulama
dotnet --version
```

### Docker Kurulumu

```bash
# Ubuntu
sudo apt-get update
sudo apt-get install -y ca-certificates curl gnupg
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Jenkins kullanicisini docker grubuna ekle
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins

# Dogrulama
docker --version
docker compose version
```

---

## 2. Jenkins Eklentileri

Jenkins > Manage Jenkins > Plugins > Available plugins

Asagidaki eklentileri kur:
- **Pipeline** (varsayilan)
- **Git plugin**
- **GitHub plugin**
- **Docker Pipeline**
- **SSH Agent Plugin**
- **Credentials Binding Plugin**
- **Timestamper** (log'larda zaman damgasi)
- **Workspace Cleanup** (build sonrasi temizlik)

---

## 3. Jenkins Credentials Ayarlari

Jenkins > Manage Jenkins > Credentials > System > Global credentials

### a) Docker Hub Credentials
- **Kind:** Username with password
- **ID:** `docker-hub-credentials`
- **Username:** `mstfaock`
- **Password:** Docker Hub token/sifre

### b) DEV Server SSH Key
- **Kind:** SSH Username with private key
- **ID:** `dev-server-ssh-key`
- **Username:** (sunucu kullanici adi)
- **Private Key:** Enter directly > SSH private key yapistir

### c) DEV Server Host
- **Kind:** Secret text
- **ID:** `dev-server-host`
- **Secret:** (DEV sunucu IP adresi)

### d) DEV Server User
- **Kind:** Secret text
- **ID:** `dev-server-user`
- **Secret:** (DEV sunucu kullanici adi)

### e) PROD Server SSH Key
- **Kind:** SSH Username with private key
- **ID:** `prod-server-ssh-key`
- **Username:** (sunucu kullanici adi)
- **Private Key:** Enter directly > SSH private key yapistir

### f) PROD Server Host
- **Kind:** Secret text
- **ID:** `prod-server-host`
- **Secret:** (PROD sunucu IP adresi)

### g) PROD Server User
- **Kind:** Secret text
- **ID:** `prod-server-user`
- **Secret:** (PROD sunucu kullanici adi)

---

## 4. Pipeline Job'lari Olusturma

### 4.1 CI Pipeline (Build & Test)

1. **New Item** > `livestock-trading-ci` > **Pipeline** > OK
2. **General:**
   - [x] GitHub project: `https://github.com/mstfa6060/LivestockTrading`
3. **Build Triggers:**
   - [x] GitHub hook trigger for GITScm polling
4. **Pipeline:**
   - Definition: **Pipeline script from SCM**
   - SCM: **Git**
   - Repository URL: `https://github.com/mstfa6060/LivestockTrading.git`
   - Credentials: (GitHub credentials ekle)
   - Branches to build: `*/dev`, `*/main`, `*/feature/*`
   - Script Path: `Jenkinsfile.ci`
5. **Save**

### 4.2 DEV Deployment Pipeline

1. **New Item** > `livestock-trading-dev` > **Pipeline** > OK
2. **General:**
   - [x] GitHub project: `https://github.com/mstfa6060/LivestockTrading`
3. **Build Triggers:**
   - [x] GitHub hook trigger for GITScm polling
4. **Pipeline:**
   - Definition: **Pipeline script from SCM**
   - SCM: **Git**
   - Repository URL: `https://github.com/mstfa6060/LivestockTrading.git`
   - Credentials: (GitHub credentials)
   - Branches to build: `*/dev`
   - Script Path: `Jenkinsfile.dev`
5. **Save**

### 4.3 PROD Deployment Pipeline

1. **New Item** > `livestock-trading-prod` > **Pipeline** > OK
2. **General:**
   - [x] GitHub project: `https://github.com/mstfa6060/LivestockTrading`
3. **Build Triggers:**
   - [x] GitHub hook trigger for GITScm polling
4. **Pipeline:**
   - Definition: **Pipeline script from SCM**
   - SCM: **Git**
   - Repository URL: `https://github.com/mstfa6060/LivestockTrading.git`
   - Credentials: (GitHub credentials)
   - Branches to build: `*/main`
   - Script Path: `Jenkinsfile.prod`
5. **Save**

---

## 5. GitHub Webhook Ayari

GitHub Repo > Settings > Webhooks > Add webhook

- **Payload URL:** `http://<JENKINS_URL>/github-webhook/`
- **Content type:** `application/json`
- **Secret:** (opsiyonel, guvenlik icin onerilir)
- **Which events:**
  - [x] Just the push event
  - [x] Pull requests (CI icin)
- **Active:** [x]

### Webhook URL Ornekleri:
```
http://123.45.67.89:8080/github-webhook/
https://jenkins.livestock-trading.com/github-webhook/
```

---

## 6. Jenkins'i Public Erisime Acma

### Secenek A: Nginx Reverse Proxy (Onerilen)

```bash
sudo apt-get install -y nginx
```

`/etc/nginx/sites-available/jenkins`:
```nginx
upstream jenkins {
    keepalive 32;
    server 127.0.0.1:8080;
}

server {
    listen 80;
    server_name jenkins.yourdomain.com;

    # SSL icin Let's Encrypt kullanin
    # listen 443 ssl;
    # ssl_certificate /etc/letsencrypt/live/jenkins.yourdomain.com/fullchain.pem;
    # ssl_certificate_key /etc/letsencrypt/live/jenkins.yourdomain.com/privkey.pem;

    location / {
        proxy_pass http://jenkins;
        proxy_http_version 1.1;

        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header Connection "";

        # Websocket destegi
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";

        proxy_read_timeout 90s;
        proxy_buffering off;
    }
}
```

```bash
sudo ln -s /etc/nginx/sites-available/jenkins /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### Secenek B: Cloudflare Tunnel (Onerilen - Guvenli)

```bash
# cloudflared kurulumu
curl -L --output cloudflared.deb https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
sudo dpkg -i cloudflared.deb

# Tunnel olustur
cloudflared tunnel login
cloudflared tunnel create jenkins-tunnel
cloudflared tunnel route dns jenkins-tunnel jenkins.yourdomain.com

# config.yml
cat > ~/.cloudflared/config.yml << EOF
tunnel: <TUNNEL_ID>
credentials-file: /home/user/.cloudflared/<TUNNEL_ID>.json

ingress:
  - hostname: jenkins.yourdomain.com
    service: http://localhost:8080
  - service: http_status:404
EOF

# Servis olarak calistir
sudo cloudflared service install
sudo systemctl start cloudflared
```

### Secenek C: ngrok (Gecici Test)

```bash
ngrok http 8080
# Verilen URL'i GitHub webhook'a ekle
```

---

## 7. Calisma Akisi

```
[Developer] --push--> [GitHub]
                          |
                          v
                    [Webhook]
                          |
          +---------------+---------------+
          |               |               |
          v               v               v
    [CI Pipeline]   [DEV Pipeline]   [PROD Pipeline]
    (Jenkinsfile.ci) (Jenkinsfile.dev) (Jenkinsfile.prod)
          |               |               |
          v               v               v
    Build & Test    Build Images     Build Images
                    Push to Hub      Push to Hub
                    Deploy DEV       [Manual Onay]
                                     Deploy PROD
```

---

## 8. Sunucu Hazirligi (DEV/PROD)

### SSH Key Ayari

Jenkins sunucusunda:
```bash
# Jenkins kullanicisi olarak
sudo su - jenkins
ssh-keygen -t ed25519 -C "jenkins@livestock-trading"

# Public key'i hedef sunuculara kopyala
ssh-copy-id user@dev-server
ssh-copy-id user@prod-server
```

### Hedef Sunucularda

```bash
# Proje dizini olustur
sudo mkdir -p /opt/livestocktrading
sudo chown -R $USER:$USER /opt/livestocktrading
cd /opt/livestocktrading

# Repo'yu klonla
git clone https://github.com/mstfa6060/LivestockTrading.git .

# Docker compose dosyalarini kopyala
cp _devops/docker/compose/docker-compose.yml .
cp _devops/docker/compose/docker-compose.dev.yml .   # veya prod
cp _devops/docker/env/.env.example .env.dev          # veya .env.prod

# .env dosyasini duzenle
nano .env.dev
```

---

## 9. Sorun Giderme

### Jenkins'te dotnet bulunamadi
```bash
# Jenkins environment'a PATH ekle
# Jenkins > Manage Jenkins > System > Global properties
# Environment variables:
# Name: PATH
# Value: /usr/share/dotnet:$PATH
```

### Docker permission denied
```bash
sudo usermod -aG docker jenkins
sudo systemctl restart jenkins
# Jenkins'i yeniden baslat ve job'u tekrar calistir
```

### SSH baglanti sorunu
```bash
# Jenkins sunucusundan test
sudo su - jenkins
ssh -v user@target-server

# Known hosts temizle
ssh-keygen -R target-server
```

### Webhook calismiyor
1. GitHub > Webhooks > Recent Deliveries kontrol et
2. Jenkins > Manage Jenkins > System > GitHub > Test connection
3. Firewall kontrolu: `sudo ufw allow 8080/tcp`

### Build cok uzun suruyor
```bash
# Docker cache kullan
# Jenkinsfile'da DOCKER_BUILDKIT=1 ekle
export DOCKER_BUILDKIT=1
```

---

## 10. Guvenlik Onerileri

1. **Jenkins'i HTTPS uzerinden sun** (Let's Encrypt + Nginx)
2. **GitHub Webhook Secret kullan**
3. **Jenkins kullanicilarini sinirla** (Role-based Authorization)
4. **Credentials'lari guvenli sakla** (HashiCorp Vault entegrasyonu)
5. **Duzgun firewall kurallari** (sadece gerekli portlar acik)
6. **Docker image'lari tara** (Trivy, Snyk)

---

## Hizli Baslangic Kontrol Listesi

- [ ] Jenkins kuruldu ve calisiyoruz
- [ ] .NET SDK 8.0 kuruldu
- [ ] Docker kuruldu ve jenkins kullanicisi docker grubunda
- [ ] Gerekli Jenkins eklentileri kuruldu
- [ ] Docker Hub credentials eklendi
- [ ] DEV server credentials eklendi
- [ ] PROD server credentials eklendi
- [ ] 3 pipeline job olusturuldu (ci, dev, prod)
- [ ] GitHub webhook ayarlandi
- [ ] Hedef sunucular hazirlandi
- [ ] Ilk test build basarili
