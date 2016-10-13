azure login

azure account list

azure account set <SubscriptionID>

azure group create <ResourceGroupName> <location>

azure group deployment create -f <PathToTemplate> -e <PathToParameterFile> -g <ResourceGroupName> -n <NameForDeployment>



conn_string = azure storage account connectionstring show storage_account_name      //TODO: Not sure if you can run commands like this and copy the output. Please check

export AZURE_STORAGE_CONNECTION_STRING= conn_sring

// Upload Product Catalog file
echo "Creating the container..."
azure storage container create catalog_container_name

//Generate & Upload logs

