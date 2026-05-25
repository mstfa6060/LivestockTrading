# Wave 4 Mid-Handover — W4.0 Kapanış Canonical SoT

**Stamped:** Sunday, 25 May 2026 (W4.0 Shared.Contracts/Identity foundation kapanış sonrası)
**Commit:** 07f436c (deviations ledger, rebuild/v2 son commit)
**Push tatbikatı:** 39/39 main INVARIANT korundu (44416138)
**Kapsam:** W4.0 tamamlandı, W4.1 Identity.Domain başlamadan önce mid-wave checkpoint

---

## 1. Repo Durumu (fiili origin SHA'lar)

| Branch / Ref | SHA | Durum |
|---|---|---|
| `main` | `44416138b978774146f992f9e0756b829ba541e0` | **INVARIANT** — 39/39 push tatbikatı boyunca dokunulmadı |
| `rebuild/v2` (lokal) | `07f436c3d7a9913b82b39232abafca4242ac0e1d` | deviations ledger commit, senkron |
| `origin/rebuild/v2` | `07f436c3d7a9913b82b39232abafca4242ac0e1d` | Senkron (ahead 0 / behind 0) |
| Tag `wave-3-complete` | origin'de mevcut | `wave-3-complete` annotated tag Wave 3 son commit'te |

