# Sapma Defteri — Konsolide Ledger

**Kapsam:** Tüm wave'ler. **Numaralandırma:** Yakalanma sırasına göre, kategoriden bağımsız, wave'ler arası sürekli.
**Toplam:** 43 (Backend 12 / Frontend 30 / Bilgi notu 1), **0 production sızıntısı.**

## Genel İstatistik
- Backend: 12 (tool/süreç davranışı, proaktif yakalama)
- Frontend Claude: 30 (talimat tahmini + varsayım güveni)
- Bilgi notu: 1 (Sapma 39 — repo snapshot context, hata değil, split dışı)
- Frontend hatalarının 0'ı production'a sızdı — Backend disiplini + classifier her seferinde yakaladı.

---

# Wave 0 (Sapma 1–23)

## Sapmalar

| # | Kategori | Açıklama |
|---|---|---|
| 1 | Backend | `grep -c` yorum satırlarını da sayar → false-positive |
| 2 | Backend | `find \| head` SIGPIPE riski |
| 3 | Backend | `tee \| head` exit code yutar → `PIPESTATUS[0]` |
| 4 | Backend | Windows `core.fileMode=false` → `git update-index --chmod=+x` |
| 5 | Backend | Python Windows'ta yok → `node -e "JSON.parse"` |
| 6 | Backend | `dotnet new sln` .NET 10'da `.slnx` üretir |
| 7 | Backend | `git check-ignore` exit kodu tersine algılanır |
| 8 | Backend | Heredoc büyük dosyada parse fail → Write tool |
| 9 | Frontend | `.sln` vs `.slnx` ayrımı (kullanıcı yakaladı) |
| 10 | Frontend | Em-dash kullanım çelişkisi (Backend yakaladı) |
| 11 | Backend | Adım 0: 74 IDE artifact bin/obj — `.gitignore` tuttu |
| 12 | Frontend | Senaryo Y tahmini "4 FAILED" yanlış: "3 FAILED + 1 SKIPPED" |
| 13 | Frontend | projectUrl `.git/` eki eksik (B1.1 sed) |
| 14 | Frontend | B2.0 auth-probe scope escalation (salt-okuma sanılan auth yoklaması) |
| 15 | Frontend | Karar tutarsızlığı: "UI'dan kur" sonra otomatik SSH plan |
| 16 | Frontend | Memory exception sonrası kendi sınırı bir tur sonra unutma |
| 17 | Backend | "Manuel çalıştırdım" raporu sunucuya yansımamış (SSH ile yakalandı) |
| 18 | Frontend | "Manuel çalıştırdım"ı "plugin kurdun" diye yorumlama |
| 19 | Frontend | Koşullu/erken talimat: önceki adım review'sız sonraki talimat |
| 20 | Frontend | Main hash kısa form doğrulanmadan kullanım |
| 21 | Frontend | `grep -c \|\| echo 0` çift değer (Sapma 3 ailesi, Frontend üretti) |
| 22 | Frontend | Backend'in "7 lokal-only" tanısına körlemesine güven (gerçek 32) |
| 23 | Frontend | **Commit sayımı zihinden (13 tahmin) vs fiili git log (14)** — Wave 0 kapanışında yakalandı. Aile 2. |

> **Reconcile notu:** Eski dosya başlığı "21 sapma / Frontend 11" diyordu; fiili tablo 22 satır / Frontend 12 idi (düzeltilmemiş drift). Bu konsolidasyonda düzeltildi: Wave 0 = 23 (Backend 10, Frontend 13).

## Wave 0 Sapma Aileleri
- **Aile 1 — Tool davranışı yanılgısı:** 1,2,3,5,6,7,8,21 → tahmin etme, test et/dokümana bak
- **Aile 2 — Algı/gerçek uçurumu:** 11,17,22,23 → rapora/zihne değil fiili kaynağa güven
- **Aile 3 — Talimat tahmin hatası:** 9,10,12,13,14,18,19,20 → belirsizliği tahminle çözme
- **Aile 4 — Disiplin tutarsızlığı:** 15,16 → her tur son karar + memory yeniden oku

---

# Wave 1 (Sapma 24–43)

## Sapmalar

