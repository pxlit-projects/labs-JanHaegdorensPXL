$CURRDIR=Get-Location

$WORKDIR="$SLNDIR\\Services\\HumanResources\\HumanResources.Api\\pub"
Set-Location $WORKDIR

az webapp deployment source config-zip `
    --src hrapp.zip `
    --resource-group $RESOURCE_GROUP `
    --name $HR_APPSERVICE_APPNAME

Set-Location $CURRDIR

$WORKDIR="$SLNDIR\\Services\\Identity\\Identity.UI\\pub"
Set-Location $WORKDIR

az webapp deployment source config-zip `
    --src identityapp.zip `
    --resource-group $RESOURCE_GROUP `
    --name $IDENTITY_APPSERVICE_APPNAME

Set-Location $CURRDIR

$WORKDIR="$SLNDIR\\Services\\DevOps\\DevOps.Api\\pub"
Set-Location $WORKDIR

az webapp deployment source config-zip `
    --src devopsapp.zip `
    --resource-group $RESOURCE_GROUP `
    --name $DEVOPS_APPSERVICE_APPNAME

Set-Location $CURRDIR