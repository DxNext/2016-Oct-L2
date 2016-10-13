INSERT OVERWRITE TABLE websiteActivity Partition (year, month, day)
	SELECT CAST(CONCAT(split(get_json_object(jsonentry, "$.eventDate"),'T')[0], ' ', SUBSTRING(split(get_json_object(jsonentry, "$.eventDate"),'T')[1],0,LENGTH(split(get_json_object(jsonentry, "$.eventDate"),'T')[1])-1)) as TIMESTAMP) as eventdate,
		 get_json_object(jsonentry, "$.userId") as userid,
         get_json_object(jsonentry, "$.type") as type,
         get_json_object(jsonentry, "$.productId") as productid,
		 CAST(get_json_object(jsonentry, "$.quantity") as int) as quantity,
         CAST(get_json_object(jsonentry, "$.price") as DOUBLE) as price,
		 year, month, day
	FROM LogsRaw
	WHERE year=${hiveconf:Year} and month=${hiveconf:Month} and day=${hiveconf:Day};