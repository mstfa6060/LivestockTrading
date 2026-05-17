# Wave 1 → Wave 2 Devir Teslim

**Durum:** Wave 1 KAPANDI + mini-wave (CI-01 + ARCHIVE-01) tamam, rebuild/v2 = 33d058d
**Tarih:** 2026-05-17
**Sonraki:** Wave 2 (Catalog.Application — feature handlers + validators + endpoints)

## Repo Durumu

| Branch | SHA | Durum |
|---|---|---|
| `main` | `4441613...` | ArfBlocks production, Wave 0+1 boyunca DOKUNULMADI (10 push tatbikatı) |
| `rebuild/v2` | `33d058d...` | Wave 0+1 toplam ahead 28; lokal=origin |
| `feature/wave-0-infra` | `cc8d51b...` | Wave 0 isi origin (Wave 0 doc gereği korundu) |
| `feature/wave-0-cleanup` | `f8a5073...` | C-serisi origin (Wave 0 doc gereği korundu — SHA Sapma 43 ile düzeltildi) |
| tag `wave-0-complete` | → `dd50923` | Wave 0 isi bitisi |
| tag `wave-1-complete` | → `33d058d` | Wave 1 + mini-wave bitisi (YENİ) |

## Wave 1 Commit Zinciri (rebuild/v2'de, main üstüne 28 commit toplam)

### C0 Shared Subset (Wave 1 başında, c83ba2a)

C0 hazırlık commit'i Wave 0 ile Wave 1 sınırında:
- Shared.Kernel kurulumu (AggregateRoot, Entity, Result, Translations, CountryCode, LanguageCode, SlugHelper, vb.)
- Shared.Contracts.Catalog subset (DTO'lar + interface'ler)

### Wave 1 Ana İş (C1, 5 atomic commit)

- 405be98 feat(catalog) C1.1 reference entities (5 entity)
- 422224a feat(catalog) C1.2 Category + CategoryAttribute + 4 event
- 0404887 feat(catalog) C1.3 Breed + 3 event
- 2b75f03 feat(catalog) C1.4 Brand + BrandCategory + 5 event
- 18c3e30 feat(catalog) C1.5 BorderRule (Faz 2 schema-ready)

### Wave 1 Doc + Reconcile (4 commit)

- a19a280 docs(deviations) Wave 1 genişletme 27-40 + KAYDET 7-10
- 946ba0f docs(decisions) §4 CertificationType revize (Sapma 36)
- 3cf8961 docs(deviations) Sapma 41 + ders W1-5
- 8dc36bf docs(deviations) L79 reconcile (Sapma 41 sonrası iteratif)

### Wave 1 Mini-Wave (2 commit)

- 309211e test(ci) WAVE-1-CI-01 webhook tetikleme doğrulama
- 33d058d docs(wave-1-mini) CI-01 + ARCHIVE-01 kapanış (Sapma 42-43 + W1-6)

## Catalog.Domain Envanteri (Wave 2 başlangıç noktası)

**Toplam:** 23 .cs / 1181 satır / 0 Uyarı 0 Hata (TreatWarningsAsErrors=true)

| Konsept | Sayı | Konum |
|---|---|---|
| Aggregate Root | 4 | Aggregates/ |
| Reference Entity (AR DEĞİL) | 5 | Entities/ |
| Child Entity | 2 | Entities/ |
| Domain Event Public | 7 | Events/Public/ |
| Domain Event Internal | 5 | Events/Internal/ |

**AR detayları:**
- Category: sealed, INT PK, factory CreateTopLevel/CreateSubcategory, immutable Code+ParentId+Level, tree depth max 2
- Breed: sealed, INT PK, factory Create, immutable Code+CategoryId, OriginCountryCode plain string?
- Brand: sealed, Guid PK (v7), factory SuggestBySeller/CreateByAdmin, 4-transition state machine (BrandStatus), CountryCode? VO
- BorderRule: sealed, Guid PK, factory Create, Faz 1 admin manuel, Faz 2 event aktive (currently event YOK)

