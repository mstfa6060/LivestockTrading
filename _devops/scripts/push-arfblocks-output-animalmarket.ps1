# =========================
# push-arfblocks-output-animalmarket.ps1 (ASCII-safe)
# =========================

# Hatalarda dur
$ErrorActionPreference = 'Stop'

# PATH (dotnet tools + npm)
$env:PATH = "$($env:USERPROFILE)\.dotnet\tools;$($env:APPDATA)\npm;$env:PATH"

# === BASE PATHS ===
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backendPath = Resolve-Path "$scriptRoot\..\.."
$madenRoot = Resolve-Path "$backendPath\.."

# === CONFIGURABLE PATHS ===
$animalmarketCliConfigPath = "$backendPath\_devops\arfblocks-cli\animalmarket.arfblocks-cli.json"
$iamCliConfigPath = "$backendPath\_devops\arfblocks-cli\animalmarket-iam.arfblocks-cli.json"
$fileProviderCliConfigPath = "$backendPath\_devops\arfblocks-cli\animalmarket-fileprovider.arfblocks-cli.json"

$frontendPath = "$madenRoot\frontend-animalmarket"
$frontendBranch = "main"

$mobilePath = "$madenRoot\animalmarket_mobil"
$mobileBranch = "main"

# === arfblocks-cli'yi bul (tam yol ile çağıracağız) ===
$arf = Get-Command arfblocks-cli -ErrorAction SilentlyContinue
if (-not $arf) {
    $candidate = Join-Path $env:USERPROFILE ".dotnet\tools\arfblocks-cli.exe"
    if (Test-Path $candidate) {
        $arfExe = $candidate
    }
    else {
        throw "arfblocks-cli not found. Ensure it is installed as a dotnet global tool and on PATH."
    }
}
else {
    $arfExe = $arf.Source
}
Write-Host "Using arfblocks-cli at: $arfExe"

function Invoke-ArfBlocks([string]$configPath) {
    if (-not (Test-Path $configPath)) { throw "Config not found: $configPath" }
    & $arfExe exec --file $configPath
    $code = $LASTEXITCODE
    if ($code -ne 0) { throw "arfblocks-cli exec failed. Code: $code -> $configPath" }
}

function Ensure-GitRepo([string]$path) {
    if (-not (Test-Path $path)) { throw "Folder not found: $path" }
    if (-not (Test-Path (Join-Path $path ".git"))) { throw "Not a git repo: $path" }
}

# === 1. ArfBlocks CLI output ===
Write-Host ""
Write-Host "ArfBlocks CLI output is being generated..."
Push-Location $backendPath

Write-Host ""
Write-Host "[1/3] AnimalMarket module..."
Invoke-ArfBlocks $animalmarketCliConfigPath

Write-Host ""
Write-Host "[2/3] IAM module..."
Invoke-ArfBlocks $iamCliConfigPath

Write-Host ""
Write-Host "[3/3] FileProvider module..."
Invoke-ArfBlocks $fileProviderCliConfigPath

Write-Host ""
Write-Host "CLI output completed."
Pop-Location

# === 2. Error Code Export ===
Write-Host ""
Write-Host "Running Error Code Export..."
$exporterPath = "$backendPath\Jobs\SpecialPurpose\DevTasks\ErrorCodeExporter"
if (Test-Path $exporterPath) {
    Push-Location $exporterPath
    dotnet run
    $code = $LASTEXITCODE
    if ($code -ne 0) { throw "ErrorCodeExporter failed. Code: $code" }
    Pop-Location
    Write-Host "Error Code Export completed."
}
else {
    throw "ErrorCodeExporter path not found: $exporterPath"
}

# === 3. Frontend push ===
Ensure-GitRepo $frontendPath
Write-Host ""
Write-Host "Pushing to Frontend..."
Push-Location $frontendPath
git pull --rebase --autostash
if ($LASTEXITCODE -ne 0) { throw "git pull (frontend) failed." }
git add -A
git commit -m "Auto ArfBlocks and error message update [AnimalMarket-Frontend]" --allow-empty
git push origin $frontendBranch
Pop-Location
Write-Host "Frontend push completed."

# === 4. MobileApp push ===
Ensure-GitRepo $mobilePath
Write-Host ""
Write-Host "Pushing to Mobile app..."
Push-Location $mobilePath
git pull --rebase --autostash
if ($LASTEXITCODE -ne 0) { throw "git pull (mobile) failed." }
git add -A
git commit -m "Auto ArfBlocks and error message update [AnimalMarket-Mobile]" --allow-empty
git push origin $mobileBranch
Pop-Location
Write-Host "Mobile app push completed."

Write-Host ""
Write-Host "All steps completed successfully."
Write-Host "Frontend: $frontendPath"
Write-Host "Mobile:   $mobilePath"
Write-Host ""
