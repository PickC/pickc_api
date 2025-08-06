using appify.Business.Contract;
using appify.models;
using Newtonsoft.Json;
using System;
using System.Text;
using Twilio.Types;

namespace appify.web.api
{
    public class WhatsAppNotification
    {
        private readonly IConfiguration config;
        private readonly string watiBaseUrl;
        private readonly string watiApiKey;
        public WhatsAppNotification(IConfiguration config) { 
            this.config = config;
            watiBaseUrl = config["Wati:Url"];
            watiApiKey = config["Wati:Key"];
        }

        public static async Task<bool> SendWhatsAppNotificationMessage(Int64 TemplateID, Int64 MemberID, Int64 OrderID, string replaceTitle, INotificationBusiness notificationBusiness, string OTPValue = "")
        {
            bool result = false;
            object parameterss;
            parameterss = new object[0];
            try
            {
                string MobileNo = string.Empty;
                string Name = string.Empty;
                string AppName = string.Empty;
                string OrderNo = string.Empty;
                string Template = string.Empty;
                var watiApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Wati:Key").Value;
                var watiBaseUrl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Wati:Url").Value;
                //////SMSNotification Template
                WhatsAppTemplate whatsAppTemplate = notificationBusiness.GetWhatsAppNotificationTemplate(TemplateID);
                List<EmailNotificationHeader> getEmailNotificationHeader = notificationBusiness.GetMemberDetails(MemberID, OrderID);

                Name = getEmailNotificationHeader[0].FirstName.Trim();
                AppName = getEmailNotificationHeader[0].AppName.Trim();
                OrderNo = getEmailNotificationHeader[0].OrderNo.Trim();
                Template = whatsAppTemplate.WhatsAppTemplateID.ToString();
                /*(1001) Order Placed - To Customer
                 *(1002) Order Placed - To Vendor
                 *(1003) Order Confirm - To Customer
                 *(1006) Order Delivered - To Customer
                 *(1010) Order Cancelled - To Customer
                 *(1011) Order Cancelled - To Vendor
                 *(1020) Order Declined - To Customer

                */
                if (TemplateID == 1001 || TemplateID == 1003 || TemplateID == 1006 || TemplateID == 1010 || TemplateID == 1020) 
                {
                    parameterss = new[]
                    {
                        new { name = "name", value = Name },
                        new { name = "order_number", value = OrderNo },
                        new { name = "shop_name", value = AppName }
                    };
                }
                else if (TemplateID == 1002 || TemplateID == 1011)
                {
                    parameterss = new[]
{
                        new { name = "shop_name", value = Name },
                        new { name = "order_number", value = OrderNo }
                    };
                }

                MobileNo = @"91"+getEmailNotificationHeader[0].MobileNo.ToString();

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {watiApiKey}");
                    var payload = new
                    {
                        template_name = Template,
                        broadcast_name = Template,
                        parameters = parameterss
                    };

                    var json = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{watiBaseUrl}/sendTemplateMessage/?whatsappNumber={MobileNo}", content);

                    var rslt = await response.Content.ReadAsStringAsync();
                }

                result = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public async Task<string> SendWhatsAppMessageAsync(string phno, string templateName, string Name, string OrderNumber)////string[] parameters
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {watiApiKey}");
                var payload = new
                {
                    template_name = templateName,
                    broadcast_name = templateName,
                    parameters = new[]
                    {
                        new { name = "name", value = Name },
                        new { name = "order_number", value = OrderNumber }
                    }
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{watiBaseUrl}/sendTemplateMessage/?whatsappNumber={phno}", content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// Send Message: /api/v1/sendSessionMessage
        /// Send Template Message: /api/v1/sendTemplateMessage
        /// Upload Media: /api/v1/media/upload
        /// Get Chat History: /api/v1/getMessages/
        /// </summary>
        /// <exception cref="Exception"></exception>
        //public async Task<string> SendWhatsappMessageAsync(string phoneNumber, string message)
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {watiApiKey}");

        //    var requestBody = new
        //    {
        //        phone = phoneNumber,
        //        message = message
        //    };

        //    var response = await client.PostAsJsonAsync($"{watiBaseUrl}//sendSessionMessage", requestBody);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadAsStringAsync();
        //    }
        //    else
        //    {
        //        throw new Exception($"Failed to send message: {response.ReasonPhrase}");
        //    }
        //}

    }
}
