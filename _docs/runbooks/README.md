# Operational Runbooks

Operational playbooks for incident response, deployment, and maintenance.

Wave 0 status: index only. Each runbook is authored in the Wave where it becomes operationally relevant (production exists from Wave 7+; most incident runbooks need live services to be meaningful).

## Runbook Index

### Incident Response

| # | Runbook | Wave | Description |
|---|---|---|---|
| 1 | service-down.md | 7 | API / DB / Redis / RabbitMQ unreachable - triage + restart |
| 2 | high-error-rate.md | 7 | 5xx spike - log investigation + rollback decision |
| 3 | db-connection-exhaustion.md | 7 | PostgreSQL connection pool exhausted - diagnosis + mitigation |
| 4 | disk-space-alert.md | 7 | Disk usage threshold - cleanup + volume expansion |
| 5 | memory-oom.md | 7 | Out-of-memory / container kill - heap analysis + limits |

### Deployment

| # | Runbook | Wave | Description |
|---|---|---|---|
| 6 | standard-deployment.md | 7 | Routine deploy via Jenkins - pre-checks + smoke test |
| 7 | rollback.md | 7 | Revert to previous release - DB + app coordination |
| 8 | db-migration-apply.md | 1 | EF Core migration apply - order, idempotency, verification |
| 9 | hotfix-deployment.md | 7 | Emergency patch - minimal-change deploy path |

### Operational

| # | Runbook | Wave | Description |
|---|---|---|---|
| 10 | backup-restore.md | 7 | pg_dump + WAL - backup schedule + restore drill |
| 11 | secret-rotation.md | 7 | Vault / .env secret rotation - zero-downtime swap |
| 12 | scaling.md | 7 | Horizontal / vertical scale - when + how |
| 13 | certificate-renewal.md | 7 | TLS cert renewal - nginx + Let's Encrypt |
| 14 | log-investigation.md | 5 | Loki query patterns - correlation IDs + trace linkage |
| 15 | performance-debugging.md | 5 | Slow endpoint triage - profiling + query analysis |

## Wave Mapping Summary

- **Wave 1:** #8 (db-migration-apply) - first migrations land with Catalog module
- **Wave 5:** #14, #15 (log-investigation, performance-debugging) - observability stack arrives
- **Wave 7:** #1-7, #9-13 - production deployment + incident response become relevant

## Runbook Template

Each runbook, when authored, follows this structure:

- **Trigger:** What alert / symptom starts this runbook
- **Severity:** P1 / P2 / P3
- **Diagnosis:** Step-by-step investigation commands
- **Mitigation:** Immediate actions to restore service
- **Root cause:** Post-incident analysis pointers
- **Prevention:** Follow-up tasks to avoid recurrence

## Notes

- Runbooks are living documents - updated after each incident.
- Wave 0 has no production environment; this index defines structure ahead of need.
- CI pipeline: see `_devops/jenkins/Jenkinsfile.ci`.
