$ErrorActionPreference = 'Continue'
$dll = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\Mods\BaseLib\BaseLib.dll'
try {
    $asm = [System.Reflection.Assembly]::LoadFrom($dll)
    $t = $asm.GetType('BaseLib.Abstracts.CustomCardModel')
    if ($t) {
        Write-Output "=== Found: $($t.FullName) base=$($t.BaseType.FullName) ==="
        $t.GetConstructors([System.Reflection.BindingFlags]'Public,NonPublic,Instance') | ForEach-Object { "CTOR: $($_.ToString())" }
        $t.GetProperties([System.Reflection.BindingFlags]'Public,NonPublic,Instance') | Where-Object { $_.Name -match 'Cost|Energy|X' } | ForEach-Object { "PROP: $($_.PropertyType.Name) $($_.Name) canWrite=$($_.CanWrite)" }
        $t.GetMethod('get_HasEnergyCostX',[System.Reflection.BindingFlags]'Public,NonPublic,Instance') | ForEach-Object { "get_HasEnergyCostX virtual=$($_.IsVirtual)" }
        $t.GetMethod('ResolveEnergyXValue',[System.Reflection.BindingFlags]'Public,NonPublic,Instance') | ForEach-Object { "ResolveEnergyXValue virtual=$($_.IsVirtual)" }
    } else {
        Write-Output 'BaseLib.Abstracts.CustomCardModel NOT FOUND'
        $asm.GetTypes() | Where-Object { $_.Name -match 'CustomCard' } | ForEach-Object { $_.FullName }
    }
} catch {
    Write-Output "ERROR: $($_.Exception.Message)"
}
