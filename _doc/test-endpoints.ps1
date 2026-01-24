$baseUrl = 'http://localhost:5221'
$global:testResults = @()

function Test-Endpoint {
    param([string]$Entity, [string]$Action, [string]$Body)
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/$Entity/$Action" -Method Post -ContentType 'application/json' -Body ([System.Text.Encoding]::UTF8.GetBytes($Body)) -TimeoutSec 30
        if ($response.hasError -eq $false) {
            $global:testResults += @{ Entity=$Entity; Action=$Action; Status='OK' }
            return @{ Status = 'OK'; Payload = $response.payload; Code = $response.code }
        } else {
            $global:testResults += @{ Entity=$Entity; Action=$Action; Status='ERROR'; Error=$response.error }
            return @{ Status = 'ERROR'; Error = $response.error; Code = $response.code }
        }
    } catch {
        $global:testResults += @{ Entity=$Entity; Action=$Action; Status='FAIL'; Error=$_.Exception.Message }
        return @{ Status = 'FAIL'; Error = $_.Exception.Message }
    }
}

function Test-CRUD {
    param(
        [string]$Entity,
        [string]$CreateBody,
        [string]$UpdateBody  # Should have {ID} placeholder
    )
    Write-Host "Testing $Entity..."

    # Create
    $r = Test-Endpoint $Entity 'Create' $CreateBody
    $id = $r.Payload.id
    Write-Host "  Create: $($r.Status) ID=$id"
    if ($r.Status -ne 'OK') {
        Write-Host "  ERROR: $($r.Error)"
        return $null
    }

    # All
    $r = Test-Endpoint $Entity 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"

    # Detail
    $r = Test-Endpoint $Entity 'Detail' "{`"id`":`"$id`"}"
    Write-Host "  Detail: $($r.Status)"

    # Pick
    $r = Test-Endpoint $Entity 'Pick' '{"keyword":"","limit":10}'
    Write-Host "  Pick: $($r.Status)"

    # Update
    $updateJson = $UpdateBody.Replace('{ID}', $id)
    $r = Test-Endpoint $Entity 'Update' $updateJson
    Write-Host "  Update: $($r.Status)"
    if ($r.Status -ne 'OK') { Write-Host "    Error: $($r.Error)" }

    # Delete
    $r = Test-Endpoint $Entity 'Delete' "{`"id`":`"$id`"}"
    Write-Host "  Delete: $($r.Status)"

    return $id
}

Write-Host "============================================"
Write-Host "FULL CRUD ENDPOINT TEST - LivestockTrading"
Write-Host "============================================"
Write-Host ""

# ==========================================
# PHASE 1: Independent Entities
# ==========================================
Write-Host "=== PHASE 1: Independent Entities ==="

Test-CRUD 'Categories' `
    '{"name":"TestCat","slug":"testcat","description":"Test","iconUrl":"","sortOrder":1,"isActive":true,"parentCategoryId":null,"nameTranslations":null,"descriptionTranslations":null,"attributesTemplate":null}' `
    '{"id":"{ID}","name":"TestCat-Upd","slug":"testcat","description":"Updated","iconUrl":"","sortOrder":2,"isActive":true,"parentCategoryId":null,"nameTranslations":null,"descriptionTranslations":null,"attributesTemplate":null}'

Test-CRUD 'Brands' `
    '{"name":"TestBrand","slug":"testbrand","description":"Test brand","logoUrl":"","isActive":true}' `
    '{"id":"{ID}","name":"TestBrand-Upd","slug":"testbrand","description":"Updated","logoUrl":"","isActive":true}'

Test-CRUD 'Locations' `
    '{"name":"TestLoc","city":"Ankara","state":"Ankara","countryCode":"TR","latitude":39.93,"longitude":32.86,"address":"Test addr","postalCode":"06000","isActive":true}' `
    '{"id":"{ID}","name":"TestLoc-Upd","city":"Istanbul","state":"Istanbul","countryCode":"TR","latitude":41.01,"longitude":28.98,"address":"Updated","postalCode":"34000","isActive":true}'

