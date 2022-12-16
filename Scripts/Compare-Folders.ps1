<#
.SYNOPSIS
This script compares two folders. It compares the files contained 
in each folder and looks for differences.

.DESCRIPTION
0.1		12/16/2022	Initial version by Bob Hansen (not all features working yet)

To-Do:
-Do we care about comparing file size or file hash?
#>

param(
	[parameter(position=0,mandatory=$true)][string]$Folder1="",
	[parameter(position=1,mandatory=$true)][string]$Folder2="",
	[parameter(position=2,mandatory=$false)][ValidateSet("UniqueToFolder1","UniqueToFolder2","Intersection")][string]$CompareType="UniqueToFolder1",
	[parameter(position=3,mandatory=$false)][ValidateSet("NameOnly","NameAndFileSize","NameAndHash")][string]$CompareFields="NameOnly",
	[parameter(position=4,mandatory=$false)][ValidateSet("No","Yes")][string]$RecurseSubfolders="Yes"
)

if ($CompareType -eq "UniqueToFolder1")
{
	Write-Host "Finding objects that are unique to (Folder1) $Folder1"
}
elseif ($CompareType -eq "UniqueToFolder2")
{
	Write-Host "Finding objects that are unique to (Folder2) $Folder2"
}
elseif ($CompareType -eq "Intersection")
{
	Write-Host "Finding objects that are shared by both folders (intersect)"
}

if ($CompareFields -eq "NameOnly")
{
	Write-Host "Comparing on file names only"
}
elseif ($CompareFields -eq "NameAndFileSize")
{
	Write-Host "Comparing on file names and sizes"
}
elseif ($CompareFields -eq "NameAndHash")
{
	Write-Host "Comparing on file names and file hashes"
}
Write-Host ""


if ($RecurseSubfolders -eq "Yes")
{
	$filesFromFolder1 = Get-ChildItem -Path $Folder1 -Recurse | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,FullName
	$filesFromFolder2 = Get-ChildItem -Path $Folder2 -Recurse | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,FullName
}
else
{
	$filesFromFolder1 = Get-ChildItem -Path $Folder1 | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,FullName
	$filesFromFolder2 = Get-ChildItem -Path $Folder2 | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,FullName
}

Write-Host "Folder1 count $($filesFromFolder1.Count)"
Write-Host "Folder2 count $($filesFromFolder2.Count)"

if ($CompareType -eq "UniqueToFolder1")
{
	if ($CompareFields -eq "NameOnly")
	{
		$results = $filesFromFolder1.Name | ?{$filesFromFolder2.Name -notcontains $_}
	}
	elseif ($CompareFields -eq "NameAndFileSize")
	{
	}
	elseif ($CompareFields -eq "NameAndHash")
	{
	}
}
elseif ($CompareType -eq "UniqueToFolder2")
{
	if ($CompareFields -eq "NameOnly")
	{
		$results = $filesFromFolder2.Name | ?{$filesFromFolder1.Name -notcontains $_}
	}
	elseif ($CompareFields -eq "NameAndFileSize")
	{
	}
	elseif ($CompareFields -eq "NameAndHash")
	{
	}
}
elseif ($CompareType -eq "Intersection")
{
	if ($CompareFields -eq "NameOnly")
	{
		$results = $filesFromFolder1.Name | ?{$filesFromFolder2.Name -contains $_}
}
	elseif ($CompareFields -eq "NameAndFileSize")
	{
	}
	elseif ($CompareFields -eq "NameAndHash")
	{
	}
}

Write-Host "results count $($results.Count)"

