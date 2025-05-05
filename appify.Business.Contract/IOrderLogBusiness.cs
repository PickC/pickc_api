using appify.models;


namespace appify.Business.Contract
{
	public interface IOrderLogBusiness
	{
		IOrderAuditLog GetOrderLog(Int64 auditID);

		List<IOrderAuditLog> ListOrderLog();

		Task<bool> SaveOrderLog(IOrderAuditLog item);

		bool DeleteOrderLog(Int64 auditID);
         
    }
}
