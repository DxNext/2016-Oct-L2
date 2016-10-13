CREATE EXTERNAL TABLE IF NOT EXISTS LogsRaw (jsonentry string) 
		PARTITIONED BY (year INT, month INT, day INT)
		STORED AS TEXTFILE LOCATION "wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/";

		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=06) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/06';
		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=07) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/07';
		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=08) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/08';
		ALTER TABLE LogsRaw ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=09) LOCATION 'wasb://partsunlimited@<StorageAccountName>.blob.core.windows.net/logs/2016/10/09';


CREATE TABLE IF NOT EXISTS websiteActivity (
		eventdate string,
		userid string,
		type string,
		productid string,
		quantity int,
		price double
	) PARTITIONED BY (year int, month int, day int) 
	ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
	STORED AS TEXTFILE LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs';

ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=06) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/06';
	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=07) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/07';
	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=08) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/08';
	ALTER TABLE websiteActivity ADD IF NOT EXISTS PARTITION (year=2016, month=10, day=09) LOCATION 'wasb://processeddata@<StorageAccountName>.blob.core.windows.net/structuredlogs/2016/10/09';