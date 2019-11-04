using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Bb.Http.Helpers
{

    internal static class ContentExtension
    {

        public static HttpContent Serialize(this object self, string contentType = null)
        {

            string payload;

            if (self is string s)
                payload = s;
            else
                payload = Newtonsoft.Json.JsonConvert.SerializeObject(self);

            if (contentType == null)
            {
                try
                {
                    if ((payload.StartsWith("{") && payload.EndsWith("}")) || (payload.StartsWith("[") && payload.EndsWith("]")))
                    {
                        JObject.Parse(payload);
                        contentType = Application.Json;
                    }
                }
                catch (System.Exception)
                {
                    contentType = Application.Octet;
                }
            }

            HttpContent c = new StringContent(payload, Encoding.UTF8, contentType);
            return c;
        
        }

        public static void AddHeader(this HttpRequestHeaders self, Dictionary<string, object> dictionary)
        {

            foreach (var item in dictionary)
                if (item.Value != null)
                    self.Add(item.Key, item.Value.ToString());

        }


    }


}
