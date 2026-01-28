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
Write-Host "COMPREHENSIVE CRUD TESTS - PART 5" -ForegroundColor Yellow
Write-Host "Relational & Transport Entities" -ForegroundColor Yellow
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

# Create Locations (pickup and delivery)
$locationData = "{`"name`": `"Pickup Lokasyon $TS`", `"countryCode`": `"TR`", `"city`": `"Istanbul`", `"isActive`": true}"
try {
    $locResp = Invoke-RestMethod -Uri "$BASE/Locations/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $locationData -ErrorAction Stop
    $PickupLocationId = $locResp.payload.id
    Write-Host "  Pickup Location created: $PickupLocationId" -ForegroundColor Green
} catch {
    Write-Host "  Pickup Location create failed" -ForegroundColor Yellow
    $PickupLocationId = "00000000-0000-0000-0000-000000000001"
}

$locationData2 = "{`"name`": `"Delivery Lokasyon $TS`", `"countryCode`": `"TR`", `"city`": `"Ankara`", `"isActive`": true}"
try {
    $locResp2 = Invoke-RestMethod -Uri "$BASE/Locations/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $locationData2 -ErrorAction Stop
    $DeliveryLocationId = $locResp2.payload.id
    Write-Host "  Delivery Location created: $DeliveryLocationId" -ForegroundColor Green
} catch {
    Write-Host "  Delivery Location create failed" -ForegroundColor Yellow
    $DeliveryLocationId = "00000000-0000-0000-0000-000000000002"
}

# Create a Category
$catData = "{`"name`": `"Dep Kategori $TS`", `"slug`": `"dep-kat5-$TS`", `"isActive`": true}"
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

# Create a Product for transport and deal entities
$productData = "{`"title`": `"Test Urun Transport $TS`", `"slug`": `"test-urun-transport-$TS`", `"description`": `"Test aciklama`", `"sellerId`": `"$SellerId`", `"categoryId`": `"$CategoryId`", `"locationId`": `"$PickupLocationId`", `"basePrice`": 2000.00, `"currency`": `"TRY`", `"stockQuantity`": 5, `"isInStock`": true, `"status`": 0, `"condition`": 0}"
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

# Create a Transporter
$transporterData = "{`"userId`": `"$UserId`", `"companyName`": `"Test Nakliye $TS`", `"vehicleTypes`": `"Truck,Van`", `"isActive`": true, `"status`": 0}"
try {
    $transporterResp = Invoke-RestMethod -Uri "$BASE/Transporters/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $transporterData -ErrorAction Stop
    $TransporterId = $transporterResp.payload.id
    Write-Host "  Transporter created: $TransporterId" -ForegroundColor Green
} catch {
    Write-Host "  Transporter create failed" -ForegroundColor Yellow
    $TransporterId = "00000000-0000-0000-0000-000000000001"
}

# Create a Conversation
$convData = "{`"participant1Id`": `"$UserId`", `"participant2Id`": `"$UserId`", `"isActive`": true}"
try {
    $convResp = Invoke-RestMethod -Uri "$BASE/Conversations/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $convData -ErrorAction Stop
    $ConversationId = $convResp.payload.id
    Write-Host "  Conversation created: $ConversationId" -ForegroundColor Green
} catch {
    Write-Host "  Conversation create failed" -ForegroundColor Yellow
    $ConversationId = "00000000-0000-0000-0000-000000000001"
}

# Create an AnimalInfo for HealthRecords and Vaccinations
$animalData = "{`"productId`": `"$ProductId`", `"breedName`": `"Test Animal $TS`", `"gender`": 1, `"ageMonths`": 24, `"healthStatus`": 1, `"purpose`": 0}"
try {
    $animalResp = Invoke-RestMethod -Uri "$BASE/AnimalInfos/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $animalData -ErrorAction Stop
    $AnimalInfoId = $animalResp.payload.id
    Write-Host "  AnimalInfo created: $AnimalInfoId" -ForegroundColor Green
} catch {
    Write-Host "  AnimalInfo create failed" -ForegroundColor Yellow
    $AnimalInfoId = "00000000-0000-0000-0000-000000000001"
}

