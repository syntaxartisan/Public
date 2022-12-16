<#
.SYNOPSIS
This script compares two folders. It compares the files contained 
in each folder and looks for differences.

.DESCRIPTION
0.3		12/16/2022	Comparisons are working (and quickly) but the results don't allow for file copies
0.2		12/16/2022	Add parameter descriptions
0.1		12/16/2022	Initial version by Bob Hansen (not all features working yet)

.PARAMETER Folder1
The first folder who's contents we want to compare

.PARAMETER Folder2
The second folder who's contents we want to compare

.PARAMETER CompareType
How to compare the two folders. Either find files that are 
unique to one of the two folders, or find files that are 
shared by both folders.

.PARAMETER CompareFields
What fields to compare. The selected fields much match exactly 
in order to determine that the files from both folders are the same.
NameOnly: File names
NameAndFileSize: File names and file sizes
NameAndHash: File names and file hashes

.PARAMETER RecurseSubfolders
When building a list of files from each folder to compare, 
select whether you want to recursively check subfolders (Yes) or not (No).

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

if ($CompareFields -eq "NameOnly")
{
	$folder1Condensed = $filesFromFolder1 | ForEach-Object {$_.Name}
	$folder2Condensed = $filesFromFolder2 | ForEach-Object {$_.Name}
}
elseif ($CompareFields -eq "NameAndFileSize")
{
	$folder1Condensed = $filesFromFolder1 | ForEach-Object {$_.Name + " " + $_.Length}
	$folder2Condensed = $filesFromFolder2 | ForEach-Object {$_.Name + " " + $_.Length}
}
elseif ($CompareFields -eq "NameAndHash")
{
}


if ($CompareType -eq "UniqueToFolder1")
{
	if ($CompareFields -eq "NameOnly")
	{
	}
	elseif ($CompareFields -eq "NameAndFileSize")
	{
	}
	elseif ($CompareFields -eq "NameAndHash")
	{
	}

	$results = $folder1Condensed | ?{$folder2Condensed -notcontains $_}
}
elseif ($CompareType -eq "UniqueToFolder2")
{
	if ($CompareFields -eq "NameOnly")
	{
	}
	elseif ($CompareFields -eq "NameAndFileSize")
	{
	}
	elseif ($CompareFields -eq "NameAndHash")
	{
	}

	$results = $folder2Condensed | ?{$folder1Condensed -notcontains $_}
}
elseif ($CompareType -eq "Intersection")
{
	if ($CompareFields -eq "NameOnly")
	{
	}
	elseif ($CompareFields -eq "NameAndFileSize")
	{
	}
	elseif ($CompareFields -eq "NameAndHash")
	{
	}

	$results = $folder1Condensed | ?{$folder2Condensed -contains $_}
}

Write-Host "results count $($results.Count)"
