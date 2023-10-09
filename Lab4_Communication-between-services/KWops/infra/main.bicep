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

module devopsAppService 'micro-api.bicep' = {
  name: 'devopsAppService'
  params: {
    location: location
    microserviceName: 'devops'
    connectionString: dbResources.outputs.devOpsConnectionString
  }
}

output hrAppServiceAppName string = hrAppService.outputs.appServiceAppName

output devopsAppServiceAppName string = devopsAppService.outputs.appServiceAppName
