# Run the AOAI API Simulator
Set-Location -Path "$PSScriptRoot\aoai-api-simulator"
$env:PYTHONPATH = "src"

Write-Host "Starting AOAI API Simulator on http://0.0.0.0:8000" -ForegroundColor Green
Write-Host "Press Ctrl+C to stop" -ForegroundColor Yellow

python -m uvicorn aoai_api_simulator.main:app --host 0.0.0.0 --port 8000 --reload
