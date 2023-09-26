$CURRDIR=Get-Location

$WORKDIR="$SLNDIR\\Services\\HumanResources\\HumanResources.Api"
Set-Location $WORKDIR

dotnet publish -c Release -o pub

Set-Location pub
Compress-Archive -Path * -DestinationPath hrapp.zip -Force

Set-Location $CURRDIR