Test-CRUD 'TaxRates' `
    '{"countryCode":"TR","taxType":"KDV","rate":18.0,"isActive":true}' `
    '{"id":"{ID}","countryCode":"TR","taxType":"KDV","rate":20.0,"isActive":true}'

Test-CRUD 'Banners' `
    '{"title":"TestBanner","imageUrl":"https://ex.com/b.jpg","linkUrl":"https://ex.com","sortOrder":1,"isActive":true,"startDate":"2026-01-01T00:00:00Z","endDate":"2026-12-31T23:59:59Z"}' `
    '{"id":"{ID}","title":"TestBanner-Upd","imageUrl":"https://ex.com/b2.jpg","linkUrl":"https://ex.com","sortOrder":2,"isActive":true,"startDate":"2026-01-01T00:00:00Z","endDate":"2026-12-31T23:59:59Z"}'

Test-CRUD 'Currencies' `
    '{"code":"TST","symbol":"T","name":"Test Currency","exchangeRateToUSD":0.05,"isActive":true}' `
    '{"id":"{ID}","code":"TST","symbol":"T","name":"Test Currency Upd","exchangeRateToUSD":0.06,"isActive":true}'

Test-CRUD 'Languages' `
    '{"code":"ts","name":"Test Language","nativeName":"Testce","isActive":true}' `
    '{"id":"{ID}","code":"ts","name":"Test Lang Upd","nativeName":"Testce","isActive":true}'

Test-CRUD 'PaymentMethods' `
    '{"name":"TestPay","code":"TESTPAY","description":"Test payment","isActive":true}' `
    '{"id":"{ID}","name":"TestPay-Upd","code":"TESTPAY","description":"Updated","isActive":true}'

Test-CRUD 'ShippingCarriers' `
    '{"name":"TestCarrier","code":"TSTCAR","trackingUrl":"https://track.test.com/{tracking}","isActive":true}' `
    '{"id":"{ID}","name":"TestCarrier-Upd","code":"TSTCAR","trackingUrl":"https://track.test.com/{tracking}","isActive":true}'

Test-CRUD 'FAQs' `
    '{"question":"Test question?","answer":"Test answer","category":"General","sortOrder":1,"isActive":true}' `
    '{"id":"{ID}","question":"Updated question?","answer":"Updated answer","category":"General","sortOrder":2,"isActive":true}'

Test-CRUD 'Deals' `
    '{"title":"TestDeal","description":"Test deal desc","discountPercentage":10.0,"startDate":"2026-01-01T00:00:00Z","endDate":"2026-12-31T23:59:59Z","isActive":true}' `
    '{"id":"{ID}","title":"TestDeal-Upd","description":"Updated","discountPercentage":15.0,"startDate":"2026-01-01T00:00:00Z","endDate":"2026-12-31T23:59:59Z","isActive":true}'

Write-Host ""
Write-Host "=== PHASE 2: Seller/Transporter/Farm/Conversation ==="

# Create persistent parent entities for Phase 2+
$r = Test-Endpoint 'Categories' 'Create' '{"name":"PersistCat","slug":"persistcat","description":"Persist","iconUrl":"","sortOrder":1,"isActive":true,"parentCategoryId":null,"nameTranslations":null,"descriptionTranslations":null,"attributesTemplate":null}'
$persistCatId = $r.Payload.id
Write-Host "Persistent Category: $persistCatId"

$r = Test-Endpoint 'Brands' 'Create' '{"name":"PersistBrand","slug":"persistbrand","description":"Persist","logoUrl":"","isActive":true}'
$persistBrandId = $r.Payload.id
Write-Host "Persistent Brand: $persistBrandId"

$r = Test-Endpoint 'Locations' 'Create' '{"name":"PersistLoc","city":"Ankara","state":"Ankara","countryCode":"TR","latitude":39.93,"longitude":32.86,"address":"Persist","postalCode":"06000","isActive":true}'
$persistLocId = $r.Payload.id
Write-Host "Persistent Location: $persistLocId"

