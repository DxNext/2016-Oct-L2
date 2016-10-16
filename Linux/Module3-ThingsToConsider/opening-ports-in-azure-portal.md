# Opening ports to a VM in Azure using the Azure Portal

The guide is based in port 80, you may need to change the port depending on the port that your application uses.

## Quick commands

First, create your Network Security Group. Select a resource group in the portal, click 'Add', then search for and select 'Network Security Group':

![Add a Network Security Group](./media/add-nsg.png)

Enter a name for your Network Security Group and select a location:

![Create a Network Security Group](./media/create-nsg.png)

Select your new Network Security Group. You now create an inbound rule:

![Add an inbound rule](./media/add-inbound-rule.png)

Provide a name for your new rule. Port 80 is already entered by default. This blade is where you would change the source, protocol, and destination when adding additional rules to your Network Security Group:

![Create an inbound rule](./media/create-inbound-rule.png)

Your final step is to associate your Network Security Group with a subnet or a specific network interface. Let's associate the Network Security Group with a subnet:

![Associate a Network Security Group with a subnet](./media/associate-subnet.png)

Select your virtual network, and then select the appropriate subnet:

![Associating a Network Security Group with virtual networking](./media/select-vnet-subnet.png)

You have now created a Network Security Group, created an inbound rule that allows traffic on port 80, and associated it with a subnet. Any VMs you connect to that subnet are reachable on port 80.