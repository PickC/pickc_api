/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2025-01-02
 * Description:
*/
using appify.Business.Contract;
using appify.models;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using appify.utility;

using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using System.IO.Compression;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    public class CommonServicesController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IOrderBusiness orderBusiness;
        private readonly INotificationBusiness notificationBusiness;
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;

        public CommonServicesController(IConfiguration configuration, IEventLogBusiness eventLogBusiness, IOrderBusiness orderBusiness, INotificationBusiness notificationBusiness, IWebHostEnvironment env)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this.configuration = configuration;
            this.eventLogBusiness = eventLogBusiness;
            this.orderBusiness = orderBusiness;
            this.notificationBusiness = notificationBusiness;
            this.env = env;
        }

        [HttpPost]
        [Route("Delhivery/GetOrderStatus")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetDelhiveryOrderStatus(string awbNumber)
        {
            GetExpectedDeliveryDateAsync(awbNumber);
            return Ok(0);
        }
        private async Task<string?> GetExpectedDeliveryDateAsync(string awbNumber)
        {
            using var client = new HttpClient();
            var token = configuration["OneDelhiveryKey:Key"].ToString();
            client.DefaultRequestHeaders.Add("Authorization", $"Token {token}");

            var response = await client.GetAsync($"https://track.delhivery.com/api/v1/packages/json/?waybill={awbNumber}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);

            var edd = data["ShipmentData"]?[0]?["Shipment"]?["ExpectedDeliveryDate"]?.ToString();
            return edd;
        }

        /// <summary>
        ///     Get The One Delivery Shipment Cost
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "startPin": "500081",
        ///       "destPin": "500001",
        ///       "weight": 500.0,
        ///       "payType": "COD",
        ///       "codAmount": 900.0
        ///     } 
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">DELHIVERY SHIPMENT COST HAS BEEN SUCCESSFULLY FETCHED!</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("Delhivery/GetShipmentCost")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> getShipmentCost(DelhiveryShipmentCost itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Clear();
                var ApiToken = configuration["OneDelhiveryKey:Key"].ToString();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {ApiToken}");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var weightInGrams = itemData.Weight;
                var codValue = itemData.CODAmount;

                var url = $"{Common.OneDelhiveryShipmentCost}?md=S&ss=Delivered&o_pin={itemData.StartPin}&d_pin={itemData.DestPin}&cod={codValue}&cgm={weightInGrams}";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // throws if status code is not 2xx

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonResponse);
                //string cleanJson = jsonArray.ToString(Formatting.None);
                //return jsonResponse;
                rm.statusCode = StatusCodes.OK;
                rm.message = "Delhivery Shipment Cost has been successfully fetched";
                rm.name = StatusName.ok;
                //rm.data = jsonArray;


                var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                decimal totalAmount = jsonObj[0].total_amount;

                rm.data = new { total_amount = totalAmount };
                
                rm.data = new { total_amount = totalAmount };

            }
            catch (HttpRequestException ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("GET SHIPMENT COST - ERROR", reqHeader, controllerURL, itemData, null, StatusName.ok, this.eventLogBusiness);
            }
            return Ok(rm);
        }


        /// <summary>
        ///     Get The One Delivery Pincode
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///    {
        ///      "pinCode": "500081"
        ///    }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">DELHIVERY PINCODE HAS BEEN SUCCESSFULLY FETCHED!</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("Delhivery/GetPincode")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> getPinCode(DeliveryPinCode itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Clear();
                var ApiToken = configuration["OneDelhiveryKey:Key"].ToString();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {ApiToken}");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"{Common.OnDeliveryPincodeService}/?filter_codes={itemData.PinCode}";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedJson = JsonConvert.DeserializeObject(jsonResponse);
                rm.statusCode = StatusCodes.OK;
                rm.message = "Delhivery Pincode has been successfully fetched";
                rm.name = StatusName.ok;
                rm.data = parsedJson;

                // Get the delivery_codes array
                var jsonObj = JObject.Parse(jsonResponse);
                var deliveryCodes = jsonObj["delivery_codes"] as JArray;
                // Return true if array exists and has elements, false otherwise
                bool canDeliver= deliveryCodes != null && deliveryCodes.Count > 0;
                
                rm.data = canDeliver;

            }
            catch (HttpRequestException ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("GET PINCODE - ERROR", reqHeader, controllerURL, itemData, null, StatusName.ok, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        [HttpPost]
        [Route("UploadImagesToAzureAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UploadImagesToAzureAsync([Required][FromForm] ParamVendorUploadImgage itemData)
        {
            string storageConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:StorageConnectionString").Value;
            string containerName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:ContainerName").Value;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            if (itemData.file == null || itemData.file.Length == 0)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "No file uploaded.";
                rm.name = StatusName.invalid;
                rm.data = "No file uploaded.";
                return Ok(rm);
            }


            if (!Path.GetExtension(itemData.file.FileName).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "Only ZIP files are supported.";
                rm.name = StatusName.invalid;
                rm.data = "Only ZIP files are supported.";
                return Ok(rm);
            }



            string tempZipPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".zip");
            string extractPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                using (var stream = new FileStream(tempZipPath, FileMode.Create))
                {
                    await itemData.file.CopyToAsync(stream);
                }

                ZipFile.ExtractToDirectory(tempZipPath, extractPath);

                var containerClient = new BlobContainerClient(storageConnectionString, containerName);
                await containerClient.CreateIfNotExistsAsync();

                foreach (var file in Directory.GetFiles(extractPath, "*.*", SearchOption.AllDirectories))
                {
                    var provider = new FileExtensionContentTypeProvider();
                    string contentType;

                    if (!provider.TryGetContentType(file, out contentType))
                    {
                        contentType = "application/octet-stream";
                    }

                    var blobHttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    };
                    string relativePath = Path.GetRelativePath(extractPath, file).Replace("\\", "/");

                    if (relativePath.Contains("/"))
                    {
                        relativePath = string.Join("/", relativePath.Split('/').Skip(1));
                    }

                    string blobPath = $"{itemData.VendorID}/{itemData.FolderName}/{relativePath}";
                    var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var blobClient = containerClient.GetBlobClient(blobPath);
                        try
                        {
                            //await blobClient.UploadAsync(fileStream, overwrite: true, cancellationToken: cts.Token);
                            if (await blobClient.ExistsAsync())
                            {
                                await blobClient.DeleteAsync();
                            }

                            await blobClient.UploadAsync(fileStream, new BlobUploadOptions
                            {
                                HttpHeaders = blobHttpHeaders
                            }, cancellationToken: cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            Console.WriteLine("Upload cancelled due to timeout or cancellation token.");
                        }
                    }
                }

                rm.statusCode = StatusCodes.OK;
                rm.message = "ZIP extracted and uploaded successfully.";
                rm.name = StatusName.ok;
                rm.data = "ZIP extracted and uploaded successfully.";
                return Ok(rm);

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = $"Upload failed: {ex.Message}";
                rm.name = StatusName.invalid;
                rm.data = $"Upload failed: {ex.Message}";
                return Ok(rm);
            }
            finally
            {
                if (System.IO.File.Exists(tempZipPath))
                    System.IO.File.Delete(tempZipPath);

                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);
            }
        }

        [HttpPost]
        [Route("fetchdrivetoazure")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DownloadVendorIconsFromDriveAndUploadToAzure(string parentFolderId)
        {
            string storageConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:StorageConnectionString").Value;
            string containerName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Azure:ContainerName").Value;
            var driveService = GetDriveService();

            var listRequest = driveService.Files.List();
            listRequest.Q = $"'{parentFolderId}' in parents and trashed = false";
            listRequest.Fields = "files(id, name, mimeType)";

            var files = await listRequest.ExecuteAsync();


            foreach (var file in files.Files)
            {
                Console.WriteLine($"{file.Name} - {file.MimeType} - {file.Id}");

                // If it's a folder
                if (file.MimeType == "application/vnd.google-apps.folder")
                {
                    // Recursively list files inside this folder
                    var childRequest = driveService.Files.List();
                    childRequest.Q = $"'{file.Id}' in parents and trashed = false";
                    childRequest.Fields = "files(id, name, mimeType)";
                    var childFiles = await childRequest.ExecuteAsync();

                    foreach (var child in childFiles.Files)
                    {
                        Console.WriteLine($"  └ {child.Name} - {child.Id}");
                    }
                }
            }

            //foreach (var file in files.Files)
            //{
            //    var stream = new MemoryStream();
            //    await driveService.Files.Get(file.Id).DownloadAsync(stream);
            //    stream.Position = 0;

            //    // Upload to Azure
            //    var blobClient = new BlobContainerClient(storageConnectionString, containerName);
            //    await blobClient.CreateIfNotExistsAsync();
            //    var blob = blobClient.GetBlobClient($"vendor-icons/{file.Name}");
            //    await blob.UploadAsync(stream, overwrite: true);
            //    stream.Close();
            //}

            rm.statusCode = StatusCodes.ERROR;
            rm.message = "No file uploaded.";
            rm.name = StatusName.invalid;
            rm.data = "No file uploaded.";
            return Ok(rm);
            // If folderIds exist (subfolders), you could recurse similarly...
        }

        private DriveService GetDriveService()
        {
            var credential = GoogleCredential
                .FromFile("zeta-cortex-464910-m3-75e0f14f518a.json")
                .CreateScoped(DriveService.ScopeConstants.DriveReadonly);

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveToAzureUploader"
            });
        }

        [HttpPost]
        [Route("CleanJson")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CleanJson()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                string jsonResult = "";

                jsonResult = "\"{\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/Product\\\\/10031010677024\\\",\\\"body_html\\\":\\\"\\\\u003cp\\\\u003ebckzdnjdsvjdfipvjdsvm\\\\u003c\\\\/p\\\\u003e\\\",\\\"created_at\\\":\\\"2025-08-22T06:22:43-04:00\\\",\\\"handle\\\":\\\"tripod\\\",\\\"id\\\":10031010677024,\\\"product_type\\\":\\\"\\\",\\\"published_at\\\":\\\"2025-08-22T06:22:47-04:00\\\",\\\"template_suffix\\\":\\\"\\\",\\\"title\\\":\\\"Tripod\\\",\\\"updated_at\\\":\\\"2025-08-22T06:27:38-04:00\\\",\\\"vendor\\\":\\\"Nikon\\\",\\\"status\\\":\\\"active\\\",\\\"published_scope\\\":\\\"global\\\",\\\"tags\\\":\\\"\\\",\\\"variants\\\":[{\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/ProductVariant\\\\/50616990171424\\\",\\\"barcode\\\":\\\"\\\",\\\"compare_at_price\\\":null,\\\"created_at\\\":\\\"2025-08-22T06:22:43-04:00\\\",\\\"id\\\":50616990171424,\\\"inventory_policy\\\":\\\"deny\\\",\\\"position\\\":1,\\\"price\\\":\\\"1000.00\\\",\\\"product_id\\\":10031010677024,\\\"sku\\\":null,\\\"taxable\\\":true,\\\"title\\\":\\\"Copper\\\",\\\"updated_at\\\":\\\"2025-08-22T06:27:37-04:00\\\",\\\"option1\\\":\\\"Copper\\\",\\\"option2\\\":null,\\\"option3\\\":null,\\\"image_id\\\":null,\\\"inventory_item_id\\\":52639499157792,\\\"inventory_quantity\\\":4,\\\"old_inventory_quantity\\\":4}],\\\"options\\\":[{\\\"name\\\":\\\"Material\\\",\\\"id\\\":12668338536736,\\\"product_id\\\":10031010677024,\\\"position\\\":1,\\\"values\\\":[\\\"Copper\\\"]}],\\\"images\\\":[{\\\"id\\\":50385265951008,\\\"product_id\\\":10031010677024,\\\"position\\\":1,\\\"created_at\\\":\\\"2025-08-22T06:20:14-04:00\\\",\\\"updated_at\\\":\\\"2025-08-22T06:20:15-04:00\\\",\\\"alt\\\":null,\\\"width\\\":225,\\\"height\\\":225,\\\"src\\\":\\\"https:\\\\/\\\\/cdn.shopify.com\\\\/s\\\\/files\\\\/1\\\\/0917\\\\/7323\\\\/9584\\\\/files\\\\/Vape_ed01366c-88fb-4647-87a2-bc083617f31e.jpg?v=1755858015\\\",\\\"variant_ids\\\":[],\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/ProductImage\\\\/50385265951008\\\"},{\\\"id\\\":50385265983776,\\\"product_id\\\":10031010677024,\\\"position\\\":2,\\\"created_at\\\":\\\"2025-08-22T06:20:14-04:00\\\",\\\"updated_at\\\":\\\"2025-08-22T06:20:15-04:00\\\",\\\"alt\\\":null,\\\"width\\\":1000,\\\"height\\\":1000,\\\"src\\\":\\\"https:\\\\/\\\\/cdn.shopify.com\\\\/s\\\\/files\\\\/1\\\\/0917\\\\/7323\\\\/9584\\\\/files\\\\/Waterbottle_db321721-fc1e-4d42-ace3-639c02e5ce52.jpg?v=1755858015\\\",\\\"variant_ids\\\":[],\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/ProductImage\\\\/50385265983776\\\"}],\\\"image\\\":{\\\"id\\\":50385265951008,\\\"product_id\\\":10031010677024,\\\"position\\\":1,\\\"created_at\\\":\\\"2025-08-22T06:20:14-04:00\\\",\\\"updated_at\\\":\\\"2025-08-22T06:20:15-04:00\\\",\\\"alt\\\":null,\\\"width\\\":225,\\\"height\\\":225,\\\"src\\\":\\\"https:\\\\/\\\\/cdn.shopify.com\\\\/s\\\\/files\\\\/1\\\\/0917\\\\/7323\\\\/9584\\\\/files\\\\/Vape_ed01366c-88fb-4647-87a2-bc083617f31e.jpg?v=1755858015\\\",\\\"variant_ids\\\":[],\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/ProductImage\\\\/50385265951008\\\"},\\\"media\\\":[{\\\"id\\\":41462843343136,\\\"product_id\\\":10031010677024,\\\"position\\\":1,\\\"created_at\\\":\\\"2025-08-22T06:20:14-04:00\\\",\\\"updated_at\\\":\\\"2025-08-22T06:20:15-04:00\\\",\\\"alt\\\":null,\\\"status\\\":\\\"READY\\\",\\\"media_content_type\\\":\\\"IMAGE\\\",\\\"preview_image\\\":{\\\"width\\\":225,\\\"height\\\":225,\\\"src\\\":\\\"https:\\\\/\\\\/cdn.shopify.com\\\\/s\\\\/files\\\\/1\\\\/0917\\\\/7323\\\\/9584\\\\/files\\\\/Vape_ed01366c-88fb-4647-87a2-bc083617f31e.jpg?v=1755858015\\\",\\\"status\\\":\\\"READY\\\"},\\\"variant_ids\\\":[],\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/MediaImage\\\\/41462843343136\\\"},{\\\"id\\\":41462843375904,\\\"product_id\\\":10031010677024,\\\"position\\\":2,\\\"created_at\\\":\\\"2025-08-22T06:20:14-04:00\\\",\\\"updated_at\\\":\\\"2025-08-22T06:20:15-04:00\\\",\\\"alt\\\":null,\\\"status\\\":\\\"READY\\\",\\\"media_content_type\\\":\\\"IMAGE\\\",\\\"preview_image\\\":{\\\"width\\\":1000,\\\"height\\\":1000,\\\"src\\\":\\\"https:\\\\/\\\\/cdn.shopify.com\\\\/s\\\\/files\\\\/1\\\\/0917\\\\/7323\\\\/9584\\\\/files\\\\/Waterbottle_db321721-fc1e-4d42-ace3-639c02e5ce52.jpg?v=1755858015\\\",\\\"status\\\":\\\"READY\\\"},\\\"variant_ids\\\":[],\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/MediaImage\\\\/41462843375904\\\"}],\\\"variant_gids\\\":[{\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/ProductVariant\\\\/50616990171424\\\",\\\"updated_at\\\":\\\"2025-08-22T10:27:37.000Z\\\"}],\\\"has_variants_that_requires_components\\\":false,\\\"category\\\":{\\\"admin_graphql_api_id\\\":\\\"gid:\\\\/\\\\/shopify\\\\/TaxonomyCategory\\\\/co-1-7-6\\\",\\\"name\\\":\\\"Tabletop Tripods\\\",\\\"full_name\\\":\\\"Cameras \\\\u0026 Optics \\\\u003e Camera \\\\u0026 Optic Accessories \\\\u003e Tripods \\\\u0026 Monopods \\\\u003e Tabletop Tripods\\\"}}\"";

                var parsedJson = JsonConvert.DeserializeObject(jsonResult);
                rm.statusCode = StatusCodes.OK;
                rm.message = "Delhivery Pincode has been successfully fetched";
                rm.name = StatusName.ok;
                rm.data = parsedJson;


            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            return Ok(rm);
        }
    }

}
