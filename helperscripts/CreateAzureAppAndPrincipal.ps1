#Requires -Version 5.1
param(
    $ServicePrincipalDisplayName = "AutomationFormGeneratorSP"

)
$ErrorActionPreference = "Stop"


if ((Get-Module Az.Accounts, Az.Resources -ListAvailable).Count -lt 2)
{
    #RunAs Administrator
    {   
    $arguments = "& '" + $myinvocation.mycommand.definition + "'"
    Start-Process powershell -Verb runAs -ArgumentList $arguments
    Break
    }
    Install-Module Az.Accounts, Az.Resources
}

#Connect to Azure
Connect-AzAccount


#SET SUBSCRIPTION
$Subscription = Get-AzSubscription | out-gridview -PassThru -Title "Select Subscription for storing service principal" | Select-AzSubscription
#>

#Create Service principal
$Sp = Get-AzADServicePrincipal -DisplayName $ServicePrincipalDisplayName
if ($null -eq $Sp)
{
    $sp = New-AzADServicePrincipal -DisplayName $ServicePrincipalDisplayName
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($sp.Secret)
    $UnsecureSecret = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}


#Create App first
#$App = Get-AzADApplication -DisplayName $ApplicationName
#if ($null -eq $App)
#{
#    $App = New-AzADApplication -DisplayName $ApplicationName -
#}

$automationAccount = Get-AzAutomationAccount | Out-GridView -PassThru -Title "Select Automation Account"

New-AzRoleAssignment -ObjectId $sp.Id -RoleDefinitionName "Automation Operator" -Scope "/subscriptions/$($automationAccount.SubscriptionId)/resourceGroups/$($automationAccount.ResourceGroupName)/providers/Microsoft.Automation/automationAccounts/$($automationAccount.AutomationAccountName)"

Get-AzRoleDefinition -Name "automation*"