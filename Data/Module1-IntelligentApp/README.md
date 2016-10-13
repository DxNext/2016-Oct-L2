# Intelligent Application Module 1 #

## Overview ##

In this module you will build the Azure backend for an automotive parts supplier called "Parts Unlimited".
The challenge here is to process millions of events from concurrent users connected from different devices across the globe. With **Azure Event Hubs** or **Azure IoT Hub** you can process large amounts of event data from connected devices and applications. These are managed services that ingest events with elastic scale to accommodate variable load profiles and the spikes caused by intermittent connectivity.
In this module we are going to be using IoT hubs to ingest our device data.  After you collect data into IoT Hubs, you can store the data using a storage cluster or transform it using a real-time analytics provider. **Azure Stream Analytics** is integrated out-of-the-box with Azure IoT Hubs to ingest millions of events per second. Stream Analytics processes ingested events in real-time, comparing multiple streams or comparing streams with historical values and models. It detects anomalies, transforms incoming data, triggers an alert when a specific error or condition appears in the stream, and displays this real-time data in your dashboard. For this scenario, you will use **Stream Analytics** to process and spool data to Blob Storage and Power BI.

### Objectives ###
In this module, you'll:
- Create an **IoT Hub**
- Register your device with the **IoT Hub**
- Use a device simulator to generate data
- Use **Stream Analytics** to process data in near-realtime and spool data to **Blob Storage** and **Power BI**
- Create a sample **Power BI** dashboard

### Prerequisites ###

The following is required to complete this module:
- [Visual Studio Community 2015][1] or greater
- [ASP.NET Core 1.0][2]
- [Microsoft Azure Storage Explorer][3] or any other tool to manage Azure Storage
- [Service Bus Explorer][4]
- An active Azure subscription

