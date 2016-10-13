$location = 'West US' # default location to created the resources

Login-AzureRmAccount

###### select subscription ######
$subscriptions = Get-AzureRmSubscription | where {!$_.State -or $_.State -eq "Enabled"}

if ($subscriptions.length -eq 1) {
	$subscriptionName = $subscriptions[0].SubscriptionName;
	$subscriptionId = $subscriptions[0].SubscriptionId;
	
	$subscriptions[0] | Select-AzureRmSubscription -ErrorAction Stop
}
elseif ($subscriptions.length -eq 0) {
	Write-Host "There are no available subscriptions for this account!" -foregroundcolor "red"
	exit
}
else {
	Write-Host Available subscriptions:
	$subscriptions | select SubscriptionName | foreach -begin {$i=0} -process { $i++; "{0}. {1}" -f $i, $_.SubscriptionName }
	Write-Host ''

	$subsIndex = 1
	$selectedSub = Read-Host 'Please select the subscription to deploy the assets?'

	while( ![int]::TryParse( $selectedSub, [ref]$subsIndex ) -or $subsIndex -gt $subscriptions.length -or $subsIndex -lt 1) {
	  $selectedSub = Read-Host 'Invalid input. Please enter the number of a subscription'
	}
	
	$subscriptionName = $subscriptions[$subsIndex - 1].SubscriptionName;
	$subscriptionId = $subscriptions[$subsIndex - 1].SubscriptionId;
	
	$subscriptions[$subsIndex - 1] | Select-AzureRmSubscription -ErrorAction Stop
}

Write-Host "Selected subscription: $subscriptionName"

# request input settings
$resourceGroupName = Read-Host 'Resource group name (default: DataCodeLab)'
if ($resourceGroupName -eq "") {
	$resourceGroupName = "DataCodeLab"
}

## prompt for existing storage account ##
$storages = Get-AzureRmStorageAccount
$storIndex = 1
Write-Host ''
$storages | select StorageAccountName | foreach -begin {$i=0} -process { $i++; "{0}. {1}" -f $i, $_.StorageAccountName }
$selectedStor = Read-Host 'Select storage account'
while( ![int]::TryParse( $selectedStor, [ref]$storIndex ) -or $storIndex -gt $storages.length -or $storIndex -lt 1) {
  $selectedStor = Read-Host 'Invalid input. Please enter the number of to storage account'
}
$storageAccountName = $storages[$storIndex - 1].StorageAccountName
$storageResourceGroupName = $storages[$storIndex - 1].ResourceGroupName

$storageAccountKey = Get-AzureRmStorageAccountKey -ResourceGroupName $storageResourceGroupName -Name $storageAccountName | %{ $_.Key1 }


$HDIClusterName = Read-Host 'HDI cluster name'
$HDIUser = Read-Host 'HDI admin user name (default: admin)'
if ($HDIUser -eq "") { $HDIUser = "admin" }
$HDIPassword = Read-Host 'HDI admin password (default: P@ssword123)'
if ($HDIPassword -eq "") { $HDIPassword = "P@ssword123" }

$DWServerName = Read-Host 'SQL Data Warehouse server name'
$DWUser = Read-Host 'SQL Data Warehouse server user (default: dwadmin)'
if ($DWUser -eq "") { $DWUser = "dwadmin" }
$DWPassword = Read-Host 'SQL Data Warehouse server password (default: P@ssword123)'
if ($DWPassword -eq "") { $DWPassword = "P@ssword123" }


$dataFactoryName = Read-Host 'Name for the new Data Factory'
$dataFactory = Get-AzureRmDataFactory -ResourceGroupName $resourceGroupName -Name $dataFactoryName -ErrorAction Ignore

if (!$dataFactory) {
	New-AzureRmDataFactory -ResourceGroupName $resourceGroupName -Name $dataFactoryName -location $location
    $dataFactory = Get-AzureRmDataFactory -ResourceGroupName $resourceGroupName -Name $dataFactoryName -ErrorAction Ignore
}

