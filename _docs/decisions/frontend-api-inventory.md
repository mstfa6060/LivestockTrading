# Frontend API Inventory — Skeleton

**Status:** PLACEHOLDER — frontend rebuild başlangıcında bu doc'un güncellenmesi gerekecek
**Üst:** [README.md](README.md), [06-api-contract.md](06-api-contract.md)

---

## Amaç

Bu doc backend ↔ frontend API kullanım envanteridir. Frontend rebuild (Next.js) başlangıcında her ekran/feature'un hangi backend endpoint'lerini kullandığı buraya eklenecek. Wave 0 öncesinde **placeholder** — sadece kullanım pattern'i + entegrasyon dokümantasyonu için referans bağlantıları.

---

## Frontend Stack (Karar 6 Madde 1, 2, 8)

| Katman | Teknoloji | Karar |
|---|---|---|
| Framework | Next.js 15 (App Router) | Karar 6 / Madde 1 |
| API contract | OpenAPI 3.1 | Karar 6 / Madde 2 |
| TS client | @hey-api/openapi-ts | Karar 6 / Madde 8 |
| Data fetching | TanStack Query v5 | Karar 6 / Madde 8 |
| Mock layer | MSW (Mock Service Worker) | Karar 6 / Madde 1 (Hybrid) |
| Realtime | SignalR JS client | Karar 6 / Madde 6 |
| i18n | next-intl | Karar 6 / Madde 4 |
| Form | react-hook-form + zod | Madde 9 (form error mapping) |
| State | Zustand (UI state) + TanStack Query (server state) | — |

---

## Module → Endpoint → Screen Matrix (TBD)

Wave 0 sonu — Wave 1 başı dolacak. Her satır:

```
{Modül} > {Endpoint Path} > {Screen Name} > {Query Key Pattern}
```

### Identity (TBD)

| Endpoint | Screen | Query Key |
|---|---|---|
| `POST /identity/auth/login` | `/login` | — (mutation) |
| `POST /identity/auth/refresh` | (global interceptor) | — |
| `GET /identity/users/me` | `/me`, header avatar | `['identity','me']` |
| `PATCH /identity/users/me/preferences` | `/settings/preferences` | — (mutation) |
| ... | ... | ... |

### Accounts (TBD)

| Endpoint | Screen | Query Key |
|---|---|---|
| `GET /accounts/sellers/{slug}` | `/seller/{slug}` | `['accounts','seller',slug]` |
| `GET /accounts/sellers/{id}/listings` | seller page tab | `['accounts','seller',id,'listings',cursor]` |
| ... | ... | ... |

### Listings (TBD)

| Endpoint | Screen | Query Key |
|---|---|---|
| `GET /listings/search` | `/listings`, home feed | `['listings','search',filterHash,cursor]` |
| `GET /listings/{id}` | `/listings/{id}` | `['listings','detail',id]` |
| `POST /listings` | `/seller/listings/new` | — (mutation) |
| ... | ... | ... |

### Marketplace (TBD)

| Endpoint | Screen | Query Key |
|---|---|---|
| `POST /marketplace/offers` | listing detail offer modal | — (mutation) |
| `GET /marketplace/deals` | `/me/deals` | `['marketplace','deals',cursor]` |
| ... | ... | ... |

### Carrier / Subscription / Messaging / Notifications / Admin / Catalog (TBD)

Frontend rebuild başlangıcında doldurulacak.

---

## Realtime Event → UI Action Matrix (Karar 6 Madde 6 Referansı)

80 event matrisi `06-api-contract.md / Madde 6`'da. Frontend tarafında her event için:

```
{Event} > {Hub} > {Client Method} > {UI Reaction (cache invalidate / toast / banner)}
```

| Event | Hub | Client Method | UI Reaction |
|---|---|---|---|
| `MessageReceived` | `/messaging/hub` | `OnMessage` | `queryClient.invalidateQueries(['messaging','conversation',id])` + toast (eğer convoy outside) |
| `TypingIndicator` | `/messaging/hub` | `OnTyping` | Local state set, 3s debounce clear |
| `MessageRead` | `/messaging/hub` | `OnMessageRead` | Cache update (sentMessages.readAt) |
| `OfferReceived` | `/notifications/hub` | `OnOffer` | `invalidateQueries(['marketplace','offers'])` + toast |
| `DealStatusChanged` | `/notifications/hub` | `OnDealStatus` | `invalidateQueries(['marketplace','deal',id])` + banner |
| `NotificationCreated` | `/notifications/hub` | `OnNotification` | `invalidateQueries(['notifications','inbox'])` + badge count++ |
| `SellerVerified` | `/notifications/hub` | `OnSellerStatus` | `invalidateQueries(['accounts','seller',myId])` + celebration toast |
| ... (80 entry) | ... | ... | ... |

Bu tablo frontend `useChat`/`useNotifications` hook'ları yazılırken doldurulacak.

---

## Auth Flow (referans)

