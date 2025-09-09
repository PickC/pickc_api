using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appify.DataAccess
{
    public partial class FacebookRepository : IFacebookRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;
        private string VENDORBUSINESSPROFILELIST = "[Operation].[usp_VendorMetaBusinessProfile]";
        private string VENDORMETABUSINESSPROFILESAVE = "[Operation].[usp_VendorMetaBusinessProfileSave]";
        private string VENDORMETABUSINESSPROFILEDELETE = "[Operation].[usp_VendorMetaBusinessProfileDelete]";
        private string VENDORCATALOGLIST = "[Operation].[usp_VendorMetaCatalog]";
        private string VENDORMETACATALOGSAVE = "[Operation].[usp_VendorMetaCatalogSave]";
        private string VENDORMETACATALOGDELETE = "[Operation].[usp_VendorMetaCatalogDelete]";
        private string VENDORMETACATALOGPRODUCTDELETE = "[Operation].[usp_VendorMetaProductDelete]";
        private string VENDORMETACATALOGALLPRODUCSTDELETE = "[Operation].[usp_VendorMetaAllProductsDelete]";
        private string CREATEAPRODUCTTOCATALOG = "[Operation].[usp_VendorMetaProductSave]";
        private string GETMETAAPICONFIG = "[Operation].[usp_MetaConfigGet]";
        private string PRODUCTSBYVENDORMETA = "[Operation].[usp_VendorProductsForMetaGet]";

        public FacebookRepository(IConfiguration config)
        {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }
        public List<MetaProduct> ProductListMeta(long VendorID, short SourceID)
        {
            List<MetaProduct> products = new List<MetaProduct>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, PRODUCTSBYVENDORMETA, VendorID, SourceID);
                products = DataTableHelper.ConvertDataTable<MetaProduct>(ds.Tables[0]);
            }
            return products;
        }
        public MetaApiConfig GetMetaApiConfig(string BusinessID, long VendorID)
        {
            MetaApiConfig metaApiConfig = new MetaApiConfig();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, GETMETAAPICONFIG, VendorID, BusinessID);
                metaApiConfig = DataTableHelper.ConvertDataTable<MetaApiConfig>(ds.Tables[0]).FirstOrDefault();
            }
            return metaApiConfig;
        }
        public List<MetaBusinessConfig> VendorBusinessProfileList(long VendorID)
        {
            List<MetaBusinessConfig> metaBusinessConfig = new List<MetaBusinessConfig>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, VENDORBUSINESSPROFILELIST, VendorID);
                metaBusinessConfig = DataTableHelper.ConvertDataTable<MetaBusinessConfig>(ds.Tables[0]);
            }
            return metaBusinessConfig;
        }
        public MetaBusinessProfile SaveBusinessProfile(MetaBusinessProfile itemData)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(VENDORMETABUSINESSPROFILESAVE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@MTBID", itemData.MTBID);
                        cmd.Parameters.AddWithValue("@BusinessID", itemData.BusinessID);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@BusinessName", itemData.BusinessName);
                        cmd.Parameters.AddWithValue("@UserName", itemData.UserName);
                        cmd.Parameters.AddWithValue("@Password", itemData.Password);
                        cmd.Parameters.AddWithValue("@BusinessMobileNo", itemData.BusinessMobileNo);
                        cmd.Parameters.AddWithValue("@AccessToken", itemData.AccessToken);
                        cmd.Parameters.AddWithValue("@ExpiryDate", itemData.ExpiryDate);
                        cmd.Parameters.AddWithValue("@CreatedBy", itemData.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", itemData.ModifiedBy);
                        cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewMTBID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        itemData.MTBID = Convert.ToInt16(outPutParameter.Value);

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
        public bool DeleteBusinessProfile(MetaBusinessProfileDelete itemData)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(VENDORMETABUSINESSPROFILEDELETE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@MTBID", itemData.MTBID);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@BusinessID", itemData.BusinessID);

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
        public List<MetaCatalogConfig> VendorCatalogList(long VendorID, string BusinessID)
        {
            List<MetaCatalogConfig> metaCatalogConfig = new List<MetaCatalogConfig>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, VENDORCATALOGLIST, VendorID, BusinessID);
                metaCatalogConfig = DataTableHelper.ConvertDataTable<MetaCatalogConfig>(ds.Tables[0]);
            }
            return metaCatalogConfig;
        }
        public MetaCatalog CreateaProductCatalog(MetaCatalog itemData)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(VENDORMETACATALOGSAVE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@MTCID", itemData.MTCID);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@CatalogID", itemData.CatalogID);
                        cmd.Parameters.AddWithValue("@BusinessID", itemData.BusinessID);
                        cmd.Parameters.AddWithValue("@CatalogName", itemData.CatalogName);
                        cmd.Parameters.AddWithValue("@CreatedBy", itemData.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", itemData.ModifiedBy);
                        cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewMTCID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        itemData.MTCID = Convert.ToInt16(outPutParameter.Value);

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
        public bool DeleteProductCatalog(MetaCatalogDelete itemData)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(VENDORMETACATALOGDELETE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@MTCID", itemData.MTCID);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@CatalogID", itemData.CatalogID);

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
        public bool DeleteCatalogProduct(MetaCatalogProuctDelete itemData)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(VENDORMETACATALOGPRODUCTDELETE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@MTPID", itemData.MTPID);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@ProductID", itemData.ProductID);

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
        public bool DeleteALLCatalogProducts(long VendorID, string CatalogID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(VENDORMETACATALOGALLPRODUCSTDELETE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@VendorID", VendorID);
                        cmd.Parameters.AddWithValue("@CatalogID", CatalogID);

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
        public MetaCatalogProduct CreateaProductToCatalog(MetaCatalogProduct itemData)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(CREATEAPRODUCTTOCATALOG))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@MTPID", itemData.MTPID);
                        cmd.Parameters.AddWithValue("@ProductID", itemData.ProductID);
                        cmd.Parameters.AddWithValue("@VendorID", itemData.VendorID);
                        cmd.Parameters.AddWithValue("@CatalogID", itemData.CatalogID);
                        cmd.Parameters.AddWithValue("@RetailerID", itemData.RetailerID);
                        cmd.Parameters.AddWithValue("@CreatedBy", itemData.CreatedBy);
                        cmd.Parameters.AddWithValue("@ModifiedBy", itemData.ModifiedBy);
                        cmd.Parameters.AddWithValue("@IsActive", itemData.IsActive);


                        //Add the output parameter to the command object
                        SqlParameter outPutParameter = new SqlParameter();
                        outPutParameter.ParameterName = "@NewMTPID";
                        outPutParameter.SqlDbType = System.Data.SqlDbType.SmallInt;
                        outPutParameter.Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(outPutParameter);

                        con.Open();
                        result = Convert.ToBoolean(cmd.ExecuteNonQuery());

                        itemData.MTPID = Convert.ToInt16(outPutParameter.Value);

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
    }
}
