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

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    public class AddressController : Controller
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IAddressBusiness addressBusiness;
        private ResponseMessage rm;
        private HttpClient httpClient;
        public AddressController(IConfiguration configuration, IAddressBusiness iResultData, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.addressBusiness = iResultData;
            this.eventLogBusiness = eventLogBusiness;
        }

    /// <summary>
    /// Adds/Update an Address
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "addressID": 0,
    ///       "linkID": 1060,
    ///       "addressType": 3517,
    ///       "houseNo": "B.29/27-k-1",
    ///       "address1": "sankat mochan road",
    ///       "address2": "varanasi",
    ///       "landmark": "Royal GYM",
    ///       "city": "Bonala",
    ///       "zipCode": "505301",
    ///       "alternateNo": "string",
    ///       "state": "Telangana",
    ///       "country": "India",
    ///       "latitude": 0.0,
    ///       "longitude": 0.0,
    ///       "isDefault": true,
    ///       "isActive": true,
    ///       "createdOn": "2024-09-30T11:32:23.830Z",
    ///       "modifiedOn": "2024-09-30T11:32:23.830Z",
    ///       "locationID": "3517_1727669597784"
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "ADDRESS SAVED SUCCESSFULLY!",
    ///       "data": {
    ///         "addressID": 1602,
    ///         "linkID": 1060,
    ///         "addressType": 3517,
    ///         "houseNo": "B.29/27-k-1",
    ///         "address1": "sankat mochan road",
    ///         "address2": "varanasi",
    ///         "landmark": "Royal GYM",
    ///         "city": "Bonala",
    ///         "zipCode": "505301",
    ///         "alternateNo": "string",
    ///         "state": "Telangana",
    ///         "country": "India",
    ///         "latitude": 0,
    ///         "longitude": 0,
    ///         "isDefault": true,
    ///         "isActive": true,
    ///         "createdOn": "2024-09-30T11:32:23.83Z",
    ///         "modifiedOn": "2024-09-30T11:32:23.83Z",
    ///         "locationID": "3517_1727669597784"
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Address Item against the AddressID </response>
    /// <response code="500">ResponseMessage with Error Description</response> 

    [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Add(Address item)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                var result = addressBusiness.SaveAddress(item);
                if (result!=null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ADDRESS SAVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, result, StatusName.ok));
                    await Common.UpdateEventLogsNew("ADDRESS SAVED SUCCESSFULLY", reqHeader, controllerURL, item, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                    await Common.UpdateEventLogsNew("ADDRESS SAVED - NO CONTENT", reqHeader, controllerURL, item, result, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Master", reqHeader, controllerURL, item, null, rm.message));
                await Common.UpdateEventLogsNew("ADDRESS SAVED - ERROR", reqHeader, controllerURL, item, null, rm.message, this.eventLogBusiness);
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
        ///       "userID": 1060,
        ///       "addressID": 1647
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "ADDRESS REMOVED",
        ///       "data": true
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Remove Address Item against the AddressID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("remove")]
        [MapToApiVersion("1.0")]

        public IActionResult Remove(ParamAddress itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = addressBusiness.DeleteAddress(itemData.addressID, itemData.userID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ADDRESS REMOVED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ADDRESS REMOVED SUCCESSFULLY", reqHeader, controllerURL, itemData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ADDRESS REMOVED - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ADDRESS REMOVED - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

    /// <summary>
    /// Get an Address
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "userID": 1923,
    ///       "addressID": 1649
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "FETCH ADDRESS ITEM",
    ///       "data": {
    ///         "addressID": 1649,
    ///         "linkID": 1923,
    ///         "addressType": 2001,
    ///         "houseNo": "g54",
    ///         "address1": "gngjgu",
    ///         "address2": "jvjvj",
    ///         "landmark": "",
    ///         "city": "Mathrusri Nagar",
    ///         "zipCode": "500049",
    ///         "alternateNo": "",
    ///         "state": "Telangana",
    ///         "country": "In",
    ///         "latitude": 0,
    ///         "longitude": 0,
    ///         "isDefault": false,
    ///         "isActive": true,
    ///         "createdOn": "2024-09-27T06:47:05.94",
    ///         "modifiedOn": "0001-01-01T00:00:00",
    ///         "locationID": "2001_1727419623640"
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">Returns Address Item</response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 

    [HttpPost, Route("getaddress")]
        [MapToApiVersion("1.0")]
        public IActionResult GetAddress(ParamAddress itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();

                var item = addressBusiness.GetAddress(itemData.addressID,itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ADDRESS ITEM";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET ADDRESS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET ADDRESS NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET ADDRESS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get Default an Address
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
        ///       "message": "FETCH ADDRESS ITEM",
        ///       "data": {
        ///         "addressID": 1602,
        ///         "linkID": 1060,
        ///         "addressType": 3517,
        ///         "houseNo": "B.29/27-k-1",
        ///         "address1": "sankat mochan road",
        ///         "address2": "varanasi",
        ///         "landmark": "Royal GYM",
        ///         "city": "Bonala",
        ///         "zipCode": "505301",
        ///         "alternateNo": "string",
        ///         "state": "Telangana",
        ///         "country": "In",
        ///         "latitude": 0,
        ///         "longitude": 0,
        ///         "isDefault": true,
        ///         "isActive": true,
        ///         "createdOn": "2024-08-21T11:33:57.1",
        ///         "modifiedOn": "2024-09-30T13:48:05.017",
        ///         "locationID": "3517_1727669597784"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Address Item</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("getdefaultaddress")]
        [MapToApiVersion("1.0")]
        public IActionResult GetAddress(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();

                var item = addressBusiness.GetDefaultAddress(itemData.userID);

                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ADDRESS ITEM";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DEFAULT ADDRESS SUCCESSFULLY", reqHeader, controllerURL, itemData, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DEFAULT ADDRESS NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GET DEFAULT ADDRESS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Get Default an Address
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
        ///       "message": "ADDRESS LIST",
        ///       "data": [
        ///         {
        ///           "addressID": 1602,
        ///           "linkID": 1060,
        ///           "addressType": 3517,
        ///           "houseNo": "B.29/27-k-1",
        ///           "address1": "sankat mochan road",
        ///           "address2": "varanasi",
        ///           "landmark": "Royal GYM",
        ///           "city": "Bonala",
        ///           "zipCode": "505301",
        ///           "alternateNo": "string",
        ///           "state": "Telangana",
        ///           "country": "INDIA",
        ///           "latitude": 0,
        ///           "longitude": 0,
        ///           "isDefault": true,
        ///           "isActive": true,
        ///           "createdOn": "2024-08-21T11:33:57.1",
        ///           "modifiedOn": "2024-09-30T13:48:05.017",
        ///           "locationID": "3517_1727669597784"
        ///         },
        ///         {
        ///           "addressID": 1616,
        ///           "linkID": 1060,
        ///           "addressType": 2000,
        ///           "houseNo": "sy 21, krishe emerald",
        ///           "address1": "kondapur",
        ///           "address2": "near flyover",
        ///           "landmark": "",
        ///           "city": "Kondapur",
        ///           "zipCode": "500081",
        ///           "alternateNo": "",
        ///           "state": "Telangana",
        ///           "country": "INDIA",
        ///           "latitude": 17.4577156,
        ///           "longitude": 78.368075,
        ///           "isDefault": true,
        ///           "isActive": true,
        ///           "createdOn": "2024-08-26T09:31:40.82",
        ///           "modifiedOn": "2024-09-30T06:47:06.21",
        ///           "locationID": "HOME_1724664697161"
        ///         },
        ///         {
        ///         "addressID": 1629,
        ///           "linkID": 1060,
        ///           "addressType": 3518,
        ///           "houseNo": "15-21-160",
        ///           "address1": "Balaji nagar",
        ///           "address2": "near ysr statue",
        ///           "landmark": "",
        ///           "city": "Kukatpally",
        ///           "zipCode": "500072",
        ///           "alternateNo": "6281438226",
        ///           "state": "Telangana",
        ///           "country": "INDIA",
        ///           "latitude": 0,
        ///           "longitude": 0,
        ///           "isDefault": false,
        ///           "isActive": true,
        ///           "createdOn": "2024-09-10T04:11:56.77",
        ///           "modifiedOn": "2024-09-30T06:47:05.57",
        ///           "locationID": "3518_1727654162033"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Address Item List</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        public IActionResult List(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                List<Address> items = addressBusiness.GetList(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ADDRESS LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ADDRESS LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ADDRESS LIST - NO CENTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ADDRESS LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        //[HttpPost, Route("saveaddress")]
        //[MapToApiVersion("1.0")]
        //public async Task<IActionResult> SaveAddress()
        //{
        //    var reqHeader = Request;
        //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    dynamic data = jsonData;
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        List<Address> items = addressBusiness.GetAddressList();

        //        AddressObj Address = new AddressObj
        //        {
        //            name = "WareHouseABCJDG",
        //            email = "gurjeetrayat84@gmail.com",
        //            phone = "9810722979",
        //            address = "WZ-51, Guru Nanak Nagar, Street No - 06",
        //            city = "New Delhi",
        //            country = "India",
        //            pin = "110018",
        //            return_address = "WZ-51, Guru Nanak Nagar, Street No - 06",
        //            return_pin = "110018",
        //            return_city = "New Delhi",
        //            return_state = "Delhi",
        //            return_country = "India"
        //        };
        //        SaveAddressToOneDelivery(Address);
        //        if (items?.Any() == true)
        //        {
        //            rm.statusCode = StatusCodes.OK;
        //            rm.message = "ADDRESS LIST";
        //            rm.name = StatusName.ok;
        //            rm.data = items;
        //            // Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
        //            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, items, StatusName.ok));
        //            await Common.UpdateEventLogsNew("OneDelhivery SAVED ADDRESS SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
        //        }
        //        else
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "NO CONTENT";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //            // Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
        //            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
        //            await Common.UpdateEventLogsNew("OneDelhivery - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = null;
        //        this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, rm.message));
        //        await Common.UpdateEventLogsNew("OneDelhivery SAVED ADDRESS - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        //    }
        //    return Ok(rm);

        //}

        //private static async Task<string> SaveAddressToOneDelivery(AddressObj Address)
        //{
        //     Create an instance of HttpClient
        //    using (var client = new HttpClient())
        //    {
        //        string responseBody = "";
        //        try
        //        {
        //            var uri = new Uri(Common.OneDelhiveryCreateURL);
        //            client.BaseAddress = new Uri(Common.OneDelhiveryCreateURL);
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", Common.OneDelhiveryToken);

        //            var jsonString = Common.ConvertObjectToJson(Address);
        //            HttpResponseMessage Res = client.PostAsync(uri, new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
        //            var response = await Res.Content.ReadAsStringAsync();


        //            //HttpContent httpContent = new StringContent(json);
        //            // StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
        //            //var response = await client.PostAsync(OneDelhiveryCreateURL, httpContent);
        //            HttpResponseMessage response = await client.PostAsJsonAsync(uri, Address);

        //             Check if the response is successful
        //            if (Res.IsSuccessStatusCode)
        //            {
        //                responseBody = await Res.Content.ReadAsStringAsync();
        //            }
        //            else
        //            {
        //                responseBody = Res.StatusCode.ToString();
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            responseBody = ex.ToString();

        //        }

        //        return responseBody;
        //    }
        //}

    }
}
