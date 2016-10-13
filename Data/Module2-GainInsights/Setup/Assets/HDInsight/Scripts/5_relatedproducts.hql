DROP VIEW IF EXISTS unique_purchases;
	CREATE VIEW unique_purchases AS 
	SELECT distinct userid, productid
	FROM websiteActivity where eventdate > date_sub(from_unixtime(unix_timestamp()),30)
	AND type = 'checkout';

	DROP VIEW IF EXISTS all_purchased_products;
	CREATE VIEW all_purchased_products AS 
	SELECT a.userid, COLLECT_LIST(CONCAT(a.productid,',',a.qty)) as product_list from (
  	 SELECT userid, productid, sum(quantity) as qty FROM websiteActivity
   	 WHERE eventdate > date_sub(from_unixtime(unix_timestamp()),30)
   	 AND type = 'checkout'
   	 GROUP BY userid, productid
   	 ORDER BY userid ASC, qty DESC) a
	GROUP BY a.userid;

	DROP VIEW IF EXISTS related_purchase_list;
	CREATE VIEW related_purchase_list AS
	SELECT a.userid, a.productid, b.product_list
	FROM unique_purchases a LEFT OUTER JOIN all_purchased_products b ON (a.userid = b.userid);

	DROP TABLE IF EXISTS related_products;
	CREATE TABLE related_products ROW FORMAT DELIMITED FIELDS TERMINATED BY ',' LINES TERMINATED BY '\n' LOCATION 'wasbs://processeddata@<StorageAccountName>.blob.core.windows.net/related_products/' AS 
	SELECT c.productid, c.related_product, c.qty, rank() OVER (PARTITION BY c.productid ORDER BY c.qty DESC) as rank FROM
	(SELECT a.productid, a.related_product, SUM(a.quantity) as qty FROM
	(SELECT b.productid, SPLIT(prod_list, ',')[0] as related_product, CAST(SPLIT(prod_list, ',')[1] as INT) as quantity
	FROM related_purchase_list b LATERAL VIEW EXPLODE(b.product_list) prodList as prod_list) a
	WHERE a.productid <> a.related_product
	GROUP BY a.productid, a.related_product
	ORDER BY a.productid ASC, qty DESC) c;