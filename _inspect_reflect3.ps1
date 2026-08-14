$ErrorActionPreference = 'Continue'
$dll = 'Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll'
try {
    $asm = [System.Reflection.Assembly]::LoadFrom($dll)
    foreach ($name in @('MegaCrit.Sts2.Core.Abstracts.CustomCardModel','MegaCrit.Sts2.Core.Models.CardModel','BaseLib.Abstracts.CustomCardModel')) {
        $t = $asm.GetType($name)
        if ($t) {
            Write-Output "=== Found: $($t.FullName) ==="
            $t.GetConstructors() | ForEach-Object { "CTOR: $($_.ToString())" }
            $t.GetMethod('get_HasEnergyCostX') | ForEach-Object { "get_HasEnergyCostX virtual=$($_.IsVirtual) abstract=$($_.IsAbstract)" }
            $t.GetMethod('ResolveEnergyXValue') | ForEach-Object { "ResolveEnergyXValue virtual=$($_.IsVirtual)" }
            $t.GetProperty('EnergyCost') | ForEach-Object { "EnergyCost: $($_.PropertyType.FullName)" }
            break
        } else {
            Write-Output "NOT FOUND: $name"
        }
    }
} catch {
    Write-Output "ERROR: $($_.Exception.Message)"
}
