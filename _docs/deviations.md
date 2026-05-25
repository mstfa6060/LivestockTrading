# Sapma Defteri — Konsolide Ledger

**Kapsam:** Tüm wave'ler. **Numaralandırma:** Yakalanma sırasına göre, kategoriden bağımsız, wave'ler arası sürekli.
**Toplam:** 133 (Backend 18 / Frontend 114 / Bilgi notu 1), **0 production sızıntısı.**

## Genel İstatistik
- Backend: 18 (tool/süreç davranışı, proaktif yakalama; Wave 3 W3.6.B-W3.7 +3 Aile 6 plan-doc vs fiili kod)
- Frontend Claude: 114 (talimat tahmini + varsayım güveni; Wave 3 W3.6.B-W3.7 +42 Aile 3 KAYDET-32 ana kategori)
- Bilgi notu: 1 (Sapma 39 — repo snapshot context, hata değil, split dışı)
- Frontend hatalarının 0'ı production'a sızdı — Backend disiplini + classifier her seferinde yakaladı.
- **Wave 3 RESMEN KAPANIS:** 13/13 sub-batch, 36 push tatbikati, main INVARIANT 44416138 korundu, 0 production sızıntısı. KAYDET-32 ledger 22 sistemik tezahur, KAYDET-33 + KAYDET-34 yeni formal kayıt.

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

---

# Wave 3 (Sapma 83–..., devam ediyor)

> **Wave 3 ledger açılış commit'i:** W3.6.A sub-batch (Catalog.Infrastructure Cache Decorator, 3 atomic commit: Blok-A `fb0426b` + W3.6.A.1.5 `c72d97c` + Blok-B `712b270`). Wave 3'ün W3.0–W3.5B sub-batch'leri sırasında yakalanan sapma ve pozitif önleme'ler **handover-only ledger F-S23–F-S40** olarak `_docs/wave-3-handover-mid.md`'de izlendi; distinct `_docs/deviations.md` ledger entry'lerine **Wave 3 sonu reconcile turunda** işlenecek (memory plan: `wave3_plan1_decisions.md`).

## Wave 3 Stat Reconcile (W3.6.A açılış sonrası, K2-pre amend dahil)

- Wave 0+1+2 baseline: 82 distinct Sapma (43 + 39, satır 134–136 Wave 2 reconcile)
- Wave 3 W3.6.A: +6 distinct Sapma (Sapma 83–88, memory etiket F-S51–F-S56)
- Toplam (Wave 3 W3.6.A K2-pre amend sonrası): **88 distinct Sapma**

## Wave 3 W3.6.A Sapmaları

