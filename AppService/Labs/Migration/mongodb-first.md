## Migrating MongoDB to DocumentDB

When migrating your application, you may decide to take advantage of the cloud for storing data. In particular, with DocumentDB, there are many features you may wish to implement, such as [global distribution](https://azure.microsoft.com/en-us/documentation/articles/documentdb-distribute-data-globally/) of your data and scaling. If you have a database already created in MongoDB, you can migrate it to DocumentDB with [protocol support for MongoDB](https://azure.microsoft.com/en-us/documentation/articles/documentdb-protocol-mongodb/), without rewriting your application. DocumentDB provides a fully managed implementation of NoSQL.

### Scenario and overview

This lab will walk you through the steps to migrate a MongoDB database, accessed via [mongoose](http://mongoosejs.com/), to DocumentDB. You'll complete the following steps

1. Create a DocumentDB with protocol support for MongoDB server on Azure
1. Migrate the database using the Database Migration Tool
1. Update the application to point to your DocumentDB database

### Create a DocumentDB with protocol support for MongoDB database on Azure

The first step to doing the migration is to create the database in Azure. You'll do this by using the Azure Portal.

#### Create the server

1. Login to the [Azure Portal](https://portal.azure.com)
1. Click **New**, and search for *DocumentDB*
1. Click **DocumentDB - Protocol Support for MongoDB** in the dropdown selection
1. Click **DocumentDB - Protocol Support for MongoDB** on the **Everything** blade
1. Click **Create** on the **DocumentDB - Protocol Support for MongoDB** blade
1. Configure the server with the following information
  - ID: *mongo-<your-name>
  - Resource Group:
    - Create New
    - Name: *mongo-resources*
  - Location: *West US*, or as appropriate
  - Pin to Dashboard: *Checked*
1. Click **Create** to create the server

### Migrate the database using the DocumentDB Migration Tool

You will use the DocumentDB Migration Tool to migrate the Invoices MongoDB Database. You will first download the zip file containing the tool, and extract it to a folder on the desktop. You will get the connection string from Azure Portal, and then use the tool to perform the migration.

#### Obtain the DocumentDB Migration Tool

1. Download the zip file containing the [DocumentDB Migration Tool](http://www.microsoft.com/downloads/details.aspx?FamilyID=cda7703a-2774-4c07-adcc-ad02ddc1a44d)
1. Create a new folder on the Desktop by right-clicking, and choosing **New**, **Folder**
1. Name the folder *Migration Tool*
1. Extract the contents of the zip file to the new folder

#### Create the invoices database


1. Login to the [Azure Portal](https://portal.azure.com)
1. Click on **mongo-<your-name>** from the landing page
1. Click **Add Database**
1. Use *invoices* for the ID
1. Click **ID**

#### Obtain the server information

1. Login to the [Azure Portal](https://portal.azure.com) (if not already done)
1. Click on **mongo-<your-name>** from the landing page (if not already done)
1. Under **General**, click **Connection string**
1. Copy the following information:
  - **Password**

#### Perform the migration

You will now perform the migration. When building the connection string, you will use need to update *<your-name>* and *<PASSWORD>* (which you collected above).

1. Open the **Migration Tool** folder from the desktop
1. Double click on **dtui.exe**, and click **Run**
1. Click **Next**
1. Configure the **Source Information**
  - Connection String: *mongodb://localhost/invoices*
  - Collection: *invoices*
1. Click **Next**
1. Configure the **Target Information**
  - Connection String: *AccountEndpoint=https://mongo-<your-name>.documents.azure.com:443/;Database=invoices;AccountKey=<PASSWORD>*
  - Collection: *invoices*
1. Click **Next**, **Next**, **Import** to migrate the database
1. The migration should complete without errors

### Update the application

With the database migrated, you are now ready to update your Node.js application to point to the new location. You will do this by updating the *.env* file the application uses, via [dotenv](https://www.npmjs.com/package/dotenv).

#### Obtain the connection string

1. Login to the [Azure Portal](https://portal.azure.com) (if not already done)
1. Click on **mongo-<your-name>** from the landing page (if not already done)
1. Under **General**, click **Connection string**
1. Copy the connection string

#### Configure the application to point to the new location

1. Navigate to **C:\inetpub\node**
1. Double click on **.env** and choose **Visual Studio Code**
1. Replace **mongodb://localhost/invoices** with the connection string you copied above
1. Click **File** > **Save**
1. Navigate to **http://locahost:8080**
  - The application should display the list of salespeople
