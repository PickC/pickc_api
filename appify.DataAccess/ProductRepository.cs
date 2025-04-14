
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public partial class ProductRepository : IProductRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public ProductRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public bool DeleteProduct(long productId,bool? IsActive)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEPRODUCTMASTER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProductID", productId);
                        cmd.Parameters.AddWithValue("@IsActive", IsActive);


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

        public bool UpdateProductImagePrice(long productId)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATEPRODUCTIMAGEPRICE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProductID", productId);


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

        public ProductMaster GetProduct(long productId)
        {
            ProductMaster item = new ProductMaster();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTPRODUCTMASTER, productId);
                item = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }

        public ProductMasterNew GetProductNew(long productId)
        {
            ProductMasterNew item = new ProductMasterNew();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTPRODUCTMASTERNEW, productId);
                item = DataTableHelper.ConvertDataTable<ProductMasterNew>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }
        public List<ProductMaster> GetProducts(long sellerID)
        {
            List<ProductMaster> items = new List<ProductMaster>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTPRODUCTMASTER, sellerID);
                items = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[0]);
            }
            return items;
        }

        public List<ProductMaster> GetAllProducts()
        {
            List<ProductMaster> items = new List<ProductMaster>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTPRODUCTMASTERALL);
                items = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[0]);
            }
            return items;
        }

        public bool HasProduct(long productId)
        {
            throw new NotImplementedException();
        }

        public List<ProductWeb> ListAll()
        {

            List<ProductWeb> items = new List<ProductWeb>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTALLPRODUCT);
                items = DataTableHelper.ConvertDataTable<ProductWeb>(ds.Tables[0]);
            }
            return items;
        }

        public ProductMaster SaveProduct(ProductMaster productmaster)
        {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEPRODUCTMASTER))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ProductID", productmaster.ProductID);
                        cmd.Parameters.AddWithValue("@VendorID", productmaster.VendorID);
                        cmd.Parameters.AddWithValue("@ProductName", productmaster.ProductName);
                        cmd.Parameters.AddWithValue("@Description", productmaster.Description);
                        cmd.Parameters.AddWithValue("@Category", productmaster.Category);
                        cmd.Parameters.AddWithValue("@Brand", productmaster.Brand);
                        cmd.Parameters.AddWithValue("@Size", productmaster.Size);
                        cmd.Parameters.AddWithValue("@Color", productmaster.Color);
                        cmd.Parameters.AddWithValue("@UOM", productmaster.UOM);
                        cmd.Parameters.AddWithValue("@Weight", productmaster.Weight);
                        cmd.Parameters.AddWithValue("@PriceID", productmaster.PriceID);
                        cmd.Parameters.AddWithValue("@Currency", productmaster.Currency);
                        cmd.Parameters.AddWithValue("@ImageID", productmaster.ImageID);
                        cmd.Parameters.AddWithValue("@IsAvailable", productmaster.IsAvailable);
                        cmd.Parameters.AddWithValue("@StockQty", productmaster.StockQty);
                        cmd.Parameters.AddWithValue("@HSNCode", productmaster.HSNCode);
                        cmd.Parameters.AddWithValue("@SKU", productmaster.SKU);
                        //cmd.Parameters.Add(new SqlParameter("@NewProductID",SqlDbType.BigInt).Direction = ParameterDirection.Output);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewProductID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        productmaster.ProductID = Convert.ToInt64(outPutParameter.Value);

                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return productmaster;
        }

        public List<NewProduct> GetNewProductsList(long VendorID,bool IsNew=false)
        {
            List<NewProduct> items = new List<NewProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.LISTNEWPRODUCTS, VendorID, IsNew);
                items = DataTableHelper.ConvertDataTable<NewProduct>(ds.Tables[0]);
            }
            return items;
        }

        public bool UpdateNewProducts(long ProductID, bool IsNew)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.UPDATENEWPRODUCTS))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProductID", ProductID);
                        cmd.Parameters.AddWithValue("@IsNew", IsNew);

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

        public List<ProductMasterCategories> GetProductMasterCategories(long parentID)
        {
            List<ProductMasterCategories> item = new List<ProductMasterCategories>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTPRODUCTMASTERCATEGORIES, parentID);
                item = DataTableHelper.ConvertDataTable<ProductMasterCategories>(ds.Tables[0]);
            }
            return item;
        }
        public List<ProductCategories> GetCategoriesList(long parentID)
        {
            List<ProductCategories> item = new List<ProductCategories>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.CATEGORIESDEFAULTLISTBYID);
                item = DataTableHelper.ConvertDataTable<ProductCategories>(ds.Tables[0]);
            }
            return item;
        }
        public List<ProductCategories> GetALLCategoriesList(long parentID, string? SearchFilter, short? pageNo, short? rows)
        {
            List<ProductCategories> item = new List<ProductCategories>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.CATEGORIESLISTBYID, parentID, SearchFilter, pageNo, rows);
                item = DataTableHelper.ConvertDataTable<ProductCategories>(ds.Tables[0]);
            }
            return item;
        }

        public String GetALLCategoriesListJSON(long parentID)
        {
            string item = "";
            SqlParameter[] parameters = new SqlParameter[]
            {new SqlParameter("@ParentID", SqlDbType.Int) { Value = parentID }};
            //DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.CATEGORIESLISTBYID, parentID);
            //item = ds.Tables[0].Rows.Count>0 ? ds.Tables[0].Rows[0][0].ToString() : "";
            string jsonResult = SqlHelper.ExecuteScalar(appify_connectionstring, CommandType.StoredProcedure, dbroutine.DBStoredProc.CATEGORIESLISTBYID, parameters)?.ToString();
            return jsonResult;
        }
        public List<ProductCategoryName> GetCategorieName(long categoryID)
        {
            List<ProductCategoryName> item = new List<ProductCategoryName>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.CATEGORIESNAME, categoryID);
                item = DataTableHelper.ConvertDataTable<ProductCategoryName>(ds.Tables[0]);
            }
            return item;
        }
        public ParentCategories SaveVendorCategories(ParentCategories vendorCategories)
        {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEPARENTCATEGORIES))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@UserID", vendorCategories.UserID);
                        cmd.Parameters.AddWithValue("@ParentCatID", vendorCategories.ParentCatID);
                        cmd.Parameters.AddWithValue("@IsDefault", vendorCategories.IsDefault);
                        cmd.Parameters.AddWithValue("@IsActive", vendorCategories.IsActive);

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

            return vendorCategories;
        }
        public List<ParentCategories> GetVendorCategories(long VendorID)
        {
            List<ParentCategories> item = new List<ParentCategories>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PARENTCATEGORIES, VendorID);
                item = DataTableHelper.ConvertDataTable<ParentCategories>(ds.Tables[0]);
            }
            return item;
        }

        public List<ParentCategories> GetALLVendorCategories(long VendorID)
        {
            List<ParentCategories> item = new List<ParentCategories>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PARENTALLCATEGORIES, VendorID);
                item = DataTableHelper.ConvertDataTable<ParentCategories>(ds.Tables[0]);
            }
            return item;
        }
        #region Featured Categories


        /// <summary>
        /// Retrieves a list of featured categories for a specific vendor.
        /// </summary>
        /// <param name="vendorID">The unique identifier of the vendor for whom featured categories are to be retrieved.</param>
        /// <returns>A list of <see cref="FeaturedCategories"/> objects representing the featured categories for the specified vendor.</returns>
        /// <remarks>
        /// This method queries the database using a stored procedure to fetch featured categories for the given vendor.
        /// The result is returned as a list of <see cref="FeaturedCategories"/> objects.
        /// </remarks>
        public List<FeaturedCategories> GetFeaturedCategories(long vendorID)
        {
            // Initialize an empty list to store the featured categories.
            List<FeaturedCategories> item = new List<FeaturedCategories>();

            // Execute the stored procedure to fetch featured categories for the specified vendor.
            // The result is stored in a DataSet.
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.FEATUREDCATEGORIES, vendorID);

            // Convert the first table in the DataSet to a list of FeaturedCategories objects.
            item = DataTableHelper.ConvertDataTable<FeaturedCategories>(ds.Tables[0]);

            // Return the list of featured categories.
            return item;
        }



        public bool UpdateFeaturedCategories(FeaturedCategories item) {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEFEATUREDCATEGORIES))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@VendorID", item.VendorID);
                        cmd.Parameters.AddWithValue("@ParentID", item.ParentID);
                        cmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);
                        cmd.Parameters.AddWithValue("@SeqNo", item.SeqNo);

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
        public bool DeleteFeaturedCategories(long VendorID) {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEFEATUREDCATEGORIES))
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

        public List<StockByPriceID> GetStockByPriceID(string PriceID)
        {
            List<StockByPriceID> item = new List<StockByPriceID>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.SELECTSTOCKBYPRICE, PriceID);
                item = DataTableHelper.ConvertDataTable<StockByPriceID>(ds.Tables[0]);
            }
            return item;
        }
        #endregion

    }
    }
