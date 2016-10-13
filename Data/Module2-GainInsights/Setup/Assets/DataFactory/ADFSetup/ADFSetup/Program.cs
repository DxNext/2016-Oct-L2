using Microsoft.Azure;
using Microsoft.Azure.Management.DataFactories;
using Microsoft.Azure.Management.DataFactories.Models;
using Microsoft.Azure.Management.DataFactories.Common.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Azure.Management.Resources;

namespace ADFSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: Convert this to Args
            //string subscriptionID = "7ceb3e0d-1ecf-4c8e-b398-5f4f8bb4e768";
            //string resourceGroupName = "buildworkshop";
            //string dataFactoryName = "PartsUnlimitedDataFactory";
            //string prefixNumber = "";
            //string part = "All";

            /*
            if (args.Count()<4)
            {
                Console.Error.WriteLine("We need exacty 4 arguments: <SubscriptionID> <ResourceGroupName> <DataFactoryName> <part1/part2/all>");
                Console.WriteLine("Press Enter to exit....");
                Console.ReadLine();
                Environment.Exit(1);
            }

            

            string subscriptionID = args[0];
            string resourceGroupName = args[1];
            string dataFactoryName = args[2];
            //string prefixNumber = "";
            string part = args[3];
            */

            Console.WriteLine("Please enter the subscriptionID:");
            string subscriptionID = Console.ReadLine();

            Console.WriteLine("Please enter the Resource Group Name:");
            string resourceGroupName = Console.ReadLine();

            Console.WriteLine("Please enter the Data Factory Name:");
            string dataFactoryName = Console.ReadLine();

            //Change this to "all" if you'd like to automate the entire DF creation. 
            //Other options are "part1" and "part2"
            //In order for this to work, the Data factory must already be created.
            string part = "all";

            int DaysToRunTheFactory = -4;
            Uri resourceManagerUri = new Uri(ConfigurationManager.AppSettings["ResourceManagerEndpoint"]);
            var StartTime = new DateTime(DateTime.UtcNow.AddDays(DaysToRunTheFactory).Year, DateTime.UtcNow.AddDays(DaysToRunTheFactory).Month, DateTime.UtcNow.AddDays(DaysToRunTheFactory).Day, 0, 0, 0, DateTimeKind.Utc);
            var EndTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);
            var dataPipelineName = "JsonLogsToTabularPipeline";

            var DefinesVars = new Dictionary<string, string>();
            DefinesVars.Add("Year", "$$Text.Format('{0:yyyy}', SliceStart)");  //TODO: Make sure it is Slice Start and not Window Start.
            DefinesVars.Add("Month", "$$Text.Format('{0:MM}', SliceStart)");
            DefinesVars.Add("Day", "$$Text.Format('{0:dd}', SliceStart)");


            TokenCloudCredentials aadTokenCredentials =
                new TokenCloudCredentials(
                    subscriptionID,
                        GetAuthorizationHeader());

            //****Linked Service Names****//
            string SQLDWServerLSName = "";
            string hdinsightLSName = "";
            string storageAccountLSName = "";

            var storageAccountName = "";

            DataFactoryManagementClient client = new DataFactoryManagementClient(aadTokenCredentials, resourceManagerUri);

            //Check if Data Factory Exists
            DataFactoryGetResponse factory = null;
            try
            {
                factory = client.DataFactories.Get(resourceGroupName, dataFactoryName) ?? null;
            }
            catch(Exception e)
            {
                factory = null;
            }
            
            if(factory == null)
            {
                Console.Error.WriteLine("The Data Factory not found. Please go to the Azure Portal, create a data factory and try running this program again!");
                Console.ReadLine();
                Environment.Exit(1);
            }

            //Fetching LinkedServices
            var LS = client.LinkedServices.List(resourceGroupName, dataFactoryName);
            var DS = client.Datasets.List(resourceGroupName, dataFactoryName);
            var Pipelines = client.Pipelines.List(resourceGroupName, dataFactoryName);

            foreach (var linkedS in LS.LinkedServices)
            {
                if (linkedS.Properties.Type.Equals("AzureStorage"))
                {
                    storageAccountLSName = linkedS.Name;
                }
                else if (linkedS.Properties.Type.Equals("HDInsight"))
                {
                    hdinsightLSName = linkedS.Name;
                }
                else if (linkedS.Properties.Type.Equals("AzureSqlDW"))
                {
                    SQLDWServerLSName = linkedS.Name;
                }
            }

            //Creating Datasets
            var rawBlobDataSet = "";
            var tempBlobDataSet = "";
            var rawSQLDataset = "";
            foreach (var dataset in DS.Datasets)
            {
                //var typeProperties = (AzureBlobDataset) dataset.Properties.TypeProperties;
                if ((dataset.Name.ToLower().Contains("json") || dataset.Name.ToLower().Contains("raw")) && dataset.Properties.Type.Equals("AzureBlob"))
                {
                    rawBlobDataSet = dataset.Name;
                }
                else if ((dataset.Name.ToLower().Contains("dummy") || dataset.Name.ToLower().Contains("temp")) && dataset.Properties.Type.Equals("AzureBlob"))
                {
                    tempBlobDataSet = dataset.Name;
                }
                else if ((dataset.Name.ToLower().Contains("log") || dataset.Name.ToLower().Contains("sql")) && dataset.Properties.Type.Equals("AzureSqlDW"))
                {
                    rawSQLDataset = dataset.Name;
                }
            }

            if (!part.ToLower().Equals("part2") && (part.ToLower().Equals("part1") || part.ToLower().Equals("all")))
            {

                #region StorageLinkedService
                /**Creating Storage Linked Service**/
                if (storageAccountLSName.Equals(""))
                {
                    storageAccountLSName = "AzureStorageLinkedService";
                    Console.WriteLine("Enter your Storage Account Name: ");
                    storageAccountName = Console.ReadLine();

                    Console.WriteLine("Enter a Storage Account Key for your Storage Account: ");
                    var storageKey = Console.ReadLine();
                    
                    client.LinkedServices.CreateOrUpdate(resourceGroupName, dataFactoryName,
                         new LinkedServiceCreateOrUpdateParameters()
                         {
                             LinkedService = new LinkedService()
                             {
                                 Name = storageAccountLSName,
                                 Properties = new LinkedServiceProperties
                                (
                                    new AzureStorageLinkedService("DefaultEndpointsProtocol=https;AccountName="+storageAccountName+";AccountKey="+storageKey)
                               )
                           }
                        }
                    );

                    Console.WriteLine("Successfully created Linked Service: "+storageAccountLSName);
                }
                else
                {
                    Console.WriteLine(storageAccountLSName + " already existed. Moving on...");
                }
                #endregion

                #region HDInsightLinkedService
                //** HDinsight Linked Service ***/
                if (hdinsightLSName.Equals(""))
                {
                    hdinsightLSName = "HDInsightLinkedService";
                    Console.WriteLine("Enter the name of your HDInsight Cluster: ");
                    var clusterName = Console.ReadLine();

                    Console.WriteLine("Enter the admin username of your cluster: ");
                    var cUsername = Console.ReadLine();

                    Console.WriteLine("Enter the admin password of your cluster: ");
                    var pass = Console.ReadLine();

                    client.LinkedServices.CreateOrUpdate(resourceGroupName, dataFactoryName,
                        new LinkedServiceCreateOrUpdateParameters()
                        {
                            LinkedService = new LinkedService()
                            {
                                Name = hdinsightLSName,
                                Properties = new LinkedServiceProperties
                               (
                                   new HDInsightLinkedService()
                                   {
                                       ClusterUri = "https://" + clusterName + ".azurehdinsight.net",
                                       UserName = cUsername,
                                       Password = pass,
                                       LinkedServiceName = storageAccountLSName
                                   }
                               )
                            }
                        }
                    );
                    Console.WriteLine("Successfully created Linked Service: " + hdinsightLSName);
                }
                else
                {
                    Console.WriteLine(hdinsightLSName + " already existed. Moving on...");
                }
                #endregion

                #region RawLogsDataset
                if(rawBlobDataSet.Equals(""))
                {
                    rawBlobDataSet = "LogJsonFromBlob";
                    Console.WriteLine("Creating RawDataSet...");
                    client.Datasets.CreateOrUpdate(resourceGroupName, dataFactoryName,
                        new DatasetCreateOrUpdateParameters()
                        {
                            Dataset = new Dataset()
                            {
                                Name = rawBlobDataSet,
                                Properties = new DatasetProperties()
                                {
                                    LinkedServiceName = storageAccountLSName,
                                    TypeProperties = new AzureBlobDataset()
                                    {
                                        FolderPath = "partsunlimited/logs/{Year}/{Month}/{Day}",
                                        PartitionedBy = new Collection<Partition>()
                                        {
                                            new Partition()
                                            {
                                                Name = "Year",
                                                Value = new DateTimePartitionValue()
                                                {
                                                    Date = "SliceStart",
                                                    Format = "yyyy"
                                                }
                                            },
                                            new Partition()
                                            {
                                                Name = "Month",
                                                Value = new DateTimePartitionValue()
                                                {
                                                    Date = "SliceStart",
                                                    Format = "MM"
                                                }
                                            },
                                            new Partition()
                                            {
                                                Name = "Day",
                                                Value = new DateTimePartitionValue()
                                                {
                                                    Date = "SliceStart",
                                                    Format = "dd"
                                                }
                                            }
                                        }
                                    },
                                    External = true,
                                    Availability = new Availability()
                                    {
                                        Frequency = SchedulePeriod.Day,
                                        Interval = 1,
                                    },
                                    Policy = new Policy()
                                    {
                                        Validation = new ValidationPolicy()
                                        {
                                            MinimumRows = 1
                                        }
                                    }
                                }
                            }
                        });

                    Console.WriteLine("Dataset: "+rawBlobDataSet+" successfully created.");
                }
                else
                {
                    Console.WriteLine("Dataset: " + rawBlobDataSet + " already exists. Moving on...");
                }
                #endregion

                #region DummyDataset
                if (tempBlobDataSet.Equals(""))
                {
                    tempBlobDataSet = "DummyDataset";
                    Console.WriteLine("Create DummyDataset for Add Partitions HDI Job");
                    client.Datasets.CreateOrUpdate(resourceGroupName, dataFactoryName,
                        new DatasetCreateOrUpdateParameters()
                        {
                            Dataset = new Dataset()
                            {
                                Name = tempBlobDataSet,
                                Properties = new DatasetProperties()
                                {
                                    LinkedServiceName = storageAccountLSName,
                                    TypeProperties = new AzureBlobDataset()
                                    {
                                        FolderPath = "partsunlimited/dummy",
                                        Format = new TextFormat()
                                    },
                                    External = false,
                                    Availability = new Availability()
                                    {
                                        Frequency = SchedulePeriod.Day,
                                        Interval = 1,
                                    },
                                    Policy = new Policy()
                                    {
                                        Validation = new ValidationPolicy()
                                        {
                                            MinimumRows = 1
                                        }
                                    }
                                }
                            }
                        });

                    Console.WriteLine("Dataset: " + tempBlobDataSet + " successfully created.");
                }
                else
                {
                    Console.WriteLine("Dataset "+ tempBlobDataSet + " already exists. Moving on...");
                }
                #endregion

                #region AddPartitions Activity
                //Creating Pipeline

                if(!storageAccountName.Equals(""))
                    DefinesVars.Add("StorageAccountName", storageAccountName);
                else
                {
                    Console.WriteLine("Please enter the name of the Storage Account that contains the Hive Scripts:");
                    storageAccountName = Console.ReadLine();
                }

                var PartitionActivityName = "";
                var HivePipelineName = "";
                foreach(var pipeline in Pipelines.Pipelines)
                {
                    var error = pipeline.Properties.ErrorMessage;
                    if (error != null)
                    {
                        Console.Error.WriteLine("You have an error in your pipeline: " + pipeline.Name + ". Please talk to a proctor.");
                        Console.WriteLine("In the meantime, let me do some more checks.");
                    }
                        

                    foreach (var activity in pipeline.Properties.Activities)
                    {
                        if(activity.Type.Equals("HDInsightHive"))
                        {
                            HivePipelineName = pipeline.Name;
                            if(error!=null)
                            {
                                HivePipelineName = "";
                            }

                            var typeProperties = (HDInsightHiveActivity) activity.TypeProperties;
                            if(typeProperties.ScriptPath.ToLower().Contains("addpartition"))
                            {
                                PartitionActivityName = activity.Name;
                            }
                        }
                    }
                }

                
                if (HivePipelineName.Equals("") || PartitionActivityName.Equals(""))
                {
                    client.Pipelines.CreateOrUpdate(resourceGroupName, dataFactoryName,
                        new PipelineCreateOrUpdateParameters()
                        {
                            Pipeline = new Pipeline()
                            {
                                Name = dataPipelineName,
                                Properties = new PipelineProperties()
                                {
                                    Activities = new Collection<Activity>()
                                    {
                                        new Activity()
                                        {
                                            Name = "CreatePartitionHiveActivity",
                                            Description = "Adds daily partitions",
                                            TypeProperties = new HDInsightHiveActivity()
                                            {
                                                ScriptLinkedService = storageAccountLSName,
                                                ScriptPath = "partsunlimited/Scripts/addpartitions.hql",
                                                Defines = DefinesVars
                                            },
                                            Inputs = new Collection<ActivityInput>()
                                            {
                                                new ActivityInput()
                                                {
                                                    Name = rawBlobDataSet
                                                }
                                            },
                                            Outputs = new Collection<ActivityOutput>()
                                            {
                                                new ActivityOutput()
                                                {
                                                    Name = tempBlobDataSet
                                                }
                                            },
                                            LinkedServiceName = hdinsightLSName,
                                            Scheduler = new Scheduler()
                                            {
                                                Frequency = "Day",
                                                Interval = 1
                                            }
                                        }
                                    },
                                    Start = StartTime,
                                    End = EndTime,
                                    IsPaused = false
                                }
                            }
                        }
                    );

                    Console.WriteLine("Done Creating Activity for Adding Partitions");
                    #endregion

                }
                else
                {
                    Console.WriteLine("Add Partitions Activity already exists. Moving on...");
                }

                Console.WriteLine("Done with Part1. Press Enter to continue . . . ");
                Console.ReadLine();
            }
            if (!part.ToLower().Equals("part1") && (part.ToLower().Equals("part2") || part.ToLower().Equals("all")))
            {
                if (storageAccountName.Equals(""))
                {
                    Console.WriteLine("Before we begin: Please enter the name of the Storage Account that contains the Hive Scripts: ");
                    storageAccountName = Console.ReadLine();
                }

                Console.WriteLine("Please confirm that you have Part1 of this factory setup. Press 'y' for Yes and 'n' for No: ");
                var resp = Console.ReadLine();

                if(!resp.ToLower().Equals("y"))
                {
                    Console.WriteLine("Please re-run the app with a 'Part1' or 'all' argument to setup the factory.");
                    Environment.Exit(0);
                }

                #region SQL DW Linked Service
                //DW Linked Service
                if (SQLDWServerLSName.Equals(""))
                {
                    SQLDWServerLSName = "SqlDWLinkedService";
                    Console.WriteLine("Please enter the DBServer name (The DBServer must already be provisioned): ");
                    var SQLDWServerName = Console.ReadLine();

                    Console.WriteLine("Please enter the Data Warehouse name (The DW must already be provisioned):");
                    var SQLDWDatabaseName = Console.ReadLine();

                    Console.WriteLine("Please enter the username: ");
                    var username = Console.ReadLine();

                    Console.WriteLine("Please enter the Password: ");
                    string pass = Console.ReadLine();


                    Console.WriteLine("Creating DW linked service...");
                    client.LinkedServices.CreateOrUpdate(resourceGroupName, dataFactoryName,
                        new LinkedServiceCreateOrUpdateParameters()
                        {
                            LinkedService = new LinkedService()
                            {
                                Name = SQLDWServerLSName,
                                Properties = new LinkedServiceProperties
                                (
                                    new AzureSqlDataWarehouseLinkedService("Data Source=tcp:" + SQLDWServerName /*+ prefixNumber*/ + ".database.windows.net,1433;Initial Catalog=;User ID=" + username + "@" + SQLDWServerName /*+ prefixNumber*/ + ";Password=" + pass + ";Integrated Security=False;Encrypt=True;Connect Timeout=30")
                                )
                            }
                        }
                    );

                    Console.WriteLine("SQL DW Linked Service with name: " + SQLDWServerLSName + " Created Successfully!");
                }
                else
                {
                    Console.WriteLine("SQL DW Linked Service already exists. Moving on...");
                }
                #endregion

                #region ProcessedBlobData
                var processedData = "LogCsvFromBlob";
                //Blob Dataset for Processed data
                Console.WriteLine("Creating the Processed Blob Dataset");
                client.Datasets.CreateOrUpdate(resourceGroupName, dataFactoryName,
                    new DatasetCreateOrUpdateParameters()
                    {
                        Dataset = new Dataset()
                        {
                            Name = processedData,
                            Properties = new DatasetProperties()
                            {
                                LinkedServiceName = storageAccountLSName,
                                Structure = new Collection<DataElement>()
                                {
                                    new DataElement()
                                    {
                                        Name = "productid",
                                        Type = "Int32"
                                    },
                                    new DataElement()
                                    {
                                        Name = "title",
                                        Type = "String"
                                    },
                                    new DataElement()
                                    {
                                        Name = "category",
                                        Type = "String"
                                    },
                                    new DataElement()
                                    {
                                        Name = "type",
                                        Type = "String"
                                    },
                                    new DataElement()
                                    {
                                        Name = "totalClicked",
                                        Type = "Int32"
                                    }
                                },
                                TypeProperties = new AzureBlobDataset()
                                {
                                    FolderPath = "processeddata/logs/{Year}/{Month}/{Day}",
                                    PartitionedBy = new Collection<Partition>()
                                    {
                                            new Partition()
                                            {
                                                Name = "Year",
                                                Value = new DateTimePartitionValue()
                                                {
                                                    Date = "SliceStart",
                                                    Format = "yyyy"
                                                }
                                            },
                                            new Partition()
                                            {
                                                Name = "Month",
                                                Value = new DateTimePartitionValue()
                                                {
                                                    Date = "SliceStart",
                                                    Format = "MM"
                                                }
                                            },
                                            new Partition()
                                            {
                                                Name = "Day",
                                                Value = new DateTimePartitionValue()
                                                {
                                                    Date = "SliceStart",
                                                    Format = "dd"
                                                }
                                            }
                                    }
                                },
                                External = false,
                                Availability = new Availability()
                                {
                                    Frequency = SchedulePeriod.Day,
                                    Interval = 1,
                                },
                                Policy = new Policy()
                                {
                                    Validation = new ValidationPolicy()
                                    {
                                        MinimumRows = 1
                                    }
                                }
                            }
                        }
                    });

                Console.WriteLine("Successfully created dataset: "+ processedData);
                #endregion

                #region RawSQLDWData
                if(rawSQLDataset.Equals(""))
                {
                    rawSQLDataset = "LogsSqlDWOutput";
                    //DW Dataset for Raw Data
                    Console.WriteLine("Creating Logs Data table in SQL DW");
                    client.Datasets.CreateOrUpdate(resourceGroupName, dataFactoryName,
                        new DatasetCreateOrUpdateParameters()
                        {
                            Dataset = new Dataset()
                            {
                                Name = rawSQLDataset,
                                Properties = new DatasetProperties()
                                {
                                    LinkedServiceName = SQLDWServerLSName,
                                    TypeProperties = new AzureSqlDataWarehouseTableDataset()
                                    {
                                        TableName = "dbo.ProductLogs"
                                    },
                                    Structure = new Collection<DataElement>()
                                    {
                                    new DataElement()
                                    {
                                        Name = "productid",
                                        Type = "Int32"
                                    },
                                    new DataElement()
                                    {
                                        Name = "title",
                                        Type = "String"
                                    },
                                    new DataElement()
                                    {
                                        Name = "category",
                                        Type = "String"
                                    },
                                    new DataElement()
                                    {
                                        Name = "type",
                                        Type = "String"
                                    },
                                    new DataElement()
                                    {
                                        Name = "totalClicked",
                                        Type = "Int32"
                                    }
                                    },
                                    External = false,
                                    Availability = new Availability()
                                    {
                                        Frequency = SchedulePeriod.Day,
                                        Interval = 1,
                                    },
                                    Policy = new Policy()
                                    {
                                        Validation = new ValidationPolicy()
                                        {
                                            MinimumRows = 1
                                        }
                                    }
                                }
                            }
                        });

                    Console.WriteLine("Successfully created SQL DW Dataset for Logs");
                }
                else
                {
                    Console.WriteLine("SQL DW Raw Dataset already exists. Moving on...");
                }
                
                #endregion

                #region ProcessedSQLDWData
                //DW Dataset for Processed Data
                var SQLDWStatsDataset = "StatsSqlDWOutput";
                Console.WriteLine("Now Creating SQL DW Dataset for Sql Stats Data");
                client.Datasets.CreateOrUpdate(resourceGroupName, dataFactoryName,
                   new DatasetCreateOrUpdateParameters()
                   {
                       Dataset = new Dataset()
                       {
                           Name = SQLDWStatsDataset,
                           Properties = new DatasetProperties()
                           {
                               LinkedServiceName = SQLDWServerLSName,
                               TypeProperties = new AzureSqlDataWarehouseTableDataset()
                               {
                                   TableName = "dbo.ProductStats"
                               },
                               External = false,
                               Availability = new Availability()
                               {
                                   Frequency = SchedulePeriod.Day,
                                   Interval = 1,
                               },
                               Policy = new Policy()
                               {
                                   Validation = new ValidationPolicy()
                                   {
                                       MinimumRows = 1
                                   }
                               }
                           }
                       }
                   });

                Console.WriteLine("Successfully created dataset for SQL DW");
                #endregion

                #region HDIProcessingActivity
                //Activity for Processing Data (HDI)
                DefinesVars["StorageAccountName"] = storageAccountName;

                //Hard-coding Datasets
                if(rawBlobDataSet.Equals(""))
                {
                    rawBlobDataSet = "LogJsonFromBlob";
                }
                if (tempBlobDataSet.Equals(""))
                {
                    tempBlobDataSet = "DummyDataset";
                }

                Console.WriteLine("Now adding HDInsight Hive Processing Activity...");
                                    client.Pipelines.CreateOrUpdate(resourceGroupName, dataFactoryName,
                        new PipelineCreateOrUpdateParameters()
                        {
                            Pipeline = new Pipeline()
                            {
                                Name = dataPipelineName,
                                Properties = new PipelineProperties()
                                {
                                    Activities = new Collection<Activity>()
                                    {
                                        new Activity()
                                        {
                                            Name = "CreatePartitionHiveActivity",
                                            Description = "Adds daily partitions",
                                            TypeProperties = new HDInsightHiveActivity()
                                            {
                                                ScriptLinkedService = storageAccountLSName,
                                                ScriptPath = "partsunlimited/Scripts/addpartitions.hql",
                                                Defines = DefinesVars
                                            },
                                            Inputs = new Collection<ActivityInput>()
                                            {
                                                new ActivityInput()
                                                {
                                                    Name = rawBlobDataSet
                                                }
                                            },
                                            Outputs = new Collection<ActivityOutput>()
                                            {
                                                new ActivityOutput()
                                                {
                                                    Name = tempBlobDataSet
                                                }
                                            },
                                            LinkedServiceName = hdinsightLSName,
                                            Scheduler = new Scheduler()
                                            {
                                                Frequency = "Day",
                                                Interval = 1
                                            }
                                        },
                                        new Activity()
                                        {
                                            Name = "ProcessDataHiveActivity",
                                            Description = "Activity to Process Blob Data using HDInsight",
                                            LinkedServiceName = hdinsightLSName,
                                            TypeProperties = new HDInsightHiveActivity()
                                            {
                                                ScriptLinkedService = storageAccountLSName,
                                                ScriptPath = "partsunlimited/Scripts/logstocsv.hql",
                                                Defines = DefinesVars
                                            },
                                            Inputs = new Collection<ActivityInput>()
                                            {
                                                new ActivityInput()
                                                {
                                                    Name = tempBlobDataSet
                                                }
                                            },
                                            Outputs = new Collection<ActivityOutput>()
                                            {
                                                new ActivityOutput()
                                                {
                                                    Name = processedData
                                                }
                                            }
                                        }
                                    },
                                    Start = StartTime,
                                    End = EndTime,
                                    IsPaused = false
                                }
                            }
                        }
                    );


                Console.WriteLine("Successfully Added Activity to the HDInsight Pipeline");
                #endregion

                #region CopyActivityBlobToDW
                //Copy Activity to DW
                Console.WriteLine("Creating Copy Pipeline to move data from Blob to SQL DW");
                client.Pipelines.CreateOrUpdate(resourceGroupName, dataFactoryName,
                    new PipelineCreateOrUpdateParameters()
                    {
                        Pipeline = new Pipeline()
                        {
                            Name = "LogsToDWPipeline",
                            Properties = new PipelineProperties
                            {
                                IsPaused = false,
                                Description = "Copy Pipeline - Blob to SQL DW",
                                Start = StartTime,
                                End = EndTime,
                                Activities = new Collection<Activity>()
                                {
                                    new Activity()
                                    {
                                        Name = "LogsToDWActivity",
                                        Description = "This activity will copy data from Blob to SQL DW",
                                        Inputs = new Collection<ActivityInput>()
                                        {
                                            new ActivityInput()
                                            {
                                                Name = "LogCsvFromBlob"
                                            }
                                        },
                                        Outputs = new Collection<ActivityOutput>()
                                        {
                                            new ActivityOutput()
                                            {
                                                Name = "LogsSqlDWOutput"
                                            }
                                        },
                                        TypeProperties = new CopyActivity()
                                        {
                                            Source = new BlobSource(),
                                            Sink = new SqlDWSink()
                                            {
                                                WriteBatchSize = 1000,
                                                WriteBatchTimeout = TimeSpan.FromMinutes(20)
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
                #endregion

                #region SPROCActivity
                //Create SQL SPROC Activity
                var sp_name = "sp_populate_stats";
                Console.WriteLine("Creating SPROC Pipeline to invoke Stored Procedure in Azure SQL DW...");
                Console.WriteLine("Can you please confirm that your Stored Procedure is named: "+ sp_name);
                Console.WriteLine("Press 'y' to confirm or 'n' to provide another name...");
                var resp1 = Console.ReadLine();
                if (resp1.Equals("n"))
                {
                    Console.WriteLine("Please enter the name of your Stored Procedure: ");
                    sp_name = Console.ReadLine();
                }
                client.Pipelines.CreateOrUpdate(resourceGroupName, dataFactoryName,
                    new PipelineCreateOrUpdateParameters()
                    {
                        Pipeline = new Pipeline()
                        {
                            Name = "SPROC Pipeline",
                            Properties = new PipelineProperties()
                            {
                                IsPaused = false,
                                Start = StartTime,
                                End = EndTime,
                                Description = "This pipeline contains the activity that invokes the Stored Procedure to process data in Azure SQL DW",
                                Activities = new Collection<Activity>()
                                {
                                new Activity()
                                {
                                    Name = "SPROC Activity",
                                    Description = "Invokes SPROC Activity",
                                    LinkedServiceName = SQLDWServerLSName,
                                    Inputs = new Collection<ActivityInput>()
                                    {
                                        new ActivityInput()
                                        {
                                        Name = rawSQLDataset
                                        }
                                    },
                                    Outputs = new Collection<ActivityOutput>()
                                    {
                                        new ActivityOutput()
                                        {
                                            Name = SQLDWStatsDataset
                                        }
                                    },
                                    TypeProperties = new SqlServerStoredProcedureActivity()
                                    {
                                        StoredProcedureName = sp_name
                                    }
                                }
                                }
                            }
                        }
                    });

                Console.WriteLine("Done creating the DW SQL SPROC Pipeline");
                #endregion

                Console.WriteLine("Done creating the Data Factory. Press Enter to exit . . .");
                Console.ReadLine();

            }
        }

        public static string GetAuthorizationHeader()
        {
            AuthenticationResult result = null;
            var thread = new Thread(() =>
            {
                try
                {
                    var context = new AuthenticationContext(ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"] + ConfigurationManager.AppSettings["ActiveDirectoryTenantId"]);

                    result = context.AcquireToken(
                        resource: ConfigurationManager.AppSettings["WindowsManagementUri"],
                        clientId: ConfigurationManager.AppSettings["AdfClientId"],
                        redirectUri: new Uri(ConfigurationManager.AppSettings["RedirectUri"]),
                        promptBehavior: PromptBehavior.Always);
                }
                catch (Exception threadEx)
                {
                    Console.WriteLine(threadEx.Message);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AcquireTokenThread";
            thread.Start();
            thread.Join();

            if (result != null)
            {
                return result.AccessToken;
            }

            throw new InvalidOperationException("Failed to acquire token");
        }
    }
}
