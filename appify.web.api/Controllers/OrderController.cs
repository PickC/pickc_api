using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using appify.utility;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Numerics;
using Microsoft.AspNetCore.Http.Extensions;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static appify.models.NotificationType;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Azure.Core;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Twilio.Http;
using appify.Business;
using Razorpay.Api;
using Asp.Versioning;

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
        private NotificationModel notificationModel;
        private ResponseMessage rm;
        private readonly INotificationBusiness notificationBusiness;
        public OrderController(IConfiguration configuration, IOrderBusiness orderBusiness, IInvoiceBusinesss invoiceBusinesss, IEventLogBusiness eventLogBusiness, INotificationBusiness IResultData)
        {
            this.configuration = configuration;
            this.orderBusiness = orderBusiness;
            this.invoiceBusinesss = invoiceBusinesss;
            this.eventLogBusiness = eventLogBusiness;
            this.notificationBusiness = IResultData;

            ////FCM Objects
            notificationModel = new NotificationModel();
        }

        [HttpPost, Route("save")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Add(appify.models.Order order)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //// SMSNotification.SMSNotificationMessage();
            try
            {
                rm = new ResponseMessage();
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

        [HttpPost, Route("remove")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Remove(Int64 orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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


        [HttpPost, Route("printinvoice")]
        [MapToApiVersion("1.0")]
        public IActionResult PrintInvoice(Int64 orderID)
        {
            //OrderPlace_PushNotification_Email(1976);
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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

        [HttpPost, Route("updatestatus")]
        [MapToApiVersion("1.0")]
        public IActionResult UpdateOrderStatus(ParamOrderStatus statusData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
                var result = orderBusiness.UpdateOrderStatus(statusData.OrderID, statusData.OrderStatus, statusData.Remarks);
                if (result)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "STATUS UPDATED SUCCESSFULLY!";
                    rm.name = StatusName.ok;
                    rm.data = statusData.OrderID.ToString();
                    OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(statusData.OrderID);
                    /////FCM Notification AND Email Notification
                    if (statusData.OrderStatus == 3587) //// Cancelled by Customer Cast 1 - We have 2 cases 1st is before confirmed the order
                    {
                        if (orderUpdateDetail.VendorID != 0)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            //EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                        }
                        if (orderUpdateDetail.MemberID != 0)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                        }
                    }
                    if (statusData.OrderStatus == 3588) //// Declined by Vendor
                    {
                        if (orderUpdateDetail.VendorID != 0)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            //EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationVendorOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                        }
                        if (orderUpdateDetail.MemberID != 0)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationVendorCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                        }
                    }
                    if (statusData.OrderStatus == 3577) //// Order Confirmed by Vendor
                    {
                        if (orderUpdateDetail.VendorID != 0)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderConfirmationVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            //EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderConfirmationOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                        }
                        if (orderUpdateDetail.MemberID != 0)
                        {
                            EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderConfirmationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                            PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
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



        [HttpPost, Route("updateorderforpickup")]
        [MapToApiVersion("1.0")]
        public IActionResult UpdateOrderForPickup(ParamOrderForPickup statusData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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



        [HttpPost, Route("updateorderawb")]
        [MapToApiVersion("1.0")]
        public IActionResult UpdateOrderAWB(ParamOrderAWB statusData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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



        [HttpPost, Route("gettrackingdetails")]
        [MapToApiVersion("1.0")]
        public IActionResult GetOrderTrackingDetails(Int64 orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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


        [HttpPost, Route("getitem")]
        [MapToApiVersion("1.0")]
        public IActionResult Getorder(long orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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


        [HttpPost, Route("getorderpickup")]
        [MapToApiVersion("1.0")]
        public IActionResult Getorderfordelivery(long orderID)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            //dynamic data = jsonData;
            try
            {
                rm = new ResponseMessage();
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

        [HttpPost, Route("list")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> List(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
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

        [HttpPost, Route("summarylist")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SummaryList(ParamMemberOrder itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
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


        [HttpPost, Route("vendororderlist")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ListByVendor(ParamMemberOrder itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
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

        [HttpPost, Route("vendororderdetail")]
        [MapToApiVersion("1.0")]
        public IActionResult GetDetailByVendor(ParamVendorOrder itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
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
                //// Order Placed By Customer COD & Online
                OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(OrderID);
                /////FCM Notification AND Email Notification
                if (orderUpdateDetail.VendorID != 0) //// New Order Placement send Mail and notification to Vendor & Opps
                {
                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderPlacementVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                    //EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderPlacementOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                }
                if (orderUpdateDetail.MemberID != 0)//// New Order Placement send Mail and notification to Customer
                {
                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderPlacementCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderPlacementCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
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
        /// <response code="200">Returns the newly created Discount Object</response>
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
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response>

        [HttpPost]
        [Route("WebhookPaymentEvents")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> WebhookPaymentEvents()/////[FromBody] RazorpayWebhookPayload payload
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            bool eventResult = false;
            string[] eventSearch ={
              "downtime",
              "payment_link",
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
                var body = await reader.ReadToEndAsync();

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
                await Common.UpdateEventLogsNew("RAZORPAY Webhook Error Response", reqHeader, controllerURL, "RAZORPAY Webhook Error Response", null, rm.message, this.eventLogBusiness);
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
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response>

        [HttpPost]
        [Route("WebhookShipRocket")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> WebhookShipRocket()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            try { 
                    // Verify the X-VERIFY header.
                   string xVerifyHeader = reqHeader.Headers["x-api-key"];////verifyRequestModel.X_VERIFY;

                    //xVerifyHeader = "Appify@1234#";
                if (xVerifyHeader == null || xVerifyHeader == "")//// || !VerifyXVerifyHeaderShipRocket(xVerifyHeader)
                {
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("SHIPROCKET Webhook Null Payload", reqHeader, controllerURL, "SHIPROCKET Webhook Null Payload", "Received null payload", StatusName.ok));
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
                        var body = await reader.ReadToEndAsync();

                        // As well as a bound model
                        //var request = JsonConvert.DeserializeObject(body);
                        //var body = "{\r\n  \"awb\": \"123456\",\r\n  \"courier_name\": \"DTDC Courier\",\r\n  \"current_status\": \"Delivered\",\r\n  \"current_status_id\": 7,\r\n  \"shipment_status\": \"Delivered\",\r\n  \"shipment_status_id\": 7,\r\n  \"current_timestamp\": \"11 06 2024 20:18:24\",\r\n  \"order_id\": \"OD10602406156\",\r\n  \"sr_order_id\": 1234,\r\n  \"etd\": \"2024-06-11 20:18:24\",\r\n  \"scans\": [\r\n    {\r\n      \"location\": \"Mumbai_Chndivli_PC (Maharashtra)\",\r\n      \"date\": \"2022-05-16 16:18:47\",\r\n      \"activity\": \"Manifested - Consignment Manifested\",\r\n      \"status\": \"new\",\r\n      \"sr-status\": \"NA\",\r\n      \"sr-status-label\": \"NA\"\r\n    },\r\n    {\r\n      \"location\": \"Mumbai_Chndivli_PC (Maharashtra)\",\r\n      \"date\": \"2022-05-17 09:59:03\",\r\n      \"activity\": \"Manifested - Consignment Manifested\",\r\n      \"status\": \"assigned_for_seller_pickup\",\r\n      \"sr-status\": 19,\r\n      \"sr-status-label\": \"OUT FOR PICKUP\"\r\n    }\r\n  ],\r\n  \"is_return\": 0,\r\n  \"channel_id\": 1234\r\n}";

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
                           /* if (orderTrackingUpdate.OrderStatus == 7) //// Delivered
                            {
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(NotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.DeliveryConfirmation), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                }
                            }
                            else if (orderTrackingUpdate.OrderStatus == 6) //// Shipped
                            {
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(NotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.ShippingDeliveryUpdates), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                }
                            }
                            else if (orderTrackingUpdate.OrderStatus == 19) //// In Transit
                            {
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(NotificationTemplateType.DeliveryUpdates), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.DeliveryUpdates), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                }
                            }
                            else if (orderTrackingUpdate.OrderStatus == 37) //// Delivery Delayed
                            {
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(NotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.DelayedShipmentNotification), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                }
                            }*/
                            if (orderTrackingUpdate.OrderStatus == 5) //// Cancelled by Customer
                            {
                                if (orderUpdateDetail.VendorID != 0)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    //EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                                }
                                if (orderUpdateDetail.MemberID != 0)
                                {
                                    EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                    PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                                }
                            }
                        }
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("SHIPROCKET WEBHOOK - SHIPROCKET RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "SHIPROCKET Webhook Sucess Response", request, StatusName.ok));
                    await Common.UpdateEventLogsNew("SHIPROCKET WEBHOOK - SHIPROCKET RESPONSE SUCCESSFULLY", reqHeader, controllerURL, "SHIPROCKET Webhook Sucess Response", requestObj, StatusName.ok, this.eventLogBusiness);
                }

                }
            catch (Exception ex)
            {
                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("SHIPROCKET Webhook Error Received", reqHeader, controllerURL, "SHIPROCKET Webhook Error Received", null, rm.message));
                await Common.UpdateEventLogsNew("SHIPROCKET Webhook Error Received", reqHeader, controllerURL, "SHIPROCKET Webhook Error Received", null, rm.message, this.eventLogBusiness);
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
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response>

        [HttpPost]
        [Route("WebhookOneDelhivery")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> WebhookOneDelhivery()
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            try { 
            // Verify the X-VERIFY header.
            string xVerifyHeader = reqHeader.Headers["x-api-key"];
            if (xVerifyHeader == null || xVerifyHeader == "")
            {
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "ONEDELHIVERY Webhook Received Null Payload", "Received null payload", StatusName.ok));
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
                    var body = await reader.ReadToEndAsync();
                //var body = "{    \"Shipment\": {      \"AWB\": \"19041618371282\",      \"ReferenceNo\": \"OD26232407001\",      \"PickUpDate\": \"2024-07-26T17:53:53\",      \"Sortcode\": \"DEL/UDY\",      \"NSLCode\": \"RD-AC\",      \"Status\": {        \"Status\": \"RTO\",        \"StatusDateTime\": \"2024-08-06T18:   27:36.822\",        \"StatusType\": \"DL\",        \"StatusLocation\": \"Pushpavanam_Vedaranyam_D (Tamil Nadu)\",        \"Instructions\": \"RETURN Accepted\"      }    }  }";
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
                         
                    var result = orderBusiness.UpdateDelhiveryOrderTrackingStatus(orderTrackingUpdate);
                    if (result>0)
                    {
                        rm.statusCode = StatusCodes.OK;
                        rm.message = "ONEDELHIVERY WEBHOOK - ONEDELHIVERY RESPONSE SUCCESSFULLY";
                        rm.name = StatusName.ok;
                        rm.data = requestObj;
                        OrderUpdateDetail orderUpdateDetail = orderBusiness.GetOrderUpdateDetail(result);
                        if (orderTrackingUpdate.Status == "RTO") //// Cancelled by Customer
                        {
                            if (orderUpdateDetail.VendorID != 0)
                            {
                                EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerVendor), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                //EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomerOpps), orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationVendor), 0, orderUpdateDetail.VendorID, orderUpdateDetail.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                            }
                            if (orderUpdateDetail.MemberID != 0)
                            {
                                EmailNotification.SendEmailNotification(Convert.ToInt64(NotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, this.notificationBusiness);
                                PushNotification.SendNotificationMessage(Convert.ToInt64(PushNotificationTemplateType.OrderCancellationCustomer), orderUpdateDetail.VendorID, orderUpdateDetail.MemberID, orderUpdateDetail.OrderID, "<first_name>", this.notificationBusiness);
                            }
                        }
                    }
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "ONEDELHIVERY Webhook Sucess Response", requestObj, StatusName.ok));

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
                await Common.UpdateEventLogsNew("ONEDELHIVERY Webhook Error", reqHeader, controllerURL, "ONEDELHIVERY Webhook Error", null, rm.message, this.eventLogBusiness);
            }
            // Respond with a 200 OK status to acknowledge the receipt of the webhook
            return Ok(rm);
        }

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
    /// WebApp - Create Order 
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "amount": 566,
    ///       "currency": "INA",
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
    /// <response code="200">Returns Product Item against the VendorID </response>
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

                //var items = customerBusiness.GetProductListByVAUA(itemData.userID);
                if (orderId != null)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "RAZORPAY ORDER HAS BEEN SUCCESSFULLY CREATED";
                    rm.name = StatusName.ok;
                    rm.data = order;

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS SUCCESSFULLY", reqHeader, controllerURL, itemData, items, StatusName.ok));
                    await Common.UpdateEventLogsNew("RAZORPAY ORDER HAS BEEN SUCCESSFULLY CREATED", reqHeader, controllerURL, itemData, order, StatusName.ok, this.eventLogBusiness);
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message));
                    await Common.UpdateEventLogsNew("RAZORPAY CREATE ORDER - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("FETCH ALL DETAILS - ERROR", reqHeader, controllerURL, itemData, null, rm.message));
                await Common.UpdateEventLogsNew("RAZORPAY CREATE ORDER - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

        }

    /// <summary>
    /// WebApp - Order Items Save
    /// </summary>
    /// <remarks>
    /// Sample request JSON :
    /// 
    ///     {
    ///       "orderID": 0,
    ///       "razorpayPaymentId": "string",
    ///       "razorpayOrderId": "string",
    ///       "razorpaySignature": "string"
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
    /// <response code="200">Returns Product Item against the VendorID </response>
    /// <response code="500">ResponseMessage with Error Description</response> 
        [HttpPost]
        [Route("saveitemsorderwebapp")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> SaveItemsOrderWebApp(ParamOrderItem itemData)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                rm.statusCode = StatusCodes.OK;
                rm.message = "RAZORPAY ORDER HAS BEEN SUCCESSFULLY SAVED";
                rm.name = StatusName.ok;
                rm.data = true;
                //var items = this.orderBusiness.GetOrderForDelivery(itemData.OrderID);
                //if (items != null)
                //{
                //    rm.statusCode = StatusCodes.OK;
                //    rm.message = "RAZORPAY ORDER HAS BEEN SUCCESSFULLY SAVED";
                //    rm.name = StatusName.ok;
                //    rm.data = items;

                //    await Common.UpdateEventLogsNew("RAZORPAY ORDER HAS BEEN SUCCESSFULLY CREATED", reqHeader, controllerURL, itemData, items, StatusName.ok, this.eventLogBusiness);
                //}
                //else
                //{
                //    rm.statusCode = StatusCodes.ERROR;
                //    rm.message = "NO CONTENT";
                //    rm.name = StatusName.invalid;
                //    rm.data = null;
                //    await Common.UpdateEventLogsNew("RAZORPAY CREATE ORDER - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
                //}


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                await Common.UpdateEventLogsNew("RAZORPAY CREATE ORDER - NO CONTENT", reqHeader, controllerURL, itemData, null, rm.message, this.eventLogBusiness);
            }
            return Ok(rm);

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
