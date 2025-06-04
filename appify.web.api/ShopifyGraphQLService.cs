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
using System.Net.Http.Headers;

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
                    products(first: 1{(cursor != null ? $", after: \"{cursor}\"" : "")}) {{
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
                                            inventoryItem {{
                                                id
                                            }}
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
                        variant.InventoryItemID = variantNode["inventoryItem"]["id"]?.Value<string>() ?? "";
                        var selectedOptions = variantNode["selectedOptions"];
                        
                        foreach(var item in selectedOptions)
                        {
                            if (item["name"].ToString().ToLower() == "color")
                                variant.Color = item["value"].ToString();
                            if (item["name"].ToString().ToLower() == "size")
                                variant.Size = item["value"].ToString();
                        }
                        //shopifyProduct.variants.Add(variant);
                        shopifyProductVariant.Rows.Add(variant.ReferenceID,variant.VariantID,variant.ProductID,variant.Title,variant.SKU,variant.Price,variant.Position,variant.Color,variant.Size,variant.Barcode,variant.Weight,variant.WeightUnit,variant.InventoryQuantity,variant.InventoryItemID, variant.CreatedAt,variant.UpdatedAt,1);
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
                if (page > 1) break; //Added break to stop after 3 pages.
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
                shopifyProductVariant.Columns.Add("InventoryItemID", typeof(string));
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
        public async Task<string> GetSingleProductAsync(string productId)
        {
            try
            {
                var query = $@"
            query {{
              product(id: ""{productId}"") {{
                id
                title
                descriptionHtml
                vendor
                variants(first: 10) {{
                  edges {{
                    node {{
                      id
                      title
                      price
                    }}
                  }}
                }}
              }}
            }}";

                return await PostGraphQLRequestAsync(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to get product: {ex.Message}", ex);
            }
        }

        // Create New Product
        public async Task<string> CreateProductAsync(string title, string description, string vendor)
        {
            try
            {
                var mutation = $@"
            mutation {{
              productCreate(input: {{
                title: ""{title}"",
                descriptionHtml: ""{description}"",
                vendor: ""{vendor}""
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
                throw new ApplicationException($"Failed to create product: {ex.Message}", ex);
            }
        }

        public async Task<string> UpdateShopifyProductAsync(ProductUpdateRequest productData)
        {
            try
            {
                var query = $@"
                    mutation productUpdate {{
                      productUpdate(input: {{
                        id: ""{productData.ProductId}""
                        title: ""{productData.Title}""
                        descriptionHtml: ""{productData.Description}""
                        vendor: ""{productData.Vendor}""
                        productType: ""{productData.ProductType}""
                        status: {productData.Status.ToUpper()}
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

                var client = new HttpClient();
                client.BaseAddress = new Uri($"{storeUrl}/admin/api/2024-04/graphql.json");
                client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

                var content = new StringContent(JsonConvert.SerializeObject(new { query }), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("", content);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Shopify API error: {response.StatusCode} - {json}");
                }

                // Handle variant update separately
                foreach (var variant in productData.Variants)
                {
                    variant.VariantId = "gid://shopify/ProductVariant/50261009858848";
                    variant.Sku = "DB1-BLK-O-" + DateTime.Now.Microsecond.ToString();
                    variant.Price = Convert.ToDecimal("18.99");
                    variant.Inventory = 89;
                    variant.Weight = Convert.ToDecimal("0.0");
                    variant.WeightUnit = "KILOGRAMS";
                    variant.InventoryItemID = 52283253391648;
                    variant.QuantityPurchased = 2;
                    await UpdateProductVariantAsync(variant);
                    if (variant.QuantityPurchased > 0)
                        await UpdateShopifyInventoryAsync(variant.InventoryItemID, variant.QuantityPurchased);
                }

                return "Product updated successfully.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public async Task<string> UpdateProductVariantAsync(ProductVariant variant)
        {
            var query = $@"
            mutation {{
              productVariantUpdate(input: {{
                id: ""{variant.VariantId}""
                sku: ""{variant.Sku}""
                price: ""{variant.Price}""
                weight: {variant.Weight}
                weightUnit: {variant.WeightUnit}
              }}) {{
                productVariant {{
                  id
                }}
                userErrors {{
                  field
                  message
                }}
              }}
            }}";
            var client = new HttpClient();
            client.BaseAddress = new Uri($"{storeUrl}/admin/api/2024-04/graphql.json");
            client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

            var content = new StringContent(JsonConvert.SerializeObject(new { query }), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("", content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Variant update error: {response.StatusCode} - {json}");
            return json;
        }

        // Update Existing Product Inventory
        public async Task<bool> UpdateShopifyInventoryAsync(long inventoryItemId, int quantityPurchased)
        {
            try
            {
                // 1. Get the Inventory Level using the inventoryItemId
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

                var response = await client.GetAsync($"{storeUrl}/admin/api/2024-04/inventory_levels.json?inventory_item_ids={inventoryItemId}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                dynamic inventoryData = JsonConvert.DeserializeObject(json);
                var inventoryLevel = inventoryData.inventory_levels[0];
                string locationId = inventoryLevel.location_id;
                int available = inventoryLevel.available;

                // 2. Calculate updated quantity
                int newQuantity = available - quantityPurchased;
                if (newQuantity < 0) newQuantity = 0;

                // 3. Set the new inventory quantity
                var payload = new
                {
                    location_id = locationId,
                    inventory_item_id = inventoryItemId,
                    available = newQuantity
                };

                var jsonContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                var setResponse = await client.PostAsync($"{storeUrl}/admin/api/2024-04/inventory_levels/set.json", jsonContent);
                setResponse.EnsureSuccessStatusCode();

                // ✅ Successfully updated
                return true;
            }
            catch (HttpRequestException ex)
            {
                // Network-related errors
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // All other errors
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return false;
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

        public async Task<string?> UploadImageToShopifyAsync(IFormFile file, long productId)
        {

            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");

            // Read file into memory
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            // Convert to Base64
            string base64Image = Convert.ToBase64String(imageBytes);

            // Build payload
            var payload = new
            {
                image = new
                {
                    attachment = base64Image,
                    filename = file.FileName,
                    alt = "Uploaded via API"
                }
            };

            string json = System.Text.Json.JsonSerializer.Serialize(payload);

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string url = $"{storeUrl}/admin/api/2024-04/products/{productId}/images.json";

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, httpContent);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(responseBody);
                return doc.RootElement.GetProperty("image").GetProperty("src").GetString();
            }
            else
            {
                throw new Exception($"Shopify API error: {response.StatusCode} - {response.ReasonPhrase}\n{responseBody}");
            }
        }

        public async Task<string> DeleteProductImageAsync(long productId, long imageId)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("X-Shopify-Access-Token", accessToken);

                var endpoint = $"{storeUrl}/admin/api/2024-04/products/{productId}/images/{imageId}.json";
                var response = await client.DeleteAsync(endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to delete image: {response.StatusCode} - {content}");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to delete product's image: {ex.Message}", ex);
            }

            return "";
        }


        //// Update Existing Product
        //public async Task<string> UpdateProductAsync(string productId, string title)
        //{
        //    try
        //    {
        //        var mutation = $@"
        //    mutation {{
        //      productUpdate(input: {{
        //        id: ""{productId}"",
        //        title: ""{title}""
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
        //        throw new ApplicationException($"Failed to update product: {ex.Message}", ex);
        //    }
        //}


        //////// GRAPH QL NOT WORKING //////////////////
        //public async Task UploadProductImageAsync(string productGid, string fileName, string filePath, IFormFile file)
        //{
        //    UploadImageToShopifyAsync(file, 45, shopUrl, accessToken);
        //    return;
        //    try
        //    {
        //        // Step 1: Request staged upload URL
        //        var stagedUploadUrl = await GetStagedUploadUrlAsync(fileName);
        //        if (stagedUploadUrl == null) throw new Exception("Failed to get staged upload URL.");

        //        // Step 2: Upload image to Shopify S3
        //        //await UploadToShopifyS3Async(stagedUploadUrl, filePath);

        //        // Step 3: Attach image to product
        //        //await AttachImageToProductAsync(productGid, stagedUploadUrl.ResourceUrl, fileName);

        //        Console.WriteLine("✅ Image uploaded & attached successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"❌ Error: {ex.Message}");
        //        // You can add logging here
        //    }
        //}

        //private async Task<StagedTarget> GetStagedUploadUrlAsync(string fileName)
        //{
        //    var graphqlQuery = @"
        //        mutation stagedUploadsCreate {
        //          stagedUploadsCreate(input: [
        //            {
        //              filename: """ + fileName + @""",
        //              mimeType: ""image/jpeg"",
        //              resource: FILE,
        //              httpMethod: POST
        //            }
        //          ]) {
        //            stagedTargets {
        //              url
        //              resourceUrl
        //              parameters {
        //                name
        //                value
        //              }
        //            }
        //            userErrors {
        //              field
        //              message
        //            }
        //          }
        //        }";
        //    var response = await QueryAsync(graphqlQuery);
        //    //var request = new HttpRequestMessage(HttpMethod.Post, $"https://{shopUrl}/admin/api/2023-07/graphql.json")//2023-07
        //    //{
        //   //     Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(graphqlQuery), Encoding.UTF8, "application/json")
        //   // };
        //   // request.Headers.Add("X-Shopify-Access-Token", accessToken);

        //   // var response = await _httpClient.SendAsync(request);
        //   // response.EnsureSuccessStatusCode();

        //    //var content = await response.Content.ReadAsStringAsync();
        //    var jsonDoc = JsonDocument.Parse(response.ToString());

        //    var stagedTarget = jsonDoc.RootElement
        //        .GetProperty("data")
        //        .GetProperty("stagedUploadsCreate")
        //        .GetProperty("stagedTargets")[0];

        //    return new StagedTarget
        //    {
        //        Url = stagedTarget.GetProperty("url").GetString(),
        //        ResourceUrl = stagedTarget.GetProperty("resourceUrl").GetString(),
        //        Parameters = stagedTarget.GetProperty("parameters").EnumerateArray()
        //            .ToDictionary(p => p.GetProperty("name").GetString(), p => p.GetProperty("value").GetString())
        //    };
        //}

        //private async Task UploadToShopifyS3Async(StagedTarget target, string filePath)
        //{
        //    var formData = new MultipartFormDataContent();
        //    try {
        //        //// Add all parameters Shopify provided
        //        //foreach (var param in target.Parameters)
        //        //{
        //        //    formData.Add(new StringContent(param.Value), param.Key);
        //        //}

        //        //// Add the file
        //        //var fileBytes = await File.ReadAllBytesAsync(filePath);
        //        //formData.Add(new ByteArrayContent(fileBytes), "file", Path.GetFileName(filePath));

        //        //var response = await _httpClient.PostAsync(target.Url, formData);
        //        //response.EnsureSuccessStatusCode();

        //        using (var client = new HttpClient())
        //        {
        //            var content = new MultipartFormDataContent();
        //            content.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        //            // Add parameters from stagedTargets.parameters
        //            foreach (var param in target.Parameters)
        //            {
        //                if (!string.IsNullOrEmpty(param.Value))
        //                {
        //                    content.Add(new StringContent(param.Value), param.Key);
        //                }
        //            }

        //            // Add the file
        //            using (var fileStream = File.OpenRead(filePath))
        //            {
        //                var fileContent = new StreamContent(fileStream);
        //                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        //                content.Add(fileContent, "file", Path.GetFileName(filePath));
        //                var response = await client.PostAsync(target.Url, content);
        //                if (!response.IsSuccessStatusCode)
        //                {
        //                    var errorContent = await response.Content.ReadAsStringAsync();
        //                    throw new Exception($"Upload failed. Status Code: {response.StatusCode}. Error: {errorContent}");
        //                }
        //            }




        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"❌ Error: {ex.Message}");
        //        // You can add logging here
        //    }
        //}

        //private async Task AttachImageToProductAsync(string productGid, string resourceUrl, string altText)
        //{
        //    var graphqlQuery = new
        //    {
        //        query = @"
        //        mutation productCreateMedia($productId: ID!, $media: [CreateMediaInput!]!) {
        //          productCreateMedia(productId: $productId, media: $media) {
        //            media {
        //              alt
        //              mediaContentType
        //              status
        //            }
        //            mediaUserErrors {
        //              code
        //              field
        //              message
        //            }
        //          }
        //        }",
        //        variables = new
        //        {
        //            productId = productGid,
        //            media = new[]
        //            {
        //            new {
        //                alt = altText,
        //                originalSource = resourceUrl,
        //                mediaContentType = "IMAGE"
        //            }
        //        }
        //        }
        //    };

        //    var request = new HttpRequestMessage(HttpMethod.Post, $"https://{shopUrl}/admin/api/2023-07/graphql.json")//2023-07
        //    {
        //        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(graphqlQuery), Encoding.UTF8, "application/json")
        //    };
        //    request.Headers.Add("X-Shopify-Access-Token", accessToken);

        //    var response = await _httpClient.SendAsync(request);
        //    response.EnsureSuccessStatusCode();

        //    var content = await response.Content.ReadAsStringAsync();
        //    var jsonDoc = JsonDocument.Parse(content);

        //    var errors = jsonDoc.RootElement
        //        .GetProperty("data")
        //        .GetProperty("productCreateMedia")
        //        .GetProperty("mediaUserErrors");

        //    if (errors.GetArrayLength() > 0)
        //    {
        //        var errorMessage = errors[0].GetProperty("message").GetString();
        //        throw new Exception($"Shopify Media Attach Error: {errorMessage}");
        //    }
        //}

        //        public async Task<string> GetTopLevelCategoriesAsync()
        //        {
        //            var query = @"
        //            {
        //              taxonomy {
        //                categories(first: 100) {
        //                  nodes {
        //                    id
        //                    name
        //                    fullName
        //                    isLeaf
        //                    childrenIds
        //                  }
        //                }
        //              }
        //            }";

        //            //var response = await QueryAsync(query);

        //            JArray jsonList = new JArray();
        //            //jsonList.Add(response);

        //            var query2 = @"mutation {
        //  metafieldsSet(metafields: [
        //    {
        //      ownerId: ""gid://shopify/Product/9776273359136"",
        //      namespace: ""taxonomy"",
        //      key: ""category_id"",
        //      type: ""single_line_text_field"",
        //      value: ""1234""
        //    }
        //  ]) {
        //    metafields {
        //      id
        //      key
        //      value
        //    }
        //    userErrors {
        //      field
        //      message
        //    }
        //  }
        //}";

        //            var response2 = await QueryAsync(query2);
        //            jsonList.Add(response2);
        //            return jsonList.ToString();
        //        }

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

    public class ProductUpdateRequest
    {
        public string ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public string ProductType { get; set; }
        public string Status { get; set; } // "ACTIVE" or "DRAFT"
        public List<ProductVariant> Variants { get; set; }
        //public List<ProductImage> Images { get; set; }
    }

    public class ProductVariant
    {
        public string VariantId { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int Inventory { get; set; }
        public Int64 InventoryItemID { get; set; }
        public int QuantityPurchased { get; set; }
        public decimal Weight { get; set; }
        public string WeightUnit { get; set; } // e.g., "KILOGRAMS"
    }
    //public class StagedTarget
    //{
    //    public string Url { get; set; }
    //    public string ResourceUrl { get; set; }
    //    public Dictionary<string, string> Parameters { get; set; }
    //}
}
