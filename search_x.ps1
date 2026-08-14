$xml = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.xml'
Write-Output '=== XML Cost ==='
Select-String -Path $xml -Pattern 'XCost|VariableCost|SpendAllEnergy|AllEnergy|X-cost' | ForEach-Object { $_.Line.Trim() } | Select-Object -First 20
Write-Output '=== BaseLib X ==='
Get-ChildItem -Path 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\Mods\BaseLib' -Recurse -Filter '*.cs' -ErrorAction SilentlyContinue | Select-String -Pattern 'XCost|VariableCost|SpendAllEnergy|X\s*费用|X-cost|AllEnergy' | ForEach-Object { $_.ToString() } | Select-Object -First 20
