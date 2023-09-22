using Newtonsoft.Json;

namespace ASPWeb_Demo2.Models
{
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Contacto
    {

        public Contacto() { }
        public Contacto(int idcontacto, string nombre, string correo) 
        {
            this.idcontacto = idcontacto;
            this.nombre = nombre;
            this.correo = correo;
        }

        [JsonProperty("idContacto")]
        public int idcontacto { get; set; }

        [JsonProperty("nombre")]
        public string nombre { get; set; }

        [JsonProperty("correo")]
        public string correo { get; set; }

        public override bool Equals(object obj)
        { 
            return base.Equals(obj);    
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
