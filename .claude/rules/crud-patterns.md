---
paths:
  - "**/RequestHandlers/**"
---
# Standard CRUD Endpoint Patterns

Her entity icin asagidaki 6 endpoint olusturulmalidir (Create, Update, Delete, All, Detail, Pick):

## Handler Constructor (tum handler'larda ayni):
```csharp
public class Handler : IRequestHandler
{
    private readonly DataAccess _dataAccessLayer;

    public Handler(ArfBlocksDependencyProvider dependencyProvider, object dataAccess)
    {
        _dataAccessLayer = (DataAccess)dataAccess;
    }
}
```

## 1. ALL (Queries/All) - Listeleme (Sayfalama, Siralama, Filtreleme)
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    public XSorting Sorting { get; set; }
    public List<XFilterItem> Filters { get; set; }
    public XPageRequest PageRequest { get; set; }
}

public class ResponseModel : IResponseModel<Array>
{
    public Guid Id { get; set; }
    // ... entity alanlari ...
    public DateTime CreatedAt { get; set; }
}

// Handler.cs - return ile page bilgisi dondurulur
var (items, page) = await _dataAccessLayer.All(req.Sorting, req.Filters, req.PageRequest, cancellationToken);
var response = mapper.MapToResponse(items);
return ArfBlocksResults.Success(response, page);

// DataAccess.cs - Sort, Filter, Paginate kullanilir
public async Task<(List<Entity> Items, XPageResponse Page)> All(
    XSorting sorting, List<XFilterItem> filters, XPageRequest pageRequest, CancellationToken ct)
{
    var query = _dbContext.Entities
        .AsNoTracking()
        .Where(e => !e.IsDeleted)
        .Sort(sorting)
        .Filter(filters);

    if (sorting == null)
        query = query.OrderByDescending(e => e.CreatedAt);

    var page = query.GetPage(pageRequest);
    var items = await query.Paginate(page).ToListAsync(ct);
    return (items, page);
}

// Mapper.cs - List<ResponseModel> dondurur
public List<ResponseModel> MapToResponse(List<Entity> items) { ... }
```

## 2. DETAIL (Queries/Detail) - Tekil Kayit
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    public Guid Id { get; set; }
}

// Handler.cs
var entity = await _dataAccessLayer.GetById(req.Id, cancellationToken);
if (entity == null)
    throw new ArfBlocksValidationException(
        ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));
return ArfBlocksResults.Success(mapper.MapToResponse(entity));

// DataAccess.cs - AsNoTracking ile
public async Task<Entity> GetById(Guid id, CancellationToken ct)
{
    return await _dbContext.Entities
        .AsNoTracking()
        .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
}
```

## 3. PICK (Queries/Pick) - Dropdown/Select Listesi
```csharp
// Models.cs
public class RequestModel : IRequestModel
{
    public List<Guid> SelectedIds { get; set; }
    public string Keyword { get; set; }
    public int Limit { get; set; } = 10;
}

public class ResponseModel : IResponseModel<Array>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // minimal alanlar
}

// DataAccess.cs - Oncelik: SelectedIds > Keyword > default
public async Task<List<Entity>> Pick(List<Guid> selectedIds, string keyword, int limit, CancellationToken ct)
{
    var query = _dbContext.Entities.AsNoTracking().Where(e => !e.IsDeleted && e.IsActive);

    if (selectedIds != null && selectedIds.Any())
        return await query.Where(e => selectedIds.Contains(e.Id))
            .OrderByDescending(e => e.CreatedAt).ToListAsync(ct);

    if (!string.IsNullOrWhiteSpace(keyword))
    {
        var lowerKeyword = keyword.ToLower();
        query = query.Where(e => e.Name.ToLower().Contains(lowerKeyword));
    }

    return await query.OrderByDescending(e => e.CreatedAt)
        .Take(limit > 0 ? limit : 10).ToListAsync(ct);
}
```

## 4. CREATE (Commands/Create)
```csharp
// Models.cs - RequestModel: entity alanlari, ResponseModel: olusturulan entity bilgileri
// Handler.cs
var entity = mapper.MapToEntity(request);
await _dataAccessLayer.Add(entity);
return ArfBlocksResults.Success(mapper.MapToResponse(entity));
```

## 5. UPDATE (Commands/Update)
```csharp
// Models.cs - RequestModel: Guid Id + guncellenecek alanlar
// Handler.cs
var entity = await _dataAccessLayer.GetById(request.Id);
if (entity == null) throw new ArfBlocksValidationException(
    ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));
mapper.MapToEntity(request, entity);
await _dataAccessLayer.SaveChanges();
return ArfBlocksResults.Success(mapper.MapToResponse(entity));
```

## 6. DELETE (Commands/Delete) - Soft Delete
```csharp
// Models.cs - RequestModel: Guid Id, ResponseModel: bool Success
// Handler.cs
var entity = await _dataAccessLayer.GetById(request.Id);
if (entity == null) throw new ArfBlocksValidationException(
    ErrorCodeGenerator.GetErrorCode(() => LivestockTradingDomainErrors.CommonErrors.IdNotValid));
entity.IsDeleted = true;
entity.DeletedAt = DateTime.UtcNow;
await _dataAccessLayer.SaveChanges();
return ArfBlocksResults.Success(new ResponseModel { Success = true });
```
