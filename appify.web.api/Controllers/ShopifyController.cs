/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.0
 * Date: 2025-04-29
 * Description:
*/
using appify.Business;
using appify.Business.Contract;
using Asp.Versioning;
using appify.utility;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]

    public class ShopifyController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private ResponseMessage rm;

        ShopifyController(IConfiguration configuration, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.eventLogBusiness = eventLogBusiness;
        }

        /// <summary>
        /// Fetch A Shopify Product List
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHPIFY PRODUCTS HAVE BEEN SUCCESSFULLY ASYNCED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("GetAllProductsAsync")]
        [MapToApiVersion("1.0")]
        public async Task <IActionResult> GetAllProductsAsync()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService();
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("SHPIFY PRODUCTS ASYNC - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return View();
        }

        /// <summary>
        /// Fetch A Shopify Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY ASYNCED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("GetProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetProductAsync(string ProductID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService();
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHPIFY PRODUCTS ASYNC - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
                return View();
        }

        /// <summary>
        /// CREATE A Shopify Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY CREATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        //[HttpPost, Route("CreateProductAsync")]
        //[MapToApiVersion("1.0")]
        //public async Task<IActionResult> CreateProductAsync()
        //{
        //    var reqHeader = Request;
        //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService();
        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;

        //        await Common.UpdateEventLogsNew("SHPIFY CREATE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        //    }
        //    return View();
        //}

        /// <summary>
        /// Update A Shopify Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY UPDATED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("UpdateProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateProductAsync(string ProductID, string Title)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService();
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHPIFY UPDATE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return View();
        }

        /// <summary>
        /// Remove A Shopify Product
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        ///     {
        ///     }
        ///     
        /// Sample response JSON :
        ///     {
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">SHPIFY PRODUCT HAS BEEN SUCCESSFULLY DELETED</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost, Route("DeleteProductAsync")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> DeleteProductAsync(string ProductID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                ShopifyGraphQLService shopifyGraphQLService = new ShopifyGraphQLService();

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("SHPIFY DELETE PRODUCT - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return View();
        }
    }
}
