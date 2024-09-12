using appify.Business.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using appify.models;
using Asp.Versioning;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]

    public class EventLogController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IEventLogBusiness eventLogBusiness;
        private ResponseMessage rm;
        public EventLogController(IConfiguration configuration, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.eventLogBusiness = eventLogBusiness;
        }
        /*/// <summary>
        /// Adds a Event Log.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : For a new Event Log object, send the eventID = 0.
        /// 
        ///     {
        ///         "eventID": 0,
        ///         "eventType": 2,
        ///         "vendorID": 1505,
        ///         "customerID": 1003,
        ///         "source": "OrderAdd",
        ///         "module": 11001112334,
        ///         "ipAddress": "127.168.1.1",
        ///         "eventLog": "May 08 10:45:44 microsoft.windows.test AgentDevice=WindowsLog",
        ///         "inputJSON": ""Context": {'CorrelationId': '064205E2-F7CF-43A6-B514-4B55536C2B67','FileName": "\\server\\share\\NodiniteHappyCustomerList.txt'}",
        ///         "eventTime": "0001-01-01T00:00:00",
        ///         "appName": "AppifVensor"
        ///     }
        /// </remarks>
        /// <param name="eventLog"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Event Log Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("Save")]
        public IActionResult eventLogAdd(EventLogs eventLog)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this.eventLogBusiness.eventLogAdd(eventLog);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "EVENT LOG SAVED SUCCESSFULLY!";
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
        /// removes Event Log by EventID
        /// </summary>
        /// <remarks>
        /// Sample Data :
        /// 
        ///     {
        ///         "EventID":2
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Boolean Value </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("Remove")]
        public IActionResult eventLogRemove(ParamEventID itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this.eventLogBusiness.eventLogRemove(itemData.EventID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "EVENT LOG REMOVED SUCCESSFULLY!";
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

        */

        /// <summary>
        /// gets Event Log by EventID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "EventID":2
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH EVENT LOG ITEM!",
        ///       "data": {
        ///         "eventID": 2,
        ///         "eventType": 2,
        ///         "vendorID": 1505,
        ///         "customerID": 1003,
        ///         "source": "OrderAdd",
        ///         "module": 11001112334,
        ///         "ipAddress": "127.168.1.1",
        ///         "eventLog": "May 08 10:45:44 microsoft.windows.test AgentDevice=WindowsLog",
        ///         "inputJSON": ""Context": {'CorrelationId': '064205E2-F7CF-43A6-B514-4B55536C2B67','FileName": "\\server\\share\\NodiniteHappyCustomerList.txt'}",
        ///         "eventTime": "0001-01-01T00:00:00",
        ///         "appName": "AppifVensor"
        ///       }
        ///     }
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns eventLog Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 



        [HttpPost, Route("get")]
        [MapToApiVersion("1.0")]
        public IActionResult eventLogGet(ParamEventID itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this.eventLogBusiness.eventLogGet(itemData.EventID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH EVENT LOG ITEM!";
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
        /// LIST of All the Event Logs
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "EventID":2
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH EVENT LOG ITEM!",
        ///       "data": [
        ///         {
        ///           "eventID": 2,
        ///           "eventType": 2,
        ///           "vendorID": 1505,
        ///           "customerID": 1003,
        ///           "source": "OrderAdd",
        ///           "module": 11001112334,
        ///           "ipAddress": "127.168.1.1",
        ///           "eventLog": "May 08 10:45:44 microsoft.windows.test AgentDevice=WindowsLog",
        ///           "inputJSON": ""Context": {'CorrelationId': '064205E2-F7CF-43A6-B514-4B55536C2B67','FileName": "\\server\\share\\Nodinite HappyCusto merList.txt'}",
        ///           "eventTime": "0001-01-01T00:00:00",
        ///             "appName": "AppifVensor"
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        public IActionResult eventLogList()
        {
            try
            {
                rm = new ResponseMessage();
                var result = this.eventLogBusiness.eventLogList();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH EVENT LOG ITEM!";
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
        /// LIST of EventLog By VendorID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "VendorID":1505
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH BY VENDOR EVENT LOG ITEM!",
        ///       "data": [
        ///         {
        ///           "eventID": 2,
        ///           "eventType": 2,
        ///           "vendorID": 1505,
        ///           "customerID": 1003,
        ///           "source": "OrderAdd",
        ///           "module": 11001112334,
        ///           "ipAddress": "127.168.1.1",
        ///           "eventLog": "May 08 10:45:44 microsoft.windows.test AgentDevice=WindowsLog",
        ///           "inputJSON": ""Context": {'CorrelationId': '064205E2-F7CF-43A6-B514-4B55536C2B67','FileName": "\\server\\share\\Nodinite    HappyCusto merList.txt'}",
        ///           "eventTime": "0001-01-01T00:00:00",
        ///             "appName": "AppifVensor"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("listbyvendor")]
        [MapToApiVersion("1.0")]
        public IActionResult eventLogListByVendor(ParamMemberUserID itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this.eventLogBusiness.eventLogListByVendor(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH BY VENDOR EVENT LOG ITEM!";
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
        /// LIST of EventLog By CustomerID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         CustomerID":1003
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH BY CUSTOMER EVENT LOG ITEM!",
        ///       "data": [
        ///         {
        ///           "eventID": 2,
        ///           "eventType": 2,
        ///           "vendorID": 1505,
        ///           "customerID": 1003,
        ///           "source": "OrderAdd",
        ///           "module": 11001112334,
        ///           "ipAddress": "127.168.1.1",
        ///           "eventLog": "May 08 10:45:44 microsoft.windows.test AgentDevice=WindowsLog",
        ///           "inputJSON": ""Context": {'CorrelationId': '064205E2-F7CF-43A6-B514-4B55536C2B67','FileName": "\\server\\share\\Nodinite    HappyCusto merList.txt'}",
        ///           "eventTime": "0001-01-01T00:00:00",
        ///             "appName": "AppifVensor"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("listbycustomer")]
        [MapToApiVersion("1.0")]
        public IActionResult eventLogListByCustomer(ParamMemberUserID itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this.eventLogBusiness.eventLogListByCustomer(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH BY CUSTOMER EVENT LOG ITEM!";
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
    }
}
