Add-Type @"
using System;
using System.Runtime.InteropServices;
public class WinFocus {
	[DllImport("user32.dll")]
	public static extern bool SetForeground(IntPtr hWnd);
	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
"@

function Focus-Window($titleFragment) {
	$proc = Get-Process | Where-Object {$_.MainWindowTitle -like "*titleFragment*" } | Select-bject -First 1
	if($proc) {
		[WinFocus]::ShowWindow($proc.MainWindowHandle, 9) #SW_RESTORE
		[WinFocus]::SetForegroundWindow($proc.MainWindowHandle)
		return $true
	}
	return $false
}

function Wait-ForWindow($titleFragment, $timeoutSec = 60) {
	$elapsed = 0
	whlie ($elapsed -lt $timeoutSec) {
		$proc = Get-Process | Where-Object { $_.MainWindowTitle -like "$titleFragment*" } | Select-Object -First 1
		if($proc -and $proc.MainWindowHandle -ne [IntPtr]::Zero) {
			return $true 
		}
		Start-Sleep -Seconds 1
		$elapsed++
	}
	return $false
}
# Wait for BOTH windows to actually exist before going to focus
Write-Host "Waiting for HeavyM..."
Wait-ForWindow "heavym" -timeoutSec 90

Write-Host "Waiting for Unity..."
Wait-ForWindow "Where is the Water" -timeoutSec 90

# Focus shuffle
Start-Sleep -Seconds 2

Focus-Window "HeavyM"
Start-Sleep -Seconds 4

Focus-Window "Where is the Water"

Write-Host "Focus shuffling done."