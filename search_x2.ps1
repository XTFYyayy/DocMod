$xml = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.xml'
Write-Output '=== XCost keyword/method ==='
Select-String -Path $xml -Pattern 'IsXCost|SetXCost|XCost|CapturedXValue|CostX' | ForEach-Object { $_.Line.Trim() } | Select-Object -First 40
