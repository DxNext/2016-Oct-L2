<a name="Top"></a>
# Gain Insights from your Data #

---

<a name="Overview"></a>
## Overview ##

Azure Data Factory is a cloud-based data integration service that orchestrates and automates the movement and transformation of data. It works across on-premises and cloud data sources and SaaS to ingest, prepare, transform, analyze, and publish your data.

Transformation activities in Azure Data Factory transform and process your raw data into predictions and insights. The transformation activity executes in a computing environment such as Azure HDInsight cluster or an Azure Batch. You will create an Azure HDInsight Hive activity to execute a Hive query.

Azure SQL Data Warehouse is an enterprise-class, distributed database capable of processing massive volumes of relational and non-relational data. It's the industry's first cloud data warehouse that combines proven SQL capabilities with the ability to grow, shrink, and pause in seconds. SQL Data Warehouse is also deeply ingrained into Azure, and you'll use Data Factory to move generated statistics into SQL Data Warehouse and visualize it in Power BI.

In this module, you'll use Data Factory to compose services into managed data flow pipelines to transform the data ingested from the Parts Unlimited logs into meaningful data to be visualized using Power BI.

<a name="Objectives"></a>
### Objectives ###
In this module, you'll see how to:

- Create a **HDInsight Cluster** and issue a **Hive** query to do analytics on your data
- Create an **Azure Data Warehouse** and load data from Azure storage
- Create an **Azure Data Factory** and orchestrate an **Azure Data Factory** workflow
- Automate data movement from **HDInsight** to **SQL Data Warehouse** using **Polybase**
- Use the data in **SQL Data Warehouse** to generate **Power BI** visualizations

<a name="Prerequisites"></a>
### Prerequisites ###

The following is required to complete this module:

- [Microsoft Visual Studio Community 2015][1] or greater
- [Microsoft Azure Storage Explorer][2] or any other tool to manage Azure Storage
- [Microsoft Azure PowerShell][3] (1.0 or above) OR [Azure CLI][4] (You will need NodeJS installed on your machine in order to use Azure CLI)
- Putty or any other ssh tool