# Sellers
Test-CRUD 'Sellers' `
    '{"userId":"11111111-1111-1111-1111-111111111111","businessName":"TestSeller","businessType":"Individual","taxNumber":"1234567890","registrationNumber":"REG001","description":"Test seller","logoUrl":"","bannerUrl":"","email":"test@seller.com","phone":"+905551111111","website":"","isActive":true,"status":0,"businessHours":"{}","acceptedPaymentMethods":"[]","returnPolicy":"","shippingPolicy":"","socialMediaLinks":"{}"}' `
    '{"id":"{ID}","userId":"11111111-1111-1111-1111-111111111111","businessName":"TestSeller-Upd","businessType":"Company","taxNumber":"1234567890","registrationNumber":"REG001","description":"Updated","logoUrl":"","bannerUrl":"","email":"upd@seller.com","phone":"+905552222222","website":"","isActive":true,"status":1,"businessHours":"{}","acceptedPaymentMethods":"[]","returnPolicy":"","shippingPolicy":"","socialMediaLinks":"{}"}'

# Create persistent seller
$r = Test-Endpoint 'Sellers' 'Create' '{"userId":"22222222-2222-2222-2222-222222222222","businessName":"PersistSeller","businessType":"Company","taxNumber":"9876543210","registrationNumber":"REG002","description":"Persist","logoUrl":"","bannerUrl":"","email":"persist@seller.com","phone":"+905553333333","website":"","isActive":true,"status":0,"businessHours":"{}","acceptedPaymentMethods":"[]","returnPolicy":"","shippingPolicy":"","socialMediaLinks":"{}"}'
$persistSellerId = $r.Payload.id
Write-Host "Persistent Seller: $persistSellerId"

# Transporters
Test-CRUD 'Transporters' `
    '{"userId":"33333333-3333-3333-3333-333333333333","companyName":"TestTransport","contactPerson":"Test Person","email":"test@transport.com","phone":"+905554444444","address":"Test addr","city":"Ankara","countryCode":"TR","licenseNumber":"LIC-001","isActive":true}' `
    '{"id":"{ID}","userId":"33333333-3333-3333-3333-333333333333","companyName":"TestTransport-Upd","contactPerson":"Test Person","email":"upd@transport.com","phone":"+905555555555","address":"Updated","city":"Istanbul","countryCode":"TR","licenseNumber":"LIC-001","isActive":true}'

# Create persistent transporter
$r = Test-Endpoint 'Transporters' 'Create' '{"userId":"44444444-4444-4444-4444-444444444444","companyName":"PersistTransport","contactPerson":"Persist","email":"persist@transport.com","phone":"+905556666666","address":"Persist","city":"Izmir","countryCode":"TR","licenseNumber":"LIC-002","isActive":true}'
$persistTransporterId = $r.Payload.id
Write-Host "Persistent Transporter: $persistTransporterId"

# Farms
$farmCreate = @"
{"name":"TestFarm","description":"Test farm","registrationNumber":"FARM-001","sellerId":"$persistSellerId","locationId":"$persistLocId","type":0,"totalAreaHectares":100.0,"cultivatedAreaHectares":80.0,"certifications":"[]","isOrganic":true,"imageUrls":"[]","videoUrl":"","isActive":true}
"@
$farmUpdate = @"
{"id":"{ID}","name":"TestFarm-Upd","description":"Updated","registrationNumber":"FARM-001","sellerId":"$persistSellerId","locationId":"$persistLocId","type":0,"totalAreaHectares":150.0,"cultivatedAreaHectares":120.0,"certifications":"[]","isOrganic":true,"imageUrls":"[]","videoUrl":"","isActive":true}
"@
Test-CRUD 'Farms' $farmCreate $farmUpdate

# Conversations
Test-CRUD 'Conversations' `
    '{"participantOneId":"11111111-1111-1111-1111-111111111111","participantTwoId":"22222222-2222-2222-2222-222222222222","subject":"Test conversation","status":0}' `
    '{"id":"{ID}","participantOneId":"11111111-1111-1111-1111-111111111111","participantTwoId":"22222222-2222-2222-2222-222222222222","subject":"Updated conversation","status":1}'

# Create persistent conversation
$r = Test-Endpoint 'Conversations' 'Create' '{"participantOneId":"11111111-1111-1111-1111-111111111111","participantTwoId":"22222222-2222-2222-2222-222222222222","subject":"Persist conv","status":0}'
$persistConvId = $r.Payload.id
Write-Host "Persistent Conversation: $persistConvId"

