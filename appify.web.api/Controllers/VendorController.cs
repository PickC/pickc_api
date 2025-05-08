using appify.models;
using appify.utility;
using appify.Business;
using appify.Business.Contract;
using appify.models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using NPOI;
using NPOI.OOXML;
using NPOI.SS.UserModel;
using System.Text;
using NPOI.XSSF.UserModel;
using Razorpay.Api;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static appify.models.NotificationType;
using appify.audit.service;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class VendorController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly ICustomerBusiness customerBusiness;
        private readonly IMemberBusiness memberBusiness;
        private readonly ILookupBusiness lookupBusiness;
        private readonly IProductBusiness productBusiness;
        private readonly IProductPriceBusiness priceBusiness;
        private readonly IProductImageBusiness imageBusiness;
        private readonly IBulkImportedProductBusiness bulkImportedProductBusiness;
        private readonly IWebHostEnvironment env;
        private readonly INotificationBusiness notificationBusiness;
        private readonly IVendorWebModuleBusiness vendorWebModuleBusiness;
        private readonly IAuditService auditService;
        private ResponseMessage rm;
        public VendorController(IConfiguration configuration,
                                ICustomerBusiness customerBusiness,
                                IEventLogBusiness eventLogBusiness,
                                IMemberBusiness iResultData,
                                ILookupBusiness lookupBusiness,
                                IProductBusiness productBusiness,
                                IProductPriceBusiness priceBusiness,
                                IProductImageBusiness imageBusiness,
                                IWebHostEnvironment env, 
                                INotificationBusiness IResultData, 
                                IVendorWebModuleBusiness vendorWebModuleBusiness,
                                IBulkImportedProductBusiness bulkImportedProductBusiness,
                                IAuditService auditService)
        {
            this.configuration = configuration;
            this.customerBusiness = customerBusiness;
            this.eventLogBusiness = eventLogBusiness;
            this.memberBusiness = iResultData;
            this.lookupBusiness = lookupBusiness;
            this.productBusiness = productBusiness;
            this.priceBusiness = priceBusiness;
            this.imageBusiness = imageBusiness;
            this.bulkImportedProductBusiness = bulkImportedProductBusiness;
            this.env = env;
            this.notificationBusiness = IResultData;
            this.vendorWebModuleBusiness = vendorWebModuleBusiness;
            this.auditService = auditService;
        }
        /// <summary>
        /// gets Product items information based on Vendor ID
        /// </summary>
        /// <remarks>
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.0 (DEFAULT version)
        /// 
        /// Description : Gets Product items information based on Vendor ID 
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1847
        ///     }
        ///     
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.1
        /// 
        /// Description : Gets Product List New Item information based on Vendor ID
        /// 
        /// -----------------------------------------------------------------------------------------
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1505
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///	    [
        ///		    {
        ///          	"productID": 1315,
        ///          	"vendorID": 0,
        ///          	"productName": "ELEGANT HOODY ",
        ///          	"category": 3713,
        ///          	"brand": "Polo",
        ///          	"price": 1429,
        ///          	"imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1702444422717.jpg",
        ///          	"isNew": false,
        ///          	"breadCrumb": "Clothing & Accessories>>Girls>>Unstitched Fabrics>>Trousers"
        ///    	    }
        ///	    ]
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("productlist")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetVendorProductsList(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<MemberProduct> items = customerBusiness.ProductListNew(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost]
        [Route("productlist")]
        [MapToApiVersion("1.1")]
        [Authorize]
        public IActionResult GetVendorProductsLists(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<MemberProduct> items = customerBusiness.ProductListNew(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// LIST of Vendor Banners By VendorID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "MemberID":1003
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///         "statusCode": 200,
        ///         "name": "SUCCESS_OK",
        ///         "message": "FETCH MEMBER BANNER ITEM!",
        ///         "data": [
        ///           {
        ///             "bannerID": 1000,
        ///             "memberID": 1003,
        ///             "bannerName": "KIA Banner",
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///             "bannerType": 2,
        ///             "startDate": "2024-04-29T09:42:11.443",
        ///             "endDate": "2024-05-19T09:42:11.443",
        ///             "isCancel": false
        ///           }
        ///         ]
        ///    }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("banner/listbyvendor")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult VendorBannerListByVendor(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.memberBusiness.memberBannerListByVendor(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH BY VENDOR MEMBER BANNER ITEM!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH BY VENDOR MEMBER BANNER ITEM SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH BY VENDOR MEMBER BANNER - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH BY VENDOR MEMBER BANNER - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
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
        [Authorize]
        public IActionResult GetSystemVendorConfigurationSettings(ParamSystemConfigSetting itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
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
        [Authorize]
        public async Task<IActionResult> ListAllForStartUpVendor(ParamLookupCategories categoryList)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
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
        /// Remove an Address
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "productID": 1003
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH PRODUCT",
        ///       "data": {
        ///         "productID": 1003,
        ///         "productName": "White Round neck Tshirts",
        ///         "description": "plain white t shirts round neck full and half sleeves ",
        ///         "category": 3608,
        ///         "brand": "Qikink Fashion ",
        ///         "color": "white ",
        ///         "uom": 3500,
        ///         "currency": "INR",
        ///         "isAvailable": true,
        ///         "hsnCode": null,
        ///         "isNew": false,
        ///         "prices": [
        ///           {
        ///             "priceID": 1007,
        ///             "price": 400,
        ///             "discount": 0,
        ///             "discountType": 0,
        ///             "effectiveDate": "2023-09-06T00:00:00",
        ///             "size": "M",
        ///             "stock": 9,
        ///             "weight": null
        ///           },
        ///           {
        ///             "priceID": 1008,
        ///             "price": 800,
        ///             "discount": 0,
        ///             "discountType": 0,
        ///             "effectiveDate": "2023-09-06T00:00:00",
        ///             "size": "L",
        ///             "stock": 10,
        ///             "weight": null
        ///           },
        ///           {
        ///         "priceID": 1009,
        ///             "price": 900,
        ///             "discount": 0,
        ///             "discountType": 0,
        ///             "effectiveDate": "2023-09-06T00:00:00",
        ///             "size": "XL",
        ///             "stock": 22,
        ///             "weight": null
        ///           }
        ///         ],
        ///         "images": [
        ///           {
        ///             "imageID": 1009,
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693993789689.jpg"
        ///           }
        ///         ]
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GET PRODUCT - SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("getitemnew")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetProductNewVendor(ParamProduct itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.productBusiness.GetProductNew(itemData.productID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT - SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                    await Common.UpdateEventLogsNew("GET PRODUCT - SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                    await Common.UpdateEventLogsNew("GET PRODUCT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("GET PRODUCT - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Get Product Stock by Price
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///          
        ///     {
        ///         "priceID": "1019, 1020, 1021, 1022"
        ///     }
        ///
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH STOCK BY PRICEID",
        ///       "data": [
        ///         {
        ///           "stock": 6,
        ///           "priceID": 1019
        ///         },
        ///         {
        ///           "stock": 4,
        ///           "priceID": 1020
        ///         },
        ///         {
        ///         "stock": 7,
        ///           "priceID": 1021
        ///         },
        ///         {
        ///         "stock": 6,
        ///           "priceID": 1022
        ///         }
        ///       ]
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH STOCK BY PRICEID!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("stock/getbyprice")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetStockByPriceIDVendor(ParamPriceID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                //List<StockByPriceID> stockitem = new List<StockByPriceID>();
                //int[] PriceIDs = itemData.PriceID.Split(',').Select(int.Parse).ToArray();
                //foreach (var priceid in PriceIDs)
                //{
                //    stockitem.Add(this.productBusiness.GetStockByPriceID(priceid));
                // }
                var stockitem = this.productBusiness.GetStockByPriceID(itemData.PriceID);
                if (stockitem != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH STOCK BY PRICEID";
                    rm.name = StatusName.ok;
                    rm.data = stockitem;
                    await Common.UpdateEventLogsNew("FETCH STOCK BY PRICEID", reqHeader, controllerURL, stockitem, stockitem, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("FETCH STOCK BY PRICEID - NO CONTENT", reqHeader, controllerURL, stockitem, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("FETCH STOCK BY PRICEID - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Get Featured Categories
        /// </summary>
        /// <remarks>
        /// Sample response JSON :
        /// 
        ///         {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH FEATURED CATEGORIES",
        ///           "data": [
        ///             {
        ///               "categoryID": 1000,
        ///               "category": "Beauty",
        ///               "parentID": 0,
        ///               "isEnabled": false,
        ///               "hierarchyLevel": 1
        ///             },
        ///             {
        ///               "categoryID": 1001,
        ///               "category": "Health and Personal Care",
        ///               "parentID": 0,
        ///               "isEnabled": false,
        ///               "hierarchyLevel": 1
        ///             },
        ///       ]
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH FEATURED CATEGORIES!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("featuredcategories/get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetFeaturedategoriesVendor(ParamMemberVendorID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.productBusiness.GetFeaturedCategories(itemData.userID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH FEATURED CATEGORIES";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    await Common.UpdateEventLogsNew("FETCH FEATURED CATEGORIES", reqHeader, controllerURL, item, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("FETCH FEATURED CATEGORIES - NO CONTENT", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("FETCH SELECTED PARENT CATEGORIES - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }



        [HttpPost("uploadProductExcel")]
        public IActionResult ImportProducts([FromForm] ParamExcelUpload itemData)
        {
            ExcelReader reader = new ExcelReader();

            rm = new ResponseMessage();

            var result = false;

            if (itemData.ExcelFile == null || itemData.ExcelFile.Length == 0)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "No file uploaded";
                rm.name = StatusName.ok;
                rm.data = "No file uploaded";

                return Ok(rm);
            }

            if (!Path.GetExtension(itemData.ExcelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "Only .xlsx files are allowed";
                rm.name = StatusName.ok;
                rm.data = "Only .xlsx files are allowed";

                return Ok(rm);


            }


            try
            {
                var products = reader.ReadExcel(itemData.ExcelFile.OpenReadStream(), itemData.VendorID);
                //var products = reader.ReadExcel(filePath, itemData.VendorID);


                if (products.Count > 0)
                {
                    result = bulkImportedProductBusiness.SaveBulkImportedProducts(products);
                }
                if(result)
                {
                    var rsltVal = bulkImportedProductBusiness.SaveBulkImportedProductsToMain(itemData.VendorID);
                    if (rsltVal)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = $"File Processed Successfully with total Count {products.Count.ToString()}";
                        rm.name = StatusName.ok;
                        rm.data = products;
                    }
                }


            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = $"Error processing file: {ex.Message}";
                rm.name = StatusName.invalid;
                rm.data = $"Error processing file: {ex.Message}";

            }

            return Ok(rm);
        }

        //public async Task DownloadGoogleDriveImagesAsync(List<Product> products)
        //{
        //    var httpClient = new HttpClient();

        //    foreach (var product in products)
        //    {
        //        foreach (var imageUrl in new[] { product.Image1, product.Image2 })
        //        {
        //            if (!string.IsNullOrEmpty(imageUrl))
        //            {
        //                var fileId = ExtractGoogleDriveFileId(imageUrl);
        //                if (!string.IsNullOrEmpty(fileId))
        //                {
        //                    var downloadUrl = $"https://drive.google.com/uc?export=download&id={fileId}";
        //                    var bytes = await httpClient.GetByteArrayAsync(downloadUrl);
        //                    var filename = $"{Guid.NewGuid()}.jpg"; // or derive from product data

        //                    await File.WriteAllBytesAsync(Path.Combine("wwwroot/images", filename), bytes);

        //                    Console.WriteLine($"Downloaded: {filename}");
        //                }
        //            }
        //        }
        //    }
        //}
        private string ExtractGoogleDriveFileId(string url)
        {
            var match = Regex.Match(url, @"\/d\/(.*?)\/");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        /// <summary>
        /// Add/Update The User
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        /// Method Type : POST
        ///          
        ///     {
        ///       "userID": 0,
        ///       "vendorID": 1060,
        ///       "memberType": 1001,
        ///       "firstName": "John",
        ///       "lastName": "Abraham",
        ///       "mobileNo": "98989898989",
        ///       "createdby": 1060,
        ///       "createdOn": "2025-05-06T06:17:35.187Z",
        ///       "modifiedBy": 1060,
        ///       "modifiedOn": "2025-05-06T06:17:35.187Z",
        ///       "isActive": true
        ///     }
        ///
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "USER HAS BEEN SUCCESSFULLY REGISTERED!",
        ///       "data": {
        ///         "userID": 100,
        ///         "vendorID": 1060,
        ///         "memberType": 1001,
        ///         "firstName": "John",
        ///         "lastName": "Abraham",
        ///         "mobileNo": "98989898989",
        ///         "createdby": 1060,
        ///         "createdOn": "2025-05-06T06:17:35.187Z",
        ///         "modifiedBy": 1060,
        ///         "modifiedOn": "2025-05-06T06:17:35.187Z",
        ///         "isActive": true
        ///       }
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">USER HAS BEEN SUCCESSFULLY REGISTERED!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("WebModule/User/Register")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> SaveUser(MemberUser itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.vendorWebModuleBusiness.SaveVendorUser(itemData);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "USER HAS BEEN SUCCESSFULLY REGISTERED!";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    await Common.UpdateEventLogsNew("USER HAS BEEN SUCCESSFULLY REGISTERED!", reqHeader, controllerURL, item, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("USER REGISTERED - NO CONTENT", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("USER REGISTERED - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get The User
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///          
        ///     {
        ///       "userID": 100
        ///     }
        ///
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "USER ITEM HAS BEEN SUCCESSFULLY FETCHED!",
        ///       "data": {
        ///         "userID": 100,
        ///         "vendorID": 1060,
        ///         "memberType": 1001,
        ///         "firstName": "John",
        ///         "lastName": "Abraham",
        ///         "mobileNo": "98989898989",
        ///         "isActive": true
        ///       }
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">USER ITEM HAS BEEN SUCCESSFULLY FETCHED!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("WebModule/User/Get")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetAUser(ParamMemberVendorID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.vendorWebModuleBusiness.GetVendorUser(itemData.userID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "USER ITEM HAS BEEN SUCCESSFULLY FETCHED!";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    await Common.UpdateEventLogsNew("USER ITEM", reqHeader, controllerURL, item, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("USER ITEM - NO CONTENT", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("USER ITEM - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }



        #region Product Audit Log



        [HttpPost, Route("getauditlog")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetVendorAuditLog(ParamMemberVendorID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = await auditService.GetLogsByEntityAsync(EntityType.Vendor, itemData.userID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH Product Audit Log";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder IS SUCCESSFULLY", reqHeader, controllerURL, orderID, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder - NO CONTENT", reqHeader, controllerURL, orderID, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder - ERROR", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }



        #endregion






        /// <summary>
        /// Get The User List By Vendor
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     {
        ///       "userID": 100
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "USER LIST HAS BEEN SUCCESSFULLY FETCHED!",
        ///       "data": [
        ///         {
        ///           "userID": 100,
        ///           "vendorID": 1060,
        ///           "memberType": 1001,
        ///           "firstName": "John",
        ///           "lastName": "Abraham",
        ///           "mobileNo": "98989898989",
        ///           "isActive": true
        ///         }
        ///       ]
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">USER LIST HAS BEEN SUCCESSFULLY FETCHED!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("WebModule/User/List")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> GetUserList(ParamMemberVendorID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.vendorWebModuleBusiness.GetVendorUserList(itemData.userID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "USER LIST HAS BEEN SUCCESSFULLY FETCHED!";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    await Common.UpdateEventLogsNew("USER LIST HAS BEEN SUCCESSFULLY FETCHED!", reqHeader, controllerURL, item, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("USER LIST - NO CONTENT", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("USER LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Update The User Status
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST  
        ///     {
        ///       "userID": 100,
        ///       "isActive": false
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "USER'S STATUS HAS BEEN SUCCESSFULLY UPDATED!",
        ///       "data": true
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">USER'S STATUS HAS BEEN SUCCESSFULLY UPDATED!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("WebModule/User/UpdateStatus")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> UpdateUserStatus(MemberUserUpdate itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.vendorWebModuleBusiness.UpdateVendorUser(itemData.UserID, itemData.IsActive);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "USER'S STATUS HAS BEEN SUCCESSFULLY UPDATED!";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    await Common.UpdateEventLogsNew("USER'S STATUS HAS BEEN SUCCESSFULLY UPDATED!", reqHeader, controllerURL, item, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("USER'S STATUS - NO CONTENT", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("USER'S STATUS - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Send The Invitation to the Particular User
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST  
        ///     {
        ///       "userID": 100,
        ///       "mobileNo": "9899898989"
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "INVITATION HAS BEEN SUCCESSFULLY SENT!",
        ///       "data": true
        ///     }
        ///     
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">INVITATION HAS BEEN SUCCESSFULLY SENT!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("WebModule/User/SendInvitation")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> SendInvitation(MemberUserInvitation itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                string OTPValue = utility.Common.GenerateRandomPassword();

                var result = SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.InvitationSendToUser), 0, 0, itemData.MobileNo, this.notificationBusiness, OTPValue);
                if (result != null)
                {
                    this.vendorWebModuleBusiness.UpdateUserPassword(itemData.UserID, itemData.MobileNo, OTPValue);
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "INVITATION HAS BEEN SUCCESSFULLY SENT!";
                    rm.name = StatusName.ok;
                    rm.data = true;
                    //await Common.UpdateEventLogsNew("INVITATION HAS BEEN SUCCESSFULLY SENT!", reqHeader, controllerURL, result, null, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = "NO CONTENT";
                    //await Common.UpdateEventLogsNew("INVITATION - NO CONTENT!", reqHeader, controllerURL, result, null, StatusName.invalid, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                //await Common.UpdateEventLogsNew("USER'S STATUS HAS BEEN SUCCESSFULLY UPDATED!", reqHeader, controllerURL, null, null, StatusName.ok, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        [HttpPost, Route("SignIn")]
        [MapToApiVersion("1.0")]
        public IActionResult SignIn(ParamLoginIn itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic loginParams = jsondata;

            try
            {
                rm = new ResponseMessage();
                var returnData = this.vendorWebModuleBusiness.MemberLogIn(itemData.MobileNo, itemData.Password, itemData.parentID);
                if (returnData != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "MEMBER DATA";
                    rm.name = StatusName.ok;
                    rm.data = returnData;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MemberLogIn - SUCCESSFULLY", reqHeader, controllerURL, itemData, returnData, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "Invalid Mobile No or Password!";
                    rm.name = StatusName.invalidCred;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Invalid MobileNo or Password!", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalidCred;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("MemberLogIn - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

    }
}