[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: http://storageexplorer.com/
[3]: https://azure.microsoft.com/en-us/documentation/articles/powershell-install-configure/
[4]: https://azure.microsoft.com/en-us/documentation/articles/xplat-cli-install/


<a name="Setup"></a>
### Setup ###
In order to run the exercises in this module, you'll need to create an HDI cluster, a SQL Data Warehouse, a storage account and upload some asset files. You can either follow the Auto Setup or the Manual Setup steps listed below. If this is the first time you're working with these services, we highly recommend you go through the Manual Setup process.  

> **Important:** when entering the names for the storage account, SQL DW server and HDInsight server, those must be **globally unique**. To ensure this, you can use your first and last name as prefixes for the resource names. For instance: "johndoestorage", "johndoehdi" and "johndoesqlserver".

<a name="AutoSetup"> </a>
#### Auto Setup #####
Navigate to the Setup Folder under 'Module 2'. You will find a folder called as Initial Setup. If you are using Azure Powershell, go into the folder named 'ps'. If you're using the Azure CLI, navigate to the 'cli' folder. Now, execute the **setup** file.

You will be prompted for the following information:

1. Login credentials for Microsoft Azure
1. Resource Group Name you'd like to use for the lab 

<a name="ManualSetupUploadFiles"></a>
#### Manual Setup 1: Manually uploading the sample files ####

In this section you will create a new storage account, and load sample data that will be used later in the module. 

Navigate to the Setup Folder under 'Module 2'. You will find a folder called Setup\CLI. 

1. Update the parameters.json file.  Update parameters with a unique suffix for use across the services. Save the file. Particularly, you will want to update the 'uniqueSuffix' variable. This will help keep your resource names globally unique. We will be executing the azuredeplou.json ARM template, which will help us setup the resources needed for this lab.

1. Open a command prompt and navigate to the cli directory.  

1. Execute the following statements to log into your Azure subscription

	````
	azure login

	azure account list

	````

1. Copy the subscription id from the subscription to use in the lab.  Paste it in the following script and execute in the command prompt.

	````
	azure account set <subcriptionid>

	````

1. Execute the following command to create a resource group.  Use the same unique prefix from the parameters file. 

	````
	azure group create <ResourceGroupName> <Location>

	````

1. Execute the following statement to execute the ARM template that will deploy the storage account, the HDInsight Cluster and the Azure DW.  Set <DeploymentName> with the same prefix used for the ResourceGroupName.

	````
	azure group deployment create -f <path to azuredeploy.json> -e <path to parameters.json> -g <ResourceGroupName> -n <DeploymentName>

	````

**This section will complete in approximately 15-25 minutes**


<a name="ManualSetupUploadFiles"></a>
#### Manual Setup 1: Manually uploading the sample files ####

In this section you will create a new storage account, and load sample data that will be used later in the module. 


1. When the storage account has been provisioned and the files are uploaded, open the [Microsoft Azure portal](https://portal.azure.com/) to copy the storage account access key.  
	1. Navigate to All Resources and type the name of your storage account in the Filter items box.  
	1. Click on the storage account.
	1. Select Access Keys from the Settings blade to copy the storage account key.  
	> **Note:** You will use the storage key in several steps during this lab. **Copy this to a text file and save in a readily accessible location (ie Desktop).**

	![Getting account name and key](Images/setup-storage-key.png?raw=true "Getting account name and key")

	_Getting account name and key_


1. Open **Azure Storage Explorer** or the tool of your preference to connect to the new storage account using the account name and key from the previous step. 
	1. On the left pane of _Azure Storage Explorer_, right-click on **Storage Accounts** and select **Connect to Azure Storage...** 
	1. Enter the account name and key in the dialog, then click **OK**.

	> **Note:** You can also add an Storage Account by login to your Azure accounts and pick storage accounts. To do this, click the settings button (the "wrench" symbol). 

	> ![Settings](Images/azure-storage-explorer-settings.png?raw=true "Settings")

	> _Settings_

	> ![Add an Account](Images/azure-storage-explorer-add-account.png?raw=true "Add an Account")

	> _Add an Account_

1. Create new blob containers by right clicking on the storage account name and clicking 'Create Blob Container'.

1. Create the following two containers:
	1. partsunlimited
	1. processeddata

1. Verify the three new Blob Containers exist. 
	1. In _Azure Storage Explorer_ expand your account and expand **Blob Containers** to verify two new containers with the name **partsunlimited** & **processeddata** were created. A separate container for the HDInsight cluster will also be present. 

1. Switch back to your repository and navigate to the folder path: Module2-GainInsights\Setup\Assets.

1. Drag the 'logs' and the 'productcatalog' folder to the Storage explorer client and drop it into the 'partsunlimited' container.

1. Switch back to your repository and navigate to the folder path: Module2-GainInsights\Setup\Assets\HDInsight.

1. Drag the 'Scripts' folder to the Storage explorer client and drop it into the 'partsunlimited' container.


You should now have sample data and a new storage account with three blob containers. 

- The partsunlimited blob container should contain the raw logs data, the productcatalog data and scripts folders. 
- The processeddata container should be empty, as it will be populated during the Azure Data Factory lab.
- The <clusterName> container which will be the same name as your HDInsight Cluster




<a name="ManualSetupHDI"></a>
#### (Optional) Manual Setup 2: Creating the HDI cluster ####

1. In the [Microsoft Azure portal](https://portal.azure.com/), create a new HDI cluster (_New > Data + Analytics > HDInsight_).

1. Enter a globally unique name for the cluster.

1. Click **Select Cluster Type** to configure the cluster.
	1. Choose **Hadoop** in the Cluster Type drop down. 

		HDInsight allows to deploy a variety of cluster types, for different data analytics workloads. Cluster types offered are:
	
		- **_Hadoop**: for query and analysis workloads_
		- **HBase**: for NoSQL workloads
		- **Storm**: for real time event processing workloads
		- **Spark**: for in-memory processing, interactive queries, stream, and machines learning workloads.

	1. Select **linux** as the _operating system_ for the cluster (you can choose between either Windows or Linux).

	1. Select **the latest version** ("Hadoop 2.7.1 (HDI 3.4)" or latest) from the Version drop-down.

	1. Select a **Standard Cluster Tier**.

	1. Click **Select** at the bottom of the Cluster Type configuration blade.


1. Click **Credentials** to configure the cluster login credentials
	1.  Enter an admin password in the **Cluster Login Password** box.  
	1.  Enter an SSH username.
	1.  Enter an SSH password. 
	1.  Click **Select** to save changes to the credential settings.

1. Click **Data Source** to configure the storage container. 
 	1. Click **Select storage account** and select the storage account created in the previous section.
	1. Type "**hdi**" for the default container name.
 	1. Verify the **Location** is set correctly.  
	1. Click **Select** at the bottom of the Data source blade.

1. Click **Pricing**.  
	1. Enter 1 for the worker nodes number. 
	1. Click **Select** at the bottom of the blade.

1. To configure the **Resource Group**.  Click **Use existing** and type or select the Resource Group you created with the storage account in the previous section.

1. Click **Pin to dashboard** so the cluster is easy to access during the labs. 

1. Click **Create**. The provisioning may take 20 minutes to complete.

	![Create HDI cluster](Images/setup-create-hdi.png?raw=true "Create HDI cluster")

	_Create HDI cluster_

> **Note:** For this module we don't need to persist the cluster when deleted, so we won't configure a Hive and Oozie metastore. 

> Using the metastore helps you to retain your Hive and Oozie metadata, so that you don't need to re-create Hive tables or Oozie jobs when you create a new cluster. By default, Hive uses an embedded Azure SQL database to store this information. The embedded database can't preserve the metadata when the cluster is deleted.

> In order to configure Oozie or Hive metadata Database, first you need to create the SQL Database. To do this, navigate to _New > Data + Storage > SQL Database_ and create a _blank database_ in the same resource group that you want to use for the HDI cluster.

> ![Create a SQL Database](Images/creating-the-sql-database.png?raw=true "Create a SQL Database")

> _Create a SQL Database_

> Then, select _Optional Configuration_ in the _New HDInsight Cluster_. Select _External Metastores_ in the _Optional Configuration_ blade and choose to use an existing SQL Database, selecting the one you created before.

> ![External Metastores configuration](Images/external-metastores-configuration.png?raw=true "External Metastores configuration")

> _External Metastores configuration_

<a name="ManualSetupSqlDW"></a>
#### Manual Setup 3: Manually create the Azure SQL Data Warehouse ####

1. In the [Microsoft Azure portal](https://portal.azure.com/), create a new Azure SQL Data Warehouse (_New > Data + Storage > SQL Data Warehouse_).

1. Enter "**partsunlimited**" for the database name.

1. Select **Use existing** resource group and type or select the resource group created in the earlier section.

1. Use **Blank database** for the source so a new empty database is used (it will use the "partsunlimited" name).

1. Click **Server** to create a new SQL Server for hosting the Data Warehouse.
	1. Select **Create a new server** in the Server blade.
	1. Enter a globally unique name for the server.
	1. Enter a admin login name: **dwadmin**
	1. Write a password for the server login: **P@ssword123**
	1. Repeat the password to confirm it.
	1. Select the same location used for the other services.

1. Using the slider, choose the minimum performance of 100 DWU. This will be enough compute for this module.

1. Click **Pin to dashboard** so the data warehouse is easily accessible for the rest of the labs. 

	![Create SQL Data Warehouse](Images/setup-create-dw2.png?raw=true "SQL Data Warehouse")

	_SQL Data Warehouse_

1. Click **Create**.  The new SQL Server and Azure SQL Data Warehouse will be available in 3-5 minutes. 

1. Once created, open the _SQL Data Warehouse_ blade. To open the SQL Data Warehouse blade navigate Browse > SQL Databases and select the **partsunlimited** database. 

1. Click on the server name to open the Sql Server settings and then click **Firewall**.

1. Click on **Add client IP** to add a rule to enable access for your machine.

	![Firewall config](Images/setup-dw-firewall2.png?raw=true "Firewall config")

	_Firewall config_

1. Click **Save** at the top of the Firewall settings blade.

<a name="ManualSetupSqlDWScript"></a>
#### Manual Setup 4: Verify connectivity to SQL Data Warehouse ####

Once the SQL Data Warehouse is created and the firewall rules are updated, validate connectivity with Visual Studio.

1. Navigate to the new SQL Data Warehouse you created.

1. In the _SQL Data Warehouse_ blade, click **Open in Visual Studio**. In the new blade, click **Open in Visual Studio**.

	![Open Data Warehouse in Visual Studio](Images/setup-open-vs.png?raw=true "Open Data Warehouse in Visual Studio")

	_Open Data Warehouse in Visual Studio_

1. Confirm you want to switch apps if prompted.

1. In Visual Studio, enter the SQL Server credentials (dwadmin/P@ssword123) and click Connect. 

1. In the left panel _SQL Server Object Explorer_, expand the server and right-click the **partsunlimited** database.   Select **New Query...**.

1. In the new query window add the following script to verify setup completed correctly.

	````SQL
	SELECT @@version
	GO
	````

1. Click the execute button (**Ctrl+Shift+E**) to run the query.

	![Validating connectivity](Images/setup-run-sql2.png?raw=true "Validating connectivity")

	_Validating connectivity_

1. Leave Visual Studio open.  You will execute more statements later in the lab. 


> **Note:** to clean your subscription just delete the resource group you created for all the associated resources.

Now you will start executing the exercises.

---

<a name="Exercises"></a>
## Exercises ##
This module includes the following exercises:

1. [Creating Hive code to do analytics on your data](#Exercise1)
1. [Loading and querying data in Azure SQL Data Warehouse](#Exercise2)
1. [Creating Azure Data Factory](#Exercise3)
1. [Orchestrating Azure Data Factory workflow](#Exercise4)
1. [Using the data in the warehouse to generate Power BI visualizations](#Exercise5)

Estimated time to complete this module: **60 minutes**

<a name="Exercise1"></a>
### Exercise 1: Creating Hive code to do analytics on your data ###

**HDInsight** is a cloud implementation on **Microsoft Azure** of the rapidly expanding **Apache Hadoop** technology stack that is the go-to solution for big data analysis.

**Apache Hive** is a data warehouse system for Hadoop, which enables data summarization, querying, and analysis of data by using **HiveQL** (a query language similar to SQL). Hive can be used to interactively explore your data or to create reusable batch processing jobs.

This exercise is meant to be an extension of Module 1. We will be using the data that was spooled by Azure Stream Analytics to Azure Blob Storage to perform our queries. However, to maintain the modularity 
In this exercise, you'll create a **Hive activity** using HQL script code to generate the output data in the Azure Storage.

<a name="Ex1Task1"></a>
#### Task 1 - Writing a Hive query to analyze the logs ####

In this task, you'll write a Hive query to generate product stats (views and cart additions) from the Parts Unlimited logs.

1. Navigate to the HDInsight server dashboard and click on the URL: https://**{clusterName}**.azurehdinsight.net/

1. You will be directed to the Ambari portal. Once there, follow the steps below:

	1. Select the **Hive view** in the views button at the top bar.

		![Hive view option in Ambari](Images/ex2task1-ambari-hive-option.png?raw=true "Hive view option in Ambari")

		_Hive view option in Ambari_

	1. In the new worksheet, write HQL code to create a partitioned table for the existing blobs and then parse the JSON format. Paste the snippet below, replace  **<****StorageAccountName****>** and update the partition to be of a valid **date** and **location** (where the input logs were uploaded according to the current date):

		````SQL
		CREATE EXTERNAL TABLE IF NOT EXISTS LogsRaw (jsonentry string) 
		PARTITIONED BY (year INT, month INT, day INT)
		STORED AS TEXTFILE LOCATION "wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/";

		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=06) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/06';
		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=07) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/07';
		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=08) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/08';
		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=09) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/09';

		````

		> **Note**: The CREATE EXTERNAL TABLE command that we used here, creates an external table, the data file can be located outside the default container and does not move the data file.

		![Hive editor](Images/ex2task1-ambari-hive-editor2.png?raw=true "Hive editor")

		_Hive editor_

	1. Click **Execute** to run the query.

	1. Wait for the status to be Succeeded, select the results tab in the "Query Process Results" panel to show the values.

		![Query output](Images/ex2task1-ambari-query-output.png?raw=true "Query output")

		_Query output_

	> **Note:** Ambari portal offers many features. You can create advanced visualization of the data results using charts, customize the Hive settings, create customized views, among many other options. To learn more about Ambari portal go to [Manage HDInsight clusters by using the Ambari Web UI](https://azure.microsoft.com/documentation/articles/hdinsight-hadoop-manage-ambari/).

1. Write a new Hive query to create the output partitioned table using a columns schema matching the output of the previous query. Paste the snippet below and replace the **StorageAccountName** placeholder. 

	````SQL
	CREATE TABLE IF NOT EXISTS websiteActivity (
		eventdate string,
		userid string,
		type string,
		productid string,
		quantity int,
		price double
	) PARTITIONED BY (year int, month int, day int) 
	ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
	STORED AS TEXTFILE LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs';
	````


1. Let's also go ahead and add a partition on the data. You'll notice that we're also creating a partition for a specific date. This will later be made dynamic using **hiveconf** variable. Paste the snippet below and replace the **StorageAccountName** placeholder. 

	````SQL
	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=06) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/06';
	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=07) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/07';
	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=08) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/08';
	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=09) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/09';
	````

1. **Submit** or **execute** the Hive query to create the output table.

<a name="Ex1Task2"></a>
#### Task 2 - Create the HQL for your data ####

In this task, we'll create our hive scripts to process out data. This is used to demonstrate the ability of Hadoop to parse nested JSONs easily.

1. Open the file located in **Setup\Assets\HDInsight\Scripts\2_structuredlogs.hql** and review its content:

	````SQL
	INSERT OVERWRITE TABLE websiteActivity Partition (year, month, day)
	SELECT CAST(CONCAT(split(get_json_object(jsonentry, "$.eventDate"),'T')[0], ' ', SUBSTRING(split(get_json_object(jsonentry, "$.eventDate"),'T')[1],0,LENGTH(split(get_json_object(jsonentry, "$.eventDate"),'T')[1])-1)) as TIMESTAMP) as eventdate,
		 get_json_object(jsonentry, "$.userId") as userid,
         get_json_object(jsonentry, "$.type") as type,
         get_json_object(jsonentry, "$.productId") as productid,
		 CAST(get_json_object(jsonentry, "$.quantity") as int) as quantity,
         CAST(get_json_object(jsonentry, "$.price") as DOUBLE) as price,
		 year, month, day
	FROM LogsRaw
	WHERE year=${hiveconf:Year} and month=${hiveconf:Month} and day=${hiveconf:Day};
	````

    Notice this script is, essentially, the Hive query you wrote and tested in the previous task but it insert the results in the output table.

1. Let's go ahead and execute this query. Replace the **${hiveconf:Year}**,**${hiveconf:Month}**,**${hiveconf:Day}** with the current year, month and day (eg: 2016/10/06). However, do not save the updated files as we will need the hive variables for our Data Factory automation. 

1. For your convenience, here's a date replaced version of the query above. Also, since we're doing a one time load, we've removed the 'Where' clause as well.

	````SQL
	INSERT OVERWRITE TABLE websiteActivity Partition (year, month, day)
	SELECT CAST(CONCAT(split(get_json_object(jsonentry, "$.eventDate"),'T')[0], ' ', SUBSTRING(split(get_json_object(jsonentry, "$.eventDate"),'T')[1],0,LENGTH(split(get_json_object(jsonentry, "$.eventDate"),'T')[1])-1)) as TIMESTAMP) as eventdate,
		 get_json_object(jsonentry, "$.userId") as userid,
         get_json_object(jsonentry, "$.type") as type,
         get_json_object(jsonentry, "$.productId") as productid,
		 CAST(get_json_object(jsonentry, "$.quantity") as int) as quantity,
         CAST(get_json_object(jsonentry, "$.price") as DOUBLE) as price,
		 year, month, day
	FROM LogsRaw;
	````


1. Open the file located in **Setup\Assets\HDInsight\Scripts\3_addpartitions.hql** and review its content:

	````SQL

	ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year}, month=${hiveconf:Month}, day=${hiveconf:Day}) LOCATION 'wasb://partsunlimited@${hiveconf:StorageAccountName}.blob.core.windows.net/logs/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}';

	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=${hiveconf:Year}, month=${hiveconf:Month}, day=${hiveconf:Day}) LOCATION 'wasb://processeddata@${hiveconf:StorageAccountName}.blob.core.windows.net/structuredlogs/${hiveconf:Year}/${hiveconf:Month}/${hiveconf:Day}';
	````

    This script adds the partitiones by date to the input and output tables. The storage account name and all the required date components for the partitiones will be passed as parameters by the Hive action running in the Data Factory.


