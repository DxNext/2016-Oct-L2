## Migrate SQL Server with local ASP.NET site

When migrating an application to Azure App Service, you have several options available to you for your migration plan. You can migrate the site first, keeping your database local. Or, you can migrate your database first, and then your application later. That is what this lab will examine.

### The Scenario

You have an Expense management application written in ASP.NET that uses SQL Server. You have decided to migrate the database to take advantage of features such as geo-replication and automatic backups in SQL Azure, while keeping the application on your local servers.

### Performing the migration

The overall steps are as follows:

  1. Create a new SQL database in Azure
  1. Configure the firewall to allow your VM to connect to SQL
  1. Migrate the database to Azure
  1. Update your application to use the database on Azure

### Creating a new SQL Server database in Azure

Azure SQL Server is a managed service, allowing you to focus on creating your application, rather than on managing servers. You can create databases using an elastic model, meaning your database can automatically scale as needed, or a single database if your load is predictable. For ease of deployment, this lab will use the single database model.

#### Create the database

1. Login to the [Azure Portal](https://portal.azure.com)
1. Click on **SQL databases** on the left, and then **Add** on the blade.
1. Configure your database with the following information:
  - Name: *expenses*
  - Resource group:
    - Create New
    - Name: *sqlfirstmigration*
  - Select source: *Blank database*
  - Server:
    - Create new
    - Name: *expenses-<your-name>*
    - Server admin login: *expenses*
    - Password: *P@ssw0rd*
    - Location: *West US* (or whatever is appropriate for your region)
    - Click *Select* to close the blade when done
  - Pricing tier: *B*
  - Pin to dashboard: *Checked*
1. Click **Create** to create the database 

### Configure the firewall to allow connection

By default, only services in Azure can access an Azure SQL Database. If you want to be able to connect to the database you need to allow your IP address to connect by configuring the *server* (not the database).

#### Add your IP address to the firewall rules

1. Login to the [Azure Portal](https://portal.azure.com) if you haven't done so already
1. On the main portal page, click on the database you created in the prior set of steps, named **expenses**.
1. Click on the **Server name** (*expenses-<your-name>.database.windows.net*)
1. Click **Firewall**
1. Add a new rule with the following information:
  - Name: *Expense-Server*
  - Start IP Address: *The IP address displayed on the Azure Portal screen*
  - End IP Address: *The IP address displayed on the Azure Portal screen*
1. Click **Save**

#### Test the connection

1. Open **SQL Server Management Studio** (SSMS)
1. Click **Connect**, **Database Engine...*
1. Configure the connection using the following information:
  - Server name: *expenses-<your-name>.database.windows.net
  - Authentication: *SQL Server Authentication*
  - Username: *expenses*
  - Password: *P@ssw0rd*
1. Click **Connect**

You should now see the server appear in Object Explorer in SSMS.

### Migrate the database to Azure

There are a couple of ways to migrate a database to Azure. You can create a BACPAK file, which is perfect for medium to large databases, or in situations where the server you're migrating from is disconnected.

For small to medium sized databases, the easiest way to perform the migration is to use the SSMS Migration Wizard.

If you want more information about performing migrations, testing your databsae, and using BACPACs, you can consult the [Azure documentation](https://azure.microsoft.com/en-us/documentation/articles/sql-database-cloud-migrate/)

#### Perform the migration

1. Open SSMS if it's not already open
1. Connect to the local system, if it's not already done
  - In Object Explorer, click **Connect**, **Database Engine**
  - In the login dialog, use the following information
    - Server name: *migration*
    - Authentication: *Windows Authentication*
  - Click *Connect*
1. Right click on **Expenses.MVC**, and choose **Deploy Database to Microsoft Azure SQL Database**
1. Configure the **Deployment Settings** page with the following information::
  - Click **Connect**
    - Server name: *expenses-<your-name>.database.windows.net
    - Authentication: *SQL Server Authentication*
    - Username: *expenses*
    - Password: *P@ssw0rd*
    - Click **Connect**
  - New database name: *Expenses.MVC*
1. Click **Next** and **Finish**
1. Click **Close**

#### Confirm the database was deployed.

1. Click **Connect**, **Database Engine...**
1. Configure the connection using the following information:
  - Server name: *expenses-<your-name>.database.windows.net
  - Authentication: *SQL Server Authentication*
  - Username: *expenses*
  - Password: *P@ssw0rd*
1. Click **Connect**
1. Expand **Databases**, **Expenses.MVC**, **Tables**
1. Right click on **dbo.DbEmployees** and choose **SELECT TOP 1000 Rows**
1. Kim Akers should appear as the employee

### Update the application to use Azure SQL Database

After migrating the database, you need to update your application to point at the new database. You will do this just as you would a normal ASP.NET application, by updating the web.config file.

1. Open **File Explorer** and navigate to **C:\inetpub\wwwroot**
1. Double click on **Web.config**, which will open in Visual Studio Code
1. Find the line that starts with **<add name="DefaultConnection"** inside the **<connectionStrings>** section (this should be line 13)
1. Set the **connectionString** attribute to the following:
  - *Data Source=expenses-<your-name>.database.windows.net;Initial Catalog=Expenses.Mvc;User ID=expenses;Password=P@ssw0rd*
1. Click **File**, **Save**
1. Open **Internet Explorer** and navigate to **localhost**
1. The application should build, and display the same data we saw previously. You could modify the data via SSMS to confirm the updates.

### Sumary

You have several options for migrating an application to Azure, including the ability to migrate the database first. This lab walked you through that scenario, finishing with a local application and a remote database server.
