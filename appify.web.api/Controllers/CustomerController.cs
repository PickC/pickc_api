using appify.Business;
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Razorpay.Api;
using System.Net;
using System.Text;
using Asp.Versioning;
using System.Net.Http.Headers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Management;
namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    public class CustomerController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly ICustomerBusiness customerBusiness;
        private ResponseMessage rm;
        private readonly string[] allowedCountries = { "IN" };
        public CustomerController(IConfiguration configuration,ICustomerBusiness customerBusiness, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.customerBusiness = customerBusiness;
            this.eventLogBusiness = eventLogBusiness;
        }

        /// <summary>
        /// gets Product items information based on Vendor ID
        /// </summary>
        /// <remarks>
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.0 (DEFAULT version)
        /// 
        /// Description : Gets Product items information based on Vendor ID 
        /// 
        /// -----------------------------------------------------------------------------------------
        /// 
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1847
        ///     }
        ///     
        /// -----------------------------------------------------------------------------------------
        /// 
        /// version : 1.1
        /// 
        /// Description : Gets Product List New Item information based on Vendor ID
        /// 
        /// -----------------------------------------------------------------------------------------
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1505
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///	    [
        ///		    {
        ///          	"productID": 1315,
        ///          	"vendorID": 0,
        ///          	"productName": "ELEGANT HOODY ",
        ///          	"category": 3713,
        ///          	"brand": "Polo",
        ///          	"price": 1429,
        ///          	"imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1702444422717.jpg",
        ///          	"isNew": false
        ///    	    }
        ///	    ]
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("productlist")]
        [MapToApiVersion("1.0")]
        public IActionResult GetMemberProducts(ParamMemberUserID itemData) {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
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

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost]
        [Route("productlist")]
        [MapToApiVersion("1.1")]
        public IActionResult GetMemberProductsList(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                List<MemberProduct> items = customerBusiness.ProductListNew(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// gets Product items information based on Vendor ID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1060,
        ///         "categoryID": 3646,
        ///         "pageNo": 1,
        ///         "rows": 2
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///	    [
        ///		    {
        ///          	"productID": 1315,
        ///          	"vendorID": 0,
        ///          	"productName": "ELEGANT HOODY ",
        ///          	"category": 3713,
        ///          	"brand": "Polo",
        ///          	"price": 1429,
        ///          	"imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1702444422717.jpg",
        ///          	"isNew": false
        ///    	    }
        ///	    ]
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("productlistbycategory")]
        [MapToApiVersion("1.0")]
        public IActionResult GetMemberProductsByCategory(ParamCategoryID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                List<MemberProduct> items = customerBusiness.ProductListByCategory(itemData.userID, itemData.categoryID, itemData.PageNo, itemData.Rows);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH PRODUCT LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH PRODUCT LIST BY CATEGORY SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST BY CATEGORY - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PRODUCT LIST BY CATEGORY - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// gets All Details based on Member ID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1505
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///	    [
        ///         {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH ALL DETAILS",
        ///           "data": {
        ///             "wareHouse": true,
        ///             "products": 47,
        ///             "category": true,
        ///             "appDetails": true
        ///           }
        ///         }
        ///	    ]
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("AllDetails")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetAllDetails(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var items = customerBusiness.GetMemberAllDetails(itemData.userID);
                if (items != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ALL DETAILS";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("FETCH ALL DETAILS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                    await Common.UpdateEventLogsNew("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// gets Product List based on UserID
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///         "userID": 1060
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///	    [
        ///         {
        ///           "statusCode": 200,
        ///           "name": "SUCCESS_OK",
        ///           "message": "FETCH ALL DETAILS",
        ///           "data": {
        ///             "wareHouse": true,
        ///             "products": 47,
        ///             "category": true,
        ///             "appDetails": true
        ///           }
        ///         }
        ///	    ]
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("productlistbyvaua")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> GetProductListByVAUA(ParamMemberUserID itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();

                var items = customerBusiness.GetProductListByVAUA(itemData.userID);
                if (items != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH ALL DETAILS";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("FETCH ALL DETAILS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                    await Common.UpdateEventLogsNew("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// This END POINT JUST FOR TESTING DIFFERENT FUCNTIONS
        /// </summary>
        [HttpPost]
        [Route("generatetoken")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> generatetoken()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();


                string clientId = "604537213086-2r3o5j2ljn2rpdkhsfsd34vspki0v4nq.apps.googleusercontent.com";
                string clientSecret = "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";
                string refreshToken = "YourRefreshToken";
                string fromEmail = "gurjeet@appi-fy.ai";
                string toEmail = "nkolweb@gmail.com";

                var clientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets,
                    new[] { "https://mail.google.com/" },
                    "user",
                    CancellationToken.None);

                if (credential.Token.IsExpired(credential.Flow.Clock))
                {
                    await credential.RefreshTokenAsync(CancellationToken.None);
                }



                //var message = new MimeMessage();
                //message.From.Add(new MailboxAddress("Your Name", fromEmail));
                //message.To.Add(new MailboxAddress("Recipient Name", toEmail));
                //message.Subject = "Test Email";
                //message.Body = new TextPart(TextFormat.Plain)
                //{
                //    Text = "This is a test email sent using OAuth2 in .NET Core."
                //};

                //using (var client = new SmtpClient())
                //{
                //    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //    var oauth2 = new SaslMechanismOAuth2(fromEmail, credential.Token.AccessToken);
                //    await client.AuthenticateAsync(oauth2);
                //    await client.SendAsync(message);
                //    await client.DisconnectAsync(true);
                //}

                return Ok(credential.Token);

                //var items = customerBusiness.GetMemberPasswordList();
                //foreach (var item in items)
                //{
                //string pass = DataHash.EncryptData(item.OldPassword);
                //var result = customerBusiness.SaveMemberPassword(item.UserID, pass);
                //}


                /////// Twilio
                //string accountSid = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMSNotification:accountSid").Value;
                //string authToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMSNotification:authToken").Value;

                //TwilioClient.Init(accountSid, authToken);
                //var message = MessageResource.Create(
                //    body: "Join Earth's mightiest heroes. Like Kevin Bacon.",
                //    from: new Twilio.Types.PhoneNumber("+919885217825"),
                //    to: new Twilio.Types.PhoneNumber("+919810722979")
                //);

                //var result = message.Sid;


                /////////////// ENCRYPTION
                // string pass = DataHash.EncryptData("Appify@123");
                //bool repass = DataHash.DecryptData("Appify@123", "UN5QLpw54G5gRLD8yQhD915AqsAgfcEIQxhnyGT8ryCJXRO+WX1Ikl0/RntoGB9Q8P9avvjxAvfSN+LR7bpj7g==");
                /////////////// IP2 LOCATION
                //var result = Common.CheckIPAddress(HttpContext, allowedCountries);

                //var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                //new ClientSecrets
                //{
                //    ClientId = "853172481735-ll2ic3il1dq9nioa7c4gh9a2g8l310pr.apps.googleusercontent.com",
                //    ClientSecret = "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"
                //},
                //new[] { "email", "profile", "https://mail.google.com/" },
                //"user",
                //CancellationToken.None
                //);

                //var jwtPayload = GoogleJsonWebSignature.ValidateAsync(credential.Token.IdToken).Result;
                //var username = jwtPayload.Email;

                rm.statusCode = StatusCodes.OK;
                rm.message = "result";
                //rm.name = orderId;
                //rm.data = order;


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

    }
}
