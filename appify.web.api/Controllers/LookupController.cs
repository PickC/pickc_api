using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace appify.web.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class LookupController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly ILookupBusiness lookupBusiness;
        private ResponseMessage rm;
        public LookupController(IConfiguration configuration, ILookupBusiness iResultData, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.lookupBusiness = iResultData;
            this.eventLogBusiness = eventLogBusiness;
        }


        [HttpPost,Route("save")]
        public IActionResult Add(Lookup item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = lookupBusiness.SaveLookUp(item);
                if (result!=null)
                {
                    var newitem = new Lookup();

                    //newitem = lookupBusiness.GetLookUp(item.LookupCode, item.LookupCategory);

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK UP ITEM SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK UP ITEM SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK UP ITEM SAVED - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK UP ITEM SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("remove")]
        public IActionResult Remove(ParamLookup itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = lookupBusiness.DeleteLookUp(itemData.lookupID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOKUP REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOKUP REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOKUP REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOKUP REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost,Route("getitem")]
        public IActionResult GetLookup(ParamLookup jsonData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();

                var item = lookupBusiness.GetLookUp(jsonData.lookupID);

                if (item!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH LOOKUP ITEM";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET LOOKUP ITEM SUCCESSFULLY", reqHeader, controllerURL, jsonData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET LOOKUP ITEM - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET LOOKUP ITEM - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost,Route("list")]
        [MapToApiVersion("1.0")]
        public IActionResult List(ParamLookupCategory jsonData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                List<Lookup> items = lookupBusiness.GetList(jsonData.category);
                if (items?.Any()==true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST SUCCESSFULLY", reqHeader, controllerURL, jsonData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
            }
            return Ok(rm);

        }
        [HttpPost, Route("list")]
        [MapToApiVersion("2.0")]
        public IActionResult Listv2(ParamLookupCategory jsonData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                List<Lookup> items = lookupBusiness.GetList(jsonData.category);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST SUCCESSFULLY", reqHeader, controllerURL, jsonData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("listbymember")]
        public IActionResult ListByMember(ParamLookupByMember jsonData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            List<Lookup> items = new List<Lookup>();
            try
            {
                rm = new ResponseMessage();
                items = lookupBusiness.GetList(jsonData.category,jsonData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST BY MEMBER SUCCESSFULLY", reqHeader, controllerURL, jsonData, items, StatusName.ok));
                }
                else
                {
                    items = new List<Lookup>();
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST BY MEMBER - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST BY MEMBER - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpGet, Route("listall")]
        public async Task<IActionResult> ListAll()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<Lookup> items = lookupBusiness.GetAllList();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                await Common.UpdateEventLogsNew("LOOK-UP LIST ALL - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        [HttpPost, Route("listallforstartup")]
        public async Task<IActionResult> ListAllForStartUp(ParamLookupCategories categoryList)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<LookupStartUpList> items = new List<LookupStartUpList>();
                //foreach (var item  in categoryList.list)
                //{
                //    items.AddRange(lookupBusiness.GetListForStartup(item.category));
                //}
                items = lookupBusiness.GetListForStartup(categoryList.list);
                //lookupBusiness.GetAllList();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST ALL FOR STARTUP SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL FOR STARTUP SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST ALL FOR STARTUP - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL FOR STARTUP - NO CONTENT", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                // this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST ALL FOR STARTUP - ERROR", reqHeader, controllerURL, null, null, rm.message));
                await Common.UpdateEventLogsNew("LOOK-UP LIST ALL FOR STARTUP - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }


        /// <summary>
        /// gets System configuration settings
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "SettingKey": "DELIVERYCHANNEL"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SYSTEM CONFIG SETTING LIST",
        ///       "data": [
        ///         {
        ///           "settingValue": "SHIPROCKET",
        ///           "settingStatus": true
        ///         },
        ///         {
        ///           "settingValue": "DELHIVERY",
        ///           "settingStatus": false
        ///         },
        ///         {
        ///         "settingValue": "EASEBUZZ",
        ///           "settingStatus": false
        ///         },
        ///         {
        ///         "settingValue": "PHONEPE",
        ///           "settingStatus": false
        ///         },
        ///         {
        ///         "settingValue": "RAZORPAY",
        ///           "settingStatus": true
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ParamSystemConfigSetting Object</returns>
        /// <response code="500">ParamSystemConfigSetting with Error Description</response> 
        [HttpPost]
        [Route("systemconfigurationtlist")]
        public IActionResult GetSystemConfigurationSettings (ParamSystemConfigSetting itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                List<SystemConfigSetting> items = lookupBusiness.GetSystemConfigurationSettings(itemData.SettingKey);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SYSTEM CONFIG SETTING LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

    }
}
