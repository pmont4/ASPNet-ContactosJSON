using ASPWeb_Demo2.Models;
using Newtonsoft.Json;
using System.Net;

namespace ASPWeb_Demo2.Util
{
    public class JsonUtils
    {

        public JsonUtils(string jsonfile_url) 
        {
            this.jsonfile_url = jsonfile_url;
        }

        private string jsonfile_url;

        public string getJsonFile_Url() => this.jsonfile_url;

        public void setJsonFile_Url(string jsonfile_url) => this.jsonfile_url = jsonfile_url;

        public bool updateJson<T>(T toWrite)
        {
            if (toWrite != null)
            {
                File.WriteAllText(this.getJsonFile_Url(), string.Empty);

                string json = this.serializeObjectToJson(toWrite);
                using (StreamWriter writer = File.AppendText(this.getJsonFile_Url()))
                {
                    writer.Write(json);
                    return true;
                }
            }
            return false;
        }

        public T deserealizeObjectFromJsonFile<T>()
        {
            WebRequest request = WebRequest.Create(this.getJsonFile_Url());
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
