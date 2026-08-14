$ErrorActionPreference = 'SilentlyContinue'
$root = Join-Path $env:TEMP 'BaseLib-StS2'
Get-ChildItem -Path $root -Recurse -Filter 'CustomCardModel.cs' | ForEach-Object { $_.FullName }
Get-ChildItem -Path $root -Recurse -Filter '*.cs' | Select-String -Pattern 'HasEnergyCostX|ResolveEnergyXValue' | ForEach-Object { $_.ToString() }
