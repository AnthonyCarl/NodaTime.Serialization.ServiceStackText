if((Get-Module | Where-Object {$_.Name -eq "psake"}) -eq $null) 
    { 
        Write-Host "psake module not found, importing it" 
        $scriptPath = Split-Path $MyInvocation.InvocationName 
        Import-Module .\tools\psake.4.3.1.0\tools\psake.psm1
    } 
Import-Module .\tools\SetVersion.psm1

function Get-VersionNumber
{
  $completeVersionNumber = ""

  $buildNumber = $Env:BUILD_NUMBER
  
  if([string]::IsNullOrEmpty($buildNumber))
  {
    $completeVersionNumber = $majorMinorVersion + ".*"
  }
  else
  {
    #running in TeamCity
    $completeVersionNumber = $majorMinorVersion + "." + $buildNumber
  }

  return ,$completeVersionNumber
}

properties {
    $configuration = "Release"
    $rootLocation = get-location
    $srcRoot = "$rootLocation\src"
    $projectBaseName = "NodaTime.Serialization.ServiceStackText"
    $csprojFile = "$srcRoot\$projectBaseName\$projectBaseName.csproj"
    $unitTestNamePart = "UnitTests"
    $testOutputPath = "$srcRoot\$projectBaseName.$unitTestNamePart\bin\$configuration"
    $testDllFileName = "$projectBaseName.$unitTestNamePart.dll"
    $testDllFullPath = "$testOutputPath\$testDllFileName"
    $ssTextv4Dll = "$rootLocation\tools\ServiceStack.Text.4\lib\net40\ServiceStack.Text.dll"
    $ssTextV4TestOutputPath = "$rootLocation\ssTextv4Test"
    $ssTextV4TestDllFullPath = "$ssTextV4TestOutputPath\$testDllFileName"
    $slnFile = "$srcRoot\$projectBaseName.sln"
    $nuspecFile ="$srcRoot\$projectBaseName\$projectBaseName.nuspec"
    $framework = "3.5-Client"
    $xunitRunner = ".\tools\xunit.runners.1.9.2\tools\xunit.console.clr4.exe"
    $nugetOutputDir = ".\ReleasePackages"
    $nugetExe = "$rootLocation\tools\nuget\nuget.exe"
    $versionFile = ".\MajorMinorVersion.txt"
    $majorMinorVersion = Get-Content $versionFile
    $completeVersionNumber = Get-VersionNumber
    $gitHubRepoUrl = "https://github.com/AnthonyCarl/NodaTime.Serialization.ServiceStackText"
    $versionSwitch = ""
    
    if(!$completeVersionNumber.EndsWith(".*"))
    {
      #running in TeamCity
      $versionSwitch = "-Version $completeVersionNumber"
      Write-Host "##teamcity[buildNumber '$completeVersionNumber']"
    }
}

task Default -depends Pack

task Clean -depends SetVersion {
  exec { msbuild "$slnFile" /t:Clean /p:Configuration=$configuration }
}

task Compile -depends Clean {
  exec { msbuild "$slnFile" /p:Configuration=$configuration }
}

task TestSsTextV4 -depends Compile {
 mkdir -p "$ssTextV4TestOutputPath" -force
 cp "$testOutputPath\*.*" "$ssTextV4TestOutputPath"
 cp "$ssTextv4Dll" "$ssTextV4TestOutputPath" -force
 exec { .$xunitRunner "$ssTextV4TestDllFullPath" }
}

task Test -depends Compile {
  exec { .$xunitRunner "$testDllFullPath" }
}

task SetReleaseNotes -depends TestSsTextV4,Test {
  $releaseNotesText = $Env:ReleaseNotes
  $vcsNumber = $Env:BUILD_VCS_NUMBER

  if(![string]::IsNullOrEmpty($vcsNumber))
  {
    Write-Host "Found VCS number: $vcsNumber"
    $releaseNotesText += [System.Environment]::NewLine + [System.Environment]::NewLine + "Includes changes up to and including: $gitHubRepoUrl/commit/$vcsNumber" + [System.Environment]::NewLine
  }
  else
  {
    Write-Host "No VCS number found."
  }

  if(![string]::IsNullOrEmpty($releaseNotesText))
  {
    Write-Host "Setting release notes to:"
    Write-Host "$releaseNotesText"

    $nuspecContents = [Xml](Get-Content "$nuspecFile")
    $releaseNotesNode = $nuspecContents.package.metadata.SelectSingleNode("releaseNotes")
    if($releaseNotesNode -eq $null)
    {
      $releaseNotesNode = $nuspecContents.CreateElement('releaseNotes')
      $ignore = $nuspecContents.package.metadata.AppendChild($releaseNotesNode)
    }

    $ignore = $releaseNotesNode.InnerText = $releaseNotesText
    $nuspecContents.Save("$nuspecFile")
  }
  else
  {
    Write-Host "No release notes added."
  }
}

task Pack -depends SetReleaseNotes {
  mkdir -p "$nugetOutputDir" -force

  $completeVersionNumber = Get-VersionNumber
  exec { invoke-expression "& '$nugetExe' pack '$csprojFile' -Symbols -Properties Configuration=$configuration -OutputDirectory '$nugetOutputDir' $versionSwitch" }
}

task SetVersion {
  Write-Host "Setting version to $completeVersionNumber"
  Set-Version $completeVersionNumber
}

