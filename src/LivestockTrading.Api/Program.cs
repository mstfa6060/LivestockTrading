using LivestockTrading.Catalog.Application;
using LivestockTrading.Catalog.Application.Features.AdminCatalogRead;
using LivestockTrading.Catalog.Application.Features.BorderRules;
using LivestockTrading.Catalog.Application.Features.Brands;
using LivestockTrading.Catalog.Application.Features.Breeds;
using LivestockTrading.Catalog.Application.Features.Categories;
using LivestockTrading.Catalog.Application.Features.CertificationTypes;
using LivestockTrading.Catalog.Application.Features.Countries;
using LivestockTrading.Catalog.Application.Features.Currencies;
using LivestockTrading.Catalog.Application.Features.Languages;
using LivestockTrading.Catalog.Application.Features.Locations;
using LivestockTrading.Catalog.Infrastructure;
using LivestockTrading.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Catalog module DI registration (W3.7 Catalog wire only; other 9 modules Wave 4+).
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(builder.Configuration);

// Authorization services (W3.7 I5 karari: policy framework default; JWT bearer scheme Wave 4).
builder.Services.AddAuthorization();

// CatalogDbContext registration (W3.4 KARAR 4b: DbContext + DomainEventDispatchInterceptor wire).
// Connection string per plan-doc 04-migration.md:137 (Search Path=catalog,public).
// Runtime user livestock_app (DML only); migrations run via livestock_migrator role (CI/manual).
var catalogConnStr = builder.Configuration.GetConnectionString("CatalogDb")
    ?? throw new InvalidOperationException("ConnectionStrings:CatalogDb is required.");

builder.Services.AddDbContext<CatalogDbContext>((sp, options) =>
{
    options.UseNpgsql(catalogConnStr, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "catalog");
        npgsql.UseNetTopologySuite();
    })
    .UseSnakeCaseNamingConvention()
    .AddInterceptors(sp.GetRequiredService<DomainEventDispatchInterceptor>());
});

var app = builder.Build();

// Authorization middleware (W3.7 I5 karari: UseAuthorization() ekle, JWT bearer Wave 4).
// Runtime /admin/catalog/* GET -> 401 Unauthorized beklenen (scheme yok), Wave 4'te cozulur.
app.UseAuthorization();

// Catalog endpoint mapping (10 endpoint extension method chain, hepsi IEndpointRouteBuilder doner).
app.MapCategoryEndpoints()
   .MapBreedEndpoints()
   .MapBrandEndpoints()
   .MapCurrencyEndpoints()
   .MapCountryEndpoints()
   .MapLanguageEndpoints()
   .MapCertificationTypeEndpoints()
   .MapLocationEndpoints()
   .MapBorderRuleEndpoints()
   .MapAdminCatalogReadEndpoints();

app.Run();
