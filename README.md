# Azure Automation Form Generator
Generate input forms from Azure Automation Runbooks and run them.





[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

*Deployment based on [this sample](https://github.com/azure-appservice-samples/ToDoApp) / [Microsoft documentation](https://docs.microsoft.com/en-us/azure/app-service/app-service-deploy-complex-application-predictably)
## Overview

1. Create Runbook with powershell parameters:
![Create Runbook with powershell parameters](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto00.png?raw=true)

2. Fetch the runbook from the url
![Fetch the runbook from the url](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto01.PNG?raw=true)

#### Default landing page ####
![Fetch the runbook from the url](https://raw.githubusercontent.com/MortenMeisler/AzureAutomationFormGenerator/master/doc/howto02.png)

## Requirements

 - Create an Azure AD application and service principal that can access your Azure Automation Account. [Read more here](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal)
 
## Getting Started

### Publish to Azure
1. Create or use existing Automation Azure Automation Run As account [Read more here](https://docs.microsoft.com/en-us/azure/automation/manage-runas-account). 
2. Go to Azure Active Directory -> App Registrations -> Find your Automation App and create a new client secret. Note down the Application Id and Client Secret
3. (Optional) Create an Azure AD App for authentication on the website (or use previous app)
2. Hit Deploy to Azure button
3. Fill out the fields and deploy.
4. Once deployed, create a new Redirect URI in your azure app (from step 3) under Authentication. Paste the following: https://`<NameOfYourWebSite>`.azurewebsites.net/signin-oidc


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
- Full Width page: `https://<NameOfYourWebSite>.azurewebsites.net/<NameOfYourRunbook>`**`?pageType=1`**
- Centered page: `https://<NameOfYourWebSite>.azurewebsites.net/<NameOfYourRunbook>`**`?pageType=2`**

Example:
`https://automationformgeneratordemo.azurewebsites.net/Do-Stuff3?pageType=2`

### Syntax and parameter options
- Use ```[Alias("My Name of Parameter")]``` in powershell runbook to have friendly names of your parameters
- Use ```[ValidateSet("Value1", "Value2", "Value3")]``` for selection dropdown list
- Use ```[string[]]``` or ```[object[]]``` or ```[PSObject[]]``` to make an array parameter for adding/removing items
- Use ```[Parameter(Mandatory=$true)]``` to make fields required
- Set your parameter equal something to have a default value. Example: ```$MyVariable = "This is my default value"```

### Authentication
By default the website uses Azure AD authentication. In Appsettings you can control if you want no authentication, Azure AD authentication or Azure AD authentication with group authorization.

#### Group Authorization
To enable group authorization open your manifest file of your Azure AD app and change the `groupMembershipClaims` to `SecurityGroup`. This change can take up to an hour to take effect.

In Appsettings change the Object Id's of the AD Security Groups that gives access to the site.