1. Open the file located in **Setup\Assets\HDInsight\Scripts\4_productcatalog.hql**,  Review the content and execute in the Ambari query tool:

	````SQL
	DROP TABLE IF EXISTS RawProductCatalog;
	CREATE EXTERNAL TABLE RawProductCatalog (
		jsonentry string
	) STORED AS TEXTFILE LOCATION "wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/productcatalog/";
	

	DROP TABLE IF EXISTS ProductCatalog;
	CREATE TABLE ProductCatalog ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
	LOCATION 'wasbs://processeddata@<StorageAccountName>.blob.core.windows.net/product_catalog/'
	AS SELECT get_json_object(jsonentry, "$.skuNumber") as skuNumber,
			  get_json_object(jsonentry, "$.id") as id,
			  get_json_object(jsonentry, "$.productId") as productId,
			  get_json_object(jsonentry, "$.categoryId") as categoryId,
			  get_json_object(jsonentry, "$.category.name") as categoryName,
			  get_json_object(jsonentry, "$.title") as title,
			  get_json_object(jsonentry, "$.price") as price,
			  get_json_object(jsonentry, "$.salePrice") as salePrice,
			  get_json_object(jsonentry, "$.costPrice") as costPrice
	FROM RawProductCatalog;
	````


<a name="Ex1Task3"></a>
#### Task 3 - Create the HQL queries to process the raw data ####
1. First, let's start by getting used to the simple HiveQL language. Let's run a query to compute the top-selling products for the day.

	````SQL
	select count(*) from websiteActivity;

	select productid, SUM(quantity) as qty from websiteActivity 
	WHERE eventdate < from_unixtime(unix_timestamp())
	AND type='checkout'
	GROUP BY productid
	ORDER BY qty DESC;
	````

1. We can see how easy it is to run a SQL-like query using HiveQL. We can easily join this data to the product catalog data and understand our top selling categories as well. The HiveQL query below does exactly that.

	````SQL
	SELECT a.productid, b.title, b.categoryName, SUM(a.quantity)
	FROM websiteActivity a LEFT OUTER JOIN ProductCatalog b
	ON a.productid = b.productid
	WHERE eventdate < from_unixtime(unix_timestamp())
	AND type='checkout'
	GROUP BY a.productid, b.title, b.categoryName;
	````

1. Next, we'll be processing some data using HiveQL. In this scenario, we will process our log data and understand which products get bought together. This will help us understand our audience a little better and come up with better marketing strategies for our e-commerce store. This is used to highlight the ease and ability of a NoSQL ETL engine like Hadoop to work with Arrays within tabular formatted data.  The code can be found in **Setup\Assets\HDInsight\Scripts\5_relatedproducts.hql**

	````SQL
	DROP VIEW IF EXISTS unique_purchases;
	CREATE VIEW unique_purchases AS 
	SELECT distinct userid, productid
	FROM websiteActivity where eventdate > date_sub(from_unixtime(unix_timestamp()),30)
	AND type = 'checkout';

	DROP VIEW IF EXISTS all_purchased_products;
	CREATE VIEW all_purchased_products AS 
	SELECT a.userid, COLLECT_LIST(CONCAT(a.productid,',',a.qty)) as product_list from (
	  	 SELECT userid, productid, sum(quantity) as qty FROM websiteActivity
	   	 WHERE eventdate > date_sub(from_unixtime(unix_timestamp()),30)
	   	 AND type = 'checkout'
	   	 GROUP BY userid, productid
	   	 ORDER BY userid ASC, qty DESC) a
	GROUP BY a.userid;

	DROP VIEW IF EXISTS related_purchase_list;
	CREATE VIEW related_purchase_list AS
	SELECT a.userid, a.productid, b.product_list
	FROM unique_purchases a LEFT OUTER JOIN all_purchased_products b ON (a.userid = b.userid);

	DROP TABLE IF EXISTS related_products;
	CREATE TABLE related_products ROW FORMAT DELIMITED FIELDS TERMINATED BY '|' LINES TERMINATED BY '\n' LOCATION 'wasbs://processeddata@<StorageAccountName>.blob.core.windows.net/related_products/' AS 
	SELECT c.productid, c.related_product, c.qty, rank() OVER (PARTITION BY c.productid ORDER BY c.qty DESC) as rank FROM
	(SELECT a.productid, a.related_product, SUM(a.quantity) as qty FROM
	(SELECT b.productid, SPLIT(prod_list, ',')[0] as related_product, CAST(SPLIT(prod_list, ',')[1] as INT) as quantity
	FROM related_purchase_list b LATERAL VIEW EXPLODE(b.product_list) prodList as prod_list) a
	WHERE a.productid <> a.related_product
	GROUP BY a.productid, a.related_product
	ORDER BY a.productid ASC, qty DESC) c;
	````

1. Now that we have the related purchases, we can use this information to power recommendations on our e-commerce website. We can move the output of this table to a transactional data store, where our web app can pick up the latest data to power the recommendations.


1. These scripts were already uploaded to your storage during the module setup by using the manual steps or the **Setup.cmd** script. Verify the HQL script was uploaded by using the **Azure Storage Explorer** to navigate to the **Scripts** folder in the **partsunlimited** container.

	1. In the left pane navigate to the **partsunlimited** container, open the **Scripts** folder and verify the files are present.

		![Verify HQL script file is in place](Images/ex2task2-explorer-scripts.png?raw=true "Verify HQL script file is in place")

		_Verify HQL script file is in place_



<a name="Exercise2"></a>
### Exercise 2: Loading and querying data in Azure SQL Data Warehouse ###
The fastest way to import data into SQL Data Warehouse is to use **PolyBase** to load data from Azure blob storage. PolyBase uses SQL Data Warehouse's massively parallel processing (MPP) design to load data in parallel from Azure blob storage. To use PolyBase, you can use T-SQL commands or an Azure Data Factory pipeline.  This exercise will use a T-SQL command.  Later in the lab you will write an Azure Data Factory pipeline to load data with PolyBase. 

In this exercise you'll create an **External Table** using PolyBase. You will build a new SQL Data Warehouse table using the Create Table As Select (CTAS) syntax and issue a few sample queries. 

Once you have had some experience using PolyBase and querying Azure SQL Data Warehouse, you create the tables and stored procedures that will be used with Azure Data Factory later in the lab.

<a name="Ex2Task1"></a>
#### Task 1 - Connect to Azure SQL Data Warehouse using Visual Studio 2015 ####

