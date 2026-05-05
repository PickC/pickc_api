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
    public class MasterController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ICategoryParameterBusiness categoryParameterBusiness;
        private readonly IParameterTypeBusiness parameterTypeBusiness;
        private readonly IDriverBusiness driverBusiness;
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;


        public MasterController(IConfiguration configuration, ICategoryParameterBusiness categoryParameterBusiness, IParameterTypeBusiness parameterTypeBusiness, IDriverBusiness driverBusiness, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.categoryParameterBusiness = categoryParameterBusiness;
            this.parameterTypeBusiness = parameterTypeBusiness;
            this.driverBusiness = driverBusiness;
            this.env = env;
        }

        #region Category Parameters

        
        /// <summary>
        /// Save/Update Category Parameter
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "parameterID": 0,
        ///       "categoryID": 0,
        ///       "parameterName": "string",
        ///       "isActive": true,
        ///       "createdBy": 0,
        ///       "createdOn": "2025-03-24T04:57:01.080Z",
        ///       "modifiedBy": 0,
        ///       "modifiedOn": "2025-03-24T04:57:01.080Z",
        ///       "category": "string",
        ///       "parentID": 0,
        ///       "parentCategory": "string"
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
        /// <response code="200">SAVE/UPDATE CATEGORY PARAMETERS </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("categoryparameter/save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult SaveCategoryParameter(CategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
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
        /// Remove the Category Parameter
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///       "parameterID": 0,
        ///       "categoryID": 0
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "CATEGORY PARAMETER REMOVED!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE THE CATEGORY PARAMETERS </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("categoryparameter/remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult RemoveCategoryParameter(ParamCategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
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
        /// Get The Category Parameter
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     {
        ///       "parameterID": 0,
        ///       "categoryID": 0
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH CATEGORY PARAMETER ITEM!",
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET THE CATEGORY PARAMETER </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("categoryparameter/get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetCategoryParameter(ParamCategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
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
        /// Get List Of The CATEGORY PARAMETERS
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///        {
        ///          "categoryID": 0
        ///        }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH CATEGORY PARAMETER LIST!",
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET LIST OF CATEGORY PARAMETERS </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("categoryparameter/list")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult ListCategoryParameters(ParamCatID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.categoryParameterBusiness.ListAll(itemData.categoryID);
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

        #endregion


        #region Parameter Types

        /*
        
        /// <summary>
        /// Save/Update Parameter Type
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "parameterID": 0,
        ///       "isMultipleValue": true,
        ///       "parameterName": "string"
        ///     }
        /// 
        /// Sample response JSON :ML
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "PARAMETER TYPE SUCCESSFULLY SAVED",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE/UPDATE PARAMETER TYPE</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("parametertype/save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult SaveParameterType(ParameterType itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.parameterTypeBusiness.Save(itemData);
                if (result == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PARAMETER TYPE SUCCESSFULLY SAVED";
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
        /// Remove the Parameter Type
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "parameterID": 0,
        ///       "parameterValue": "string"
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
        /// <response code="200">REMOVE THE PARAMETER TYPE</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("parametertype/remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult RemoveParameterType(ParamCategoryParameterTypes itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.parameterTypeBusiness.Delete(itemData.ParameterID, itemData.ParameterValue);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PARAMETER TYPE REMOVED!";
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
        /// Get The Parameter Type
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "parameterID": 0,
        ///       "parameterValue": "string"
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH PARAMETER TYPE ITEM!",
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET THE PARAMETER TYPE </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("parametertype/get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetParameterType(ParamCategoryParameterTypes itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.parameterTypeBusiness.Get(itemData.ParameterID, itemData.ParameterValue);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PARAMETER TYPE ITEM!";
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
        /// Get List Of The Parameter Type
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "parameterID": 0,
        ///       "categoryID": 0
        ///     }
        /// 
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH CATEGORY PARAMETER LIST!",
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET LIST OF PARAMETER TYPES </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("parametertype/list")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult ListParameterType(ParamCategoryParameter itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.parameterTypeBusiness.ListAll(itemData.ParameterID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PARAMETER TYPES LIST!";
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
        */

        #endregion


        #region Available Drivers

        /// <summary>
        /// Get all on-duty available drivers with current GPS location and vehicle details
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/master/drivers/available
        ///     GET /api/master/drivers/available?vehicleGroupId=1000
        ///
        /// Sample response JSON:
        ///
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "AVAILABLE DRIVERS FETCHED SUCCESSFULLY",
        ///       "data": [
        ///         {
        ///           "driverId": "D001",
        ///           "driverName": "Ravi Kumar",
        ///           "vehicleGroupId": 1000,
        ///           "vehicleGroupName": "Mini",
        ///           "vehicleTypeId": 1300,
        ///           "vehicleTypeName": "Open",
        ///           "vehicleNumber": "TS09EA1234",
        ///           "currentLatitude": 17.385044,
        ///           "currentLongitude": 78.486671
        ///         }
        ///       ]
        ///     }
        ///
        /// </remarks>
        /// <param name="vehicleGroupId">Optional: 1000=Mini, 1001=Small, 1002=Medium, 1003=Large</param>
        /// <returns>List of available drivers with GPS and vehicle details</returns>
        /// <response code="200">Returns available drivers (empty array if none available)</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Server error</response>
        [HttpGet, Route("drivers/available")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetAvailableDrivers([FromQuery] int? vehicleGroupId = null)
        {
            try
            {
                rm = new ResponseMessage();
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.driverBusiness.GetAvailableDrivers(vehicleGroupId);
                rm.statusCode = StatusCodes.OK;
                rm.message = "AVAILABLE DRIVERS FETCHED SUCCESSFULLY";
                rm.name = StatusName.ok;
                rm.data = result;
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

        #endregion

    }
}