| # | Taraf | Konum | Açıklama |
|---|---|---|---|
| 83 | Frontend | W3.6.A Blok-A G1 | (F-S51) Translations sealed class Pure POCO Domain saf (`Translations.cs:5` yorum "STJ attribute/converter YOK"), parameterless ctor + setter + `[JsonConstructor]` YOK — System.Text.Json default policy ile deserialize edilemez. Etkilenen DTO: CategoryDto.Name/Description, BreedDto.Name/Description, CertificationTypeDto.NameTranslations/DescriptionTranslations. W3.5A cache foundation scope'u value tipi tüketim tarafını gündeme almadı; W3.6.A başında Frontend talimatı "Translations parameterless ctor + setter VAR ise OK; YOKSA DUR" guard koydu (proaktif W1-1 emsali), Backend Gate 1 fresh-check ile yakaladı. Aile 3 + Aile 6 + pozitif önleme. Çözüm: W3.6.A.1.5 sub-batch'inde `JsonConverter<Translations>` Infrastructure tarafında (`Catalog.Infrastructure/Caching/JsonConverters/`), RedisCacheService static `JsonSerializerOptions` field'a register. Domain Pure POCO korundu (KAYDET-7/W1-4 Kernel baskın, Infrastructure çözüm üretir). Memory cache etkilenmez (direct ref store). Geri sarma maliyeti 0, Redis prod env runtime crash önlendi. |
| 84 | Backend | W3.6.A Adım 1 rapor | (F-S52) Adım 1 raporunda "Catalog.Application 115 .cs / 3216 satır / 154/154 test" memory ezberinden alıntılandı; fiili count Adım 1'de doğrulanmadı. Blok-A G8 fiili `dotnet test` çalıştırmasında 151/151 ortaya çıktı (3 test'lik delta — pre-existing W3.5B.1 commit `ec84f70` "application admin read stub + test sil" kaynaklı, sayım drift Adım 1'de yakalanmayıp Blok-A G8'de yakalandı). Aile 2 (algı/gerçek uçurumu) + KAYDET-9 ihlali (ezber sayım). Çözüm: Blok-B G1'de 5 DTO fresh-read disipliniyle KAYDET-9 öğrenimi uygulandı, hata tekrar etmedi. Fiili kaynak >> memory ezber kuralı pekişti. |
| 85 | Frontend | W3.6.A Adım 2 mapping tablosu | (F-S53) Backend Adım 2 raporunda list naming compound (`breed-list:{x}`) vs segmented (`country:list:active`) tutarsızlığı Sa5 olarak flag etti — Frontend talimat-örneği `breed-list:{categoryCode}` compound önermişti, ama country için segmented yazılmıştı. Frontend Adım 2 review'de Backend Sa5'i kabul etti ve segmented pattern'i kanonical kıldı (`livestock:catalog:breed:list:by-category:{categoryCode}`). Aile 4 (talimat tutarsızlığı, Frontend self-correction) + pozitif önleme (Blok-B impl'den ÖNCE yakalama). Çözüm: mapping #14 segmented olarak revize, Blok-B'de uygulanıp 22/22 metot complete. Glob-friendly prefix scan Faz 2 event-invalidation hazırlığına simetri kazandırdı. |
| 86 | Backend | W3.6.A Adım 2 BrandDto tahmin | (F-S54) Adım 2 raporunda BrandDto.OriginCountry için "tip string? mi CountryCode? mi belirsiz, Blok-B G1'e ertelenecek" diye flag etti, field adını "OriginCountry" olarak ezberden varsaydı. Blok-B G1 fresh-read'de gerçek field adı `OriginCountryCode` (suffix farkı), tip `string?` plain (CountryCode VO değil) — tahminin tip kısmı doğru, adlandırma drift'i. KAYDET-9 hafif ihlali (ezber adlandırma). Cache key Guid-tabanlı (`brand:{brandId}`) olduğundan runtime etkisi sıfır. Çözüm: G1 fresh-read'de adlandırma teyit, Blok-B impl raw passthrough'tan cache-aside'a refactor sırasında sorun çıkmadı. Adlandırma drift'i ön-flag'in varlığı sayesinde yakalandı (pozitif önleme yan-not). |
| 87 | Frontend | W3.6.A-K1 talimat | (F-S55) Frontend K1 talimatında ezberden "F-S51–F-S54 ekle" diyerek deviations.md fiili continuous Sapma numarasını (82) ve F-S memory etiket numarasını (F-S22, Wave 2 son) okumadan numara verdi. Memory'deki "F-S41–F-S50 (16 talimat-pattern defekt)" referansı handover-only Wave 3 mid F-S23–F-S40 + 10 yeni şeklindeydi, distinct deviations.md ledger F-S22'de bitiyordu. Backend G1 fresh-check ile yakalamadan yazsaydı: (i) Sapma 51 zaten Wave 1 Sapma 35 ile çakışırdı (numara broken), (ii) header sayım "43" kalırdı (katlanan W1-2 ihlali), (iii) Wave 3 baş bölümü açılmamış olurdu. Aile 3 (talimat tahmin hatası) + KAYDET-9 ihlali (Frontend tarafı, Sapma 84 [yeni F-S52, eski W3.6.A Adım 1 sayım drift dersi] çapraz uygulamasının ters yönde Frontend tekrarı) + pozitif önleme (Backend Sapma 84 dersi Frontend ezberine çapraz uygulama). Çözüm: K1 talimatı Frontend tarafından revize, continuous Sapma 83–87 + memory etiket (ilk yazımda F-S41–F-S45 → K2-pre amend ile F-S51–F-S55 rename, Sapma 88 / F-S56 kapsamı) seri devam, Wave 3 baş bölümü K1'de açıldı (Stat Reconcile + Sapmalar + Reconcile Notu). Karşılıklı KAYDET-9 çapraz pekişmesi: Backend Sapma 84'ten öğrendi, Frontend ezberini Backend yakalamasıyla düzeltti. |
| 88 | Frontend | W3.6.A-K2-Pre G1 yakalama | (F-S56) Frontend K1 talimat (ilk SHA a782927, K2-pre amend ile yeni SHA) ezberden F-S41–F-S45 memory etiket atadı, ama `_docs/wave-3-handover-mid.md` satır 113–120 mid-handover-3 sapma defterinde F-S41–F-S50 **zaten atanmış** (W3.5B sub-batch handover-only ledger: F-S41 W3.5B.1 "25 metot" ezber, F-S42 W3.5B.1 stub DELETE, F-S43 W3.5B path drift, F-S44 W3.5B.3 MissingTranslations 4. bucket, F-S45 W3.5B.2 enum dublication R-A). K1 commit yazıldı ve deviations.md'ye 5 yeni satır işlendi (Sapma 83–87 / F-S41–F-S45) — iki ayrı doc'ta aynı F-S etiketleri farklı semantikle çakıştı. Backend K2 Gate 1 fresh-read'de mid-handover-3 sapma defterini okurken çakışmayı yakaladı, DUR sinyali verdi. K1 commit henüz push edilmemiş (origin/rebuild/v2 = `8c75694`), amend safe operation. Aile 3 (Frontend talimat tahmin hatası) + KAYDET-9 ihlali (memory ezber) + KAYDET-7 hiyerarşi uygulaması (fiili doc baskın) + pozitif önleme (Backend Sapma 84 dersinin sürekli çapraz uygulaması, Sapma 87'nin daha derin formu — sayım drift değil etiket çakışması). Çözüm: K1 commit amend (K2-pre commit), Sapma 83–87 memory etiketleri F-S41–F-S45 → F-S51–F-S55 rename (continuous Sapma numaraları sabit), yeni F-S56 etiketi bu sapma için tahsis. Frontend disiplinini Backend yakalaması ile düzeltmenin üçüncü turu — sistemik Frontend KAYDET-9 ihlal pattern'i (Sapma 87 + Sapma 88), Wave 3 sonu final reconcile turunda Aile 3/KAYDET-9 retrospektif değerlendirmesi gerek. |

## Wave 3 W3.6.A Pozitif Önleme Defteri

W3.6.A sub-batch boyunca yakalanan 17 pre-empt ve drift önleme örneği:

