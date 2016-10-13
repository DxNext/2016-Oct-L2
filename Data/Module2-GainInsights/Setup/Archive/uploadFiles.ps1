$resourceGroupName = 'DataCodeLab' # default resource group name to create the resources
$logsContainer = 'partsunlimited' # container where the sample log files and HQL script will be uploaded (container is created by this script)
$statsContainer = 'processeddata' # container where the processed data will be stored (container is created by this script)
$dataFolder = 'Assets' # folder where are the files to be uploaded to the storage
# Note: other parameters will be prompted to the user

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

$location = Read-Host "Please enter a location where you'd like to host your service [Case Sensitive] (e.g.: East US)" # default location to created the resources

###### create Resource Group ######
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore

if (!$resourceGroup) {
	## create resource group ##
	Write-Host "Creating resource group $resourceGroupName in $location..."
	New-AzureRmResourceGroup -Location $location -Name $resourceGroupName
}

###### create or select storage account ######
$choices = New-Object Collections.ObjectModel.Collection[Management.Automation.Host.ChoiceDescription]
$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&Create new'))
$choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&Reuse existing'))
$decision = $Host.UI.PromptForChoice('Storage account for HDI cluster and logs', 'Do you want to create a new storage account or reuse an existing one?', $choices, 0)
if ($decision -eq 0) {
	## create new storage account ##
	$storageAccountName = Read-Host 'Enter a name for the new storage account'
	$storage = New-AzureRmStorageAccount -Location $location -ResourceGroupName $resourceGroupName -Name $storageAccountName -Type "Standard_LRS"
	
	while(!$storage) {
		Write-Host "Could not create storage account! Try using another account name." -foregroundcolor "red"
		$storageAccountName = Read-Host 'Enter a name for the new storage account'
		$storage = New-AzureRmStorageAccount -Location $location -ResourceGroupName $resourceGroupName -Name $storageAccountName -Type "Standard_LRS"
	}
	
	Write-Host "Getting access key..."
	$storageAccountKey = Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGroupName -Name $storageAccountName | %{ $_.Key1 }
} else {
	## prompt for existing storage account ##
	Write-Host ''
	$storages = Get-AzureRmStorageAccount
	$storIndex = 1
	$storages | select StorageAccountName | foreach -begin {$i=0} -process { $i++; "{0}. {1}" -f $i, $_.StorageAccountName }
	$selectedStor = Read-Host 'Select storage account'
	while( ![int]::TryParse( $selectedStor, [ref]$storIndex ) -or $storIndex -gt $storages.length -or $storIndex -lt 1) {
	  $selectedStor = Read-Host 'Invalid input. Please enter the number of to storage account'
	}
	$storageAccountName = $storages[$storIndex - 1].StorageAccountName
	$resourceGroupName = $storages[$storIndex - 1].ResourceGroupName
	
	$storageAccountKey = Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGroupName -Name $storageAccountName | %{ $_.Key1 }
}

###### populate storage ######
$storageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey

# create containers
Write-Host "Creating container for logs..."
New-AzureStorageContainer -Name $logsContainer -Context $storageContext -Permission Container

Write-Host "Creating container for results..."
New-AzureStorageContainer -Name $statsContainer -Context $storageContext -Permission Container

Write-Host "Uploading files..."

$paths = Get-ChildItem $dataFolder -Recurse -File -Name 

foreach ($path in $paths)
{
	Write-Host "Uploading file..."
	Set-AzureStorageBlobContent -Container $logsContainer -File "$dataFolder\$path" -Context $storageContext -Blob $path -Force
}

@"
UploadFiles.ps1 run date: $(Get-Date)`r
Connection string: DefaultEndpointsProtocol=https;AccountName=$storageAccountName;AccountKey=$storageAccountKey`r
Account name: $storageAccountName`r
Account key: $storageAccountKey`r
Logs container: $logsContainer`r
Processed data container: $statsContainer
"@ | Set-Content Settings_Storage.txt

###### Display used settings ######
Write-Host Resources created in subscription $subscriptionName and resource group $resourceGroupName
Write-Host ''
Write-Host Please take note of the following values:
Write-Host  - Storage Account -
Write-Host "Connection string: DefaultEndpointsProtocol=https;AccountName=$storageAccountName;AccountKey=$storageAccountKey"
Write-Host Account name: $storageAccountName
Write-Host Account key: $storageAccountKey
Write-Host Logs container: $logsContainer
Write-Host ''