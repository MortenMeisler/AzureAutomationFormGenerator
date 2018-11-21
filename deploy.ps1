Import-Module AzureRM
Set-Location $PSScriptRoot

<# 
# Run below code to create a PS script containing the vars that we need. This file is being ignored by git so that
# we do not accidentally expose them to Github
@"
$SubscriptionId = 'SUBSCRIPTION_ID'
$TenantId = 'TENANT_ID'
"@ | Out-File ".\vars.ps1"
#>

# dot source the vars file
. ".\vars.ps1"

try {
    Select-AzureRmSubscription -SubscriptionId $SubscriptionId | Out-Null
}
catch {
    Login-AzureRmAccount -TenantId $TenantId
}

Select-AzureRmSubscription -SubscriptionId $SubscriptionId | Out-Null

$ResourceGroupName = 'rg81e35VYltTU3'
$Location = 'northeurope'
$TemplateParameterObject = @{
    siteName = 'siteB8jHNFN8wusv'    
    Location = $Location
    repoUrl = 'https://github.com/MortenMeisler/AzureAutomationFormGenerator'
}

New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location -Force | Out-Null

Test-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile ".\azuredeploy.json" -TemplateParameterObject $TemplateParameterObject

New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile ".\azuredeploy.json" -TemplateParameterObject $TemplateParameterObject