using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Extensions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Persistence.Constants;
using FirstSparrow.Persistence.Repositories.Base;

namespace FirstSparrow.Persistence.Repositories;

public class OtpRepository(DbManagementContext context) : IOtpRepository
{
    public async Task Insert(Otp otp, CancellationToken cancellationToken = default)
    {
        int optId = await context.QuerySingleAsync<int>(OtpRepositoryQueries.InsertQuery, otp);
        otp.Id = optId;
    }

    public async Task Update(Otp otp, CancellationToken cancellationToken = default)
    {
        await context.ExecuteAsync(OtpRepositoryQueries.UpdateQuery, otp);
    }

    public async Task<Otp?> Get(int id, bool ensureExists, CancellationToken cancellationToken = default)
    {
        Otp? otp = await context.QuerySingleOrDefaultAsync<Otp>(OtpRepositoryQueries.GetByIdQuery, new { Id = id });
        if (ensureExists)
        {
            otp.EnsureExists(id);
        }
        
        return otp;
    }

    public Task Delete(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}