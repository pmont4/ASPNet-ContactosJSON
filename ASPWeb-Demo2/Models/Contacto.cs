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
        private int idcontacto;

        [JsonProperty("nombre")]
        private string nombre;

        [JsonProperty("correo")]
        private string correo;

        public int getIdContacto() => this.idcontacto;

        public void setIdContacto(int id) => this.idcontacto = id;

        public string getNombre() => this.nombre;

        public void setNombre(string nombre) => this.nombre = nombre;

        public string getCorreo() => this.correo;

        public void setCorreo(string correo) => this.correo = correo;

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
