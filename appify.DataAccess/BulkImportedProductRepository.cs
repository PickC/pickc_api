using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace appify.DataAccess
{
    public partial class BulkImportedProductRepository : IBulkImportedProductRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SELECTBULKIMPORTEDPRODUCT = "[Operation].[usp_BulkImportedProductSelect]";
        public const string LISTBULKIMPORTEDPRODUCT = "[Operation].[usp_BulkImportedProductList]";
        public const string SAVEBULKIMPORTEDPRODUCT = "[Operation].[usp_BulkImportedProductSave]";
        public const string REMOVEBULKIMPORTEDPRODUCT = "[Operation].[usp_BulkImportedProductDelete]";

        public BulkImportedProductRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public BulkImportedProduct GetBulkImportedProduct(Int64 itemID)
        {
            BulkImportedProduct item = new BulkImportedProduct();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, SELECTBULKIMPORTEDPRODUCT, itemID);
                item = DataTableHelper.ConvertDataTable<BulkImportedProduct>(ds.Tables[0]).FirstOrDefault();
                con.Close();
            }

            return item;
        }

        public List<BulkImportedProduct> ListBulkImportedProduct(short vendorID, string productFileName)
        {
            List<BulkImportedProduct> items = new List<BulkImportedProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, LISTBULKIMPORTEDPRODUCT, vendorID, productFileName);
                items = DataTableHelper.ConvertDataTable<BulkImportedProduct>(ds.Tables[0]);
                con.Close();
            }

            return items;

        }
        public bool SaveBulkImportedProduct(BulkImportedProduct itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEBULKIMPORTEDPRODUCT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ItemID", itemData.ItemID);
                        cmd.Parameters.AddWithValue("@ProductFileName", itemData.ProductFileName);
                        cmd.Parameters.AddWithValue("@ItemNo", itemData.ItemNo);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@ProductName", itemData.ProductName);
                        cmd.Parameters.AddWithValue("@BrandName", itemData.BrandName);
                        cmd.Parameters.AddWithValue("@HSNCode", itemData.HSNCode);
                        cmd.Parameters.AddWithValue("@Color", itemData.Color);
                        cmd.Parameters.AddWithValue("@ProductDescription", itemData.ProductDescription);
                        cmd.Parameters.AddWithValue("@CategoryID", itemData.CategoryID);
                        cmd.Parameters.AddWithValue("@Category", itemData.Category);
                        cmd.Parameters.AddWithValue("@Dimension", itemData.Dimension);
                        cmd.Parameters.AddWithValue("@Price", itemData.Price);
                        cmd.Parameters.AddWithValue("@Stock", itemData.Stock);
                        cmd.Parameters.AddWithValue("@Weight", itemData.Weight);
                        cmd.Parameters.AddWithValue("@Image1", itemData.Image1);
                        cmd.Parameters.AddWithValue("@Image2", itemData.Image2);
                        cmd.Parameters.AddWithValue("@Image3", itemData.Image3);
                        cmd.Parameters.AddWithValue("@Image4", itemData.Image4);
                        cmd.Parameters.AddWithValue("@Image5", itemData.Image5);
                        cmd.Parameters.AddWithValue("@Remarks", itemData.Remarks);
                        cmd.Parameters.AddWithValue("@ErrorMessage", itemData.ErrorMessage);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public bool DeleteBulkImportedProduct(Int64 itemID)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(REMOVEBULKIMPORTEDPRODUCT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ItemID", itemID);
                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }


    }
}