# ShippingZones
$szCreate = @"
{"sellerId":"$persistSellerId","name":"TestZone","countryCodes":"[\"TR\",\"DE\"]","isActive":true}
"@
$szUpdate = @"
{"id":"{ID}","sellerId":"$persistSellerId","name":"TestZone-Upd","countryCodes":"[\"TR\",\"DE\",\"FR\"]","isActive":true}
"@
Test-CRUD 'ShippingZones' $szCreate $szUpdate

# Create persistent ShippingZone
$r = Test-Endpoint 'ShippingZones' 'Create' "{`"sellerId`":`"$persistSellerId`",`"name`":`"PersistZone`",`"countryCodes`":`"[\`"TR\`"]\`",`"isActive`":true}"
$persistZoneId = $r.Payload.id
Write-Host "Persistent ShippingZone: $persistZoneId"

Write-Host ""
Write-Host "=== PHASE 3: Products ==="

$prodCreate = @"
{"title":"TestProduct","slug":"test-product","description":"Test product desc","shortDescription":"Short","categoryId":"$persistCatId","brandId":"$persistBrandId","basePrice":1000.0,"currency":"TRY","discountedPrice":null,"priceUnit":"head","stockQuantity":10,"stockUnit":"head","minOrderQuantity":1,"maxOrderQuantity":5,"isInStock":true,"sellerId":"$persistSellerId","locationId":"$persistLocId","status":0,"condition":0,"isShippingAvailable":true,"shippingCost":100.0,"isInternationalShipping":false,"weight":500.0,"weightUnit":"kg","attributes":"{}","metaTitle":"","metaDescription":"","metaKeywords":""}
"@
$prodUpdate = @"
{"id":"{ID}","title":"TestProduct-Upd","slug":"test-product","description":"Updated","shortDescription":"Updated short","categoryId":"$persistCatId","brandId":"$persistBrandId","basePrice":1200.0,"currency":"TRY","discountedPrice":1100.0,"priceUnit":"head","stockQuantity":8,"stockUnit":"head","minOrderQuantity":1,"maxOrderQuantity":3,"isInStock":true,"sellerId":"$persistSellerId","locationId":"$persistLocId","status":1,"condition":0,"isShippingAvailable":true,"shippingCost":150.0,"isInternationalShipping":true,"weight":500.0,"weightUnit":"kg","attributes":"{}","metaTitle":"","metaDescription":"","metaKeywords":""}
"@
Test-CRUD 'Products' $prodCreate $prodUpdate

# Create persistent product
$r = Test-Endpoint 'Products' 'Create' $prodCreate.Replace('TestProduct','PersistProduct').Replace('test-product','persist-product')
$persistProductId = $r.Payload.id
Write-Host "Persistent Product: $persistProductId"

Write-Host ""
Write-Host "=== PHASE 4: Product-Dependent Entities ==="

# ProductImages
$piCreate = "{`"productId`":`"$persistProductId`",`"imageUrl`":`"https://ex.com/img.jpg`",`"thumbnailUrl`":`"https://ex.com/thumb.jpg`",`"altText`":`"Test`",`"sortOrder`":1,`"isPrimary`":true}"
$piUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"imageUrl`":`"https://ex.com/img2.jpg`",`"thumbnailUrl`":`"https://ex.com/thumb2.jpg`",`"altText`":`"Updated`",`"sortOrder`":2,`"isPrimary`":false}"
Test-CRUD 'ProductImages' $piCreate $piUpdate

# ProductVideos
$pvCreate = "{`"productId`":`"$persistProductId`",`"videoUrl`":`"https://ex.com/vid.mp4`",`"thumbnailUrl`":`"https://ex.com/vthumb.jpg`",`"title`":`"Test video`",`"duration`":120,`"sortOrder`":1}"
$pvUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"videoUrl`":`"https://ex.com/vid2.mp4`",`"thumbnailUrl`":`"https://ex.com/vthumb2.jpg`",`"title`":`"Updated video`",`"duration`":180,`"sortOrder`":2}"
Test-CRUD 'ProductVideos' $pvCreate $pvUpdate

