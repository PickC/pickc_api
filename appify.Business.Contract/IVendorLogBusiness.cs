using appify.models;


namespace appify.Business.Contract
{
	public interface IVendorLogBusiness
	{
		IVendorAuditLog GetVendorLog(Int64 auditID);

		List<IVendorAuditLog> ListVendorLog();

		Task<bool> SaveVendorLog(IVendorAuditLog item);

		bool DeleteVendorLog(Int64 auditID);

	}
}
