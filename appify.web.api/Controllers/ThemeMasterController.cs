using appify.Business;
using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ThemeMasterController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IThemeMasterBusiness themeMasterBusiness;
        private ResponseMessage rm;


        public ThemeMasterController(IConfiguration configuration, IThemeMasterBusiness iResultData, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.themeMasterBusiness = iResultData;
            this.eventLogBusiness = eventLogBusiness;
        }



        [HttpPost, Route("save")]
        public IActionResult Add(ThemeMaster item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var result = themeMasterBusiness.Save(item);
                if (result!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THEME MASTER SAVED SUCCESSFULLY!";
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
        public IActionResult Remove(long themeID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = themeMasterBusiness.Delete(themeID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THEME REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, themeID, result, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, themeID, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Master", reqHeader, controllerURL, themeID, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
            }
            return Ok(rm);

        }

        [HttpPost, Route("get")]
        public IActionResult GetThemeMaster(long themeID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();

                var item = themeMasterBusiness.Get(themeID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH THEME ITEM";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, themeID, item, StatusName.ok);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, themeID, null, rm.message);
                    this.eventLogBusiness.eventLogAdd(eventlog);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                EventLogs eventlog = UpdateEventLog.UpdateEventLogs("Transaction", reqHeader, controllerURL, themeID, null, rm.message);
                this.eventLogBusiness.eventLogAdd(eventlog);
            }
            return Ok(rm);

        }

        [HttpPost, Route("list")]
        public IActionResult List()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                List<ThemeMaster> items = themeMasterBusiness.ListAll();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "THEME LIST";
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
