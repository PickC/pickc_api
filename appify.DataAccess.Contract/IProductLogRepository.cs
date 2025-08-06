using appify.models;
using System.Transactions;


namespace appify.DataAccess.Contract
{
	public interface IProductLogRepository
	{
		IProductAuditLog GetProductLog(Int64 auditID);

		Task<IEnumerable<IProductAuditLog>> ListProductLog(Int64 productID);

		Task<bool> SaveProductLog(IProductAuditLog item);

		bool DeleteProductLog(Int64 auditID);

	}
}
