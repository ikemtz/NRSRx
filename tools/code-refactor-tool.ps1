param (
  [string]$findValue, # = "Lookup value",
  [string]$replacementValue  # = "Replacement value"
 )

Set-Location $PSScriptRoot
Get-ChildItem -Path $PSScriptRoot -Filter *$findValue* -Recurse | 
Foreach-Object {

    $replacementValueFileName =  $_.Name.Replace($findValue, $replacementValue);
    Write-Host "findValue: $_"
    Write-Host "replacementValue: $replacementValueFileName"
    
    Rename-Item $_.FullName $_.FullName.Replace($findValue, $replacementValue);
}

Get-ChildItem -Path $PSScriptRoot -Filter *.cs -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}

Get-ChildItem -Path $PSScriptRoot -Filter *.ps1 -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}

Get-ChildItem -Path $PSScriptRoot -Filter *.sql  -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}

Get-ChildItem -Path $PSScriptRoot -Filter *.csproj -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}

Get-ChildItem -Path $PSScriptRoot -Filter *Dockerfile -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}

Get-ChildItem -Path $PSScriptRoot -Filter *.yml -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}

Get-ChildItem -Path $PSScriptRoot -Filter *.sln -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}

Get-ChildItem -Path $PSScriptRoot -Filter *.ts -Recurse | 
Foreach-Object {
  $fileContents = [System.IO.File]::ReadAllText($_.FullName)
    if ( $fileContents.Contains($findValue) )
    {
     Write-Host "Updating File Content On:" $_.Name
     $fileContents = $fileContents.Replace($findValue, $replacementValue);
     [System.IO.File]::WriteAllText($_.FullName, $fileContents)
    }
}