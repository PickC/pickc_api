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
        private ResponseMessage rm;

        public CommonServicesController(IConfiguration configuration, IEventLogBusiness eventLogBusiness, IOrderBusiness orderBusiness, INotificationBusiness notificationBusiness)
        {
            this.configuration = configuration;
            this.eventLogBusiness = eventLogBusiness;
            this.orderBusiness = orderBusiness;
            this.notificationBusiness = notificationBusiness;
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
            try
            {
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
                    if (result > 0)
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
    }
}
