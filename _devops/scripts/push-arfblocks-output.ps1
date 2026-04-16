# =========================
# push-arfblocks-output.ps1
# GlobalLivestock - API Client ve Error Code Export
# =========================

# Hatalarda dur
$ErrorActionPreference = 'Stop'

# PATH (dotnet tools + npm)
$env:PATH = "$($env:USERPROFILE)\.dotnet\tools;$($env:APPDATA)\npm;$env:PATH"

# === BASE PATHS ===
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backendPath = Resolve-Path "$scriptRoot\..\.."
$projectRoot = Resolve-Path "$backendPath\.."

# === CONFIGURABLE PATHS ===
$livestockCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock.arfblocks-cli.json"
$iamCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock-iam.arfblocks-cli.json"
$fileProviderCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock-fileprovider.arfblocks-cli.json"

$webPath = "$projectRoot\livestock-frontend"
$webBranch = "main"

$mobilePath = "$projectRoot\livestock-mobile"
$mobileBranch = "main"

# === arfblocks-cli bul ===
$arf = Get-Command arfblocks-cli -ErrorAction SilentlyContinue
if (-not $arf) {
    $candidate = Join-Path $env:USERPROFILE ".dotnet\tools\arfblocks-cli.exe"
    if (Test-Path $candidate) {
        $arfExe = $candidate
    }
    else {
        throw "arfblocks-cli bulunamadi. dotnet global tool olarak kurulu ve PATH'e ekli oldugundan emin olun."
    }
}
else {
    $arfExe = $arf.Source
}
Write-Host "arfblocks-cli: $arfExe"

function Invoke-ArfBlocks([string]$configPath) {
    if (-not (Test-Path $configPath)) { throw "Config bulunamadi: $configPath" }
    & $arfExe exec --file $configPath
    $code = $LASTEXITCODE
    if ($code -ne 0) { throw "arfblocks-cli exec basarisiz. Code: $code -> $configPath" }
}

function Ensure-GitRepo([string]$path) {
    if (-not (Test-Path $path)) { throw "Klasor bulunamadi: $path" }
    if (-not (Test-Path (Join-Path $path ".git"))) { throw "Git repo degil: $path" }
}

# === 1. ArfBlocks CLI ciktisi ===
Write-Host ""
Write-Host "=========================================="
Write-Host "1. ArfBlocks CLI ciktisi olusturuluyor..."
Write-Host "=========================================="
Push-Location $backendPath

Write-Host ""
Write-Host "[1/3] LivestockTrading modulu..."
if (Test-Path $livestockCliConfigPath) {
    Invoke-ArfBlocks $livestockCliConfigPath
} else {
    Write-Host "UYARI: $livestockCliConfigPath bulunamadi, atlaniyor."
}

Write-Host ""
Write-Host "[2/3] IAM modulu..."
if (Test-Path $iamCliConfigPath) {
    Invoke-ArfBlocks $iamCliConfigPath
} else {
    Write-Host "UYARI: $iamCliConfigPath bulunamadi, atlaniyor."
}

Write-Host ""
Write-Host "[3/3] FileProvider modulu..."
if (Test-Path $fileProviderCliConfigPath) {
    Invoke-ArfBlocks $fileProviderCliConfigPath
} else {
    Write-Host "UYARI: $fileProviderCliConfigPath bulunamadi, atlaniyor."
}

Write-Host ""
Write-Host "CLI ciktisi tamamlandi."
Pop-Location

# === 2. Error Code Export ===
Write-Host ""
Write-Host "=========================================="
Write-Host "2. Error Code Export calistiriliyor..."
Write-Host "=========================================="
$exporterPath = "$backendPath\Jobs\SpecialPurpose\DevTasks\ErrorCodeExporter"
if (Test-Path $exporterPath) {
    Push-Location $exporterPath
    dotnet run
    $code = $LASTEXITCODE
    if ($code -ne 0) { throw "ErrorCodeExporter basarisiz. Code: $code" }
    Pop-Location
    Write-Host "Error Code Export tamamlandi."
}
else {
    Write-Host "UYARI: ErrorCodeExporter yolu bulunamadi: $exporterPath"
}

# === 3. Web push ===
Write-Host ""
Write-Host "=========================================="
Write-Host "3. Web projesine gonderiliyor..."
Write-Host "=========================================="
if (Test-Path $webPath) {
    Ensure-GitRepo $webPath
    Push-Location $webPath
    git pull --rebase --autostash
    if ($LASTEXITCODE -ne 0) { throw "git pull (web) basarisiz." }
    git add -A
    git commit -m "Auto ArfBlocks ve error message guncellemesi [Web]" --allow-empty
    git push origin $webBranch
    Pop-Location
    Write-Host "Web push tamamlandi."
}
else {
    Write-Host "UYARI: Web yolu bulunamadi: $webPath"
}

# === 4. Mobile push ===
Write-Host ""
Write-Host "=========================================="
Write-Host "4. Mobil projesine gonderiliyor..."
Write-Host "=========================================="
if (Test-Path $mobilePath) {
    Ensure-GitRepo $mobilePath
    Push-Location $mobilePath
    git pull --rebase --autostash
    if ($LASTEXITCODE -ne 0) { throw "git pull (mobil) basarisiz." }
    git add -A
    git commit -m "Auto ArfBlocks ve error message guncellemesi [Mobile]" --allow-empty
    git push origin $mobileBranch
    Pop-Location
    Write-Host "Mobil push tamamlandi."
}
else {
    Write-Host "UYARI: Mobil yolu bulunamadi: $mobilePath"
}

Write-Host ""
Write-Host "=========================================="
Write-Host "Tum adimlar tamamlandi!"
Write-Host "=========================================="
Write-Host "Web:    $webPath"
Write-Host "Mobil:  $mobilePath"
Write-Host ""
