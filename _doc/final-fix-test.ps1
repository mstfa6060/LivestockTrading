$baseUrl = 'http://localhost:5221'

function Test-Endpoint {
    param([string]$Entity, [string]$Action, [string]$Body)
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/$Entity/$Action" -Method Post -ContentType 'application/json' -Body ([System.Text.Encoding]::UTF8.GetBytes($Body)) -TimeoutSec 30
        if ($response.hasError -eq $false) {
            return @{ Status = 'OK'; Id = $response.payload.id }
        } else {
            return @{ Status = 'ERROR'; Error = $response.error.message }
        }
    } catch {
        return @{ Status = 'FAIL'; Error = $_.Exception.Message }
    }
}

Write-Host "=== Final Fix Tests (Correct Data Types) ==="

# Create parent entities
$cat = Test-Endpoint 'Categories' 'Create' '{"name":"FinalTest","slug":"finaltest","description":"Final","iconUrl":"","sortOrder":1,"isActive":true}'
$catId = $cat.Id
Write-Host "Category: $catId"

$brand = Test-Endpoint 'Brands' 'Create' '{"name":"FinalBrand","slug":"finalbrand","description":"Final","logoUrl":"","isActive":true}'
$brandId = $brand.Id
Write-Host "Brand: $brandId"

$loc = Test-Endpoint 'Locations' 'Create' '{"name":"FinalLoc","city":"Ankara","state":"Ankara","countryCode":"TR","latitude":39.93,"longitude":32.86,"address":"Final","postalCode":"06000","isActive":true}'
$locId = $loc.Id
Write-Host "Location: $locId"

$seller = Test-Endpoint 'Sellers' 'Create' '{"userId":"cccccccc-cccc-cccc-cccc-cccccccccccc","businessName":"FinalSeller","businessType":"Company","taxNumber":"9999999999","registrationNumber":"FIN001","description":"Final","logoUrl":"","bannerUrl":"","email":"final@test.com","phone":"+905559999999","website":"","isActive":true,"status":0,"businessHours":"{}","acceptedPaymentMethods":"[]","returnPolicy":"","shippingPolicy":"","socialMediaLinks":"{}"}'
$sellerId = $seller.Id
Write-Host "Seller: $sellerId"

# ShippingZones - countryCodes as proper JSON array string
$zoneBody = "{`"sellerId`":`"$sellerId`",`"name`":`"FinalZone`",`"countryCodes`":`"[`\`"TR`\`"]`",`"isActive`":true}"
$zone = Test-Endpoint 'ShippingZones' 'Create' $zoneBody
Write-Host "ShippingZones: $($zone.Status) - $($zone.Error)"
if ($zone.Status -eq 'OK') {
    $zoneId = $zone.Id
    Write-Host "  ID: $zoneId"
    $r = Test-Endpoint 'ShippingZones' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'ShippingZones' 'Delete' "{`"id`":`"$zoneId`"}"
    Write-Host "  Delete: $($r.Status)"
}

# Product for dependent entities
$prodBody = "{`"title`":`"FinalProduct`",`"slug`":`"final-product`",`"description`":`"Final`",`"shortDescription`":`"F`",`"categoryId`":`"$catId`",`"brandId`":`"$brandId`",`"basePrice`":1000.0,`"currency`":`"TRY`",`"priceUnit`":`"head`",`"stockQuantity`":10,`"stockUnit`":`"head`",`"minOrderQuantity`":1,`"maxOrderQuantity`":5,`"isInStock`":true,`"sellerId`":`"$sellerId`",`"locationId`":`"$locId`",`"status`":0,`"condition`":0,`"isShippingAvailable`":true,`"shippingCost`":100.0,`"isInternationalShipping`":false,`"weight`":500.0,`"weightUnit`":`"kg`",`"attributes`":`"{}`",`"metaTitle`":`"`",`"metaDescription`":`"`",`"metaKeywords`":`"`"}"
$prod = Test-Endpoint 'Products' 'Create' $prodBody
$productId = $prod.Id
Write-Host "Product: $productId"

# ProductDocuments - type as int (0)
$pdBody = "{`"productId`":`"$productId`",`"documentUrl`":`"https://ex.com/doc.pdf`",`"fileName`":`"doc.pdf`",`"title`":`"Doc`",`"type`":0,`"fileSizeBytes`":1024}"
$pd = Test-Endpoint 'ProductDocuments' 'Create' $pdBody
Write-Host "ProductDocuments: $($pd.Status) - $($pd.Error)"
if ($pd.Status -eq 'OK') {
    $r = Test-Endpoint 'ProductDocuments' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'ProductDocuments' 'Delete' "{`"id`":`"$($pd.Id)`"}"
    Write-Host "  Delete: $($r.Status)"
}

