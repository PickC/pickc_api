/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2024-09-01
 * Description:
*/
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
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
    public class LookupController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly ILookupBusiness lookupBusiness;
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;
        public LookupController(IConfiguration configuration, ILookupBusiness iResultData, IEventLogBusiness eventLogBusiness, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.lookupBusiness = iResultData;
            this.eventLogBusiness = eventLogBusiness;
            this.env = env;
        }


        /// <summary>
        /// GET a LOOKUP List
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "category": "SMSTYPE"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "LOOK-UP LIST",
        ///       "data": [
        ///         {
        ///           "lookupID": 5169,
        ///           "lookupCode": "BULKSMS",
        ///           "lookupDescription": "BULKSMS",
        ///           "lookupCategory": "SMSTYPE",
        ///           "status": true,
        ///           "mappingCode": "",
        ///           "createdBy": "ADMIN",
        ///           "createdOn": "2024-09-18T09:29:22.88",
        ///           "modifiedBy": "ADMIN",
        ///           "modifiedOn": "2024-09-18T09:29:22.88"
        ///         },
        ///         {
        ///           "lookupID": 5170,
        ///           "lookupCode": "FIREBASE",
        ///           "lookupDescription": "FIREBASE",
        ///           "lookupCategory": "SMSTYPE",
        ///           "status": true,
        ///           "mappingCode": "",
        ///           "createdBy": "ADMIN",
        ///           "createdOn": "2024-09-18T09:29:22.883",
        ///           "modifiedBy": "ADMIN",
        ///           "modifiedOn": "2024-09-18T09:29:22.883"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Lookup Item against the Category </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult List(ParamLookupCategory jsonData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);

                List<Lookup> items = lookupBusiness.GetList(jsonData.category);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST SUCCESSFULLY", reqHeader, controllerURL, jsonData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
            }
            return Ok(rm);

        }




        /// <summary>
        /// Adds/Update an LOOKUP
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///         {
        ///           "lookupID": 0,
        ///           "lookupCode": "Mens ware",
        ///           "lookupDescription": "PRODUCT CATEGORY",
        ///           "lookupCategory": "PRODUCTCATEGORY",
        ///           "status": true,
        ///           "mappingCode": "1849",
        ///           "createdBy": "VENDOR",
        ///           "createdOn": "2024-09-30T14:40:35.437Z",
        ///           "modifiedBy": "VENDOR",
        ///           "modifiedOn": "2024-09-30T14:40:35.437Z"
        ///         }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "LOOK UP ITEM SAVED SUCCESSFULLY!",
        ///       "data": {
        ///         "lookupID": 4023,
        ///         "lookupCode": "Mens ware",
        ///         "lookupDescription": "PRODUCT CATEGORY",
        ///         "lookupCategory": "PRODUCTCATEGORY",
        ///         "status": false,
        ///         "mappingCode": "1849",
        ///         "createdBy": "VENDOR",
        ///         "createdOn": "2024-09-30T14:40:35.437Z",
        ///         "modifiedBy": "VENDOR",
        ///         "modifiedOn": "2024-09-30T14:40:35.437Z"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Save/Update Lookup Item</response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult Add(Lookup item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                CheckToken.IsValidToken(Request, configuration);
                var result = lookupBusiness.SaveLookUp(item);
                if (result != null)
                {
                    var newitem = new Lookup();

                    //newitem = lookupBusiness.GetLookUp(item.LookupCode, item.LookupCategory);

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK UP ITEM SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK UP ITEM SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK UP ITEM SAVED - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK UP ITEM SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove a LOOKUP ID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "lookupID": 4023
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "LOOKUP REMOVED",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Remove Lookup Item against the LookupID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult Remove(ParamLookup itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);

                var result = lookupBusiness.DeleteLookUp(itemData.lookupID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOKUP REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOKUP REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOKUP REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOKUP REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// GET a LOOKUP Item
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "lookupCode": "Mens ware",
        ///       "category": "PRODUCTCATEGORY"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH LOOKUP ITEM",
        ///       "data": {
        ///         "lookupID": 4023,
        ///         "lookupCode": "Mens ware",
        ///         "lookupDescription": "PRODUCT CATEGORY",
        ///         "lookupCategory": "PRODUCTCATEGORY",
        ///         "status": false,
        ///         "mappingCode": "1849",
        ///         "createdBy": "VENDOR",
        ///         "createdOn": "2024-06-17T09:23:06.877",
        ///         "modifiedBy": "VENDOR",
        ///         "modifiedOn": "2024-09-30T14:44:42.433"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Lookup Item against the LookupCode, Category </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("getitem")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetLookup(ParamLookupCode jsonData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);

                var item = lookupBusiness.GetLookUp(jsonData.lookupCode, jsonData.category);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH LOOKUP ITEM";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET LOOKUP ITEM SUCCESSFULLY", reqHeader, controllerURL, jsonData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET LOOKUP ITEM - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET LOOKUP ITEM - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
            }
            return Ok(rm);

        }
        //[HttpPost, Route("list")]
        //[MapToApiVersion("2.0")]
        //public IActionResult Listv2(ParamLookupCategory jsonData)
        //{
        //    var reqHeader = Request;
        //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    //dynamic data = jsonData;
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        List<Lookup> items = lookupBusiness.GetList(jsonData.category);
        //        if (items?.Any() == true)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "LOOK-UP LIST";
        //            rm.name = StatusName.ok;
        //            rm.data = items;
        //            //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
        //            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST SUCCESSFULLY", reqHeader, controllerURL, jsonData, items, StatusName.ok));
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //            //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
        //            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;
        //        this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
        //    }
        //    return Ok(rm);

        //}
        /// <summary>
        /// GET a LOOKUP List By Member
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "category": "SMSTYPE",
        ///       "userID":1060
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "LOOK-UP LIST",
        ///       "data": [
        ///         {
        ///           "lookupID": 5169,
        ///           "lookupCode": "BULKSMS",
        ///           "lookupDescription": "BULKSMS",
        ///           "lookupCategory": "SMSTYPE",
        ///           "status": true,
        ///           "mappingCode": "",
        ///           "createdBy": "ADMIN",
        ///           "createdOn": "2024-09-18T09:29:22.88",
        ///           "modifiedBy": "ADMIN",
        ///           "modifiedOn": "2024-09-18T09:29:22.88"
        ///         },
        ///         {
        ///           "lookupID": 5170,
        ///           "lookupCode": "FIREBASE",
        ///           "lookupDescription": "FIREBASE",
        ///           "lookupCategory": "SMSTYPE",
        ///           "status": true,
        ///           "mappingCode": "",
        ///           "createdBy": "ADMIN",
        ///           "createdOn": "2024-09-18T09:29:22.883",
        ///           "modifiedBy": "ADMIN",
        ///           "modifiedOn": "2024-09-18T09:29:22.883"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Lookup Item against the MemberID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("listbymember")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult ListByMember(ParamLookupByMember jsonData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            List<Lookup> items = new List<Lookup>();
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);

                items = lookupBusiness.GetList(jsonData.category, jsonData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST BY MEMBER SUCCESSFULLY", reqHeader, controllerURL, jsonData, items, StatusName.ok));
                }
                else
                {
                    items = new List<Lookup>();
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST BY MEMBER - NO CONTENT", reqHeader, controllerURL, jsonData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST BY MEMBER - ERROR", reqHeader, controllerURL, jsonData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// GET a LOOKUP List ALL
        /// </summary>
        /// <remarks>
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "LOOK-UP LIST",
        ///       "data": [
        ///         {
        ///           "lookupID": 1000,
        ///           "lookupCode": "VENDOR",
        ///           "lookupDescription": "VENDOR",
        ///           "lookupCategory": "MEMBERTYPE",
        ///           "status": true,
        ///           "mappingCode": "",
        ///           "createdBy": "ADMIN",
        ///           "createdOn": "2023-07-06T05:23:46.017",
        ///           "modifiedBy": "ADMIN",
        ///           "modifiedOn": "2023-07-06T05:23:46.017"
        ///         },
        ///         {
        ///           "lookupID": 1001,
        ///           "lookupCode": "USER",
        ///           "lookupDescription": "USER",
        ///           "lookupCategory": "MEMBERTYPE",
        ///           "status": true,
        ///           "mappingCode": "",
        ///           "createdBy": "ADMIN",
        ///           "createdOn": "2023-07-06T05:23:46.017",
        ///           "modifiedBy": "ADMIN",
        ///           "modifiedOn": "2023-07-06T05:23:46.017"
        ///         }
        ///         ]
        ///    }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Lookup All Items </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpGet, Route("listall")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> ListAll()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);

                List<Lookup> items = lookupBusiness.GetAllList();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                await Common.UpdateEventLogsNew("LOOK-UP LIST ALL - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// GET List of Startup Categories
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "list": "PAYMENTTYPE,DELIVERYCHANNEL,PLATFORMTYPEANDROID,BANNERTYPE,DISCOUNTTYPE"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "LOOK-UP LIST",
        ///       "data": [
        ///         {
        ///           "lookupID": 3000,
        ///           "lookupCode": "AMT",
        ///           "lookupCategory": "DISCOUNTTYPE"
        ///         },
        ///         {
        ///           "lookupID": 3001,
        ///           "lookupCode": "PER",
        ///           "lookupCategory": "DISCOUNTTYPE"
        ///         },
        ///         {
        ///         "lookupID": 3002,
        ///           "lookupCode": "NONE",
        ///           "lookupCategory": "DISCOUNTTYPE"
        ///         },
        ///         {
        ///         "lookupID": 3703,
        ///           "lookupCode": "COD",
        ///           "lookupCategory": "PAYMENTTYPE"
        ///         },
        ///         {
        ///         "lookupID": 3704,
        ///           "lookupCode": "ONLINE",
        ///           "lookupCategory": "PAYMENTTYPE"
        ///         },
        ///         {
        ///         "lookupID": 3921,
        ///           "lookupCode": "SHIPROCKET",
        ///           "lookupCategory": "DELIVERYCHANNEL"
        ///         },
        ///         {
        ///         "lookupID": 3922,
        ///           "lookupCode": "DELHIVERY",
        ///           "lookupCategory": "DELIVERYCHANNEL"
        ///         },
        ///         {
        ///         "lookupID": 3970,
        ///           "lookupCode": "DISCOUNT",
        ///           "lookupCategory": "BANNERTYPE"
        ///         },
        ///         {
        ///         "lookupID": 3971,
        ///           "lookupCode": "NEWARRIVAL",
        ///           "lookupCategory": "BANNERTYPE"
        ///         },
        ///         {
        ///         "lookupID": 3972,
        ///           "lookupCode": "OFFERS",
        ///           "lookupCategory": "BANNERTYPE"
        ///         },
        ///         {
        ///         "lookupID": 3994,
        ///           "lookupCode": "ANDROID",
        ///           "lookupCategory": "PLATFORMTYPEANDROID"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns List of Startup Categories </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("listallforstartup")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ListAllForStartUp(ParamLookupCategories categoryList)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<LookupStartUpList> items = new List<LookupStartUpList>();
                //foreach (var item  in categoryList.list)
                //{
                //    items.AddRange(lookupBusiness.GetListForStartup(item.category));
                //}
                items = lookupBusiness.GetListForStartup(categoryList.list);
                //lookupBusiness.GetAllList();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "LOOK-UP LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST ALL FOR STARTUP SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL FOR STARTUP SUCCESSFULLY", reqHeader, controllerURL, categoryList.list, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST ALL FOR STARTUP - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                    await Common.UpdateEventLogsNew("LOOK-UP LIST ALL FOR STARTUP - NO CONTENT", reqHeader, controllerURL, categoryList.list, null, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                // this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("LOOK-UP LIST ALL FOR STARTUP - ERROR", reqHeader, controllerURL, null, null, rm.message));
                await Common.UpdateEventLogsNew("LOOK-UP LIST ALL FOR STARTUP - ERROR", reqHeader, controllerURL, categoryList.list, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// gets System configuration settings
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "SettingKey": "DELIVERYCHANNEL"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SYSTEM CONFIG SETTING LIST",
        ///       "data": [
        ///         {
        ///           "settingValue": "SHIPROCKET",
        ///           "settingStatus": true
        ///         },
        ///         {
        ///           "settingValue": "DELHIVERY",
        ///           "settingStatus": false
        ///         },
        ///         {
        ///         "settingValue": "EASEBUZZ",
        ///           "settingStatus": false
        ///         },
        ///         {
        ///         "settingValue": "PHONEPE",
        ///           "settingStatus": false
        ///         },
        ///         {
        ///         "settingValue": "RAZORPAY",
        ///           "settingStatus": true
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ParamSystemConfigSetting Object</returns>
        /// <response code="500">ParamSystemConfigSetting with Error Description</response> 
        [HttpPost]
        [Route("systemconfigurationtlist")]
        [MapToApiVersion("1.0")]
        public IActionResult GetSystemConfigurationSettings(ParamSystemConfigSetting itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                List<SystemConfigSetting> items = lookupBusiness.GetSystemConfigurationSettings(itemData.SettingKey);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SYSTEM CONFIG SETTING LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// GENERATE OTP
        /// </summary>
        /// <remarks>
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "GENERATE OTP ",
        ///       "data": "010262"
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns GENERATE OTP </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpGet, Route("generateOTP")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        public IActionResult GenerateOTP()
        {
            //var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var OTPSecretKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppifyOTPKey:SecretKey").Value;

                String OTPValue = utility.Common.GenerateOTP(OTPSecretKey);
                rm.statusCode = StatusCodes.OK;
                rm.message = "GENERATE OTP ";
                rm.name = StatusName.ok;
                rm.data = OTPValue;

                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));



            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH SYSTEM CONFIG SETTING LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

    }
}