# Create a TransportRequest for TransportOffers, TransportTrackings, TransporterReviews
$transportReqData = "{`"productId`": `"$ProductId`", `"sellerId`": `"$SellerId`", `"buyerId`": `"$UserId`", `"pickupLocationId`": `"$PickupLocationId`", `"deliveryLocationId`": `"$DeliveryLocationId`", `"currency`": `"TRY`", `"transportType`": 0, `"status`": 0}"
try {
    $transportReqResp = Invoke-RestMethod -Uri "$BASE/TransportRequests/Create" -Method POST -Headers @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $JWT"
    } -Body $transportReqData -ErrorAction Stop
    $TransportRequestId = $transportReqResp.payload.id
    Write-Host "  TransportRequest created: $TransportRequestId" -ForegroundColor Green
} catch {
    $errBody = $_.ErrorDetails.Message
    if ($errBody) {
        $err = $errBody | ConvertFrom-Json
        Write-Host "  TransportRequest create failed: $($err.error.message)" -ForegroundColor Red
    } else {
        Write-Host "  TransportRequest create failed: $($_.Exception.Message)" -ForegroundColor Red
    }
    $TransportRequestId = "00000000-0000-0000-0000-000000000001"
}

Write-Host ""

$dealDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
$expiryDate = (Get-Date).AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ss")
$vaccDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
$recordDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")

# 1. Deals
$results += Test-Entity -Entity "Deals" `
    -CreateData1 "{`"dealNumber`": `"DN-A-$TS`", `"productId`": `"$ProductId`", `"sellerId`": `"$SellerId`", `"buyerId`": `"$UserId`", `"agreedPrice`": 5000, `"currency`": `"TRY`", `"quantity`": 1, `"status`": 0, `"dealDate`": `"$dealDate`", `"deliveryMethod`": 0}" `
    -CreateData2 "{`"dealNumber`": `"DN-B-$TS`", `"productId`": `"$ProductId`", `"sellerId`": `"$SellerId`", `"buyerId`": `"$UserId`", `"agreedPrice`": 7500, `"currency`": `"TRY`", `"quantity`": 2, `"status`": 0, `"dealDate`": `"$dealDate`", `"deliveryMethod`": 1}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"dealNumber`": `"DN-U-$TS`", `"productId`": `"$ProductId`", `"sellerId`": `"$SellerId`", `"buyerId`": `"$UserId`", `"agreedPrice`": 6000, `"currency`": `"TRY`", `"quantity`": 1, `"status`": 1, `"dealDate`": `"$dealDate`", `"deliveryMethod`": 0}"

# 2. Messages
$results += Test-Entity -Entity "Messages" `
    -CreateData1 "{`"conversationId`": `"$ConversationId`", `"senderUserId`": `"$UserId`", `"recipientUserId`": `"$UserId`", `"content`": `"Test mesaj A-$TS`"}" `
    -CreateData2 "{`"conversationId`": `"$ConversationId`", `"senderUserId`": `"$UserId`", `"recipientUserId`": `"$UserId`", `"content`": `"Test mesaj B-$TS`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"conversationId`": `"$ConversationId`", `"senderUserId`": `"$UserId`", `"recipientUserId`": `"$UserId`", `"content`": `"GUNCELLENDI`"}"

# 3. Offers
$results += Test-Entity -Entity "Offers" `
    -CreateData1 "{`"productId`": `"$ProductId`", `"buyerUserId`": `"$UserId`", `"sellerUserId`": `"$UserId`", `"offeredPrice`": 4500, `"currency`": `"TRY`", `"quantity`": 1, `"status`": 0, `"expiryDate`": `"$expiryDate`"}" `
    -CreateData2 "{`"productId`": `"$ProductId`", `"buyerUserId`": `"$UserId`", `"sellerUserId`": `"$UserId`", `"offeredPrice`": 4000, `"currency`": `"TRY`", `"quantity`": 1, `"status`": 0, `"expiryDate`": `"$expiryDate`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"productId`": `"$ProductId`", `"buyerUserId`": `"$UserId`", `"sellerUserId`": `"$UserId`", `"offeredPrice`": 4200, `"currency`": `"TRY`", `"quantity`": 1, `"status`": 1}"

# 4. TransportOffers
$results += Test-Entity -Entity "TransportOffers" `
    -CreateData1 "{`"transportRequestId`": `"$TransportRequestId`", `"transporterId`": `"$TransporterId`", `"offeredPrice`": 500, `"currency`": `"TRY`", `"vehicleType`": `"Truck`", `"status`": 0, `"insuranceIncluded`": true}" `
    -CreateData2 "{`"transportRequestId`": `"$TransportRequestId`", `"transporterId`": `"$TransporterId`", `"offeredPrice`": 600, `"currency`": `"TRY`", `"vehicleType`": `"Van`", `"status`": 0, `"insuranceIncluded`": false}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"transportRequestId`": `"$TransportRequestId`", `"transporterId`": `"$TransporterId`", `"offeredPrice`": 550, `"currency`": `"TRY`", `"vehicleType`": `"Truck`", `"status`": 1}"

