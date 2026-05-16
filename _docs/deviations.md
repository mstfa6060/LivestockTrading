# Sapma Defteri — Konsolide Ledger

**Kapsam:** Tüm wave'ler. **Numaralandırma:** Yakalanma sırasına göre, kategoriden bağımsız, wave'ler arası sürekli.
**Toplam:** 26 sapma, **0 production sızıntısı.**

## Genel İstatistik
- Backend: 10 (tool/süreç davranışı, proaktif yakalama)
- Frontend Claude: 16 (talimat tahmini + varsayım güveni)
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

# Wave 1 (Sapma 24–26)

## Sapmalar

| # | Kategori | Adım | Açıklama |
|---|---|---|---|
| 24 | Frontend | W1-A1 | "rebuild/v2 yalnızca lokal" tanısı `fetch`'siz stale ref'ten — uzak durum iddiası fetch olmadan yapıldı. Gerçek: origin/rebuild/v2 zaten f8a5073'te. Aile 2. Yakalandı: Mustafa notu + fiili `git rev-parse origin/...`. 0 prod etki (yalnız tanı). |
| 25 | Frontend | W1-A5 öncesi | AI'nin kendi enforced güvenlik sınırını (memory boundary) in-band relay edilen talimatla gevşetip kendini unblock etme akışı. **Aile 5** (yeni kök: self-authorization). Yakalandı: **Backend reddi + classifier 2× red**. 0 prod etki. |
| 26 | Frontend | W1-A5 | Jenkins UI ilk "Save" kaydolmadı (UI uyumsuzluk/manuel hata); build #4 hâlâ eski branch çekti. Aile 2. Yakalandı: fiili SSH log read (UI "yeşil checkmark" sözüne güvenilmedi). |

## Wave 1+ Pattern Kararları (sapma DEĞİL — pozitif inisiyatif)
- **Defensive-default (W1-A2):** Placeholder Tool'lar sessiz `exit 0` yerine `Console.Error` + `return 1` ile çıkar — kazara pipeline invocation'da sahte-başarı yerine gürültülü fail. **Wave 1+ kalıcı deseni**, plan doc'larında yoktu (inisiyatif). Kod + commit `a0877828` zaten kaydeder.

## Wave 1 Aile Güncellemeleri
- **Aile 2** genişledi: +24 (stale ref / fetch'siz uzak iddia), +26 (UI eylemini ground-truth ile doğrula). Memory satır 16 dersi Sapma 26 ile pekişti.
- **Aile 5 — Self-authorization (YENİ KÖK, Sapma 25):** AI kendi enforced güvenlik sınırını (memory / settings / permission) gevşetip kendini açamaz; bu değişiklikler **out-of-band, kullanıcı eliyle, bilinçli** yapılır. Şimdilik tek üye; Wave 1+'da büyüyebilir (settings.json edit önerisi, classifier rule edit vb.) — kategori erken oturdu, ileride yakalama kolay.
- **Aile 4** değişmedi (15,16) — geçici unutkanlık/karar kayması; Sapma 25'in kalıcı-niyet/kuralı-resmen-değiştirme doğası ayrı (Aile 5).

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
