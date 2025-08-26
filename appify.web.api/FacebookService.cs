using appify.models;
using appify.utility;
using System.Text;

namespace appify.web.api
{
    public class FacebookService : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string Url;
        private readonly string accessToken; //= "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";
        private readonly string apiVersion;// = "2024-01";//"2023-10";
        private static string businessId = "743286941807169";
        private static string catalogId = "1483040102697129"; // Catalog ID if you already have one
        public FacebookService(string apiKey)
        {
            httpClient = new HttpClient();
            Url = "https://graph.facebook.com";
            accessToken = "EAAhHakaglF0BPbDVrOZCZBOJcBCnaZAFDGpuVmeEZC01jaRRbaDAqm9lJooJjwZAVqF9K1ESgVDsbOsIiuJZCmswsJ9wM49PmRA5m7Tn3Lpn2XNldWGKmuitrEZA7LLqWjgnnqgAkEaFWSxgVYBf0fs9cTQOZBYZBTdAQ3NYRCCbmNxBKzexdmD5My5Vq5cmlnZCvNxhzHQHZAG2KUEsWZCPwj38AXEV6ABRsl7EkuUNapVY";
            apiVersion = "v21.0";
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

        public string CreateCatalog(string businessID, string catalogName)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{businessID}/{Common.CreateCatalog}";

                    var payload = new
                    {
                        name = catalogName,
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
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }

        public async Task<string> EditCatalogAsync(string catalogID, string newCatalogName)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{catalogID}";
                    var payload = new
                    {
                        name = newCatalogName,
                        access_token = accessToken
                    };
                    var json = System.Text.Json.JsonSerializer.Serialize(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content);
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                    }
                }
                
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }
        public async Task<string> DeleteCatalogAsync(string catalogID)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient()) {
                    string url = $"{Url}/{apiVersion}/{catalogID}?access_token={accessToken}";
                    var response = await client.DeleteAsync(url);
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result;
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }

        public async Task<string> GetCatalogsListAsync(string businessID)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{businessID}/owned_product_catalogs?access_token={accessToken}";
                    var response = await client.GetAsync(url);
                    var result = await response.Content.ReadAsStringAsync();
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

    public async Task CreateSingleProductAsync(string catalogID)
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

        public string CreateProduct(MetaProduct itemData)
        {
            string responseObj = "";
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
                        price = (itemData.Price*100),               // e.g. "999.00 INR"
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
                    }
                    else
                    {
                        throw new Exception($"Error creating product: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }
        public string UpdateProduct(string productItemID, MetaProduct itemData)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient())
                {
                    //string requestUrl = $"{Url}/{apiVersion}/{itemData.CatalogID}/products/{itemData.RetailerID}";
                    string requestUrl = $"{Url}/{apiVersion}/{productItemID}";
                    var payload = new
                    {
                        retailer_id = itemData.RetailerID,   // Unique ID in your system
                        name = itemData.Name,
                        description = itemData.Description,
                        url = itemData.Url,
                        image_url = itemData.ImageUrl,
                        brand = itemData.Brand,
                        price = (itemData.Price * 100),               // e.g. "999.00 INR"
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
                    }
                    else
                    {
                        throw new Exception($"Error creating product: {result}");
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Unexpected Error: {ex.Message}";
            }
            return responseObj;
        }

        public async Task<string> UpdateProductAsync(string productId, string catalogId, string retailerId, string name, string description, string price, string currency = "USD")
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

        public string DeleteProductAsync(string productItemID)
        {
            string responseObj = "";
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{Url}/{apiVersion}/{productItemID}?access_token={accessToken}";

                    var response =  client.DeleteAsync(url).GetAwaiter().GetResult();
                    var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        responseObj = result; // Usually returns {"success": true}
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


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
