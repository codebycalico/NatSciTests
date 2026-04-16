$password = ConvertTo-SecureString "ismoomsi" -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ('OMSI-Admin', $password)
$ShortcutHeavym = "C:\OMSI\App\SpoutMap.lnk"
$arg = "-NoExit WindowStyle Minimized -ExecutionPolicy bypass -File $WatchDog"

cmd /c $ShortcutHeavym -credential $credential