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

        /// <summary>
        /// Get Subscription Plan By PlanID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 1000
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTIONHEADER ITEM!",
        ///       "data": {
        ///         "planID": 1000,
        ///         "planName": "Basic Plan",
        ///         "description": "Entry-level subscription",
        ///         "isActive": true
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTIONHEADER ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Get Subscription Plan List
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTIONHEADER LIST!",
        ///       "data": [
        ///         {
        ///           "planID": 1000,
        ///           "planName": "Basic Plan",
        ///           "description": "Entry-level subscription",
        ///           "isActive": true
        ///         },
        ///         {
        ///           "planID": 1001,
        ///           "planName": "Advanced Plan",
        ///           "description": "Mid-tier subscription",
        ///           "isActive": true
        ///         },
        ///         {
        ///         "planID": 1002,
        ///           "planName": "Professional Plan",
        ///           "description": "Premium subscription",
        ///           "isActive": true
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTIONHEADER ITEM LIST!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Save/Update Subscription Plan
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 0,
        ///       "planName": "Professional Plan",
        ///       "description": "Premium subscription",
        ///       "isActive": true,
        ///       "createdBy": 1000,
        ///       "modifiedBy": 1000,
        ///     }   
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE SUBSCRIPTIONHEADER ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Delete Subscription Plan By PlanID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 1000
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE SUBSCRIPTIONHEADER ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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
        /// <summary>
        /// Get Subscription's Feature Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 1000,
        ///       "itemID": 100,
        ///       "featureID": 100
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTIONITEM ITEM!",
        ///       "data": {
        ///         "itemID": 100,
        ///         "planID": 1000,
        ///         "featureID": 100,
        ///         "value": "",
        ///         "isActive": true
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTIONITEM ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Get Subscription's Feature Item List
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 1000
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTIONITEM LIST!",
        ///       "data": [
        ///         {
        ///           "itemID": 100,
        ///           "planID": 1000,
        ///           "featureID": 100,
        ///           "value": "",
        ///           "isActive": true
        ///         },
        ///         {
        ///           "itemID": 101,
        ///           "planID": 1000,
        ///           "featureID": 101,
        ///           "value": "",
        ///           "isActive": true
        ///         },
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTIONITEM ITEM LIST!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Save/Update Subscription's Feature Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "itemID": 0,
        ///       "planID": 1000,
        ///       "featureID": 100,
        ///       "value": "abcd",
        ///       "isActive": true,
        ///       "createdBy": 0,
        ///       "modifiedBy": 0
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE/UPDATE SUBSCRIPTIONITEM ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Delete Subscription's Feature Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 1000,
        ///       "itemID": 100,
        ///       "featureID": 100
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE SUBSCRIPTIONITEM ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Get Subscription's Feature by FeatureID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "featureID": 100
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTION FEATURE!",
        ///       "data": {
        ///         "featureID": 100,
        ///         "featureName": "AppliedCommission",
        ///         "description": "Appify Commission",
        ///         "isActive": true
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTION FEATURE!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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
                    rm.message = "FETCH SUBSCRIPTION FEATURE!";
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

        /// <summary>
        /// Get Subscription's Feature List
        /// </summary>
        /// <remarks>   
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTION FEATURE LIST!",
        ///       "data": [
        ///         {
        ///           "featureID": 100,
        ///           "featureName": "AppliedCommission",
        ///           "description": "Appify Commission",
        ///           "isActive": true
        ///         },
        ///         {
        ///           "featureID": 101,
        ///           "featureName": "WarehouseCount",
        ///           "description": "Number of Warehouses",
        ///           "isActive": true
        ///         },
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTION FEATURE LIST!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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
                    rm.message = "FETCH SUBSCRIPTION FEATURE LIST!";
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

        /// <summary>
        /// Save/Update Subscription's Feature
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "featureID": 0,
        ///       "featureName": "ProductCatalog",
        ///       "description": "Product Catalog",
        ///       "isActive": true,
        ///       "createdBy": 1000,
        ///       "modifiedBy": 1000
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE/UPDATE SUBSCRIPTION FEATURE!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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
                    rm.message = "SAVE SUBSCRIPTION FEATURE!";
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

        /// <summary>
        /// Delete Subscription's Feature by FeatureID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "featureID": 100
        ///     }   
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE SUBSCRIPTION FEATURE!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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
                    rm.message = "REMOVE SUBSCRIPTION FEATURE!";
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
        /// <summary>
        /// Get Subscription's Price Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 1000,
        ///       "priceID": 1000
        ///     }
        ///     
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTIONPRICE ITEM!",
        ///       "data": {
        ///         "priceID": 1000,
        ///         "price": 499,
        ///         "term": 1,
        ///         "planID": 1000,
        ///         "planName": "Baric Plan",
        ///         "isActive": true
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTION PRICE ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Get Subscription's Price Item List
        /// </summary>
        /// <remarks>
        /// Sample response:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SUBSCRIPTIONPRICE LIST!",
        ///       "data": [
        ///         {
        ///           "priceID": 1000,
        ///           "price": 499,
        ///           "term": 1,
        ///           "planID": 1000,
        ///           "planName": "Baric Plan",
        ///           "isActive": true
        ///         },
        ///         {
        ///           "priceID": 1001,
        ///           "price": 1799,
        ///           "term": 6,
        ///           "planID": 1000,
        ///           "planName": "Baric Plan",
        ///           "isActive": true
        ///         },
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SUBSCRIPTION PRICE ITEM LIST!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Save/Update Subscription's Price Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "priceID": 0,
        ///       "price": 4999,
        ///       "term": 12,
        ///       "planID": 1004,
        ///       "createdBy": 1000,
        ///       "modifiedBy": 1000
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE/UPDATE SUBSCRIPTION PRICE ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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

        /// <summary>
        /// Get Subscription's Price Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "planID": 1000,
        ///       "priceID": 1000
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE SUBSCRIPTION PRICE ITEM!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 
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
