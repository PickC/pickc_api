using appify.models;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using System;


namespace appify.Business
{
	public partial class BulkImportedProductBusiness : IBulkImportedProductBusiness 
	{
		IBulkImportedProductRepository repository;
		public BulkImportedProductBusiness (IBulkImportedProductRepository repository){
			this.repository = repository;
		}
		public BulkImportedProduct GetBulkImportedProduct(Int64 itemID)
		{
			return repository.GetBulkImportedProduct(itemID);
		}

        public List<BulkImportedProduct> ListBulkImportedProduct(short vendorID, string productFileName)
		{
			return repository.ListBulkImportedProduct(vendorID,productFileName);
		}


		public bool SaveBulkImportedProducts(List<BulkImportedProduct> item) {

			var result = false;
			foreach (BulkImportedProduct product in item)
			{
                result = repository.SaveBulkImportedProduct(product);
				if (!result) {
					break;
				}
            }

			return result;
        }


        public bool SaveBulkImportedProduct(BulkImportedProduct item)
		{
			return repository.SaveBulkImportedProduct(item);
		}

		public bool DeleteBulkImportedProduct(Int64 itemID)
		{
			return repository.DeleteBulkImportedProduct(itemID);
		}

	}
}
