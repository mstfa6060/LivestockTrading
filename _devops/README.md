# DevOps Kılavuzu

## 🚀 Hızlı Başlangıç

### Development Ortamı

```bash
cd _devops/docker
docker compose -p maden_dev --env-file env/dev.env -f compose/common.yml -f compose/dev.yml up -d
```

### Production Ortamı

```bash
cd _devops/docker
docker compose -p maden_prod --env-file env/prod.env -f compose/common.yml -f compose/prod.yml up -d
```

## 📁 Dosya Yapısı

```
_devops/
├── docker/
│   ├── compose/           # Docker compose dosyaları
│   │   ├── common.yml     # Ortak servisler
│   │   ├── dev.yml        # Development override
│   │   └── prod.yml       # Production override
│   ├── env/               # Environment dosyaları
│   │   ├── dev.env        # Development değişkenleri
│   │   └── prod.env       # Production değişkenleri
│   ├── nginx/             # Nginx yapılandırması
│   └── Dockerfile-*       # Servis Dockerfile'ları
└── scripts/               # Yardımcı scriptler
```

## 🔧 Servisler

| Servis | Port | Açıklama |
|--------|------|----------|
| API Gateway | 5000 | Ana gateway |
| Hirovo API | 8090 | Hirovo modülü |
| IAM API | 8081 | Kimlik yönetimi |
| FileProvider API | 8082 | Dosya servisi |
| SQL Server | 1433 | Veritabanı |
| RabbitMQ | 5672/15672 | Message broker |

## 🔒 Güvenlik

### Environment Dosyaları
- `dev.env`: Development şifreleri
- `prod.env`: **ÖNEMLİ**: Production şifrelerini güncelleyin!

### Önemli Notlar
- Production şifreleri placeholder'dır, gerçek değerlerle değiştirin
- `.env` dosyalarını `.gitignore`'a ekleyin
- Secrets management sistemini düşünün

## 🚀 CI/CD

### GitHub Actions
- `dev` branch → Development deployment
- `main` branch → Production deployment

### Deployment Komutları
```bash
# Development
docker compose -p maden_dev --env-file env/dev.env -f compose/common.yml -f compose/dev.yml pull
docker compose -p maden_dev --env-file env/dev.env -f compose/common.yml -f compose/dev.yml up -d

# Production
docker compose -p maden_prod --env-file env/prod.env -f compose/common.yml -f compose/prod.yml pull
docker compose -p maden_prod --env-file env/prod.env -f compose/common.yml -f compose/prod.yml up -d
```

## 🧹 Temizlik

```bash
# Kullanılmayan image'ları temizle
docker image prune -f

# Sistem temizliği
docker system prune -af --volumes
```

## 📝 Loglar

```bash
# Servis logları
docker compose -p maden_dev logs -f [servis-adı]

# Tüm loglar
docker compose -p maden_dev logs -f
```
