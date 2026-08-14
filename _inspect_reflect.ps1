$ErrorActionPreference = 'SilentlyContinue'
$dll = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\Mods\BaseLib\BaseLib.dll'
Add-Type -Path $dll
$asm = [System.Reflection.Assembly]::LoadFrom($dll)
Write-Output '=== CustomCardModel ctors ==='
$t = $asm.GetType('BaseLib.Abstracts.CustomCardModel')
if ($t) {
    $t.GetConstructors() | ForEach-Object { $_.ToString() }
    Write-Output '=== CustomCardModel members (cost/energy related) ==='
    $t.GetProperties() | Where-Object { $_.Name -match 'Cost|Energy|X' } | ForEach-Object { "PROP: $($_.PropertyType.Name) $($_.Name) canWrite=$($_.CanWrite)" }
    Write-Output '=== HasEnergyCostX? ==='
    $t.GetMethod('get_HasEnergyCostX') | ForEach-Object { "METHOD: $($_.ToString()) virtual=$($_.IsVirtual) abstract=$($_.IsAbstract)" }
} else {
    Write-Output 'CustomCardModel NOT FOUND in BaseLib.dll'
    Write-Output '=== all types matching Custom ==='
    $asm.GetTypes() | Where-Object { $_.Name -match 'Custom' } | ForEach-Object { $_.FullName }
}
