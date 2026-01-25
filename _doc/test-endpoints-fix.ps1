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
        [string]$UpdateBody
    )
    Write-Host "Testing $Entity..."
    $r = Test-Endpoint $Entity 'Create' $CreateBody
    $id = $r.Payload.id
    Write-Host "  Create: $($r.Status) ID=$id"
    if ($r.Status -ne 'OK') {
        Write-Host "    Error: $($r.Error)"
        return $null
    }
    $r = Test-Endpoint $Entity 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint $Entity 'Detail' "{`"id`":`"$id`"}"
    Write-Host "  Detail: $($r.Status)"
    $r = Test-Endpoint $Entity 'Pick' '{"keyword":"","limit":10}'
    Write-Host "  Pick: $($r.Status)"
    $updateJson = $UpdateBody.Replace('{ID}', $id)
    $r = Test-Endpoint $Entity 'Update' $updateJson
    Write-Host "  Update: $($r.Status)"
    if ($r.Status -ne 'OK') { Write-Host "    Error: $($r.Error)" }
    $r = Test-Endpoint $Entity 'Delete' "{`"id`":`"$id`"}"
    Write-Host "  Delete: $($r.Status)"
    return $id
}

Write-Host "============================================"
Write-Host "FIX TESTS - Previously Failed Entities"
Write-Host "============================================"
Write-Host ""

# Create required parent entities first
Write-Host "Creating parent entities..."

# Category
$r = Test-Endpoint 'Categories' 'Create' '{"name":"FixTestCat","slug":"fixtestcat","description":"For fix tests","iconUrl":"","sortOrder":1,"isActive":true,"parentCategoryId":null,"nameTranslations":null,"descriptionTranslations":null,"attributesTemplate":null}'
$catId = $r.Payload.id
Write-Host "Category: $catId"

# Brand
$r = Test-Endpoint 'Brands' 'Create' '{"name":"FixTestBrand","slug":"fixtestbrand","description":"For fix tests","logoUrl":"","isActive":true}'
$brandId = $r.Payload.id
Write-Host "Brand: $brandId"

# Location
$r = Test-Endpoint 'Locations' 'Create' '{"name":"FixTestLoc","city":"Ankara","state":"Ankara","countryCode":"TR","latitude":39.93,"longitude":32.86,"address":"Fix test","postalCode":"06000","isActive":true}'
$locId = $r.Payload.id
Write-Host "Location: $locId"

# Seller
$r = Test-Endpoint 'Sellers' 'Create' '{"userId":"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa","businessName":"FixTestSeller","businessType":"Company","taxNumber":"1111111111","registrationNumber":"FIX001","description":"For fix tests","logoUrl":"","bannerUrl":"","email":"fix@test.com","phone":"+905551111111","website":"","isActive":true,"status":0,"businessHours":"{}","acceptedPaymentMethods":"[]","returnPolicy":"","shippingPolicy":"","socialMediaLinks":"{}"}'
$sellerId = $r.Payload.id
Write-Host "Seller: $sellerId"

# Transporter
$r = Test-Endpoint 'Transporters' 'Create' '{"userId":"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb","companyName":"FixTransport","contactPerson":"Fix Person","email":"fix@transport.com","phone":"+905552222222","address":"Fix addr","city":"Ankara","countryCode":"TR","licenseNumber":"FIX-LIC-001","isActive":true}'
$transporterId = $r.Payload.id
Write-Host "Transporter: $transporterId"

# ShippingZone (using correct JSON format)
$szBody = "{`"sellerId`":`"$sellerId`",`"name`":`"FixTestZone`",`"countryCodes`":`"[\`"TR\`"]\`",`"isActive`":true}"
$r = Test-Endpoint 'ShippingZones' 'Create' $szBody
$zoneId = $r.Payload.id
Write-Host "ShippingZone: $zoneId ($($r.Status))"
if ($r.Status -ne 'OK') { Write-Host "  Error: $($r.Error)" }

