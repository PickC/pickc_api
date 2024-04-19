using appify.DataAccess.Contract;
using appify.models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.utility;

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
        public bool DeleteProduct(long productId)
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
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTPRODUCTMASTER, productId);
            item = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<ProductMaster> GetProducts(long sellerID)
        {
            List<ProductMaster> items = new List<ProductMaster>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTPRODUCTMASTER, sellerID);
            items = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[0]);

            return items;
        }

        public List<ProductMaster> GetAllProducts()
        {
            List<ProductMaster> items = new List<ProductMaster>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTPRODUCTMASTERALL);
            items = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[0]);

            return items;
        }

        public bool HasProduct(long productId)
        {
            throw new NotImplementedException();
        }

        public List<ProductWeb> ListAll()
        {

            List<ProductWeb> items = new List<ProductWeb>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTALLPRODUCT);
            items = DataTableHelper.ConvertDataTable<ProductWeb>(ds.Tables[0]);

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

        public bool UpdateProductIsNewStatus(long productID, bool isNew) {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.PRODUCTMASTERUPDATEISNEW))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ProductID", productID);
                        cmd.Parameters.AddWithValue("@IsNew", isNew);
                        //cmd.Parameters.Add(new SqlParameter("@NewProductID",SqlDbType.BigInt).Direction = ParameterDirection.Output);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewProductID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        
                        con.Close();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
                return result;
            }

            return result;
        }

        public List<NewProduct> NewProducts(long sellerID) {
            
            List<NewProduct> items = new List<NewProduct>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.PRODUCTMASTERISNEWSTATUS);
            items = DataTableHelper.ConvertDataTable<NewProduct>(ds.Tables[0]);

            return items;

        }

        public List<ProductMaster> GetNewProductsList(long VendorID)
        {
            List<ProductMaster> items = new List<ProductMaster>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTNEWPRODUCTS, VendorID);
            items = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[0]);

            return items;
        }

        public bool UpdateNewProducts(long ProductID, int IsNew)
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


    }

}
