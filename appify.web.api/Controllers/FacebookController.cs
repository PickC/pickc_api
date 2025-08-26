using appify.Business;
using appify.Business.Contract;
using appify.utility;
using appify.models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class FacebookController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;
        private ResponseMessage rm;
        private readonly HttpClient httpClient;
        public FacebookController(IEventLogBusiness eventLogBusiness, IConfiguration configuration, IWebHostEnvironment env)
        {
            this.eventLogBusiness = eventLogBusiness;
            this.configuration = configuration;
            this.env = env;
        }


    /// <summary>
    /// CREATE A PRODUCT CATALOG
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     businessID : 743286941807169
    ///     catalogName : MyCatalog25082025
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "CATALOG HAS BEEN SUCCESSFULLY CREATED",
    ///       "data": {
    ///         "id": "2773114199553599"
    ///       }
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">CATALOG HAS BEEN SUCCESSFULLY CREATED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("CreateaProductCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> CreateaProductCatalog([Required] string businessID, string catalogName)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
                var result = fs.CreateCatalog(businessID, catalogName);//"{\"id\":\"2773114199553599\"}";//
                if (result!=null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG HAS BEEN SUCCESSFULLY CREATED";
                rm.name = StatusName.ok;
                rm.data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
            }
            else
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG NOT CREATED";
                rm.name = StatusName.ok;
                rm.data = false;
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("CATALOG CREATE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }


    /// <summary>
    /// UPDATE A PRODUCT CATALOG
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     catalogID : 1288984646294739
    ///     catalogName: MyCatalog25082025
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "CATALOG HAS BEEN SUCCESSFULLY UPDATED!",
    ///       "data": true
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">CATALOG HAS BEEN SUCCESSFULLY UPDATED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("UpdateaProductCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> UpdateaProductCatalog([Required] string catalogID, string catalogName)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
            var result = fs.EditCatalogAsync(catalogID, catalogName);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG HAS BEEN SUCCESSFULLY UPDATED";
                rm.name = StatusName.ok;
                rm.data = true;
            }
            else
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG NOT UPDATED";
                rm.name = StatusName.ok;
                rm.data = false;
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("CATALOG UPDATE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }
    /// <summary>
    /// DELETE A PRODUCT CATALOG
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     catalogID : 1288984646294739
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "CATALOG HAS BEEN SUCCESSFULLY DELETED!",
    ///       "data": true
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">CATALOG HAS BEEN SUCCESSFULLY DELETED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("DeleteaProductCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> DeleteaProductCatalog([Required] string catalogID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
            var result = fs.DeleteCatalogAsync(catalogID);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG HAS BEEN SUCCESSFULLY DELETED";
                rm.name = StatusName.ok;
                rm.data = true;
            }
            else
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG NOT DELETED";
                rm.name = StatusName.ok;
                rm.data = false;
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("CATALOG DELETE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }
    ///// <summary>
    ///// GET LIST OF PRODUCT CATALOG
    ///// </summary>
    ///// <remarks>
    ///// Sample request JSON :
    ///// 
    /////     businessID : 743286941807169
    ///// 
    ///// Sample response JSON :
    ///// 
    /////     {
    /////       "statusCode": 200,
    /////       "name": "SUCCESS_OK",
    /////       "message": "CATALOG LIST HAS BEEN SUCCESSFULLY FETCHED!",
    /////       "data": true
    /////     }
    /////     
    ///// </remarks>
    ///// <returns>Response Message Object</returns>
    ///// <response code="200">CATALOG LIST HAS BEEN SUCCESSFULLY FETCHED!</response>
    ///// <response code="500">ResponseMessage with Error Description</response>
    //[HttpPost]
    //[Route("ListOfProductCatalog")]
    //[MapToApiVersion("1.0")]
    //[Authorize]
    //public async Task<IActionResult> ListOfProductCatalog([Required] string businessID)
    //{
    //    var reqHeader = Request;
    //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
    //    try
    //    {
    //        rm = new ResponseMessage();
    //        FacebookService fs = new FacebookService("");
    //        var result = fs.GetCatalogsListAsync(businessID);
    //        if (result != null)
    //        {
    //            rm.statusCode = StatusCodes.OK;
    //            rm.message = "CATALOG LIST HAS BEEN SUCCESSFULLY FETCHED";
    //            rm.name = StatusName.ok;
    //            rm.data = result;
    //        }
    //    }
    //    catch (Exception ex)
    //    {

    //        rm.statusCode = StatusCodes.ERROR;
    //        rm.message = ex.Message.ToString();
    //        rm.name = StatusName.invalid;
    //        rm.data = ex.Message.ToString();
    //        await Common.UpdateEventLogsNew("CATALOG LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
    //    }
    //    return Ok(rm);
    //    }
    /// <summary>
    /// GET ALL PRODUCTS FROM PARTICULAR CATALOG
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     catalogID : 713758285029769
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">PRODUCTS HAS BEEN SUCCESSFULLY FETCHED FROM CATALOG!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("GetAllProductsFromCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> GetAllProductsFromCatalog([Required] string catalogID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
            var result = fs.GetAllProductsFromCatalogAsync(catalogID);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCTS HAS BEEN SUCCESSFULLY FETCHED FROM CATALOG";
                rm.name = StatusName.ok;
                rm.data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("PRODUCTS FETCHED FROM CATALOG - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }

    /// <summary>
    /// CREATE SINGLE PRODUCT INTO CATALOG
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "catalogID": "713758285029769",
    ///       "retailerID": "1060",
    ///       "name": "Men's Blue Shirt",
    ///       "description": "Premium cotton shirt, slim fit",
    ///       "url": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1697860526481.jpg",
    ///       "imageUrl": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1697860526481.jpg",
    ///       "brand": "Lee",
    ///       "price": 999,
    ///       "currency": "INR",
    ///       "availability": "in stock"
    ///     }
    /// 
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT HAS BEEN SUCCESSFULLY SAVED INTO CATALOG!",
    ///       "data": true
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">CATALOG LIST HAS BEEN SUCCESSFULLY FETCHED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("CreateSingleProductToCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> CreateSingleProductToCatalog([Required] MetaProduct itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
            var result = fs.CreateProduct(itemData);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT HAS BEEN SUCCESSFULLY SAVED INTO CATALOG";
                rm.name = StatusName.ok;
                rm.data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("PRODUCT SAVE TO CATALOG - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }

    /// <summary>
    /// UPDATE SINGLE PRODUCT INTO CATALOG
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "catalogID": "713758285029769",
    ///       "retailerID": "1060",
    ///       "name": "Men's Blue Shirt",
    ///       "description": "Premium cotton shirt, slim fit",
    ///       "url": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1697860526481.jpg",
    ///       "imageUrl": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1697860526481.jpg",
    ///       "brand": "Lee",
    ///       "price": 999,
    ///       "currency": "INR",
    ///       "availability": "in stock"
    ///     }
    /// 
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT HAS BEEN SUCCESSFULLY UPDATED INTO CATALOG!",
    ///       "data": true
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">CATALOG LIST HAS BEEN SUCCESSFULLY UPDATED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("UpdateSingleProductToCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> UpdateSingleProductToCatalog([Required] string productItemID, MetaProduct itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
            var result = fs.UpdateProduct(productItemID, itemData);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT HAS BEEN SUCCESSFULLY UPDATED INTO CATALOG";
                rm.name = StatusName.ok;
                rm.data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("PRODUCT UPDATE TO CATALOG - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }

    /// <summary>
    /// DELETE SINGLE PRODUCT FROM CATALOG
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     productItemID : 24425472147088795
    /// 
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT HAS BEEN SUCCESSFULLY DELETED FROM CATALOG!",
    ///       "data": true
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">PRODUCT HAS BEEN SUCCESSFULLY DELETED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("DeleteSingleProductFromCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> DeleteSingleProductFromCatalog([Required] string productItemID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
            var result = fs.DeleteProductAsync(productItemID);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT HAS BEEN SUCCESSFULLY DELETED FROM CATALOG";
                rm.name = StatusName.ok;
                rm.data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("PRODUCT DELETED FROM CATALOG - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
        }
    }
}
