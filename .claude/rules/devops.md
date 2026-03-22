---
paths:
  - "_devops/**"
  - "**/docker-compose*"
  - "**/Dockerfile*"
  - "**/Jenkinsfile*"
  - "**/ocelot.json"
---
# API Gateway & Routing

**Production/Dev Architecture:**
```
Internet -> Nginx (443) -> API Gateway (Ocelot) -> Backend APIs
                              |
                    +---------+---------+
                    |         |         |
               IAM API   FileProvider  LivestockTrading
```

**Ocelot Gateway** (`Gateways/Api/Gateways.ApiGateway.Api/ocelot.json`):
- Routes requests based on URL prefix
- Handles JWT authentication centrally
- Public endpoints (no auth): `/iam/Auth/*`, `/iam/Users/Create`, `/iam/Countries/All`
- Protected endpoints: All others require valid JWT

**Route Mappings:**
- `/iam/*` -> `iam-api-container:8080`
- `/fileprovider/*` -> `fileprovider-api-container:8080`
- `/livestocktrading/*` -> `livestocktrading-api-container:8080`

**Port Configuration (Docker):**
| Service | Default Port | Container Port |
|---------|-------------|----------------|
| API Gateway | 5000 (GATEWAY_PORT) | 8080 |
| LivestockTrading API | 5001 (GLOBALLIVESTOCK_API_PORT) | 8080 |
| IAM API | 5002 (IAM_API_PORT) | 8080 |
| FileProvider API | 5003 (FILEPROVIDER_API_PORT) | 8080 |

**Adding New Route to Gateway:**
1. Edit `Gateways/Api/Gateways.ApiGateway.Api/ocelot.json`
2. Add public route BEFORE catch-all routes
3. For authenticated endpoints, add `AuthenticationOptions` with the secret key
4. Rebuild and deploy API Gateway

# DevOps & Deployment

## Docker Structure

```
_devops/
├── docker/
│   ├── compose/
│   │   ├── docker-compose.yml          # Base services
│   │   ├── docker-compose.dev.yml      # Dev overrides
│   │   └── docker-compose.prod.yml     # Prod overrides (resource limits)
│   ├── env/
│   │   └── .env.example                # Environment template
│   ├── Dockerfile.api-gateway
│   ├── Dockerfile.livestocktrading-api
│   ├── Dockerfile.iam-api
│   ├── Dockerfile.fileprovider-api
│   ├── Dockerfile.iam-mail-worker
│   ├── Dockerfile.iam-sms-worker
│   └── Dockerfile.resource-seeder
└── jenkins/
    ├── Jenkinsfile.dev                 # Dev pipeline (dev branch)
    └── Jenkinsfile.prod                # Prod pipeline (main branch)
```

## Docker Compose Usage

```bash
# Development
cd _devops/docker/compose
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d

# Production
docker compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d
```

## Jenkins CI/CD

**Dev Pipeline (`Jenkinsfile.dev`):**
- Triggers on `dev` branch
- Builds all images with `dev-latest` tag
- Deploys to dev server
- Runs migrations/seeders automatically

**Prod Pipeline (`Jenkinsfile.prod`):**
- Triggers on `main` branch
- Builds with `latest` + immutable tag (`prod-{BUILD_ID}-{COMMIT}`)
- Deploys to production server
- Migrations require explicit parameter

**Build Modules:**
```
api-gateway, livestocktrading-api, iam-api, fileprovider-api,
iam-mail-worker, iam-sms-worker, resource-seeder
```

## Server Paths

```
/opt/livestocktrading/
├── repo/                    # Git repository
├── .env.dev                 # Dev environment variables
└── .env.prod                # Prod environment variables
```
