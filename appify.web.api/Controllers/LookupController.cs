using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace appify.web.api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class LookupController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ILookupBusiness lookupBusiness;
        private ResponseMessage rm;
        public LookupController(IConfiguration configuration, ILookupBusiness iResultData)
        {
            this.configuration = configuration;
            this.lookupBusiness = iResultData;

        }


        [HttpPost,Route("save")]
        public IActionResult Add(Lookup item)
        {
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
        public IActionResult Remove(ParamLookup itemData)
        {

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

        [HttpPost,Route("getitem")]
        public IActionResult GetLookup(ParamLookup jsonData)
        {

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

        [HttpPost,Route("list")]
        public IActionResult List(ParamLookupCategory jsonData)
        {
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

        [HttpPost, Route("listbymember")]
        public IActionResult ListByMember(ParamLookupByMember jsonData)
        {
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
                }
                else
                {
                    items = new List<Lookup>();
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.ok;
                    rm.data = items;
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

        [HttpGet, Route("listall")]
        public IActionResult ListAll()
        {
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