# Product
$prodBody = "{`"title`":`"FixTestProduct`",`"slug`":`"fix-test-product`",`"description`":`"Fix test`",`"shortDescription`":`"Fix`",`"categoryId`":`"$catId`",`"brandId`":`"$brandId`",`"basePrice`":1000.0,`"currency`":`"TRY`",`"discountedPrice`":null,`"priceUnit`":`"head`",`"stockQuantity`":10,`"stockUnit`":`"head`",`"minOrderQuantity`":1,`"maxOrderQuantity`":5,`"isInStock`":true,`"sellerId`":`"$sellerId`",`"locationId`":`"$locId`",`"status`":0,`"condition`":0,`"isShippingAvailable`":true,`"shippingCost`":100.0,`"isInternationalShipping`":false,`"weight`":500.0,`"weightUnit`":`"kg`",`"attributes`":`"{}`",`"metaTitle`":`"`",`"metaDescription`":`"`",`"metaKeywords`":`"`"}"
$r = Test-Endpoint 'Products' 'Create' $prodBody
$productId = $r.Payload.id
Write-Host "Product: $productId"

# Conversation (with correct field names)
$convBody = "{`"participantUserId1`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"participantUserId2`":`"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`",`"productId`":`"$productId`",`"subject`":`"Fix test conversation`",`"status`":0}"
$r = Test-Endpoint 'Conversations' 'Create' $convBody
$convId = $r.Payload.id
Write-Host "Conversation: $convId ($($r.Status))"
if ($r.Status -ne 'OK') { Write-Host "  Error: $($r.Error)" }

# AnimalInfo (with correct field names)
$animalBody = "{`"productId`":`"$productId`",`"breedName`":`"Holstein`",`"gender`":0,`"ageMonths`":36,`"weightKg`":500,`"color`":`"Black/White`"}"
$r = Test-Endpoint 'AnimalInfos' 'Create' $animalBody
$animalInfoId = $r.Payload.id
Write-Host "AnimalInfo: $animalInfoId ($($r.Status))"
if ($r.Status -ne 'OK') { Write-Host "  Error: $($r.Error)" }

# TransportRequest
$trBody = "{`"productId`":`"$productId`",`"sellerId`":`"$sellerId`",`"buyerId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"pickupLocationId`":`"$locId`",`"deliveryLocationId`":`"$locId`",`"transportType`":0,`"status`":0,`"isUrgent`":false,`"notes`":`"Fix test`"}"
$r = Test-Endpoint 'TransportRequests' 'Create' $trBody
$trId = $r.Payload.id
Write-Host "TransportRequest: $trId"

Write-Host ""
Write-Host "=== Testing Previously Failed Entities ==="
Write-Host ""

# 1. TaxRates (needs taxName)
Test-CRUD 'TaxRates' `
    '{"countryCode":"TR","taxName":"KDV","rate":18.0,"isActive":true}' `
    '{"id":"{ID}","countryCode":"TR","taxName":"KDV","rate":20.0,"isActive":true}'

# 2. Deals (with correct fields)
$dealCreate = "{`"dealNumber`":`"DEAL-FIX-001`",`"productId`":`"$productId`",`"sellerId`":`"$sellerId`",`"buyerId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"agreedPrice`":5000.0,`"currency`":`"TRY`",`"quantity`":1,`"status`":0}"
$dealUpdate = "{`"id`":`"{ID}`",`"dealNumber`":`"DEAL-FIX-001`",`"productId`":`"$productId`",`"sellerId`":`"$sellerId`",`"buyerId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"agreedPrice`":5500.0,`"currency`":`"TRY`",`"quantity`":2,`"status`":1}"
Test-CRUD 'Deals' $dealCreate $dealUpdate

# 3. Conversations (already tested above, but let's do full CRUD)
if ($convId) {
    Write-Host "Testing Conversations (already created)..."
    $r = Test-Endpoint 'Conversations' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'Conversations' 'Detail' "{`"id`":`"$convId`"}"
    Write-Host "  Detail: $($r.Status)"
    $r = Test-Endpoint 'Conversations' 'Pick' '{"keyword":"","limit":10}'
    Write-Host "  Pick: $($r.Status)"
    $updateBody = "{`"id`":`"$convId`",`"participantUserId1`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"participantUserId2`":`"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`",`"productId`":`"$productId`",`"subject`":`"Updated conversation`",`"status`":1}"
    $r = Test-Endpoint 'Conversations' 'Update' $updateBody
    Write-Host "  Update: $($r.Status)"
}

