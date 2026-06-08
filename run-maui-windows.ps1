$ErrorActionPreference = "Stop"

$repo = Split-Path -Parent $MyInvocation.MyCommand.Path
$dotnet = Join-Path $env:USERPROFILE ".dotnet\dotnet.exe"

if (-not (Test-Path $dotnet)) {
    Write-Host "SDK local do .NET nao encontrado. Instalando .NET 8 em $env:USERPROFILE\.dotnet..."
    $installScript = Join-Path $env:TEMP "dotnet-install.ps1"
    Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $installScript
    & powershell -NoProfile -ExecutionPolicy Bypass -File $installScript -Channel 8.0 -InstallDir "$env:USERPROFILE\.dotnet" -Architecture x64
}

$env:DOTNET_ROOT = Join-Path $env:USERPROFILE ".dotnet"
$env:PATH = "$env:DOTNET_ROOT;$env:DOTNET_ROOT\tools;$env:PATH"

cmd /c "subst Z: /D >nul 2>nul"
cmd /c "subst Z: `"$repo`""

$project = "Z:\DeltaFour.Maui\DeltaFour.Maui.csproj"
$framework = "net8.0-windows10.0.19041.0"
$outputExe = "Z:\DeltaFour.Maui\bin\Debug\$framework\win-x64\DeltaFour.Maui.exe"

Write-Host "Verificando workload MAUI..."
$workloads = & $dotnet workload list
if ($workloads -notmatch "maui") {
    & $dotnet workload install maui
}

Write-Host "Compilando app MAUI Windows..."
& $dotnet build $project `
    -f $framework `
    -p:WindowsPackageType=None `
    -p:WindowsAppSDKSelfContained=true `
    -p:SelfContained=true `
    -p:RuntimeIdentifier=win-x64

if (-not (Test-Path $outputExe)) {
    throw "Executavel nao encontrado: $outputExe"
}

Write-Host "Abrindo Ponto IA..."
Start-Process -FilePath $outputExe -WorkingDirectory (Split-Path $outputExe)
