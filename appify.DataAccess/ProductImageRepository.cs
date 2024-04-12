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
    public partial class ProductImageRepository : IProductImageRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public ProductImageRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool AddProductImage(ProductImage productimage)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.SAVEPRODUCTIMAGE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                         
                        cmd.Parameters.AddWithValue("@ImageID",  productimage.ImageID);
                        cmd.Parameters.AddWithValue("@ProductID",   productimage.ProductID);
                        cmd.Parameters.AddWithValue("@ImageName",  productimage.ImageName);
                        cmd.Parameters.AddWithValue("@ContentType",  productimage.ContentType);



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

        public ProductImage GetProductImage(long imageID, long productID)
        {
            ProductImage item = new ProductImage();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.SELECTPRODUCTIMAGE, imageID, productID);
            item = DataTableHelper.ConvertDataTable<ProductImage>(ds.Tables[0]).FirstOrDefault();

            return item;
        }

        public List<ProductImage> GetProductImages(long productID)
        {
            List<ProductImage> items = new List<ProductImage>();
            DataSet ds = SqlHelper.ExecuteDataset(appify_connectionstring, dbroutine.DBStoredProc.LISTPRODUCTIMAGE, productID);
            items = DataTableHelper.ConvertDataTable<ProductImage>(ds.Tables[0]);

            return items;
        }

        public bool RemoveProductImage(long imageID, long productID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(dbroutine.DBStoredProc.DELETEPRODUCTIMAGE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ImageID", imageID);
                        cmd.Parameters.AddWithValue("@ProductID", productID);


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
