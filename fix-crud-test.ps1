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
Write-Host "FIX CRUD TESTS - Sorunlu Entity'ler" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Yellow

# Dependencies
Write-Host "Creating dependencies..." -ForegroundColor Magenta

# Create a Seller
$sellerData = "{`"userId`": `"$UserId`", `"businessName`": `"Test Seller Fix $TS`", `"businessType`": `"Farm`", `"email`": `"testfix@test.com`", `"phone`": `"+905551234567`", `"isActive`": true, `"status`": 0}"
try {
    $sellerResp = Invoke-RestMethod -Uri "$BASE/Sellers/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $sellerData -ErrorAction Stop
    $SellerId = $sellerResp.payload.id
    Write-Host "  Seller created: $SellerId" -ForegroundColor Green
} catch {
    $SellerId = "00000000-0000-0000-0000-000000000001"
}

# Create a Category
$catData = "{`"name`": `"Fix Kategori $TS`", `"slug`": `"fix-kat-$TS`", `"isActive`": true}"
try {
    $catResp = Invoke-RestMethod -Uri "$BASE/Categories/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $catData -ErrorAction Stop
    $CategoryId = $catResp.payload.id
    Write-Host "  Category created: $CategoryId" -ForegroundColor Green
} catch {
    $CategoryId = "00000000-0000-0000-0000-000000000001"
}

# Create a Location
$locationData = "{`"name`": `"Fix Lokasyon $TS`", `"countryCode`": `"TR`", `"city`": `"Istanbul`", `"isActive`": true}"
try {
    $locResp = Invoke-RestMethod -Uri "$BASE/Locations/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $locationData -ErrorAction Stop
    $LocationId = $locResp.payload.id
    Write-Host "  Location created: $LocationId" -ForegroundColor Green
} catch {
    $LocationId = "00000000-0000-0000-0000-000000000001"
}

# Create a Product
$productData = "{`"title`": `"Fix Urun $TS`", `"slug`": `"fix-urun-$TS`", `"description`": `"Test`", `"sellerId`": `"$SellerId`", `"categoryId`": `"$CategoryId`", `"locationId`": `"$LocationId`", `"basePrice`": 1000.00, `"currency`": `"TRY`", `"stockQuantity`": 10, `"isInStock`": true, `"status`": 0, `"condition`": 0}"
try {
    $prodResp = Invoke-RestMethod -Uri "$BASE/Products/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $productData -ErrorAction Stop
    $ProductId = $prodResp.payload.id
    Write-Host "  Product created: $ProductId" -ForegroundColor Green
} catch {
    $ProductId = "00000000-0000-0000-0000-000000000001"
}

# ShippingZone
$zoneData = "{`"name`": `"Fix Zone $TS`", `"countryCodes`": `"TR`", `"isActive`": true}"
try {
    $zoneResp = Invoke-RestMethod -Uri "$BASE/ShippingZones/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $zoneData -ErrorAction Stop
    $ShippingZoneId = $zoneResp.payload.id
    Write-Host "  ShippingZone created: $ShippingZoneId" -ForegroundColor Green
} catch {
    $ShippingZoneId = "00000000-0000-0000-0000-000000000001"
}

# ShippingCarrier
$carrierData = "{`"name`": `"Fix Carrier $TS`", `"code`": `"FC$TS`", `"isActive`": true}"
try {
    $carrierResp = Invoke-RestMethod -Uri "$BASE/ShippingCarriers/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $carrierData -ErrorAction Stop
    $ShippingCarrierId = $carrierResp.payload.id
    Write-Host "  ShippingCarrier created: $ShippingCarrierId" -ForegroundColor Green
} catch {
    $ShippingCarrierId = "00000000-0000-0000-0000-000000000001"
}

Write-Host ""

