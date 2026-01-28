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
Write-Host "COMPREHENSIVE CRUD TESTS - PART 2" -ForegroundColor Yellow
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

Write-Host ""

# 11. Banners (requires startDate, endDate)
$startDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
$endDate = (Get-Date).AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ss")
$results += Test-Entity -Entity "Banners" `
    -CreateData1 "{`"title`": `"Test Banner A-$TS`", `"imageUrl`": `"https://example.com/a.jpg`", `"linkUrl`": `"https://example.com`", `"position`": 0, `"startDate`": `"$startDate`", `"endDate`": `"$endDate`", `"isActive`": true}" `
    -CreateData2 "{`"title`": `"Test Banner B-$TS`", `"imageUrl`": `"https://example.com/b.jpg`", `"linkUrl`": `"https://example.com`", `"position`": 1, `"startDate`": `"$startDate`", `"endDate`": `"$endDate`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"title`": `"GUNCELLENDI`", `"imageUrl`": `"https://example.com/u.jpg`", `"position`": 0, `"startDate`": `"$startDate`", `"endDate`": `"$endDate`", `"isActive`": true}"

# 12. Farms
$results += Test-Entity -Entity "Farms" `
    -CreateData1 "{`"name`": `"Test Ciftlik A-$TS`", `"sellerId`": `"$SellerId`", `"locationId`": `"$LocationId`", `"farmType`": `"Dairy`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Ciftlik B-$TS`", `"sellerId`": `"$SellerId`", `"locationId`": `"$LocationId`", `"farmType`": `"Livestock`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI`", `"sellerId`": `"$SellerId`", `"locationId`": `"$LocationId`", `"farmType`": `"Mixed`", `"isActive`": true}"

# 13. Notifications
$results += Test-Entity -Entity "Notifications" `
    -CreateData1 "{`"userId`": `"$UserId`", `"title`": `"Test Bildirim A-$TS`", `"message`": `"Test mesaj A`", `"type`": 0, `"isRead`": false}" `
    -CreateData2 "{`"userId`": `"$UserId`", `"title`": `"Test Bildirim B-$TS`", `"message`": `"Test mesaj B`", `"type`": 1, `"isRead`": false}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"userId`": `"$UserId`", `"title`": `"GUNCELLENDI`", `"message`": `"Updated`", `"type`": 0, `"isRead`": true}"

# 14. Conversations
$results += Test-Entity -Entity "Conversations" `
    -CreateData1 "{`"participant1Id`": `"$UserId`", `"participant2Id`": `"$UserId`", `"isActive`": true}" `
    -CreateData2 "{`"participant1Id`": `"$UserId`", `"participant2Id`": `"$UserId`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"participant1Id`": `"$UserId`", `"participant2Id`": `"$UserId`", `"isActive`": false}"

# 15. UserPreferences
$results += Test-Entity -Entity "UserPreferences" `
    -CreateData1 "{`"userId`": `"$UserId`", `"preferenceKey`": `"test_key_a_$TS`", `"preferenceValue`": `"value_a`"}" `
    -CreateData2 "{`"userId`": `"$UserId`", `"preferenceKey`": `"test_key_b_$TS`", `"preferenceValue`": `"value_b`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"userId`": `"$UserId`", `"preferenceKey`": `"updated_key`", `"preferenceValue`": `"updated_value`"}"

# 16. SearchHistories
$results += Test-Entity -Entity "SearchHistories" `
    -CreateData1 "{`"userId`": `"$UserId`", `"searchQuery`": `"test search A-$TS`", `"searchType`": `"product`"}" `
    -CreateData2 "{`"userId`": `"$UserId`", `"searchQuery`": `"test search B-$TS`", `"searchType`": `"seller`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"userId`": `"$UserId`", `"searchQuery`": `"updated search`", `"searchType`": `"product`"}"

# 17. Transporters
$results += Test-Entity -Entity "Transporters" `
    -CreateData1 "{`"userId`": `"$UserId`", `"companyName`": `"Test Nakliye A-$TS`", `"vehicleTypes`": `"Truck,Van`", `"isActive`": true, `"status`": 0}" `
    -CreateData2 "{`"userId`": `"$UserId`", `"companyName`": `"Test Nakliye B-$TS`", `"vehicleTypes`": `"Truck`", `"isActive`": true, `"status`": 0}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"userId`": `"$UserId`", `"companyName`": `"GUNCELLENDI`", `"vehicleTypes`": `"Van`", `"isActive`": true, `"status`": 0}"

# Summary
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "SONUCLAR - PART 2" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

$results | Format-Table -AutoSize

$successCount = ($results | Where-Object { $_.Create1 -eq "OK" -and $_.Create2 -eq "OK" -and $_.Update -eq "OK" -and $_.Delete -eq "OK" }).Count
$totalCount = $results.Count

Write-Host "`nToplam: $successCount / $totalCount basarili" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