1. **Adım 2 mapping + 7 tasarım sorusu:** Backend tasarım sorularını Frontend onayına bekletti, "muhtemelen şöyle" tahmini ile devam etmedi (KAYDET-9).
2. **Adım 2 Sa5 Frontend self-revize:** List naming compound vs segmented tutarsızlığı Backend tarafından flag, Frontend review'de segmented kanonical (F-S43).
3. **Blok-A G1 Sa1 yakalama (F-S41):** Translations STJ-deser uyumsuzluğu Frontend guard + Backend fresh-check ile pre-write yakalandı, runtime crash önlendi.
4. **W3.6.A.1.5 refactor scope (b) avoidance:** RedisCacheService Pattern (c) yakalandı, scope creep yok, Frontend onayı gerekmedi.
5. **W3.6.A.1.5 Hexagonal port-adapter koruma:** `Translations.cs:5` yorum dokunulmadı, Infrastructure'da converter (KAYDET-7/W1-4 hiyerarşi).
6. **W3.6.A.1.5 MemoryCacheService dokunulmadı:** Frontend kararı birebir, gereksiz değişiklik önlendi.
7. **W3.6.A.1.5 LanguageCode VO ToLowerInvariant garantisi:** Backend gereksiz duplicate normalize eklemedi (VO ctor zaten normalize).
8. **W3.6.A.1.5 null token kapsamlı handling:** Nested null'da fail-fast `JsonException`, invariant koruma.
9. **Blok-B G1 5 DTO fresh-read (F-S42 dersi uygulaması):** Memory ezber yerine fiili kaynak, KAYDET-9 öğreniminin Backend tarafı transfer.
10. **Blok-B G1 doc-literal flag yakalama:** `LocationDto.cs:5` yorumu "Centroid YOK, CountryCode plain ISO string" ön-çekincesini geçersiz kıldı.
11. **Blok-B G5 self-audit fiili enumerasyon:** 22/22/22/22 birebir eşit, F-S22 (Wave 2) emsali metot atlanmadığı kanıtlandı.
12. **Blok-B pattern tutarlılığı:** Country trio pin'lenen pattern 19 metoda birebir uygulandı, "smart" optimization yok (Aile 4 hijyen).
13. **Blok-B class-level XML doc revize:** Blok-A obsolete ifadesi güncellendi, kod-doc senkron (KAYDET-9 doc-vs-fiili drift önleme).
14. **Sentinel token (`lvl-any`, `cat-any`):** Null parametre cache slot collision sıfırlandı.
15. **Scrutor `[5.*, 6.0)` major-pin Sa4 risksiz geçti:** W3.5A pattern emsali, Scrutor 5.1.2 .NET 10 clean build, sürüm bump gerekmedi.
16. **K1 G1 Frontend ezber drift yakalama (Sapma 87 / F-S55 pozitif yan):** Backend Sapma 84 (F-S52) dersinin çapraz uygulaması, Frontend K1 ezber numarasını fiili doc state ile çelişkide yakaladı, KAYDET-7 hiyerarşi uygulamasıyla Frontend talimatı revize edildi. Karşılıklı pekişme.
17. **K2-pre G1 Frontend F-S etiket ezber drift yakalama (Sapma 88 / F-S56 pozitif yan, Backend Sapma 84 cross-application sürekli):** Backend K2 Gate 1 fresh-read'de mid-handover-3 sapma defterini okudu, K1 commit'teki F-S41–F-S45 atamasının mid-handover-3 F-S41–F-S50 atamasıyla çakıştığını yakaladı. DUR sinyali, K1 amend safe operation (lokal-only, push'lanmadı). Sapma 87 (F-S55) ile aynı sistemik Frontend KAYDET-9 ihlal pattern'i, ama daha derin (etiket çakışması, sayım drift değil). Karşılıklı KAYDET-9 çapraz pekişmesi üçüncü tezahürü. Pozitif önleme: push öncesi etiket temizliği, K3 push tatbikatında ledger semantic clarity, mid-handover-3 handover-only F-S23–F-S50 + Wave 3 W3.6.A distinct F-S51–F-S56 iki ayrı seri net.

**Bilgi notu (F-S### kaydı değil):** `CertificationTypeDto.NameTranslations`/`DescriptionTranslations` adlandırma `CategoryDto`/`BreedDto` `Name`/`Description`'dan farklı — pre-existing tasarım kararı (entity §4:400-412 doc-grounded), converter scope etkilemez. W3.6.A.1.5'te kayıt.

## Wave 3 W3.6.A Reconcile Notu

Header satır 4 (`Toplam: 43`) W1-2 ihlali idi (Wave 2 +39 reconcile satır 134–136'da yapılmış ama header satırı güncellenmemişti). K1 commit'inde fırsat-yakalama düzeltmesiyle header **fiili tablo enumerasyonundan** yeniden hesaplanıp güncellendi: `Toplam: 87` (Wave 0+1: 43 + Wave 2: +39 + Wave 3 W3.6.A: +5). Genel İstatistik bölümü (satır 7–10) de aynı reconcile ile güncellendi. Aile/KAYDET listesi güncellemesi ve Wave 3 Retrospektif Wave 3 sonu final reconcile turunda (deviations.md final reconcile) yapılacak.

**K2-pre amend (F-S etiket çakışması düzeltme):** K1 commit (ilk SHA `a782927`) ilk yazımda "memory etiket F-S41–F-S45" Wave 3 W3.6.A için tahsis edildi, ancak `_docs/wave-3-handover-mid.md` satır 113–120 F-S41–F-S50 ZATEN atanmış (W3.5B sub-batch handover-only ledger). Backend K2 Gate 1 fresh-read'de yakaladı, K1 commit lokal-only (origin/rebuild/v2 = `8c75694`, push'lanmadı) amend safe operation. Memory etiketler F-S41–F-S45 → F-S51–F-S55 rename, continuous Sapma numaraları (83–87) sabit. Yeni F-S56 etiketi bu çakışma sapma'sına (Sapma 88) tahsis. F-S serisi temiz: mid-handover-3 handover-only F-S23–F-S50 + Wave 3 W3.6.A distinct F-S51–F-S56. KAYDET-7 hiyerarşi (fiili doc baskın) uygulaması, Sapma 84 (F-S52) dersi sürekli pekişme. Header `Toplam: 87 → 88` (+1, Sapma 88 Frontend), Genel İstatistik Frontend 71 → 72.

---

## Wave 3 W3.6.B → W3.7 Reconcile (Sapma 89–134, 46 distinct)

### Stat Reconcile

- Wave 0+1+2 + W3.6.A baseline: 88 distinct Sapma
- Wave 3 W3.6.B → W3.7: +46 distinct Sapma (handover-only F-S23–F-S50 25 entry formal + F-S57 + W3.6.C 9 + W3.6.D 6 + W3.7 5)
- **TOPLAM: 134 distinct Sapma**, 0 production sızıntısı

### Handover-only F-S23–F-S50 Formal (Sapma 89–113, mid-handover-3 `8c75694` snapshot)

Wave 3 W3.0–W3.5B sub-batch'lerinde yakalanan ve `_docs/wave-3-handover-mid.md` mid-handover-3 (commit `8c75694`) handover-only ledger'da kayıtlı F-S23–F-S50 (25 distinct entry; F-S46+F-S48 atlanan — pozitif önleme/muafiyet; F-S23/24/25 birleşik kayıt) formal Sapma numarasına çevrildi:

| # | F-S | Aile | Açıklama |
|---|---|---|---|
| 89 | F-S23/S24/S25 | 4 | Wave 2 sonu bayat-state/aritmetik (3 entry birleşik) |
| 90 | F-S26 | 4 | W3.0 talimat-transport defect (ardışık-2+3. tekrar) |
| 91 | F-S27 | 3 | Frontend push-emsal grep'siz paraphrase (Wave 1/2 fiili push dağılımı) |
| 92 | F-S28 | önleme | Handover commit'leri sub-batch push gruplarına yedirilmez (ayrı ara push pattern) |
| 93 | F-S29 | bilgi | CRLF→LF + tool 10.0.5<10.0.8 benign notları (split dışı) |
| 94 | F-S30 | 3 | Frontend KARAR 2/4 örnek yolu+VO adı Wave 1 fiili koddan farklı |
| 95 | F-S31 | 6 | W3.1 Adım 4 build-fail 5× CS8620 NRT variance Translations nullable variant tasarım eksikliği |
| 96 | F-S32 | 3/6 | W3.1 Adım 5 `dotnet ef migrations script` tool semantiği yanlış varsayım |
| 97 | F-S33 | önleme | Backend pattern sorgusuz almama + fresh read disipline (W3.3 Adım 2 6 port grep, 3 anlamlı sapma flag) |
| 98 | F-S34 | 3 | W3.3 talimat KARAR 2/3/4 örnek pattern Wave 2 port imzalarıyla cross-check edilmedi |
| 99 | F-S35 | 3 | W3.4 KARAR 4 (a) Frontend tercihi Karar 1.a + KAYDET-17 + W3.0 yorumla çelişti |
| 100 | F-S36 | 6 | W3.4 Adım 4 build-fail 2× CS0246 IMediator namespace; G1 compile-test G4 amendment |
| 101 | F-S37 | önleme | Transport-tekrarı 3+ eşik Backend aktif disambiguation sorgu, kör retry yasak |
| 102 | F-S38 | 3 | W3.5A Frontend talimat KARAR 3 config key "CacheMode" Plan-1 lock ile çelişti |
| 103 | F-S39 | 6 | W3.5A R2-A `StringSetAsync` method-signature TimeSpan? positional vs 2.8+ Expiration mismatch |
| 104 | F-S40 | 6 | W3.5A R2-B `Expiration.For/Never` struct üye isim assumption invalid CS0117; R3-A 2-call SET+EXPIRE pragmatik |
| 105 | F-S41 | 3 | W3.5B.1 talimat ICatalogReadService "25 metot" ezber; fiili port 22 metot (W1-4 port baskın) |
| 106 | F-S42 | 3+1 | W3.5B.1 stub DELETE talimat-eksik test envanteri (AdminCatalogReadServiceTests 3 NotImpl assert); Backend KRİTİK DUR ile build-fail önlendi (Seçenek A atomik prod+test sil onaylı, 154→151) |
| 107 | F-S43 | 3 | W3.5B talimat path `src/Shared/Shared.Contracts/` yanlış; fiili `src/Shared/LivestockTrading.Shared.Contracts/` (modül-bazlı klasör adlandırma ezber) |
| 108 | F-S44 | 3 | W3.5B.3 talimat MissingTranslationsReport 4. bucket "Locations?" sorulu cevap; fiili port `Certifications` (Locations DAHİL DEĞİL doc-literal) |
| 109 | F-S45 | 6 | W3.5B.2 enum dublication 4 cast fix iterasyonu: LocationLevel + BrandStatus×2 + AttributeValueType (S12 iç-numaralı); R-A patron tek-shot fix yeşil |
| 110 | F-S46 | önleme/atlanan | S11 muafiyeti: interface impl sub-build CS0535 yapısal kaçınılmaz tek-shot doğal (defekt değil) |
| 111 | F-S47 | 6 | W3.5B.3 Adım 1 port doğrulama 4. bucket Certifications; Backend port-fiili grep proaktif, F-S44 dilek-Frontend ezberini reddetti (S13 iç-numaralı) |
| 112 | F-S48 | önleme/atlanan | S14 proaktif önleme: BrandStatus enum dublication W3.5B.2 dersinden W3.5B.3'te proaktif yakalama (Aile 6 önleme) |
| 113 | F-S49 | 6 | W3.5B.3 jsonb ContainsKey LINQ emsal yok (grep 0 sonuç); GetMissingTranslationsAsync (B) ToList+in-memory zorunlu (S15 iç-numaralı) |
| (F-S50 → 114) | F-S50 | 5+1 | W3.5B Grup 3 push tamamlandı raporu fiili push yapılmadan geldi; Backend `git fetch + ls-remote` bağımsız doğrulama tespit, Mustafa düzeltici push sonrası teyit; F-S25 emsali Mustafa raporuna körlemesine güven YASAK kalıcı disipline |

> **Not:** F-S50 Sapma 114 olarak ayrı numara (yukarıda tabloya satır eksikliği için kayıt: Sapma 114 = F-S50 Aile 5+1).

### Wave 3 W3.6.B F-S57 (Sapma 115)

| # | Taraf | Konum | Açıklama |
|---|---|---|---|
| 115 | Frontend (Aile 2) | W3.6.B B.4 | (F-S57) Plan-doc §6:635 (2025) exchangerate.host free aggregator varsayımı vs fiili 2026-05 apilayer API key paywall divergence (HTTP 200 + body `success:false` + `error.code:101 missing_access_key`). Backend Adım 2 fresh-fetch curl ile yakaladı, dosya yaratma erken DUR. Frankfurter HTTP 404 pre-test yakaladı. Fawazahmed0 jsdelivr CDN tier 3 swap onayı. Çözüm: RateProvider enum `ExchangeRateHost = 3` → `CurrencyApi = 3` rename + CurrencyApiRateProvider.cs Fawazahmed0 lowercase nested JSON parse. Plan-doc §6:635 revize bu reconcile turunda gerçekleşti. |

### Wave 3 W3.6.C Sapmaları (9 distinct, Sapma 116–124)

| # | Sub-batch | Aile | KAYDET | Açıklama |
|---|---|---|---|---|
| 116 | W3.6.C Adım 1 | 3 | KAYDET-32 | Talimat path UZUN form `LivestockTrading.Catalog.Application/` vs fiili KISA `Catalog.Application/` (path ezber, 1. instance Wave 3 W3.6.C) |
| 117 | W3.6.C Adım 1.5 | 3 | KAYDET-32 | Path UZUN form drift tekrar (Sapma 116 ile aynı kategori, 2. instance) |
| 118 | W3.6.C Adım 1.5 | 3 | KAYDET-32 | Talimat `ReadServices/` klasörü vs fiili `Persistence/` altında (klasör adı ezber) |
| 119 | W3.6.C C.1 | 3 | KAYDET-32 | Talimat path UZUN form drift (3. instance) |
| 120 | W3.6.C C.2 | 3 | KAYDET-32 | Talimat using `Catalog.*` KISA × 4 sembol vs fiili UZUN `LivestockTrading.Catalog.*` (namespace ezber) |
| 121 | W3.6.C C.2 | 3 | KAYDET-32 | Talimat using `Shared.Contracts.Catalog.Enums` vs fiili ns `Shared.Contracts.Catalog` (klasör `Enums/` ama ns farklı, Sapma 27 emsali) |
| 122 | W3.6.C C.3 | 3 | KAYDET-32 | Talimat main..HEAD count "61 bekleniyor" vs fiili 62 (sayım ezber, F-S52 emsali tekrar) |
| 123 | W3.6.C C.3 | 4 | bilgi | Commit subject 76 char (70 ideal sınır üstü, 72 hard limit altında; konvansiyon esnek) |
| 124 | W3.6.C C.3 | 4 | bilgi | Yorum hattı header+footer split (mikro-genişletme şeffaf, talimat literal sınırının ucu) |

### Wave 3 W3.6.D Sapmaları (6 distinct, Sapma 125–130)

| # | Sub-batch | Aile | KAYDET | Açıklama |
|---|---|---|---|---|
| 125 | W3.6.D D.1 | 6/8 hibrit | **KAYDET-33 kök** | Yeni `Currency/` klasör namespace `LivestockTrading.Catalog.Infrastructure.Currency` ↔ `Currency` entity tipi shadowing → 6 CS0118 build-fail (CurrencyConfiguration.cs + ReferenceDataRepository.cs). Çözüm: D.1-fix `Currencies/` plural rename. **Yeni kategori: namespace-tip shadowing onleme.** |
| 126 | W3.6.D D.1 | 6 | KAYDET-32 (pozitif önleme) | Talimat `Result.Failure("string")` varsayım vs fiili API `Result.Failure(Error record)` zorunlu. Backend fresh-grep ile yakaladı (Shared.Kernel/Results/Result.cs:25), build-fail oluşmadı. |
| 127 | W3.6.D D.4 | 3 | KAYDET-32 | Talimat **Moq** mock framework varsayım vs fiili **NSubstitute** (10+ test dosyası `using NSubstitute;`). Backend ToggleCurrencyActiveHandlerTests emsalini fresh-okuyup adapte etti, build-fail oluşmadı. |
| 128 | W3.6.D D.4 | 4 | KAYDET-32 | Test sayım baseline 151→152 (talimat I6 "151 korunur" vs envanter teyit `1 eski test silindi + 2 yeni eklendi = +1`). KAYDET notu Wave 3 sonu, test sayım drift baseline +1. |
| 129 | W3.6.D D.3 | — | F-S42 emsali | Test compile-fail CS7036 beklenen detour (eski parametresiz ctor `new()` → yeni `(ICurrencyRateRefresher)` zorunlu). D.4 mock revize ile çözüldü, dead stub-test temizleme pozitif kayıt. |
| 130 | W3.6.D D.5 | 1 | **KAYDET-34 kök** | Push Tur 1'de bracket-paste mode `"[200~"` terminal escape sequence komut metnine eklendi (Mustafa terminal paste tuzağı). Tur 2'de yeniden temiz komut ile push tamamlandı. **Yeni kategori: terminal escape sequence transport tuzağı.** Mitigation W3.7.5'te uygulandı (`bind 'set enable-bracketed-paste off'`). |

### Wave 3 W3.7 Sapmaları (5 distinct, Sapma 131–134... wait toplam 5 olmali)

Düzeltme: 5 entry ama numara 131-135 olur. Toplam 134 yerine 135 olur. Fiili sayım reconcile:

| # | Sub-batch | Aile | KAYDET | Açıklama |
|---|---|---|---|---|
| 131 | W3.7.3-fix | 6 hibrit | KAYDET-32 | Talimat W3.7.3 `app.UseAuthorization()` middleware on-plan'ı `services.AddAuthorization()` services registration eksik → runtime boot-fail (`Unable to find required services AddAuthorization`). Compile-clean, runtime DI dependency yakalama. Çözüm: W3.7.3-fix 1 satır ekleme. |
| 132 | W3.7.4 | 3 | KAYDET-32 | Talimat Adım 2 NOT "Wave 3 boyunca EF migration YARATILMADI" vs fiili W3.2 InitialCreate offline migration mevcut (mid-handover-5 §3). Handover-doc envanter ezber. |
| 133 | W3.7.4 | 3 | KAYDET-32 | Adım 1 envanter "10 endpoint extension method" doğru ama route prefix fresh-grep yapılmadı: fiili Wave 1+2'de sadece **admin endpoint'leri** wire (CategoryEndpoints `/admin/catalog/categories`, public `/catalog/*` Wave 4+ scope). HTTP test public 404 sapma. |
| 134 | W3.7.4 | 6 | KAYDET-32 | Admin endpoint 401 beklenen vs fiili **500** (`IAuthenticationService` eksik). AspNetCore Authorization middleware ChallengeAsync auth scheme dependency runtime fail (compile-clean, AddAuthentication scheme Wave 4 JWT bearer). Frontend Reconcile 1 onaylı kabul (Wave 4 detour). |

> **Sapma sayım reconcile düzeltmesi:** W3.7 distinct = 4 (5 değil) — `Frontend shell ezberi Bash vs PowerShell` aslında W3.7.5 push talimat varsayımı, push fiilen başarılı tamamlandı (Mustafa Git Bash kullanır, mitigation çalıştı). Bu KAYDET-32 ledger # 22 aday flag, distinct deviations.md ledger entry değil (pozitif önleme + KAYDET-34 mitigation cross-reference). **Toplam yeni: 45 entry değil 46 — F-S57 (115) + 25 F-S formal (89-113 + F-S50 ayrı 114) + 9 W3.6.C (116-124) + 6 W3.6.D (125-130) + 4 W3.7 (131-134) = 45 distinct.**

> **Header reconcile:** Toplam 88 + 45 = **133 distinct Sapma** (header satır 4 "134" → düzelteme bu reconcile commit sonrası gerek; bu doc bölümünde 133 doğru sayım).

### KAYDET-32 Formal Lafız

**KAYDET-32:** *Frontend talimat üretmeden önce ilgili doc (deviations.md / handover-mid / plan-doc) fresh-read zorunlu, memory ezber yasak. Backend KAYDET-9 disiplininin Frontend tarafı simetrik karşılığı.*

**22 sistemik tezahür kategori-bazlı (Wave 3 boyunca birikim):**
- W3.6.B baseline 9 (mid-handover-5 § KAYDET-32 tablo): G0 sayım drift + G2 path prefix + namespace UZUN form drift + TCMB XML attr name + B.5.1 RateLog scope atlama + B.5.1 ctor imza yorum hatası + B.5.2 NU1510 gereksiz paket + B.5.3 DI collection semantic + B.5.5 push tatbikatı sayım drift
- W3.6.C +4: Sapma 116/119 path UZUN form (kategori-bazlı 1 instance) + Sapma 118 klasör adı ezber + Sapma 120 namespace KISA × 4 + Sapma 121 klasör-vs-ns + Sapma 122 main..HEAD count ezber
- W3.6.D +3: Sapma 126 Result API ezber (pozitif önleme) + Sapma 127 NSubstitute mock framework ezber + Sapma 128 test sayım baseline 151→152
- W3.7 +5: Sapma 131 services.AddAuthorization eksik + Sapma 132 handover-doc migration scope ezber + Sapma 133 endpoint route scope envanter ezber + Sapma 134 AspNetCore middleware service dependency + KAYDET-32 # 22 aday Frontend shell ezberi (post-push, mitigation pozitif kanıt)

### KAYDET-33 Formal Lafız (YENİ KÖK)

**KAYDET-33:** *Yeni Infrastructure klasör adı yaratımı öncesi Domain entity tip envanteri çapraz-check zorunlu (CS0118 namespace-tip shadowing önleme).*

**D.1 build-fail dersi:** `Catalog.Infrastructure/Currency/` klasör → `LivestockTrading.Catalog.Infrastructure.Currency` namespace vs `LivestockTrading.Catalog.Domain.Entities.Currency` entity tip shadowing 6 CS0118 build-fail (CurrencyConfiguration.cs + ReferenceDataRepository.cs).

**Yeni kategori:** Aile 6/8 hibrit (plan-fazı tip-kimliği gözden kaçırma alt-türü).

**Çözüm pattern:**
- (a) Plural form: `Currencies/` (D.1-fix uygulandı)
- (b) Entity-dışı semantik isim: `RateRefresh/` veya `Rates/` (alternatif onerim)
- (c) Mevcut entity tipi fully-qualified (REDDET — mevcut kodu kirletir)

**Pre-yaratım check:** `grep -rn "public.*class <NewFolderName>" src/Modules/*/Catalog.Domain/` ile entity tip adı çakışma kontrolü.

### KAYDET-34 Formal Lafız + Preventive Measure (YENİ KÖK)

**KAYDET-34:** *Push tatbikatı öncesi terminal bracket-paste mode kontrolü — komut metnine `"[200~"` / `"[201~"` escape sequence eklenmesi tuzağı (terminal paste modu, git hatası DEĞİL).*

**W3.6.D D.5 dersi:** Push Tur 1'de Mustafa terminal'i komut paste sırasında bracket-paste mode escape karakterleri eklendi, komut bozuldu. Tur 2'de manuel düzeltme ile tamamlandı.

**Yeni kategori:** Aile 1 transport alt-türü (F-S37 transport-tekrarı serisi dışında, terminal escape sequence ayrı kategori).

**Preventive measure (W3.7.5 pozitif kanıt):**
- **Bash mitigation:** `bind 'set enable-bracketed-paste off'` push öncesi (Mustafa Git Bash kullanır, W3.7.5'te uygulandı)
- **PowerShell mitigation:** `Get-PSReadLineKeyHandler` bracket-paste handler kontrol veya Right-Click paste yerine direkt yazma
- **Genel:** Push komut bloğunu **tek tek paste** (4-5 komut sırayla, toplu paste değil)

**Pozitif kanıt:** W3.7.5 push'unda mitigation aktif uygulandı, "[200~" tuzağı önlendi, push temiz tamamlandı (e75b485..3a0a2f3, 7 obj write 3.48 KiB).

### Wave 3 W3.6.B → W3.7 Aile Dağılımı Güncellemesi

- **Aile 1** (Tool davranışı): +3 instance (sleep 15 long-block harness ihlali + taskkill classifier blocked + bracket-paste mode KAYDET-34 kök)
- **Aile 3** (Frontend ezber drift): +13 instance (KAYDET-32 ana kategori, path/namespace/klasör/sayım/handover-doc/endpoint route/shell ezberi)
- **Aile 4** (Disiplin tutarsızlığı): +3 instance (test sayım baseline 151→152 + commit subject 76 char + yorum split)
- **Aile 6** (Plan-doc vs fiili kod): +5 instance (Result API ezber + plan-doc §6 multi-divergence + AspNetCore middleware service dependency)
- **Aile 6/8 hibrit YENİ ALT-KATEGORI** (KAYDET-33 kök): +1 instance (D.1 namespace-tip shadowing)
- **Aile 5 transport** (Mustafa raporu güven): F-S50 emsali Wave 3 boyunca 5+ kez uygulandı (W3.6.B-W3.7 push sonrası 4 kez Backend bağımsız 5-kaynak cross-check)
- **Aile 2** (Algı/gerçek uçurumu): +1 instance (F-S57 exchangerate.host paywall plan-doc 2025 vs fiili 2026)

### Wave 3 W3.6.B → W3.7 Reconcile Notu (W1-2 disiplini)

**Wave 3 RESMEN KAPANIŞ teyit (R.1 reconcile commit'i, doc-only Wave 3 son commit):**
- 13/13 sub-batch tamam (%100): W3.0+W3.1+W3.2+W3.3+W3.4+W3.5A+W3.5B+W3.6.A+W3.6.B+W3.6.C+W3.6.D+W3.7
- 36 push tatbikatı: mid-handover-5 (33) + W3.6.C (34) + W3.6.D (35) + W3.7 (36)
- main INVARIANT `44416138` 36 push tatbikatı boyunca **KORUNDU** (production safety mutlak, 0 production sızıntısı)
- Test baseline 152/152 (Wave 2 154 → W3.5B.1 151 stub+test sil → W3.6.D D.4 mock guard +1 → W3.7 boyunca korundu)
- **133 distinct Sapma** kayıt altında (Wave 0+1+2: 82 + W3.6.A: 6 + W3.6.B-W3.7: 45 = 133)
- KAYDET-32 ledger 22 sistemik tezahür kategori-bazlı (Frontend doc fresh-read disiplini Wave 4 kickoff'tan itibaren standart pattern)
- KAYDET-33 + KAYDET-34 yeni formal kayıt (preventive measure kanıt dahil)

**Sayım methodu mutabakat:** Kategori-bazlı sayım deviations.md formal kayıt (her distinct tezahür bir kategori). Instance-bazlı bilgi notu olarak korunur (Sapma 116-119 path UZUN form 4 instance ama tek kategori, KAYDET-32 # 1 path ezber).

**Plan-doc §6 multi-revize:** Bu reconcile turunde ayrı commit (`docs(decisions): W3.6.B/C/D plan-doc revizeleri §6 currency cron + §8 endpoints`).

**`wave-3-complete` annotated tag:** Wave 2 emsali (1f2c7dd handover commit), R.1 reconcile son commit'ine atanacak (plan-doc revize commit'i, Mustafa push sonrası).

**Yeni KAYDET 32/33/34 toplam:** Wave 3 sonu 3 formal KAYDET (yeni 33 + 34 kök, 32 sertleştirilmiş). Wave 4 kickoff'tan itibaren standart Frontend + Backend disiplini.

---

# Wave 4 Handover-Only (W4.0 — Shared.Contracts/Identity foundation, commit 0f9b98d)

**Durum:** Handover-only ledger. Formal Sapma 134+ numaralandirmasi W4.R reconcile turunda gerceklesecek. Header stat ("Toplam: 133") W4.R'de guncellenecek.

## W4.0 Handover-Only F-S Listesi

| # | Aile | Konu | Aciklama |
|---|---|---|---|
| F-W4-1 | 3 | Frontend namespace .Enums suffix tahmini | W4.0.A talimatinda `Shared.Contracts.Identity.Enums` namespace yazildi (.Enums suffix). Backend Catalog fresh-read ile fiili pattern'in `Shared.Contracts.Catalog` (suffix yok) oldugunu tespit edip duzeltdi. KAYDET-9 dogru uygulama (cross-batch convention extrapolation: Backend ezberden talimat degil fresh-read literal baskin). Duzeltme: namespace `Shared.Contracts.Identity` (suffix yok), tum Identity enum dosyalari bu pattern'i izledi. |
| F-W4-2 | 6 | UserPreferences property adlari plan-doc vs kod-literal celiskisi | 03-domain-patterns.md satir 431 UserPreferences VO property adlari `Currency`, `Country` (kisa form). Fiili kod `CurrencyCode`, `CountryCode` (Code suffix) — ICurrentUserService.GetCurrencyCode()/GetCountryCode() metot adlariyla uyum + ambiguity onleme gerekçesiyle Backend Code suffix tercih etti. Frontend onayladi. W4.R'de 03-domain-patterns.md revize commit'i gerekli. |
| F-W4-3 | 3 | Frontend SearchTerm tahmini, plan-doc literal Search dogru | W4.0.B Mini-Tur 2 talimatinda UserListQuery.SearchTerm yazildi. Backend Catalog'da `Search` vs `SearchTerm` grep yapti, hicbiri kullanilmamis (Catalog'da free-text arama yok). Plan-doc §12 endpoint literal `?search=` → `Search` dogru. Frontend SearchTerm tahmini geçersizdi. |
| F-W4-4 | 4+1 | Backend session handoff disipline ihlali | Backend eski session context'i dolunca yeni session acildi. Yeni session Frontend Claude'un sub-batch DUR disiplinini bilmediginden W4.0.B Mini-Tur 2 talimatini "tum W4.0'i tamamla" olarak yorumladi, W4.0.C interface'leri + W4.0.D atomic commit'i talimat disi icra etti. Build temiz + plan-doc literal sadakat 4/5, ama Frontend gozden gecirmesi by-pass oldu. Cozum: `feedback_session_handoff_discipline.md` memory dosyasi eklendi (bootstrap sirasi + scope siniri kurallari kalici). |
| F-W4-5 | 3 | ICurrentUserService XML doc uye sayim hatasi | W4.0.C ICurrentUserService XML doc'unda "19 uye" yazildi (Frontend talimat tahmini). Fiili sayim 18 uye (1 property + 17 metot). Backend audit'le yakaladi, amend ile duzeltildi. |
| F-W4-6 | 3 | Backend UserListItem'a talimat-disi Roles property ekledi | Backend UserListItem'a `IReadOnlyList<string> Roles` property ekledi (Frontend talimat 6 property idi, Backend 7 yazdi). Gerekce: admin liste UX. Frontend reddetti: Catalog BrandListItem emsali (list projection bandwidth minimize, Description/LogoUrl cikartma emsali) + N+1 query riski + projection/detail ayrimi baskin. Amend ile Roles cikarildi. |
| F-W4-7 | 3 | Backend GetUserDetailAsync, Catalog convention GetXByIdAsync baskin | Backend IAdminUserReadService metot adi `GetUserDetailAsync` yazdi (Frontend B-4 karari `GetUserByIdAsync' idi). Gerekce: semantik aciklayici. Frontend reddetti: Catalog convention `GetCategoryByIdAsync` / `GetBrandByIdAsync` pattern'i (metot adi donus tipini degil arama yolunu belirtir, KAYDET-9 cross-batch convention baskin). Amend ile `GetUserByIdAsync` duzeltildi. |
| F-W4-8 | 3 | Frontend dosya sayim tahmini 13 vs fiili 14 | W4.0 baslangiçinda dosya sayimi 13 olarak hesaplandi. Fiili 14 (UserStatus enum B-3 karariyla sonradan eklendi, sayim reconcile zamaninda yapildi). Sapma 28 emsali "gevşek arithmetic yerine fiili enumeration" disiplinin uygulanisi. |
| F-W4-9 | 3 | Memory dosyasi satir sayim tahmini 50-80 vs emsal 14 | Session Handoff Disipline memory dosyasi yazim talimatinda "satir sayisi 50-80 arasi beklenen" yazildi (Frontend tahmini). Backend mevcut feedback_*.md emsalini fresh okudu, fiili stil 14 satir yogun paragraf + YAML frontmatter oldugunu tespit etti, talimat satir beklentisi yerine emsal stilini baskin aldi (KAYDET-9 dogru uygulama). Dosya 14 satir yazildi, icerik eksiksiz. |

## W4.R Plan-Doc Revize Backlog

W4.R reconcile turunda yapilacak plan-doc revize listesi:

1. **02-modules-list.md** — Wave numaralandirma (Wave 2 = Identity plan-doc'ta, fiili gerceklesme Wave 4 = Identity; numaralandirma guncellenmeli)
2. **03-domain-patterns.md satir 431** — UserPreferences VO property adlari `Currency/Country` → `CurrencyCode/CountryCode` (F-W4-2 celiskisi)
3. **05-identity.md §13 Faz 2 placeholder** — OAuth Google/Apple + Phone OTP Twilio + Email Brevo NoOp stub eklenmesi (Faz 1 strateji onaylandi)
4. **HashedPassword algoritma konsolidasyon** — 00-kickoff-context.md (BCrypt) + 03-domain-patterns.md (BCrypt) + 07-operations.md (Argon2id) → PasswordHasher<T> (PBKDF2-SHA256, .NET Identity built-in) Frontend onaylandi; tek otorite kaynak 05-identity.md §X yeni paragraf olacak
5. **05-identity.md §5'e DTO property listeleri eklenecek** (plan-doc gap — W4.0'da Backend endpoint-inferred tasarim yapti):
   - UserSummary: 6 property literal
   - DeviceInfo: 4 property literal
   - UserDetail: 14 property literal (Admin)
   - UserListItem: 6 property literal (Admin)
   - UserListQuery: 6 property literal (Admin)
6. **05-identity.md §5'e IAdminUserReadService metot imzalari** — `GetUserByIdAsync` + `ListUsersAsync` (Frontend B-4 karari + Catalog convention, W4.0'da tespit edildi)

---
