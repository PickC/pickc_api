using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace appify.utility
{
    public static class Common
    {
        public static string ConvertObjectToJson(object obj)
        {
            // Serialize the object to JSON
            string json = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static T ConvertJsonToObject<T>(string json)
        {
            // Deserialize JSON to object
            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public static readonly string IMAGECLASSIFIER_URL = "https://appify-image-classifier.azurewebsites.net/classify-image/";

        public static readonly int IMAGE_SIZE = 5 * 1024 * 1024; /// 5 MB

    }
}
