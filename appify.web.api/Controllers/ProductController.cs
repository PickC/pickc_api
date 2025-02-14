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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Net;
namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    public class ProductController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IProductBusiness productBusiness;
        private readonly IProductPriceBusiness priceBusiness;
        private readonly IProductImageBusiness imageBusiness;
        private ResponseMessage rm;
        public ProductController(IConfiguration configuration, IProductBusiness iResultData, IProductPriceBusiness priceBusiness, IProductImageBusiness imageBusiness, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.productBusiness = iResultData;
            this.priceBusiness = priceBusiness;
            this.imageBusiness = imageBusiness;
            this.eventLogBusiness = eventLogBusiness;
        }

        /// <summary>
        /// Add/Update a Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "productID": 1093,
        ///       "vendorID": 1058,
        ///       "productName": "Cotton Shirt",
        ///       "description": "Cotton Shirt",
        ///       "category": 3641,
        ///       "brand": "Kf2",
        ///       "size": "string",
        ///       "color": "6 color",
        ///       "uom": 3502,
        ///       "weight": 100,
        ///       "priceID": 2181,
        ///       "currency": "INR",
        ///       "imageID": 1411,
        ///       "isActive": true,
        ///       "isAvailable": true,
        ///       "stockQty": 0,
        ///       "createdOn": "2024-05-01T12:21:24.754Z",
        ///       "modifiedOn": "2024-05-01T12:21:24.754Z",
        ///       "hsnCode": "string",
        ///       "SKU":"string",
        ///       "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_B58001A3-2429-486F-A781-///D    B8 3A60E8A33-4419-000004E3B3BF730E.jpg",
        ///       "isNew": true,
        ///       "prices": [
        ///         {
        ///           "priceID": 1281,
        ///           "productID": 1093,
        ///           "price": 6,
        ///           "discount": 0,
        ///           "discountType": 0,
        ///           "effectiveDate": "2024-05-01T12:21:24.754Z",
        ///           "isActive": true,
        ///           "size": "m",
        ///           "stock": 3,
        ///           "weight": 100
        ///         }
        ///       ],
        ///       "images": [
        ///         {
        ///           "imageID": 1411,
        ///           "productID": 1093,
        ///           "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_B58001A3-2429-486F-A781-///D    B8 3A60E8A33-4419-000004E3B3BF730E.jpg",
        ///           "isActive": true,
        ///           "contentType": 0,
        ///           "createdOn": "2024-05-01T12:21:24.754Z"
        ///         }
        ///       ]
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "PRODUCT SUCCESSFUL!",
        ///       "data": {
        ///         "prices": [
        ///           {
        ///             "priceID": 2181,
        ///             "productID": 1093,
        ///             "price": 60,
        ///             "discount": 0,
        ///             "discountType": 0,
        ///             "effectiveDate": "2024-05-01T00:00:00",
        ///             "isActive": true,
        ///             "size": "M",
        ///             "stock": 3,
        ///             "weight": 1001
        ///           }
        ///         ],
        ///         "images": [
        ///           {
        ///             "imageID": 1411,
        ///             "productID": 1093,
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_B58001A3-2429-486F-A781-///D  B8 3A60E8A33-4419-000004E3B3BF730E.jpg",
        ///             "isActive": true,
        ///             "contentType": 0,
        ///             "createdOn": "2023-12-07T19:37:15.113"
        ///           },
        ///           {
        ///         "imageID": 1412,
        ///             "productID": 1093,
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer////i    ma ge_cropper_7A720428-1EAE-4AC2-9B42-7DE46FAEE9D6-4419-000004E3BF859451.jpg",
        ///             "isActive": true,
        ///             "contentType": 0,
        ///             "createdOn": "2023-12-07T19:37:15.2"
        ///           }
        ///         ],
        ///         "productID": 1093,
        ///         "vendorID": 1058,
        ///         "productName": "Cotton Shirt",
        ///         "description": "Cotton Shirt",
        ///         "category": 3641,
        ///         "brand": "Kf2",
        ///         "size": "string",
        ///         "color": "6 color",
        ///         "uom": 3502,
        ///         "weight": 100,
        ///         "priceID": 2181,
        ///         "currency": "INR",
        ///         "imageID": 1411,
        ///         "isActive": true,
        ///         "isAvailable": true,
        ///         "stockQty": 0,
        ///         "createdOn": "2024-05-01T12:21:24.754Z",
        ///         "modifiedOn": "2024-05-01T12:21:24.754Z",
        ///         "hsnCode": "string",
        ///         "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_B58001A3-2429-486F-A781-///D  B8 3A60E8A33-4419-000004E3B3BF730E.jpg",
        ///         "isNew": true
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">PRODUCT SUCCESSFUL </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Add(Product product)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();////To Do Need to check internal - price, image is empty then return exception message

                if (product.prices?.Any() == true && product.images?.Any() == true)
                {
                        var productMaster = this.productBusiness.SaveProduct(product);
                        if (productMaster != null)
                        {
                            rm.statusCode = StatusCodes.OK;
                            rm.message = "PRODUCT SUCCESSFUL!";
                            rm.name = StatusName.ok;
                            //rm.data = productMaster;

                            var canUpdate = this.productBusiness.UpdateProductImagePrice(product.ProductID);

                            var newproduct = new ProductMaster();


                            newproduct = this.productBusiness.GetProduct(product.ProductID);
                            if (newproduct != null)
                            {
                                product.ProductID = newproduct.ProductID;
                                //product.ProductID=item.ProductID;
                                product.prices = this.priceBusiness.PriceList(product.ProductID);
                                product.images = this.imageBusiness.GetProductImages(product.ProductID);
                            }

                            rm.data = product;
                        //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                        //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT SAVED SUCCESSFULLY", reqHeader, controllerURL, product, rm.data, StatusName.ok));
                        await Common.UpdateEventLogsNew("PRODUCT SAVED SUCCESSFULLY", reqHeader, controllerURL, product, rm.data, rm.message, this.eventLogBusiness);
                    }
                        else
                        {
                            rm.statusCode = StatusCodes.ERROR;
                            rm.message = "UNABLE TO ADD/UPDATE PRODUCT";
                            rm.name = StatusName.invalid;
                            rm.data = null;
                        //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                        //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO ADD/UPDATE PRODUCT", reqHeader, controllerURL, product, null, rm.message));
                        await Common.UpdateEventLogsNew("UNABLE TO ADD/UPDATE PRODUCT", reqHeader, controllerURL, product, null, rm.message, this.eventLogBusiness);
                    }
                   }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO ADD/UPDATE PRODUCT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("UNABLE TO ADD/UPDATE PRODUCT", reqHeader, controllerURL, product, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT SAVED - ERROR", reqHeader, controllerURL, product, null, rm.message));
                await Common.UpdateEventLogsNew("PRODUCT SAVED - ERROR", reqHeader, controllerURL, product, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove a Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "productID": 1003
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">PRODUCT REMOVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("remove")]
        [MapToApiVersion("1.0")]
        public IActionResult Remove(ParamProduct itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = productBusiness.DeleteProduct(itemData.productID,itemData.IsActive);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PRODUCT REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DE-ACTIVATE PRODUCT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO DE-ACTIVATE PRODUCT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
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
    ///         "vendorID": 1001,
    ///         "productName": "White Round neck Tshirts",
    ///         "description": "plain white t shirts round neck full and half sleeves ",
    ///         "category": 3608,
    ///         "brand": "Qikink Fashion ",
    ///         "size": "",
    ///         "color": "white ",
    ///         "uom": 3500,
    ///         "weight": 0,
    ///         "priceID": 1007,
    ///         "currency": "INR",
    ///         "imageID": 1009,
    ///         "isActive": true,
    ///         "isAvailable": true,
    ///         "stockQty": 0,
    ///         "createdOn": null,
    ///         "modifiedOn": "2023-09-21T09:38:16.513",
    ///         "hsnCode": null,
    ///         "imageName": null,
    ///         "isNew": false
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">GET PRODUCT - SUCCESSFULLY </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("getitem")]
        [MapToApiVersion("1.0")]
        public IActionResult GetProduct(ParamProduct itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = this.productBusiness.GetProduct(itemData.productID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT - SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
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
        public async Task<IActionResult> GetProductNew(ParamProduct itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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
        /// Get Product List by UserID
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
        ///       "message": "FETCH PRODUCT LIST",
        ///       "data": [
        ///         {
        ///           "productID": 1064,
        ///           "vendorID": 1060,
        ///           "productName": "Men's Slim Fit Casual Shirts",
        ///           "description": "This Shirt comes in cotton fabric and is perfect for casual and formal wear.",
        ///           "category": 1751,
        ///           "brand": "I AM BACK ",
        ///           "size": "",
        ///           "color": "Beige ",
        ///           "uom": 3502,
        ///           "weight": 0,
        ///           "priceID": 5649,
        ///           "currency": "INR",
        ///           "imageID": 1086,
        ///           "isActive": true,
        ///           "isAvailable": true,
        ///           "stockQty": 0,
        ///           "createdOn": null,
        ///           "modifiedOn": "2024-09-30T15:44:43.447",
        ///           "hsnCode": "t5678",
        ///           "imageName": null,
        ///           "isNew": false
        ///         },
        ///         {
        ///           "productID": 1065,
        ///           "vendorID": 1060,
        ///           "productName": "Men's Casual Wear Shirt with Vertical Stripes ",
        ///           "description": "This shirt is a perfect fit for both casual and formal wear, it comes in pure cotton fabric. A good to go for party wear a    s well.",
        ///           "category": 3646,
        ///           "brand": "I AM BACK ",
        ///           "size": "",
        ///           "color": "White and Grey",
        ///           "uom": 3502,
        ///           "weight": 0,
        ///           "priceID": 5653,
        ///           "currency": "INR",
        ///           "imageID": 1087,
        ///           "isActive": true,
        ///           "isAvailable": true,
        ///           "stockQty": 0,
        ///           "createdOn": null,
        ///           "modifiedOn": "2024-06-17T16:12:10.68",
        ///           "hsnCode": "t8699",
        ///           "imageName": null,
        ///           "isNew": false
        ///         }]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH PRODUCT LIST </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        public IActionResult List(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<ProductMaster> items = productBusiness.GetProducts(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get Product List ALL
        /// </summary>
        /// <remarks>
        ///     
        /// Sample response JSON :
        /// 
        ///         {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH PRODUCT LIST",
        ///           "data": [
        ///             {
        ///               "productID": 1000,
        ///               "vendorID": 1000,
        ///               "productName": "Linen Sarees ",
        ///               "description": "Linen wear",
        ///               "category": 3607,
        ///               "brand": "Dhaage Life",
        ///               "size": "",
        ///               "color": "Multi colour ",
        ///               "uom": 3500,
        ///               "weight": 0,
        ///               "priceID": 1000,
        ///               "currency": "INR",
        ///               "imageID": 1000,
        ///               "isActive": true,
        ///               "isAvailable": true,
        ///               "stockQty": 0,
        ///               "createdOn": null,
        ///               "modifiedOn": null,
        ///               "hsnCode": null,
        ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
        ///               "isNew": false
        ///             },
        ///             {
        ///               "productID": 1000,
        ///               "vendorID": 1000,
        ///               "productName": "Linen Sarees ",
        ///               "description": "Linen wear",
        ///               "category": 3607,
        ///               "brand": "Dhaage Life",
        ///               "size": "",
        ///               "color": "Multi colour ",
        ///               "uom": 3500,
        ///               "weight": 0,
        ///               "priceID": 1000,
        ///               "currency": "INR",
        ///               "imageID": 1000,
        ///               "isActive": true,
        ///               "isAvailable": true,
        ///               "stockQty": 0,
        ///               "createdOn": null,
        ///               "modifiedOn": null,
        ///               "hsnCode": null,
        ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992902396.jpg",
        ///               "isNew": false
        ///             }
        ///             ]
        ///         }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH PRODUCT LIST </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("listall")]
        [MapToApiVersion("1.0")]
        public IActionResult ListAll()
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<ProductMaster> items = productBusiness.GetAllProducts();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, null, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT LIST - ERROR", reqHeader, controllerURL, null, null, rm.message));
            }
            return Ok(rm);

        }
    /// <summary>
    /// Get Product's Price List
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
    ///       "message": "FETCH PRODUCT-PRICE LIST",
    ///       "data": [
    ///         {
    ///           "priceID": 1007,
    ///           "productID": 1003,
    ///           "price": 400,
    ///           "discount": 0,
    ///           "discountType": 0,
    ///           "effectiveDate": "2023-09-06T00:00:00",
    ///           "isActive": true,
    ///           "size": "M",
    ///           "stock": 9,
    ///           "weight": null
    ///         },
    ///         {
    ///           "priceID": 1008,
    ///           "productID": 1003,
    ///           "price": 800,
    ///           "discount": 0,
    ///           "discountType": 0,
    ///           "effectiveDate": "2023-09-06T00:00:00",
    ///           "isActive": true,
    ///           "size": "L",
    ///           "stock": 10,
    ///           "weight": null
    ///         },
    ///         {
    ///         "priceID": 1009,
    ///           "productID": 1003,
    ///           "price": 900,
    ///           "discount": 0,
    ///           "discountType": 0,
    ///           "effectiveDate": "2023-09-06T00:00:00",
    ///           "isActive": true,
    ///           "size": "XL",
    ///           "stock": 22,
    ///           "weight": null
    ///         }
    ///       ]
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">FETCH PRODUCT-PRICE LIST </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 

    [HttpPost, Route("price/list")]
        [MapToApiVersion("1.0")]
        public IActionResult ListProductPrice(ParamProduct itemData)
        {
            //dynamic data=jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var items = priceBusiness.PriceList(itemData.productID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT-PRICE LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT-PRICE LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT-PRICE LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT-PRICE LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
    /// <summary>
    /// Get Product's Price
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "productID": 1003,
    ///       "priceID": 1010,
    ///       "size": "XXL"
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">FETCH PRODUCT-PRICE </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("price/getprice")]
        [MapToApiVersion("1.0")]
        public IActionResult GetProductPrice(ParamProductPrice itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data=jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = priceBusiness.GetPrice(itemData.priceID, itemData.productID, itemData.size);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT-PRICE";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT-PRICE - SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT-PRICE - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT-PRICE - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
    /// <summary>
    /// Add/Update Product's Price
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "priceID": 1010,
    ///       "productID": 1003,
    ///       "price": 1100.00,
    ///       "discount": 0.00,
    ///       "discountType": 0,
    ///       "effectiveDate": "2024-09-30T18:42:32.625Z",
    ///       "isActive": true,
    ///       "size": "string",
    ///       "stock": 10,
    ///       "weight": 130
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT-PRICE SAVED SUCCESSFULLY",
    ///       "data": true
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">PRODUCT-PRICE SAVED SUCCESSFULLY </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("price/save")]
    [MapToApiVersion("1.0")]
    public IActionResult AddProductPrice(ProductPrice price)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            var result = priceBusiness.SavePrice(price);
            if (result)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT-PRICE SAVED SUCCESSFULLY";
                rm.name = StatusName.ok;
                rm.data = result;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-PRICE SAVED SUCCESSFULLY", reqHeader, controllerURL, price, result, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-PRICE SAVED - NO CONTENT", reqHeader, controllerURL, price, null, rm.message));
            }


        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-PRICE SAVED - ERROR", reqHeader, controllerURL, price, null, rm.message));
        }
        return Ok(rm);

    }
    /// <summary>
    /// Remove Product's Price
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "productID": 1003,
    ///       "priceID": 1010,
    ///       "size": "XXL"
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT-PRICE REMOVED",
    ///       "data": true
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">PRODUCT-PRICE REMOVED </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("price/remove")]
    [MapToApiVersion("1.0")]
    public IActionResult RemoveProductPrice(ParamProductPrice itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            var result = priceBusiness.RemovePrice(itemData.priceID, itemData.productID, itemData.size);

            if (result)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT-PRICE REMOVED";
                rm.name = StatusName.ok;
                rm.data = result;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-PRICE REMOVED - SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-PRICE REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-PRICE REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
        }
        return Ok(rm);

    }

    /// <summary>
    /// Get Product's Image List
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "productID": 1000,
    ///       "imageID": 1000
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "FETCH PRODUCT-IMAGE",
    ///       "data": {
    ///         "imageID": 1000,
    ///         "productID": 1000,
    ///         "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
    ///         "isActive": false,
    ///         "contentType": 0,
    ///         "createdOn": "2023-09-06T02:36:54.12"
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">FETCH PRODUCT-IMAGE </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost, Route("image/list")]
    [MapToApiVersion("1.0")]
    public IActionResult ListProductImage(ParamProduct itemData)
    {
        //dynamic data = jsonData;
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            var items = imageBusiness.GetProductImages(itemData.productID);
            if (items?.Any() == true)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "FETCH PRODUCT-IMAGE LIST";
                rm.name = StatusName.ok;
                rm.data = items;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT-IMAGE LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT-IMAGE LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT-IMAGE LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
        }
        return Ok(rm);

    }
    /// <summary>
    /// Get Product's Image
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "productID": 1000,
    ///       "imageID": 1000
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "FETCH PRODUCT-IMAGE",
    ///       "data": {
    ///         "imageID": 1000,
    ///         "productID": 1000,
    ///         "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
    ///         "isActive": false,
    ///         "contentType": 0,
    ///         "createdOn": "2023-09-06T02:36:54.12"
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">FETCH PRODUCT-IMAGE </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost, Route("image/getimage")]
    [MapToApiVersion("1.0")]
    public IActionResult GetProductImage(ParamProductImage itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            var item = imageBusiness.GetProductImage(itemData.imageID, itemData.productID);

            if (item != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "FETCH PRODUCT-IMAGE";
                rm.name = StatusName.ok;
                rm.data = item;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT-IMAGE - SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT-IMAGE - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
            }

        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET PRODUCT-IMAGE - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
        }
        return Ok(rm);

    }
    /// <summary>
    /// Adds/Update an Product Image
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "imageID": 1000,
    ///       "productID": 1000,
    ///       "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1693992886365.jpg",
    ///       "isActive": true,
    ///       "contentType": 0,
    ///       "createdOn": "2024-09-30T18:28:24.225Z"
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT-IMAGE SAVED SUCCESSFULLY",
    ///       "data": true
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">PRODUCT-IMAGE SAVED SUCCESSFULLY </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost, Route("image/save")]
    [MapToApiVersion("1.0")]
    public IActionResult AddProductImage(ProductImage item)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            var result = imageBusiness.AddProductImage(item);
            if (result)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT-IMAGE SAVED SUCCESSFULLY";
                rm.name = StatusName.ok;
                rm.data = result;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE SAVED - NO CONTENT", reqHeader, controllerURL, item, null, rm.message));
            }


        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message));
        }
        return Ok(rm);

    }
    /// <summary>
    /// Remove Product's Image
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "productID": 1000,
    ///       "imageID": 1000
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT-IMAGE REMOVED",
    ///       "data": true
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">PRODUCT-IMAGE REMOVED </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    ///
    [HttpPost, Route("image/remove")]
        [MapToApiVersion("1.0")]
        public IActionResult RemoveProductImage(ParamProductImage itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = imageBusiness.RemoveProductImage(itemData.imageID, itemData.productID);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PRODUCT-IMAGE REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Verify Image Based On ImageURL
        /// </summary>
        [HttpPost, Route("image/verify")]
        [MapToApiVersion("1.0")]
        public IActionResult VerifyImage([Required] string imagePath)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                // rm.data = ImageClassifier(imagePath);
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT-IMAGE VERIFIED";
                rm.name = StatusName.ok;
                JObject jObject = JObject.Parse(ImageClassifier(imagePath).Result);
                rm.data = jObject["data"].Value<string>();
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE VERIFIED SUCCESSFULLY", reqHeader, controllerURL, imagePath, rm.data, StatusName.ok));
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE VERIFIED - ERROR", reqHeader, controllerURL, imagePath, null, rm.message));
            }

            return Ok(rm);

        }

        /// <summary>
        /// Verify Image Based On IForm File
        /// </summary>
        [HttpPost, Route("image/verifyByIForm")]
        [MapToApiVersion("1.0")]
        public IActionResult VerifyImageByIForm([Required]IFormFile file)
        {/////[FromForm] IFormFile file  [FileExtensions(Extensions = "jpg,png,gif,jpeg")] 
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                if (file == null || file.Length == 0)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO FILE OR INVALID FILE FORMAT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    return Ok(rm);
                }
                if (Path.GetExtension(file.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(file.FileName).ToLower() != ".png"
                    && Path.GetExtension(file.FileName).ToLower() != ".gif"
                    && Path.GetExtension(file.FileName).ToLower() != ".jpeg")
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "FILE TYPE ONLY ACCEPT .JPG, .PNG, .GIF, .JPEG";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    return Ok(rm);
                }

                
                if (file.Length > Common.IMAGE_SIZE)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "FILE SIZE GREATER THAN 5 MB";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    return Ok(rm);
                }

                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCT-IMAGE VERIFIED";
                rm.name = StatusName.ok;
                JObject jObject = JObject.Parse(ImageClassifieryByIForm(file).Result);
                rm.data = jObject["data"].Value<string>();

                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE VERIFIED SUCCESSFULLY", reqHeader, controllerURL, file.FileName, rm.data, StatusName.ok));
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT-IMAGE VERIFIED - ERROR", reqHeader, controllerURL, file.FileName, null, rm.message));
            }

            return Ok(rm);
        }

        private static async Task<string> ImageClassifieryByIForm(IFormFile file)
        {
            // Create an instance of HttpClient
            using (var client = new HttpClient())
            {
                string responseBody = "";
                try
                {
                    byte[] data;
                    using (var br = new BinaryReader(file.OpenReadStream()))
                        data = br.ReadBytes((int)file.OpenReadStream().Length);

                    ByteArrayContent bytes = new ByteArrayContent(data);
                    MultipartFormDataContent multiContent = new MultipartFormDataContent();

                    multiContent.Add(bytes, "file", file.FileName);
                    HttpResponseMessage response = await client.PostAsync(Common.IMAGECLASSIFIER_URL, multiContent);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content (if needed)
                        responseBody = await response.Content.ReadAsStringAsync();

                    }
                    else
                    {
                        responseBody = response.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    responseBody = ex.ToString();

                }

                return responseBody;
            }
        }
        private static async Task<string> ImageClassifier(string imagePath)
        {


            // Create an instance of HttpClient
            using (var client = new HttpClient())
            {
                string responseBody = "";
                try
                {

                    var content = new MultipartFormDataContent();

                    using (WebClient webClient = new WebClient())
                    {
                        string url = string.Format(imagePath);
                        var fileName = Path.GetFileName(url);
                        var memoryStream = new MemoryStream(webClient.DownloadData(url));
                        var fileContent = new StreamContent(memoryStream);
                        fileContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("image/jpeg");

                        content.Add(fileContent, "file", fileName);
                        memoryStream.Flush();
                    }

                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(Common.IMAGECLASSIFIER_URL, content);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content (if needed)
                        responseBody = await response.Content.ReadAsStringAsync();

                    }
                    else
                    {
                        responseBody = response.StatusCode.ToString();
                    }


                }
                catch (Exception ex)
                {
                    responseBody = ex.ToString();

                }

                return responseBody;
            }
        }

    /// <summary>
    /// Get New Product List By Vendor
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    /// POST /Get List of New Products
    ///     {
    ///       "userID": 0,
    ///       "isNew": true
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCTS LIST WITH NEW STATUS",
    ///       "data": [
    ///         {
    ///           "productID": 1711,
    ///           "productName": "LEE MORGAN TRENDY SHIRT",
    ///           "description": "Product Details : \nFit : Regular\nFabric : Cotton Blend\nSleeve : Full Sleeve\nPattern : Solid\nColour : Light Green \    nF abric //care :/ Machine Wash.",
    ///           "brand": "LEE MORGAN ",
    ///           "price": 949,
    ///           "isNew": true
    ///         },
    ///         {
    ///           "productID": 1715,
    ///           "productName": "Trendy Double Pocket Light Grey Shirt For Men's ",
    ///           "description": "Product Details : \nFit : Regular\nFabric : Cotton Blend\nSleeve : Full Sleeve\nPattern : Solid\nColour : Light Grey\  nF abric //care : /Machine Wash.",
    ///           "brand": "LEE MORGAN ",
    ///           "price": 949,
    ///           "isNew": true
    ///         }]
    ///     }
    /// 
    /// </remarks>
    /// <param name="itemData"></param>
    /// <returns>Response Message Object Type : ProductMaster</returns>
    /// <response code="200">PRODUCTS LIST WITH NEW STATUS</response>
    /// <response code="500">Returns Error ResponseMessages </response> 

    [HttpPost, Route("NewProductsList")]
    [MapToApiVersion("1.0")]
    public IActionResult GetNewProductsList(ParamNewProductsByMember itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
            var result = this.productBusiness.GetNewProductsList(itemData.userID,itemData.IsNew );
            if (result != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "PRODUCTS LIST WITH NEW STATUS";
                rm.name = StatusName.ok;
                rm.data = result;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCTS LIST WITH NEW STATUS SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("TransacPRODUCTS LIST WITH NEW STATUS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
            }
        }
        catch (Exception ex)
        {
            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("TransacPRODUCTS LIST WITH NEW STATUS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
        }

        return Ok(rm);
    }
    /// <summary>
    /// Update New Product By ProductID and IsNew.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     Method Type : POST
    ///     
    ///     {
    ///         "ProductID": 1004,
    ///         "IsNew":true
    ///     }
    /// 
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PRODUCT NEW STATUS UPDATED SUCCESSFULLY!",
    ///       "data": true
    ///     }
    /// 
    /// </remarks>
    /// <param name="itemData"></param>
    /// <returns>Boolean value</returns>
    /// <response code="200">PRODUCT NEW STATUS UPDATED SUCCESSFULLY!</response>
    /// <response code="500">Returns Error ResponseMessages </response> 
    [HttpPost, Route("UpdateProductAsNew")]
        [MapToApiVersion("1.0")]
        public IActionResult UpdateNewProducts(List<ParamNewProduct> itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            bool result = false;
            try
            {
                rm = new ResponseMessage();
                foreach (var item in itemData)
                {
                    result = this.productBusiness.UpdateNewProducts(item.ProductID, item.IsNew);
                }
                
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PRODUCT NEW STATUS UPDATED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT NEW STATUS UPDATED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT NEW STATUS UPDATED - NO CONTENT!", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT NEW STATUS UPDATED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }

            return Ok(rm);
        }

        /// <summary>
        /// Get Master Categories By ParentID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///         "ParentID": 1001
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH PRODUCT MASTER CATEGORIES",
        ///       "data": [
        ///         {
        ///           "categoryID": 1026,
        ///           "category": "Home Medical Supplies and Equipment "
        ///         },
        ///         {
        ///           "categoryID": 1027,
        ///           "category": "Personal Care"
        ///         },
        ///         {
        ///         "categoryID": 1028,
        ///           "category": "Health Care "
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH PRODUCT MASTER CATEGORIES!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("getmastercategories")]
        [MapToApiVersion("1.0")]
        public IActionResult GetMasterCategories(ParamParent itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = this.productBusiness.GetProductMasterCategories(itemData.parentID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT MASTER CATEGORIES";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT MASTER CATEGORIES SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT MASTER CATEGORIES - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT MASTER CATEGORIES - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get Categories List By ParentID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///         "ParentID": 1001
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH CATEGORIES BY PARENTID",
        ///       "data": [
        ///         {
        ///           "categoryID": 1026,
        ///           "category": "Home Medical Supplies and Equipment ",
        ///           "parentID": 1001,
        ///           "fullCategory": "Health and Personal Care>>Home Medical Supplies and Equipment "
        ///         },
        ///         {
        ///           "categoryID": 1027,
        ///           "category": "Personal Care",
        ///           "parentID": 1001,
        ///           "fullCategory": "Health and Personal Care>>Personal Care"
        ///         }]
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH CATEGORIES BY PARENTID!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("getcategoriesbyid")]
        [MapToApiVersion("1.0")]
        public IActionResult GetCategoriesList(ParamParent itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = this.productBusiness.GetCategoriesList(itemData.parentID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH CATEGORIES BY PARENTID";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH CATEGORIES SUCCESSFULLY BY PARENTID", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH CATEGORIES BY PARENTID - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH CATEGORIES BY PARENTID - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

    /// <summary>
    /// Get Categories Name By ParentID
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     Method Type : POST
    ///     
    ///     {
    ///         "ParentID": 1248
    ///     }
    /// 
    /// Sample response JSON :
    /// 
    ///        {
    ///          "statusCode": 200,
    ///          "name": "SUCCESS_OK",
    ///          "message": "FETCH CATEGORIES BY PARENTID",
    ///          "data": [
    ///            {
    ///              "parentID": 1000,
    ///              "breadCrumb": "Beauty>>Make-up>>Eyes>>Kajal and Kohls"
    ///            }
    ///          ]
    ///        }
    /// 
    /// </remarks>
    /// <param name="itemData"></param>
    /// <returns>Boolean value</returns>
    /// <response code="200">FETCH CATEGORIES BY PARENTID!</response>
    /// <response code="500">Returns Error ResponseMessages </response> 

    [HttpPost, Route("getcategoryname")]
    [MapToApiVersion("1.0")]
    public IActionResult GetCategorieName(ParamCatID itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
            var item = this.productBusiness.GetCategorieName(itemData.categoryID);
            if (item != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "FETCH CATEGORIES BY PARENTID";
                rm.name = StatusName.ok;
                rm.data = item;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH CATEGORIES SUCCESSFULLY BY PARENTID", reqHeader, controllerURL, itemData, item, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH CATEGORIES BY PARENTID - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
            }

        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH CATEGORIES BY PARENTID - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
        }
        return Ok(rm);

    }

        /// <summary>
        /// Save Selected Parent Categories By VendorID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {	
        ///       "userID": 1060,
        ///       "parentCatID": 1000,
        ///       "isActive":true
        ///     },
        ///     {
        ///       "userID": 1060,
        ///       "parentCatID": 1001,
        ///       "isActive":false
        ///     },
        ///     {
        ///       "userID": 1060,
        ///       "parentCatID": 1002,
        ///       "isActive":true
        ///     },
        ///     {
        ///       "userID": 1060,
        ///       "parentCatID": 1003,
        ///       "isActive":true
        ///     }
        /// 
        /// </remarks>
        /// <returns>Boolean value</returns>
        /// <response code="200">VENDOR SELECTED PARENT CATEGORIES SAVED SUCCESSFULLY!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("savevendorcategories")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SaveVendorCategories(List<ParentCategories> vendorCategories)
        {
            var reqHeader = Request;
            var result = true;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                List<ParentCategories> returnItem = new List<ParentCategories>();
                foreach (var item in vendorCategories)
                {
                    returnItem.Add(this.productBusiness.SaveVendorCategories(item));
                }
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "VENDOR SELECTED PARENT CATEGORIES SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = returnItem;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    await Common.UpdateEventLogsNew("VENDOR SELECTED PARENT CATEGORIES SAVED SUCCESSFULLY!", reqHeader, controllerURL, vendorCategories, returnItem, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("VENDOR SELECTED PARENT CATEGORIES - NO CONTENT", reqHeader, controllerURL, vendorCategories, returnItem, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("VENDOR SELECTED PARENT CATEGORIES - ERROR", reqHeader, controllerURL, vendorCategories, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get Selected Parent Categories By VendorID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Method Type : POST
        ///     
        ///     {
        ///         "userID": 1060
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH SELECTED PARENT CATEGORIES",
        ///       "data": [
        ///         {
        ///           "userID": 1060,
        ///           "parentCatID": 1000,
        ///           "isActive": true
        ///         },
        ///         {
        ///           "userID": 1060,
        ///           "parentCatID": 1001,
        ///           "isActive": false
        ///         },
        ///         {
        ///         "userID": 1060,
        ///           "parentCatID": 1002,
        ///           "isActive": true
        ///         },
        ///         {
        ///         "userID": 1060,
        ///           "parentCatID": 4191,
        ///           "isActive": false
        ///         },
        ///         {
        ///         "userID": 1060,
        ///           "parentCatID": 1003,
        ///           "isActive": true
        ///         }
        ///       ]
        ///     }
        ///     
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">FETCH SELECTED PARENT CATEGORIES!</response>
        /// <response code="500">Returns Error ResponseMessages </response> 

        [HttpPost, Route("getvendorcategories")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetVendorCategories(ParamMemberVendorID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = this.productBusiness.GetVendorCategories(itemData.userID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH SELECTED PARENT CATEGORIES";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    await Common.UpdateEventLogsNew("FETCH SELECTED PARENT CATEGORIES", reqHeader, controllerURL, itemData, item, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("FETCH SELECTED PARENT CATEGORIES - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("FETCH SELECTED PARENT CATEGORIES - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
    }
}
