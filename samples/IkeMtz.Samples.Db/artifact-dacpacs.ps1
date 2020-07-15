param (
  [string]$sourceFolder = $PSScriptRoot,
  [string]$artifactFolder =  "c:\temp"
 )
 
Get-ChildItem  -Filter *.dacpac -Recurse | 
Foreach-Object { 
Write-Host "Dacpac File: $($_.Name)";
Copy-Item "$($_.FullName)" "$($artifactFolder)\" -Verbose -Force;
}
