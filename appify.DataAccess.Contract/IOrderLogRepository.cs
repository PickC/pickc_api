using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IOrderLogRepository
	{
		IOrderAuditLog GetOrderLog(Int64 auditID);

		Task<IEnumerable<IOrderAuditLog>> ListOrderLog(Int64 orderID);

		Task<bool> SaveOrderLog(IOrderAuditLog item);

		bool DeleteOrderLog(Int64 auditID);

	}
}
