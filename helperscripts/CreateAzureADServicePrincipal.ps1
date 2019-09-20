#Requires -Version 5.1
#---------------------------------------------------------------------------------------------------------------------------
# This will create a Service Principal for your selected Azure AD Tenant. 
# It will assign Reader and Automation Job Operator for a selected Automation Account (you will get an out-gridview to select things)
#---------------------------------------------------------------------------------------------------------------------------
param(
    $ServicePrincipalDisplayName = "AutomationFormGeneratorSP"

)
$ErrorActionPreference = "Stop"

#Install required modules if not exist
$modules = @("Az.Accounts2", "Az.Resources", "Az.Automation")
if ((Get-Module $modules  -ListAvailable).Count -lt $modules.Count)
{
    #RunAs Administrator
    If (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
    {   
        $arguments = "& '" + $myinvocation.mycommand.definition + "'"
        Start-Process powershell -Verb runAs -ArgumentList $arguments
        Break
    }
    write-host "Installing modules: $modules ..."
    Install-Module $modules
}

#Connect to Azure
Connect-AzAccount


#Select Subscription
$Subscription = Get-AzSubscription | out-gridview -PassThru -Title "Select Subscription for storing service principal" | Select-AzSubscription
#>

#Create Service principal
$Sp = Get-AzADServicePrincipal -DisplayName $ServicePrincipalDisplayName
if ($null -eq $Sp)
{
    $sp = New-AzADServicePrincipal -DisplayName $ServicePrincipalDisplayName
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($sp.Secret)
    $UnsecureSecret = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}else{
    $confirmation = Read-Host "Service Principal with name $ServicePrincipalDisplayName already exists, do you want to create a new secret? (y/n)"
    if ($confirmation -eq 'y') {
        $creds = New-AzADServicePrincipalCredential -ObjectId $sp.Id
        $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($creds.Secret)
        $UnsecureSecret = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    }
    
}



#Select automation account
$automationAccount = Get-AzAutomationAccount | Out-GridView -PassThru -Title "Select Automation Account"

#Assign roles to service principal for automation account
New-AzRoleAssignment -ObjectId $sp.Id -RoleDefinitionName "Reader" -Scope "/subscriptions/$($automationAccount.SubscriptionId)/resourceGroups/$($automationAccount.ResourceGroupName)/providers/Microsoft.Automation/automationAccounts/$($automationAccount.AutomationAccountName)"
New-AzRoleAssignment -ObjectId $sp.Id -RoleDefinitionName "Automation Job Operator" -Scope "/subscriptions/$($automationAccount.SubscriptionId)/resourceGroups/$($automationAccount.ResourceGroupName)/providers/Microsoft.Automation/automationAccounts/$($automationAccount.AutomationAccountName)"

#Output
#Get-AzRoleDefinition | ?{$_.Name -like "*job operator*"}
$obj = [ordered]@{
        "Subscription Id"                    = $Subscription.Subscription.Id
        "Application Id"                     = $sp.ApplicationId
        "Application Secret"                 = $UnsecureSecret
        "Automation Account Name"            = $automationAccount.AutomationAccountName
        "Automation Account Subscription Id" = $automationAccount.SubscriptionId
        }

$PSObject = New-Object PSObject -Property $obj


"-"*25 + " Use this infomation for creating the Form Generator Website deployment " + "-"*25
$PSObject | fl