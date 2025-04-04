using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/Master/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class SubscriptionController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ISubscriptionBusiness subscriptionBusiness;
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;


        public SubscriptionController(IConfiguration configuration, ISubscriptionBusiness subscriptionBusiness)
        {
            this.configuration = configuration;
            this.subscriptionBusiness = subscriptionBusiness;

        }




        /// <summary>
        /// Adds a Product's Subscription.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : For a new Subscription object, send the SubscriptionID = 0.
        /// 
        ///     {
        ///       "subscriptionID": 1000,
        ///       "planName": "Basic Plan",
        ///       "planDescription": "Entry-level subscription",
        ///       "appliedCommission": 6.99,
        ///       "warehouseCount": 1,
        ///       "userAccountCount": 1,
        ///       "hasEcommerceIntegration": false,
        ///       "ecommercePlatforms": null,
        ///       "hasBulkUpload": true,
        ///       "hasProductCatalog": true,
        ///       "hasInvoice": true,
        ///       "hasSMSService": true,
        ///       "discountCouponCount": 0,
        ///       "hasAnalytics": true,
        ///       "hasStoreLocation": false,
        ///       "isWhiteLabeled": false,
        ///       "hasAccountManager": false,
        ///       "imageEnhancerCount": 25,
        ///       "productListingCount": 25,
        ///       "productCategoryCount": 1,
        ///       "bannerCount": 3,
        ///       "monthlyFee": 499,
        ///       "halfYearlyFee": 1799,
        ///       "annualFee": 2999,
        ///       "isActive": true,
        ///       "createdOn": "2025-03-26T10:38:45.56",
        ///       "modifiedOn": "0001-01-01T00:00:00",
        ///       "features": "Basic Theme",
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Subscription Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("Save")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SubscriptionAdd(Subscription item)
        {
            var result = true;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                List<Subscription> returnItem = new List<Subscription>();

                rm = new ResponseMessage();

               
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SUBSCRIPTION SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = returnItem;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }

            return Ok(rm);
        }

        /// <summary>
        /// removes Product's Subscription Item
        /// </summary>
        /// <remarks>
        /// Sample Data :
        /// 
        ///     {
        ///         "SubscriptionID":1015,
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Boolean Value </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("Remove")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionRemove(ParamSubscription itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionBusiness.Delete(itemData.SubscriptionID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SUBSCRIPTION REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }

            return Ok(rm);
        }



        /// <summary>
        /// gets Product's Subscription Item
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "SubscriptionID":1000
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "subscriptionID": 1000,
        ///       "planName": "Basic Plan",
        ///       "planDescription": "Entry-level subscription",
        ///       "appliedCommission": 6.99,
        ///       "warehouseCount": 1,
        ///       "userAccountCount": 1,
        ///       "hasEcommerceIntegration": false,
        ///       "ecommercePlatforms": null,
        ///       "hasBulkUpload": true,
        ///       "hasProductCatalog": true,
        ///       "hasInvoice": true,
        ///       "hasSMSService": true,
        ///       "discountCouponCount": 0,
        ///       "hasAnalytics": true,
        ///       "hasStoreLocation": false,
        ///       "isWhiteLabeled": false,
        ///       "hasAccountManager": false,
        ///       "imageEnhancerCount": 25,
        ///       "productListingCount": 25,
        ///       "productCategoryCount": 1,
        ///       "bannerCount": 3,
        ///       "monthlyFee": 499,
        ///       "halfYearlyFee": 1799,
        ///       "annualFee": 2999,
        ///       "isActive": true,
        ///       "createdOn": "2025-03-26T10:38:45.56",
        ///       "modifiedOn": "0001-01-01T00:00:00",
        ///       "features": "Basic Theme",
        ///       "marketingTools": "Push Notifications, WhatsApp Marketing",
        ///       "supportTypes": "Email Support",
        ///       "paymentGateways": "Appify Partnered",
        ///       "deliveryPartners": "Appify Partnered"
        ///     }
        /// </remarks>
        /// <param name="SubscriptionID"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Subscription Object </response>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code="500">ResponseMessage with Error Description</response> 



        [HttpPost, Route("Get")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionGet(ParamSubscription itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionBusiness.Get(itemData.SubscriptionID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTION ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }

            return Ok(rm);
        }

        /// <summary>
        /// gets Product's Subscription Items LIST (ONLY FOR TEST PURPOSE, NOT IMPLEMENTED IN THE APPS)
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        /// Sample response JSON :
        ///
        ///     
        ///     [{
        ///       "subscriptionID": 1000,
        ///       "planName": "Basic Plan",
        ///       "planDescription": "Entry-level subscription",
        ///       "appliedCommission": 6.99,
        ///       "warehouseCount": 1,
        ///       "userAccountCount": 1,
        ///       "hasEcommerceIntegration": false,
        ///       "ecommercePlatforms": null,
        ///       "hasBulkUpload": true,
        ///       "hasProductCatalog": true,
        ///       "hasInvoice": true,
        ///       "hasSMSService": true,
        ///       "discountCouponCount": 0,
        ///       "hasAnalytics": true,
        ///       "hasStoreLocation": false,
        ///       "isWhiteLabeled": false,
        ///       "hasAccountManager": false,
        ///       "imageEnhancerCount": 25,
        ///       "productListingCount": 25,
        ///       "productCategoryCount": 1,
        ///       "bannerCount": 3,
        ///       "monthlyFee": 499,
        ///       "halfYearlyFee": 1799,
        ///       "annualFee": 2999,
        ///       "isActive": true,
        ///       "createdOn": "2025-03-26T10:38:45.56",
        ///       "modifiedOn": "0001-01-01T00:00:00",
        ///       "features": "Basic Theme",
        ///       "marketingTools": "Push Notifications, WhatsApp Marketing",
        ///       "supportTypes": "Email Support",
        ///       "paymentGateways": "Appify Partnered",
        ///       "deliveryPartners": "Appify Partnered"
        ///     }]
        ///     
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Subscription Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("List")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionList()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionBusiness.List();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTION ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }

            return Ok(rm);
        }




    }
}
