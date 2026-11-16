dotnet tool install -g dotnet-reportgenerator-globaltool

$Project = Join-Path $PSScriptRoot "KooliProjekt.Application.UnitTests.csproj"
$UnitTestsDir = Join-Path $PSScriptRoot "BuildReports\UnitTests"
$CoverageDir = Join-Path $PSScriptRoot "BuildReports\Coverage"

if (Test-Path $UnitTestsDir) {
    Remove-Item $UnitTestsDir -Recurse -Force
}

if (Test-Path $CoverageDir) {
    Remove-Item $CoverageDir -Recurse -Force
}

dotnet test $Project `
    --collect "XPlat Code Coverage" `
    --results-directory $UnitTestsDir

$CoverageFile = Get-ChildItem `
    -Path $UnitTestsDir `
    -Filter "coverage.cobertura.xml" `
    -Recurse |
    Select-Object -First 1

if ($null -eq $CoverageFile) {
    Write-Host "ERROR: coverage.cobertura.xml faili ei leitud."
    exit 1
}

Write-Host ""
Write-Host "Coverage file:"
Write-Host $CoverageFile.FullName

reportgenerator `
    "-reports:$($CoverageFile.FullName)" `
    "-targetdir:$CoverageDir" `
    "-reporttype:Html" `
    "-filefilters:-**/Behaviors/**;-**/Data/**;-**/Dto/**;-**/Infrastructure/**;-**/Migrations/**" `
    "-classfilters:-AspNetCoreGeneratedDocument.*;-*Command;-*Query"

$Report = Join-Path $CoverageDir "index.htm"

if (Test-Path $Report) {
    Start-Process $Report
}
else {
    Write-Host "ERROR: Coverage HTML reporti ei leitud."
}