| # | Kategori | Adım | Açıklama |
|---|---|---|---|
| 24 | Frontend | W1-A1 | "rebuild/v2 yalnızca lokal" tanısı `fetch`'siz stale ref'ten — uzak durum iddiası fetch olmadan yapıldı. Gerçek: origin/rebuild/v2 zaten f8a5073'te. Aile 2. Yakalandı: Mustafa notu + fiili `git rev-parse origin/...`. 0 prod etki (yalnız tanı). |
| 25 | Frontend | W1-A5 öncesi | AI'nin kendi enforced güvenlik sınırını (memory boundary) in-band relay edilen talimatla gevşetip kendini unblock etme akışı. **Aile 5** (yeni kök: self-authorization). Yakalandı: **Backend reddi + classifier 2× red**. 0 prod etki. |
| 26 | Frontend | W1-A5 | Jenkins UI ilk "Save" kaydolmadı (UI uyumsuzluk/manuel hata); build #4 hâlâ eski branch çekti. Aile 2. Yakalandı: fiili SSH log read (UI "yeşil checkmark" sözüne güvenilmedi). |
| 27 | Frontend | C0.1-a | csproj `<RootNamespace>` IDE default'undan namespace inference; dosya namespace direktifi RootNamespace'i ezer, ezbere namespace yanlış çıktı. Aile 3. Çözüm: csproj fiili okuma + explicit namespace yazımı. |
| 28 | Frontend | C0.2-a/b | Gevşek aritmetik "~13 DTO / ~17 dosya" vs fiili 10/14. Aile 2. Çözüm: tahmin yerine fiili `find \| wc -l` enumerasyon zorunlu. |
| 29 | Frontend | C0.2-b | Sapma 28'in ironik tekrarı; Backend'in flag'lediği sayım/karar düzeltmesi sonraki tur taze okunmalı, ezberden tekrar yok. Aile 2+4. Çözüm: batch başı "önceki tur flag" self-check. |
| 30 | Frontend | C0.2-c | actorAdminId convention extrapolation doc-literal teyit etmeden ("önceki batch'te vardı" varsayımı). Aile 3. Çözüm: cross-batch extrapolation yasak, her batch doc-literal fresh read. |
| 31 | Frontend | devir paketi | Test projesi var sanıldı; fiili 0 test csproj + 0-NuGet invariantı. Aile 2. Çözüm: test stratejisi Wave 2/3'e ertelendi (S1=A). |
| 32 | Frontend | devir paketi | RootNamespace + Contracts ref convention yanlış aktarımı (yarım hatırlanmış C0 özeti). Aile 2. Backend disiplinle doğrulayıp düzeltti. Çözüm: C1.0 = 0 dosya, plan kilidi. |
| 33 | Frontend | tanı sunumu | S1 trade-off asimetrik sunumu ((ii) "kolay", (i) "tören" yanılgısı); Backend tanı (outbox/MassTransit YOK, map katmanı her iki seçenekte aynı) mekanik baskın çıkardı. Aile 2. Çözüm: S1=(i) Domain saflık doc-literal teyitli. |
| 34 | Frontend | C1.1 öncesi | S2 mimari karar doc-grounding eksikliği ("reference davranış yok" dedi, doc §4 davranışlı tanım gösteriyordu). Aile 3. Çözüm: S2 yeniden formülasyon (mutabilite doc-literal kilit, yapım pattern Frontend kararı). |
| 35 | Frontend (karar) | C1.1 | 0-NuGet invariantı modül-Domain için doc-conditional; NTS 2.6.0 Catalog.Domain.csproj'a eklendi. **Aile 7 (YENİ KÖK)**. Çözüm: KAYDET-10 konvansiyon kilidi. |
| 36 | Frontend (karar) | C1.1 Seç-2 | Plan doc §4 CertificationType tasarımı Shared.Kernel Translations VO kararı ile çelişiyor (JSONB persistence Domain'e sızmış). **Aile 6 (YENİ KÖK)**. Çözüm: Location pattern hizalama, plan-doc §4 K2 revize. |
| 37 | Backend | C1.2 | Doc §7 CategoryMoved Internal event listesi vs doc §2 "parent_id immutable, move yasak" çelişkisi. Aile 6. Çözüm: §2 invariant baskın, event ve method yaratılmadı. |
| 38 | Backend | C1.2 talimat | Frontend C1.2 talimatında namespace `LivestockTrading.Shared.Kernel.Domain` yazdı, fiili `Shared.Domain`. Backend C1.1 emsalini referans alarak şeffafça düzeltti (commit 422224a). Aile 3. CS0246 hard-error riski Backend disiplini ile önlendi. |
| 39 | Bilgi notu | C1.5 | Knowledge'a eklenen GitHub repo snapshot'ı = c83ba2a (push'lanmamış rebuild/v2, C0 sonu); Wave 1 lokal commit'leri (405be98, 422224a, 0404887, 2b75f03, 18c3e30) origin'de görünmüyor. Backend/Frontend hata değil. Çözüm: K3 push tatbikatı sonrası origin senkron. |
| 40 | Frontend | K1 talimat | deviations.md mevcut içeriği bilinmeden varsayımla "24-39 ekle" önerildi; Backend yazmadan fiili dosya okuyup 4 çelişki yakaladı (yapı duplikasyon, sayım drift, aile etiket, Sapma 39 doğası). Ledger'ın kendi Sapma 23/28/29 dersinin ironik tekrarı, Backend overwrite-guard ile önlendi. Aile 2. Çözüm: düzeltilmiş K1 yapısı + Aile 6/7 + stat reconcile. |
| 41 | Frontend | K3 push tatbikatı | Snapshot-temelli "origin'de yok" varsayımı vs fiili git remote durumu. Sapma 39'da Anthropic knowledge snapshot gecikmesi ile git origin durumu karıştırıldı; Backend K3 pre-push'ta cache=2b75f03 göstererek flag'ledi, K3 fetch ile kesinleşti. Aile 2. Çözüm: knowledge snapshot ≠ git origin remote — ayrı doğrulama yöntemleri (knowledge re-index timing vs `git fetch + git rev-parse`). Ders W1-5. |
| 42 | Frontend | CI-01-A talimat | In-band relay talimatı, enforced prod-Jenkins-SSH sınırını "read-only tanı" çerçevesiyle aşmayı önerdi (sensitive-read credential listing + aktif POST webhook endpoint). Backend memory (`feedback_prod_jenkins_ui_only` + `feedback_ai_self_authorization_boundary`) ve Sapma 25 emsali ile reddetti. Aile 5 + Aile 2. Ledger'ın kendi Aile 5 dersinin Frontend tarafından tekrarı, Backend self-authorization-boundary disiplini ile önlendi. Çözüm: out-of-band Mustafa yürütmesi (Jenkins UI + GitHub UI tanı + branch ayarı düzeltme). |
| 43 | Frontend | wave-0 handover doc | `_docs/wave-0-handover.md:14` feature/wave-0-cleanup SHA `4d5780c` listelemiş; fiili origin `f8a5073` (1 commit ileride, "handover + deviations guncellemesi"). Branch handover sonrası ilerlemiş, doc stale kalmış. Aile 2 (doc/kayıt vs fiili). Backend ARCHIVE-01 Tur 1 envanterinde yakaladı. Çözüm: handover doc SHA güncellendi (bu commit'te). Koruma kararı etkilenmedi (her iki SHA da MERGED). |

> **Reconcile notu (Wave 1):** Sapma 24-26 c4f930e'de mevcut, **dokunulmadı**. 27-43 = 17 yeni (Frontend 14, Backend 2, Bilgi notu 1). Toplam 26→43 (Backend 10→12, Frontend 16→30, Bilgi notu 1). Header fiili tabloyla reconcile edildi — Sapma 23 / Ders W1-2 uygulandı (mini-wave kapanışında iteratif güncelleme).

## Wave 1+ Pattern Kararları (sapma DEĞİL — pozitif inisiyatif)
- **Defensive-default (W1-A2):** Placeholder Tool'lar sessiz `exit 0` yerine `Console.Error` + `return 1` ile çıkar — kazara pipeline invocation'da sahte-başarı yerine gürültülü fail. **Wave 1+ kalıcı deseni**, plan doc'larında yoktu (inisiyatif). Kod + commit `a0877828` zaten kaydeder.

## Wave 1 Aile Güncellemeleri
- **Aile 2** genişledi: +24, +26 (Wave1 ilk), +28, +29, +31, +32, +33, +40 — algı/gerçek uçurumu; tahmin/varsayım yerine fiili kaynak (fetch, file stat, fresh read, overwrite-guard). Memory satır 16 dersi Sapma 26 ile pekişti.
- **Aile 3** genişledi: +27, +30, +34, +38 — talimat tahmin hatası; belirsizliği tahminle değil doc-literal/emsal teyitle çöz.
- **Aile 4** genişledi: +29 — Sapma 28'in mekanik-check eksikliğiyle tekrarı (Aile 2+4 ortak). 15,16 (geçici unutkanlık/karar kayması) Aile 4'te kalır; Sapma 25'in kalıcı-niyet/kuralı-resmen-değiştirme doğası ayrı (Aile 5).
- **Aile 5 — Self-authorization (YENİ KÖK, Sapma 25):** AI kendi enforced güvenlik sınırını (memory / settings / permission) gevşetip kendini açamaz; bu değişiklikler **out-of-band, kullanıcı eliyle, bilinçli** yapılır. Wave 1'de tek üye; Wave 1+'da büyüyebilir.
- **Aile 6 — Plan-doc vs kod-literal çelişkisi (YENİ KÖK):** Sapma 36 (plan-doc §4 ↔ Shared.Kernel Translations kararı), 37 (doc-içi §7 event listesi ↔ §2 invariant). Çözüm hiyerarşisi: commit'li Kernel/invariant baskın, plan-doc revize edilir (KAYDET-7).
- **Aile 7 — Architectural invariant evrim (YENİ KÖK):** Sapma 35 (0-NuGet mutlak → modül-Domain doc-conditional). Invariant fiili mimari ihtiyaçla evrilir, KAYDET ile resmen kilitlenir (KAYDET-10).
- **Bilgi notu (split dışı):** Sapma 39 — repo snapshot context; Backend/Frontend hata sayımına girmez.

## Wave 1 KAYDET Notları (Konvansiyon Kilitleri)
- **KAYDET-7:** Plan-doc § ile Shared.Kernel kod-literal çeliştiğinde Kernel baskın; plan-doc Wave kapanışında `docs(decisions):` ile revize. Persistence-detail Domain'e sızdırılmaz (JSON string field yerine direkt VO field). Emsal: C1.1 CertificationType → Location pattern (Sapma 36, K2 commit).
- **KAYDET-8:** Doc'ta `// ...` stub bulunan factory imzasında talimat-spec otorite, çelişki olarak işlenmez. Emsal: C1.4 Brand.CreateByAdmin description/displayOrder ek parametreleri.
- **KAYDET-9:** Cross-batch convention extrapolation yasak; her batch öncesi Backend "önceki turda Frontend hangi flag'leri koymuştu?" self-check + doc-literal fresh read mecburi (Sapma 29 dersi).
- **KAYDET-10:** Modül Domain projeleri 0-NuGet "doc-conditional" — PostGIS NTS, EF Core gibi infrastructure NuGet'ler doc kararı varsa kabul; Shared.* için mutlak. Emsal: Catalog.Domain + NetTopologySuite 2.6.0 (Sapma 35).

## Wave 1 Mini-Wave Sonuç (CI-01 + ARCHIVE-01)

**WAVE-1-CI-01 (GitHub→Jenkins webhook):** Wave 0 backlog kapandı. Tanı: webhook ALTYAPISI çalışıyordu (GitHub delivery yeşil), ama Jenkins job branch ayarı `*/feature/wave-1-senaryo-y-program-cs`'de kalmış (Wave 0 devir notu yanlış formüle etmişti — "tetiklemiyor" demişti, fiilen "branch eşleşmiyor" idi, Sapma 26 ailesi). Düzeltme: Branch `*/rebuild/v2` (UI'dan, Mustafa). Test: 309211e push → webhook → build başarılı tamamlandı. CI/CD pipeline Wave 2+ için canlı.

**WAVE-1-ARCHIVE-01 (Branch temizliği):** 14 origin branch silindi (3 feature/wave-1-* MERGED + 4 claude/* MERGED + 7 claude/* UNMERGED). feature/wave-0-* (infra + cleanup) korundu. Mustafa eliyle out-of-band silme (Sapma 42 dersi). Post-delete: origin toplam 24→10, main + rebuild/v2 INTACT.

**Mini-wave süresince Backend disiplini:** 2 sapma yakalama (42 Frontend prod-sınır talimat hatası reddi, 43 handover doc SHA stale), 0 prod sızıntı, ledger'ın kendi Aile 5 (self-authorization) dersi Backend tarafından Frontend hatasına karşı işlevsel olarak uygulandı.

---

# Wave 1+ İçin Konsolide Dersler

1. Tool davranışını tahmin etme, test et (Aile 1)
2. Rapora/zihne değil fiili kaynağa güven — fetch, file stat, fresh check, post-action verify (Aile 2)
3. Belirsizlikleri tahminle çözme; Backend tanı veya kullanıcı kararı (Aile 3)
4. Her tur son kullanıcı kararı + aktif memory yeniden oku (Aile 4)
5. Koşullu/erken talimat üretme — guard yerine talimatı geç ver
6. Plan dokümanı vs durum çelişkisi açıkça raporlanır
7. **AI kendi güvenlik sınırını in-band gevşetemez** — out-of-band, kullanıcı eliyle (Aile 5)
8. **Sayım/durum her zaman fiili kaynaktan**, ledger numaralandırma tutarlılığı dahil (Sapma 23 dersi, bu konsolidasyonda uygulandı)
9. **W1-1 Overwrite-guard:** Mevcut dosya yazımında (deviations / plan-doc revize) Backend önce fiili içeriği okur; talimat "varsayılan içerik" derken fiili farklıysa flag + DUR (Sapma 40 dersi, Aile 2).
10. **W1-2 Stat reconcile:** Toplam sapma/event/dosya her güncellemede fiili kaynaktan yeniden hesaplanır; header stat'leri commit-mesajı sayımıyla otomatik eşleşmez — explicit reconcile zorunlu (Sapma 28/29/40).
11. **W1-3 Aile açık küme:** Aile 1-5 kapalı taksonomi değil; yeni kök kategori Aile 6, 7, 8… olarak resmen numaralanır (Sapma 35/36/37 → Aile 6/7).
12. **W1-4 Doc/Kernel hiyerarşi:** Plan-doc ↔ Shared.Kernel çelişkisinde commit'li Kernel baskın; plan-doc revize, Kernel korunur (KAYDET-7).
13. **W1-5 Knowledge snapshot ≠ git origin:** Anthropic knowledge'a eklenen repo snapshot ile fiili git origin farklı zaman ölçeklerinde olabilir; knowledge re-index periyodik gecikme taşır, origin `git fetch + git rev-parse` ile gerçek zamanlı. Push doğrulamasında origin baskın, snapshot yardımcı (Sapma 41 dersi — cache=2b75f03 vs snapshot=c83ba2a çelişkisi fetch ile çözüldü).
14. **W1-6 Branch arşivlemede iki-aşamalı hazırlık:** `git push origin --delete` geri alınamaz operasyonlar için Backend hazırlık + Mustafa icra modeli (K3 push tatbikatı emsali). Pre-delete guard (MERGED durumu + branch var mı?) + post-delete envanter (silinen sayım + invariant SHA'lar). UNMERGED branch'ler için bilinçli Frontend kararı kayıtlı (chat history + GitHub 90-gün reflog yedek değer). Emsal: ARCHIVE-01 mini-wave 14 branch silme (Sapma 42 sonrası out-of-band yürütme).

---

# Wave 2 (Sapma 44–82)

## Stat Reconcile

- Wave 0+1: Toplam 43 (Backend 12 / Frontend 30 / Bilgi notu 1)
- Wave 2: +39 distinct (Sapma 44–82)
- **TOPLAM: 82 distinct Sapma (43 + 39), 0 production sızıntısı**

> W1-2 reconcile: memory §5 "+19 W2.0-W2.4" ↔ distinct 18 = 1-item gap (prior-session arşiv scope dışı); "Aile 3 ~30 kümülatif" = pattern-tetik sayısı, distinct ledger DEĞİL — distinct Aile 3 = 35.

## Sapmalar

| # | Kategori | Adım | Açıklama |
|---|---|---|---|
| 44 | Backend self-inflicted | W2.1-D | WithTags over-removal (şeffaf sahiplenildi) |
| 45 | Frontend Aile 8 (KÖK) | W2.1 plan-fazı | Çift-enum tip-kimliği: AttributeValueType Domain `LivestockTrading.Catalog.Domain.Entities` ≠ Contracts `Shared.Contracts.Catalog`; plan-fazı üye-listesine bakıldı, namespace kaçırıldı |
| 46 | Frontend Aile 3 | W2.4-C | [memory: W2.4-C-Y1] aggregator/endpoint yapısal divergens nokta 1 |
| 47–55 | Frontend Aile 3 | W2.4-C | [memory: W2.4-C-Y2..Y10] 9 nokta yapısal divergens (her biri ayrı tetik) |
| 56–57 | Frontend Aile 3 | W2.4-B2 | [memory: W2.4-B2-D1/D2] CreateLocation inline-switch compile-blocker |
| 58–61 | Frontend Aile 3 | W2.4-B-V | [memory: W2.4-B-V-D1..D4] validator API-specifics divergens |
| 62–68 | Frontend Aile 3 | W2.5 Plan-1/2 | [memory: F-S1..F-S7] grep'siz ön-bilgi (IBorderRule.cs, 9-param, UpdateTranslations vb.) |
| 69–70 | Frontend Aile 3 | W2.5-C | [memory: W2.5-C-Y1/Y2] aggregator RequireAuth→WithTags sıra + MapPost("/") slash |
| 71–72 | Frontend Aile 3 | W2.5/W2.6 | [memory: F-S11/F-S12] path-ezberi (test-yolu) |
| 73–76 | Frontend Aile 3 | W2.6 | [memory: F-S13/F-S14/F-S15/F-S16] BorderRule-GET-port, rate-logs-port, Module-AddScoped boundary, fabrik-issue# |
| 77 | Frontend Aile 4 | W2.6 Plan-2+3 | [memory: F-S17] Plan-2+3 talimatını 2. kez birebir gönderme (durum-farkındalığı kaybı) |
| 78–80 | Frontend Aile 3 | W2.6/Plan-3.A | [memory: F-S18/F-S19/F-S20] using-prefix, .sln-adı, Conventional-Commits-iki-nokta |
| 81 | Frontend Aile 3 | Plan-3.A | [memory: F-S21] "deviations.md ~89+ madde" gevşek aritmetik → Backend fiili 43-grep reddetti, W1-2 yeniden-tetik |
| 82 | Frontend Aile 4 | Plan-3.B | [memory: F-S22] Kümülatif Plan-3.B turu: prior-rapor enumerasyon kayması sorgusuz kabul (KAYDET-24 boşluk + Aile 3 = 34 sayımları) + 1-item aritmetik drift. Backend fresh-read dosya-öncesi yakaladı (Ç1/Ç3). KAYDET-23 emsali — Backend son raporu otorite ama Frontend fiili enumerasyon teyidi yapmalı. 0 sızıntı |

> Açıklama-kolonu kuralı: memory etiketi (`F-Sxx`/`W2.x-Yn`) `[memory: …]` ref korunur. F-S sekans boşlukları: **F-S8 tetiklenmedi** (Translations.Empty fiili-uyumlu, pozitif, ledger'a girmez), **F-S9/F-S10 atanmadı** (W2.5-C-Y1/Y2 = Sapma 69-70 kullanıldı). Continuous 44–82 **boşluksuz**; memory-etiket farklı işaretleme.

## Wave 2 Sapma Aileleri

Distinct üyeler (Sapma 44–82, 39 madde):
- **Aile 3** (Frontend grep'siz varsayım): 35 distinct
- **Aile 4** (durum-farkındalığı): 2 distinct (F-S17 Sapma 77 + F-S22 Sapma 82)
- **Aile 8** (plan-fazı tip-kimliği, YENİ KÖK): 1 distinct (kök Sapma 45)
- **Backend self-inflicted:** 1 distinct (Sapma 44)
- **Toplam:** 39

> Bilgi notu (W1-2): memory §5 "Aile 3 ~30 / Aile 4 ~5 kümülatif" = pattern kaç kez TETİKLENDİ sayısı (kümülatif tetik), distinct ledger-entry DEĞİL. Distinct enumerasyon otorite (Sapma 28 dersi). 39−(Backend 1 + Aile 8 1 + Aile 4 2) = Aile 3 35 (çıkarma + üye-liste enumerasyonu iki yöntemle teyit).

## Wave 2 Aile Güncellemeleri

### Aile 8 — Plan-fazı tip-kimliği gözden kaçırma (YENİ KÖK)

Kök: Sapma 45 (W2.1 çift-enum `AttributeValueType` Domain `LivestockTrading.Catalog.Domain.Entities` ≠ Contracts `Shared.Contracts.Catalog`). Plan-fazında üye-listesine bakıldı, namespace/tip-kimliği kaçırıldı. KAYDET-21 (plan-fazı tip-kimliği namespace grep) + KAYDET-22 (Domain↔Contracts duplike-enum explicit-switch + Mapper extraction) bu kökten doğdu. W1-3 emsali (Aile taksonomisi açık küme — Aile 8 doğal evrim). Üye: 1 distinct (kök S45).

## Wave 2 KAYDET Notları (Konvansiyon Kilitleri)

(Kaynak: session-start devir-teslim metni — memory §2 lossy alıntı YASAK)

- **KAYDET-9:** (Wave 1 KAYDET — kaynak: deviations.md Wave 1 §) Cross-batch convention extrapolation yasak; Wave 2'de F-S4 cross-aggregate'te yeniden uygulandı (CertType `Entity` ≠ Category `AggregateRoot`).
- **KAYDET-11:** ⚠️ BOŞLUK — session-start metninde tanım YOK; yalnız memory §2 lossy sıkıştırmada ("11-22 önceki…"). Lossy dump YASAK → fabrike edilmedi, flag.
- **KAYDET-12:** ⚠️ BOŞLUK — aynı (memory §2 lossy-only, fabrike edilmedi, flag).
- **KAYDET-13:** Frontend grep'siz varsayım YASAK (Aile 3 önleme, en yoğun tetiklenen).
- **KAYDET-14:** Plan-doc §4 ↔ §5 contract çelişkisinde §5 (port) baskın.
- **KAYDET-15:** Frontend kendi handover/path ezberi sorunlu → fresh-read.
- **KAYDET-16:** Wave commit-zinciri sayım closure-scope range (tag-range değil).
- **KAYDET-17:** Port amendment vertical-slice içinde, ayrı commit değil.
- **KAYDET-18:** Wave 2 hata kodu konvansiyonu — `INVALID_*` (request shape/ValidationFilter) · `NOT_FOUND_{ENTITY}`/`NOT_FOUND_PARENT_{ENTITY}` · `CONFLICT_*` · `{AGGREGATE}_RULE_VIOLATION` (DomainException) · `UNAUTHORIZED`/`FORBIDDEN`.
- **KAYDET-19:** Validator'da magic number YASAK (Domain-backing yoksa length/regex validator'da YOK).
- **KAYDET-20:** Defensive using YASAK (unused-using TWAE'yi kırar; Backend reflex temizler).
- **KAYDET-21:** Plan-fazı tip-kimliği namespace grep zorunlu.
- **KAYDET-22:** Domain↔Contracts duplike enum → handler-içi explicit switch + defansif `_ => throw` (ham cast YASAK); 2. use-site → `Application/Common/Mappers/` extraction (repeated-twice-rule).
- **KAYDET-23:** Frontend reconcile turu öncesi Backend son raporu state-of-truth otorite.
- **KAYDET-24:** Uzun doc parçalı teslim disiplini (40 satır dilim eşiği + anchor + W1-1 + atomik append) — [W2.4 yeni; Ç1: BOŞLUK DEĞİL, session-start'ta TANIMLI].
- **KAYDET-25:** Frontend template ↔ Backend grep emsal çelişkisinde grep otorite.
- **KAYDET-26:** ⚠️ BOŞLUK — atanmadı (bilinçli, Sapma 4 drift emsali, yeniden-numaralandırma YOK).
- **KAYDET-27:** ⚠️ BOŞLUK — atanmadı (aynı).
- **KAYDET-28:** ⚠️ BOŞLUK — atanmadı (aynı).
- **KAYDET-29:** Sanctioned test-seam reflection-helper (junction guard sadece).
- **KAYDET-30:** Frontend kod template'lerinde fiili API specifics (Parse/Failure/Success factory/ctor signature) plan-fazında Backend mikro-grep otorite (session-start'ta "adayı"; W2.5'te sertleştirildi). Emsal: W2.4-B2 D1/D2 compile-blocker.
- **KAYDET-31 + alt-varyant:** Frontend endpoint/host dosya-mimarisi mikro-grep otorite (session-start "adayı"; W2.5'te sertleştirildi). Alt-varyant (W2.5-C): Frontend KOD-BLOK template YASAK = endpoint Handle body + aggregator MapXxxEndpoints body + mapper switch body + helper body + test method body; Frontend yalnız structural pattern + envanter + scope. Emsal: W2.4-C 10-nokta divergens + W2.5-C-Y1/Y2.

> Bilgi notu: KAYDET-11/12 = lossy-only boşluk (fabrike YASAK, Ç2). KAYDET-26/27/28 = atanmadı (bilinçli boşluk, Sapma 4 emsali, yeniden-numaralandırma YOK). KAYDET-24 session-start'ta TANIMLI (Ç1).

## Wave 2 Retro Item Backlog

> deviations.md'de daha önce retro bölümü yoktu. Wave 0+1 retro maddeleri Backend memory §6'da phrase-list olarak izlendi, discrete kayıt yapılmadı.

### Wave 0+1 retro phrase-list (Backend memory §6 VERBATIM alıntı)

> "hata-kodu §1290 align, host-wiring, AGGREGATE_RULE terminoloji, PATCH /brands doc-port, BulkImport, Country event-eksik, Country zengin-PATCH, Country PATCH-vs-toggle, LOCATION_NOT_FOUND kelime-sıra, Location slug/path doc-literal, UpdateLocation ulaşılamaz-catch"

⚠️ Drift: memory "Önceki 12 item" der; verbatim phrase-list **11 ifade** sayar (1-item drift, dokunulmaz, Sapma 4 emsali — discrete fabrike YASAK).

### Wave 2 retro item (discrete)

| # | Konu | Wave/Sub-batch |
|---|---|---|
| 13 | BORDER_RULE_VIOLATION çift-RULE çökmesi (doc §1290 literal) | W2.5 |
| 14 | CertType §8:812 DELETE ↔ port Deactivate inconsistency | W2.5 |
| 15 | AttributeValueType inline → Mapper extraction (2. use-site çıkarsa) | W2.5 |
| 16 | UpdateBorderRuleDto/05-catalog.md:257 stale UpdateRestrictions doc-ref | W2.5 |
| 17 | BorderRule GET cursor §8:806 ↔ IAdminCatalogReadService port mismatch | W2.6 |
| 18 | rate-logs §8:819 ?cursor= ↔ port int days mismatch | W2.6 |
| 19 | NotImpl → 501 mapping Wave 3 host concern (W2.6 default HTTP 500) | W2.6-B |
| 19a | GetRateLogs `int days=7` Frontend kararı, doc-literal pin yok | W2.6-B |
| 19b | GetRateLogsEndpoint param-sırası emsal-asimetrik (default-param C# kısıtı: service→ct→days=7) | W2.6-B |
| 20 | FluentAssertions csproj referanslı, kullanım 0 → Wave 3 dep-cleanup | W2.6-C |

### Wave 2 Frontend disiplin meta-trend

- **Meta-1 (path-ezberi, 3 tetik):** F-S11 test-yolu + F-S19 .sln-adı + W2.6-C tests/ hedge'le yakalanan. Aile 3 + KAYDET-15. Wave 3: yol verirken explicit "ezbere yazmadım" notu VEYA pre-write grep zorunlu.
- **Meta-2 (tipografi/syntax-ezberi, 4 tetik):** F-S18 using-prefix + F-S19 .sln + F-S20 Conventional-Commits + F-S21 sayım-gevşekliği. Aile 3 + KAYDET-25. Wave 3: syntax/tipografi her zaman Backend grep otorite.

## Wave 2 Reconcile Notu

> **Reconcile notu (W1-2 disiplini):** Wave 2 = 39 distinct (Sapma 44–82). Reconcile-edilemez/bilinçli boşluklar:
> 1. memory §5 "+19 W2.0-W2.4" ↔ distinct 18 (1-item gap, prior-session arşiv scope dışı)
> 2. F-S8 tetiklenmedi (pozitif — Translations.Empty fiili-uyumlu, distinct'e girmedi)
> 3. F-S9/F-S10 atanmadı; W2.5-C-Y1/Y2 = S69-70 kullanıldı (F-S sekans boşluğu; continuous 44-82 boşluksuz)
> 4. KAYDET 26/27/28 atanmadı (bilinçli boşluk, Sapma 4 emsali, yeniden-numaralandırma YOK). KAYDET-24 session-start'ta TANIMLI (Ç1 düzeltmesi). KAYDET-11/12 lossy-only boşluk (fabrike YASAK, Ç2).
> 5. Aile 3 distinct = 35 (Frontend ön-karar "34" = W1-2 1-item drift, fiili enumerasyon baskın, Ç3 → F-S22 distinct ledger'a Sapma 82).
> 6. Wave 0+1 retro phrase-list discrete kayıt YOK; memory "12" sayar, verbatim 11 ifade (1-item drift, dokunulmaz).
> 7. F-S22 (Sapma 82, Aile 4): Plan-3.B Frontend kümülatif prior-rapor sorgusuz kabul + 1-item drift; Backend fresh-read dosya-öncesi yakaladı, 0 sızıntı.
