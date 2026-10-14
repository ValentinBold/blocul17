$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { throw 'Este necesar .NET Framework 4.x pe Windows.' }
$bin = Join-Path $root 'build'
New-Item -ItemType Directory -Force -Path $bin | Out-Null
& $compiler /nologo /target:winexe /platform:anycpu /optimize+ /codepage:65001 "/out:$bin\Blocul17.exe" "/win32manifest:$root\app.manifest" /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Xml.dll /r:System.Core.dll "$root\GameModel.cs" "$root\Economy.cs" "$root\Tunnel.cs" "$root\Storage.cs" "$root\GameView.cs" "$root\SocialView.cs" "$root\StorageView.cs" "$root\Program.cs" "$root\AssemblyInfo.cs"
if ($LASTEXITCODE -ne 0) { throw 'Compilarea jocului a eșuat.' }
Write-Output "Creat: $bin\Blocul17.exe"
