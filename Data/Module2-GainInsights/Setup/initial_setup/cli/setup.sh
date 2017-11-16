#!/bin/bash
clear

startTime=$(date)

declare resourceGroupName=""
declare location=""
pathToTemplate="./parameters.json"
pathToParameterFile="./azuredeploy.json"
deploymentName="deploy$resourceGroupName"

while getopts ":g:l:" arg; do
	case "${arg}" in
        g)
            resourceGroupName=${OPTARG}
            ;;
		l)
			location=${OPTARG}
            ;;
		esac
done
shift $((OPTIND-1))

echo "ResourceGroupName: $resourceGroupName"
echo "SubscriptionId: $location"

#Basic environment configuration
azure config mode arm

#Template deployment
azure group create $resourceGroupName $location
azure group deployment create -f $pathToTemplate -e $pathToParameterFile -g $resourceGroupName -n $deploymentName

#Get storage account information
storageAccountName=$(azure storage account list -g $resourceGroupName | grep $resourceGroupName | awk '{print $2}')
storageAccountKey=$(azure storage account keys list $storageAccountName -g $resourceGroupName | grep key1 | awk '{print $3}')

#Create Azure Storage Blob Containers
uploadContainerName="partsunlimited"
processContainerName="processeddata"

azure storage container create -p Off -a $storageAccountName -k $storageAccountKey --container $uploadContainerName
azure storage container create -p Off -a $storageAccountName -k $storageAccountKey --container $processContainerName

#Upload files
shopt -s globstar
assetsFolderPath="../../Assets/"
assetsFolderPathLength=${#assetsFolderPath}

logsFolderPath=${assetsFolderPath}logs
allLogFiles=$logsFolderPath/**/*.txt
for f in $allLogFiles
do
    fileName=${f:assetsFolderPathLength}
    echo "Uploading $f"
    azure storage blob upload --blobtype block --blob $fileName --file $f --container $uploadContainerName --account-name $storageAccountName --account-key $storageAccountKey --concurrenttaskcount 100 --quiet
done

hdInsightFolderPath=${assetsFolderPath}HDInsight/
hdInsightFolderPathLength=${#hdInsightFolderPath}
scriptsFolderPath=${hdInsightFolderPath}Scripts
allScriptsFiles=$scriptsFolderPath/**/*.hql
for f in $allScriptsFiles
do
    fileName=${f:hdInsightFolderPathLength}
    echo "Uploading $f"
    azure storage blob upload --blobtype block --blob $fileName --file $f --container $uploadContainerName --account-name $storageAccountName --account-key $storageAccountKey --concurrenttaskcount 100 --quiet
done

endTime=$(date)

echo "Start time: $startTime"
echo "End time: $endTime"