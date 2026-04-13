using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;

namespace PickC.Modules.Master.Application.Services;

public interface IAddressService
{
    Task<List<AddressDto>> GetByLinkIdAsync(string linkId, CancellationToken ct = default);
    Task<bool> SaveAsync(AddressSaveDto dto, string userId, CancellationToken ct = default);
    Task<bool> DeleteAsync(int addressId, CancellationToken ct = default);
}

public class AddressService : IAddressService
{
    private readonly IAddressRepository _repository;

    public AddressService(IAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AddressDto>> GetByLinkIdAsync(string linkId, CancellationToken ct = default)
    {
        var addresses = await _repository.GetByLinkIdAsync(linkId, ct);
        return addresses.Select(a => new AddressDto
        {
            AddressId = a.AddressId,
            AddressLinkID = a.AddressLinkID,
            SeqNo = a.SeqNo,
            AddressType = a.AddressType,
            Address1 = a.Address1,
            Address2 = a.Address2,
            CityName = a.CityName,
            StateName = a.StateName,
            ZipCode = a.ZipCode,
            MobileNo = a.MobileNo,
            Contact = a.Contact,
            Email = a.Email,
            FullAddress = a.FullAddress
        }).ToList();
    }

    public async Task<bool> SaveAsync(AddressSaveDto dto, string userId, CancellationToken ct = default)
    {
        var address = new Address
        {
            AddressId = dto.AddressId,
            AddressLinkID = dto.AddressLinkID,
            SeqNo = dto.SeqNo,
            AddressType = dto.AddressType,
            Address1 = dto.Address1,
            Address2 = dto.Address2,
            Address3 = dto.Address3,
            Address4 = dto.Address4,
            CityName = dto.CityName,
            StateName = dto.StateName,
            CountryCode = dto.CountryCode,
            ZipCode = dto.ZipCode,
            TelNo = dto.TelNo,
            MobileNo = dto.MobileNo,
            Contact = dto.Contact,
            Email = dto.Email,
            CreatedBy = userId
        };

        return await _repository.SaveAsync(address, ct);
    }

    public async Task<bool> DeleteAsync(int addressId, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(addressId, ct);
    }
}
