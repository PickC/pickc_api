using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using System;


namespace appify.Business
{
	public partial class ProductLogBusiness : IProductLogBusiness 
	{
		IProductLogRepository repository;
		public ProductLogBusiness (IProductLogRepository repository){
			this.repository = repository;
		}
		public IProductAuditLog GetProductLog(Int64 auditID)
		{
			return repository.GetProductLog(auditID);
		}

		public async Task<List<IProductAuditLog>> ListProductLog(Int64 productID)
		{
			return new List<IProductAuditLog>(await repository.ListProductLog(productID));
		}

		public async Task<bool> SaveProductLog(IProductAuditLog item)
		{
			return await repository.SaveProductLog(item);
		}

		public bool DeleteProductLog(Int64 auditID)
		{
			return repository.DeleteProductLog(auditID);
		}

	}
}
