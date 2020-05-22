-- SQL POCO Class Generator
-- Creator: Isaac Martinez
-- POCO => Plain Old C# Object
-- Notes: All you have to do is set the entity name below and run the script
-- The POCO class type output will be available as the executed script message
-- Version Date: 05/19/2020

DECLARE @EntityName AS NVARCHAR(50), @TableName AS NVARCHAR(50)
--@EntityName should typically be singular
SET @EntityName = 'Example'
--@@TableName should typically be plural
SET @TableName = 'Examples'

--Everything else after this point is part of the script
DECLARE @property AS NVARCHAR(2000)

PRINT 'public partial class ' + @EntityName
PRINT '{'

IF EXISTS (SELECT 1
FROM sys.foreign_keys
WHERE OBJECT_NAME(referenced_object_id) = @TableName)
BEGIN
	PRINT '  public ' + @TableName + ' ()'
	PRINT '  {'
	DECLARE child_table_cursor CURSOR
FOR
	SELECT OBJECT_NAME(parent_object_id) +' = new HashSet<' + 
	CASE
		WHEN (OBJECT_NAME(parent_object_id) LIKE '%s')
			THEN SUBSTRING(OBJECT_NAME(parent_object_id), 0, LEN(OBJECT_NAME(parent_object_id)))
			ELSE OBJECT_NAME(parent_object_id)
	END
	+ '>();'
	AS PROPERTY
	FROM sys.foreign_keys
	WHERE OBJECT_NAME(referenced_object_id) = @TableName
	OPEN child_table_cursor
	FETCH NEXT FROM child_table_cursor INTO @property
	WHILE @@FETCH_STATUS = 0
BEGIN
		PRINT '    ' +  @property
		FETCH NEXT FROM child_table_cursor INTO @property
	END
	CLOSE child_table_cursor;
	DEALLOCATE child_table_cursor;
	PRINT '  }'
	PRINT ''
END

DECLARE property_cursor CURSOR
FOR  
	SELECT
	CASE
			WHEN IS_NULLABLE = 'NO' THEN '[Required] '
			ELSE ''
		END
		+ 
		CASE
			WHEN DATA_TYPE LIKE '%char' THEN '[MaxLength(' + CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR) + ')] '
			ELSE ''
		END
		+ 'public ' 
		+ CASE   
		-- The following logic is based on this Microsoft Docs Article
		-- SQL Server Data Type Mappings
		-- https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql-server-data-type-mappings
    -- Accessed ON 5/19/2020
			 WHEN DATA_TYPE = 'uniqueidentifier' AND IS_NULLABLE = 'YES' THEN 'Guid? '
			 WHEN DATA_TYPE = 'uniqueidentifier' AND IS_NULLABLE = 'NO' THEN 'Guid '  
			 WHEN DATA_TYPE LIKE '%char' OR DATA_TYPE LIKE '%text' THEN 'string '
			 WHEN DATA_TYPE = 'int64' AND IS_NULLABLE = 'YES' THEN 'Int64? '
			 WHEN DATA_TYPE = 'int64' AND IS_NULLABLE = 'NO' THEN 'Int64 '  
			 WHEN DATA_TYPE = 'int' AND IS_NULLABLE = 'YES' THEN 'int? '
			 WHEN DATA_TYPE = 'int' AND IS_NULLABLE = 'NO' THEN 'int '    
			 WHEN (DATA_TYPE = 'smallint') AND IS_NULLABLE = 'YES' THEN 'Int16? '
			 WHEN (DATA_TYPE = 'smallint') AND IS_NULLABLE = 'NO' THEN 'Int16 '
			 WHEN (DATA_TYPE = 'tinyint') AND IS_NULLABLE = 'YES' THEN 'byte? '
			 WHEN (DATA_TYPE = 'tinyint') AND IS_NULLABLE = 'NO' THEN 'byte '
			 WHEN (DATA_TYPE = 'float') AND IS_NULLABLE = 'YES' THEN 'double? '
			 WHEN (DATA_TYPE = 'float') AND IS_NULLABLE = 'NO' THEN 'double '
			 WHEN (DATA_TYPE LIKE '%binary' OR DATA_TYPE = 'image' OR DATA_TYPE = 'timestamp') AND IS_NULLABLE = 'YES' THEN 'byte[]? '
			 WHEN (DATA_TYPE LIKE '%binary' OR DATA_TYPE = 'image' OR DATA_TYPE = 'timestamp') AND IS_NULLABLE = 'NO' THEN 'byte[] '
			 WHEN DATA_TYPE = 'time' AND IS_NULLABLE = 'YES' THEN 'TimeSpan? '
			 WHEN DATA_TYPE = 'time' AND IS_NULLABLE = 'NO' THEN 'TimeSpan '  
			 WHEN DATA_TYPE = 'bit' AND IS_NULLABLE = 'YES' THEN 'bool? '
			 WHEN DATA_TYPE = 'bit' AND IS_NULLABLE = 'NO' THEN 'bool '  
			 WHEN (DATA_TYPE = 'decimal' OR DATA_TYPE LIKE '%money' OR DATA_TYPE = 'numeric') AND IS_NULLABLE = 'YES' THEN 'decimal? '  
			 WHEN (DATA_TYPE = 'decimal' OR DATA_TYPE LIKE '%money' OR DATA_TYPE = 'numeric') AND IS_NULLABLE = 'NO' THEN 'decimal ' 
			 WHEN DATA_TYPE = 'datetimeoffset' AND IS_NULLABLE = 'YES' THEN 'DateTimeOffset? '  
			 WHEN DATA_TYPE = 'datetimeoffset' AND IS_NULLABLE = 'NO' THEN 'DateTimeOffset '  
			 WHEN DATA_TYPE LIKE '%date%' AND IS_NULLABLE = 'YES' THEN 'DateTime? '  
			 WHEN DATA_TYPE LIKE '%date%' AND IS_NULLABLE = 'NO' THEN 'DateTime '
			 ELSE 'object '  
		  END
		+ COLUMN_NAME
		+ ' { get; set; }' AS PROPERTY
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @TableName
OPEN property_cursor
FETCH NEXT FROM property_cursor INTO @property
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT '  ' +  REPLACE(@property, '] ', ']' + char(10) + '  ')
	FETCH NEXT FROM property_cursor INTO @property