# ProductDocuments
$pdCreate = "{`"productId`":`"$persistProductId`",`"documentUrl`":`"https://ex.com/doc.pdf`",`"title`":`"Test doc`",`"description`":`"Test`",`"fileType`":`"pdf`",`"fileSize`":1024}"
$pdUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"documentUrl`":`"https://ex.com/doc2.pdf`",`"title`":`"Updated doc`",`"description`":`"Updated`",`"fileType`":`"pdf`",`"fileSize`":2048}"
Test-CRUD 'ProductDocuments' $pdCreate $pdUpdate

# ProductPrices
$ppCreate = "{`"productId`":`"$persistProductId`",`"currency`":`"TRY`",`"price`":1000.0,`"discountedPrice`":900.0,`"minQuantity`":1,`"maxQuantity`":10}"
$ppUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"currency`":`"TRY`",`"price`":1100.0,`"discountedPrice`":1000.0,`"minQuantity`":1,`"maxQuantity`":5}"
Test-CRUD 'ProductPrices' $ppCreate $ppUpdate

# ProductVariants
$pvarCreate = "{`"productId`":`"$persistProductId`",`"name`":`"TestVariant`",`"sku`":`"TST-001`",`"price`":1000.0,`"stockQuantity`":5,`"attributes`":`"{}`",`"isActive`":true}"
$pvarUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"name`":`"TestVariant-Upd`",`"sku`":`"TST-001`",`"price`":1100.0,`"stockQuantity`":3,`"attributes`":`"{}`",`"isActive`":true}"
Test-CRUD 'ProductVariants' $pvarCreate $pvarUpdate

# ProductReviews
$prCreate = "{`"productId`":`"$persistProductId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"rating`":5,`"title`":`"Great`",`"comment`":`"Excellent product`",`"isVerifiedPurchase`":true}"
$prUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"rating`":4,`"title`":`"Good`",`"comment`":`"Updated review`",`"isVerifiedPurchase`":true}"
Test-CRUD 'ProductReviews' $prCreate $prUpdate

# AnimalInfos
$aiCreate = "{`"productId`":`"$persistProductId`",`"breed`":`"Holstein`",`"age`":3,`"gender`":0,`"weight`":500.0,`"color`":`"Black/White`",`"healthStatus`":0,`"isVaccinated`":true,`"registrationNumber`":`"ANI-001`"}"
$aiUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"breed`":`"Holstein`",`"age`":4,`"gender`":0,`"weight`":520.0,`"color`":`"Black/White`",`"healthStatus`":0,`"isVaccinated`":true,`"registrationNumber`":`"ANI-001`"}"
Test-CRUD 'AnimalInfos' $aiCreate $aiUpdate

# Offers
$ofCreate = "{`"productId`":`"$persistProductId`",`"buyerUserId`":`"11111111-1111-1111-1111-111111111111`",`"sellerUserId`":`"22222222-2222-2222-2222-222222222222`",`"offeredPrice`":900.0,`"currency`":`"TRY`",`"quantity`":1,`"message`":`"Test offer`",`"status`":0,`"expiryDate`":`"2026-12-31T23:59:59Z`",`"counterOfferToId`":null}"
$ofUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"buyerUserId`":`"11111111-1111-1111-1111-111111111111`",`"sellerUserId`":`"22222222-2222-2222-2222-222222222222`",`"offeredPrice`":950.0,`"currency`":`"TRY`",`"quantity`":1,`"message`":`"Updated offer`",`"status`":1,`"expiryDate`":`"2026-12-31T23:59:59Z`",`"counterOfferToId`":null}"
Test-CRUD 'Offers' $ofCreate $ofUpdate

# FavoriteProducts
$fpCreate = "{`"productId`":`"$persistProductId`",`"userId`":`"11111111-1111-1111-1111-111111111111`"}"
$fpUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"userId`":`"11111111-1111-1111-1111-111111111111`"}"
Test-CRUD 'FavoriteProducts' $fpCreate $fpUpdate

