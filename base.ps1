param(
    [string]$InputFilePath,
    [string]$OutputFileName
)

try {
    $base64String = [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($InputFilePath))
    [System.IO.File]::WriteAllText($OutputFileName, $base64String)
    Write-Host "File encoded successfully and saved as $OutputFileName"
} catch {
    Write-Host "An error occurred: $_"
}