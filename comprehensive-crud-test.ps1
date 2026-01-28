$JWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIwZTZiYzkwOC0zODg3LTQ2YWMtYjc1MS1iMzkxNTBlYjc1NTIiLCJnaXZlbl9uYW1lIjoibW9jYWsiLCJ1bmlxdWVfbmFtZSI6Ik11c3RhZ2Egb2NhayIsImVtYWlsIjoibS5tdXN0YWZhb2Nha0BnbWFpbC5jb20iLCJidWNrZXRJZCI6IiIsInRlbmFudElkIjoiMDAwMDAwMDAtMDAwMC0wMDAwLTAwMDAtMDAwMDAwMDAwMDAwIiwicGxhdGZvcm0iOiIwIiwidXNlclNvdXJjZSI6IjAiLCJyZWZyZXNoVG9rZW5JZCI6ImU4MzU5MGYzLWEwZmUtNGM3ZS04ZDI4LTgxNDhkMTZiZDFhYyIsImNvbXBhbnlJZCI6IjAwMDAwMDAwLTAwMDAtMDAwMC0wMDAwLTAwMDAwMDAwMDAwMCIsInJvbGUiOiJMaXZlc3RvY2tUcmFkaW5nLkFkbWluIiwibmJmIjoxNzY5NjAwNTY1LCJleHAiOjE3NzAyMDUzNjQsImlhdCI6MTc2OTYwMDU2NSwiYXVkIjoiYXBpLmhpcm92by5jb20ifQ.DkxeT-cDY1Tmvz2iqk8XiFhLAMRU472cPJ3jXNV2FJM"
$BASE = "http://localhost:5221"
$TS = [DateTimeOffset]::Now.ToUnixTimeSeconds()

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
        $result.Create1 = "OK ($id1)"
        Write-Host "  Create 1: OK" -ForegroundColor Green
    } catch {
        $err = $_.ErrorDetails.Message | ConvertFrom-Json
        $result.Create1 = "FAIL: $($err.error.message)"
        Write-Host "  Create 1: FAIL - $($err.error.message)" -ForegroundColor Red
        $id1 = $null
    }

    # Create 2
    try {
        $response = Invoke-RestMethod -Uri "$BASE/$Entity/Create" -Method POST -Headers @{
            "Content-Type" = "application/json"
            "Authorization" = "Bearer $JWT"
        } -Body $CreateData2 -ErrorAction Stop
        $id2 = $response.payload.id
        $result.Create2 = "OK ($id2)"
        Write-Host "  Create 2: OK" -ForegroundColor Green
    } catch {
        $err = $_.ErrorDetails.Message | ConvertFrom-Json
        $result.Create2 = "FAIL: $($err.error.message)"
        Write-Host "  Create 2: FAIL - $($err.error.message)" -ForegroundColor Red
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
            $err = $_.ErrorDetails.Message | ConvertFrom-Json
            $result.Update = "FAIL: $($err.error.message)"
            Write-Host "  Update: FAIL - $($err.error.message)" -ForegroundColor Red
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
            $err = $_.ErrorDetails.Message | ConvertFrom-Json
            $result.Delete = "FAIL: $($err.error.message)"
            Write-Host "  Delete: FAIL - $($err.error.message)" -ForegroundColor Red
        }
    }

    return $result
}

Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "COMPREHENSIVE CRUD TESTS" -ForegroundColor Yellow
Write-Host "========================================`n" -ForegroundColor Yellow

# 1. Categories
$results += Test-Entity -Entity "Categories" `
    -CreateData1 "{`"name`": `"Test Kategori A-$TS`", `"slug`": `"test-kat-a-$TS`", `"description`": `"Test`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Kategori B-$TS`", `"slug`": `"test-kat-b-$TS`", `"description`": `"Test`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI-$TS`", `"slug`": `"updated-$TS`", `"isActive`": true}"

# 2. Brands
$results += Test-Entity -Entity "Brands" `
    -CreateData1 "{`"name`": `"Test Marka A-$TS`", `"slug`": `"test-marka-a-$TS`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Marka B-$TS`", `"slug`": `"test-marka-b-$TS`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI-$TS`", `"slug`": `"updated-marka-$TS`", `"isActive`": true}"

