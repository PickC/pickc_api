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

        public async Task<string> SendWhatsAppMessageAsync(string phno, string templateName, string Name, string OrderNumber)////string[] parameters
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {watiApiKey}");

                //var payload = new
                //{
                //    template_name = templateName,
                //    broadcast_name = "Order Updates",
                //    parameters = parameters.Select(p => new { name = "var", value = p }).ToArray(),
                //    whatsappNumber = phno
                //};

                //this is FormatException to send on multiple numbers
                //var payload = new
                //{
                //    template_name = "order_place",
                //    broadcast_name = "order_place",
                //    receivers = new[]
                //    {
                //        new
                //        {
                //            whatsappNumber = "919810722979",
                //            customParams = new[]
                //            {
                //                new { name = "name", value = "John" },
                //                new { name = "ordernumber", value = "OD10602504053" }
                //            }
                //        }
                //    }
                //};

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
