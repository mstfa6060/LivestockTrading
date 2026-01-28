$JWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZTZiYzkwOC0zODg3LTQ2YWMtYjc1MS1iMzkxNTBlYjc1NTIiLCJnaXZlbl9uYW1lIjoibW9jYWsiLCJ1bmlxdWVfbmFtZSI6Ik11c3RhZ2Egb2NhayIsImVtYWlsIjoibS5tdXN0YWZhb2Nha0BnbWFpbC5jb20iLCJidWNrZXRJZCI6IiIsInRlbmFudElkIjoiMDAwMDAwMDAtMDAwMC0wMDAwLTAwMDAtMDAwMDAwMDAwMDAwIiwicGxhdGZvcm0iOiIwIiwidXNlclNvdXJjZSI6IjAiLCJyZWZyZXNoVG9rZW5JZCI6ImU4MzU5MGYzLWEwZmUtNGM3ZS04ZDI4LTgxNDhkMTZiZDFhYyIsImNvbXBhbnlJZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsInJvbGUiOiJMaXZlc3RvY2tUcmFkaW5nLkFkbWluIiwibmJmIjoxNzY5NjAwNTY1LCJleHAiOjE3NzAyMDUzNjQsImlhdCI6MTc2OTYwMDU2NSwiYXVkIjoiYXBpLmhpcm92by5jb20ifQ.DkxeT-cDY1Tmvz2iqk8XiFhLAMRU472cPJ3jXNV2FJM"
$BASE = "http://localhost:5221"
$TS = [DateTimeOffset]::Now.ToUnixTimeSeconds()
$UserId = "0e6bc908-3887-46ac-b751-b39150eb7552"

$results = @()

