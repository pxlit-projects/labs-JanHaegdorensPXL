@description('Location of resources')
param location string = 'westeurope'

@description('Name of the SQL Server, randomized based on resource group id')
param sqlServerName string = 'kwops-${uniqueString(resourceGroup().id)}'

module dbResources 'db.bicep' = {
  name: 'dbResources'
  params: {
    location: location
    sqlServerName: sqlServerName
  }
}


module hrAppService 'micro-api.bicep' = {
  name: 'hrAppService'
  params: {
    location: location
    microserviceName: 'hr'
    connectionString: dbResources.outputs.hrConnectionString
  }
}

output hrAppServiceAppName string = hrAppService.outputs.appServiceAppName