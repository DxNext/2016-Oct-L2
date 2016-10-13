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


[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: https://www.microsoft.com/en-us/download/details.aspx?id=53591



<a name="Setup"></a>
### Setup ###
In order to work on this exercise, it is recommended that you complete module 2. However, if you have not completed module 2, you can still work on this module.

>**Note:** If you've completed Module 2, you may skip the setup and start with Exercise #1.

