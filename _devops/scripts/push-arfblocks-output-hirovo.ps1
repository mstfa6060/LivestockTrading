# === Fail-fast ve PATH garanti ===
$ErrorActionPreference = 'Stop'
$env:PATH = "$($env:USERPROFILE)\.dotnet\tools;$($env:APPDATA)\npm;$env:PATH"

# === BASE PATHS ===
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Definition
$backendPath = Resolve-Path "$scriptRoot\..\.."          # D:\Projects\Maden\backend
$madenRoot = Resolve-Path "$backendPath\.."            # D:\Projects\Maden

# === CONFIGURABLE PATHS ===
$hirovoCliConfigPath = "$backendPath\_devops\arfblocks-cli\hirovo.arfblocks-cli.json"
$iamCliConfigPath = "$backendPath\_devops\arfblocks-cli\hirovo-iam.arfblocks-cli.json"
$fileProviderCliConfigPath = "$backendPath\_devops\arfblocks-cli\hirovo-fileprovider.arfblocks-cli.json"

$frontendPath = "$madenRoot\frontend-hirovo"
$frontendBranch = "main"

$mobilePath = "$madenRoot\hirovo_mobil_bare"
$mobileBranch = "main"

# === arfblocks-cli bulun ve tam yol ile çağır ===
$arf = Get-Command arfblocks-cli -ErrorAction SilentlyContinue
if (-not $arf) {
    $candidate = Join-Path $env:USERPROFILE ".dotnet\tools\arfblocks-cli.exe"
    if (Test-Path $candidate) {
        $arfExe = $candidate
    }
    else {
        throw "arfblocks-cli bulunamadı. ~/.dotnet/tools altına kurulu mu ve PATH eklendi mi?"
    }
}
else {
    $arfExe = $arf.Source   # örn: C:\Users\...\arfblocks-cli.exe
}
Write-Host "🔎 Using arfblocks-cli at: $arfExe"

function Invoke-ArfBlocks([string]$configPath) {
    if (-not (Test-Path $configPath)) { throw "Config bulunamadı: $configPath" }
    & $arfExe exec --file $configPath
    if ($LASTEXITCODE -ne 0) { throw "arfblocks-cli exec başarısız (exit $LASTEXITCODE) -> $configPath" }
}

# === 1. ArfBlocks CLI çıktısı oluştur ===
Write-Host "`n📦 ArfBlocks CLI çıktısı oluşturuluyor..."
Set-Location $backendPath

Write-Host "`n▶️ Hirovo modülü..."
Invoke-ArfBlocks $hirovoCliConfigPath

Write-Host "`n▶️ IAM modülü..."
Invoke-ArfBlocks $iamCliConfigPath

Write-Host "`n▶️ FileProvider modülü..."
Invoke-ArfBlocks $fileProviderCliConfigPath

Write-Host "`n CLI çıktısı tamamlandı.`n"

# === 2. Error Code Export ===
Write-Host "`n📦 Error Code Export çalıştırılıyor..."
$exporterPath = "$backendPath\Jobs\SpecialPurpose\DevTasks\ErrorCodeExporter"
if (Test-Path $exporterPath) {
    Set-Location $exporterPath
    dotnet run
    if ($LASTEXITCODE -ne 0) { throw "ErrorCodeExporter başarısız (exit $LASTEXITCODE)" }
    Write-Host " Error Code Export tamamlandı.`n"
}
else {
    throw "❌ ErrorCodeExporter yolu bulunamadı: $exporterPath"
}

# === 3. Frontend push ===
if (Test-Path $frontendPath) {
    Write-Host "🚀 Frontend'e gönderiliyor..."
    Set-Location $frontendPath
    if (-not (Test-Path ".git")) { throw "Bu klasör git repo değil: $frontendPath" }
    git add -A
    git commit -m "🔄 Otomatik ArfBlocks ve hata mesajı güncellemesi [frontend]" --allow-empty
    git push origin $frontendBranch
    Write-Host " Frontend güncellendi.`n"
}
else {
    throw "❌ Frontend yolu bulunamadı: $frontendPath"
}

# === 4. MobileApp push ===
if (Test-Path $mobilePath) {
    Write-Host "🚀 Mobil app'e gönderiliyor..."
    Set-Location $mobilePath
    if (-not (Test-Path ".git")) { throw "Bu klasör git repo değil: $mobilePath" }
    git add -A
    git commit -m "🔄 Otomatik ArfBlocks ve hata mesajı güncellemesi [mobileapp]" --allow-empty
    git push origin $mobileBranch
    Write-Host " Mobil app güncellendi.`n"
}
else {
    throw "❌ Mobil app yolu bulunamadı: $mobilePath"
}
