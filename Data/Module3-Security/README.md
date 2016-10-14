<a name="Top"></a>
#Module 3 - Multi-tenant Security #
---


<a name="Overview"></a>
## Overview ##

The first question that anyone asks when working with the cloud is about security. Microsoft Azure in itself is secure, but, there are a few measures to be taken in order to ensure that your tenant's data is secure and not accessible by anyone but the tenant.

In this module, we will go through the best practices of row-level security (RLS) and encryption of your data. Azure SQL DB & SQL Data warehouse offer TDE (Transparent Data Encryption) out of the box, which help protect your data against thread of malicious activity by performing real-time encryption and decryption of your data at rest.

We will understand how connection security works and how firewall rules can help restrict unauthorized usage of data. We will also focus on how authentication and authorization can help keep data safe from unauthorized users and how it can help us achieve RLS for analytical stores such as SQL Data Warehouse & HDInsight that don't have the notion of RLS. Finally, we'll learn how you can encrypt the data in your warehouse using TDE.



<a name="Objectives"></a>
### Objectives ###
In this module, you'll see how to:

- Learn how to set firewall rules on your SQL DB and SQL DW
- Add authentication and authorization to your database to help prevent unauthorized access
- Demonstrate Row-level security in SQL Data Warehouse, which can be carried over into Azure SQL DB & HDInsight
- Learn how to enable TDE on your data


<a name="Prerequisites"></a>
### Prerequisites ###

The following is required to complete this module:

- [Microsoft Visual Studio Community 2015][1] or greater
- [Microsoft Command Line Utilities 13.1 for SQL Server][2] or greater
OR
- [Node JS][3] & [SQL commandline utility for NodeJS][4] 


[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: https://www.microsoft.com/en-us/download/details.aspx?id=53591
[3]: https://nodejs.org/en/download/
[4]: https://www.npmjs.com/package/sql-cli



<a name="Setup"></a>
### Setup ###
In order to work on this exercise, it is recommended that you complete module 2. However, if you have not completed module 2, please follow this setup in order to successfully complete this exercise.

>**Note:** If you've completed Module 2, you may skip the setup and start with Exercise #1.

<a name="SqlDWCreation"> </a>
#### SQL Data Warehouse Creation #####

Navigate to the Setup Folder under 'Module 3'. You will find a folder called Setup\CLI. 

1. Update the parameters.json file.  Update parameters with a unique suffix for use across the services. Save the file. Particularly, you will want to update the 'uniqueSuffix' variable. This will help keep your resource names globally unique. We will be executing the azuredeploy.json ARM template, which will help us setup the resources needed for this lab.

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

1. Execute the following statement to execute the ARM template that will deploy the Azure SQL  DW.  Set <DeploymentName> with the same prefix used for the ResourceGroupName.

	````
	azure group deployment create -f <path to azuredeploy.json> -e <path to parameters.json> -g <ResourceGroupName> -n <DeploymentName>

	````

1. Next, let's load up the data into our Data Warehouse. We will be using the 'bcp' tool to achieve this. You can find the data files under the folder 'Module3-Security\Setup\data'.

>**Note:** (For Mac Users) You will need to create a  blob storage account and a container within the account named 'processeddata'. Then, upload the data found in the data folder to the 'processeddata' container and follow the Exercise 2 - Tasks 1-4 in module 2 to load your data into SQL DW.

>**Note:** (For Ubuntu Users) Please follow [this][1] blog to install bcp on your machine to setup sqlcmd and bcp on your machine.

[1]: https://blogs.msdn.microsoft.com/joseph_idzioreks_blog/2015/09/13/azure-sql-database-sqlcmd-and-bcp-on-ubuntu-linux/


1. Open the command line tool or connect to the SQL Data Warehouse using Visual Studio and enter the following commands to create a new schema for our internally managed tables. 

	````SQL
	CREATE SCHEMA [adw]
	GO

	````

1.  Execute the following in a query window to create a new partitioned table.  Note this table will be created based on a SELECT statement issued on the external table. 

	````SQL
	CREATE TABLE adw.FactWebsiteActivity
	(
		EventDate datetime2,
		UserId nvarchar(20),
		Type nvarchar(20),
		ProductId nvarchar(20), 
		Quantity int, 
		Price float
	)
	WITH (
		CLUSTERED COLUMNSTORE INDEX,
		DISTRIBUTION = HASH(ProductID)
		)
	
	GO
	````

1. Similarly, let's pull our latest and greatest product catalog data from blob storage.

	````SQL
	IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'adw' AND TABLE_NAME = 'DimProductCatalog')
		DROP TABLE adw.DimProductCatalog
	GO

	CREATE TABLE adw.DimProductCatalog
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
		CLUSTERED COLUMNSTORE INDEX,
		DISTRIBUTION = HASH(ProductID)
		)
	
	````

1. Now, let's do a bulk insert into our 2 tables.

