/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace appify.DataAccess
{
    public class CustomerRepository : ICustomerRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public CustomerRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public List<Member> GetAllCustomersByVendor(long vendorID, int pageNo, int rows)
        {
            List<Member> customers = new List<Member>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PAGEVIEWCUSTOMERBYMEMBER, vendorID, pageNo, rows);
                customers = DataTableHelper.ConvertDataTable<Member>(ds.Tables[0]);
            }
            return customers;
        }

        public List<MemberProduct> ProductList(long vendorID) {
            List<MemberProduct> products = new List<MemberProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PRODUCTSBYVENDOR, vendorID);
                products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);
            }
            return products;


        }
        public List<MemberProduct> ProductListNew(long vendorID)
        {
            List<MemberProduct> products = new List<MemberProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PRODUCTSBYVENDORNEW, vendorID);
                products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);
            }
            return products;


        }
        public ProductListResponse ProductListPageView(ProductSearch itemData)
        {
            ProductListResponse response = new ProductListResponse();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PRODUCTSBYVENDORPAGEVIEW, itemData.VendorID, itemData.ProductName, itemData.CategoryID, itemData.PageNo, itemData.Rows, itemData.PriceFrom, itemData.PriceTo, itemData.StockFrom, itemData.StockTo,itemData.ProductCount);
                response.Products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);
                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    response.TotalCount = Convert.ToInt32(ds.Tables[1].Rows[0][0]);
                }
            }
            return response;
        }
        public List<MemberProduct> ProductListByCategory(long vendorID, long CategoryID, int pageNo, int rows)
        {
            List<MemberProduct> products = new List<MemberProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PRODUCTSBYCATEGORY, vendorID, CategoryID, pageNo, rows);
                products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);
            }
            return products;


        }
        public MemberAllDetail GetMemberAllDetails(long userID)
        {
            MemberAllDetail item = new MemberAllDetail();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.MEMBERALLDETAILS, userID);
                item = DataTableHelper.ConvertDataTable<MemberAllDetail>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }
        public HomePageProductByCategory GetProductListByVAUA(long userID)
        {
            HomePageProductByCategory item = new HomePageProductByCategory();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PRODUCTSBYCATEGORIES, userID);
                item.categories = DataTableHelper.ConvertDataTable<ProductMasterCategories>(ds.Tables[0]);
                item.products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[1]);
                //item.productdetails = DataTableHelper.ConvertDataTable<ProductMaster>(ds.Tables[2]);
            }
                return item;
        }
        public List<MemberPassword> GetMemberPasswordList()
        {
            List<MemberPassword> products = new List<MemberPassword>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.GETMEMBERPASSWORDLIST);
                products = DataTableHelper.ConvertDataTable<MemberPassword>(ds.Tables[0]);
            }
            return products;
        }
        public bool SaveMemberPassword(long userID, string password)
        {
            var result = false;
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.GENERATEMEMBERPASSWORD))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@Password", password);

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
        public List<MemberProduct> ProductListByFeaturedCat(ProductsByFeaturedCat itemData)
        {
            List<MemberProduct> products = new List<MemberProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PRODUCTSBYFEATUREDCAT, itemData.VendorID, itemData.CategoryID, itemData.ParentID, itemData.Count, itemData.ProductCount, itemData.PageNo,itemData.Rows);
                products = DataTableHelper.ConvertDataTable<MemberProduct>(ds.Tables[0]);
            }
            return products;
        }
        public List<ProductPriceLite> GetProductListbyPriceID(string PriceID)
        {
            List<ProductPriceLite> products = new List<ProductPriceLite>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, dbroutine.DBStoredProc.PRODUCTSBYPRICEID, PriceID);
                products = DataTableHelper.ConvertDataTable<ProductPriceLite>(ds.Tables[0]);
            }
            return products;
        }
    }
}
