$CURRDIR=Get-Location

$WORKDIR="$SLNDIR\\Services\\HumanResources\\HumanResources.Api\\pub"
Set-Location $WORKDIR

az webapp deployment source config-zip `
    --src hrapp.zip `
    --resource-group $RESOURCE_GROUP `
    --name $HR_APPSERVICE_APPNAME

Set-Location $CURRDIR