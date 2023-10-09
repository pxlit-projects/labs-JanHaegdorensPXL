@description('location of the service, e.g. westeurope')
param location string

@description('name of the microservice, e.g. hr')
param microserviceName string

@description('the connection string to the database')
param connectionString string

@description('runtime stack of the web app')
param windowsFxVersion string = 'DOTNETCORE|6.0'

var solutionName = '${microserviceName}-${uniqueString(resourceGroup().id)}'
var appServicePlanName = '${solutionName}-plan'
var appServiceAppName = '${solutionName}-app'

resource appServicePlan 'Microsoft.Web/serverfarms@2022-09-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'F1'
    tier: 'Free'
  }
}

resource appServiceApp 'Microsoft.Web/sites@2022-09-01' = {
  name: appServiceAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      windowsFxVersion: windowsFxVersion
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        {
          name: 'ASPNETCORE_URLS'
          value: 'https://+:443;http://+:80'
        }
        {
          name: 'ConnectionString'
          value: connectionString
        }
      ]
    }
  }
}

output appServiceAppName string = appServiceAppName