using Newtonsoft.Json;

namespace ASPWeb_Demo2.Models
{

    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Sesion
    {

        public Sesion() { }

        [JsonProperty("idSesion")]
        private int idSesion;

        [JsonProperty("fecha")]
        private string fecha;

        [JsonProperty("registros")]
        private List<string> registros;

        public int getIdSesion() => this.idSesion;

        public void setIdSesion(int id) => this.idSesion = id;

        public string getFecha() => this.fecha;

        public List<string> getRegistros() => this.registros;

        public void setRegistros(List<string> registros) => this.registros = registros;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

    }

}
