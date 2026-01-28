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
Write-Host "COMPREHENSIVE CRUD TESTS - PART 6" -ForegroundColor Yellow
Write-Host "Products, Sellers, TransportRequests" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Yellow

# First, create dependencies
Write-Host "Creating dependencies..." -ForegroundColor Magenta

# Create a Location
$locationData = "{`"name`": `"Test Lokasyon $TS`", `"countryCode`": `"TR`", `"city`": `"Istanbul`", `"isActive`": true}"
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

$locationData2 = "{`"name`": `"Test Lokasyon2 $TS`", `"countryCode`": `"TR`", `"city`": `"Ankara`", `"isActive`": true}"
try {
    $locResp2 = Invoke-RestMethod -Uri "$BASE/Locations/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $locationData2 -ErrorAction Stop
    $LocationId2 = $locResp2.payload.id
    Write-Host "  Location2 created: $LocationId2" -ForegroundColor Green
} catch {
    Write-Host "  Location2 create failed" -ForegroundColor Yellow
    $LocationId2 = "00000000-0000-0000-0000-000000000002"
}

# Create a Category
$catData = "{`"name`": `"Dep Kategori $TS`", `"slug`": `"dep-kat6-$TS`", `"isActive`": true}"
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

Write-Host ""

# 1. Sellers (full CRUD)
$results += Test-Entity -Entity "Sellers" `
    -CreateData1 "{`"userId`": `"$UserId`", `"businessName`": `"Test Satici A-$TS`", `"businessType`": `"Farm`", `"email`": `"sellerA@test.com`", `"phone`": `"+905551111111`", `"isActive`": true, `"status`": 0}" `
    -CreateData2 "{`"userId`": `"$UserId`", `"businessName`": `"Test Satici B-$TS`", `"businessType`": `"Trader`", `"email`": `"sellerB@test.com`", `"phone`": `"+905552222222`", `"isActive`": true, `"status`": 0}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"userId`": `"$UserId`", `"businessName`": `"GUNCELLENDI`", `"businessType`": `"Farm`", `"email`": `"updated@test.com`", `"phone`": `"+905553333333`", `"isActive`": true, `"status`": 0}"

# Get seller ID for Products
$sellerForProducts = $null
try {
    $sellerResp = Invoke-RestMethod -Uri "$BASE/Sellers/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body "{`"userId`": `"$UserId`", `"businessName`": `"Seller For Products $TS`", `"businessType`": `"Farm`", `"email`": `"sellerprod@test.com`", `"phone`": `"+905554444444`", `"isActive`": true, `"status`": 0}" -ErrorAction Stop
    $sellerForProducts = $sellerResp.payload.id
    Write-Host "  Seller for Products: $sellerForProducts" -ForegroundColor Green
} catch {
    Write-Host "  Seller for products failed" -ForegroundColor Yellow
    $sellerForProducts = "00000000-0000-0000-0000-000000000001"
}

# 2. Products (full CRUD)
$results += Test-Entity -Entity "Products" `
    -CreateData1 "{`"title`": `"Test Urun A-$TS`", `"slug`": `"test-urun-a-$TS`", `"description`": `"Aciklama A`", `"sellerId`": `"$sellerForProducts`", `"categoryId`": `"$CategoryId`", `"locationId`": `"$LocationId`", `"basePrice`": 1500.00, `"currency`": `"TRY`", `"stockQuantity`": 10, `"isInStock`": true, `"status`": 0, `"condition`": 0}" `
    -CreateData2 "{`"title`": `"Test Urun B-$TS`", `"slug`": `"test-urun-b-$TS`", `"description`": `"Aciklama B`", `"sellerId`": `"$sellerForProducts`", `"categoryId`": `"$CategoryId`", `"locationId`": `"$LocationId`", `"basePrice`": 2500.00, `"currency`": `"TRY`", `"stockQuantity`": 5, `"isInStock`": true, `"status`": 0, `"condition`": 1}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"title`": `"GUNCELLENDI`", `"slug`": `"guncellendi-$TS`", `"description`": `"Updated Aciklama`", `"sellerId`": `"$sellerForProducts`", `"categoryId`": `"$CategoryId`", `"locationId`": `"$LocationId`", `"basePrice`": 2000.00, `"currency`": `"TRY`", `"stockQuantity`": 15, `"isInStock`": true, `"status`": 0, `"condition`": 0}"

# Get product ID for TransportRequests
$productForTransport = $null
try {
    $prodResp = Invoke-RestMethod -Uri "$BASE/Products/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body "{`"title`": `"Product For Transport $TS`", `"slug`": `"prod-transport-$TS`", `"description`": `"Transport product`", `"sellerId`": `"$sellerForProducts`", `"categoryId`": `"$CategoryId`", `"locationId`": `"$LocationId`", `"basePrice`": 3000.00, `"currency`": `"TRY`", `"stockQuantity`": 3, `"isInStock`": true, `"status`": 0, `"condition`": 0}" -ErrorAction Stop
    $productForTransport = $prodResp.payload.id
    Write-Host "  Product for Transport: $productForTransport" -ForegroundColor Green
} catch {
    Write-Host "  Product for transport failed" -ForegroundColor Yellow
    $productForTransport = "00000000-0000-0000-0000-000000000001"
}

# 3. TransportRequests (full CRUD)
$results += Test-Entity -Entity "TransportRequests" `
    -CreateData1 "{`"productId`": `"$productForTransport`", `"sellerId`": `"$sellerForProducts`", `"buyerId`": `"$UserId`", `"pickupLocationId`": `"$LocationId`", `"deliveryLocationId`": `"$LocationId2`", `"currency`": `"TRY`", `"transportType`": 0, `"status`": 0, `"notes`": `"Not A-$TS`"}" `
    -CreateData2 "{`"productId`": `"$productForTransport`", `"sellerId`": `"$sellerForProducts`", `"buyerId`": `"$UserId`", `"pickupLocationId`": `"$LocationId`", `"deliveryLocationId`": `"$LocationId2`", `"currency`": `"TRY`", `"transportType`": 1, `"status`": 0, `"notes`": `"Not B-$TS`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$productForTransport`", `"sellerId`": `"$sellerForProducts`", `"buyerId`": `"$UserId`", `"pickupLocationId`": `"$LocationId`", `"deliveryLocationId`": `"$LocationId2`", `"currency`": `"TRY`", `"transportType`": 0, `"status`": 1, `"notes`": `"GUNCELLENDI`"}"

# Summary
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "SONUCLAR - PART 6" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

$results | Format-Table -AutoSize

$successCount = ($results | Where-Object { $_.Create1 -eq "OK" -and $_.Create2 -eq "OK" -and $_.Update -eq "OK" -and $_.Delete -eq "OK" }).Count
$totalCount = $results.Count

Write-Host "`nToplam: $successCount / $totalCount basarili" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
