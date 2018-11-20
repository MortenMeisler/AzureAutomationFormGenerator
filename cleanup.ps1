Import-Module AzureRM

try {
    Select-AzureRmSubscription -SubscriptionId $SubscriptionId | Out-Null
}
catch {
    Login-AzureRmAccount -TenantId $TenantId
}

Select-AzureRmSubscription -SubscriptionId $SubscriptionId | Out-Null

#<# cleanup
Get-AzureRmResourceGroup | Remove-AzureRmResourceGroup -AsJob -Force -Verbose | Out-Null
while((Get-AzureRmResourceGroup).Count -gt 0)
{
    Start-Sleep -Seconds 5
}
#>