# 5. TransportTrackings
$results += Test-Entity -Entity "TransportTrackings" `
    -CreateData1 "{`"transportRequestId`": `"$TransportRequestId`", `"latitude`": 41.0082, `"longitude`": 28.9784, `"locationDescription`": `"Istanbul A-$TS`", `"status`": 0, `"statusDescription`": `"Yukleniyor`"}" `
    -CreateData2 "{`"transportRequestId`": `"$TransportRequestId`", `"latitude`": 39.9334, `"longitude`": 32.8597, `"locationDescription`": `"Ankara B-$TS`", `"status`": 1, `"statusDescription`": `"Yolda`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"transportRequestId`": `"$TransportRequestId`", `"latitude`": 40.5, `"longitude`": 30.5, `"locationDescription`": `"GUNCELLENDI`", `"status`": 2}"

# 6. TransporterReviews
$results += Test-Entity -Entity "TransporterReviews" `
    -CreateData1 "{`"transporterId`": `"$TransporterId`", `"userId`": `"$UserId`", `"transportRequestId`": `"$TransportRequestId`", `"overallRating`": 5, `"timelinessRating`": 5, `"communicationRating`": 5, `"carefulHandlingRating`": 5, `"professionalismRating`": 5, `"comment`": `"Harika hizmet A-$TS`"}" `
    -CreateData2 "{`"transporterId`": `"$TransporterId`", `"userId`": `"$UserId`", `"transportRequestId`": `"$TransportRequestId`", `"overallRating`": 4, `"timelinessRating`": 4, `"communicationRating`": 4, `"carefulHandlingRating`": 4, `"professionalismRating`": 4, `"comment`": `"Iyi hizmet B-$TS`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"transporterId`": `"$TransporterId`", `"userId`": `"$UserId`", `"transportRequestId`": `"$TransportRequestId`", `"overallRating`": 5, `"timelinessRating`": 5, `"communicationRating`": 5, `"carefulHandlingRating`": 5, `"professionalismRating`": 5, `"comment`": `"GUNCELLENDI`"}"

# 7. HealthRecords
$results += Test-Entity -Entity "HealthRecords" `
    -CreateData1 "{`"animalInfoId`": `"$AnimalInfoId`", `"recordDate`": `"$recordDate`", `"recordType`": `"Genel Kontrol A-$TS`", `"veterinarianName`": `"Dr. Ahmet`", `"diagnosis`": `"Saglikli`"}" `
    -CreateData2 "{`"animalInfoId`": `"$AnimalInfoId`", `"recordDate`": `"$recordDate`", `"recordType`": `"Asi B-$TS`", `"veterinarianName`": `"Dr. Mehmet`", `"diagnosis`": `"Asi yapildi`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"animalInfoId`": `"$AnimalInfoId`", `"recordDate`": `"$recordDate`", `"recordType`": `"GUNCELLENDI`", `"veterinarianName`": `"Dr. Updated`", `"diagnosis`": `"Updated`"}"

# 8. Vaccinations
$results += Test-Entity -Entity "Vaccinations" `
    -CreateData1 "{`"animalInfoId`": `"$AnimalInfoId`", `"vaccineName`": `"Anthrax A-$TS`", `"vaccineType`": `"Canli`", `"vaccinationDate`": `"$vaccDate`", `"veterinarianName`": `"Dr. Ahmet`"}" `
    -CreateData2 "{`"animalInfoId`": `"$AnimalInfoId`", `"vaccineName`": `"FMD B-$TS`", `"vaccineType`": `"Inaktif`", `"vaccinationDate`": `"$vaccDate`", `"veterinarianName`": `"Dr. Mehmet`"}" `
    -UpdateDataTemplate "{`"id`": `"ID_PLACEHOLDER`", `"animalInfoId`": `"$AnimalInfoId`", `"vaccineName`": `"GUNCELLENDI`", `"vaccineType`": `"Updated`", `"vaccinationDate`": `"$vaccDate`", `"veterinarianName`": `"Dr. Updated`"}"

# Summary
Write-Host "`n========================================" -ForegroundColor Yellow
Write-Host "SONUCLAR - PART 5" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow

$results | Format-Table -AutoSize

$successCount = ($results | Where-Object { $_.Create1 -eq "OK" -and $_.Create2 -eq "OK" -and $_.Update -eq "OK" -and $_.Delete -eq "OK" }).Count
$totalCount = $results.Count

Write-Host "`nToplam: $successCount / $totalCount basarili" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
