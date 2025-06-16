/*
 * Company: AppifyRetail.
 * Author: Gurjeet
 * Version: 1.1
 * Date: 2025-16-10
 * Description:
*/
using appify.Business.Contract;
using appify.models;
using appify.utility;
using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using Azure.Storage.Blobs.Models;
using System;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]
    public class RazorpayController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IWebHostEnvironment env;
        private ResponseMessage rm;
        public RazorpayController(IConfiguration configuration, IWebHostEnvironment env, IEventLogBusiness eventLogBusiness)
        {
            this.configuration = configuration;
            this.eventLogBusiness = eventLogBusiness;
            this.env = env;
        }

        //// <summary>
        //// 1. Name and email fields are mandatory for each account & phone number is optional
        //// 2. (50.00 MB Max)
        //// 3. The number of accounts in the file should not exceed 500.
        //// </summary>
        //[HttpPost]
        //[Route("CreateLinkedAccounts")]
        //[MapToApiVersion("1.0")]
        ////[Consumes("multipart/form-data")]
        //public async Task<IActionResult> CreateLinkedAccounts([Required] IFormFile file)//([Required]IFormFile file)
        //{
        //    var reqHeader = Request;
        //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        string responseBody = "NO FILE OR INVALID FILE FORMAT";
        //        //////[FromForm] ParamEmailFields item
        //        if (file == null || file.Length == 0)
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //            return Ok(rm);
        //        }
        //        if (Path.GetExtension(file.FileName).ToLower() != ".csv"
        //            && Path.GetExtension(file.FileName).ToLower() != ".xls"
        //            && Path.GetExtension(file.FileName).ToLower() != ".xlsx"
        //            && Path.GetExtension(file.FileName).ToLower() != ".xlsb")
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.message = "FILE TYPE ONLY ACCEPT .csv, .xls, .xlsx, .xlsb";
        //            rm.name = StatusName.invalid;
        //            rm.data = null;
        //            return Ok(rm);
        //        }

        //        if (file.Length > Common.IMAGE_SIZE)
        //        {
        //            rm.statusCode = StatusCodes.ERROR;
        //            rm.name = StatusName.invalid;
        //            rm.message = "FILE SIZE GREATER THAN 50 MB";
        //            rm.data = null;
        //            return Ok(rm);
        //        }

        //        rm.statusCode = StatusCodes.OK;
        //        rm.message = "BULK UPLOAD HAS BEEN SUCCESSFULLY SAVED";
        //        rm.name = StatusName.ok;
        //        rm.data = responseBody;


        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = ex.Message.ToString();
        //        await Common.UpdateEventLogsNew("For Testing Different Functions", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        //    }
        //    return Ok(rm);

        //}

        /// <summary>
        /// Create Linked Account - STEP 1
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "email": "vrajbrahm007@gmail.com",
        ///       "phone": "9810722970",
        ///       "type": "route",
        ///       "legal_business_name": "KP Furniture",
        ///       "contact_name": "Vraj Brahm",
        ///       "business_type": "individual",
        ///       "profile": {
        ///         "category": "ecommerce",
        ///         "subcategory": "ecommerce_marketplace",
        ///         "addresses": {
        ///           "registered": {
        ///             "street1": "507, Koramangala 1st block",
        ///             "street2": "MG Road",
        ///             "city": "Bengaluru",
        ///             "state": "KARNATAKA",
        ///             "postal_code": "560034",
        ///             "country": "IN"
        ///           }
        ///         }
        ///       }
        ///     }
        ///     
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "Linked Account has been created successfully",
        ///       "data": {
        ///         "id": "acc_QgGd0eULKGM8gd",
        ///         "type": "route",
        ///         "status": "created",
        ///         "email": "vrajbrahm007@gmail.com",
        ///         "profile": {
        ///           "category": "ecommerce",
        ///           "subcategory": "ecommerce_marketplace",
        ///           "addresses": {
        ///             "registered": {
        ///               "street1": "507, Koramangala 1st block",
        ///               "street2": "MG Road",
        ///               "city": "Bengaluru",
        ///               "state": "KARNATAKA",
        ///               "postal_code": "560034",
        ///               "country": "IN"
        ///             }
        ///           }
        ///         },
        ///         "notes": [],
        ///         "created_at": 1749727496,
        ///         "live": false,
        ///         "hold_funds": false,
        ///         "phone": "+919810722970",
        ///         "contact_name": "Vraj Brahm",
        ///         "business_type": "not_yet_registered",
        ///         "legal_business_name": "KP Furniture",
        ///         "customer_facing_business_name": "KP Furniture"
        ///       }
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>
        [HttpPost]
        [Route("CreateLinkedAccount")] ////// Working to create new route link account
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> CreateLinkedAccount(Merchant itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            //var accountData0 = new
            //{
            //    email = "saurabhbag08@gmail.com",
            //    phone = "7972391084",
            //    type = "route",
            //    reference_id = "124124",
            //    legal_business_name = "S Garments",
            //    business_type = "individual",
            //    contact_name = "Saurabh Bag",
            //    profile = new
            //    {
            //    category = "healthcare",
            //        subcategory = "clinic",
            //        addresses = new
            //        {
            //            registered = new
            //            {
            //                street1 = "507, Koramangala 1st block",
            //                street2 = "MG Road",
            //                city = "Bengaluru",
            //                state = "KARNATAKA",
            //                postal_code = "560034",
            //                country = "IN"
            //            }
            //        }
            //    },
            //    legal_info = new
            //    {
            //        pan = "AAACL1234C",
            //        gst = "18AABCU9603R1ZM"
            //    }
            //};

            var accountData = new
            {
                email = "vrajbrahm08@gmail.com",
                phone = "9810722910",
                type = "route",
                legal_business_name = "KP Furniture",
                business_type = "individual",
                profile = new
                {
                    category = "services",
                    subcategory = "contractors",
                    addresses = new
                    {
                        registered = new
                        {
                            street1 = "507, Koramangala 1st block",
                            street2 = "MG Road",
                            city = "Bengaluru",
                            state = "KARNATAKA",
                            postal_code = "560034",
                            country = "IN"
                        }
                    }
                }
            };

            // Serialize the account data to JSON
            string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(itemData);

            // Create HttpClient instance
            using (var client = new HttpClient())
            {
                // Set up Basic Authentication
                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                // Set up the request content
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    // Send the POST request to Razorpay API
                    HttpResponseMessage response = await client.PostAsync(Common.RazorPayCreateAccount, content);

                    // Handle the response
                    if (response.IsSuccessStatusCode)
                    {
                        dynamic responseBody =  JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                        //string status = responseBody.;
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Linked Account has been created successfully";
                        rm.name = StatusName.ok;
                        rm.data = responseBody;
                    }
                    else
                    {
                        var errorResponse = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Error creating linked account";
                        rm.name = StatusName.ok;
                        rm.data = errorResponse;
                    }
                }
                catch (Exception ex)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = ex.Message.ToString();
                    rm.name = StatusName.invalid;
                    rm.data = ex.Message.ToString();
                }
            }
            return Ok(rm);
        }

        /// <summary>
        /// Create Stake Holder - STEP 2
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "name": "Saurabh Bag",
        ///       "email": "saurabh@example.com",
        ///       "phone": {
        ///         "primary": "9876543210"
        ///       },
        ///       "percentage_ownership": 100,
        ///       "relationship": {
        ///         "director": false,
        ///         "executive": true
        ///       },
        ///       "addresses": {
        ///         "residential": {
        ///           "street": "Koramangala",
        ///           "city": "Bangalore",
        ///           "state": "Karnataka",
        ///           "postal_code": "560034",
        ///           "country": "IN"
        ///         }
        ///       },
        ///       "kyc": {
        ///         "pan": "CEWPB1197K"
        ///       }
        ///     }
        ///     
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "Stakeholder successfully added",
        ///       "data": {
        ///         "id": "sth_QgHiQh5wHHeKIP",
        ///         "entity": "stakeholder",
        ///         "relationship": {
        ///           "executive": true
        ///         },
        ///         "phone": {
        ///           "primary": "9876543210"
        ///         },
        ///         "notes": [],
        ///         "kyc": {
        ///           "pan": "CEWPB1197K"
        ///         },
        ///         "name": "Saurabh Bag",
        ///         "email": "saurabh@example.com",
        ///         "percentage_ownership": 100,
        ///         "addresses": {
        ///           "residential": {
        ///             "street": "Koramangala",
        ///             "city": "Bangalore",
        ///             "state": "Karnataka",
        ///             "postal_code": "560034",
        ///             "country": "in"
        ///           }
        ///         }
        ///       }
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>
        [HttpPost]
        [Route("CreateAStakeHolder")] ////// Working to create new stake holder account
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> AddStakeholderAsync(string accountId, StakeholderPayload itemData)
        {
            string url = $"{Common.RazorPayCreateAccount}/{accountId}/stakeholders";
            rm = new ResponseMessage();

            var payload = new
            {
                name = "Saurabh Bag",
                email = "saurabh@example.com",
                phone = new
                {
                    primary = "9876543210"
                },
                percentage_ownership = 100,
                relationship = new
                {
                    director = false,
                    executive = true
                },
                addresses = new
                {
                    residential = new
                    {
                        street = "Koramangala",
                        city = "Bangalore",
                        state = "Karnataka",
                        postal_code = "560034",
                        country = "IN"
                    }
                },
                kyc = new { pan = "CEWPB1197K" }
            };

            using (var client = new HttpClient())
            {
                try
                {
                    var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                    var json = JsonConvert.SerializeObject(itemData);
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Stakeholder successfully added";
                        rm.name = StatusName.ok;
                        rm.data = JsonConvert.DeserializeObject(responseBody);
                    }
                    else
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Failed to add stakeholder";
                        rm.name = StatusName.ok;
                        rm.data = JsonConvert.DeserializeObject(responseBody);
                    }
                }
                catch (Exception ex)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = ex.Message;
                    rm.name = StatusName.invalid;
                    rm.data = ex.Message;
                }
            }

            return Ok(rm);
        }

        /// <summary>
        /// Request Route Product Configuration - STEP 3
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "accountID": "acc_QgDTR3dlYgZuZF"
        ///     }
        ///     
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "Route product configuration requested successfully",
        ///       "data": {
        ///         "requested_configuration": [],
        ///         "active_configuration": {
        ///           "settlements": {
        ///             "account_number": null,
        ///             "beneficiary_name": null,
        ///             "ifsc_code": null
        ///           }
        ///         },
        ///         "requirements": [
        ///           {
        ///             "field_reference": "settlements.beneficiary_name",
        ///             "resolution_url": "/accounts/acc_QgDTR3dlYgZuZF/products/acc_prd_QgI3hMXQRyRXyW",
        ///             "reason_code": "field_missing",
        ///             "status": "required"
        ///           },
        ///           {
        ///             "field_reference": "settlements.account_number",
        ///             "resolution_url": "/accounts/acc_QgDTR3dlYgZuZF/products/acc_prd_QgI3hMXQRyRXyW",
        ///             "reason_code": "field_missing",
        ///             "status": "required"
        ///           },
        ///           {
        ///         "field_reference": "settlements.ifsc_code",
        ///             "resolution_url": "/accounts/acc_QgDTR3dlYgZuZF/products/acc_prd_QgI3hMXQRyRXyW",
        ///             "reason_code": "field_missing",
        ///             "status": "required"
        ///           }
        ///         ],
        ///         "tnc": {
        ///         "id": "tnc_QgI3hE6AaTha4f",
        ///           "accepted": true,
        ///           "accepted_at": 1749732533
        ///         },
        ///         "id": "acc_prd_QgI3hMXQRyRXyW",
        ///         "product_name": "route",
        ///         "activation_status": "needs_clarification",
        ///         "account_id": "acc_QgDTR3dlYgZuZF",
        ///         "requested_at": 1749732533
        ///       }
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>
        [HttpPost]
        [Route("RequestRouteProduct")] ////// Working to create new stake holder account
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> RequestRouteProductAsync([FromBody] LinkedAccount itemData)
        {
            string url = $"{Common.RazorPayCreateAccount}/{itemData.AccountID}/products";
            rm = new ResponseMessage();

            var payload = new
            {
                product_name = "route",
                tnc_accepted = true
            };

            using (var client = new HttpClient())
            {
                try
                {
                    var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                    var json = JsonConvert.SerializeObject(payload);
                    HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Route product configuration requested successfully";
                        rm.name = StatusName.ok;
                        rm.data = JsonConvert.DeserializeObject(responseBody);
                    }
                    else
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Failed to request product";
                        rm.name = StatusName.ok;
                        rm.data = responseBody;
                    }
                }
                catch (Exception ex)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = ex.Message;
                    rm.name = StatusName.invalid;
                    rm.data = ex.Message;
                }
            }

            return Ok(rm);
        }

        /// <summary>
        /// Update Route Product Configuration - STEP 4
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "settlements": {
        ///         "account_number": "000501687518",
        ///         "ifsc_code": "ICIC0000005",
        ///         "beneficiary_name": "Saurabh Bag"
        ///       },
        ///       "tnc_accepted": true
        ///     }
        ///     
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "Bank info updated successfully",
        ///       "data": {
        ///         "requested_configuration": [],
        ///         "active_configuration": {
        ///           "settlements": {
        ///             "account_number": "000501687518",
        ///             "beneficiary_name": "Saurabh Bag",
        ///             "ifsc_code": "ICIC0000005"
        ///           }
        ///         },
        ///         "requirements": [],
        ///         "tnc": {
        ///           "id": "tnc_QgI3hE6AaTha4f",
        ///           "accepted": true,
        ///           "accepted_at": 1749732533
        ///         },
        ///         "id": "acc_prd_QgI3hMXQRyRXyW",
        ///         "product_name": "route",
        ///         "activation_status": "needs_clarification",
        ///         "account_id": "acc_QgDTR3dlYgZuZF",
        ///         "requested_at": 1749732533
        ///       }
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateRouteProduct")] ////// Working to create new stake holder account
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> UpdateRouteProductAsync(string accountId, string productId, UpdateProductConfigPayload itemData)
        {
            string url = $"{Common.RazorPayCreateAccount}/{accountId}/products/{productId}";
            rm = new ResponseMessage();

            var payload = new
            {
                bank_account = new
                {
                    ifsc_code = "ICIC0000005",
                    account_number = "000501687518",
                    beneficiary_name = "Saurabh Bag"
                }
            };

            using (var client = new HttpClient())
            {
                try
                {
                    var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                    var json = JsonConvert.SerializeObject(itemData);
                    var request = new HttpRequestMessage(new System.Net.Http.HttpMethod("PATCH"), url)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };

                    HttpResponseMessage response = await client.SendAsync(request);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Bank info updated successfully";
                        rm.name = StatusName.ok;
                        rm.data = JsonConvert.DeserializeObject(responseBody);
                    }
                    else
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Failed to update bank info";
                        rm.name = StatusName.ok;
                        rm.data = responseBody;
                    }
                }
                catch (Exception ex)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = ex.Message;
                    rm.name = StatusName.invalid;
                    rm.data = ex.Message;
                }
            }

            return Ok(rm);
        }
        /// <summary>
        /// Get Linked Account Status
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "accountID": "acc_QfrfMDvUqTdDgR"
        ///     }
        ///     
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "Linked Account has been successfully fetched",
        ///       "data": {
        ///         "id": "acc_QfrfMDvUqTdDgR",
        ///         "type": "route",
        ///         "status": "activated",
        ///         "email": "saurabh@appi-fy.ai",
        ///         "profile": {
        ///           "category": null,
        ///           "subcategory": null,
        ///           "addresses": []
        ///         },
        ///         "notes": [],
        ///         "created_at": 1749639589,
        ///         "live": true,
        ///         "hold_funds": false,
        ///         "activated_at": 1749639708,
        ///         "business_type": "not_yet_registered",
        ///         "legal_business_name": "Saurabh Collections",
        ///         "customer_facing_business_name": "Saurabh Collections"
        ///       }
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>
        [HttpPost]
        [Route("CheckLinkedAccountStatus")] ////// Working to check created route link account
        public async Task<IActionResult> CheckLinkedAccountStatusAsync(LinkedAccount itemData)
        {
            string url = $"{Common.RazorPayCreateAccount}/{itemData.AccountID}";
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            using (var client = new HttpClient())
            {
                try
                {
                    // Set authentication
                    var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                    // Call Razorpay API
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Read response
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic json = JsonConvert.DeserializeObject(responseBody);

                        // Extract useful info (status, activation status, etc.)
                        //string status = json.status;
                        //string email = json.email;
                        //string contact = json.contact_name;
                        //string createdAt = json.created_at;

                        //result = $"✅ Sub-Account Status: {status}, Contact: {contact}, Email: {email}, Created At: {createdAt}";
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Linked Account has been successfully fetched";
                        rm.name = StatusName.ok;
                        rm.data = json;
                    }
                    else
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Error fetching linked account";
                        rm.name = StatusName.ok;
                        rm.data = $"❌ API Error ({response.StatusCode}): {responseBody}";
                    }
                }
                catch (Exception ex)
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = ex.Message.ToString();
                    rm.name = StatusName.invalid;
                    rm.data = ex.Message.ToString();
                }
            }

            return Ok(rm);
        }

        /// <summary>
        /// Update Linked Account
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "phone": "9000090005",
        ///       "legal_business_name": "Acme Corp V4",
        ///       "contact_name": "New Contact new",
        ///       "profile": {
        ///         "category": "services",
        ///         "subcategory": "contractors",
        ///         "addresses": {
        ///           "registered": {
        ///             "street1": "5071, Koramangala 6th block4",
        ///             "street2": "Koramangala",
        ///             "city": "Bengaluru",
        ///             "state": "Karnataka",
        ///             "postal_code": "560040",
        ///             "country": "IN"
        ///           }
        ///         }
        ///       }
        ///     }
        ///     
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "Linked Account has been updated successfully",
        ///       "data": {
        ///         "id": "acc_QgDTR3dlYgZuZF",
        ///         "type": "route",
        ///         "status": "created",
        ///         "email": "vrajbrahm@gmail.com",
        ///         "profile": {
        ///           "category": "services",
        ///           "subcategory": "contractors",
        ///           "addresses": {
        ///             "registered": {
        ///               "street1": "5071, Koramangala 6th block3",
        ///               "street2": "Koramangala",
        ///               "city": "Bengaluru",
        ///               "state": "KARNATAKA",
        ///               "postal_code": "560048",
        ///               "country": "IN"
        ///             },
        ///             "operation": {
        ///               "street1": "5071, Koramangala 6th block",
        ///               "street2": "Koramangala",
        ///               "city": "Bengaluru",
        ///               "state": "KARNATAKA",
        ///               "country": "IN",
        ///               "postal_code": "560047"
        ///             }
        ///           }
        ///         },
        ///         "notes": [],
        ///         "created_at": 1749716387,
        ///         "live": false,
        ///         "hold_funds": false,
        ///         "phone": "+919000090004",
        ///         "contact_name": "New Contact Name",
        ///         "business_type": "not_yet_registered",
        ///         "legal_business_name": "Acme Corp V3",
        ///         "customer_facing_business_name": "Acme Corp V2"
        ///       }
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateLinkedAccount")]
        [MapToApiVersion("1.0")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateLinkedAccountAsync(string accountId, MerchantUpdate itemData)
        {
            //                customer_facing_business_name = "Acme Corp V2",
            var payload = new
            {
                phone = "9000090004",
                legal_business_name = "Acme Corp V3",
                profile = new
                {
                    addresses = new
                    {
                        registered = new
                        {
                            street1 = "5071, Koramangala 6th block3",
                            street2 = "Koramangala",
                            city = "Bengaluru",
                            state = "Karnataka",
                            postal_code = "560048",
                            country = "IN"
                        }
                    }
                },
                contact_name = "New Contact Name",
            };

            rm = new ResponseMessage();
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(itemData);
                var uri = $"{Common.RazorPayCreateAccount}/{accountId}";
                using (var client = new HttpClient())
                {
                    var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var request = new HttpRequestMessage(new System.Net.Http.HttpMethod("PATCH"), uri)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };

                    var response = await client.SendAsync(request);
                    var respContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Linked Account has been updated successfully";
                        rm.name = StatusName.ok;
                        rm.data = JsonConvert.DeserializeObject(respContent.ToString());
                    }
                    else
                    {
                        //var errorContent = await response.Content.ReadAsStringAsync();
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Error Update linked account";
                        rm.name = StatusName.ok;
                        rm.data = $"Update failed. Status: {response.StatusCode}, Message: {respContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            return Ok(rm);
        }

        [HttpPost]
        [Route("DeleteLinkedAccount")] //////// Working to delete Route Link Account(Suspended)
        [MapToApiVersion("1.0")]
        //[Consumes("multipart/form-data")]
        public async Task<IActionResult> DeleteLinkedAccount(LinkedAccount itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            try
            {
                using (var client = new HttpClient())
                {
                    // Set Basic Auth Header
                    var byteArray = Encoding.ASCII.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    var requestUri = $"{Common.RazorPayCreateAccount}/{itemData.AccountID}";

                    // Send DELETE request
                    var response = await client.DeleteAsync(requestUri);

                    if (response.IsSuccessStatusCode)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = $"Successfully deleted account: {itemData.AccountID}";
                        rm.name = StatusName.ok;
                        rm.data = response;
                    }
                    else
                    {
                        var content = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Failed to delete linked account";
                        rm.name = StatusName.ok;
                        rm.data = $"Failed to delete account: {itemData.AccountID}. Status: {response.StatusCode}, Message: {content}";
                    }
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = ex.Message.ToString();
            }
            return Ok(rm);

        }

        /// <summary>
        /// Create Split Payment
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "paymentId": "pay_QdAROHL12qtHHg",
        ///       "totalAmount":10000,
        ///       "currency": "INR",
        ///       "onHold": false,
        ///       "accountId": "acc_QBOuFMPEh3zBGm"
        ///     }
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "entity": "collection",
        ///       "count": 1,
        ///       "items": [
        ///         {
        ///           "id": "trf_QdSQe2BaoPPH7S",
        ///           "entity": "transfer",
        ///           "status": "pending",
        ///           "source": "pay_QdAROHL12qtHHg",
        ///           "recipient": "acc_QBOuFMPEh3zBGm",
        ///           "amount": 900,
        ///           "currency": "INR",
        ///           "amount_reversed": 0,
        ///           "notes": [],
        ///           "linked_account_notes": [],
        ///           "on_hold": false,
        ///           "on_hold_until": null,
        ///           "recipient_settlement_id": null,
        ///           "created_at": 1749114033,
        ///           "processed_at": null,
        ///           "error": {
        ///             "code": null,
        ///             "description": null,
        ///             "reason": null,
        ///             "field": null,
        ///             "step": null,
        ///             "id": "trf_QdSQe2BaoPPH7S",
        ///             "source": null,
        ///             "metadata": null
        ///           },
        ///           "source_channel": "online"
        ///         }
        ///       ]
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>

        [HttpPost]
        [Route("SplitPayment")]  //// Working to transfer money
        public async Task<IActionResult> PaymentBaseSplitPayment(SplitPayment itemData)
        {
            string responseBody = "";
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            var transferData = new
            {
                transfers = new[]
                {
                    new { account = itemData.AccountId, amount = 10000, currency = "INR", on_hold = itemData.OnHold },
                    new { account = "acc_QfrQMLZB9pgQ7n", amount = 10000 , currency = "INR" , on_hold = itemData.OnHold }
            }
            };

            string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(transferData);

            using (var client = new HttpClient())
            {
                var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Common.RazorPayKey}:{Common.RazorPaySecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authString);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(Common.RazorpayPaymentTransfers.Replace("PAYMENT_ID", itemData.PaymentId), content);

                    if (response.IsSuccessStatusCode)
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        rm.statusCode = StatusCodes.OK;
                        rm.message = $"Transfer created successfully:";
                        rm.name = StatusName.ok;
                        rm.data = responseBody;
                    }
                    else
                    {
                        responseBody = await response.Content.ReadAsStringAsync();
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "Error creating transfer";
                        rm.name = StatusName.ok;
                        rm.data = responseBody;
                    }
                }
                catch (Exception ex)
                {
                    responseBody = ex.Message;
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = ex.Message.ToString();
                    rm.name = StatusName.invalid;
                    rm.data = responseBody;
                }
            }
            return Ok(rm);
        }

        /// <summary>
        /// Create Order and Get Payment Link
        /// </summary>
        /// <remarks>
        /// Sample Request JSON:
        /// 
        ///     {
        ///       "amountInPaise": 10000,
        ///       "receiptId": "receipt_001",
        ///       "description": "Test Payment",
        ///       "customerName": "John Doe",
        ///       "customerEmail": "johndoe786@gmail.com",
        ///       "customerContact": "9876543210",
        ///       "callbackUrl": "https://yourapp.com/razorpay/callback"
        ///     }
        /// Sample Response JSON:
        /// 
        ///     {
        ///       "orderId": "order_Qd95CmhS78RULQ",
        ///       "paymentLinkUrl": "https://rzp.io/l/abcd1234",
        ///       "status": "link_created"
        ///     }
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns Product Item against the VendorID </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// <returns></returns>

        [HttpPost("CreateOrderAndLink")]  //// Working Create and Generate Payment Link
        public IActionResult CreateOrderAndLink([FromBody] CreateOrderLinkRequest request)
        {
            try
            {

                var client = new RazorpayClient(Common.RazorPayKey, Common.RazorPaySecret);
                var orderOptions = new Dictionary<string, object>
                {
                    { "amount", request.AmountInPaise },
                    { "currency", "INR" },
                    { "receipt", request.ReceiptId },
                    { "payment_capture", 1 }
                };
                var order = client.Order.Create(orderOptions);

                var linkOptions = new Dictionary<string, object>
                {
                    { "amount", request.AmountInPaise },
                    { "currency", "INR" },
                    { "accept_partial", false },
                    { "description", request.Description },
                    { "reference_id", order["id"].ToString() },
                    { "customer", new Dictionary<string, object>
                        {
                            { "name", request.CustomerName },
                            { "email", request.CustomerEmail },
                            { "contact", request.CustomerContact }
                        }
                    },
                    { "notify", new Dictionary<string, object>
                        {
                            { "sms", true },
                            { "email", true }
                        }
                    },
                    { "reminder_enable", true },
                    { "callback_url", request.CallbackUrl },
                    { "callback_method", "get" }
                };
                var paymentLink = client.PaymentLink.Create(linkOptions);

                return Ok(new
                {
                    orderId = order["id"].ToString(),
                    paymentLinkUrl = paymentLink["short_url"].ToString(),
                    status = "link_created"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to create order and payment link", details = ex.Message });
            }
        }
        /// <summary>
        /// Get Order's Payment Status
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///       "razorpayPaymentId": "pay_QdAROHL12qtHHg"
        ///     }
        ///
        /// Sample Response:
        /// 
        ///     {
        ///       "id": "pay_QdAROHL12qtHHg",
        ///       "entity": "payment",
        ///       "amount": 10000,
        ///       "currency": "INR",
        ///       "status": "captured",
        ///       "order_id": "order_QdAQx7Y1JC1bY1",
        ///       "invoice_id": null,
        ///       "international": false,
        ///       "method": "card",
        ///       "amount_refunded": 0,
        ///       "refund_status": null,
        ///       "captured": true,
        ///       "description": "#QdAPhGUrUCjOds",
        ///       "card_id": "card_QdAROaJLI8oJe7",
        ///       "card": {
        ///         "id": "card_QdAROaJLI8oJe7",
        ///         "entity": "card",
        ///         "name": "",
        ///         "last4": "5449",
        ///         "network": "MasterCard",
        ///         "type": "credit",
        ///         "issuer": "UTIB",
        ///         "international": false,
        ///         "emi": false,
        ///         "sub_type": "consumer",
        ///         "token_iin": null
        ///       },
        ///       "bank": null,
        ///       "wallet": null,
        ///       "vpa": null,
        ///       "email": "void@razorpay.com",
        ///       "contact": "+919810722979",
        ///       "notes": [],
        ///       "fee": 200,
        ///       "tax": 0,
        ///       "error_code": null,
        ///       "error_description": null,
        ///       "error_source": null,
        ///       "error_step": null,
        ///       "error_reason": null,
        ///       "acquirer_data": {
        ///         "auth_code": "353731"
        ///       },
        ///       "created_at": 1749050687
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">The address removal request</param>
        /// <returns>Result of the operation</returns>
        /// <response code="200">Returns the success status</response>
        [HttpPost("GetPaymentStatus")]
        public IActionResult CapturePayment([FromBody] CapturePaymentRequest request)
        {
            try
            {
                var client = new RazorpayClient(Common.RazorPayKey, Common.RazorPaySecret);

                Payment payment = client.Payment.Fetch(request.RazorpayPaymentId);
                string jsonval = JsonConvert.SerializeObject(payment.Attributes);

                return Ok(jsonval);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to capture payment", details = ex.Message });
            }
        }
        /// <summary>
        /// Get Transaction Status
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///       "razorpayTransactionId": "trf_QdQuwIoM07WIFl"
        ///     }
        ///
        /// Sample Response:
        /// 
        ///     {
        ///       "Attributes": {
        ///         "id": "trf_QdQuwIoM07WIFl",
        ///         "entity": "transfer",
        ///         "status": "processed",
        ///         "source": "pay_QdAROHL12qtHHg",
        ///         "recipient": "acc_QBOuFMPEh3zBGm",
        ///         "amount": 900,
        ///         "currency": "INR",
        ///         "amount_reversed": 0,
        ///         "fees": 1,
        ///         "tax": 0,
        ///         "notes": [],
        ///         "linked_account_notes": [],
        ///         "on_hold": false,
        ///         "on_hold_until": null,
        ///         "settlement_status": "pending",
        ///         "recipient_settlement_id": null,
        ///         "created_at": 1749108710,
        ///         "processed_at": 1749108711,
        ///         "error": {
        ///           "code": null,
        ///           "description": null,
        ///           "reason": null,
        ///           "field": null,
        ///           "step": null,
        ///           "id": "trf_QdQuwIoM07WIFl",
        ///           "source": null,
        ///           "metadata": null
        ///         },
        ///         "source_channel": "online"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <param name="request">The address removal request</param>
        /// <returns>Result of the operation</returns>
        /// <response code="200">Returns the success status</response>
        [HttpPost("GetTransactionStatus")]
        public IActionResult GetTransactionStatus([FromBody] CaptureTransactionRequest request)
        {
            try
            {
                var client = new RazorpayClient(Common.RazorPayKey, Common.RazorPaySecret);

                var transfer = client.Transfer.Fetch(request.RazorpayTransactionId);
                string jsonval = JsonConvert.SerializeObject(transfer);// "pending", "processed", or "failed"

                return Ok(jsonval);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to capture payment", details = ex.Message });
            }
        }
        //[HttpPost]
        //[Route("RazorPay/Settlement/paymenttosubmerchant")]
        //[MapToApiVersion("1.0")]
        ////[Consumes("multipart/form-data")]
        //public async Task<IActionResult> paymenttosubmerchant(string PaymentID)
        //{
        //    var reqHeader = Request;
        //    string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    try
        //    {
        //        rm = new ResponseMessage();
        //        RazorpayClient client = new RazorpayClient(Common.RazorPayKey, Common.RazorPaySecret);

        //        Dictionary<string, object> transferRequest = new Dictionary<string, object>();
        //        List<Dictionary<string, object>> transfers = new List<Dictionary<string, object>>();
        //        Dictionary<string, object> transferParams = new Dictionary<string, object>();
        //        transferParams.Add("account", "acc_QBOuFMPEh3zBGm");
        //        transferParams.Add("amount", 100);
        //        transferParams.Add("currency", "INR");
        //        Dictionary<string, object> notes = new Dictionary<string, object>();
        //        notes.Add("name", "Gaurav Kumar");
        //        notes.Add("roll_no", "IEC2011025");
        //        transferParams.Add("notes", notes);
        //        List<string> linkedAccountNotes = new List<string>();
        //        linkedAccountNotes.Add("roll_no");
        //        transferParams.Add("linked_account_notes", linkedAccountNotes);
        //        transferParams.Add("on_hold", true);
        //        transfers.Add(transferParams);
        //        transferRequest.Add("transfers", transfers);

        //        List<Transfer> transfer = client.Payment.Fetch(PaymentID).Transfer(transferRequest);
        //        rm.statusCode = StatusCodes.OK;
        //        rm.message = "BULK UPLOAD HAS BEEN SUCCESSFULLY SAVED";
        //        rm.name = StatusName.ok;
        //        rm.data = "";

        //    }
        //    catch (Exception ex)
        //    {

        //        rm.statusCode = StatusCodes.ERROR;
        //        rm.message = ex.Message.ToString();
        //        rm.name = StatusName.invalid;
        //        rm.data = ex.Message.ToString();
        //        await Common.UpdateEventLogsNew("For Testing Different Functions", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
        //    }
        //    return Ok(rm);

        //}
    }
}
