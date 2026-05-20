using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace ReplLib.Extensions;

public static partial class ReplSqlConnectionExtensions
{

#region  private members
  // GO delimiter format: {spaces?}GO{spaces?}{repeat?}{comment?}
    // https://learn.microsoft.com/sql/t-sql/language-elements/sql-server-utilities-statements-go
    [GeneratedRegex(@"^\s*GO(?<repeat>\s+\d{1,6})?(\s*\-{2,}.*)?\s*$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    internal static partial Regex GoStatements();

    private static async Task<int> ExecuteNonQueryAsync(SqlConnection conn,
         string query, CancellationToken ct, params SqlParameter[] parameters)
    {
        using var reader = new StringReader(query);
        var batchBuilder = new StringBuilder();
        var result = 0;
        while (reader.ReadLine() is { } line)
        {
            var matchGo = GoStatements().Match(line);

            if (matchGo.Success)
            {
                // Execute the current batch
                var count = matchGo.Groups["repeat"].Success ? int.Parse(matchGo.Groups["repeat"].Value, CultureInfo.InvariantCulture) : 1;
                var batch = batchBuilder.ToString();

                for (var i = 0; i < count; i++)
                {
                    using var command = conn.CreateCommand();
                    command.CommandTimeout = 0;
                    command.CommandText = batch;
                    if(parameters != null && parameters.Length > 0)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    result += await command.ExecuteNonQueryAsync(ct);
                    command.Parameters.Clear();
                }

                batchBuilder.Clear();
            }
            else
            {
                // Prevent batches with only whitespace
                if (!string.IsNullOrWhiteSpace(line))
                {
                    batchBuilder.AppendLine(line);
                }
            }
        }

        // Process the remaining batch lines
        if (batchBuilder.Length > 0)
        {
            using var command = conn.CreateCommand();
            command.CommandTimeout = 0;
            command.CommandText = batchBuilder.ToString();
            command.Parameters.AddRange(parameters);
            result +=  await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
            command.Parameters.Clear();
        }
        return result;
    }

    private static DataTable GetDataTableFromStoredProc(SqlConnection conn, string query, params SqlParameter[] parameters)
    {
        DataTable dt = new DataTable();

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // 1. Specify that the command is a Stored Procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 2. Add input parameters
                if(parameters != null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                
                // 3. Use a SqlDataAdapter to fill the DataTable
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    // The Fill method automatically opens and closes the connection if it's closed
                    da.Fill(dt);
                }
            }
    
        return dt;
    }
#endregion
  
#region  public members

    public static DataTable sp_get_distributor(this SqlConnection conn)
    {
        var databaseName = "master";
        var formattedSql = $$"""
            -- Use the database
            USE [{{databaseName}}];
            GO
            EXEC sp_serveroption 
                @server = @@SERVERNAME,
                @optname = 'DATA ACCESS',
                @optvalue = 'TRUE';
            GO

            """;
        var parameters = new List<SqlParameter>();
        return GetDataTableFromStoredProc(conn, "sp_get_distributor", parameters.ToArray());
    }

    public static Task<int> sp_serveroption(this SqlConnection conn, CancellationToken ct)
    {
        var databaseName = "master";
        var formattedSql = $$"""
            -- Use the database
            USE [{{databaseName}}];
            GO
            EXEC sp_serveroption 
                @server = @@SERVERNAME,
                @optname = 'DATA ACCESS',
                @optvalue = 'TRUE';
            GO

            """;
        var parameters = new List<SqlParameter>();
        return ExecuteNonQueryAsync(conn, formattedSql, ct, parameters.ToArray());
    }

    public static async Task<int> sp_adddistributor(this SqlConnection conn, CancellationToken ct,
        string password)
    {
        var databaseName = "master";
        var formattedSql = $$"""
            -- Use the database
            USE [{{databaseName}}];
            GO
            EXEC sp_adddistributor 
                @distributor = @@SERVERNAME,
                @password = @password,
                @from_scripting = 1;
            GO

            """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_adddistributiondb(this SqlConnection conn, CancellationToken ct,
        string login, string password)
    {
        var databaseName = "master";
        var formattedSql = $$"""
            -- Use the database
            USE [{{databaseName}}];
            GO
            EXEC sp_adddistributiondb 
                @database = @database, 
                @security_mode = 0,
                @login = @login,
                @password = @password;
            GO

            """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@database", SqlDbType.NVarChar) { Value = "distribution" });
        parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar) { Value = login });
        parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_adddistpublisher(this SqlConnection conn, CancellationToken ct,
        string publisherDb, string login, string password, string working_directory = "/var/opt/mssql")
    {
        var formattedSql = $$"""
            -- Use the database
            USE [{{publisherDb}}];
            GO
            EXEC sp_adddistpublisher 
                @publisher=@@SERVERNAME, 
                @distribution_db=@distribution_db, 
                @security_mode = 0,
                @login = @login,
                @password = @password,
                @working_directory = @working_directory,
                @trusted = N'false',
                @thirdparty_flag = 0,
                @publisher_type = N'MSSQLSERVER';
            GO
            """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@database", SqlDbType.NVarChar) { Value = "distribution" });
        parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar) { Value = login });
        parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });
        parameters.Add(new SqlParameter("@working_directory", SqlDbType.NVarChar) { Value = working_directory });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_replicationdboption(this SqlConnection conn, CancellationToken ct,
        string publisherDb)
    {
        var formattedSql = $$"""
            -- Use the database
            USE [{{publisherDb}}];
            GO
            EXEC sp_replicationdboption 
                @dbname= @dbname, 
                @optname=N'publish',
                @value = N'true';
            GO
            """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@dbname", SqlDbType.NVarChar) { Value = publisherDb });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_addlogreader_agent(this SqlConnection conn, CancellationToken ct,
        string publisherDb, string login, string password)
    {
        var formattedSql = $$"""
        -- Use the database
        USE [{{publisherDb}}];
        GO
        EXEC sp_addlogreader_agent 
            -- Explicitly specify the use of Windows Integrated Authentication (default) 
            -- when connecting to the Publisher.
            @publisher_security_mode = 0, -- Use SQL Server Authentication
            @publisher_login = @login, 
            @publisher_password = @password;
        GO
        """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar) { Value = login });
        parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_addpublication(this SqlConnection conn, CancellationToken ct,
        string publisherDb, string publication)
    {
        var formattedSql = $$"""
        -- Use the database
        USE [{{publisherDb}}];
        GO
        EXEC sp_addpublication 
            @publication = @publication, 
            @status = N'active',
            @allow_push = N'true',
            @allow_pull = N'true',
            @independent_agent = N'true';
        GO
        """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_addpublication_snapshot(this SqlConnection conn, CancellationToken ct,
        string publisherDb, string publication, string login, string password)
    {
        var formattedSql = $$"""
        -- Use the database
        USE [{{publisherDb}}];
        GO
        EXEC sp_addpublication_snapshot 
            @publication = @publication, 
            -- Explicitly specify the use of Windows Integrated Authentication (default) 
            -- when connecting to the Publisher.
            @publisher_security_mode = 0,
            @publisher_login = @login,
            @publisher_password = @password;
        GO
        """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication });
        parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar) { Value = login });
        parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_addarticle(this SqlConnection conn, CancellationToken ct,
        string publisherDb, string publication, string table, string schemaowner = "dbo")
    {
        var formattedSql = $$"""
        -- Use the database
        USE [{{publisherDb}}];
        GO
        EXEC sp_addarticle 
            @publication = @publication, 
            @article = @table, 
            @source_object = @table,
            @source_owner = @schemaowner, 
            @schema_option = 0x80030F3,
            @vertical_partition = N'true', 
            @type = N'logbased';
        GO
        """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication});
        parameters.Add(new SqlParameter("@table", SqlDbType.NVarChar) { Value = table });
        parameters.Add(new SqlParameter("@schemaowner", SqlDbType.NVarChar) { Value = schemaowner });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_addsubscription(this SqlConnection conn, CancellationToken ct,
        string publisherDb, string publication, string subscriberDb)
    {
        var formattedSql = $$"""
        -- Use the database
        USE [{{publisherDb}}];
        GO
        EXEC sp_addsubscription 
            @publication = @publication, 
            @subscriber = @@servername, 
            @destination_db = @destination_db, 
            @subscription_type = N'push';
        GO
        """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication});
        parameters.Add(new SqlParameter("@destination_db", SqlDbType.NVarChar) { Value = subscriberDb });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

    public static async Task<int> sp_addpushsubscription_agent(this SqlConnection conn, CancellationToken ct,
        string publisherDb, string publication, string subscriberDb, string login, string password)
    {
        var formattedSql = $$"""
        -- Use the database
        USE [{{publisherDb}}];
        GO
        EXEC sp_addpushsubscription_agent 
            @publication = @publication, 
            @subscriber = @@servername, 
            @subscriber_db = @subscriber_db, 
            @subscriber_security_mode = 0,
            @subscriber_login = @login,
            @subscriber_password = @password;
        GO
        """;

        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication});
        parameters.Add(new SqlParameter("@subscriber_db", SqlDbType.NVarChar) { Value = subscriberDb });
        parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar) { Value = login });
        parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar) { Value = password });
        return await ExecuteNonQueryAsync(conn ,formattedSql,ct, parameters.ToArray() );
    }

#endregion

}