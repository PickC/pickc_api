using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using appify.models;
using appify.utility;
using System.Data;
using System.Data.SqlClient;
using appify.DataAccess.Contract;
namespace appify.DataAccess
{
    public partial class ShopifyRepository : IShopifyRepository
    {
        private IConfiguration configuration;
        private string appify_connectionstring;

        public const string SAVEPRODUCTSBYVENDOR = "[Operation].[usp_ShopifyProductSave]";
        public const string SAVEPRODUCTVARIANTSBYVENDOR = "[Operation].[usp_ShopifyProductVariantSave]";
        public const string SAVEPRODUCTIMAGESBYVENDOR = "[Operation].[usp_ShopifyProductImageSave]";
        public const string GETSHOPIFYCONFIGBYVENDOR = "[Operation].[usp_ShopifyConfigByVendorSelect]";
        public const string GETSHOPIFYCONFIGBYSTOREURL = "[Operation].[usp_ShopifyConfigByStoreSelect]";
        public const string SAVESHOPIFYPRODUCTSTOAPPIFY = "[Operation].[usp_GenerateShopifyProducts]";
        public const string DELETEPRODUCTSBYVENDOR = "[Operation].[usp_ShopifyProductDelete]";
        public const string UPDATEVARIANTSIMAGESBYPRODUCT = "[Operation].[usp_ShopifyVariantsImagesUpdate]";
        public const string GETPRODUCTSBYVENDOR = "[Operation].[usp_ShopifyProductSelect]";
        public const string UPDATESHOPIFYPRODUCTIMAGEPRICE = "[Operation].[usp_ShopifyProductMasterUpdatePriceImage]";
        public ShopifyRepository(IConfiguration config) {
            this.configuration = config;
            this.appify_connectionstring = config["ConnectionStrings:appify.connectionstring"].ToString();
        }

