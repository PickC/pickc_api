using appify.models;


namespace appify.Business.Contract
{
	public interface IProductLogBusiness
	{
		IProductAuditLog GetProductLog(Int64 auditID);

        Task<List<IProductAuditLog>> ListProductLog(Int64 orderId); // New method


        Task<bool> SaveProductLog(IProductAuditLog item);

		bool DeleteProductLog(Int64 auditID);

	}
}
