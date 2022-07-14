<#
-- Prerequisites --
1) Enable script execution
- Open Powershell ISE (as Administrator) and run the following commands:
Get-ExecutionPolicy
Set-ExecutionPolicy -ExecutionPolicy Unrestricted
Get-ExecutionPolicy
2) Download PsTools suite
https://docs.microsoft.com/en-us/sysinternals/downloads/pstools
3) Extract PsTools to C:\PsTools
If you extract anywhere else, change the value below for $psToolsPath

-- Running the script --
1) Save the script to whereever (say D:\)
2) Run the script in Powershell ISE with the command:
powershell "D:\Clear-PublicSession.ps1"
#>

$psToolsPath = "C:\PSTools"
$suspendTimeSeconds = 8
$processName = "GTA5"

$executablePath = Join-Path -Path $psToolsPath -ChildPath "pssuspend64.exe"
if (-not(Test-Path $executablePath))
{
	Write-Host "Process suspender not found at $executablePath"
	Exit
}

pushd $psToolsPath
try
{
	$currentTime = Get-Date -UFormat "%m/%d/%Y %r"
	Write-Host ""
	Write-Host "Current time $currentTime"

	# Make sure process is running
	[void](Get-Process -Name $processName -ErrorAction Stop)

	# Check whether the EULA for pssuspend64 has been accepted
	# Get-Item -Path "HKCU:\SOFTWARE\Sysinternals\PsSuspend"

	# Suspend the process
	# The EULA will be accepted automatically upon first runtime
	.\pssuspend64.exe $processName -accepteula

	# Delay
	Write-Host ""
	Write-Host "Countdown to resume the process"
	for ($secondOnTimer = $suspendTimeSeconds; $secondOnTimer -gt 0; $secondOnTimer--)
	{
		Write-Host $secondOnTimer
		Start-Sleep -Seconds 1
	}

	# Resume the process
	.\pssuspend64.exe $processName -r
}
catch
{
	"GTA5 process not running"
}
