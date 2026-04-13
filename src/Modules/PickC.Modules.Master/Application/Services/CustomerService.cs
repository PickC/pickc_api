using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.SharedKernel.Exceptions;

namespace PickC.Modules.Master.Application.Services;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerDto> GetByMobileAsync(string mobileNo, CancellationToken ct = default);
    Task<bool> SaveAsync(CustomerSaveDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(string mobileNo, CancellationToken ct = default);
    Task<bool> UpdateDeviceIdAsync(CustomerUpdateDeviceDto dto, CancellationToken ct = default);
    Task<bool> UpdatePasswordAsync(CustomerUpdatePasswordDto dto, CancellationToken ct = default);
}

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CustomerDto>> GetAllAsync(CancellationToken ct = default)
    {
        var customers = await _repository.GetAllAsync(ct);
        return customers.Select(c => new CustomerDto
        {
            MobileNo = c.MobileNo,
            Name = c.Name,
            EmailID = c.EmailID,
            DeviceID = c.DeviceID,
            CreatedOn = c.CreatedOn
        }).ToList();
    }

    public async Task<CustomerDto> GetByMobileAsync(string mobileNo, CancellationToken ct = default)
    {
        var customer = await _repository.GetByMobileAsync(mobileNo, ct)
            ?? throw new NotFoundException("Customer", mobileNo);

        return new CustomerDto
        {
            MobileNo = customer.MobileNo,
            Name = customer.Name,
            EmailID = customer.EmailID,
            DeviceID = customer.DeviceID,
            CreatedOn = customer.CreatedOn
        };
    }

    public async Task<bool> SaveAsync(CustomerSaveDto dto, CancellationToken ct = default)
    {
        var customer = new Customer
        {
            MobileNo = dto.MobileNo,
            Password = dto.Password,
            Name = dto.Name,
            EmailID = dto.EmailID,
            DeviceID = dto.DeviceID
        };

        return await _repository.SaveAsync(customer, ct);
    }

    public async Task<bool> DeleteAsync(string mobileNo, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(mobileNo, ct);
    }

    public async Task<bool> UpdateDeviceIdAsync(CustomerUpdateDeviceDto dto, CancellationToken ct = default)
    {
        return await _repository.UpdateDeviceIdAsync(dto.MobileNo, dto.DeviceID, ct);
    }

    public async Task<bool> UpdatePasswordAsync(CustomerUpdatePasswordDto dto, CancellationToken ct = default)
    {
        return await _repository.UpdatePasswordAsync(dto.MobileNo, dto.NewPassword, ct);
    }
}
