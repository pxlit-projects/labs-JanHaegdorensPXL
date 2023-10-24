@description('Name of the Service Bus namespace')
param serviceBusNamespaceName string

@description('Location for all resources.')
param location string = resourceGroup().location

var uniqueServiceBusNamespaceName = '${serviceBusNamespaceName}-${uniqueString(resourceGroup().id)}'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: uniqueServiceBusNamespaceName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {} 
}

// Retrieve the (automatically created) shared access policy and get the connection string
var rootManageSharedAccessKeyName = '${serviceBusNamespace.id}/AuthorizationRules/RootManageSharedAccessKey'
output sharedAccessConnectionString string = listKeys(rootManageSharedAccessKeyName, serviceBusNamespace.apiVersion).primaryConnectionString