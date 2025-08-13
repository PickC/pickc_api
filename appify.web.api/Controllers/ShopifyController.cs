/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.0
 * Date: 2025-04-29
 * Description:
*/
using appify.Business;
using appify.Business.Contract;
using Asp.Versioning;
using appify.utility;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using static appify.web.api.ParamVendorProduct;
using appify.models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using NPOI.HPSF;
using System.ComponentModel.DataAnnotations;
using NPOI.Util;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using Org.BouncyCastle.Utilities.Zlib;
using System.Security.Policy;
using System.Text;
using Twilio.Rest;
using Twilio.TwiML.Voice;
using Twilio.TwiML.Messaging;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Org.BouncyCastle.Ocsp;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]

    public class ShopifyController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private ResponseMessage rm;
        private readonly IShopifyBusiness shopifyBusiness;
        public ShopifyController(IConfiguration configuration, IEventLogBusiness eventLogBusiness, IShopifyBusiness shopifyBusiness)
        {
            this.configuration = configuration;
            this.eventLogBusiness = eventLogBusiness;
            this.shopifyBusiness = shopifyBusiness;
        }

        /// <summary>
        /// Fetch A Shopify Product List
        /// </summary>
        /// <remarks>  
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY IMPORTED!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY IMPORTED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("GetAllProductsAsync")]
        [MapToApiVersion("1.0")]
        public async Task <IActionResult> GetAllProductsAsync(ParamVendorRef item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID, item.ReferenceID);
                if(shopifyGraphQLService.IsFound==false)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY STORE NOT FOUND BY VENDOR";
                    rm.name = StatusName.ok;
                    rm.data = "SHOPIFY STORE NOT FOUND BY VENDOR";
                    return Ok(rm);
                }
                var result = await shopifyGraphQLService.FetchAllProductsAsync();
                //var jsonString = JsonConvert.SerializeObject(result, Formatting.Indented);
                //var bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                //var fileName = "products.json";

                //return File(bytes, "application/json", fileName);
                if(result!=null)
                {
                    var products = shopifyBusiness.SaveShopifyProductToAppify(item.VendorID);
                    if (products != null)
                    {

                        rm.statusCode = StatusCodes.OK;
                        rm.message = "SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY IMPORTED!";
                        rm.name = StatusName.ok;
                        rm.data = result;
                        await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED", reqHeader, controllerURL, null, products, StatusName.ok, this.eventLogBusiness);
                    }
                }

                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS ASYNC - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Update A Shopify Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "productId": "gid://shopify/Product/9899667226900",
        ///       "title": "women solid sleeveless tiered dress",
        ///       "description": "Occasion : Casual wear color : Pink print and pattern : Solids length : Knee Length type : Fit and flare,tiered dress neck type : key hole fit : Regular fit sleeve type : Sleeveless material : Polyester",
        ///       "vendor": "Saurabh wallpapers",
        ///       "productType": "Accessories",
        ///       "status": "Active",
        ///       "variants": [
        ///         {
        ///           "variantId": "gid://shopify/ProductVariant/51034392953108",
        ///           "sku": "DB1-BLK-O",
        ///           "price": 1349.00,
        ///           "inventory": 12,
        ///           "inventoryItemID": "gid://shopify/InventoryItem/52996731732244",
        ///           "quantityPurchased": 1,
        ///           "weight": 123,
        ///           "weightUnit": "GRAMS"
        ///         }
        ///       ]
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY UPDATED!",
        ///       "data": "Product updated successfully."
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY UPDATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UpdateProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProductAsync(long VendorID, ProductUpdateRequest productData)
        {
            /*
             * 
                    Inventory - Permissions
                    View or manage inventory across multiple locations
                    write_inventory
                    read_inventory
             * 
             */
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, VendorID);
                var result = await shopifyGraphQLService.UpdateShopifyProductAsync(productData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY UPDATED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = "NO CONTENT";
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();

                await Common.UpdateEventLogsNew("SHOPIFY UPDATE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Update The Shopify Product's Inventory
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "vendorID": 1060,
        ///       "inventoryItemID": "gid://shopify/InventoryItem/52996731732244",
        ///       "quantityPurchased": 1
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SHOPIFY PRODUCT'S INVENTORY HAVE BEEN SUCCESSFULLY UPDATED!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT'S INVENTORY HAVE BEEN SUCCESSFULLY UPDATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UpdateProductInvantoryAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProductInvantoryAsync(ParamShopifyInventory item)
        {
            /*
             * 
                    Inventory - Permissions
                    View or manage inventory across multiple locations
                    write_inventory
                    read_inventory
             * 
             */
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID);
                //"52283253391648"//long.Parse(InventoryItemID)
                var result = await shopifyGraphQLService.UpdateShopifyInventoryAsync(item.InventoryItemID, item.QuantityPurchased);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY PRODUCT'S INVENTORY HAVE BEEN SUCCESSFULLY UPDATED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT'S INVENTORY HAVE BEEN SUCCESSFULLY UPDATED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = "NO CONTENT";
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT'S INVENTORY - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();

                await Common.UpdateEventLogsNew("SHPIFY PRODUCT'S INVENTORY - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Upload Shopify Product Image
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY UPLOADED!",
        ///       "data": "https://cdn.shopify.com/s/files/1/0942/7219/2788/files/tshirts_b6932816-2a7a-4f20-93e8-fc883ee03f99.jpeg?v=1751510835"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY UPLOADED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UploadProductImageAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UploadProductImageAsync([Required][FromForm] ParamVendorUploadImg item)
        {
            /*
                Files - Permissions
                View or manage files
                write_files
                read_files

             */
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                if (item.file == null || item.file.Length == 0)
                {
                    rm.statusCode = api.StatusCodes.ERROR;
                    rm.message = "No file uploaded";
                    rm.name = StatusName.ok;
                    rm.data = "No file uploaded";

                    return Ok(rm);
                }
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" , ".webp"};

                if (!allowedExtensions.Contains(Path.GetExtension(item.file.FileName).ToLowerInvariant()))
                {
                    //if (!Path.GetExtension(item.file.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                    rm.statusCode = api.StatusCodes.ERROR;
                    rm.message = "only .jpg, .jpeg, .png, .svg, .webp files are allowed";
                    rm.name = StatusName.ok;
                    rm.data = "only .jpg, .jpeg, .png, .svg, .webp files are allowed";
                    return Ok(rm);
                }

                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID, item.ReferenceID);
                var result = await shopifyGraphQLService.UploadImageToShopifyAsync(item.file, item.ProductID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY UPLOADED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY UPLOADED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT'S IMAGE - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();

                await Common.UpdateEventLogsNew("SHOPIFY CREATE IMAGE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Delete Shopify Product's Image
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "vendorID": 1060,
        ///       "referenceID": 0,
        ///       "imageID": "gid://shopify/ProductImage/52397812580628",
        ///       "productID": "gid://shopify/Product/9899667226900"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY DELETED!",
        ///       "data": "Image has been successfully removed!"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY DELETED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DeleteProductImageAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteProductImageAsync(ParamVendorDeleteImg item)
        {
            /*
                Files - Permissions
                View or manage files
                write_files
                read_files

             */
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID, item.ReferenceID);
                var result =  await shopifyGraphQLService.DeleteProductImageAsync(item.ProductID, item.ImageID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY DELETED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY DELETED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT'S IMAGE - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();

                await Common.UpdateEventLogsNew("SHOPIFY CREATE IMAGE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Delete A Shopify Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///{
        ///  "vendorID": 1060,
        ///  "productID": "gid://shopify/Product/9899667226900"
        ///}
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY DELETED!",
        ///       "data": "Product has been successfully removed!"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY DELETED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DeleteProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteProductAsync([Required] ParamVendorProduct item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID);
                var result = await shopifyGraphQLService.DeleteProductAsync(item.ProductID);

                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY DELETED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY DELETED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHOPIFY DELETE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        private bool VerifyWebhookRequestAsync(HttpRequest request, string shopifySecret)
        {
            using var reader = new StreamReader(request.Body);
            var requestBody = reader.ReadToEndAsync().Result;

            // Get the Shopify HMAC header
            var shopifyHmac = request.Headers["X-Shopify-Hmac-Sha256"].ToString();

            // Convert secret to bytes
            var secretBytes = Encoding.UTF8.GetBytes(shopifySecret);

            // Compute the hash of the body
            using var hmacSha256 = new HMACSHA256(secretBytes);
            var hashBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(requestBody));

            // Convert hash to base64 string
            var calculatedHmac = Convert.ToBase64String(hashBytes);

            // OPTIONAL: Log both for debugging
            Console.WriteLine($"Key - {shopifyHmac} -- calculatedHmac: {calculatedHmac}");

            return shopifyHmac.Equals(calculatedHmac, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Shopify WebHook for PaymentEvents.
        /// </summary>
        /// <remarks>
        /// Sample Response:
        /// NOTE : Shopify WebHook for PaymentEvents.
        /// 
        ///     {
        ///         "event": "payment.captured",
        ///         "payload": {
        ///             "payment": {
        ///                 "entity": {
        ///                     "id": "pay_29QQoUBi66xm2f",
        ///                     "amount": 5000,
        ///                     "currency": "INR",
        ///                     "status": "captured",
        ///                     "order_id": "order_DBJOWzybf0sJbb"
        ///                 }
        ///             }
        ///         }
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">RECEIVED WEBHOOK - SHOPIFY RESPONSE SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response>

        [HttpPost]
        [Route("ShopifyWebhook")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> WebhookPaymentEvents()
        {

            /*
             * 
             * Shopify - Headers (For Reference)
             * 
                headers - Accept:
                Accept - Encoding: gzip; q = 1.0,deflate; q = 0.6,identity; q = 0.3--
                Content - Length: 4968--
                Content - Type: application / json--
                Host: appifyapi.azurewebsites.net--
                Max - Forwards: 10--
                User - Agent: Shopify - Captain - Hook--
                X - Shopify - Api - Version: 2025 - 01--
                X - Shopify - Event - Id: e04cd3d0 - b977 - 41b5 - b855 - 8d182178d8b1--
                X - Shopify - Hmac - Sha256: z / 8HXPYxM4qKmI4bot5q1BX5s5u / MBaLrgIesfvbgNw = --
                X - Shopify - Product - Id: 9903995289876--
                X - Shopify - Shop - Domain: d9nnfq - s0.myshopify.com--
                X - Shopify - Topic: products / update--
                X - Shopify - Triggered - At: 2025 - 07 - 08T12: 41:31.821061678Z--
                X - Shopify - Webhook - Id: e5877a5e - f3cf - 40a5 - b4da - 5f5d493eafb5--
                X - ARR - LOG - ID: 1bdbfe00 - 9171 - 4cac - b2f7 - f8be97f45047--
                CLIENT - IP: 34.139.82.252:55432--
                DISGUISED - HOST: appifyapi.azurewebsites.net--
                X - SITE - DEPLOYMENT - ID: appifyapi--
                WAS - DEFAULT - HOSTNAME: appifyapi.azurewebsites.net--
                X - Forwarded - Proto: https--
                X - AppService - Proto: https--
                X - ARR - SSL: 2048 | 256 | CN = Microsoft Azure RSA TLS Issuing CA 07, O = Microsoft Corporation, C = US | CN = *.azurewebsites.net, O = Microsoft Corporation, L = Redmond, S = WA, C = US-- X - Forwarded - TlsVersion: 1.3--
                X - Forwarded - For: 34.139.82.252:55432-- X - Original - URL: //api/Shopify/ShopifyWebhook -- X-WAWS-Unencoded-URL: //api/Shopify/ShopifyWebhook --
                ***/

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            var body = "";
            try
            {
                string topic = Request.Headers["X-Shopify-Topic"];
                string shopDomain = Request.Headers["X-Shopify-Shop-Domain"];
                string xVerifyHeader = Request.Headers["X-Shopify-Hmac-Sha256"];
//#if DEBUG
//                topic = "products/create";
//                shopDomain = "dsgclothes.myshopify.com";
//                xVerifyHeader = "Sl7sUmG3db0RePvoIg37dtYrYBclxttx3Ce/qAGzsmI=";
//#endif
                ShopifyConfigLite shopifyConfig;
                string inputJson = "Topic - " + topic + " - Domain - " + shopDomain + " - Hmac - "+ xVerifyHeader;
                if (shopDomain != null)
                {
                    shopifyConfig = shopifyBusiness.GetShopifyConfigByStoreUrl(shopDomain);
                    ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, shopifyConfig.VendorID);
                    //var isValid = VerifyWebhookRequestAsync(reqHeader, shopifyConfig.SecretKey);
                    using var reader = new StreamReader(HttpContext.Request.Body);
                    body = await reader.ReadToEndAsync();
                    // Verify the X-Shopify-Hmac header.
                    if (xVerifyHeader == null || xVerifyHeader == "")
                     {
                        rm.statusCode = StatusCodes.ERROR;
                        rm.message = "Invalid webhook signature.";
                        rm.name = StatusName.invalid;
                        rm.data = null;
                        await Common.UpdateEventLogsNew("SHOPIFY WEBHOOK RECEIVED NULL SIGNATURE", reqHeader, controllerURL, "SHOPIFY Webhook Received Null Payload", body, StatusName.invalid, this.eventLogBusiness);
                    }
                    else
                    {
                        switch (topic)
                        {
                            case "products/create":
                                var result = shopifyGraphQLService.ShopifyProductCreateAsync(body);
                                if (result != null)
                                {
                                    //var products = shopifyBusiness.SaveShopifyProductToAppify(shopifyConfig.VendorID);
                                    //if (products != null)
                                    //{

                                        rm.statusCode = StatusCodes.OK;
                                        rm.message = "SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY CREATED!";
                                        rm.name = StatusName.ok;
                                        rm.data = result;
                                        await Common.UpdateEventLogsNew("SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY CREATED", reqHeader, controllerURL, inputJson, body, StatusName.ok, this.eventLogBusiness);
                                    //}
                                }

                                else
                                {
                                    rm.statusCode = StatusCodes.ERROR;
                                    rm.message = "NO CONTENT";
                                    rm.name = StatusName.invalid;
                                    rm.data = null;
                                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT CREATE ASYNC - NO CONTENT", reqHeader, controllerURL, inputJson, body, rm.message, this.eventLogBusiness);
                                }
                                break;

                            case "products/update":
                                var resultObj = shopifyGraphQLService.ShopifyProductUpdateAsync(body);
                                if (resultObj != null)
                                {

                                        rm.statusCode = StatusCodes.OK;
                                        rm.message = "SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY UPDATED!";
                                        rm.name = StatusName.ok;
                                        rm.data = resultObj;
                                        await Common.UpdateEventLogsNew("SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY UPDATED", reqHeader, controllerURL, inputJson, body, StatusName.ok, this.eventLogBusiness);
                                }

                                else
                                {
                                    rm.statusCode = StatusCodes.ERROR;
                                    rm.message = "NO CONTENT";
                                    rm.name = StatusName.invalid;
                                    rm.data = null;
                                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT UPDATE ASYNC - NO CONTENT", reqHeader, controllerURL, inputJson, body, rm.message, this.eventLogBusiness);
                                }
                                break;

                            case "products/delete":
                                var resultObjDel = shopifyGraphQLService.ShopifyProductDeleteAsync(body, shopifyConfig.VendorID);
                                if (resultObjDel != null)
                                {
                                    rm.statusCode = StatusCodes.OK;
                                    rm.message = "SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY DELETED!";
                                    rm.name = StatusName.ok;
                                    rm.data = resultObjDel;
                                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS HAS BEEN SUCCESSFULLY DELETED", reqHeader, controllerURL, inputJson, body, StatusName.ok, this.eventLogBusiness);
                                }

                                else
                                {
                                    rm.statusCode = StatusCodes.ERROR;
                                    rm.message = "NO CONTENT";
                                    rm.name = StatusName.invalid;
                                    rm.data = null;
                                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT DELETE ASYNC - NO CONTENT", reqHeader, controllerURL, inputJson, body, rm.message, this.eventLogBusiness);
                                }
                                break;

                            default:
                                await Common.UpdateEventLogsNew($"Unhandled Shopify webhook topic: {topic}", reqHeader, controllerURL, inputJson, body, rm.message, this.eventLogBusiness);
                                break;
                        }

                    }
                }
                                                                                 

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("SHOPIFY WEBHOOK Error Response", reqHeader, controllerURL, "SHOPIFY WEBHOOK Error Response->" + "body", null, rm.message, this.eventLogBusiness);
            }

            return Ok(rm);
        }

        /// <summary>
        /// Get The Shopify Product SyncHistory
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "vendorID": 2391
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY FETCHED SYNCED HISTORY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("ShopifySyncHistory")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ShopifySyncHistory([Required] ParamVendor item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = shopifyBusiness.GetShopifySyncHistory(item.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY DELETED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY DELETED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = StatusCodes.ERROR;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHOPIFY DELETE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        [HttpPost, Route("ShopifyProductUpdateToStore")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ShopifyProductUpdateToStore([Required] long ProductID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = shopifyBusiness.IsShopifyProduct(ProductID);
                var result2 = shopifyBusiness.ShopifyProductUpdateToStore(ProductID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY DELETED!";
                    rm.name = StatusName.ok;
                    rm.data = result2;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS HAVE BEEN SUCCESSFULLY DELETED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = StatusCodes.ERROR;
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHOPIFY DELETE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

    }

}
