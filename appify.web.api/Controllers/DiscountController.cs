using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]

    public class DiscountController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration _configuration;
        private readonly IDiscountHeaderBusiness discountHeaderBusiness;
        private readonly IDiscountDetailBusiness discountDetailBusiness;
        private ResponseMessage rm;
        public DiscountController(IConfiguration configuration, IDiscountHeaderBusiness discountHeaderBusiness, IDiscountDetailBusiness discountDetailBusiness, IEventLogBusiness eventLogBusiness)
        {
            this._configuration = configuration;
            this.discountHeaderBusiness = discountHeaderBusiness;
            this.discountDetailBusiness = discountDetailBusiness;
            this.eventLogBusiness = eventLogBusiness;
        }


        /// <summary>
        /// Adds a Product's Discount.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : For a new Product Discount object, send the DiscountID = 0.
        /// 
        ///     [{
        ///         "DiscountID": 0,
        ///         "ProductID": 1505,
        ///         "DiscountType": 3001,
        ///         "DiscountValue": 0.33,
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
        /// <param name="discountHeader"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("Save")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> discountHeaderAdd(List<DiscountHeader> discountHeader)
        {
            var result = true;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                List<DiscountHeader> returnItem = new List<DiscountHeader>();

                rm = new ResponseMessage();

                foreach(var item in discountHeader)
                {
                    returnItem.Add(this.discountHeaderBusiness.Save(item));
                }
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = returnItem;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, discountHeader, returnItem, StatusName.ok));
                    await Common.UpdateEventLogsNew("DISCOUNT SAVED SUCCESSFULLY", reqHeader, controllerURL, discountHeader, returnItem, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, discountHeader, null, rm.message));
                    await Common.UpdateEventLogsNew("DISCOUNT SAVE - NO CONTENT", reqHeader, controllerURL, discountHeader, returnItem, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, discountHeader, null, rm.message));
                await Common.UpdateEventLogsNew("DISCOUNT SAVE - ERROR", reqHeader, controllerURL, discountHeader, null, rm.message, this.eventLogBusiness);
            }

            return Ok(rm);
        }

        /// <summary>
        /// removes Product's Discount Item
        /// </summary>
        /// <remarks>
        /// Sample Data :
        /// 
        ///     {
        ///         "DiscountID":1015,
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
        public IActionResult discountHeaderRemove(ParamDiscountDetail itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.Remove(itemData.DiscountID, itemData.productID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }



        /// <summary>
        /// gets Product's Discount Item
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "DiscountID":1000
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DISCOUNT ITEM!",
        ///       "data": {
        ///         "discountID": 1001,
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
        /// <param name="DiscountID"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code="500">ResponseMessage with Error Description</response> 



        [HttpPost, Route("Get")]
        [MapToApiVersion("1.0")]
        public IActionResult discountHeaderGet(ParamDiscount itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.Get(itemData.DiscountID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// gets Product's Discount Items LIST (ONLY FOR TEST PURPOSE, NOT IMPLEMENTED IN THE APPS)
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DISCOUNT ITEM!",
        ///       "data": [
        ///         {
        ///           "discountID": 1001,
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
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("List")]
        [MapToApiVersion("1.0")]
        public IActionResult discountHeaderList()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetAll();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEM - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEM - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }



        /// <summary>
        /// gets Product's Discount items by vendor
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "VendorID":1839
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DISCOUNT ITEM!",
        ///       "data": [
        ///         {
        ///           "discountID": 1001,
        ///           "productID": 2334,
        ///           "productName": "URBAN TRIBE Polyester Plank 23L Gym Bag for Mens and Womens",
        ///           "description": "Easy To OpenCarry: U-shape main zippered compartment for easy packing and viewing.",
        ///           "brand": "Urban",
        ///           "category": 3713,
        ///           "effectiveDate": "2024-04-25T15:55:06.807",
        ///           "expiryDate": "2024-04-30T15:55:06.807",
        ///           "price": 1200,
        ///           "discountType": 3000,
        ///           "discountValue": 1000,
        ///           "discountTypeDescription": "AMOUNT",
        ///           "isActive": true,
        ///           "isNew": false
        ///           "imageID": 2552,
        ///           "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1713267250886.jpg"
        ///         }
        ///       ]
        ///     }
        ///     
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Discounted Products List Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("listbyvendor")]
        [MapToApiVersion("1.0")]
        public IActionResult ListByVendor(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.ListByVendor(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT LIST BY VENDOR SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT LIST BY VENDOR - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT LIST BY VENDOR - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }




        /// <summary>
        /// gets Product's Discount items by vendor
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "productID":2334
        ///     }
        /// 
        /// Sample response JSON :
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH PRODUCT DISCOUNTS",
        ///       "data": [
        ///         {
        ///           "productID": 2334,
        ///           "productName": "URBAN TRIBE Polyester Plank 23L Gym Bag for Mens and Womens",
        ///           "description": "Easy To Open Carry: U-shape main zippered compartment for easy packing and viewing.",
        ///           "brand": "Urban",
        ///           "effectiveDate": "2024-04-25T15:55:06.807",
        ///           "expiryDate": "2024-04-30T15:55:06.807",
        ///           "price": 1200,
        ///           "discountType": 3000,
        ///           "discountValue": 1000,
        ///           "discountTypeDescription": "AMOUNT"
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Discounted Products List Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("listbyproduct")]
        [MapToApiVersion("1.0")]
        public IActionResult ListByProduct(ParamProduct itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.ListByProduct(itemData.productID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT DISCOUNTS";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT DISCOUNTS SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT DISCOUNTS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT DISCOUNTS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Adds a Order's Discount.
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request:
        /// NOTE : For a new Order Discount object, send the DiscountID = 0.
        /// 
        ///     [
        ///         {
        ///           "discountID": 0,
        ///           "vendorID":1060,
        ///           "uom": 3501,
        ///           "qty": 2,
        ///           "effectiveDate": "2024-11-11T10:16:25.323Z",
        ///           "expiryDate": "2024-11-30T10:16:25.323Z",
        ///           "discountType": 3000,
        ///           "discountAmount": 500,
        ///           "isActive": true,
        ///           "createdBy": 1060,
        ///           "modifiedBy": 1060
        ///         }
        ///     ]
        /// 
        /// 
        /// </remarks>
        /// <param name="orderDiscount"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DiscountSave")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> orderDiscountAdd(List<OrderDiscount> orderDiscount)
        {
            var result = true;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                List<OrderDiscount> returnItem = new List<OrderDiscount>();

                rm = new ResponseMessage();

                foreach (var item in orderDiscount)
                {
                    returnItem.Add(this.discountHeaderBusiness.DiscountSave(item));
                }
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = returnItem;
                    await Common.UpdateEventLogsNew("DISCOUNT SAVED SUCCESSFULLY", reqHeader, controllerURL, orderDiscount, returnItem, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("DISCOUNT SAVE - NO CONTENT", reqHeader, controllerURL, orderDiscount, returnItem, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("DISCOUNT SAVE - ERROR", reqHeader, controllerURL, orderDiscount, null, rm.message, this.eventLogBusiness);
            }

            return Ok(rm);
        }

        [HttpPost, Route("DiscountSave")]
        [MapToApiVersion("1.1")]
        public async Task<IActionResult> orderDiscountAdd1(List<OrderDiscount> orderDiscount)
        {
            var result = true;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                List<OrderDiscount> returnItem = new List<OrderDiscount>();

                rm = new ResponseMessage();

                foreach (var item in orderDiscount)
                {
                    returnItem.Add(this.discountHeaderBusiness.DiscountSave(item));
                }
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = returnItem;
                    await Common.UpdateEventLogsNew("DISCOUNT SAVED SUCCESSFULLY", reqHeader, controllerURL, orderDiscount, returnItem, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("DISCOUNT SAVE - NO CONTENT", reqHeader, controllerURL, orderDiscount, returnItem, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("DISCOUNT SAVE - ERROR", reqHeader, controllerURL, orderDiscount, null, rm.message, this.eventLogBusiness);
            }

            return Ok(rm);
        }

        /// <summary>
        /// removes Order's Discount Item
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample Data :
        /// 
        ///     {
        ///         "DiscountID":1015
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Boolean Value </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("DiscountRemove")]
        [MapToApiVersion("1.0")]
        public IActionResult orderDiscountRemove(ParamDiscount itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.DiscountRemove(itemData.DiscountID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        [HttpPost, Route("DiscountRemove")]
        [MapToApiVersion("1.1")]
        public IActionResult orderDiscountRemove1(ParamDiscount itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.DiscountRemove(itemData.DiscountID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DISCOUNT REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// gets Order's Discount Item
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "DiscountID":1000
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DISCOUNT ITEM!",
        ///       "data": {
        ///         "discountID": 1001,
        ///         "vendorID": 1060,
        ///         "uom": 3501,
        ///         "qty": 2,
        ///         "effectiveDate": "2024-11-11T10:16:25.323",
        ///         "expiryDate": "2024-11-30T10:16:25.323",
        ///         "discountType": 3000,
        ///         "discountAmount": 500,
        ///         "isActive": false,
        ///         "createdBy": 1060,
        ///         "modifiedBy": 0
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("GetDiscount")]
        [MapToApiVersion("1.0")]
        public IActionResult orderDiscountGet(ParamDiscount itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetDiscount(itemData.DiscountID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        [HttpPost, Route("GetDiscount")]
        [MapToApiVersion("1.1")]
        public IActionResult orderDiscountGet1(ParamDiscount itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetDiscount(itemData.DiscountID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// gets Order's Discount Items LIST BY Vendor
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "VendorID":1000
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DiscountListByVendor")]
        [MapToApiVersion("1.0")]
        public IActionResult discountListByVendor(ParamVendor item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetDiscountByVendor(item.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEMS LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }

        [HttpPost, Route("DiscountListByVendor")]
        [MapToApiVersion("1.1")]
        public IActionResult discountListByVendor1(ParamVendor item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetDiscountByVendor(item.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEMS LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// gets Order's Discount Items LIST BY Vendor Pagination
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request JSON :
        ///     {
        ///       "userID": 1060,
        ///       "pageNo": 1,
        ///       "rows": 10
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("DiscountListByVendorRow")]
        [MapToApiVersion("1.0")]
        public IActionResult discountListByVendorRow(ParamProductList item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                List<OrderDiscount> items = discountHeaderBusiness.GetDiscountListbyVendorRows(item.userID, item.PageNo, item.Rows);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST SUCCESSFULLY", reqHeader, controllerURL, item, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT LIST - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT LIST - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost]
        [Route("DiscountListByVendorRow")]
        [MapToApiVersion("1.1")]
        public IActionResult discountListByVendorRow1(ParamProductList item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                List<OrderDiscount> items = discountHeaderBusiness.GetDiscountListbyVendorRows(item.userID, item.PageNo, item.Rows);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT ITEMS LIST SUCCESSFULLY", reqHeader, controllerURL, item, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT LIST - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH DISCOUNT LIST - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// gets Order's Discount Count
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "DiscountID":1000
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DISCOUNT COUNT!",
        ///       "data": 2
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("GetDiscountCount")]
        [MapToApiVersion("1.0")]
        public IActionResult orderDiscountCount(ParamVendor itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetDiscountCount(itemData.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT COUNT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT COUNT SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT COUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT COUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        [HttpPost, Route("GetDiscountCount")]
        [MapToApiVersion("1.1")]
        public IActionResult orderDiscountCount1(ParamVendor itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetDiscountCount(itemData.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT COUNT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT COUNT SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT COUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT COUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Get Order's Discount By Vendor
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "VendorID":1060
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DISCOUNT COUNT!",
        ///       "data": [
        ///         {
        ///           "discountID": 1000,
        ///           "uom": 3501,
        ///           "qty": 2,
        ///           "discountType": 3000,
        ///           "discountAmount": 500
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("GetOrderDiscountByVendor")]
        [MapToApiVersion("1.0")]
        public IActionResult getOrderDiscountByVendor(ParamVendor itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.discountHeaderBusiness.GetOrderDiscountByVendor(itemData.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT COUNT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ORDER DISCOUNT ITEMS SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ORDER DISCOUNT ITEMS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ORDER DISCOUNT ITEMS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        ///////////// Discount Detail

        //[HttpPost, Route("DiscountDetailSave")]
        //public IActionResult discountDetailAdd(DiscountDetail discountDetail)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this.discountDetailBusiness.Save(discountDetail);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "DISCOUNT DETAILED SAVED SUCCESSFULLY!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;
        //    }

        //    return Ok(rm);
        //}

        //[HttpPost, Route("Detail/Remove")]
        //public IActionResult discountDetailRemove(ParamDiscountDetail itemData)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this.discountDetailBusiness.Remove(itemData.DiscountID, itemData.productID);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "DISCOUNT DETAILED REMOVED SUCCESSFULLY!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;
        //    }

        //    return Ok(rm);
        //}

        //[HttpPost, Route("Detail/Get")]
        //public IActionResult discountDetailGet(ParamDiscountDetail itemData)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this.discountDetailBusiness.Get(itemData.DiscountID, itemData.productID);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "FETCH DISCOUNT DETAIL ITEM!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;
        //    }

        //    return Ok(rm);
        //}

        //[HttpPost, Route("Detail/List")]
        //public IActionResult discountDetailList(ParamDiscountDetail itemData)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this.discountDetailBusiness.GetAll(itemData.DiscountID, itemData.productID);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "FETCH DISCOUNT DETAIL ITEM!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;
        //    }

        //    return Ok(rm);
        //}

        //#endregion
    }
}