1. Return to the Azure Portal and navigate to the new SQL Data Warehouse you created.

1. In the _SQL Data Warehouse_ blade, click **Open in Visual Studio**. In the new blade, click **Open in Visual Studio**.

	![Open Data Warehouse in Visual Studio](Images/setup-open-vs.png?raw=true "Open Data Warehouse in Visual Studio")

	_Open Data Warehouse in Visual Studio_

1. Confirm you want to switch apps if prompted.

1. In Visual Studio, enter the SQL Server credentials (dwadmin/P@ssword123).

1. In the _SQL Server Object Explorer_, expand the server and right-click the **partsunlimited** database.   Select **New Query...**.

You have now connected to your Azure SQL Data Warehouse and are ready to begin building and loading tables.  

<a name="Ex2Task2"></a>
#### Task 2 - Configure Connectivity with Azure Blob Storage ####
PolyBase uses T-SQL external objects to define the location and attributes of the external data. The external object definitions are stored in SQL Data Warehouse. The data itself is stored externally in Azure Blob Storage.

All scripts for this exercise are available in the folder Module2-GainInsights\Setup\Assets\ADW\Scripts\.

1. Execute the following SQL statement in the new query window. Replace the _Azure storage key_ placeholder with your Azure storage account key. The script will create a Master Key if it doesn't already exist and create a new Database Scoped Credential. The Master Key is needed to encrypt the database scoped credentials that will connect to your storage account. 

	````SQL
	IF NOT EXISTS (SELECT * FROM sys.symmetric_keys where name = '##MS_DatabaseMasterKey##')
	CREATE MASTER KEY;

	CREATE DATABASE SCOPED CREDENTIAL AzureStorageCredential
	WITH
    IDENTITY = 'user',
    SECRET = '<Azure storage key>';
	````

1. Execute the following SQL statement in the new query window to create an external data source. Replace the _Azure storage account name_ placeholder with your Azure storage account name.  Note that PolyBase uses Hadoop APIs to access data in Azure blob storage.  

	````SQL
	CREATE EXTERNAL DATA SOURCE AzureStorage
	WITH (
	    TYPE = HADOOP,
	    LOCATION = 'wasbs://processeddata@<Azure storage account name>.blob.core.windows.net',
	    CREDENTIAL = AzureStorageCredential
	);
	````

1. Once you have set up connectivity, open a new query window and execute the following SQL statement to define the data format.  

	````SQL
	CREATE EXTERNAL FILE FORMAT TextFile
	WITH (
	    FORMAT_TYPE = DelimitedText,
	    FORMAT_OPTIONS (FIELD_TERMINATOR = ',')
	);
	````

<a name="Ex2Task3"></a>
#### Task 3 - Set up the external table ####
An external table contains column names and types, just like any other database table. It simply doesn't contain the data.

Now let's create the external tables. All we are doing here is defining column names and data types, and binding them to the location and format of the Azure blob storage files. The location is the folder under the root directory of the Azure Storage Blob.

1. We will create a separate schema for our external tables.

	````SQL
	CREATE SCHEMA [asb]
	GO
	````

1. Open a query window and execute the following SQL to create the external table.  Notice the WITH clause is using the data source and file format created in the previous task.

	````SQL
	CREATE EXTERNAL TABLE asb.WebsiteActivityExternal
	(
		EventDate datetime2,
		UserId nvarchar(20),
		Type nvarchar(20),
		ProductId nvarchar(20), 
		Quantity int, 
		Price float
	)
	WITH (
	    LOCATION='/structuredlogs/2016/10/',
	    DATA_SOURCE=AzureStorage,
	    FILE_FORMAT=TextFile
	);

	````

1. Let's create another table to load the Product Catalog into SQL DW so that we can perform some meaningful analytics like understanding our most profitable products the last 30 days, so that we can promote them further.

	````SQL
	CREATE EXTERNAL TABLE asb.ProductCatalogExternal
	(
		SkuNumber nvarchar(50),
		Id int,
		ProductId nvarchar(20),
		CategoryId nvarchar(20),
		CategoryName nvarchar(100),
		Title nvarchar(100),
		Price float,
		SalePrice float,
		CostPrice float
	)
	WITH (
	    LOCATION='/product_catalog/',
	    DATA_SOURCE=AzureStorage,
	    FILE_FORMAT=TextFile
	);

	````

1. Now that the table has been created, let's run a couple of sample queries to get used to the SQL DW environment. Notice how you query the external table as you would any other table in the database.

	````SQL
	SELECT COUNT(*) FROM asb.WebsiteActivityExternal;

	SELECT
		ProductId,
		SUM(CASE WHEN Type = 'view' THEN Quantity ELSE 0 END) AS ProdViews,
		SUM(CASE WHEN Type = 'add' THEN Quantity ELSE 0 END) AS ProdAdds
	FROM asb.WebsiteActivityExternal
	GROUP BY ProductId;

	````


<a name="Ex2Task4"></a>
#### Task 4 - Loading Data with CTAS ####
You can query data using the external tables, but often you will want to load the data in Azure Data Warehouse where you can optimize the table for query and analysis.  

The easiest and most efficient way to load data from Azure blob storage is to use [CREATE TABLE AS SELECT]. Loading with CTAS leverages the strongly typed external tables you have just created.

1. Open a new query window and execute the following to create a new schema for our internally managed tables. 

	````SQL
	CREATE SCHEMA [adw]
	GO

	````

1. Execute the following in a query window to create a new partitioned table.  Note this table will be created based on a SELECT statement issued on the external table. 

	````SQL
	CREATE TABLE adw.FactWebsiteActivity
	WITH (
		CLUSTERED COLUMNSTORE INDEX,
		DISTRIBUTION = HASH(ProductID)
		)
	AS
	SELECT
		EventDate,
		UserId,
		Type,
		ProductId, 
		Quantity, 
		Price
	FROM asb.WebsiteActivityExternal
	GO
	````

1. Similarly, let's pull our latest and greatest product catalog data from blob storage.

	````SQL
	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'adw' AND TABLE_NAME = 'DimProductCatalog')
		DROP TABLE adw.DimProductCatalog
	GO

	CREATE TABLE adw.DimProductCatalog
	WITH (
		CLUSTERED COLUMNSTORE INDEX,
		DISTRIBUTION = HASH(ProductID)
		)
	AS 
	SELECT
		skuNumber,
		id,
		productId,
		categoryId,
		categoryName,
		title,
		price,
		salePrice,
		costPrice
	FROM asb.ProductCatalogExternal
	GO
	````


1. Azure SQL Data Warehouse does not automatically manage your statistics.  It is a good practice to create single column statistics on your internally managed tables immediately after a load. There are some choices for statistics. For example, if you create single-column statistics on every column it might take a long time to rebuild statistics. If you know certain columns are not going to be in query predicates, you could skip creating statistics on those columns. For instance, you'll notice that we have not done statistics on the 'UserID' column, because we will not be using it for our query.

	````SQL	
	CREATE STATISTICS Stat_adw_FactWebsiteActivity_EventDate on adw.FactWebsiteActivity(EventDate);
	CREATE STATISTICS Stat_adw_FactWebsiteActivity_Type on adw.FactWebsiteActivity([Type]);
	CREATE STATISTICS Stat_adw_FactWebsiteActivity_ProductID on adw.FactWebsiteActivity(ProductID);
	CREATE STATISTICS Stat_adw_FactWebsiteActivity_Quantity on adw.FactWebsiteActivity(Quantity);
	CREATE STATISTICS Stat_adw_FactWebsiteActivity_Price on adw.FactWebsiteActivity(Price);
	````

<a name="Ex2Task5"></a>
#### Task 5 - Create a new summary table for reporting ####

Before we move to the next exercise, create a stored procedure to understand our most profitable products that will be used for reporting and analysis.

1. Execute the following statement to create the summary table and table statistics.

	````SQL	

	IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'asp_populate_productlogsummary')
		DROP PROCEDURE adw.asp_populate_productlogsummary
	GO

	CREATE PROCEDURE adw.asp_populate_ProfitableProducts AS
		IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'adw' AND TABLE_NAME = 'ProfitableProducts')
			DROP TABLE adw.ProfitableProducts

		CREATE TABLE adw.ProfitableProducts
		WITH
		(   
			CLUSTERED COLUMNSTORE INDEX,
			DISTRIBUTION = ROUND_ROBIN
		)
		AS
		SELECT 
			a.ProductId, 
			b.CategoryName,
			SUM(a.Price - (b.CostPrice*a.Quantity)) as Profit
		FROM adw.FactWebsiteActivity AS a LEFT OUTER JOIN adw.DimProductCatalog AS b
			ON a.ProductId = b.ProductId
		WHERE  DATEDIFF(day, a.eventdate, GetDate()) < 30
		AND a.[Type]='checkout'
		GROUP BY a.ProductId, b.CategoryName;

	
		CREATE STATISTICS Stat_adw_ProfitableProducts_ProductId on adw.ProfitableProducts(ProductId);
		CREATE STATISTICS Stat_adw_ProfitableProducts_CategoryName on adw.ProfitableProducts(CategoryName);
		CREATE STATISTICS Stat_adw_ProfitableProducts_Profit on adw.ProfitableProducts(Profit);
	GO
	````

