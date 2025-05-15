using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Drawing;


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
        public const string SAVEBULKIMPORTEDPRODUCTTOMAIN = "[Operation].[usp_GenerateBulkImportedProducts]";
        public const string BULKIMPORTEDPRODUCTLOGS = "[Operation].[usp_BulkImportedProductLogSelect]";
        public const string BULKIMPORTEDPRODUCTHISTORY = "[Operation].[usp_BulkImportedProductHistory]";
        public const string CHECKBULKIMPORTEDPRODUCTFILENAME = "[Operation].[usp_CheckBulkImportedProductFileName]";
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
        public BulkImportedProduct SaveBulkImportedProduct(BulkImportedProduct itemData)
        {

            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEBULKIMPORTEDPRODUCT))
                    {
                        /*
                            @ItemID bigint, 
                            @ProductFileName varchar(500), 
                            @ItemNo smallint, 
                            @VendorID bigint, 
                            @ProductName nvarchar(500), 
                            @BrandName nvarchar(200), 
                            @HSNCode varchar(15), 
                            @Color varchar(50), 
                            @ProductDescription nvarchar(MAX), 
                            @CategoryID varchar(50), 
                            @Category varchar(200), 
                            @Dimension varchar(50), 
                            @Size varchar(50), 
                            @Price decimal, 
                            @Stock int, 
                            @Weight varchar(50), 
                            @Image1 varchar(1000), 
                            @Image2 varchar(1000), 
                            @Image3 varchar(1000), 
                            @Image4 varchar(1000), 
                            @Image5 varchar(1000), 
                            @Remarks varchar(1000), 
                            @ErrorMessage varchar(MAX) 

                         
                         
                         */






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
                        cmd.Parameters.AddWithValue("@Size", itemData.Size);
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

                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewItemID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());
                        itemData.ItemID = Convert.ToInt64(outPutParameter.Value);
                        con.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return itemData;
        }
        public bool SaveBulkImportedProductsToMain(long VendorID)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEBULKIMPORTEDPRODUCTTOMAIN))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@VendorID", VendorID);
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
        public bool SaveBulkImportedProductsToMain(List<BulkImportedProduct> item)
        {
            return true;
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
        public string checkProductFileName(string productFileName)
        {
            string result = "";
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, CHECKBULKIMPORTEDPRODUCTFILENAME, productFileName);
                result = ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0][0].ToString() : "";
                con.Close();
            }

            return result;
        }
        public List<BulkImportedProductLog> GetBulkImportedProductsLogs(long vendorID, string productFileName)
        {
            List<BulkImportedProductLog> items = new List<BulkImportedProductLog>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, BULKIMPORTEDPRODUCTLOGS, vendorID, productFileName);
                items = DataTableHelper.ConvertDataTable<BulkImportedProductLog>(ds.Tables[0]);
                con.Close();
            }

            return items;
        }
        public List<BulkImportedProductHistory> GetBulkImportedProductsHistory(long vendorID)
        {
            List<BulkImportedProductHistory> items = new List<BulkImportedProductHistory>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, BULKIMPORTEDPRODUCTHISTORY, vendorID);
                items = DataTableHelper.ConvertDataTable<BulkImportedProductHistory>(ds.Tables[0]);
                con.Close();
            }

            return items;
        }
    }
}
