
$hdiPassword = 'P@ssword123' # password used to access the HDI cluster dashboard website
$sshPassword = 'MSFTBuild2016'
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

###### select storage account ######

## prompt for existing storage account ##
Write-Host ''
$storages = Get-AzureRmStorageAccount
$storIndex = 1
$storages | select StorageAccountName | foreach -begin {$i=0} -process { $i++; "{0}. {1}" -f $i, $_.StorageAccountName }
$selectedStor = Read-Host 'Select storage account'
while( ![int]::TryParse( $selectedStor, [ref]$storIndex ) -or $storIndex -gt $storages.length -or $storIndex -lt 1) {
  $selectedStor = Read-Host 'Invalid input. Please enter the number corresponding to storage account'
}
$storageAccountName = $storages[$storIndex - 1].StorageAccountName
$resourceGroupName = $storages[$storIndex - 1].ResourceGroupName

$storageAccountKey = Get-AzureRmStorageAccountKey -ResourceGroupName $resourceGroupName -Name $storageAccountName | %{ $_.Key1 }

$storage = Get-AzureRmStorageAccount -ResourceGroupName $resourceGroupName -Name $storageAccountName

$location = $storage.Location

$clusterName = Read-Host 'Enter a unique name for the HDI cluster'

$hdiContainer = $clusterName # container where the HDI cluster files are stored (container is created by this script)
Write-Host "Creating container for HDI cluster..."
$storageContext = New-AzureStorageContext -StorageAccountName $storageAccountName -StorageAccountKey $storageAccountKey
New-AzureStorageContainer -Name $hdiContainer -Context $storageContext -Permission Container


###### Create HDI cluster ######
$passwordAsSecureString = ConvertTo-SecureString $hdiPassword -AsPlainText -Force
$clusterCredential = New-Object System.Management.Automation.PSCredential ("admin", $passwordAsSecureString)

$sshpasswordAsSecureString = ConvertTo-SecureString $sshPassword -AsPlainText -Force
$sshCredential = New-Object System.Management.Automation.PSCredential ("hdiadmin", $sshpasswordAsSecureString)

Write-Host "Creating HDI cluster (will take ~15 minutes)..."

$config = New-AzureRmHDInsightClusterConfig `
    -HeadNodeSize Large `
    -WorkerNodeSize Large `
    -ClusterType Hadoop 

$hdi = New-AzureRmHDInsightCluster `
	-OSType Linux `
	-Version "3.4" `
	-ClusterSizeInNodes 2 `
	-ResourceGroupName $resourceGroupName `
	-ClusterName $clusterName `
	-HttpCredential $clusterCredential `
	-Location $location `
    -Config $config `
    -DefaultStorageAccountName "$storageAccountName.blob.core.windows.net" `
    -DefaultStorageAccountKey $storageAccountKey `
	-DefaultStorageContainer $hdiContainer 


while(!$hdi) {
	Write-Host "Could not create HDI cluster! Please try with another cluster name" -foregroundcolor "red"
	$clusterName = Read-Host 'Enter a unique name for the HDI cluster'
	Write-Host "Creating HDI cluster (will take ~15 minutes)..."
	$hdi = New-AzureRmHDInsightCluster `
		-ClusterType Hadoop `
		-OSType Windows `
		-Version "HDI 3.3" `
		-ClusterSizeInNodes 1 `
		-ResourceGroupName $resourceGroupName `
		-ClusterName $clusterName `
		-HttpCredential $clusterCredential `
		-Location $location `
		-DefaultStorageAccountName "$storageAccountName.blob.core.windows.net" `
		-DefaultStorageAccountKey $storageAccountKey `
		-DefaultStorageContainer $hdiContainer 
}

@"
CreateHDICluster.ps1 run date: $(Get-Date)`r
Cluster name: $clusterName`r
HTTP user: admin`r
HTTP password: $hdiPassword`r
Storage account name: $storageAccountName`r
Storage account key: $storageAccountKey`r
HDI storage container: $hdiContainer
"@ | Set-Content Settings_HDICluster.txt

###### Display used settings ######
Write-Host Resources created in subscription $subscriptionName and resource group $resourceGroupName
Write-Host ''
Write-Host Please take note of the following values:
Write-Host  - HDI cluster -
Write-Host Cluster name: $clusterName
Write-Host HTTP user: admin
Write-Host HTTP password: $hdiPassword
Write-Host Storage account name: $storageAccountName
Write-Host Storage account key: $storageAccountKey
Write-Host HDI storage container: $hdiContainer
Write-Host ''


Use-AzureRmHDInsightCluster -ResourceGroupName $resourceGroupName -ClusterName $clusterName -HttpCredential $clusterCredential
 

$queryString = "DROP TABLE IF EXISTS RawData;"

$queryString += "CREATE EXTERNAL TABLE LogsRaw (jsonentry string) 
PARTITIONED BY (year int, month int, day int)
STORED AS TEXTFILE LOCATION 'wasb://partsunlimited@$storageAccountName.blob.core.windows.net/logs';"


$queryString = "DROP TABLE IF EXISTS OutputTable;"

$queryString += "CREATE EXTERNAL TABLE OutputTable (
productid int,
title string,
category string,
type string,
totalClicked int
) PARTITIONED BY (year int, month int, day int) 
ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
STORED AS TEXTFILE LOCATION 'wasb://processeddata@$storageAccountName.blob.core.windows.net/logs';"


$uriHiveJobsubmit = "https://$clusterName.azurehdinsight.net/templeton/v1/hive?user.name=$clusterUsername"

$hiveJobDefinition = @{execute=$queryString
                       statusdir="ShowTableStatus"
                       enablelog="false"}


$hiveJobDefinition = New-AzureRmHDInsightHiveJobDefinition -Query $queryString 

#Submit the job to the cluster
Write-Host "Start the Hive job..." -ForegroundColor Green

$hiveJob = Start-AzureRmHDInsightJob -ClusterName $clusterName -JobDefinition $hiveJobDefinition -ClusterCredential $clusterCredential

#Wait for the Hive job to complete
Write-Host "Wait for the job to complete..." -ForegroundColor Green
Wait-AzureRmHDInsightJob -ClusterName $clusterName -JobId $hiveJob.JobId -ClusterCredential $clusterCredential


Write-Host "HDInsight Cluster is created along with the tables: RawData & OutputTable!"