# 4. ProductDocuments (needs fileName)
$pdCreate = "{`"productId`":`"$productId`",`"documentUrl`":`"https://ex.com/doc.pdf`",`"fileName`":`"testdoc.pdf`",`"title`":`"Test Document`",`"type`":`"Certificate`",`"fileSizeBytes`":1024}"
$pdUpdate = "{`"id`":`"{ID}`",`"productId`":`"$productId`",`"documentUrl`":`"https://ex.com/doc2.pdf`",`"fileName`":`"testdoc2.pdf`",`"title`":`"Updated Document`",`"type`":`"Certificate`",`"fileSizeBytes`":2048}"
Test-CRUD 'ProductDocuments' $pdCreate $pdUpdate

# 5. ProductPrices (needs currencyCode)
$ppCreate = "{`"productId`":`"$productId`",`"currencyCode`":`"TRY`",`"price`":1000.0,`"discountedPrice`":900.0,`"isActive`":true}"
$ppUpdate = "{`"id`":`"{ID}`",`"productId`":`"$productId`",`"currencyCode`":`"TRY`",`"price`":1100.0,`"discountedPrice`":1000.0,`"isActive`":true}"
Test-CRUD 'ProductPrices' $ppCreate $ppUpdate

# 6. AnimalInfos (needs breedName, already tested above)
if ($animalInfoId) {
    Write-Host "Testing AnimalInfos (already created)..."
    $r = Test-Endpoint 'AnimalInfos' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'AnimalInfos' 'Detail' "{`"id`":`"$animalInfoId`"}"
    Write-Host "  Detail: $($r.Status)"
    $r = Test-Endpoint 'AnimalInfos' 'Pick' '{"keyword":"","limit":10}'
    Write-Host "  Pick: $($r.Status)"
    $updateBody = "{`"id`":`"$animalInfoId`",`"productId`":`"$productId`",`"breedName`":`"Angus`",`"gender`":1,`"ageMonths`":48,`"weightKg`":550,`"color`":`"Black`"}"
    $r = Test-Endpoint 'AnimalInfos' 'Update' $updateBody
    Write-Host "  Update: $($r.Status)"
}

# 7. HealthRecords (needs animalInfoId, recordDate, recordType)
if ($animalInfoId) {
    $hrCreate = "{`"animalInfoId`":`"$animalInfoId`",`"recordDate`":`"2026-01-20T00:00:00Z`",`"recordType`":`"Checkup`",`"veterinarianName`":`"Dr. Test`",`"diagnosis`":`"Healthy`",`"notes`":`"Routine check`"}"
    $hrUpdate = "{`"id`":`"{ID}`",`"animalInfoId`":`"$animalInfoId`",`"recordDate`":`"2026-01-21T00:00:00Z`",`"recordType`":`"Treatment`",`"veterinarianName`":`"Dr. Test`",`"diagnosis`":`"Updated`",`"notes`":`"Updated notes`"}"
    Test-CRUD 'HealthRecords' $hrCreate $hrUpdate
}

# 8. Vaccinations (needs animalInfoId, vaccineName, vaccinationDate)
if ($animalInfoId) {
    $vacCreate = "{`"animalInfoId`":`"$animalInfoId`",`"vaccineName`":`"FMD Vaccine`",`"vaccinationDate`":`"2026-01-15T00:00:00Z`",`"nextDueDate`":`"2026-07-15T00:00:00Z`",`"veterinarianName`":`"Dr. Test`",`"batchNumber`":`"BATCH-001`"}"
    $vacUpdate = "{`"id`":`"{ID}`",`"animalInfoId`":`"$animalInfoId`",`"vaccineName`":`"FMD Vaccine`",`"vaccinationDate`":`"2026-01-15T00:00:00Z`",`"nextDueDate`":`"2026-08-15T00:00:00Z`",`"veterinarianName`":`"Dr. Test`",`"batchNumber`":`"BATCH-002`"}"
    Test-CRUD 'Vaccinations' $vacCreate $vacUpdate
}

