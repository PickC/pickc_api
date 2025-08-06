using appify.models;


namespace appify.Business.Contract
{
    public interface IVendorLogBusiness
    {
        IVendorAuditLog GetVendorLog(Int64 auditID);

        Task<List<IVendorAuditLog>> ListVendorLog(Int64 vendorID);

        Task<bool> SaveVendorLog(IVendorAuditLog item);

        bool DeleteVendorLog(Int64 auditID);

    }
}
