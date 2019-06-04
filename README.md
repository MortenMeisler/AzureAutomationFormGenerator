# Azure Automation Form Generator
Generate input forms from Azure Automation Runbooks and run them.





[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

*Deployment based on [this sample](https://github.com/azure-appservice-samples/ToDoApp) / [Microsoft documentation](https://docs.microsoft.com/en-us/azure/app-service/app-service-deploy-complex-application-predictably)
## Overview

1. Create a Runbook with powershell parameters:
<br><br>
![Create a Runbook with powershell parameters](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto00.png?raw=true)

2. Fetch the runbook from the url
<br><br>
![Fetch the runbook from the url](https://github.com/MortenMeisler/AzureAutomationFormGenerator/blob/master/doc/howto01.PNG?raw=true)

#### ...or just use the default landing page to get a list of runbooks that you have tagged to be published ####
<br><br>
![default landingpage](https://raw.githubusercontent.com/MortenMeisler/AzureAutomationFormGenerator/master/doc/howto02.png)

(page can be styled as you like through the site.css file)

## Requirements

The website relies on a Run As Account (Service Principal) for executing the runbooks (this is created through the Automation Account setup) and an Azure AD application for authentication and authorization of the user accessing the site. Authentication is optional in case you want to embed the site as an iframe inside another site of yours (ex. sharepoint, ITSM platform etc.). There are support for Cross-origin resource sharing (CORS) where you specify your primary site address. Check Page types section.
 
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
Default landing page will have a left menu with your runbooks listed. This will only grap your runbooks that has a tag of Key: `FormGenerator:Visibility` Value: `Public`. Neither will it be possible to fetch a runbook without this tag.

### Other tags
`FormGenerator:DisplayName` Value: `My Runbook Name` Make a friendly runbook name with spaces, otherwise it uses the current runbook name. 

Example:
<br><br>
![tags](https://raw.githubusercontent.com/MortenMeisler/AzureAutomationFormGenerator/master/doc/tagshowto03.png)

## Page types
Instead of using the default landing page, you can use other page options to only show the individual runbook and then integrate the site in another site of yours:
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
