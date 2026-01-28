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
            $result.Create1 = "FAIL"
            Write-Host "  Create 1: FAIL" -ForegroundColor Red
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
            $result.Create2 = "FAIL"
            Write-Host "  Create 2: FAIL" -ForegroundColor Red
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
                $result.Update = "FAIL"
                Write-Host "  Update: FAIL" -ForegroundColor Red
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
                $result.Delete = "FAIL"
                Write-Host "  Delete: FAIL" -ForegroundColor Red
            }
        }
    }

    return $result
}

Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "COMPREHENSIVE CRUD TESTS - PART 3" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Yellow

# Create dependencies first
Write-Host "Creating dependencies..." -ForegroundColor Magenta

# Create Seller
$sellerData = "{`"userId`": `"$UserId`", `"businessName`": `"Dep Seller $TS`", `"businessType`": `"Farm`", `"email`": `"dep@test.com`", `"phone`": `"+905559999999`", `"isActive`": true, `"status`": 0}"
try {
    $sellerResp = Invoke-RestMethod -Uri "$BASE/Sellers/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $sellerData -ErrorAction Stop
    $SellerId = $sellerResp.payload.id
    Write-Host "  Seller: $SellerId" -ForegroundColor Green
} catch { $SellerId = $null }

# Create Location
$locationData = "{`"name`": `"Dep Loc $TS`", `"countryCode`": `"TR`", `"city`": `"Istanbul`", `"isActive`": true}"
try {
    $locResp = Invoke-RestMethod -Uri "$BASE/Locations/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $locationData -ErrorAction Stop
    $LocationId = $locResp.payload.id
    Write-Host "  Location: $LocationId" -ForegroundColor Green
} catch { $LocationId = $null }

# Create Category
$catData = "{`"name`": `"Dep Cat $TS`", `"slug`": `"dep-cat-$TS`", `"isActive`": true}"
try {
    $catResp = Invoke-RestMethod -Uri "$BASE/Categories/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $catData -ErrorAction Stop
    $CategoryId = $catResp.payload.id
    Write-Host "  Category: $CategoryId" -ForegroundColor Green
} catch { $CategoryId = $null }

# Create Brand
$brandData = "{`"name`": `"Dep Brand $TS`", `"slug`": `"dep-brand-$TS`", `"isActive`": true}"
try {
    $brandResp = Invoke-RestMethod -Uri "$BASE/Brands/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $brandData -ErrorAction Stop
    $BrandId = $brandResp.payload.id
    Write-Host "  Brand: $BrandId" -ForegroundColor Green
} catch { $BrandId = $null }

# Create Product
$productData = "{`"title`": `"Dep Product $TS`", `"slug`": `"dep-product-$TS`", `"description`": `"Test`", `"categoryId`": `"$CategoryId`", `"brandId`": `"$BrandId`", `"sellerId`": `"$SellerId`", `"locationId`": `"$LocationId`", `"basePrice`": 1000, `"status`": 0, `"isInStock`": true}"
try {
    $prodResp = Invoke-RestMethod -Uri "$BASE/Products/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $productData -ErrorAction Stop
    $ProductId = $prodResp.payload.id
    Write-Host "  Product: $ProductId" -ForegroundColor Green
} catch {
    Write-Host "  Product create failed: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    $ProductId = $null
}

Write-Host ""

