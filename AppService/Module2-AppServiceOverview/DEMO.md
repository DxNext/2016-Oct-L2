## Demo Prerequisites
- [Install Bash on Windows](https://msdn.microsoft.com/commandline/wsl/install_guide)
- [Install CLI 2.0 (Preview) under Bash](https://github.com/Azure/azure-cli/blob/master/doc/preview_install_guide.md#ubuntu-1404-lts-and-bash-on-windows-build-14362)
- [Git](http://www.git-scm.com/downloads).
- A Microsoft Azure account. If you don't have an account, you can 
[sign up for a free trial](/pricing/free-trial/?WT.mc_id=A261C142F) or 
[activate your Visual Studio subscriber benefits](/pricing/member-offers/msdn-benefits-details/?WT.mc_id=A261C142F).

## Creating a Web app in the Portal with GitHub Continuous Deployment
1. Create a new web app in the portal.

    ![Create App Service web app in portal](images/create-web-app-in-portal.png "Create App Service web app in portal")

1. Fill in a name for the site, create a resource group, and create the site.

    ![](images/webapp-portal-create-settings.png)

1. Fork the [https://github.com/Azure-Samples/app-service-web-nodejs-get-started](https://github.com/Azure-Samples/app-service-web-nodejs-get-started) repo.
1. Open the **Deployment Options** settings group in the portal, select **GitHub**, and click **Authorize**.

    ![](images/portal-deployment-options.png)

1. Fill in the required settings to point to your personal account and the master branch of your new fork of the *app-service-web-nodejs-get-started* project. Click **OK**. This will automatically begin deploying the content in your GitHub repository to your Web App.

    ![](images/deployment-source.png)

1. Select the **Overview** tab for your new Web App and click on the site **URL**.

    ![](images/continuous-deployment-browse-site.png)

    ![](images/continuous-deployment-view-site.png)

1. Browse to the **/views/index.jade** file for your fork in GitHub and click the **Edit** (pencil) icon.

    ![](images/continuous-deployment-edit-file.png)

1. Update the view code to add a new message that says *Updated via continuous deployment* as shown below, then click the **Commit Changes** button at the bottom:

    ```jade
    extends layout

    block content
        h1= title
        p Welcome to #{title}
        h2 Updated via continuous deployment
    ```

    > **Note**: If you prefer, you can make this change by cloning the fork to your local machine, modifying the code locally, and pushing the change back to GitHub.

1. Switch to the portal and re-open the **Deployent Options** tab to show that a redeployment has been triggered.
1. Refresh the site to show the change is live.

    ![](images/continuous-deployment-site-updated.png)

    > **Note**: If desired, you can click on a previous deployment in the portal and click the **Redeploy** button to show site rollback.

## Azure CLI 2.0 Preview
1. Open command prompt and enter bash mode by typing `bash`
1. Walk through the install steps to [Install CLI 2.0 (Preview) under Bash](https://github.com/Azure/azure-cli/blob/master/doc/preview_install_guide.md#ubuntu-1404-lts-and-bash-on-windows-build-14362). Note that everything was pre-installed as prerequisites, so the install steps should go quickly, even over slow wi-fi. Steps are as follows: 

    ```bash
      sudo apt-get update
      sudo apt-get install -y libssl-dev libffi-dev
      sudo apt-get install -y python-dev
      curl -L https://aka.ms/InstallAzureCli | sudo bash
    ```

1. Reload the bash environment to ensure tab completion works using `exec -l $SHELL`
1. List the available commands by typing `az`
1. Demonstrate browser-integrated login by typing `az login` and following the directions.
1. Change default output format to table by typing `az configure` and selecting the opion for table.
1. List resources by typing `az resource list`.

> Note: Additional CLI demo commands are demonstrated in the [Ignite presentation](https://myignite.microsoft.com/secondscreen/2673) at 34:00.

## Visual Studio Publishing
> Note: This is excerpted from the Managing Environments lab.

1. Open **Visual Studio Community 2015** and select **File | New | Project...** to create a new solution.

1. In the **New Project** dialog box, select **ASP.NET Core Web Application (.NET Core)** under the **Visual C# | Web** tab, and make sure **.NET Framework 4.6.1** is selected. Name the project _MyWebApp_, choose a **Location** and click **OK**.

    ![New ASP.NET Web Application project](images/new-aspnet-web-application-project.png "New ASP.NET Web Application project")

    _New ASP.NET Web Application project_

1. In the **New ASP.NET Project** dialog box, select the **Web Application** template under **ASP.NET 5 Templates**. Also, make sure that the **Authentication** option is set to **No Authentication**. Make sure the "**Host in the cloud**" option is not checked (you will run this manually). Click **OK** to continue.

    ![Creating a new project with the Web Application template](images/creating-a-new-aspnet-project.png "Creating a new project with the Web Application template")

    _Creating a new project with the Web Application template_

1. Right-click the **MyWebApp** project and select **Publish...**. In the **Publish Web** dialog, click **Microsoft Azure App Service**.

    ![Selecting Microsoft Azure App Service](images/selecting-azure-app-service.png "Microsoft Azure App Service")

    _Microsoft Azure App Service_

1. Click **Add an account...**. to sign in to Visual Studio with your Azure account.

    ![Adding an account](images/adding-an-account.png "Adding an account")

    _Adding an account_

1. Then, click **New...** to open the _Create App Service_ dialog box. The _Create App Service_ dialog box will appear. Fill the **Web App Name** and **Resource Group** fields. Then click the **New...** button next to **App Service Plan**.

    ![Create App Service dialog box](images/01-CreateAppService.png "Create App Service dialog box")

    _Create App Service dialog box_

1. Click **OK** in the Configure App Service Plan dialog.

    ![Creating the App Service](images/02-CreateAppServicePlan.png "Configure the App Service Plan")

    _Configure the App Service Plan_

1. Click the **Create** button in the Create App Service Plan and wait while Azure provisions your resources.

1. Back in the **Publish Web** dialog, all the connection fields should be populated. Click **Next >** to view the **Settings** tab, which shows the Configuration and Target Framework. Click **Publish** to publish the site.

    ![Publishing the site to the new Microsoft Azure Web App](images/publishing-the-site-to-azure.png "Publishing the site to the new Microsoft Azure Web App")

    _Publishing the site to the new Microsoft Azure Web App_

    Once publishing completes, the web app will be automatically launched in your browser (at **http://{yourwebappname}.azurewebsites.net**).

    ![Web app published](images/webapp-published.png "Web app published to Azure")

    _Web app published to Azure_