function Test-Entity {
    param(
        [string]$Entity,
        [string]$CreateData1,
        [string]$CreateData2,
        [string]$UpdateDataTemplate
    )

    $result = [PSCustomObject]@{
        Entity = $Entity
        Create1 = "SKIP"
        Create2 = "SKIP"
        Update = "SKIP"
        Delete = "SKIP"
    }

    Write-Host "=== $Entity ===" -ForegroundColor Cyan

    # Create 1
    try {
        $response = Invoke-RestMethod -Uri "$BASE/$Entity/Create" -Method POST -Headers @{
            "Content-Type" = "application/json"
            "Authorization" = "Bearer $JWT"
        } -Body $CreateData1 -ErrorAction Stop
        $id1 = $response.payload.id
        if (-not $id1) { $id1 = $response.payload.Id }
        $result.Create1 = "OK"
        Write-Host "  Create 1: OK ($id1)" -ForegroundColor Green
    } catch {
        $errBody = $_.ErrorDetails.Message
        if ($errBody) {
            $err = $errBody | ConvertFrom-Json
            $result.Create1 = "FAIL: $($err.error.message)"
            Write-Host "  Create 1: FAIL - $($err.error.message)" -ForegroundColor Red
        } else {
            $result.Create1 = "FAIL: $($_.Exception.Message)"
            Write-Host "  Create 1: FAIL - $($_.Exception.Message)" -ForegroundColor Red
        }
        $id1 = $null
    }

    # Create 2
    try {
        $response = Invoke-RestMethod -Uri "$BASE/$Entity/Create" -Method POST -Headers @{
            "Content-Type" = "application/json"
            "Authorization" = "Bearer $JWT"
        } -Body $CreateData2 -ErrorAction Stop
        $id2 = $response.payload.id
        if (-not $id2) { $id2 = $response.payload.Id }
        $result.Create2 = "OK"
        Write-Host "  Create 2: OK ($id2)" -ForegroundColor Green
    } catch {
        $errBody = $_.ErrorDetails.Message
        if ($errBody) {
            $err = $errBody | ConvertFrom-Json
            $result.Create2 = "FAIL: $($err.error.message)"
            Write-Host "  Create 2: FAIL - $($err.error.message)" -ForegroundColor Red
        } else {
            $result.Create2 = "FAIL: $($_.Exception.Message)"
            Write-Host "  Create 2: FAIL - $($_.Exception.Message)" -ForegroundColor Red
        }
        $id2 = $null
    }

    # Update (first record)
    if ($id1) {
        try {
            $updateData = $UpdateDataTemplate -replace "ID_PLACEHOLDER", $id1
            $response = Invoke-RestMethod -Uri "$BASE/$Entity/Update" -Method POST -Headers @{
                "Content-Type" = "application/json"
                "Authorization" = "Bearer $JWT"
            } -Body $updateData -ErrorAction Stop
            $result.Update = "OK"
            Write-Host "  Update: OK" -ForegroundColor Green
        } catch {
            $errBody = $_.ErrorDetails.Message
            if ($errBody) {
                $err = $errBody | ConvertFrom-Json
                $result.Update = "FAIL: $($err.error.message)"
                Write-Host "  Update: FAIL - $($err.error.message)" -ForegroundColor Red
            } else {
                $result.Update = "FAIL: $($_.Exception.Message)"
                Write-Host "  Update: FAIL - $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }

    # Delete (second record)
    if ($id2) {
        try {
            $deleteData = "{`"id`": `"$id2`"}"
            $response = Invoke-RestMethod -Uri "$BASE/$Entity/Delete" -Method POST -Headers @{
                "Content-Type" = "application/json"
                "Authorization" = "Bearer $JWT"
            } -Body $deleteData -ErrorAction Stop
            $result.Delete = "OK"
            Write-Host "  Delete: OK" -ForegroundColor Green
        } catch {
            $errBody = $_.ErrorDetails.Message
            if ($errBody) {
                $err = $errBody | ConvertFrom-Json
                $result.Delete = "FAIL: $($err.error.message)"
                Write-Host "  Delete: FAIL - $($err.error.message)" -ForegroundColor Red
            } else {
                $result.Delete = "FAIL: $($_.Exception.Message)"
                Write-Host "  Delete: FAIL - $($_.Exception.Message)" -ForegroundColor Red
            }
        }
    }

    return $result
}

Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "COMPREHENSIVE CRUD TESTS - PART 4" -ForegroundColor Yellow
Write-Host "Product Detail Entities" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Yellow

# First, create dependencies
Write-Host "Creating dependencies..." -ForegroundColor Magenta

# Create a Seller first
$sellerData = "{`"userId`": `"$UserId`", `"businessName`": `"Test Seller $TS`", `"businessType`": `"Farm`", `"email`": `"test@test.com`", `"phone`": `"+905551234567`", `"isActive`": true, `"status`": 0}"
try {
    $sellerResp = Invoke-RestMethod -Uri "$BASE/Sellers/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $sellerData -ErrorAction Stop
    $SellerId = $sellerResp.payload.id
    Write-Host "  Seller created: $SellerId" -ForegroundColor Green
} catch {
    Write-Host "  Seller create failed, using existing" -ForegroundColor Yellow
    $SellerId = "00000000-0000-0000-0000-000000000001"
}

# Create a Location
$locationData = "{`"name`": `"Dep Lokasyon $TS`", `"countryCode`": `"TR`", `"city`": `"Istanbul`", `"isActive`": true}"
try {
    $locResp = Invoke-RestMethod -Uri "$BASE/Locations/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $locationData -ErrorAction Stop
    $LocationId = $locResp.payload.id
    Write-Host "  Location created: $LocationId" -ForegroundColor Green
} catch {
    Write-Host "  Location create failed" -ForegroundColor Yellow
    $LocationId = "00000000-0000-0000-0000-000000000001"
}

# Create a Category
$catData = "{`"name`": `"Dep Kategori $TS`", `"slug`": `"dep-kat-$TS`", `"isActive`": true}"
try {
    $catResp = Invoke-RestMethod -Uri "$BASE/Categories/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $catData -ErrorAction Stop
    $CategoryId = $catResp.payload.id
    Write-Host "  Category created: $CategoryId" -ForegroundColor Green
} catch {
    Write-Host "  Category create failed" -ForegroundColor Yellow
    $CategoryId = "00000000-0000-0000-0000-000000000001"
}

# Create a Brand
$brandData = "{`"name`": `"Dep Brand $TS`", `"slug`": `"dep-brand-$TS`", `"isActive`": true}"
try {
    $brandResp = Invoke-RestMethod -Uri "$BASE/Brands/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $brandData -ErrorAction Stop
    $BrandId = $brandResp.payload.id
    Write-Host "  Brand created: $BrandId" -ForegroundColor Green
} catch {
    Write-Host "  Brand create failed" -ForegroundColor Yellow
    $BrandId = "00000000-0000-0000-0000-000000000001"
}

# Create a Product for detail entities
$productData = "{`"title`": `"Test Urun Detay $TS`", `"slug`": `"test-urun-detay-$TS`", `"description`": `"Test aciklama`", `"sellerId`": `"$SellerId`", `"categoryId`": `"$CategoryId`", `"brandId`": `"$BrandId`", `"locationId`": `"$LocationId`", `"basePrice`": 1000.00, `"currency`": `"TRY`", `"stockQuantity`": 10, `"isInStock`": true, `"status`": 0, `"condition`": 0}"
try {
    $prodResp = Invoke-RestMethod -Uri "$BASE/Products/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $productData -ErrorAction Stop
    $ProductId = $prodResp.payload.id
    Write-Host "  Product created: $ProductId" -ForegroundColor Green
} catch {
    $errBody = $_.ErrorDetails.Message
    if ($errBody) {
        $err = $errBody | ConvertFrom-Json
        Write-Host "  Product create failed: $($err.error.message)" -ForegroundColor Red
    } else {
        Write-Host "  Product create failed: $($_.Exception.Message)" -ForegroundColor Red
    }
    $ProductId = "00000000-0000-0000-0000-000000000001"
}

Write-Host ""

# 1. AnimalInfos
$results += Test-Entity -Entity "AnimalInfos" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"breedName`": `"Holstein A-$TS`", `"gender`": 1, `"ageMonths`": 24, `"weightKg`": 450, `"color`": `"Siyah-Beyaz`", `"healthStatus`": 1, `"purpose`": 0}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"breedName`": `"Holstein B-$TS`", `"gender`": 0, `"ageMonths`": 36, `"weightKg`": 380, `"color`": `"Beyaz`", `"healthStatus`": 1, `"purpose`": 1}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"breedName`": `"GUNCELLENDI`", `"gender`": 1, `"ageMonths`": 30, `"weightKg`": 480, `"healthStatus`": 1, `"purpose`": 0}"

