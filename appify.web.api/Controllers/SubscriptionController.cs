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
        private readonly ISubscriptionItemBusiness subscriptionitemBusiness;
        private readonly ISubscriptionFeatureBusiness subscriptionfeatureBusiness;
        private readonly ISubscriptionPriceBusiness subscriptionpriceBusiness;

        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;
        public SubscriptionController(IConfiguration configuration, IWebHostEnvironment env,
                                      ISubscriptionHeaderBusiness subscriptionheaderBusiness,
                                      ISubscriptionItemBusiness subscriptionitemBusiness,
                                      ISubscriptionFeatureBusiness subscriptionfeatureBusiness,
                                      ISubscriptionPriceBusiness subscriptionpriceBusiness)
        {
            this.configuration = configuration;
            this.env = env;
            this.subscriptionheaderBusiness = subscriptionheaderBusiness;
            this.subscriptionitemBusiness = subscriptionitemBusiness;
            this.subscriptionfeatureBusiness = subscriptionfeatureBusiness;
            this.subscriptionpriceBusiness = subscriptionpriceBusiness;
        }

        #region Subscription Header


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

        #endregion

        #region Subscription Item

        #region Get SubscriptionItem Item

        [HttpPost, Route("item/get")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionItemGet(ParamSubscriptionItem itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionitemBusiness.GetSubscriptionItem(itemData.ItemID, itemData.PlanID, itemData.FeatureID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONITEM ITEM!";
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


        #region Get SubscriptionItem List

        [HttpPost, Route("item/list")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionItemList(ParamSubscription itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionitemBusiness.ListSubscriptionItem(itemData.PlanID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONITEM LIST!";
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


        #region Save SubscriptionItem Item

        [HttpPost, Route("item/save")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionItemSave(SubscriptionItem itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionitemBusiness.SaveSubscriptionItem(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SAVE SUBSCRIPTIONITEM ITEM!";
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


        #region Remove SubscriptionItem Item

        [HttpPost, Route("item/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionItemRemove(ParamSubscriptionItem itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionitemBusiness.DeleteSubscriptionItem(itemData.ItemID, itemData.PlanID, itemData.FeatureID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "REMOVE SUBSCRIPTIONITEM ITEM!";
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


        #endregion


        #region Subscription Feature


        #region Get SubscriptionFeature Item

        [HttpPost, Route("feature/get")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionFeatureGet(ParamSubscriptionFeature itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionfeatureBusiness.GetSubscriptionFeature(itemData.FeatureID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONFEATURE ITEM!";
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


        #region Get SubscriptionFeature List

        [HttpPost, Route("feature/list")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionFeatureList()
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionfeatureBusiness.ListSubscriptionFeature();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONFEATURE LIST!";
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


        #region Save SubscriptionFeature Item

        [HttpPost, Route("feature/save")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionFeatureSave(SubscriptionFeature itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionfeatureBusiness.SaveSubscriptionFeature(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SAVE SUBSCRIPTIONFEATURE ITEM!";
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


        #region Remove SubscriptionFeature Item

        [HttpPost, Route("feature/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionFeatureRemove(ParamSubscriptionFeature itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionfeatureBusiness.DeleteSubscriptionFeature(itemData.FeatureID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "REMOVE SUBSCRIPTIONFEATURE ITEM!";
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

        #endregion

        #region Subscription Price

        #region Get SubscriptionPrice Item

        [HttpPost, Route("price/get")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionPriceGet(ParamSubscriptionPrice itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionpriceBusiness.GetSubscriptionPrice(itemData.PriceID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONPRICE ITEM!";
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


        #region Get SubscriptionPrice List

        [HttpPost, Route("price/list")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionPriceList()
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionpriceBusiness.ListSubscriptionPrice();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SUBSCRIPTIONPRICE LIST!";
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


        #region Save SubscriptionPrice Item

        [HttpPost, Route("price/save")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionPriceSave(SubscriptionPrice itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionpriceBusiness.SaveSubscriptionPrice(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SAVE SUBSCRIPTIONPRICE ITEM!";
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


        #region Remove SubscriptionPrice Item

        [HttpPost, Route("price/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult SubscriptionPriceRemove(ParamSubscriptionPrice itemData)
        {

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.subscriptionpriceBusiness.DeleteSubscriptionPrice(itemData.PriceID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "REMOVE SUBSCRIPTIONPRICE ITEM!";
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

        #endregion

    }
}
