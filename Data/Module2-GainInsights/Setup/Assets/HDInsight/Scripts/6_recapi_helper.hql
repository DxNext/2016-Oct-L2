CREATE TABLE RecUsageData ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n' LOCATION 'wasbs://processeddata@<StorageAccountName>.blob.core.windows.net/recommendations_usage_data/' AS 
	SELECT userId, productId, eventDate, CASE 
		WHEN type='checkout' THEN 'Purchase'
		WHEN type='view' THEN 'Click'
		WHEN type='add' THEN 'AddShopCart'
		WHEN type='remove' THEN 'RemoveShopCart'
	FROM WebsiteActivity


	CREATE TABLE RecProductCatalog ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n' LOCATION 'wasbs://processeddata@<StorageAccountName>.blob.core.windows.net/recommendations_usage_data/' AS 
	SELECT productId, title, categoryId
	FROM ProductCatalog