# 2. ChemicalInfos
$results += Test-Entity -Entity "ChemicalInfos" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"type`": 0, `"subType`": `"Herbisit A-$TS`", `"activeIngredients`": `"Glifosat`", `"applicationMethod`": `"Sprey`", `"toxicityLevel`": 2, `"isOrganic`": false}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"type`": 1, `"subType`": `"Insektisit B-$TS`", `"activeIngredients`": `"Piretrin`", `"applicationMethod`": `"Toz`", `"toxicityLevel`": 1, `"isOrganic`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"type`": 0, `"subType`": `"GUNCELLENDI`", `"activeIngredients`": `"Updated`", `"toxicityLevel`": 1}"

# 3. FeedInfos
$results += Test-Entity -Entity "FeedInfos" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"type`": 0, `"targetAnimal`": `"Buyukbas A-$TS`", `"targetAge`": `"Yetiskin`", `"proteinPercentage`": 18.5, `"form`": 0, `"isOrganic`": true}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"type`": 1, `"targetAnimal`": `"Kucukbas B-$TS`", `"targetAge`": `"Yavru`", `"proteinPercentage`": 22.0, `"form`": 1, `"isOrganic`": false}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"type`": 0, `"targetAnimal`": `"GUNCELLENDI`", `"proteinPercentage`": 20.0, `"form`": 0}"

# 4. MachineryInfos
$results += Test-Entity -Entity "MachineryInfos" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"type`": 0, `"model`": `"Model A-$TS`", `"yearOfManufacture`": 2022, `"powerHp`": 120, `"hasWarranty`": true}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"type`": 1, `"model`": `"Model B-$TS`", `"yearOfManufacture`": 2021, `"powerHp`": 80, `"hasWarranty`": false}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"type`": 0, `"model`": `"GUNCELLENDI`", `"yearOfManufacture`": 2023, `"powerHp`": 150}"

# 5. SeedInfos
$results += Test-Entity -Entity "SeedInfos" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"type`": 0, `"variety`": `"Variety A-$TS`", `"scientificName`": `"Test scientificus`", `"germinationRate`": 95.5, `"isOrganic`": true, `"isGMO`": false}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"type`": 1, `"variety`": `"Variety B-$TS`", `"scientificName`": `"Test botanicus`", `"germinationRate`": 92.0, `"isOrganic`": false, `"isGMO`": false}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"type`": 0, `"variety`": `"GUNCELLENDI`", `"germinationRate`": 98.0}"

# 6. VeterinaryInfos
$results += Test-Entity -Entity "VeterinaryInfos" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"type`": 0, `"therapeuticCategory`": `"Antibiyotik A-$TS`", `"targetSpecies`": `"Buyukbas`", `"activeIngredients`": `"Amoksisilin`", `"route`": 0, `"requiresPrescription`": true}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"type`": 1, `"therapeuticCategory`": `"Vitamin B-$TS`", `"targetSpecies`": `"Kucukbas`", `"activeIngredients`": `"B12`", `"route`": 1, `"requiresPrescription`": false}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"type`": 0, `"therapeuticCategory`": `"GUNCELLENDI`", `"targetSpecies`": `"Tum hayvanlar`", `"route`": 0}"

# Summary
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "SONUCLAR - PART 4" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

$results | Format-Table -AutoSize

$successCount = ($results | Where-Object { $_.Create1 -eq "OK" -and $_.Create2 -eq "OK" -and $_.Update -eq "OK" -and $_.Delete -eq "OK" }).Count
$totalCount = $results.Count

Write-Host "`nToplam: $successCount / $totalCount basarili" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
