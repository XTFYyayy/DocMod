$bytes = [System.IO.File]::ReadAllBytes('Y:\SteamLibrary\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll')
$text = [System.Text.Encoding]::ASCII.GetString($bytes)
foreach ($name in @('SlowPower','StunPower','ParalyzePower','VulnerablePower','WeakPower','SlowingPower')) {
    $found = $text.IndexOf($name)
    Write-Output ("{0}: {1}" -f $name, $found)
}