[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: https://docs.asp.net/en/latest/getting-started/installing-on-windows.html#install-asp-net-5-with-visual-studio
[3]: http://storageexplorer.com/
[4]: https://github.com/paolosalvatori/ServiceBusExplorer

## Exercises ##
This module includes the following exercises:

1. [Creating and integrating IoT Hubs](#Exercise1)
1. [Using Stream Analytics to process your data](#Exercise3)
1. [Visualizing your data with Power BI](#Exercise4)

Estimated time to complete this module: **75 minutes**

### Exercise 1: Creating and integrating IoT Hubs ###

Azure IoT Hubs is an event processing service that provides event and telemetry ingress to the cloud at massive scale, with low latency and high reliability. This service, used with other downstream services, is particularly useful in application instrumentation, user experience or workflow processing, and Internet of Things (IoT) scenarios.
In this exercise, you will use Azure IoT Hubs to track the user behavior in your retail website when viewing a product and also when adding it to the cart.

#### Task 1 - Creating the IoT Hub ####

There is a fantastic online resource already written that details exactly how to create your IoT hub and you can find it here [Getting Started](https://azure.microsoft.com/en-gb/documentation/articles/iot-hub-csharp-csharp-getstarted/).  The article then goes on to cover how you can register your device with the IoT Hub using C#.  We have already done this part for you.  From your IoT Hub though you will need to grab a few details and these are;

- The hub connection string (Settings | Shared access policies | iothubowner
- The hub host name

Now we have an IoT hub ready to receive events from our device/s

#### Task 2 - Creating a DocDB account

When you register your device (you will see how to do this in the next section), the application routine returns to you a key from the IoT Hub registry.  You use this key to authenticate yourself against IoT hub along with your device name.  You could retrieve this key from the device registry when you want to send data to the IoT hub but best practice says that you store the key in a durable store.  That durable store in this instance is DocumentDB.

To create your first DocumentDB account follow this article [Getting Started](https://azure.microsoft.com/en-gb/documentation/articles/documentdb-create-account/).  Now you need to do two more things
-  Create a DocumentDB database
-  Create a DocumentDB collection.  This will hold your IoT hub registration document.

Just as we did when creating the IoT Hub we are going to need to collect some details so we can connect to DocumentDB from our C# console application.

- The DocumentDB Key
- The Database name
- The Collection name
- The DocumentDB account name

OK so now we have an IoT hub and a DocumentDB account setup.  We are almost there for setup now.

#### Task 3 - Create a Storage account ####

One of the data repositories for our **Azure Streaming Analytics** jobs is blob storage.  We are going to need to setup and account now.
You can see exactly how to setup a storage acccount [here](https://azure.microsoft.com/en-gb/documentation/articles/storage-create-storage-account/).  We are going to create a **General Purpose Storage Account**  The article we just mentioned has a good discussion on the differences between the different types of account as well as the difference between hot and cold storage.
We won't need the details from this storage account until we start to use **Azure Streaming Analaytics**

Now we have things setup we can go ahead and register our device as well as start sending data to our IoT Hub.

#### Task 4 - Configuring and starting event generator application ####

In this task, you'll set up and run a console application that will randomly create and send events - such as add, view, checkout and remove - to your Event Hub. Later in this module, you'll visualize these events in Power BI.

1. Open in Visual Studio the **PartsUnlimitedDataGen.sln** solution located at **Source / Ex3 / Begin** folder.

You will notice there are two projects

- CreateDeviceIdentity - this will register your device with the IoTHub and store keys in DocumentDB.
- PartsUnlimitedDataGen - this is the simulated device.

This is now the point where you alter the app.config file in each project.

Settings for CreateDeviceIdentity
```xml
  <appSettings>
    <add key="hubconnectionstring" value="<hub connection string>" />
    <add key="docdbkey" value="Your DocDB Key" />
    <add key="databaseId" value="Your DocDB database" />
    <add key="collectionId" value="Your DocDB Collection Name" />
    <add key="docdburi" value="https://<DocDB account>.documents.azure.com:443/" />
  </appSettings>
```


Settings for PartsUnlimitedDataGen
```xml
    <appSettings>
        <add key="docdburi" value="https://<docdb account>.documents.azure.com:443/" />
        <add key="docdbkey" value="Your DocDB Key" />
        <add key="databaseId" value="Your DocDB database" />
        <add key="collectionId" value="Your DocDB Collection Name" />
        <add key="hubhostname" value="<IoT Hub account>.azure-devices.net" />
    </appSettings>
```


1. Build the solution to trigger the download of required NuGet packages.

1. Run the CreateDeviceIdentity application.  If everything is hooked up correctly then you should see your device key on the screen and if you navigate to the DocumentDB collection you just created you should see a document similar to this
```json
{
  "deviceId": "MY_POS_DEVICE1",
  "generationId": "636031296179141357",
  "etag": "\"MA==\"",
  "connectionState": "Disconnected",
  "status": "enabled",
  "statusReason": null,
  "connectionStateUpdatedTime": "0001-01-01T00:00:00",
  "statusUpdatedTime": "0001-01-01T00:00:00",
  "lastActivityTime": "0001-01-01T00:00:00",
  "cloudToDeviceMessageCount": 0,
  "authentication": {
    "symmetricKey": {
      "primaryKey": "8medevHT9l9u+9omMpRiOEx5XHGAkCftjM75nllJfl4=",
      "secondaryKey": "Ly1xW4NPH5NP9dlsqpLNH6ICwsp/9T62ugZhUI2u0k4="
    },
    "x509Thumbprint": {
      "primaryThumbprint": null,
      "secondaryThumbprint": null
    }
  },
  "id": "067a945c-6d0e-4484-bf3e-2d34be8b9317"
}
```

1. Now run the PartsUnlimitedDataGen project.  Again, if everything is hooked up the application should;
- Go to DocumentDb and retrieve your key
- Connect to IoT hub and start sending messages


You should now have events streaming to your IoT Hub.  We have always found it useful to actually look at the events as they land on the IoT hub because even though they may be getting there, they may not be in the format you think they are.  This is where you can use something like Device Explorer.

Paste in your IoT Hub Connection String --> Hit **Update** --> Now move to the **Data** tab and push the **monitor** button

If you wait a couple of seconds you should start to see events flowing through your IoT Hub

### Exercise 2: Using Stream Analytics to Analyse your Data ###

Now that we have a stream of events, you'll set up a Stream Analytics job to analyze these events in real-time.
Azure Stream Analytics (ASA) is a fully managed, cost effective real-time event processing engine that helps to unlock deep insights from data. Stream Analytics makes it easy to set up real-time analytic computations on data streaming from devices, sensors, web sites, social media, applications, infrastructure systems, and more.

#### Task 1 - Creating Stream Analytics job ####

In this task, you'll set up a Stream Analytics job to analyze the events in real-time.

1. In the [Azure portal](https://portal.azure.com/), click **New** > **Data + Analytics** > **Stream Analytics job**.
Specify the following values, and then click **Create**:

	- **Job Name**: Enter a job name.
	- **Region**: Select the region where you want to run the job. Consider placing the job and the event hub in the same region to ensure better performance and to ensure that you won't be paying to transfer data between regions.
	- **Storage Account**: Choose the Azure storage account that you'd like to use to store monitoring data for all Stream Analytics jobs running within this region. You have the option to choose an existing storage account or to create a new one.
	
1. The new job will be shown with a status of Created. Notice that the Start button is disabled. You must configure the job **Input**, **Output**, and **Query** before you can start the job.

#### Task 2 - Specifying Data Stream job Input ####

In this task, you'll specify a job Input using the IoT Hub you previously created.

1. In your Stream Analytics job topology click **Inputs**, and then click **Add**. The blade to create a new Input appears on the right.

1. Type or select the following values for each setting:

	- **Input Alias**: Enter a friendly name for this job input such as **IoTHubInput**. Note that you'll be using this name in the query later.
	- **Source Type**: Select Data Stream.
	- **Source**: IoT Hub.
	- **Subscription**: Use IoT Hub from current subscription
	- **IoT Hub**: Name of the IoT Hub
	- **Endpoint**: Messaging (other option is Operations monitoring)
	- **Shared access policy name**: service
	- **Consumer group**: $Default
	- **Event Serialization Format**: JSON.
	- **Encoding**: UTF8.

1. Click **Create**.

#### Task 3 - Specifying job Query ####

Stream Analytics supports a simple, declarative query model for describing transformations for real-time processing. To learn more about the language, see the [Azure Stream Analytics Query Language Reference](https://msdn.microsoft.com/library/dn834998.aspx).
In this task, you'll create a query that extracts events from your input stream.

1. Click **Query** from the main Stream Analytics job page.

1. Add the following to the code editor:

	```sql
	SELECT
	    *
	INTO
	    RawBlobOutput
	FROM
	    IoTHubInput
	```

1. Click **Save**.

	>**Note:** In this query you're projecting all fields in the payload of the event to the output, you could read some of them of them by using _SELECT [field name]_.


#### Task 4 - Specifying job Output ####

In this task, you'll create an output that will store the query results in Blob storage.  We created a storage account earlier

1. Create a Container such as eventhubanalytics and set its access to Blob.

	1. Open the **Azure Storage Explorer** or the tool of your preference and configure a new storage account using the account name and key from the storage account you created. In _Azure Storage Explorer_, right-click on **Storage Accounts**, select **Attach External Storage...** and enter the account name and key in the dialog, then click **OK**.

	1. Create a new Blob Container with the name "**eventhubanalytics**" and "Container" access level. In _Azure Storage Explorer_ expand your account and right-click on **Blob Containers**, select **Create Blob Container** and enter "eventhubanalytics". Press enter to create the container. Then right-click on the new container and select **Set Public Access Level..** and choose **Public read access for blobs**.

1. Now, in your Stream Analytics job, click **Outputs** from the main page, and then click **Add**. The options blade requires the following information:

	- **Output alias**: Set a friendly name to use in the query. We will be using **RawBlobOutput** as our friendly name.
	- **Sink**: Blob storage.
	- **Storage account**: Select the name of the storage account.
	- **Storage account key**: Set the account key.
	- **Container**: Select the name of the container.
	- **Path pattern**: Type in a file prefix to use when writing blob output. E.g. analyticsoutput-{date}
	- **Event Serializer Format**: JSON.
	- **Encoding**: UTF8.
	- **Format**: Line Separated

1. Click **Create**.

#### Task 5 - Starting the job for real time processing ####

In this task you'll run the Stream Analytics job and view the output in Visual Studio Storage Explorer.

1. From the job **Dashboard**, click the **Start** button.

1. In the **Start job** blade, select Job output start time **Now**, and then click **Start** at the bottom of the blade. The job status will change to Starting; it can take several minutes to start.

1. Open the blob storage using **Azure Storage Explorer**.

1. Navigate to the container you set in the previous task.




### Exercise 3: Visualizing your data with Power BI (optional - requires Organizational account ###

In this exercise, you'll use Azure Stream Analytics with Microsoft Power BI. You will learn how to build a live dashboard quickly. You will also learn how to perform a JOIN operation in Azure Steam Analytics.


#### Task 1 - Adding an input for Reference Data ####
Since our streaming data only contains productId, we need to Join our input stream to our product catalog data in order to get meaningful results. In order to do that, we will first add the product catalog data as a reference dataset to the stream analytics query

1. From the [Azure portal](https://portal.azure.com/), go to Stream Analytics and click the one you created.

1. Click the **STOP** button at the top of the page. We need to stop it in order to add a new input/output.

1. Click **INPUTS** in the middle of the page, and then click **+ Add**.

1. In the **New Input** dialog box, make sure the following options are selected.

	- **Input Alias**: Enter a friendly name for this job input such as **RefData**. Note that you'll be using this name in the query later.
	- **Source Type**: Select *Reference Data*.
	- **Source**: Select *Use blob storage from current subscription*.
	- **Storage Account**: Select the storage account that contains your product catalog file.
	- **Container**: Select the container that contains your product catalog file (eg: processeddata).
	- **path pattern**: Enter the path that contains your product catalog file (eg: productcatalog.json). 
	- **Date Format**: Needs to be non-editable
	- **Time Format**: Needs to be non-editable
	- **Event Serialization Format**: JSON.
	- **Encoding**: UTF8.


#### Task 2 - Adding Power BI output to Stream Analytics ####

In this task, you'll add a new output to your Stream Analytics job.

1. From the [Azure portal](https://portal.azure.com/), go to Stream Analytics and click the one you created.

1. Click the **STOP** button below. We need to stop it in order to add a new output.

1. Click **OUTPUTS** from the top of the page, and then click **Add Output**.

1. In the **Add an Output** dialog box, select **Power BI** and then click the right button.

1. In the **Add a Microsoft Power BI output**, supply a work or school account for the Stream Analytics job output. If you already have Power BI account, select **Authorize Now**. If not, choose **Sign up now**.

1. Next, provide the values for:

	- **Output Alias** – You can put any output alias that is easy for you to refer to. This output alias is particularly helpful if you decide to have multiple outputs for your job. In that case, you have to refer to this output in your query. For example, let’s use the output alias value “AbandonedCartsPowerBI”.
	- **Dataset Name** - Provide a dataset name that you want your Power BI output to have. For example, let’s use “datamodulepbi”.
	- **Table Name** - Provide a table name under the dataset of your Power BI output. Let’s say we call it “datamodulepbi”. Currently, Power BI output from Stream Analytics jobs may only have one table in a dataset.
	- **Workspace** - You can use the default, My Workspace.

1. Click **OK, Test Connection** and now your Power BI connection is completed.

1. Lastly, you should update your **Query** to use this output and **start** the job.

	```sql
	WITH AbandonedCarts as (
    SELECT 
        a.userId, a.productId, a.EventDate 
    FROM 
        IoTHubInput as A TIMESTAMP BY EventDate
    LEFT OUTER JOIN IoTHub as B TimeStamp By EventDate
    ON a.userId=b.userId AND a.productId = b.productId and b.type='checkout'
    AND DATEDIFF(minute, A, B) BETWEEN 0 AND 5
    WHERE a.type = 'add'
    AND b.type IS NULL
	)

	SELECT a.productId, b.title, b.category.name, MIN(a.EventDate) as eventStartTime, count(a.productId)
	INTO AbandonedCartsPowerBI
	FROM AbandonedCarts AS a
	JOIN RefData as B
	ON a.productId = b.productId
	GROUP BY a.productId, b.title, b.category.name, TUMBLINGWINDOW(minute, 5)

	SELECT * INTO RawBlobOutput FROM IoTHubInput TIMESTAMP BY EventDate
	```

	>**Note:** As we are grouping the results, a window type is required. See [GROUP BY](https://msdn.microsoft.com/library/azure/dn835023.aspx). The query uses a 10-minute tumbling window. The INTO clause tells Stream Analytics which of the outputs to write the data from this statement. The WITH statement is to reuse the results for different statements; in this case we could used it for both outputs but we'll keep storing all fields into the blob.


1. (Optional): You can also add another PowerBI Output for tracking the overall activity of your e-commerce store. You can do this by repeating steps 4-7 of **Task 2** and changing the alias and the dataset name. You can use the query below to perform the aggregations. We have used **AllProductsPowerBI** as our PowerBI Output alias.

	```sql

	WITH AllEvents AS (
	SELECT
	    productId, type, Count(CAST(productId as String)) AS [total]
	FROM
	    IoTHubInput TIMESTAMP BY EventDate
	GROUP BY
	    productId, type, TumblingWindow(minute, 5)
	)

	SELECT a.productId, a.type, b.title, b.category.name, a.total INTO AllProductsPowerBI FROM AllEvents a JOIN RefData as b on a.productId = b.productId

	```
	
Overall, your query should look as follows:

	```sql

	WITH AllEvents AS (
	SELECT
	    productId, type, Count(CAST(productId as String)) AS [total]
	FROM
	    IoTHubInput TIMESTAMP BY EventDate
	GROUP BY
	    productId, type, TumblingWindow(minute, 5)
	),
	AbandonedCarts as (
    SELECT 
        a.userId, a.productId, a.EventDate 
    FROM 
        IoTHubInput as A TIMESTAMP BY EventDate
    LEFT OUTER JOIN IoTHub as B TimeStamp By EventDate
    ON a.userId=b.userId AND a.productId = b.productId and b.type='checkout'
    AND DATEDIFF(minute, A, B) BETWEEN 0 AND 5
    WHERE a.type = 'add'
    AND b.type IS NULL
	)

	SELECT a.productId, b.title, b.category.name, MIN(a.EventDate) as eventStartTime, count(a.productId)
	INTO AbandonedCartsPowerBI
	FROM AbandonedCarts AS a
	JOIN RefData as B
	ON a.productId = b.productId
	GROUP BY a.productId, b.title, b.category.name, TUMBLINGWINDOW(minute, 5)
	     
	SELECT a.productId, a.type, b.title, b.category.name, a.total INTO AllProductsPowerBI FROM AllEvents a JOIN RefData as b
	on a.productId = b.productId
	
	SELECT * INTO RawBlobOutput FROM IoTHubInput TIMESTAMP BY EventDate
	```

#### Task 3 - Creating the dashboard in Power BI ####

1. Go to [PowerBI.com](https://powerbi.microsoft.com/) and login with your work or school account. If the Stream Analytics job query outputs results, you'll see your dataset is already created:

1. For creating the dashboard, go to the **Dashboards** option and create a new Dashboard, e.g. My Dashboard.

1. Now click the dataset created by your Stream Analytics job (“datamodulepbi” in our current example). You will be taken to a page to create a chart on top of this dataset.



1. Select the **Table visualization** icon from the **Visualizations** menu on the right, then check all fields but productId from the **Fields** list.

1. Within the **Filters** section, click **category** Advanced filtering and select the option to show items when the value _is not blank_.

1. Apply the filter and click **Save** on the top right. You can name it "Events report".

1. You will see the new report within the **Reports** section, click on it, select **Pin Live Page** to your existing dashboard.

1. Go to your dashboard, click on the **ellipsis** button at the top-right corner of the tile and click the **pen** button to edit the _Tile details_, select the **Display last refresh time** functionality and apply the changes.



1. Go back to the "datamodulepbi" dataset, click the **Funnel** icon from the **Visualization** menu and check _type_ and _total_ from the **Fields** list.

1. Click **Save** on the top right, enter a name like Events Summary and save it.

1. Lets add it to your dashboard by clicking **Pin Live Page** on the top right, select your dashboard and hit **Pin Live**.



---

## Summary ##

By completing this module, you should have:

- Created an **IoT Hub**
- Walked through **DocumentDB** integration
- Used **Stream Analytics** to process data in near-realtime and spool data to **Blob Storage** and **Power BI**
- Created sample **Power BI** charts & graphs