1. Click execute button (**Ctrl+Shift+E**) to run the statement and create the stored procedure.

	![Creating schema for Data Warehouse](Images/ex2task5-create_proc.png?raw=true "Creating schema for Data Warehouse")

	_Creating schema for Data Warehouse_

1. Execute the stored procedure to create the table.  Issue a select statement on the ProfitableProducts table to verify the procedure executed correctly.

	````SQL	

	exec adw.asp_populate_ProfitableProducts; 

	SELECT * FROM adw.ProfitableProducts;

	````
	

<a name="Exercise3"></a>
### Exercise 3: Creating Azure Data Factory ###

The next exercise will walk you through automating a data pipeline using Azure Data Factory.  At the end of this section you will have a data pipeline that will process data with HDInsight, move data from HDInsight to Azure Data Warehouse, and execute a stored procedure on Azure Data Warehouse to populate a summary table on a daily schedule.  This is a canonical workload whereby you will use HDInsight for large batch processing and move data to a relational structure where it can be readily consumed by end user reporting and analysis tools. You will also be introduced to the Monitoring tool for Azure Data Factory. 

All JSON assets to create the Data Factory objects are available in _Module2-GainInsights\Setup\Assets\DataFactory_.

_Data factory key concepts_

Azure Data Factory has a few key entities that work together to define the input and output data, processing events, and the schedule and resources required to execute the desired data flow.

- **Activities**: Activities define the actions to perform on your data. Each activity takes zero or more datasets as inputs and produces one or more datasets as outputs.
- **Pipelines**: Pipelines are a logical grouping of Activities. They are used to group activities into a unit that together perform a task (i.e. clean log).
- **Datasets**: Datasets are named references/pointers to the data you want to use as an input or an output of an Activity. Datasets identify data structures within different data stores including tables, files, folders, and documents.
- **Linked service**: Linked services define the information needed for Data Factory to connect to external resources (data stores or compute resources for hosting activities execution).


![Data factory key concepts](Images/data-factory-exercises.png?raw=true "Data factory key concepts")

> **Note:** If you need to skip the manual instructions to create the data factory and provisioning of the linked services, datasets and pipelines. You can run the CreateDataFactory.cmd script to quickly provision it.


<a name="Ex3Task1"></a>
#### Task 1 - Creating the Data Factory in the Azure Portal ####

In this task, you create a data factory to orchestrate the linked services and data transformation activities.

1. On the Hub menu, select **New -> Data + Analytics -> Data Factory**.

1. Use the following values to create the new data factory:

 1. Enter a name for the data factory.  
 1. Click **Use existing** under Resource group and select the resource group you created at the start of the lab.  
 1. Make sure the **Pin to dashboard** is selected so you can have a quick access and click **Create**.

	![New data factory blade](Images/ex1task2-new-data-factory.png?raw=true "New data factory blade")

	_New data factory blade_

1. After the creation is complete, you'll find the data factory in the dashboard.

<a name="Ex3Task2"></a>
#### Task 2 - Adding the linked services ####

In this task, you'll create the linked services.  Linked Services are essentially connections to data sources and destinations. At the end of this task you will have created the following linked services:
- Azure Storage
- HDInsight
- Azure Data Warehouse


>**Note:** It is recommended that you open a new tab and navigate to the Azure portal and use this tab to copy the keys/server names needed to create the linked services in Data Factory.


1. In the *Data Factory* blade, click **Author and deploy** tile to launch the *Editor* for the data factory.

	![Author and deploy](Images/ex1task2-author-and-deploy.png?raw=true "Author and deploy")

	_Author and deploy_

1. In the **Editor**, click **New data store** button on the toolbar and select **Azure storage** from the drop down menu. You should see the JSON template for creating an Azure storage linked service in the right pane.

	![New Azure storage data store](Images/ex1task2-new-storage.png?raw=true "New Azure storage data store")

	_New Azure storage data store_

