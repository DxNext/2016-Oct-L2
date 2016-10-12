# Deployment and Azure #
## Exercises ##
### Exercise 1: Build and Deploy from Visual Studio ###
#### Task 1 - Deploying a ASP.NET Core application to Azure ####

1. Create an app with Web Application template.
2. Use **Publish** to create web app resource in Azure App Service with an Azure SQL Database.
3. Publish app to the new web app resource.

### Exercise 2: Using Entity Framework Migrations ###
#### Task 1 - Enabling Migrations ####

1. Open Source\Ex2\Begin\MyWebApp.sln and `Ctrl-Shift-B` to build.
2. Open command prompt and go to Source\Ex2\Begin\src\MyWebApp
3. Enable migration

        `dnx ef migrations add InitialMigration --context PeopleContext`

4. Update database

        dnx ef database update --context PeopleContext

5. Verify changes in SQL Server Object Explorer

#### Task 2 - Updating Database Schema Using Migrations ####

1. Add `public string Country { get; set; }` to Models\Persons.cs.

1. Add a migration

        dnx ef migrations add PersonCountry --context PeopleContext

1. Update database

        dnx ef database update --context PeopleContext

1. Verify changes in SQL Server Object Explorer

### Exercise 3: Deploying a Web Site to Staging ###
#### Task 1 - Creating a Microsoft Azure Web App ####

1. Go to Azure Portal, then **New** > **Search** > **Web App + SQL**.

    - New app service plan
    - New SQL database server

2. Browse to web app from main blade

#### Task 2 - Deploying to Staging Using Git ####

1. Scale up to **Standard** tier.

2. Set up local git

3. Add a deployment slot called **staging** and copy configuration from production site.

4. Sert up local git for slot.

5. Copy Git URL

6. Push Git URL from Source\Ex3\Begin\

        cd "[YOUR-APPLICATION-PATH]"
        git init
        git config --global user.email "{username@example.com}"
        git config --global user.name "{your-user-name}"
        git add .
        git commit -m "Initial commit"

        git remote add azure [GIT-URL-STAGING-SLOT]
        git push azure master

7. Verify deployment in portal

#### Task 3 - Promoting the Web App to Production ####

1. Promote from staging to production

1. Update Git remote to production

        git remote set-url azure https://<your-user>@<your-web-site>.scm.azurewebsites.net:443/<your-web-site>.git

### Exercise 4: Performing Deployment Rollback in Production ###
#### Task 1 - Updating the application ####

1. Open solution file in Source\Ex3\Begin

1. Insert buggy code in Startup.cs, before `app.UseMvc`. 

        app.Use((context, next) =>
        {
            var cultureQuery = context.Request.Query["culture"];
            var testCulture = "test-culture";
            if (!string.IsNullOrWhiteSpace(cultureQuery))
            {
                var culture = new System.Globalization.CultureInfo(testCulture);
        #if !DNXCORE50
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        #else
                System.Globalization.CultureInfo.CurrentCulture = culture;
                System.Globalization.CultureInfo.CurrentUICulture = culture;
        #endif
            }

            // Call the next delegate/middleware in the pipeline
            return next();
        });

#### Task 2 - Redeploying the application ####

1. Git push the changes

        git add .
        git commit -m "Added culture middleware"
        git push azure master

1. Browse to show HTTP 500

1. Go to portal blade > **Settings** > **Deployment Source** to redeploy (revert) previous commit. 

Optionally, revert using Git

    git revert HEAD --no-edit
    git push azure master

### Exercise 5: Identity with Azure Active Directory ###
#### Task 1 - Create an Azure Active Directory ####

1. Go to http://manage.windowsazure.com

1. Add new Azure AD

1. Add **Global Admin** user and remember temp password

1. Sign in to https://login.microsoftonline.com/ with the new user to change the password

#### Task 2 - Adding a new website to an organization ####

1. In VS, create new Web Application template

    - Use **Work and School Accounts** for authentication
    - Use the new Azure AD you created

1. `Ctrl-F5` to run app
  
1. Sign in using the global admin user you created

1. Go to http://manage.windowsazure.com and see new Azure AD application that's autocreated

#### Task 3 - Walk through the Web Application code ####

1. Open solution in Source/Ex5/End/

1. Startup.cs > `Configure` method > find Azure AD code in `app.UseOpenIdConnectAuthentication()`  

1. **Manage User Secrets** to look at secrets.json

1. In project.json, find `Microsoft.AspNet.Authentication.OpenIdConnect` in `dependencies`

## Summary ##

review