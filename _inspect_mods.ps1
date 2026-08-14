$ErrorActionPreference = 'SilentlyContinue'
$mods = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\Mods'
Write-Output '=== Mods dirs ==='
Get-ChildItem -Path $mods -Directory | ForEach-Object { $_.FullName }
Write-Output '=== BaseLib files ==='
Get-ChildItem -Path (Join-Path $mods 'BaseLib') -Recurse -File | Select-Object -First 40 | ForEach-Object { $_.FullName }
