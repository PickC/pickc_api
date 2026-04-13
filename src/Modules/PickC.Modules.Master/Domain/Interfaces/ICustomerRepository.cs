using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
    Task<Customer?> GetByMobileAsync(string mobileNo, CancellationToken ct = default);
    Task<bool> SaveAsync(Customer customer, CancellationToken ct = default);
    Task<bool> DeleteAsync(string mobileNo, CancellationToken ct = default);
    Task<bool> UpdateDeviceIdAsync(string mobileNo, string deviceId, CancellationToken ct = default);
    Task<bool> UpdatePasswordAsync(string mobileNo, string newPassword, CancellationToken ct = default);
    Task<bool> SaveOtpAsync(string mobileNo, string otp, DateTime sentAt, CancellationToken ct = default);
    Task<bool> VerifyOtpAsync(string mobileNo, string otp, DateTime verifiedAt, CancellationToken ct = default);
}