END
CLOSE property_cursor;
DEALLOCATE property_cursor;


DECLARE parent_table_cursor CURSOR
FOR
	SELECT 'public virtual ' + 
	CASE
		WHEN (OBJECT_NAME(referenced_object_id) LIKE '%s')
			THEN SUBSTRING(OBJECT_NAME(referenced_object_id), 0, LEN(OBJECT_NAME(referenced_object_id)))
			ELSE OBJECT_NAME(referenced_object_id)
	END
	+ ' ' + 
	CASE
		WHEN (OBJECT_NAME(referenced_object_id) LIKE '%s')
			THEN SUBSTRING(OBJECT_NAME(referenced_object_id), 0, LEN(OBJECT_NAME(referenced_object_id)))
			ELSE OBJECT_NAME(referenced_object_id)
	END
	+ ' { get; set; } '
	AS PROPERTY
FROM sys.foreign_keys
WHERE OBJECT_NAME(parent_object_id) = @TableName
OPEN parent_table_cursor
FETCH NEXT FROM parent_table_cursor INTO @property
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT '  ' +  @property
	FETCH NEXT FROM parent_table_cursor INTO @property
END
CLOSE parent_table_cursor;
DEALLOCATE parent_table_cursor;


DECLARE child_table_cursor CURSOR
FOR
	SELECT 'public virtual ICollection<' + 
	CASE
		WHEN (OBJECT_NAME(parent_object_id) LIKE '%s')
			THEN SUBSTRING(OBJECT_NAME(parent_object_id), 0, LEN(OBJECT_NAME(parent_object_id)))
			ELSE OBJECT_NAME(parent_object_id)
	END
	+ '> ' + 
	OBJECT_NAME(parent_object_id) 
	+ ' { get; } '
	AS PROPERTY
FROM sys.foreign_keys
WHERE OBJECT_NAME(referenced_object_id) = @TableName
OPEN child_table_cursor
FETCH NEXT FROM child_table_cursor INTO @property
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT '  ' +  @property
	FETCH NEXT FROM child_table_cursor INTO @property
END
CLOSE child_table_cursor;
DEALLOCATE child_table_cursor;

PRINT '}'
