@description('Location of database server')
param location string = 'westeurope'

@description('Name of the SQL Server, randomized based on resource group id')
param sqlServerName string = 'kwops-${uniqueString(resourceGroup().id)}'

var administratorLogin = 'kwopsadmin'
var administratorLoginPassword = 'Pass@word'

var hrDatabaseName = 'KWops.HumanResources'
var devOpsDatabaseName = 'KWops.DevOps'

resource sqlServer 'Microsoft.Sql/servers@2022-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    publicNetworkAccess: 'Enabled'
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorLoginPassword
  }

  resource firewall 'firewallRules@2022-02-01-preview' = {
    name: 'Azure Services'
    properties: {
      // Allow all clients
      // Note: range [0.0.0.0-0.0.0.0] means "allow all Azure-hosted clients only".
      // This is not sufficient, because we also want to allow direct access from developer machine, for debugging purposes.
      startIpAddress: '0.0.0.1'
      endIpAddress: '255.255.255.254'
    }
  }
}

resource hrDatabase 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  parent: sqlServer
  name: hrDatabaseName
  location: location
  sku: {
    tier: 'Basic'
    name: 'Basic'
  }
}

resource devOpsDatabase 'Microsoft.Sql/servers/databases@2022-02-01-preview' = {
  parent: sqlServer
  name: devOpsDatabaseName
  location: location
  sku: {
    tier: 'Basic'
    name: 'Basic'
  }
}

@description('Connection string for the hr database')
output hrConnectionString string = 'Server=${sqlServer.properties.fullyQualifiedDomainName}; Database=${hrDatabaseName}; User=${administratorLogin}; Password=${administratorLoginPassword}'

@description('Connection string for the devOps database')
output devOpsConnectionString string = 'Server=${sqlServer.properties.fullyQualifiedDomainName}; Database=${devOpsDatabaseName}; User=${administratorLogin}; Password=${administratorLoginPassword}'
