param (
    [string]$MigrationName
)

if (-not $MigrationName) {
    Write-Host "Please provide a migration name."
    exit 1
}

$scriptLocation = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $scriptLocation "..\EntwineBackend"
Set-Location $projectPath

dotnet ef migrations add $MigrationName
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to add migration."
    exit $LASTEXITCODE
}

dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to update database."
    exit $LASTEXITCODE
}

$postgresLocation = $env:POSTGRES_LOCATION
if (-not $postgresLocation) {
    Write-Host "POSTGRES_LOCATION environment variable is not set."
    exit 1
}

$pgDumpPath = Join-Path $postgresLocation "pg_dump.exe"
$schemaDumpPath = Join-Path $postgresLocation "schema_dump.sql"
& $pgDumpPath --schema-only -U postgres -d postgres -s -f $schemaDumpPath
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to run pg_dump."
    exit $LASTEXITCODE
}

$scriptLocation = Split-Path -Parent $MyInvocation.MyCommand.Path
Copy-Item -Path $schemaDumpPath -Destination $scriptLocation
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to copy schema_dump.sql."
    exit $LASTEXITCODE
}

Write-Host "Success"