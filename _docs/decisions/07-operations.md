# Karar 7 — Operasyonel Plan (Observability + Operations + Process)

**Status:** FINAL
**Üst:** [README.md](README.md)
**İlgili:** [01-architecture.md](01-architecture.md), [04-migration.md](04-migration.md), [05-patch.md](05-patch.md)

---

## İçindekiler

- [Grup A — Observability + Monitoring](#grup-a--observability--monitoring)
  - [1. Prometheus + Grafana (Metrics)](#1-prometheus--grafana-metrics)
  - [2. Loki + Serilog (Logs)](#2-loki--serilog-logs)
  - [3. Sentry (Error Tracking)](#3-sentry-error-tracking)
  - [4. OpenTelemetry (Traces)](#4-opentelemetry-traces)
  - [5. Health Checks](#5-health-checks)
- [Grup B — Operations + Infrastructure](#grup-b--operations--infrastructure)
  - [1. CI/CD Pipeline](#1-cicd-pipeline)
  - [2. Backup & DR](#2-backup--dr)
  - [3. Secret Management](#3-secret-management)
  - [4. External Integrations](#4-external-integrations)
  - [5. Security Hardening](#5-security-hardening)
- [Grup C — Process + Performance + Faz 2 + DX](#grup-c--process--performance--faz-2--dx)
  - [1. SLO + Load Test](#1-slo--load-test)
  - [2. PR Review / Runbook / Postmortem](#2-pr-review--runbook--postmortem)
  - [3. Faz 2 Readiness (K3s, PgBouncer, Meilisearch)](#3-faz-2-readiness-k3s-pgbouncer-meilisearch)
  - [4. Developer Experience (DX)](#4-developer-experience-dx)

---

## Grup A — Observability + Monitoring

### 1. Prometheus + Grafana (Metrics)

#### 1.1. OpenTelemetry Exporter Setup

Tüm modüller (10 API + 2 Worker host) Prometheus metrics endpoint expose eder. OpenTelemetry SDK ile instrument edilir; Prometheus scrape eder, Grafana görselleştirir.

**Program.cs setup (her API host'unda):**

```csharp
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r
        .AddService(serviceName: builder.Environment.ApplicationName,
                    serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString())
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName,
            ["module.name"] = "identity"  // her modülde değişir
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()           // HTTP server (RED)
        .AddHttpClientInstrumentation()           // HTTP client (downstream)
        .AddRuntimeInstrumentation()              // GC, ThreadPool, Memory (USE)
        .AddProcessInstrumentation()              // CPU, RSS
        .AddNpgsql()                              // PostgreSQL queries
        .AddMeter("Livestock.Identity")           // custom domain meter
        .AddMeter("Livestock.Identity.Auth")
        .AddPrometheusExporter());

app.MapPrometheusScrapingEndpoint("/metrics");    // Prometheus pulls here
```

**Meter pattern (her modül kendi meter'ını yaratır):**

```csharp
// Domain layer
public static class IdentityMetrics
{
    public static readonly Meter Meter = new("Livestock.Identity", "1.0");

    public static readonly Counter<long> LoginAttempts = Meter.CreateCounter<long>(
        "livestock_identity_login_attempts_total",
        description: "Total login attempts by method and result");

    public static readonly Histogram<double> TokenIssuanceDuration = Meter.CreateHistogram<double>(
        "livestock_identity_token_issuance_duration_seconds",
        unit: "s",
        description: "JWT issuance latency");
}

// Handler içinde
IdentityMetrics.LoginAttempts.Add(1,
    new KeyValuePair<string, object?>("method", "email"),
    new KeyValuePair<string, object?>("result", "success"));
```

#### 1.2. Standart Metric Listesi (RED + USE)

**RED — Request, Errors, Duration (HTTP/gRPC katmanı, otomatik):**

| Metric | Type | Labels | Açıklama |
|---|---|---|---|
| `http_server_request_duration_seconds` | Histogram | method, route, status_code | Endpoint latency |
| `http_server_active_requests` | Gauge | method, route | In-flight requests |
| `http_client_request_duration_seconds` | Histogram | method, target, status_code | Downstream call latency |
| `kestrel_active_connections` | Gauge | endpoint | TCP connections |
| `kestrel_connection_duration_seconds` | Histogram | - | Connection lifetime |
| `signalr_connections_active` | Gauge | hub | Active WS connections |
| `signalr_messages_received_total` | Counter | hub, method | Inbound WS messages |
| `signalr_messages_sent_total` | Counter | hub, target | Outbound WS messages |

**USE — Utilization, Saturation, Errors (Runtime, otomatik):**

| Metric | Type | Açıklama |
|---|---|---|
| `process_cpu_seconds_total` | Counter | CPU consumed |
| `process_resident_memory_bytes` | Gauge | RSS |
| `dotnet_gc_heap_size_bytes` | Gauge (gen0/1/2/loh) | GC heap |
| `dotnet_gc_collections_total` | Counter (gen) | GC count |
| `dotnet_gc_pause_time_seconds_total` | Counter | GC pause time |
| `dotnet_thread_pool_threads_count` | Gauge | Thread pool size |
| `dotnet_thread_pool_queue_length` | Gauge | Queued work items |
| `dotnet_jit_il_compiled_bytes_total` | Counter | JIT throughput |
| `dotnet_exceptions_total` | Counter (type) | Unhandled exception count |
| `dotnet_assemblies_count` | Gauge | Loaded assemblies |

**Database (Npgsql instrumentation):**

| Metric | Type | Labels | Açıklama |
|---|---|---|---|
| `db_client_operation_duration_seconds` | Histogram | operation, db.name | Query latency |
| `db_client_connections_usage` | Gauge | state (idle/used), pool | Pool utilization |
| `db_client_connections_max` | Gauge | pool | Pool ceiling |
| `db_client_connections_pending_requests` | Gauge | pool | Connection wait queue |
| `db_client_connections_timeouts_total` | Counter | pool | Connection acquire timeouts |

**RabbitMQ (MassTransit instrumentation):**

| Metric | Type | Labels | Açıklama |
|---|---|---|---|
| `masstransit_consume_duration_seconds` | Histogram | message_type, consumer | Consume latency |
| `masstransit_consume_total` | Counter | message_type, result | Consume count |
| `masstransit_publish_duration_seconds` | Histogram | message_type | Publish latency |
| `masstransit_publish_total` | Counter | message_type, result | Publish count |
| `rabbitmq_queue_messages` | Gauge | queue | Queue depth (rabbitmq-exporter) |
| `rabbitmq_queue_messages_ready` | Gauge | queue | Unconsumed |
| `rabbitmq_consumer_count` | Gauge | queue | Live consumers |

**Cache (Redis):**

| Metric | Type | Labels | Açıklama |
|---|---|---|---|
| `cache_hits_total` | Counter | cache, tier (L1/L2) | Hit count |
| `cache_misses_total` | Counter | cache, tier | Miss count |
| `cache_evictions_total` | Counter | cache, reason | Eviction count |
| `cache_get_duration_seconds` | Histogram | cache, tier | Lookup latency |

#### 1.3. Custom Domain Metric Listesi (20+ per modül; toplam 200+)

**Identity:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_identity_login_attempts_total` | method (email/phone/nationalId), result (success/fail/locked/otp_required) | Login funnel |
| `livestock_identity_otp_sent_total` | channel (sms/email), purpose (login/verify/reset) | OTP throughput |
| `livestock_identity_otp_verified_total` | channel, result (success/expired/wrong) | OTP success rate |
| `livestock_identity_token_issuance_duration_seconds` | grant_type | JWT issuance latency |
| `livestock_identity_refresh_token_used_total` | result (success/revoked/expired) | Refresh hygiene |
| `livestock_identity_password_reset_total` | step (requested/completed) | Reset funnel |
| `livestock_identity_kvkk_consent_total` | consent_type, action (granted/revoked) | KVKK audit |
| `livestock_identity_data_export_requested_total` | format (json/csv) | GDPR requests |
| `livestock_identity_active_sessions` | (Gauge) | Active refresh tokens |

**Accounts:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_accounts_seller_verification_step_total` | step (D1..D8), result | Verification funnel |
| `livestock_accounts_seller_status_changes_total` | from, to | Status transitions |
| `livestock_accounts_review_submitted_total` | rating, role (buyer/seller) | Review volume |
| `livestock_accounts_follow_total` | action (follow/unfollow) | Follow graph delta |
| `livestock_accounts_vet_profile_created_total` | - | Vet onboarding |
| `livestock_accounts_farm_count` | (Gauge) purpose, country | Farm inventory |
| `livestock_accounts_certification_uploaded_total` | type | Certification activity |

**Listings:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_listings_created_total` | category, currency, country | Listing creation |
| `livestock_listings_status_changes_total` | from, to | Status transitions |
| `livestock_listings_search_total` | result_count_bucket | Search activity |
| `livestock_listings_view_total` | category | View events |
| `livestock_listings_edit_total` | edit_type (price/photo/description) | Edit policy hits |
| `livestock_listings_quota_check_total` | result (pass/blocked) | Plan quota gate |
| `livestock_listings_translation_pending` | (Gauge) target_lang | Translation backlog |
| `livestock_listings_reserve_total` | reason (offer_accepted/manual) | Reserve count |

**Marketplace:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_marketplace_offer_created_total` | category | Offer volume |
| `livestock_marketplace_offer_status_changes_total` | from, to | Funnel (sent→accepted→rejected→expired) |
| `livestock_marketplace_deal_created_total` | category, currency | Deal volume |
| `livestock_marketplace_deal_value_amount` | (Histogram) currency, category | GMV distribution |
| `livestock_marketplace_dispute_opened_total` | reason | Dispute volume |
| `livestock_marketplace_dispute_resolved_total` | resolution | Resolution mix |
| `livestock_marketplace_commission_charged_amount` | (Counter, double) currency | Commission revenue |
| `livestock_marketplace_escrow_balance` | (Gauge) currency | Held funds |

**Carrier:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_carrier_shipment_created_total` | service_type | Shipment volume |
| `livestock_carrier_shipment_status_changes_total` | from, to | Funnel |
| `livestock_carrier_capacity_utilization_ratio` | (Gauge) carrier_id | Capacity usage |
| `livestock_carrier_route_proposed_total` | source_country, dest_country | Route requests |

**Subscription:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_subscription_active_subscriptions` | (Gauge) plan, status | MRR slice |
| `livestock_subscription_invoice_total` | result (paid/failed/refunded) | Invoice outcome |
| `livestock_subscription_invoice_amount` | (Counter) currency | Recognized revenue |
| `livestock_subscription_payment_method_added_total` | provider | PM additions |
| `livestock_subscription_boost_campaign_started_total` | category, duration_days | Boost adoption |
| `livestock_subscription_quota_usage_ratio` | (Gauge) quota_type | Quota saturation |
| `livestock_subscription_churn_total` | reason | Churn |
| `livestock_subscription_dunning_step_total` | step (grace/notice/suspend) | Dunning funnel |

**Catalog:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_catalog_search_total` | type (category/brand/location) | Catalog search |
| `livestock_catalog_translation_coverage_ratio` | (Gauge) entity, target_lang | i18n coverage |
| `livestock_catalog_exchange_rate_fetch_total` | provider (tcmb/ecb/exchangerate), result | Rate provider health |
| `livestock_catalog_exchange_rate_age_seconds` | (Gauge) currency | Rate staleness |

**Messaging:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_messaging_messages_sent_total` | content_type (text/image/system) | Message volume |
| `livestock_messaging_typing_indicator_total` | - | Typing events |
| `livestock_messaging_read_receipt_total` | - | Read receipts |
| `livestock_messaging_conversations_active` | (Gauge) | Active conversations |

**Notifications:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_notifications_dispatched_total` | channel (push/email/sms/inapp), result | Delivery |
| `livestock_notifications_dispatch_duration_seconds` | channel | Delivery latency |
| `livestock_notifications_provider_failure_total` | provider, error_type | Provider health |
| `livestock_notifications_unsubscribe_total` | channel, reason | Opt-out |

**Admin:**

| Metric | Labels | Açıklama |
|---|---|---|
| `livestock_admin_action_total` | actor_role, action_type | Admin activity |
| `livestock_admin_impersonation_total` | result (started/ended) | Impersonation audit |
| `livestock_admin_audit_log_writes_total` | - | Audit volume |
| `livestock_admin_feature_flag_toggle_total` | flag, value | Flag changes |

#### 1.4. Prometheus Scrape Config (`prometheus.yml`)

```yaml
global:
  scrape_interval: 15s
  evaluation_interval: 15s
  external_labels:
    cluster: livestock-prod
    region: eu-central-1

rule_files:
  - /etc/prometheus/rules/*.yml

alerting:
  alertmanagers:
    - static_configs:
        - targets: ['alertmanager:9093']

scrape_configs:
  - job_name: 'livestock-modules'
    metrics_path: /metrics
    scrape_interval: 15s
    static_configs:
      - targets:
          - identity-api:8080
          - accounts-api:8080
          - catalog-api:8080
          - listings-api:8080
          - marketplace-api:8080
          - carrier-api:8080
          - subscription-api:8080
          - messaging-api:8080
          - notifications-api:8080
          - admin-api:8080
        labels:
          tier: api
    relabel_configs:
      - source_labels: [__address__]
        regex: '([^-]+)-api:.*'
        target_label: module
        replacement: '$1'

  - job_name: 'livestock-workers'
    static_configs:
      - targets:
          - notification-worker:8080
          - outbox-dispatcher:8080
        labels:
          tier: worker

  - job_name: 'postgres'
    static_configs:
      - targets: ['postgres-exporter:9187']

  - job_name: 'rabbitmq'
    static_configs:
      - targets: ['rabbitmq-exporter:9419']

  - job_name: 'redis'
    static_configs:
      - targets: ['redis-exporter:9121']

  - job_name: 'node'
    static_configs:
      - targets: ['node-exporter:9100']

  - job_name: 'nginx'
    static_configs:
      - targets: ['nginx-exporter:9113']

  - job_name: 'blackbox-http'
    metrics_path: /probe
    params:
      module: [http_2xx]
    static_configs:
      - targets:
          - https://api.livestock.com/health
          - https://api.livestock.com/identity/health/ready
    relabel_configs:
      - source_labels: [__address__]
        target_label: __param_target
      - source_labels: [__param_target]
        target_label: instance
      - target_label: __address__
        replacement: blackbox-exporter:9115
```

#### 1.5. Alert Rules (`rules/livestock.yml`)

```yaml
groups:
  - name: livestock-availability
    interval: 30s
    rules:
      - alert: ModuleDown
        expr: up{job="livestock-modules"} == 0
        for: 2m
        labels:
          severity: critical
          team: platform
        annotations:
          summary: "Module {{ $labels.module }} is down"
          runbook: "https://runbook.livestock.com/module-down"

      - alert: HighErrorRate
        expr: |
          (sum by (module) (rate(http_server_request_duration_seconds_count{status_code=~"5.."}[5m]))
           /
           sum by (module) (rate(http_server_request_duration_seconds_count[5m]))) > 0.05
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Error rate > 5% on {{ $labels.module }}"

      - alert: LatencyP95High
        expr: |
          histogram_quantile(0.95,
            sum by (module, route, le) (rate(http_server_request_duration_seconds_bucket[5m]))
          ) > 1.0
        for: 10m
        labels:
          severity: warning
        annotations:
          summary: "p95 latency > 1s on {{ $labels.module }}{{ $labels.route }}"

  - name: livestock-database
    interval: 30s
    rules:
      - alert: DbPoolSaturation
        expr: |
          db_client_connections_usage{state="used"} /
          db_client_connections_max > 0.8
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "DB pool > 80% utilized on {{ $labels.pool }}"

      - alert: DbConnectionTimeouts
        expr: rate(db_client_connections_timeouts_total[5m]) > 0.1
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "DB connection timeouts on {{ $labels.pool }}"

      - alert: SlowQueries
        expr: |
          histogram_quantile(0.95,
            rate(db_client_operation_duration_seconds_bucket[5m])
          ) > 0.5
        for: 10m
        labels:
          severity: warning
        annotations:
          summary: "DB p95 query > 500ms on {{ $labels.module }}"

  - name: livestock-messaging
    interval: 30s
    rules:
      - alert: RabbitQueueBacklog
        expr: rabbitmq_queue_messages_ready > 10000
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Queue {{ $labels.queue }} backlog > 10k"

      - alert: OutboxStuck
        expr: |
          (time() - max(masstransit_publish_total{message_type=~".*Event"})) > 300
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "Outbox publish stuck on {{ $labels.module }}"

      - alert: ConsumerLag
        expr: rabbitmq_queue_messages_ready > 1000 and rabbitmq_consumer_count == 0
        for: 2m
        labels:
          severity: critical
        annotations:
          summary: "Queue {{ $labels.queue }} has backlog and no consumer"

  - name: livestock-domain
    interval: 60s
    rules:
      - alert: LoginFailureSpike
        expr: |
          (sum(rate(livestock_identity_login_attempts_total{result="fail"}[5m]))
           /
           sum(rate(livestock_identity_login_attempts_total[5m]))) > 0.5
        for: 10m
        labels:
          severity: warning
          team: identity
        annotations:
          summary: "Login failure ratio > 50% — possible brute force"

      - alert: PaymentProviderDown
        expr: |
          rate(livestock_subscription_invoice_total{result="failed"}[10m]) > 1
          and
          rate(livestock_subscription_invoice_total{result="paid"}[10m]) == 0
        for: 5m
        labels:
          severity: critical
          team: payments
        annotations:
          summary: "All invoices failing — payment provider issue"

      - alert: NotificationProviderFailures
        expr: rate(livestock_notifications_provider_failure_total[5m]) > 0.1
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Notification provider {{ $labels.provider }} failing"

      - alert: ExchangeRateStale
        expr: livestock_catalog_exchange_rate_age_seconds > 86400
        for: 1h
        labels:
          severity: warning
        annotations:
          summary: "Exchange rate for {{ $labels.currency }} stale > 24h"

      - alert: DisputeSpike
        expr: increase(livestock_marketplace_dispute_opened_total[1h]) > 20
        for: 5m
        labels:
          severity: warning
          team: operations
        annotations:
          summary: "Unusual dispute volume — investigate"

  - name: livestock-infrastructure
    interval: 30s
    rules:
      - alert: HighCpu
        expr: avg by (instance) (rate(process_cpu_seconds_total[5m])) * 100 > 80
        for: 10m
        labels:
          severity: warning
        annotations:
          summary: "CPU > 80% on {{ $labels.instance }}"

      - alert: HighMemory
        expr: process_resident_memory_bytes / on(instance) node_memory_MemTotal_bytes > 0.85
        for: 10m
        labels:
          severity: warning

      - alert: DiskFull
        expr: (node_filesystem_avail_bytes / node_filesystem_size_bytes) < 0.15
        for: 5m
        labels:
          severity: critical

      - alert: GcPressure
        expr: rate(dotnet_gc_pause_time_seconds_total[5m]) > 0.1
        for: 10m
        labels:
          severity: warning
        annotations:
          summary: "GC pausing > 10% of time on {{ $labels.module }}"
```

#### 1.6. Grafana Dashboard Listesi (10 dashboard)

| # | Dashboard | Scope | Panels |
|---|---|---|---|
| 1 | **Platform Overview** | All modules | RED across modules, top-5 error endpoints, top-5 slow endpoints, error budget burn |
| 2 | **Module Drill-Down** | Per-module (template var) | RED for module, DB pool, RabbitMQ in/out, cache hit ratio, GC, custom domain panel slot |
| 3 | **Identity & Auth** | Identity | Login funnel, OTP success ratio, active sessions, token issuance p50/p95, refresh hygiene |
| 4 | **Marketplace & Revenue** | Marketplace + Subscription | GMV by currency, deal funnel (offer→deal), commission revenue, MRR by plan, invoice success ratio |
| 5 | **Listings Health** | Listings + Catalog | Listing creation rate, search QPS, translation coverage by lang, exchange rate freshness |
| 6 | **Real-time / SignalR** | Messaging + Notifications | Active connections per hub, message throughput, notification dispatch by channel, provider error rate |
| 7 | **Database** | PostgreSQL | Per-module pool utilization, query p95 by module, slow query top-10, replication lag, table bloat |
| 8 | **Queue & Outbox** | RabbitMQ + Outbox | Queue depth, consumer lag, outbox dispatch latency, dead letter count, retry count |
| 9 | **Infrastructure** | Node + Container | CPU/RAM/disk by host, container restarts, network errors, .NET GC heat map |
| 10 | **SLO Burn Rate** | All modules | Availability SLO, latency SLO, error budget remaining (multi-window: 1h/6h/24h/7d) |

**Grafana provisioning (`datasources.yml`):**

```yaml
apiVersion: 1
datasources:
  - name: Prometheus
    type: prometheus
    url: http://prometheus:9090
    isDefault: true
  - name: Loki
    type: loki
    url: http://loki:3100
  - name: Tempo
    type: tempo
    url: http://tempo:3200
    jsonData:
      tracesToLogsV2:
        datasourceUid: loki
        spanStartTimeShift: '-5m'
        spanEndTimeShift: '5m'
```

Dashboards stored as JSON in `_devops/grafana/dashboards/`, provisioned via `dashboards.yml` ConfigMap mount.

#### 1.7. SLI/SLO Targets

| Service | SLI | Target | Error Budget (30d) |
|---|---|---|---|
| Identity (login) | Availability (2xx/3xx ratio) | 99.9% | 43m 12s |
| Marketplace (offer/deal) | Availability | 99.9% | 43m 12s |
| Listings (search) | Latency p95 < 500ms | 99% | 7h 12m |
| Notifications (push dispatch) | Delivery within 30s | 99% | 7h 12m |
| Messaging (SignalR connect) | Connection success | 99.5% | 3h 36m |
| Cross-module event delivery | End-to-end < 5s p95 | 99% | 7h 12m |

---

### 2. Loki + Serilog (Logs)

**Serilog setup (every host):**

```csharp
builder.Host.UseSerilog((ctx, services, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("module", "identity")
    .Enrich.WithProperty("env", ctx.HostingEnvironment.EnvironmentName)
    .Enrich.WithSpan()                 // OpenTelemetry trace_id, span_id
    .Enrich.WithCorrelationId()
    .Enrich.WithMachineName()
    .WriteTo.Console(new RenderedCompactJsonFormatter())   // stdout → Docker → Promtail → Loki
    .WriteTo.Sentry(o =>
    {
        o.MinimumBreadcrumbLevel = LogEventLevel.Information;
        o.MinimumEventLevel = LogEventLevel.Error;
    }));
```

**Structured logging convention:**

```csharp
_logger.LogInformation("Login attempt {Method} {UserId} {Result}",
    "email", userId, "success");

// PII redaction — never log: password, OTP code, JWT, refresh token, NationalId, IBAN
// Destructure with [LogMasked] attribute (custom enricher)
```

**Loki labels (low cardinality only):**

- `module` (10 values)
- `env` (3: dev/staging/prod)
- `level` (5: Debug/Info/Warn/Error/Fatal)
- `tier` (api/worker)

High-cardinality fields (user_id, trace_id, correlation_id) live in **structured payload**, not labels — queryable via LogQL `| json | trace_id="..."`.

**Promtail pipeline (`promtail.yml` excerpt):**

```yaml
clients:
  - url: http://loki:3100/loki/api/v1/push

scrape_configs:
  - job_name: containers
    docker_sd_configs:
      - host: unix:///var/run/docker.sock
        refresh_interval: 10s
    relabel_configs:
      - source_labels: ['__meta_docker_container_label_module']
        target_label: module
      - source_labels: ['__meta_docker_container_label_env']
        target_label: env
    pipeline_stages:
      - json:
          expressions:
            level: '"@l"'
            trace_id: TraceId
            message: '"@mt"'
      - labels:
          level:
      - timestamp:
          source: '@t'
          format: RFC3339Nano
```

**Retention:**

| Tier | Storage | Retention |
|---|---|---|
| Hot (Loki local) | NVMe | 14 days |
| Warm (Loki S3 backend) | S3 IA | 90 days |
| Cold (Glacier export) | S3 Glacier | 7 years (KVKK audit) |

**Loki LogQL examples:**

```logql
# Errors per module
sum by (module) (rate({env="prod"} |= "error" [5m]))

# Specific trace
{module="marketplace"} | json | trace_id="01HJ..."

# Slow requests (>1s)
{module="identity"} | json | duration_ms > 1000 | line_format "{{.@mt}}"
```

---

### 3. Sentry (Error Tracking)

Sentry SaaS (self-hosted on-prem alternatif Faz 2 — GlitchTip).

**SDK setup:**

```csharp
builder.WebHost.UseSentry(o =>
{
    o.Dsn = builder.Configuration["Sentry:Dsn"];
    o.Environment = builder.Environment.EnvironmentName;
    o.Release = typeof(Program).Assembly.GetName().Version?.ToString();
    o.TracesSampleRate = 0.1;          // 10% transaction sampling
    o.ProfilesSampleRate = 0.1;
    o.SendDefaultPii = false;          // KVKK
    o.MaxBreadcrumbs = 100;
    o.AttachStacktrace = true;
    o.AutoSessionTracking = true;

    o.BeforeSend = evt =>
    {
        // PII scrub
        evt.Request.Headers.Remove("Authorization");
        evt.Request.Headers.Remove("Cookie");
        return evt;
    };
});

app.UseSentryTracing();
```

**Issue grouping convention:**

- Sentry'de proje başına 1 DSN — toplam 12 proje (10 API + 2 Worker)
- Tag: `module`, `wave`, `feature`
- Alert rules: yeni issue / regression / 100+ events/hour

**PII allowlist:**

User context'inde sadece `user_id` (Guid) ve `role` (string) gönderilir. Email, phone, IP scrub edilir.

---

### 4. OpenTelemetry (Traces)

**Setup:**

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetSampler(new TraceIdRatioBasedSampler(0.1))     // 10% in prod, 1.0 in dev
        .AddAspNetCoreInstrumentation(o =>
        {
            o.RecordException = true;
            o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health")
                             && !ctx.Request.Path.StartsWithSegments("/metrics");
        })
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(o => o.SetDbStatementForText = true)
        .AddNpgsql()
        .AddSource("MassTransit")
        .AddSource("Livestock.*")
        .AddOtlpExporter(o => o.Endpoint = new Uri("http://tempo:4317")));
```

**Custom span pattern:**

```csharp
private static readonly ActivitySource Source = new("Livestock.Marketplace");

using var activity = Source.StartActivity("OfferAccept");
activity?.SetTag("offer.id", offerId);
activity?.SetTag("buyer.id", buyerId);
// ... logic
activity?.SetStatus(ActivityStatusCode.Ok);
```

**Trace correlation:**

- Serilog auto-attaches `trace_id` and `span_id` to every log line
- Grafana → Loki query → "View trace" link → Tempo → distributed view
- W3C Trace Context (`traceparent` header) propagates across modules + RabbitMQ + SignalR

**Tempo retention:** 14 days hot, no warm tier (traces are sampled — re-sample if needed).

---

### 5. Health Checks

**Endpoints (every API host):**

| Path | Purpose | Checks |
|---|---|---|
| `/health/live` | Kubernetes/Docker liveness | Process responsive (no deps) |
| `/health/ready` | Readiness | DB, Redis, RabbitMQ reachable |
| `/health/startup` | Startup probe | Migrations applied, seed loaded |

**Setup:**

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Identity")!,
        name: "postgres-identity", tags: new[] { "ready" })
    .AddRedis(builder.Configuration["Caching:Redis:ConnectionString"]!,
        name: "redis", tags: new[] { "ready" })
    .AddRabbitMQ(rabbitConnectionString: builder.Configuration["RabbitMQ:ConnectionString"]!,
        name: "rabbitmq", tags: new[] { "ready" })
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddCheck<MigrationsAppliedHealthCheck>("migrations", tags: new[] { "startup" });

app.MapHealthChecks("/health/live",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
app.MapHealthChecks("/health/ready",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("ready") });
app.MapHealthChecks("/health/startup",
    new HealthCheckOptions { Predicate = r => r.Tags.Contains("startup") });
```

**Docker Compose healthcheck:**

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:8080/health/ready"]
  interval: 30s
  timeout: 5s
  retries: 3
  start_period: 60s
```

**Synthetic uptime (Blackbox Exporter — see Prometheus config):**

External probes against `/health/ready` of each API + gateway every 15s. Alert on consecutive 3 failures.

---

## Grup B — Operations + Infrastructure

### 1. CI/CD Pipeline

**Repository:** Tek mono-repo (current: `c:\workspace\GlobalLivestock\LivestockTrading`). Branch strategy: `main` (prod) ← `dev` (default integration) ← feature branches.

**Pipeline tool:** Jenkins (mevcut). Faz 2'de GitHub Actions değerlendirilebilir.

**Pipeline aşamaları:**

```
1. Checkout
2. Restore + Build (dotnet build -c Release)
3. Unit Tests (dotnet test --filter Category=Unit)
4. Integration Tests (Testcontainers: PG + Redis + RabbitMQ)
5. Static Analysis
   - dotnet format --verify-no-changes
   - Roslyn analyzers (built-in)
   - SonarQube scan (optional Faz 1.5)
6. Security Scan
   - dotnet list package --vulnerable
   - Trivy image scan
   - Gitleaks (secret detection)
7. Docker Build (per module — 10 API + 2 Worker + Gateway)
   - Multi-stage: SDK → runtime
   - Layer cache reuse
   - Image tag: ${BRANCH}-${BUILD_ID}-${COMMIT_SHA:0:7}
8. Push to Registry (Harbor/GHCR)
9. Migration Dry Run (dev only — pg-restore staging → run migrations)
10. Deploy
    - dev: auto on push to dev
    - prod: manual approval gate, push to main
11. Smoke Tests (curl health endpoints + 5 critical user flows)
12. Notification (Slack #deploys)
```

**Jenkinsfile.dev (Wave 0 timeline):**

```groovy
pipeline {
  agent { label 'docker' }

  environment {
    REGISTRY = 'harbor.internal.livestock.com'
    NAMESPACE = 'livestock'
  }

  stages {
    stage('Checkout') { steps { checkout scm } }

    stage('Build & Test') {
      parallel {
        stage('Unit') {
          steps { sh 'dotnet test --filter Category=Unit --logger trx --collect:"XPlat Code Coverage"' }
        }
        stage('Integration') {
          steps { sh 'dotnet test --filter Category=Integration' }
        }
        stage('Lint') {
          steps { sh 'dotnet format --verify-no-changes' }
        }
      }
    }

    stage('Security') {
      steps {
        sh 'dotnet list package --vulnerable --include-transitive'
        sh 'gitleaks detect --source . --no-banner'
      }
    }

    stage('Docker Build & Push') {
      steps {
        script {
          def modules = ['identity', 'accounts', 'catalog', 'listings',
                         'marketplace', 'carrier', 'subscription',
                         'messaging', 'notifications', 'admin',
                         'gateway', 'notification-worker', 'outbox-dispatcher']
          modules.each { m ->
            sh """
              docker build -f _devops/docker/Dockerfile.${m} \
                -t ${REGISTRY}/${NAMESPACE}/${m}:dev-${BUILD_ID}-${env.GIT_COMMIT.take(7)} \
                -t ${REGISTRY}/${NAMESPACE}/${m}:dev-latest .
              docker push ${REGISTRY}/${NAMESPACE}/${m}:dev-${BUILD_ID}-${env.GIT_COMMIT.take(7)}
              docker push ${REGISTRY}/${NAMESPACE}/${m}:dev-latest
            """
          }
        }
      }
    }

    stage('Deploy Dev') {
      when { branch 'dev' }
      steps {
        sshagent(['dev-server']) {
          sh '''
            ssh deploy@dev-server "cd /opt/livestock/repo && \
              git pull && \
              docker compose -f _devops/docker/compose/docker-compose.yml \
                -f _devops/docker/compose/docker-compose.dev.yml \
                --env-file /opt/livestock/.env.dev \
                pull && \
              docker compose ... up -d --remove-orphans"
          '''
        }
      }
    }

    stage('Migrations') {
      when { branch 'dev' }
      steps {
        sshagent(['dev-server']) {
          sh 'ssh deploy@dev-server "docker run --rm --env-file /opt/livestock/.env.dev livestock/migration-job:dev-latest"'
        }
      }
    }

    stage('Smoke') {
      steps {
        sh 'curl -fsS https://dev-api.livestock.com/health/ready'
        sh 'curl -fsS https://dev-api.livestock.com/identity/health/ready'
      }
    }
  }

  post {
    success { slackSend channel: '#deploys', color: 'good', message: "✅ dev deploy ${env.BUILD_ID} ok" }
    failure { slackSend channel: '#deploys', color: 'danger', message: "❌ dev deploy ${env.BUILD_ID} failed" }
  }
}
```

**Prod pipeline farkları:**

- Manual approval gate before deploy
- Image tags: `prod-${BUILD_ID}-${COMMIT}` + `latest` only after success
- Blue-green strategy (Faz 1): yeni container set başlat, nginx upstream swap, eski container'ları kapat
- Migration parametresi explicit (`RUN_MIGRATIONS=true` boolean param)
- Rollback runbook link in failure notification

**Per-wave switchover (Karar 4'ten):**

Her wave'in deploy'u feature flag arkasında. Cutover günü `IFeatureFlagService` üzerinden eski sistem disable + yeni sistem enable. Rollback: feature flag toggle (immediate). Database tarafında: Wave 0'da Phase 0 init zaten yapıldı, sadece schema-by-schema migration apply.

---

### 2. Backup & DR

**PostgreSQL backup stratejisi:**

| Tip | Frekans | Retention | Yöntem |
|---|---|---|---|
| **WAL streaming** | Continuous | 7 days | pgBackRest async archive_command to S3 |
| **Base backup (full)** | Daily 02:00 UTC | 30 days | pgBackRest `--type=full` |
| **Differential** | Hourly | 7 days | pgBackRest `--type=diff` |
| **Logical export** | Weekly | 1 year | pg_dump per-schema, encrypted, S3 Glacier |
| **Pre-migration snapshot** | On-demand (CI) | 30 days | pgBackRest full + tagged |

**RPO (Recovery Point Objective):** ≤ 5 minutes (WAL streaming)
**RTO (Recovery Time Objective):** ≤ 1 hour (base + WAL replay to S3-region)

**pgBackRest config (`/etc/pgbackrest/pgbackrest.conf`):**

```ini
[global]
repo1-type=s3
repo1-s3-endpoint=s3.eu-central-1.amazonaws.com
repo1-s3-bucket=livestock-pgbackup
repo1-s3-region=eu-central-1
repo1-retention-full=30
repo1-retention-diff=7
repo1-cipher-type=aes-256-cbc
repo1-cipher-pass=$BACKUP_ENCRYPTION_KEY
process-max=4
compress-type=zst
compress-level=3

[livestock]
pg1-path=/var/lib/postgresql/17/main
pg1-port=5432
```

**Restore drill (monthly):**

1. Pull latest base + WAL to staging server
2. `pgbackrest --stanza=livestock restore --type=time --target="2026-05-10 14:00:00"`
3. Verify schemas, row counts vs production snapshot
4. Run smoke queries (top-10 expensive queries)
5. Document RTO actual vs target
6. Report in #ops-monthly

**Other backups:**

| Component | Frequency | Method |
|---|---|---|
| Redis | Daily AOF snapshot to S3 (data is cache — eventually consistent OK) | `BGREWRITEAOF` cron |
| RabbitMQ definitions (queues, exchanges, bindings) | On change (CI) | `rabbitmqadmin export` to Git |
| MinIO (file storage) | Continuous replication to secondary bucket | `mc mirror --watch` |
| Configuration / secrets | On change (audit log) | Vault snapshots → S3 |

**DR plan:**

- **Region failure:** Multi-AZ in single region (Faz 1) → restore in alternate region from S3 backup (RTO 4h)
- **Data corruption:** Point-in-time recovery via WAL replay
- **Accidental delete:** Logical export → restore specific schema
- **Provider lock-in mitigation:** All backups in vendor-neutral format (pgBackRest, plain SQL dumps)

Multi-region active-active **Faz 2** (logical replication + PgBouncer geo-routing — see Grup C).

---

### 3. Secret Management

**Tool:** HashiCorp Vault (Wave 0) — self-hosted, OSS edition.

**Secret hierarchy:**

```
secret/
├── livestock/
│   ├── prod/
│   │   ├── postgres/
│   │   │   ├── livestock_migrator (password)
│   │   │   └── livestock_app
│   │   ├── redis/
│   │   ├── rabbitmq/
│   │   ├── jwt/
│   │   │   ├── signing-key-rsa-private
│   │   │   └── signing-key-rsa-public
│   │   ├── oauth/
│   │   │   ├── google/{client_id, client_secret}
│   │   │   └── apple/{client_id, key_id, team_id, private_key}
│   │   ├── providers/
│   │   │   ├── stripe/{secret_key, webhook_secret}
│   │   │   ├── brevo/{api_key, smtp_password}
│   │   │   ├── netgsm/{username, password}
│   │   │   ├── twilio/{account_sid, auth_token}
│   │   │   └── firebase/{server_key}
│   │   └── encryption/
│   │       ├── totp-secret-key
│   │       ├── national-id-key
│   │       └── iban-key
│   └── dev/ (same structure, separate values)
```

**Vault Agent sidecar pattern:**

```yaml
# docker-compose snippet
services:
  identity-api:
    image: livestock/identity:latest
    environment:
      VAULT_ADDR: http://vault:8200
      VAULT_ROLE: identity-api
    depends_on:
      - vault-agent
    volumes:
      - vault-secrets:/secrets:ro
  vault-agent:
    image: hashicorp/vault:1.15
    command: agent -config=/etc/vault/agent.hcl
    volumes:
      - ./vault/agent.hcl:/etc/vault/agent.hcl:ro
      - vault-secrets:/secrets
```

**Approle auth (per-service):**

```bash
vault write auth/approle/role/identity-api \
  token_policies="identity-read" \
  secret_id_ttl=24h \
  token_ttl=1h \
  token_max_ttl=4h \
  bind_secret_id=true \
  secret_id_bound_cidrs="10.0.0.0/8"
```

**Rotation policy:**

| Secret | Frequency | Method |
|---|---|---|
| JWT signing key | 90 days (overlap 30d via `kid`) | Vault transit + manual flag flip |
| DB passwords | 30 days | Vault DB secrets engine (dynamic creds Faz 2; manual + script Faz 1) |
| OAuth credentials | When upstream rotates | Manual + audit log |
| Provider API keys | 90 days | Manual + audit log |
| Encryption keys | Never (data re-encryption costly) | Master + versioned envelope keys |

**Application reading secret:**

```csharp
// appsettings.Production.json
{
  "ConnectionStrings": {
    "Identity": "Host=...;Username=livestock_app;Password=${PG_PASSWORD};..."
  }
}

// Vault Agent renders /secrets/postgres.json → env var substitution
// Or in-app: IConfiguration + VaultSharp + IOptionsSnapshot for hot reload
```

**.env file rules (dev only):**

- Never committed (`.gitignore`)
- Template at `_devops/docker/env/.env.example`
- Dev secrets in `1Password` shared vault (team)
- Onboarding script (`setup-dev.sh`) pulls dev secrets via `op` CLI

---

### 4. External Integrations

| Provider | Purpose | Module | Failover |
|---|---|---|---|
| **TCMB (Türkiye Cumhuriyet Merkez Bankası)** | TRY exchange rates (primary) | Catalog | → ECB → exchangerate.host |
| **ECB** | EUR-base rates (secondary) | Catalog | → exchangerate.host |
| **exchangerate.host** | Fallback all currencies | Catalog | Cache last-known (max 7d stale) |
| **Google OAuth** | Social login | Identity | None (graceful degrade — email/phone login still works) |
| **Apple Sign-In** | Social login | Identity | None |
| **Stripe** | Payment processing (Faz 1.5) | Subscription | Stripe Status API monitor + scheduled retry |
| **Brevo (Sendinblue)** | Transactional email | Notifications | → Mailgun (Faz 2 secondary) |
| **NetGSM** | SMS (TR primary) | Notifications | → Twilio (intl + fallback) |
| **Twilio** | SMS (intl) | Notifications | → AWS SNS (Faz 2) |
| **Firebase Cloud Messaging** | Push notifications (Android + iOS) | Notifications | APNS direct (Faz 2 redundancy) |
| **MinIO / S3** | File storage | Catalog/Accounts/Listings (image uploads) | Cross-region replication |
| **Sentry** | Error tracking | All | None (best-effort — logs still in Loki) |

**Integration contract pattern:**

- Every external call wrapped in `Polly` retry (3 attempts, exponential backoff) + circuit breaker (5 fails / 30s open / 60s half-open)
- Webhook signatures verified (Stripe, Apple notifications, NetGSM delivery reports)
- Outbound IP allowlist documented per provider (some require static IP — Stripe webhook delivery)
- Vendor status pages monitored via Slack webhook + auto-incident creation when downstream affects us > 5m

**Webhook receiver pattern (Stripe example):**

```csharp
[HttpPost("/webhooks/stripe")]
public async Task<IActionResult> Stripe([FromHeader(Name = "Stripe-Signature")] string sig)
{
    var payload = await new StreamReader(Request.Body).ReadToEndAsync();
    var evt = EventUtility.ConstructEvent(payload, sig, _settings.WebhookSecret);

    // Idempotency: store evt.Id in `subscription.webhook_inbox`
    if (await _inbox.ExistsAsync(evt.Id)) return Ok();

    await _publisher.Publish(new StripeWebhookReceivedEvent { ... });
    await _inbox.RecordAsync(evt.Id);
    return Ok();
}
```

---

### 5. Security Hardening

**Network:**

- Cloudflare in front of nginx (DDoS, WAF, bot detection)
- nginx terminates TLS (Let's Encrypt via cert-manager; auto-renew)
- All internal traffic via Docker overlay network, no published ports except 80/443
- API Gateway is the only ingress; backend modules unreachable from public

**TLS / HTTPS:**

- TLS 1.2 minimum, TLS 1.3 preferred
- HSTS header (`max-age=31536000; includeSubDomains; preload`)
- OCSP stapling enabled
- Cipher suite hardened (Mozilla "Intermediate")

**Application:**

| Concern | Mitigation |
|---|---|
| **OWASP A01 Broken Access Control** | Verificator pattern enforces auth per endpoint; admin routes gated by `NoImpersonationOnAdminRoutes` middleware |
| **A02 Cryptographic Failures** | Argon2id password hashing; envelope encryption for NationalId/IBAN/TOTP secret; AES-256-GCM at rest |
| **A03 Injection** | EF Core parameterized queries only; no raw SQL except `FromSqlRaw` audited cases; FluentValidation for input |
| **A04 Insecure Design** | Threat model documented per module; PR template includes "Security considerations" |
| **A05 Misconfiguration** | Hardened base images (distroless or chainguard); no `latest` tag in prod; minimal env exposure |
| **A06 Vulnerable Components** | Dependabot + `dotnet list package --vulnerable` in CI |
| **A07 Auth Failures** | OTP rate limit (3/min per phone), login rate limit (10/min per IP+email), refresh token rotation |
| **A08 Software & Data Integrity** | Container images signed (cosign); SBOM generated (Syft); commit signing required for `main` |
| **A09 Logging/Monitoring Failures** | Audit log for admin actions (append-only Faz 2), Sentry/Loki retention 90d/7y |
| **A10 SSRF** | HttpClient explicit allowlist (no user-supplied URL fetches); image upload via signed S3 URLs only |

**Rate limiting (ASP.NET Core RateLimiter):**

```csharp
builder.Services.AddRateLimiter(o =>
{
    o.AddPolicy("login", ctx => RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: ctx.Connection.RemoteIpAddress!.ToString(),
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        }));

    o.AddPolicy("otp", ctx => RateLimitPartition.GetTokenBucketLimiter(
        partitionKey: ctx.Request.RouteValues["phone"]?.ToString() ?? "anon",
        factory: _ => new TokenBucketRateLimiterOptions
        {
            TokenLimit = 3, TokensPerPeriod = 1, ReplenishmentPeriod = TimeSpan.FromMinutes(1)
        }));

    o.AddPolicy("api-default", ctx => RateLimitPartition.GetSlidingWindowLimiter(
        partitionKey: ctx.User.Identity?.Name ?? ctx.Connection.RemoteIpAddress!.ToString(),
        factory: _ => new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 600, Window = TimeSpan.FromMinutes(1), SegmentsPerWindow = 6
        }));

    o.OnRejected = async (ctx, ct) =>
    {
        ctx.HttpContext.Response.StatusCode = 429;
        await ctx.HttpContext.Response.WriteAsJsonAsync(new
        {
            type = "https://livestock.com/errors/rate-limited",
            title = "Too Many Requests",
            code = "COMMON_RATE_LIMITED"
        }, ct);
    };
});
```

**CORS:**

Whitelisted origins only — `https://app.livestock.com`, `https://admin.livestock.com`, mobile schemes. No `*`.

**Pen-test cadence:** Annual external audit (CREST-accredited firm) + quarterly internal red-team exercise (Wave 7+).

---

## Grup C — Process + Performance + Faz 2 + DX

### 1. SLO + Load Test

**SLO targets:** see [Grup A / 1.7](#17-slislo-targets).

**Load test tooling:** **k6** (TypeScript) — checked into repo at `_devops/loadtest/`.

**Test scenarios:**

| Scenario | Load Pattern | Target | Pass Criteria |
|---|---|---|---|
| **Cold start** | 0 → 50 RPS over 1m, hold 10m | Search + Listing detail | p95 < 500ms, error < 1% |
| **Steady state** | 200 RPS constant 30m | Mixed read traffic (70% search, 20% detail, 10% messaging) | p95 < 500ms, error < 0.5% |
| **Marketplace spike** | 500 RPS for 5m | Offer + Deal flow | p95 < 1s, error < 1% |
| **Auth burst** | 200 logins/s for 2m | Identity | p95 < 800ms, error < 2% |
| **SignalR fan-out** | 5k connected users, broadcast 1 msg → 1k recipients | Messaging | Delivery p95 < 2s |
| **DB connection storm** | Ramp 0 → 500 RPS in 30s | All modules | No connection timeouts |

**k6 example (`offer-flow.js`):**

```javascript
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Trend } from 'k6/metrics';

const offerLatency = new Trend('offer_latency');

export const options = {
  scenarios: {
    spike: {
      executor: 'ramping-arrival-rate',
      startRate: 0,
      timeUnit: '1s',
      preAllocatedVUs: 100,
      stages: [
        { target: 500, duration: '1m' },
        { target: 500, duration: '5m' },
        { target: 0, duration: '30s' },
      ],
    },
  },
  thresholds: {
    'http_req_duration{name:offer_create}': ['p(95)<1000'],
    'http_req_failed': ['rate<0.01'],
  },
};

const TOKEN = __ENV.TOKEN;

export default function () {
  const res = http.post(
    'https://staging-api.livestock.com/marketplace/offers',
    JSON.stringify({ listingId: '...', amount: 1000, currency: 'TRY' }),
    { headers: { Authorization: `Bearer ${TOKEN}`, 'Content-Type': 'application/json' },
      tags: { name: 'offer_create' } },
  );
  check(res, { 'status 201': r => r.status === 201 });
  offerLatency.add(res.timings.duration);
  sleep(Math.random() * 2);
}
```

**Cadence:**

- **Per-wave acceptance test:** mandatory at end of each wave (Wave 1+)
- **Pre-release regression:** every prod deploy on `main`
- **Quarterly capacity planning:** 2× peak traffic test
- **Chaos engineering Faz 2:** ChaosMesh — kill pod, network partition

**Capacity targets (Faz 1 / per single host before K3s):**

| Module | Target RPS | DB Pool | Worker Threads |
|---|---|---|---|
| Identity | 200 | 30 | 200 |
| Listings (search-heavy) | 500 | 50 | 200 |
| Marketplace | 100 | 30 | 100 |
| Messaging (SignalR) | 5k concurrent | 20 | 200 |
| Notifications | 1000 (mostly worker) | 20 | 100 |
| Other modules | 100 each | 20 | 100 |

---

### 2. PR Review / Runbook / Postmortem

#### 2.1. PR Review

**PR template (`.github/pull_request_template.md`):**

```markdown
## Summary
<what + why>

## Module(s) affected
- [ ] Catalog
- [ ] Identity
- ...

## Type
- [ ] Feature / [ ] Bugfix / [ ] Refactor / [ ] Migration / [ ] Docs / [ ] Test

## Domain checklist
- [ ] No cross-module FK (Karar 3d)
- [ ] New events documented in module's "Public Events" section
- [ ] DI registrations updated (ApplicationDependencyProvider)
- [ ] Migration script + rollback plan (if schema change)
- [ ] Error codes added to DomainErrors with unique prefix
- [ ] OpenAPI spec regenerated (per-module + combined)
- [ ] Frontend client regenerated (@hey-api/openapi-ts) — Linear ticket if needed

## Test plan
- [ ] Unit tests
- [ ] Integration tests
- [ ] Manual smoke (which endpoint)

## Security considerations
<auth changes / new external call / PII handling>

## Observability
- [ ] New metrics added (if applicable)
- [ ] Log statements use structured fields
- [ ] Sentry breadcrumbs sufficient

## Deployment
- [ ] Feature flag wired (if risky)
- [ ] Rollback path documented
- [ ] Migration runs idempotently
```

**Review rules:**

- **2 approvals** for `main`-bound PRs (1 owner of affected module + 1 from platform)
- **1 approval** for `dev`-bound feature work
- **Hotfix to main:** 1 approval + post-merge review note
- Reviewer SLA: 4 business hours response, 1 business day completion
- Code owners file (`.github/CODEOWNERS`) auto-assigns per `05-modules/*` path

**Quality gates:**

- All CI green (build + unit + integration + lint + security)
- Coverage delta non-negative (Faz 1.5: enforce > 70% on changed lines)
- No new high/critical vulnerabilities introduced
- No `TODO without ticket` (linter rule)

#### 2.2. Runbook

Stored under `_devops/runbooks/`. Each operational alert in Prometheus rules has matching runbook URL.

**Runbook template:**

```markdown
# Runbook: <alert_name>

## Summary
<one-liner>

## Severity
critical / warning

## Symptoms
- Metric: ...
- User impact: ...

## Investigation
1. Check Grafana dashboard X panel Y
2. Loki query: `{module="..."} |= "error"`
3. Tempo trace search: ...

## Mitigation (in order)
1. <fast safe action>
2. <next action>
3. <last resort, requires approval>

## Root cause investigation
- See related dashboards
- Check recent deploys (link to deploy history)
- ...

## Postmortem trigger
If user-impacting > 15 minutes → create postmortem doc
```

**Required runbooks (Wave 0 — must exist before prod cutover):**

1. `module-down.md`
2. `db-pool-saturation.md`
3. `db-connection-timeout.md`
4. `outbox-stuck.md`
5. `rabbit-queue-backlog.md`
6. `consumer-lag.md`
7. `payment-provider-down.md`
8. `notification-provider-failure.md`
9. `exchange-rate-stale.md`
10. `auth-brute-force-suspected.md`
11. `disk-full.md`
12. `cert-expiry.md`
13. `deploy-rollback.md`
14. `db-restore-from-backup.md`
15. `feature-flag-emergency-disable.md`

#### 2.3. Postmortem

**Trigger criteria:**

- User-facing incident > 15 minutes
- Data integrity compromised
- Security breach (any)
- SLO budget burn > 25% in single event

**Template (`_devops/postmortems/YYYY-MM-DD-incident.md`):**

```markdown
# Postmortem: <date> — <short title>

**Status:** draft / review / finalized
**Author:** <name>
**Reviewers:** <names>
**Incident commander:** <name>
**Severity:** sev1 / sev2 / sev3

## Summary
<2-3 sentence executive summary>

## Impact
- Duration: <start UTC> → <end UTC> (X minutes)
- Affected users: ~N
- Affected functionality: <list>
- Revenue impact (if measurable): ...

## Timeline (all UTC)
- HH:MM — first signal
- HH:MM — page fired
- HH:MM — engineer ack
- HH:MM — root cause identified
- HH:MM — mitigation applied
- HH:MM — resolved

## Root cause
<technical detail; 5-whys>

## Detection
How did we find out? What signal fired? Was it fast enough?

## Response
What did we do? What worked, what didn't?

## Lessons learned
### What went well
### What didn't go well
### Where we got lucky

## Action items
| # | Action | Owner | Due | Linear |
|---|---|---|---|---|
| 1 | ... | ... | ... | LIVE-... |
```

**Process:** Blameless culture. Postmortem published in internal wiki within 5 business days. Action items tracked in Linear, status reviewed in weekly ops meeting.

---

### 3. Faz 2 Readiness (K3s, PgBouncer, Meilisearch)

#### 3.1. K3s (Kubernetes)

**Trigger:** When Docker Compose host saturates (> 70% CPU sustained) or modules need horizontal scaling beyond 2 replicas with shared session/cache concerns.

**Estimated trigger:** Month 12-18 (after Wave 7+ all modules live + traffic > 500 RPS sustained).

**Preparation work in Faz 1 (cheap to do early):**

- All services 12-factor — no local filesystem state (already done via S3/Redis)
- Health probes implemented (`/health/live`, `/health/ready`, `/health/startup`) — already done in Grup A.5
- Graceful shutdown on SIGTERM (already supported by ASP.NET Core)
- Stateless sessions (JWT — already done)
- Externalize config (Vault — already done)
- Resource limits documented per module (CPU/RAM baseline + p99 → translate to k8s `requests`/`limits`)
- Logs to stdout (already done — Promtail picks up)
- Helm chart skeleton committed (`_devops/helm/`)

**K3s topology (Faz 2 target):**

```
ingress-nginx → identity-deployment (2 pods, HPA 2-10)
              → marketplace-deployment (2 pods, HPA 2-10)
              → ... (per module)
              → messaging-deployment (3 pods sticky-session for SignalR)
postgres-statefulset (Patroni HA — Faz 2.5)
redis-statefulset (sentinel)
rabbitmq-statefulset (cluster x3)
```

**HPA (Horizontal Pod Autoscaler) rule example:**

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata: { name: listings-api }
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: listings-api
  minReplicas: 2
  maxReplicas: 10
  metrics:
    - type: Resource
      resource: { name: cpu, target: { type: Utilization, averageUtilization: 70 } }
    - type: Pods
      pods:
        metric: { name: http_server_active_requests }
        target: { type: AverageValue, averageValue: "100" }
```

#### 3.2. PgBouncer

**Trigger:** DB connection pool consistently > 70% utilized across modules (per-module pool of 30 × 10 modules = 300 connections starts pressuring PG's default 100).

**Estimated trigger:** Month 6-9 (after Wave 5).

**Topology:**

```
identity-api ─┐
accounts-api ─┤
... ─────────┼──→ PgBouncer (transaction pooling) ──→ PostgreSQL
admin-api ───┘     (1000 client conn, 30 PG conn per pool)
```

**Config (`pgbouncer.ini`):**

```ini
[databases]
livestock_identity = host=postgres dbname=livestock pool_mode=transaction
livestock_accounts = host=postgres dbname=livestock pool_mode=transaction
# ... one per schema

[pgbouncer]
listen_port = 6432
auth_type = scram-sha-256
auth_file = /etc/pgbouncer/userlist.txt
max_client_conn = 1000
default_pool_size = 30
reserve_pool_size = 5
reserve_pool_timeout = 3
server_idle_timeout = 60
query_wait_timeout = 30
```

**Caveat:** Transaction pooling = no session-level features (LISTEN/NOTIFY, prepared statements default, advisory locks tied to session). Audit current usage before flip. EF Core needs `Pooling=false` in connection string (PgBouncer handles pooling instead) + careful `Persist Security Info=false`.

#### 3.3. Meilisearch

**Trigger:** Listings search latency p95 > 500ms OR catalog search needs typo tolerance / multi-language fuzzy match beyond PG `pg_trgm`.

**Estimated trigger:** Month 4-6 (Wave 4 — Listings module live + traffic).

**Why Meilisearch over alternatives:**

- TypeScript-friendly REST API (no JVM ops burden of Elasticsearch)
- Built-in typo tolerance, ranking rules, faceted search
- Self-hostable, single binary
- Good i18n support (Listings titles in 50 languages)

**Architecture:**

```
Listings module ──publish─→ ListingCreatedEvent ──→ Notifications/Search Sync Worker
                                                          ↓
                                                  Meilisearch index "listings"
                                                          ↑ search
Search API ←─────────────────────────────────────────────┘
```

**Index schema (`listings`):**

```json
{
  "primaryKey": "id",
  "searchableAttributes": [
    "title", "title_translations.en", "title_translations.tr",
    "description", "category_name", "brand_name", "location_city"
  ],
  "filterableAttributes": [
    "category_id", "brand_id", "country_code", "city",
    "price_amount", "price_currency", "status", "seller_id"
  ],
  "sortableAttributes": ["created_at", "price_amount", "views"]
}
```

**Index lifecycle:**

- Backfill: one-time batch job reads all listings → bulk insert
- Live updates: dedicated worker consumes `ListingCreatedEvent` / `ListingUpdatedEvent` / `ListingDeletedEvent` → idempotent upsert
- Reindex on schema change: blue-green index swap

**Fallback:** If Meilisearch unavailable, fall back to PG `pg_trgm` query (slower but functional). Search worker monitors Meilisearch `/health`; on failure, set feature flag `search.use_meilisearch=false`.

---

### 4. Developer Experience (DX)

#### 4.1. `setup-dev.sh`

Single command to bootstrap a fresh dev environment.

```bash
#!/usr/bin/env bash
set -euo pipefail

echo "=== Livestock dev setup ==="

# 1. Tooling check
for cmd in dotnet docker docker-compose git op psql; do
  command -v "$cmd" >/dev/null || { echo "Missing: $cmd"; exit 1; }
done

dotnet_version=$(dotnet --version)
[[ "$dotnet_version" == 10.* ]] || { echo "Need .NET 10, have $dotnet_version"; exit 1; }

# 2. Pull dev secrets from 1Password
echo "Pulling dev secrets..."
op signin
op item get "livestock-dev-env" --format=json | jq -r '.fields[] | "\(.label)=\(.value)"' > _devops/docker/compose/.env.dev

# 3. Start infra (PG + Redis + RabbitMQ + MinIO + Vault dev mode)
cd _devops/docker/compose
docker compose -f docker-compose.yml -f docker-compose.dev.yml --env-file .env.dev up -d \
  postgres redis rabbitmq minio vault prometheus grafana loki

# 4. Wait for PG ready
until docker exec livestock_dev-postgres-1 pg_isready -U livestock_migrator; do sleep 1; done

# 5. Apply migrations
cd ../..
dotnet run --project Jobs/RelationalDB/MigrationJob -- development

# 6. Seed reference data
dotnet run --project Jobs/RelationalDB/MigrationJob -- development --seed-all

# 7. Local app config
cp appsettings.local.json.example appsettings.local.json

# 8. Open OpenAPI / Scalar UI
echo "==="
echo "✅ Done."
echo "  Gateway:    http://localhost:5000"
echo "  Identity:   http://localhost:5001 (Scalar: /scalar/v1)"
echo "  Grafana:    http://localhost:3000 (admin/admin)"
echo "  RabbitMQ:   http://localhost:15672"
echo "  Vault:      http://localhost:8200"
echo "==="
```

#### 4.2. CLI helpers (`_devops/scripts/`)

| Script | Purpose |
|---|---|
| `setup-dev.sh` | First-time bootstrap (above) |
| `reset-db.sh` | Drop + recreate + migrate + seed |
| `new-endpoint.ps1 -Module X -Feature Y -Op Create` | Scaffold 6 handler files |
| `regen-openapi.sh` | Regenerate per-module + combined OpenAPI |
| `regen-frontend-client.sh` | Run @hey-api/openapi-ts against latest spec |
| `tail-logs.sh` | `docker compose logs -f` filtered by module |
| `port-forward-prod.sh` | SSH tunnel to prod read replica (read-only, audited) |

#### 4.3. IDE setup

**Recommended:** Rider (preferred — better EF + Razor support) or VS Code with C# Dev Kit extension.

**Shared `.editorconfig`** committed — enforces:

- 4-space indent (.cs), 2-space (.ts/.json/.yaml)
- Final newline
- Trim trailing whitespace
- `dotnet format` compliance (no manual style tweaking)

**Recommended VS Code extensions** (committed `.vscode/extensions.json`):

```json
{
  "recommendations": [
    "ms-dotnettools.csdevkit",
    "ms-azuretools.vscode-docker",
    "humao.rest-client",
    "redhat.vscode-yaml",
    "tamasfe.even-better-toml"
  ]
}
```

#### 4.4. Documentation

**Mkdocs site (`_docs/`):**

- Auto-built from `c:\workspace\livestock-trading-planning\` repo → deployed to internal Confluence-replacement
- Search index updated on commit to `main`
- Architecture decision records (ADR) format: `_docs/adr/NNNN-title.md`

**`CLAUDE.md` in project root:** Living guidance for AI-assisted development (current file already exists for legacy stack — will be rewritten in Wave 1 for new stack).

#### 4.5. Onboarding checklist (new engineer)

```markdown
- [ ] Read 01-architecture.md + 02-modules-list.md (1 hr)
- [ ] Read 03-domain-patterns.md (focused on layer they'll work) (1 hr)
- [ ] Run setup-dev.sh (30 min)
- [ ] Hit /scalar/v1 and explore one module's endpoints (30 min)
- [ ] Pair with module owner on a "starter" Linear ticket (1 day)
- [ ] Open first PR with reviewer assigned (week 1)
- [ ] Shadow on-call for one cycle (week 2)
- [ ] Read postmortem index — last 3 incidents (week 2)
```

**Target:** new engineer pushes first non-trivial change to dev within 5 working days.

---

## Özet — Wave Tied Deliverables

| Wave | Operations Deliverable |
|---|---|
| **0** | init.sql + Prometheus/Grafana baseline + Vault + Sentry + nginx + cert-manager + setup-dev.sh + 15 runbooks + Jenkins dev pipeline |
| **1** (Catalog) | Custom metrics added; load test scenario 1; postmortem template signed off |
| **2** (Identity) | Auth alerts; OWASP review; pen-test simulation |
| **3** (Accounts) | Backup restore drill #1 (PIT recovery validated) |
| **4** (Listings) | Meilisearch evaluation; load test scenario 4 |
| **5** (Carrier/Sub) | Stripe sandbox + Webhook signing; dunning runbook |
| **6** (Marketplace/Msg/Notif) | SignalR fan-out load test; dispute alerting |
| **7** (Admin) | Audit log read-only enforcement; impersonation log review |
| **Prod cutover** | DR drill (full restore from S3 in alt region); chaos test; first prod postmortem template |
| **Faz 2 prep** | K3s POC environment; PgBouncer evaluation; Meilisearch live |
