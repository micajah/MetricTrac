CREATE PROCEDURE pr_Disable_Foreign_Keys_Triggers
    @disable BIT = 1,
    @tableName VARCHAR(128) = null
AS
    DECLARE
        @sql VARCHAR(500),
        @sqltrigger VARCHAR(500),
        @foreignKeyName VARCHAR(128)

    -- A list of all foreign keys and table names
    DECLARE foreignKeyCursor CURSOR
    FOR SELECT
        ref.constraint_name AS FK_Name,
        fk.table_name AS FK_Table
    FROM
        INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS ref
        INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS fk 
    ON ref.constraint_name = fk.constraint_name
    WHERE fk.table_name = @tableName
    ORDER BY
        fk.table_name,
        ref.constraint_name 
	OPEN foreignKeyCursor

    FETCH NEXT FROM foreignKeyCursor 
    INTO @foreignKeyName, @tableName

    WHILE ( @@FETCH_STATUS = 0 )
        BEGIN
            IF @disable = 1
				BEGIN
                SET @sql = 'ALTER TABLE [‘ 
                    + @tableName + ‘] NOCHECK CONSTRAINT [‘ 
                    + @foreignKeyName + ‘]'
                    
                SET @sqltrigger = 'ALTER TABLE '+@tableName+' DISABLE TRIGGER ALL'
				END
            ELSE
				BEGIN
                SET @sql = 'ALTER TABLE [‘ 
                    + @tableName + ‘] CHECK CONSTRAINT [‘ 
                    + @foreignKeyName + ‘]'

				SET @sqltrigger = 'ALTER TABLE '+@tableName+' ENABLE TRIGGER ALL'
				END
        PRINT 'Executing FK Enable/Disable - ' + @sql
        PRINT 'Executing Trigger Enable/Disable - ' + @sqltrigger

        EXECUTE(@sql)
        EXECUTE(@sqltrigger)
        FETCH NEXT FROM foreignKeyCursor 
        INTO @foreignKeyName, @tableName
    END

    CLOSE foreignKeyCursor
    DEALLOCATE foreignKeyCursor
    
