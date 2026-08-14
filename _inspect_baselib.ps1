$ErrorActionPreference = 'SilentlyContinue'
$baseLib = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\Mods\BaseLib'
Write-Output '=== BaseLib structure ==='
Get-ChildItem -Path $baseLib -Recurse | Select-Object -First 60 | ForEach-Object { $_.FullName }
Write-Output '=== EnergyCost/XCost refs ==='
Get-ChildItem -Path $baseLib -Recurse -Filter '*.cs' | Select-String -Pattern 'HasEnergyCostX|CostsX|CardEnergyCost|SetEnergyCost' | ForEach-Object { $_.ToString() }
