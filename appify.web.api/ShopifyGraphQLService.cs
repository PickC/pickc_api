using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace appify.web.api
{
    public class ShopifyGraphQLService : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string shopUrl = "dsgclothes.myshopify.com";
        private readonly string accessToken = "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";
        private readonly string apiVersion = "2024-01";//"2023-10";

        public ShopifyGraphQLService()
        {
            httpClient = new HttpClient();
        }

        private async Task<string> PostGraphQLRequestAsync(string query)
        {
            try
            {
                var requestBody = System.Text.Json.JsonSerializer.Serialize(new { query });
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://{shopUrl}/admin/api/2023-10/graphql.json");
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
            string apiUrl = $"https://{shopUrl}/admin/api/{apiVersion}/graphql.json";

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

        public async Task<string> FetchAllProductsAsync()
        {
            string cursor = null;
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
                                variants(first: 100) {{
                                    edges {{
                                        node {{
                                            id
                                            title
                                            price
                                            sku
                                        }}
                                    }}
                                }}
                                images(first: 10) {{
                                    edges {{
                                        node {{
                                            src
                                            altText
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

                var products = response["data"]["products"]["edges"];

                // Process the 'products' JToken here.  This JToken contains the array
                // of products, each with its 'cursor' and 'node' properties.
                // You can iterate through 'products' and extract the data you need.

                foreach (var product in products)
                {
                    jsonList.Add(product["node"]);
                    ////    string productCursor = product["cursor"].Value<string>();
                    ////    var productNode = product["node"];

                    ////    string productId = productNode["id"].Value<string>();
                    ////    string productTitle = productNode["title"].Value<string>();
                    ////    string productDescription = productNode["descriptionHtml"].Value<string>();

                    ////    Console.WriteLine($"  Cursor: {productCursor}, ID: {productId}, Title: {productTitle}, Description: {productDescription}");

                    ////    // Example of getting variants:
                    ////    var variants = productNode["variants"]["edges"];
                    ////    Console.WriteLine("    Variants:");
                    ////    foreach (var variant in variants)
                    ////    {
                    ////        var variantNode = variant["node"];
                    ////        string variantId = variantNode["id"].Value<string>();
                    ////        string variantTitle = variantNode["title"].Value<string>();
                    ////        decimal variantPrice = variantNode["price"].Value<decimal>();
                    ////        string variantSku = variantNode["sku"].Value<string>();
                    ////        Console.WriteLine($"      ID: {variantId}, Title: {variantTitle}, Price: {variantPrice}, Sku: {variantSku}");
                    ////    }

                    ////    // Example of getting images
                    ////    var images = productNode["images"]["edges"];
                    ////    Console.WriteLine("    Images:");
                    ////    foreach (var image in images)
                    ////    {
                    ////        var imageNode = image["node"];
                    ////        string imageSrc = imageNode["src"].Value<string>();
                    ////        string imageAltText = imageNode["altText"].Value<string>();
                    ////        Console.WriteLine($"      Source: {imageSrc}, Alt Text: {imageAltText}");
                    ////    }
                }

                Console.WriteLine($"Has Next Page: {hasNextPage}, Cursor: {cursor}");
                page++;
                if (page > 1) break; //Added break to stop after 3 pages.
            }
            Console.WriteLine("Done");
            return jsonList.ToString();
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
                variants(first: 100) {{
                    edges {{
                        node {{
                            id
                            title
                            price
                            sku
                        }}
                    }}
                }}
                images(first: 10) {{
                    edges {{
                        node {{
                            src
                            altText
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

        // Update Existing Product
        public async Task<string> UpdateProductAsync(string productId, string title)
        {
            try
            {
                var mutation = $@"
            mutation {{
              productUpdate(input: {{
                id: ""{productId}"",
                title: ""{title}""
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
