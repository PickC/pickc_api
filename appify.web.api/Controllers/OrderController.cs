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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X509;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;
using static appify.models.NotificationType;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    [ApiVersion("1.0")]


    public class OrderController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IOrderBusiness orderBusiness;
        private readonly IInvoiceBusinesss invoiceBusinesss;
        private readonly IWebHostEnvironment env;

        private NotificationModel notificationModel;
        private ResponseMessage rm;
        private readonly INotificationBusiness notificationBusiness;
        public OrderController(IConfiguration configuration, 
                               IOrderBusiness orderBusiness, 
                               IInvoiceBusinesss invoiceBusinesss, 
                               IEventLogBusiness eventLogBusiness, 
                               INotificationBusiness IResultData,
                               IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.orderBusiness = orderBusiness;
            this.invoiceBusinesss = invoiceBusinesss;
            this.eventLogBusiness = eventLogBusiness;
            this.notificationBusiness = IResultData;
            this.env = env;

            ////FCM Objects
            notificationModel = new NotificationModel();
        }
        /// <summary>
        /// Add/Update an Order
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "orderID": 0,
        ///       "orderNo": "PO1473150202312150614",
        ///       "orderDate": "2024-04-08T08:09:13.710Z",
        ///       "vendorID": 1060,
        ///       "memberID": 1868,
        ///       "firstName": "sharma sudarshan",
        ///       "emailID": "sharma@appi-fy.ai",
        ///       "mobileNo": "9885217825",
        ///       "addressID": 1520,
        ///       "orderAmount": 2000,
        ///       "discountAmount": 15,
        ///       "taxAmount": 0,
        ///       "totalAmount": 0,
        ///       "currency": "INR",
        ///       "paidAmount": 2000,
        ///       "remarks": "string",
        ///       "receiverName": "string",
        ///       "receiverMobileNo": "string",
        ///       "deliveryInstruction": "string",
        ///       "deliveryCost": 0,
        ///       "paymentType": 3703,
        ///       "isSameState": true,
        ///       "deliveryChannel": 0,
        ///       "deliveryChannelDescription": "string",
        ///        "deviceToken": "e1JVr9HPR-SqjaFxf4Ggln:APA91bFdTYVFb5CA0Iqu8C3nfZm0v65rFJTQ0iCd9xXAAUQpQFLyW23uFpnbxXWnRQGYO0zJ5HuENs75-///G  5T piqdYExL4BbqfllMKY3waouaWkdEsSVIswpG31fJiThIHXTA4cESnlK5",
        ///       "items": [
        ///         {
        ///           "itemID": 1,
        ///           "orderID": 0,
        ///           "productID": 1217,
        ///           "sellerID": 1060,
        ///           "quantity": 2,
        ///           "unitPrice": 2000,
        ///           "discountType": 3001,
        ///           "discountAmount": 15,
        ///           "sellingPrice": 2000,
        ///           "priceID": 0
        ///         }
        ///       ]
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "order SUCCESSFUL!",
        ///       "data": {
        ///         "orderNo": "",
        ///         "accessKey": "Invalid merchant key.",
        ///         "orderID": 2016,
        ///         "errorMsg": null
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">ORDER HAS BEEN SUCCESSFULLY SAVED </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> Add(appify.models.Order order)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //// SMSNotification.SMSNotificationMessage();
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = this.orderBusiness.Save(order);
                if (result != null)
                {

                    PaymentTransactionData sampleData = new PaymentTransactionData();

                    sampleData.txnid = result.OrderID.ToString();
                    sampleData.phone = order.MobileNo;
                    sampleData.email = order.EmailID;
                    sampleData.amount = (float)order.TotalAmount;

                    var paymentData = InitiatePayment(sampleData);


                    appify.models.Invoice invoiceItem = new appify.models.Invoice();

                    invoiceItem.InvoiceID = 0;
                    invoiceItem.OrderID = result.OrderID;
                    invoiceItem.InvoiceNo = "";// string.Format("INV{0}{1}", order.VendorID.ToString(), DateTime.Now.ToString("ssyyyyyMMddHHmm"));
                    invoiceItem.MemberID = order.MemberID;
                    invoiceItem.SellerID = order.VendorID;
                    invoiceItem.InvoiceAmount = order.OrderAmount;
                    invoiceItem.items = new List<InvoiceDetail>();

                    foreach (var ordItem in order.items)
                    {

                        decimal gstValue = 0.00M;
                        decimal GSTPercent = 2.5M;
                        decimal originalPrice = 0.00M;
                        decimal SellingAmount = 0.00M;

                        ///// For Testing purpose : gurjeet at 12:25pm  30-05-2024
                        //if ((ordItem.UnitPrice * 100) / (100 + (GSTPercent * 2)) > 1050)
                        //{
                        //    GSTPercent = 6.0M;

                        //}

                        //originalPrice = Math.Round((ordItem.UnitPrice * 100) / (100 + (GSTPercent * 2)), 2, MidpointRounding.AwayFromZero);

                        ///// For Testing purpose : gurjeet at 12:25pm  30-05-2024
                        //originalPrice = Math.Round(((ordItem.UnitPrice * 100) / (100 + (GSTPercent * 2))), 2);

                        //gstValue = (ordItem.UnitPrice * (GSTPercent * 2)) / 100;
                        //gstValue = Math.Round(((originalPrice * (GSTPercent)) / 100), 2, MidpointRounding.AwayFromZero);

                        switch (ordItem.DiscountType)
                        {
                            case 3001:
                                ordItem.SellingPrice = ordItem.UnitPrice - ((ordItem.UnitPrice * ordItem.DiscountAmount) / 100);
                                break;
                            case 3000:
                                ordItem.SellingPrice = ordItem.UnitPrice - ordItem.DiscountAmount;
                                break;
                            default:
                                break;
                        }

                        if (ordItem.SellingPrice * 100 / (100 + (GSTPercent * 2)) > 1050)
                        {
                            GSTPercent = 6.0M;
                        }

                        originalPrice = Math.Round(((ordItem.SellingPrice * 100) / (100 + (GSTPercent * 2))), 2);

                        gstValue = Math.Round(((originalPrice * (GSTPercent)) / 100), 2);

                        InvoiceDetail dt = new InvoiceDetail();

                        dt.DiscountAmount = ordItem.DiscountAmount;
                        dt.DiscountType = ordItem.DiscountType;
                        dt.ProductID = ordItem.ProductID;
                        dt.Quantity = ordItem.Quantity;


                        dt.IGST = 0.00M;
                        dt.CGST = 0.00M;
                        dt.SGST = 0.00M;

                        if (order.IsSameState)
                        {
                            //dt.CGST = Math.Round((gstValue * ordItem.Quantity), 2, MidpointRounding.AwayFromZero);
                            //dt.SGST = Math.Round((gstValue * ordItem.Quantity), 2, MidpointRounding.AwayFromZero);
                            dt.CGST = Math.Round((gstValue * ordItem.Quantity), 2);
                            dt.SGST = Math.Round((gstValue * ordItem.Quantity), 2);

                        }
                        else
                        {
                            //dt.IGST = Math.Round((gstValue * 2 * ordItem.Quantity), 2, MidpointRounding.AwayFromZero);
                            dt.IGST = Math.Round((gstValue * 2 * ordItem.Quantity), 2);
                        }
                        //dt.TaxAmount = Math.Round(dt.SGST + dt.CGST + dt.IGST, 2, MidpointRounding.AwayFromZero);
                        //dt.UnitPrice = Math.Round(originalPrice, 2, MidpointRounding.AwayFromZero);
                        //dt.SellingAmount = Math.Round(((dt.UnitPrice * ordItem.Quantity)  + dt.TaxAmount), 2, MidpointRounding.AwayFromZero);

                        dt.TaxAmount = dt.SGST + dt.CGST + dt.IGST;

                        ///// For Testing purpose : gurjeet at 12:25pm  30-05-2024
                        //dt.UnitPrice = Math.Round(originalPrice);

                        dt.UnitPrice = ordItem.UnitPrice;
                        //dt.SellingPrice = ordItem.SellingPrice * ordItem.Quantity;
                        //dt.SellingAmount = Math.Round((ordItem.SellingPrice - dt.TaxAmount), 2);

                        dt.SellingPrice = ((ordItem.SellingPrice * ordItem.Quantity) - dt.TaxAmount);
                        dt.SellingAmount = ordItem.SellingPrice * ordItem.Quantity;

                        /*
                        switch (ordItem.DiscountType)
                        {
                            case 3001:
                                dt.SellingPrice = originalPrice * ordItem.Quantity;
                                dt.SellingAmount = Math.Round((originalPrice + dt.TaxAmount), 2);
                                break;
                            case 3000:
                                dt.SellingPrice = Math.Round((ordItem.SellingPrice + dt.TaxAmount), 2); 
                                dt.SellingAmount = ordItem.SellingPrice * ordItem.Quantity;
                                break;
                            default:
                                break;
                        }
                        */


                        invoiceItem.items.Add(dt);

                        //ordItem.UnitPrice = dt.UnitPrice;
                        //ordItem.SellingPrice = dt.SellingAmount;


                    }


                    decimal TotalSellingAmount = invoiceItem.items.Sum(dt => dt.SellingAmount);
                    decimal TotalTaxAmount = invoiceItem.items.Sum(dt => dt.TaxAmount);

                    invoiceItem.TaxAmount = TotalTaxAmount;
                    invoiceItem.TotalAmount = TotalSellingAmount;
                    invoiceItem.InvoiceAmount = TotalSellingAmount - TotalTaxAmount;

                    var invoiceStatus = invoiceBusinesss.Save(invoiceItem);

                    var data = new OrderPaymentData();

                    data.AccessKey = paymentData;
                    data.OrderNo = result.OrderNo;
                    data.OrderID = result.OrderID;

                    rm.statusCode = StatusCodes.OK;
                    rm.message = "order SUCCESSFUL!";
                    rm.name = StatusName.ok;
                    //rm.data = orderMaster;
                    rm.data = data;

                    //string firstName = order.FirstName.ToString();
                    //if (firstName.Length != 0)
                    //{
                    //    firstName = char.ToUpper(firstName[0]) + firstName.Substring(1);
                    //}

                    if (order.PaymentType == 3703) //// 3703 is COD (Cash on Delivery)
                    {
                        OrderPlace_PushNotification_Email(data.OrderID);
                    }

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER HAS BEEN SUCCESSFULLY SAVED", reqHeader, controllerURL, order, data, StatusName.ok));
                    await Common.UpdateEventLogsNew("ORDER HAS BEEN SUCCESSFULLY SAVED", reqHeader, controllerURL, order, data, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO ADD/UPDATE order";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER - UNSBLE/ADD ORDER", reqHeader, controllerURL, order, null, rm.message));
                    await Common.UpdateEventLogsNew("ORDER - UNSBLE/ADD ORDER", reqHeader, controllerURL, order, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER - ERROR", reqHeader, controllerURL, order, null, rm.message));
                await Common.UpdateEventLogsNew("ORDER - ERROR", reqHeader, controllerURL, order, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Remove the Order
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "orderID": 1003
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">order REMOVED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("remove")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> Remove(Int64 orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = orderBusiness.Delete(orderID);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "order REMOVED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Removed", reqHeader, controllerURL, orderID, result, StatusName.ok));
                    await Common.UpdateEventLogsNew("ORDER REMOVED SUCCESSFULLY", reqHeader, controllerURL, orderID, result, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DE-ACTIVATE order";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order - Unable to Removed", reqHeader, controllerURL, orderID, null, rm.message));
                    await Common.UpdateEventLogsNew("ORDER - UNABLE TO REMOVED", reqHeader, controllerURL, orderID, result, rm.message, this.eventLogBusiness);
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order - Remove - Error", reqHeader, controllerURL, orderID, null, rm.message));
                await Common.UpdateEventLogsNew("ORDER - REMOVED - ERROR", reqHeader, controllerURL, orderID, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// GENERATE AN ORDER INVOICE
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "orderID": 1976
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "INVOICE GENERATED",
        ///       "data": {
        ///         "invoiceNo": "TX10602409001",
        ///         "orderNo": "OD10602409001",
        ///         "orderDate": "2024-09-02T16:36:50.907",
        ///         "sellerID": 1060,
        ///         "memberID": 1864,
        ///         "invoiceAmount": 838.1,
        ///         "taxAmount": 41.9,
        ///         "totalAmount": 880,
        ///         "roundOffAmount": -0.11,
        ///         "companyName": "I AM BACK",
        ///         "companyAddress1": "B.29/27-K-1 SANKAT MOCHAN ROAD",
        ///         "companyAddress2": "VARANASI",
        ///         "companyState": "TELANGANA",
        ///         "companyZipCode": "505301",
        ///         "customerName": "RAMAKRISHNA",
        ///         "customerAddress1": null,
        ///         "customerAddress2": null,
        ///         "customerState": null,
        ///         "customerZipCode": null,
        ///         "deliveryCost": 83.11,
        ///         "deliveryGST": 0,
        ///         "deliveryGSTPercent": 0,
        ///         "sellerGSTIN": "",
        ///         "sellerPAN": "",
        ///         "memberGSTIN": "",
        ///         "memberPAN": "",
        ///         "paymentType": "CASH ON DELIVERY",
        ///         "paymentReference": "1976",
        ///         "receiverName": "",
        ///         "receiverMobileNo": "",
        ///         "subTotal": 963.11,
        ///         "grandTotal": 963,
        ///         "invoiceItems": [
        ///           {
        ///             "productID": "1064",
        ///             "quantity": 1,
        ///             "unitPrice": 880,
        ///             "taxAmount": 41.9,
        ///             "sellingPrice": 838.1,
        ///             "sellingAmount": 880,
        ///             "productName": "Men's Slim Fit Casual Shirts",
        ///             "description": "This Shirt comes in cotton fabric and is perfect for casual and formal wear.",
        ///             "brand": "I AM BACK ",
        ///             "discountAmount": 0,
        ///             "discountPrice": 880,
        ///             "cgst": 20.95,
        ///             "sgst": 20.95,
        ///             "igst": 0,
        ///             "cgstPercent": null,
        ///             "sgstPercent": null,
        ///             "igstPercent": null,
        ///             "hsnCode": null,
        ///             "discountTypeDescription": ""
        ///           },
        ///           {
        ///             "productID": "999999",
        ///             "quantity": 1,
        ///             "unitPrice": 70.43,
        ///             "taxAmount": 12.68,
        ///             "sellingPrice": 70.43,
        ///             "sellingAmount": 83.11,
        ///             "productName": "Shipping Charges",
        ///             "description": "Shipping Charges",
        ///             "brand": "",
        ///             "discountAmount": 0,
        ///             "discountPrice": 0,
        ///             "cgst": 6.34,
        ///             "sgst": 6.34,
        ///             "igst": 0,
        ///             "cgstPercent": null,
        ///             "sgstPercent": null,
        ///             "igstPercent": null,
        ///             "hsnCode": null,
        ///             "discountTypeDescription": ""
        ///           }
        ///         ]
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">ORDER INVOICE GENERATED SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("printinvoice")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult PrintInvoice(Int64 orderID)
        {
            ////OrderPlace_PushNotification_Email(orderID);

            //EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.SuccessfulSignupVendor), 1927, 0, this.notificationBusiness);

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = invoiceBusinesss.PrintInvoice(orderID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "INVOICE GENERATED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER INVOICE GENERATED SUCCESSFULLY", reqHeader, controllerURL, orderID, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO GENERATE INVOICE";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER NOT GENERATED", reqHeader, controllerURL, orderID, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER INVOICE ERROR", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// GENERATE AN VENDOR RECEIPT
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "vendorID": 1060
        ///     }
        /// </remarks>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="500"></response>
        [HttpPost, Route("printreceipt")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult PrintReceipt(Int64 VendorID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = invoiceBusinesss.PrintReceipt(VendorID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "RECEIPT GENERATED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR RECEIPT GENERATED SUCCESSFULLY", reqHeader, controllerURL, VendorID, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO GENERATE INVOICE";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR RECEIPT NOT GENERATED", reqHeader, controllerURL, VendorID, null, rm.message));
                }
            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("VENDOR RECEIPT ERROR", reqHeader, controllerURL, VendorID, null, rm.message));
            }
            return Ok(rm);
        }
    /// <summary>
    /// Update Order's Status
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "orderID": 1604,
    ///       "orderStatus": 3577,
    ///       "remarks": "Order has been Confirmed"
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "STATUS UPDATED SUCCESSFULLY!",
    ///       "data": "1604"
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">STATUS UPDATED SUCCESSFULLY </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("updatestatus")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public IActionResult UpdateOrderStatus(ParamOrderStatus statusData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //dynamic data = jsonData;
        try
        {
            rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            var result = orderBusiness.UpdateOrderStatus(statusData.OrderID, statusData.OrderStatus, statusData.Remarks);
            if (result)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "STATUS UPDATED SUCCESSFULLY!";
                rm.name = StatusName.ok;
                rm.data = statusData.OrderID.ToString();
                OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(statusData.OrderID);
                    if (orderUpdateDetail.SkipNo != orderUpdateDetail.VendorMobileNo && orderUpdateDetail.SkipNo != orderUpdateDetail.MemberMobileNo)
                    {
                        /////FCM Notification AND Email Notification
                        if (statusData.OrderStatus == 3587) //// Cancelled by Customer Cast 1 - We have 2 cases 1st is before confirmed the order
                        {
                            if (orderUpdateDetail.VendorID != 0)
                            {
                                if (orderUpdateDetail.IsEmail == true)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    if (orderUpdateDetail.IsEmailOpps == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                }
                                if (orderUpdateDetail.IsSMS == true)
                                {
                                    SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsPush == true)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                //// In App Notification
                                InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                            }
                            if (orderUpdateDetail.MemberID != 0)
                            {
                                if (orderUpdateDetail.IsEmail == true)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsSMS == true)
                                {
                                    SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsPush == true)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                //// In App Notification
                                InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                            }
                        }
                        if (statusData.OrderStatus == 3588) //// Declined by Vendor
                        {
                            if (orderUpdateDetail.VendorID != 0)
                            {
                                if (orderUpdateDetail.IsEmail == true)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    if (orderUpdateDetail.IsEmailOpps == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationVendorOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                }
                                if (orderUpdateDetail.IsSMS == true)
                                {
                                    SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsPush == true)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                //// In App Notification
                                InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderDeclinedByVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                            }
                            if (orderUpdateDetail.MemberID != 0)
                            {
                                if (orderUpdateDetail.IsEmail == true)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationVendorCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsSMS == true)
                                {
                                    SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsPush == true)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                //// In App Notification
                                InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                            }
                        }
                        if (statusData.OrderStatus == 3577) //// Order Confirmed by Vendor
                        {
                            if (orderUpdateDetail.VendorID != 0)
                            {
                                if (orderUpdateDetail.IsEmail == true)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderConfirmationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    if (orderUpdateDetail.IsEmailOpps == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderConfirmationOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                }
                                if (orderUpdateDetail.IsSMS == true)
                                {
                                    SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsPush == true)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                //// In App Notification
                                InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                            }
                            if (orderUpdateDetail.MemberID != 0)
                            {
                                if (orderUpdateDetail.IsEmail == true)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderConfirmationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsSMS == true)
                                {
                                    SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.IsPush == true)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                //// In App Notification
                                InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                            }
                        }

                    }
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER IS UPDATED", reqHeader, controllerURL, statusData, result, StatusName.ok));
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "UNABLE TO UPDATE ORDER STATUS";
                rm.name = StatusName.invalid;
                rm.data = result;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER NOT UPDATED", reqHeader, controllerURL, statusData, null, rm.message));
            }
        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER UPDATED ERROR", reqHeader, controllerURL, statusData, null, rm.message));
        }
        return Ok(rm);

    }

    /// <summary>
    /// Update PICKUP STATUS
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "orderID": 1604,
    ///       "weight": 250.00,
    ///       "length": 7.00,
    ///       "width": 5.00,
    ///       "height": 7.00
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PICKUP STATUS UPDATED SUCCESSFULLY!",
    ///       "data": "1604"
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">PICKUP STATUS UPDATED </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 

    [HttpPost, Route("updateorderforpickup")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult UpdateOrderForPickup(ParamOrderForPickup statusData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = orderBusiness.UpdateOrderPickup(statusData.OrderID, statusData.Weight, statusData.Length, statusData.Width, statusData.Height);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PICKUP STATUS UPDATED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = statusData.OrderID.ToString();
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER PICKUP STATUS UPDATED SUCCESSFULLY", reqHeader, controllerURL, statusData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER PICKUP STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO UPDATE ORDER PICKUP STATUS", reqHeader, controllerURL, statusData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER PICKUP STATUS ERROR", reqHeader, controllerURL, statusData, null, rm.message));
            }
            return Ok(rm);

        }

    /// <summary>
    /// Update PICKUP STATUS
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "orderID": 1275,
    ///       "courierRefID": "495983952",
    ///       "shipmentID": "494137262",
    ///       "awb": "339942452210"
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PICKUP STATUS UPDATED SUCCESSFULLY!",
    ///       "data": "1275"
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">PICKUP STATUS UPDATED </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 

    [HttpPost, Route("updateorderawb")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult UpdateOrderAWB(ParamOrderAWB statusData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = orderBusiness.UpdateOrderAWB(statusData.OrderID, statusData.CourierRefID, statusData.ShipmentID, statusData.AWB);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PICKUP STATUS UPDATED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = statusData.OrderID.ToString();
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER PICKUP STATUS UPDATED SUCCESSFULLY", reqHeader, controllerURL, statusData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER PICKUP STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO UPDATE ORDER PICKUP STATUS", reqHeader, controllerURL, statusData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER PICKUP STATUS NOT UPDATED", reqHeader, controllerURL, statusData, null, rm.message));
            }
            return Ok(rm);

        }

    /// <summary>
    /// GET ORDER TRACKING DETAILS
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "userID": 1005
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "ORDER TRACKING DETAILS FETCHED SUCCESSFULLY!",
    ///       "data": {
    ///         "orderID": 1005,
    ///         "courierRefID": "454503294",
    ///         "shipmentID": "452671447",
    ///         "awb": "1504848093812"
    ///       }
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">ORDER TRACKING DETAILS FETCHED </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 

    [HttpPost, Route("gettrackingdetails")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetOrderTrackingDetails(Int64 orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var result = orderBusiness.GetOrderTrackingDetails(orderID);
                if (result != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "ORDER TRACKING DETAILS FETCHED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = result;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER TRACKING DETAILS FETCHED SUCCESSFULLY", reqHeader, controllerURL, orderID, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO FETCH ORDER TRACKING STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = orderID;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("UNABLE TO FETCH ORDER TRACKING", reqHeader, controllerURL, orderID, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER TRACKING DETAILS FETCHED ERROR", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }
        /// <summary>
        /// Get an Order Item
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "orderID": 1005
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH order",
        ///       "data": {
        ///         "items": [
        ///           {
        ///             "itemID": 6,
        ///             "orderID": 0,
        ///             "productID": 0,
        ///             "sellerID": 0,
        ///             "quantity": 1,
        ///             "unitPrice": 599,
        ///             "discountType": 0,
        ///             "discountAmount": 0,
        ///             "sellingPrice": 599,
        ///             "isCancel": false,
        ///             "isDelivered": false,
        ///             "deliveryID": null,
        ///             "deliverDate": null,
        ///             "createdOn": null,
        ///             "modifiedOn": null,
        ///             "cancelBy": null,
        ///             "priceID": 0,
        ///             "size": "",
        ///             "price": null,
        ///             "weight": 0,
        ///             "productDescription": "Geometric Pattern Cotton Shirt Pack of 3",
        ///             "hsnCode": "",
        ///             "color": "Mixed colour ",
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1701860737832.png"
        ///           }
        ///         ],
        ///         "orderID": 1005,
        ///         "orderNo": "PO1473240202312161138",
        ///         "orderDate": "2023-12-16T17:08:23.247",
        ///         "vendorID": 1473,
        ///         "memberID": 1563,
        ///         "orderStatus": 3932,
        ///         "orderAmount": 599,
        ///         "discountAmount": 0,
        ///         "taxAmount": 29.95,
        ///         "totalAmount": 791.24,
        ///         "isCancel": false,
        ///         "isDelivered": false,
        ///         "remarks": "",
        ///         "deliveryInstruction": "",
        ///         "deliveryCost": 0,
        ///         "orderStatusDescription": "Order Placed",
        ///         "firstName": null,
        ///         "lastName": null,
        ///         "paymentType": 3703,
        ///         "paymentTypeDescription": "CASH ON DELIVERY",
        ///         "addressID": 1212,
        ///         "mobileNo": null,
        ///         "zipCode": "500081",
        ///         "address1": "Krishe Emerald",
        ///         "address2": "Sy. 11, Kondapur, Hi tech city",
        ///         "city": "Hyderabad",
        ///         "state": "Telangana",
        ///         "country": "In",
        ///         "landmark": "",
        ///         "alternateNo": "",
        ///         "productID": 1286,
        ///         "productDescription": "Geometric Pattern Cotton Shirt Pack of 3",
        ///         "deliveryChannel": 3921,
        ///         "deliveryChannelDescription": "SHIP ROCKET"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Get an Order </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 

        [HttpPost, Route("getitem")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult Getorder(long orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.orderBusiness.GetCustomerOrder(orderID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH order";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder IS SUCCESSFULLY", reqHeader, controllerURL, orderID, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder - NO CONTENT", reqHeader, controllerURL, orderID, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder - ERROR", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get an Order Item
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "orderID": 2274
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH order",
        ///       "data": {
        ///         "items": [
        ///           {
        ///             "itemID": 1654,
        ///             "quantity": 1,
        ///             "unitPrice": 880,
        ///             "sellingPrice": 880,
        ///             "priceID": 5657,
        ///             "size": "M",
        ///             "price": 880,
        ///             "weight": 250,
        ///             "productDescription": "Men's Slim Fit Shirt with Vertical Stripes ",
        ///             "hsnCode": "t56789",
        ///             "color": "White and Grey ",
        ///             "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1698825832157.jpg"
        ///           }
        ///         ],
        ///         "orderID": 2274,
        ///         "orderNo": "OD10602502001",
        ///         "orderDate": "2025-02-13T16:15:48.253",
        ///         "addressID": 1592,
        ///         "orderStatus": 3587,
        ///         "orderAmount": 880,
        ///         "discountAmount": 0,
        ///         "taxAmount": 0,
        ///         "totalAmount": 966.06,
        ///         "remarks": "",
        ///         "deliveryInstruction": "",
        ///         "deliveryCost": 86.06,
        ///         "firstName": "user",
        ///         "lastName": "appify",
        ///         "paymentType": 3703,
        ///         "deliveredOn": null,
        ///         "settlementStatus": 0,
        ///         "settlementDate": "2025-02-20T08:47:49.57",
        ///         "settlementAmount": 0,
        ///         "reason": "",
        ///         "deliveryChannel": 3922,
        ///         "deliveryChannelDescription": "DELHIVERY",
        ///         "shippingAddress": "sy11, we work, krishe emerald, Telangana, 500081",
        ///         "currentRemarks": "",
        ///         "currentDate": "2025-02-20T08:47:49.57"
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Get an Order </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 

        [HttpPost, Route("getitemnew")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetOrderNew(long orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.orderBusiness.GetCustomerOrderNew(orderID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH order";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder IS SUCCESSFULLY", reqHeader, controllerURL, orderID, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder - NO CONTENT", reqHeader, controllerURL, orderID, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetCustomerOrder - ERROR", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get Order For Delivery
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "orderID": 1005
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH order",
        ///       "data": {
        ///         "order_id": "PO1473240202312161138",
        ///         "order_date": "12/16/2023 5:08:23 PM",
        ///         "pickup_location": "SHOP_1702623311894",
        ///         "pickup_address": "NO -24/18, WEST STREET, MUGAIYUR - VILL, Anjalivan Stationery, Mugaiyur viluppuram Mugaiyur,Tamil Nadu,605757",
        ///         "channel_id": "Reseller: M/s Sooriya Textile",
        ///         "comment": "",
        ///         "billing_customer_name": null,
        ///         "billing_last_name": null,
        ///         "billing_address": "Krishe Emerald",
        ///         "billing_address_2": "Sy. 11, Kondapur, Hi tech city",
        ///         "billing_city": "Hyderabad",
        ///         "billing_pincode": "500081",
        ///         "billing_state": "Telangana",
        ///         "billing_country": "In",
        ///         "billing_email": null,
        ///         "billing_phone": null,
        ///         "shipping_is_billing": true,
        ///         "shipping_customer_name": null,
        ///         "shipping_last_name": null,
        ///         "shipping_address": "Krishe Emerald",
        ///         "shipping_address_2": "Sy. 11, Kondapur, Hi tech city",
        ///         "shipping_city": "Hyderabad",
        ///         "shipping_pincode": "500081",
        ///         "shipping_country": "In",
        ///         "shipping_state": "Telangana",
        ///         "shipping_email": null,
        ///         "shipping_phone": null,
        ///         "payment_method": "COD",
        ///         "shipping_charges": 0,
        ///         "giftwrap_charges": 0,
        ///         "transaction_charges": 0,
        ///         "total_discount": 0,
        ///         "sub_total": 791.24,
        ///         "length": 0,
        ///         "breadth": 0,
        ///         "height": 0,
        ///         "weight": 0,
        ///         "order_items": [
        ///           {
        ///             "name": "Geometric Pattern Cotton Shirt Pack of 3",
        ///             "sku": "1286",
        ///             "units": 1,
        ///             "selling_price": 599,
        ///             "discount": 0,
        ///             "tax": 0,
        ///             "hsn": ""
        ///           }
        ///         ]
        ///       }
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">GetOrderForDelivery IS SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("getorderpickup")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult Getorderfordelivery(long orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                var item = this.orderBusiness.GetOrderForDelivery(orderID);
                if (item != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH order";
                    rm.name = StatusName.ok;
                    rm.data = item;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetOrderForDelivery IS SUCCESSFULLY", reqHeader, controllerURL, orderID, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetOrderForDelivery - NO CONTENT", reqHeader, controllerURL, orderID, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("GetOrderForDelivery - ERROR", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }
    /// <summary>
    /// Get an Order List
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "userID": 1673
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "FETCH order LIST",
    ///       "data": [
    ///         {
    ///           "items": [
    ///             {
    ///               "itemID": 115,
    ///               "orderID": 0,
    ///               "productID": 0,
    ///               "sellerID": 0,
    ///               "quantity": 1,
    ///               "unitPrice": 1198,
    ///               "discountType": 0,
    ///               "discountAmount": 0,
    ///               "sellingPrice": 1198,
    ///               "isCancel": false,
    ///               "isDelivered": false,
    ///               "deliveryID": null,
    ///               "deliverDate": null,
    ///               "createdOn": null,
    ///               "modifiedOn": null,
    ///               "cancelBy": null,
    ///               "priceID": 0,
    ///               "size": "",
    ///               "price": null,
    ///               "weight": 0,
    ///               "productDescription": "Floral Men's Cotton Shirt ",
    ///               "hsnCode": "",
    ///               "color": "Blue",
    ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1700720218041.jpg"
    ///             }
    ///           ],
    ///           "orderID": 1113,
    ///           "orderNo": "PO1422270202401081551",
    ///           "orderDate": "2024-01-08T21:21:25.927",
    ///           "vendorID": 1422,
    ///           "memberID": 1673,
    ///           "orderStatus": 3932,
    ///           "orderAmount": 1198,
    ///           "discountAmount": 0,
    ///           "taxAmount": 0,
    ///           "totalAmount": 1381.48,
    ///           "isCancel": false,
    ///           "isDelivered": false,
    ///           "remarks": "",
    ///           "deliveryInstruction": "",
    ///           "deliveryCost": 0,
    ///           "orderStatusDescription": "Order Placed",
    ///           "firstName": null,
    ///           "lastName": null,
    ///           "paymentType": 3704,
    ///           "paymentTypeDescription": "ONLINE",
    ///           "addressID": 1340,
    ///           "mobileNo": null,
    ///           "zipCode": "560037",
    ///           "address1": "Subbaiah Reddy Colony",
    ///           "address2": "Marathahalli Village, Marathahalli",
    ///           "city": "Bangalore",
    ///           "state": "Karnataka",
    ///           "country": "In",
    ///           "landmark": "",
    ///           "alternateNo": "",
    ///           "productID": 1156,
    ///           "productDescription": "Floral Men's Cotton Shirt ",
    ///           "deliveryChannel": 3921,
    ///           "deliveryChannelDescription": "SHIP ROCKET"
    ///         }]}]
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">FETCH ORDER LIST SUCCESSFULLY </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> List(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<CustomerOrder> items = orderBusiness.List(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH order LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    ////this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("FETCH ORDER LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                    await Common.UpdateEventLogsNew("FETCH ORDER LIST - NO CONTENT", reqHeader, controllerURL, itemData, items, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH ORDER LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get an Order List
        /// </summary>
        /// <remarks> 
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH order LIST",
        ///       "data": [
        ///         {
        ///           "orderID": 1000,
        ///           "orderNo": "PO1473150202312150614",
        ///           "orderStatus": 3735,
        ///           "orderStatusDescription": null,
        ///           "productID": 1217,
        ///           "productDescription": "shirt",
        ///           "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1701443047420.jpg"
        ///         },
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH ORDER LIST SUCCESSFULLY </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("listall")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> ListAll(ParamMIDMType itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<OrderList> items = orderBusiness.OrderList(itemData.userID,itemData.userType);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH order LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    ////this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("FETCH ORDER LIST SUCCESSFULLY", reqHeader, controllerURL, null, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                    await Common.UpdateEventLogsNew("FETCH ORDER LIST - NO CONTENT", reqHeader, controllerURL, null, items, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH ORDER LIST - ERROR", reqHeader, controllerURL, null, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

        /// <summary>
        /// Get a Summarylist
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1864,
        ///       "orderStatus": "CURRENT",
        ///       "pageNo": 1,
        ///       "rows": 2
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH order LIST",
        ///       "data": [
        ///         {
        ///           "orderID": 2013,
        ///           "orderNo": "OD10602409038",
        ///           "orderDate": "2024-09-26T12:48:02.047",
        ///           "orderStatus": 3932,
        ///           "orderAmount": 729
        ///         },
        ///         {
        ///           "orderID": 2012,
        ///           "orderNo": "OD10602409037",
        ///           "orderDate": "2024-09-26T12:47:44.24",
        ///           "orderStatus": 3932,
        ///           "orderAmount": 599
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH order LIST </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("summarylist")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> SummaryList(ParamMemberOrder itemData)
    {
        //dynamic data = jsonData;
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<CustomerOrderSummary> items = orderBusiness.CustomerSummaryList(itemData.userID, itemData.OrderStatus, itemData.PageNo, itemData.Rows);
            if (items?.Any() == true)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "FETCH order LIST";
                rm.name = StatusName.ok;
                rm.data = items;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("CustomerSummaryList IS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                await Common.UpdateEventLogsNew("CustomerSummaryList IS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("CustomerSummaryList - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("CustomerSummaryList - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }


        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("CustomerSummaryList - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            await Common.UpdateEventLogsNew("CustomerSummaryList - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

    }
        /// <summary>
        /// Gets Daily Order Summarylist
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH daily order summary LIST",
        ///       "data": [
        ///         {
        ///           "orderID": 2013,
        ///           "orderNo": "OD10602409038",
        ///           "orderDate": "2024-09-26T12:48:02.047",
        ///           "vendorName":"High On Style",
        ///           "customerName":"Sri",
        ///           "mobileNo":"9840793066",
        ///           "emailID":"Balasri805@gmail.com",
        ///           "orderStatus":"Declined",
        ///           "orderAmount":790.00,
        ///           "paymentType":"CASH ON DELIVERY"
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH order LIST </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("dailyordersummarylist")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> DailyOrderSummaryList()
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<DailyOrderSummary> items = orderBusiness.GetDailyOrderSummary();
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH order LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("CustomerSummaryList IS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("DAILY ORDER SUMMARY LIST IS SUCCESSFULLY", reqHeader, controllerURL, "", items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("CustomerSummaryList - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                    await Common.UpdateEventLogsNew("DailyOrderSummaryList - NO CONTENT", reqHeader, controllerURL, "", null, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("CustomerSummaryList - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("DailyOrderSummaryList - ERROR", reqHeader, controllerURL, "", null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }
        /// <summary>
        /// Fetch Vendor Order List
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "userID": 1060,
        ///       "orderStatus": "CURRENT",
        ///       "pageNo": 1,
        ///       "rows": 2
        ///     }
        ///     
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "FETCH VENDOR ORDER LIST",
        ///       "data": [
        ///         {
        ///           "orderID": 1924,
        ///           "orderNo": "OD10602408019",
        ///           "orderDate": "2024-08-08T17:55:37.607",
        ///           "addressID": 1645,
        ///           "orderStatus": 3577,
        ///           "orderAmount": 880,
        ///           "discountAmount": 0,
        ///           "taxAmount": 0,
        ///           "totalAmount": 966.7,
        ///           "remarks": "",
        ///           "deliveryInstruction": "",
        ///           "deliveryCost": 86.7,
        ///           "firstName": "Ramakrishna",
        ///           "lastName": "Ganga",
        ///           "paymentType": 3703,
        ///           "deliveredOn": null,
        ///           "settlementStatus": "0",
        ///           "settlementDate": "2024-08-08T17:55:37.607",
        ///           "settlementAmount": 966.7,
        ///           "reason": "",
        ///           "deliveryChannel": 3922,
        ///           "deliveryChannelDescription": null,
        ///           "shippingAddress": "001, Kondapur main road, Laxmi Cyber city, Telangana, 500081",
        ///           "currentRemarks": "order has been confirmed. Pickup Initated",
        ///           "currentDate": "2024-08-08T18:07:01.717",
        ///           "items": [
        ///             {
        ///               "itemID": 1090,
        ///               "quantity": 1,
        ///               "unitPrice": 880,
        ///               "sellingPrice": 880,
        ///               "priceID": 5651,
        ///               "size": "L",
        ///               "price": 880,
        ///               "weight": 250,
        ///               "productDescription": "Men's Slim Fit Casual Shirts",
        ///               "hsnCode": "t5678",
        ///               "color": "Beige ",
        ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1698825443996.jpg"
        ///             }
        ///           ]
        ///         },
        ///         {
        ///         "orderID": 1839,
        ///           "orderNo": "OD10602407080",
        ///           "orderDate": "2024-07-16T18:47:59.19",
        ///           "addressID": 1645,
        ///           "orderStatus": 3932,
        ///           "orderAmount": 1200,
        ///           "discountAmount": 0,
        ///           "taxAmount": 0,
        ///           "totalAmount": 1368.1,
        ///           "remarks": "",
        ///           "deliveryInstruction": "",
        ///           "deliveryCost": 168.1,
        ///           "firstName": "bablu",
        ///           "lastName": "",
        ///           "paymentType": 3703,
        ///           "deliveredOn": null,
        ///           "settlementStatus": "0",
        ///           "settlementDate": "2024-07-16T18:47:59.19",
        ///           "settlementAmount": 1368.1,
        ///           "reason": "",
        ///           "deliveryChannel": 3921,
        ///           "deliveryChannelDescription": null,
        ///           "shippingAddress": "001, Kondapur main road, Laxmi Cyber city, Telangana, 500081",
        ///           "currentRemarks": "",
        ///           "currentDate": "2024-07-16T13:18:07.14",
        ///           "items": [
        ///             {
        ///             "itemID": 1005,
        ///               "quantity": 1,
        ///               "unitPrice": 1200,
        ///               "sellingPrice": 1200,
        ///               "priceID": 5772,
        ///               "size": "30",
        ///               "price": 1200,
        ///               "weight": 0,
        ///               "productDescription": "ankle fit jeans",
        ///               "hsnCode": "",
        ///               "color": "blue",
        ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1701830676092.jpg"
        ///             }
        ///           ]
        ///         }
        ///       ]
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">FETCH VENDOR ORDER LIST </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 

        [HttpPost, Route("vendororderlist")]
    [MapToApiVersion("1.0")]
    [Authorize]
    public async Task<IActionResult> ListByVendor(ParamMemberOrder itemData)
    {
        //dynamic data = jsonData;
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<VendorOrderNew> items = orderBusiness.ListByVendorNew(itemData.userID, itemData.OrderStatus, itemData.PageNo, itemData.Rows);
            if (items?.Any() == true)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "FETCH VENDOR ORDER LIST";
                rm.name = StatusName.ok;
                rm.data = items;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                ////this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, items, StatusName.ok));
                await Common.UpdateEventLogsNew("FETCH VENDOR ORDER LIST SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;
                //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                ////this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("FETCH VENDOR ORDER LIST - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }

        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            ////this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            await Common.UpdateEventLogsNew("FETCH VENDOR ORDER LIST - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

    }
    /// <summary>
    /// Get Vendor's Order Detail
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "vendorID": 1473,
    ///       "orderID":1005
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "FETCH VENDOR ORDER DETAIL",
    ///       "data": [
    ///         {
    ///           "orderID": 1005,
    ///           "orderNo": "PO1473240202312161138",
    ///           "orderDate": "2023-12-16T17:08:23.247",
    ///           "vendorID": 1473,
    ///           "memberID": 1563,
    ///           "addressID": 1201,
    ///           "orderStatus": 3932,
    ///           "orderAmount": 599,
    ///           "discountAmount": 0,
    ///           "taxAmount": 29.95,
    ///           "totalAmount": 791.24,
    ///           "isCancel": false,
    ///           "isDelivered": false,
    ///           "remarks": "",
    ///           "deliveryInstruction": "",
    ///           "deliveryCost": 0,
    ///           "orderStatusDescription": "Order Placed",
    ///           "firstName": null,
    ///           "lastName": null,
    ///           "paymentType": 3703,
    ///           "paymentTypeDescription": "CASH ON DELIVERY",
    ///           "deliveredOn": null,
    ///           "settlementStatus": "0",
    ///           "settlementDescription": "",
    ///           "settlementDate": "2023-12-16T17:08:23.247",
    ///           "settlementAmount": 791.24,
    ///           "reason": "",
    ///           "deliveryChannel": 3921,
    ///           "deliveryChannelDescription": "SHIP ROCKET",
    ///           "shippingAddress": "",
    ///           "currentRemarks": null,
    ///           "currentDate": "0001-01-01T00:00:00",
    ///           "items": [
    ///             {
    ///               "itemID": 6,
    ///               "orderID": 0,
    ///               "productID": 0,
    ///               "sellerID": 0,
    ///               "quantity": 1,
    ///               "unitPrice": 599,
    ///               "discountType": 0,
    ///               "discountAmount": 0,
    ///               "sellingPrice": 599,
    ///               "isCancel": false,
    ///               "isDelivered": false,
    ///               "deliveryID": null,
    ///               "deliverDate": null,
    ///               "createdOn": null,
    ///               "modifiedOn": null,
    ///               "cancelBy": null,
    ///               "priceID": 0,
    ///               "size": "",
    ///               "price": null,
    ///               "weight": 0,
    ///               "productDescription": "Geometric Pattern Cotton Shirt Pack of 3",
    ///               "hsnCode": "",
    ///               "color": "Mixed colour ",
    ///               "imageName": "https://appifystorage.blob.core.windows.net/appifystoragecontainer/image_cropper_1701860737832.png"
    ///             }
    ///           ]
    ///         }
    ///       ]
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">FETCH VENDOR ORDER DETAIL </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 
    [HttpPost, Route("vendororderdetail")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public IActionResult GetDetailByVendor(ParamVendorOrder itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                //CheckToken.IsValidToken(Request, configuration);
                TokenValidator.IsValidToken(Request, configuration, env);
                List<VendorOrder> items = orderBusiness.GetByVendorDetail(itemData.VendorID, itemData.OrderID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR ORDER DETAIL";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR ORDER DETAIL SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR ORDER DETAIL - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH VENDOR ORDER DETAIL - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }
        private void OrderPlace_PushNotification_Email(long OrderID)
        {
            try
            {
                this.orderBusiness.StockUpdate(OrderID, 3932);
                //// Order Placed By Customer COD & Online
                OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(OrderID);

                if (orderUpdateDetail.SkipNo != orderUpdateDetail.VendorMobileNo && orderUpdateDetail.SkipNo != orderUpdateDetail.MemberMobileNo)
                {

                    /////FCM Notification AND Email Notification
                    if (orderUpdateDetail.VendorID != 0) //// New Order Placement send Mail and notification to Vendor & Opps
                    {
                        if (orderUpdateDetail.IsEmail == true)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderPlacementVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            if (orderUpdateDetail.IsEmailOpps == true)
                            {
                                EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderPlacementOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            }
                        }
                        if (orderUpdateDetail.IsSMS == true)
                        {
                            SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                        }
                        if (orderUpdateDetail.IsPush == true)
                        {
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                        }
                        //// In App Notification.
                        InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                    }
                    if (orderUpdateDetail.MemberID != 0)//// New Order Placement send Mail and notification to Customer
                    {
                        if (orderUpdateDetail.IsEmail == true)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderPlacementCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                        }
                        if (orderUpdateDetail.IsSMS == true)
                        {
                            SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                        }
                        if (orderUpdateDetail.IsPush == true)
                        {
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                        }
                        //// In App Notification.
                        InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /* [HttpPost]
         [Route("PhonePayWebhookPaymentPaid")]
         public IActionResult ReceiveWebhook([FromBody] PhonePeWebhookPayload payload)////object payload
         {
             var reqHeader = Request;
             string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
             rm = new ResponseMessage();
             if (payload == null)
             {
                 this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "Received null payload", "Received null payload", StatusName.ok));
                 rm.statusCode = StatusCodes.ERROR;
                 rm.message = "Invalid payload";
                 rm.name = StatusName.invalid;
                 rm.data = null;
             }
             bool isValid = ValidatePayload(payload);
             if (!isValid)
             {
                 return BadRequest("Invalid payload");
             }
             this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "Received webhook", "Received webhook: {Payload}" + JsonConvert.SerializeObject(payload), StatusName.ok));

             rm.statusCode = StatusCodes.OK;
             rm.message = "RECEIVED WEBHOOK - PHONEPAY RESPONSE SUCCESSFULLY";
             rm.name = StatusName.ok;
             rm.data = payload;

             // Respond with a 200 OK status to acknowledge the receipt of the webhook
             return Ok(rm);
         }
         private bool ValidatePayload(PhonePeWebhookPayload payload)
         {
             // Implement validation logic as per your requirements
             // Example: Check if the transaction ID and status are not null
             if (string.IsNullOrEmpty(payload.data.merchantId) || string.IsNullOrEmpty(payload.data.merchantTransactionId))
             {
                 return false;
             }

             // Additional validation logic
             return true;
         }*/

        /// <summary>
        /// PhonePe WebHook for Order Paid.
        /// </summary>
        /// <remarks>
        /// Sample Response:
        /// NOTE : PhonePe WebHook for Order Paid.
        /// 
        ///     {
        ///         "success": true,
        ///         "code": "PAYMENT_INITIATED",
        ///         "message": "Payment Iniiated",
        ///         "data": {
        ///             "merchantId": "MERCHANTUAT",
        ///             "merchantTransactionId": "MT7850590068188104",
        ///             "instrumentResponse": {
        ///                 "type": "PAY_PAGE",
        ///                 "redirectInfo": {
        ///                     "url": "https://mercury-uat.phonepe.com/transact?///t    ok en=MjdkNmQ0NjM2MTk5ZTlmNDcxYjY3NTAxNTY5MDFhZDk2ZjFjMDY0YTRiN2VhMjgzNjIwMjBmNzUwN2JiNTkxOWUwNDVkMTM2YTllOTpkNzNkNmM2NWQ2MWNiZjVhM2MwOWMzODU0ZGEzMDczNA",
        ///                     "method": "GET"
        ///                 }
        ///             }
        ///         }
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">RECEIVED WEBHOOK - PHONEPAY RESPONSE SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost]
        [Route("PhonepeCallBack")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> PhonePeCallback()////object payload VerifyRequestModel verifyRequestModel
        {////string val = "{\r\n  \"response\": \"ewoJInN1Y2Nlc3MiOiB0cnVlLAoJImNvZGUiOiAiUEFZTUVOVF9TVUNDRVNTIiwKCSJkYXRhIjogewoJCSJ0cmFuc2FjdGlvbklkIjogImY2MjI0MjBmLTJmNTgtNGYyZS04MzJmIiwKCQkibWVyY2hhbnRJZCI6ICJNSURURVNUIiwKCQkiYW1vdW50IjogMTAwMCwKCQkicHJvdmlkZXJSZWZlcmVuY2VJZCI6ICJQMTkxMjE4MTIxMDM1NzQyMTc1Njc1NSIsCgkJInBheW1lbnRTdGF0ZSI6ICJDT01QTEVURUQiLAoJCSJwYXlSZXNwb25zZUNvZGUiOiAiU1VDQ0VTUyIKCX0KfQ==\"\r\n}";

            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            try { 
                    // Verify the X-VERIFY header.
                    string xVerifyHeader = reqHeader.Headers["X-VERIFY"];////verifyRequestModel.X_VERIFY;
                    if (xVerifyHeader == null || !VerifyXVerifyHeaderPhonepe(xVerifyHeader))
                    {
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RECEIVED WEBHOOK - PHONEPAY RESPONSE NULL", reqHeader, controllerURL, "PhonepeCallBack Received Null Payload", "Received null payload", StatusName.ok));
                    await Common.UpdateEventLogsNew("RECEIVED WEBHOOK - PHONEPAY RESPONSE NULL", reqHeader, controllerURL, "PhonepeCallBack Received Null Payload", "Received null payload", StatusName.ok, this.eventLogBusiness);
                    rm.statusCode = StatusCodes.ERROR;
                        rm.message = "Invalid payload";
                        rm.name = StatusName.invalid;
                        rm.data = null;
                    }
                    using var reader = new StreamReader(HttpContext.Request.Body);

                    // You now have the body string raw
                    var body = await reader.ReadToEndAsync();////verifyRequestModel.base64;

                    JObject json = JObject.Parse(body);
                    byte[] data = Convert.FromBase64String(json["response"].ToString());
                    string decodedString = System.Text.Encoding.UTF8.GetString(data);

                    // As well as a bound model
                    var request = JsonConvert.DeserializeObject(decodedString);

                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RECEIVED WEBHOOK - PHONEPAY RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "PhonepeCallBack Webhook Sucess Response", decodedString, StatusName.ok));
                await Common.UpdateEventLogsNew("RECEIVED WEBHOOK - PHONEPAY RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "PhonepeCallBack Webhook Sucess Response", decodedString, StatusName.ok, this.eventLogBusiness);
                rm.statusCode = StatusCodes.OK;
                    rm.message = "RECEIVED WEBHOOK - PHONEPAY RESPONSE SUCCESSFULLY";
                    rm.name = StatusName.ok;
                    rm.data = request;
                }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RECEIVED WEBHOOK - PHONEPAY ERROR", reqHeader, controllerURL, "PhonepeCallBack Webhook Error Response", null, rm.message));
                await Common.UpdateEventLogsNew("RECEIVED WEBHOOK - PHONEPAY ERROR", reqHeader, controllerURL, "PhonepeCallBack Webhook Error Response", null, rm.message, this.eventLogBusiness);
            }
            // Respond with a 200 OK status to acknowledge the receipt of the webhook
            return Ok(rm);
        }

        private bool VerifyXVerifyHeaderPhonepe(string xVerifyHeader)
        {
            // TODO: Implement the logic to verify the X-VERIFY header.
            return true;
        }

        private bool VerifyXVerifyHeadeRazorpay(string xVerifyHeader)
        {
            string Secret = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("RazorPayKey:Secret").Value;
            // TODO: Implement the logic to verify the X-VERIFY header.
            if (Secret == xVerifyHeader)
                return true;
            else
                return false;
        }

        /// <summary>
        /// RazorPay WebHook for PaymentEvents.
        /// </summary>
        /// <remarks>
        /// Sample Response:
        /// NOTE : RazorPay WebHook for PaymentEvents.
        /// 
        ///     {
        ///         "event": "payment.captured",
        ///         "payload": {
        ///             "payment": {
        ///                 "entity": {
        ///                     "id": "pay_29QQoUBi66xm2f",
        ///                     "amount": 5000,
        ///                     "currency": "INR",
        ///                     "status": "captured",
        ///                     "order_id": "order_DBJOWzybf0sJbb"
        ///                 }
        ///             }
        ///         }
        ///     }
        /// 
        /// 
        /// </remarks>
        /// <param name="payload"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">RECEIVED WEBHOOK - RAZORPAY RESPONSE SUCCESSFULLY</response>
        /// <response code="500">ResponseMessage with Error Description</response>

        [HttpPost]
        [Route("WebhookPaymentEvents")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> WebhookPaymentEvents()/////[FromBody] RazorpayWebhookPayload payload
        {
            var body = "";
            string paymentType = "";
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            bool eventResult = false;
            string[] eventSearch ={
              "downtime",
              ////"payment_link",
              "notification",
              "authorized",
              "order.paid"
            };

            try { 
            // Verify the X-VERIFY header.
            string xVerifyHeader = reqHeader.Headers["X-Razorpay-Signature"];////verifyRequestModel.X_VERIFY;
            ////xVerifyHeader = "Appyfy@1234$";
            if (xVerifyHeader == null)//// || !VerifyXVerifyHeadeRazorpay(xVerifyHeader)
                {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "Invalid payload";
                rm.name = StatusName.invalid;
                rm.data = null;
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RAZORPAY Received null payload", reqHeader, controllerURL, "RAZORPAY Received Null Payload X-Razorpay-Signature-" + xVerifyHeader, "RAZORPAY Received null payload", StatusName.ok));
                    await Common.UpdateEventLogsNew("RAZORPAY Received null payload", reqHeader, controllerURL, "RAZORPAY Received Null Payload X-Razorpay-Signature-" + xVerifyHeader, "RAZORPAY Received null payload", StatusName.ok, this.eventLogBusiness);
                }
            else
            {
                using var reader = new StreamReader(HttpContext.Request.Body);
                body = await reader.ReadToEndAsync();
                    //body = "{    \"account_id\": \"acc_OCjTrbZShxQc7y\",    \"contains\": [      \"refund\",      \"payment\"    ],    \"created_at\": 1729663607,    \"entity\": \"event\",    \"event\": \"refund.created\",    \"payload\": {      \"payment\": {        \"entity\": {          \"acquirer_data\": {            \"rrn\": \"428487107955\",            \"upi_transaction_id\": \"PTM5e6b325a3c1243c2a1745401cce7b542\"          },          \"amount\": 53950,          \"amount_refunded\": 53950,          \"amount_transferred\": 0,          \"bank\": null,          \"base_amount\": 53950,          \"captured\": true,          \"card_id\": null,          \"contact\": \"+918688647764\",          \"created_at\": 1728543101,          \"currency\": \"INR\",          \"description\": \"Order Payment\",          \"email\": \"g52976433@gmail.com\",          \"entity\": \"payment\",          \"error_code\": null,          \"error_description\": null,          \"error_reason\": null,          \"error_source\": null,          \"error_step\": null,          \"fee\": 1273,          \"id\": \"pay_P7F5reTtlujp0Z\",          \"international\": false,          \"invoice_id\": null,          \"method\": \"upi\",          \"notes\": {            \"device\": \"Android\",            \"orderId\": \"1614\"          },          \"order_id\": \"order_P7F5bB5kTbeiKE\",          \"provider\": null,          \"refund_status\": \"full\",          \"reward\": null,          \"status\": \"refunded\",          \"tax\": 194,          \"upi\": {            \"payer_account_type\": \"bank_account\",            \"vpa\": \"8688647764@ptaxis\"          },          \"vpa\": \"8688647764@ptaxis\",          \"wallet\": null        }      },      \"refund\": {        \"entity\": {          \"acquirer_data\": {            \"rrn\": null          },          \"amount\": 53950,          \"batch_id\": null,          \"created_at\": 1729663604,          \"currency\": \"INR\",          \"entity\": \"refund\",          \"id\": \"rfnd_PCNGwVoLSRV7aq\",          \"notes\": {            \"comment\": \"Refund of Order Id - OD17332410037, Vendor Name - Agu Chicha Fashion\"          },          \"payment_id\": \"pay_P7F5reTtlujp0Z\",          \"receipt\": null,          \"speed_processed\": \"normal\",          \"speed_requested\": \"normal\",          \"status\": \"processed\"        }      }    }  }";


                    ////body = "{\"entity\":\"event\",\"account_id\":\"acc_OCjTrbZShxQc7y\",\"event\":\"payment.captured\",\"contains\":[\"payment\"],\"payload\":{\"payment\":{\"entity\":{\"id\":\"pay_QIQkcRDQUWixSk\",\"entity\":\"payment\",\"amount\":149900,\"currency\":\"INR\",\"status\":\"captured\",\"order_id\":\"order_QIQkNFicMOTDBh\",\"invoice_id\":null,\"international\":false,\"method\":\"upi\",\"amount_refunded\":0,\"refund_status\":null,\"captured\":true,\"description\":\"subscription_payment\",\"card_id\":null,\"bank\":null,\"wallet\":null,\"vpa\":\"success@razorpay\",\"email\":\"rama@appi-fy.ai\",\"contact\":\"+916281438226\",\"notes\":{\"orderId\":\"1744522966806\",\"vendorId\":\"2205\",\"device\":\"Android\",\"paymentType\":\"oneTimeSubscription\"},\"fee\":3538,\"tax\":540,\"error_code\":null,\"error_description\":null,\"error_source\":null,\"error_step\":null,\"error_reason\":null,\"acquirer_data\":{\"rrn\":\"371595461222\",\"upi_transaction_id\":\"033E6336C4B4E9F17059AB28B1CAAF1A\"},\"created_at\":1744522982,\"reward\":null,\"upi\":{\"vpa\":\"success@razorpay\"},\"base_amount\":149900}}},\"created_at\":1744522983}";


                    var request = JsonConvert.DeserializeObject<JObject>(body.Replace("Response: ",""));
                string eventname = System.String.IsNullOrEmpty((string?)request["event"]) ? "" : Convert.ToString(request["event"]);
                foreach (var s in eventSearch)
                {
                    eventResult = eventname.Contains(s);
                    if (eventResult == true)
                        break;
                }
                if(eventResult==false)
                {
                    paymentType = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["notes"]["paymentType"]) ? "" : Convert.ToString(request["payload"]["payment"]["entity"]["notes"]["paymentType"]);

                    if(paymentType== "orderPayment")
                    {
                        long ts = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["created_at"]) ? 0 : Convert.ToInt64(request["payload"]["payment"]["entity"]["created_at"]);

                        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(ts).ToLocalTime();
                        OrderPayment orderPayment = new OrderPayment
                        {
                            PaymentID = 0,
                            PaymentDate = dt,
                            OrderID = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["notes"]["orderId"]) ? 0 : Convert.ToInt64(request["payload"]["payment"]["entity"]["notes"]["orderId"]),
                            EventName = Convert.ToString(request["event"]),
                            PaymentAmount = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["amount"]) ? 0 : Convert.ToDecimal(request["payload"]["payment"]["entity"]["amount"]) / 100,
                            OrderReferenceNo = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["order_id"]) ? "" : Convert.ToString(request["payload"]["payment"]["entity"]["order_id"]),
                            PaymentReferenceNo = System.String.IsNullOrEmpty((string?)request["payload"]["payment"]["entity"]["id"]) ? "" : Convert.ToString(request["payload"]["payment"]["entity"]["id"]),
                            PaymentMode = 0,
                            LookupCode = "RAZORPAY"
                        };
                        var result = orderBusiness.OrderPaymentSave(orderPayment);
                        if (result)
                        {
                            rm.statusCode = StatusCodes.OK;
                            rm.message = "RECEIVED WEBHOOK - RAZORPAY RESPONSE SUCCESSFULLY";
                            rm.name = StatusName.ok;
                            rm.data = request;
                            if (orderPayment.EventName == "payment.captured")
                            {
                                OrderPlace_PushNotification_Email(orderPayment.OrderID);
                            }
                            //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RECEIVED WEBHOOK - RAZORPAY RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "RAZORPAY Webhook - Success Response", request, StatusName.ok));
                            await Common.UpdateEventLogsNew("RECEIVED WEBHOOK - RAZORPAY RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "RAZORPAY Webhook - Success Response", request, StatusName.ok, this.eventLogBusiness);
                        }
                    }
                    else if (paymentType == "oneTimeSubscription")
                    {

                    }
                 }
                else
                {
                        //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RAZORPAY Webhook", reqHeader, controllerURL, "RAZORPAY Webhook - " + request["event"].ToString(), request, StatusName.ok));
                        //await Common.UpdateEventLogsNew("RAZORPAY Webhook", reqHeader, controllerURL, "RAZORPAY Webhook - " + request["event"].ToString(), request, StatusName.ok, this.eventLogBusiness);
                }

              }

            }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("RAZORPAY Webhook Error Response", reqHeader, controllerURL, "RAZORPAY Webhook Error Response", null, rm.message));
                await Common.UpdateEventLogsNew("RAZORPAY Webhook Error Response", reqHeader, controllerURL, "RAZORPAY Webhook Error Response->"+ body, null, rm.message, this.eventLogBusiness);
            }
            // Respond with a 200 OK status to acknowledge the receipt of the webhook
            return Ok(rm);
        }

    /// <summary>
    /// ShipRocket WebHook for DeliveryEvents.
    /// </summary>
    /// <remarks>
    /// Sample Response:
    /// NOTE : ShipRocket WebHook for DeliveryEvents.
    /// 
    ///     {
    ///        "awb":"19041424751540",
    ///        "courier_name":"Delhivery Surface",
    ///        "current_status":"IN TRANSIT",
    ///        "current_status_id":20,
    ///        "shipment_status":"IN TRANSIT",
    ///        "shipment_status_id":18,
    ///        "current_timestamp":"23 05 2023 11:43:52",
    ///        "order_id":"1373900_150876814",
    ///        "sr_order_id":348456385,
    ///        "awb_assigned_date":"2023-05-19 11:59:16",
    ///        "pickup_scheduled_date":"2023-05-19 11:59:17",
    ///        "etd":"2023-05-23 15:40:19",
    ///        "scans":[
    ///           {
    ///              "date":"2023-05-19 11:59:16",
    ///              "status":"X-UCI",
    ///              "activity":"Manifested - Manifest uploaded",
    ///              "location":"Chomu_SamodRd_D (Rajasthan)",
    ///              "sr-status":"5",
    ///              "sr-status-label":"MANIFEST GENERATED"
    ///           }
    ///           ]
    ///     }
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">SHIPROCKET WEBHOOK - SHIPROCKET RESPONSE SUCCESSFULLY</response>
    /// <response code="500">ResponseMessage with Error Description</response>

    [HttpPost]
    [Route("WebhookShipRocket")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> WebhookShipRocket()
    {
        var body = "";
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        rm = new ResponseMessage();
        try { 
                // Verify the X-VERIFY header.
                string xVerifyHeader = reqHeader.Headers["x-api-key"];////verifyRequestModel.X_VERIFY;

                //xVerifyHeader = "Appify@1234#";
            if (xVerifyHeader == null || xVerifyHeader == "")//// || !VerifyXVerifyHeaderShipRocket(xVerifyHeader)
            {
                await Common.UpdateEventLogsNew("SHIPROCKET Webhook Null Payload", reqHeader, controllerURL, "SHIPROCKET Webhook Null Payload", "Received null payload", StatusName.ok, this.eventLogBusiness);
                rm.statusCode = StatusCodes.ERROR;
                    rm.message = "Invalid payload";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                }
                else
                {
                    using var reader = new StreamReader(HttpContext.Request.Body);
                    // You now have the body string raw
                    body = await reader.ReadToEndAsync();

                    var requestObj = (JObject)JsonConvert.DeserializeObject(body);

                    OrderTrackingUpdate orderTrackingUpdate = new OrderTrackingUpdate
                    {
                        OrderNo = System.String.IsNullOrEmpty((string?)(JValue)requestObj["order_id"]) ? "" : Convert.ToString((JValue)requestObj["order_id"]),
                        OrderStatus = Convert.ToInt16((JValue)requestObj["current_status_id"]),
                        Remarks = System.String.IsNullOrEmpty((string?)(JValue)requestObj["current_status"]) ? "" : Convert.ToString((JValue)requestObj["current_status"]),
                        CourierRefID = System.String.IsNullOrEmpty((string?)(JValue)requestObj["channel_id"]) ? "" : Convert.ToString((JValue)requestObj["channel_id"]),
                        ShipmentID = "",////Convert.ToString((JValue)trackingObj["tracking_data"]["shipment_track"]["shipment_id"]),
                        AWB = System.String.IsNullOrEmpty((string?)(JValue)requestObj["awb"]) ? "" : Convert.ToString((JValue)requestObj["awb"]),
                        DeliveredOn = Convert.ToDateTime((JValue)requestObj["etd"]),
                        CourierName = System.String.IsNullOrEmpty((string?)(JValue)requestObj["courier_name"]) ? "" : Convert.ToString((JValue)requestObj["courier_name"]),
                        TrackURL = Common.ShiproketDeliveryTrackingURL + (System.String.IsNullOrEmpty((string?)(JValue)requestObj["awb"]) ? "" : (JValue)requestObj["awb"].ToString())
                    };

                    var result = orderBusiness.UpdateOrderTrackingStatus(orderTrackingUpdate);
                    if (result>0)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "SHIPROCKET WEBHOOK - SHIPROCKET RESPONSE SUCCESSFULLY";
                        rm.name = StatusName.ok;
                        rm.data = requestObj;
                        /*
                        Shiprocket Order Status Table

                        ->	3	Ready To Ship
                        ->	4	Pickup Scheduled
                        ->	5	Canceled
                        ->	6	Shipped
                        ->	7	Delivered
                        ->	8	ePayment Failed
                        ->	9	Returned
                        ->	19	Out for Delivery
                        ->	20	In Transit
                        ->	34	Out For Pickup
                        ->	51	Picked Up
                        ->  37  Delivery Delayed
                        */
                        OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(result);

                        if (orderUpdateDetail.SkipNo != orderUpdateDetail.VendorMobileNo && orderUpdateDetail.SkipNo != orderUpdateDetail.MemberMobileNo)
                        {
                            if (orderTrackingUpdate.OrderStatus == 7) //// Delivered
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderDeliveredVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderDeliveredCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            else if (orderTrackingUpdate.OrderStatus == 6) //// Shipped
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderShippedVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);

                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderShippedCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            else if (orderTrackingUpdate.OrderStatus == 19) //// In Transit
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderOutForDelivery), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderOutForDelivery), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            else if (orderTrackingUpdate.OrderStatus == 37) //// Delivery Delayed
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    /*if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderDelayVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }*/
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    /*if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderDelayCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }*/
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            if (orderTrackingUpdate.OrderStatus == 5) //// Cancelled by Customer
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                        }
                    }
                await Common.UpdateEventLogsNew("SHIPROCKET WEBHOOK - SHIPROCKET RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "SHIPROCKET Webhook Sucess Response", requestObj, StatusName.ok, this.eventLogBusiness);
            }

        }
        catch (Exception ex)
        {
            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            await Common.UpdateEventLogsNew("SHIPROCKET Webhook Error Received", reqHeader, controllerURL, "SHIPROCKET Webhook Error Received->" + body, null, rm.message, this.eventLogBusiness);
        }
        // Respond with a 200 OK status to acknowledge the receipt of the webhook
        return Ok(rm);
    }

    private bool VerifyXVerifyHeaderShipRocket(string xVerifyHeader)
    {
        string Secret = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ShipRocketKey:Secret").Value;
        // TODO: Implement the logic to verify the X-VERIFY header.
        if (Secret == xVerifyHeader)
            return true;
        else
            return false;
    }

    /// <summary>
    /// OneDelhivery WebHook for DeliveryEvents.
    /// </summary>
    /// <remarks>
    /// Sample Response:
    /// NOTE : OneDelhivery WebHook for DeliveryEvents.
    /// 
    ///     {
    ///        "awb":"19041424751540",
    ///        "courier_name":"Delhivery Surface",
    ///        "current_status":"IN TRANSIT",
    ///        "current_status_id":20,
    ///        "shipment_status":"IN TRANSIT",
    ///        "shipment_status_id":18,
    ///        "current_timestamp":"23 05 2023 11:43:52",
    ///        "order_id":"1373900_150876814",
    ///        "sr_order_id":348456385,
    ///        "awb_assigned_date":"2023-05-19 11:59:16",
    ///        "pickup_scheduled_date":"2023-05-19 11:59:17",
    ///        "etd":"2023-05-23 15:40:19",
    ///        "scans":[
    ///           {
    ///              "date":"2023-05-19 11:59:16",
    ///              "status":"X-UCI",
    ///              "activity":"Manifested - Manifest uploaded",
    ///              "location":"Chomu_SamodRd_D (Rajasthan)",
    ///              "sr-status":"5",
    ///              "sr-status-label":"MANIFEST GENERATED"
    ///           }
    ///           ]
    ///     }
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">ONEDELHIVERY WEBHOOK - ONEDELHIVERY RESPONSE SUCCESSFULLY</response>
    /// <response code="500">ResponseMessage with Error Description</response>

    [HttpPost]
    [Route("WebhookOneDelhivery")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> WebhookOneDelhivery()
    {
        var body = "";
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        rm = new ResponseMessage();
        try { 
        // Verify the X-VERIFY header.
        string xVerifyHeader = reqHeader.Headers["x-api-key"];
        if (xVerifyHeader == null || xVerifyHeader == "")
        {
            await Common.UpdateEventLogsNew("ONEDELHIVERY Webhook Received Null Payload", reqHeader, controllerURL, "ONEDELHIVERY Webhook Received Null Payload", "Received null payload", StatusName.invalid, this.eventLogBusiness);
            rm.statusCode = StatusCodes.ERROR;
            rm.message = "Invalid payload";
            rm.name = StatusName.invalid;
            rm.data = null;
        }
        else//// if (xVerifyHeader == "Appify@1234#")
        {
            using var reader = new StreamReader(HttpContext.Request.Body);
                // You now have the body string raw
            body = await reader.ReadToEndAsync();
            // As well as a bound model
            //var request = JsonConvert.DeserializeObject(body);
            var requestObj = (JObject)JsonConvert.DeserializeObject(body);

            OrderTrackingUpdateDelhivery orderTrackingUpdate = new OrderTrackingUpdateDelhivery
            {
                AWB = System.String.IsNullOrEmpty((string?)requestObj["Shipment"]["AWB"]) ? "" : Convert.ToString((JValue)requestObj["Shipment"]["AWB"]),
                Status = System.String.IsNullOrEmpty((string?)requestObj["Shipment"]["Status"]["Status"]) ? "" : Convert.ToString((JValue)requestObj["Shipment"]["Status"]["Status"]),

                StatusType = System.String.IsNullOrEmpty((string?)requestObj["Shipment"]["Status"]["StatusType"]) ? "" : Convert.ToString((JValue)requestObj["Shipment"]["Status"]["StatusType"]),
                Instructions = System.String.IsNullOrEmpty((string?)requestObj["Shipment"]["Status"]["Instructions"]) ? "" : Convert.ToString((JValue)requestObj["Shipment"]["Status"]["Instructions"]),
                ReferenceNo = System.String.IsNullOrEmpty((string?)requestObj["Shipment"]["ReferenceNo"]) ? "" : Convert.ToString((JValue)requestObj["Shipment"]["ReferenceNo"]),
                StatusDateTime = System.String.IsNullOrEmpty((string?)requestObj["Shipment"]["Status"]["StatusDateTime"]) ? Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")) : Convert.ToDateTime((JValue)requestObj["Shipment"]["Status"]["StatusDateTime"])
            };
                    if (orderTrackingUpdate.Status == "In Transit" && orderTrackingUpdate.StatusType == "UD" && orderTrackingUpdate.Instructions == "Shipment picked up")
                    {
                        var expectedDeliveryDate = await Common.GetExpectedDeliveryDateAsync(orderTrackingUpdate.AWB);
                        orderTrackingUpdate.ExpectedDeliveryDate = System.String.IsNullOrEmpty((string?)expectedDeliveryDate) ? null : Convert.ToDateTime(expectedDeliveryDate);
                    }
                    var result = orderBusiness.UpdateDelhiveryOrderTrackingStatus(orderTrackingUpdate);
                    if (result > 0)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "ONEDELHIVERY WEBHOOK - ONEDELHIVERY RESPONSE SUCCESSFULLY";
                        rm.name = StatusName.ok;
                        rm.data = requestObj;
                        OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(result);
                        if (orderUpdateDetail.SkipNo != orderUpdateDetail.VendorMobileNo && orderUpdateDetail.SkipNo != orderUpdateDetail.MemberMobileNo)
                        {
                            if (orderTrackingUpdate.Status == "RTO" && orderTrackingUpdate.StatusType == "DL") //// Cancelled by Customer
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            if (orderTrackingUpdate.Status == "Delivered" && orderTrackingUpdate.StatusType == "DL") //// Delivered 
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderDeliveredVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderDeliveredCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            if (orderTrackingUpdate.Status == "In Transit" && orderTrackingUpdate.StatusType == "UD" && orderTrackingUpdate.Instructions== "Shipment picked up") //// Shipped
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderShippedVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderShippedCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            if (orderTrackingUpdate.Status == "Dispatched" && orderTrackingUpdate.StatusType == "UD" && orderTrackingUpdate.Instructions == "Out for delivery") //// Out for Delivery
                            {
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    if (orderUpdateDetail.IsEmail == true)
                                    {
                                        EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderOutForDelivery), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsSMS == true)
                                    {
                                        SMSNotification.SendSMSNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    if (orderUpdateDetail.IsPush == true)
                                    {
                                        PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    }
                                    //// In App Notification
                                    InAppNotification.SendInAppNotification(Convert.ToInt64(PushNotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                            //if (orderTrackingUpdate.Status == "Dispatched" && orderTrackingUpdate.StatusType == "RT") //// Dispatched for RTO
                            //{

                            //}
                        }
                    }
                    //// Need to comment
                    await Common.UpdateEventLogsNew("RECEIVED WEBHOOK - ONEDELHIVERY RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "ONEDELHIVERY Webhook Sucess Response", requestObj, StatusName.ok, this.eventLogBusiness);
                rm.statusCode = StatusCodes.OK;
                rm.message = "RECEIVED WEBHOOK - ONEDELHIVERY RESPONSE SUCCESSFULLY";
                rm.name = StatusName.ok;
                ////rm.data = request;
            }

    }
        catch (Exception ex)
        {
            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "ONEDELHIVERY Webhook Error", null, rm.message));
            await Common.UpdateEventLogsNew("ONEDELHIVERY Webhook Error", reqHeader, controllerURL, "ONEDELHIVERY Webhook Error->" + body, null, rm.message, this.eventLogBusiness);
        }
        // Respond with a 200 OK status to acknowledge the receipt of the webhook
        return Ok(rm);
    }

        /// <summary>
        /// Server Downtime Alert
        /// </summary>
        /// <remarks>
        /// Sample request JSON :
        /// 
        ///     {
        ///       "Service": "Delivery",
        ///       "MemberID": 0,
        ///       "MemberType": 0,
        ///       "OrderID": 0,
        ///       "AppVersion":"10.1",
        ///       "AppName":"Beard Bro"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">RAZORPAY ORDER HAS BEEN SUCCESSFULLY SAVED </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("sendalert")]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> sendalert(ParamDownTimeAlert itemData)
        {
            var reqHeader = Request;
            var items = false;
            rm = new ResponseMessage();
            //CheckToken.IsValidToken(Request, configuration);
            TokenValidator.IsValidToken(Request, configuration, env);
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                List<EmailConfig> serverDownAlert = orderBusiness.GetAlertHeader();
                var ToOpps = serverDownAlert.Where(x => x.SettingKey== "ALERTTOOPPS").FirstOrDefault().SettingValue.ToString();
                var ToTech = serverDownAlert.Where(x => x.SettingKey == "ALERTTOTECH").FirstOrDefault().SettingValue.ToString();
                var IsActiveOpps = Convert.ToBoolean(serverDownAlert.Where(x => x.SettingKey == "IsActiveOpps").FirstOrDefault().SettingValue.ToString());
                var IsActiveTech = Convert.ToBoolean(serverDownAlert.Where(x => x.SettingKey == "IsActiveTech").FirstOrDefault().SettingValue.ToString());
                if(IsActiveOpps!=false)
                {
                    items = EmailNotification.SendEmailAlert(itemData, Convert.ToInt64(NotificationTemplateType.ServerDownAlert), ToOpps, notificationBusiness);
                }
                if (IsActiveTech != false)
                {
                    items = EmailNotification.SendEmailAlert(itemData, Convert.ToInt64(NotificationTemplateType.ServerDownAlert), ToTech, notificationBusiness);
                }
                if (items != false)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "SERVER DOWNTIME ALERT HAS BEEN SUCCESSFULLY SENT";
                    rm.name = StatusName.ok;
                    rm.data = items;

                    await Common.UpdateEventLogsNew("SERVER DOWNTIME ALERT HAS BEEN SUCCESSFULLY SENT", reqHeader, controllerURL, itemData, items, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    await Common.UpdateEventLogsNew("SERVER DOWNTIME - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("Server downtime alert - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }


        /// <summary>
        /// PAYMENT GATEWAY VALIDATION
        /// </summary>
        /// <remarks>    
        /// Sample response JSON :
        /// 
        ///     {
        ///       "statusCode": 200,
        ///       "name": "SUCCESS_OK",
        ///       "message": "PAYMENT GATEWAY KEY",
        ///       "data": "{\"status\":false,\"msg\":\"Transaction not found.\"}"
        ///     }
        /// 
        /// </remarks>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">PAYMENT GATEWAY VALIDATION </response>
        /// <response code="500">ResponseMessage with Error Description</response> 
        /// 
        [HttpPost, Route("TestPayment")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> InitiatePaymentAsync()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            var objEZ = new Easebuzz(PaymentGatewayConfig.PAYMENTGATEWAY_SALT,
                                     PaymentGatewayConfig.PAYMENTGATEWAY_KEY,
                                     PaymentGatewayConfig.PAYMENTGATEWAY_ENV,
                                     PaymentGatewayConfig.PAYMENTGATEWAY_SEAMLESS);

            PaymentTransactionData sampleData = new PaymentTransactionData();



            rm = new ResponseMessage();

            var paymentDict = new Dictionary<string, string>
        {

            { "key", PaymentGatewayConfig.PAYMENTGATEWAY_KEY}, // tesing kit key
            { "amount", sampleData.amount.ToString() },
            { "firstname", sampleData.firstname },
            { "email", sampleData.email },
            { "phone", sampleData.phone },
            { "productinfo", sampleData.productinfo},
            { "surl", PaymentGatewayConfig.PAYMENTGATEWAY_SUCCESSURL },
            { "furl", PaymentGatewayConfig.PAYMENTGATEWAY_ERRORURL },
            { "txnid", sampleData.txnid },
            { "udf1", sampleData.udf1 },
            { "udf2", sampleData.udf2},
            { "udf3", sampleData.udf3},
            { "udf4", sampleData.udf4},
            { "udf5", sampleData.udf5},
            { "udf6", sampleData.udf6},
            { "udf7", sampleData.udf7},
            { "udf8", sampleData.udf8},
            { "udf9", sampleData.udf9},
            { "udf10",sampleData.udf10 }
        };


            var strResult = objEZ.initiatePaymentAPI(paymentDict);

            Dictionary<string, string> resultDict = new Dictionary<string, string>();

            if (strResult.Contains("error_desc"))
            {
                resultDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(strResult.ToString());
            }



            if (resultDict.Count > 0)
            {
                if (resultDict["status"].ToString() == "0")
                {


                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "PAYMENT GATEWAY VALIDATION";
                    rm.name = StatusName.invalid;
                    rm.data = resultDict["error_desc"].ToString();
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PAYMENT GATEWAY VALIDATION", reqHeader, controllerURL, null, rm.data, rm.message));
                }

            }
            else
            {

                rm.statusCode = StatusCodes.OK;
                rm.message = "PAYMENT GATEWAY KEY";
                rm.name = StatusName.ok;
                rm.data = strResult;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PAYMENT GATEWAY KEY", reqHeader, controllerURL, null, strResult, StatusName.ok));
            }


            return Ok(rm);


        }

    /// <summary>
    /// Create an Order and Get Order_ID - WebApp
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "amount": 566,
    ///       "currency": "INR",
    ///       "receipt": "SAM",
    ///       "note": [
    ///         "string"
    ///       ],
    ///       "partialPayment": false,
    ///       "firstPaymentMinAmount": 0
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///	    [
    ///         {
    ///           "id": "order_Z6t7VFTb9xHeOs",
    ///           "entity": "order",
    ///           "amount": 100,
    ///           "amount_paid": 0,
    ///           "amount_due": 100,
    ///           "currency": "INR",
    ///           "receipt": "receipt#1",
    ///           "offer_id": null,
    ///           "status": "created",
    ///           "attempts": 0,
    ///           "notes": [],
    ///           "created_at": 1582628071
    ///         }
    ///	    ]
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">WebApp - Create Order  </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost]
    [Route("createorderwebapp")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> CreateOrderWebApp(OrderCreateWebApp itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
            rm = new ResponseMessage();

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount",itemData.Amount);
            input.Add("currency", itemData.Currency);
            input.Add("receipt",itemData.Receipt);
            input.Add("notes", itemData.note);
            input.Add("partial_payment", itemData.PartialPayment);
            input.Add("first_payment_min_amount", itemData.FirstPaymentMinAmount);

            RazorpayClient client = new RazorpayClient(Common.RazorPayKey, Common.RazorPaySecret);
            Razorpay.Api.Order order = client.Order.Create(input);

            var orderId = order["id"].ToString();

            if (orderId != null)
            {
                rm.statusCode = StatusCodes.OK;
                rm.message = "WebApp - Create Order CREATED";
                rm.name = StatusName.ok;
                rm.data = order;

                await Common.UpdateEventLogsNew("ORDER HAS BEEN CREATED SUCCESSFULLY - WEBAPP", reqHeader, controllerURL, itemData, order, StatusName.ok, this.eventLogBusiness);
            }
            else
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = "NO CONTENT";
                rm.name = StatusName.invalid;
                rm.data = null;

                await Common.UpdateEventLogsNew("CREATE ORDER - WEBAPP - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }


        }
        catch (Exception ex)
        {

            rm.statusCode = StatusCodes.ERROR;
            rm.message = ex.Message.ToString();
            rm.name = StatusName.invalid;
            rm.data = null;
            //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
            await Common.UpdateEventLogsNew("CREATE ORDER - WEBAPP - ERROR", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
        }
        return Ok(rm);

        }

    /// <summary>
    /// Verify Payment Signature - WebApp
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "orderID": 1986,
    ///       "razorpayPaymentId": "pay_29QQoUBi66xm2f",
    ///       "razorpayOrderId": "order_9A33XWu170gUtm",
    ///       "razorpaySignature": "c6e56af81070b5373f5f3fc9d4cccd873e5f8b9957bf71f6ef4dfa4535cb0c6e"
    ///     }
    ///     
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PAYMENT SIGNATURE HAS BEEN SUCCESSFULLY VERIFIED",
    ///       "data": "Payment Signature has been successfully verified"
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">"PAYMENT SIGNATURE HAS BEEN SUCCESSFULLY VERIFIED</response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    [HttpPost]
    [Route("verifypaymentsignature")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> verifypaymentsignature(ParamVerifySignature itemData)
    {
        var reqHeader = Request;
        string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        try
        {
                RazorpayClient client = new RazorpayClient(Common.RazorPayKey, Common.RazorPaySecret);
                var result = VerifySignature(Convert.ToString(itemData.OrderID), itemData.razorpayPaymentId, itemData.razorpaySignature);
                if(result!="")
                {
                    rm = new ResponseMessage();
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "PAYMENT SIGNATURE HAS BEEN SUCCESSFULLY VERIFIED";
                    rm.name = StatusName.ok;
                    rm.data = result;
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("WebApp - Order Items - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

    }
        private string VerifySignature(string OrderID, string razorpay_payment_id, string razorpay_signature)
        {
            var result = "";
            try
            {
                string key = OrderID + "|" + razorpay_payment_id+ Common.RazorPaySecret;
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(key);
                    byte[] hashBytes = sha256.ComputeHash(bytes);

                    var generated_signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Convert to hex string

                    if (generated_signature == razorpay_signature)
                    {
                        result = "Payment Signature has been successfully verified";
                    }
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private string InitiatePayment(PaymentTransactionData data)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            string returnResult = "";

            var objEZ = new Easebuzz(PaymentGatewayConfig.PAYMENTGATEWAY_SALT,
                             PaymentGatewayConfig.PAYMENTGATEWAY_KEY,
                             PaymentGatewayConfig.PAYMENTGATEWAY_ENV,
                             PaymentGatewayConfig.PAYMENTGATEWAY_SEAMLESS);

            //PaymentTransactionData sampleData = new PaymentTransactionData();



            rm = new ResponseMessage();

            var paymentDict = new Dictionary<string, string>
        {

            { "key", PaymentGatewayConfig.PAYMENTGATEWAY_KEY}, // tesing kit key
            { "amount", data.amount.ToString() },
            { "firstname", data.firstname },
            { "email", data.email },
            { "phone", data.phone },
            { "productinfo", data.productinfo},
            { "surl", PaymentGatewayConfig.PAYMENTGATEWAY_SUCCESSURL },
            { "furl", PaymentGatewayConfig.PAYMENTGATEWAY_ERRORURL },
            { "txnid", data.txnid },
            { "udf1", data.udf1 },
            { "udf2", data.udf2},
            { "udf3", data.udf3},
            { "udf4", data.udf4},
            { "udf5", data.udf5},
            { "udf6", data.udf6},
            { "udf7", data.udf7},
            { "udf8", data.udf8},
            { "udf9", data.udf9},
            { "udf10",data.udf10 }
        };


            var strResult = objEZ.initiatePaymentAPI(paymentDict);

            Dictionary<string, string> resultDict = new Dictionary<string, string>();

            if (strResult.Contains("error_desc"))
            {
                resultDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(strResult.ToString());
            }



            if (resultDict.Count > 0)
            {
                if (resultDict["status"].ToString() == "0")
                {
                    //rm.statusCode = StatusCodes.ERROR;
                    //rm.message = "PAYMENT GATEWAY VALIDATION";
                    //rm.name = StatusName.invalid;
                    //rm.data = resultDict["error_desc"].ToString();

                    returnResult = resultDict["error_desc"].ToString();

                }

            }
            else
            {

                //rm.statusCode = StatusCodes.OK;
                //rm.message = "PAYMENT GATEWAY KEY";
                //rm.name = StatusName.ok;
                //rm.data = strResult;

                returnResult = strResult;

            }


            return returnResult;



        }

    /// <summary>
    /// PAYMENT GATEWAY VALIDATION
    /// </summary>
    /// <remarks>    
    /// Sample response JSON :
    /// 
    ///     {
    ///       "statusCode": 200,
    ///       "name": "SUCCESS_OK",
    ///       "message": "PAYMENT GATEWAY KEY",
    ///       "data": "{\"status\":false,\"msg\":\"Transaction not found.\"}"
    ///     }
    /// 
    /// </remarks>
    /// <returns>ResponseMessage Object</returns>
    /// <response code="200">PAYMENT GATEWAY VALIDATION </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
    /// 

    [HttpPost, Route("TestTransaction")]
    [MapToApiVersion("1.0")]
        public async Task<IActionResult> TestTransactionAPIAsync()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            var objEZ = new Easebuzz(PaymentGatewayConfig.PAYMENTGATEWAY_SALT,
                                         PaymentGatewayConfig.PAYMENTGATEWAY_KEY,
                                         PaymentGatewayConfig.PAYMENTGATEWAY_ENV,
                                         PaymentGatewayConfig.PAYMENTGATEWAY_SEAMLESS);



            PaymentTransactionData sampleData = new PaymentTransactionData();

            rm = new ResponseMessage();



            var strResult = objEZ.transactionAPI(sampleData.txnid, sampleData.amount.ToString(), sampleData.email, sampleData.phone);


            Dictionary<string, string> resultDict = new Dictionary<string, string>();

            if (strResult.Contains("error_desc"))
            {
                resultDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(strResult.ToString());
            }



            if (resultDict.Count > 0)
            {
                if (resultDict["status"].ToString() == "0")
                {


                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "PAYMENT GATEWAY VALIDATION";
                    rm.name = StatusName.invalid;
                    rm.data = resultDict["error_desc"].ToString();
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PAYMENT GATEWAY VALIDATION", reqHeader, controllerURL, null, rm.data, rm.message));
                }

            }
            else
            {

                rm.statusCode = StatusCodes.OK;
                rm.message = "PAYMENT GATEWAY KEY";
                rm.name = StatusName.ok;
                rm.data = strResult;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("PAYMENT GATEWAY KEY", reqHeader, controllerURL, null, strResult, StatusName.ok));
            }


            return Ok(rm);

        }

        internal class OrderPaymentData
        {

            public string OrderNo { get; set; }
            public string AccessKey { get; set; }

            public Int64 OrderID { get; set; }
            public string ErrorMsg { get; set; }
        }

        internal class PaymentTransactionData
        {
            public float amount { get; set; }
            public string firstname { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string productinfo { get; set; }
            public string txnid { get; set; }
            public string udf1 { get; set; }
            public string udf2 { get; set; }
            public string udf3 { get; set; }
            public string udf4 { get; set; }
            public string udf5 { get; set; }
            public string udf6 { get; set; }
            public string udf7 { get; set; }
            public string udf8 { get; set; }
            public string udf9 { get; set; }
            public string udf10 { get; set; }



            public PaymentTransactionData()
            {
                amount = 200.00F;
                firstname = "appify_testuser";
                email = "testuser@appi-fy.ai";
                phone = "9885217825";
                productinfo = "test product";
                txnid = "TXNORD_TEST001";
                udf1 = "";
                udf2 = "";
                udf3 = "";
                udf10 = "";
                udf4 = "";
                udf5 = "";
                udf6 = "";
                udf7 = "";
                udf8 = "";
                udf9 = "";

            }

        }
    }

}
