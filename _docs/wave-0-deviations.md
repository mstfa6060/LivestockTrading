# Wave 0 — Sapma Defteri

**Toplam:** 21 sapma, 0 production sızıntısı.
**Numaralandırma:** Yakalanma sırasına göre, kategoriden bağımsız.

**Asimetri:**
- Backend tarafı 10 sapma (tool/süreç davranışı + proaktif yakalama)
- Frontend Claude 11 sapma (talimat tahmini hataları + varsayım güveni)

Backend gerçek dünyaya yakın (komut çıktıları, fiili dosya durumu).
Frontend abstraksiyon katmanında (talimat tahmini, model güveni).
Frontend hatalarının **0'ı production'a sızdı** çünkü Backend disiplini her seferinde yakaladı.

## Sapmalar (yakalanma sırasına göre)

| # | Kategori | Açıklama |
|---|---|---|
| 1 | Backend | `grep -c` yorum satırlarını da sayar → false-positive |
| 2 | Backend | `find \| head` SIGPIPE riski (broken pipe) |
| 3 | Backend | `tee \| head` exit code yutar → `PIPESTATUS[0]` kullan |
| 4 | Backend | Windows `core.fileMode=false` → executable için `git update-index --add --chmod=+x` |
| 5 | Backend | Python Windows'ta default yok → JSON validate için `node -e "JSON.parse(...)"` |
| 6 | Backend | `dotnet new sln` .NET 10'da `.slnx` default üretir |
| 7 | Backend | `git check-ignore` exit code: 0=ignored, 1=tracked (tersine algılanır) |
| 8 | Backend | Heredoc büyük dosyalarda parse fail → Write tool'a düş |
| 9 | Frontend | `.sln` vs `.slnx` ayrımı (kullanıcı yakaladı) |
| 10 | Frontend | Em-dash kullanımı çelişkisi (Backend yakaladı) |
| 11 | Backend | Adım 0: 74 IDE artifact bin/obj — `.gitignore` tuttu, sızıntı yok |
| 12 | Frontend | Senaryo Y tahmin eksikliği: "Api + 3 Tools = 4 FAILED" doğrusu "3 FAILED + 1 SKIPPED" (OpenApiGen dependency-skip) |
| 13 | Frontend | projectUrl `.git/` eki eksikliği (B1.1 sed komutu) |
| 14 | Frontend | B2.0 auth-probe scope escalation — production salt okuma sanılan eylem aslında auth yoklaması |
| 15 | Frontend | Kullanıcı karar tutarsızlığı: "UI'dan kur" sonra otomatik SSH plan (Backend yakaladı) |
| 16 | Frontend | Memory exception sonrası kendi koyduğum sınırı bir tur sonra unuttum |
| 17 | Backend | "Manuel çalıştırdım" raporu sunucuya yansımamıştı — algı/gerçek uçurumu (fiili SSH doğrulama ile yakalandı) |
| 18 | Frontend | "Manuel çalıştırdım" mesajını "plugin manuel kurdun" diye yorumlama (algı yorumlama hatası) |
| 19 | Frontend | Koşullu/erken talimat üretme: önceki adım çıktısı review'sız sonraki adım talimatı |
| 20 | Frontend | Main hash kısa form (`4441613`) doğrulanmadan kullanım, prompt'a uydurma uzantı |
| 21 | Frontend | Bash heuristic talimatı bozuk: `grep -c \|\| echo 0` çift değer (Sapma 3 ailesinden, ama Frontend ürettiği için Frontend sapması) |
| 22 | Frontend | Backend'in önceki tur tanısına ("7 lokal-only claude/*") körlemesine güven, kendi doğrulama yapmadı (gerçek: 21 lokal + 11 origin) |

## Sapma Aileleri (Wave 1+ için derslerin kökleri)

### Aile 1: Tool davranışı yanılgısı (Sapmalar 1, 2, 3, 5, 6, 7, 8, 21)
Komut/araç davranışı tahmin edileni değil gerçeği yapar. `grep -c` yorumları sayar, `find | head` SIGPIPE atar, `tee | head` exit yutar. Wave 1+ kuralı: tool davranışını tahmin etme, test et veya dokümana bak.

### Aile 2: Algı/gerçek uçurumu (Sapmalar 11, 17, 22)
"Yapıldı" bilgisi yerine "yapıldı mı?" doğrulaması. IDE 74 bin/obj attı sanılmadı, Mustafa "plugin kurdum" raporu sunucuya yansımamıştı, Backend'in 7 lokal-only tanısı gerçekte 32 idi. Kural: rapora değil fiili kaynağa güven (post-push fetch, file stat, fresh check).

### Aile 3: Talimat tahmin hatası (Sapmalar 9, 10, 12, 13, 14, 18, 19, 20)
Frontend Claude talimat üretirken eksik bilgi varsayım yapar. `.sln`/`.slnx`, em-dash, Senaryo Y, projectUrl format, auth scope, hash format. Kural: belirsizlikleri tahminle çözme; Backend'den tanı talimatı al veya kullanıcıya kararı götür.

### Aile 4: Disiplin tutarsızlığı (Sapmalar 15, 16)
Kullanıcı kararı verildikten sonra zincirde kayma, kendi koyduğum kuralı bir tur sonra unutma. Kural: her tur başında "son kullanıcı kararı + aktif memory kuralı" yeniden oku.

## Wave 1+ İçin Çıkarılan Dersler

1. **Tool davranışını tahmin etme, test et** (Aile 1)
2. **Rapora değil fiili kaynağa güven** (Aile 2)
3. **Belirsizlikleri tahminle çözme** (Aile 3)
4. **Her tur kullanıcı kararını ve memory kurallarını yeniden oku** (Aile 4)
5. **Koşullu/erken talimat üretme** — guard yerine "talimatı geç vermek"
6. **Plan dokümanı vs durum çelişkisi açıkça raporlanır** — varsayım gizlenmez
