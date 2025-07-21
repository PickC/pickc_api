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
using Razorpay.Api;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]

    public class ThemeMasterController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IThemeMasterBusiness themeMasterBusiness;
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;


        public ThemeMasterController(IConfiguration configuration, 
                                     IThemeMasterBusiness iResultData, 
                                     IEventLogBusiness eventLogBusiness,
                                     IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.themeMasterBusiness = iResultData;
            this.eventLogBusiness = eventLogBusiness;
            this.env = env;
        }
    /// <summary>
    /// Adds/Update a Template
    /// </summary>
    /// <remarks>
    /// Sample request JSON : 
    /// 
    ///     {
    ///       "templateID": 0,
    ///       "name": "Sky",
    ///       "description": "Sky Blue",
    ///       "banner": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1750741033152",
    ///       "code": "TEMPATE2",
    ///       "isActive": true
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Save/Update TEMPLATE </response>
    /// <response code="500">ResponseMessage with Error Description</response> 

    [HttpPost, Route("savetemplate")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> AddTemplate(TemplateMaster item)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            var result = themeMasterBusiness.SaveTemplate(item);
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "TEMPLATE HAS BEEN SAVED SUCCESSFULLY!";
                rm.name = StatusName.ok;
                rm.data = result;
                await Common.UpdateEventLogsNew("TEMPLATE HAS BEEN SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("TEMPLATE SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
            }

        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
              await Common.UpdateEventLogsNew("TEMPLATE SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

    }
    /// <summary>
    /// Remove a Template
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "templateID": 1001
    ///     }   
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Remove a Template </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("removetemplate")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> RemoveTemplate(long templateID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            var result = themeMasterBusiness.DeleteTemplate(templateID);
            if (result)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "TEMPLATE REMOVED SUCCESSFULLY";
                rm.name = StatusName.ok;
                rm.data = result;
                await Common.UpdateEventLogsNew("TEMPLATE REMOVED SUCCESSFULLY", reqHeader, controllerURL, templateID, result, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("TEMPLATE REMOVED - NO CONTENT", reqHeader, controllerURL, templateID, result, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("TEMPLATE REMOVED - ERROR", reqHeader, controllerURL, templateID, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

        }
    /// <summary>
    /// Get a Template
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "templateID": 1001
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "FETCH THEME ITEM",
    ///       "data": {
    ///         "templateID": 1000,
    ///         "name": "Minimalist",
    ///         "description": "Simple Sleek and morden designs",
    ///         "banner": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1750741033152",
    ///         "code": "TEMPATE1",
    ///         "isActive": true
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Template Item </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("gettemplate")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> GetTemplate(long templateID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            var item = themeMasterBusiness.GetTemplate(templateID);

            if (item != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "FETCH THEME ITEM";
                rm.name = StatusName.ok;
                rm.data = item;
                await Common.UpdateEventLogsNew("FETCH TEMPLATE ITEM SUCCESSFULLY", reqHeader, controllerURL, item, templateID, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("FETCH TEMPLATE ITEM - NO CONTENT", reqHeader, controllerURL, templateID, null, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("FETCH TEMPLATE ITEM - ERROR", reqHeader, controllerURL, templateID, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

        }
    /// <summary>
    /// Get a Template List
    /// </summary>
    /// <remarks>
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED!",
    ///       "data": [
    ///         {
    ///           "templateID": 1000,
    ///           "name": "Minimalist",
    ///           "description": "Simple Sleek and morden designs",
    ///           "banner": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1750741033152",
    ///           "code": "TEMPATE1",
    ///           "isActive": true
    ///         },
    ///         {
    ///           "templateID": 1001,
    ///           "name": "Sky",
    ///           "description": "Sky Blue",
    ///           "banner": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1750741033152",
    ///           "code": "TEMPATE2",
    ///           "isActive": true
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Template List Items </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost, Route("listtemplate")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> TemplateList()
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            List<TemplateMaster> items = themeMasterBusiness.ListAllTemplate();
            if (items?.Any() == true)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED!";
                rm.name = StatusName.ok;
                rm.data = items;
                await Common.UpdateEventLogsNew("TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("TEMPLATE LIST - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("TEMPLATE LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

        }
    /// <summary>
    /// View All Templates List with Themes
    /// </summary>
    /// <remarks>
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED!",
    ///       "data": [
    ///         {
    ///           "templateID": 1000,
    ///           "title": "Minimalist",
    ///           "description": "Simple Sleek and morden designs",
    ///           "banner": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1750741033152",
    ///           "themes": [
    ///             {
    ///               "themeID": 1000,
    ///               "themeName": "Green",
    ///               "themeColor": "#008000",
    ///               "themeJSON": "{\"scaffoldBgColor\":\"rgba(10,10,10)\",\"primaryColor\":\"\",\"primaryLightColor\":\"\",\"textColor\":\"\",\"subtextColor\"    :/\  "\",//\"inputTextcolor\":\"\",\"iconColor\":\"\",\"deleteColor\":\"\",\"inputTextBoxBorderColor\":\"\",\"backgroundBoxColor\":\"\",\"borderColor\" :/\ "\",//\"primaryGradient\":{\"begin\":\"center\",\"end\":\"top\",\"colors\":[\"#ffff\",\"#000\"],\"stops\":[0.512,0.121]}}"
    ///             },
    ///             {
    ///               "themeID": 1001,
    ///               "themeName": "Red",
    ///               "themeColor": "#FF0000",
    ///               "themeJSON": "{\"scaffoldBgColor\":\"rgba(10,10,10)\",\"primaryColor\":\"\",\"primaryLightColor\":\"\",\"textColor\":\"\",\"subtextColor\"    :/\  "\",//\"inputTextcolor\":\"\",\"iconColor\":\"\",\"deleteColor\":\"\",\"inputTextBoxBorderColor\":\"\",\"backgroundBoxColor\":\"\",\"borderColor\" :/\ "\",//\"primaryGradient\":{\"begin\":\"center\",\"end\":\"top\",\"colors\":[\"#ffff\",\"#000\"],\"stops\":[0.512,0.121]}}"
    ///             }
    ///           ]
    ///         }
    ///       ]
    ///     }
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Template List Items </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost, Route("viewalltemplates")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> ViewAllTemplateList()
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            List<TemplatesMaster> items = themeMasterBusiness.ViewAllTemplateList();
            if (items?.Any() == true)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED!";
                rm.name = StatusName.ok;
                rm.data = items;
                await Common.UpdateEventLogsNew("TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("TEMPLATE LIST - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("TEMPLATE LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

        }
    /// <summary>
    /// Get A Template By ThemeID
    /// </summary>
    /// <remarks>
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED!",
    ///       "data": {
    ///         "pages": "{\"lightPages\":[{\"title\":\"Home Page\",\"description\":\"Minimalist\",\"image\":\"https://image1.png\"},{\"title\":\"Search Page\   ", ///\"description\":\"Minimalist\",\"image\":\"https://image1.png\"}]}"
    ///     }
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Template List Items </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost, Route("gettemplatebytheme")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> GetTemplateByTheme(long templateID, long themeID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            var items = themeMasterBusiness.GetTemplateByTheme(templateID, themeID);
            if (items != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED!";
                rm.name = StatusName.ok;
                rm.data = items;
                await Common.UpdateEventLogsNew("TEMPLATE LIST HAS BEEN SUCCESSFULLY FETCHED", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("TEMPLATE LIST - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("TEMPLATE LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

    }
    /// <summary>
    /// Adds/Update a THEME
    /// </summary>
    /// <remarks>
    /// Sample request JSON : 
    /// 
    ///     {
    ///       "themeID": 0,
    ///       "themeName": "Red",
    ///       "themeColor": "#FF0000",
    ///       "darkThemeColor": "",
    ///       "templateID": 1000,
    ///       "themeJSON": "{\"scaffoldBgColor\":\"rgba(10,10,10)\",\"primaryColor\":\"\",\"primaryLightColor\":\"\",\"textColor\":\"\",\"subtextColor\":\"\",///\    "inputTextcolor\":\"\",\"iconColor\":\"\",\"deleteColor\":\"\",\"inputTextBoxBorderColor\":\"\",\"backgroundBoxColor\":\"\",\"borderColor\":\"\",///\   "primaryGradient\":{\"begin\":\"center\",\"end\":\"top\",\"colors\":[\"#ffff\",\"#000\"],\"stops\":[0.512,0.121]}}",
    ///       "themePagesJSON": "{\"lightPages\":[{\"title\":\"Home Page\",\"description\":\"Minimalist\",\"image\":\"https://image1.png\"},{\"title\":\"Search P     age/\",//\"description\":\"Minimalist\",\"image\":\"https://image1.png\"}]}",
    ///       "isThemeAvailable": true,
    ///       "isDark": false,
    ///       "isActive": true
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Save/Update THEME </response>
    /// <response code="500">ResponseMessage with Error Description</response> 

    [HttpPost, Route("savetheme")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> AddTheme(ThemeMaster item)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = themeMasterBusiness.SaveTheme(item);
            if (result!=null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "THEME MASTER SAVED SUCCESSFULLY!";
                rm.name = StatusName.ok;
                rm.data = result;
                await Common.UpdateEventLogsNew("THEME MASTER SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("THEME MASTER SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
            }

        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("THEME MASTER SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

    }
    /// <summary>
    /// Remove a Theme
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "themeID": 1001
    ///     }   
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Remove a Theme </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("removetheme")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> RemoveTheme(long themeID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = themeMasterBusiness.DeleteTheme(themeID);
            if (result)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "THEME REMOVED";
                rm.name = StatusName.ok;
                rm.data = result;
                await Common.UpdateEventLogsNew("THEME REMOVED SUCCESSFULLY", reqHeader, controllerURL, themeID, result, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("THEME REMOVED - NO CONTENT", reqHeader, controllerURL, themeID, result, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("THEME REMOVED - ERROR", reqHeader, controllerURL, themeID, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

        }
    /// <summary>
    /// Get a Theme
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "themeID": 1001
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "FETCH THEME ITEM",
    ///       "data": {
    ///         "themeID": 1000,
    ///         "themeName": "Green",
    ///         "themeColor": "#008000",
    ///         "darkThemeColor": "",
    ///         "templateID": 1000,
    ///         "themeJSON": "{\"scaffoldBgColor\":\"rgba(10,10,10)\",\"primaryColor\":\"\",\"primaryLightColor\":\"\",\"textColor\":\"\",\"subtextColor\":\ "\ ",///\"inputTextcolor\":\"\",\"iconColor\":\"\",\"deleteColor\":\"\",\"inputTextBoxBorderColor\":\"\",\"backgroundBoxColor\":\"\",\"borderColor\":\    " \",///\"primaryGradient\":{\"begin\":\"center\",\"end\":\"top\",\"colors\":[\"#ffff\",\"#000\"],\"stops\":[0.512,0.121]}}",
    ///         "themePagesJSON": "{\"lightPages\":[{\"title\":\"Home Page\",\"description\":\"Minimalist\",\"image\":\"https://image1.png\"},{\"title\":\"Search P   age//\",/\"description\":\"Minimalist\",\"image\":\"https://image1.png\"}]}",
    ///         "isThemeAvailable": true,
    ///         "isDark": false,
    ///         "isActive": true
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Theme Item </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("gettheme")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> GetTheme(long themeID)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = themeMasterBusiness.GetTheme(themeID);

            if (item != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "FETCH THEME ITEM";
                rm.name = StatusName.ok;
                rm.data = item;
                await Common.UpdateEventLogsNew("FETCH THEME SUCCESSFULLY", reqHeader, controllerURL, themeID, item, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("FETCH THEME - NO CONTENT", reqHeader, controllerURL, themeID, null, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("FETCH THEME - ERROR", reqHeader, controllerURL, themeID, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

        }
    /// <summary>
    /// Get a Theme List
    /// </summary>
    /// <remarks>
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "THEME LIST",
    ///       "data": [
    ///         {
    ///           "themeID": 1000,
    ///           "themeName": "Green",
    ///           "themeColor": "#008000",
    ///           "darkThemeColor": "",
    ///           "templateID": 1000,
    ///           "themeJSON": "{\"scaffoldBgColor\":\"rgba(10,10,10)\",\"primaryColor\":\"\",\"primaryLightColor\":\"\",\"textColor\":\"\",\"subtextColor\":\   "\ ",///\"inputTextcolor\":\"\",\"iconColor\":\"\",\"deleteColor\":\"\",\"inputTextBoxBorderColor\":\"\",\"backgroundBoxColor\":\"\",\"borderColor\":\  "\  ",///\"primaryGradient\":{\"begin\":\"center\",\"end\":\"top\",\"colors\":[\"#ffff\",\"#000\"],\"stops\":[0.512,0.121]}}",
    ///           "themePagesJSON": "{\"lightPages\":[{\"title\":\"Home Page\",\"description\":\"Minimalist\",\"image\":\"https://image1.png\"},{\"title\":\ "S earch //Page/\",\"description\":\"Minimalist\",\"image\":\"https://image1.png\"}]}",
    ///           "isThemeAvailable": true,
    ///           "isDark": false,
    ///           "isActive": true
    ///         }
    ///       ]
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Theme List Items </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("listtheme")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> ListTheme()
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<ThemeMaster> items = themeMasterBusiness.ListAllTheme();
            if (items?.Any() == true)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "THEME LIST";
                rm.name = StatusName.ok;
                rm.data = items;
                await Common.UpdateEventLogsNew("THEME LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("THEME LIST - NO CONTENT", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = ex.Message.ToString();
            await Common.UpdateEventLogsNew("THEME LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

    }
    }
}
