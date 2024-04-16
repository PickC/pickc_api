using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class ProductController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IProductBusiness productBusiness;
        private readonly IProductPriceBusiness priceBusiness;
        private readonly IProductImageBusiness imageBusiness;
        private ResponseMessage rm;
        public ProductController(IConfiguration configuration, 
                        IProductBusiness iResultData, 
                        IProductPriceBusiness priceBusiness,
                        IProductImageBusiness imageBusiness)
        {
            this.configuration = configuration;
            this.productBusiness = iResultData;
            this.priceBusiness = priceBusiness;
            this.imageBusiness = imageBusiness;
             
        }


        [HttpPost,Route("save")]
        public IActionResult Add(Product product)
        {
            try
            {
                rm = new ResponseMessage();
                var productMaster = this.productBusiness.SaveProduct(product);
                if (productMaster !=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PRODUCT SUCCESSFUL!";
                    rm.name = StatusName.ok;
                    //rm.data = productMaster;


                    var canUpdate = this.productBusiness.UpdateProductImagePrice(product.ProductID);

                    var newproduct = new ProductMaster();


                    newproduct = this.productBusiness.GetProduct(product.ProductID);
                    if (newproduct !=null)
                    {
                        product.ProductID = newproduct.ProductID;
                        //product.ProductID=item.ProductID;
                        product.prices = this.priceBusiness.PriceList(product.ProductID);
                        product.images = this.imageBusiness.GetProductImages(product.ProductID);
                    }

                    rm.data = product;
                
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO ADD/UPDATE PRODUCT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost,Route("remove")]
        public IActionResult Remove(ParamProduct itemData)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DE-ACTIVATE PRODUCT";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost,Route("getitem")]
        public IActionResult GetProduct(ParamProduct itemData)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost,Route("list")]
        public IActionResult List(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }


        [HttpPost, Route("listall")]
        public IActionResult ListAll()
        {
            //dynamic data = jsonData;

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }


        [HttpPost, Route("price/list")]
        public IActionResult ListProductPrice(ParamProduct itemData)
        {
            //dynamic data=jsonData;

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost, Route("price/getprice")]
        public IActionResult GetProductPrice(ParamProductPrice itemData)
        {

            //dynamic data=jsonData;
            try
            {
                rm = new ResponseMessage();
                var item = priceBusiness.GetPrice(itemData.priceID, itemData.productID,itemData.size);

                if (item!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT-PRICE";
                    rm.name = StatusName.ok;
                    rm.data = item;
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost, Route("price/save")]
        public IActionResult AddProductPrice(ProductPrice price)
        {
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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost, Route("price/remove")]
        public IActionResult RemoveProductPrice(ParamProductPrice itemData)
        {
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = priceBusiness.RemovePrice(itemData.priceID, itemData.productID,itemData.size);

                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PRODUCT-PRICE REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }


        [HttpPost, Route("image/list")]
        public IActionResult ListProductImage(ParamProduct itemData)
        {
            //dynamic data = jsonData;

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost, Route("image/getimage")]
        public IActionResult GetProductImage(ParamProductImage itemData)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost, Route("image/save")]
        public IActionResult AddProductImage(ProductImage item)
        {
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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost, Route("image/remove")]
        public IActionResult RemoveProductImage(ParamProductImage itemData)
        {
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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }
            return Ok(rm);

        }

        [HttpPost, Route("image/verify")]
        public IActionResult VerifyImage(string imagePath) {
            
            try
            {
                rm = new ResponseMessage();
                rm.data = ImageClassifier(imagePath);
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }

            return Ok(rm);

        }

        private static async Task<string> ImageClassifier(string imagePath)
        {
             

            // Create an instance of HttpClient
            using (var client = new HttpClient())
            {
                string responseBody = "";
                try
                {


                    // Create a StringContent with the image data and set the content type
                    var content = new FormUrlEncodedContent(new[] {
                        new KeyValuePair<string, string>("",imagePath)
                    });

                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg"); // Adjust content type as needed

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

                return responseBody ;
            }
        }
    }
}
