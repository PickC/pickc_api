using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace appify.web.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
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
                if (result)
                {
                    var newitem = new Lookup();

                    //newitem = lookupBusiness.GetLookUp(item.LookupCode, item.LookupCategory);

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK UP ITEM SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, item, result, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, item, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
            }
            return Ok(rm);

        }

        [HttpPost,Route("list")]
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, items, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, items, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    items = new List<Lookup>();
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, jsonData, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
            }
            return Ok(rm);

        }

        [HttpGet, Route("listall")]
        public IActionResult ListAll()
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
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok);
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

    }
}
