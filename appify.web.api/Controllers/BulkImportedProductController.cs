using appify.models;
using appify.Business.Contract;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace appify.web.api.Controllers
{
		   [Route("api/Vendor/[controller]")]
	    [ApiController]
	    [EnableCors("AllowOrigin")]
	    [ApiVersion("1.0")]
	    [ApiVersion("1.1")]
	public class BulkImportedProductController : Controller 
	{
		private readonly IConfiguration configuration; 
		private readonly IBulkImportedProductBusiness bulkimportedproductBusiness; 
		private readonly IWebHostEnvironment env; 

		private ResponseMessage rm; 
		public BulkImportedProductController(IConfiguration configuration,IWebHostEnvironment env, IBulkImportedProductBusiness bulkimportedproductBusiness)
		{
			  this.configuration = configuration;
			  this.env = env;
		    this.bulkimportedproductBusiness = bulkimportedproductBusiness; 
		}

		#region Get BulkImportedProduct Item

		[HttpPost, Route("get")]
		[MapToApiVersion("1.0")]
		public IActionResult BulkImportedProductGet(ParamBulkImportedProduct itemData)
		{

			var reqHeader = Request; 
			string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri; 
			try 
			{ 
			    rm = new ResponseMessage(); 
			    var result = this.bulkimportedproductBusiness.GetBulkImportedProduct(itemData.ItemID); 
			    if (result != null) 
			    { 
			        rm.statusCode = StatusCodes.OK; 
			        rm.message = "FETCH BULKIMPORTEDPRODUCT ITEM!"; 
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
		#endregion


		#region Get BulkImportedProduct List

		[HttpPost, Route("list")]
		[MapToApiVersion("1.0")]
		public IActionResult BulkImportedProductList(ParamBulkImportedProductList itemData)
		{

			var reqHeader = Request; 
			string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri; 
			try 
			{ 
			    rm = new ResponseMessage(); 
			    var result = this.bulkimportedproductBusiness.ListBulkImportedProduct(itemData.VendorID,itemData.ProductFileName); 
			    if (result != null) 
			    { 
			        rm.statusCode = StatusCodes.OK; 
			        rm.message = "FETCH BULKIMPORTEDPRODUCT LIST!"; 
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
		#endregion


		#region Save BulkImportedProduct Item

		[HttpPost, Route("save")]
		[MapToApiVersion("1.0")]
		public IActionResult BulkImportedProductSave(BulkImportedProduct itemData)
		{

			var reqHeader = Request; 
			string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri; 
			try 
			{ 
			    rm = new ResponseMessage(); 
			    var result = this.bulkimportedproductBusiness.SaveBulkImportedProduct(itemData); 
			    if (result != null) 
			    { 
			        rm.statusCode = StatusCodes.OK; 
			        rm.message = "SAVE BULKIMPORTEDPRODUCT ITEM!"; 
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
		#endregion


		#region Remove BulkImportedProduct Item

		[HttpPost, Route("remove")]
		[MapToApiVersion("1.0")]
		public IActionResult BulkImportedProductRemove(ParamBulkImportedProduct itemData)
		{

			var reqHeader = Request; 
			string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri; 
			try 
			{ 
			    rm = new ResponseMessage(); 
			    var result = this.bulkimportedproductBusiness.DeleteBulkImportedProduct(itemData.ItemID); 
			    if (result != null) 
			    { 
			        rm.statusCode = StatusCodes.OK; 
			        rm.message = "REMOVE BULKIMPORTEDPRODUCT ITEM!"; 
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
		#endregion


    }




    #region JSON Parameter Class

    public class ParamBulkImportedProduct
    {
        public Int64 ItemID { get; set; }

    }

    public class ParamBulkImportedProductList
    {
        public short VendorID { get; set; }
        public string ProductFileName { get; set; }

    }

    #endregion


}