```
1. /login form submit
   → POST /identity/auth/login {method: 'email', email, password}
   → 200 {accessToken, refreshToken, user}
   → localStorage: refreshToken
   → memory: accessToken
   → router.push(from || '/')

2. Authenticated request
   → fetch with Authorization: Bearer ${accessToken}

3. 401 + code=TOKEN_EXPIRED
   → POST /identity/auth/refresh {refreshToken}
   → 200 {accessToken, refreshToken} → retry original request
   → Failure (TOKEN_REVOKED) → router.push('/login')

4. /logout
   → POST /identity/auth/logout {refreshToken}
   → Clear localStorage + memory
   → router.push('/login')
```

`AuthContext` + interceptor wrapper kullanılacak (06-api-contract.md / Madde 8).

---

## Error Handling Pattern (Karar 6 Madde 9 Referansı)

```typescript
// Global error boundary (Madde 9)
function GlobalErrorHandler({ error }: { error: ApiError }) {
  switch (error.code) {
    case 'AUTH_REQUIRED':
    case 'TOKEN_EXPIRED': return redirect('/login');
    case 'ACCOUNT_SUSPENDED': return <SuspendedBanner />;
    case 'RATE_LIMITED': return <RateLimitedToast />;
    case 'MAINTENANCE_MODE': return <MaintenancePage />;
    default: return toast.error(t(error.code, { defaultValue: error.detail }));
  }
}

// Form field error mapping (Madde 9)
useApiFormErrors(setError, mutation.error); // 422 VALIDATION_ERROR → field-level
```

155 error code i18n bundle:
- `web/locales/{tr,en,de,fr,es,ar,ru,...}/errors.json`
- Field code'lar (REQUIRED, TOO_LONG, INVALID_EMAIL, ...) `field-errors.json`

---

## i18n Bundle Layout (Karar 6 Madde 4 Referansı)

```
web/
├── locales/
│   ├── tr/
│   │   ├── common.json
│   │   ├── errors.json          # 155 backend error code
│   │   ├── field-errors.json    # 17 field-level code
│   │   ├── identity.json
│   │   ├── listings.json
│   │   ├── marketplace.json
│   │   └── ...
│   ├── en/ (mirror)
│   └── ... (50 dil total Faz 2; Faz 1 sadece TR + EN)
```

Locale resolution 6-step middleware (06-api-contract.md / Madde 4):

```
1. URL prefix (/tr/...)
2. Cookie locale=tr
3. User.Preferences.Locale (authenticated)
4. Accept-Language header
5. CF-IPCountry → country default locale
6. Fallback en
```

---

## Type Generation Pipeline

```bash
# Backend exposes per-module + combined OpenAPI
GET /openapi/identity.json
GET /openapi/accounts.json
...
GET /openapi/combined.json

# Frontend codegen (arf-cli replacement)
npm run codegen
# → @hey-api/openapi-ts reads combined.json
# → outputs ./generated/api/{module}/index.ts
# → TanStack Query options factories included
```

Cadence: backend endpoint değiştiğinde Linear ticket → frontend codegen run + PR.

---

## Migration Timing (06-api-contract.md / Madde 10 Referansı)

| Faz | Frontend State | Backend State |
|---|---|---|
| **Wave 0** | MSW only (mock all 417 endpoints) | Init.sql + skeleton hosts (no domain) |
| **Wave 1 (Catalog)** | MSW + real `/catalog/*` switch (per-feature flag) | Catalog endpoints live |
| **Wave 2 (Identity)** | + real `/identity/*` | Identity live |
| **Wave 3-7** | Per-wave switchover | Modules live |
| **Cutover** | All real | All modules live |

Frontend feature flag (NEXT_PUBLIC_USE_REAL_CATALOG=true) per modül switch.

---

## TBD Items

Bu doc Wave 1 başlangıcında dolacak. O ana kadar şu plak/check'leri yapıyoruz:

- [ ] @hey-api/openapi-ts pipeline çalışıyor (Wave 0)
- [ ] MSW handler factory katalog endpoint'leri için kurulu (Wave 0)
- [ ] AuthContext + token refresh interceptor (Wave 2 başı)
- [ ] SignalR client wrapper auto-reconnect (Wave 6 başı — Messaging)
- [ ] i18n bundle TR + EN (Wave 0); diğer 48 dil Faz 2
- [ ] Form field error mapping helper (Wave 1)
- [ ] Map component karar (Mapbox vs Leaflet) — Wave 1 Catalog/Location

---

## İlişkili Dokümanlar

- [01-architecture.md](01-architecture.md) — Backend solution yapısı
- [06-api-contract.md](06-api-contract.md) — OpenAPI + error + realtime + locale
- [backlog.md](backlog.md) — Frontend backlog (F bölümü, 28 item)

---

**NOT:** Frontend repo henüz başlamadı (`c:\workspace\GlobalLivestock\web` veya yeni repo TBD). Frontend rebuild kararı verildiğinde bu doc detaylı API → screen mapping ile güncellenecek; o aşamada Faz 1 (Wave 0-7) sırasında değişen API contract'larını da takip ediyor olacak.
