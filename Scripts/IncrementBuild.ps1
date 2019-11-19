# Major.Minor.Build.Revision
#
# ..\UiPathCloudAPI\Old\Properties\AssemblyInfo.cs
# "(AssemblyFileVersion\("")(?:\d+\.\d+\.\d+\.\d+)(""\))"
# 
# ..\UiPathCloudAPI\UiPathCloudAPI.csproj
#

param (
	[Int32]$part=2
)

function IncrementMajor($versionString, $partCount) {
	$version = [version] $versionString
	if ($partCount -eq 4) {
		"{0}.{1}.{2}.{3}" -f ($version.Major + 1), $version.Minor, $version.Build, $version.Revision
	}
	else {
		"{0}.{1}.{2}" -f ($version.Major + 1), $version.Minor, $version.Build, 0
	}
	return
}

function IncrementMinor($versionString, $partCount) {
	$version = [version] $versionString
	if ($partCount -eq 4) {
		"{0}.{1}.{2}.{3}" -f $version.Major, ($version.Minor + 1), $version.Build, $version.Revision
	}
	else {
		"{0}.{1}.{2}" -f $version.Major, ($version.Minor + 1), $version.Build
	}
	return
}

function IncrementBuild($versionString, $partCount) {
	$version = [version] $versionString
	if ($partCount -eq 4) {
		"{0}.{1}.{2}.{3}" -f $version.Major, $version.Minor, ($version.Build + 1), $version.Revision
	}
	else {
		"{0}.{1}.{2}" -f $version.Major, $version.Minor, ($version.Build + 1)
	}
	return
}

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Write-Host "Script path: " $scriptPath
$filePath = "$scriptPath\..\UiPathCloudAPI\UiPathCloudAPI.csproj"
Write-Host "Project file path: " $filePath

$content = [System.IO.File]::ReadAllText($filePath)

# Base version string for match
$versionStringBase = [RegEx]::Match($content,"(<Version>((?:\d+\.\d+\.\d+\.\d+)|(?:\d+\.\d+\.\d+))<\/Version>)")
Write-Host "Version base: " $versionStringBase
$partMatches = Select-String -InputObject $versionStringBase -Pattern "\d+" -AllMatches
$partCount = $partMatches.Matches.Count
Write-Host "Version parts: " $partCount

# Specific string, needs to be in the right format
$versionString = [RegEx]::Match($versionStringBase,"((?:\d+\.\d+\.\d+\.\d+)|(?:\d+\.\d+\.\d+))").Value
Write-Host "Original version string: " $versionString

if ($part -eq 0) {
	$newVersionString = IncrementMajor $versionString $partCount
}
elseif ($part -eq 1) {
	$newVersionString = IncrementMinor $versionString $partCount
}
else {
	$newVersionString = IncrementBuild $versionString $partCount
}
Write-Host "New version string: " $newVersionString

$content = $content -Replace $versionString,$newVersionString
Write-Host $fileName
[System.IO.File]::WriteAllText($filePath, $content)

$filePath = "$scriptPath\..\UiPathCloudAPI\Old\Properties\AssemblyInfo.cs"
$content = [System.IO.File]::ReadAllText($filePath)
if ($partCount -eq 3) {
	$content = $content -replace "\d+\.\d+\.\d+",$newVersionString
}
else {
	$content = $content -replace "\d+\.\d+\.\d+\.\d+",$newVersionString
}
[System.IO.File]::WriteAllText($filePath, $content)

$filePath = "$scriptPath\..\Specific Files\UiPathCloudAPI.net40.nuspec"
$content = [System.IO.File]::ReadAllText($filePath)
if ($partCount -eq 3) {
	$content = $content -replace "\d+\.\d+\.\d+",$newVersionString
}
else {
	$content = $content -replace "\d+\.\d+\.\d+\.\d+",$newVersionString
}
[System.IO.File]::WriteAllText($filePath, $content)

$filePath = "$scriptPath\..\UiPathCloudAPI\Old\UiPathCloudAPIOld.nuspec"
$content = [System.IO.File]::ReadAllText($filePath)
$versionStringBase = [RegEx]::Match($content,"(<version>((?:\d+\.\d+\.\d+\.\d+)|(?:\d+\.\d+\.\d+))<\/version>)")
if ($partCount -eq 3) {
	$newVersionStringBase = $versionStringBase -replace "\d+\.\d+\.\d+",$newVersionString
}
else {
	$newVersionStringBase = $versionStringBase -replace "\d+\.\d+\.\d+\.\d+",$newVersionString
}
$content = $content -replace $versionStringBase,$newVersionStringBase
[System.IO.File]::WriteAllText($filePath, $content)

$filePath = "$scriptPath\..\UiPathCloudAPI\Old\UiPathCloudAPI.net40.nuspec"
if (Test-Path -path $filePath) {
	$content = [System.IO.File]::ReadAllText($filePath)
	if ($partCount -eq 3) {
		$content = $content -replace "\d+\.\d+\.\d+",$newVersionString
	}
	else {
		$content = $content -replace "\d+\.\d+\.\d+\.\d+",$newVersionString
	}
	[System.IO.File]::WriteAllText($filePath, $content)
}
