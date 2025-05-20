using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Repositories.Abstractions.Base;

namespace FirstSparrow.Application.Repositories.Abstractions;

public interface IDepositRepository : IBaseRepository<Deposit, int>
{
    
}