# 9. Messages (needs conversationId, senderUserId, recipientUserId, content)
if ($convId) {
    $msgCreate = "{`"conversationId`":`"$convId`",`"senderUserId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"recipientUserId`":`"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`",`"content`":`"Test message`"}"
    $msgUpdate = "{`"id`":`"{ID}`",`"conversationId`":`"$convId`",`"senderUserId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"recipientUserId`":`"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb`",`"content`":`"Updated message`"}"
    Test-CRUD 'Messages' $msgCreate $msgUpdate
}

# 10. SellerReviews (needs sellerId, userId, overallRating 1-5)
$srCreate = "{`"sellerId`":`"$sellerId`",`"userId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"overallRating`":5,`"title`":`"Great seller`",`"comment`":`"Very good service`",`"isVerifiedPurchase`":true}"
$srUpdate = "{`"id`":`"{ID}`",`"sellerId`":`"$sellerId`",`"userId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"overallRating`":4,`"title`":`"Good seller`",`"comment`":`"Updated review`",`"isVerifiedPurchase`":true}"
Test-CRUD 'SellerReviews' $srCreate $srUpdate

# 11. ShippingRates (needs shippingZoneId, shippingCost)
if ($zoneId) {
    $shRateCreate = "{`"shippingZoneId`":`"$zoneId`",`"shippingCost`":50.0,`"currency`":`"TRY`",`"estimatedDeliveryDays`":3,`"isActive`":true}"
    $shRateUpdate = "{`"id`":`"{ID}`",`"shippingZoneId`":`"$zoneId`",`"shippingCost`":75.0,`"currency`":`"TRY`",`"estimatedDeliveryDays`":5,`"isActive`":true}"
    Test-CRUD 'ShippingRates' $shRateCreate $shRateUpdate
}

# 12. TransportOffers (needs transportRequestId, transporterId, offeredPrice > 0)
$toCreate = "{`"transportRequestId`":`"$trId`",`"transporterId`":`"$transporterId`",`"offeredPrice`":500.0,`"currency`":`"TRY`",`"estimatedDurationDays`":2,`"status`":0}"
$toUpdate = "{`"id`":`"{ID}`",`"transportRequestId`":`"$trId`",`"transporterId`":`"$transporterId`",`"offeredPrice`":450.0,`"currency`":`"TRY`",`"estimatedDurationDays`":3,`"status`":1}"
Test-CRUD 'TransportOffers' $toCreate $toUpdate

# 13. TransporterReviews (needs transporterId, userId, transportRequestId, overallRating 1-5)
$tRevCreate = "{`"transporterId`":`"$transporterId`",`"userId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"transportRequestId`":`"$trId`",`"overallRating`":5,`"comment`":`"Great transport`"}"
$tRevUpdate = "{`"id`":`"{ID}`",`"transporterId`":`"$transporterId`",`"userId`":`"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa`",`"transportRequestId`":`"$trId`",`"overallRating`":4,`"comment`":`"Updated review`"}"
Test-CRUD 'TransporterReviews' $tRevCreate $tRevUpdate

# 14. FeedInfos (only needs productId)
$fiCreate = "{`"productId`":`"$productId`",`"type`":`"Grain`",`"targetAnimal`":`"Cattle`",`"proteinPercentage`":18.5,`"isOrganic`":true}"
$fiUpdate = "{`"id`":`"{ID}`",`"productId`":`"$productId`",`"type`":`"Silage`",`"targetAnimal`":`"Cattle`",`"proteinPercentage`":20.0,`"isOrganic`":true}"
Test-CRUD 'FeedInfos' $fiCreate $fiUpdate

# 15. ChemicalInfos (only needs productId)
$ciCreate = "{`"productId`":`"$productId`",`"type`":`"Pesticide`",`"activeIngredients`":`"TestIngredient`",`"toxicityLevel`":`"Low`",`"isOrganic`":false}"
$ciUpdate = "{`"id`":`"{ID}`",`"productId`":`"$productId`",`"type`":`"Herbicide`",`"activeIngredients`":`"TestIngredient2`",`"toxicityLevel`":`"Medium`",`"isOrganic`":false}"
Test-CRUD 'ChemicalInfos' $ciCreate $ciUpdate

