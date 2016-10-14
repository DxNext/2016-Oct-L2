# Migration Lab Setup

The goal of this document is to help you prepare for the migration labs. The requirements for this lab are relatively minimal, and chances are you already meet them, save for the source code for the applications, which you can obtain from GitHub. In order to complete the labs, you will need the following pieces of software installed on either your local system or a virtual machine.

For all labs, you will need a Git client installed. If you are using Windows, you can use [GitHub Desktop](https://desktop.github.com/).

## ASP.NET labs

### Software

- [Visual Studio Community](https://www.visualstudio.com/vs/community/)
- [SQL Server 2016 Developer edition](https://www.microsoft.com/en-us/cloud-platform/sql-server-editions-developers)

### Source code

(To be uploaded soon)

## Node.js labs

### Software

- [Visual Studio Code](https://code.visualstudio.com/) (or another text editor)
- [MongoDB Community Server](https://www.mongodb.com/download-center?jmp=nav#community)

### Source code

The source code is available on [GitHub](https://github.com/GeekTrainer/node-invoicing). To create the database, ensure that MongoDB is running and execute the following command:

``` console
node createdata.js
```