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

module serviceBus 'servicebus.bicep' = {
  name: 'serviceBus'
  params: {
    location: location
    serviceBusNamespaceName: 'kwops-eventbus'
  }
}

module identityService 'identity-api.bicep' = {
  name: 'identityService'
  params: {
    location: location
    microserviceName: 'identity'
  }
}

module hrAppService 'micro-api.bicep' = {
  name: 'hrAppService'
  params: {
    location: location
    microserviceName: 'hr'
    connectionString: dbResources.outputs.hrConnectionString
    eventBusConnectionString: serviceBus.outputs.sharedAccessConnectionString
    identityUrl: identityService.outputs.appServiceUrl
  }
}

module devOpsAppService 'micro-api.bicep' = {
  name: 'devOpsAppService'
  params: {
    location: location
    microserviceName: 'devops'
    connectionString: dbResources.outputs.devOpsConnectionString
    eventBusConnectionString: serviceBus.outputs.sharedAccessConnectionString
    identityUrl: identityService.outputs.appServiceUrl
  }
}


output hrAppServiceAppName string = hrAppService.outputs.appServiceAppName
output devOpsAppServiceAppName string = devOpsAppService.outputs.appServiceAppName
output identityAppServiceAppName string = identityService.outputs.appServiceAppName
output hrAppServiceUrl string = hrAppService.outputs.appServiceUrl
output devOpsAppServiceUrl string = devOpsAppService.outputs.appServiceUrl