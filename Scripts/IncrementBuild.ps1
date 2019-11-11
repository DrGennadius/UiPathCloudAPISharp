# Major.Minor.Build.Revision

function IncrementMajor($versionString) {
	$version = [version] $versionString
	$newMajor = $version.Major + 1
	$versionString -Replace "(?<=^)\d*(?=\.)","$newMajor"
	return
}

function IncrementMinor($versionString) {
	$version = [version] $versionString
	$newMinor = $version.Minor + 1
	$versionString -Replace "(?<=\.)\d*(?=\.\d*\.)","$newMinor"
	return
}

function IncrementBuild($versionString) {
	$version = [version] $versionString
	$newBuild = $version.Build + 1
	$versionString -Replace "(?<=\d\.)\d*(?=\.\d*$)","$newBuild"
	return
}

$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
Write-Host ("Script Path: " + $scriptPath)
$assemblyInfoPath = "$scriptPath\..\UiPathCloudAPI\Old\Properties\AssemblyInfo.cs"
Write-Host ("Assembly Info Path: " + $assemblyInfoPath)

$contents = [System.IO.File]::ReadAllText($assemblyInfoPath)

# Base version string for match
$versionStringBase = [RegEx]::Match($contents,"(AssemblyFileVersion\("")(?:\d+\.\d+\.\d+\.\d+)(""\))")
Write-Host ("AssemblyFileVersion: " + $versionStringBase)

# Specific string, needs to be in the right format
$versionString = [RegEx]::Match($versionStringBase,"((?:\d+\.\d+\.\d+\.\d+))").Value
Write-Host ("Clear version string: " + $versionString)

$versionString = IncrementMajor $versionString
Write-Host ("New version string 1: " + $versionString)

$versionString = IncrementMinor $versionString
Write-Host ("New version string 2: " + $versionString)

$versionString = IncrementBuild $versionString
Write-Host ("New version string 3: " + $versionString)
