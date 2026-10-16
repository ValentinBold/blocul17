param(
    [string]$Version = '',
    [string]$OutputDirectory = 'D:\04_DOWNLOADS\Blocul17_GitHub_Releases'
)
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$assembly = Get-Content -LiteralPath (Join-Path $root 'AssemblyInfo.cs') -Raw
$assemblyVersion = [regex]::Match($assembly, 'AssemblyVersion\("([0-9]+\.[0-9]+\.[0-9]+)\.0"\)').Groups[1].Value
if (!$assemblyVersion) { throw 'Cannot determine assembly version.' }
if (!$Version) { $Version = $assemblyVersion }
if ($Version -notmatch '^[0-9]+\.[0-9]+\.[0-9]+$' -or $Version -ne $assemblyVersion) { throw 'Tag and assembly versions differ.' }
if ($assembly -notmatch ('AssemblyFileVersion\("' + [regex]::Escape($Version) + '\.0"\)')) { throw 'File version differs.' }
[xml]$manifestXml = Get-Content -LiteralPath (Join-Path $root 'app.manifest')
if ($manifestXml.assembly.assemblyIdentity.version -ne "$Version.0") { throw 'Manifest version differs.' }
$changelog = Get-Content -LiteralPath (Join-Path $root 'CHANGELOG.md') -Raw -Encoding UTF8
$section = [regex]::Match($changelog, '(?ms)^## ' + [regex]::Escape($Version) + ' [^\r\n]*\r?\n(.*?)(?=^## |\z)')
if (!$section.Success) { throw 'Missing changelog section.' }
$name = "Blocul17_Windows_v$Version"
$out = [IO.Path]::GetFullPath($OutputDirectory)
$stage = Join-Path $out $name
$zip = Join-Path $out "$name.zip"
if ((Test-Path -LiteralPath $stage) -or (Test-Path -LiteralPath $zip)) { throw 'Output already exists; choose a fresh directory or version.' }
& (Join-Path $root 'test.ps1')
New-Item -ItemType Directory -Path $stage | Out-Null
New-Item -ItemType Directory -Path (Join-Path $stage 'surse'), (Join-Path $stage 'dovezi') | Out-Null
Copy-Item -LiteralPath (Join-Path $root 'build\Blocul17.exe') -Destination $stage
foreach ($file in @('CITESTE_MA.txt', 'VERIFICARE.md', 'CHANGELOG.md', 'LICENSE', 'README.md')) {
    Copy-Item -LiteralPath (Join-Path $root $file) -Destination $stage
}
# Include the complete public project, without Git metadata, generated output or personal files.
$sourceFiles = @('GameModel.cs','Economy.cs','Tunnel.cs','Storage.cs','Waiting.cs','Security.cs','Planning.cs','PlanningView.cs','PlanningTests.cs','SecurityView.cs','SecurityTests.cs','UPDATE_0_6_PLAN.md','GameView.cs','SocialView.cs','StorageView.cs','Program.cs','AssemblyInfo.cs','app.manifest','Tests.cs','TunnelTests.cs','StorageTests.cs','WaitingTests.cs','build.ps1','test.ps1','package.ps1','DESIGN.md','README.md','CITESTE_MA.txt','VERIFICARE.md','CHANGELOG.md','CONTRIBUTING.md','RELEASING.md','LICENSE','.gitignore','.gitattributes')
foreach ($file in $sourceFiles) { Copy-Item -LiteralPath (Join-Path $root $file) -Destination (Join-Path $stage 'surse') }
foreach ($dir in @('fixtures','docs','.github')) { Copy-Item -LiteralPath (Join-Path $root $dir) -Destination (Join-Path $stage 'surse') -Recurse }
Copy-Item -LiteralPath (Join-Path $root 'docs') -Destination $stage -Recurse
foreach ($file in @('tests.txt','window.txt')) { Copy-Item -LiteralPath (Join-Path $root "qa\$file") -Destination (Join-Path $stage 'dovezi') }
$hashes = [ordered]@{}
foreach ($file in (Get-ChildItem -LiteralPath $stage -Recurse -File | Sort-Object FullName)) {
    $relative = $file.FullName.Substring($stage.Length + 1).Replace('\','/')
    $hashes[$relative] = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
}
$hashes | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $stage 'SHA256.json') -Encoding UTF8
Add-Type -AssemblyName System.IO.Compression.FileSystem
Add-Type -AssemblyName System.IO.Compression
$archive = [IO.Compression.ZipFile]::Open($zip, [IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($file in (Get-ChildItem -LiteralPath $stage -Recurse -File | Sort-Object FullName)) {
        $relative = $file.FullName.Substring($stage.Length + 1).Replace('\','/')
        [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $file.FullName, "$name/$relative", [IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
} finally { $archive.Dispose() }
$archive = [IO.Compression.ZipFile]::OpenRead($zip)
try {
    foreach ($key in $hashes.Keys) {
        $entry = $archive.GetEntry("$name/$key")
        if (!$entry) { throw "Archive entry missing: $key" }
        $stream = $entry.Open()
        $sha = [Security.Cryptography.SHA256]::Create()
        try { $actual = [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','').ToLowerInvariant() }
        finally { $stream.Dispose(); $sha.Dispose() }
        if ($actual -ne $hashes[$key]) { throw "Archive checksum mismatch: $key" }
    }
} finally { $archive.Dispose() }
$zipHash = (Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant()
"$zipHash  $name.zip" | Set-Content -LiteralPath "$zip.sha256" -Encoding ASCII
$section.Value.Trim() | Set-Content -LiteralPath (Join-Path $out 'RELEASE_NOTES.md') -Encoding UTF8
Write-Output "Verified release: $zip"
Write-Output "SHA256: $zipHash"
