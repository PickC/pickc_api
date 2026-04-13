using Microsoft.EntityFrameworkCore;
using PickC.Modules.Identity.Domain.Entities;
using PickC.Modules.Identity.Infrastructure.Data;

namespace PickC.Modules.Identity.Infrastructure.Repositories;

public interface IAuthRepository
{
    Task<CustomerCredential?> GetCustomerByMobileAsync(string mobileNo, CancellationToken ct = default);
}

public class AuthRepository : IAuthRepository
{
    private readonly IdentityDbContext _context;

    public AuthRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerCredential?> GetCustomerByMobileAsync(string mobileNo, CancellationToken ct = default)
    {
        return await _context.CustomerCredentials
            .FirstOrDefaultAsync(c => c.MobileNo == mobileNo, ct);
    }
}
