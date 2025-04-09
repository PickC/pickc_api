/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class VendorPaymentController : ControllerBase
    {
        private ResponseMessage rm;
        private readonly INotificationBusiness notificationBusiness;
        public readonly IEventLogBusiness eventLogBusiness;
        public readonly IVendorPaymentBusiness vendorPaymentBusiness;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;

        public VendorPaymentController(IConfiguration configuration, 
                                       INotificationBusiness IResultData, 
                                       IEventLogBusiness eventLogBusiness, 
                                       IVendorPaymentBusiness vendorPaymentBusiness,
                                       IWebHostEnvironment env)
        {
            this.notificationBusiness = IResultData;
            this.eventLogBusiness = eventLogBusiness;
            this.vendorPaymentBusiness = vendorPaymentBusiness;
            this.configuration = configuration;
            this.env = env;
        }

        /// <summary>
        /// Adds a Vendor's Payment.
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request:
        /// NOTE : For a new Vendor Payment, send the PaymentID = 0.
        /// 
        ///     {
        ///       "paymentID": 0,
        ///       "vendorID": 1060,
        ///       "subscriptionType": 4107,
        ///       "paymentAmount": 500,
        ///       "taxAmount": 0,
        ///       "totalAmount": 500,
        ///       "referenceNo": "112200",
        ///       "paymentStatus": 1000,
        ///       "createdOn": "2024-11-04T08:25:04.986Z"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> Add(VendorPayment item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.SaveVendorPayment(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        [HttpPost, Route("save")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public async Task<IActionResult> Add1(VendorPayment item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.SaveVendorPayment(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove a Vendor's Payment.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///       "paymentID": 1001
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">VENDOR PAYMENT REMOVED SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("Remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult Remove(ParamVendorPayment itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.vendorPaymentBusiness.RemoveVendorPayment(itemData.PaymentID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR PAYMENT REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        [HttpPost, Route("Remove")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult Remove1(ParamVendorPayment itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.vendorPaymentBusiness.RemoveVendorPayment(itemData.PaymentID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR PAYMENT REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Get a Vendor's Payment.
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request:
        /// 
        ///     {
        ///       "paymentID": 1000
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET VENDOR PAYMENT SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("Get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult Get(ParamVendorPayment itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.Get(itemData.PaymentID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR PAYMENT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        [HttpPost, Route("Get")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult Get1(ParamVendorPayment itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.Get(itemData.PaymentID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR PAYMENT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// GetAll Vendor's Payments.
        /// </summary>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH VENDOR PAYMENT SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("ListAll")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult ListAll()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.vendorPaymentBusiness.GetAll();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR PAYMENT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }
        [HttpPost, Route("ListAll")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult ListAll1()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.vendorPaymentBusiness.GetAll();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR PAYMENT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// GetAll Vendor's Payments by Pagination.
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request:
        /// 
        ///     {
        ///       "pageNo": 1,
        ///       "rows": 10
        ///     }
        ///
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH VENDOR PAYMENT SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response>
        [HttpPost]
        [Route("ListAllByRows")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult ListAllByRows(ParamPaymentList itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<VendorPayment> items = vendorPaymentBusiness.PaymentListbyRows(itemData.PageNo, itemData.Rows);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        [HttpPost]
        [Route("ListAllByRows")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult ListAllByRows1(ParamPaymentList itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<VendorPayment> items = vendorPaymentBusiness.PaymentListbyRows(itemData.PageNo, itemData.Rows);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR PAYMENT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR PAYMENT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get a Vendor's Payment Status.
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request:
        ///   {
        ///     "vendorID": 1060
        ///   }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET VENDOR PAYMENT STATUS SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("GetPaymentStatus")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetPaymentStatus(ParamVendor itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.GetPaymentStatus(itemData.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR PAYMENT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT STATUS SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT STATUS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT STATUS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        [HttpPost, Route("GetPaymentStatus")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult GetPaymentStatus1(ParamVendor itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.GetPaymentStatus(itemData.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR PAYMENT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT STATUS SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT STATUS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET VENDOR PAYMENT STATUS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Update a Vendor's Payment Reference No.
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request:
        ///     {
        ///       "paymentID": 1000,
        ///       "vendorID": 1060,
        ///       "referenceNo": "112200"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UpdateReferenceNo")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> UpdateReferenceNo(VendorPaymentStatus item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.UpdateReferenceNo(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        [HttpPost, Route("UpdateReferenceNo")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public async Task<IActionResult> UpdateReferenceNo1(VendorPaymentStatus item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = vendorPaymentBusiness.UpdateReferenceNo(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT HAS BEEN SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("VENDOR PAYMENT SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// GetAll Vendor's Payments.
        /// </summary>
        /// <remarks>
        /// 
        /// version : 1.1
        /// 
        /// Sample request:
        ///     {
        ///       "vendorID": 1060
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCHED PAYMENT LIST BY VENDOR SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("ListByVendor")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult ListByVndor(ParamVendor item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.vendorPaymentBusiness.ListByVendor(item.VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PAYMENT LIST BY VENDOR!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCHED PAYMENT LIST BY VENDOR SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCHED PAYMENT LIST BY VENDOR - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCHED PAYMENT LIST BY VENDOR - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }

        [HttpPost, Route("ListByVendor")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult ListByVndor1(ParamVendor item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.vendorPaymentBusiness.ListByVendor(item.VendorID);
                
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PAYMENT LIST BY VENDOR!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCHED PAYMENT LIST BY VENDOR SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCHED PAYMENT LIST BY VENDOR - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCHED PAYMENT LIST BY VENDOR - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }
    }
}
