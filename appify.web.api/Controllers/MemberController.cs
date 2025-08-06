/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.audit.service;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Utilities;
using Razorpay.Api;
using static appify.models.NotificationType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    public class MemberController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IMemberBusiness memberBusiness;
        private readonly IMemberReturnPolicyBusiness memberReturnPolicyBusiness;
        private readonly IMemberAppSettingBusiness memberAppSettingBusiness;
        private readonly IMemberThemeBusiness memberThemeBusiness;
        private readonly IMemberKYCBusiness memberKYCBusiness;
        private readonly IMemberContactBusiness memberContactBusiness;
        private readonly INotificationBusiness notificationBusiness;
        private readonly IWebHostEnvironment env;
        private readonly IAuditService auditService;
        private ResponseMessage rm;
        private readonly IOrderBusiness orderBusiness;

        public MemberController(IConfiguration configuration,
                                IMemberBusiness iResultData,
                                IMemberReturnPolicyBusiness memberReturnPolicyBusiness,
                                IMemberAppSettingBusiness memberAppSettingBusiness,
                                IMemberThemeBusiness memberThemeBusiness,
                                IMemberKYCBusiness memberKYCBusiness,
                                IMemberContactBusiness memberContactBusiness, 
                                IEventLogBusiness eventLogBusiness, 
                                INotificationBusiness IResultData, 
                                IOrderBusiness orderBusiness,
                                IWebHostEnvironment env, IAuditService auditService)
        {
            this.configuration = configuration;
            this.memberBusiness = iResultData;
            this.memberReturnPolicyBusiness = memberReturnPolicyBusiness;
            this.memberAppSettingBusiness = memberAppSettingBusiness;
            this.memberThemeBusiness = memberThemeBusiness;
            this.memberKYCBusiness = memberKYCBusiness;
            this.memberContactBusiness = memberContactBusiness;
            this.eventLogBusiness = eventLogBusiness;
            this.notificationBusiness = IResultData;
            this.orderBusiness = orderBusiness;
            this.auditService = auditService;
            this.env = env;
        }
        /// <summary>
        /// Get a Member List
        /// </summary>
        /// <remarks>   
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH MEMBER LIST",
        ///       "data": [
        ///         {
        ///           "userID": 1008,
        ///           "emailID": "kvr210885@gmail.com",
        ///           "mobileNo": "8682944609",
        ///           "password": "Entombed@25",
        ///           "firstName": "Kalyan",
        ///           "lastName": "KVR",
        ///           "memberType": 1001,
        ///           "otp": "604174",
        ///           "isOTPSent": true,
        ///           "otpSentDate": "2023-09-13T12:35:28.417",
        ///           "isResendOTP": false,
        ///           "isOTPVerified": true,
        ///           "isEmailVerified": true,
        ///           "isActive": true,
        ///           "createdOn": "2023-09-13T00:05:29.93",
        ///           "profilePhoto": "image_cropper_1694677575946.jpg",
        ///           "token": "¦iät\u0006\tOŸ\u0005c `\u0015\u001f",
        ///           "platformType": 0,
        ///           "parentID": 1004,
        ///           "isRegisteredByMobile": true,
        ///           "isOnlinePaymentEnabled": true,
        ///           "isEnterprise": null,
        ///           "isEcommerce": null,
        ///           "isWelcomeEmail": null
        ///         }]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER LIST SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET: api/<MemberController>
        [HttpGet]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetAllMembers()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var items = this.memberBusiness.GetAllMembers();

                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok));

                    await Common.UpdateEventLogsNew("GET MEMBER LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                    await Common.UpdateEventLogsNew("GET MEMBER LIST - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                await Common.UpdateEventLogsNew("GET MEMBER LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }

            return Ok(rm);
        }
        /// <summary>
        /// Get a Member
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1860
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH MEMBER",
        ///       "data": {
        ///         "userID": 1860,
        ///         "emailID": "kalyan@appify.ai",
        ///         "mobileNo": "6303467186",
        ///         "password": "Appify@123",
        ///         "firstName": "sai",
        ///         "lastName": "Chow",
        ///         "memberType": 1001,
        ///         "otp": "637551",
        ///         "isOTPSent": true,
        ///         "otpSentDate": "2023-12-02T19:19:36.847",
        ///         "isResendOTP": false,
        ///         "isOTPVerified": true,
        ///         "isEmailVerified": false,
        ///         "isActive": true,
        ///         "createdOn": "2024-05-24T16:01:35.71",
        ///         "profilePhoto": "",
        ///         "token": "fw4Fw_AVjEOslW0-Ltv_bX:APA91bEw-ehnGhWHPpgxmu-CS-E9qlBtdQQHnj-xysD_h-///C  gh VQ_hSIMU6G3ceDjUJr_Hj25iHbgzhxd12fxd70791UCm6DI8fmTpAZ1Thg4xFHbp8gMh3cKiE6H_M9YbLxhBbjPV_3X",
        ///         "platformType": 0,
        ///         "parentID": 1833,
        ///         "isRegisteredByMobile": true,
        ///         "isOnlinePaymentEnabled": true,
        ///         "isEnterprise": false,
        ///         "isEcommerce": false,
        ///         "isWelcomeEmail": false
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpGet("{userID}")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetMember(long userID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var member = this.memberBusiness.GetMember(Convert.ToInt64(userID));
                if (member != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER";
                    rm.name = StatusName.ok;
                    rm.data = member;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER SUCCESSFULLY", reqHeader, controllerURL, userID, member, StatusName.ok));

                    await Common.UpdateEventLogsNew("FETCH MEMBER SUCCESSFULLY", reqHeader, controllerURL, userID, member, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER - NO CONTENT", reqHeader, controllerURL, userID, null, rm.message));
                    await Common.UpdateEventLogsNew("FETCH MEMBER - NO CONTENT", reqHeader, controllerURL, userID, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER - ERROR", reqHeader, controllerURL, userID, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH MEMBER - ERROR", reqHeader, controllerURL, userID, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }


        /// <summary>
        /// Get a MemberID by Store URL
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///        "storeUrl": "makxoutfit"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///         "statusCode": 200,
        ///         "name": "SUCCESS_OK",
        ///         "message": "FETCH MEMBER ID",v
        ///         "data": 1064
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH MEMBER ID BY STORE URL</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost,Route("getVendorByStoreUrl")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetMemberByStoreRUL(ParamVendorStoreUrl itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                //TokenValidator.IsValidToken(Request, configuration, env);
                var memberID = this.memberAppSettingBusiness.GetMemberIdByAppName(itemData.StoreUrl);
                if (memberID != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER ID BY STORE URL";
                    rm.name = StatusName.ok;
                    rm.data = memberID;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER SUCCESSFULLY", reqHeader, controllerURL, userID, member, StatusName.ok));

                    //await Common.UpdateEventLogsNew("FETCH MEMBER ID SUCCESSFULLY", reqHeader, controllerURL, userID, member, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER - NO CONTENT", reqHeader, controllerURL, userID, null, rm.message));
                    //await Common.UpdateEventLogsNew("FETCH MEMBER - NO CONTENT", reqHeader, controllerURL, userID, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER - ERROR", reqHeader, controllerURL, userID, null, rm.message));
                //await Common.UpdateEventLogsNew("FETCH MEMBER - ERROR", reqHeader, controllerURL, userID, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }










        /// <summary>
        /// Add/Update Member.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : For a new / update Member
        /// 
        ///     {
        ///       "userID": 1060,
        ///       "emailID": "kvrkalyan1985@gmail.com",
        ///       "mobileNo": "6382014003",
        ///       "password": "Appify@123",
        ///       "firstName": "I AM",
        ///       "lastName": "BACK",
        ///       "memberType": 1000,
        ///       "otp": "731885",
        ///       "isOTPSent": true,
        ///       "otpSentDate": "2023-11-01 13:20:59.313",
        ///       "isResendOTP": false,
        ///       "isOTPVerified": true,
        ///       "isEmailVerified": false,
        ///       "isActive": true,
        ///       "createdOn": "2023-11-01 00:51:01.600",
        ///       "profilePhoto": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1721297240519",
        ///              "token": "eAv9G_F7TwqiPU0e6wp1rR:APA91bEfdEOHKbaIx2yv8YPSGKsUmbnyWoMW5fhEMdoAwCYnGQEWWlmEvMGNWNUNxEc2UeiXYGYO0JdI_GnSSAsl6BfsLl51Pk8YVGcGQONoUsGhSzyIEHutcUkb7rWyA4Gp0hDWrTnn",
        ///       "platformType": 3994,
        ///       "parentID": 0,
        ///       "isRegisteredByMobile": true,
        ///       "isOnlinePaymentEnabled": true,
        ///       "isEnterprise": false,
        ///       "isEcommerce": false,
        ///       "IsWelcomeEmail":false
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <param name="discountHeader"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 


        // POST api/<MemberController>
        [HttpPost, Route("Register")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Register(appify.models.Member item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            var fullName = item.FirstName + ' ' + item.LastName;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            var eventType = item.UserID > 0 ? "Vendor Updated" : "New Vendor Created";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                Int64 UserID = item.UserID;
                rm = new ResponseMessage();
                var memberItem = this.memberBusiness.RegisterMember(item);
                if (memberItem.UserID > 0)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER REGISTRATION SUCCESSFUL!";
                    rm.name = StatusName.ok;
                    rm.data = memberItem;
                    OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(0);
                    if (orderUpdateDetail.SkipNo != item.MobileNo)
                    {
                        if (UserID > 0 && item.MemberType == 1000 && item.IsWelcomeEmail == false) //// Welcome email to Vendor
                        {
                            if (orderUpdateDetail.IsEmail == true)
                            {
                                EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.SuccessfulSignupVendor), memberItem.UserID, 0, this.notificationBusiness);
                                if (orderUpdateDetail.IsEmailOpps == true)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.SuccessfulSignupOpps), memberItem.UserID, 0, this.notificationBusiness);
                                }

                            }
                            if (orderUpdateDetail.IsSMS == true)
                            {
                                SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.SuccessfulSignup), memberItem.UserID, 0, "<first_name>", this.notificationBusiness);
                            }
                            if (orderUpdateDetail.IsPush == true)
                            {
                                PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.SuccessfulSignup), 0, memberItem.UserID, 0, "<first_name>", this.notificationBusiness);
                            }
                            this.memberBusiness.UpdateWelcomeEmail(memberItem.UserID, true);
                        }

                        if (UserID > 0 && item.MemberType == 1001 && item.IsWelcomeEmail == false) //// Welcome email to Customer
                        {
                            if (orderUpdateDetail.IsEmail == true)
                            {
                                EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.SuccessfulSignupCustomer), memberItem.UserID, 0, this.notificationBusiness);
                            }
                            if (orderUpdateDetail.IsSMS == true)
                            {
                                SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.SuccessfulSignupCustomer), memberItem.UserID, 0, "<first_name>", this.notificationBusiness);
                            }
                            if (orderUpdateDetail.IsPush == true)
                            {
                                PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.SuccessfulSignup), 0, memberItem.UserID, 0, "<first_name>", this.notificationBusiness);
                            }
                            this.memberBusiness.UpdateWelcomeEmail(memberItem.UserID, true);
                        }

                        //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                        //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER REGISTRATION SUCCESSFUL", reqHeader, controllerURL, item, memberItem, StatusName.ok));
                        await Common.UpdateEventLogsNew("MEMBER REGISTRATION SUCCESSFUL", reqHeader, controllerURL, item, memberItem, StatusName.ok, this.eventLogBusiness);

                        if(item.MemberType == 1000)
                        {
                            await auditService.LogAsync(EntityType.Vendor, item.UserID, eventType, item.UserID.ToString(), AppName, sourceIPAddress, item);
                            if(eventType == "New Vendor Created")
                                await auditService.LogAsync(EntityType.Vendor, item.UserID, "New AppSetting Created", item.UserID.ToString(), AppName, sourceIPAddress, item);
                        }

                    }
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO REGISTER MEMBER";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO REGISTER MEMBER", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("UNABLE TO REGISTER MEMBER", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER REGISTRATION - ERROR", reqHeader, controllerURL, item, null, rm.message));
                await Common.UpdateEventLogsNew("MEMBER REGISTRATION - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// GenerateOTP.
        /// </summary>
        /// <remarks>
        /// Sample Response:
        /// NOTE : GenerateOTP.
        ///
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "OTP HAS BEEN SUCCESSFULLY GENERATED",
        ///       "data": "yryl1A"
        ///     }
        ///
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response>

        [HttpPost, Route("generateotp")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GenerateOTP(string MobileNo)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                // dynamic data = jsondata;
                rm = new ResponseMessage();
                var OTPSecretKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppifyOTPKey:SecretKey").Value;
                string OTPValue = utility.Common.GenerateOTP(OTPSecretKey);

                ////Checking Member is Already Exits or Not
                CheckOTPSent getotpresult = this.memberBusiness.GetOTPSent(MobileNo);
                if (getotpresult == null)
                {
                    var result = SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OTP), 0, 0, MobileNo, this.notificationBusiness, OTPValue);
                    this.RegisterMobileOTP(MobileNo, true, false);
                }
                else if (getotpresult != null)
                {
                    //var result = SMSNotification.SendSMSNotification(MobileNo, OTPValue, this.notificationBusiness);
                    var result = SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OTP), 0, 0, MobileNo, this.notificationBusiness, OTPValue);
                    this.RegisterMobileOTP(MobileNo, false, true);
                }
                //TODO: to implement the above dashboard information
                if (OTPValue != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "OTP HAS BEEN SUCCESSFULLY GENERATED & SENT";
                    rm.name = StatusName.ok;
                    rm.data = OTPValue;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    await Common.UpdateEventLogsNew("OTP HAS BEEN SUCCESSFULLY GENERATED & SENT", reqHeader, controllerURL, MobileNo, OTPValue, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO GENERATE OTP";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    await Common.UpdateEventLogsNew("UNABLE TO GENERATE OTP", reqHeader, controllerURL, MobileNo, rm.message, StatusName.ok, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("UNABLE TO GENERATE OTP", reqHeader, controllerURL, MobileNo, rm.message, StatusName.ok, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        private bool RegisterMobileOTP(string MobileNo, bool Issent, bool IsResent)
        {
            var result = false;
            try
            {
                RegisterOTP mobileotp = new RegisterOTP
                {
                    MobileNo = MobileNo,
                    IsSent = Issent,
                    IsResent = IsResent,
                    SentOn = DateTime.Now,
                };

                var memberItem = this.memberBusiness.RegisterMobileOTP(mobileotp);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }

        /// <summary>
        /// De-Active A Member
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1860
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MEMBER DE-ACTIVATED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("DeActivateMember")]
        [MapToApiVersion("1.0")]
        //[Authorize]
        public IActionResult DeactivateMember(ParamMemberUserID itemData)
        {
            //dynamic data = jsondata;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                var result = this.memberBusiness.RemoveMember(itemData.userID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DE-ACTIVATED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER DE-ACTIVATED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DE-ACTIVATE MEMBER";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO DE-ACTIVATE MEMBER", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER DE-ACTIVATED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }
        /// <summary>
        /// Delete a Member
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "mobileNo": "6303467186",
        ///       "password": "Appify@123"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MEMBER DELETED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 

        [HttpPost, Route("DeleteMember")]
        [MapToApiVersion("1.0")]
        //[Authorize]
        public IActionResult DeleteMember(ParamDeactivateMember itemData)
        {
            //dynamic data = jsondata;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                var result = this.memberBusiness.RemoveMemberByMobileNo(itemData.mobileNo, itemData.password);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DELETED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER DELETED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DELETED MEMBER";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO DELETED MEMBER", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER DELETED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// Reset Member Password
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1860,
        ///       "password": "Appify@123"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">PASSWORD RESET SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("ResetPassword")]
        [MapToApiVersion("1.0")]

        public IActionResult ResetPassword(ParamMemberResetPassword itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                //dynamic data = jsondata;

                rm = new ResponseMessage();
                var member = this.memberBusiness.ResetPassword(itemData.userID, itemData.password);
                if (member != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PASSWORD RESET SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = member;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PASSWORD RESET SUCCESSFULLY", reqHeader, controllerURL, itemData, member, StatusName.ok));
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
        /// Check Member with emailid and mobile no
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "emailID": "rama@appi-fy.ai",
        ///       "mobileNo": "9959625612",
        ///       "memberType": 1000,
        ///       "vendorID": 0
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "MEMBER WITH SIMILAR MOBILE NO. EXISTS!",
        ///       "data": {
        ///         "userID": 1937,
        ///         "emailID": "rama@appi-fy.ai",
        ///         "mobileNo": "9959625612",
        ///         "password": "Appify@123",
        ///         "firstName": "appify",
        ///         "lastName": "kalyan",
        ///         "memberType": 1000,
        ///         "otp": "078862",
        ///         "isOTPSent": true,
        ///         "otpSentDate": "2024-09-19T16:40:11.967",
        ///         "isResendOTP": false,
        ///         "isOTPVerified": true,
        ///         "isEmailVerified": false,
        ///         "isActive": true,
        ///         "createdOn": "2024-09-19T16:40:12.643",
        ///         "profilePhoto": "",
        ///         "token": "CD9BF9EB-A940-4430-A7DA-D51B02CF4AD7",
        ///         "platformType": 0,
        ///         "parentID": 0,
        ///         "isRegisteredByMobile": true,
        ///         "isOnlinePaymentEnabled": true,
        ///         "isEnterprise": null,
        ///         "isEcommerce": null,
        ///         "isWelcomeEmail": null
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MEMBER WITH SIMILAR MOBILE NO. EXISTS </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("CheckMember")]
        public IActionResult CheckMember(ParamCheckMember itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            try
            {
                //dynamic data = jsondata;

                //var objData = new
                //{
                //    EmailID = data.emailID,
                //    MobileNo = data.mobileNo
                //};


                rm = new ResponseMessage();
                Member result = this.memberBusiness.IsMemberExist(itemData.emailID, itemData.mobileNo, itemData.memberType, itemData.vendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER WITH SIMILAR MOBILE NO. EXISTS!";
                    rm.name = StatusName.ok;

                    //var itemdata = this.memberBusiness.GetMember();

                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER WITH SIMILAR MOBILE NO. EXISTS", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DOES NOT EXIST!";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER DOES NOT EXIST", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER EXIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }



        /// <summary>
        /// Check Member with emailid and mobile no
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "emailID": "rama@appi-fy.ai",
        ///       "mobileNo": "9959625612",
        ///       "memberType": 1000,
        ///       "vendorID": 0
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "CUSTOMER WITH SIMILAR MOBILE NO. EXISTS!",
        ///       "data": {
        ///         "userID": 1937,
        ///         "emailID": "rama@appi-fy.ai",
        ///         "mobileNo": "9959625612",
        ///         "password": "Appify@123",
        ///         "firstName": "appify",
        ///         "lastName": "kalyan",
        ///         "memberType": 1000,
        ///         "otp": "078862",
        ///         "isOTPSent": true,
        ///         "otpSentDate": "2024-09-19T16:40:11.967",
        ///         "isResendOTP": false,
        ///         "isOTPVerified": true,
        ///         "isEmailVerified": false,
        ///         "isActive": true,
        ///         "createdOn": "2024-09-19T16:40:12.643",
        ///         "profilePhoto": "",
        ///         "token": "CD9BF9EB-A940-4430-A7DA-D51B02CF4AD7",
        ///         "platformType": 0,
        ///         "parentID": 0,
        ///         "isRegisteredByMobile": true,
        ///         "isOnlinePaymentEnabled": true,
        ///         "isEnterprise": null,
        ///         "isEcommerce": null,
        ///         "isWelcomeEmail": null
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">CUSTOMER WITH SIMILAR MOBILE NO. EXISTS </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("CheckCustomer")]
        public IActionResult CheckCustomer(ParamCheckMember itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            try
            {
                //dynamic data = jsondata;

                //var objData = new
                //{
                //    EmailID = data.emailID,
                //    MobileNo = data.mobileNo
                //};


                rm = new ResponseMessage();
                Member result = this.memberBusiness.IsCustomerExist(itemData.emailID, itemData.mobileNo, itemData.memberType, itemData.vendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER WITH SIMILAR MOBILE NO. EXISTS!";
                    rm.name = StatusName.ok;

                    //var itemdata = this.memberBusiness.GetMember();

                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("CUSTOMER WITH SIMILAR MOBILE NO. EXISTS", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DOES NOT EXIST!";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER DOES NOT EXIST", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER EXIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }





        /// <summary>
        /// Get Member Order Count
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1600
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "ORDERS COUNT",
        ///       "data": 2
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MemberOrderCount SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("OrdersCount")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> OrdersCount(ParamMemberUserID item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var count = this.memberBusiness.MemberOrderCount(item.userID);
                if (count > 0)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ORDERS COUNT";
                    rm.name = StatusName.ok;
                    rm.data = count;
                    await Common.UpdateEventLogsNew("MemberOrderCount SUCCESSFULLY", reqHeader, controllerURL, item, count, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = "NO CONTENT";
                    await Common.UpdateEventLogsNew("MemberOrderCount - NO CONTENT", reqHeader, controllerURL, item, count, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();

                await Common.UpdateEventLogsNew("MemberOrderCount - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        /// Get Member Order Count
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
        ///       "message": "VENDOR ORDERS COUNT",
        ///       "data": 2
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MemberOrderCount SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("VendorOrdersCount")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult VendorOrdersCount(ParamMemberUserID item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var count = this.memberBusiness.VendorOrderCount(item.userID);
                if (count > 0)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR ORDERS COUNT";
                    rm.name = StatusName.ok;
                    rm.data = count;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VendorOrderCount SUCCESSFULLY", reqHeader, controllerURL, item, count, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "VENDOR NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VendorOrderCount - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VendorOrderCount - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// Check Member Online Payment Status
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
        ///       "message": "ONLINE PAYMENT STATUS",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">ONLINE PAYMENT STATUS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("OnlinePaymentAllowed")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult OnlinePaymentStatus(ParamMemberUserID item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                bool isAllowed = this.memberBusiness.CheckMemberOnlinePaymentStatus(item.userID);

                rm.statusCode = StatusCodes.OK;
                rm.message = "ONLINE PAYMENT STATUS";
                rm.name = StatusName.ok;
                rm.data = isAllowed;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ONLINE PAYMENT STATUS SUCCESSFULLY", reqHeader, controllerURL, item, isAllowed, StatusName.ok));
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ONLINE PAYMENT STATUS - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// Check Member Online Payment Status
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
        ///       "message": "ONLINE PAYMENT STATUS",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">ONLINE PAYMENT STATUS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        // GET api/<MemberController>/5
        [HttpPost, Route("IsDeliveryEnabled")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult IsDeliveryEnabled(ParamMemberUserID item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                bool isAllowed = this.memberBusiness.CheckMemberDeliveryStatus(item.userID);

                rm.statusCode = StatusCodes.OK;
                rm.message = "DELIVERY STATUS";
                rm.name = StatusName.ok;
                rm.data = isAllowed;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DELIVERY STATUS SUCCESSFULLY", reqHeader, controllerURL, item, isAllowed, StatusName.ok));
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DELIVERY STATUS - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// Member Login
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///{
        ///  "emailID": "rama@appi-fy.ai",
        ///  "mobileNo": "9959625612",
        ///  "password": "Appify@123",
        ///  "parentID": 0
        ///}
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "MEMBER DATA",
        ///       "data": {
        ///         "userID": 1937,
        ///         "emailID": "rama@appi-fy.ai",
        ///         "mobileNo": "9959625612",
        ///         "password": "Appify@123",
        ///         "firstName": "appify",
        ///         "lastName": "kalyan",
        ///         "memberType": 1000,
        ///         "otp": "078862",
        ///         "isOTPSent": true,
        ///         "otpSentDate": "2024-09-19T16:40:11.967",
        ///         "isResendOTP": false,
        ///         "isOTPVerified": true,
        ///         "isEmailVerified": false,
        ///         "isActive": true,
        ///         "createdOn": "2024-09-19T16:40:12.643",
        ///         "profilePhoto": "",
        ///         "token": "CD9BF9EB-A940-4430-A7DA-D51B02CF4AD7",
        ///         "platformType": 0,
        ///         "parentID": 0,
        ///         "isRegisteredByMobile": true,
        ///         "isOnlinePaymentEnabled": false,
        ///         "isEnterprise": null,
        ///         "isEcommerce": null,
        ///         "isWelcomeEmail": null
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MemberLogIn - SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 

        [HttpPost, Route("SignIn")]
        [MapToApiVersion("1.0")]
        public IActionResult SignIn(ParamSignIn itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic loginParams = jsondata;

            try
            {
                //var objData = new
                //{
                //    EmailID = loginParams.emailID,
                //    MobileNo = loginParams.mobileNo,
                //    Password = loginParams.password
                //};

                rm = new ResponseMessage();
                var returnData = this.memberBusiness.MemberLogIn(itemData.emailID, itemData.mobileNo, itemData.password, itemData.parentID);
                if (returnData != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DATA";
                    rm.name = StatusName.ok;
                    rm.data = returnData;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MemberLogIn - SUCCESSFULLY", reqHeader, controllerURL, itemData, returnData, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "Invalid Mobile No or Password!";
                    rm.name = StatusName.invalidCred;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Invalid MobileNo or Password!", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalidCred;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MemberLogIn - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }
        /// <summary>
        /// Dashboard.
        /// </summary>
        /// <remarks>
        /// Sample Response:
        /// NOTE : Vendor Dashboard.
        ///
        ///     {
        ///       "userID": 1060
        ///     }
        ///
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response>
        [HttpPost, Route("dashboard")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult MemberDashboard(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                // dynamic data = jsondata;

                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                //var dashboard = this.memberBusiness.MemberDashboard(Convert.ToInt64(itemData.userID));

                //TODO: to implement the above dashboard information


                MemberDashboard dashboard = new MemberDashboard()
                {
                    AdROAS = 0,
                    AdTotalConversions = 0,
                    AdTotalInstalls = 0,
                    AdTotalSpends = 0,
                    TotalProducts = 0,
                    DeliveredOrders = 0,
                    PendingOrders = 0,
                    Products = new List<MemberDashboard.DashboardProducts>() {
                        new MemberDashboard.DashboardProducts() {
                            ProductName = "",
                            TotalOrders = 0
                        }
                    }

                };

                if (dashboard != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DASHBOARD INFORMATION!";
                    rm.name = StatusName.ok;
                    rm.data = dashboard;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DASHBOARD INFORMATION SUCCESSFULLY", reqHeader, controllerURL, itemData, dashboard, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO SEND DASHBOARD DATA";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO SEND DASHBOARD DATA", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("DASHBOARD - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        /// <summary>
        /// Vendor Dashboard.
        /// </summary>
        /// <remarks>
        /// Sample Response:
        /// NOTE : Vendor Dashboard.
        ///
        ///     {
        ///       "userID": 1060,
        ///       "dateFrom": "2024-01-01",
        ///       "dateTo": "2024-07-04"
        ///     }
        ///
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response>

        [HttpPost, Route("dashboardsummery")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult MemberDashboardSummery(ParamMemberDashboard itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                // dynamic data = jsondata;
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var dashboard = this.memberBusiness.MemberDashboard(Convert.ToInt64(itemData.userID), itemData.dateFrom, itemData.dateTo);

                //TODO: to implement the above dashboard information

                if (dashboard != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DASHBOARD INFORMATION!";
                    rm.name = StatusName.ok;
                    rm.data = dashboard;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MemberDashboardSummery INFORMATION SUCCESSFULLY", reqHeader, controllerURL, itemData, dashboard, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO SEND DASHBOARD DATA";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MemberDashboardSummery UNABLE TO SEND DASHBOARD DATA", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MemberDashboardSummery - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }
        /// <summary>
        /// Get Return Policy
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1044
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH RETURN-POLICY",
        ///       "data": {
        ///         "memberID": 1044,
        ///         "maxReturnDays": 0,
        ///         "isProductDamaged": false,
        ///         "isDeliveryDelay": false,
        ///         "isWrongSize": true,
        ///         "inCompatible": true,
        ///         "isQualityIssue": false,
        ///         "isDifferentProduct": false,
        ///         "isNotNeeded": true,
        ///         "isOthers": true,
        ///         "isImage": false,
        ///         "isVideo": false,
        ///         "remarks": "",
        ///         "isActive": false
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH RETURN-POLICY SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("rp/get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetReturnPolicy(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = memberReturnPolicyBusiness.GetItem(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH RETURN-POLICY";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH RETURN-POLICY SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH RETURN-POLICY - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH RETURN-POLICY - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Add/Update a Return Policy
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "memberID": 1044,
        ///         "maxReturnDays": 0,
        ///         "isProductDamaged": false,
        ///         "isDeliveryDelay": false,
        ///         "isWrongSize": true,
        ///         "inCompatible": true,
        ///         "isQualityIssue": false,
        ///         "isDifferentProduct": false,
        ///         "isNotNeeded": true,
        ///         "isOthers": true,
        ///         "isImage": false,
        ///         "isVideo": false,
        ///         "remarks": "",
        ///         "isActive": false
        ///       }
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "RETURN POLICY SAVED SUCCESSFULLY",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">RETURN POLICY SAVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("rp/save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult AddReturnPolicy(MemberReturnPolicy item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = memberReturnPolicyBusiness.Save(item);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "RETURN POLICY SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RETURN POLICY SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RETURN POLICY SAVED - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RETURN POLICY SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove a Policy
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1014
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">RETURN POLICY REMOVED </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("rp/remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult RemoveReturnPolicy(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = memberReturnPolicyBusiness.Remove(itemData.userID);

                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "RETURN POLICY REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RETURN POLICY REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RETURN POLICY - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RETURN POLICY - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }





        /// <summary>
        /// Get an App Setting
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1014
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH APP SETTINGS",
        ///       "data": {
        ///         "userID": 1014,
        ///         "appName": "Megha Store",
        ///         "appName1": null,
        ///         "appName2": null,
        ///         "shortDescription":"Short",
        ///         "description": "fashion dress 👗",
        ///         "logo": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1694761053957.jpg",
        ///         "playStoreID": "",
        ///         "appStoreID": "",
        ///         "appIcon": null,
        ///         "webAppURL": ""
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH APP SETTINGS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/get")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAppSetting(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            MemberAppSettingLite itemLite;

            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.GetMemberAppSetting(itemData.userID);

                if (item != null)
                {
                    itemLite = new MemberAppSettingLite
                    {
                        UserID = item.UserID,
                        AppName = item.AppName,
                        AppName1 = item.AppName1,
                        AppName2 = item.AppName2,
                        ShortDescription = item.ShortDescription,
                        Description = item.LongDescription,
                        Logo = item.AppLogo,
                        AppIcon = item.AppIcon,
                        PlayStoreID = item.AndroidBundleID,
                        AppStoreID = item.AppleAppID,
                        WebAppURL = item.WebAppURL
                    };

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH APP SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = itemLite;
                    await Common.UpdateEventLogsNew("FETCH APP SETTINGS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = "NO CONTENT";
                    await Common.UpdateEventLogsNew("FETCH APP SETTINGS - NO CONTENT", reqHeader, controllerURL, itemData, item, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("FETCH APP SETTINGS - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Add/Update App Settings
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1014,
        ///         "appName": "Megha Store",
        ///         "appName1": null,
        ///         "appName2": null,
        ///         "shortDescription": "Fashion",
        ///         "description": "fashion dress 👗",
        ///         "logo": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1694761053957.jpg",
        ///         "playStoreID": "",
        ///         "appStoreID": "",
        ///         "appIcon": null,
        ///         "webAppURL": ""
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "APP SETTINGS SAVED SUCCESSFULLY",
        ///       "data": {
        ///         "userID": 1014,
        ///         "appName": "Megha Store",
        ///         "appName1": null,
        ///         "appName2": null,
        ///         "shortDescription":"Short",
        ///         "description": "fashion dress 👗",
        ///         "logo": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1694761053957.jpg",
        ///         "playStoreID": "",
        ///         "appStoreID": "",
        ///         "appIcon": null,
        ///         "webAppURL": ""
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">APP SETTINGS SAVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/save")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddAppSetting(MemberAppSettingLite item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            var eventType = item.UserID > 0 ? "AppSetting Updated" : "New AppSetting Created";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                var itemData = new MemberAppSetting
                {
                    UserID = item.UserID,
                    AppName = item.AppName,
                    AppName1 = item.AppName1,
                    AppName2 = item.AppName2,
                    ShortDescription = item.ShortDescription,
                    LongDescription = item.Description,
                    AppLogo = item.Logo,
                    AppIcon = item.AppIcon,
                    AndroidBundleID = item.PlayStoreID,
                    AppleBundleID = item.AppStoreID,
                    WebAppURL = item.WebAppURL
                };

                rm = new ResponseMessage();
                var result = memberAppSettingBusiness.SaveMemberAppSetting(itemData);
                if (result)
                {
                    var returndata = memberAppSettingBusiness.GetMemberAppSetting(item.UserID);

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "APP SETTINGS SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = returndata;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED SUCCESSFULLY", reqHeader, controllerURL, item, returndata, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, item.UserID, eventType, item.UserID.ToString(), AppName, sourceIPAddress, item);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove an App Settings
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1014,
        ///       "appName": "Megha Store"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">APP SETTINGS REMOVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/remove")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RemoveAppSetting(ParamAppSetting itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = memberAppSettingBusiness.DeleteMemberAppSetting(itemData.userID);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "APP SETTINGS REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, itemData.userID, "AppSetting Removed", itemData.userID.ToString(), AppName, sourceIPAddress, itemData);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        #region APPSettings/web/cicd

        /// <summary>
        /// Get an App Setting
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
        ///       "message": "FETCH APP SETTINGS",
        ///       "data": {
        ///         "userID": 1060,
        ///         "appName": "I AM BACK",
        ///         "androidBundleID": "com.appifyai.iAmBack",
        ///         "appleBuldleID": "com.appifyai.iAmBack",
        ///         "appleAppID": "6471469501",
        ///         "mobileNo": "7995995962",
        ///         "fireBaseProjectID": "appify-android-gcp",
        ///         "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/logo.png",
        ///         "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon.png",
        ///         "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon_tr.png",
        ///         "modifiedBy:1001,
        ///         "modifiedOn:null
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH APP SETTINGS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/cicd/get")]
        [MapToApiVersion("1.0")]
        public IActionResult GetMemberAppSettingCICD(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.GetMemberAppSettingCICD(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH APP SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP SETTINGS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP SETTINGS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP SETTINGS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Get The List Of App Setting
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        /// {
        ///   "statusCode": 200,
        ///   "name": "SUCCESS_OK",
        ///   "message": "FETCH APP SETTINGS",
        ///   "data": [
        ///     {
        ///       "userID": 1044,
        ///       "appName": "REKHAS DESIGNS",
        ///       "androidBundleID": "com.appi_fy_ai.rekhas_designs",
        ///       "appleBuldleID": "com.appi-fy-ai.rekhas-designs",
        ///       "appleAppID": "6475161371",
        ///       "mobileNo": "9701167951",
        ///       "fireBaseProjectID": "appify-android-gcp",
        ///       "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1044/logo.png",
        ///       "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1044/icon.png",
        ///       "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1044/icon_tr.png",
        ///       "modifiedBy:1001,
        ///       "modifiedOn:null
        ///     },
        ///     {
        ///       "userID": 1049,
        ///       "appName": "BAKM lifestyle",
        ///       "androidBundleID": "com.appi_fy_ai.bakm_lifestyle",
        ///       "appleBuldleID": "com.appifyai.bakmLifestyle",
        ///       "appleAppID": "6471470575",
        ///       "mobileNo": "9994853833",
        ///       "fireBaseProjectID": "appify-android-gcp",
        ///       "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1049/logo.png",
        ///       "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1049/icon.png",
        ///       "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1049/icon_tr.png"
        ///       "modifiedBy:1001,
        ///       "modifiedOn:null
        ///     }
        /// ]        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH LIST APP SETTINGS SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/cicd/list")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ListMemberAppSettingCICD()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            MemberAppSettingCICD itemLite;

            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.ListMemberAppSettingCICD();

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH APP SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH LIST APP SETTINGS SUCCESSFULLY", reqHeader, controllerURL, item, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = item;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP LIST SETTINGS - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP CICD LIST SETTINGS - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Add App Settings
        /// </summary>
        /// <remarks>
        /// sample Request JSON :
        /// 
        ///     {
        ///         "userID": 1060,
        ///         "appName": "I AM BACK",
        ///         "androidBundleID": "com.appifyai.iAmBack",
        ///         "appleBuldleID": "com.appifyai.iAmBack",
        ///         "appleAppID": "6471469501",
        ///         "mobileNo": "7995995962",
        ///         "fireBaseProjectID": "appify-android-gcp",
        ///         "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/logo.png",
        ///         "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon.png",
        ///         "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon_tr.png",
        ///         "modifiedBy:1001
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "APP CICD SETTINGS SAVED SUCCESSFULLY",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">APP SETTINGS UPDATED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/cicd/update")]
        [MapToApiVersion("1.0")]
        public IActionResult SaveMemberAppSettingCICD(MemberAppSettingCICD itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = memberAppSettingBusiness.UpdateMemberAppSettingCICD(itemData);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "APP SETTINGS SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        #endregion

        /// <summary>
        /// Get an App Status
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
        ///       "message": "FETCH APP STATUS",
        ///       "data": {
        ///         "deploymentStatusAndroid": 4127,
        ///         "deploymentStatusApple": 4127,
        ///         "androidAppURL": "https://play.google.com/store/apps/details?id=com.appifyai.iAmBack",
        ///         "appleAppURL": "https://apps.apple.com/in/app/id6471469501"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH APP SETTINGS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/cicd/getappstatus")]
        [MapToApiVersion("1.0")]
        public IActionResult GetAppStatusWeb(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            MemberAppSettingLite itemLite;

            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.GetAppStatusCICD(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH APP STATUS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP STATUS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP STATUS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP STATUS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }


        /// <summary>
        /// Get an App Status
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1060,
        ///         "deploymentStatusAndroid": 4127,
        ///         "deploymentStatusApple": 4127,
        ///         "androidAppURL": "https://play.google.com/store/apps/details?id=com.appifyai.iAmBack",
        ///         "appleAppURL": "https://apps.apple.com/in/app/id6471469501"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH APP STATUS",
        ///       "data": {
        ///         "deploymentStatusAndroid": 4127,
        ///         "deploymentStatusApple": 4127,
        ///         "androidAppURL": "https://play.google.com/store/apps/details?id=com.appifyai.iAmBack",
        ///         "appleAppURL": "https://apps.apple.com/in/app/id6471469501"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH APP SETTINGS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/cicd/updateappstatus")]
        [MapToApiVersion("1.0")]
        public IActionResult UpdateMemberAppStatus(MemberAppSettingUpdate itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            MemberAppSettingLite itemLite;

            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.UpdateMemberAppStatus(itemData);

                if (item ==true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "UPDATE APP STATUS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UPDATED APP STATUS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP STATUS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP STATUS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }





        //Member Theme APIs
        /// <summary>
        /// GET THEME SETTING
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "memberID": 2391
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH THEME SETTINGS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("theme/get")]
        [MapToApiVersion("1.0")]

        public IActionResult GetMemberTheme(ParamUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                //TokenValidator.IsValidToken(Request, configuration, env);
                var item = memberThemeBusiness.Get(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH THEME SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH THEME SETTINGS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH THEME SETTINGS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH THEME SETTINGS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Add/Update Theme Setting
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "memberID": 2391,
        ///       "templateID": 1001,
        ///       "themeID": 1001,
        ///       "createdBy": 1060,
        ///       "modifiedBy": 0,
        ///       "isActive": true
        ///     }
        ///     
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">THEME SETTINGS SAVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("theme/save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> AddMemberTheme(MemberThemeHeader item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            var eventType = item.ThemeID > 0 ? "Theme Updated" : "New Theme Created";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);

                var result = memberThemeBusiness.Save(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THEME SETTINGS SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("THEME SETTINGS SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, item.MemberID, eventType, item.MemberID.ToString(), AppName, sourceIPAddress, item);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("THEME SETTINGS SAVED - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("THEME SETTINGS SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove an Theme Settings
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "memberID": 2391,
        ///       "templateID": 1001,
        ///       "themeID": 1001
        ///     }
        ///     
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">THEME SETTINGS REMOVED </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("theme/remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> RemoveMemberTheme(ParamMemberTheme itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = memberThemeBusiness.Delete(itemData.MemberID, itemData.TemplateID, itemData.ThemeID);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THEME SETTINGS REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("THEME SETTINGS REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, itemData.MemberID, "Theme Removed", itemData.MemberID.ToString(), AppName, sourceIPAddress, itemData);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("THEME SETTINGS REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("THEME SETTINGS REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        #region Member KYC 


        //Member KYC APIs
        /// <summary>
        /// Get a Member's KYC 
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1008
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH KYC SETTINGS",
        ///       "data": {
        ///         "memberID": 1008,
        ///         "pan": "PHWUV8934R",
        ///         "gst": "",
        ///         "aadharNo": "781909458733",
        ///         "bankName": "HDFC",
        ///         "bankAccountNo": "9901110",
        ///         "ifsc": "HDFC000011",
        ///         "bankAccountType": 3900,
        ///         "beneficiaryName": "Kiran",
        ///         "chequeImage": null,
        ///         "panImage": null,
        ///         "gstImage": null,
        ///         "aadharImage": null,
        ///         "aadharImage2": null,
        ///         "digitalSignatureImage": null,
        ///         "kvicNo": null,
        ///         "address": null,
        ///         "addressImage": null
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH KYC SETTINGS </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("kyc/get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetMemberKYC(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = memberKYCBusiness.Get(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH KYC SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH KYC SETTINGS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH KYC SETTINGS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH KYC SETTINGS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Save a Member KYC 
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "memberID": 1008,
        ///         "pan": "PHWUV8934R",
        ///         "gst": "",
        ///         "aadharNo": "781909458733",
        ///         "bankName": "HDFC",
        ///         "bankAccountNo": "9901110",
        ///         "ifsc": "HDFC000011",
        ///         "bankAccountType": 3900,
        ///         "beneficiaryName": "Kiran",
        ///         "chequeImage": null,
        ///         "panImage": null,
        ///         "gstImage": null,
        ///         "aadharImage": null,
        ///         "aadharImage2": null,
        ///         "digitalSignatureImage": null,
        ///         "kvicNo": null,
        ///         "address": null,
        ///         "addressImage": null
        ///     }
        ///      
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">KYC SAVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("kyc/save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> AddMemberKYC(MemberKYC item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            var eventType = item.MemberID > 0 ? "Member KYC Updated" : "Member KYC Created";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);


                var result = memberKYCBusiness.Save(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "KYC SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, result, StatusName.ok));
                    await Common.UpdateEventLogsNew("KYC SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
                    await auditService.LogAsync(EntityType.Vendor, item.MemberID, eventType, item.MemberID.ToString(), AppName, sourceIPAddress, item);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("KYC SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                await Common.UpdateEventLogsNew("KYC SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove a KYC
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
        /// <response code="200">KYC SETTINGS REMOVED </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("kyc/remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> RemoveMemberKYC(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = memberKYCBusiness.Delete(itemData.userID);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "KYC SETTINGS REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("KYC SETTINGS REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, itemData.userID, "Member KYC Removed", itemData.userID.ToString(), AppName, sourceIPAddress, itemData);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("KYC SETTINGS REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("KYC SETTINGS REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        #endregion


        //Member Contact APIs
        /// <summary>
        /// GET MEMBER CONTACT SETTINGS
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "memberID": 1060,
        ///       "mobileNo": "9827609876"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH MEMBER CONTACT SETTINGS",
        ///       "data": {
        ///         "memberID": 1060,
        ///         "mobileNo": "9827609876",
        ///         "contactName": "subbu",
        ///         "emailID": "asdfgh@gmal.com"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH MEMBER CONTACT SETTINGS </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("contact/get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetMemberContact(ParamMemberContact itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = memberContactBusiness.Get(itemData.MemberID, itemData.MobileNo);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER CONTACT SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER CONTACT SETTINGS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER CONTACT SETTINGS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER CONTACT SETTINGS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// GET MEMBER CONTACT LIST
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
        ///       "message": "FETCH MEMBER CONTACT LIST",
        ///       "data": [
        ///         {
        ///           "memberID": 1060,
        ///           "mobileNo": "9827609876",
        ///           "contactName": "subbu",
        ///           "emailID": "asdfgh@gmal.com"
        ///         },
        ///         {
        ///           "memberID": 1060,
        ///           "mobileNo": "9866214563",
        ///           "contactName": "hanis",
        ///           "emailID": ""
        ///         },
        ///         {
        ///         "memberID": 1060,
        ///           "mobileNo": "9866523514",
        ///           "contactName": "bablu",
        ///           "emailID": ""
        ///         },
        ///         {
        ///         "memberID": 1060,
        ///           "mobileNo": "9876543210",
        ///           "contactName": "kalyan",
        ///           "emailID": "zxcvb@gmail.com"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH MEMBER CONTACT LIST </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("contact/list")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult ListMemberContact(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = memberContactBusiness.List(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER CONTACT LIST";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER CONTACT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER CONTACT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER CONTACT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Save a Member Contact
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///     "memberID": 0,
        ///     "mobileNo": "6303467186",
        ///     "contactName": "kvr kalyan",
        ///     "emailID": "kvrkalyan1985@gmail.com"
        ///     }  
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MEMBER CONTACT SAVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("contact/save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> AddMemberContact(MemberContact item)
        {
            var reqHeader = Request;
            var fullName = item.ContactName;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            var eventType = item.MemberID > 0 ? "Contact Updated" : "New Contact Created";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                if (item.MemberID == 0)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "INVALID MEMBER ID";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("INVALID MEMBER ID", reqHeader, controllerURL, item, null, rm.message));
                }


                var result = memberContactBusiness.Save(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER CONTACT SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, item.MemberID, eventType, item.MemberID.ToString(), AppName, sourceIPAddress, item);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT SAVED SUCCESSFULLY - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT SAVED SUCCESSFULLY - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Save a Member Contact
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     [
        ///       {
        ///         "memberID": 0,
        ///         "mobileNo": "6303467186",
        ///         "contactName": "kvr kalyan",
        ///         "emailID": "kvrkalyan1985@gmail.com"
        ///       }
        ///     ]
        ///     
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MEMBER CONTACT SAVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("contact/bulksave")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> AddMemberContactBulk(List<MemberContact> items)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);



                var result = memberContactBusiness.BulkSave(items);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER CONTACT SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT BULK SAVED SUCCESSFULLY", reqHeader, controllerURL, items, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, items[0].MemberID, "Bulk Contacts Created", items[0].MemberID.ToString(), AppName, sourceIPAddress, items);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT BULK SAVED - NO CONTENT", reqHeader, controllerURL, items, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT BULK SAVED - ERROR", reqHeader, controllerURL, items, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove a Member Contact
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "memberID": 1002,
        ///       "mobileNo": "6303467186"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">MEMBER CONTACT REMOVED </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("contact/remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> RemoveMemberKYC(ParamMemberContact itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = memberContactBusiness.Delete(itemData.MemberID, itemData.MobileNo);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER CONTACT REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, itemData.MemberID, "Contact Removed - " + itemData.MemberID.ToString(), VendorID, AppName, sourceIPAddress, itemData);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER CONTACT REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// Adds a Member Banner.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : For a new Member Banner object, send the BannerID = 0.
        /// 
        ///      {
        ///        "BannerID": 0,
        ///        "memberID": 1003,
        ///        "bannerName": "KIA Banner",
        ///        "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///        "bannerType": 2,
        ///        "startDate": "2024-04-29 09:42:11.442Z",
        ///        "endDate": "2024-05-19 09:42:11.442Z",
        ///        "isCancel": false
        ///      }
        /// </remarks>
        /// <param name="memberBanner"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Member Banner Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("banner/Save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> memberBannerAdd(MemberBanner memberBanner)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            var eventType = memberBanner.BannerID > 0 ? "Banner Updated" : "New Banner Created";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.memberBusiness.memberBannerAdd(memberBanner);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER BANNER SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER BANNER SAVED SUCCESSFULLY", reqHeader, controllerURL, memberBanner, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, memberBanner.MemberID, eventType, memberBanner.MemberID.ToString(), AppName, sourceIPAddress, memberBanner);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER BANNER SAVED - NO CONTENT", reqHeader, controllerURL, memberBanner, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER BANNER SAVED - ERROR", reqHeader, controllerURL, memberBanner, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// removes Member Banner by BannerID
        /// </summary>
        /// <remarks>
        /// Sample Data :
        /// 
        ///     {
        ///         "BannerID":1003
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Boolean Value </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("banner/Remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> memberBannerRemove(ParamBannerID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string sourceIPAddress = reqHeader.Headers["IPAddress"].Count > 0 ? reqHeader.Headers["IPAddress"] : "Not Found";
            string AppName = reqHeader.Headers["AppName"].Count > 0 ? reqHeader.Headers["AppName"] : "WEB";
            string VendorID = reqHeader.Headers["VendorID"].Count > 0 ? reqHeader.Headers["VendorID"] : "0";
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.memberBusiness.memberBannerRemove(itemData.bannerID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER BANNER REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER BANNER REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                    await auditService.LogAsync(EntityType.Vendor, long.Parse(VendorID), "Banner Removed - "+itemData.bannerID.ToString(), VendorID.ToString(), AppName, sourceIPAddress, itemData);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER BANNER REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MEMBER BANNER REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }



        /// <summary>
        /// gets Member Banner by MemberID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "MemberID":1003
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///         "statusCode": 200,
        ///         "name": "SUCCESS_OK",
        ///         "message": "FETCH MEMBER BANNER ITEM!",
        ///         "data": {
        ///           "bannerID": 1000,
        ///           "memberID": 1003,
        ///           "bannerName": "KIA Banner",
        ///           "imageName": https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///           "bannerType": 2,
        ///           "startDate": "2024-04-29T09:42:11.443",
        ///           "endDate": "2024-05-19T09:42:11.443",
        ///          "isCancel": false
        ///         }
        ///     }
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns MemberBanner Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("banner/get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult memberBannerGet(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.memberBusiness.memberBannerGet(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER BANNER ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET MEMBER BANNER ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET MEMBER BANNER ITEM - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET MEMBER BANNER ITEM - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// LIST of Member Banners
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "MemberID":1003
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///         "statusCode": 200,
        ///         "name": "SUCCESS_OK",
        ///         "message": "FETCH MEMBER BANNER ITEM!",
        ///         "data": [
        ///           {
        ///             "bannerID": 1000,
        ///             "memberID": 1003,
        ///             "bannerName": "KIA Banner",
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///             "bannerType": 2,
        ///             "startDate": "2024-04-29T09:42:11.443",
        ///             "endDate": "2024-05-19T09:42:11.443",
        ///             "isCancel": false
        ///           }
        ///         ]
        ///    }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("banner/list")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult memberBannerList()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.memberBusiness.memberBannerList();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER BANNER ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status 
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER BANNER ITEM SUCCESSFULLY", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER BANNER ITEM - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH MEMBER BANNER ITEM - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// LIST of Member Banners By VendorID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "MemberID":1003
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///         "statusCode": 200,
        ///         "name": "SUCCESS_OK",
        ///         "message": "FETCH MEMBER BANNER ITEM!",
        ///         "data": [
        ///           {
        ///             "bannerID": 1000,
        ///             "memberID": 1003,
        ///             "bannerName": "KIA Banner",
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///             "bannerType": 2,
        ///             "startDate": "2024-04-29T09:42:11.443",
        ///             "endDate": "2024-05-19T09:42:11.443",
        ///             "isCancel": false
        ///           }
        ///         ]
        ///    }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("banner/listbyvendor")]
        [MapToApiVersion("1.0")]

        public IActionResult memberBannerListByVendor(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.memberBannerListByVendor(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH BY VENDOR MEMBER BANNER ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH BY VENDOR MEMBER BANNER ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH BY VENDOR MEMBER BANNER - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH BY VENDOR MEMBER BANNER - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// gets Member SMS Setting
        /// </summary>
        /// <remarks>
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
        ///       "message": "FETCH MEMBER SMS SETTING!",
        ///       "data": {
        ///         "vendorID": 1060,
        ///         "isFirebase": false
        ///       }
        ///     }
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns MemberBanner Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("getsmssetting")]
        [MapToApiVersion("1.0")]
        public IActionResult memberSmsSettingGet(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.memberSMSSettingGet(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER SMS SETTING!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET MEMBER SMS SETTING SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET MEMBER SMS SETTING - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET MEMBER SMS SETTING - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Get Share My App Links By Vendor
        /// </summary>
        /// <remarks>
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
        ///       "message": "APP LINKS HAVE BEEN SUCCESSFULLY FETCHED!",
        ///       "data": {
        ///         "appStoreLink": "https://apps.apple.com/in/app/id6471469501",
        ///         "playstoreLink": "https://play.google.com/store/apps/details?id=com.appifyai.iAmBack"
        ///       }
        ///     }
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns MemberBanner Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("getapplinks")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult getAppLinks(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.memberBusiness.getAppLinks(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "APP LINKS HAVE BEEN SUCCESSFULLY FETCHED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP LINKS HAVE BEEN SUCCESSFULLY FETCHED!", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }



        #region APPSettings/web/publish

        /// <summary>
        /// Get an App Publish Setting
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
        ///       "message": "FETCH APP SETTINGS",
        ///       "data": {
        ///         "userID": 1060,
        ///         "appName": "I AM BACK",
        ///         "androidBundleID": "com.appifyai.iAmBack",
        ///         "appleBuldleID": "com.appifyai.iAmBack",
        ///         "appleAppID": "6471469501",
        ///         "mobileNo": "7995995962",
        ///         "fireBaseProjectID": "appify-android-gcp",
        ///         "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/logo.png",
        ///         "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon.png",
        ///         "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon_tr.png",
        ///         "modifiedBy:1001,
        ///         "modifiedOn:null
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH APP PUBLISH SETTINGS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/publish/get")]
        [MapToApiVersion("1.0")]
        public IActionResult GetMemberAppPublishSetting(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.GetMemberAppPublishSetting(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH APP PUBLISH SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP SETTINGS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP SETTINGS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP SETTINGS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Get The List Of App Publish Setting
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        /// {
        ///   "statusCode": 200,
        ///   "name": "SUCCESS_OK",
        ///   "message": "FETCH APP PUBLISH SETTINGS",
        ///   "data": [
        ///     {
        ///       "userID": 1044,
        ///       "appName": "REKHAS DESIGNS",
        ///       "androidBundleID": "com.appi_fy_ai.rekhas_designs",
        ///       "appleBuldleID": "com.appi-fy-ai.rekhas-designs",
        ///       "appleAppID": "6475161371",
        ///       "mobileNo": "9701167951",
        ///       "fireBaseProjectID": "appify-android-gcp",
        ///       "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1044/logo.png",
        ///       "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1044/icon.png",
        ///       "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1044/icon_tr.png",
        ///       "modifiedBy:1001,
        ///       "modifiedOn:null
        ///     },
        ///     {
        ///       "userID": 1049,
        ///       "appName": "BAKM lifestyle",
        ///       "androidBundleID": "com.appi_fy_ai.bakm_lifestyle",
        ///       "appleBuldleID": "com.appifyai.bakmLifestyle",
        ///       "appleAppID": "6471470575",
        ///       "mobileNo": "9994853833",
        ///       "fireBaseProjectID": "appify-android-gcp",
        ///       "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1049/logo.png",
        ///       "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1049/icon.png",
        ///       "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1049/icon_tr.png"
        ///       "modifiedBy:1001,
        ///       "modifiedOn:null
        ///     }
        /// ]        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH LIST APP PUBLISH SETTINGS SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/publish/list")]
        [MapToApiVersion("1.0")]
        public IActionResult ListMemberAppPublishSetting()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;


            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.ListMemberAppPublishSetting();

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH APP PUBLISH SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH LIST APP SETTINGS SUCCESSFULLY", reqHeader, controllerURL, item, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = item;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP LIST SETTINGS - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH APP CICD LIST SETTINGS - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Add App Publish Settings
        /// </summary>
        /// <remarks>
        /// sample Request JSON :
        /// 
        ///     {
        ///         "userID": 1060,
        ///         "appName": "I AM BACK",
        ///         "androidBundleID": "com.appifyai.iAmBack",
        ///         "appleBuldleID": "com.appifyai.iAmBack",
        ///         "appleAppID": "6471469501",
        ///         "mobileNo": "7995995962",
        ///         "fireBaseProjectID": "appify-android-gcp",
        ///         "appLogo": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/logo.png",
        ///         "appIcon": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon.png",
        ///         "appIconTransparent": "https://appify-assets.s3.ap-south-2.amazonaws.com/1060/icon_tr.png",
        ///         "modifiedBy:1001
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "APP PUBLISH SETTINGS SAVED SUCCESSFULLY",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">APP PUBLISH SETTINGS UPDATED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("appsetting/web/publish/update")]
        [MapToApiVersion("1.0")]
        public IActionResult SaveMemberAppPublishSetting(MemberAppPublishSetting itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = memberAppSettingBusiness.UpdateMemberAppPublishSetting(itemData);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "APP PUBLISH SETTINGS SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("APP SETTINGS SAVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        #endregion






    }
}
