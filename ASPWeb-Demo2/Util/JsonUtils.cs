using Newtonsoft.Json;
using System.Net;

namespace ASPWeb_Demo2.Util
{
    public class JsonUtils
    {

        public static readonly string USER_FILE_LINK = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\usuarios.json";
        public static readonly string CONTACT_FILE_LINK = @"C:\Users\EJRKC\source\repos\ASPWeb-Demo2\ASPWeb-Demo2\json\contactos.json";

        public JsonUtils() {}

        public bool updateJson<T>(string url, T toWrite)
        {
            if (toWrite != null)
            {
                File.WriteAllText(url, string.Empty);

                string json = this.serializeObjectToJson(toWrite);
                using (StreamWriter writer = File.AppendText(url))
                {
                    writer.Write(json);
                    return true;
                }
            }
            return false;
        }

        public T deserealizeObjectFromJsonFile<T>(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            using (Stream data = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    string responseServer = reader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(responseServer);
                }
            }
        }

        public string? serializeObjectToJson(object? toSerialize) => JsonConvert.SerializeObject(toSerialize, Formatting.Indented);

    }
}
