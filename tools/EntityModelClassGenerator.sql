-- SQL Model Class Generator
-- Creator: Isaac Martinez
-- Notes: All you have to do is set the entity name below and run the script
-- The model class type output will be available as the executed script message

DECLARE @EntityName AS NVARCHAR(50)
--@EntityName should be singular
SET @EntityName = 'Unit'

PRINT 'public partial class ' + @EntityName
PRINT '{'

DECLARE @property AS NVARCHAR(2000)
DECLARE property_cursor CURSOR
FOR  
	SELECT
		CASE
			WHEN IS_NULLABLE = 'NO' THEN '[Required] '
			ELSE ''
		END
		+ 
		CASE
			WHEN DATA_TYPE = 'nvarchar' OR DATA_TYPE = 'char' THEN '[MaxLength(' + CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR) + ')] '
			ELSE ''
		END
		+ 'public ' 
		+ CASE   
			 WHEN DATA_TYPE = 'uniqueidentifier' AND IS_NULLABLE = 'YES' THEN 'Guid? '
			 WHEN DATA_TYPE = 'uniqueidentifier' AND IS_NULLABLE = 'NO' THEN 'Guid '  
			 WHEN DATA_TYPE = 'nvarchar' OR DATA_TYPE = 'char' THEN 'string '
			 WHEN DATA_TYPE = 'decimal' AND IS_NULLABLE = 'YES' THEN 'decimal? '  
			 WHEN DATA_TYPE = 'decimal' AND IS_NULLABLE = 'NO' THEN 'decimal ' 
			 WHEN DATA_TYPE = 'datetimeoffset' AND IS_NULLABLE = 'YES' THEN 'DateTimeOffset? '  
			 WHEN DATA_TYPE = 'datetimeoffset' AND IS_NULLABLE = 'NO' THEN 'DateTimeOffset '  
			 ELSE 'object '  
		  END
		+ COLUMN_NAME
		+ ' {get; set;}' AS PROPERTY
	FROM INFORMATION_SCHEMA.COLUMNS
	WHERE TABLE_NAME = @EntityName + 's'
OPEN property_cursor
FETCH NEXT FROM property_cursor INTO @property
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT '  ' + @property
    FETCH NEXT FROM property_cursor INTO @property  
END
CLOSE property_cursor;  
DEALLOCATE property_cursor;
PRINT '}'
