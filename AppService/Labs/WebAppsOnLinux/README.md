# Create a Web App with App Service on Linux

## Using the Management Portal to create your web app
You can start creating your Web App on Linux from the [management portal](https://portal.azure.com) as shown in the image below.

![][1]

Once you select the option below, you will be shown the Create blade as shown in the image below. 

![][2]

-	Give your web app a name.
-	Choose an existing Resource Group or create a new one. (See regions available in the [limitations section]https://azure.microsoft.com/en-us/documentation/articles/app-service-linux-intro/)).
-	Choose an existing app service plan or create a new one (See app service plan notes in the [limitations section]https://azure.microsoft.com/en-us/documentation/articles/app-service-linux-intro/)). 
-	Choose the application stack you intend to use. You will get to choose between several versions of Node.js and PHP. 

Once you have the app created, you can change the application stack from the application settings as shown in the image below.

![][3]

## Deploying your web app

Choosing "deployment options" from the management portal gives you the option to use local a Git repository or a GitHub repository to deploy your application. The instructions thereafter are similarly to a non-Linux web app and you can follow the instructions in either our [local Git deployment]https://azure.microsoft.com/en-us/documentation/articles/app-service-deploy-local-git/) or our [continuous deployment]https://azure.microsoft.com/en-us/documentation/articles/app-service-continuous-deployment/) article for GitHub.

You can also use FTP to upload your application to your site. You can get the FTP endpoint for your web app from the diagnostics logs section as shown in the image below.

![][4]


## Next Steps ##

* [What is App Service on Linux?]https://azure.microsoft.com/en-us/documentation/articles/app-service-linux-intro/)
* [Using PM2 Configuration for Node.js in Web Apps on Linux]https://azure.microsoft.com/en-us/documentation/articles/app-service-linux-using-nodejs-pm2/)

<!--Image references-->
[1]: images/top-level-create.png
[2]: images/create-blade.png
[3]: images/application-settings-change-stack.png
[4]: images/diagnostic-logs-ftp.png