# Test entities that require Product
if ($ProductId) {
    # ProductImages
    $results += Test-Entity -Entity "ProductImages" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"imageUrl`": `"https://example.com/img1.jpg`", `"sortOrder`": 1, `"isPrimary`": true}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"imageUrl`": `"https://example.com/img2.jpg`", `"sortOrder`": 2, `"isPrimary`": false}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"imageUrl`": `"https://example.com/updated.jpg`", `"sortOrder`": 1, `"isPrimary`": true}"

    # ProductVideos
    $results += Test-Entity -Entity "ProductVideos" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"videoUrl`": `"https://youtube.com/watch?v=a1`", `"title`": `"Video A`", `"sortOrder`": 1}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"videoUrl`": `"https://youtube.com/watch?v=b1`", `"title`": `"Video B`", `"sortOrder`": 2}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"videoUrl`": `"https://youtube.com/updated`", `"title`": `"Updated`", `"sortOrder`": 1}"

    # ProductDocuments
    $results += Test-Entity -Entity "ProductDocuments" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"documentUrl`": `"https://example.com/doc1.pdf`", `"documentType`": `"certificate`", `"title`": `"Doc A`"}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"documentUrl`": `"https://example.com/doc2.pdf`", `"documentType`": `"manual`", `"title`": `"Doc B`"}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"documentUrl`": `"https://example.com/updated.pdf`", `"documentType`": `"updated`", `"title`": `"Updated`"}"

    # ProductPrices
    $results += Test-Entity -Entity "ProductPrices" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"price`": 1000, `"currencyCode`": `"TRY`", `"priceType`": `"retail`"}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"price`": 900, `"currencyCode`": `"USD`", `"priceType`": `"wholesale`"}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"price`": 1500, `"currencyCode`": `"EUR`", `"priceType`": `"retail`"}"

    # ProductVariants
    $results += Test-Entity -Entity "ProductVariants" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"name`": `"Variant A`", `"sku`": `"SKU-A-$TS`", `"price`": 1100, `"stockQuantity`": 10}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"name`": `"Variant B`", `"sku`": `"SKU-B-$TS`", `"price`": 1200, `"stockQuantity`": 20}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"name`": `"Updated Variant`", `"sku`": `"SKU-U-$TS`", `"price`": 1300, `"stockQuantity`": 15}"

    # ProductViewHistories
    $results += Test-Entity -Entity "ProductViewHistories" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"userId`": `"$UserId`", `"viewedAt`": `"2026-01-28T12:00:00`"}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"userId`": `"$UserId`", `"viewedAt`": `"2026-01-28T13:00:00`"}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"userId`": `"$UserId`", `"viewedAt`": `"2026-01-28T14:00:00`"}"

    # FavoriteProducts
    $results += Test-Entity -Entity "FavoriteProducts" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"userId`": `"$UserId`"}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"userId`": `"$UserId`"}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"userId`": `"$UserId`"}"

    # ProductReviews
    $results += Test-Entity -Entity "ProductReviews" `
        -CreateData1 "{`"productId`": `"$ProductId`", `"userId`": `"$UserId`", `"rating`": 5, `"title`": `"Great A`", `"comment`": `"Comment A`"}" `
        -CreateData2 "{`"productId`": `"$ProductId`", `"userId`": `"$UserId`", `"rating`": 4, `"title`": `"Good B`", `"comment`": `"Comment B`"}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"userId`": `"$UserId`", `"rating`": 3, `"title`": `"Updated`", `"comment`": `"Updated Comment`"}"
}

# Test Seller-related entities
if ($SellerId) {
    # SellerReviews
    $results += Test-Entity -Entity "SellerReviews" `
        -CreateData1 "{`"sellerId`": `"$SellerId`", `"userId`": `"$UserId`", `"rating`": 5, `"comment`": `"Comment A`"}" `
        -CreateData2 "{`"sellerId`": `"$SellerId`", `"userId`": `"$UserId`", `"rating`": 4, `"comment`": `"Comment B`"}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"sellerId`": `"$SellerId`", `"userId`": `"$UserId`", `"rating`": 3, `"comment`": `"Updated`"}"

    # ShippingRates
    $results += Test-Entity -Entity "ShippingRates" `
        -CreateData1 "{`"sellerId`": `"$SellerId`", `"name`": `"Rate A`", `"price`": 50, `"countryCode`": `"TR`", `"isActive`": true}" `
        -CreateData2 "{`"sellerId`": `"$SellerId`", `"name`": `"Rate B`", `"price`": 100, `"countryCode`": `"DE`", `"isActive`": true}" `
        -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"sellerId`": `"$SellerId`", `"name`": `"Updated`", `"price`": 75, `"countryCode`": `"US`", `"isActive`": true}"
}

# Summary
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "SONUCLAR - PART 3" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

$results | Format-Table -AutoSize

$successCount = ($results | Where-Object { $_.Create1 -eq "OK" -and $_.Create2 -eq "OK" -and $_.Update -eq "OK" -and $_.Delete -eq "OK" }).Count
$totalCount = $results.Count

Write-Host "`nToplam: $successCount / $totalCount basarili" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
