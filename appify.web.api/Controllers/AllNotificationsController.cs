//using appify.Business.Contract;
//using appify.models;
//using FirebaseAdmin.Messaging;
//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Mvc;

//namespace appify.web.api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [EnableCors("AllOrigin")]
//    public class AllNotificationsController : Controller
//    {
//        /// <summary>
//        /// //Add Some Comments abcd efgh
//        /// </summary>
//        private readonly IConfiguration _configuration;
//        private readonly INotificationBusiness notificationBusiness;
//        private ResponseMessage rm; 
//        public AllNotificationsController(IConfiguration configuration, INotificationBusiness IResultData)
//        {
//            this._configuration = configuration;
//            this.notificationBusiness = IResultData;
//        }

//        [HttpPost,Route("PushNotification")]
//        public IActionResult PushNotification()
//        {
//            return View();
//        }

//        [HttpPost, Route("EmailNotification")]
//        public IActionResult EmailNotification(Notifications item)
//        {
//            rm = new ResponseMessage();
//            var result = notificationBusiness.SendEmail(item);
//            if(result!=null)
//            {
//                rm.statusCode = StatusCodes.OK;
//                rm.message = "Email has been successfully sent!";
//                rm.name = StatusName.ok;
//                rm.data = result;
//            }
//            else
//            {
//                rm.statusCode = StatusCodes.ERROR;
//                rm.message = "Something went wrong";
//                rm.name = StatusName.invalid;
//                rm.data = null;
//            }
//            return Ok(rm);
//        }

//        [HttpPost, Route("InAppNotification")]
//        public IActionResult InAppNotification()
//        {
//            return View();
//        }

//        [HttpPost,Route("WhatsAppNotification")]
//        public IActionResult WhatsAppNotification()
//        {
//            return View();
//        }
//    }
//}
