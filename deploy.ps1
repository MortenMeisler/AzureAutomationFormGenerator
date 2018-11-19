Import-Module AzureRM

try {
    Select-AzureRmSubscription -SubscriptionId $SubscriptionId | Out-Null
}
catch {
    Login-AzureRmAccount -TenantId $TenantId
}

Select-AzureRmSubscription -SubscriptionId $SubscriptionId | Out-Null

$ResourceGroupName = 'rg81e35VYltTU3'
$Location = 'northeurope'

Set-Location $PSScriptRoot

New-AzureRmResourceGroup -Name $ResourceGroupName -Location $Location -Force | Out-Null

$TemplateParameterObject = @{
    siteName = 'siteB8jHNFN8wusv'
    hostingPlanName = 'planB8jHNFN8wusv'
    siteLocation = $Location
    repoUrl = 'https://github.com/MortenMeisler/AzureAutomationFormGenerator'
    sqlServerName = 'sqlB8jHNFN8wusv'.ToLower()
    sqlServerLocation = $Location
    sqlServerAdminPassword = 'TQ4NkJRjo4ICZfC*8sm7!pnm'
    automationAccountName = 'dXRgrIsj4Dd7FgW7tPL75YO5'
    automationAccountResourceGroup = 'dXRgrIsj4Dd7FgW7tPL75YO5'
}

Test-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile ".\azuredeploy.json" -TemplateParameterObject $TemplateParameterObject

New-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName -TemplateFile ".\azuredeploy.json" -TemplateParameterObject $TemplateParameterObject