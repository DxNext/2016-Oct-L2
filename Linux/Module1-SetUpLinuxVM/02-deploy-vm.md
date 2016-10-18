# Using Azure CLI to provision the VM

1. We will be using Azure CLI, if you don't have it you can find it [here](https://azure.microsoft.com/en-us/documentation/articles/xplat-cli-install/)
1. We login into our account
    ```Shell
    azure login
    ...
    info:    Executing command login
    \info:    To sign in, use a web browser to open the page https://aka.ms/devicelogin. Enter the code XXXXXXXX to authenticate.
    info:    Added subscription Linux Apps - brmedi
    info:    login command OK
    ```
1. We need to make sure our subsc is on ARM mode type the following:
    ```Shell
    azure
    ```
    Notice that at the end of you will see the mode:
    ```Shell
    help:    Current Mode: arm (Azure Resource Management)
    ```
    If you are not using ARM you need to enter the following command:
    ```Shell
    azure config mode arm
    ...
    info:    Executing command config mode
    info:    New mode is arm
    info:    config mode command OK
    ```

1. We create a resource group for this excercise. The syntax is the following:
     ```Shell
    resource create [options] <resource-group> <name> <resource-type> <location> <api-version>
    ```
    For me it would be something like this:
    ```Shell
    azure group create my-centos-demo-name -l westus
    ```
1. Now lets deploy the vm using the JSON file of the template
     ```Shell
    azure group deployment create <resource-group> <my-deployment-name> --template-uri <arm-json-uri>
    ```
    It would look something  like this:
    ```Shell
    azure group deployment create my-centos-demo-name centos --template-uri hhttps://raw.githubusercontent.com/DxNext/2016-Oct-L2/master/Linux/Module1-SetUpLinuxVM/vm-details/azuredeploy.json
    ```
1. Please provide the values for the an username, ssh public key and a name for the VM.
1. After a few minutes we will have the following result:
    ```Shell
    data:    Outputs            :
    data:    Name        Type    Value
    data:    ----------  ------  ------------------------------------------------------
    data:    sshCommand  String  ssh usuario@XXXXXXXXXXXXXXX.westus.cloudapp.azure.com
    info:    group deployment create command OK
    ```
1. As you can notice we get a suggestion called "sshComand" that allows us to connect to our VM using [bash](content/01-set-up/03-connect-to-vm-bash.md), but we can also use [PuTTY](content/01-set-up/03-connect-to-vm-putty.md)

1. Lets verify our VM using the show command:
    ```Shell
    azure vm show <group-name> <vm-name>
    ```
    And we can see the details of our VM.
    Please, take a moment to verify that you have created a CentOS VM with two data disks, a public IP, and a proper FQDN. 
1. Alternatively, you could set up the VM fro the portal, just make sure you add you SSH public key.

    ![alt text][set-vm-up]


[set-vm-up]:../../img/set-vm-up.jpg "Fill it up with your info."
