## Continuous Deployment using GitHub

Visual Studio offers built-in tooling for deploying applications to Microsoft Azure App Service. However, having someone manually deploy doesn't give you continuous deployment. In addition, you might be using a different tool, such as Visual Studio Code or Sublime, which doesn't offer such capabilities.

Fortunately,  App Services allows you to deploy code from most common repositories, including TFS, Git, Bitbucket, and GitHub. By configuring App Service to deploy from a repository, you're able to enable Continuous Deployment.

In addition, App Service, through Hybrid Connections, allows you to access local resources, such as MongoDB. This offers you the flexibility to deploy your application in stages, 

### Overview

In this lab, you will take a Node.JS application deployed to GitHub and configure continuous deployment.

Fortunately,  App Service allows you to deploy code from most common repositories, including TFS, Git, Bitbucket, and GitHub. By configuring App Services to deploy from a repository, you're able to enable Continuous Deployment.

In addition, App Service allows you to access local resources such as MongoDB by using Hybrid Connections. This offers you the flexibility to deploy your applicaiton in stages.

### Overview

In this lab, you will take a Node.js application deployed to GitHub and configure continuous deployment.

In this lab, you'll perform the following tasks:

1. Create a web app in App Service
1. Enable Hybrid Connections to connect to MongoDB
1. Fork the **node-invoicing** application
1. Configure App Services to pull the code from your repository, and perform the initial deployment
1. Configure the connection string for your application

### Create a web app in App Services

The first step to deploying our application to App Service is to create a web app to host our code. When creating a web app you have several options available to you for monitoring, scaling, and debugging. To start you'll set up a basic application, and then add features later.

You will be creating an App Service Plan as part of this lab. The plan will determine the pricing tier and features available to your web app. You will be choosing a free plan, but you could always migrate to a higher level later.

#### Create the application

