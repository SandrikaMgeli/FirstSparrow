using System.Data;
using Dapper;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManagementContext
{
    internal NpgsqlConnection? Connection { get; set; }

    internal NpgsqlTransaction? Transaction { get; set; }
}

public static class DbManagementContextExtensions
{
    /// <summary>
    /// Returns amount of raws effected
    /// </summary>
    public static Task<int> ExecuteAsync(this DbManagementContext context, string sql, object? param)
    {
        return context.Connection!.ExecuteAsync(sql, param, context.Transaction);
    }

    public static Task<T?> QuerySingleOrDefaultAsync<T>(this DbManagementContext context, string sql, object? param)
    {
        return context.Connection!.QuerySingleOrDefaultAsync<T>(sql, param, context.Transaction);
    }
}