$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'build.ps1')
$root = $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$qa = Join-Path $root 'qa'
New-Item -ItemType Directory -Force -Path $qa | Out-Null
& $compiler /nologo /target:exe /codepage:65001 "/out:$root\build\Tests.exe" /r:System.Drawing.dll /r:System.Xml.dll /r:System.Core.dll "$root\GameModel.cs" "$root\Economy.cs" "$root\Tunnel.cs" "$root\Storage.cs" "$root\GameView.cs" "$root\SocialView.cs" "$root\StorageView.cs" "$root\Tests.cs" "$root\TunnelTests.cs" "$root\StorageTests.cs"
if ($LASTEXITCODE -ne 0) { throw 'Compilarea testelor a eșuat.' }
& "$root\build\Tests.exe" $qa | Tee-Object "$qa\tests.txt"
if ($LASTEXITCODE -ne 0) { throw 'Testele motorului au eșuat.' }
$process = Start-Process -FilePath "$root\build\Blocul17.exe" -ArgumentList @('--smoke-test', ('"'+$qa+'\window.txt"')) -WindowStyle Hidden -Wait -PassThru
if ($process.ExitCode -ne 0) { throw 'Testul ferestrei a eșuat.' }
Get-Content -LiteralPath "$qa\window.txt"
$process = Start-Process -FilePath "$root\build\Blocul17.exe" -ArgumentList @('--render', ('"'+$qa+'"')) -WindowStyle Hidden -Wait -PassThru
if ($process.ExitCode -ne 0) { throw 'Randarea de verificare a eșuat.' }
