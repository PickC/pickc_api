using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]

    public class WebAdminController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IProductBusiness productBusiness;
        private readonly IMemberBusiness memberBusiness;
        private ResponseMessage rm;

        public WebAdminController(IConfiguration configuration, IMemberBusiness memberBusiness, IProductBusiness product, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.productBusiness = product;
            this.memberBusiness = memberBusiness;
            this.eventLogBusiness = eventLogBusiness;

        }
        /// <summary>
        /// FETCH PRODUCT LIST
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH PRODUCT LIST",
        ///       "data": [
        ///         {
        ///           "vendorName": null,
        ///           "categoryDescription": null,
        ///           "productID": 1000,
        ///           "vendorID": 1000,
        ///           "productName": "Linen Sarees ",
        ///           "description": "Linen wear",
        ///           "category": 3607,
        ///           "brand": "Dhaage Life",
        ///           "size": "",
        ///           "color": "Multi colour ",
        ///           "uom": 3500,
        ///           "weight": 0,
        ///           "priceID": 1000,
        ///           "currency": "INR",
        ///           "imageID": 1000,
        ///           "isActive": true,
        ///           "isAvailable": true,
        ///           "stockQty": 0,
        ///           "createdOn": null,
        ///           "modifiedOn": null,
        ///           "hsnCode": null,
        ///           "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///           "isNew": false
        ///         } 
        ///         ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost]
        [Route("products")]
        [MapToApiVersion("1.0")]
        public IActionResult ListAllProducts() {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<ProductWeb> items = productBusiness.ListAll();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// FETCH VENDOR LIST
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH VENDOR LIST",
        ///           "data": [
        ///             {
        ///               "userID": 1008,
        ///               "emailID": "kvr210885@gmail.com",
        ///               "mobileNo": "8682944609",
        ///               "password": "Entombed@25",
        ///               "firstName": "Kalyan",
        ///               "lastName": "KVR",
        ///               "memberType": 1001,
        ///               "otp": "604174",
        ///               "isOTPSent": true,
        ///               "otpSentDate": "2023-09-13T12:35:28.417",
        ///               "isResendOTP": false,
        ///               "isOTPVerified": true,
        ///               "isEmailVerified": true,
        ///               "isActive": true,
        ///               "createdOn": "2023-09-13T00:05:29.93",
        ///               "profilePhoto": "image_cropper_1694677575946.jpg",
        ///               "token": "¦iät\u0006\tOŸ\u0005c `\u0015\u001f",
        ///               "platformType": 0,
        ///               "parentID": 1004,
        ///               "isRegisteredByMobile": true,
        ///               "isOnlinePaymentEnabled": true,
        ///               "isEnterprise": null,
        ///               "isEcommerce": null,
        ///               "isWelcomeEmail": null
        ///             }
        ///             ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Vendor List Items </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost]
        [Route("vendors")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ListAllVendors()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<Member> items = memberBusiness.GetAllMembers();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("FETCH VENDOR LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                    await Common.UpdateEventLogsNew("FETCH VENDOR LIST - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                ////this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH VENDOR LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// FETCH VENDOR'S DETAIL
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1060
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "VENDOR'S DETAIL",
        ///       "data": {
        ///         "userID": 1060,
        ///         "emailID": "kiran76@gmail.com",
        ///         "mobileNo": "6382014003",
        ///         "password": "Appify@123",
        ///         "firstName": "I Am Back",
        ///         "lastName": "appify",
        ///         "memberType": 1000,
        ///         "otp": "731885",
        ///         "isOTPSent": true,
        ///         "otpSentDate": "2023-11-01T13:20:59.313",
        ///         "isResendOTP": false,
        ///         "isOTPVerified": true,
        ///         "isEmailVerified": false,
        ///         "isActive": true,
        ///         "createdOn": "2023-11-01T00:51:01.6",
        ///         "profilePhoto": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/1726647603113",
        ///         "token": "dQy98vnwSRaGmf4VpUzLfN:APA91bGBvqzspXLG0J3nKdPOIvC3-HQkdz-///l ok xmtplx04cmCEcnbkubosRxIR2fjGKrwa2YpqF-4LfwV68dqPwRzISIhUH_5zTd8DMxIT4x1yg4W3WFDnbZidpA0sVqnCz8e_4CZEad",
        ///         "platformType": 0,
        ///         "parentID": 0,
        ///         "isRegisteredByMobile": true,
        ///         "isOnlinePaymentEnabled": true,
        ///         "isEnterprise": false,
        ///         "isEcommerce": false,
        ///         "isWelcomeEmail": true
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns VENDOR'S DETAILS against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost]
        [Route("vendordetails")]
        [MapToApiVersion("1.0")]
        public IActionResult GetVendorDetails(ParamMemberUserID itemData )
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            Member item = new Member();
            try
            {
                rm = new ResponseMessage();
                item = memberBusiness.GetMember(itemData.userID);
                if (item!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR'S DETAIL";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDORDETAILS FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    item = new Member();
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = item;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDORDETAILS FETCH PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDORDETAILS FETCH PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }


        //[HttpPost]
        //[Route("customersbyvendor")]
        //public IActionResult CustomersByVendor(ParamMemberUserID itemData)
        //{
        //    Member item = new Member();
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        item = memberBusiness.GetAllMembers();
        //        if (item != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "FETCH PRODUCT LIST";
        //            rm.name = StatusName.ok;
        //            rm.data = item;
        //        }
        //        else
        //        {
        //            item = new Member();
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = item;
        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;
        //    }
        //    return Ok(rm);

        //}


    }
}