**Reference Entity detayları:**
- Country/Currency/Language/CertificationType (basit kod tablosu, public ctor, private set, davranış method'ları)
- Location: 5-level hierarchy, NTS Point Centroid (Faz 1 dahil), LocationLevel enum dosya-içi

**Child Entity detayları:**
- CategoryAttribute: Guid PK, internal ctor (Category.AddAttribute'tan), AttributeValueType enum dosya-içi
- BrandCategory: composite PK (Guid BrandId + int CategoryId), IdentityValue ValueTuple

## NuGet Durumu

| Proje | NuGet | Sebep |
|---|---|---|
| Shared.Kernel | 0 | Mutlak invariant |
| Shared.Contracts.* | 0 | Mutlak invariant |
| Catalog.Domain | 1 (NetTopologySuite 2.6.0) | Location.Centroid (KAYDET-10 doc-conditional) |
| Catalog.Application | 0 (henüz iskelet) | Wave 2'de pattern netleşecek |
| Catalog.Infrastructure | 0 (henüz iskelet) | Wave 3'te EF Core + diğerleri eklenir |

## Sapma Defteri Konsolide

`_docs/deviations.md` Wave 0+1 toplam 43 sapma, 0 production sızıntısı.

**Wave 1 (Sapma 24-43):**
- Backend yakalama: 2 (Sapma 37 doc çelişkisi, Sapma 38 namespace literal)
- Frontend yakalama: 17 (talimat tahmin/varsayım hataları, Sapma 25 + 42 self-authorization sınır)
- Bilgi notu: 1 (Sapma 39 knowledge snapshot)

**Konvensiyon kilitleri Wave 1 (KAYDET 7-10):**
- 7: Plan-doc ↔ Kernel çelişkisinde Kernel baskın
- 8: Doc'ta `// ...` stub ise talimat-spec otorite
- 9: Cross-batch convention extrapolation yasak (doc-literal fresh read)
- 10: Modül Domain 0-NuGet doc-conditional

**Yeni sapma aileleri Wave 1:**
- Aile 6: Plan-doc vs kod-literal çelişkisi (üye: 36, 37)
- Aile 7: Architectural invariant evrim (üye: 35)

**Wave 1 dersler (W1-1..W1-6):**
- W1-1: Overwrite-guard zorunlu (mevcut dosya yazımında fiili içerik okuma)
- W1-2: Stat reconcile fiili kaynaktan (header sayım hesabı her güncellemede)
- W1-3: Aile açık küme (yeni kök kategoriler numaralı)
- W1-4: Doc/Kernel hiyerarşi (Kernel baskın, doc revize)
- W1-5: Knowledge snapshot ≠ git origin (ayrı doğrulama yöntemleri)
- W1-6: Branch arşivlemede iki-aşamalı hazırlık (Backend hazırlık + Mustafa icra)

## Wave 1 Push Tatbikat Sayısı

10 push tatbikatı tamamlandı, main her seferinde INTACT (4441613):
- Wave 0+C0 push'ları (Wave 0 handover'da kayıtlı)
- Wave 1 Catalog.Domain ana iş push'u (2b75f03 vs sonra)
- Wave 1 K1+K2 docs push'u (8dc36bf + 946ba0f vs sonra)
- Wave 1 mini-wave öncesi düzeltme push (eksik 3 commit fast-forward)
- Wave 1 mini-wave test push (309211e CI tetikleme)
- Wave 1 mini-wave kapanış push (33d058d)

## CI/CD Durumu

- Jenkins: `https://jenkins.hirovo.com`
- Job: `LivestockTrading Backend Rebuild CI`
- Branch: `*/rebuild/v2` (Wave 1 mini-wave'de düzeltildi)
- Webhook: GitHub → Jenkins çalışır halde (delivery yeşil)
- Last successful build: 309211e (Wave 1 mini-wave test push)
- Pipeline: agent any + `docker run mcr.microsoft.com/dotnet/sdk:10.0` + dotnet restore/build/format
- Backlog WAVE-1-CI-01: ✓ KAPALI (Wave 0 yarım kalan iş tamamlandı)

## Origin Branch Durumu (ARCHIVE-01 sonrası)

Toplam 10 branch:
- **Aktif:** main, rebuild/v2
- **Wave 0 koruma:** feature/wave-0-infra, feature/wave-0-cleanup
- **Wave kapsamı dışı (dokunulmadı):** dev, master, archive/vertical-slice-migration, feat/mst-79-*, feat/mst-80-*

ARCHIVE-01 mini-wave'inde 14 branch silindi:
- 3 feature/wave-1-* (MERGED)
- 4 claude/* MERGED
- 7 claude/* UNMERGED (Frontend kararı, chat history + GitHub reflog yedek değer)

## Wave 2 İçin Açık Backlog

| ID | Açıklama | Öncelik |
|---|---|---|
| WAVE-2-PROJ | Catalog.Application proje iskeleti + NuGet kararları (MediatR, FluentValidation vb.) | Yüksek |
| WAVE-2-FEAT | Feature folder yapısı (Categories/, Breeds/, Brands/, BorderRules/, Locations/ + admin sub-folders) | Yüksek |
| WAVE-2-IMPL | IAdminCatalogCommands + ICatalogReadService implementation (handlers + validators + projections) | Yüksek |
| WAVE-2-CACHE | Redis cache layer (Karar 5 — TTL 1dk-1h per data type) | Orta |
| WAVE-2-EVT | Domain event handler'lar (in-process MassTransit.Mediator, Karar 3b) | Orta |
| WAVE-2-RATE | Currency rate provider (TCMB primary + ECB fallback + exchangerate.host tier 3) | Orta |
| WAVE-2-BULK | Brand BulkImport CSV/JSON | Düşük |

## Wave 2 Kickoff Önerilen Adımlar

1. **Plan-doc okuma:** `_docs/decisions/05-modules/05-catalog.md` §5 (Admin endpoints) + §6 (Read model) + `_docs/decisions/06-api-contract.md` Catalog section. Karar 3a/3b/3d (event public/internal ayrımı, AR boundary).
2. **Catalog.Application.csproj iskelet:** RootNamespace, NuGet kararları (MediatR + FluentValidation muhtemel), ProjectReference (Catalog.Domain + Shared.Contracts.Catalog).
3. **Feature folder dekompozisyonu:** Plan-doc okuma sonrası sub-batch dekompozisyonu (muhtemelen aggregate-bazlı: Categories/, Breeds/, Brands/, BorderRules/, Locations/, ReadModel/).
4. **İlk sub-batch (W2.1):** Plan-doc onayı + en küçük feature (örn. Country toggle-active veya Category create — Backend kararı).
5. **C1 emsalini koru:** Plan kilidi → doc-literal sadakat → atomic commit per sub-batch → build yeşil her batch → DUR sonrası Frontend onayı → next sub-batch.

## Wave 1 Boyunca Frontend Disiplin Kuralları (Wave 2 için aktif)

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
