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
        /// NOTE : For a new Product Subscription object, send the SubscriptionID = 0.
        /// 
        ///     [{
        ///         "SubscriptionID": 0,
        ///         "ProductID": 1505,
        ///         "SubscriptionType": 3001,
        ///         "SubscriptionValue": 0.33,
        ///         "EffectiveDate": "2024-04-11T15:55:06.807",
        ///         "ExpiryDate": "2024-04-18T15:55:06.807",
        ///         "IsCancel": false,
        ///         "CreatedBy": 1505,
        ///         "CreatedOn": "2024-04-11T18:20:59.953",
        ///         "ModifiedBy": 1505,
        ///         "ModifiedOn": "2024-04-11T18:21:53.250"
        ///     }]
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
        ///         "ProductID":1904
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
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTION ITEM!",
        ///       "data": {
        ///         "subscriptionID": 1001,
        ///         "productID": 2334,
        ///         "discountType": 3000,
        ///         "discountValue": 1000,
        ///         "effectiveDate": "2024-04-25T15:55:06.807",
        ///         "expiryDate": "2024-04-30T15:55:06.807",
        ///         "isCancel": false,
        ///         "createdBy": 1505,
        ///         "createdOn": "2024-04-29T15:55:06.807",
        ///         "modifiedBy": 0,
        ///         "modifiedOn": "0001-01-01T00:00:00",
        ///         "isActive": true
        ///       }
        ///     }
        /// 
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
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTION ITEM!",
        ///       "data": [
        ///         {
        ///           "subscriptionID": 1001,
        ///           "productID": 2334,
        ///           "discountType": 3000,
        ///           "discountValue": 1000,
        ///           "effectiveDate": "2024-04-25T15:55:06.807",
        ///           "expiryDate": "2024-04-30T15:55:06.807",
        ///           "isCancel": false,
        ///           "createdBy": 1505,
        ///           "createdOn": "2024-04-29T15:55:06.807",
        ///           "modifiedBy": 0,
        ///           "modifiedOn": "0001-01-01T00:00:00",
        ///           "isActive": true
        ///         }
        ///       ]
        ///     }
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
