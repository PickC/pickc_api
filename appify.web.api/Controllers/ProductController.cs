using appify.Business;
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
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;
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
                var result = productBusiness.DeleteProduct(itemData.productID);
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
        ///     POST /Get List of New Products
        ///     {
        ///         "VendorID": 1004
        ///     }
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
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>Boolean value</returns>
        /// <response code="200">PRODUCT NEW STATUS UPDATED SUCCESSFULLY!</response>
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
    }
}
