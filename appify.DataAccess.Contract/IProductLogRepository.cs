using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IProductLogRepository
	{
		IProductAuditLog GetProductLog(Int64 auditID);

		List<IProductAuditLog> ListProductLog(Int64 productID);

		Task<bool> SaveProductLog(IProductAuditLog item);

		bool DeleteProductLog(Int64 auditID);

	}
}
