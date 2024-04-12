using appify.Business;
using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
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
        private readonly IConfiguration configuration;
        private readonly IAddressBusiness addressBusiness;
        private ResponseMessage rm;

        public AddressController(IConfiguration configuration, IAddressBusiness iResultData)
        {
            this.configuration = configuration;
            this.addressBusiness = iResultData;

        }

        [HttpPost, Route("save")]
        public IActionResult Add(Address item)
        {
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

        
        [HttpPost, Route("remove")]
       
        public IActionResult Remove(ParamAddress itemData)
        {
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

        [HttpPost, Route("getaddress")]
        public IActionResult GetAddress(ParamAddress itemData)
        {
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


        [HttpPost, Route("getdefaultaddress")]
        public IActionResult GetAddress(ParamMemberUserID itemData)
        {
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

        [HttpPost, Route("list")]
        public IActionResult List(ParamMemberUserID itemData)
        {
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
