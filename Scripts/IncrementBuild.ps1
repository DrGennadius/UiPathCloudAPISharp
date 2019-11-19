# Major.Minor.Build.Revision
#
# ..\UiPathCloudAPI\Old\Properties\AssemblyInfo.cs
# "(AssemblyFileVersion\("")(?:\d+\.\d+\.\d+\.\d+)(""\))"
# 
# ..\UiPathCloudAPI\UiPathCloudAPI.csproj
#

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
Write-Host "Script Path: " $scriptPath
$filePath = new-object System.String("$scriptPath\..\UiPathCloudAPI\UiPathCloudAPI.csproj")
Write-Host "Assembly Info Path: " $filePath

$content = [System.IO.File]::ReadAllText($filePath)

# Base version string for match
$versionStringBase = [RegEx]::Match($content,"(<Version>((?:\d+\.\d+\.\d+\.\d+)|(?:\d+\.\d+\.\d+))<\/Version>)")
Write-Host "AssemblyFileVersion: " $versionStringBase
$partMatches = Select-String -InputObject $versionStringBase -Pattern "\d+" -AllMatches
$partCount = $partMatches.Matches.Count
Write-Host "VersionParts: " $partCount

# Specific string, needs to be in the right format
$versionString = [RegEx]::Match($versionStringBase,"((?:\d+\.\d+\.\d+\.\d+)|(?:\d+\.\d+\.\d+))").Value
Write-Host "Clear version string: " $versionString

$newVersionString = IncrementMajor $versionString $partCount
Write-Host "New version string 1: " $newVersionString

$newVersionString = IncrementMinor $newVersionString $partCount
Write-Host "New version string 2: " $newVersionString

$newVersionString = IncrementBuild $newVersionString $partCount
Write-Host "New version string 3: " $newVersionString

$content = $content -Replace $versionString,$newVersionString
Write-Host $fileName
[System.IO.File]::WriteAllText($filePath, $content)
