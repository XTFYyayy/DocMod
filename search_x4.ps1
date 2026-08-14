$xml = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.xml'
Write-Output '=== HasEnergyCostX full doc ==='
Select-String -Path $xml -Pattern 'HasEnergyCostX' -Context 5,15 | Select-Object -First 1 | ForEach-Object { ($_.Context.PreContext + $_.Line + $_.Context.PostContext) -join "`n" }
Write-Output '=== X-cost-cards doc ==='
Select-String -Path $xml -Pattern 'X-cost' -Context 3,10 | Select-Object -First 3 | ForEach-Object { ($_.Context.PreContext + $_.Line + $_.Context.PostContext) -join "`n" }
