/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using static appify.models.NotificationType;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]

    public class WebAdminController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IProductBusiness productBusiness;
        private readonly IMemberBusiness memberBusiness;
        private readonly IWebAdminBusiness webAdminBusiness;
        private readonly IRolesBusiness rolesBusiness;
        private ResponseMessage rm;
        private readonly INotificationBusiness notificationBusiness;
        public WebAdminController(IConfiguration configuration, IMemberBusiness memberBusiness, IProductBusiness product, IEventLogBusiness eventLogBusiness, IWebAdminBusiness webAdminBusiness, INotificationBusiness notificationBusiness, IRolesBusiness rolesBusiness)
        {
            this.configuration = configuration;
            this.productBusiness = product;
            this.memberBusiness = memberBusiness;
            this.eventLogBusiness = eventLogBusiness;
            this.webAdminBusiness = webAdminBusiness;
            this.notificationBusiness = notificationBusiness;
            this.rolesBusiness = rolesBusiness;
        }

        /// <summary>
        /// Add/Update User.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : For a new / update User
        /// 
        ///     {
        ///       "userID": 0,
        ///       "userName": "Gurjeet",
        ///       "password": "Singh",
        ///       "Department": 1000,
        ///       "userDesignation": 10,
        ///       "employeeID": "ABC001",
        ///       "emailID": "nkolweb@gmail.com",
        ///       "contactNo": "9810722979",
        ///       "isActive": true,
        ///       "isAccepted":false,
        ///       "isAllowLogOn": true,
        ///       "isOperational": true,
        ///       "createdBy": "SuperAdmin",
        ///       "createdOn": "2025-01-16T11:28:51.296Z",
        ///       "modifiedBy": "SuperAdmin",
        ///       "modifiedOn": "2025-01-16T11:28:51.296Z",
        ///       "roleCode": "Role001"
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created User Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 


        // POST api/<MemberController>
        [HttpPost, Route("User/Register")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Register(appify.models.User item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                Int64 UserID = item.UserID;
                rm = new ResponseMessage();
                var memberItem = this.webAdminBusiness.RegisterUser(item);
                if (memberItem.UserID > 0)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "USER REGISTRATION SUCCESSFUL!";
                    rm.name = StatusName.ok;
                    rm.data = memberItem;

                        await Common.UpdateEventLogsNew("USER REGISTRATION SUCCESSFUL", reqHeader, controllerURL, item, memberItem, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO REGISTER USER";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("UNABLE TO REGISTER USER", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("USER REGISTRATION - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Delete the User
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1000
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MEMBER DELETED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 

        [HttpPost, Route("User/DeleteUser")]
        [MapToApiVersion("1.0")]
        public IActionResult DeleteMember(ParamUserID itemData)
        {
            //dynamic data = jsondata;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.webAdminBusiness.RemoveUser(itemData.userID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "USER HAS BEEN SUCCESSFULLY DELETED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("USER DELETED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DELET THE USER";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO DELETED USER", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("USER DELETE - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// Get the User
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1000
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH USER",
        ///       "data": {
        ///         "userID": 1000,
        ///         "userName": "Gurjeet222",
        ///         "password": "Singh222",
        ///         "department": 1003,
        ///         "userDesignation": 122,
        ///         "employeeID": "ABC001",
        ///         "emailID": "nkolweb@gmail.com",
        ///         "contactNo": "981072297922",
        ///         "isActive": false,
        ///         "isAccepted":true,
        ///         "isAllowLogOn": true,
        ///         "isOperational": false,
        ///         "createdBy": "SuperAdmin",
        ///         "createdOn": "2025-01-20T10:34:24.76",
        ///         "modifiedBy": "SuperAdmin",
        ///         "modifiedOn": "2025-01-20T10:36:47.663",
        ///         "roleCode": "Role00222"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("User/Get")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetMember(ParamUserID ItemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var user = this.webAdminBusiness.GetUser(ItemData.userID);
                if (user != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH USER";
                    rm.name = StatusName.ok;
                    rm.data = user;

                    await Common.UpdateEventLogsNew("FETCH USER SUCCESSFULLY", reqHeader, controllerURL, ItemData, user, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("FETCH USER - NO CONTENT", reqHeader, controllerURL, ItemData, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("FETCH MEMBER - ERROR", reqHeader, controllerURL, ItemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }
        /// <summary>
        /// Users RecordCount
        /// </summary>
        /// <remarks>   
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH USERS COUNT!",
        ///       "data": 1
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("User/recordcount")]
        [MapToApiVersion("1.0")]
        public IActionResult UsersRecordCount()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.webAdminBusiness.GetUsersCount();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH USERS COUNT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// User List by PageView
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "pageNo": 1,
        ///       "rows": 10
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES BY PAGE VIEW!",
        ///       "data": [
        ///         {
        ///           "userID": 1000,
        ///           "userName": "string",
        ///           "password": "string",
        ///           "department": 0,
        ///           "userDesignation": 0,
        ///           "employeeID": "string",
        ///           "emailID": "string",
        ///           "contactNo": "string",
        ///           "isActive": false,
        ///           "isAccepted": false,
        ///           "isAllowLogOn": false,
        ///           "isOperational": false,
        ///           "createdBy": "string",
        ///           "createdOn": "2025-01-20T17:42:55.19",
        ///           "modifiedBy": "string",
        ///           "modifiedOn": "2025-01-23T10:18:05.81",
        ///           "roleCode": "string"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("User/pageview")]
        [MapToApiVersion("1.0")]
        public IActionResult UsersByPageView(ParamPageView itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.webAdminBusiness.ListbyPageView(itemData.PageNo, itemData.Rows);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES BY PAGE VIEW!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Check the User if exists
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": "nkolweb@gmail.com"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCHED USER DATA",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("User/Check")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CheckUser(ParamEmail itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var user = this.webAdminBusiness.CheckUser(itemData.emailID);
                if (user != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH USER";
                    rm.name = StatusName.ok;
                    rm.data = user;
                    //await Common.UpdateEventLogsNew("FETCHED USER DATA SUCCESSFULLY", reqHeader, controllerURL, itemData, user, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //await Common.UpdateEventLogsNew("FETCHED USER DATA - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //await Common.UpdateEventLogsNew("FETCHED USER - ERROR", reqHeader, controllerURL, userID, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }
        ///<summary>
        /// User Login
        /// </summary>
        ///<remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "emailID": "rama@appi-fy.ai",
        ///       "password": "Appify@123",
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "LOGIN DATA",
        ///       "data": {
        ///         "userID": 1001,
        ///         "userName": "Gurjeet2",
        ///         "password": "Singh2",
        ///         "department": 1002,
        ///         "userDesignation": 12,
        ///         "employeeID": "ABC001",
        ///         "emailID": "nkolweb@gmail.com",
        ///         "contactNo": "98107229792",
        ///         "isActive": true,
        ///         "isAccepted":false,
        ///         "isAllowLogOn": true,
        ///         "isOperational": true,
        ///         "createdBy": "SuperAdmin",
        ///         "createdOn": "2025-01-20T10:35:50.4",
        ///         "modifiedBy": "SuperAdmin",
        ///         "modifiedOn": "2025-01-20T10:35:50.4",
        ///         "roleCode": "Role002"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Login - SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response>
        [HttpPost,Route("User/Login")]
        [MapToApiVersion("1.0")]
        public IActionResult SignIn(ParamLogIn itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            try
            {
                rm = new ResponseMessage();
                var returnData = this.webAdminBusiness.LogIn(itemData.emailID, itemData.password);
                if (returnData != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOGIN DATA";
                    rm.name = StatusName.ok;
                    rm.data = returnData;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("WebLogIn - SUCCESSFULLY", reqHeader, controllerURL, itemData, returnData, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "Invalid EmailID or Password!";
                    rm.name = StatusName.invalidCred;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Invalid EmailID or Password!", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalidCred;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("WebLogIn - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// Reset the User Password
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "emailID": "nkolweb@gmail.com",
        ///       "password": "Singh2"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">PASSWORD RESET SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("User/ResetPassword")]
        [MapToApiVersion("1.0")]
        public IActionResult ResetPassword(ParamLogIn itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                //dynamic data = jsondata;

                rm = new ResponseMessage();
                var user = this.webAdminBusiness.ResetPassword(itemData.emailID, itemData.password);
                if (user != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PASSWORD RESET SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = user;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PASSWORD RESET SUCCESSFULLY", reqHeader, controllerURL, itemData, user, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO RESET PASSWORD";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO RESET PASSWORD", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PASSWORD RESET - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// User's Activation
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "EmailID": "nkolweb@gmail.com"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "THE EMAIL HAS BEEN SENT SUCCESSFULLY",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("User/Activation")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Activation(ParamEmail itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                string mailbody = string.Empty;
                EmailNotificationTemplate emailNotificationTemplate = notificationBusiness.GetEmailNotificationTemplate(Convert.ToInt64(NotificationTemplateType.UserActivation));
                List<EmailUserHeader> getEmailUserHeader = notificationBusiness.GetUserDetails(itemData.emailID);

                Notifications notifications = new Notifications
                {
                    EmailSubject = emailNotificationTemplate.Subject,//Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString()),
                    EmailTemplateURL = emailNotificationTemplate.TemplateURL,
                    ToEmail = itemData.emailID
                };
                string path = notifications.EmailTemplateURL;
                using (StreamReader reader = new StreamReader(path))
                {
                    mailbody = reader.ReadToEnd();
                }

                mailbody = mailbody.Replace("{{name}}", getEmailUserHeader.Count==0? "User" : getEmailUserHeader[0].UserName.ToString());
                mailbody = mailbody.Replace("{{userId}}", getEmailUserHeader.Count == 0 ? "1000" : getEmailUserHeader[0].UserID.ToString());


                notifications.EmailBody = mailbody;
                var user = EmailNotification.SendEmailCommon(notifications, notificationBusiness);
                if (user != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THE EMAIL HAS BEEN SENT SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = user;
                    //await Common.UpdateEventLogsNew("THE EMAIL HAS BEEN SENT SUCCESSFULLY", reqHeader, controllerURL, EmailID, user, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //await Common.UpdateEventLogsNew("EMAIL HAS - NO CONTENT", reqHeader, controllerURL, EmailID, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //await Common.UpdateEventLogsNew("EMAIL - ERROR", reqHeader, controllerURL, EmailID, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Forgot User's Password
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "EmailID": "nkolweb@gmail.com"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "THE EMAIL HAS BEEN SENT SUCCESSFULLY",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("User/ForgotPassword")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ForgotPassword(ParamEmail itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                string mailbody = string.Empty;
                EmailNotificationTemplate emailNotificationTemplate = notificationBusiness.GetEmailNotificationTemplate(Convert.ToInt64(NotificationTemplateType.ForgotPassword));
                List<EmailUserHeader> getEmailUserHeader = notificationBusiness.GetUserDetails(itemData.emailID);

                Notifications notifications = new Notifications
                {
                    EmailSubject = emailNotificationTemplate.Subject,//Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString()),
                    EmailTemplateURL = emailNotificationTemplate.TemplateURL,
                    ToEmail = itemData.emailID
                };
                string path = notifications.EmailTemplateURL;
                using (StreamReader reader = new StreamReader(path))
                {
                    mailbody = reader.ReadToEnd();
                }

                mailbody = mailbody.Replace("{{name}}", getEmailUserHeader.Count==0 ? "User" : getEmailUserHeader[0].UserName.ToString());
                mailbody = mailbody.Replace("{{userId}}", getEmailUserHeader.Count == 0 ? "1000" : getEmailUserHeader[0].UserID.ToString());

                notifications.EmailBody = mailbody;
                var user = EmailNotification.SendEmailCommon(notifications, notificationBusiness);
                if (user != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THE EMAIL HAS BEEN SENT SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = user;
                    //await Common.UpdateEventLogsNew("THE EMAIL HAS BEEN SENT SUCCESSFULLY", reqHeader, controllerURL, EmailID, user, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //await Common.UpdateEventLogsNew("EMAIL HAS - NO CONTENT", reqHeader, controllerURL, EmailID, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //await Common.UpdateEventLogsNew("EMAIL - ERROR", reqHeader, controllerURL, EmailID, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// FETCH PRODUCT LIST
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH PRODUCT LIST",
        ///       "data": [
        ///         {
        ///           "vendorName": null,
        ///           "categoryDescription": null,
        ///           "productID": 1000,
        ///           "vendorID": 1000,
        ///           "productName": "Linen Sarees ",
        ///           "description": "Linen wear",
        ///           "category": 3607,
        ///           "brand": "Dhaage Life",
        ///           "size": "",
        ///           "color": "Multi colour ",
        ///           "uom": 3500,
        ///           "weight": 0,
        ///           "priceID": 1000,
        ///           "currency": "INR",
        ///           "imageID": 1000,
        ///           "isActive": true,
        ///           "isAvailable": true,
        ///           "stockQty": 0,
        ///           "createdOn": null,
        ///           "modifiedOn": null,
        ///           "hsnCode": null,
        ///           "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///           "isNew": false
        ///         } 
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost]
        [Route("products")]
        [MapToApiVersion("1.0")]
        public IActionResult ListAllProducts() {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<ProductWeb> items = productBusiness.ListAll();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// FETCH VENDOR LIST
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH VENDOR LIST",
        ///           "data": [
        ///             {
        ///               "userID": 1008,
        ///               "emailID": "kvr210885@gmail.com",
        ///               "mobileNo": "8682944609",
        ///               "password": "Entombed@25",
        ///               "firstName": "Kalyan",
        ///               "lastName": "KVR",
        ///               "memberType": 1001,
        ///               "otp": "604174",
        ///               "isOTPSent": true,
        ///               "otpSentDate": "2023-09-13T12:35:28.417",
        ///               "isResendOTP": false,
        ///               "isOTPVerified": true,
        ///               "isEmailVerified": true,
        ///               "isActive": true,
        ///               "createdOn": "2023-09-13T00:05:29.93",
        ///               "profilePhoto": "image_cropper_1694677575946.jpg",
        ///               "token": "¦iät\u0006\tOŸ\u0005c `\u0015\u001f",
        ///               "platformType": 0,
        ///               "parentID": 1004,
        ///               "isRegisteredByMobile": true,
        ///               "isOnlinePaymentEnabled": true,
        ///               "isEnterprise": null,
        ///               "isEcommerce": null,
        ///               "isWelcomeEmail": null
        ///             }
        ///             ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Vendor List Items </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost]
        [Route("vendors")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ListAllVendors()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<Member> items = memberBusiness.GetAllMembers();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("FETCH VENDOR LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                    await Common.UpdateEventLogsNew("FETCH VENDOR LIST - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                ////this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH VENDOR LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// FETCH VENDOR'S DETAIL
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1060
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "VENDOR'S DETAIL",
        ///       "data": {
        ///         "userID": 1060,
        ///         "emailID": "kiran76@gmail.com",
        ///         "mobileNo": "6382014003",
        ///         "password": "Appify@123",
        ///         "firstName": "I Am Back",
        ///         "lastName": "appify",
        ///         "memberType": 1000,
        ///         "otp": "731885",
        ///         "isOTPSent": true,
        ///         "otpSentDate": "2023-11-01T13:20:59.313",
        ///         "isResendOTP": false,
        ///         "isOTPVerified": true,
        ///         "isEmailVerified": false,
        ///         "isActive": true,
        ///         "createdOn": "2023-11-01T00:51:01.6",
        ///         "profilePhoto": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1726647603113",
        ///         "token": "dQy98vnwSRaGmf4VpUzLfN:APA91bGBvqzspXLG0J3nKdPOIvC3-HQkdz-///l ok xmtplx04cmCEcnbkubosRxIR2fjGKrwa2YpqF-4LfwV68dqPwRzISIhUH_5zTd8DMxIT4x1yg4W3WFDnbZidpA0sVqnCz8e_4CZEad",
        ///         "platformType": 0,
        ///         "parentID": 0,
        ///         "isRegisteredByMobile": true,
        ///         "isOnlinePaymentEnabled": true,
        ///         "isEnterprise": false,
        ///         "isEcommerce": false,
        ///         "isWelcomeEmail": true
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost]
        [Route("vendordetails")]
        [MapToApiVersion("1.0")]
        public IActionResult GetVendorDetails(ParamMemberUserID itemData )
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            Member item = new Member();
            try
            {
                rm = new ResponseMessage();
                item = memberBusiness.GetMember(itemData.userID);
                if (item!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR'S DETAIL";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDORDETAILS FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    item = new Member();
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = item;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDORDETAILS FETCH PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDORDETAILS FETCH PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }


        //[HttpPost]
        //[Route("customersbyvendor")]
        //public IActionResult CustomersByVendor(ParamMemberUserID itemData)
        //{
        //    Member item = new Member();
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        item = memberBusiness.GetAllMembers();
        //        if (item != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "FETCH PRODUCT LIST";
        //            rm.name = StatusName.ok;
        //            rm.data = item;
        //        }
        //        else
        //        {
        //            item = new Member();
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = item;
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
        /// <summary>
        /// Get Role Details
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "roleCode": "ROLE_SUPER_ADMIN"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES ITEM!",
        ///       "data": {
        ///         "roleCode": "ROLE_SUPER_ADMIN",
        ///         "roleDescription": "Has unrestricted access to all system features, settings, and data. Responsible for managing other a",
        ///         "isActive": false,
        ///         "createdBy": 0,
        ///         "createdOn": "2025-01-23T08:40:21.037",
        ///         "modifiedBy": 0,
        ///         "modifiedOn": "0001-01-01T00:00:00"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("roles/get")]
        [MapToApiVersion("1.0")]
        public IActionResult getRole(ParamRole itemData) {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.Get(itemData.RoleCode);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Role List
        /// </summary>
        /// <remarks>  
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES ITEM!",
        ///       "data": {
        ///         "roleCode": "ROLE_SUPER_ADMIN",
        ///         "roleDescription": "Has unrestricted access to all system features, settings, and data. Responsible for managing other a",
        ///         "isActive": false,
        ///         "createdBy": 0,
        ///         "createdOn": "2025-01-23T08:40:21.037",
        ///         "modifiedBy": 0,
        ///         "modifiedOn": "0001-01-01T00:00:00"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/list")]
        [MapToApiVersion("1.0")]
        public IActionResult ListRoles()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.ListAll();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Roles RecordCount
        /// </summary>
        /// <remarks>   
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES COUNT!",
        ///       "data": 1
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/recordcount")]
        [MapToApiVersion("1.0")]
        public IActionResult RolesRecordCount()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.GetRolesCount();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES COUNT!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Role List by PageView
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "pageNo": 0,
        ///       "rows": 0
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES BY PAGE VIEW!",
        ///       "data": [
        ///         {
        ///           "roleCode": "ROLE_SUPER_ADMIN",
        ///           "roleDescription": "Has unrestricted access to all system features, settings, and data. Responsible for managing other a",
        ///           "isActive": false,
        ///           "createdBy": 0,
        ///           "createdOn": "2025-01-23T08:40:21.037",
        ///           "modifiedBy": 0,
        ///           "modifiedOn": "0001-01-01T00:00:00"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/pageview")]
        [MapToApiVersion("1.0")]
        public IActionResult RolesByPageView(ParamPageView itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.ListbyPageView(itemData.PageNo,itemData.Rows);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES BY PAGE VIEW!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Add/Edit Roles
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "roleCode": "string",
        ///       "roleDescription": "string",
        ///       "isActive": true,
        ///       "createdBy": 0,
        ///       "createdOn": "2025-01-23T08:28:46.248Z",
        ///       "modifiedBy": 0,
        ///       "modifiedOn": "2025-01-23T08:28:46.248Z"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SAVE ROLES ITEM!",
        ///       "data": {
        ///         "roleCode": "ROLE_SUPER_ADMIN",
        ///         "roleDescription": "Has unrestricted access to all system features, settings, and data. Responsible for managing other admins and o  ve rseeing the ///entire platform.",
        ///         "isActive": true,
        ///         "createdBy": 0,
        ///         "createdOn": "2025-01-23T08:39:51.511Z",
        ///         "modifiedBy": 0,
        ///         "modifiedOn": "2025-01-23T08:39:51.511Z"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/save")]
        [MapToApiVersion("1.0")]
        public IActionResult SaveRole(Roles itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.Save(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SAVE ROLES ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Remove Role's
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "roleCode": "string",
        ///       "modifiedBy": 0
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "REMOVE ROLES ITEM!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult DeleteRole(RolesDecativate itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.Delete(itemData.RoleCode,itemData.ModifiedBy);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "REMOVE ROLES ITEM!";
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
        /// Get the User
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "category": "ROLES"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH USER",
        ///       "data": [
        ///         {
        ///           "lookupCategory": "ROLES",
        ///           "lookupCode": "ROLE_ADMIN",
        ///           "lookupDescription": "Has access to most features, including managing users, content, and system settings, but with some restrictions compared to the Super Admin.",
        ///           "mappingCode": ""
        ///         },
        ///         {
        ///           "lookupCategory": "ROLES",
        ///           "lookupCode": "ROLE_EDITOR",
        ///           "lookupDescription": "Manages the creation, editing, and publishing of content, such as blog posts, pages, or articles.",
        ///           "mappingCode": ""
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("roles/AccessType")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAccessType(ParamLookupCategory itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var user = this.rolesBusiness.GetAccessType(itemData.category);
                if (user != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH USER";
                    rm.name = StatusName.ok;
                    rm.data = user;

                    //await Common.UpdateEventLogsNew("FETCH USER SUCCESSFULLY", reqHeader, controllerURL, LookupCategory, user, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //await Common.UpdateEventLogsNew("FETCH USER - NO CONTENT", reqHeader, controllerURL, LookupCategory, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //await Common.UpdateEventLogsNew("FETCH MEMBER - ERROR", reqHeader, controllerURL, LookupCategory, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }
    }
}
