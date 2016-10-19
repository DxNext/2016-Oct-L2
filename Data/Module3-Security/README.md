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
- [Node JS][3] & [MS SQL commandline utility for NodeJS][4] 


[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: https://www.microsoft.com/en-us/download/details.aspx?id=53591
[3]: https://nodejs.org/en/download/
[4]: https://www.npmjs.com/package/sql-cli



<a name="Setup"></a>
### Setup (To be completed if you did not complete Module 2) ###
In order to work on this exercise, it is recommended that you complete module 2. However, if you have not completed module 2, please follow this setup in order to successfully complete this exercise.

>**Note:** If you've completed **Module 2**, you may **skip the setup** and start with Exercise #1.

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


1. Connect to the SQL Data Warehouse using the command line tool or using Visual Studio 
	1. (For Command Line) Ensure that the SQL-CLI node package is installed on your machine.
		1. From the command line, connect to the SQL DW using the following command:
		````
		mssql -s <serverName>.database.windows.net -u <username>@<servername> -p <password> -d <databaseName> -e
		````
		1. If you used the default naming & credentials, this is what the mssql command looks like:
		````
		mssql -s readinesssqlsvr<uniqueSuffix>.database.windows.net -u labuser@readinesssqlsvr<uniqueSuffix> -p labP@ssword1 -d readinessdw -e
		````

	1. (For visual studio) Go to the Azure Portal (http://portal.azure.com) and navigate to the new SQL Data Warehouse you created.

		1. In the _SQL Data Warehouse_ blade, click **Open in Visual Studio**. In the new blade, click **Open in Visual Studio**.

			![Open Data Warehouse in Visual Studio](Images/setup-open-vs.png?raw=true "Open Data Warehouse in Visual Studio")
		
			_Open Data Warehouse in Visual Studio_

		1. Confirm you want to switch apps if prompted.

		1. In Visual Studio, enter the SQL Server credentials (labuser/labP@ssword1).

		1. In the _SQL Server Object Explorer_, expand the server and right-click the **readinessdw** database.   Select **New Query...**.


1. Enter the following commands to create a new schema for our internally managed tables. We will be 

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

1. Now, let's do a bulk insert into our 2 tables. Let's start with our Product Catalog table. We will use 'bcp' via the command line for this. Be sure to exit out of the SQL CLI or the sqlcmd before running bcp. Execute the following command to load the Product Catalog data.


	````
	bcp.exe adw.DimProductCatalog in ...\Module3-Security\Setup\data\product_catalog\000000_0 -S readinesssqlsvr<uniqueSuffix>.database.windows.net -d readinessdw -U labuser -P labP@ssword1 -c -q -t -E
	````

	![Import Data into SQL DW using bcp](Images/setup-bcp.jpg?raw=true "Import Data into SQL DW using bcp")
		
	_Import Data into SQL DW using bcp_


1. Similarly, let's upload our Fact data. Since our Fact data is made up of multiple files, we'll run the bcp command multiple times.

	````
	bcp.exe adw.FactWebsiteActivity in C:\Work\Projects\oct16_upskilling\Data\Module3-Security\Setup\data\structuredlogs\000001_0 -S readinesssqlsvr<uniqueSuffix>.database.windows.net -d readinessdw -U labuser -P labP@ssword1 -c -q -t -E

	bcp.exe adw.FactWebsiteActivity in C:\Work\Projects\oct16_upskilling\Data\Module3-Security\Setup\data\structuredlogs\000001_1 -S readinesssqlsvr<uniqueSuffix>.database.windows.net -d readinessdw -U labuser -P labP@ssword1 -c -q -t -E
	
	bcp.exe adw.FactWebsiteActivity in C:\Work\Projects\oct16_upskilling\Data\Module3-Security\Setup\data\structuredlogs\000010_0 -S readinesssqlsvr<uniqueSuffix>.database.windows.net -d readinessdw -U labuser -P labP@ssword1 -c -q -t -E
	
	bcp.exe adw.FactWebsiteActivity in C:\Work\Projects\oct16_upskilling\Data\Module3-Security\Setup\data\structuredlogs\000010_1 -S readinesssqlsvr<uniqueSuffix>.database.windows.net -d readinessdw -U labuser -P labP@ssword1 -c -q -t -E
	````

1. You can verify that the data has been successfully loaded by switching back to your favorite SQL tool (VS or commandline) and executing the following SQL query.
	````SQL
	SELECT * from adw.DimProductCatalog
	GO

	SELECT count(*) from adw.FactWebsiteActivity
	GO
	````

>**Note:** For the purposes of this lab, we will be using SQL Data warehouse to demonstrate the security best practices. However, the same techniques can be applied to other data technologies such as Azure SQL DB and Hive/Spark-SQL on HDInsight (Apache Ranger). There are a few differences when it comes to Apache Hive/Spark-SQL, however, it is outside of the scope of this lab.


<a name="Exercise1"></a>
### Exercise 1: Set Firewall rules ###

>**Note:** The code samples below assume that you have used **labuser** as your admin username and **labP@ssword1** as your password.

Setting firewall rules are the most basic way of securing your data store. Azure SQL Servers allow you to add firewall rules to your database. This helps you restrict access to the databases only those computers/IPs that have been white-listed in the firewall rules.

When creating the he SQL DW, we whitelisted _almost_ all IP address. So, in this exercise, we will edit these firewall rules and showcase how firewall rules can be used to avoid unauthorized access.

>**Note:** (HDInsight Parity) Azure HDInsight allows you to create Hadoop clusters within secure Virtual Networks. These can be secured by adding ACLs that will deny outside traffic to talk to the cluster. A few Azure IP addresses however must be granted access in order to have a stabled managed cluster.


1. Navigate to the Azure portal (https://portal.azure.com) and make your way to the Azure SQL Server home page.
	![Firewall setting on Azure SQL Server home page](Images/ex1-show-firewall-settings.png?raw=true "Firewall setting on SQL server homepage")
		
			

1. Click on 'Show Firewall Settings'. This should open the firewall rules of your Azure SQL Server. You'll notice that it has one rule pre-defined, which allows any IP address between the range of 0.0.0.0 and 255.255.255.0 to access the server. (You will not see this if you have skipped the setup in Module 3 and are using manually created setup from Module 2).

1. Click on the '...' on the right side of this rule and click delete.
	

1. Click on save to persist the firewall settings. You should receive a confirmation that the firewall settings have been saved.
	![Firewall setting confirmation](Images/ex1-firewall-conf.png?raw=true "Firewall setting confirmation")
		
			

1. Now, let's switch to our SQL CLI and try to connect to the Azure SQL Data Warehouse using the following command:
	````
	mssql -s <servername>.database.windows.net -u <username>@<servername> -p <password> -d <datawarehousename> -e 
	````

1. You should receive an error stating that you do not have enough permissions to access the SQL Data Warehouse.
	![Failed connecting to SQL Server](Images/ex1-sql-failed-conn.png?raw=true "Failed connecting to SQL Server")
		
			

1. Make a note of the IP address.

1. Switch back to the Azure SQL Server on the Azure portal and add a new Firewall rule with the following details:
	1. Rule Name: 'Readinnes IP'
	2. Start IP: <The IP you just recorded>
	3. End IP: <The IP you just recorded>

1. Save the changes. This should grant access to the Data Warehouse to only your IP Address.

1. Let's switch again to our SQL CLI and try to connect again to our Data Warehouse. You should now be successful in connecting to the Warehouse.
	![Successfully connected to SQL Server](Images/ex1-sql-failed-conn.png?raw=true "Successfully connected to SQL Server")
		
			


<a name="Exercise2"></a>
### Exercise 2: Authentication & Authorization ###

The primary concept of authentication & authorization is to create separate user identities and permissions for different roles. This is particularly important when you have different departments within your company working on the same Azure SQL Server or if an ISV has customers accessing the same server for information.

>**Note:** (HDInsight Parity) Apache Hadoop contains a component called as 'Apache Ranger' Ranger helps you in securing your cluster. Apache Ranger also helps you create new users and assign them appropriate roles.

#### Task 1 - Creating Authentication ####

1. From the CLI, let's login to the **master** database.
	````
	mssql -s readinesssqlsvr<uniqueSuffix>.database.windows.net -u labuser@readinesssqlsvr<uniqueSuffix> -p labP@ssword1 -d master -e
	````

1. Create a login for customer1 on the master database by executing the following command:
	````
	CREATE LOGIN customer1 WITH PASSWORD = 'P@ssword1';
	````
	
1. Exit out of the current session
	````
	.quit
	````

1. Next, let's login to the **readinessdw** database with the admin credentials.
	````
	mssql -s readinesssqlsvr<uniqueSuffix>.database.windows.net -u labuser@readinesssqlsvr<uniqueSuffix> -p labP@ssword1 -d readinessdw -e
	````

#### Task 2 - Creating Authorization ####

1. We'll create the user on our data warehouse as well and give our newly created user reading privileges to the database. This can be achieved by the following command. Reading privileges will ensure that the user cannot delete or modify the data in the database.
	````
	CREATE USER customer1 FOR LOGIN customer1;

	EXEC sp_addrolemember N'db_datareader', N'customer1'
	````

1. Exit out of the current session again.
	````
	.quit
	````

1. Now, login to the Data Warehouse using the customer 1 credentials.
	````
	mssql -s readinesssqlsvr<uniqueSuffix>.database.windows.net -u customer1@readinesssqlsvr<uniqueSuffix> -p P@ssword1 -d readinessdw -e
	````

1. Once logged in, let's run a simple select query to ensure that the new user has rights to do so.
	````
	SELECT top 10 * from adw.DimProductCatalog
	````

	![Select query results](Images/ex2-select-query.png?raw=true "Select query results")
		
			


1. Now, let's try deleting the table. Since we only gave our user READ access to the database, we should not be able to delete the database.
	````
	DROP TABLE adw.DimProductsCatalog
	````

	![Drop table error](Images/ex2-drop-table.png?raw=true "Drop table error")
		
			




<a name="Exercise3"></a>
### Exercise 3: Row-level Security ###

Row-level security is an important feature for ISVs and SaaS application providers that are handling data from multiple customers. It is important to ensure that you are exposing each customer to only their data. Azure SQL DB offers Row-level security out-of-the-box. However, other analytical data stores, such as SQL Data Warehouse & Apache Hive do not offer this feature. In this exercise, we will add row-level security to our data warehouse.

>**NOTE:** (Azure SQL DB parity) Row-level Security in Azure SQL DB is based on the same concept as what is being discussed in this exercise. More details can be found here: https://msdn.microsoft.com/library/dn765131.aspx

>**Note:**: (Hive on HDInsight parity) As previously mentioned, authentication & roles can be managed via Apache Ranger. The underlying concept of Row-level security is same as what is being discussed in this exercise can be replicated to Apache Hive with the help of Apache Ranger.


1. For the purposes of this exercise, we will assume that we have a different manufacturer or every category of products that we sell on our e-commerce website i.e. We have one customer that is selling 'Lighting' on our webiste, whereas another one selling 'Wheels & Tires', etc.

1. We will create separate users for these customers and give them permissions to only view their sales and not sales of any other category.

1. Let's start by logging into our **master** database using our admin credentials and creating logins for these users. Do not forget to log off as Customer1 created in the previous exercise.
	````
	mssql -s <servername>.database.windows.net -u <username>@readinesssqlsvr<uniqueSuffix> -p <password> -d master -e	

	CREATE LOGIN lighting_user WITH PASSWORD = 'P@ssword1'
	CREATE LOGIN wheels_user WITH PASSWORD = 'P@ssword2'

	.quit
	````

1. Now, let's switch to our **readinessdw** data warehouse.
	````
	mssql -s readinesssqlsvr<uniqueSuffix>.database.windows.net -u labuser@readinesssqlsvr<uniqueSuffix> -p labP@ssword1 -d readinessdw -e
	````

1. The first step here is to create a new **schema** in order to create views on top of our data
	````
	CREATE SCHEMA App AUTHORIZATION dbo
	````

1. Next, we'll create a new database role that only has access to this newly created **App** schema
	````
	CREATE ROLE app_view AUTHORIZATION [dbo]


	GRANT SELECT, VIEW DEFINITION ON SCHEMA::App TO app_view
	````

1. We shall now create new users on our database for our different manufacturers. Notice how we set the default schema to **App**

	````
	CREATE USER lighting_user FOR LOGIN lighting_user WITH DEFAULT_SCHEMA = App
	
	CREATE USER wheels_user FOR LOGIN wheels_user WITH DEFAULT_SCHEMAR = App
	````

1. Finally, we shall assign the role of **app_view** to our newly created users. This will restrict them to only those tables and views that are within the _app_ schema.
	````
	EXEC sp_addrolemember N'app_view',N'lighting_user'
	EXEC sp_addrolemember N'app_view',N'wheels_user'
	````

1. We can quickly test to ensure that our users don't have the rights to see any tables by logging into the data warehouse using their credentials and running the **.tables** command.
	````
	mssql -s readinesssqlsvr<uniqueSuffix>.database.windows.net -u lighting_user@readinesssqlsvr<uniqueSuffix> -p P@ssword1 -d readinessdw -e

	.tables
	````
	![No tables returned](Images/ex3-user-view-no-tables.png?raw=true "No tables returned")
		
			

1. Now that we've assigned our users to the **app** schema. Let's add some views to the schema so that our users can see the data that they're really interested in. 

	1. Let's logout of of the current user.
		````
		.quit
		````

	1. Log back into the Data warehouse using the admin credentials
		````
		mssql -s readinesssqlsvr<uniqueSuffix>.database.windows.net -u labuser@readinesssqlsvr<uniqueSuffix> -p labP@ssword1 -d readinessdw -e
		````

	1. Now, let's modify our tables to add another column to the table called as **TenantId**. Took keep it simple, we'll let the value of TenantId be the same as CategoryId.
		````
		ALTER TABLE adw.DimProductCatalog ADD TenantId int NULL
		````

		````
		UPDATE adw.DimProductCatalog SET TenantId = CategoryId
		````

	1. Next, let's create a mapping table between our **Users** and **TenantId** so that we know which information to display when a specific user logs in.
		````
		CREATE Table adw.UserTenant (UserName nvarchar(20), TenantId int)	

		INSERT INTO adw.UserTenant VALUES ('lighting_user', 1)
		INSERT INTO adw.UserTenant VALUES ('wheels_user',2)
		````

	1. Now, let's create a view on top of our fact table that joins with our dimension table and adds the product attributes to each transaction. We'll join the data with our UserTenant table to filter out the values that are not to be shown.

		````
		CREATE VIEW app.FactWebsiteActivity as 
			SELECT a.*, b.Title, b.CategoryName, b.CostPrice, b.SalePrice, b.TenantId 
				from adw.FactWebsiteActivity a 
					INNER JOIN adw.DimProductCatalog b ON a.ProductId = b.ProductId 
					INNER JOIN adw.UserTenant c on b.TenantId = c.TenantId
				WHERE c.UserName = System_User
		````

1. Now that our view is created. Let's log in to the database using our **lighting_user** profile and run some queries on the data. You should see similar results as shown in the screenshot.
	````
	mssql -s <servername>.database.windows.net -u lighting_user@<servername> -p P@ssword1 -d readinessdw -e
	
	.tables

	SELECT COUNT(*) from App.FactWebsiteActivity
	````
	
	![Show tables](Images/ex3-show-tables-app-view.png?raw=true "Show tables")
	_Show Tables Command_

1. Let's also run a simple select query on the view to ensure we are not seeing data that we aren't supposed to.
	````
	SELECT TOP 200 * from App.FactWebsiteActivity
	````
	
	![Select top 200 rows](Images/ex3-select-query.png?raw=true "Select top 200 rows")
	_Select top 200 rows_

1. Let's run the steps again for the other user to ensure that we see a different dataset than what was just returned by the SELECT query.

	````
	mssql -s <server name>.database.windows.net -u lighting_user@<server name> -p P@ssword1 -d readinessdw -e
	
	.tables

	SELECT COUNT(*) from App.FactWebsiteActivity

	SELECT TOP 200 * from App.FactWebsiteActivity
	````

With that, we have successfully added Row Level security to our Data Warehouse.


<a name="Exercise4"></a>
### Exercise 4: Transparent Data Encryption on your data ###

Transparent Data Encryption (TDE) helps encrypt database files on the source disk. It's an important requirement for many companies to have their data encrypted at rest. TDE is supported out-of-the-box by Azure SQL DB and Azure SQL Data Warehouse. HDInsight uses Azure Blob Storage underneath the covers as its storage layer and Azure Blob Storage supports Data Encryption at-rest.

1. To Enable Encryption on your data Warehouse, connect to the **master** database on the server hosting the database using a login that is an administrator or a member of the **dbmanager** role in the master database

	````
	mssql -s <server name>.database.windows.net -u <user name>@<server name> -p <password> -d master -e

	ALTER DATABASE [readinessdw] SET ENCRYPTION ON;

	````
	You may get a timeout message, please do ensure you execute the next query to check if the encryption has been turned on.

1. Finally, let's verify that the data warehouse is indeed encrypted. 

	````
	SELECT
    [name],
    [is_encrypted]
		FROM
    sys.databases
	````
	![Encryption status](Images/ex4-encryption.png?raw=true "Encryption status")

	
With that, we've successfully completed Module 3.

To recap, in this module, we've covered the basics of how you can secure your database.
