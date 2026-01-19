# =========================
# push-arfblocks-output-livestock.ps1
# =========================

# Hatalarda dur
$ErrorActionPreference = 'Stop'

# PATH (dotnet tools + npm)
$env:PATH = "$($env:USERPROFILE)\.dotnet\tools;$($env:APPDATA)\npm;$env:PATH"

# === BASE PATHS ===
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backendPath = Resolve-Path "$scriptRoot\..\.."
$projectRoot = Split-Path -Parent $backendPath

# === CONFIGURABLE PATHS ===
$livestockCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock.arfblocks-cli.json"
$iamCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock-iam.arfblocks-cli.json"
$fileProviderCliConfigPath = "$backendPath\_devops\arfblocks-cli\livestock-fileprovider.arfblocks-cli.json"

$webPath = "$projectRoot\web"
$webBranch = "main"

$mobilePath = "$projectRoot\mobil"
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

# === 1. ArfBlocks CLI output ===
Write-Host ""
Write-Host "[INFO] ArfBlocks CLI output is being generated..." -ForegroundColor Yellow
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

# === 2. Error Code Export ===
Write-Host ""
Write-Host "[INFO] Running Error Code Export..." -ForegroundColor Yellow
$exporterPath = "$backendPath\Jobs\SpecialPurpose\DevTasks\ErrorCodeExporter"
if (Test-Path $exporterPath) {
    Push-Location $exporterPath

    Write-Host "[WARN] Make sure ErrorCodeExporter includes LivestockTradingDomainErrors!" -ForegroundColor Yellow

    dotnet run
    $code = $LASTEXITCODE
    if ($code -ne 0) {
        Write-Host "[WARN] ErrorCodeExporter failed. Code: $code" -ForegroundColor Yellow
        Write-Host "       Continuing anyway..." -ForegroundColor Yellow
    } else {
        Write-Host "[OK] Error Code Export completed." -ForegroundColor Green
    }
    Pop-Location
}
else {
    Write-Host "[WARN] ErrorCodeExporter path not found: $exporterPath" -ForegroundColor Yellow
    Write-Host "       Skipping error code export..." -ForegroundColor Yellow
}

# === 3. Web push ===
if (Ensure-GitRepo $webPath) {
    Write-Host ""
    Write-Host "[INFO] Pushing to Web..." -ForegroundColor Yellow
    Push-Location $webPath

    git pull --rebase --autostash
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[WARN] git pull (web) failed, continuing..." -ForegroundColor Yellow
    }

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

# === 4. Mobile push ===
if (Ensure-GitRepo $mobilePath) {
    Write-Host ""
    Write-Host "[INFO] Pushing to Mobile..." -ForegroundColor Yellow
    Push-Location $mobilePath

    git pull --rebase --autostash
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[WARN] git pull (mobile) failed, continuing..." -ForegroundColor Yellow
    }

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
