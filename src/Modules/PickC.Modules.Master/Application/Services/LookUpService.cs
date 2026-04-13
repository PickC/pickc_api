using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;

namespace PickC.Modules.Master.Application.Services;

public interface ILookUpService
{
    Task<List<LookUpDto>> GetAllAsync(CancellationToken ct = default);
    Task<List<LookUpDto>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<bool> SaveAsync(LookUpDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(short lookupId, CancellationToken ct = default);
}

public class LookUpService : ILookUpService
{
    private readonly ILookUpRepository _repository;

    public LookUpService(ILookUpRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LookUpDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<List<LookUpDto>> GetByCategoryAsync(string category, CancellationToken ct = default)
    {
        var items = await _repository.GetByCategoryAsync(category, ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<bool> SaveAsync(LookUpDto dto, CancellationToken ct = default)
    {
        var entity = new LookUp
        {
            LookupID = dto.LookupID,
            LookupCode = dto.LookupCode,
            LookupDescription = dto.LookupDescription,
            LookupCategory = dto.LookupCategory,
            Image = dto.Image
        };
        return await _repository.SaveAsync(entity, ct);
    }

    public async Task<bool> DeleteAsync(short lookupId, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(lookupId, ct);
    }

    private static LookUpDto MapToDto(LookUp l) => new()
    {
        LookupID = l.LookupID,
        LookupCode = l.LookupCode,
        LookupDescription = l.LookupDescription,
        LookupCategory = l.LookupCategory,
        Image = l.Image
    };
}
