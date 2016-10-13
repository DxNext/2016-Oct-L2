$resourceGroupName = 'DataCodeLab' # default resource group name to create the resources
$dataWarehouseUser = 'dwadmin' # username used for the SQL Server where the Data Warehouse is hosted
$dataWarehousePassword = 'P@ssword123' # password used for the SQL Server where the Data Warehouse is hosted
$dataWarehouseDbName = 'partsunlimited' # name of the SQL Data Warehouse database
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


$dwServerName = Read-Host 'Enter a unique name for the SQL Data Warehouse server'

###### Create SQL Data Warehouse ######
$passwordAsSecureString = ConvertTo-SecureString $dataWarehousePassword -AsPlainText -Force
$dwCredential = New-Object System.Management.Automation.PSCredential ($dataWarehouseUser, $passwordAsSecureString)
Write-Host Creating SQL Server...
$sqlServer = New-AzureRmSqlServer -ResourceGroupName $resourceGroupName -ServerName $dwServerName -SqlAdministratorCredentials $dwCredential -Location $location -ServerVersion "12.0"
while(!$sqlServer) {
	Write-Host "Could not create SQL Data Warehouse! Please try with another server name" -foregroundcolor "red"
	$dwServerName = Read-Host 'Enter a unique name for the SQL Data Warehouse server'
	Write-Host Creating SQL Server...
	$sqlServer = New-AzureRmSqlServer -ResourceGroupName $resourceGroupName -ServerName $dwServerName -SqlAdministratorCredentials $dwCredential -Location $location -ServerVersion "12.0"
}
Write-Host Setting Firewall rule for Azure services...
New-AzureRmSqlServerFirewallRule -ResourceGroupName $resourceGroupName -ServerName $dwServerName -FirewallRuleName "azure" -StartIpAddress "0.0.0.0" -EndIpAddress "255.255.255.255"
Write-Host Setting Firewall rule for local machine...


Write-Host Creating Data Warehouse $dataWarehouseDbName...
New-AzureRmSqlDatabase -RequestedServiceObjectiveName "DW100" -DatabaseName $dataWarehouseDbName -ServerName $dwServerName -ResourceGroupName $resourceGroupName -Edition "DataWarehouse"

## create tables
Write-Host Creating database schema...
$sqlConn = New-Object System.Data.SQLClient.SQLConnection "Data Source=tcp:$dwServerName.database.windows.net,1433;Initial Catalog=$dataWarehouseDbName;Integrated Security=False;User ID=$dataWarehouseUser@$dwServerName;Password=$dataWarehousePassword;Connect Timeout=30;Encrypt=True";
$sqlConn.Open()
$sqlCmd = New-Object System.Data.SqlClient.SqlCommand
$sqlCmd.Connection = $sqlConn
$sqlCmd.CommandText = "CREATE TABLE dbo.ProductLogs (productid int, title nvarchar(50), category nvarchar(50), type nvarchar(5), totalClicked int)"
$sqlCmd.ExecuteNonQuery()
$sqlCmd.CommandText = "CREATE TABLE dbo.ProductStats (category nvarchar(50), title nvarchar(50), views int, adds int)"
$sqlCmd.ExecuteNonQuery()
$sqlCmd.CommandText = @"
CREATE PROCEDURE sp_populate_stats AS 
BEGIN 
 DELETE FROM dbo.ProductStats; 
 INSERT INTO dbo.ProductStats 
 SELECT 
  category, 
  title, 
  SUM(CASE WHEN type = 'view' THEN totalClicked ELSE 0 END) AS views, 
  SUM(CASE WHEN type = 'add' THEN totalClicked ELSE 0 END) AS adds 
 FROM dbo.ProductLogs GROUP BY title, category 
END
"@
$sqlCmd.ExecuteNonQuery()
$sqlConn.Close()
$sqlConn.Dispose()

@"
CreateDataWarehouse.ps1 run date: $(Get-Date)`r
Server name: $dwServerName`r
Server user: $dataWarehouseUser`r
Server password: $dataWarehousePassword`r
Database name: $dataWarehouseDbName
"@ | Set-Content Settings_DataWarehouse.txt

###### Display used settings ######
Write-Host Resources created in subscription $subscriptionName and resource group $resourceGroupName
Write-Host ''
Write-Host Please take note of the following values:
Write-Host  - SQL Data Warehouse -
Write-Host Server name: $dwServerName
Write-Host Server user: $dataWarehouseUser
Write-Host Server password: $dataWarehousePassword
Write-Host Database name: $dataWarehouseDbName
Write-Host ''