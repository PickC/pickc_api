using appify.models;


namespace appify.Business.Contract
{
	public interface IProductLogBusiness
	{
		IProductAuditLog GetProductLog(Int64 auditID);

		List<IProductAuditLog> ListProductLog();

		Task<bool> SaveProductLog(IProductAuditLog item);

		bool DeleteProductLog(Int64 auditID);

	}
}
