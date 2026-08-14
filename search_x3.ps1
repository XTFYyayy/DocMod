$xml = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.xml'
Write-Output '=== HasEnergyCostX doc ==='
Select-String -Path $xml -Pattern 'HasEnergyCostX' -Context 3,6 | ForEach-Object { ($_.Context.PreContext + $_.Line + $_.Context.PostContext) -join "`n" } | Select-Object -First 60
Write-Output '=== CapturedXValue doc ==='
Select-String -Path $xml -Pattern 'CapturedXValue' -Context 2,8 | ForEach-Object { ($_.Context.PreContext + $_.Line + $_.Context.PostContext) -join "`n" } | Select-Object -First 60
