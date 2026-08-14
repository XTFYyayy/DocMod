$ErrorActionPreference = 'SilentlyContinue'
$dll = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\Mods\BaseLib\BaseLib.dll'
$xml = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.xml'
Write-Output '=== BaseLib.dll exists? ==='
Test-Path $dll
Write-Output '=== CustomCardModel ctor doc ==='
Select-String -Path $xml -Pattern 'CustomCardModel\.#ctor' -Context 0,20 | Select-Object -First 3 | ForEach-Object { ($_.Line + "`n" + ($_.Context.PostContext -join "`n")) }
Write-Output '=== HasEnergyCostX doc ==='
Select-String -Path $xml -Pattern 'MegaCrit.Sts2.Core.Models.CardModel.HasEnergyCostX' -Context 0,15 | Select-Object -First 2 | ForEach-Object { ($_.Line + "`n" + ($_.Context.PostContext -join "`n")) }
Write-Output '=== ResolveEnergyXValue doc ==='
Select-String -Path $xml -Pattern 'ResolveEnergyXValue' -Context 0,10 | Select-Object -First 2 | ForEach-Object { ($_.Line + "`n" + ($_.Context.PostContext -join "`n")) }
