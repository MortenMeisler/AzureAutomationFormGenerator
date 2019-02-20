# Azure Automation Form Generator
Generate input forms from Azure Automation Runbooks and submit

deployment based on [https://github.com/azure-appservice-samples/ToDoApp](https://github.com/azure-appservice-samples/ToDoApp)

Microsoft documentation: [https://docs.microsoft.com/en-us/azure/app-service/app-service-deploy-complex-application-predictably](https://docs.microsoft.com/en-us/azure/app-service/app-service-deploy-complex-application-predictably)

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

## Overview

1. Create Runbook with powershell parameters:
![Create Runbook with powershell parameters](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto00.png?raw=true)

2. Fetch the runbook from the url
![Fetch the runbook from the url](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto01.PNG?raw=true)

## Requirements

 - Create a Service Principal that has permission to your Azure Automation Account. [Read more here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal)
## Getting Started - Publish to Azure
1. Create a new azure ad app that has access to your Azure Automation account (or use existing)
2. Hit Deploy to Azure button
3. Fill out the fields
4. Create a new Redirect URI in your azure app. Under Authentication. Paste the following: https://<NameOfYourWebSite>.azurewebsites.net/signin-oidc


## Getting Started - Development

1. Fork or clone the project
2. Open appsettings.json and change the values to your own
3. Right-click WebUI project and click Manage User Secrets:
4. Paste the following:

```
{
  
  "ClientSecret": "YOURSECRETAPPKEY"
}
``'




