using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]

    public class DiscountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDiscountHeaderBusiness _discountHeaderBusiness;
        private readonly IDiscountDetailBusiness _discountDetailBusiness;
        private ResponseMessage rm;
        public DiscountController(IConfiguration configuration, IDiscountHeaderBusiness discountHeaderBusiness, IDiscountDetailBusiness discountDetailBusiness)
        {
            this._configuration = configuration;
            this._discountHeaderBusiness = discountHeaderBusiness;
            this._discountDetailBusiness = discountDetailBusiness;
        }

        [HttpPost, Route("Save")]
        public IActionResult discountHeaderAdd(DiscountHeader discountHeader)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this._discountHeaderBusiness.Save(discountHeader);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT SAVED SUCCESSFULLY!";
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
            catch(Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }

            return Ok(rm);
        }

        [HttpPost, Route("Remove")]
        public IActionResult discountHeaderRemove(ParamDiscountRemove itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this._discountHeaderBusiness.Remove(itemData.DiscountID, itemData.ModifiedBy);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "DISCOUNT REMOVED SUCCESSFULLY!";
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

        [HttpPost, Route("Get")]
        public IActionResult discountHeaderGet(ParamDiscount itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this._discountHeaderBusiness.Get(itemData.DiscountID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
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

        [HttpPost, Route("List")]
        public IActionResult discountHeaderList(ParamDiscount itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this._discountHeaderBusiness.GetAll(itemData.DiscountID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
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
        [HttpPost, Route("listbyvendor")]
        public IActionResult ListByVendor(ParamMemberUserID itemData)
        {
            try
            {
                rm = new ResponseMessage();
                var result = this._discountHeaderBusiness.ListByVendor(itemData.userID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH DISCOUNT ITEM!";
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


        ///////////// Discount Detail
        ///
        //[HttpPost, Route("DiscountDetailSave")]
        //public IActionResult discountDetailAdd(DiscountDetail discountDetail)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this._discountDetailBusiness.Save(discountDetail);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "DISCOUNT DETAILED SAVED SUCCESSFULLY!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
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

        //[HttpPost, Route("DiscounDetailRemove")]
        //public IActionResult discountDetailRemove(ParamDiscountDetail itemData)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this._discountDetailBusiness.Remove(itemData.DiscountID, itemData.productID);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "DISCOUNT DETAILED REMOVED SUCCESSFULLY!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
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

        //[HttpPost, Route("DiscountDetailGet")]
        //public IActionResult discountDetailGet(ParamDiscountDetail itemData)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this._discountDetailBusiness.Get(itemData.DiscountID, itemData.productID);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "FETCH DISCOUNT DETAIL ITEM!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
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

        //[HttpPost, Route("DiscountDetailList")]
        //public IActionResult discountDetailList(ParamDiscountDetail itemData)
        //{
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        var result = this._discountDetailBusiness.GetAll(itemData.DiscountID, itemData.productID);
        //        if (result != null)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "FETCH DISCOUNT DETAIL ITEM!";
        //            rm.name = StatusName.ok;
        //            rm.data = result;
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
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
