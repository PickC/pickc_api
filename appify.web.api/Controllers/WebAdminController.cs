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
using System.Json;
using static appify.models.HomePageProductByCategory;
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
        private readonly IWebHostEnvironment env;

        private readonly IAdminDashboardBusiness adminDashboardBusiness;
        private ResponseMessage rm;
        private readonly INotificationBusiness notificationBusiness;
        public WebAdminController(IConfiguration configuration, 
                                  IMemberBusiness memberBusiness, 
                                  IProductBusiness product, 
                                  IEventLogBusiness eventLogBusiness, 
                                  IWebAdminBusiness webAdminBusiness, 
                                  INotificationBusiness notificationBusiness,  
                                  IAdminDashboardBusiness adminDashboardBusiness,
                                  IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.productBusiness = product;
            this.memberBusiness = memberBusiness;
            this.eventLogBusiness = eventLogBusiness;
            this.webAdminBusiness = webAdminBusiness;
            this.notificationBusiness = notificationBusiness;
            this.adminDashboardBusiness = adminDashboardBusiness;
            this.env = env;
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
        ///       "roleID": 1000
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

                    
                    /// <summary>
                    /// send email for the newly registered user.
                    /// </summary>

                    string mailbody = string.Empty;
                    EmailNotificationTemplate emailNotificationTemplate = notificationBusiness.GetEmailNotificationTemplate(Convert.ToInt64(NotificationTemplateType.UserActivation));
                    //List<EmailUserHeader> getEmailUserHeader = notificationBusiness.GetUserDetails(memberItem.EmailID);

                    Notifications notifications = new Notifications
                    {
                        EmailSubject = emailNotificationTemplate.Subject,//Replace("{{order_number}}", getEmailNotificationHeader[0].OrderNo.ToString()),
                        EmailTemplateURL = emailNotificationTemplate.TemplateURL,
                        ToEmail = memberItem.EmailID
                    };
                    string path = notifications.EmailTemplateURL;
                    using (StreamReader reader = new StreamReader(path))
                    {
                        mailbody = reader.ReadToEnd();
                    }

                    //mailbody = mailbody.Replace("{{name}}", getEmailUserHeader.Count == 0 ? "User" : getEmailUserHeader[0].UserName.ToString());
                    //mailbody = mailbody.Replace("{{userId}}", getEmailUserHeader.Count == 0 ? "1000" : getEmailUserHeader[0].UserID.ToString());

                    mailbody = mailbody.Replace("{{name}}", memberItem.UserName.ToString());
                    mailbody = mailbody.Replace("{{userId}}", memberItem.UserID.ToString());


                    notifications.EmailBody = mailbody;
                    var emailResult = await EmailNotification.SendEmailCommon(notifications, notificationBusiness);




                    await Common.UpdateEventLogsNew("USER REGISTRATION SUCCESSFUL", reqHeader, controllerURL, item, memberItem, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO REGISTER USER";
                    rm.name = StatusName.invalid;
                    rm.data = memberItem;
                    await Common.UpdateEventLogsNew("UNABLE TO REGISTER USER", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
                rm.data = ex.Message.ToString();
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
        ///         "roleID": 1000
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
                    rm.data = user;
                    await Common.UpdateEventLogsNew("FETCH USER - NO CONTENT", reqHeader, controllerURL, ItemData, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
        ///           "roleID": 1000
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
                    rm.data = user;
                    //await Common.UpdateEventLogsNew("FETCHED USER DATA - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
        ///         "roleID": 1000
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Login - SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response>
        [HttpPost, Route("User/Login")]
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

                    if (returnData.IsActive)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "LOGIN DATA";
                        rm.name = StatusName.ok;
                        rm.data = returnData;

                    }
                    else {
                        rm.statusCode = StatusCodes.ERROR;
                        rm.message = "User Inactive.Please contact administrator";
                        rm.name = StatusName.invalidCred;
                        rm.data = "User Inactive.Please contact administrator";

                    }


                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("WebLogIn - SUCCESSFULLY", reqHeader, controllerURL, itemData, returnData, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "Invalid EmailID or Password!";
                    rm.name = StatusName.invalidCred;
                    rm.data = returnData;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Invalid EmailID or Password!", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalidCred;
                rm.data = ex.Message.ToString();
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
                    rm.data = user;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO RESET PASSWORD", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
                List<EmailUserHeader> getEmailUserHeader = notificationBusiness.GetUserDetailsForActivation(itemData.emailID);

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

                mailbody = mailbody.Replace("{{name}}", getEmailUserHeader.Count == 0 ? "User" : getEmailUserHeader[0].UserName.ToString());
                mailbody = mailbody.Replace("{{userId}}", getEmailUserHeader.Count == 0 ? "1000" : getEmailUserHeader[0].UserID.ToString());


                notifications.EmailBody = mailbody;
                var emailResult = await EmailNotification.SendEmailCommon(notifications, notificationBusiness);
                if (emailResult ==true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THE EMAIL HAS BEEN SENT SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = emailResult;
                    //await Common.UpdateEventLogsNew("THE EMAIL HAS BEEN SENT SUCCESSFULLY", reqHeader, controllerURL, EmailID, user, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = emailResult;
                    //await Common.UpdateEventLogsNew("EMAIL HAS - NO CONTENT", reqHeader, controllerURL, EmailID, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
            string mailbody = string.Empty;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<EmailUserHeader> getEmailUserHeader = notificationBusiness.GetUserDetails(itemData.emailID);
                if (getEmailUserHeader.Count > 0)
                {
                    EmailNotificationTemplate emailNotificationTemplate = notificationBusiness.GetEmailNotificationTemplate(Convert.ToInt64(NotificationTemplateType.ForgotPassword));

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

                    mailbody = mailbody.Replace("{{name}}", getEmailUserHeader.Count == 0 ? "User" : getEmailUserHeader[0].UserName.ToString());
                    mailbody = mailbody.Replace("{{userId}}", getEmailUserHeader.Count == 0 ? "1000" : getEmailUserHeader[0].UserID.ToString());

                    notifications.EmailBody = mailbody;
                    var user = await EmailNotification.SendEmailCommon(notifications, notificationBusiness);
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
                        rm.data = user;
                        //await Common.UpdateEventLogsNew("EMAIL HAS - NO CONTENT", reqHeader, controllerURL, EmailID, null, rm.message, this.eventLogBusiness);
                    }
                }
                else
                {
                    rm.statusCode = 200;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.ok;
                    rm.data = getEmailUserHeader;
                    //await Common.UpdateEventLogsNew("EMAIL HAS - NO CONTENT", reqHeader, controllerURL, EmailID, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
        public IActionResult ListAllProducts()
        {
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
                    rm.data = items;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
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
                    rm.data = items;
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
                rm.data = ex.Message.ToString();
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
        public IActionResult GetVendorDetails(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            Member item = new Member();
            try
            {
                rm = new ResponseMessage();
                item = memberBusiness.GetMember(itemData.userID);
                if (item != null)
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
                rm.data = ex.Message.ToString();
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
        //        rm.data = ex.Message.ToString();
        //    }
        //    return Ok(rm);

        /// <summary>
        /// Get Categories List By VendorID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///         "VendorID": 1060
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///      "statusCode": 200,
        ///      "name": "SUCCESS_OK",
        ///      "message": "FETCH PRODUCT LIST",
        ///      "data": [
        ///        {
        ///          "productID": 2436,
        ///          "vendorID": 1060,
        ///          "productName": "jeans shirt",
        ///          "category": 1392,
        ///          "size": "M, L",
        ///          "color": "blue",
        ///          "categoryName": "Men",
        ///          "isActive": true,
        ///          "stockQty": 2,
        ///          "hsnCode": "t5678",
        ///          "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1727160529988"
        ///        }
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH CATEGORIES BY VENDORID!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("Product/list")]
        [MapToApiVersion("1.0")]
        public IActionResult List(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<ProductMasterByVendor> items = this.webAdminBusiness.GetProducts(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = items;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Get Seller List.
        /// </summary>
        /// <remarks>
        /// Sample Response JSON:
        ///
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SELLER LIST HAS BEEN SUCCESSFULY FETCHED!",
        ///       "data": [
        ///         {
        ///           "userID": 1044,
        ///           "logo": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1722947813107",
        ///           "appName": "dddd",
        ///           "regDate": "2023-10-19T02:38:24.68",
        ///           "name": "kusu ku  suma",
        ///           "city": "Kondapur",
        ///           "totalOrders": 7,
        ///           "contactNo": "9701167951"
        ///         }]
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the Seller List</response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("Seller/List")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSellerList()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<SellerList> items = this.webAdminBusiness.GetSellerList();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SELLER LIST HAS BEEN SUCCESSFULY FETCHED!";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    await Common.UpdateEventLogsNew("SELLER LIST HAS BEEN SUCCESSFULY FETCHED", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO FETCHED SELLER LIST";
                    rm.name = StatusName.invalid;
                    rm.data = items;
                    await Common.UpdateEventLogsNew("UNABLE TO FETCHED SELLER LIST", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("SELLER LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get Orders Seller List.
        /// </summary>
        /// <remarks>
        /// Sample Response JSON:
        ///
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SELLER LIST HAS BEEN SUCCESSFULY FETCHED!",
        ///       "data": [
        ///             {
        ///               "orderID": 2371,
        ///               "orderNo": "OD10602503017",
        ///               "vendorID": 1060,
        ///               "appName": "RK",
        ///               "orderDate": "0001-01-01T00:00:00",
        ///               "price": 918.86,
        ///               "status": "Awaiting Payment",
        ///               "paymentMode": "ONLINE",
        ///               "settlementStatus": "Processed"
        ///             },
        ///             {
        ///               "orderID": 2370,
        ///               "orderNo": "OD10602503016",
        ///               "vendorID": 1060,
        ///               "appName": "RK",
        ///               "orderDate": "0001-01-01T00:00:00",
        ///               "price": 9047.46,
        ///               "status": "Order Placed",
        ///               "paymentMode": "CASH ON DELIVERY",
        ///               "settlementStatus": "Processed"
        ///             }
        ///            ]
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the Seller List</response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("Seller/OrderList")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetSellerOrderList()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<SellerOrderList> items = this.webAdminBusiness.GetSellerOrderList();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SELLER LIST HAS BEEN SUCCESSFULY FETCHED!";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    await Common.UpdateEventLogsNew("SELLER LIST HAS BEEN SUCCESSFULY FETCHED", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO FETCHED SELLER LIST";
                    rm.name = StatusName.invalid;
                    rm.data = items;
                    await Common.UpdateEventLogsNew("UNABLE TO FETCHED SELLER LIST", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("SELLER LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Update Settlement Status
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "orderID": 1452,
        ///       "status": false
        ///     }
        ///     
        /// Sample Response JSON:
        ///
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SETTLEMENT STATUS HAS BEEN SUCCESSFULY UPDATED!",
        ///       "data": true
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Update Settlement Status</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("Seller/SettlementStatusUpdate")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SellerSettlement(ParamSettlementStatus itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.webAdminBusiness.SettlementStatusUpdate(itemData.OrderID, itemData.Status);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SETTLEMENT STATUS HAS BEEN SUCCESSFULY UPDATED!";
                    rm.name = StatusName.ok;
                    rm.data = result;

                    await Common.UpdateEventLogsNew("SETTLEMENT STATUS HAS BEEN SUCCESSFULY UPDATED", reqHeader, controllerURL, null, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO FETCHED SELLER LIST";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    await Common.UpdateEventLogsNew("UNABLE TO UPDATE SETTLEMENT STATUS", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("SETTLEMENT STATUS - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Management Dashboard Summary
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH MANAGEMENT DASHBOARD SUMMARY LIST!",
        ///       "data": [
        ///         {
        ///           "TotalRevenue": "2344.02",
        ///           "Percent": "10%"
        ///         },
        ///         {
        ///           "TotalVendors": "119.00",
        ///           "Percent": "5%"
        ///         },
        ///         {
        ///         "TotalCustomers": "554.00",
        ///           "Percent": "2%"
        ///         },
        ///         {
        ///         "TotalProducts": "549.00",
        ///           "Percent": "4%"
        ///         },
        ///         {
        ///         "TotalOrders": "1006.00",
        ///           "Percent": "5%"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Management Dashboard Summary</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("Management/dashboard/summary")]
        [MapToApiVersion("1.0")]
        public IActionResult ManagementDashboardSummary(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

                var result = this.adminDashboardBusiness.ManagementDashboardSummary(itemData.StartDate, itemData.EndDate);
                if (result.Any() != null)
                {
                    foreach (var summary in result)
                    {
                        Dictionary<string, object> itemlist = new Dictionary<string, object>();
                        itemlist.Add(summary.MetricName, summary.MetricValue);
                        itemlist.Add("Percent", summary.MetricPercentageValue);
                        dataList.Add(itemlist);
                    }
                }
                if (dataList != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MANAGEMENT DASHBOARD SUMMARY LIST!";
                    rm.name = StatusName.ok;
                    rm.data = dataList;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = dataList;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Top Products
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DASHBOARD TOP PRODUCTS LIST!",
        ///       "data": [
        ///         {
        ///           "productName": "Casual shirtt",
        ///           "totalSales": 301
        ///         },
        ///         {
        ///           "productName": "shirts ",
        ///           "totalSales": 277
        ///         },
        ///         {
        ///         "productName": " shirt with full sleeves ",
        ///           "totalSales": 68
        ///         },
        ///         {
        ///         "productName": "swet t shirt",
        ///           "totalSales": 56
        ///         },
        ///         {
        ///         "productName": " swet t shirt",
        ///           "totalSales": 41
        ///         },
        ///         {
        ///         "productName": "ankle fit jeans",
        ///           "totalSales": 37
        ///         },
        ///         {
        ///         "productName": "ankle length",
        ///           "totalSales": 34
        ///         },
        ///         {
        ///         "productName": "Cotton Shirt",
        ///           "totalSales": 33
        ///         },
        ///         {
        ///         "productName": "I AM BACK Men's Casual Wear Shirt ",
        ///           "totalSales": 28
        ///         },
        ///         {
        ///         "productName": "juzz twill RFD short",
        ///           "totalSales": 28
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Top Products</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/topproducts")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardTopProducts(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardTopProducts(itemData.StartDate, itemData.EndDate);
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD TOP PRODUCTS LIST!";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Top Vendors
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DASHBOARD TOP VENDORS LIST!",
        ///       "data": [
        ///         {
        ///           "vendorName": "RK",
        ///           "totalSales": 1154
        ///         },
        ///         {
        ///           "vendorName": "Beast",
        ///           "totalSales": 344
        ///         },
        ///         {
        ///         "vendorName": "Beard Bro",
        ///           "totalSales": 179
        ///         },
        ///         {
        ///         "vendorName": "Tunix",
        ///           "totalSales": 129
        ///         },
        ///         {
        ///         "vendorName": "STYLE LEAD",
        ///           "totalSales": 42
        ///         },
        ///         {
        ///         "vendorName": "Appify",
        ///           "totalSales": 25
        ///         },
        ///         {
        ///         "vendorName": "asds१",
        ///           "totalSales": 13
        ///         },
        ///         {
        ///         "vendorName": "appi",
        ///           "totalSales": 7
        ///         },
        ///         {
        ///         "vendorName": "test",
        ///           "totalSales": 1
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Top Vandors</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/topvendors")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardTopVendors(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardTopVendors(itemData.StartDate, itemData.EndDate);
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD TOP VENDORS LIST!";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Top Orders By City
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DASHBOARD TOP ORDERS BY CITY LIST!",
        ///       "data": [
        ///         {
        ///           "cityName": "Cyberabad",
        ///           "totalSales": 1154
        ///         },
        ///         {
        ///           "cityName": "Cuddalore",
        ///           "totalSales": 346
        ///         },
        ///         {
        ///         "cityName": "string",
        ///           "totalSales": 179
        ///         },
        ///         {
        ///         "cityName": "Kondapur",
        ///           "totalSales": 142
        ///         },
        ///         {
        ///         "cityName": "Karaikal",
        ///           "totalSales": 71
        ///         },
        ///         {
        ///         "cityName": "Hyderabad",
        ///           "totalSales": 55
        ///         },
        ///         {
        ///         "cityName": "Mugaiyur",
        ///           "totalSales": 54
        ///         },
        ///         {
        ///         "cityName": "Coimbatore",
        ///           "totalSales": 17
        ///         },
        ///         {
        ///         "cityName": "Sankarankovil",
        ///           "totalSales": 8
        ///         },
        ///         {
        ///         "cityName": "Madurai",
        ///           "totalSales": 6
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Top Orders By City</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/topordersbycity")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardTopOrdersByCity(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardTopOrdersByCity(itemData.StartDate, itemData.EndDate);
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD TOP ORDERS BY CITY LIST!";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Orders And Delivery Charges
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///         {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH DASHBOARD ORDER AND DELIVERY CHARGES LIST!",
        ///           "data": [
        ///             {
        ///               "vendorID": 1044,
        ///               "appName": "asds१",
        ///               "totalPrice": 72900,
        ///               "totalDeliveryCharges": 1506.57,
        ///               "grandTotal": 74406.57
        ///             },
        ///             {
        ///               "vendorID": 1058,
        ///               "appName": "Beard Bro",
        ///               "totalPrice": 168370,
        ///               "totalDeliveryCharges": 5865.4,
        ///               "grandTotal": 174235.4
        ///         },
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Orders and Delivery Charges</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/orderdeliverycharges")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardOrderDeliveryCharges(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardOrderDeliveryCharges(itemData.StartDate, itemData.EndDate);
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD ORDER AND DELIVERY CHARGES LIST!";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Monthly Sales
        /// </summary>
        /// <remarks>
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DASHBOARD MONTHLY SALES!",
        ///       "data": [
        ///         {
        ///           "name": "January",
        ///           "totalSales": 2
        ///         },
        ///         {
        ///           "name": "February",
        ///           "totalSales": 35
        ///         },
        ///         {
        ///         "name": "March",
        ///           "totalSales": 9
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Monthly Sales</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/monthlysales")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardMonthlySales()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardMonthlySales();
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD MONTHLY SALES!";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard On-Board Vendors
        /// </summary>
        /// <remarks>
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DASHBOARD MONTHLY ON BOARD VENDORS!",
        ///       "data": [
        ///         {
        ///           "name": "January",
        ///           "totalVendors": 3
        ///         },
        ///         {
        ///           "name": "February",
        ///           "totalVendors": 39
        ///         },
        ///         {
        ///         "name": "March",
        ///           "totalVendors": 8
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Monthly Sales</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/onboardvendors")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardOnBoardVendors()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardOnBoardVendors();
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD MONTHLY ON BOARD VENDORS!";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Total Revenue
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DASHBOARD TOTAL REVENUE LIST!",
        ///       "data": [
        ///         {
        ///           "vendorID": 1060,
        ///           "appName": "RK",
        ///           "totalCOD": 1112459.2,
        ///           "totalOnline": 90856.19,
        ///           "totalRevanue": 1203315.39
        ///         },
        ///         {
        ///           "vendorID": 1058,
        ///           "appName": "Beard Bro",
        ///           "totalCOD": 172782,
        ///           "totalOnline": 2180.1,
        ///           "totalRevanue": 174962.1
        ///         },
        ///         {
        ///         "vendorID": 1833,
        ///           "appName": "Tunix",
        ///           "totalCOD": 96663.74,
        ///           "totalOnline": 74088.71,
        ///           "totalRevanue": 170752.45
        ///         },
        ///         {
        ///         "vendorID": 1044,
        ///           "appName": "asds१",
        ///           "totalCOD": 74406.57,
        ///           "totalOnline": 0,
        ///           "totalRevanue": 74406.57
        ///         },
        ///         {
        ///         "vendorID": 1505,
        ///           "appName": "STYLE LEAD",
        ///           "totalCOD": 58565.18,
        ///           "totalOnline": 3492.26,
        ///           "totalRevanue": 62057.44
        ///         },
        ///         {
        ///         "vendorID": 1684,
        ///           "appName": "Beast",
        ///           "totalCOD": 23680.98,
        ///           "totalOnline": 1435.8,
        ///           "totalRevanue": 25116.78
        ///         },
        ///         {
        ///         "vendorID": 1804,
        ///           "appName": "Appify",
        ///           "totalCOD": 17759.94,
        ///           "totalOnline": 883.55,
        ///           "totalRevanue": 18643.49
        ///         },
        ///         {
        ///         "vendorID": 1937,
        ///           "appName": "appi",
        ///           "totalCOD": 4481.77,
        ///           "totalOnline": 0,
        ///           "totalRevanue": 4481.77
        ///         },
        ///         {
        ///         "vendorID": 1810,
        ///           "appName": "test",
        ///           "totalCOD": 1315.68,
        ///           "totalOnline": 0,
        ///           "totalRevanue": 1315.68
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Total Revenue</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/totalrevenue")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardTotalRevenue(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardTotalRevenue(itemData.StartDate, itemData.EndDate);
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD TOTAL REVENUE LIST!";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Order Status
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH DASHBOARD TOTAL REVENUE LIST!",
        ///       "data": [
        ///         {
        ///           "type": "Delivered",
        ///           "total": 2344.02
        ///         },
        ///         {
        ///           "type": "Pending",
        ///           "total": 0
        ///         },
        ///         {
        ///         "type": "Returned",
        ///           "total": 916.82
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Order Status</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/orderstatus")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardOrderStatus(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardOrderStatus(itemData.StartDate, itemData.EndDate);
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD ORDER STATUS";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Operations Dashboard Summary
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH OPERATION DASHBOARD SUMMARY!",
        ///       "data": [
        ///         {
        ///           "TotalRevenue": "2344.02",
        ///           "Percent": "10%"
        ///         },
        ///         {
        ///           "TotalOrderProcessed": "2",
        ///           "Percent": "5%"
        ///         },
        ///         {
        ///         "OrderFullfillRate": "2",
        ///           "Percent": "2%"
        ///         },
        ///         {
        ///         "AvgProcessingTime": "110",
        ///           "Percent": "4%"
        ///         },
        ///         {
        ///         "PendingShipping": "277",
        ///           "Percent": "5%"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Operation Dashboard Summary</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("Operations/dashboard/summary")]
        [MapToApiVersion("1.0")]
        public IActionResult OperationsDashboardSummary(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

                var result = this.adminDashboardBusiness.OperationsDashboardSummary(itemData.StartDate, itemData.EndDate);
                if (result.Any() != null)
                {
                    foreach (var summary in result)
                    {
                        Dictionary<string, object> itemlist = new Dictionary<string, object>();
                        itemlist.Add(summary.MetricName, summary.MetricValue);
                        itemlist.Add("Percent", summary.MetricPercentageValue);
                        dataList.Add(itemlist);
                    }
                }
                if (dataList != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH OPERATION DASHBOARD SUMMARY!";
                    rm.name = StatusName.ok;
                    rm.data = dataList;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = dataList;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Dashboard Top Products Vendors
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "startDate": "2022-03-06T19:09:38.018Z",
        ///       "endDate": "2025-03-06T19:09:38.018Z"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///         {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH DASHBOARD TOP PRODUCTS VENDORS",
        ///           "data": [
        ///             {
        ///               "productID": 1031,
        ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1697709386207.jpg",
        ///               "productName": "Multicolor Pure Georgette Gown with Fancy Lace Dup",
        ///               "stockRemaining": 2
        ///             },
        ///             {
        ///               "productID": 1061,
        ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1698824046743.jpg",
        ///               "productName": "Embroidered Plane Green Kurti",
        ///               "stockRemaining": 20
        ///          },
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">Dashboard Top Products Vendors</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        /// 
        [HttpPost, Route("dashboard/topvendorsproducts")]
        [MapToApiVersion("1.0")]
        public IActionResult DashboardTopVendorsProducts(ParamFilter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = this.adminDashboardBusiness.DashboardTopVendorsProducts(itemData.StartDate, itemData.EndDate);
                if (result.Any())
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DASHBOARD TOP PRODUCTS VENDORS";
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
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
    }
}
