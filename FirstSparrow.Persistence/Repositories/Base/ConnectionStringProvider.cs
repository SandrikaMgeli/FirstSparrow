using System.ComponentModel.DataAnnotations;
using Npgsql;

namespace FirstSparrow.Persistence.Repositories.Base;

public class ConnectionStringProvider
{
    private readonly NpgsqlConnectionStringBuilder _connectionStringBuilder = new NpgsqlConnectionStringBuilder();

    [Required]
    public string Host
    {
        get => _connectionStringBuilder.Host!;
        set => _connectionStringBuilder.Host = value;
    }

    [Required]
    public bool Pooling
    {
        get => _connectionStringBuilder.Pooling;
        set => _connectionStringBuilder.Pooling = value;
    }

    [Required]
    public int MaxPoolSize
    {
        get => _connectionStringBuilder.MaxPoolSize;
        set => _connectionStringBuilder.MaxPoolSize = value;
    }

    [Required]
    public string Username
    {
        get => _connectionStringBuilder.Username!;
        set => _connectionStringBuilder.Username = value;
    }

    [Required]
    public string Password
    {
        get => _connectionStringBuilder.Password!;
        set => _connectionStringBuilder.Password = value;
    }

    [Required]
    public int Port
    {
        get => _connectionStringBuilder.Port;
        set => _connectionStringBuilder.Port = value;
    }

    [Required]
    public string Database
    {
        get => _connectionStringBuilder.Database!;
        set => _connectionStringBuilder.Database = value;
    }

    public string ConnectionString => _connectionStringBuilder.ConnectionString;
}