using appify.Business.Contract;
using appify.models;
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
        public IActionResult discountHeaderAdd(List<DiscountHeader> discountHeader)
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, discountHeader, returnItem, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, discountHeader, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, discountHeader, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, result, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, result, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
        ///           "effectiveDate": "2024-04-25T15:55:06.807",
        ///           "expiryDate": "2024-04-30T15:55:06.807",
        ///           "price": 1200,
        ///           "discountType": 3000,
        ///           "discountValue": 1000,
        ///           "discountTypeDescription": "AMOUNT",
        ///           "isActive": true,
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, result, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, result, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
