using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Azure.Core;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace appify.web.api
{
    public class FacebookService : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string Url;
        private readonly string accessToken;
        private readonly string apiVersion;
        //private static string businessId = "743286941807169";
        //private static string catalogId = "713758285029769"; // Catalog ID if you already have one
        public FacebookService(string businessID, long vendorID, IFacebookBusiness facebookBusiness)
        {
            httpClient = new HttpClient();
            //apiVersion = "v23.0";
            MetaApiConfig metaApiConfig = facebookBusiness.GetMetaApiConfig(businessID, vendorID);
            Url = metaApiConfig.APIUrl;
            apiVersion = metaApiConfig.APIVersion;
            accessToken = "EAAhHakaglF0BPTW1HRxcESJfb5AXIh9paEPaZBBJWXADqEmhU03GmotvRGkUeoPGYqHyeS1POM3zhmto1CZBHnaT533bb61ZC6cIEHZBFx9wEXS3K7Ix3i3ZCHvmYjZB1DIQFwWt0cLqafWtdZCpaZAK22cemvovDNnqmYSuGf4xadulQVZCm8Ob9UZCZBds8nAphZAYNwZDZD";//metaApiConfig.AccessToken;
        }
        //public async Task<string> CreateCatalogAsync(string businessID, string catalogName)
        //{
        //    string responseObj = "";
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string url = $"{Url}/{apiVersion}/{businessID}/{Common.CreateCatalog}";
        //            var payload = new
        //            {
        //                name = catalogName,
        //                vertical = "commerce",
        //                access_token = accessToken
        //            };
        //            var json = System.Text.Json.JsonSerializer.Serialize(payload);
        //            var content = new StringContent(json,Encoding.UTF8, "application/json");
        //            var response = await client.PostAsync(url, content);
        //            var result = await response.Content.ReadAsStringAsync();
        //            if(response.IsSuccessStatusCode)
        //            {
        //                responseObj = result;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Unexpected Error: {ex.Message}";
        //    }
        //    return responseObj;
        //}

        public MetaCatalog CreateCatalog(MetaCatalog itemData, IFacebookBusiness facebookBusiness)
        {
            string responseObj = "";
            MetaCatalog metaCalalog = new MetaCatalog();
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{itemData.BusinessID}/{Common.CreateCatalog}";

                    var payload = new
                    {
                        name = itemData.CatalogName,
                        vertical = "commerce",
                        access_token = accessToken
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Synchronous call
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;

                        metaCalalog.MTCID = itemData.MTCID;
                        metaCalalog.CatalogID = JsonDocument.Parse(result).RootElement.GetProperty("id").GetString();
                        metaCalalog.VendorID = itemData.VendorID;
                        metaCalalog.BusinessID = itemData.BusinessID;
                        metaCalalog.CatalogName = itemData.CatalogName;
                        metaCalalog.CreatedBy = itemData.CreatedBy;
                        metaCalalog.ModifiedBy = itemData.ModifiedBy;
                        metaCalalog.IsActive = itemData.IsActive;
                        metaCalalog = facebookBusiness.CreateaProductCatalog(metaCalalog);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine ($"Unexpected Error: {ex.Message}");
            }
            return metaCalalog;
        }

        public MetaCatalog EditCatalogAsync(MetaCatalog itemData, IFacebookBusiness facebookBusiness)
        {
            string responseObj = "";
            MetaCatalog metaCalalog = new MetaCatalog();
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{itemData.CatalogID}";
                    var payload = new
                    {
                        name = itemData.CatalogName,
                        access_token = accessToken
                    };
                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                        metaCalalog.MTCID = itemData.MTCID;
                        metaCalalog.CatalogID = itemData.CatalogID;
                        metaCalalog.VendorID = itemData.VendorID;
                        metaCalalog.BusinessID = itemData.BusinessID;
                        metaCalalog.CatalogName = itemData.CatalogName;
                        metaCalalog.CreatedBy = itemData.CreatedBy;
                        metaCalalog.ModifiedBy = itemData.ModifiedBy;
                        metaCalalog.IsActive = itemData.IsActive;
                        facebookBusiness.CreateaProductCatalog(metaCalalog);
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
            return metaCalalog;
        }
        public async Task<string> DeleteCatalogAsync(MetaCatalogDelete itemData, IFacebookBusiness facebookBusiness)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient()) {
                    string url = $"{Url}/{apiVersion}/{itemData.CatalogID}?access_token={accessToken}";
                    var response = await client.DeleteAsync(url);
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                        facebookBusiness.DeleteProductCatalog(itemData);
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }

        public string GetCatalogsListAsync(string businessID) ///// NOT IN USED
        {
            /////////var rr = fs.GetCatalogsListAsync("743286941807169"); Now working to fetch catalogs
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{businessID}/owned_product_catalogs?access_token={accessToken}";
                    var response =  client.GetAsync(url).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception($"Error fetching catalogs: {result}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }

    public async Task CreateSingleProductAsync(string catalogID) //// NOT IN USED
        {
            string url = $"https://graph.facebook.com/v21.0/{catalogID}/products";

            var json = @"{
                ""retailer_id"": ""SKU12345"",
                ""name"": ""Men's Blue T-Shirt"",
                ""description"": ""Comfortable cotton t-shirt"",
                ""url"": ""https://yourstore.com/product/sku12345"",
                ""image_url"": ""https://yourstore.com/images/sku12345.jpg"",
                ""price"": ""29.99 USD"",
                ""brand"": ""MyBrand"",
                ""availability"": ""in stock"",
                ""condition"": ""new"",
                ""access_token"": """ + accessToken + @"""
            }";

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Upload Product Response: " + result);
        }

        public MetaCatalogProduct CreateProduct(MetaProduct itemData, IFacebookBusiness facebookBusiness)
        {
            string responseObj = "";
            MetaCatalogProduct metaCatalogProduct = new MetaCatalogProduct();
            try
            {
                using (var client = new HttpClient())
                {
                    string requestUrl = $"{Url}/{apiVersion}/{itemData.CatalogID}/products";

                    var payload = new
                    {
                        retailer_id = itemData.RetailerID,   // Unique ID in your system
                        name = itemData.Name,
                        description = itemData.Description,
                        url = itemData.Url,
                        image_url = itemData.ImageUrl,
                        brand = itemData.Brand,
                        price = (int)(itemData.Price*100),               // e.g. "999.00 INR"
                        currency = itemData.Currency,         // e.g. "INR"
                        availability = itemData.Availability, // "in stock" | "out of stock" | "preorder"
                        access_token = accessToken
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(requestUrl, content).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                        metaCatalogProduct.MTPID = itemData.MTPID;
                        metaCatalogProduct.ProductID = JsonDocument.Parse(result).RootElement.GetProperty("id").GetString();
                        metaCatalogProduct.VendorID = itemData.VendorID;
                        metaCatalogProduct.CatalogID = itemData.CatalogID;
                        metaCatalogProduct.RetailerID = itemData.RetailerID;
                        metaCatalogProduct.CreatedBy = itemData.CreatedBy;
                        metaCatalogProduct.ModifiedBy = itemData.ModifiedBy;
                        metaCatalogProduct.IsActive = itemData.IsActive;
                        metaCatalogProduct = facebookBusiness.CreateaProductToCatalog(metaCatalogProduct);
                    }
                    else
                    {
                        throw new Exception($"Error creating product: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
            return metaCatalogProduct;
        }
        public MetaCatalogProduct UpdateProduct(MetaProduct itemData, IFacebookBusiness facebookBusiness)
        {
            string responseObj = "";
            MetaCatalogProduct metaCatalogProduct = new MetaCatalogProduct();
            try
            {
                using (var client = new HttpClient())
                {
                    //string requestUrl = $"{Url}/{apiVersion}/{itemData.CatalogID}/products/{itemData.RetailerID}";
                    string requestUrl = $"{Url}/{apiVersion}/{itemData.ProductID}";
                    var payload = new
                    {
                        retailer_id = itemData.RetailerID,   // Unique ID in your system
                        name = itemData.Name,
                        description = itemData.Description,
                        url = itemData.Url,
                        image_url = itemData.ImageUrl,
                        brand = itemData.Brand,
                        price = (int)(itemData.Price * 100),               // e.g. "999.00 INR"
                        currency = itemData.Currency,         // e.g. "INR"
                        availability = itemData.Availability, // "in stock" | "out of stock" | "preorder"
                        access_token = accessToken
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(requestUrl, content).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                        metaCatalogProduct.MTPID = itemData.MTPID;
                        metaCatalogProduct.ProductID = itemData.ProductID;
                        metaCatalogProduct.VendorID = itemData.VendorID;
                        metaCatalogProduct.CatalogID = itemData.CatalogID;
                        metaCatalogProduct.RetailerID = itemData.RetailerID;
                        metaCatalogProduct.CreatedBy = itemData.CreatedBy;
                        metaCatalogProduct.ModifiedBy = itemData.ModifiedBy;
                        metaCatalogProduct.IsActive = itemData.IsActive;
                        metaCatalogProduct = facebookBusiness.CreateaProductToCatalog(metaCatalogProduct);
                    }
                    else
                    {
                        throw new Exception($"Error creating product: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
            return metaCatalogProduct;
        }

        public async Task<string> UpdateProductAsync(string productId, string catalogId, string retailerId, string name, string description, string price, string currency = "USD") //// NOT IN USED
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient())
                {
                    // Product URL format: https://graph.facebook.com/{version}/{product_fbid}
                    string url = $"{Url}/{apiVersion}/{productId}";

                    var payload = new Dictionary<string, string>
            {
                { "access_token", accessToken },
                { "retailer_id", retailerId }, // REQUIRED
                { "name", name },
                { "description", description },
                { "price", $"{price} {currency}" }
            };

                    var content = new FormUrlEncodedContent(payload);
                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                    }
                    else
                    {
                        throw new Exception($"Error updating product: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }

        public string DeleteProductAsync(MetaCatalogProuctDelete itemData, IFacebookBusiness facebookBusiness)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{itemData.ProductID}?access_token={accessToken}";

                    var response =  client.DeleteAsync(url).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                        facebookBusiness.DeleteCatalogProduct(itemData);
                    }
                    else
                    {
                        responseObj = $"Error: {response.StatusCode}, Details: {result}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }

            return responseObj;
        }

        public string GetAllProductsFromCatalogAsync(string catalogId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string listUrl = $"{Url}/{apiVersion}/{catalogId}/products?access_token={accessToken}";
                    var response = client.GetAsync(listUrl).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                        return $"Error fetching products: {result}";

                    // Parse JSON response
                    var jsonObj = System.Text.Json.JsonDocument.Parse(result);
                    if (!jsonObj.RootElement.TryGetProperty("data", out var products))
                        return "No products found in catalog.";

                    return products.ToString();

                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }

        public List<MetaCatalogProduct> BulkUploadAllProductsToCatalog(List<MetaProduct> itemsData, string CatalogID, IFacebookBusiness facebookBusiness)
        {
            var results = new List<string>();
            MetaCatalogProduct metaCatalogProduct;
            List<MetaCatalogProduct> metaProduct = new List<MetaCatalogProduct>();
            try
            {
                using (var client = new HttpClient())
                {
                    foreach (var item in itemsData)
                    {
                        metaCatalogProduct = new MetaCatalogProduct();
                        string requestUrl = $"{Url}/{apiVersion}/{CatalogID}/products";

                        var payload = new
                        {
                            retailer_id = item.RetailerID,
                            name = item.Name,
                            description = item.Description,
                            url = item.Url,
                            image_url = item.ImageUrl,
                            brand = item.Brand,
                            price = (int)(item.Price * 100), // in minor units
                            currency = item.Currency,
                            availability = item.Availability,
                            access_token = accessToken
                        };

                        var json = System.Text.Json.JsonSerializer.Serialize(payload);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = client.PostAsync(requestUrl, content).GetAwaiter().GetResult();
                        var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (response.IsSuccessStatusCode)
                        {
                            results.Add($"SUCCESS: {result}");
                            metaCatalogProduct.MTPID = item.MTPID;
                            metaCatalogProduct.ProductID = JsonDocument.Parse(result).RootElement.GetProperty("id").GetString();
                            metaCatalogProduct.VendorID = item.VendorID;
                            metaCatalogProduct.CatalogID = CatalogID;
                            metaCatalogProduct.RetailerID = item.RetailerID;
                            metaCatalogProduct.CreatedBy = Convert.ToInt16(item.VendorID);
                            metaCatalogProduct.ModifiedBy = item.ModifiedBy;
                            metaCatalogProduct.IsActive = item.IsActive;
                            metaProduct.Add(facebookBusiness.CreateaProductToCatalog(metaCatalogProduct));
                        }
                        
                        else
                            results.Add($"FAILED: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }

            //return string.Join("\n", results);
            return metaProduct;
        }

        //public string BulkUploadAllProductsToCatalog(List<MetaProduct> itemsData)
        //{
        //    string responseObj = "";
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            string requestUrl = $"{Url}/{apiVersion}/{catalogId}/products";

        //            var products = itemsData.Select(item => new
        //            {
        //                retailer_id = item.RetailerID,
        //                name = item.Name,
        //                description = item.Description,
        //                url = item.Url,
        //                image_url = item.ImageUrl,
        //                brand = item.Brand,
        //                price = (item.Price * 100),
        //                currency = item.Currency,
        //                availability = item.Availability
        //            }).ToList();

        //            var payload = new
        //            {
        //                data = products,
        //                access_token = accessToken
        //            };

        //            var json = JsonSerializer.Serialize(payload);
        //            var content = new StringContent(json, Encoding.UTF8, "application/json");

        //            var response = client.PostAsync(requestUrl, content).GetAwaiter().GetResult();
        //            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //            if (response.IsSuccessStatusCode)
        //                responseObj = result;
        //            else
        //                throw new Exception($"Error bulk uploading products: {result}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Unexpected Error: {ex.Message}";
        //    }
        //    return responseObj;
        //}
        public async Task<string> DeleteAllProductsFromCatalogAsync(long VendorID, string CatalogID, IFacebookBusiness facebookBusiness)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string listUrl = $"{Url}/{apiVersion}/{CatalogID}/products?access_token={accessToken}";
                    var response = await client.GetAsync(listUrl);
                    var result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                        return $"Error fetching products: {result}";

                    // Parse JSON response
                    var jsonObj = System.Text.Json.JsonDocument.Parse(result);
                    if (!jsonObj.RootElement.TryGetProperty("data", out var products))
                        return "No products found in catalog.";

                    List<string> failedDeletions = new List<string>();
                    foreach (var product in products.EnumerateArray())
                    {
                        if (product.TryGetProperty("id", out var productId))
                        {
                            string deleteUrl = $"{Url}/{apiVersion}/{productId.GetString()}?access_token={accessToken}";
                            var deleteResponse = await client.DeleteAsync(deleteUrl);
                            var deleteResult = await deleteResponse.Content.ReadAsStringAsync();

                            if (!deleteResponse.IsSuccessStatusCode)
                            {
                                failedDeletions.Add($"Product {productId.GetString()} failed: {deleteResult}");
                            }
                        }
                    }
                    facebookBusiness.DeleteALLCatalogProducts(VendorID, CatalogID);
                    return failedDeletions.Count == 0
                        ? "All products deleted successfully."
                        : $"Some products failed to delete: {string.Join(", ", failedDeletions)}";
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }

        public string BulkUploadProductFeed(string feedName, string feedUrl, string catalogId)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient())
                {
                    string requestUrl = $"https://graph.facebook.com/v21.0/{catalogId}/product_feeds";

                    var payload = new
                    {
                        name = feedName,
                        schedule = new
                        {
                            interval = "DAILY",  // HOURLY, DAILY, WEEKLY
                            url = feedUrl,       // Public CSV/TSV/XML URL
                            hour = 4             // Time of day to fetch (0-23, UTC)
                        },
                        access_token = accessToken
                    };

                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(requestUrl, content).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                        responseObj = result;
                    else
                        throw new Exception($"Error creating feed: {result}");
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }

        public string SendPurchaseEventAsync(string pixelID, string Token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"https://graph.facebook.com/v21.0/{pixelID}/events?access_token={Token}";
                    var eventTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    var payload = new
                    {
                            data = new[]
                            {
                            new
                            {
                                event_name = "Purchase",
                                event_time = eventTime,
                                action_source = "website",
                                user_data = new
                                {
                                    em = new[]
                                    {
                                        HashData("gurjeetrayat84@gmail.com")
                                    },
                                    ph = new string?[]
                                    {
                                        null
                                    }
                                },
                                attribution_data = new
                                {
                                    attribution_share = "0.3"
                                },
                                custom_data = new
                                {
                                    currency = "USD",
                                    value = "142.52"
                                },
                                original_event_data = new
                                {
                                    event_name = "Purchase",
                                    event_time = eventTime
                                }
                            }
                        }
                    };
                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = client.PostAsync(url, content).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception($"Error creating product: {result}");
                    }
                }
            }
            catch(Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }

        private string HashData(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input.ToLower().Trim()));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }


        public string GetInstagramBusinessAccountIdAsync()
        {
            try
            {
                // Step 1: Get Pages available to the token
                var pagesUrl = $"https://graph.facebook.com/v23.0/me/accounts?access_token={accessToken}";
                var pagesResponse = httpClient.GetAsync(pagesUrl).GetAwaiter().GetResult();
                var pagesJson = pagesResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!pagesResponse.IsSuccessStatusCode)
                    throw new Exception($"Failed to fetch pages: {pagesJson}");

                using var pagesDoc = JsonDocument.Parse(pagesJson);
                foreach (var page in pagesDoc.RootElement.GetProperty("data").EnumerateArray())
                {
                    var pageId = page.GetProperty("id").GetString();

                    // Step 2: Check if page has an Instagram Business Account
                    var igUrl = $"https://graph.facebook.com/v23.0/{pageId}?fields=instagram_business_account&access_token={accessToken}";
                    var igResponse = httpClient.GetAsync(igUrl).GetAwaiter().GetResult();
                    var igJson = igResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!igResponse.IsSuccessStatusCode)
                        continue;

                    using var igDoc = JsonDocument.Parse(igJson);
                    if (igDoc.RootElement.TryGetProperty("instagram_business_account", out var igAcc))
                    {
                        var igId = igAcc.GetProperty("id").GetString();
                        return $"IG ID: {igId}, Page ID: {pageId}";
                    }
                }

                return "No Instagram Business Account found linked to your pages.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public string AssignInstagramToCatalogAsync2(InstagramAccount itemData, IFacebookBusiness facebookBusiness)
        {
            try
            {
                // Step 1: Get connected Facebook Page ID from IG Business Account
                var pageLookupUrl = $"https://graph.facebook.com/{apiVersion}/{itemData.InstagramID}?fields=connected_page&access_token={accessToken}";
                var pageResponse =  httpClient.GetAsync(pageLookupUrl).GetAwaiter().GetResult();
                var pageResult =  pageResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!pageResponse.IsSuccessStatusCode)
                    throw new Exception($"Error fetching connected page: {pageResult}");

                using var doc = JsonDocument.Parse(pageResult);
                if (!doc.RootElement.TryGetProperty("connected_page", out var pageElement))
                    throw new Exception("No connected Facebook Page found for this Instagram Business Account.");

                string pageId = pageElement.GetProperty("id").GetString();

                // Step 2: Assign Page ID to Catalog
                var assignUrl = $"https://graph.facebook.com/{apiVersion}/{itemData.CatalogID}/assigned_users";

                var payload = new
                {
                    user = pageId,
                    role = "CATALOG_MANAGE",
                    access_token = accessToken
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response =  httpClient.PostAsync(assignUrl, content).GetAwaiter().GetResult();
                var result =  response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error assigning catalog to page: {result}");

                return result;
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }

        public string AssignCatalogToPageAsync(string catalogId, string pageId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    string assignUrl = $"https://graph.facebook.com/{apiVersion}/{catalogId}/assigned_pages?access_token={accessToken}";

                    var requestBody = new
                    {
                        page_id = pageId
                    };

                    var jsonPayload = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(assignUrl, content).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error assigning catalog to page: {result}");

                    return result; // should return { "success": true }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
        }

        public string AttachCatalogToFacebookPageAsync(string pageId, string catalogId)
        {
            // Define the API endpoint to attach the product catalog to the Facebook Page.
            var requestUrl = $"{Url}/{apiVersion}/{pageId}/owned_product_catalogs?access_token={accessToken}";

            // The request body needs to specify the product catalog ID.
            var requestBody = new
            {
                product_catalog_id = catalogId
            };
            var jsonPayload = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = httpClient.PostAsync(requestUrl, content).GetAwaiter().GetResult(); ;

                // Throw an exception if the response status code indicates an error.
                response.EnsureSuccessStatusCode();

                // Read the response content to check for success.
                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                if (result.success != null && result.success == true)
                {
                    return $"Successfully attached catalog ID {catalogId} to Facebook Page ID {pageId}.";
                }
                else
                {
                    return "Failed to attach catalog. The API returned an error.";
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP-specific errors, such as network issues or bad status codes.
                return $"Request failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors.
                return $"An unexpected error occurred: {ex.Message}";
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
