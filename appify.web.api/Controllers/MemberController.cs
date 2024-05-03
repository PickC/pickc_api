using appify.Business;
using appify.Business.Contract;
using appify.DataAccess.Contract;
using appify.models;
using appify.utility;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Hosting;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
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
        private ResponseMessage rm;

        public MemberController(IConfiguration configuration, 
                                IMemberBusiness iResultData,
                                IMemberReturnPolicyBusiness memberReturnPolicyBusiness,
                                IMemberAppSettingBusiness memberAppSettingBusiness,
                                IMemberThemeBusiness memberThemeBusiness,
                                IMemberKYCBusiness memberKYCBusiness,
                                IMemberContactBusiness memberContactBusiness, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.memberBusiness = iResultData;
            this.memberReturnPolicyBusiness = memberReturnPolicyBusiness;
            this.memberAppSettingBusiness = memberAppSettingBusiness;
            this.memberThemeBusiness = memberThemeBusiness;
            this.memberKYCBusiness = memberKYCBusiness;
            this.memberContactBusiness = memberContactBusiness;
            this.eventLogBusiness = eventLogBusiness;
        }

        // GET: api/<MemberController>
        [HttpGet]
        public IActionResult GetAllMembers()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                
                var items = this.memberBusiness.GetAllMembers();

                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));

            }

            return Ok(rm);
        }

        // GET api/<MemberController>/5
        [HttpGet("{userID}")]
        public IActionResult GetMember(long userID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var member = this.memberBusiness.GetMember(Convert.ToInt64(userID));
                if (member !=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER";
                    rm.name = StatusName.ok;
                    rm.data = member;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, userID, member, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, userID, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, userID, null, rm.message));

            }
            return Ok(rm);
        }





        // POST api/<MemberController>
        [HttpPost,Route("Register")]
        public IActionResult Register(appify.models.Member item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var memberItem = this.memberBusiness.RegisterMember(item);
                if (memberItem.UserID>0)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER REGISTRATION SUCCESSFUL!";
                    rm.name = StatusName.ok;
                    rm.data = memberItem;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, memberItem, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO REGISTER MEMBER";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost,Route("DeActivateMember")]
        public IActionResult DeactivateMember(ParamMemberUserID itemData)
        {
            //dynamic data = jsondata;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.RemoveMember(itemData.userID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DE-ACTIVATED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DE-ACTIVATE MEMBER";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }


        [HttpPost, Route("DeleteMember")]
        public IActionResult DeleteMember(ParamDeactivateMember itemData)
        {
            //dynamic data = jsondata;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.RemoveMemberByMobileNo(itemData.mobileNo,itemData.password);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DELETED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DELETED MEMBER";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }
        

        [HttpPost,Route("ResetPassword")]
        public IActionResult ResetPassword(ParamMemberResetPassword itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                //dynamic data = jsondata;

                rm = new ResponseMessage();
                var member = this.memberBusiness.ResetPassword(itemData.userID,itemData.password);
                if (member != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PASSWORD RESET SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = member;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, member, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO RESET PASSWORD";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        [HttpPost,Route("CheckMember")]
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
                Member result = this.memberBusiness.IsMemberExist(itemData.emailID,itemData.mobileNo,itemData.memberType,itemData.vendorID);
                if (result !=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER WITH SIMILAR MOBILE NO. EXISTS!";
                    rm.name = StatusName.ok;

                    //var itemdata = this.memberBusiness.GetMember();

                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "MEMBER DOES NOT EXIST!";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }


        // GET api/<MemberController>/5
        [HttpPost,Route("OrdersCount")]
        public IActionResult OrdersCount(ParamMemberUserID item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var count = this.memberBusiness.MemberOrderCount(item.userID);
                if (count >0 )
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ORDERS COUNT";
                    rm.name = StatusName.ok;
                    rm.data = count;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, item, count, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, item, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);
        }

        // GET api/<MemberController>/5
        [HttpPost, Route("OnlinePaymentAllowed")]
        public IActionResult OnlinePaymentStatus(ParamMemberUserID item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                bool isAllowed = this.memberBusiness.CheckMemberOnlinePaymentStatus(item.userID);
                 
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ONLINE PAYMENT STATUS";
                    rm.name = StatusName.ok;
                    rm.data = isAllowed;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, item, isAllowed, StatusName.ok));
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);
        }



        [HttpPost, Route("SignIn")]
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
                var returnData = this.memberBusiness.MemberLogIn(itemData.emailID,itemData.mobileNo, itemData.password,itemData.parentID);
                if (returnData != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DATA";
                    rm.name = StatusName.ok;
                    rm.data = returnData;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, returnData, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "MEMBER DOES NOT EXIST!";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        [HttpPost, Route("dashboard")]
        public IActionResult MemberDashboard(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
               // dynamic data = jsondata;

                rm = new ResponseMessage();
                //var member = this.memberBusiness.MemberDashboard(Convert.ToInt64(data.userID));

                //TODO: to implement the above dashboard information


                MemberDashboard dashboard = new MemberDashboard(){
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, dashboard, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO SEND DASHBOARD DATA";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);
        }

        [HttpPost, Route("rp/get")]
        public IActionResult GetReturnPolicy(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberReturnPolicyBusiness.GetItem(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH RETURN-POLICY";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("rp/save")]
        public IActionResult AddReturnPolicy(MemberReturnPolicy item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = memberReturnPolicyBusiness.Save(item);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "RETURN POLICY SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("rp/remove")]
        public IActionResult RemoveReturnPolicy(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = memberReturnPolicyBusiness.Remove(itemData.userID);

                if (result!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "RETURN POLICY REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("appsetting/get")]
        public IActionResult GetAppSetting(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberAppSettingBusiness.GetAppSettingList(itemData.userID).FirstOrDefault();

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH APP SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("appsetting/save")]
        public IActionResult AddAppSetting(MemberAppSetting item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = memberAppSettingBusiness.saveAppSetting(item);
                if (result)
                {
                    var returndata = memberAppSettingBusiness.GetAppSetting(item.UserID, item.AppName);

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "APP SETTINGS SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = returndata;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, returndata, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("appsetting/remove")]
        public IActionResult RemoveAppSetting(ParamAppSetting itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = memberAppSettingBusiness.DeleteAppSetting(itemData.userID,itemData.appName);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "APP SETTINGS REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        //Member Theme APIs

        [HttpPost, Route("theme/get")]
        public IActionResult GetMemberTheme(ParamMemberTheme itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberThemeBusiness.Get(itemData.MemberID,itemData.ThemeID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH THEME SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("theme/save")]
        public IActionResult AddMemberTheme(ParamMemberTheme item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                MemberTheme memberTheme = new MemberTheme();
                memberTheme.ThemeID = item.ThemeID;
                memberTheme.MemberID = item.MemberID;

                var result = memberThemeBusiness.Save(memberTheme);
                if (result!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THEME SETTINGS SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("theme/remove")]
        public IActionResult RemoveMemberTheme(ParamMemberTheme itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = memberThemeBusiness.Delete(itemData.MemberID, itemData.ThemeID);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THEME SETTINGS REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }


        //Member KYC APIs

        [HttpPost, Route("kyc/get")]
        public IActionResult GetMemberKYC(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberKYCBusiness.Get(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH KYC SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("kyc/save")]
        public IActionResult AddMemberKYC(MemberKYC item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();




                var result = memberKYCBusiness.Save(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "KYC SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("kyc/remove")]
        public IActionResult RemoveMemberKYC(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = memberKYCBusiness.Delete(itemData.userID);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "KYC SETTINGS REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }


        //Member Contact APIs

        [HttpPost, Route("contact/get")]
        public IActionResult GetMemberContact(ParamMemberContact itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberContactBusiness.Get(itemData.MemberID,itemData.MobileNo);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER CONTACT SETTINGS";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("contact/list")]
        public IActionResult ListMemberContact(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = memberContactBusiness.List(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER CONTACT LIST";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("contact/save")]
        public IActionResult AddMemberContact(MemberContact item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                if (item.MemberID==0)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "INVALID MEMBER ID";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                }


                var result = memberContactBusiness.Save(item);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER CONTACT SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("contact/bulksave")]
        public IActionResult AddMemberContactBulk(List<MemberContact> items)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();




                var result = memberContactBusiness.BulkSave(items);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER CONTACT SAVED SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, items, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, items, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, items, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("contact/remove")]
        public IActionResult RemoveMemberKYC(ParamMemberContact itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = memberContactBusiness.Delete(itemData.MemberID, itemData.MobileNo);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER CONTACT REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
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
        public IActionResult memberBannerAdd(MemberBanner memberBanner)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.memberBannerAdd(memberBanner);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER BANNER SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, memberBanner, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, memberBanner, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, memberBanner, null, rm.message));
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
        public IActionResult memberBannerRemove(ParamBannerID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.memberBannerRemove(itemData.bannerID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER BANNER REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, itemData, null, rm.message));
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
        public IActionResult memberBannerGet(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.memberBannerGet(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER BANNER ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
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
        public IActionResult memberBannerList()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.memberBusiness.memberBannerList();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH MEMBER BANNER ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

    }
}
