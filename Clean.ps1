$root = $PSScriptRoot

Write-Host "Deleting from root $root"

Get-ChildItem "$root\*\obj" | Remove-Item -Force -Recurse
Get-ChildItem "$root\*\bin" | Remove-Item -Force -Recurse
