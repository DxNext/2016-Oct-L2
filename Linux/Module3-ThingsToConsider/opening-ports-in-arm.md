# Opening ports to a Linux VM in Azure
You open a port, or create an endpoint, to a virtual machine (VM) in Azure by creating a network filter on a subnet or VM network interface. You place these filters, which control both inbound and outbound traffic, on a Network Security Group attached to the resource that receives the traffic.

## Quick commands
To create a Network Security Group and rules you need the Azure CLI in Resource Manager mode:

```
azure config mode arm
```

Create your Network Security Group, **entering your own names and location appropriately**:

**TIP:** You can list the Network Security Groups (NSG) in a resource group using the following command:
```
azure network nsg list --resource-group <your-resource-group>
```

**Example output:**
info:    Executing command network nsg list
Getting the network security groups
data:    Name                        Location  Resource group   Provisioning state  Rules number
data:    --------------------------  --------  ---------------  ------------------  ------------
data:    oguzp-centos-72-2-nsg       westus    field-readiness  Succeeded           7
info:    network nsg list command OK


## Opening port TCP 5000
```
azure network nsg rule create --protocol tcp --priority 1010 --direction inbound --destination-port-range 5000 --access allow --resource-group <your-resource-group> --nsg-name <your-nsg-name> --name TCP5000
```
## Opening port TCP 5001
```
azure network nsg rule create --protocol tcp --priority 1020 --direction inbound --destination-port-range 5001 --access allow --resource-group <your-resource-group> --nsg-name <your-nsg-name> --name TCP5001
```