$start = Get-Date (Get-Date).AddDays(-3) -f yyyy-MM-ddT00:00:00Z
$end = Get-Date (Get-Date).AddDays(1) -f yyyy-MM-ddT00:00:00Z

Write-Host Populating JSON snippets with setting values...

(Get-Content .\DataFactory\AzureStorageLinkedService.json) | Foreach-Object {
    $_ -replace '<StorageAccountName>', $storageAccountName `
    -replace '<StorageAccountKey>', $storageAccountKey
    } | Set-Content .\DataFactory\AzureStorageLinkedService.json.temp

(Get-Content .\DataFactory\AzureSqlDWLinkedService.json) | Foreach-Object {
    $_ -replace '<DWServerName>', $DWServerName `
    -replace '<DWUser>', $DWUser `
    -replace '<DWPassword>', $DWPassword
    } | Set-Content .\DataFactory\AzureSqlDWLinkedService.json.temp

(Get-Content .\DataFactory\HDInsightLinkedService.json) | Foreach-Object {
    $_ -replace '<HDIClusterName>', $HDIClusterName `
    -replace '<HDIUser>', $HDIUser `
    -replace '<HDIPassword>', $HDIPassword
    } | Set-Content .\DataFactory\HDInsightLinkedService.json.temp

(Get-Content .\DataFactory\JsonLogsToTabularPipeline.json) | Foreach-Object {
    $_ -replace '<StorageAccountName>', $HDIClusterName `
    -replace '<Start>', $start `
    -replace '<End>', $end
    } | Set-Content .\DataFactory\JsonLogsToTabularPipeline.json.temp

(Get-Content .\DataFactory\LogsToDWPipeline.json) | Foreach-Object {
    $_ -replace '<Start>', $start `
    -replace '<End>', $end
    } | Set-Content .\DataFactory\LogsToDWPipeline.json.temp

(Get-Content .\DataFactory\PopulateProductStatsDWPipeline.json) | Foreach-Object {
    $_ -replace '<Start>', $start `
    -replace '<End>', $end
    } | Set-Content .\DataFactory\PopulateProductStatsDWPipeline.json.temp

# upload files
Write-Host Creating linked services...
New-AzureRmDataFactoryLinkedService $dataFactory -File .\DataFactory\AzureStorageLinkedService.json.temp
New-AzureRmDataFactoryLinkedService $dataFactory -File .\DataFactory\AzureSqlDWLinkedService.json.temp
New-AzureRmDataFactoryLinkedService $dataFactory -File .\DataFactory\HDInsightLinkedService.json.temp

Write-Host Creating datasets...
New-AzureRmDataFactoryDataset $dataFactory -File .\DataFactory\LogJsonFromBlob.json
New-AzureRmDataFactoryDataset $dataFactory -File .\DataFactory\LogCsvFromBlob.json
New-AzureRmDataFactoryDataset $dataFactory -File .\DataFactory\LogsSqlDWOutput.json
New-AzureRmDataFactoryDataset $dataFactory -File .\DataFactory\StatsSqlDWOuput.json

Write-Host Creating pipelines...
New-AzureRmDataFactoryPipeline $dataFactory -File .\DataFactory\LogsToDWPipeline.json.temp
New-AzureRmDataFactoryPipeline $dataFactory -File .\DataFactory\JsonLogsToTabularPipeline.json.temp
New-AzureRmDataFactoryPipeline $dataFactory -File .\DataFactory\PopulateProductStatsDWPipeline.json.temp

Write-Host Cleaning temporal files...
Remove-Item .\DataFactory\AzureStorageLinkedService.json.temp
Remove-Item .\DataFactory\AzureSqlDWLinkedService.json.temp
Remove-Item .\DataFactory\HDInsightLinkedService.json.temp
Remove-Item .\DataFactory\LogsToDWPipeline.json.temp
Remove-Item .\DataFactory\JsonLogsToTabularPipeline.json.temp
Remove-Item .\DataFactory\PopulateProductStatsDWPipeline.json.temp