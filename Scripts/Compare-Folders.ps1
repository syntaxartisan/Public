<#
.SYNOPSIS
This script compares two folders. It compares the files contained 
in each folder and looks for differences.

.DESCRIPTION
1.0		12/16/2022	Copy files if OutputPath is provided; Rename DisplayDebug to DebugMode; First working version
0.8		12/16/2022	Add OutputPath parameter
0.7		12/16/2022	Improve formatting of console output
0.6		12/16/2022	Add file hash comparison option
0.5		12/16/2022	Add DisplayDebug switch
0.4		12/16/2022	Comparison results now include file detailsComparison results now include file details
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

.PARAMETER OutputPath
Populate OutputPath with a directory if you want to copy the results 
to another folder. OutputPath must be an empty directory.

.PARAMETER DebugMode
Display debugging data to the console window in different formats of verbosity.

To-Do:
-Copy results files to another folder
#>

param(
	[parameter(position=0,mandatory=$true)][string]$Folder1="",
	[parameter(position=1,mandatory=$true)][string]$Folder2="",
	[parameter(position=2,mandatory=$false)][ValidateSet("NameOnly","NameAndFileSize","NameAndHash")][string]$CompareFields="NameAndFileSize",
	[parameter(position=3,mandatory=$false)][ValidateSet("UniqueToFolder1","UniqueToFolder2","Intersection")][string]$CompareType="UniqueToFolder1",
	[parameter(position=4,mandatory=$false)][ValidateSet("No","Yes")][string]$RecurseSubfolders="Yes",
	[parameter(position=5,mandatory=$false)][string]$OutputPath="",
	[parameter(position=6,mandatory=$false)][ValidateSet("Off","Basic","Detailed")]$DebugMode="Off"
)

if ($OutputPath.Length -gt 0)
{
	if (-not(Test-Path -Path $OutputPath))
	{
		Write-Host "The output path $OutputPath doesn't exist. Please create the path manually. This is intentional out of abundance of caution." -ForegroundColor Red -BackgroundColor Black
		exit
	}

	$itemCount = @(Get-ChildItem -Path $OutputPath).Count # count of both files and folders
	if ($itemCount -gt 0)
	{
		Write-Host "The output path $OutputPath contains files and/or folders. Please select an empty directory for copying files. This is intentional out of abundance of caution." -ForegroundColor Red -BackgroundColor Black
		exit
	}
}

if ($RecurseSubfolders -eq "Yes")
{
	$recurseString = " (recursively)"
}
else
{
	$recurseString = ""
}

if ($CompareType -eq "UniqueToFolder1")
{
	Write-Host "Finding objects$recurseString that are unique to (Folder1) $Folder1" -ForegroundColor Yellow
}
elseif ($CompareType -eq "UniqueToFolder2")
{
	Write-Host "Finding objects$recurseString that are unique to (Folder2) $Folder2" -ForegroundColor Yellow
}
elseif ($CompareType -eq "Intersection")
{
	Write-Host "Finding objects$recurseString that are shared by both folders (intersect)" -ForegroundColor Yellow
}

if ($CompareFields -eq "NameOnly")
{
	Write-Host "Comparing on file names only" -ForegroundColor Yellow
}
elseif ($CompareFields -eq "NameAndFileSize")
{
	Write-Host "Comparing on file names and sizes" -ForegroundColor Yellow
}
elseif ($CompareFields -eq "NameAndHash")
{
	Write-Host "Comparing on file names and file hashes" -ForegroundColor Yellow
}


if ($RecurseSubfolders -eq "Yes")
{
	$filesFromFolder1 = Get-ChildItem -Path $Folder1 -Recurse | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,Directory,FullName
	$filesFromFolder2 = Get-ChildItem -Path $Folder2 -Recurse | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,Directory,FullName
}
else
{
	$filesFromFolder1 = Get-ChildItem -Path $Folder1 | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,Directory,FullName
	$filesFromFolder2 = Get-ChildItem -Path $Folder2 | Where-Object {$_.PSIsContainer -eq $false} | Select-Object Name,Length,Directory,FullName
}

if ($DebugMode -eq "Detailed")
{
	Write-Host ""
	Write-Host "Folder1 items: (count $($filesFromFolder1.Count))"
	$filesFromFolder1
	Write-Host ""
	Write-Host "Folder2 items: (count $($filesFromFolder2.Count))"
	$filesFromFolder2
}
elseif ($DebugMode -eq "Basic")
{
	Write-Host ""
	Write-Host "Folder1 items: (count $($filesFromFolder1.Count))"
	Write-Host "Folder2 items: (count $($filesFromFolder2.Count))"
}


if ($CompareFields -eq "NameOnly")
{
	$compareResults = Compare-Object -ReferenceObject $filesFromFolder1 -DifferenceObject $filesFromFolder2 -Property Name -IncludeEqual -PassThru
}
elseif ($CompareFields -eq "NameAndFileSize")
{
	$compareResults = Compare-Object -ReferenceObject $filesFromFolder1 -DifferenceObject $filesFromFolder2 -Property Name,Length -IncludeEqual -PassThru
}
elseif ($CompareFields -eq "NameAndHash")
{
	$filesFromFolder1 | Add-Member -NotePropertyName "FileHash" -NotePropertyValue ""
	$filesFromFolder1 | ForEach-Object {$_.FileHash = (Get-FileHash -Path $_.FullName -Algorithm MD5).Hash}
	$filesFromFolder2 | Add-Member -NotePropertyName "FileHash" -NotePropertyValue ""
	$filesFromFolder2 | ForEach-Object {$_.FileHash = (Get-FileHash -Path $_.FullName -Algorithm MD5).Hash}

	$compareResults = Compare-Object -ReferenceObject $filesFromFolder1 -DifferenceObject $filesFromFolder2 -Property Name,FileHash -IncludeEqual -PassThru
}

if ($CompareType -eq "UniqueToFolder1")
{
	$finalResults = @($compareResults | Where-Object {$_.SideIndicator -eq "<="})
	$sourceFolder = $Folder1
}
elseif ($CompareType -eq "UniqueToFolder2")
{
	$finalResults = @($compareResults | Where-Object {$_.SideIndicator -eq "=>"})
	$sourceFolder = $Folder2
}
elseif ($CompareType -eq "Intersection")
{
	$finalResults = @($compareResults | Where-Object {$_.SideIndicator -eq "=="})
	$sourceFolder = $Folder1
}

if ($DebugMode -eq "Detailed")
{
	Write-Host ""
	Write-Host "Final results: (count $($finalResults.Count))"
#	$finalResults
	$finalResults.Name
}
elseif ($DebugMode -eq "Basic")
{
	Write-Host ""
	Write-Host "Final results: (count $($finalResults.Count))"
}


if ($OutputPath.Length -gt 0)
{
	Write-Host ""
	Write-Host "Copying final results from $sourceFolder to $OutputPath" -ForegroundColor Yellow
	Write-Host "Progress..."
	for ($index = 0; $index -lt $finalResults.Length; $index++)
	{
		if (($index + 1) %100 -eq 0)
		{
			Write-Host "Copying file $($index + 1)"
		}
		Copy-Item -Path $finalResults[$index].FullName -Destination $OutputPath
	}

	Write-Host ""
	Write-Host "File copy has completed"
}