# ProductViewHistories
$pvhCreate = "{`"productId`":`"$persistProductId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"viewedAt`":`"2026-01-24T10:00:00Z`"}"
$pvhUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"viewedAt`":`"2026-01-24T12:00:00Z`"}"
Test-CRUD 'ProductViewHistories' $pvhCreate $pvhUpdate

# HealthRecords
$hrCreate = "{`"productId`":`"$persistProductId`",`"recordDate`":`"2026-01-20T00:00:00Z`",`"type`":`"Checkup`",`"description`":`"Routine check`",`"veterinarian`":`"Dr. Test`",`"notes`":`"Healthy`"}"
$hrUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"recordDate`":`"2026-01-21T00:00:00Z`",`"type`":`"Treatment`",`"description`":`"Updated`",`"veterinarian`":`"Dr. Test`",`"notes`":`"Updated notes`"}"
Test-CRUD 'HealthRecords' $hrCreate $hrUpdate

# Vaccinations
$vacCreate = "{`"productId`":`"$persistProductId`",`"vaccineName`":`"TestVaccine`",`"vaccineDate`":`"2026-01-15T00:00:00Z`",`"nextDueDate`":`"2026-07-15T00:00:00Z`",`"veterinarian`":`"Dr. Test`",`"batchNumber`":`"BATCH-001`",`"notes`":`"Test`"}"
$vacUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"vaccineName`":`"TestVaccine-Upd`",`"vaccineDate`":`"2026-01-15T00:00:00Z`",`"nextDueDate`":`"2026-08-15T00:00:00Z`",`"veterinarian`":`"Dr. Test`",`"batchNumber`":`"BATCH-001`",`"notes`":`"Updated`"}"
Test-CRUD 'Vaccinations' $vacCreate $vacUpdate

Write-Host ""
Write-Host "=== PHASE 5: Messages, Reviews, Transport, Remaining ==="

# Messages
$msgCreate = "{`"conversationId`":`"$persistConvId`",`"senderUserId`":`"11111111-1111-1111-1111-111111111111`",`"content`":`"Test message`",`"messageType`":0}"
$msgUpdate = "{`"id`":`"{ID}`",`"conversationId`":`"$persistConvId`",`"senderUserId`":`"11111111-1111-1111-1111-111111111111`",`"content`":`"Updated message`",`"messageType`":0}"
Test-CRUD 'Messages' $msgCreate $msgUpdate

# SellerReviews
$srCreate = "{`"sellerId`":`"$persistSellerId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"rating`":5,`"title`":`"Great seller`",`"comment`":`"Very good`",`"isVerifiedPurchase`":true}"
$srUpdate = "{`"id`":`"{ID}`",`"sellerId`":`"$persistSellerId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"rating`":4,`"title`":`"Good seller`",`"comment`":`"Updated`",`"isVerifiedPurchase`":true}"
Test-CRUD 'SellerReviews' $srCreate $srUpdate

# Notifications
Test-CRUD 'Notifications' `
    '{"userId":"11111111-1111-1111-1111-111111111111","title":"Test Notification","message":"Test msg","type":0,"isRead":false}' `
    '{"id":"{ID}","userId":"11111111-1111-1111-1111-111111111111","title":"Updated Notification","message":"Updated","type":1,"isRead":true}'

# SearchHistories
Test-CRUD 'SearchHistories' `
    '{"userId":"11111111-1111-1111-1111-111111111111","searchQuery":"Holstein cow","filters":"{}","resultCount":5}' `
    '{"id":"{ID}","userId":"11111111-1111-1111-1111-111111111111","searchQuery":"Angus cow","filters":"{}","resultCount":3}'

# UserPreferences
Test-CRUD 'UserPreferences' `
    '{"userId":"11111111-1111-1111-1111-111111111111","preferenceKey":"language","preferenceValue":"tr-TR"}' `
    '{"id":"{ID}","userId":"11111111-1111-1111-1111-111111111111","preferenceKey":"language","preferenceValue":"en-US"}'

# ShippingRates
$shRateCreate = "{`"shippingZoneId`":`"$persistZoneId`",`"carrierId`":null,`"minWeight`":0,`"maxWeight`":100,`"price`":50.0,`"currency`":`"TRY`",`"estimatedDays`":3,`"isActive`":true}"
$shRateUpdate = "{`"id`":`"{ID}`",`"shippingZoneId`":`"$persistZoneId`",`"carrierId`":null,`"minWeight`":0,`"maxWeight`":200,`"price`":75.0,`"currency`":`"TRY`",`"estimatedDays`":5,`"isActive`":true}"
Test-CRUD 'ShippingRates' $shRateCreate $shRateUpdate

