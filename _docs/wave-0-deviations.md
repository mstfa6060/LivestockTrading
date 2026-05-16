# Wave 0 — Sapma Defteri (19 sapma, 0 production sızıntısı)

**Asimetri:** Backend tarafı 10 sapma (tool/süreç davranışı, proaktif yakalama), Frontend Claude 9 sapma (talimat tahmini hataları). İkisi de doğal — Backend gerçek dünyaya yakın, Frontend abstraksiyon katmanında. Önemli: Frontend hatalarının 0'ı production'a sızdı çünkü Backend disiplini her seferinde yakaladı.

## Backend Tool/Süreç Sapmaları (10)

1. `grep -c` yorum satırlarını da sayar → false-positive
2. `find | head` SIGPIPE riski (broken pipe)
3. `tee | head` exit code yutar → `PIPESTATUS[0]` kullan
4. Windows `core.fileMode=false` → executable için `git update-index --add --chmod=+x`
5. Python Windows'ta default yok → JSON validate için `node -e "JSON.parse(...)"`
6. `dotnet new sln` .NET 10'da `.slnx` default üretir
7. `git check-ignore` exit code: 0=ignored, 1=tracked (tersine algılanır)
8. Heredoc büyük dosyalarda parse fail → Write tool'a düş
9. Adım 0: 74 IDE artifact bin/obj — `.gitignore` tuttu, sızıntı yok
10. Build #1 raporlanan "manuel düzeltmeler" sunucuya yansımamıştı — algı/gerçek uçurumu, fiili SSH doğrulama ile yakalandı

## Frontend Claude Talimat Tahmini Hataları (9)

11. `.sln` vs `.slnx` ayrımı (kullanıcı yakaladı)
12. Em-dash kullanımı çelişkisi (Backend yakaladı)
13. Senaryo Y tahmin eksikliği: "Api + 3 Tools = 4 FAILED" demiştim, doğrusu "3 FAILED + 1 SKIPPED" (OpenApiGen dependency-skip)
14. projectUrl `.git/` eki eksikliği (B1.1 sed komutu)
15. B2.0 auth-probe scope escalation — production salt okuma sanılan eylem aslında auth yoklaması
16. Kullanıcı karar tutarsızlığı: "UI'dan kur" kararı verildikten sonra otomatik SSH plan üretildi (Backend yakaladı)
17. Memory exception sonrası kendi koyduğum sınırı bir tur sonra unuttum
18. "Manuel çalıştırdım" mesajını "plugin manuel kurdun" diye yorumlama (algı yorumlama hatası)
19. Koşullu/erken talimat üretme: bir önceki adımın çıktısı review edilmeden sonraki adımın talimatını üretme
20. Wave 0 başında "main = 4441613" kısa form, full hash hiç doğrulanmadı, prompt'a uydurma uzantı

(Not: yukarıda 11-20 = 10 madde ama 19 sapma diyorum çünkü bazıları çift sayılabilir. Asıl sayım Wave 1+ için temizlenecek.)

## Wave 1+ İçin Çıkarılan Dersler

1. **"Yapıldı sanılan" eylemler için fiili kaynak doğrulaması zorunlu** (Sapma 17 → tüm post-push doğrulama pattern'i)
2. **Memory exception kendi kuralı için de geçerli** — Frontend Claude kuralı koyarsa sonraki turda hatırlamalı
3. **Frontend Claude koşullu/erken talimat üretmemeli** — guard yerine "talimatı geç vermek"
4. **Kullanıcıya soru sormak yerine Backend'e tanı sor** — Backend zaten teknik durumu okuyabiliyor
5. **Prompt'a değil fiili veriye güven** — hash/SHA/sayım her zaman gerçek kaynaktan
6. **Plan dokümanı vs durum çelişkisi açıkça raporlanır** — varsayım gizlenmez
