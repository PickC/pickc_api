using appify.Business.Contract;
using appify.models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using appify.utility;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Numerics;

namespace appify.web.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]

    public class OrderController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IOrderBusiness orderBusiness;
        private readonly IInvoiceBusinesss invoiceBusinesss;
        //public NotificationService _notificationService;
        //private readonly FcmNotificationSetting? _fcmNotificationSetting;
        private NotificationModel _notificationModel;
        private ResponseMessage rm;
        //private Notifications _emailnotifications;
        //private readonly INotificationBusiness notificationBusiness;
        //private PushNotification notification;
        public OrderController(IConfiguration configuration, IOrderBusiness orderBusiness, IInvoiceBusinesss invoiceBusinesss)
        {////, INotificationBusiness IResultData
            this.configuration = configuration;
            this.orderBusiness = orderBusiness;
            this.invoiceBusinesss = invoiceBusinesss;
            ////this.notificationBusiness = IResultData;

           // _fcmNotificationSetting = new FcmNotificationSetting();

            ///// FCM Notification

           /// _notificationModel = new NotificationModel();
           // _fcmNotificationSetting.ServerKey = configuration["FcmNotification:ServerKey"].ToString();
           // _fcmNotificationSetting.SenderId = configuration["FcmNotification:SenderId"].ToString();
            //_notificationService = new NotificationService(_fcmNotificationSetting);

            //Email Notification
            ///_emailnotifications = new Notifications();
        }

        [HttpPost, Route("save")]
        public IActionResult Add(Order order)
        {

            try
            {
                rm = new ResponseMessage();
                var result = this.orderBusiness.Save(order);
                if (result != null)
                {

                    PaymentTransactionData sampleData = new PaymentTransactionData();

                    sampleData.txnid = result.OrderNo;
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
                    //string firstName = order.FirstName.ToString();
                    //if (firstName.Length != 0)
                    //{
                    //    firstName = char.ToUpper(firstName[0]) + firstName.Substring(1);
                    //}
                    //_emailnotifications.ToEmail = order.EmailID;
                    //_emailnotifications.ToEmailCC = NotificationConfig.TO_BCC;
                    //_emailnotifications.ToEmailBCC = NotificationConfig.TO_CC;
                    //_emailnotifications.EmailSubject = NotificationConfig.ORDER_EMAIL_SUBJECT;
                    //_emailnotifications.EmailTemplateURL = NotificationConfig.ORDER_SAVE_EMAIL_TEMPLATE_URL;
                    //_emailnotifications.EmailTemplae_ReplaceName = firstName;
                    //notificationBusiness.SendEmail(_emailnotifications);

                    /////FCM Notification --------

                    ////_notificationModel.IsAndroiodDevice = true;
                    //// _notificationModel.DeviceId = order.DeviceToken;
                    // _notificationModel.Title = "Hi " + firstName;
                    //// _notificationModel.Body = NotificationConfig.ORDER_FCM_SUBJECT;
                    //// _notificationService.SendNotification(_notificationModel);
                    //using (NotificationService service = new NotificationService(_fcmNotificationSetting))
                    //{
                    //    _notificationModel.IsAndroiodDevice = true;
                    //    _notificationModel.DeviceId = order.DeviceToken;
                    //    _notificationModel.Title = "Hi " + firstName;
                    //    _notificationModel.Body = NotificationConfig.ORDER_FCM_SUBJECT;
                    //    service.SendNotificationAsync(_notificationModel);
                    //}
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO ADD/UPDATE order";
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

        [HttpPost, Route("remove")]
        public IActionResult Remove(Int64 orderID)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO DE-ACTIVATE order";
                    rm.name = StatusName.invalid;
                    rm.data = result;
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


        [HttpPost, Route("printinvoice")]
        public IActionResult PrintInvoice(Int64 orderID)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO GENERATE INVOICE";
                    rm.name = StatusName.invalid;
                    rm.data = result;
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

        [HttpPost, Route("updatestatus")]
        public IActionResult UpdateOrderStatus(ParamOrderStatus statusData)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
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



        [HttpPost, Route("updateorderforpickup")]
        public IActionResult UpdateOrderForPickup(ParamOrderForPickup statusData)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER PICKUP STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
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

         

        [HttpPost, Route("updateorderawb")]
        public IActionResult UpdateOrderAWB(ParamOrderAWB statusData)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO UPDATE ORDER PICKUP STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = result;
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



        [HttpPost, Route("gettrackingdetails")]
        public IActionResult GetOrderTrackingDetails(Int64 orderID)
        {

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
                }
                else
                {
                    rm.statusCode = StatusCodes.ERROR;
                    rm.message = "UNABLE TO FETCH ORDER TRACKING STATUS";
                    rm.name = StatusName.invalid;
                    rm.data = orderID;
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


        [HttpPost, Route("getitem")]
        public IActionResult Getorder(long orderID)
        {

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


        [HttpPost, Route("getorderpickup")]
        public IActionResult Getorderfordelivery(long orderID)
        {

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

        [HttpPost, Route("list")]
        public IActionResult List(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;

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

        [HttpPost, Route("summarylist")]
        public IActionResult SummaryList(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;

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


        [HttpPost, Route("vendororderlist")]
        public IActionResult ListByVendor(ParamMemberUserID itemData)
        {
            //dynamic data = jsonData;

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


        [HttpPost, Route("TestPayment")]
        public async Task<IActionResult> InitiatePaymentAsync()
        {
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
                }

            }
            else
            {

                rm.statusCode = StatusCodes.OK;
                rm.message = "PAYMENT GATEWAY KEY";
                rm.name = StatusName.ok;
                rm.data = strResult;

            }


            return Ok(rm);


        }


        private string InitiatePayment(PaymentTransactionData data)
        {

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
                }

            }
            else
            {

                rm.statusCode = StatusCodes.OK;
                rm.message = "PAYMENT GATEWAY KEY";
                rm.name = StatusName.ok;
                rm.data = strResult;

            }


            return Ok(rm);

        }
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
