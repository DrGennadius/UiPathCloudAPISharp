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
		"{0}.{1}.{2}.{3}" -f ($version.Major + 1), $version.Minor, $version.Build, 0
	}
	return
}

function IncrementMinor($versionString, $partCount) {
	$version = [version] $versionString
	if ($partCount -eq 4) {
		"{0}.{1}.{2}.{3}" -f $version.Major, ($version.Minor + 1), $version.Build, $version.Revision
	}
	else {
		"{0}.{1}.{2}.{3}" -f $version.Major, ($version.Minor + 1), $version.Build, 0
	}
	return
}

function IncrementBuild($versionString, $partCount) {
	$version = [version] $versionString
	if ($partCount -eq 4) {
		"{0}.{1}.{2}.{3}" -f $version.Major, $version.Minor, ($version.Build + 1), $version.Revision
	}
	else {
		"{0}.{1}.{2}.{3}" -f $version.Major, $version.Minor, ($version.Build + 1), 0
	}
	return
}

function PrintVersion($prefixText, $versionString) {
	$partMatches = Select-String -InputObject $versionString -Pattern "\d+" -AllMatches
	$partCount = $partMatches.Matches.Count
	if ($partCount -eq 4) {
		Write-Host ($prefixText + $versionString)
	}
	else {
		$versionStringView = [RegEx]::Match($versionString,"((?:\d+\.\d+\.\d+))").Value
		Write-Host ($prefixText + $versionString)
	}
}

function RewriteFile([string]$fileName, $content, $originVersionString, $newVersionString) {
	$partMatches = Select-String -InputObject $originVersionString -Pattern "\d+" -AllMatches
	$partCount = $partMatches.Matches.Count
	if ($partCount -eq 3) {
		$newVersionString = [RegEx]::Match($newVersionString,"((?:\d+\.\d+\.\d+))").Value
	}
	$contents = $contents -Replace $originVersionString,$newVersionString
	Write-Host "$fileName"
	#[System.IO.File]::WriteAllText($fileName, $content)
}

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Write-Host ("Script Path: " + $scriptPath)
$assemblyInfoPath = "$scriptPath\..\UiPathCloudAPI\UiPathCloudAPI.csproj"
Write-Host ("Assembly Info Path: " + $assemblyInfoPath)

$contents = [System.IO.File]::ReadAllText($assemblyInfoPath)

# Base version string for match
$versionStringBase = [RegEx]::Match($contents,"(<Version>((?:\d+\.\d+\.\d+\.\d+)|(?:\d+\.\d+\.\d+))<\/Version>)")
Write-Host ("AssemblyFileVersion: " + $versionStringBase)
$partMatches = Select-String -InputObject $versionStringBase -Pattern "\d+" -AllMatches
$partCount = $partMatches.Matches.Count
Write-Host ("VersionParts: " + $partCount)

# Specific string, needs to be in the right format
$versionString = [RegEx]::Match($versionStringBase,"((?:\d+\.\d+\.\d+\.\d+)|(?:\d+\.\d+\.\d+))").Value
PrintVersion ("Clear version string: ", $versionString)

$newVersionString = IncrementMajor $versionString $partCount
PrintVersion ("New version string 1: ", $newVersionString)

$newVersionString = IncrementMinor $newVersionString $partCount
PrintVersion ("New version string 2: ", $newVersionString)

$newVersionString = IncrementBuild $newVersionString $partCount
PrintVersion ("New version string 3: ", $newVersionString)

Write-Host "$assemblyInfoPath"
RewriteFile "$assemblyInfoPath", $contents, $versionString, $newVersionString
Write-Host "$assemblyInfoPath"
