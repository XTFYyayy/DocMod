$ErrorActionPreference = 'Continue'
$dll = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll'
try {
    $asm = [System.Reflection.Assembly]::LoadFrom($dll)
    Write-Output '=== Loaded sts2.dll ==='
    $t = $asm.GetType('MegaCrit.Sts2.Core.Abstracts.CustomCardModel')
    if (-not $t) {
        $t = $asm.GetTypes() | Where-Object { $_.Name -eq 'CustomCardModel' } | Select-Object -First 1
    }
    if ($t) {
        Write-Output "=== Found: $($t.FullName) ==="
        $t.GetConstructors() | ForEach-Object { "CTOR: $($_.ToString())" }
        Write-Output '=== HasEnergyCostX / ResolveEnergyXValue ==='
        $t.GetMethod('get_HasEnergyCostX') | ForEach-Object { "get_HasEnergyCostX virtual=$($_.IsVirtual)" }
        $t.GetMethod('ResolveEnergyXValue') | ForEach-Object { "ResolveEnergyXValue virtual=$($_.IsVirtual)" }
        Write-Output '=== EnergyCost prop ==='
        $t.GetProperty('EnergyCost') | ForEach-Object { "EnergyCost: $($_.PropertyType.FullName)" }
    } else {
        Write-Output 'CustomCardModel NOT FOUND'
    }
} catch {
    Write-Output "ERROR: $($_.Exception.Message)"
}
