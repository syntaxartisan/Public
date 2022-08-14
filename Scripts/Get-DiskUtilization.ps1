<#
.SYNOPSIS
This script helps pinpoint where your disk space is being used.
It scans a disk (or any subfolder) and display the folders 
and files in that folder in descending order by size. 

.PARAMETER PathToScan
Provide the path that you would like to scan.
#>

param(
	[parameter(position=0,mandatory=$true)][string]$PathToScan=""
)

$startTime = Get-Date

if (!(Test-Path -Path $PathToScan))
{
	Write-Host "Invalid path $PathToScan" -BackgroundColor Black -ForegroundColor Red
	exit
}

$isDirectory = (Get-Item -Path $PathToScan) -is [System.IO.DirectoryInfo]
if (!($isDirectory))
{
	Write-Host "Please select a directory for PathToScan" -BackgroundColor Black -ForegroundColor Red
	exit
}

$folderItems = [string[]](Get-ChildItem -Path $PathToScan).FullName
$unsortedDirectories = @()
$unsortedFiles = @()


for ($folderItemIndex=0; $folderItemIndex -lt $folderItems.Count;$folderItemIndex++)
{
	$currentFolderItem = $folderItems[$folderItemIndex]
	Write-Progress -Activity "Scanning item.." -Status $currentFolderItem

	$isDirectory = (Get-Item -Path $currentFolderItem) -is [System.IO.DirectoryInfo]

	try
	{
		$lengthInBytes = 0
		if ($isDirectory)
		{
			$lengthInBytes = (Get-ChildItem -Path $currentFolderItem -Recurse | Measure-Object -Property Length -Sum -ErrorAction SilentlyContinue).Sum
		}
		else
		{
			$lengthInBytes = (Get-ChildItem -Path $currentFolderItem | Measure-Object -Property Length -Sum -ErrorAction SilentlyContinue).Sum
		}

		if ($lengthInBytes.Length -eq 0)
		{
			$lengthInBytes = 0
		}
	}
	catch
	{
		Write-Host "Failed to scan folder item $currentFolderItem" -BackgroundColor Black -ForegroundColor Red
		exit
	}

	$oneFolderItem = New-Object PSObject
	$oneFolderItem | Add-Member -MemberType NoteProperty -Name "ItemPath" -Value $currentFolderItem
	$oneFolderItem | Add-Member -MemberType NoteProperty -Name "SizeInBytes" -Value ([System.UInt64]$lengthInBytes)

	if ($isDirectory)
	{
		$unsortedDirectories += $oneFolderItem
	}
	else
	{
		$unsortedFiles += $oneFolderItem
	}
}


if ($unsortedDirectories.Count -gt 1)
{
	$sortedDirectories = $unsortedDirectories | Sort-Object -Property "SizeInBytes" -Descending
}
else
{
	$sortedDirectories = $unsortedDirectories
}

if ($unsortedFiles.Count -gt 1)
{
	$sortedFiles = $unsortedFiles | Sort-Object -Property "SizeInBytes" -Descending
}
else
{
	$sortedFiles = $unsortedFiles
}


for ($resultsIndex=0; $resultsIndex -lt $sortedDirectories.Count; $resultsIndex++)
{
	if ($resultsIndex -lt $sortedDirectories.Count)
	{
		$lengthInKB = $lengthInMB = $lengthInGB = $lengthInTB = 0
		$lengthInBytes = $sortedDirectories[$resultsIndex].SizeInBytes
		if ($lengthInBytes -gt 1024)
		{
			$lengthInKB = [System.Math]::Round($lengthInBytes/1024,1)
		}
		if ($lengthInKB -gt 1024)
		{
			$lengthInMB = [System.Math]::Round($lengthInKB/1024,1)
		}
		if ($lengthInMB -gt 1024)
		{
			$lengthInGB = [System.Math]::Round($lengthInMB/1024,1)
		}
		if ($lengthInGB -gt 1024)
		{
			$lengthInTB = [System.Math]::Round($lengthInGB/1024,1)
		}
	}

	$sizeString = ""
	if ($lengthInTB -gt 0)
	{
		$sizeString = "$lengthInTB TB"
	}
	elseif ($lengthInGB -gt 0)
	{
		$sizeString = "$lengthInGB GB"
	}
	elseif ($lengthInMB -gt 0)
	{
		$sizeString = "$lengthInMB MB"
	}
	elseif ($lengthInKB -gt 0)
	{
		$sizeString = "$lengthInKB KB"
	}
	elseif ($lengthInBytes -gt 0)
	{
		$sizeString = "$lengthInBytes B"
	}

	Write-Host "$($sortedDirectories[$resultsIndex].ItemPath) $sizeString" -ForegroundColor Cyan -BackgroundColor Black
}

for ($resultsIndex=0; $resultsIndex -lt $sortedFiles.Count; $resultsIndex++)
{
	if ($resultsIndex -lt $sortedFiles.Count)
	{
		$lengthInKB = $lengthInMB = $lengthInGB = $lengthInTB = 0
		$lengthInBytes = $sortedFiles[$resultsIndex].SizeInBytes
		if ($lengthInBytes -gt 1024)
		{
			$lengthInKB = [System.Math]::Round($lengthInBytes/1024,1)
		}
		if ($lengthInKB -gt 1024)
		{
			$lengthInMB = [System.Math]::Round($lengthInKB/1024,1)
		}
		if ($lengthInMB -gt 1024)
		{
			$lengthInGB = [System.Math]::Round($lengthInMB/1024,1)
		}
		if ($lengthInGB -gt 1024)
		{
			$lengthInTB = [System.Math]::Round($lengthInGB/1024,1)
		}
	}

	$sizeString = ""
	if ($lengthInTB -gt 0)
	{
		$sizeString = "$lengthInTB TB"
	}
	elseif ($lengthInGB -gt 0)
	{
		$sizeString = "$lengthInGB GB"
	}
	elseif ($lengthInMB -gt 0)
	{
		$sizeString = "$lengthInMB MB"
	}
	elseif ($lengthInKB -gt 0)
	{
		$sizeString = "$lengthInKB KB"
	}
	elseif ($lengthInBytes -gt 0)
	{
		$sizeString = "$lengthInBytes B"
	}

	Write-Host "$($sortedFiles[$resultsIndex].ItemPath) $sizeString" -ForegroundColor DarkCyan -BackgroundColor Black
}

$endTime = Get-Date
$timeElapsed = "$(($endTime - $startTime).Minutes.ToString("0"))m$(($endTime - $startTime).Seconds.ToString("00"))s"
Write-Host ""
Write-Host "Scan completed in $timeElapsed"
