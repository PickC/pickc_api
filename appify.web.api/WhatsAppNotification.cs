using Newtonsoft.Json;
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

        public async Task<string> SendWhatsAppMessageAsync(string phno, string templateName, string[] parameters)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {watiApiKey}");

                var payload = new
                {
                    template_name = templateName,
                    broadcast_name = "Order Updates",
                    parameters = parameters.Select(p => new { name = "var", value = p }).ToArray(),
                    phone_number = phno
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{watiBaseUrl}/sendTemplateMessage", content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
