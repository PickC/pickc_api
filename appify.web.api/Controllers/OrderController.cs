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

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]

    public class OrderController : ControllerBase
    {
        public readonly IEventLogBusiness eventLogBusiness;
        private readonly IConfiguration configuration;
        private readonly IOrderBusiness orderBusiness;
        private readonly IInvoiceBusinesss invoiceBusinesss;
        private NotificationModel notificationModel;
        private ResponseMessage rm;
        private Notifications emailnotifications;
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

            ////Email Notification Objects
            emailnotifications = new Notifications();
        }

        [HttpPost, Route("save")]
        public IActionResult Add(Order order)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;

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


                    Invoice invoiceItem = new Invoice();

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


                        if ((ordItem.UnitPrice * 100) / (100 + (GSTPercent * 2)) > 1050)
                        {
                            GSTPercent = 6.0M;
                            
                        }

                        //originalPrice = Math.Round((ordItem.UnitPrice * 100) / (100 + (GSTPercent * 2)), 2, MidpointRounding.AwayFromZero);
                        originalPrice = Math.Round(((ordItem.UnitPrice * 100) / (100 + (GSTPercent * 2))),2);

                        //gstValue = (ordItem.UnitPrice * (GSTPercent * 2)) / 100;
                        //gstValue = Math.Round(((originalPrice * (GSTPercent)) / 100), 2, MidpointRounding.AwayFromZero);
                        gstValue = Math.Round(((originalPrice * (GSTPercent)) / 100),2);




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
                            dt.CGST = Math.Round((gstValue * ordItem.Quantity),2);
                            dt.SGST = Math.Round((gstValue * ordItem.Quantity),2);

                        }
                        else
                        {
                            //dt.IGST = Math.Round((gstValue * 2 * ordItem.Quantity), 2, MidpointRounding.AwayFromZero);
                            dt.IGST = Math.Round((gstValue * 2 * ordItem.Quantity),2);
                        }
                        //dt.TaxAmount = Math.Round(dt.SGST + dt.CGST + dt.IGST, 2, MidpointRounding.AwayFromZero);
                        //dt.UnitPrice = Math.Round(originalPrice, 2, MidpointRounding.AwayFromZero);
                        //dt.SellingAmount = Math.Round(((dt.UnitPrice * ordItem.Quantity)  + dt.TaxAmount), 2, MidpointRounding.AwayFromZero);

                        dt.TaxAmount = Math.Round(dt.SGST + dt.CGST + dt.IGST);
                        dt.UnitPrice = Math.Round(originalPrice);
                        dt.SellingAmount = Math.Round(((dt.UnitPrice * ordItem.Quantity) + dt.TaxAmount), 2);


                        invoiceItem.items.Add(dt);

                        ordItem.UnitPrice = dt.UnitPrice;
                        ordItem.SellingPrice = dt.SellingAmount;


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

                    ////Email Notification -------

                    //emailnotifications.ToEmail = order.EmailID;
                    //emailnotifications.ToEmailCC = NotificationConfig.TO_BCC;
                    //emailnotifications.ToEmailBCC = NotificationConfig.TO_CC;
                    //emailnotifications.EmailSubject = NotificationConfig.ORDER_EMAIL_SUBJECT;
                    //emailnotifications.EmailTemplateURL = NotificationConfig.ORDER_SAVE_EMAIL_TEMPLATE_URL;
                    //emailnotifications.EmailTemplae_ReplaceName = firstName;
                    //notificationBusiness.SendEmail(emailnotifications);

                    /////FCM Notification --------

                    //string firstName = order.FirstName.ToString();
                    //if (firstName.Length != 0)
                    //{
                    //    firstName = char.ToUpper(firstName[0]) + firstName.Substring(1);
                    //}


                    if (order.MemberID!=0)
                    {
                        Pushnotification.SendNotificationMessage(Convert.ToInt64(NotificationTemplateType.OrderConfirmation), order.VendorID, order.MemberID, data.OrderID, "<first_name>",this.notificationBusiness);
                    }
                    if(order.VendorID!=0)
                    {
                        Pushnotification.SendNotificationMessage(Convert.ToInt64(NotificationTemplateType.OrderPlacement), 0, order.VendorID, data.OrderID, "<Vendor/Shop>", this.notificationBusiness);
                    }

                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Saved", reqHeader, controllerURL, order, data, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO ADD/UPDATE order";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order - Unable", reqHeader, controllerURL, order, null, rm.message));
                     
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order - Error", reqHeader, controllerURL, order, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("remove")]
        public IActionResult Remove(Int64 orderID)
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Removed", reqHeader, controllerURL, orderID, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DE-ACTIVATE order";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order - Unable to Removed", reqHeader, controllerURL, orderID, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order - Remove - Error", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }


        [HttpPost, Route("printinvoice")]
        public IActionResult PrintInvoice(Int64 orderID)
        {
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Invoice Generated", reqHeader, controllerURL, orderID, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO GENERATE INVOICE";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Not Generated", reqHeader, controllerURL, orderID, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Invoice Error", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("updatestatus")]
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
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order is Updated", reqHeader, controllerURL, statusData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order not Updated", reqHeader, controllerURL, statusData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Updated Error", reqHeader, controllerURL, statusData, null, rm.message));
            }
            return Ok(rm);

        }



        [HttpPost, Route("updateorderforpickup")]
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Pickup status updated", reqHeader, controllerURL, statusData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER PICKUP STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Pickup status not updated", reqHeader, controllerURL, statusData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Pickup status Error", reqHeader, controllerURL, statusData, null, rm.message));
            }
            return Ok(rm);

        }

         

        [HttpPost, Route("updateorderawb")]
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Pickup status updated", reqHeader, controllerURL, statusData, result, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER PICKUP STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Pickup status not updated", reqHeader, controllerURL, statusData, null, rm.message));
                }
            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Order Pickup status not updated", reqHeader, controllerURL, statusData, null, rm.message));
            }
            return Ok(rm);

        }



        [HttpPost, Route("gettrackingdetails")]
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("ORDER TRACKING DETAILS FETCHED", reqHeader, controllerURL, orderID, result, StatusName.ok));
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, orderID, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, orderID, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }


        [HttpPost, Route("getorderpickup")]
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, orderID, item, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, orderID, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, orderID, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("list")]
        public IActionResult List(ParamMemberUserID itemData)
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        [HttpPost, Route("summarylist")]
        public IActionResult SummaryList(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<CustomerOrderSummary> items = orderBusiness.CustomerSummaryList(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH order LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }


            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }


        [HttpPost, Route("vendororderlist")]
        public IActionResult ListByVendor(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            try
            {
                rm = new ResponseMessage();
                List<VendorOrder> items = orderBusiness.ListByVendor(itemData.userID);
                if (items?.Any() == true)
                {
                    rm.statusCode = StatusCodes.OK;
                    rm.message = "FETCH VENDOR ORDER LIST";
                    rm.name = StatusName.ok;
                    rm.data = items;
                    //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, items, StatusName.ok));
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "NO CONTENT";
                    rm.name = StatusName.invalid;
                    rm.data = null;
                    //// Passing HttpRequest, Controller Url, InputJSon, OutJson, Status
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
                }

            }
            catch (Exception ex)
            {

                rm.statusCode = StatusCodes.ERROR;
                rm.message = ex.Message.ToString();
                rm.name = StatusName.invalid;
                rm.data = null;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, itemData, null, rm.message));
            }
            return Ok(rm);

        }

        /// <summary>
        /// PhonePe WebHook for Order Paid.
        /// </summary>
        /// <remarks>
        /// Sample request:
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
        /// <param name="payload"></param>
        /// <returns>ResponseMessage Object</returns>
        /// <response code="200">Returns the newly created Discount Object</response>
        /// <response code="500">ResponseMessage with Error Description</response> 

        [HttpPost]
        [Route("phonepaywebhook_paid")]
        public IActionResult ReceiveWebhook([FromBody] PhonePeWebhookPayload payload)////object payload
        {  var reqHeader = Request;
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
        }

        /// <summary>
        /// RazorPay WebHook for Payment Captured.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// NOTE : RazorPay WebHook for Payment Captured.
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
        [Route("payment-captured")]
        public IActionResult PaymentCaptured([FromBody] RazorpayWebhookPayload payload)
        {
            var reqHeader = Request;
            string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
            rm = new ResponseMessage();
            try
            {
                if (payload == null || payload.Event != "payment.captured")
                {
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "Received null payload", "Received null payload", StatusName.ok));
                }

                // Log the payload for debugging purposes
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "Received webhook", "Received webhook: {Payload}" + JsonConvert.SerializeObject(payload), StatusName.ok));
                // Process the payment captured event
                var payment = payload.Payload.Payment.Entity;
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, "Received webhook", $"Payment captured: Id={payment.Id}, Amount={payment.Amount}, Status={payment.Status}", StatusName.ok));

                rm.statusCode = StatusCodes.OK;
                rm.message = "RECEIVED WEBHOOK - RAZORPAY RESPONSE SUCCESSFULLY";
                rm.name = StatusName.ok;
                rm.data = payload;
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

        [HttpPost, Route("TestPayment")]
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("TestPayment", reqHeader, controllerURL, null, rm.data, rm.message));
                }

            }
            else
            {

                rm.statusCode = StatusCodes.OK;
                rm.message = "PAYMENT GATEWAY KEY";
                rm.name = StatusName.ok;
                rm.data = strResult;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("TestPayment", reqHeader, controllerURL, null, strResult, StatusName.ok));
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
                    this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("TestPayment", reqHeader, controllerURL, null, rm.data, rm.message));
                }

            }
            else
            {

                rm.statusCode = StatusCodes.OK;
                rm.message = "PAYMENT GATEWAY KEY";
                rm.name = StatusName.ok;
                rm.data = strResult;
                //// Passing EventType, HttpRequest, Controller Url, InputJSon, OutJson, Status
                this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("TestPayment", reqHeader, controllerURL, null, strResult, StatusName.ok));
            }


            return Ok(rm);

        }

        //private void SendNotificationMessage(Int64 TemplateID, Int64 VendorID, Int64 MemberID, Int64 OrderID, string replaceTitle)
        //{
        //   // var reqHeader = Request;
        //   // string controllerURL = new Uri(HttpContext.Request.GetDisplayUrl()).AbsoluteUri;
        //    try
        //    {
        //        //////Notification Template
        //        NotificationTemplate notificationTemplate = this.notificationBusiness.GetNotificationTemplate(TemplateID);
        //        VendorDetails vendorDetails = this.notificationBusiness.GetVendorDetails(MemberID, OrderID);
        //        notificationModel.IsAndroiodDevice = true;
        //        notificationModel.DeviceId = vendorDetails.Token;
        //        notificationModel.FCMSenderID = vendorDetails.FCMSenderID;
        //        notificationModel.FCMServerKey = vendorDetails.FCMServerKey;
        //        notificationModel.Title = notificationTemplate.MessageTitle.Replace(replaceTitle, vendorDetails.FirstName).Trim();
        //        if(TemplateID==1007) ////Order Status Change
        //        {
        //            notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
        //                .Replace("<Delivery Date>", vendorDetails.OrderNo).Trim()
        //                .Replace("<Delivery Date>", vendorDetails.OrderNo).Trim();
        //        }
        //        else if(TemplateID ==1010) ////Refund Processed
        //        {
        //            notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
        //               .Replace("<date range>", vendorDetails.OrderNo).Trim();
        //        }
        //        else if(TemplateID == 1011) ////Order Received
        //        {
        //                    notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim()
        //                   .Replace("<Tracking Link>", vendorDetails.OrderNo).Trim();
        //        }
        //        else if(TemplateID == 1013) ////Back-in-Stock Notification
        //        {
        //            notificationModel.Body = notificationTemplate.MessageBody.Replace("<product page link>", vendorDetails.OrderNo).Trim();
        //        }
        //        else
        //        {
        //            notificationModel.Body = notificationTemplate.MessageBody.Replace("#<order No>", vendorDetails.OrderNo).Trim();
        //        }

        //        Pushnotification.FCMPushNotification(notificationModel);

        //        PushNotificationMessage pushNotificationMessage = new PushNotificationMessage
        //        { SenderID = VendorID, ReceiverID = MemberID, NotificationTitle = notificationModel.Title, NotificationMessage = notificationModel.Body };
        //        this.notificationBusiness.addNotificationMessage(pushNotificationMessage);

        //        //this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, pushNotificationMessage, null, StatusName.ok));
        //    }
        //    catch (Exception ex)
        //    {

        //      //  this.eventLogBusiness.eventLogAdd(Common.UpdateEventLogs("Transaction", reqHeader, controllerURL, null, null, ex.Message.ToString()));
        //    }
        //}
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