`main..HEAD = 62` commit (production'dan rebuild/v2 ucuna gercek delta).

Wave 4 push butcesi: 2/12-18 (push #38 W4.0 + push #39 ledger).

---

## 2. W4.0 Commit Zinciri (3 commit, Wave 4 başlangıcı)

| # | SHA | Subject | Tip |
|---|---|---|---|
| 1 | `0f9b98d` | feat(identity): W4.0 Shared.Contracts/Identity foundation | Backend (14 dosya, 178 ins) |
| 2 | `02f42f7` | claude.md oluşturuldu | Mustafa (repo guidance, 119 satır) |
| 3 | `07f436c` | docs(deviations): W4.0 handover-only F-S1..9 ledger | Backend (36 ins) |

---

## 3. W4.0 Kazanımı — Shared.Contracts/Identity Foundation

**14 dosya, 178 insertions, atomic commit 0f9b98d.**

### Identity/ (13 dosya)

```
Identity/Enums/AccountType.cs          — Producer/Trader/Vet/Buyer (plan-doc §2 literal)
Identity/Enums/ConsentType.cs          — TermsAndPrivacy/MinistryDataShare/MarketingEmail
Identity/Enums/UserStatus.cs           — EmailUnverified/Active/Suspended/PendingDeletion/Deleted
Identity/UserSummary.cs                — 6 property cross-modül record (Cache TTL 5dk)
Identity/UserPreferences.cs            — 6 property (CurrencyCode/CountryCode — W4.R'de plan-doc revize)
Identity/DeviceInfo.cs                 — 4 property (push token dağıtımı)
Identity/ICurrentUserService.cs        — 18 uye JWT claim okuyucu O(1) (plan-doc §5 literal)
Identity/IIdentityReadService.cs       — 10 metot cross-modül kimlik okuma (plan-doc §5 literal)
Identity/Admin/UserDetail.cs           — 14 property admin detay projeksiyonu
Identity/Admin/UserListItem.cs         — 6 property admin liste projeksiyonu (bandwidth minimize)
Identity/Admin/UserListQuery.cs        — 6 property cursor pagination + filtreler
Identity/Admin/IAdminUserCommands.cs   — 6 metot suspend/role/logout/impersonate (plan-doc §5 literal)
Identity/Admin/IAdminUserReadService.cs — 2 metot stub (GetUserByIdAsync + ListUsersAsync)
```

### DataExport/ (1 dosya)

```
DataExport/IDataExportContributor.cs   — GDPR §11 literal (ModuleName + ExportAsync)
```

**Plan-doc §5 literal sadakat:** 4/5 interface birebir. IAdminUserReadService: plan-doc'ta imza yok (stub), Backend tasarim Frontend B-4 onaylı (GetUserByIdAsync Catalog convention).

---

## 4. Wave 4 Frontend Onaylı Kararlar

| Karar | Karar |
|---|---|
| B-1 | UserSummary 6 prop + DeviceInfo 4 prop — Backend inference minimum set |
| B-2 | Domain event contract W4.4/W4.5'te (Catalog emsali tutarlılık) |
| B-3 | RevocationReason enum Identity.Domain internal |
| B-4 | IAdminUserReadService: GetUserByIdAsync + ListUsersAsync (Catalog convention) |
| B-5 | UserStatus Shared.Contracts/Identity/Enums (Catalog BrandStatus emsali) |
| B-6 | UserPreferences Shared.Contracts, ConsentGrant Domain internal |
| Scope | Identity tek wave, 10 sub-batch (W4.0–W4.9) |
| Branching | rebuild/v2 direkt commit (Wave 1–3 emsali) |
| Foundation | W4.0 ayrı atomic batch (Catalog C0 emsali) |
| Password | Microsoft.AspNetCore.Identity PasswordHasher<T> (PBKDF2-SHA256) |
| Token | Hibrit: OpenIddict JWT + custom RefreshToken rotation chain |
| 3rd party | Faz 1 hepsi stub/NoOp (Google/Apple/Twilio/Brevo) |
| Catalog regresyon | W4.8 sonrası impact-review + mini-batch |

---

## 5. Sapma Defteri Ozeti (W4.0 — 9 F-W4 Handover-Only)

Formal Sapma 134+ numaralandırması W4.R reconcile turunda. Mevcut "Toplam: 133" header W4.R'de güncellenecek.

| # | Aile | Taraf | Ozet |
|---|---|---|---|
| F-W4-1 | 3 | Frontend | Namespace .Enums suffix tahmini — Backend Catalog emsal duzeltdi |
| F-W4-2 | 6 | Backend | UserPreferences plan-doc Currency/Country vs kod CurrencyCode/CountryCode celiskisi |
| F-W4-3 | 3 | Frontend | SearchTerm tahmini — plan-doc §12 search= literal Search dogru |
| F-W4-4 | 4+1 | Backend | Session handoff disipline ihlali — feedback_session_handoff_discipline.md eklendi |
| F-W4-5 | 3 | Frontend | ICurrentUserService XML doc 19 uye tahmini — fiili 18 |
| F-W4-6 | 3 | Backend | UserListItem Roles eklendi — Frontend Catalog emsal (bandwidth) reddetti |
| F-W4-7 | 3 | Backend | GetUserDetailAsync — Frontend Catalog GetXByIdAsync convention baskın |
| F-W4-8 | 3 | Frontend | Dosya sayım 13 tahmini — fiili 14 (UserStatus B-5 kararıyla eklendi) |
| F-W4-9 | 3 | Frontend | Memory dosyası 50-80 satır tahmini — fiili emsal 14 satır yogun paragraf |

**Ozet:** Frontend 6, Backend 3. Sıfır production sızıntısı. 3 sapma amend ile duzeltildi (F-W4-5/6/7).

---

## 6. W4.R Plan-Doc Revize Backlog (6 madde)

deviations.md W4.0 bolumunde tam detay:

1. **02-modules-list.md** — Wave numaralandırma (plan-doc Wave 2=Identity, fiili Wave 4=Identity)
2. **03-domain-patterns.md satır 431** — UserPreferences VO `Currency/Country` → `CurrencyCode/CountryCode`
3. **05-identity.md §13** — Faz 2 placeholder OAuth/OTP/Email NoOp stub stratejisi
4. **HashedPassword algoritma** — 3 doc çelişkisi (BCrypt×2 + Argon2id) → PasswordHasher<T> tek otorite
5. **05-identity.md §5** — DTO property listeleri literal (UserSummary/DeviceInfo/UserDetail/UserListItem/UserListQuery)
6. **05-identity.md §5** — IAdminUserReadService metot imzaları (GetUserByIdAsync + ListUsersAsync)

---

## 7. Memory Kazanımı (W4.0)

**Yeni dosya:** `feedback_session_handoff_discipline.md` (14 satır)

Bootstrap sırası (5 adım):
1. MEMORY.md okuma
2. Tum `feedback_*.md` okuma
3. Tum `reference_*.md` okuma
4. `CLAUDE.md` okuma (repo koku — build komutları, mimari ozet, DDD pattern'leri)
5. Frontend talimatı uygulama

**CLAUDE.md:** Commit `02f42f7` ile repo kokunde mevcut (119 satır, Mustafa push'u). Claude Code extension her chat'te otomatik okuyor — repo guidance kaynağı.

---

## 8. W4.1 Için Acık Yol — Identity.Domain

### Plan-doc referans
05-identity.md §2–§3 (satır 45–254):
- User AR + lifecycle metotları
- 5 child entity (User AR içinde): RefreshToken, UserDevice, UserExternalLogin, UserRole, UserConsent
- AR dışında: PhoneVerificationTicket (User'a referans, AR değil)
- Enum'lar: RevocationReason (Identity.Domain internal), PhonePurpose

### VO'lar
- `HashedPassword` — PasswordHasher<T> PBKDF2-SHA256 wrap
- `EmailAddress` — format validation
- `PhoneNumber` — E.164 (libphonenumber-csharp)
- `NationalId` — TC 11-hane algoritmik (plan-doc §4)
- `PersonName` — GivenName + FamilyName
- `ConsentGrant` — Domain internal (tip + versiyon + timestamp)

### Sub-batch dekompozisyon önerisi (yeni session fresh-read + plan-only ile kesinleşecek)

| Sub-batch | İçerik |
|---|---|
| W4.1.A | VO'lar (HashedPassword + EmailAddress + PhoneNumber + NationalId + PersonName + ConsentGrant) |
| W4.1.B | Child entity'ler (RefreshToken + UserDevice + UserExternalLogin + UserRole + UserConsent) |
| W4.1.C | User AR + factory Create + lifecycle metotları |
| W4.1.D | PhoneVerificationTicket + RevocationReason enum + PhonePurpose enum |
| W4.1.E | Domain event'ler (6 public + 6 internal, §6 literal) |
| W4.1.F | Build teyit + atomic commit |

**Kritik notlar W4.1 için:**
- User AR hassas: password-related metotlar PasswordHasher<T> inject alır (Domain bağımlılığı yoktur — servis inject edilir)
- RefreshToken rotation family detection Domain'de (FamilyId + ParentTokenId)
- KAYDET-33: Identity.Domain kurmadan önce tum entity isimleri Domain grep ile teyit et

---

## 9. Frontend Disipline Aktif Kurallar (Yeni Session Özeti)

| Kural | Ozet |
|---|---|
| Dil | Türkce konuş, ASCII fallback commit mesajı, em-dash UTF-8 güvenli |
| Scope | Plan-first, DUR bağlayıcı, "sonraki tur X" icra emri degil |
| KAYDET-32 | Doc-literal sadakat — fresh-read zorunlu, ezber YASAK |
| KAYDET-33 | Yeni klasor/proje oncesi Domain entity grep zorunlu |
| KAYDET-34 | Push oncesi bracket-paste mitigation (git add scoped) |
| KAYDET-9 | Cross-batch convention extrapolation YASAK (her batch fresh emsal) |
| Aile 5 | AI kendi sinirini in-band gevsete mez |
| main | 44416138 DOKUNULMAZ, her push sonrası 3 SHA teyit |
| git | push --force YASAK, git add . YASAK, amend sadece lokal-only |
| 3rd party | Faz 1 hepsi stub/NoOp (Google/Apple/Twilio/Brevo) |
| Bootstrap | MEMORY.md → feedback_*.md → reference_*.md → CLAUDE.md → talimat |

---

## 10. Wave 4 Push Bütçesi

| Metrik | Deger |
|---|---|
| Tahmini toplam push | 12–18 (10 sub-batch × 1.5 emsal) |
| Tamamlanan | 2 (#38 W4.0 + #39 ledger) |
| Kalan tahmini | 10–16 |
| Wave emsalleri | Wave 1: 11 push, Wave 2: 9 push, Wave 3: 16 push |

---
