using appify.models;


namespace appify.Business.Contract
{
	public interface IBulkImportedProductBusiness
	{
        public BulkImportedProduct GetBulkImportedProduct(Int64 itemID);

        public List<BulkImportedProduct> ListBulkImportedProduct(short vendorID, string productFileName);

        public BulkImportedProduct SaveBulkImportedProduct(BulkImportedProduct item);
        public List<BulkImportedProduct> SaveBulkImportedProducts(List<BulkImportedProduct> item);
        public string checkProductFileName(string productFileName);
        public bool SaveBulkImportedProductsToMain(long VendorID);
        public bool SaveBulkImportedProductsToMain(List<BulkImportedProduct> item);
        public bool DeleteBulkImportedProduct(Int64 itemID);
        public List<BulkImportedProductLog> GetBulkImportedProductsLogs(long vendorID, string productFileName);
        public List<BulkImportedProductHistory> GetBulkImportedProductsHistory(long vendorID);
    }
}
