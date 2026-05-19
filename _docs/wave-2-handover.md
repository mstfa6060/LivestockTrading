# Wave 2 → Wave 3 Devir Teslim

**Durum:** Wave 2 RESMİ KAPANIŞ — Catalog.Application tamam (9 aggregate + admin read NotImpl), retro defteri konsolide. rebuild/v2 = 74550a7 (push #19 sonrası, lokal=origin senkron)
**Tarih:** 2026-05-19
**Sonraki:** Wave 3 — Catalog.Infrastructure (EF DbContext + Configurations + Repository impl + RateProviders 3-tier + Quartz + RateLog + Program.cs host-wiring + NotImpl replace)

## Repo Durumu

| Branch/Ref | SHA | Durum |
|---|---|---|
| `main` | `44416138` | ArfBlocks production — Wave 0+1+2 boyunca DOKUNULMADI (INVARIANT, 19 push tatbikatı) |
| `rebuild/v2` | `74550a7` | Wave 2 kapanış (retro deviations) — lokal = origin senkron (ahead 0) |
| `feature/wave-0-infra` | `cc8d51b` | Korumalı, dokunulmadı (lokal=origin teyit) |
| `feature/wave-0-cleanup` | `f8a5073` | Korumalı, dokunulmadı (Sapma 43 düzeltmeli SHA, lokal=origin teyit) |
| `feature/wave-1-catalog` | `c83ba2a` | Lokal-only; Wave 1 handover'da listelenmemiş — fiili rev-parse (C0 Shared-subset sınır commit'i). Dokunulmadı |
| tag `wave-0-complete` | obj `40927c8` → commit `dd50923` | Intact (tag-deref disiplini) |
| tag `wave-1-complete` | obj `deaadb7` → commit `33d058d` | Intact (tag-deref disiplini) |
| tag `wave-2-complete` | (Plan-2.6'da oluşturulacak → `74550a7`) | Beklenen — push #20'de origin'e gidecek |

## Wave 2 Commit Zinciri (rebuild/v2'de, closure-scope `2910193..HEAD` = 10)

| Sıra | SHA | Tarih | Subject |
|---|---|---|---|
| — | `2910193` | 2026-05-17 | docs(wave-1): kapanis - Wave 2 devir teslim paketi *(closure anchor, scope-dışı)* |
| 1 | `9e56b61` | 2026-05-17 | chore(dx): IDE language-service cache dosyalarini gitignore'a ekle |
| 2 | `80ac049` | 2026-05-17 | chore(catalog): W2.0 Application foundation skeleton |
| 3 | `9843108` | 2026-05-17 | feat(catalog): W2.1 Categories command handlers + endpoints + tests |
| 4 | `d449a32` | 2026-05-17 | docs(wave-2): Wave 2 progress raporu - W2.0+W2.1 kapanis |
| 5 | `0948960` | 2026-05-17 | feat(catalog): W2.2 Breeds command handlers + endpoints + tests |
| 6 | `952d694` | 2026-05-17 | feat(catalog): W2.3 Brands command handlers + endpoints + tests |
| 7 | `7c7c61f` | 2026-05-18 | feat(catalog): W2.4 admin commands + RefreshRates stub |
| 8 | `d2e6208` | 2026-05-19 | feat(catalog): W2.5 CertType + BorderRule admin commands |
| 9 | `24c55da` | 2026-05-19 | feat(catalog): W2.6 admin catalog read service NotImpl stub |
| 10 | `74550a7` | 2026-05-19 | docs(deviations): Wave 2 sapma defteri + 39 distinct + retro backlog |

**Closure reconcile (W1-2):** `2910193..HEAD` = **10** · `main..HEAD` = **39** = 28 (Wave 0+1 → `33d058d` wave-1-complete) + 1 (`2910193` devir-paketi, closure-scope dışı anchor) + 10 (Wave 2 closure). `wave-1-complete` annotated tag-obj `deaadb7` ≠ commit `33d058d` (`^{commit}` deref disiplini).

## Catalog.Application Envanteri (Wave 2 sonu)

**Toplam: 115 .cs / 3216 satır / build 0 Uyarı 0 Hata (TreatWarningsAsErrors=true)**

> Stat disiplini (W1-2 + Sapma 11 emsali): ham `find -name "*.cs"` = 121 .cs / 3284 satır **obj/Debug+obj/Release generated 6 .cs / 68 satır içerir** — fiili kaynak obj/bin HARİÇ = **115 / 3216**.

| Dizin | .cs |
|---|---|
| Features/Categories | 19 |
| Features/Brands | 15 |
| Features/CertificationTypes | 12 |
| Features/Breeds | 12 |
| Features/BorderRules | 12 |
| Features/Locations | 9 |
| Features/Currencies | 8 |
| Abstractions | 6 |
| Features/Languages | 5 |
| Features/Countries | 5 |
| Features/AdminCatalogRead | 5 |
| Common | 3 |
| Common/PipelineFilters | 2 |
| Common/Mappers | 1 |
| CatalogApplicationModule.cs | 1 |
| **Toplam** | **115** |

**Test projesi (Catalog.Application.Tests):** 46 test .cs (obj hariç) · 150 `[Fact]`/`[Theory]` attribute · **runtime `dotnet test` = Başarısız 0 / Başarılı 154 / Toplam 154 (154/154 PASS, 2 s)**.

> Conflate-uyarısı (Sapma 28 dersi): grep attribute (150) ≠ runtime test-case (154); `[Theory][InlineData]` genişlemesi (+4, örn. BorderRuleKindMapperTests). **Otorite = `dotnet test` runtime 154/154**, grep değil.

**Catalog.Infrastructure:** 0 .cs (boş skeleton, yalnız csproj) — Wave 3'e DOKUNULMADI, host-inert teyit.

## NuGet Durumu (delta Wave 1 → Wave 2)

| Proje | Wave 1 handover | Wave 2 FİİLİ (csproj grep) | Delta |
|---|---|---|---|
| Shared.Kernel / Shared.Contracts.* | 0 (mutlak invariant) | (Wave 2 değişmedi) | — |
| Catalog.Domain | 1 (NetTopologySuite 2.6.0) | (Wave 2 değişmedi) | — |
| Catalog.Application | "0 (henüz iskelet)" | MassTransit `[8.*, 9.0)` + FluentValidation `12.1.1` + FluentValidation.DependencyInjectionExtensions `12.1.1` | **+3** (Wave 2 pattern netleşti: Mediator-Result + validator) |
| Catalog.Application.Tests | (Wave 1'de yoktu) | Microsoft.NET.Test.Sdk `18.5.1` + xunit `2.9.3` + xunit.runner.visualstudio `3.1.5` + FluentAssertions `[7.0.0, 8.0.0)` + NSubstitute `5.3.0` | yeni proje |
| Catalog.Infrastructure | "0 (iskelet)" | 0 .cs | dokunulmadı (Wave 3) |

> Retro 20 teyit: `FluentAssertions [7.0.0,8.0.0)` Tests.csproj'da referanslı, **kullanım 0** (tüm test native xUnit `Assert`) → Wave 3 dependency-cleanup adayı.

## Sapma Defteri Konsolide (iki katman: committed + handover post-commit)

### deviations.md (committed `74550a7`, immutable)

- **Wave 0+1+2 TOPLAM: 82 distinct Sapma** (Wave 0+1: 43 + Wave 2: 39, Sapma 44–82), **0 production sızıntısı**
- Wave 2 Aile distinct: **Aile 3 = 35** · **Aile 4 = 2** (F-S17 S77 + F-S22 S82) · **Aile 8 = 1** (YENİ KÖK, plan-fazı tip-kimliği, kök S45) · **Backend self-inflicted = 1** (S44)
- KAYDET: Wave 1 = 7-10 (deviations.md Wave 1 §) · Wave 2 = 9 / 13-25 / 29 / 30 / 31(+alt-varyant) · **boşluk-flag: 11/12/26/27/28** (fabrike YASAK, 26/27/28 bilinçli Sapma 4 emsali, 11/12 memory-lossy-only)
- Retro backlog: discrete item 13-20 + 19a/19b + Meta-1/Meta-2

### Handover post-commit reconcile (F-S23/F-S24 — committed-ledger amend YOK)

Plan-3.D Plan-1/Plan-2 esnasında yakalanan ek sapmalar; deviations.md `74550a7` commit'inden SONRA tetiklendi → **committed-ledger tek-taraflı amend YASAK** (Sapma 28/40 emsali, commit immutable). Handover-only stat:

| # | Etiket | Aile | Beklenen → Fiili | Fark | Yakalama |
|---|---|---|---|---|---|
| 83 | F-S23 | 4 (durum-farkındalığı) | `origin/rebuild/v2` = `7c7c61f` → **`d2e6208`** | Frontend memory §1 bayat "push #18 BEKLIYOR" state'ini sorgusuz aktardı; canonical state fresh-read atlandı; talimat-içi `7c7c61f`-beklentisi ↔ `ahead 2` çelişiyordu | Backend `git rev-parse`, Mustafa-öncesi, **0 risk**; memory §1 düzeltildi |
| 84 | F-S24 | 3 (gevşek aritmetik) | Plan-2 talimat "12 bölüm" → **11 bölüm** (Wave 1 handover fiili) | Frontend Plan-2 talimatında Backend Plan-1 envanteri "11 bölüm" enumerasyonu sorgusuz kabul edilmek yerine "12 bölüm" gevşek yazıldı; W1-2 fiili enumerasyon disiplini Frontend ihlali | Backend draft uygulamada 11 bölüm doğru uyguladı, dolaylı flag (draft yapısı talimat sayımına uymadı), **0 risk** |

**Handover-only TOPLAM:** Wave 2 = **41 distinct** (Sapma 44–84), Aile 3 = 36, Aile 4 = 3 (S77+S82+S83), Aile 8 = 1, Backend = 1 → **41**. Genel TOPLAM = **84**. Asimetri bilinçli: committed deviations.md `74550a7` = 82/39 (immutable), handover post-commit = 84/41 (Sapma 28/40 disiplini — commit sonrası yakalanan handover'da reconcile-not, deviations.md amend yok).

## Push Tatbikat Sayısı

- **Wave 2 push'ları:** push #18 (`d2e6208` W2.5) · push #19 (`24c55da` W2.6 atomic + `74550a7` retro — 2 commit birlikte)
- **Wave 0+1+2 kümülatif: 19 push tatbikatı** — `main` INVARIANT `44416138` HİÇ değişmedi (production safety mutlak, her push `git ls-remote` ile teyit)
- push #20 Plan-3.D'de gelecek (handover commit + `wave-2-complete` annotated tag birlikte)

## CI/CD Durumu

- Jenkins: `https://jenkins.hirovo.com` · Job: `LivestockTrading Backend Rebuild CI` · Branch: `*/rebuild/v2` · Webhook GitHub→Jenkins yeşil (Wave 1 mini-wave WAVE-1-CI-01'de düzeltildi)
- Wave 1 last successful build: `309211e` (Wave 1 mini-wave test push)
- ⚠️ **push #18 (`d2e6208`) build durumu: BİLİNMİYOR** — Backend prod-Jenkins SSH/sensitive-read YASAK (`feedback_prod_jenkins_ui_only`, Aile 5 disiplini, fabrike YASAK). **Mustafa Jenkins UI teyidi bekleniyor**
- ⚠️ **push #19 (`74550a7`) build durumu: BİLİNMİYOR** — aynı disiplin, Mustafa Jenkins UI teyidi bekleniyor
- WAVE-1-CI-01 Wave 1'de KAPANDI; Wave 2'de tetiklenmedi (webhook stabil)

## Origin Branch Durumu

- Aktif: `rebuild/v2` (HEAD `74550a7`, lokal=origin)
- **Wave 2 YENİ branch oluşturmadı** — `feedback_direct_push_main` OVERRIDE: backend rebuild boyunca direkt `rebuild/v2` (feature/wave-N-* model Wave 2'de kullanılmadı)
- **Origin temiz (9 branch):** `main`, `rebuild/v2`, `feature/wave-0-cleanup`, `feature/wave-0-infra`, `dev`, `master`, `archive/vertical-slice-migration`, `feat/mst-79-*`, `feat/mst-80-*` → **Wave 2 stale-remote branch YOK**
- Lokal-only ilgisiz (`claude/*`, `worktree-agent-*`, `feat/mst-*`, `feature/wave-1-*`): başka ajan/iş, **Wave 2 kapsamı dışı, Wave 2-tarafından-oluşturulmadı**
- WAVE-1-ARCHIVE-01 (Wave 1 stale-branch silme mini-wave) Wave 2'de tetiklenmedi (direct-push model — silinecek Wave 2 branch yok)

## Wave 3 Açık Backlog

| ID | Açıklama | Öncelik |
|---|---|---|
| WAVE-3-INFRA-EF | Catalog.Infrastructure: EF DbContext + Configurations + Repository impl (IBrandRepository, IBreedRepository.GetByCodeAsync, IBorderRuleRepository, IReferenceDataRepository) | Yüksek |
| WAVE-3-INFRA-RATE | RateProviders 3-tier (TCMB primary + ECB fallback + exchangerate.host tier-3) + RateLog entity + RateLogRepository | Yüksek |
| WAVE-3-INFRA-QUARTZ | Quartz Job (RefreshExchangeRates cron; retro 19a `days=7` default teyit/revize) | Yüksek |
| WAVE-3-INFRA-NOTIMPL | NotImpl stub replace: AdminCatalogReadService 3 metot (ListBrands/GetMissingTranslations/GetRateLogs) + RefreshExchangeRatesHandler (W2.4/W2.6 placeholder EF read ile dolacak) | Yüksek |
| WAVE-3-HOST-WIRE | LivestockTrading.Api host-wiring: MapAdminCatalogReadEndpoints + tüm aggregator Map çağrıları + AddCatalogApplication + AddCatalogInfrastructure | Yüksek |
| WAVE-3-CI-JENKINS | push #18/#19 build durumu Mustafa Jenkins UI teyidi (handover unknown — fabrike YASAK) | Yüksek |
| WAVE-3-HOST-501 | 501-mapping middleware (NotImplementedException → 501, UseExceptionHandler/IExceptionHandler — retro 19) | Orta |
| WAVE-3-RETRO-1318 | Retro 13-18: BORDER_RULE çift-RULE · CertType DELETE↔Deactivate · AttributeValueType 2.use-site extraction · stale doc-ref · GET-cursor port-mismatch · rate-logs ?cursor= mismatch — kickoff'ta değerlendir | Orta |
| WAVE-3-RETRO-19-20 | Retro 19/19a/19b (501-mapping + days=7 + param-sırası) + 20 (FluentAssertions cleanup) | Orta |
| WAVE-3-DISCIPLINE | Meta-1 (path-ezberi 3 tetik) + Meta-2 (tipografi/syntax/aritmetik/state-ezberi 6 tetik) → Wave 3 kalıcı Frontend disiplin kuralı | Yüksek |

## Wave 3 Kickoff Önerilen Adımlar

1. **Plan-doc okuma:** `_docs/decisions/05-modules/05-catalog.md` §10 (read model) + Karar 4 (EF migration) + Karar 5 (Catalog modül detay) + Karar 3a/3b/3d (event public/internal, AR boundary)
2. **Catalog.Infrastructure.csproj iskelet:** RootNamespace + NuGet (EF Core + Npgsql + EFCore.NamingConventions + NetTopologySuite + Quartz + HttpClient providers) + ProjectReference (Catalog.Domain + Catalog.Application + Shared.Contracts.Catalog)
3. **Feature folder dekompozisyonu:** Configurations/ + Repositories/ + RateProviders/ + Jobs/ + DbContext + Module class
4. **İlk sub-batch (W3.1):** Plan-doc onayı + DbContext + ilk Configurations batch (Backend kararı, en küçük başlangıç)
5. **W2 emsalini koru:** Plan kilidi (Plan-1 grep + Plan-2 yazım talimatı + DUR-1/DUR-2) → doc-literal sadakat → atomic commit per sub-batch → build yeşil her batch → DUR + Frontend onayı → next
6. **Frontend disiplin sertleştirme (Wave 3 aktif):** session-start canonical anchor + her tur fiili enumerasyon + KOD BLOK template YASAK (KAYDET-31 alt-varyant) + Meta-1/Meta-2 önleme

## Frontend Disiplin Kuralları (Wave 3 için aktif)

**Wave 1'den verbatim (1-16):**

1. Plan-first (kod/komut açık plan onayı olmadan üretilmez)
2. Adım adım (her mesaj 1 mantıksal adım + DUR)
3. 3-seviye sapma yakalama (beklenen/gerçek/fark)
4. Koşullu talimat yok
5. Prompt'a değil fiili veriye güven
6. Belirsizlikleri tahminle çözme (Backend tanı veya Mustafa kararı)
7. Mustafa'ya soru yerine Backend tanı
8. Talimat başlığı net DUR sinyali
9. Markdown güvenli format
10. Her tur Mustafa kararı + memory yeniden oku
11. AI in-band güvenlik sınırı gevşetme yasak (Aile 5)
12. Backend flag'lediği düzeltme sonraki turda taze veri olarak okunmalı
13. Doc-literal vs convention-extrapolation explicit ayrım (KAYDET-9)
14. Mevcut dosya yazımında overwrite-guard (W1-1)
15. Stat reconcile fiili kaynaktan (W1-2)
16. Branch arşivlemede iki-aşamalı hazırlık (W1-6)

**Wave 2 birikimi (17-25):**

17. KAYDET-25 — Frontend template ↔ Backend grep emsal çelişkisinde **grep otorite**
18. KAYDET-30 — Frontend kod template'lerinde fiili API specifics → Backend mikro-grep otorite
19. KAYDET-31 + alt-varyant — Frontend KOD BLOK template YASAK kapsamı (endpoint Handle body + aggregator MapXxxEndpoints body + mapper switch body + helper body + test method body; Frontend yalnız structural pattern + envanter + scope)
20. Session-start metni = canonical state anchor; her turda fresh-read, memory bayat-state ezbere YASAK (F-S23 emsali)
21. Stat reconcile fiili-kaynak protokolü: `find` ham çıktı obj/bin exclusion (Sapma 11), grep-attribute ≠ runtime test (`dotnet test` otorite), Backend lokal `git fetch` read-only çalışır (push/sensitive-op Mustafa eli)
22. Gevşek aritmetik YASAK (W1-2): "~89+", "beklenen ~150 insertion", "12 bölüm" tipi tahminler distinct ledger'a girer (F-S21/F-S22/F-S24), fiili enumerasyon zorunlu
23. Plan kapanış turlarında sub-batch yığma YASAK: Plan-3.A (atomic commit) / 3.B (retro commit) / 3.C (push) / 3.D (handover+tag+push) ayrı turlar (F-S17 emsali)
24. Committed-ledger tek-taraflı amend YASAK: post-commit yakalanan sapmalar handover'da reconcile-not (deviations.md `74550a7` immutable, F-S23/F-S24, Sapma 28/40)
25. Prod-Jenkins UI-only, Backend SSH/sensitive-read YASAK; fabrike YASAK, Mustafa UI teyidi gerekiyorsa "unknown, Mustafa teyit bekliyor" notu (Aile 5)