# 3. Languages
$results += Test-Entity -Entity "Languages" `
    -CreateData1 "{`"code`": `"ta$TS`", `"name`": `"Test Dil A`", `"nativeName`": `"Test A`", `"isActive`": true}" `
    -CreateData2 "{`"code`": `"tb$TS`", `"name`": `"Test Dil B`", `"nativeName`": `"Test B`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"code`": `"tu$TS`", `"name`": `"GUNCELLENDI`", `"nativeName`": `"Updated`", `"isActive`": true}"

# 4. Currencies
$results += Test-Entity -Entity "Currencies" `
    -CreateData1 "{`"code`": `"TA$TS`", `"name`": `"Test Currency A`", `"symbol`": `"TA`", `"isActive`": true}" `
    -CreateData2 "{`"code`": `"TB$TS`", `"name`": `"Test Currency B`", `"symbol`": `"TB`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"code`": `"TU$TS`", `"name`": `"GUNCELLENDI`", `"symbol`": `"TU`", `"isActive`": true}"

# 5. FAQs
$results += Test-Entity -Entity "FAQs" `
    -CreateData1 "{`"question`": `"Test Soru A-$TS?`", `"answer`": `"Test Cevap A`", `"sortOrder`": 1, `"isActive`": true}" `
    -CreateData2 "{`"question`": `"Test Soru B-$TS?`", `"answer`": `"Test Cevap B`", `"sortOrder`": 2, `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"question`": `"GUNCELLENDI?`", `"answer`": `"Updated`", `"sortOrder`": 1, `"isActive`": true}"

# 6. ShippingCarriers
$results += Test-Entity -Entity "ShippingCarriers" `
    -CreateData1 "{`"name`": `"Test Kargo A-$TS`", `"code`": `"KA$TS`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Kargo B-$TS`", `"code`": `"KB$TS`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI`", `"code`": `"KU$TS`", `"isActive`": true}"

# 7. ShippingZones
$results += Test-Entity -Entity "ShippingZones" `
    -CreateData1 "{`"name`": `"Test Zone A-$TS`", `"countryCode`": `"TR`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Zone B-$TS`", `"countryCode`": `"DE`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI`", `"countryCode`": `"US`", `"isActive`": true}"

# 8. PaymentMethods
$results += Test-Entity -Entity "PaymentMethods" `
    -CreateData1 "{`"name`": `"Test Odeme A-$TS`", `"code`": `"PA$TS`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Odeme B-$TS`", `"code`": `"PB$TS`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI`", `"code`": `"PU$TS`", `"isActive`": true}"

# 9. TaxRates
$results += Test-Entity -Entity "TaxRates" `
    -CreateData1 "{`"name`": `"Test Vergi A-$TS`", `"rate`": 18, `"countryCode`": `"TR`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Vergi B-$TS`", `"rate`": 20, `"countryCode`": `"DE`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI`", `"rate`": 25, `"countryCode`": `"US`", `"isActive`": true}"

# 10. Locations
$results += Test-Entity -Entity "Locations" `
    -CreateData1 "{`"name`": `"Test Lokasyon A-$TS`", `"countryCode`": `"TR`", `"city`": `"Istanbul`", `"district`": `"Kadikoy`", `"isActive`": true}" `
    -CreateData2 "{`"name`": `"Test Lokasyon B-$TS`", `"countryCode`": `"TR`", `"city`": `"Ankara`", `"district`": `"Cankaya`", `"isActive`": true}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"name`": `"GUNCELLENDI`", `"countryCode`": `"TR`", `"city`": `"Izmir`", `"isActive`": true}"

# Summary
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "SONUCLAR" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

$results | Format-Table -AutoSize

$successCount = ($results | Where-Object { $_.Create1 -like "OK*" -and $_.Create2 -like "OK*" -and $_.Update -eq "OK" -and $_.Delete -eq "OK" }).Count
$totalCount = $results.Count

Write-Host "`nToplam: $successCount / $totalCount basarili" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
