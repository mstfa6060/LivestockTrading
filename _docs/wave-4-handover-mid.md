# Wave 4 W4.2 (Identity.Application) Mid-Handover

**Stamped:** Sunday, 31 May 2026 (W4.2.E + W4.2.E.R reconcile sonrasi — W4.2 RESMI KAPANIS)
**Commit:** W4.2.E.R reconcile (self — bu commit; SHA push sonrasi origin/rebuild/v2'de gorulur). Onceki kod ucu `809d7e7` (W4.2.E.4 forceLogout).
**Push tatbikati:** YOK (Backend-executed milestone, W4.2.E.R commit + tag + push); ahead 29 lokal
**Context:** Esige yaklasildi, yeni Frontend Claude session W4.2'yi D.3'ten devralabilsin diye uretildi.

---

## 1. Repo Durumu (canli)

| Branch / Ref | SHA | Durum |
|---|---|---|
| `main` | `44416138b978774146f992f9e0756b829ba541e0` | **INVARIANT** — 44/44 push tatbikati boyunca dokunulmadi |
| `rebuild/v2` (lokal) | `809d7e7` → E.R reconcile (self — bu commit) | W4.2 TUMU KAPANDI (D + D2-out + R + E + E.R), 30 commit ahead push oncesi |
| `origin/rebuild/v2` | `b8c7845` | W4.1 (Identity.Domain) son push; W4.2 lokal-only |
| Tag `wave-0-complete` | intact | |
| Tag `wave-1-complete` | intact | |
| Tag `wave-2-complete` | intact (`e7e7fec` → `1f2c7dd`) | |
| Tag `wave-3-complete` | intact | |
| Tag `wave-4-1-complete` | `b8c7845` | W4.1 Identity.Domain kapanis |
| Tag `wave-4-complete` | YOK | Wave 4 sonu eklenecek (W4.3 + W4.4 sonrasi); wave-ici ara-tag emsali yok (F-S 155 kayit) |

`main..HEAD = 101` (production'dan rebuild/v2 ucuna gercek delta; W4.2.E.R toplu commit + tag sonrasi 102+ olacak).
Working tree: clean.

---

## 2. W4.2 Commit Zinciri (30 commit, origin `b8c7845` sonrasi, hepsi lokal, push bekliyor)

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
| docs(handover-mid-2) | 2ad0a20 | W4.2.D handover guncelleme (D.3 + D.3.5 kapanis) |
| D2-out.0 | 8ae54cf | reset-channel foundation (EmailPurpose enum + 3 port + Domain amendment) |
| D2-out.1 | e91a60e | password reset dual-channel (Forgot + Reset quartet, RevokeAll cascade) |
| D2-out.2 | 4a20f21 | phone OTP register verify (Register purpose, anonim Public) |
| docs(D2-out.2) | c1fddd8 | 05-identity §13 ChangePhone Faz-2 placeholder (B-W4.2-D2-1 audit-trail) |
| W4.2.R | 03d7fa8 | deviations.md W4.0 + D2-out F-S 135-151 formal kayit, Pozitif Onleme 4 kalem |
| E.1 | f50e29d | admin users read foundation (AdminSessionInfo + IAdminUserReadService +1 + 3 read endpoint + aggregator) |
| E.2 | 6a44486 | admin users suspend reactivate (Suspend quartet + Reactivate trio + aggregator +2) |
| E.3 | 133bccf | admin users grant revoke role (GrantRole + RevokeRole quartet + aggregator +2; B-W4.1-4 cift-event teyit) |
| E.4 | 809d7e7 | admin users force logout (ForceLogout trio handler-cascade + aggregator +1; /admin/users 8/8 yapisal tam) |
| W4.2.E.R | self (bu commit) | deviations.md W4.2.E F-S 152-155 formal (152/153 Backend Domain-gozlem + 154/155 Frontend self-catch SHA-self-ref + ara-tag emsalsiz) + Pozitif Onleme 3 + Karar 3 (B-W4.2-E-1/2/3); handover-mid guncelleme |

---

## 3. Tamamlanan Endpoint Yuzeyi (doc §12 sayim)

### Public (13/15)
- `POST /auth/login` (B.2, 3 method)
- `POST /auth/refresh` (B.2, reuse detection)
- `POST /auth/logout` (B.2)
- `POST /auth/register` (B.3, KVKK zorunlu)
- `POST /auth/email/send-verify` (C.3)
- `POST /auth/email/verify` (C.3)
- `POST /auth/oauth/google` (C.6, auto-link)
- `POST /auth/oauth/apple` (C.6, auto-link)
- **TAMAM (D2-out — B-W4.2-D2-1 Karar Z)**: `POST /auth/password/forgot` + `POST /auth/password/reset` (D2-out.1 `e91a60e`); `POST /auth/phone/send-code` + `POST /auth/phone/verify` (D2-out.2 `4a20f21`)
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

### Admin (8/9 — TAMAM, audit delege)
TAMAM (W4.2.E sub-batch'ler):
- `GET /admin/users` (E.1 list, `[AsParameters] UserListQuery` cursor + 4 filtre)
- `GET /admin/users/{id}` (E.1 detay, `UserDetail?` null → 404)
- `GET /admin/users/{id}/sessions` (E.1 sessions, `AdminSessionInfo` 7-field projeksiyon)
- `POST /admin/users/{id}/suspend` (E.2 quartet, reason body, Domain cascade refresh-revoke + UserSuspended event)
- `POST /admin/users/{id}/reactivate` (E.2 trio, void)
- `POST /admin/users/{id}/grant-role` (E.3 quartet, role body, Domain H-10 idempotent + UserRoleGranted event)
- `POST /admin/users/{id}/revoke-role` (E.3 quartet, role body, UserRoleRevoked event)
- `POST /admin/users/{id}/force-logout` (E.4 trio handler-cascade, `RevocationReason.AdminRevoked`)
- **HARIC:** `GET /admin/users/{id}/audit` (Wave 7 Admin module delege); `POST /admin/users/{id}/impersonate` (B-W4.2-E-1: Wave 7 / W4.3 host-auth delege, contract `IAdminUserCommands.ImpersonateAsync` durur ama Application impl yok)
- **Auth:** `RequireRole("admin")` tek-rol (B-W4.2-E-2 super-admin tier; Catalog `("admin","moderator")` REDDEDILDI). Auth-inert host-auth wave revisit (W4.4).

---

## 4. Application Port Envanteri (W4.3 Infrastructure girdisi)

### EF-bagimli (5)
- `IUnitOfWork` (SaveChangesAsync, UnitOfWorkFilter cagrir)
- `IUserRepository`
- `IPhoneVerificationTicketRepository`
- `IEmailVerificationTicketRepository` (D2-out.0: `GetActiveByEmailAsync` imzasi +`EmailPurpose` param)
- `IMeReadService` (CQRS read model projeksiyonu)

### Kripto / token (4)
- `IJwtTokenService` — RS256 + key rotation (90 gun, 2-cert overlap) + Redis jti blacklist (W4.3)
- `IRefreshTokenGenerator` — 32-byte hex raw + SHA256 hash (W4.3)
- `IEmailVerificationTokenGenerator` — raw hex + `SHA256.HashData(Convert.FromHexString(raw))` hash. **KONTRAT D.1c'de sertlesti** — UTF8 YASAK; Domain `RequestEmailChange` + `VerifyEmail` ayni algoritmayi varsayar.
- `IPhoneOtpCodeGenerator` (D2-out.0) — 6-digit numeric raw + `SHA256.HashData(Encoding.UTF8.GetBytes(rawCode))` byte[32]. W4.3 concrete bu algoritmaya **uymak zorundadir** (round-trip test: `Hash(Generate().Raw) == Generate().Hash`).

### NoOp Faz 1 (3)
- `IEmailSender` — `SendEmailVerificationAsync` + `SendEmailChangeAsync` (D.1b) + `SendPasswordResetAsync` (D2-out.0); W4.3 Faz 1 log stub, Faz 2 SMTP
- `IExternalLoginValidator` — Google/Apple ID-token validate; W4.3 Faz 1 NoOp (Backlog #79), Faz 2 gercek validator
- `ISmsSender` (D2-out.0) — `SendOtpAsync(PhoneNumber, code, ct)`; W4.3 Faz 1 NoOp + log adapter, Faz 2 Twilio (Notifications module Backlog #57/#73)

### Cross-cutting (1)
- `IFileStorage` (Shared.Contracts.Storage, D.1b) — concrete **Shared.Infrastructure** MinIO (bucket=`avatars` + `data-exports`); B-W4.2-D-1 karari.

### Shared.Contracts/Identity/Admin (W4.0 + W4.2.E)
- `IAdminUserCommands` (6 metot): Suspend / Reactivate / GrantRole / RevokeRole / ForceLogout / **Impersonate**. W4.2.E'de **5 Application impl** yapildi (Suspend/Reactivate/GrantRole/RevokeRole/ForceLogout handler-direct User AR cagrisi, IAdminUserCommands impl Wave 7 / W4.3 delege). Impersonate impl YOK (B-W4.2-E-1).
- `IAdminUserReadService` (3 metot): `ListUsersAsync` + `GetUserByIdAsync` (W4.0) + **`GetSessionsByUserAsync` (E.1 additive)**. Concrete W4.3 Infrastructure (NotImpl emsali, auth-inert).
- `AdminSessionInfo` (E.1 yeni DTO): 7-field projeksiyon (IsCurrent dusurulmus, B-W4.2-E-3 Karar c). Me-scope `SessionInfo` (MeProfile.cs:44) dokunulmadi.

### Domain amendment (D2-out.0 c-light)
- **Yeni enum:** `EmailPurpose { Verify = 1, ResetPassword = 2 }` — `Identity.Domain/Enums/EmailPurpose.cs`
- **`EmailVerificationTicket.Purpose`** property discriminator + `Issue` factory +EmailPurpose param. Migration etkisi YOK (Identity.Infrastructure'da hicbir migration yok, W4.3 InitialCreate'de NOT NULL kolon DEFAULT 1 ile uretilir).

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
5. `IEmailSender` + `IExternalLoginValidator` Faz 1 NoOp+log stub (`SendPasswordResetAsync` D2-out.0 ile +1 metot).
6. `IRefreshTokenGenerator` concrete (RNG 32-byte hex + SHA256).
7. **YENI (D2-out.0):** `IPhoneOtpCodeGenerator` concrete — 6-digit numeric (`Random.Shared.Next(0, 1_000_000).ToString("D6")` emsali) + `SHA256.HashData(Encoding.UTF8.GetBytes(raw))` byte[32]. Round-trip test ZORUNLU (kontrat D2-out.0 port XML doc).
8. **YENI (D2-out.0):** `ISmsSender` Faz 1 NoOp + log adapter (IEmailSender stub emsali). Faz 2'de Twilio (Notifications module delege, Backlog #57/#73).
9. `DataExportWorker` (Quartz) + `IDataExportContributor` (doc §11) — **veya W4.4 ayri batch**.
10. **YENI (E.1):** `AdminSessionInfo.IpAddress` (non-null `string`) kaynak entity field karari. `RefreshToken` + `UserDevice` Domain'de `IpAddress` field **YOK** (sadece `UserConsent`'te var); Me-scope `SessionInfo.IpAddress` ayni durumda. W4.3 mapping secenekleri: (a) `RefreshToken` entity'ye `IpAddress` alani ekle (Domain amendment, migration), (b) audit log ayri lookup, (c) DTO nullable'a cevir (geri-uyum sapmasi, B-W4.2-E-3 revize). Karar W4.3 InitialCreate Domain-review turunde.

---

## 6. W4.2 Kalan Is

### E (Admin 8/9 endpoint) — KAPANDI ✓
- 5 commit (E.1 `f50e29d` + E.2 `6a44486` + E.3 `133bccf` + E.4 `809d7e7` + E.R self bu commit): 23 yeni .cs (AdminUsers/ klasor 22 + Shared.Contracts/Admin AdminSessionInfo) + 1 modify (IAdminUserReadService).
- Solution-wide build 0/0 (E.5 sanity teyit).
- audit + impersonate Wave 7 / W4.3 delege (B-W4.2-E-1 karari).
- Application impl handler-direct User AR cagrisi; IAdminUserCommands concrete Wave 7 / W4.3.

### W4.2.E.R (KAPANDI — bu commit) ✓
- `deviations.md` W4.2.E F-S 152-153 formal (Aile 6 Domain idempotency tutarsizligi, Backend-CATCH) + Pozitif Onleme 3 entry + 3 yeni karar (B-W4.2-E-1/2/3).
- Bu handover-mid guncellendi ("wave-4-2 complete" snapshot).
- **Sirada:** Backend-executed push (30 commit milestone). **Tag YOK** bu push'ta (F-S 155: wave-ici ara-tag emsali yok); `wave-4-complete` Wave 4 sonu (W4.3 + W4.4 sonrasi).

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
| B-W4.2-D2-1 | Phone OTP send-code/verify Faz-1 SADECE Register purpose (anonim Public); ChangePhone + LoginPhoneOtp Faz-2 (§13 placeholder). Password reset cift-kanal (email + phone) c-light (EmailVerificationTicket.Purpose discriminator). | D2-out.0/1/2 + docs (`8ae54cf` + `e91a60e` + `4a20f21` + `c1fddd8`) |
| B-W4.2-E-1 | Impersonate Wave 7 / W4.3 host-auth delege — spec 447 JWT mint + `IJwtTokenService` RS256+jti bagimli; `IAdminUserCommands.ImpersonateAsync` contract'ta durur, Application handler+endpoint YAZILMADI. Auth-inert Faz 1. | E.0 PLAN |
| B-W4.2-E-2 | Admin auth role `"admin"`-only tek-rol (`RequireAuthorization(policy => policy.RequireRole("admin"))`). Spec doc-silent; Catalog emsali `("admin", "moderator")` REDDEDILDI (user suspend/role super-admin tier). Auth-inert host-auth wave revisit (W4.4). | E.1 |
| B-W4.2-E-3 | `AdminSessionInfo` 7-field DTO (Karar c) — Me-scope `SessionInfo`'dan `IsCurrent` dusurulerek tureti; admin baglaminda current-session marker tanimsiz. Me-tarafi DOKUNULMADI (0 modify). Spec doc-silent, defansif default. | E.1 |

---

## 8. Sapma Defteri Ozeti (W4.2.R'de deviations.md'ye formal tasindi — `03d7fa8`)

**W4.2.R RECONCILE TAMAM** (`03d7fa8`): W4.0 handover-only 9 entry (F-W4-1..F-W4-9) + W4.2 D2-out 8 negatif sapma **formal F-S 135-151** numara aldi. Pozitif kod-emsali kayitlari ayri **Pozitif Onleme** bolumune girdi (formal sayima girmez).

**Header reconcile:** eski 133/134 cakismasi → fiili tablo 151 satir. Aile dagilim: Aile 1 +1 (CRLF), Aile 2 +2 (sayim + handover reconcile), Aile 3 +7 (KAYDET-9 cross-ref 4 entry), Aile 4+1 hibrit +1 (session handoff), Aile 6 +6 (spec vs kod). Backend 19, Frontend 131, Bilgi 1.

**W4.2.E.R RECONCILE TAMAM** (self bu commit): F-S 152-155 formal eklendi (152/153 Backend Domain-gozlem + 154/155 Frontend self-catch ADIM 3a) → **toplam 155** (Backend 21, Frontend 133, Bilgi 1). Pozitif Onleme 3 entry (E.4 Aile-1 stale-Edit + E.3 B-W4.1-4 cift-event teyit + E.5 F2 sayim reconcile) formal-disi. 3 yeni karar (B-W4.2-E-1/2/3) karar arsivine eklendi.

**KRITIK NOT — F-S 152/153 sayac yakalama-tarafi konvansiyonu:** F-S 152 (User.Suspend already-suspended event-dup) + F-S 153 (Domain idempotency tutarsizligi GrantRole H-10 vs Suspend kosulsuz) **Backend-CATCH** kayit (Domain-gozlem; E.2/E.3 yaziminda grep ile yakalandi). E batch'inde **iki tarafta da fiili kod-hatasi YOK** — Application yazimi dogru (Domain'e guvenir, Application'da idempotency guard ekleme yanlis olurdu cunku Domain otoritedir). Sayac yakalama-tarafi (kim yakaladi) konvansiyonuna gore Backend 21'e gitti; sorun Wave 7 Domain-katmani hardening backlog'da. **Gelecek-okuma:** F-S 152/153 "Frontend talimat sapmasi" veya "Backend kod sapmasi" olarak yanlis-yorumlanmamali.

### KRITIK NOT — Sayim metodolojisi (KAYDET-32 tuzagi)

**Formal toplam 151 = WAVE-KUMULATIF sayim metodu** (W3 emsali, satir 391 reconcile notu). **Fiili `grep -c "^| [0-9]+ |"` distinct numara = 117** (W0-2 reconcile sayim-atlamalari, alt-tablo KAYDET satirlari karisik). **Gelecek reconcile bu metodu KORUMALI** — fiili grep ile 151'i yanlis-duzeltME. Aksi halde wave-bazli kumulatif sayim metodolojisi (her wave +N satir, wave reconcile notlari) kirilir; 151 dogru.

### Pozitif onleme (Backend pre-write yakalama, deviations.md ayri "Pozitif Onleme" bolumune tasindi — formal-disi)
- B.2: `IPasswordHasher` Domain-VO catismasi (port gereksiz, B.1-rev)
- B.3: `PersonName.From` ezber (gercek API farkli, doc-literal refleksi)
- C.1: Event uydurma engelleme (Domain event listesinden secim)
- C.2: `RevokeConsent` guard telafi (Domain'de eksik, Application validator + telafi)
- C.3: `ChangePassword` cascade (refresh token revoke unutulmasin)
- C.4: `IPublishEndpoint` ilk emsal (MassTransit producer pattern)
- D.1a: Email-change token-match guvenlik acigi (W4.1 retro check-list maddesi)
- D.3 deflect → D.1c: Hash algoritma uyusmazlik
- D.3 smooth (D2-out reconcile turunde pozitif olarak Wave 4 W4.2 Pozitif Onleme Defterine girdi)
- **D2-out yeni pozitif:** enumeration-protection idiom + generic `INVALID_OR_EXPIRED` failure mode + `RevokeAllRefreshTokens` cascade Reset basarisinda. Doc-literal degil, kod-konvansiyonu (SendEmailVerifyHandler:42-49 + ChangePasswordHandler:65 emsali); D2-out.1 Forgot + Reset + D2-out.2 SendPhoneOtp + VerifyPhoneOtp'de uyumlu uygulandi.

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
- **KAYDET-32 (W4.2.R'de exercise edildi):** Frontend talimat uretmeden once ilgili doc/defter FRESH-READ. Sayim/SHA/F-S/Aile **ezber YASAK** — fiili kaynak. Wave 3 W3.6.B'de formalize, W4.2.D.HANDOVER + W4.2.R'de exercise (header drift duzeltme; 151 vs 117 distinct ayrimi metodoloji teyit; D.3.0/D.3.1.0/D.3.2.0/D.3.5.0 pre-write keşif turları). Backend G1 fresh-read ile capraz pekisme (KAYDET-9 Backend tarafi).

---

## 9. Yeni Session Ilk Aksiyon

1. Bu handover'i (wave-4-handover-mid.md) bastan sona oku — HEAD W4.2.E.R reconcile (self bu commit; onceki kod ucu `809d7e7`), push sonrasi ahead 0, `main..HEAD = 102`, main `44416138` **INVARIANT**.
2. W4.2 **TAMAMI KAPANDI** (D + D2-out + R + E + E.R): Public 13/15 + Authenticated 19/19 + Admin 8/9 (audit + impersonate delege).
3. Sirada **W4.3 Infrastructure** — §5 W4.3 backlog 10 madde oncelik sirali (KRITIK ilk: IEmailVerificationTokenGenerator round-trip kontrat). Plan-only kickoff: backlog reorder + ilk concrete sub-batch (muhtemel EF Core repository concrete + DbContext + InitialCreate migration).
4. **Backend-executed push** (W4.2.E.R commit + 30 commit, **tag YOK** bu push'ta — F-S 155 wave-ici ara-tag emsalsiz); operating-model: push Backend yurutur, guard `--dry-run` once + 3-SHA teyit; main'e ASLA, `--force` ASLA. `wave-4-complete` Wave 4 sonu (W4.3 + W4.4 sonrasi).
5. Backend VS Code hazir; ilk talimat W4.2.E.R toplu commit (deviations.md + wave-4-handover-mid.md scoped add).

**DUR.** Push yok, SSH yok, main DOKUNULMAZ.
