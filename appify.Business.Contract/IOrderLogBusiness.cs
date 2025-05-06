using appify.models;


namespace appify.Business.Contract
{
	public interface IOrderLogBusiness
	{
		IOrderAuditLog GetOrderLog(Int64 auditID);

        //Task<IEnumerable<IOrderAuditLog>> ListOrderLog(Int64 orderId); // New method

        Task<bool> SaveOrderLog(IOrderAuditLog item);

		bool DeleteOrderLog(Int64 auditID);
         
    }
}