# FeedInfos - type as int (0)
$fiBody = "{`"productId`":`"$productId`",`"type`":0,`"targetAnimal`":`"Cattle`",`"proteinPercentage`":18.5,`"isOrganic`":true}"
$fi = Test-Endpoint 'FeedInfos' 'Create' $fiBody
Write-Host "FeedInfos: $($fi.Status) - $($fi.Error)"
if ($fi.Status -eq 'OK') {
    $r = Test-Endpoint 'FeedInfos' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'FeedInfos' 'Delete' "{`"id`":`"$($fi.Id)`"}"
    Write-Host "  Delete: $($r.Status)"
}

# ChemicalInfos - type as int (0)
$ciBody = "{`"productId`":`"$productId`",`"type`":0,`"activeIngredients`":`"Test`",`"toxicityLevel`":`"Low`",`"isOrganic`":false}"
$ci = Test-Endpoint 'ChemicalInfos' 'Create' $ciBody
Write-Host "ChemicalInfos: $($ci.Status) - $($ci.Error)"
if ($ci.Status -eq 'OK') {
    $r = Test-Endpoint 'ChemicalInfos' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'ChemicalInfos' 'Delete' "{`"id`":`"$($ci.Id)`"}"
    Write-Host "  Delete: $($r.Status)"
}

# SeedInfos - type as int (0)
$siBody = "{`"productId`":`"$productId`",`"type`":0,`"variety`":`"Test`",`"germinationRate`":95.0,`"isOrganic`":true}"
$si = Test-Endpoint 'SeedInfos' 'Create' $siBody
Write-Host "SeedInfos: $($si.Status) - $($si.Error)"
if ($si.Status -eq 'OK') {
    $r = Test-Endpoint 'SeedInfos' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'SeedInfos' 'Delete' "{`"id`":`"$($si.Id)`"}"
    Write-Host "  Delete: $($r.Status)"
}

# MachineryInfos - type as int (0)
$miBody = "{`"productId`":`"$productId`",`"type`":0,`"model`":`"T-100`",`"yearOfManufacture`":2024,`"powerHp`":100}"
$mi = Test-Endpoint 'MachineryInfos' 'Create' $miBody
Write-Host "MachineryInfos: $($mi.Status) - $($mi.Error)"
if ($mi.Status -eq 'OK') {
    $r = Test-Endpoint 'MachineryInfos' 'All' '{"PageRequest":{"Page":1,"PageSize":10}}'
    Write-Host "  All: $($r.Status)"
    $r = Test-Endpoint 'MachineryInfos' 'Delete' "{`"id`":`"$($mi.Id)`"}"
    Write-Host "  Delete: $($r.Status)"
}

# Cleanup
Write-Host ""
Write-Host "Cleaning up..."
Test-Endpoint 'Products' 'Delete' "{`"id`":`"$productId`"}" | Out-Null
Test-Endpoint 'Sellers' 'Delete' "{`"id`":`"$sellerId`"}" | Out-Null
Test-Endpoint 'Locations' 'Delete' "{`"id`":`"$locId`"}" | Out-Null
Test-Endpoint 'Brands' 'Delete' "{`"id`":`"$brandId`"}" | Out-Null
Test-Endpoint 'Categories' 'Delete' "{`"id`":`"$catId`"}" | Out-Null
Write-Host "Done!"
