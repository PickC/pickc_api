using appify.Business;
using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]

    public class CustomerController : ControllerBase
    {

        private readonly IConfiguration configuration;
        private readonly ICustomerBusiness customerBusiness;
        private ResponseMessage rm;

        public CustomerController(IConfiguration configuration,ICustomerBusiness customerBusiness)
        {
            this.configuration = configuration;
            this.customerBusiness = customerBusiness;
        }

        [HttpPost]
        [Route("productlist")]
        public IActionResult GetMemberProducts(ParamMemberUserID itemData) {

            try
            {
                rm = new ResponseMessage();
                List<MemberProduct> items = customerBusiness.ProductList(itemData.userID);
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
    }
}
