$env:ASPNETCORE_ENVIRONMENT = "Test"
Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "src/SifenApi.WebApi", "--urls", "http://localhost:5001" -WindowStyle Hidden
Write-Host "API iniciándose en segundo plano..."
Start-Sleep -Seconds 5
Write-Host "API debería estar lista en http://localhost:5001"