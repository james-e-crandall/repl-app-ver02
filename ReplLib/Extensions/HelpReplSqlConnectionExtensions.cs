using System.Data;
using Microsoft.Data.SqlClient;

namespace ReplLib.Extensions;

public static partial class ReplSqlConnectionExtensions
{
    public static DataTable sp_get_distributor(this SqlConnection conn)
    {
        var parameters = new List<SqlParameter>();
        if(conn.State != ConnectionState.Open)
        {
            conn.Open();
        }
        return GetDataTableFromStoredProc(conn, "sp_get_distributor", parameters.ToArray());
    }

    public static DataTable sp_helplogreader_agent(this SqlConnection conn)
    {
        var parameters = new List<SqlParameter>();
        if(conn.State != ConnectionState.Open)
        {
            conn.Open();
        }
        return GetDataTableFromStoredProc(conn, "sp_helplogreader_agent", parameters.ToArray());
    }

    public static DataTable sp_helppublication(this SqlConnection conn, string publication)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication });
        if(conn.State != ConnectionState.Open)
        {
            conn.Open();
        }
        return GetDataTableFromStoredProc(conn, "sp_helppublication", parameters.ToArray());
    }

    public static DataTable sp_helppublication_snapshot(this SqlConnection conn, string publication)
    {
        var parameters = new List<SqlParameter>();
        if(conn.State != ConnectionState.Open)
        {
            conn.Open();
        }
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication });
        return GetDataTableFromStoredProc(conn, "sp_helppublication_snapshot", parameters.ToArray());
    }

    public static DataTable sp_helparticle(this SqlConnection conn, string publication)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication });
        if(conn.State != ConnectionState.Open)
        {
            conn.Open();
        }
        return GetDataTableFromStoredProc(conn, "sp_helparticle", parameters.ToArray());
    }

    public static DataTable sp_helparticlecolumns(this SqlConnection conn, string publication, string article)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(new SqlParameter("@publication", SqlDbType.NVarChar) { Value = publication });
        parameters.Add(new SqlParameter("@article", SqlDbType.NVarChar) { Value = article });
        if(conn.State != ConnectionState.Open)
        {
            conn.Open();
        }
        return GetDataTableFromStoredProc(conn, "sp_helparticlecolumns", parameters.ToArray());
    }


}