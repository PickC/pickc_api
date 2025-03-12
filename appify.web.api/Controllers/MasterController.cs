using appify.Business.Contract;
using appify.models;
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
    public class MasterController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ICategoryParameterBusiness categoryParameterBusiness;
        private readonly IParameterTypeBusiness parameterTypeBusiness;
        private ResponseMessage rm;


        public MasterController(IConfiguration configuration, ICategoryParameterBusiness categoryParameterBusiness, IParameterTypeBusiness parameterTypeBusiness)
        {
            this.configuration = configuration;
            this.categoryParameterBusiness = categoryParameterBusiness;
            this.parameterTypeBusiness = parameterTypeBusiness;
        }



        /// <summary>
        /// Save/Update Role-Rights
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "roleID": 1010,
        ///       "securableID": 1010,
        ///       "isAdd": true,
        ///       "isEdit": true,
        ///       "isView": true,
        ///       "isDownload": true,
        ///       "isDelete": true,
        ///       "createdBy": 1000,
        ///       "modifiedBy": 1000
        ///     }
        /// 
        /// Sample response JSON :ML
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "CATEGORY PARAMETERS SUCCESSFULLY SAVED",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE/UPDATE WEBPAGE </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        public IActionResult SaveCategoryParameter(CategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.categoryParameterBusiness.Save(itemData);
                if (result == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "CATEGORY PARAMETERS SUCCESSFULLY SAVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Remove the Role-Right
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "roleID": 1010,  
        ///       "securableID": 1010
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "PARAMETER REMOVED!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE THE WEBPAGE </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("remove")]
        [MapToApiVersion("1.0")]
        public IActionResult RemoveCategoryParameter(ParamCategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.categoryParameterBusiness.Delete(itemData.ParameterID, itemData.CategoryID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PARAMETER REMOVED!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }

            return Ok(rm);
        }

        /// <summary>
        /// Get The Role-Right
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "roleID": 1000,  
        ///       "securableID": 1001
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH CATEGORY PARAMETER ITEM!",
        ///       "data": {
        ///       "roleID": 1000,
        ///       "securableID": 1001,
        ///       "isAdd": true,
        ///       "isEdit": true,
        ///       "isView": true,
        ///       "isDelete": true,
        ///       "isDownload": true,
        ///       "createdBy": 0,
        ///       "createdOn": "0001-01-01T00:00:00",
        ///       "modifiedBy": 0,
        ///       "modifiedOn": "0001-01-01T00:00:00",
        ///       "pageLink": "https://appi-fy.ai/dashboard",
        ///       "pageName": "Dashboard",
        ///       "parentName": null
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET THE WEBPAGE </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("get")]
        [MapToApiVersion("1.0")]
        public IActionResult GetCategoryParameter(ParamCategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.categoryParameterBusiness.Get(itemData.ParameterID, itemData.CategoryID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH CATEGORY PARAMETER ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }
        /// <summary>
        /// Get List Of The WebPage
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// (Existing Roles-rights for the Role)
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "securableID": 1001
        ///     }
        /// 
        /// (To Create a New Roles-rights )
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "securableID": 0
        ///     }
        /// 
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH CATEGORY PARAMETER LIST!",
        ///       "data": [
        ///         "roleID": 1000,
        ///         "securableID": 1001,
        ///         "isAdd": false,
        ///         "isEdit": false,
        ///         "isView": false,
        ///         "isDelete": false,
        ///         "isDownload": false,
        ///         "createdBy": 1000,
        ///         "createdOn": "0001-01-01T00:00:00",
        ///         "modifiedBy": 1000,
        ///         "modifiedOn": "0001-01-01T00:00:00",
        ///         "pageLink": "https://appi-fy.ai/dashboard",
        ///         "pageName": "Dashboard",
        ///         "parentName": null
        ///         ],
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET LIST OF WEBPAGE </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        public IActionResult ListCategoryParameters(ParamCategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.categoryParameterBusiness.ListAll(itemData.ParameterID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH CATEGORY PARAMETER LIST!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DISCOUNT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

    }
}
