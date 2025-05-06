using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;


namespace appify.Business
{
	public partial class OrderLogBusiness : IOrderLogBusiness 
	{
		IOrderLogRepository repository;
		public OrderLogBusiness (IOrderLogRepository repository){
			this.repository = repository;
		}
		public IOrderAuditLog GetOrderLog(Int64 auditID)
		{
			return repository.GetOrderLog(auditID);
		}

		public List<IOrderAuditLog> ListOrderLog(Int64 orderID)
		{
			return repository.ListOrderLog(orderID);
		}

        public async Task<bool> SaveOrderLog(IOrderAuditLog item)
        {

			return await repository.SaveOrderLog(item);
             
            
        }

        public bool DeleteOrderLog(Int64 auditID)
		{
			return repository.DeleteOrderLog(auditID);
		}

	}
}
