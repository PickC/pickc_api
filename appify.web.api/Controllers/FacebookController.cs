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

    /// <summary>
    /// BULK UPLOAD PRODUCTS TO CATALOG
    /// </summary>
    /// <remarks>
    /// Sample Request JSON :
    /// 
    ///    vendorID : 1060,
    ///    sourceID : 4143 [(4140-APP), (4141-WEB), (4142-BULK), 4143-SHOPIFY]
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">ALL THE PRODUCTS HAVE BEEN DELETED FROM CATALOG</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost, Route("BulkUploadAllProductsToCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> BulkUploadAllProductsToCatalog([Required] Int64 VendorID, Int16 SourceID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            List<MetaProduct> itemsData = new List<MetaProduct>();
            itemsData = facebookBusiness.ProductListMeta(VendorID, SourceID);
            if(itemsData.Count>0)
            {
                FacebookService fs = new FacebookService("");
                var result = fs.BulkUploadAllProductsToCatalog(itemsData);
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
    ///    catalogID : 713758285029769
    ///     
    /// </remarks>
    /// <returns>Response Message Object</returns>
    /// <response code="200">ALL THE PRODUCTS HAVE BEEN DELETED FROM CATALOG</response>
    /// <response code="500">ResponseMessage with Error Description</response>
    [HttpPost, Route("DeleteAllProductsFromCatalog")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> DeleteAllProductsFromCatalog([Required] string catalogID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            FacebookService fs = new FacebookService("");
            var result = fs.DeleteAllProductsFromCatalogAsync(catalogID);
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
    public async Task<FileContentResult> BulkUploadProductFeed([Required] Int64 VendorID)
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
            FacebookService fs = new FacebookService("");
            var result = fs.BulkUploadProductFeed("feed1", filePath);

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
            FacebookService fs = new FacebookService("");
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