        public bool SaveShopifyProduct(Shopify shopifyProduct)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEPRODUCTSBYVENDOR))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@ReferenceID", shopifyProduct.ReferenceID);
                        cmd.Parameters.AddWithValue("@ProductID", shopifyProduct.ProductID);
                        cmd.Parameters.AddWithValue("@Title", shopifyProduct.Title);
                        cmd.Parameters.AddWithValue("@Description", shopifyProduct.Description);
                        cmd.Parameters.AddWithValue("@Handle", shopifyProduct.Handle);
                        cmd.Parameters.AddWithValue("@Status", shopifyProduct.Status);
                        cmd.Parameters.AddWithValue("@Vendor", shopifyProduct.Vendor);
                        cmd.Parameters.AddWithValue("@VendorID", shopifyProduct.VendorID);
                        cmd.Parameters.AddWithValue("@ProductType", shopifyProduct.ProductType);
                        cmd.Parameters.AddWithValue("@CreatedAt", shopifyProduct.CreatedAt);
                        cmd.Parameters.AddWithValue("@UpdatedAt", shopifyProduct.UpdatedAt);
                        cmd.Parameters.AddWithValue("@PublishedAt", shopifyProduct.PublishedAt);
                        cmd.Parameters.AddWithValue("@LegacyResourceId", shopifyProduct.LegacyResourceId);
                        cmd.Parameters.AddWithValue("@TotalInventory", shopifyProduct.TotalInventory);
                        cmd.Parameters.AddWithValue("@IsActive", shopifyProduct.IsActive);
                        cmd.Parameters.AddWithValue("@CategoryID", shopifyProduct.CategoryID);
                        cmd.Parameters.AddWithValue("@Category", shopifyProduct.Category);
                        cmd.Parameters.AddWithValue("@BreadCrumb", shopifyProduct.BreadCrumb);
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
        public bool SaveShopifyProductVarient(ShopifyProductVariant item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEPRODUCTVARIANTSBYVENDOR))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ReferenceID", item.ReferenceID);
                        cmd.Parameters.AddWithValue("@VariantID", item.VariantID);
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@SKU", item.SKU);
                        cmd.Parameters.AddWithValue("@Price", item.Price);
                        cmd.Parameters.AddWithValue("@Position", item.Position);
                        cmd.Parameters.AddWithValue("@Color", item.Color);
                        cmd.Parameters.AddWithValue("@Size", item.Size);
                        cmd.Parameters.AddWithValue("@Barcode", item.Barcode);
                        cmd.Parameters.AddWithValue("@Weight", item.Weight);
                        cmd.Parameters.AddWithValue("@WeightUnit", item.WeightUnit);
                        cmd.Parameters.AddWithValue("@InventoryQuantity", item.InventoryQuantity);
                        cmd.Parameters.AddWithValue("@InventoryItemID", item.InventoryItemID);
                        cmd.Parameters.AddWithValue("@CreatedAt", item.CreatedAt);
                        cmd.Parameters.AddWithValue("@UpdatedAt", item.UpdatedAt);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);
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
        public bool SaveShopifyProductVarientImage(ShopifyProductVariantImage item)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVEPRODUCTIMAGESBYVENDOR))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ReferenceID", item.ReferenceID);
                        cmd.Parameters.AddWithValue("@ImageID", item.ImageID);
                        cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                        cmd.Parameters.AddWithValue("@ALT", item.ALT);
                        cmd.Parameters.AddWithValue("@Width", item.Width);
                        cmd.Parameters.AddWithValue("@Height", item.Height);
                        cmd.Parameters.AddWithValue("@SRC", item.SRC);
                        cmd.Parameters.AddWithValue("@IsActive", item.IsActive);

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

        public bool DeleteShopifyProduct(string ProductID, long VendorID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(DELETEPRODUCTSBYVENDOR))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProductID", ProductID);
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

        public bool UpdateShopifyVariantsImages(string ProductID, long VendorID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(UPDATEVARIANTSIMAGESBYPRODUCT))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProductID", ProductID);
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

        public ShopifyConfig GetShopifyConfigByVendor(long VendorID)
        {
            ShopifyConfig item = new ShopifyConfig();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, GETSHOPIFYCONFIGBYVENDOR, VendorID);
                item = DataTableHelper.ConvertDataTable<ShopifyConfig>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }
        public ShopifyConfigLite GetShopifyConfigByStoreUrl(string StoreURL)
        {
            ShopifyConfigLite item = new ShopifyConfigLite();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, GETSHOPIFYCONFIGBYSTOREURL, StoreURL);
                item = DataTableHelper.ConvertDataTable<ShopifyConfigLite>(ds.Tables[0]).FirstOrDefault();
            }
            return item;
        }
        public bool BulkInsertShopifyProducts(DataTable shopifyProductMaster, DataTable shopifyProductVariant, DataTable shopifyProductImage)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    con.Open();
                    using (var bulkProductMaster = new SqlBulkCopy(con))
                    {
                        bulkProductMaster.DestinationTableName = "Operation.ShopifyProductMaster";
                        bulkProductMaster.ColumnMappings.Add("ReferenceID", "ReferenceID");
                        bulkProductMaster.ColumnMappings.Add("ProductID", "ProductID");
                        bulkProductMaster.ColumnMappings.Add("Title", "Title");
                        bulkProductMaster.ColumnMappings.Add("Description", "Description");
                        bulkProductMaster.ColumnMappings.Add("Handle", "Handle");
                        bulkProductMaster.ColumnMappings.Add("Status", "Status");
                        bulkProductMaster.ColumnMappings.Add("Vendor", "Vendor");
                        bulkProductMaster.ColumnMappings.Add("VendorID", "VendorID");
                        bulkProductMaster.ColumnMappings.Add("ProductType", "ProductType");
                        bulkProductMaster.ColumnMappings.Add("CreatedAt", "CreatedAt");
                        bulkProductMaster.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                        bulkProductMaster.ColumnMappings.Add("PublishedAt", "PublishedAt");
                        bulkProductMaster.ColumnMappings.Add("LegacyResourceId", "LegacyResourceId");
                        bulkProductMaster.ColumnMappings.Add("TotalInventory", "TotalInventory");
                        bulkProductMaster.ColumnMappings.Add("IsActive", "IsActive");
                        bulkProductMaster.ColumnMappings.Add("CategoryID", "CategoryID");
                        bulkProductMaster.ColumnMappings.Add("Category", "Category");
                        bulkProductMaster.ColumnMappings.Add("BreadCrumb", "BreadCrumb");
                        bulkProductMaster.BatchSize = 1000;
                        bulkProductMaster.BulkCopyTimeout = 120; // in seco
                        bulkProductMaster.WriteToServer(shopifyProductMaster);
                    }
                    using (var bulkProductVariant = new SqlBulkCopy(con))
                    {
                        bulkProductVariant.DestinationTableName = "Operation.ShopifyProductVariant";

                        bulkProductVariant.ColumnMappings.Add("ReferenceID", "ReferenceID");
                        bulkProductVariant.ColumnMappings.Add("VariantID", "VariantID");
                        bulkProductVariant.ColumnMappings.Add("ProductID", "ProductID");
                        bulkProductVariant.ColumnMappings.Add("Title", "Title");
                        bulkProductVariant.ColumnMappings.Add("SKU", "SKU");
                        bulkProductVariant.ColumnMappings.Add("Price", "Price");
                        bulkProductVariant.ColumnMappings.Add("Position", "Position");
                        bulkProductVariant.ColumnMappings.Add("Color", "Color");
                        bulkProductVariant.ColumnMappings.Add("Size", "Size");
                        bulkProductVariant.ColumnMappings.Add("Barcode", "Barcode");
                        bulkProductVariant.ColumnMappings.Add("Weight", "Weight");
                        bulkProductVariant.ColumnMappings.Add("WeightUnit", "WeightUnit");
                        bulkProductVariant.ColumnMappings.Add("InventoryQuantity", "InventoryQuantity");
                        bulkProductVariant.ColumnMappings.Add("InventoryItemID", "InventoryItemID");
                        bulkProductVariant.ColumnMappings.Add("CreatedAt", "CreatedAt");
                        bulkProductVariant.ColumnMappings.Add("UpdatedAt", "UpdatedAt");
                        bulkProductVariant.ColumnMappings.Add("IsActive", "IsActive");
                        bulkProductVariant.BatchSize = 1000;
                        bulkProductVariant.BulkCopyTimeout = 120; // in seco
                        bulkProductVariant.WriteToServer(shopifyProductVariant);
                    }
                    using (var bulkProductImage = new SqlBulkCopy(con))
                    {
                        bulkProductImage.DestinationTableName = "Operation.ShopifyProductImage";
                        bulkProductImage.ColumnMappings.Add("ReferenceID", "ReferenceID");
                        bulkProductImage.ColumnMappings.Add("ImageID", "ImageID");
                        bulkProductImage.ColumnMappings.Add("ProductID", "ProductID");
                        bulkProductImage.ColumnMappings.Add("ALT", "ALT");
                        bulkProductImage.ColumnMappings.Add("Width", "Width");
                        bulkProductImage.ColumnMappings.Add("Height", "Height");
                        bulkProductImage.ColumnMappings.Add("SRC", "SRC");
                        bulkProductImage.ColumnMappings.Add("IsActive", "IsActive");

                        bulkProductImage.BatchSize = 1000;
                        bulkProductImage.BulkCopyTimeout = 120; // in seco
                        bulkProductImage.WriteToServer(shopifyProductImage);
                    }
                    result = true;
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }

        public bool SaveShopifyProductToAppify(long VendorID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(SAVESHOPIFYPRODUCTSTOAPPIFY))
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

        public List<ShopifyProductID> GetShopifyProductIDByVendor(long VendorID)
        {
            List<ShopifyProductID> item = new List<ShopifyProductID>();
            using (SqlConnection con = new SqlConnection(appify_connectionstring))
            {
                con.Open();
                DataSet ds = SqlHelper.ExecuteDataset(con, GETPRODUCTSBYVENDOR, VendorID);
                item = DataTableHelper.ConvertDataTable<ShopifyProductID>(ds.Tables[0]);
            }
            return item;
        }

        public bool UpdateProductImagePrice(string ProductID)
        {
            var result = false;
            //DataTable dt = DataTableHelper.CreateDataTableFromObj(item);
            try
            {
                using (SqlConnection con = new SqlConnection(appify_connectionstring))
                {
                    using (SqlCommand cmd = new SqlCommand(UPDATESHOPIFYPRODUCTIMAGEPRICE))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@ProductID", ProductID);


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
