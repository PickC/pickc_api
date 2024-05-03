using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Hosting;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class AddressController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IAddressBusiness addressBusiness;
        private ResponseMessage rm;

        public AddressController(IConfiguration configuration, IAddressBusiness iResultData, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.addressBusiness = iResultData;
            this.eventLogBusiness = eventLogBusiness;       
        }

        [HttpPost, Route("save")]
        public IActionResult Add(Address item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = addressBusiness.SaveAddress(item);
                if (result!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ADDRESS SAVED SUCCESSFULLY!";
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

        
        [HttpPost, Route("remove")]
       
        public IActionResult Remove(ParamAddress itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = addressBusiness.DeleteAddress(itemData.addressID, itemData.userID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ADDRESS REMOVED";
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

        [HttpPost, Route("getaddress")]
        public IActionResult GetAddress(ParamAddress itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();

                var item = addressBusiness.GetAddress(itemData.addressID,itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ADDRESS ITEM";
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


        [HttpPost, Route("getdefaultaddress")]
        public IActionResult GetAddress(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();

                var item = addressBusiness.GetDefaultAddress(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ADDRESS ITEM";
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

        [HttpPost, Route("list")]
        public IActionResult List(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                List<Address> items = addressBusiness.GetList(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ADDRESS LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, items, StatusName.ok));
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
