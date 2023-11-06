##To be able to redeploy the solution (= execute step 3 and 4), you need to set the following variables in your powershell session

# use the same resource group name you used for the initial deployment
$global:RESOURCE_GROUP="rg-fullstack"

# contains the sln-file, change to your own path
$global:SLNDIR="C:\\devtest\\KWops\\src"

# names and urls of the appservices (needed for step 3 and 4). Can be found in the Azure portal.
$global:HR_APPSERVICE_APPNAME = "hr-yxe7amqvozxwe-app"
$global:DEVOPS_APPSERVICE_APPNAME = "devops-yxe7amqvozxwe-app"
$global:IDENTITY_APPSERVICE_APPNAME = "identity-yxe7amqvozxwe-app"
$global:HR_APPSERVICE_URL = "https://hr-yxe7amqvozxwe-app.azurewebsites.net"
$global:DEVOPS_APPSERVICE_URL = "https://devops-yxe7amqvozxwe-app.azurewebsites.net"