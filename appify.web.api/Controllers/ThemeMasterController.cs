using appify.Business;
using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ThemeMasterController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IThemeMasterBusiness themeMasterBusiness;
        private ResponseMessage rm;


        public ThemeMasterController(IConfiguration configuration, IThemeMasterBusiness iResultData)
        {
            this.configuration = configuration;
            this.themeMasterBusiness = iResultData;

        }



        [HttpPost, Route("save")]
        public IActionResult Add(ThemeMaster item)
        {
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
        public IActionResult Remove(long themeID)
        {

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

        [HttpPost, Route("get")]
        public IActionResult GetThemeMaster(long themeID)
        {

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
        public IActionResult List()
        {
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
