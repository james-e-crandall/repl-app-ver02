When HierarchyId fails to replicate in SQL Server, it is usually because the article's @schema_option is not set to convert the HierarchyId CLR data type
 to binary. To fix this, update the article's schema option to include 0x2000000000 (which converts hierarchyid to varbinary(max)) and reinitialize the subscription. 

https://learn.microsoft.com/en-us/sql/relational-databases/system-stored-procedures/sp-changearticle-transact-sql?view=sql-server-ver17

How to Fix via Stored Procedures
You can alter the existing publication article on the Publisher by executing the sp_changearticle system stored procedure. 
First, determine your current @schema_option value using the following query on the Publisher:

SELECT name, schema_option 
FROM sysarticles 
WHERE object_id = OBJECT_ID('dbo.YourTableName');


Once you have your current schema option, add the 0x2000000000 bitwise flag using a bitwise | OR operator. Pass this combined value into sp_changearticle: 

EXEC sp_changearticle 
    @publication = 'YourPublicationName', 
    @article = 'YourArticleName', 
    @property = 'schema_option', 
    @value = 0x...; -- Your existing schema option OR | cast(0x2000000000 AS varbinary(8))


This works...


            DECLARE @schema_option AS int;
        SET @schema_option = (SELECT CAST(0x80030F3 AS int) | cast(0x2000000000 AS int));
        EXEC sp_addarticle 
            @publication = @publication, 
            @article = @table, 
            @source_object = @table,
            @source_owner = @schemaowner, 
            @schema_option = @schema_option,
            @vertical_partition = N'true', 
            @type = N'logbased';


Exception has occurred: CLR/System.InvalidOperationException
An exception of type 'System.InvalidOperationException' occurred in Microsoft.EntityFrameworkCore.dll but was not handled in user code:
 'The entity type 'HierarchyId' requires a primary key to be defined. If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'.
 For more information on keyless entity types, see https://go.microsoft.com/fwlink/?linkid=2141943.'
