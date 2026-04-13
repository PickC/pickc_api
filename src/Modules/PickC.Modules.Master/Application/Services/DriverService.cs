using PickC.Modules.Master.Application.DTOs;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.SharedKernel.Exceptions;

namespace PickC.Modules.Master.Application.Services;

public interface IDriverService
{
    Task<List<DriverDto>> GetAllAsync(CancellationToken ct = default);
    Task<DriverDetailDto> GetByIdAsync(string driverId, CancellationToken ct = default);
    Task<List<DriverDto>> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> SaveAsync(DriverSaveDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(string driverId, CancellationToken ct = default);
    Task<bool> UpdatePasswordAsync(DriverUpdatePasswordDto dto, CancellationToken ct = default);
}

public class DriverService : IDriverService
{
    private readonly IDriverRepository _repository;

    public DriverService(IDriverRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DriverDto>> GetAllAsync(CancellationToken ct = default)
    {
        var drivers = await _repository.GetAllAsync(ct);
        return drivers.Select(MapToDto).ToList();
    }

    public async Task<DriverDetailDto> GetByIdAsync(string driverId, CancellationToken ct = default)
    {
        var driver = await _repository.GetByIdAsync(driverId, ct)
            ?? throw new NotFoundException("Driver", driverId);

        return new DriverDetailDto
        {
            DriverID = driver.DriverID,
            DriverName = driver.DriverName,
            VehicleNo = driver.VehicleNo,
            MobileNo = driver.MobileNo,
            PhoneNo = driver.PhoneNo,
            LicenseNo = driver.LicenseNo,
            Status = driver.Status,
            OperatorID = driver.OperatorID,
            IsVerified = driver.IsVerified,
            FatherName = driver.FatherName,
            DateOfBirth = driver.DateOfBirth,
            PlaceOfBirth = driver.PlaceOfBirth,
            Gender = driver.Gender,
            MaritialStatus = driver.MaritialStatus,
            PANNo = driver.PANNo,
            AadharCardNo = driver.AadharCardNo,
            Nationality = driver.Nationality,
            VehicleRCNo = driver.VehicleRCNo,
            Addresses = driver.Addresses.Select(a => new AddressDto
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
            }).ToList(),
            Attachments = driver.Attachments.Select(a => new DriverAttachmentDto
            {
                AttachmentId = a.AttachmentId,
                DriverID = a.DriverID,
                LookupCode = a.LookupCode,
                ImagePath = a.ImagePath
            }).ToList(),
            BankDetails = driver.BankDetails is null ? null : new DriverBankDetailsDto
            {
                DriverID = driver.BankDetails.DriverID,
                BankName = driver.BankDetails.BankName,
                Branch = driver.BankDetails.Branch,
                AccountNumber = driver.BankDetails.AccountNumber,
                AccountType = driver.BankDetails.AccountType,
                IFSC = driver.BankDetails.IFSC,
                AccountName = driver.BankDetails.AccountName
            }
        };
    }

    public async Task<List<DriverDto>> GetByNameAsync(string name, CancellationToken ct = default)
    {
        var drivers = await _repository.GetByNameAsync(name, ct);
        return drivers.Select(MapToDto).ToList();
    }

    public async Task<bool> SaveAsync(DriverSaveDto dto, CancellationToken ct = default)
    {
        var driver = new Driver
        {
            DriverID = dto.DriverID,
            DriverName = dto.DriverName,
            Password = dto.Password,
            VehicleNo = dto.VehicleNo,
            FatherName = dto.FatherName,
            DateOfBirth = dto.DateOfBirth,
            PlaceOfBirth = dto.PlaceOfBirth,
            Gender = dto.Gender,
            MaritialStatus = dto.MaritialStatus,
            MobileNo = dto.MobileNo,
            PhoneNo = dto.PhoneNo,
            PANNo = dto.PANNo,
            AadharCardNo = dto.AadharCardNo,
            LicenseNo = dto.LicenseNo,
            Nationality = dto.Nationality,
            OperatorID = dto.OperatorID,
            VehicleRCNo = dto.VehicleRCNo
        };

        return await _repository.SaveAsync(driver, ct);
    }

    public async Task<bool> DeleteAsync(string driverId, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(driverId, ct);
    }

    public async Task<bool> UpdatePasswordAsync(DriverUpdatePasswordDto dto, CancellationToken ct = default)
    {
        return await _repository.UpdatePasswordAsync(dto.DriverID, dto.NewPassword, ct);
    }

    private static DriverDto MapToDto(Driver d) => new()
    {
        DriverID = d.DriverID,
        DriverName = d.DriverName,
        VehicleNo = d.VehicleNo,
        MobileNo = d.MobileNo,
        PhoneNo = d.PhoneNo,
        LicenseNo = d.LicenseNo,
        Status = d.Status,
        OperatorID = d.OperatorID,
        IsVerified = d.IsVerified
    };
}
