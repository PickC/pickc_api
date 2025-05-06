using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IVendorLogRepository
	{
		IVendorAuditLog GetVendorLog(Int64 auditID);

		List<IVendorAuditLog> ListVendorLog(Int64 vendorID);

		Task<bool> SaveVendorLog(IVendorAuditLog item);

		bool DeleteVendorLog(Int64 auditID);

	}
}
