using System.Data;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class DbManagementContext
{
    internal NpgsqlConnection? Connection { get; set; }

    internal NpgsqlTransaction? Transaction { get; set; }
}