# TransportRequests
$trCreate = "{`"productId`":`"$persistProductId`",`"sellerId`":`"$persistSellerId`",`"buyerId`":`"11111111-1111-1111-1111-111111111111`",`"pickupLocationId`":`"$persistLocId`",`"deliveryLocationId`":`"$persistLocId`",`"transportType`":0,`"status`":0,`"isUrgent`":false,`"notes`":`"Test request`"}"
$trUpdate = "{`"id`":`"{ID}`",`"productId`":`"$persistProductId`",`"sellerId`":`"$persistSellerId`",`"buyerId`":`"11111111-1111-1111-1111-111111111111`",`"pickupLocationId`":`"$persistLocId`",`"deliveryLocationId`":`"$persistLocId`",`"transportType`":0,`"status`":1,`"isUrgent`":true,`"notes`":`"Updated`"}"
Test-CRUD 'TransportRequests' $trCreate $trUpdate

# Create persistent TransportRequest for TransportOffers/Trackings
$r = Test-Endpoint 'TransportRequests' 'Create' $trCreate.Replace('Test request','Persist request')
$persistTrId = $r.Payload.id
Write-Host "Persistent TransportRequest: $persistTrId"

# TransportOffers
$toCreate = "{`"transportRequestId`":`"$persistTrId`",`"transporterId`":`"$persistTransporterId`",`"price`":500.0,`"currency`":`"TRY`",`"estimatedDays`":2,`"notes`":`"Test offer`",`"status`":0}"
$toUpdate = "{`"id`":`"{ID}`",`"transportRequestId`":`"$persistTrId`",`"transporterId`":`"$persistTransporterId`",`"price`":450.0,`"currency`":`"TRY`",`"estimatedDays`":3,`"notes`":`"Updated`",`"status`":1}"
Test-CRUD 'TransportOffers' $toCreate $toUpdate

# TransportTrackings
$ttCreate = "{`"transportRequestId`":`"$persistTrId`",`"status`":0,`"location`":`"Ankara`",`"notes`":`"Departed`",`"latitude`":39.93,`"longitude`":32.86}"
$ttUpdate = "{`"id`":`"{ID}`",`"transportRequestId`":`"$persistTrId`",`"status`":1,`"location`":`"Istanbul`",`"notes`":`"In transit`",`"latitude`":41.01,`"longitude`":28.98}"
Test-CRUD 'TransportTrackings' $ttCreate $ttUpdate

# TransporterReviews
$tRevCreate = "{`"transporterId`":`"$persistTransporterId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"rating`":5,`"title`":`"Great transport`",`"comment`":`"Fast delivery`"}"
$tRevUpdate = "{`"id`":`"{ID}`",`"transporterId`":`"$persistTransporterId`",`"userId`":`"11111111-1111-1111-1111-111111111111`",`"rating`":4,`"title`":`"Good transport`",`"comment`":`"Updated`"}"
Test-CRUD 'TransporterReviews' $tRevCreate $tRevUpdate

Write-Host ""
Write-Host "=== PHASE 6: Remaining Info Entities ==="

# VeterinaryInfos
Test-CRUD 'VeterinaryInfos' `
    '{"productId":"00000000-0000-0000-0000-000000000001","veterinarianName":"Dr. Test","clinicName":"Test Clinic","phone":"+905551111111","email":"vet@test.com","licenseNumber":"VET-001","specialization":"Large Animals","isActive":true}' `
    '{"id":"{ID}","productId":"00000000-0000-0000-0000-000000000001","veterinarianName":"Dr. Updated","clinicName":"Updated Clinic","phone":"+905552222222","email":"vet2@test.com","licenseNumber":"VET-001","specialization":"Large Animals","isActive":true}'

