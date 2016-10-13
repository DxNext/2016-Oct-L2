$HDInsightFolder = '<local path of the HDInsights log folder. For example C:\GitHub\azure_lambda_labs\Module2-GainInsights\Setup\Assets\HDInsight>' 
$dwdataFolder = '<local path of the ADW log folder. For example C:\GitHub\azure_lambda_labs\Module2-GainInsights\Setup\Assets\ADW>' 


Login-AzureRmAccount

$logsContainer = 'partsunlimited' # container where the sample log files and HQL script will be uploaded (container is created by this script)
$dwdataContainer = 'dwdata' # container where the ADW sample data data will be stored (container is created by this script)
$statsContainer = 'processeddata' # container where the processed data will be stored (container is created by this script)


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

###### create Resource Group ######
$location = Read-Host "Please enter a location where you'd like to host your service [Case Sensitive] (e.g.: East US)" # default location to created the resources
$resourceGroupName = Read-Host "Please enter a globally unique name for the resource group"

$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction Ignore

if (!$resourceGroup) {
	## create resource group ##
	Write-Host "Creating resource group $resourceGroupName in $location..."
	New-AzureRmResourceGroup -Location $location -Name $resourceGroupName
}

	## create new storage account ##
	$storageAccountName = Read-Host 'Enter a name for the new storage account'
	$storage = New-AzureRmStorageAccount -Location $location -ResourceGroupName $resourceGroupName -Name $storageAccountName -Type "Standard_LRS"
	
	while(!$storage) {
		Write-Host "Could not create storage account! Try using another account name." -foregroundcolor "red"
		$storageAccountName = Read-Host 'Enter a name for the new storage account'
		$storage = New-AzureRmStorageAccount -Location $location -ResourceGroupName $resourceGroupName -Name $storageAccountName -Type "Standard_LRS"}
	Write-Host "Getting access key..."
	$storageAccountKey = (Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGroupName -Name $storageAccountName).Value[0]

###### populate storage ######
$storageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey

# create containers
Write-Host "Creating container for logs..."
New-AzureStorageContainer -Name $logsContainer -Context $storageContext -Permission Container

Write-Host "Creating container for results..."
New-AzureStorageContainer -Name $statsContainer -Context $storageContext -Permission Container

Write-Host "Creating container for results..."
New-AzureStorageContainer -Name $dwdataContainer -Context $storageContext -Permission Container

Write-Host "Uploading files..."

$paths = Get-ChildItem $HDInsightFolder -Recurse -File -Name 

foreach ($path in $paths)
{
	Write-Host "Uploading file..."
	Set-AzureStorageBlobContent -Container $logsContainer -File "$HDInsightFolder\$path" -Context $storageContext -Blob $path -Force
}

$paths = Get-ChildItem $dwdataFolder -Recurse -File -Name 

foreach ($path in $paths)
{
	Write-Host "Uploading file..."
	Set-AzureStorageBlobContent -Container $dwdataContainer -File "$dwdataFolder\$path" -Context $storageContext -Blob $path -Force
}