1. Replace **<****accountname****>** and **<****accountkey****>** placeholders with the account name and key values of the Azure storage account where the Parts Unlimited logs are stored (created with Setup.cmd script or with manual steps).

	````JavaScript
	{
		 "name": "AzureStorageLinkedService",
		 "properties": {
			  "type": "AzureStorage",
			  "description": "",
			  "typeProperties": {
					"connectionString": "DefaultEndpointsProtocol=https;AccountName=<accountname>;AccountKey=<accountkey>"
			  }
		 }
	}
	````

    See [JSON Scripting Reference](http://go.microsoft.com/fwlink/?linkid=516971&clcid=0x409) for details about JSON properties.

1. Click **Deploy** on the toolbar to deploy the **StorageLinkedService**.

	![Deploy linked service](Images/ex1task2-deploy.png?raw=true "Deploy linked service")

	_Deploy linked service_

1. Confirm that you see the message *Linked service deployed successfully* on the title bar.

	![Linked service deployed successfully](Images/ex1task2-deployed.png?raw=true "Linked service deployed successfully")

	_Linked service deployed successfully_

1. Repeat to create another service for the SQL Data Warehouse server. Click **New data store** button on the toolbar and select **Azure SQL Data Warehouse** from the drop down menu.

1. Replace placeholders in the connectionString (**<****servername****>**, **<****databasename****>**, **<****username****>** and **<****password****>**) with the setting values used when creating the SQL Data Warehouse database in setup script.

	````JavaScript
	{
		 "name": "AzureSqlDWLinkedService",
		 "properties": {
			  "type": "AzureSqlDW",
			  "description": "",
			  "typeProperties": {
					"connectionString": "Data Source=tcp:<servername>.database.windows.net,1433;Initial Catalog=partsunlimited;User ID=dwadmin@<servername>;Password=P@ssword123;Integrated Security=False;Encrypt=True;Connect Timeout=30"
			  }
		 }
	}
	````

1. Click **Deploy** on the toolbar to create and deploy the **AzureSqlDWLinkedService**.

1. Now, click **New compute** button on the toolbar and select **HDInsight cluster** from the drop down menu. You should see the JSON template for creating the *HDInsight cluster linked service* in the right pane.

    > **Note**: Alternatively, you can use a [On Demand HDInsight cluster](https://azure.microsoft.com/en-us/documentation/articles/data-factory-compute-linked-services/#azure-hdinsight-on-demand-linked-service), but it will take around 15 minutes to create the cluster when running the Data Factory. Use this approach only if the provided cluster is not available and make sure to specify the proper password.

1. Update the setting placeholders as shown in the snippet below. Replace the **<****hdiclustername****>** placeholder with the name you used when running the **Setup.cmd** script or with manual steps.

	````JavaScript
	{
        "name": "HDInsightLinkedService",
        "properties": {
            "type": "HDInsight",
            "description": "",
            "typeProperties": {
                "clusterUri": "https://<hdiclustername>.azurehdinsight.net/",
                "userName": "admin",
                "password": "P@ssword123",
                "linkedServiceName": "AzureStorageLinkedService"
            }
        }
    }
	````

1. Click **Deploy** on the toolbar to create and deploy the **HDInsightLinkedService**.

	![Linked services created](Images/ex1task2-linked-services.png?raw=true "Linked services created")

	_Linked services created_

    > **Note:** if the deployment fails, it may be due to the HDI cluster is still being provisioned. Verify the status in the portal.

<a name="Ex3Task3"></a>
#### Task 3 - Creating the input and output datasets ####

In this task, you'll create the input and output tables corresponding to the linked services.

1. In the Editor for the Data Factory, click **New dataset** button on the toolbar and click **Azure Blob storage** from the drop down menu.

1. Change the **name** propety to "RawJsonData" and the **linkedServiceName** property to "AzureStorageLinkedService".

1. Remove the **structure** property. The file is in JSON format and the structure can be omitted.

1. Change the **typeProperties** to use the _/year/month/day_ partition for the blob paths.

	````JavaScript
	"typeProperties": {
		"folderPath": "partsunlimited/logs/{Year}/{Month}/{Day}",
		"partitionedBy": 
		[
			 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
			 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
			 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
		],
		"format": {
			"type": "TextFormat"
		}
	},
	````

1. Set the availability frequency to daily.

	````JavaScript
	"availability": {
		"frequency": "Day",
		"interval": 1
	},
	````

1. Add an **external** property to the dataset and set it to **true**. This setting informs the Data Factory service that this table is external to the data factory and not produced by an activity in the data factory.

	````JavaScript
	"external": true
	````

    The final _RawJsonData_ dataset should look like the following snippet:
    
	````JavaScript
	{
		"name": "RawJsonData",
		"properties": {
			"type": "AzureBlob",
			"linkedServiceName": "AzureStorageLinkedService",
			"typeProperties": {
				"folderPath": "partsunlimited/logs/{Year}/{Month}/{Day}",
				"partitionedBy": 
				[
					 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
					 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
					 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
				],
				"format": {
					"type": "JsonFormat"
				}
			},
			"availability": {
				"frequency": "Day",
				"interval": 1
			},
			"external": true
		}
	}
	````

1. Click **Deploy** on the toolbar to create the input dataset.

1. Repeat the steps to create another dataset for output with the following settings:

 1. **name**: "WebsiteActivityBlob"
 1. **linkedServiceName**: "AzureStorageLinkedService"
 1. structure (this file is structured using CSV format with the fields specified below):

		````JavaScript
		"structure": [
			{
				"name": "eventdate",
				"type": "DateTime"	
			},
			{
				"name": "userid",
				"type": "String"
			},
			{
				"name": "type",
				"type": "String"
			},
			{
				"name": "productid",
				"type": "String"
			},
			{
				"name": "quantity",
				"type": "Int32"
			},
			{
				"name": "price",
				"type": "Double"
			}
		],
		````

 1. typeProperties (the format will use the defaults for CSV: comma -,- for columns delimiter and line breaks -\n- for row delimiter):

	````JavaScript
		"typeProperties": {
			"folderPath": "processeddata/structuredlogs/{Year}/{Month}/{Day}",
			"partitionedBy": 
				[
					 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
					 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
					 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
				],
			"format": {
				"type": "TextFormat"
			}
		},
	````

 1. Daily availability:

	````JavaScript
		"availability": {
			"frequency": "Day",
			"interval": 1
		}
	````

 1. Make sure the final _WebsiteActivityBlob_ dataset looks like the following snippet:
 
	````JavaScript
		{
			"name": "WebsiteActivityBlob",
			"properties": {
				"type": "AzureBlob",
				"linkedServiceName": "AzureStorageLinkedService",
				"structure": [
					{
						"name": "eventdate",
						"type": "DateTime"
					},
					{
						"name": "userid",
						"type": "String"
					},
					{
						"name": "type",
						"type": "String"
					},
					{
						"name": "productid",
						"type": "String"
					},
					{
						"name": "quantity",
						"type": "Int32"
					},
					{
						"name": "price",
						"type": "Double"
					}
				],
				"typeProperties": {
					"folderPath": "processeddata/structuredlogs/{Year}/{Month}/{Day}",
					"partitionedBy": 
					[
					 { "name": "Year", "value": { "type": "DateTime", "date": "SliceStart", "format": "yyyy" } },
					 { "name": "Month", "value": { "type": "DateTime", "date": "SliceStart", "format": "MM" } }, 
					 { "name": "Day", "value": { "type": "DateTime", "date": "SliceStart", "format": "dd" } }
					],
					"format": {
						"type": "TextFormat",
						"columnDelimiter": ","
					}
				},
				"availability": {
					"frequency": "Day",
					"interval": 1
				}
			}
		}
	````

 1. Click **Deploy** on the toolbar to deploy the dataset.

 1. (Optional) Finally, let's create a raw and structured dataset for the Product Catalog data. Following the steps laid out in the previous steps, our dataset should look as follows. Do not forget to mark this dataset as external.
 	
	>**Note:** This step is optional since we've already added the Product Catalog data to the SQL DW database during our earlier exercise. 

		````JavaScript
		{
			"name": "RawProductCatalogBlob",
			"properties": {
				"type": "AzureBlob",
				"linkedServiceName": "AzureStorageLinkedService",
				"typeProperties": {
					"folderPath": "partsunlimited/productcatalog",
					"format": {
						"type": "JsonFormat"
					}
				},
				"availability": {
					"frequency": "Day",
					"interval": 1
				},
				"external": true
			}
		}

		````

 1. (Optional) Here's what the Structured Dataset would look like.

	````JavaScript
		{
			"name": "StructuredProductCatalogBlob",
			"properties": {
				"type": "AzureBlob",
				"linkedServiceName": "AzureStorageLinkedService",
				"structure": [
					{
						"name": "skuNumber",
						"type": "String"
					},
					{
						"name": "id",
						"type": "Int32"
					},
					{
						"name": "productId",
						"type": "String"
					},
					{
						"name": "categoryId",
						"type": "String"
					},
					{
						"name": "categoryName",
						"type": "String"
					},
					{
						"name": "title",
						"type": "String"
					},
					{
						"name": "price",
						"type": "Double"
					},
					{
						"name": "salePrice",
						"type": "Double"
					},
					{
						"name": "costPrice",
						"type": "Double"
					}			
				],
				"typeProperties": {
					"folderPath": "processeddata/product_catalog",
					"format": {
						"type": "TextFormat"
					}
				},
				"availability": {
					"frequency": "Day",
					"interval": 1
				}
			}
		}
	````

1. Create the dataset for the SQL Data Warehouse output using the same structure:

 1. In the Editor for the Data Factory, click **New dataset** button on the toolbar and click **Azure SQL Data Warehouse** from the drop down menu.
 1. Set the **name** to "WebsiteActivitySQL" and the **linkedServiceName** to "AzureSqlDWLinkedService"
 1. Use the same structure as the previous dataset:

		````JavaScript
		"structure": [
			{
				"name": "EventDate",
				"type": "Int32"
			},
			{
				"name": "UserId",
				"type": "String"
			},
			{
				"name": "Type",
				"type": "String"
			},
			{
				"name": "ProductId",
				"type": "String"
			},
			{
				"name": "Quantity",
				"type": "Int32"
			},
			{
				"name": "Price",
				"type": "Double"
			}
		],
		````

 1. Set the **tableName** property to **adw.FactProductLog**:

		````JavaScript
		"typeProperties": {
			"tableName": "adw.FactWebsiteActivity"
		},
		````

 1. Daily availability:

		````JavaScript
		"availability": {
			"frequency": "Day",
			"interval": 1
		}
		````

 1. Make sure the final _WebsiteActivitySQL_ dataset looks like the following snippet:
 
		````JavaScript
		{
			"name": "WebsiteActivitySQL",
			"properties": {
				"type": "AzureSqlDWTable",
				"linkedServiceName": "AzureSqlDWLinkedService",
				"structure": [
					{
						"name": "EventDate",
						"type": "Datetime"
					},
					{
						"name": "UserId",
						"type": "String"
					},
					{
						"name": "Type",
						"type": "String"
					},
					{
						"name": "ProductId",
						"type": "String"
					},
					{
						"name": "Quantity",
						"type": "Int32"
					},
					{
						"name": "Price",
						"type": "Double"
					}
				],
				"typeProperties": {
					"tableName": "adw.FactWebsiteActivity"
				},
				"availability": {
					"frequency": "Day",
					"interval": 1
				}
			}
		}
		````

 1. Click **Deploy** on the toolbar to deploy the dataset.


 1. (Optional) Let's also create a SQL DW dataset for the Product Catalog table. Following the steps from the previous datasets, the JSON should look as follows. You'll notice that we've marked this dataset as **external**. For the purposes of this lab, the data is already loaded into SQL DW in the previous exercise. 

>**NOTE:** If you completed all of the other optional exercises, please DO NOT mark this dataset as external.
>**NOTE**: Using the 'Clone' option for the ADF JSON helps speed up the process.
 
	````JavaScript
			{
			"name": "StructuredProductCatalogSQL",
			"properties": {
				"type": "AzureSqlDWTable",
				"linkedServiceName": "AzureSqlDWLinkedService",
				"structure": [
					{
						"name": "skuNumber",
						"type": "String"
					},
					{
						"name": "id",
						"type": "Int32"
					},
					{
						"name": "productId",
						"type": "String"
					},
					{
						"name": "categoryId",
						"type": "String"
					},
					{
						"name": "categoryName",
						"type": "String"
					},
					{
						"name": "title",
						"type": "String"
					},
					{
						"name": "price",
						"type": "Double"
					},
					{
						"name": "salePrice",
						"type": "Double"
					},
					{
						"name": "costPrice",
						"type": "Double"
					}			
				],
				"typeProperties": {
					"tableName": "adw.DimProductCatalog"
				},
				"external":true,
				"availability": {
					"frequency": "Day",
					"interval": 1
				}
			}
		}

	```` 

 1. Click **Deploy** on the toolbar to deploy the dataset.
 
		![Data sets created](Images/ex1task3-datasets-created.png?raw=true "Data sets created")

		_Data sets created_

Now you finished creating the input/output datasets and the associated linked services for the pipelines of the Data Factory. In the upcoming exercises you will create the pipelines using the datasets and linked services you just created.

<a name="Exercise4"></a>
### Exercise 4: **Orchestrating Azure Data Factory workflow** ###

**Azure Data Factory** enables you to compose data movement and data processing tasks as a data driven workflow. In this exercise, you'll learn how to build your first pipeline that uses HDInsight to transform and analyze the logs from the Parts Unlimited web site.

<a name="Ex4Task1"></a>
#### Task 1 - Create the pipeline that will run the Hive activity ####

In this task, you'll create the pipeline to generate the stats output using a _HDInsight Hive activity_.  This output will be processed to Azure Data Warehouse in the next task.  Data is processed in a pipeline.  A **Pipeline** is a logical grouping of Activities. They are used to group activities into a unit that performs a task.

**Activities** define the actions to perform on your data. Each activity takes zero or more datasets as inputs and produces one or more datasets as output. An activity is a unit of orchestration in Azure Data Factory.

An _HDInsight Hive activity_ executes Hive queries on a _HDInsight_ cluster.


1. Go back to the Azure Portal and open the **Data Factory**.

1. In the **Author and Deploy** blade of the Data Factory, click **New dataset** button on the toolbar and select **Azure Blob Storage**. We will create a dummy dataset for the Hive activity that runs the addpartitions.hql script that does not requires any input/output dataset in the data factory.

1. Update the dataset JSON to the following snippet and click **Deploy** to create the dummy dataset. We will use this dummy dataset to perform partition creation in HiveQL.

	````JavaScript
	{
		 "name": "DummyDataset",
		 "properties": {
			  "type": "AzureBlob",
			  "linkedServiceName": "AzureStorageLinkedService",
			  "typeProperties": {
					"folderPath": "dummy",
					"format": {
						 "type": "TextFormat"
					}
			  },
			  "availability": {
					"frequency": "Day",
					"interval": 1
			  }
		 }
	}
	````

1. In the **Author and Deploy** blade of the Data Factory, click **New pipeline** button on the toolbar (click ellipsis button if you don't see the New pipeline button).

1. Change the **name** to "JsonLogsToTabularPipeline" and set the description to "Create tabular data using Hive".

1. Set the **start** date to be 3 days before the current date (for instance: "2016-10-16T00:00:00Z").

1. Set the **end** date to be tomorrow (for instance: "2016-10-20T00:00:00Z").

1. Add a Hive activity to run the "3_addpartitions.hql" script located in the storage at "partsunlimited\Scripts\addpartitions.hql" and pass the slice date components as parameters. Make sure to replace the **<****StorageAccountName****>** placeholder with the storage account name:

	````JavaScript
	"activities": [
		{
			 "name": "CreatePartitionHiveActivity",
			 "type": "HDInsightHive",
			 "linkedServiceName": "HDInsightLinkedService",
			 "typeProperties": {
				  "scriptPath": "partsunlimited\\Scripts\\3_addpartitions.hql",
				  "scriptLinkedService": "AzureStorageLinkedService",
				  "defines": {
				      "StorageAccountName": "<StorageAccountName>",
				      "Year": "$$Text.Format('{0:yyyy}', SliceStart)",
				      "Month": "$$Text.Format('{0:MM}', SliceStart)",
				      "Day": "$$Text.Format('{0:dd}', SliceStart)"
				  }
			 },
			 "inputs": [
				{ "name": "RawJsonData" }
			 ],
			 "outputs": [
				{ "name": "DummyDataset" }
			 ],
			 "scheduler": {
				  "frequency": "Day",
				  "interval": 1
			 }
		}
	],
	````

	This activity will run the **addpartitions.hql** script to create the date partition corresponding to the current slice.

1. Add another Hive activity to run the "2_structuredlogs.hql" script located in the storage at "partsunlimited\Scripts\structuredlogs.hql" and pass the slice date components as parameters:

	````JavaScript
	"activities": [
		{
			 //...
		},
		{
			 "name": "ProcessDataHiveActivity",
			 "type": "HDInsightHive",
			 "linkedServiceName": "HDInsightLinkedService",
			 "typeProperties": {
				  "scriptPath": "partsunlimited\\Scripts\\2_structuredlogs.hql",
				  "scriptLinkedService": "AzureStorageLinkedService",
				  "defines": {
				      "Year": "$$Text.Format('{0:yyyy}', SliceStart)",
				      "Month": "$$Text.Format('{0:MM}', SliceStart)",
				      "Day": "$$Text.Format('{0:dd}', SliceStart)"
				  }
			 },
			 "inputs": [
				  { "name": "DummyDataset" }
			 ],
			 "outputs": [
				  { "name": "WebsiteActivityBlob" }
			 ],
			 "scheduler": {
				  "frequency": "Day",
				  "interval": 1
			 }
		}
	],
	````

1. Click **Deploy** on the toolbar to create and deploy the pipeline.


1. (Optional) Finally, let's also create a pipeline to move our product catalog data.

	````JavaScript
	"activities": [
		{
			 //...
		},
		{
			 "name": "ProductCatalogHiveActivity",
			 "type": "HDInsightHive",
			 "linkedServiceName": "HDInsightLinkedService",
			 "typeProperties": {
				  "scriptPath": "partsunlimited\\Scripts\\4_productcatalog.hql",
				  "scriptLinkedService": "AzureStorageLinkedService",
				  "defines": {
				      "StorageAccountName": "<StorageAccountName>"
				  }
			 },
			 "inputs": [
				  { "name": "RawProductCatalogBlob" }
			 ],
			 "outputs": [
				  { "name": "StructuredProductCatalogBlob" }
			 ],
			 "scheduler": {
				  "frequency": "Day",
				  "interval": 1
			 }
		}
	],
	````



<a name="Ex4Task2"></a>

####Task 2 - Create a new pipeline to move the HDI output to SQL Data Warehouse####

In this task, you'll create a new pipeline to move the Hive activity output (stored in a blob) to the SQL Data Warehouse database so it can be consumed using Power BI in the next exercise.

1. In the **Author and Deploy** blade of the Data Factory, click **New pipeline** button on the toolbar (click ellipsis button if you don't see the New pipeline button).

1. Change the **name** to "LogsToDWPipeline" and set the description to "Move blob data to SQL Data Warehouse".

1. Set the **start** date to be 2 days before the current date.

1. Set the **end** date to be tomorrow.

1. (Optional) Let's first create a Copy activity to move the Product Catalog data from Blob Storage to SQL DW. Notice in the _Sink_ section, how Polybase has been used to copy the data to SQL Data Warehouse.
	````JavaScript
	"activities": [
		{
			"type": "Copy",
			"name": "ProductCatalogToDWActivity",
			"typeProperties": {
				"source": {
					"type": "BlobSource"
				},
				"sink": {
					"type": "SqlDWSink",
                    "sqlWriterCleanupScript": "$$Text.Format('TRUNCATE table adw.DimProductCatalog')",
                    "writeBatchSize": 0,
                    "writeBatchTimeout": "00:00:00"

				}
			},
			"inputs": [
				{
					"name": "StructuredProductCatalogBlob"
				}
			],
			"outputs": [
				{
					"name": "StructuredProductCatalogSQL"
				}
			],
			"scheduler": {
				"frequency": "Day",
				"interval": 1
			}
		}
	]
	
	````

1. Add a Move activity to move the generated tabular blobs to the SQL Data Warehouse. Notice in the _Sink_ section, how Polybase has been used to copy the data to SQL Data Warehouse.

	````JavaScript
	"activities": [
		{
			"type": "Copy",
			"name": "HiveToDWActivity",
			"typeProperties": {
				"source": {
					"type": "BlobSource"
				},
				"sink": {
					"type": "SqlDWSink",
                    "sqlWriterCleanupScript": "$$Text.Format('DELETE FROM adw.FactWebsiteActivity WHERE EventDate >= \\'{0:yyyy-MM-dd HH:mm}\\' AND EventDate < \\'{1:yyyy-MM-dd HH:mm}\\'', WindowStart, WindowEnd)",
                    "writeBatchSize": 0,
                    "writeBatchTimeout": "00:00:00"
				    }
				},
			"inputs": [
				{
					"name": "WebsiteActivityBlob"
				}
			],
			"outputs": [
				{
					"name": "WebsiteActivitySQL"
				}
			],
			"scheduler": {
				"frequency": "Day",
				"interval": 1
			}
		}
	]
	````

1. Click **Deploy** on the toolbar to create and deploy the pipeline.

1. In the Data Factory blade, click **Diagram**, you'll see how the parts were connected and the details.

	![Updated diagram](Images/ex3task2-diagram.png?raw=true "Updated diagram")

	_Updated diagram_

    You can zoom in, zoom out, zoom to 100%, zoom to fit, automatically position pipelines and tables, and show lineage information (highlights upstream and downstream items of selected items). You can double-click an object (input/output table or pipeline) to see its properties.

<a name="Ex4Task3"></a>
#### Task 3 - Monitoring the Data Factory ####

With the Monitoring and Managing app you can easily monitor and manage your data factory pipelines, this app:

- Presents information in a simple way and provide powerful insights into your pipelines.
- Improves navigation and debug ability of pipelines in the data factory by enabling different views of the data factory.
- Enables batch actions in data factory to improve developer productivity.
- Allows you to create and enable email notifications on various events with simple steps (ex. failure).

In this task, you'll explore the features of the Monitoring App using the data factory you created.

1. In the _Summary_ section of the Data Factory blade, click on **Monitoring App** to launch the monitoring app in a new tab.

	![Open Monitoring App](Images/ex3task3-open-monitoring-app.png?raw=true "Open Monitoring App")

	_Open Monitoring App_

1. Review the Data Factory view with full status details. You can easily view the active pipelines and the slices/windows status.

	![Monitoring App](Images/ex3task3-monitoring-app.png?raw=true "Monitoring App")

	_Monitoring App_

1. Click on the glasses icon at the left pane. Notice you can switch the system view to see the activity windows filtered by state.

	![Monitoring Views](Images/ex3task3-monitoring-views.png?raw=true "Monitoring Views")

	_Monitoring Views_

	This section enables pre-canned list of views for the data factories with the Diagram view as the center of the universe viz. recent activity windows, failed activity windows, in-progress activity windows.

1. Double-click on the top margin of the **LogCsvFromBlob** dataset. Notice a calendar shows up with the activity windows highlighted. Click on the calendar' dates to see how it highlights the corresponding activity windows.

	![Activity Calendar](Images/ex3task3-activity-calendar.png?raw=true "Activity Calendar")

	_Activity Calendar_

	You have the ability to view the data factory in terms of activity windows in pipelines. Makes it easier to monitor, browse activity windows with fewer clicks. Showcases activity run details with all the attempts corresponding to each activity window. Allows you to view activity executions in calendar view, or a hover over a dataset.

1. You can also filter and sort the activity windows in an intuitive way by clicking on the column headers.

	![Sorting and filtering activity windows](Images/ex3task3-sort-filter.png?raw=true "Sorting and filtering activity windows")

	_Sorting and filtering activity windows_

1. You can stop, pause and resume the pipelines using the buttons below each pipeline.

	![Pause pipeline](Images/ex3task3-pause.png?raw=true "Pause pipeline")

	_Pause pipeline_

	If you use the **Resource Explorer** on the left pane, you can select multiple pipelines to execute the puase/resume action.

There are more features to continue exploring in the Monitoring App. You can also configure _alerts_ to create email notifications on various events (failure, success, etc.), re-run failed activities and even configure the app layout by dragging the panes.

<a name="Ex4Task4"></a>
#### (Optional) Task 4 - Executing Stored Procedure in SQL Data Warehouse ####

In this task you will add a new pipeline to run the stored procedure that populates the adw.ProfitableProducts table in the SQL Data Warehouse.

Instead of invoking the Stored Procedure along with the copy activity, you can use the SQL Server Stored Procedure activity in a Data Factory pipeline to invoke a stored procedure.

1. Create a new dataset for the ProfitableProducts table that will be the output of the new pipeline.
 1. In the Editor for the Data Factory, click **New dataset** button on the toolbar and click **Azure SQL Data Warehouse** from the drop down menu.
 1. Set the **name** to "StatsSqlDWOuput" and the **linkedServiceName** to "AzureSqlDWLinkedService"
 1. Set the **tableName** property to **adw.ProfitableProducts**:

		````JavaScript
		"typeProperties": {
			"tableName": "adw.ProfitableProducts"
		},
		````

 1. Daily availability:

		````JavaScript
		"availability": {
			"frequency": "Day",
			"interval": 1
		}
		````

 1. Make sure the final _ProfitableProductSP_ dataset looks like the following snippet:
 
		````JavaScript
		{
			 "name": "ProfitableProductsSQL",
			 "properties": {
				  "published": false,
				  "type": "AzureSqlDWTable",
				  "linkedServiceName": "AzureSqlDWLinkedService",
				  "typeProperties": {
						"tableName": "adw.ProfitableProducts"
				  },
				  "availability": {
						"frequency": "Day",
						"interval": 1
				  }
			 }
		}
		````

 1. Click **Deploy** on the toolbar to create and deploy the new dataset.

1. Now, create the pipeline to run the **adw.asp_populate_ProfitableProducts** stored procedure.
 1. In the **Author and Deploy** blade of the Data Factory, click **New pipeline** button on the toolbar (click ellipsis button if you don't see the New pipeline button).
 1. Change the **name** to "SqlDWSprocActivity" and set the description to "Run stored procedure to populate product stats".
 1. Set the **start** date to be 4 days before the current date.
 1. Set the **end** date to be tomorrow.
 1. Add a _SqlServerStoredProcedure_ activity to to run the **adw.asp_populate_ProfitableProduct** stored procedure from the SQL Data Warehouse. Make sure to replace the **<****StorageAccountName****>** placeholder with the storage account name:

		````JavaScript
		"activities": [
			{
				 "type": "SqlServerStoredProcedure",
				 "typeProperties": {
					  "storedProcedureName": "adw.asp_populate_ProfitableProducts"
				 },
				 "inputs":[
					{
						"name": "WebsiteActivitySQL"
					}
				 ],
				 "outputs": [
					  {
							"name": "ProfitableProductsSQL"
					  }
				 ],
				 "scheduler": {
					  "frequency": "Day",
					  "interval": 1
				 },
				 "name": "SqlDWSprocActivity"
			}
		],
		````

 1. Click **Deploy** on the toolbar to create and deploy the pipeline.

 1. In the Data Factory blade, click **Diagram**, you'll see the new pipeline.

	![New pipeline to run stored procedure](Images/ex3task4-sp-pipeline.png?raw=true "New pipeline to run stored procedure")

	_New pipeline to run stored procedure_

1. Make sure the output dataset slices have completed with success so the adw.ProfitableProducts table is populated.  Once the summary table is populated you can proceed with the next exercise to consume the just populated table.

	![New pipeline output slices](Images/ex3task4-sp-pipeline-slices.png?raw=true "New pipeline output slices")

	_New pipeline output slices_

<a name="Exercise5"></a>
### Exercise 5: Using the data in the warehouse to generate Power BI visualizations ###

Power BI and SQL Data Warehouse allow you to quickly and easily create data visualizations, even at terabyte scale, or contains both relational and non-relational aspects.

<a name="Ex5Task1"></a>
#### Task 1 - Open the Data Warehouse in PowerBI ####
The easiest way to move between your _SQL Data Warehouse_ and _Power BI_ is with the **Open in Power BI** button. This button allows you to seamlessly begin creating new dashboards in Power BI.

In this task, you'll open Power BI and connect to the SQL Data Warehouse from the Azure Portal.

1. Navigate to the SQL Data Warehouse instance in the Azure Portal.

1. Click the **Open in Power BI** button.

	![Open in Power BI](Images/ex4task1-open-powerbi.png?raw=true "Open in Power BI")

	_Open in Power BI_

1. Sign in using an organization account.

1. You will be directed to the SQL Data Warehouse connection page, with the information from your SQL Data Warehouse pre-populated.

	![Connect to Azure SQL Data Warehouse](Images/ex4task1-connect-azure.png?raw=true "Connect to Azure SQL Data Warehouse")

	_Connect to Azure SQL Data Warehouse_

1. Click **Next** and enter your SQL Data Warehouse username and password (it should be **P@ssword123** if you did not change the script), then click **Sign in**.

	![Server credentials](Images/ex4task1-server-credentials.png?raw=true "Server credentials")

	_Server credentials_

1. Once the connection is made, the partsunlimited dataset will appear on the left blade.

<a name="Ex5Task2"></a>
#### Task 2 - Create Reports in Power BI ####
A **Power BI report** is one or more pages of visualizations (charts and graphs). A **dashboard** is a single canvas that contains one or more tiles that you can use to connect to multiple datasets and display visualizations from many different reports.

In this task, you'll create a report based on the SQL Data Warehouse dataset you created.

1. Click the **Azure SQL Data Warehouse** tile from your dashboard to create a new report.

	![Azure SQL Data Warehouse tile](Images/ex4task2-dw-tile.png?raw=true "Azure SQL Data Warehouse tile")

	_Azure SQL Data Warehouse tile_

	In the navigation pane, your reports are listed under the **Reports** heading. Each listed report represents one or more pages of visualizations based on one or more of the underlying datasets.

1. In the _Fields_ pane, check **category** and **views** from the **adf.ProfitableProducts** table.

	![Product fields](Images/ex4task2-fields.png?raw=true "Product fields")

	_Product fields_

1. You will see a table showing the product categories with the total views.

	![Categories view table](Images/ex4task2-product-views-table.png?raw=true "Categories view table")

	_Categories view table_

1. Select the **Stacked columns chart** in the _Visualizations_ pane.

	![Stacked columns visualization](Images/ex4task2-chart-visualization.png?raw=true "Stacked columns visualization")

	_Stacked columns chart visualization_

	![Categories view chart](Images/ex4task2-product-views-chart.png?raw=true "Categories view chart")

	_Categories view chart_

	> **Note:** Chart values will differ since the values are randomly generated when creating the input data files in the setup script.

1. Now lets create a new chart, click the canvas, outside the table, and check the fields **title** and **views**.

1. Select the **Pie chart** visualization.

	![Products pie chart](Images/ex4task2-categories-pie.png?raw=true "Products pie chart")

	_Products pie chart_

    You can continue creating charts using any combination of fields and also apply filters and other operations. Then, you can change the report layout, add labels, shapes and other visual objects. To learn more about Power BI reports visit [Reports in Power BI](https://powerbi.microsoft.com/en-us/documentation/powerbi-service-reports/)

1. When you're done with your report, go to **File** > **Save** and enter a name for your report.

	![Save report](Images/ex4task2-save-report.png?raw=true "Save report")

	_Save report_

---

<a name="Summary"></a>
## Summary ##

By completing this module, you should have:

- Create a **HDInsight Cluster** and issue a **Hive** query to do analytics on your data
- Create an **Azure Data Warehouse** and load data from Azure storage
- Create an **Azure Data Factory** and orchestrate an **Azure Data Factory** workflow
- Automate data movement from **HDInsight** to **SQL Data Warehouse**
- Use the data in **SQL Data Warehouse** to generate **Power BI** visualizations

