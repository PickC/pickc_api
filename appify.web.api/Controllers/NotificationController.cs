/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.Business.Contract;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]

    public class NotificationController : Controller
    {
        private IConfiguration configuration;
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly INotificationBusiness notificationBusiness; 
        private ResponseMessage rm;
        public NotificationController(IConfiguration configuration, INotificationBusiness notificationBusiness, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.notificationBusiness = notificationBusiness;
            this.eventLogBusiness = eventLogBusiness;
        }

        /// <summary>
        /// gets Notification items information based on Vendor ID
        /// </summary>
        /// <remarks>
        /// Changes :
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.0 (DEFAULT version)
        /// 
        /// Description : Retrives the In-App Notifications based on Customer ID 
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1833
        ///     }
        ///
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.1
        /// 
        /// Description : Retrives the In-App Notifications based on Vendor ID with pageview
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1833,
        ///         "pageNo":1,
        ///         "rows":10
        ///     }
        /// 
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH NOTIFICATION LIST!",
        ///       "data": [
        ///         {
        ///           "notificationID": 1000,
        ///           "senderID": 1833,
        ///           "receiverID": 1847,
        ///           "notificationDate": "2024-05-06T15:55:06.807",
        ///           "notificationMessage": "You have received a new order is PO1473150202312150614. click here to view the order details.",
        ///           "notificationEvent": 0,
        ///           "isRead": false,
        ///           "notificationStatus": 1,
        ///           "readOn": false,
        ///           "isCancel": false
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("notificationlistbyvendor")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetNotificationByVendor(ParamMemberVendorID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = this.notificationBusiness.GetNotificationByVendor(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH NOTIFICATION LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByVendor SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByVendor - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByVendor - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
       
        [HttpPost]
        [Route("notificationlistbyvendor")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult GetNotificationByVendorPagination(ParamMemberVendorIDPagination itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = this.notificationBusiness.GetNotificationByVendor(itemData.userID,itemData.PageNo,itemData.Rows);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH NOTIFICATION LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByVendor SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByVendor - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByVendor - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }


        /// <summary>
        /// gets Notification items information based on Customer ID
        /// </summary>
        /// <remarks>
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.0 (DEFAULT version)
        /// 
        /// Description : Retrives the In-App Notifications based on Customer ID 
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1847
        ///     }
        ///     
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.1
        /// 
        /// Description : Retrives the In-App Notifications based on Customer ID with pageview
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1833,
        ///         "pageNo":1,
        ///         "rows":10
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH NOTIFICATION LIST!",
        ///       "data": [
        ///         {
        ///           "notificationID": 1000,
        ///           "senderID": 1833,
        ///           "receiverID": 1847,
        ///           "notificationDate": "2024-05-06T15:55:06.807",
        ///           "notificationMessage": "Your Order No is PO1473150202312150614 Successfuly Placed! View your order details here",
        ///           "notificationEvent": 0,
        ///           "isRead": false,
        ///           "notificationStatus": 1,
        ///           "readOn": false,
        ///           "isCancel": false
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("notificationlistbyuser")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetNotificationByUser(ParamMemberVendorID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = this.notificationBusiness.GetNotificationByUser(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH NOTIFICATION LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByUser SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByUser - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByUser - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }



        /// <summary>
        /// gets Notification items information based on Customer ID
        /// </summary>
        /// <remarks>
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.1
        /// 
        /// Description : Retrives the In-App Notifications based on Vendor ID with pageview
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1833,
        ///         "pageNo":1,
        ///         "rows":10
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH NOTIFICATION LIST!",
        ///       "data": [
        ///         {
        ///           "notificationID": 1000,
        ///           "senderID": 1833,
        ///           "receiverID": 1847,
        ///           "notificationDate": "2024-05-06T15:55:06.807",
        ///           "notificationMessage": "Your Order No is PO1473150202312150614 Successfuly Placed! View your order details here",
        ///           "notificationEvent": 0,
        ///           "isRead": false,
        ///           "notificationStatus": 1,
        ///           "readOn": false,
        ///           "isCancel": false
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("notificationlistbyuser")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult GetNotificationByUserPagination(ParamMemberVendorIDPagination itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = this.notificationBusiness.GetNotificationByUser(itemData.userID,itemData.PageNo,itemData.Rows);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH NOTIFICATION LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByUser SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByUser - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetNotificationByUser - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }


        /// <summary>
        /// SET IsRead Notification by NotificationID
        /// </summary>
        /// <remarks>
        /// Sample Data :
        /// 
        ///     {
        ///         "NotificationID":1001
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Boolean Value </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("IsRead")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult isReadNotification(ParamMemberNotificationID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = this.notificationBusiness.IsReadNotification(itemData.NotificationID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ISREAD NOTIFICATION HAS BEEN SUCCESSFULLY SET!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ISREAD NOTIFICATION HAS BEEN SUCCESSFULLY SET", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ISREAD NOTIFICATION - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ISREAD NOTIFICATION - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// GET UnRead Count Notification by UserID
        /// </summary>
        /// <remarks>
        /// Sample Data :
        /// 
        ///     {
        ///         "UserID":1001
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns String Value </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("UnReadCount")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult unReadCountNotification(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = this.notificationBusiness.unReadCountNotification(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "UNREAD NOTIFICATION COUNT HAS BEEN SUCCESSFULLY RETURNED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNREAD NOTIFICATION COUNT HAS BEEN SUCCESSFULLY RETURNED", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNREAD NOTIFICATION - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNREAD NOTIFICATION - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

    }
}