# 16. SeedInfos (only needs productId)
$siCreate = "{`"productId`":`"$productId`",`"type`":`"Wheat`",`"variety`":`"TestVariety`",`"germinationRate`":95.0,`"isOrganic`":true}"
$siUpdate = "{`"id`":`"{ID}`",`"productId`":`"$productId`",`"type`":`"Barley`",`"variety`":`"TestVariety2`",`"germinationRate`":92.0,`"isOrganic`":true}"
Test-CRUD 'SeedInfos' $siCreate $siUpdate

# 17. MachineryInfos (only needs productId)
$miCreate = "{`"productId`":`"$productId`",`"type`":`"Tractor`",`"model`":`"T-100`",`"yearOfManufacture`":2024,`"powerHp`":100}"
$miUpdate = "{`"id`":`"{ID}`",`"productId`":`"$productId`",`"type`":`"Harvester`",`"model`":`"H-200`",`"yearOfManufacture`":2025,`"powerHp`":150}"
Test-CRUD 'MachineryInfos' $miCreate $miUpdate

Write-Host ""
Write-Host "============================================"
Write-Host "FIX TEST RESULTS SUMMARY"
Write-Host "============================================"
$okCount = ($global:testResults | Where-Object { $_.Status -eq 'OK' }).Count
$errorCount = ($global:testResults | Where-Object { $_.Status -eq 'ERROR' }).Count
$failCount = ($global:testResults | Where-Object { $_.Status -eq 'FAIL' }).Count
$total = $global:testResults.Count
Write-Host "Total: $total | OK: $okCount | ERROR: $errorCount | FAIL: $failCount"
Write-Host ""

if ($errorCount -gt 0 -or $failCount -gt 0) {
    Write-Host "STILL FAILING:"
    $global:testResults | Where-Object { $_.Status -ne 'OK' } | ForEach-Object {
        Write-Host "  $($_.Entity)/$($_.Action): $($_.Status) - $($_.Error)"
    }
}

# Cleanup
Write-Host ""
Write-Host "Cleaning up test data..."
if ($convId) { Test-Endpoint 'Conversations' 'Delete' "{`"id`":`"$convId`"}" | Out-Null }
if ($animalInfoId) { Test-Endpoint 'AnimalInfos' 'Delete' "{`"id`":`"$animalInfoId`"}" | Out-Null }
Test-Endpoint 'TransportRequests' 'Delete' "{`"id`":`"$trId`"}" | Out-Null
Test-Endpoint 'Products' 'Delete' "{`"id`":`"$productId`"}" | Out-Null
Test-Endpoint 'Transporters' 'Delete' "{`"id`":`"$transporterId`"}" | Out-Null
Test-Endpoint 'Sellers' 'Delete' "{`"id`":`"$sellerId`"}" | Out-Null
if ($zoneId) { Test-Endpoint 'ShippingZones' 'Delete' "{`"id`":`"$zoneId`"}" | Out-Null }
Test-Endpoint 'Locations' 'Delete' "{`"id`":`"$locId`"}" | Out-Null
Test-Endpoint 'Brands' 'Delete' "{`"id`":`"$brandId`"}" | Out-Null
Test-Endpoint 'Categories' 'Delete' "{`"id`":`"$catId`"}" | Out-Null
Write-Host "Cleanup done."
