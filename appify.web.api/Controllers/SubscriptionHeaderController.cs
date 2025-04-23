using appify.models;
using appify.Business.Contract;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class SubscriptionController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ISubscriptionHeaderBusiness subscriptionheaderBusiness;
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;
        public SubscriptionController(IConfiguration configuration, IWebHostEnvironment env, ISubscriptionHeaderBusiness subscriptionheaderBusiness)
        {
            this.configuration = configuration;
            this.env = env;
            this.subscriptionheaderBusiness = subscriptionheaderBusiness;
        }

        #region Get SubscriptionHeader Item

        [HttpPost, Route("get")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionHeaderGet(ParamSubscription itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionheaderBusiness.GetSubscriptionHeader(itemData.PlanID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONHEADER ITEM!";
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
        #endregion


        #region Get SubscriptionHeader List

        [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionHeaderList()
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionheaderBusiness.ListSubscriptionHeader();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONHEADER LIST!";
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
        #endregion


        #region Save SubscriptionHeader Item

        [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionHeaderSave(SubscriptionHeader itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionheaderBusiness.SaveSubscriptionHeader(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SAVE SUBSCRIPTIONHEADER ITEM!";
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
        #endregion


        #region Remove SubscriptionHeader Item

        [HttpPost, Route("remove")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionHeaderRemove(ParamSubscription itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionheaderBusiness.DeleteSubscriptionHeader(itemData.PlanID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "REMOVE SUBSCRIPTIONHEADER ITEM!";
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
        #endregion

    }
}
