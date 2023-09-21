using Newtonsoft.Json;

namespace ASPWeb_Demo2.Models
{

    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Sesion
    {

        public Sesion() { }
        public Sesion(int idSesion, string fecha, List<Registro>? registros)
        {
            this.idSesion = idSesion;
            this.fecha = fecha;
            this.registros = registros;
        }

        [JsonProperty("idSesion")]
        private int idSesion;

        [JsonProperty("fecha")]
        private string fecha;

        [JsonProperty("registros")]
        private List<Registro>? registros;

        public int getIdSesion() => this.idSesion;

        public void setIdSesion(int id) => this.idSesion = id;

        public string getFecha() => this.fecha;

        public void setFecha(string fecha) => this.fecha = fecha;

        public List<Registro>? getRegistros() => this.registros;

        public void setRegistros(List<Registro>? registros) => this.registros = registros;

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

    }

}
