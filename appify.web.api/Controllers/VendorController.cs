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
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;
        public VendorController(IConfiguration configuration, 
                                ICustomerBusiness customerBusiness, 
                                IEventLogBusiness eventLogBusiness, 
                                IMemberBusiness iResultData,
                                ILookupBusiness lookupBusiness,
                                IProductBusiness productBusiness, 
                                IProductPriceBusiness priceBusiness, 
                                IProductImageBusiness imageBusiness,
                                IWebHostEnvironment env) {
            this.configuration = configuration;
            this.customerBusiness = customerBusiness;
            this.eventLogBusiness = eventLogBusiness;
            this.memberBusiness = iResultData;
            this.lookupBusiness = lookupBusiness;
            this.productBusiness = productBusiness;
            this.priceBusiness = priceBusiness;
            this.imageBusiness = imageBusiness;
            this.env = env;
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
    }
}
