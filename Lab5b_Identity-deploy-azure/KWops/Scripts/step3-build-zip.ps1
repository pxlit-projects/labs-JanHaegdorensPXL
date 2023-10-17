$CURRDIR=Get-Location

$WORKDIR="$SLNDIR\\Services\\HumanResources\\HumanResources.Api"
Set-Location $WORKDIR

dotnet publish -c Release -o pub

Set-Location pub
Compress-Archive -Path * -DestinationPath hrapp.zip -Force

Set-Location $CURRDIR

$WORKDIR="$SLNDIR\\Services\\DevOps\\DevOps.Api"
Set-Location $WORKDIR

dotnet publish -c Release -o pub

Set-Location pub
Compress-Archive -Path * -DestinationPath devopsapp.zip -Force

Set-Location $CURRDIR

$WORKDIR="$SLNDIR\\Services\\Identity\\Identity.UI"
Set-Location $WORKDIR

# build a release version of the Identity.UI project
dotnet publish -c Release -o pub

# read the contents of the appsettings.json file located in the WORKDIR
# replace the localhost urls with the urls of the deployed apps
$appsettings = Get-Content "$WORKDIR\\appsettings.json" -Raw
$publishedAppsettings = $appsettings -replace "https://localhost:5001", "$HR_APPSERVICE_URL" `
                                     -replace "https://localhost:8001", "$DEVOPS_APPSERVICE_URL"
$publishedAppsettings | Set-Content "$WORKDIR\\pub\\appsettings.json"

Set-Location pub
Compress-Archive -Path * -DestinationPath identityapp.zip -Force

Set-Location $CURRDIR