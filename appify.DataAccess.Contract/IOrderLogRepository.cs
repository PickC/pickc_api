using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IOrderLogRepository
	{
		IOrderAuditLog GetOrderLog(Int64 auditID);

		List<IOrderAuditLog> ListOrderLog();

		Task<bool> SaveOrderLog(IOrderAuditLog item);

		bool DeleteOrderLog(Int64 auditID);

	}
}
