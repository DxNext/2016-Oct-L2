## Migrate ASP.NET site with local SQL Server using Hybrid Connections

When migrating an application to Azure App Services, there are three ways to accomplish the task. You can migrate the application and data store at the same time, or you can migrate either the application or data store to Azure, and keep the other on-premises. In this scenario, we will examine how to migrate the application while keeping the database local.

This style of a migration allows you to continue to take advantage of the investment made in your data servers, while leveraging the power of Azure App Service. In addition, for scenarios where you will not be migrating the data store to the cloud, hybrid connections will allow you to still migrate the application into Azure.

### Performing the migration

The overall steps are as follows:

1. Create the web app in Azure. This will be where you'll host the web application.
1. Register a hybrid connection in the web app. This will allow Azure to connect to your local database.
1. Install the necessary software in Windows. This will allow your local system to accept the requests.

#### Create the web app

You will start by creating the web app in App Services. In order to do so, you will need an Azure account. You will be using the free level for the lab, however, you should choose the appropriate sized VM for real deployments.

1. Open [Azure Portal](https://portal.azure.com)
1. Click **App Services** on the left side, then **Add** at the top.
1. Configure your new Web App with the following information:
  - **App name**: *migrate-&lt;your name&gt;*
  - **Subscription**: Your Azure subscription
  - **Resource Group**:
    - *Create new*
    - Name: *migrate-group*
  - **App Service plan/Location**: Open the blade and choose **Create New**
    - **App Service plan**: *migrate-free*
    - **Location**: Choose a location near you
    - **Pricing tier**:
      - Click **View all**
      - Scroll down and choose **F1 Free**
      - Click **Select**
1. Click **OK** to close the blades, and then **Create** to create the web app

When Azure Portal notifies you the web app has been created you're ready to create the hybrid connection.

#### Register Hybrid Connection

After creating the web app the next step is to create the Hybrid Connection. This will allow your application to access your on-premises SQL Server instance.

1. Open [Azure Portal](https://portal.azure.com) if not already done
1. Click **App Services** on the left side, and then choose the web app you created previously, named **migrate-&lt;your name&gt;**
1. In the **settings** column on the left, scroll down and choose **Networking** under **Settings**
1. Click **Configure your hybrid connection endpoints**
1. Click **Add** in the **Hybrid connections** blade
1. Configure your hybrid connection with the following information:
   - **Name**: *hybrid&lt;your name&gt;* (no dashes)
   - **Hostname**: The name of your system; *migrate* for this lab
   - **Port**: The port of the service; *1433* for this lab, which uses SQL Server
   - **BizTalk Service**: This will be the service created in Azure to allow the connection. Configure it with the following information:
    - **Name**: *hybrid&lt;your name&gt;* (no dashes)
    - The remainder of the settings will stay as the default
    - Click **OK**
  - Click **OK**

Azure will now create the hybrid connection. Once this is completed you will need to install the necessary software on Windows.

#### Install and configure the hybrid connection listener

Once Azure is configured to connect to your system, your system must be setup to accept the connection. This is accomplished by installing and configuring a listener. The steps below will work when installing the listener from the system hosting SQL Server. You can also install and configure the listener on a different system by copying the primary endpoint address, downloading the listener software, and manually configuring it.

If you don't have the Hybrid connections blade open, follow these steps to open the correct blade:

1. Open [Azure Portal](https://portal.azure.com) if not already done
1. Click **App Services** on the left side, and then choose the web app you created previously, named **migrate-&lt;your name&gt;**
1. In the **settings** column on the left, scroll down and choose **Networking** under **Settings**
1. Click **Configure your hybrid connection endpoints**

Once the **Hybrid connections** blade is open, you can install and configure the listener as follows:

1. Click **hybrid&lt;your name&gt;**
1. Click **Listener Setup**
1. Click **Install and configure now**
1. Click **Run** on the application installer
1. Click **run** on the **Open File - Security Warning** dialog
  - The installer will run and install the listener and connection manager. When completed, click **Close**

After a few moments, the Azure Portal should automatically refresh and indicate the connection has a status of **Connected**.

Azure is now configured to communicate with your local VM, and your local VM is now configured to accept requests from Azure. Now you're ready to deploy your web application.

#### Deploy web application

One of the great advantages to using Hybrid Connections is the on-premises SQL Server will look as though it's on the same network as the application. In many scenarios, no additional configuration will be required to connect to SQL Server after deployment.

In this section, you will open the Expenses.Mvc application and explore the Web.config. You won't make any changes in this section, simply confirm that the application works locally using the name of the server **migration**

1. From the desktop, double click **Expenses.Mvc.sln**
1. Under **Expenses.Web**, open **Web.config**
1. At line **13**, make a note of the connection string
  - The name of the **data source** is **migration**, which is the name of the local system, and the name you used when configuring the connection in Azure
  - Note that the connection is using SQL Authentication. Windows Authentication is not supported using Hybrid Connections.
  - This connection string **does not** need to be updated after deployment

In this section, you will test the application to confirm it runs locally. It has already been deployed to the local instance of IIS.

1. Navigate to **http://localhost**
  - Note the existing data
  - This data is being pulled from the local instance of SQL Server

In this section, you will deploy the application to Azure. You will do this by first downloading the profile, and then using Visual Studio to publish the application. You will confirm everything works by opening the site in Azure.

1. Open [Azure Portal](https://portal.azure.com) if not already done
1. Click **App Services** on the left side, and then choose the web app you created previously, named **migrate-&lt;your name&gt;**
1. Click **...More**, then **Get publish profile**
1. Click **Save** to save the profile to **Downloads**
1. Return to Visual Studio
1. Right click on **Expenses.Web**, then choose **Publish...**
1. Click **Import**
1. Click **Browse**
1. Navigate to **Downloads** by clicking it under **This PC**
1. Choose **migrate-&lt;your name&gt;.PublishSettings** and click **Open**, then **OK**
1. Click **Publish**, as no other changes need to be made

Visual Studio will deploy your site, and then automatically open the site in Azure in Internet Explorer. Note the site displays the same data you saw earlier, as it is using your local instance of SQL Server. If you want, you could change some of the data directly by using SQL Server Management Studio, refresh the page, and see the updates.

### Summary

You have a lot of options available to you when migrating a website to Azure App Services. You can choose to migrate your application in stages, by either first uploading the database or, in our case, uploading the application. By using Hybrid Connections, you're able to access SQL Server locally, without having to update your application.

What we saw in this exercise was how to create the web app in App Services, how to deploy our application, and then how to create and configure the Hybrid Connection.
