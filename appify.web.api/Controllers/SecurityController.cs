using appify.Business.Contract;
using appify.models;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    public partial class SecurityController : ControllerBase
    {
        public readonly IConfiguration Configuration;
        public readonly IRoleRightsBusiness roleRightsBusiness;
        private ResponseMessage rm;
        private readonly IRolesBusiness rolesBusiness;
        private readonly ISecurablesBusiness securablesBusiness;
        public SecurityController(IConfiguration configuration, IRoleRightsBusiness roleRightsBusiness, IRolesBusiness rolesBusiness, ISecurablesBusiness securablesBusiness)
        {
            this.Configuration = configuration;
            this.roleRightsBusiness = roleRightsBusiness;
            this.rolesBusiness = rolesBusiness;
            this.securablesBusiness = securablesBusiness;
        }


        //}
        /// <summary>
        /// Get Role Details
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "roleID": 1000
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES ITEM!",
        ///       "data": {
        ///         "roleID": 1000,
        ///         "roleCode": "ROLE_SUPER_ADMIN",
        ///         "roleDescription": "Has unrestricted access to all system features, settings, and data. Responsible for managing other a",
        ///         "isActive": false,
        ///         "createdBy": 0,
        ///         "createdOn": "2025-01-23T08:40:21.037",
        ///         "modifiedBy": 0,
        ///         "modifiedOn": "0001-01-01T00:00:00"
        ///         "roleRights": [
        ///             {
        ///               "roleID": 1005,
        ///               "securableID": 1001,
        ///               "isAdd": false,
        ///               "isEdit": false,
        ///               "isView": true,
        ///               "isDelete": false,
        ///               "isDownload": false,
        ///               "createdBy": 0,
        ///               "createdOn": "0001-01-01T00:00:00",
        ///               "modifiedBy": 0,
        ///               "modifiedOn": "0001-01-01T00:00:00",
        ///               "pageLink": "https://appify-dashboard-green.vercel.app/overview",
        ///               "pageName": "Overview",
        ///               "parentName": null
        ///             },
        ///             {
        ///               "roleID": 1005,
        ///               "securableID": 1002,
        ///               "isAdd": false,
        ///               "isEdit": false,
        ///               "isView": false,
        ///               "isDelete": false,
        ///               "isDownload": false,
        ///               "createdBy": 0,
        ///               "createdOn": "0001-01-01T00:00:00",
        ///               "modifiedBy": 0,
        ///               "modifiedOn": "0001-01-01T00:00:00",
        ///               "pageLink": "https://appify-dashboard-green.vercel.app/sellers",
        ///               "pageName": "Sellers",
        ///               "parentName": null
        ///             }
        ///            ]
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns ROLE's details along with Role-rights against the RoleID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/get")]
        [MapToApiVersion("1.0")]
        public IActionResult getRole(ParamRole itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.Get(itemData.RoleID);
                if (result != null)
                {
                    

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES ITEM!";
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
        /// Role List
        /// </summary>
        /// <remarks>  
        /// Sample request JSON : with Role Code 
        /// 
        ///     {
        ///       "roleCode": "admin",
        ///       "roleDescription": null
        ///     }
        ///     
        /// Sample request JSON : with Role Description
        /// 
        ///     {
        ///       "roleCode": null,
        ///       "roleDescription": "create"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES ITEM!",
        ///       "data": {
        ///         "roleID": 1000,
        ///         "roleCode": "ROLE_SUPER_ADMIN",
        ///         "roleDescription": "Has unrestricted access to all system features, settings, and data. Responsible for managing other a",
        ///         "isActive": false,
        ///         "createdBy": 0,
        ///         "createdOn": "2025-01-23T08:40:21.037",
        ///         "modifiedBy": 0,
        ///         "modifiedOn": "0001-01-01T00:00:00"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns ROLEs List against the Role Code or Role Description </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/list")]
        [MapToApiVersion("1.0")]
        public IActionResult ListRoles(ParamRoleSearch itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.ListAll(itemData.RoleCode,itemData.RoleDescription);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES LIST!";
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
        /// Roles RecordCount
        /// </summary>
        /// <remarks>   
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES COUNT!",
        ///       "data": 6
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns total Roles Count. </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/recordcount")]
        [MapToApiVersion("1.0")]
        public IActionResult RolesRecordCount()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.GetRolesCount();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES COUNT!";
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
        /// Role List by PageView
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "pageNo": 0,
        ///       "rows": 0
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLES BY PAGE VIEW!",
        ///       "data": [
        ///             {
        ///               "roleID": 1007,
        ///               "roleCode": "Tester",
        ///               "roleDescription": "Tester can create any Role users",
        ///               "isActive": true,
        ///               "createdBy": 0,
        ///               "createdOn": "2025-03-17T10:08:42.81",
        ///               "modifiedBy": 0,
        ///               "modifiedOn": "0001-01-01T00:00:00",
        ///               "roleRights": null
        ///             },
        ///             {
        ///               "roleID": 1005,
        ///               "roleCode": "VIEWER",
        ///               "roleDescription": "VIEWER",
        ///               "isActive": true,
        ///               "createdBy": 1000,
        ///               "createdOn": "2025-01-27T10:57:43.87",
        ///               "modifiedBy": 0,
        ///               "modifiedOn": "0001-01-01T00:00:00",
        ///               "roleRights": null
        ///             }
        ///           ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns List of Roles. </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/pageview")]
        [MapToApiVersion("1.0")]
        public IActionResult RolesByPageView(ParamPageView itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.ListbyPageView(itemData.PageNo, itemData.Rows);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLES BY PAGE VIEW!";
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
        /// Add/Edit Roles
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "roleID":1000,
        ///       "roleCode": "string",
        ///       "roleDescription": "string",
        ///       "isActive": true,
        ///       "createdBy": 0,
        ///       "createdOn": "2025-01-23T08:28:46.248Z",
        ///       "modifiedBy": 0,
        ///       "modifiedOn": "2025-01-23T08:28:46.248Z"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SAVE ROLES ITEM!",
        ///       "data": {
        ///         "roleID": "1000",
        ///         "roleCode": "ROLE_SUPER_ADMIN",
        ///         "roleDescription": "Has unrestricted access to all system features, settings, and data. Responsible for managing other admins and o  ve rseeing the ///entire platform.",
        ///         "isActive": true,
        ///         "createdBy": 0,
        ///         "createdOn": "2025-01-23T08:39:51.511Z",
        ///         "modifiedBy": 0,
        ///         "modifiedOn": "2025-01-23T08:39:51.511Z"
        ///         "roleRights": [
        ///             {
        ///               "roleID": 1005,
        ///               "securableID": 1001,
        ///               "isAdd": false,
        ///               "isEdit": false,
        ///               "isView": true,
        ///               "isDelete": false,
        ///               "isDownload": false,
        ///               "createdBy": 0,
        ///               "createdOn": "0001-01-01T00:00:00",
        ///               "modifiedBy": 0,
        ///               "modifiedOn": "0001-01-01T00:00:00",
        ///               "pageLink": "https://appify-dashboard-green.vercel.app/overview",
        ///               "pageName": "Overview",
        ///               "parentName": null
        ///             },
        ///             {
        ///               "roleID": 1005,
        ///               "securableID": 1002,
        ///               "isAdd": false,
        ///               "isEdit": false,
        ///               "isView": false,
        ///               "isDelete": false,
        ///               "isDownload": false,
        ///               "createdBy": 0,
        ///               "createdOn": "0001-01-01T00:00:00",
        ///               "modifiedBy": 0,
        ///               "modifiedOn": "0001-01-01T00:00:00",
        ///               "pageLink": "https://appify-dashboard-green.vercel.app/sellers",
        ///               "pageName": "Sellers",
        ///               "parentName": null
        ///             }
        ///            ]
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Updated Roles object based on the given input </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/save")]
        [MapToApiVersion("1.0")]
        public IActionResult SaveRole(Roles itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.Save(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SAVE ROLES ITEM!";
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
        /// Remove Role's
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "roleID": 1000,
        ///       "modifiedBy": 1000
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "REMOVE ROLES ITEM!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns a BOOLEAN response if the Delete is successful/failed </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("roles/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult DeleteRole(ParamRoleDeactivate itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.rolesBusiness.Delete(itemData.RoleID, itemData.ModifiedBy);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "REMOVE ROLES ITEM!";
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
        /// Get the User
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "category": "ROLES"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH USER",
        ///       "data": [
        ///         {
        ///           "lookupCategory": "ROLES",
        ///           "lookupCode": "ROLE_ADMIN",
        ///           "lookupDescription": "Has access to most features, including managing users, content, and system settings, but with some restrictions compared to the Super Admin.",
        ///           "mappingCode": ""
        ///         },
        ///         {
        ///           "lookupCategory": "ROLES",
        ///           "lookupCode": "ROLE_EDITOR",
        ///           "lookupDescription": "Manages the creation, editing, and publishing of content, such as blog posts, pages, or articles.",
        ///           "mappingCode": ""
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET MEMBER </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        //
        [HttpPost, Route("roles/AccessType")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAccessType(ParamLookupCategory itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var user = this.rolesBusiness.GetAccessType(itemData.category);
                if (user != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH USER";
                    rm.name = StatusName.ok;
                    rm.data = user;

                    //await Common.UpdateEventLogsNew("FETCH USER SUCCESSFULLY", reqHeader, controllerURL, LookupCategory, user, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = user;
                    //await Common.UpdateEventLogsNew("FETCH USER - NO CONTENT", reqHeader, controllerURL, LookupCategory, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //await Common.UpdateEventLogsNew("FETCH MEMBER - ERROR", reqHeader, controllerURL, LookupCategory, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);
        }



        #region Securables ( ONLY FOR INTERNAL TESTING)

        

        /// <summary>
        /// Save/Update Securable (To be used for Internal / API testing. and not to be exposed to the module functionalities)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "securableID": 1010,
        ///       "pageName": "Operation",
        ///       "pageLink": "https://appify-dashboard-green.vercel.app/overview/operations",
        ///       "parentID": 0
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SECURABLE HAS BEEN SUCCESSFULLY SAVED",
        ///       "data": {
        ///           "securableID": 1010,
        ///           "pageName": "Operation",
        ///           "pageLink": "https://appify-dashboard-green.vercel.app/overview/operations",
        ///           "parentID": 0
        ///         }
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE/UPDATE SECURABLE </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("securable/save")]
        [MapToApiVersion("1.0")]
        public IActionResult SaveSecurables(Securables itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.securablesBusiness.Save(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SECURABLE HAS BEEN SUCCESSFULLY SAVED";
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
        /// Remove the Securable(To be used for Internal / API testing. and not to be exposed to the module functionalities)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "securableID": 1010
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "SECURABLE HAS BEEN REMOVED!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE THE SECURABLE </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("securable/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult RemoveSecurables(ParamSecurableID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.securablesBusiness.Delete(itemData.SecurableID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SECURABLE HAS BEEN REMOVED!";
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
        /// Remove Securables (To be used for Internal / API testing. and not to be exposed to the module functionalities)
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "securableID": 1010
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SECURABLE ITEM!",
        ///       "data": {
        ///         "securableID": 1010,
        ///         "pageName": "Developers",
        ///         "pageLink": "https://appify-dashboard-green.vercel.app/overview/developers",
        ///         "parentID": 1001
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET THE SECURABLE Item </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("securable/get")]
        [MapToApiVersion("1.0")]
        public IActionResult getSecurables(ParamSecurableID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.securablesBusiness.Get(itemData.SecurableID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SECURABLE ITEM!";
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
        /// Get List Of Securable items (To be used for Internal / API testing. and not to be exposed to the module functionalities)
        /// </summary>
        /// <remarks>
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SECURABLE LIST!",
        ///       "data": [
        ///         {
        ///           "securableID": 1001,
        ///           "pageName": "Overview",
        ///           "pageLink": "none",
        ///           "parentID": 0
        ///         },
        ///         {
        ///           "securableID": 1002,
        ///           "pageName": "Sellers",
        ///           "pageLink": "None",
        ///           "parentID": 0
        ///     },]
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">GET LIST OF SECURABLES </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("securable/list")]
        [MapToApiVersion("1.0")]
        public IActionResult ListSecurables()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.securablesBusiness.ListAll();
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SECURABLE LIST!";
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

        #region Role-Rights

        

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
        ///       "message": "ROLE-RIGHTS SUCCESSFULLY SAVED",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">SAVE/UPDATE ROLE-RIGHTS </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("rolerights/save")]
        [MapToApiVersion("1.0")]
        public IActionResult SaveRoleRights(RoleRights itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.roleRightsBusiness.Save(itemData);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ROLE-RIGHTS SUCCESSFULLY SAVED";
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
        ///       "message": "ROLE-RIGHT REMOVED!",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">REMOVE THE ROLE-RIGHT </response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("rolerights/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult RemoveRoleRights(ParamRoleRights itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.roleRightsBusiness.Delete(itemData.RoleID, itemData.SecurableID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ROLE-RIGHT REMOVED!";
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
        ///       "message": "FETCH ROLE-RIGHT ITEM!",
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
        /// <response code="200">GET THE ROLE-RIGHT </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("rolerights/get")]
        [MapToApiVersion("1.0")]
        public IActionResult GetRoleRights(ParamRoleRights itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.roleRightsBusiness.Get(itemData.RoleID, itemData.SecurableID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLE-RIGHT ITEM!";
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
        ///       "roleID": 1001
        ///     }
        /// 
        /// (To Create a New Roles-rights )
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///       "roleID": 0
        ///     }
        /// 
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH ROLE-RIGHT LIST!",
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
        /// <response code="200">GET LIST OF ROLE-RIGHT </response>
        /// <response code="500">Returns Error ResponseMessages </response> 
        [HttpPost, Route("rolerights/list")]
        [MapToApiVersion("1.0")]
        public IActionResult ListRoleRights(ParamRoleRights itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = this.roleRightsBusiness.ListAll(itemData.RoleID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ROLE-RIGHTS LIST!";
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
    }
}
