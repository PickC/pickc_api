/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2025-01-02
 * Description:
*/
using appify.Business;
using appify.Business.Contract;
using appify.models;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static appify.models.NotificationType;
using appify.utility;


using System.ComponentModel.DataAnnotations;
using Razorpay.Api;
using Twilio.Types;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Azure.Core;
using NPOI.SS.Formula.Functions;
using Twilio.TwiML.Fax;
using Twilio.TwiML.Voice;
using System.Buffers.Text;
using Microsoft.AspNetCore.Http.HttpResults;


namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    public class CommonServicesController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IOrderBusiness orderBusiness;
        private readonly INotificationBusiness notificationBusiness;
        private readonly IWebHostEnvironment env;

        private ResponseMessage rm;

        public CommonServicesController(IConfiguration configuration, IEventLogBusiness eventLogBusiness, IOrderBusiness orderBusiness, INotificationBusiness notificationBusiness,IWebHostEnvironment env)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            this.configuration = configuration;
            this.eventLogBusiness = eventLogBusiness;
            this.orderBusiness = orderBusiness;
            this.notificationBusiness = notificationBusiness;
            this.env = env;
        }

        [HttpPost]
        [Route("Delhivery/GetOrderStatus")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetDelhiveryOrderStatus(string awbNumber)
        {
            GetExpectedDeliveryDateAsync(awbNumber);
            return Ok(0);
        }
        private async Task<string?> GetExpectedDeliveryDateAsync(string awbNumber)
        {
            using var client = new HttpClient();
            var token = configuration["OneDelhiveryKey:Key"].ToString();
            client.DefaultRequestHeaders.Add("Authorization", $"Token {token}");

            var response = await client.GetAsync($"https://track.delhivery.com/api/v1/packages/json/?waybill={awbNumber}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(json);

            var edd = data["ShipmentData"]?[0]?["Shipment"]?["ExpectedDeliveryDate"]?.ToString();
            return edd;
        }

        /// <summary>
        ///     Get The One Delivery Shipment Cost
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "startPin": "500081",
        ///       "destPin": "500001",
        ///       "weight": 500.0,
        ///       "payType": "COD",
        ///       "codAmount": 900.0
        ///     } 
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">DELHIVERY SHIPMENT COST HAS BEEN SUCCESSFULLY FETCHED!</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("Delhivery/GetShipmentCost")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> getShipmentCost(DelhiveryShipmentCost itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Clear();
                var ApiToken = configuration["OneDelhiveryKey:Key"].ToString();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {ApiToken}");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var weightInGrams = itemData.Weight;
                var codValue = itemData.CODAmount;

                var url = $"{Common.OneDelhiveryShipmentCost}?md=S&ss=Delivered&o_pin={itemData.StartPin}&d_pin={itemData.DestPin}&cod={codValue}&cgm={weightInGrams}";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // throws if status code is not 2xx

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JArray jsonArray = JArray.Parse(jsonResponse);
                //string cleanJson = jsonArray.ToString(Formatting.None);
                //return jsonResponse;
                rm.statusCode = StatusCodes.OK;
                rm.message = "Delhivery Shipment Cost has been successfully fetched";
                rm.name = StatusName.ok;
                //rm.data = jsonArray;


                var jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                decimal totalAmount = jsonObj[0].total_amount;

                
                rm.data = new { total_amount = totalAmount };


            }
            catch (HttpRequestException ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("GET SHIPMENT COST - ERROR", reqHeader, controllerURL, itemData, null, StatusName.ok, this.eventLogBusiness);
            }
            return Ok(rm);
        }

        /// <summary>
        ///     Get The One Delivery Pincode
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///    {
        ///      "pinCode": "500081"
        ///    }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">DELHIVERY PINCODE HAS BEEN SUCCESSFULLY FETCHED!</response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("Delhivery/GetPincode")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> getPinCode(DeliveryPinCode itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Clear();
                var ApiToken = configuration["OneDelhiveryKey:Key"].ToString();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Token {ApiToken}");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"{Common.OnDeliveryPincodeService}/?filter_codes={itemData.PinCode}";

                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var parsedJson = JsonConvert.DeserializeObject(jsonResponse);
                rm.statusCode = StatusCodes.OK;
                rm.message = "Delhivery Pincode has been successfully fetched";
                rm.name = StatusName.ok;
                rm.data = parsedJson;



                // Get the delivery_codes array

                var jsonObj = JObject.Parse(jsonResponse);
                var deliveryCodes = jsonObj["delivery_codes"] as JArray;

                // Return true if array exists and has elements, false otherwise
                bool canDeliver= deliveryCodes != null && deliveryCodes.Count > 0;
                
                rm.data = canDeliver;


            }
            catch (HttpRequestException ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
                await Common.UpdateEventLogsNew("GET PINCODE - ERROR", reqHeader, controllerURL, itemData, null, StatusName.ok, this.eventLogBusiness);
            }
            return Ok(rm);
        }
    }

}
