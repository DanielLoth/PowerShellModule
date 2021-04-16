param (
	[Parameter(Mandatory = $true)] [string] $ConfigurationName,
	[Parameter(Mandatory = $true)] [string] $ProjectName,
	[Parameter(Mandatory = $true)] [string] $ProjectFileName,
	[Parameter(Mandatory = $true)] [string] $ProjectPath,
    [Parameter(Mandatory = $true)] [string] $SolutionDir,
	[Parameter(Mandatory = $true)] [string] $SolutionFileName,
	[Parameter(Mandatory = $true)] [string] $SolutionPath,
    [Parameter(Mandatory = $true)] [string] $TargetDir,
	[Parameter(Mandatory = $true)] [string] $TargetFileName,
	[Parameter(Mandatory = $true)] [string] $TargetName,
	[Parameter(Mandatory = $true)] [string] $TargetPath
)

Write-Host ""
Write-Host "Running postbuild.ps1 ..."

Write-Host "ConfigurationName = $ConfigurationName"
Write-Host "ProjectName = $ProjectName"
Write-Host "ProjectFileName = $ProjectFileName"
Write-Host "ProjectPath = $ProjectPath"
Write-Host "SolutionDir = $SolutionDir"
Write-Host "SolutionFileName = $SolutionFileName"
Write-Host "SolutionPath = $SolutionPath"
Write-Host "TargetFileName = $TargetFileName"
Write-Host "TargetDir = $TargetDir"
Write-Host "TargetName = $TargetName"
Write-Host "TargetPath = $TargetPath"

if (-Not (Test-Path -Path $SolutionDir -PathType Container)) {
    exit 1
}

$SolutionDirParentDir = Split-Path -Parent $SolutionDir
if (-Not (Test-Path -Path $SolutionDirParentDir -PathType Container)) {
    exit 1
}

$DistDir = Join-Path -Path $SolutionDirParentDir -ChildPath "dist"
if (-Not (Test-Path -Path $DistDir -PathType Container)) {
    New-Item -Path $DistDir -ItemType Directory | Out-Null
}

$DistVersionGuid = [Guid]::NewGuid().ToString("N").ToLower()
$DistVersionDir = Join-Path -Path $DistDir -ChildPath (Join-Path -Path $DistVersionGuid -ChildPath "Modules/PowerShellModule")
if (-Not (Test-Path -Path $DistVersionDir -PathType Container)) {
    New-Item -Path $DistVersionDir -ItemType Directory | Out-Null
}

Get-ChildItem -Path $TargetDir | Copy-Item -Destination $DistVersionDir

[Environment]::SetEnvironmentVariable("POWERSHELLMODULE_HOME", $DistVersionDir, [EnvironmentVariableTarget]::User)

if (-Not [Environment]::GetEnvironmentVariable("PSModulePath", [EnvironmentVariableTarget]::User)) {
	[Environment]::SetEnvironmentVariable("PSModulePath", "%POWERSHELLMODULE_HOME%", [EnvironmentVariableTarget]::User)
}

$UserPsModulePath = [Environment]::GetEnvironmentVariable("PSModulePath", [EnvironmentVariableTarget]::User)
if ($UserPsModulePath -and $UserPsModulePath -notlike "*%POWERSHELLMODULE_HOME%*") {
	[Environment]::SetEnvironmentVariable("PSModulePath", "$UserPsModulePath;%POWERSHELLMODULE_HOME%;", [EnvironmentVariableTarget]::User)
}

Write-Host "Done"
Write-Host ""
