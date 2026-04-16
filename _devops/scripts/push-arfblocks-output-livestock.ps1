# =========================
# push-arfblocks-output-livestock.ps1
# =========================
#
# Yeni akis (v2):
#   0. Dizinleri olustur
#   1. Web & Mobile git pull (ONCE cevirileri cek)
#   2. ArfBlocks CLI output (API client)
#   3. ErrorCodeExporter (sadece tr.ts uretir)
#   4. translate-errors.js --sync (yeni keyleri diger dillere ekle, cevirileri koru)
#   5. Web git add + commit + push
#   6. Mobile git add + commit + push
#
# Tam ceviri icin (Google Translate API ile):
#   cd web && node scripts/translate-errors.js --missing
#

# Hatalarda dur
$ErrorActionPreference = 'Stop'

# PATH (dotnet tools + npm + node)
$env:PATH = "$($env:USERPROFILE)\.dotnet\tools;$($env:APPDATA)\npm;$env:PATH"

# === BASE PATHS ===
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backendPath = Resolve-Path "$scriptRoot\..\.."
$projectRoot = Split-Path -Parent $backendPath

# === CONFIGURABLE PATHS ===
$livestockCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock.arfblocks-cli.json"
$iamCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock-iam.arfblocks-cli.json"
$fileProviderCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock-fileprovider.arfblocks-cli.json"

$webPath = "$projectRoot\livestock-frontend"
$webBranch = "main"

$mobilePath = "$projectRoot\livestock-mobile"
$mobileBranch = "main"

# === arfblocks-cli'yi bul (tam yol ile cagiracagiz) ===
$arf = Get-Command arfblocks-cli -ErrorAction SilentlyContinue
if (-not $arf) {
    $candidate = Join-Path $env:USERPROFILE ".dotnet\tools\arfblocks-cli.exe"
    if (Test-Path $candidate) {
        $arfExe = $candidate
    }
    else {
        throw "arfblocks-cli not found. Install: dotnet tool install --global Arfware.ArfBlocks-cli --version 2.0.0"
    }
}
else {
    $arfExe = $arf.Source
}
Write-Host "[OK] Using arfblocks-cli at: $arfExe" -ForegroundColor Green

function Invoke-ArfBlocks([string]$configPath) {
    if (-not (Test-Path $configPath)) {
        Write-Host "[WARN] Config not found: $configPath" -ForegroundColor Yellow
        Write-Host "       Skipping this module..." -ForegroundColor Yellow
        return
    }
    Write-Host "[RUN] Executing: $configPath" -ForegroundColor Cyan
    & $arfExe exec --file $configPath
    $code = $LASTEXITCODE
    if ($code -ne 0) { throw "arfblocks-cli exec failed. Code: $code -> $configPath" }
}

function Ensure-GitRepo([string]$path) {
    if (-not (Test-Path $path)) {
        Write-Host "[WARN] Folder not found: $path" -ForegroundColor Yellow
        return $false
    }
    if (-not (Test-Path (Join-Path $path ".git"))) {
        Write-Host "[WARN] Not a git repo: $path" -ForegroundColor Yellow
        return $false
    }
    return $true
}

# === 0. Ensure output directories exist ===
Write-Host ""
Write-Host "[INFO] Ensuring output directories exist..." -ForegroundColor Yellow

$webApiPath = "$projectRoot\web\common\livestock-api\src\api\business_modules\livestocktrading"
$mobileApiPath = "$projectRoot\mobil\common\livestock-api\src\api\business_modules\livestocktrading"

if (-not (Test-Path $webApiPath)) {
    New-Item -ItemType Directory -Path $webApiPath -Force | Out-Null
    Write-Host "[OK] Created web API directory: $webApiPath" -ForegroundColor Green
}

if (-not (Test-Path $mobileApiPath)) {
    New-Item -ItemType Directory -Path $mobileApiPath -Force | Out-Null
    Write-Host "[OK] Created mobile API directory: $mobileApiPath" -ForegroundColor Green
}

# === 1. Git Pull ONCE (cevirileri kaybet diye) ===
Write-Host ""
Write-Host "[INFO] Step 1: Pulling latest from web & mobile repos..." -ForegroundColor Yellow

if (Ensure-GitRepo $webPath) {
    Push-Location $webPath
    Write-Host "  Pulling web ($webBranch)..." -ForegroundColor Cyan
    git pull --rebase --autostash
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[WARN] git pull (web) failed, continuing..." -ForegroundColor Yellow
    }
    Pop-Location
}

if (Ensure-GitRepo $mobilePath) {
    Push-Location $mobilePath
    Write-Host "  Pulling mobile ($mobileBranch)..." -ForegroundColor Cyan
    git pull --rebase --autostash
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[WARN] git pull (mobile) failed, continuing..." -ForegroundColor Yellow
    }
    Pop-Location
}

Write-Host "[OK] Git pull completed." -ForegroundColor Green

# === 2. ArfBlocks CLI output ===
Write-Host ""
Write-Host "[INFO] Step 2: ArfBlocks CLI output is being generated..." -ForegroundColor Yellow
Push-Location $backendPath

Write-Host ""
Write-Host "[1/3] LivestockTrading module..." -ForegroundColor Magenta
Invoke-ArfBlocks $livestockCliConfigPath

Write-Host ""
Write-Host "[2/3] IAM module..." -ForegroundColor Magenta
Invoke-ArfBlocks $iamCliConfigPath

Write-Host ""
Write-Host "[3/3] FileProvider module..." -ForegroundColor Magenta
Invoke-ArfBlocks $fileProviderCliConfigPath

