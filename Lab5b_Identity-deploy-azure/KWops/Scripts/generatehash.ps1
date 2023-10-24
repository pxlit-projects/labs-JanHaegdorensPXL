$plaintext = "SuperSecretClientSecret"
$bytes = [System.Text.Encoding]::UTF8.GetBytes($plaintext)
$sha256 = [System.Security.Cryptography.SHA256Managed]::Create()
$hashBytes = $sha256.ComputeHash($bytes)
$base64Hash = [System.Convert]::ToBase64String($hashBytes)

Write-Host "SHA256 (Base64):" $base64Hash