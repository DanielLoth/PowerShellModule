New-Item -Name "Dan" -Type Directory -Path (Join-Path -Path ([Environment]::GetFolderPath("MyDocuments")) -ChildPath "PowerShell/Modules") -Force | Out-Null
Copy-Item -Path ./* -Destination (Join-Path -Path ([Environment]::GetFolderPath("MyDocuments")) -ChildPath "PowerShell/Modules") -Force

Import-Module -Name Dan -Force