Write-Host ""
Write-Host "[OK] CLI output completed." -ForegroundColor Green
Pop-Location

# === 3. Error Code Export (sadece tr.ts) ===
Write-Host ""
Write-Host "[INFO] Step 3: Running Error Code Export (tr.ts only)..." -ForegroundColor Yellow
$exporterPath = "$backendPath\Jobs\SpecialPurpose\DevTasks\ErrorCodeExporter"
if (Test-Path $exporterPath) {
    Push-Location $exporterPath

    dotnet run
    $code = $LASTEXITCODE
    if ($code -ne 0) {
        Write-Host "[WARN] ErrorCodeExporter failed. Code: $code" -ForegroundColor Yellow
        Write-Host "       Continuing anyway..." -ForegroundColor Yellow
    } else {
        Write-Host "[OK] Error Code Export completed (tr.ts)." -ForegroundColor Green
    }
    Pop-Location
}
else {
    Write-Host "[WARN] ErrorCodeExporter path not found: $exporterPath" -ForegroundColor Yellow
    Write-Host "       Skipping error code export..." -ForegroundColor Yellow
}

# === 4. Translate errors (sync new keys to all languages) ===
Write-Host ""
Write-Host "[INFO] Step 4: Syncing error keys to all languages..." -ForegroundColor Yellow

$translateScript = "$webPath\scripts\translate-errors.js"
if (Test-Path $translateScript) {
    Push-Location $webPath

    # --sync: Cevirmeden yeni keyleri Turkce fallback ile ekler, mevcut cevirileri korur
    # --missing: Ucretsiz Google Translate ile cevrilmemis keyleri cevirir (API key gerektirmez)
    Write-Host "  Ucretsiz Google Translate ile cevrilmemis keyler cevriliyor..." -ForegroundColor Cyan
    node scripts/translate-errors.js --missing

    $code = $LASTEXITCODE
    if ($code -ne 0) {
        Write-Host "[WARN] translate-errors.js failed. Code: $code" -ForegroundColor Yellow
        Write-Host "       Continuing anyway..." -ForegroundColor Yellow
    } else {
        Write-Host "[OK] Error translation sync completed." -ForegroundColor Green
    }
    Pop-Location

    # Mobile icin: web'deki error dosyalarini mobile'a kopyala
    $webErrorsPath = "$webPath\common\livestock-api\src\errors\locales\modules\backend"
    $mobileErrorsPath = "$mobilePath\common\livestock-api\src\errors\locales\modules\backend"
    if ((Test-Path $webErrorsPath) -and (Test-Path $mobilePath)) {
        Write-Host "  Copying error translations to mobile..." -ForegroundColor Cyan
        # common modulunu kopyala
        if (Test-Path "$mobileErrorsPath\common") {
            Copy-Item -Path "$webErrorsPath\common\*.ts" -Destination "$mobileErrorsPath\common\" -Force
        }
        # livestocktrading modulunu kopyala
        if (Test-Path "$mobileErrorsPath\livestocktrading") {
            Copy-Item -Path "$webErrorsPath\livestocktrading\*.ts" -Destination "$mobileErrorsPath\livestocktrading\" -Force
        }
        Write-Host "[OK] Error translations copied to mobile." -ForegroundColor Green
    }
}
else {
    Write-Host "[WARN] translate-errors.js not found: $translateScript" -ForegroundColor Yellow
    Write-Host "       Skipping error translation..." -ForegroundColor Yellow
}

# === 5. Web push ===
if (Ensure-GitRepo $webPath) {
    Write-Host ""
    Write-Host "[INFO] Step 5: Pushing to Web..." -ForegroundColor Yellow
    Push-Location $webPath

    git add -A
    git commit -m "Auto ArfBlocks and error message update [Livestock-Web]" --allow-empty
    git push origin $webBranch

    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Web push completed." -ForegroundColor Green
    } else {
        Write-Host "[WARN] Web push failed." -ForegroundColor Yellow
    }

    Pop-Location
}
else {
    Write-Host "[WARN] Web path not found or not a git repo: $webPath" -ForegroundColor Yellow
    Write-Host "       Skipping web push..." -ForegroundColor Yellow
}

# === 6. Mobile push ===
if (Ensure-GitRepo $mobilePath) {
    Write-Host ""
    Write-Host "[INFO] Step 6: Pushing to Mobile..." -ForegroundColor Yellow
    Push-Location $mobilePath

    git add -A
    git commit -m "Auto ArfBlocks and error message update [Livestock-Mobile]" --allow-empty
    git push origin $mobileBranch

    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Mobile push completed." -ForegroundColor Green
    } else {
        Write-Host "[WARN] Mobile push failed." -ForegroundColor Yellow
    }

    Pop-Location
}
else {
    Write-Host "[WARN] Mobile path not found or not a git repo: $mobilePath" -ForegroundColor Yellow
    Write-Host "       Skipping mobile push..." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "[DONE] All steps completed!" -ForegroundColor Green
Write-Host "Backend: $backendPath" -ForegroundColor Cyan
Write-Host "Web:     $webPath" -ForegroundColor Cyan
Write-Host "Mobile:  $mobilePath" -ForegroundColor Cyan
Write-Host ""
Write-Host "Tekrar ceviri icin (ucretsiz, API key gerektirmez):" -ForegroundColor Yellow
Write-Host "  cd $webPath" -ForegroundColor Cyan
Write-Host "  node scripts/translate-errors.js --missing   (cevrilmemis keyleri cevir)" -ForegroundColor Cyan
Write-Host "  node scripts/translate-errors.js --all       (hepsini yeniden cevir)" -ForegroundColor Cyan
Write-Host ""
