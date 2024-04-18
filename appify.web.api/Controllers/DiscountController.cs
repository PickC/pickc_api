using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
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


        /// <summary>
        /// Adds a Product's Discount.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : For a new Product Discount object, send the DiscountID = 0.
        /// 
        ///     {
        ///         "DiscountID": 0,
        ///         "VendorID": 1505,
        ///         "DiscountType": 3001,
        ///         "DiscountValue": 0.33,
        ///         "EffectiveDate": "2024-04-11T15:55:06.807",
        ///         "ExpiryDate": "2024-04-18T15:55:06.807",
        ///         "IsCancel": false,
        ///         "CreatedBy": 1505,
        ///         "CreatedOn": "2024-04-11T18:20:59.953",
        ///         "ModifiedBy": "",
        ///         "ModifiedOn": "2024-04-11T18:21:53.250",
        ///         "DiscountDetails": [
        ///             {
        ///                 "DiscountID": 1000,
        ///                 "ProductID": 1005,
        ///                 "IsActive": true
        ///             }
        ///          ]
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <param name="discountHeader"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
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
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
            }

            return Ok(rm);
        }

        /// <summary>
        /// removes Product's Discount Item
        /// </summary>
        /// <remarks>
        /// Sample Data :
        /// 
        ///     {
        ///         "DiscountID":1000,
        ///         "ModifiedBy":1505
        ///     }
        /// 
        /// </remarks>
        /// <param name="itemData"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Boolean Value </response>
        /// <response code="500">ResponseMessage with Error Description</response> 

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



        /// <summary>
        /// gets Product's Discount Item
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "DiscountID":1000
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///         "DiscountID": 1000,
        ///         "VendorID": 1505,
        ///         "DiscountType": 3001,
        ///         "DiscountValue": 0.33,
        ///         "EffectiveDate": "2024-04-11T15:55:06.807",
        ///         "ExpiryDate": "2024-04-18T15:55:06.807",
        ///         "IsCancel": false,
        ///         "CreatedBy": 1505,
        ///         "CreatedOn": "2024-04-11T18:20:59.953",
        ///         "ModifiedBy": "",
        ///         "ModifiedOn": "2024-04-11T18:21:53.250",
        ///         "DiscountDetails": [
        ///             {
        ///                 "DiscountID": 1000,
        ///                 "ProductID": 1005,
        ///                 "IsActive": true
        ///             }
        ///          ]
        ///     }
        /// 
        /// </remarks>
        /// <param name="DiscountID"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <remarks>
        /// 
        /// </remarks>
        /// <response code="500">ResponseMessage with Error Description</response> 



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

        /// <summary>
        /// gets Product's Discount Items LIST (ONLY FOR TEST PURPOSE, NOT IMPLEMENTED IN THE APPS)
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        /// Sample response JSON :
        /// 
        ///     {
        ///         "DiscountID": 1000,
        ///         "VendorID": 1505,
        ///         "DiscountType": 3001,
        ///         "DiscountValue": 0.33,
        ///         "EffectiveDate": "2024-04-11T15:55:06.807",
        ///         "ExpiryDate": "2024-04-18T15:55:06.807",
        ///         "IsCancel": false,
        ///         "CreatedBy": 1505,
        ///         "CreatedOn": "2024-04-11T18:20:59.953",
        ///         "ModifiedBy": "",
        ///         "ModifiedOn": "2024-04-11T18:21:53.250",
        ///         "DiscountDetails": [
        ///             {
        ///                 "DiscountID": 1000,
        ///                 "ProductID": 1005,
        ///                 "IsActive": true
        ///             }
        ///          ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns DiscountHeader Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
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
        


        /// <summary>
        /// gets Product's Discount items by vendor
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "VendorID":1505
        ///     }
        /// 
        /// Sample response JSON :
        /// 
        ///     [
        ///         {
        ///             "DiscountID": 1000,
        ///             "EffectiveDate": "2024-04-16T15:55:06.807",
        ///             "ExpiryDate": "2024-04-18T15:55:06.807",
        ///             "ProductID": 1005,
        ///             "ProductName": "Tshirt",
        ///             "Description": "boys and girls kids white tshirts",
        ///             "Brand": "qikink kids ",
        ///             "Price": 200.00,
        ///             "DiscountType": 3003,
        ///             "DiscountValue": 0.43,
        ///             "DiscountTypeDescription": "",
        ///             "IsActive": 1,
        ///             "ImageID": 1014,
        ///             "ImageName": "https:\/\/appifystorage.blob.core.windows.net\/appifystoragecontainer\/image_cropper_1694071692121.jpg"
        ///         }
        ///     ]    
        /// </remarks>
        /// <param name="VendorID"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Discounted Products List Object </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
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

        //[HttpPost, Route("Detail/Remove")]
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

        //[HttpPost, Route("Detail/Get")]
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

        //[HttpPost, Route("Detail/List")]
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

        #endregion
    }
}
