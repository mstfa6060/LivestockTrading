# Wave 4 W4.2 (Identity.Application) Mid-Handover

**Stamped:** Sunday, 31 May 2026 (W4.2.D KAPANDI: D.3 + D.3.5 sonrasi)
**Commit:** da74b7e (W4.2.D.3.5 spec revize, lokal rebuild/v2 ucu)
**Push tatbikati:** YOK (Mustafa eli Wave sonu tek milestone push); ahead 18 lokal
**Context:** Esige yaklasildi, yeni Frontend Claude session W4.2'yi D.3'ten devralabilsin diye uretildi.

---

## 1. Repo Durumu (canli)

| Branch / Ref | SHA | Durum |
|---|---|---|
| `main` | `44416138b978774146f992f9e0756b829ba541e0` | **INVARIANT** — 44/44 push tatbikati boyunca dokunulmadi |
| `rebuild/v2` (lokal) | `da74b7e` | W4.2.D KAPANDI (D.3 + D.3.5), 18 commit ahead |
| `origin/rebuild/v2` | `b8c7845` | W4.1 (Identity.Domain) son push; W4.2 lokal-only |
| Tag `wave-0-complete` | intact | |
| Tag `wave-1-complete` | intact | |
| Tag `wave-2-complete` | intact (`e7e7fec` → `1f2c7dd`) | |
| Tag `wave-3-complete` | intact | |
| Tag `wave-4-1-complete` | `b8c7845` | W4.1 Identity.Domain kapanis |
| Tag `wave-4-2-complete` | YOK | W4.2 sonu eklenecek |

`main..HEAD = 90` (production'dan rebuild/v2 ucuna gercek delta; bu handover guncellemesi commit'i ile 91 olacak).
Working tree: clean.

---

## 2. W4.2 Commit Zinciri (18 commit, origin `b8c7845` sonrasi, hepsi lokal, push bekliyor)

| Sub-batch | SHA | Kapsam (kisa) |
|---|---|---|
| A | 147f0b6 | Foundation iskelet (Application csproj + Common helpers + ValidationFilter/UnitOfWorkFilter pipeline + 3 port skeleton) |
| B.1 | a93a777 | Auth port (IJwtTokenService + IRefreshTokenGenerator) + Domain RevokeFamilyRefreshTokens + LoginResponse DTO |
| B.1-rev | 839b14c | fix: IPasswordHasher port revert (Domain VO Verify<TUser> kullanilir, port gereksiz) |
| B.2 | 8f0426d | Token lifecycle (login 3-method: password/phone/email + refresh rotate + reuse detection + logout) |
| B.3 | 0cbefed | Password register + auth core kapanis (KVKK zorunlu consent validator) |
| C.1 | 0a4a56a | Self-service Domain amendment (5 metot) + EmailVerificationTicket AR + 5 port + MeProfile read model |
| C.2 | dee4f8f | Profil (GET /me, PATCH /me, preferences, consents) |
| C.3 | 12d600d | Sifre degisimi + sessions (list/revoke) + email-verify send/verify |
| C.4 | df77f8a | Hesap silme/geri alma + data-export (DataExportRequested **internal** event) |
| C.5 | 2488bc5 | Cihaz yonetimi + harici giris baglantisi (link/unlink) |
| C.6 | 3382377 | Sosyal kayit + TokenIssuance helper (auto-link doc §10 emsali) |
| D.1a | d7467a0 | fix: email-change token-match guvenlik acigi (ConfirmEmailChange byte[] hash imza) |
| D.1b | 0943df0 | IFileStorage Shared.Contracts.Storage port + SetAvatarUrl (D1) + IEmailSender.SendEmailChangeAsync |
| D.2 | 4af6640 | Avatar yukleme (multipart, ilk emsal endpoint) |
| D.1c | 65958f5 | fix: email token hash algoritma kontrati (FromHexString→SHA256 netlesti, UTF8 YASAK) |
| docs(handover-mid-1) | f8fa2a1 | W4.2 mid-handover (D.1c sonrasi) |
| D.3 | 8f640b9 | email-change 3 use-case (Request/Confirm/Cancel quartet + MeEndpoints 3 route) |
| D.3.5 | da74b7e | 05-identity spec 19 endpoint hizalama (B-W4.2-D-3 audit-trail) |

