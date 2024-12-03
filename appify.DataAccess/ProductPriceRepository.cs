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
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace appify.DataAccess
{
    public partial class ProductPriceRepository : IProductPriceRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public ProductPriceRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public ProductPrice GetPrice(long priceID, long productID,string size)
        {
            ProductPrice item = new ProductPrice();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTPRODUCTPRICE,priceID, productID,size);
            item = DataTableHelper.ConvertDataTable<ProductPrice>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<ProductPrice> PriceList(long productID)
        {
            List<ProductPrice> items = new List<ProductPrice>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTPRODUCTPRICE, productID);
            items = DataTableHelper.ConvertDataTable<ProductPrice>(ds.Tables[0]);

            return items;
        }
        public List<ProductPriceNew> PriceListNew(long productID)
        {
            List<ProductPriceNew> items = new List<ProductPriceNew>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTPRODUCTPRICENEW, productID);
            items = DataTableHelper.ConvertDataTable<ProductPriceNew>(ds.Tables[0]);

            return items;
        }

        public bool RemovePrice(long priceID, long productID,string size)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEPRODUCTPRICE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@PriceID", priceID);
                        cmd.Parameters.AddWithValue("@ProductID", productID);
                        cmd.Parameters.AddWithValue("@Size", size);


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

        public bool SavePrice(ProductPrice productprice)
        {

            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEPRODUCTPRICE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;


                        cmd.Parameters.AddWithValue("@PriceID",  productprice.PriceID);
                        cmd.Parameters.AddWithValue("@ProductID", productprice.ProductID);
                        cmd.Parameters.AddWithValue("@Size", productprice.Size);
                        cmd.Parameters.AddWithValue("@Price", productprice.Price);
                        cmd.Parameters.AddWithValue("@Discount", productprice.Discount);
                        cmd.Parameters.AddWithValue("@DiscountType",  productprice.DiscountType);
                        cmd.Parameters.AddWithValue("@EffectiveDate", productprice.EffectiveDate);
                        cmd.Parameters.AddWithValue("@Stock", productprice.Stock);
                        cmd.Parameters.AddWithValue("@Weight", productprice.Weight);


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
