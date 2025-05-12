using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.Http;
using appify.models;
using appify.Business.Contract;
using System.Data;
using appify.Business;
using Razorpay.Api;

namespace appify.web.api
{
    public class ShopifyGraphQLService : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string storeUrl; //= "https://dsgclothes.myshopify.com";
        private readonly string accessToken; //= "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";
        private readonly string apiVersion;// = "2024-01";//"2023-10";
        private readonly IShopifyBusiness shopifyBusiness;
        private readonly long VendorID;
        private readonly short ReferenceID;
        DataTable shopifyProductMaster = new DataTable();
        DataTable shopifyProductVariant = new DataTable();
        DataTable shopifyProductImage = new DataTable();
        public ShopifyGraphQLService(IShopifyBusiness shopifyBusiness, long vendorID, short referenceID=0)
        {
            httpClient = new HttpClient();
            this.shopifyBusiness = shopifyBusiness;
            VendorID = vendorID;
            var shopifyConfig = this.shopifyBusiness.GetShopifyConfigByVendor(vendorID);
            storeUrl = shopifyConfig.StoreURL;
            accessToken = shopifyConfig.AccessToken;
            apiVersion = shopifyConfig.APIVersion;
            ReferenceID = referenceID;
            CreateTableStucture();
        }

        private async Task<string> PostGraphQLRequestAsync(string query)
        {
            try
            {
                var requestBody = System.Text.Json.JsonSerializer.Serialize(new { query });
                var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, $"{storeUrl}");
                request.Headers.Add("X-Shopify-Access-Token", accessToken);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return jsonResponse;
            }
            catch (Exception ex)
            {
                // Log Exception
                throw new ApplicationException($"GraphQL request failed: {ex.Message}", ex);
            }
        }
        public async Task<JObject> QueryAsync(string query, object variables = null)
        {
            string apiUrl = $"{storeUrl}";

            var payload = new
            {
                query,
                variables
            };
            string jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

            using (HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content))
            {
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();
                return JObject.Parse(result);
            }
        }

        public async Task<bool> FetchAllProductsAsync()
        {
            string cursor = null;
            bool result = false;
            bool hasNextPage = true;
            int page = 1;
            JArray jsonList = new JArray();
            while (hasNextPage)
            {
                Console.WriteLine($"Page: {page}");
                var query = $@"
                query {{
                    products(first: 50{(cursor != null ? $", after: \"{cursor}\"" : "")}) {{
                        edges {{
                            cursor
                            node {{
                                id
                                title
                                descriptionHtml
                                handle
                                status
                                vendor
                                productType
                                tags
                                createdAt
                                updatedAt
                                publishedAt
                                legacyResourceId
                                totalInventory
                                variants(first: 100) {{
                                    edges {{
                                        node {{
                                            id
                                            title
                                            sku
                                            price
                                            position
                                            barcode
                                            weight
                                            weightUnit
                                            inventoryQuantity
                                            createdAt
                                            updatedAt
                                     selectedOptions {{
                                                name
                                                value
                                            }}
                                        }}
                                    }}
                                }}
                                images(first: 10) {{
                                    edges {{
                                        node {{
                                            id
                                            altText
                                            width
                                            height
                                            url
                                        }}
                                    }}
                                }}
                            }}
                        }}
                        pageInfo {{
                            hasNextPage
                            endCursor
                        }}
                    }}
                }}";

                var response = await QueryAsync(query);

                hasNextPage = response["data"]["products"]["pageInfo"]["hasNextPage"].Value<bool>();
                cursor = response["data"]["products"]["pageInfo"]["endCursor"].Value<string>();
                Shopify shopifyProduct = new Shopify();




                var products = response["data"]["products"]["edges"];
                foreach (var product in products)
                {
                    jsonList.Add(product["node"]);
                    var fullnode = jsonList.ToString();
                    var productNode = product["node"];
                    shopifyProduct.ReferenceID = ReferenceID;
                    shopifyProduct.ProductID = productNode["id"]?.Value<string>() ?? "";
                    shopifyProduct.Title = productNode["title"]?.Value<string>() ?? "";
                    shopifyProduct.Description = productNode["descriptionHtml"]?.Value<string>() ?? "";
                    shopifyProduct.Handle = productNode["handle"]?.Value<string>() ?? "";
                    shopifyProduct.Status = productNode["status"]?.Value<string>() ?? "";
                    shopifyProduct.Vendor = productNode["vendor"]?.Value<string>() ?? "";
                    shopifyProduct.VendorID = VendorID;
                    shopifyProduct.ProductType = productNode["productType"]?.Value<string>() ?? "";
                    shopifyProduct.CreatedAt = System.String.IsNullOrEmpty((string?)productNode["createdAt"]) ? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) : Convert.ToDateTime((JValue)productNode["createdAt"]); //productNode["createdAt"]?.Value<DateTime>() ?? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    shopifyProduct.UpdatedAt = System.String.IsNullOrEmpty((string?)productNode["updatedAt"]) ? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) : Convert.ToDateTime((JValue)productNode["updatedAt"]);//productNode["updatedAt"]?.Value<DateTime>() ?? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    shopifyProduct.PublishedAt = System.String.IsNullOrEmpty((string?)productNode["publishedAt"]) ? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) : Convert.ToDateTime((JValue)productNode["publishedAt"]);//productNode["publishedAt"]?.Value<DateTime>() ?? DateTime.MinValue;
                    shopifyProduct.LegacyResourceId = productNode["legacyResourceId"]?.Value<string>() ?? "";
                    shopifyProduct.TotalInventory = productNode["totalInventory"]?.Value<short?>() ?? 0;

                    shopifyProductMaster.Rows.Add(shopifyProduct.ReferenceID, shopifyProduct.ProductID, shopifyProduct.VendorID, shopifyProduct.Vendor, shopifyProduct.Title, shopifyProduct.Description, shopifyProduct.Handle, shopifyProduct.Status, shopifyProduct.ProductType, shopifyProduct.CreatedAt, shopifyProduct.UpdatedAt, shopifyProduct.PublishedAt, shopifyProduct.LegacyResourceId, shopifyProduct.TotalInventory,1);

                    var variants = productNode["variants"]["edges"];
                    foreach (var variant2 in variants)
                    {
                        var variantNode = variant2["node"];
                        ShopifyProductVariant variant = new ShopifyProductVariant
                        {
                            ReferenceID = ReferenceID,
                            VariantID = variantNode["id"]?.Value<string>() ?? "",
                            ProductID = shopifyProduct.ProductID,
                            Title = variantNode["title"]?.Value<string>() ?? "",
                            SKU = variantNode["sku"]?.Value<string>() ?? "",
                            Price = variantNode["price"]?.Value<decimal?>() ?? 0,
                            Position = variantNode["position"]?.Value<short?>() ?? 0,
                            Barcode = variantNode["barcode"]?.Value<string>() ?? "",
                            Weight = variantNode["weight"]?.Value<short?>() ?? 0,
                            WeightUnit = variantNode["weightUnit"]?.Value<string>() ?? "",
                            InventoryQuantity = variantNode["inventoryQuantity"]?.Value<short?>() ?? 0,
                            CreatedAt = System.String.IsNullOrEmpty((string?)variantNode["createdAt"]) ? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) : Convert.ToDateTime((JValue)variantNode["createdAt"]),//variantNode["createdAt"]?.Value<DateTime>() ?? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")),
                            UpdatedAt = System.String.IsNullOrEmpty((string?)variantNode["updatedAt"]) ? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) : Convert.ToDateTime((JValue)variantNode["updatedAt"]),//variantNode["updatedAt"]?.Value<DateTime>() ?? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"))

                        };
                        var selectedOptions = variantNode["selectedOptions"];
                        
                        foreach(var item in selectedOptions)
                        {
                            if (item["name"].ToString().ToLower() == "color")
                                variant.Color = item["value"].ToString();
                            if (item["name"].ToString().ToLower() == "size")
                                variant.Size = item["value"].ToString();
                        }
                        //shopifyProduct.variants.Add(variant);
                        shopifyProductVariant.Rows.Add(variant.ReferenceID,variant.VariantID,variant.ProductID,variant.Title,variant.SKU,variant.Price,variant.Position,variant.Color,variant.Size,variant.Barcode,variant.Weight,variant.WeightUnit,variant.InventoryQuantity,variant.CreatedAt,variant.UpdatedAt,1);
                    }

                    var images = productNode["images"]["edges"];
                    foreach (var image2 in images)
                    {
                        var imageNode = image2["node"];
                        ShopifyProductVariantImage image = new ShopifyProductVariantImage
                        {
                            ReferenceID= ReferenceID,
                            ImageID = imageNode["id"]?.Value<string>() ?? "",
                            ALT = imageNode["altText"]?.Value<string>() ?? "",
                            ProductID = shopifyProduct.ProductID,
                            Width = imageNode["width"]?.Value<short?>() ?? 0,
                            Height = imageNode["height"]?.Value<short?>() ?? 0,
                            SRC = imageNode["url"]?.Value<string>() ?? ""
                        };
                        //shopifyProduct.images.Add(image);
                        shopifyProductImage.Rows.Add(image.ReferenceID,image.ImageID, image.ProductID, image.ALT,image.Width,image.Height,image.SRC,1);
                    }
                    
                }
                Console.WriteLine($"Has Next Page: {hasNextPage}, Cursor: {cursor}");
                result = shopifyBusiness.BulkInsertShopifyProducts(shopifyProductMaster, shopifyProductVariant, shopifyProductImage);////shopifyBusiness.SaveShopifyProduct(shopifyProduct);
                shopifyProductMaster.Rows.Clear();
                shopifyProductVariant.Rows.Clear();
                shopifyProductImage.Rows.Clear();
                page++;
                //if (page > 1) break; //Added break to stop after 3 pages.
            }
            return result;
        }
        private void CreateTableStucture()
        {
            try
            {
                ////// Product Table
                shopifyProductMaster.Columns.Add("ReferenceID", typeof(short));
                shopifyProductMaster.Columns.Add("ProductID", typeof(string));
                shopifyProductMaster.Columns.Add("VendorID", typeof(long));
                shopifyProductMaster.Columns.Add("Vendor", typeof(string));
                shopifyProductMaster.Columns.Add("Title", typeof(string));
                shopifyProductMaster.Columns.Add("Description", typeof(string));
                shopifyProductMaster.Columns.Add("Handle", typeof(string));
                shopifyProductMaster.Columns.Add("Status", typeof(string));
                shopifyProductMaster.Columns.Add("ProductType", typeof(string));
                shopifyProductMaster.Columns.Add("CreatedAt", typeof(DateTime));
                shopifyProductMaster.Columns.Add("UpdatedAt", typeof(DateTime));
                shopifyProductMaster.Columns.Add("PublishedAt", typeof(DateTime));
                shopifyProductMaster.Columns.Add("LegacyResourceId", typeof(string));
                shopifyProductMaster.Columns.Add("TotalInventory", typeof(Int16));
                shopifyProductMaster.Columns.Add("IsActive", typeof(bool));

                shopifyProductVariant.Columns.Add("ReferenceID", typeof(short));
                shopifyProductVariant.Columns.Add("VariantID", typeof(string));
                shopifyProductVariant.Columns.Add("ProductID", typeof(string));
                shopifyProductVariant.Columns.Add("Title", typeof(string));
                shopifyProductVariant.Columns.Add("SKU", typeof(string));
                shopifyProductVariant.Columns.Add("Price", typeof(decimal));
                shopifyProductVariant.Columns.Add("Position", typeof(Int16));
                shopifyProductVariant.Columns.Add("Color", typeof(string));
                shopifyProductVariant.Columns.Add("Size", typeof(string));
                shopifyProductVariant.Columns.Add("Barcode", typeof(string));
                shopifyProductVariant.Columns.Add("Weight", typeof(Int16));
                shopifyProductVariant.Columns.Add("WeightUnit", typeof(string));
                shopifyProductVariant.Columns.Add("InventoryQuantity", typeof(Int16));
                shopifyProductVariant.Columns.Add("CreatedAt", typeof(DateTime));
                shopifyProductVariant.Columns.Add("UpdatedAt", typeof(DateTime));
                shopifyProductVariant.Columns.Add("IsActive", typeof(bool));

                shopifyProductImage.Columns.Add("ReferenceID", typeof(string));
                shopifyProductImage.Columns.Add("ImageID", typeof(string));
                shopifyProductImage.Columns.Add("ProductID", typeof(string));
                shopifyProductImage.Columns.Add("ALT", typeof(string));
                shopifyProductImage.Columns.Add("Width", typeof(Int16));
                shopifyProductImage.Columns.Add("Height", typeof(Int16));
                shopifyProductImage.Columns.Add("SRC", typeof(string));
                shopifyProductImage.Columns.Add("IsActive", typeof(bool));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Fetch Single Product
        //public async Task<string> GetSingleProductAsync(string productId)
        //{
        //    try
        //    {
        //        var query = $@"
        //    query {{
        //      product(id: ""{productId}"") {{
        //        id
        //        title
        //        descriptionHtml
        //        vendor
        //        variants(first: 100) {{
        //            edges {{
        //                node {{
        //                    id
        //                    title
        //                    price
        //                    sku
        //                }}
        //            }}
        //        }}
        //        images(first: 10) {{
        //            edges {{
        //                node {{
        //                    src
        //                    altText
        //                }}
        //            }}
        //        }}
        //      }}
        //    }}";

        //        return await PostGraphQLRequestAsync(query);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException($"Failed to get product: {ex.Message}", ex);
        //    }
        //}

        // Create New Product
        //public async Task<string> CreateProductAsync(string title, string description, string vendor)
        //{
        //    try
        //    {
        //        var mutation = $@"
        //    mutation {{
        //      productCreate(input: {{
        //        title: ""{title}"",
        //        descriptionHtml: ""{description}"",
        //        vendor: ""{vendor}""
        //      }}) {{
        //        product {{
        //          id
        //          title
        //        }}
        //        userErrors {{
        //          field
        //          message
        //        }}
        //      }}
        //    }}";

        //        return await PostGraphQLRequestAsync(mutation);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException($"Failed to create product: {ex.Message}", ex);
        //    }
        //}

        // Update Existing Product
        public async Task<string> UpdateProductAsync(string productId, string title, string descriptionHtml, string status)
        {
            try
            {
                var mutation = $@"
            mutation {{
              productUpdate(input: {{
                id: ""{productId}"",
                title: ""{title}"",
                descriptionHtml: ""{descriptionHtml}"",
                status: {status}
              }}) {{
                product {{
                  id
                  title
                  descriptionHtml
                  status
                }}
                userErrors {{
                  field
                  message
                }}
              }}
            }}";

                return await PostGraphQLRequestAsync(mutation);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update product: {ex.Message}", ex);
            }
        }

        // Update Existing Product's Variant
        public async Task<string> UpdateVariantAsync(string variantId, double price, Int16 weight, string weightUnit, Int16 inventoryQuantity)
        {
            try
            {
                var mutation = $@"
            mutation {{
              productVariantUpdate(input: {{
                    id: ""{variantId}"",
                    price: ""{price}"",
                    weight: {weight},
                    weightUnit: {weightUnit},
                    inventoryQuantity: {inventoryQuantity}
              }}) {{
                productVariant {{
                    id
                    price
                    weight
                    weightUnit
                    inventoryQuantity
                }}
                userErrors {{
                  field
                  message
                }}
              }}
            }}";

                return await PostGraphQLRequestAsync(mutation);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update product: {ex.Message}", ex);
            }
        }

        // Update Existing Product's Stock
        public async Task<string> UpdateProductStockAsync(ShopifyProductStock itemData)
        {
            try
            {
                var mutation = $@"
            mutation {{
              productUpdate(input: {{
                id: ""{itemData.ProductID}"",
                totalInventory: ""{itemData.InventoryQuantity}""
              }}) {{
                product {{
                  id
                  title
                }}
                userErrors {{
                  field
                  message
                }}
              }}
            }}";

                return await PostGraphQLRequestAsync(mutation);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to update product: {ex.Message}", ex);
            }
        }

        // Delete Product
        public async Task<string> DeleteProductAsync(string productId)
        {
            try
            {
                var mutation = $@"
            mutation {{
              productDelete(input: {{
                id: ""{productId}""
              }}) {{
                deletedProductId
                userErrors {{
                  field
                  message
                }}
              }}
            }}";

                return await PostGraphQLRequestAsync(mutation);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete product: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            //if (_fcmNotificationSetting != null && _fcmNotificationSetting is IDisposable canDispose)
            //{
            //    canDispose.Dispose();
            //}

            GC.SuppressFinalize(this);
            //throw new NotImplementedException();
        }
    }
}
