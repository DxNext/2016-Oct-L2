DROP TABLE IF EXISTS RawProductCatalog;
	CREATE EXTERNAL TABLE RawProductCatalog (
		jsonentry string
	) STORED AS TEXTFILE LOCATION "wasb://partsunlimited@${hiveconf:StorageAccountName}.blob.core.windows.net/productcatalog/";
	

	DROP TABLE IF EXISTS ProductCatalog;
	CREATE TABLE ProductCatalog ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n'
	LOCATION 'wasbs://processeddata@<StorageAccountName>.blob.core.windows.net/product_catalog/'
	AS SELECT get_json_object(jsonentry, "$.skuNumber") as skuNumber,
			  get_json_object(jsonentry, "$.id") as id,
			  get_json_object(jsonentry, "$.productId") as productId,
			  get_json_object(jsonentry, "$.categoryId") as categoryId,
			  get_json_object(jsonentry, "$.category.name") as categoryName,
			  get_json_object(jsonentry, "$.title") as title,
			  get_json_object(jsonentry, "$.price") as price,
			  get_json_object(jsonentry, "$.salePrice") as salePrice,
			  get_json_object(jsonentry, "$.costPrice") as costPrice
	FROM RawProductCatalog;