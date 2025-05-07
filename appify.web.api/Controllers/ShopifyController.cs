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
        /// <response code="200">SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED</response>
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
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY IMPORTED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("SHPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("SHPIFY PRODUCTS ASYNC - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Fetch A Shopify Product
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
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY ASYNCED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        //[HttpPost, Route("GetProductAsync")]
        //[MapToApiVersion("1.0")]
        //public async Task<IActionResult> GetProductAsync(ParamVendorProduct item)
        //{
        //    var reqHeader = Request;
        //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID);
        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;

        //        await Common.UpdateEventLogsNew("SHPIFY PRODUCTS ASYNC - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        //    }
        //        return View();
        //}

        /// <summary>
        /// CREATE A Shopify Product
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
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY CREATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        //[HttpPost, Route("CreateProductAsync")]
        //[MapToApiVersion("1.0")]
        //public async Task<IActionResult> CreateProductAsync()
        //{
        //    var reqHeader = Request;
        //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService();
        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;

        //        await Common.UpdateEventLogsNew("SHPIFY CREATE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        //    }
        //    return View();
        //}

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
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY UPDATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UpdateProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProductAsync(ParamVendorProduct item, string Title)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID);
                var result = await shopifyGraphQLService.UpdateProductAsync("gid://shopify/Product/9776273359136", "Zebra Print Bikini222", "ok then", "DRAFT");
                var result2 = await shopifyGraphQLService.UpdateVariantAsync("gid://shopify/ProductVariant/49684002701600", 16.99, 10, "KILOGRAMS",98);
                if (result != null && result2 != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY IMPORTED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                    await Common.UpdateEventLogsNew("SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED", reqHeader, controllerURL, null, result2, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("SHPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHPIFY UPDATE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
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
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY UPDATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UpdateProductStockAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProductStockAsync(ShopifyProductStock item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness, item.VendorID);
                var result = await shopifyGraphQLService.UpdateProductStockAsync(item);

                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY IMPORTED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("SHPIFY PRODUCTS ASYNC - NO CONTENT", reqHeader, controllerURL, null, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHPIFY UPDATE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Remove A Shopify Product
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
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY DELETED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DeleteProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteProductAsync(ParamVendorProduct item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService(this.shopifyBusiness,item.VendorID);

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHPIFY DELETE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return View();
        }
        /// <summary>
        /// RazorPay WebHook for PaymentEvents.
        /// </summary>
        /// <remarks>
        /// Sample Response:
        /// NOTE : RazorPay WebHook for PaymentEvents.
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
        /// <param name="payload"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">RECEIVED WEBHOOK - RAZORPAY RESPONSE SUCCESSFULLY</response>
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
