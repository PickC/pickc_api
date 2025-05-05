using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using System;


namespace appify.Business
{
	public partial class VendorLogBusiness : IVendorLogBusiness 
	{
		IVendorLogRepository repository;
		public VendorLogBusiness (IVendorLogRepository repository){
			this.repository = repository;
		}
		public IVendorAuditLog GetVendorLog(Int64 auditID)
		{
			return repository.GetVendorLog(auditID);
		}

		public List<IVendorAuditLog> ListVendorLog()
		{
			return repository.ListVendorLog();
		}

		public async Task<bool> SaveVendorLog(IVendorAuditLog item)
		{
			return await repository.SaveVendorLog(item);
		}

		public bool DeleteVendorLog(Int64 auditID)
		{
			return repository.DeleteVendorLog(auditID);
		}

	}
}
