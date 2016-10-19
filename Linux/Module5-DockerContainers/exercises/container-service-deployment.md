# Deploy an Azure Container Service cluster

This document walks you through deploying a Docker Swarm Cluster on Azure Container Services by using the [Azure portal](#creating-a-service-using-the-azure-portal), the [Azure command-line interface (CLI)](#creating-a-service-using-the-azure-cli), and the [Azure PowerShell module](#creating-a-service-using-powershell).

Choose on of the deployment options mentioned above.

## Create a service by using the Azure portal

Sign in to the Azure portal, select **New**, and search the Azure Marketplace for **Azure Container Service**.

![Create deployment 1](images/acs-portal1.png)  <br />

Select **Azure Container Service**, and click **Create**.

![Create deployment 2](images/acs-portal2.png)  <br />

Enter the following information:

- **User name**: This is the user name that will be used for an account on each of the virtual machines and virtual machine scale sets in the Azure Container Service cluster.
- **Subscription**: Select an Azure subscription.
- **Resource group**: Select an existing resource group, or create a new one.
- **Location**: Select an Azure region for the Azure Container Service deployment.
- **SSH public key**: Add the public key that will be used for authentication against Azure Container Service virtual machines. It is very important that this key contains no line breaks, and that it includes the 'ssh-rsa' prefix and the 'username@domain' postfix. It should look something like the following: **ssh-rsa AAAAB3Nz...<...>...UcyupgH azureuser@linuxvm**. For guidance on creating Secure Shell (SSH) keys, see the [Linux]( https://azure.microsoft.com/documentation/articles/virtual-machines-linux-ssh-from-linux/) and [Windows]( https://azure.microsoft.com/documentation/articles/virtual-machines-linux-ssh-from-windows/) articles.

Click **OK** when you're ready to proceed.

![Create deployment 3](images/acs-portal3.png)  <br />

Select **"Swarm"** as the Orchestration type. The options are:

Click **OK** when you're ready to proceed.

![Create deployment 4](images/acs-portal4.png)  <br />

Enter the following information:

- **Master count**: The number of masters in the cluster.
- **Agent count**: For Docker Swarm, this will be the initial number of agents in the agent scale set. For DC/OS, this will be the initial number of agents in a private scale set. Additionally, a public scale set is created, which contains a predetermined number of agents. The number of agents in this public scale set is determined by how many masters have been created in the cluster--one public agent for one master, and two public agents for three or five masters.
- **Agent virtual machine size**: The size of the agent virtual machines.
- **DNS prefix**: A world unique name that will be used to prefix key parts of the fully qualified domain names for the service.

Click **OK** when you're ready to proceed.

![Create deployment 5](images/acs-portal5.png)  <br />

Click **OK** after service validation has finished.

![Create deployment 6](images/acs-portal6.png)  <br />

Click **Create** to start the deployment process.

![Create deployment 7](images/acs-portal7.png)  <br />

If you've elected to pin the deployment to the Azure portal, you can see the deployment status.

![Create deployment 8](images/acs-portal8.png)  <br />

When the deployment has completed, the Azure Container Service cluster is ready for use.

## Create a service by using the Azure CLI

To create an instance of Azure Container Service by using the command line, you need an Azure subscription. If you don't have one, then you can sign up for a [free trial](http://azure.microsoft.com/pricing/free-trial/?WT.mc_id=AA4C1C935). You also need to have [installed](../xplat-cli-install.md) and [configured](../xplat-cli-connect.md) the Azure CLI.

To deploy a Docker Swarm cluster, select one of the following templates from GitHub.

* [Swarm template](https://github.com/Azure/azure-quickstart-templates/tree/master/101-acs-swarm)

Next, make sure that the Azure CLI has been connected to an Azure subscription. You can do this by using the following command:

```bash
azure account show
```
If an Azure account is not returned, use the following command to sign the CLI in to Azure.

```bash
azure login -u user@domain.com
```

Next, configure the Azure CLI tools to use Azure Resource Manager.

```bash
azure config mode arm
```

Create an Azure resource group and Container Service cluster with the following command, where:

- **RESOURCE_GROUP** is the name of the resource group that you want to use for this service.
- **LOCATION** is the Azure region where the resource group and Azure Container Service deployment will be created.
- **TEMPLATE_URI** is the location of the deployment file. Note that this must be the Raw file, not a pointer to the GitHub UI. To find this URL, select the azuredeploy.json file in GitHub, and click the **Raw** button.

> [AZURE.NOTE] When you run this command, the shell will prompt you for deployment parameter values.

```bash
azure group create -n RESOURCE_GROUP DEPLOYMENT_NAME -l LOCATION --template-uri TEMPLATE_URI
```

### Provide template parameters

This version of the command requires you to define parameters interactively. If you want to provide parameters, such as a JSON-formatted string, you can do so by using the `-p` switch. For example:

 ```bash
azure group deployment create RESOURCE_GROUP DEPLOYMENT_NAME --template-uri TEMPLATE_URI -p '{ "param1": "value1" … }'
```

Alternatively, you can provide a JSON-formatted parameters file by using the `-e` switch:

```bash
azure group deployment create RESOURCE_GROUP DEPLOYMENT_NAME --template-uri TEMPLATE_URI -e PATH/FILE.JSON
```

To see an example parameters file named `azuredeploy.parameters.json`, look for it with the Azure Container Service templates in GitHub.

## Create a service by using PowerShell

You can also deploy an Azure Container Service cluster with PowerShell. This document is based on the version 1.0 [Azure PowerShell module](https://azure.microsoft.com/blog/azps-1-0/).

To deploy a DC/OS or Docker Swarm cluster, select one of the following templates.

* [Swarm template](https://github.com/Azure/azure-quickstart-templates/tree/master/101-acs-swarm)

Before creating a cluster in your Azure subscription, verify that your PowerShell session has been signed in to Azure. You can do this with the `Get-AzureRMSubscription` command:

```powershell
Get-AzureRmSubscription
```

If you need to sign in to Azure, use the `Login-AzureRMAccount` command:

```powershell
Login-AzureRmAccount
```

If you're deploying to a new resource group, you must first create the resource group. To create a new resource group, use the `New-AzureRmResourceGroup` command, and specify a resource group name and destination region:

```powershell
New-AzureRmResourceGroup -Name GROUP_NAME -Location REGION
```

After you create a resource group, you can create your cluster with the following command. The URI of the desired template will be specified for the `-TemplateUri` parameter. When you run this command, PowerShell will prompt you for deployment parameter values.

```powershell
New-AzureRmResourceGroupDeployment -Name DEPLOYMENT_NAME -ResourceGroupName RESOURCE_GROUP_NAME -TemplateUri TEMPLATE_URI
```

### Provide template parameters

If you're familiar with PowerShell, you know that you can cycle through the available parameters for a cmdlet by typing a minus sign (-) and then pressing the TAB key. This same functionality also works with parameters that you define in your template. As soon as you type the template name, the cmdlet fetches the template, parses the parameters, and adds the template parameters to the command dynamically. This makes it very easy to specify the template parameter values. And, if you forget a required parameter value, PowerShell prompts you for the value.

Below is the full command, with parameters included. You can provide your own values for the names of the resources.

```powershell
New-AzureRmResourceGroupDeployment -ResourceGroupName RESOURCE_GROUP_NAME-TemplateURI TEMPLATE_URI -adminuser value1 -adminpassword value2 ....
```
Wait for approximately 10 minutes for the Docker Swarm cluster to be created.

Now that your Docker Swarm cluster is ready, it's time to connect.

# Connect to an Azure Container Service cluster

The Docker Swarm clusters that are deployed by Azure Container Service expose REST endpoints. However, these endpoints are not open to the outside world. In order to manage these endpoints, you must create a Secure Shell (SSH) tunnel. After an SSH tunnel has been established, you can run commands against the cluster endpoints and view the cluster UI through a browser on your own system. This document walks you through creating an SSH tunnel from Linux, OS X, and Windows.

## Create an SSH tunnel on Linux or OS X

The first thing that you do when you create an SSH tunnel on Linux or OS X is to locate the public DNS name of load-balanced masters. To do this, expand the resource group so that each resource is being displayed. Locate and select the public IP address of the master. This will open up a blade that contains information about the public IP address, which includes the DNS name. Save this name for later use. <br />


![Public DNS name](images/pubdns.png)

Now open a shell and run the following command where:

**PORT** is the port of the endpoint that you want to expose. For Swarm, this is 2375. 
**USERNAME** is the user name that was provided when you deployed the cluster.  
**DNSPREFIX** is the DNS prefix that you provided when you deployed the cluster.  
**REGION** is the region in which your resource group is located.  
**PATH_TO_PRIVATE_KEY** [OPTIONAL] is the path to the private key that corresponds to the public key you provided when you created the Container Service cluster. Use this option with the -i flag.

```bash
ssh -L PORT:localhost:PORT -f -N [USERNAME]@[DNSPREFIX]mgmt.[REGION].cloudapp.azure.com -p 2200
```
> The SSH connection port is 2200--not the standard port 22.

## Swarm tunnel

To open a tunnel to the Swarm endpoint, execute a command that looks similar to the following:

```bash
ssh -L 2375:localhost:2375 -f -N azureuser@acsexamplemgmt.japaneast.cloudapp.azure.com -p 2200
```

Now you can set your DOCKER_HOST environment variable as follows. You can continue to use your Docker command-line interface (CLI) as normal.

```bash
export DOCKER_HOST=:2375
```

## Create an SSH tunnel on Windows

There are multiple options for creating SSH tunnels on Windows. This document will describe how to use PuTTY to do this.

Download PuTTY to your Windows system and run the application.

Enter a host name that is comprised of the cluster admin user name and the public DNS name of the first master in the cluster. The **Host Name** will look like this: `adminuser@PublicDNS`. Enter 2200 for the **Port**.

![PuTTY configuration 1](images/putty1.png)

Select **SSH** and **Authentication**. Add your private key file for authentication.

![PuTTY configuration 2](images/putty2.png)

Select **Tunnels** and configure the following forwarded ports:
- **Source Port:** Use 2375 for Swarm.
- **Destination:** Use localhost:2375 for Swarm.

The following example is configured for DC/OS, but will look similar for Docker Swarm.

>[AZURE.NOTE] Port 80 must not be in use when you create this tunnel.

![PuTTY configuration 3](images/putty3.png)

When you're finished, save the connection configuration, and connect the PuTTY session. When you connect, you can see the port configuration in the PuTTY event log.

![PuTTY event log](images/putty4.png)

When you've configured the tunnel for Docker Swarm, you can access the Swarm cluster through the Docker CLI. You will first need to configure a Windows environment variable named `DOCKER_HOST` with a value of ` :2375`.

