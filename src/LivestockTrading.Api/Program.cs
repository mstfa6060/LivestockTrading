// =============================================================================
// WAVE-1 PLACEHOLDER — LivestockTrading.Api host bootstrap
//
// Bu dosyanın TEK işi: CS5001 (entry point yok) hatasını temizleyip Release
// build'i yeşillendirmek. GERÇEK host wiring (Serilog, Sentry, OpenTelemetry,
// OpenApi/Scalar, 10 modül DI, /live + /health) HENÜZ YOK — ilgili NuGet
// paketleri bilinçli girildiğinde sonraki wave'lerde bu placeholder DEĞİŞİR.
//
// Otorite dokümanlar (bu stub onların YERİNE GEÇMEZ):
//   _docs/decisions/06-api-contract.md  — OpenApi + Scalar setup, Program.cs
//   _docs/decisions/07-operations.md    — OpenTelemetry/Serilog/Sentry host
//
// Kısıt: 0 NuGet paketi (Wave 0 doğrulanmış invariantı). Yalnızca ASP.NET Core
// shared framework kullanılır (Microsoft.NET.Sdk.Web FrameworkReference).
// =============================================================================

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Run();