# 1. TaxRates - Check required fields
$results += Test-Entity -Entity "TaxRates" `
    -CreateData1 "{`"countryCode`": `"TR`", `"taxName`": `"KDV A-$TS`", `"rate`": 18.0, `"type`": 0, `"isActive`": true}" `
    -CreateData2 "{`"countryCode`": `"TR`", `"taxName`": `"KDV B-$TS`", `"rate`": 8.0, `"type`": 1, `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"countryCode`": `"TR`", `"taxName`": `"GUNCELLENDI`", `"rate`": 20.0, `"type`": 0, `"isActive`": true}"

# 2. Conversations - FIXED property names
$results += Test-Entity -Entity "Conversations" `
    -CreateData1 "{`"participantUserId1`": `"$UserId`", `"participantUserId2`": `"$UserId`", `"subject`": `"Test Konusma A-$TS`", `"status`": 0}" `
    -CreateData2 "{`"participantUserId1`": `"$UserId`", `"participantUserId2`": `"$UserId`", `"subject`": `"Test Konusma B-$TS`", `"status`": 0}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"participantUserId1`": `"$UserId`", `"participantUserId2`": `"$UserId`", `"subject`": `"GUNCELLENDI`", `"status`": 1}"

# 3. Transporters - FIXED property names (no vehicleTypes, status)
$results += Test-Entity -Entity "Transporters" `
    -CreateData1 "{`"userId`": `"$UserId`", `"companyName`": `"Test Nakliye A-$TS`", `"email`": `"nakliyeA@test.com`", `"phone`": `"+905551111111`", `"isActive`": true}" `
    -CreateData2 "{`"userId`": `"$UserId`", `"companyName`": `"Test Nakliye B-$TS`", `"email`": `"nakliyeB@test.com`", `"phone`": `"+905552222222`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"userId`": `"$UserId`", `"companyName`": `"GUNCELLENDI`", `"email`": `"updated@test.com`", `"phone`": `"+905553333333`", `"isActive`": true}"

# 4. ProductDocuments
$results += Test-Entity -Entity "ProductDocuments" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"documentUrl`": `"https://example.com/docA.pdf`", `"fileName`": `"DocA.pdf`", `"title`": `"Document A-$TS`", `"type`": 0, `"sortOrder`": 0}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"documentUrl`": `"https://example.com/docB.pdf`", `"fileName`": `"DocB.pdf`", `"title`": `"Document B-$TS`", `"type`": 1, `"sortOrder`": 1}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"documentUrl`": `"https://example.com/updated.pdf`", `"fileName`": `"Updated.pdf`", `"title`": `"GUNCELLENDI`", `"type`": 0, `"sortOrder`": 0}"

# 5. SellerReviews
$results += Test-Entity -Entity "SellerReviews" `
    -CreateData1 "{`"sellerId`": `"$SellerId`", `"userId`": `"$UserId`", `"overallRating`": 5, `"communicationRating`": 5, `"shippingSpeedRating`": 5, `"productQualityRating`": 5, `"title`": `"Harika A-$TS`", `"comment`": `"Cok iyi`", `"isVerifiedPurchase`": true}" `
    -CreateData2 "{`"sellerId`": `"$SellerId`", `"userId`": `"$UserId`", `"overallRating`": 4, `"communicationRating`": 4, `"shippingSpeedRating`": 4, `"productQualityRating`": 4, `"title`": `"Iyi B-$TS`", `"comment`": `"Iyi`", `"isVerifiedPurchase`": false}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"sellerId`": `"$SellerId`", `"userId`": `"$UserId`", `"overallRating`": 5, `"communicationRating`": 5, `"shippingSpeedRating`": 5, `"productQualityRating`": 5, `"title`": `"GUNCELLENDI`", `"comment`": `"Updated`"}"

# 6. ShippingRates
$results += Test-Entity -Entity "ShippingRates" `
    -CreateData1 "{`"shippingZoneId`": `"$ShippingZoneId`", `"shippingCarrierId`": `"$ShippingCarrierId`", `"shippingCost`": 50.0, `"currency`": `"TRY`", `"isActive`": true}" `
    -CreateData2 "{`"shippingZoneId`": `"$ShippingZoneId`", `"shippingCarrierId`": `"$ShippingCarrierId`", `"shippingCost`": 75.0, `"currency`": `"TRY`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"shippingZoneId`": `"$ShippingZoneId`", `"shippingCarrierId`": `"$ShippingCarrierId`", `"shippingCost`": 60.0, `"currency`": `"TRY`", `"isActive`": true}"

# Summary
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "SONUCLAR - FIX TEST" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

$results | Format-Table -AutoSize

$successCount = ($results | Where-Object { $_.Create1 -eq "OK" -and $_.Create2 -eq "OK" -and $_.Update -eq "OK" -and $_.Delete -eq "OK" }).Count
$totalCount = $results.Count

Write-Host "`nToplam: $successCount / $totalCount basarili" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
