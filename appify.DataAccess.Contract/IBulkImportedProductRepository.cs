using appify.models;


namespace appify.DataAccess.Contract
{
	public interface IBulkImportedProductRepository
	{
		public BulkImportedProduct GetBulkImportedProduct(Int64 itemID);

		public List<BulkImportedProduct> ListBulkImportedProduct(short vendorID, string productFileName);

		public bool SaveBulkImportedProduct(BulkImportedProduct item);

		public bool DeleteBulkImportedProduct(Int64 itemID);

	}
}
