using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.Modules.Master.Infrastructure.Data;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Master.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly MasterDbContext _context;

    public CustomerRepository(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Customers
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Customer?> GetByMobileAsync(string mobileNo, CancellationToken ct = default)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.MobileNo == mobileNo, ct);
    }

    public async Task<bool> SaveAsync(Customer customer, CancellationToken ct = default)
    {
        var existing = await _context.Customers
            .FirstOrDefaultAsync(c => c.MobileNo == customer.MobileNo, ct);

        if (existing is null)
        {
            customer.CreatedOn = IstClock.Now;
            _context.Customers.Add(customer);
        }
        else
        {
            // Only update registration fields — preserve OTP columns set by the OTP flow
            existing.Password = customer.Password;
            existing.Name = customer.Name;
            existing.EmailID = customer.EmailID;
            existing.DeviceID = customer.DeviceID;
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(string mobileNo, CancellationToken ct = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.MobileNo == mobileNo, ct);

        if (customer is null) return false;

        _context.Customers.Remove(customer);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> UpdateDeviceIdAsync(string mobileNo, string deviceId, CancellationToken ct = default)
    {
        return await _context.Customers
            .Where(c => c.MobileNo == mobileNo)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(c => c.DeviceID, deviceId), ct) > 0;
    }

    public async Task<bool> UpdatePasswordAsync(string mobileNo, string newPassword, CancellationToken ct = default)
    {
        return await _context.Customers
            .Where(c => c.MobileNo == mobileNo)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(c => c.Password, newPassword), ct) > 0;
    }

    public async Task<bool> SaveOtpAsync(string mobileNo, string otp, DateTime sentAt, CancellationToken ct = default)
    {
        var existing = await _context.Customers
            .FirstOrDefaultAsync(c => c.MobileNo == mobileNo, ct);

        if (existing is null)
        {
            _context.Customers.Add(new Customer
            {
                MobileNo = mobileNo,
                OTP = otp,
                OTPSendDate = sentAt,
                IsOTPVerified = false,
                CreatedOn = sentAt
            });
        }
        else
        {
            existing.OTP = otp;
            existing.OTPSendDate = sentAt;
            existing.IsOTPVerified = false;
            existing.OTPVerifiedDate = null;
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> VerifyOtpAsync(string mobileNo, string otp, DateTime verifiedAt, CancellationToken ct = default)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.MobileNo == mobileNo, ct);

        if (customer is null || customer.OTP != otp)
            return false;

        customer.IsOTPVerified = true;
        customer.OTPVerifiedDate = verifiedAt;

        return await _context.SaveChangesAsync(ct) > 0;
    }
}
