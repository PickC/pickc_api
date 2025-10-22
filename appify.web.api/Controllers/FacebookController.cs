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
using Azure.Core;
using System.Text;

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
        private readonly IFacebookBusiness facebookBusiness;
        public FacebookController(IEventLogBusiness eventLogBusiness, IConfiguration configuration, IWebHostEnvironment env, IFacebookBusiness facebookBusiness)
        {
            this.eventLogBusiness = eventLogBusiness;
            this.configuration = configuration;
            this.env = env;
            this.facebookBusiness = facebookBusiness;
        }

    /// <summary>
    /// GET A LIST OF VENDOR'S BUSINESS PROFILE
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     vendorID : 1060
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "BUSINESS PROFILE LIST HAS BEEN SUCCESSFULLY FETCHED",
    ///       "data": [
    ///         {
    ///           "mtbid": 100,
    ///           "businessID": "743286941807169",
    ///           "vendorID": 1060,
    ///           "businessName": "AGU Chicha",
    ///           /"accessToken": /"EAAhHakaglF0BPbDVrOZCZBOJcBCnaZAFDGpuVmeEZC01jaRRbaDAqm9lJooJjwZAVqF9K1ESgVDsbOsIiuJZCmswsJ9wM49PmRA5m7Tn3Lpn2XNldWGKmuitrEZA7L   LqWj/gnnqgAk/EaFWSxgVYBf/0fs9cTQOZBYZBTdAQ3NYRCCbmNxBKzexdmD5My5Vq5cmlnZCvNxhzHQHZAG2KUEsWZCPwj38AXEV6ABRsl7EkuUNapVY",
    ///           "pixelID": "",
    ///           "pixelAccessToken": ""
    ///         }
    ///       ]
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">BUSINESS PROFILE LIST HAS BEEN SUCCESSFULLY FETCHED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("VendorBusinessProfileList")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> VendorBusinessProfileList(ParamVendor itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();

            var result = this.facebookBusiness.VendorBusinessProfileList(itemData.VendorID);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "BUSINESS PROFILE LIST HAS BEEN SUCCESSFULLY FETCHED";
                rm.name = StatusName.ok;
                rm.data = result;
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("BUSINESS PROFILE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
        }

    /// <summary>
    /// ADD/UPDATE A BUSINESS PROFILE
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     {
    ///       "mtbid": 0,
    ///       "businessID": "123456789",
    ///       "vendorID": 1060,
    ///       "businessName": "DG Design",
    ///       "userName": "shekhar",
    ///       "password": "kapoor",
    ///       "businessMobileNo": "9898989898",
    ///       "accessToken": "sdghjkl;hgkfjdhfgfghjkl;jghfd",
    ///       "expiryDate": "2025-09-06T08:07:55.844Z",
    ///       "createdBy": 1000,
    ///       "modifiedBy": 1000,
    ///       "isActive": true
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">BUSINESS PROFILE HAS BEEN SUCCESSFULLY CREATED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("SaveBusinessProfile")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> SaveBusinessProfile([Required] MetaBusinessProfile itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            var result = facebookBusiness.SaveBusinessProfile(itemData);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "BUSINESS PROFILE HAS BEEN SUCCESSFULLY CREATED";
                rm.name = StatusName.ok;
                rm.data = result;
            }
            else
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "BUSINESS PROFILE NOT CREATED";
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
            await Common.UpdateEventLogsNew("BUSINESS PROFILE ADD/UPDATE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
        }
    /// <summary>
    /// DELETE BUSINESS PROFILE 
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "mtbid": 101,
    ///       "vendorID": 1060,
    ///       "businessID": "123456789"
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">BUSINESS PROFILE HAS BEEN SUCCESSFULLY DELETED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("DeleteBusinessProfile")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> DeleteBusinessProfile([Required] MetaBusinessProfileDelete itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();

            var result = facebookBusiness.DeleteBusinessProfile(itemData);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "BUSINESS PROFILE HAS BEEN SUCCESSFULLY DELETED!";
                rm.name = StatusName.ok;
                rm.data = result;
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("BUSINESS PROFILE - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }
    /// <summary>
    /// GET A LIST OF VENDOR'S CATALOG
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     vendorID : 1060,
    ///     businessID : "743286941807169"
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "CATALOGS LIST HAS BEEN SUCCESSFULLY FETCHED",
    ///       "data": [
    ///         {
    ///           "mtcid": 100,
    ///           "catalogID": "713758285029769",
    ///           "vendorID": 1060,
    ///           "businessID": "743286941807169",
    ///           "catalogName": "MyCatalog26082025-001"
    ///         },
    ///         {
    ///           "mtcid": 101,
    ///           "catalogID": "2773114199553599",
    ///           "vendorID": 1060,
    ///           "businessID": "743286941807169",
    ///           "catalogName": "MyCatalog26082025"
    ///         },
    ///         {
    ///         "mtcid": 102,
    ///           "catalogID": "1288984646294739",
    ///           "vendorID": 1060,
    ///           "businessID": "743286941807169",
    ///           "catalogName": "MyCatalog2508202577"
    ///         }
    ///       ]
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">CATALOGS LIST HAS BEEN SUCCESSFULLY FETCHED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("VendorCatalogList")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> VendorCatalogList([Required] long VendorID, string BusinessID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();

            var result = this.facebookBusiness.VendorCatalogList(VendorID, BusinessID);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOGS LIST HAS BEEN SUCCESSFULLY FETCHED";
                rm.name = StatusName.ok;
                rm.data = result;
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("CATALOGS - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
        }

    /// <summary>
    /// CREATE A PRODUCT CATALOG
    /// </summary>
    /// <remarks>
    /// 
    /// Sample request JSON :
    /// 
    ///     {
    ///       "mtcid": 0,
    ///       "vendorID": 1060,
    ///       "catalogID": "string",
    ///       "businessID": "743286941807169",
    ///       "catalogName": "MyCatalog06092025",
    ///       "createdBy": 1000,
    ///       "modifiedBy": 1000,
    ///       "isActive": true
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "CATALOG HAS BEEN SUCCESSFULLY CREATED",
    ///       "data": {
    ///         "mtcid": 103,
    ///         "vendorID": 1060,
    ///         "catalogID": "2304271346678634",
    ///         "businessID": "743286941807169",
    ///         "catalogName": "MyCatalog06092025",
    ///         "createdBy": 1000,
    ///         "modifiedBy": 1000,
    ///         "isActive": true
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
    public async Task<IActionResult> CreateaProductCatalog([Required] MetaCatalog itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.CreateCatalog(itemData, facebookBusiness);
                if (result!=null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG HAS BEEN SUCCESSFULLY CREATED";
                rm.name = StatusName.ok;
                rm.data = result;
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
    ///     {
    ///       "mtcid": 103,
    ///       "vendorID": 1060,
    ///       "catalogID": "2304271346678634",
    ///       "businessID": "743286941807169",
    ///       "catalogName": "MyCatalog06092025",
    ///       "createdBy": 1000,
    ///       "modifiedBy": 1000,
    ///       "isActive": true
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "CATALOG HAS BEEN SUCCESSFULLY CREATED",
    ///       "data": {
    ///         "mtcid": 103,
    ///         "vendorID": 1060,
    ///         "catalogID": "2304271346678634",
    ///         "businessID": "743286941807169",
    ///         "catalogName": "MyCatalog06092025",
    ///         "createdBy": 1000,
    ///         "modifiedBy": 1000,
    ///         "isActive": true
    ///       }
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
    public async Task<IActionResult> UpdateaProductCatalog([Required] MetaCatalog itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.EditCatalogAsync(itemData, facebookBusiness);   
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "CATALOG HAS BEEN SUCCESSFULLY UPDATED";
                rm.name = StatusName.ok;
                rm.data = result;
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
    ///     {
    ///       "mtcid": 103,
    ///       "vendorID": 1060,
    ///       "businessID": "743286941807169",
    ///       "catalogID": "2304271346678634"
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
    public async Task<IActionResult> DeleteaProductCatalog([Required] MetaCatalogDelete itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.DeleteCatalogAsync(itemData, facebookBusiness);
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
    ///     {
    ///       "vendorID": 1060,
    ///       "businessID": "743286941807169",
    ///       "catalogID": "713758285029769"
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">PRODUCTS HAS BEEN SUCCESSFULLY FETCHED FROM CATALOG!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("GetAllProductsFromCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> GetAllProductsFromCatalog([Required] ParamProductsFromCatalog itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.GetAllProductsFromCatalogAsync(itemData.CatalogID);
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
    ///       "mtpid": 0,
    ///       "productID": "string",
    ///       "vendorID": 1060,
    ///       "businessID": "743286941807169",
    ///       "catalogID": "2773114199553599",
    ///       "retailerID": 223344,
    ///       "name": "Antibiotics",
    ///       "description": "Antibiotics are powerful medicines used to treat bacterial infections.",
    ///       "url": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/atb3.jpg1750666893990",
    ///       "imageUrl": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/atb3.jpg1750666893990",
    ///       "brand": "I Am Back",
    ///       "price": 100.00,
    ///       "currency": "INR",
    ///       "availability": "in stock",
    ///       "createdBy": 1000,
    ///       "modifiedBy": 1000,
    ///       "isActive": true
    ///     }
    /// 
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT HAS BEEN SUCCESSFULLY SAVED INTO CATALOG",
    ///       "data": {
    ///         "mtpid": 101,
    ///         "productID": "31134623192819677",
    ///         "vendorID": 1060,
    ///         "catalogID": "2773114199553599",
    ///         "retailerID": 223344,
    ///         "createdBy": 1000,
    ///         "modifiedBy": 1000,
    ///         "isActive": true
    ///       }
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
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.CreateProduct(itemData, facebookBusiness);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT HAS BEEN SUCCESSFULLY SAVED INTO CATALOG";
                rm.name = StatusName.ok;
                rm.data = result;
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
    ///       "mtpid": 101,
    ///       "productID": "31134623192819677",
    ///       "vendorID": 1060,
    ///       "businessID": "743286941807169",
    ///       "catalogID": "2773114199553599",
    ///       "retailerID": 223344,
    ///       "name": "Antibiotics",
    ///       "description": "Antibiotics are powerful medicines used to treat bacterial infections. They work by killing bacteria or inhibiting their growth. T h    e /y ///do //not work against viruses, such as those that cause colds, flu, or COVID-19.",
    ///       "url": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/atb3.jpg1750666893990",
    ///       "imageUrl": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/atb3.jpg1750666893990",
    ///       "brand": "I Am Back",
    ///       "price": 100.00,
    ///       "currency": "INR",
    ///       "availability": "in stock",
    ///       "createdBy": 1000,
    ///       "modifiedBy": 1000,
    ///       "isActive": true
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
    public async Task<IActionResult> UpdateSingleProductToCatalog([Required] MetaProduct itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.UpdateProduct(itemData, facebookBusiness);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT HAS BEEN SUCCESSFULLY UPDATED INTO CATALOG";
                rm.name = StatusName.ok;
                rm.data = result;
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
    ///     {
    ///       "mtpid": 101,
    ///       "vendorID": 1060,
    ///       "businessID": "743286941807169",
    ///       "productID": "31134623192819677"
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
    public async Task<IActionResult> DeleteSingleProductFromCatalog([Required] MetaCatalogProuctDelete itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.DeleteProductAsync(itemData, facebookBusiness);
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

    /// <summary>
    /// BULK UPLOAD PRODUCTS TO CATALOG
    /// </summary>
    /// <remarks>
    /// Sample Request JSON :
    /// 
    ///    sourceID : 4143 [(4140-APP), (4141-WEB), (4142-BULK), 4143-SHOPIFY]
    ///    
    ///     {
    ///       "vendorID": 1060,
    ///       "sourceID": 4143,
    ///       "businessID": "743286941807169",
    ///       "catalogID": "787732230439073"
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">ALL THE PRODUCTS HAVE BEEN DELETED FROM CATALOG</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost, Route("BulkUploadAllProductsToCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> BulkUploadAllProductsToCatalog([Required] ParamAllProductsToCatalog itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            List<MetaProduct> itemsData = new List<MetaProduct>();
            itemsData = facebookBusiness.ProductListMeta(itemData.VendorID, itemData.SourceID);
            if(itemsData.Count>0)
            {
                FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
                var result = fs.BulkUploadAllProductsToCatalog(itemsData, itemData.CatalogID, facebookBusiness);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PRODUCTS HAVE BEEN SUCCESSFULLY UPLOADED ON CATALOG";
                    rm.name = StatusName.ok;
                    rm.data = true;//Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(result);
                }
            }
            else
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "NO PRODUCTS";
                rm.name = StatusName.ok;
                rm.data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>("[]");
            }

        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("PRODUCTS UPLOADED ON CATALOG - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }

    /// <summary>
    /// DELETE ALL PRODUCTS FROM CATALOG
    /// </summary>
    /// <remarks>
    /// Sample Request JSON :
    /// 
    ///     {
    ///       "vendorID": 1060,
    ///       "businessID": "743286941807169",
    ///       "catalogID": "787732230439073"
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">ALL THE PRODUCTS HAVE BEEN DELETED FROM CATALOG</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost, Route("DeleteAllProductsFromCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> DeleteAllProductsFromCatalog([Required] ParamProductsFromCatalog itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService(itemData.BusinessID, itemData.VendorID, facebookBusiness);
            var result = fs.DeleteAllProductsFromCatalogAsync(itemData.VendorID, itemData.CatalogID, facebookBusiness);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCTS HAVE BEEN SUCCESSFULLY DELETED FROM CATALOG";
                rm.name = StatusName.ok;
                rm.data = true;
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("PRODUCTS DELETED FROM CATALOG - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }

    [HttpPost, Route("BulkUploadProductFeed")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<FileContentResult> BulkUploadProductFeed([Required] Int64 VendorID, string CatalogID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        byte[] FileContent = null;
        string fileName = $"{VendorID}_CatalogProducts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        try
        {
            rm = new ResponseMessage();
            ExcelReader excelReader = new ExcelReader();
            FileContent = excelReader.GenerateCatalogProductsExcelFile();
            var filePath = excelReader.UploadCatalogProductsExcelToBlobStorage(FileContent, fileName, VendorID);
            FacebookService fs = new FacebookService("",0, facebookBusiness);
            var result = fs.BulkUploadProductFeed("feed1", filePath, CatalogID);

                if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCTS HAVE BEEN SUCCESSFULLY UPLOADED ON CATALOG";
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
            await Common.UpdateEventLogsNew("PRODUCTS UPLOADED ON CATALOG - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return File(FileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //return Ok(rm);
    }

    /// <summary>
    /// SendPurchaseEvent
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
    ///       "message": "SendPurchaseEvent",
    ///       "data": {
    ///         "events_received": 1,
    ///         "messages": [],
    ///         "fbtrace_id": "AnoKvjVdzgCZ7sVC_87Z15d"
    ///       }
    ///     }
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">SendPurchaseEvent!</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost]
    [Route("SendPurchaseEvent")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> SendPurchaseEvent()
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("",0, facebookBusiness);
            var result = fs.SendPurchaseEventAsync("24209911681952804", "EAAKgNLTO7VMBPawT1FsUEoF01u8w3gYRsgLERBxHMFJyupPmBFZC6fiZA3CMioXU5KY0KaG3WgXxmGeHYZAzOx5fBQqJvqtzkKkM4DevZBY73ZCX45Rhm2ZBfvnofTPyzMjKu9ojZCgPUOZCL9QjIhrbGTPDoOCj48SrkhrvgvauGZAH4GlVWjOrgrbNryAJnfJFjlQZDZD");
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "SendPurchaseEvent";
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
            await Common.UpdateEventLogsNew("SendPurchaseEvent - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);
    }

    /// <summary>
    /// Assign Instagram Account to Catalog
    /// </summary>
    /// <remarks>
    /// Sample Request JSON:
    /// 
    /// 
    /// Sample Response JSON:
    /// 
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">INSTAGRAM ACCOUNT HAS BEEN SUCCESSFULLY ASSINED!</response>
    /// <response code="500">ResponseMessage with Error Description</response>

    [HttpPost, Route("")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> AssignInstagramToCatalogAsync([Required] InstagramAccount itemData)
    {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                FacebookService fs = new FacebookService("743286941807169", 1060, facebookBusiness);
                //var rr = fs.GetInstagramBusinessAccountIdAsync(); 
                //var result = fs.AttachCatalogToFacebookPageAsync("707880625751881","1049018633973735");
                var result = fs.AssignCatalogToPageAsync("1850104765568669", "707880625751881");
                //var result = fs.AssignInstagramToCatalogAsync2(itemData, facebookBusiness);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "INSTAGRAM ACCOUNT HAS BEEN SUCCESSFULLY ASSINED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("INSTAGRAM ACCOUNT ASSINED - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }
        private static List<T> getVal<T>()
        {
            List<T> list = new List<T>();
            IList<T> list2 = new List<T>();
            //list.Where
            return list;
        }
        //[HttpPost]
        //[Route("SendPurchaseEvent2")]
        //[MapToApiVersion("1.0")]
        //public async Task<IActionResult> ReceiveEvent([FromBody] EventRequest req)
        //{
        //    HttpClient httpClient= new HttpClient();
        //    string pixelId = "24209911681952804";
        //    string accessToken = "EAAKgNLTO7VMBPawT1FsUEoF01u8w3gYRsgLERBxHMFJyupPmBFZC6fiZA3CMioXU5KY0KaG3WgXxmGeHYZAzOx5fBQqJvqtzkKkM4DevZBY73ZCX45Rhm2ZBfvnofTPyzMjKu9ojZCgPUOZCL9QjIhrbGTPDoOCj48SrkhrvgvauGZAH4GlVWjOrgrbNryAJnfJFjlQZDZD";
        //    try
        //    {
        //        var eventId = req.EventId ?? Guid.NewGuid().ToString();
        //        var eventTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        //        // Build user_data with hashed fields where possible
        //        var userData = new Dictionary<string, object>();
        //        if (!string.IsNullOrEmpty(req.Email)) userData["em"] = new[] { HashSha256(req.Email) };
        //        if (!string.IsNullOrEmpty(req.Phone)) userData["ph"] = new[] { HashSha256(req.Phone) };
        //        if (!string.IsNullOrEmpty(req.Fbp)) userData["fbp"] = req.Fbp;
        //        if (!string.IsNullOrEmpty(req.Fbc)) userData["fbc"] = req.Fbc;

        //        // Add client IP and UA if present (from server)
        //        userData["client_ip_address"] = HttpContext.Connection.RemoteIpAddress?.ToString();
        //        userData["client_user_agent"] = Request.Headers["User-Agent"].ToString();

        //        var evt = new
        //        {
        //            event_name = req.EventName,
        //            event_time = eventTime,
        //            event_id = eventId,
        //            action_source = "website",
        //            user_data = userData,
        //            custom_data = new
        //            {
        //                content_ids = req.ContentIds,
        //                contents = req.Contents,
        //                currency = req.Currency ?? "USD",
        //                value = req.Value
        //            }
        //        };

        //        var payload = new { data = new[] { evt } };
        //        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        //        var url = $"https://graph.facebook.com/v14.0/{pixelId}/events?access_token={accessToken}";

        //        var response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        //        var respText = await response.Content.ReadAsStringAsync();

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            // log for retry later
        //            // save to DB or queue
        //            return StatusCode((int)response.StatusCode, respText);
        //        }

        //        return Ok(respText);
        //    }
        //    catch (Exception ex)
        //    {
        //        // log exception
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        private static string HashSha256(string input)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input.Trim().ToLower()));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
    }
}
