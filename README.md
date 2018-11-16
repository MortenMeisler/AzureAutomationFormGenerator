# Azure Automation Form Generator
Generate input forms from Azure Automation Runbooks and submit


## Overview

1. Create Runbook with powershell parameters:
![Create Runbook with powershell parameters](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto00.png?raw=true)

2. Fetch the runbook from the url
![Fetch the runbook from the url](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto01.png?raw=true)

## Requirements

 - Create Service Principal that has permission to your Azure Automation Account. [Read more here:](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal)

## Getting Started

```
1. Fork or clone the project
2. Open appsettings.josn and change the values to your own
3. Right-click WebUI project and click Manage User Secrets:
4. Paste the following:
{
  
  "ClientSecret": "YOURSECRETAPPKEY"
}
Alternatively create an encrypted key called ClientSecret in Application Settings on your Azure Web App (if you decide to publish it)


```


