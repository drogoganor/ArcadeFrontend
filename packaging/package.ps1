$installerExeName = "Arcade Frontend Setup.exe"

if (Test-Path $installerExeName) {
  Remove-Item $installerExeName
}

Remove-Item -LiteralPath "app" -Force -Recurse -ErrorAction SilentlyContinue
New-Item -Path "." -Name "app" -ItemType "directory" | Out-Null
Copy-Item -Path "..\ArcadeFrontend\bin\Release\net10.0\*" -Destination "app" -Force -Recurse
Copy-Item -Path "..\deps\*.dll" -Destination "app" -Force -Recurse

Start-Process -FilePath "C:\Program Files (x86)\NSIS\makensis.exe" -ArgumentList "ArcadeFrontend.nsi" -Wait
Start-Sleep -Seconds 2

if (Test-Path $installerExeName) {
  Move-Item -Path $installerExeName -Destination "..\$installerExeName" -Force
}

Remove-Item -LiteralPath "app" -Force -Recurse -ErrorAction SilentlyContinue