1. Login to the [Azure Portal](https://portal.azure.com)
1. Click **App Services** to open the App Services blade
1. Click **Add**, and configure your new Web App with the following information
    - App name: *node-<your-name>*
    - Resource Group:
        - **Create new**
        - *node-resource-<your-name>*
    - Create a new App Service plan
        1. Click on **App Service plan/Location**
        1. Click **Create New**, and configure it using the following information:
            - App Service plan: *node-free-plan-<your-name>*
            - Location: *West US* (or a region that is appropriate)
            - Pricing tier: *Free* (available under **View All**)
            - Click **Select**, then **OK**
1. Click **Create** to create your new web app

### Create a hybrid connection to allow connections to a local MongoDB server

There are many reasons you might decide to not deploy your entire application to the cloud. You may need to keep your database server on your local network, while deploying the web application to Azure. Azure supports this configuration through a few different means, the simplest to configure being Hybrid Connections.

[Hybrid Connections](https://azure.microsoft.com/en-us/documentation/articles/integration-hybrid-connection-overview/) allow you to have a local resource, such as a SQL Server database, or, in our case, MongoDB, that is accessible from a web app deployed to Azure. The application does not need to know the database is actually on your local network. Creating a Hybrid Connection involves two main steps.

The first is to configure the connection in Azure. You'll tell Azure the port number you wish to connect on, as well as the name the application will use to connect to the resource. This name **must** match the name of the server on your network, which for purposes of the lab is *migration*.

The second is to configure the server on your local network. You'll do this by installing a bit of software, and configuring the software to connect back to Azure.

#### Create Hybrid Connection in Azure 

1. Login to the [Azure Portal](https://portal.azure.com), if you're not already logged on
1. Click **App Services** to open the App Services blade
1. Click **node-<your-name>**
1. Under **Settings**, **Routing**, choose **Networking**
1. Click **Configure your hybrid connection endpoints**
1. Click **Add**, **New hybrid connection**, and configure your new connection with the following information:
    - Name: *nodehybrid*
    - Hostname: *migration* (this will be the name you will use to connect)
    - Port: *27017*
    - BizTalk Service:
        - Name: *node<your-name>*
1. Click **OK** three times. After a few moments your new hybrid connection will appear in the list.

#### Configure the local server to use your Hybrid Connection

1. After the connection appears under Hybrid Connections, click on **node-<your-name>**
1. Click **Listener Setup**
1. Click **Install and configure now**, and walk through the installation wizard
1. After the installation completes, the status of your connection will change to Connected.

You've now created a two-way link between your web app and your local server.

### Fork the node-invoicing repository

The lab uses a sample invoicing application written in Node.js. The application is available on [GitHub](https://github.com/GeekTrainer/node-invoicing). This is the application you will use to configure GitHub integration with your Web App, and test continuous deployment.

For this portion of the lab, you will fork the repository to your GitHub account. In order to complete this part of the lab you do need a [GitHub](https://github.com) account.

#### Fork the repository

1. Login to [GitHub](https://github.com)
1. Navigate to [node-invoicing](https://github.com/GeekTrainer/node-invoicing)
1. Click **Fork**, and then choose your account (if prompted)
1. Wait for GitHub to complete the fork

### Configure continuous deployment for your web app

Web apps allow you to automatically pull code from most any repository, including locations like Dropbox and OneDrive. In this portion of the lab you will configure your web app to pull code from the **node-invoicing** repository you forked in the prior task.

*Please note* that the application will not run at the end of this step. The reason for this is you will need to update the connection string. While we could have ensured that everything would have been the same, we wanted to add configuration as part of the lab.

#### Enable GitHub integration

1. Login to the [Azure Portal](https://portal.azure.com), if you're not already logged on
1. Click **App Services**, and then **node-<your-name>**
1. Under **Publishing**, click **Deployment source**
1. Click **Choose Source**, then **GitHub**
1. If you have not already authorized GitHub, you will do so by clicking **Authorization**, and then authenticating with your GitHub credentials
1. Click **Choose your organization**, and select **Personal**
1. Click **Choose project**, and select **node-invoicing**
1. master will be chosen as the branch by default
1. Click **OK**
1. Azure will automatically begin migrating the code, and installing npm packages.
1. Click **Deployment source**, and wait for the deployment to complete

### Configure the connection string for your application

Previously we created a Hybrid Connection, and gave it a hostname of **mongo**, which will be the hostname the application needs to connect to the database server. While you could use the same name as the server, for purposes of this lab, we wanted to use a different server to demonstrate the use of application settings with Node.js.

You are able to develop applications using many web technologies, giving you the freedom to choose the framework you're most comfortable with. Azure integrates with the application, meaning that you are able to configure your Web App using the same tools, regardless of the underlying framework your application uses. In the case of Node.js, you access environmental variables by using `process.env`. When you create application settings in Azure, the Azure runtime will ensure those values become available to the `env` object in your Node.js application.

For this task, you will first test the website, confirming that it fails to connect to MongoDB. You will then create an application setting for the connection string, and then confirm that the application does connect.

#### Configure the connection string for your application

1. Navigate to *node-<your-name>.azurewebsites.net*
    - **You will receive a 500 error** as the database connection string has not been updated
    - You will now fix it by following the steps below
1. Login to the [Azure Portal](https://portal.azure.com), if you're not already logged on
1. Click **App Services**, and then **node-<your-name>**
1. Under **General**, click **Application settings**
1. Add a new entry in **App settings** with the following information:
    - Key: *DB_CONNECTION_STRING* (this is the key name the application uses)
    - Value: *mongodb://migration/invoice*
1. Click **Save**
1. Refresh *node-<your-name>.azurewebsites.net*
    - The site should now display a list of salespeople

### Summary

Microsoft Azure App Service offers you tooling to support both continuous deployment and hybrid configurations. In this lab you saw how to create a connection from your App Service to a local instance of MongoDB. You also saw how to use GitHub as the repository for your application, and enable continuous deployment. 