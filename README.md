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

#### Default landing page ####
![Fetch the runbook from the url](https://raw.githubusercontent.com/MortenMeisler/AzureAutomationFormGenerator/master/doc/howto02.png)

## Requirements

 - create an Azure AD application and service principal that can access your Azure Automation Account. [Read more here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal)
 
## Getting Started

### Publish to Azure
1. Create a new Azure AD App that has access to your Azure Automation account (or use existing)
2. Hit Deploy to Azure button
3. Fill out the fields and deploy.
4. Once deployed, create a new Redirect URI in your azure app under Authentication. Paste the following: https://`<NameOfYourWebSite>`.azurewebsites.net/signin-oidc


### Development

1. Fork or clone the project
2. Open appsettings.json and change the values to your own
3. Right-click WebUI project and click Manage User Secrets:
4. Paste the following:

```
{
  
  "ClientSecret": "YOURSECRETAPPKEY"
}
```
5. You can now build project and if you want publish to own website host.

## Usage
Default landing page will have a left menu with you runbooks listed. This will only grap your runbooks with the following **tag**:
- Key: `FormGenerator` Value: `Public`
### Other page types
- Full Width page: https://`<NameOfYourWebSite>`.azurewebsites.net/`<NameOfYourRunbook>`**?pageType=1**
- Centered page: https://`<NameOfYourWebSite>`.azurewebsites.net/`<NameOfYourRunbook>`**?pageType=2**

### Syntax and parameter options
- Use ```[Alias("My Name of Parameter")]``` in powershell runbook to have friendly names of your parameters
- Use ```[ValidateSet("Value1", "Value2", "Value3")]``` for selection dropdown list
- Use ```[string[]]``` or ```[object[]]``` or ```[PSObject[]]``` to make an array parameter for adding/removing items
- Use ```[Parameter(Mandatory=$true)]``` to make fields required
- Set your parameter equal something to have a default value. Example: ```$MyVariable = "This is my default value"```



