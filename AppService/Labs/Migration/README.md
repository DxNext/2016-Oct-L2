# Migration Labs

Chances are you already have an application you're running locally that you'd like to deploy to Azure App Services. Or, maybe you are using a tool such as Sublime, or Visual Studio Code, and don't have access to the cool right-click menus offered in Visual Studio Community (or Professional). If that's the case, you have several options available to you when it comes to deploying your application to App Services.

You can obtain the necessary software and source code by following the [setup instructions](setup.md).

## Overall Migration Steps

Regardless of how you intend to deploy your application to App Services, you have two main steps; you can either deploy your application first, or you can deploy your data store. App Services supports both scenarios. This allows you to migrate your application piecemeal, ensuring uptime during your migration, or the ability to test your application without placing all services into the cloud.

## Application First

If you decide to deploy your application first, you can keep your data store local by using [Hybrid Connections](https://azure.microsoft.com/en-us/documentation/articles/integration-hybrid-connection-overview/). Hybrid Connections enable resources hosted on-premises, such as SQL Server deployments, to appear to an application hosted in App Services as a cloud deployed, or local, service. Hybrid Connections work with any type of application, including ASP.NET, PHP and Node.js.

### Application First with ASP.NET

If you have an ASP.NET application that you'd like to deploy, while keeping your SQL Server local, you can follow the steps in **[Migrate ASP.NET site with local SQL Server using Hybrid Connections](asp-first.md)**.

### Application First with Node.js

If you have a Node.js application that you'd like to deploy, while keeping your MongoDB server local, you can follow the steps in **[Continuous Deployment using GitHub](node-github.md)**. The lab uses GitHub to ease the deployment process, but gives you the additional bonus of continuous deployment.

## Data Store First

Deploying your data store first enables you to take advantage of features such as replication and potentially enhanced service level agreements (SLAs), while keeping your application on your on-premises servers. Both SQL Server, and DocumentDB, allow access from clients located elsewhere, including non-cloud locations.

### Data Store First with ASP.NET

You can migrate your SQL Server database to Microsoft Azure SQL Database, while keeping your application local, by following the steps in **[Migrate SQL Server with local ASP.NET site](sql-first.md)**. One important thing to note when making the migration is you are required to open a hole in the firewall to allow your on-premises servers to connect to Azure SQL Database by identifying your public-facing IP address. If your IP address is prone to change, you may want to consider enabling a range.

### Data Store First with ASP.NET

You can create a NoSQL server on Microsoft Azure as a managed service (Platform as a Service, or PaaS), by using [https://azure.microsoft.com/en-us/documentation/articles/documentdb-protocol-mongodb/](Azure DocumentDB with MongoDB protocol support), which supports many of the same calls that MongoDB supports. You can walk through the steps by following **[Migrating MongoDB to DocumentDB](mongodb-first.md)**.