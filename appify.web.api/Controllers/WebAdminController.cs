using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class WebAdminController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IProductBusiness productBusiness;
        private readonly IMemberBusiness memberBusiness;
        private ResponseMessage rm;

        public WebAdminController(IConfiguration configuration,IMemberBusiness memberBusiness,IProductBusiness product)
        {
            this.configuration = configuration;
            this.productBusiness = product;
            this.memberBusiness = memberBusiness;


        }

        [HttpPost]
        [Route("products")]
        public IActionResult ListAllProducts() {

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

        [HttpPost]
        [Route("vendors")]
        public IActionResult ListAllVendors()
        {

            try
            {
                rm = new ResponseMessage();
                List<Member> items = memberBusiness.GetAllMembers();
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

        [HttpPost]
        [Route("vendordetails")]
        public IActionResult GetVendorDetails(ParamMemberUserID itemData )
        {
            Member item = new Member();
            try
            {
                rm = new ResponseMessage();
                item = memberBusiness.GetMember(itemData.userID);
                if (item!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = item;
                }
                else
                {
                    item = new Member();
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = item;
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
