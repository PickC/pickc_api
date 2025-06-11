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
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
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

                productData.ProductId = "gid://shopify/Product/9963806064928";
                productData.Title = "Skeleton Black Hoody" + DateTime.Now.Microsecond.ToString();
                productData.Description = "Lovely black zip-up black hoodie with all over skeleton print." + DateTime.Now.Microsecond.ToString();
                productData.Vendor = "Docblack-" + DateTime.Now.Microsecond.ToString();
                productData.ProductType = "Accessories";
                productData.Status = "Active";

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

                await Common.UpdateEventLogsNew("SHOPIFY UPDATE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Update The Shopify Product's Inventory
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT'S INVENTORY HAVE BEEN SUCCESSFULLY UPDATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UpdateProductInvantoryAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProductInvantoryAsync(string InventoryItemID, int QuantityPurchased, ParamVendorProduct item)
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

                InventoryItemID = "gid://shopify/InventoryItem/52283253391648";
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID);

                var result = await shopifyGraphQLService.UpdateShopifyInventoryAsync(52283253391648, QuantityPurchased);
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
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("SHOPIFY PRODUCT'S INVENTORY - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHPIFY PRODUCT'S INVENTORY - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Upload Shopify Product Image
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY UPLOADED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UploadProductImageAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UploadProductImageAsync(ParamVendorRef item, [Required][FromForm] long ProductID, IFormFile file)
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

                if (file == null || file.Length == 0)
                {
                    rm.statusCode = api.StatusCodes.ERROR;
                    rm.message = "No file uploaded";
                    rm.name = StatusName.ok;
                    rm.data = "No file uploaded";

                    return Ok(rm);
                }

                if (!Path.GetExtension(file.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    rm.statusCode = api.StatusCodes.ERROR;
                    rm.message = "only .jpg files are allowed";
                    rm.name = StatusName.ok;
                    rm.data = "only .jpg files are allowed";
                    return Ok(rm);
                }

                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID, item.ReferenceID);
                var result = await shopifyGraphQLService.UploadImageToShopifyAsync(file, ProductID);
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
            return View();
        }

        /// <summary>
        /// Delete Shopify Product's Image
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT'S IMAGE HAS BEEN SUCCESSFULLY DELETED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DeleteProductImageAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteProductImageAsync(ParamVendorRef item, string ImageID, string ProductID)
        {
            /*
                Files - Permissions
                View or manage files
                write_files
                read_files

             */
            ImageID = "gid://shopify/ProductImage/49587254624544";
            ProductID = "gid://shopify/Product/9776279585056";
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID, item.ReferenceID);
                var result =  await shopifyGraphQLService.DeleteProductImageAsync(9776279585056, 49587254624544);
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
            return View();
        }

        /// <summary>
        /// Delete A Shopify Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHOPIFY PRODUCT HAS BEEN SUCCESSFULLY DELETED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DeleteProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteProductAsync(ShopifyProductStock item, [Required] string ProductID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ProductID = "gid://shopify/Product/9776279585056";
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID);
                var result = await shopifyGraphQLService.DeleteProductAsync(ProductID);

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
            var body = "";
            string paymentType = "";
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            bool eventResult = false;
            string[] eventSearch ={
              "downtime",
              ////"payment_link",
              "notification",
              "authorized",
              "order.paid"
            };

            try
            {
                // Verify the X-VERIFY header.
                string xVerifyHeader = reqHeader.Headers["X-Razorpay-Signature"];
                                                                                 
                if (xVerifyHeader == null)//// || !VerifyXVerifyHeadeRazorpay(xVerifyHeader)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "Invalid payload";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("SHOPIFY WEBHOOK RECEIVED NULL PAYLOAD", reqHeader, controllerURL, "SHOPIFY WEBHOOK RECEIVED NULL PAYLOAD-SHOpIFY-Signature-" + xVerifyHeader, "SHOPIFY WEBHOOK RECEIVED NULL PAYLOAD", StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    using var reader = new StreamReader(HttpContext.Request.Body);
                    body = await reader.ReadToEndAsync();

                    //var request = JsonConvert.DeserializeObject<JObject>(body.Replace("Response: ", ""));
                    //string eventname = System.String.IsNullOrEmpty((string?)request["event"]) ? "" : Convert.ToString(request["event"]);
                    //foreach (var s in eventSearch)
                    //{
                    //    eventResult = eventname.Contains(s);
                    //    if (eventResult == true)
                    //        break;
                    //}
                    //if (eventResult == false)
                    //{
                    //    paymentType = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["notes"]["paymentType"]) ? "" : Convert.ToString(request["payload"]["payment"]["entity"]["notes"]["paymentType"]);

                    //    if (paymentType == "orderPayment")
                    //    {
                    //        long ts = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["created_at"]) ? 0 : Convert.ToInt64(request["payload"]["payment"]["entity"]["created_at"]);

                    //        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts).ToLocalTime();
                    //        OrderPayment orderPayment = new OrderPayment
                    //        {
                    //            PaymentID = 0,
                    //            PaymentDate = dt,
                    //            OrderID = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["notes"]["orderId"]) ? 0 : Convert.ToInt64(request["payload"]["payment"]["entity"]["notes"]["orderId"]),
                    //            EventName = Convert.ToString(request["event"]),
                    //            PaymentAmount = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["amount"]) ? 0 : Convert.ToDecimal(request["payload"]["payment"]["entity"]["amount"]) / 100,
                    //            OrderReferenceNo = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["order_id"]) ? "" : Convert.ToString(request["payload"]["payment"]["entity"]["order_id"]),
                    //            PaymentReferenceNo = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["id"]) ? "" : Convert.ToString(request["payload"]["payment"]["entity"]["id"]),
                    //            PaymentMode = 0,
                    //            LookupCode = "RAZORPAY"
                    //        };
                            //var result = orderBusiness.OrderPaymentSave(orderPayment);
                            //if (result)
                           // {
                                rm.statusCode = StatusCodes.OK;
                                rm.message = "SHOPIFY WEBHOOK - SHOPIFY RESPONSE SUCCESSFULLY";
                                rm.name = StatusName.ok;
                                rm.data = body;
                                await Common.UpdateEventLogsNew("SHOPIFY WEBHOOK - SHOPIFY RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "SHOPIFY WEBHOOK - Success Response", body, StatusName.ok, this.eventLogBusiness);
                            //}
                    //    }
                    //    else if (paymentType == "oneTimeSubscription")
                    //    {

                    //    }
                    //}

                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("SHOPIFY WEBHOOK Error Response", reqHeader, controllerURL, "SHOPIFY WEBHOOK Error Response->" + body, null, rm.message, this.eventLogBusiness);
            }

            return Ok(rm);
        }
    }
}