---

## 3. Tamamlanan Endpoint Yuzeyi (doc §12 sayim)

### Public (9/15)
- `POST /auth/login` (B.2, 3 method)
- `POST /auth/refresh` (B.2, reuse detection)
- `POST /auth/logout` (B.2)
- `POST /auth/register` (B.3, KVKK zorunlu)
- `POST /auth/email/send-verify` (C.3)
- `POST /auth/email/verify` (C.3)
- `POST /auth/oauth/google` (C.6, auto-link)
- `POST /auth/oauth/apple` (C.6, auto-link)
- **ACIK** (D2-out): `POST /auth/phone/send-otp`, `POST /auth/phone/verify-otp`, `POST /auth/password/forgot`, `POST /auth/password/reset` (4 endpoint)
- **SCOPE-DISI**: OpenIddict `/connect/*` 3 endpoint (W4.3+ Infrastructure)

### Authenticated (19/19) — TAMAM
- `/me` GET (C.2), PATCH (C.2, email-change HARIC — D.3.5 spec hizalandi)
- `/me/password` PATCH (C.3, cascade revoke refresh)
- `/me/sessions` GET (C.3), DELETE (C.3)
- `/me/data-export` POST (C.4, DataExportRequested internal event)
- `/me/account` DELETE (C.4, soft-delete), `/me/account/restore` POST (C.4)
- `/me/external-logins` POST (C.5, link), DELETE (C.5, unlink + son-auth-method invariant)
- `/me/devices` POST (C.5), `/me/devices/{id}/push-token` PATCH (C.5), DELETE (C.5)
- `/me/preferences` PATCH (C.2)
- `/me/consents` PATCH (C.2, RevokeConsent guard telafi)
- `/me/avatar` POST (D.2, multipart) — IFileStorage W4.3'te MinIO concrete
- **TAMAM (D.3 — 8f640b9)**: `POST /me/email-change/request`, `POST /me/email-change/confirm`, `DELETE /me/email-change` (3 endpoint)

### Admin (0/9)
HEPSI ACIK — W4.2.E:
- `GET /admin/users` (list, filter+page)
- `GET /admin/users/{id}` (detay)
- `POST /admin/users/{id}/suspend`
- `POST /admin/users/{id}/reactivate`
- `POST /admin/users/{id}/roles` (grant)
- `DELETE /admin/users/{id}/roles/{role}` (revoke)
- `POST /admin/users/{id}/force-logout` (RevokeAllRefreshTokens)
- `GET /admin/users/{id}/sessions`
- (audit endpoint HARIC — Wave 7 Admin module delege)

---

## 4. Application Port Envanteri (W4.3 Infrastructure girdisi)

### EF-bagimli (5)
- `IUnitOfWork` (SaveChangesAsync, UnitOfWorkFilter cagrir)
- `IUserRepository`
- `IPhoneVerificationTicketRepository`
- `IEmailVerificationTicketRepository`
- `IMeReadService` (CQRS read model projeksiyonu)

### Kripto / token (3)
- `IJwtTokenService` — RS256 + key rotation (90 gun, 2-cert overlap) + Redis jti blacklist (W4.3)
- `IRefreshTokenGenerator` — 32-byte hex raw + SHA256 hash (W4.3)
- `IEmailVerificationTokenGenerator` — raw hex + `SHA256.HashData(Convert.FromHexString(raw))` hash. **KONTRAT D.1c'de sertlesti** — UTF8 YASAK; Domain `RequestEmailChange` + `VerifyEmail` ayni algoritmayi varsayar.

