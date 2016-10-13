	CREATE TABLE dbo.ProductLogs
	 (productid int, title nvarchar(50), category nvarchar(50), type nvarchar(5), totalClicked int)
	GO
	
	CREATE TABLE dbo.ProductStats
	 (category nvarchar(50), title nvarchar(50), views int, adds int)
	GO
	
	CREATE PROCEDURE sp_populate_stats AS
	BEGIN
	 DELETE FROM dbo.ProductStats;
	 INSERT INTO dbo.ProductStats 
	 SELECT
	  category,
	  title,
	  SUM(CASE WHEN type = 'view' THEN totalClicked ELSE 0 END) AS views,
	  SUM(CASE WHEN type = 'add' THEN totalClicked ELSE 0 END) AS adds
	 FROM dbo.ProductLogs GROUP BY title, category
	END
	GO