# FeedInfos
Test-CRUD 'FeedInfos' `
    '{"name":"TestFeed","type":"Grain","description":"Test feed","nutritionalInfo":"{}","manufacturer":"TestMfg","isOrganic":true,"isActive":true}' `
    '{"id":"{ID}","name":"TestFeed-Upd","type":"Grain","description":"Updated","nutritionalInfo":"{}","manufacturer":"TestMfg","isOrganic":true,"isActive":true}'

# ChemicalInfos
Test-CRUD 'ChemicalInfos' `
    '{"name":"TestChem","type":"Pesticide","description":"Test chemical","activeIngredients":"TestIngredient","safetyDataSheet":"{}","manufacturer":"TestMfg","isOrganic":false,"isActive":true}' `
    '{"id":"{ID}","name":"TestChem-Upd","type":"Herbicide","description":"Updated","activeIngredients":"TestIngredient","safetyDataSheet":"{}","manufacturer":"TestMfg","isOrganic":false,"isActive":true}'

# SeedInfos
Test-CRUD 'SeedInfos' `
    '{"name":"TestSeed","type":"Wheat","variety":"TestVariety","description":"Test seed","germinationRate":95.0,"manufacturer":"TestMfg","isOrganic":true,"isActive":true}' `
    '{"id":"{ID}","name":"TestSeed-Upd","type":"Wheat","variety":"TestVariety","description":"Updated","germinationRate":92.0,"manufacturer":"TestMfg","isOrganic":true,"isActive":true}'

# MachineryInfos
Test-CRUD 'MachineryInfos' `
    '{"name":"TestMachine","type":"Tractor","brand":"TestBrand","model":"T-100","year":2024,"condition":0,"description":"Test machinery","specifications":"{}","isActive":true}' `
    '{"id":"{ID}","name":"TestMachine-Upd","type":"Tractor","brand":"TestBrand","model":"T-200","year":2025,"condition":1,"description":"Updated","specifications":"{}","isActive":true}'

Write-Host ""
Write-Host "============================================"
Write-Host "TEST RESULTS SUMMARY"
Write-Host "============================================"
$okCount = ($global:testResults | Where-Object { $_.Status -eq 'OK' }).Count
$errorCount = ($global:testResults | Where-Object { $_.Status -eq 'ERROR' }).Count
$failCount = ($global:testResults | Where-Object { $_.Status -eq 'FAIL' }).Count
$total = $global:testResults.Count
Write-Host "Total: $total | OK: $okCount | ERROR: $errorCount | FAIL: $failCount"
Write-Host ""

if ($errorCount -gt 0 -or $failCount -gt 0) {
    Write-Host "FAILED TESTS:"
    $global:testResults | Where-Object { $_.Status -ne 'OK' } | ForEach-Object {
        Write-Host "  $($_.Entity)/$($_.Action): $($_.Status) - $($_.Error)"
    }
}

# Cleanup persistent entities
Write-Host ""
Write-Host "Cleaning up persistent test data..."
Test-Endpoint 'TransportRequests' 'Delete' "{`"id`":`"$persistTrId`"}" | Out-Null
Test-Endpoint 'Products' 'Delete' "{`"id`":`"$persistProductId`"}" | Out-Null
Test-Endpoint 'Conversations' 'Delete' "{`"id`":`"$persistConvId`"}" | Out-Null
Test-Endpoint 'Sellers' 'Delete' "{`"id`":`"$persistSellerId`"}" | Out-Null
Test-Endpoint 'Transporters' 'Delete' "{`"id`":`"$persistTransporterId`"}" | Out-Null
Test-Endpoint 'ShippingZones' 'Delete' "{`"id`":`"$persistZoneId`"}" | Out-Null
Test-Endpoint 'Categories' 'Delete' "{`"id`":`"$persistCatId`"}" | Out-Null
Test-Endpoint 'Brands' 'Delete' "{`"id`":`"$persistBrandId`"}" | Out-Null
Test-Endpoint 'Locations' 'Delete' "{`"id`":`"$persistLocId`"}" | Out-Null
Write-Host "Cleanup done."