### NoOp Faz 1 (2)
- `IEmailSender` — `SendEmailVerificationAsync` + `SendEmailChangeAsync` (D.1b'de ikincisi eklendi); W4.3 Faz 1 log stub, Faz 2 SMTP
- `IExternalLoginValidator` — Google/Apple ID-token validate; W4.3 Faz 1 NoOp (Backlog #79), Faz 2 gercek validator

### Cross-cutting (1)
- `IFileStorage` (Shared.Contracts.Storage, D.1b) — concrete **Shared.Infrastructure** MinIO (bucket=`avatars` + `data-exports`); B-W4.2-D-1 karari.

### Transitive
- `IPasswordHasher<User>` (Microsoft.AspNetCore.Identity, Domain.csproj'dan; B.1-rev'de port reverted, Domain VO Verify<TUser> kullanir)

### MassTransit registration
- **AddMediator assembly-scan**: 21 consumer + 12 validator (manuel-liste YASAK — yeni handler eklerken DI degisikligi yok)

---

## 5. W4.3 Infrastructure Backlog (oncelik sirasi)

1. **KRITIK** — `IEmailVerificationTokenGenerator` concrete:
   - Round-trip test: `Hash(Generate().Raw) == Generate().Hash` **VE** Domain `RequestEmailChange` algoritmasiyla (`SHA256.HashData(Convert.FromHexString(raw))`) eslesme.
   - Uyusmazsa email-change confirm + email-verify **SESSIZ FAIL** (kullanici dogru token girer, 404 alir).
2. EF Core repository concrete'leri (5) + DbContext + schema-per-module (`identity` schema) + InitialCreate migration.
3. `IJwtTokenService` RS256 + key rotation (90 gun 2-cert overlap) + Redis jti blacklist.
4. `IFileStorage` MinIO concrete (Shared.Infrastructure/Storage, bucket=`avatars` + `data-exports`).
5. `IEmailSender` + `IExternalLoginValidator` Faz 1 NoOp+log stub.
6. `IRefreshTokenGenerator` concrete (RNG 32-byte hex + SHA256).
7. `DataExportWorker` (Quartz) + `IDataExportContributor` (doc §11) — **veya W4.4 ayri batch**.

---

## 6. W4.2 Kalan Is

### D2-out (SIRADA)
Phone OTP (send-code + verify-code, `PhoneVerificationTicket` AR hazir) + password forgot/reset.
**ACIK KARAR (D2-out girisinde Mustafa onayina):** Password-reset ticket stratejisi:
- (a) `PhoneVerificationTicket` purpose reuse (`PhonePurpose.ResetPassword` enum'da var — minimum kod, semantik karisik)
- (b) Ayri `PasswordResetTicket` AR (temiz, +1 entity +1 repo)
- (c) `EmailVerificationTicket` emsali yeni AR (`PasswordResetTicket : EmailVerificationTicket` pattern reuse)

### E (Admin 8 endpoint)
- audit HARIC (Wave 7 Admin module delege)
- `IAdminUserCommands` + `IAdminUserReadService` concrete (W4.0 Shared.Contracts'ta imza var)
- Domain metotlar hazir: `Suspend`, `Reactivate`, `GrantRole`, `RevokeRole`, `RevokeAllRefreshTokens`

### W4.2.R (Reconcile)
- `deviations.md` W4.2 F-S kayitlari (asagidaki §8 ozeti kanonik kayda donusturulecek)
- Bu handover guncellenip "complete" snapshot
- W4.2 commit log (15+D.3+D2-out+E sayim) deviations.md'de
- Tek milestone push hazirlik (Mustafa eli) → `wave-4-2-complete` tag

---

## 7. Acik Karar Arsivi (W4.2 boyunca, reconcile edilmis)

| Kod | Karar | Sub-batch |
|---|---|---|
| B-W4.2-1 | Clock: `TimeProvider.System` (BCL) | A |
| B-W4.2-4 | PasswordHasher: Domain VO `Verify<User>` (BCL `IPasswordHasher<User>` transitive); non-generic port revert | B.1-rev |
| B-W4.2-N-8 | `INVALID_CREDENTIALS` → 401 (Identity `ResultExtensions` ozel-case, doc emsali yok) | B.2 |
| C-3 | PATCH `/me` email-change HARIC (ayri endpoint, D.3) | C.2 |
| C-5 | `DataExportRequested` **internal** event (Worker Identity-internal, Notifications consumer YOK) | C.4 |
| C-7 | External-login unlink son-auth-method invariant Domain'de (Application yalniz delege) | C.5 |
| C-9 | Sosyal kayit tek handler + 2 route (provider body discriminator) | C.6 |
| D-2 | Email-change token-match: Domain amendment (`PendingEmailTokenHash` byte[16]) | D.1a |
| D-1 | `IFileStorage` Shared.Contracts.Storage (doc §466); concrete Shared.Infrastructure W4.3 | D.1b |
| D.1c | Hash algoritma kontrati: `FromHexString → SHA256.HashData` (UTF8 YASAK; round-trip W4.3 testi) | D.1c |
| B-W4.2-D-3 | Email-change 3 ayri REST endpoint (PATCH `/me` icine gomme yerine); spec satir 561/566/580/596/654 revize | D.3 + D.3.5 (`8f640b9` + `da74b7e`) |

---

## 8. Sapma Defteri Ozeti (W4.2 boyunca, W4.2.R'de deviations.md'ye)

### Pozitif onleme (Backend pre-write yakalama)
- B.2: `IPasswordHasher` Domain-VO catismasi (port gereksiz, B.1-rev)
- B.3: `PersonName.From` ezber (gercek API farkli, doc-literal refleksi)
- C.1: Event uydurma engelleme (Domain event listesinden secim)
- C.2: `RevokeConsent` guard telafi (Domain'de eksik, Application validator + telafi)
- C.3: `ChangePassword` cascade (refresh token revoke unutulmasin)
- C.4: `IPublishEndpoint` ilk emsal (MassTransit producer pattern)
- D.1a: Email-change token-match guvenlik acigi (W4.1 retro check-list maddesi: "raw token donduren ama hash saklamayan metot = guvenlik bos")
- D.3 deflect → D.1c: Hash algoritma uyusmazlik (Domain `Convert.FromHexString` vs Application yanlislikla `Encoding.UTF8` riski)
- D.3 smooth: 5 yazim turu (Request/Confirm/Cancel quartet + MeEndpoints register + spec revize) 0 yazim sapmasi; tum sapma adaylari kesif turlarinda erken yakalandi (D.3.0/D.3.1.0/D.3.2.0/D.3.5.0). D.3-dev-1 (Confirm 3-param imza), D.3-dev-2 (Domain self-generate token, `_tokenGen` REQUEST'te yok), D.3-dev-3 (DTO primitive string, VO degil) + CRLF benign (Aile 1) + spec-deviation B-W4.2-D-3 → W4.2.R'de deviations.md'ye F-S numarasi alacak (handover'da kayit, henuz numara yok).
- D.HANDOVER reconcile: 4 sayim duzeltmesi (ahead 19→18, kaynak .cs 116→110 obj-haric, tatbikat 47→44 push-yok, §2 15→18) — W1-2 stat reconcile + Aile 1 obj/ sahte-pozitif onleme calisti.

### Aile-tezahurleri
- **Aile 1** (Edit cache stale): Commit sonrasi `str_replace` oncesi `Read`-refresh mecburi — C.3 + C.6 iki kez kanitlandi, kalici refleks.
- **Aile 5** (main DOKUNULMAZ): Wave 4 boyunca 44/44 push tatbikatinda korundu.

### W4.1 retrospect
- "Domain'de raw token uretip hash saklamayan metot = guvenlik bos" → W4.2 check-list maddesi → D.1a kaynak.

### Frontend disiplin (W4.2 boyunca)
- Plan-first
- DUR (sapma anlik durdur)
- 3-seviye sapma (Backend → Frontend → Mustafa)
- Doc-literal (planning doc satir-bazli takip)
- W1-1 (overwrite guard) + W1-2 (file-exists guard)
- Aile 5 (main DOKUNULMAZ)

---

## 9. Yeni Session Ilk Aksiyon

1. Bu handover'i (wave-4-handover-mid.md) bastan sona oku — HEAD `da74b7e`, ahead 18, `main..HEAD = 91` (bu handover guncellemesi commit'i dahil), main `44416138` **INVARIANT**.
2. W4.2.D **KAPANDI**; sirada **D2-out** (phone OTP + password reset).
3. D2-out kickoff: ONCE plan-only tur (kod yok) — kac use-case, hangi Domain metotlari hazir, spec hangi endpoint'ler. Doc fresh-read mecburi (KAYDET-32 / Aile 1 emsali).
4. D2-out sonrasi **E** (Admin 8) → **W4.2.R** (reconcile + deviations.md F-S kayit + `wave-4-2-complete` tag + Mustafa-eli push, 18+ commit tek milestone).
5. Backend VS Code'da hazir mi teyit; ilk talimat plan-only D2-out (password-reset ticket stratejisi karari a/b/c).

**DUR.** Push yok, SSH yok, main DOKUNULMAZ.
