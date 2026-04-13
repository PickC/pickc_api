using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Domain.Interfaces;

public interface ILookUpRepository
{
    Task<List<LookUp>> GetAllAsync(CancellationToken ct = default);
    Task<List<LookUp>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<LookUp?> GetByIdAsync(short lookupId, CancellationToken ct = default);
    Task<bool> SaveAsync(LookUp lookUp, CancellationToken ct = default);
    Task<bool> DeleteAsync(short lookupId, CancellationToken ct = default);
}
