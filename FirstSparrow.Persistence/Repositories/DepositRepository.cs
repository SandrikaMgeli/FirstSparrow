using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Persistence.Repositories.Base;

namespace FirstSparrow.Persistence.Repositories;

public class DepositRepository(
    DbManagementContext context) : BaseRepository<Deposit, int>(context, DepositRepositorySql.Insert, DepositRepositorySql.Delete, DepositRepositorySql.Update, DepositRepositorySql.GetById), IDepositRepository
{
    
}

public static class DepositRepositorySql
{
    public const string Insert = @$"";

    public const string GetById = null;

    public const string Update = @$"";

    public const string Delete = null;
}