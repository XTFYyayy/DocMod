$ErrorActionPreference = 'SilentlyContinue'
$mods = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\Mods'
Get-ChildItem -Path (Join-Path $mods 'BaseLib') -Recurse -File | ForEach-Object { $_.FullName }
Write-Output '=== search xml for CustomCardModel ==='
Get-ChildItem -Path (Join-Path $mods 'BaseLib') -Recurse -Filter '*.xml' | Select-String -Pattern 'CustomCardModel' | ForEach-Object { $_.ToString() } | Select-Object -First 20
