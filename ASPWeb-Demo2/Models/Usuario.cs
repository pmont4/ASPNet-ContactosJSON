using Newtonsoft.Json;

namespace ASPWeb_Demo2.Models
{

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    public class Usuario
    {

        public Usuario() { }

        public Usuario(int idUsuario, string nombre, string correo, string contrasena, string? ipv4, List<Sesion>? sesiones) 
        {
            this.idUsuario = idUsuario;
            this.nombre = nombre;
            this.correo = correo;
            this.contrasena = contrasena;
            this.ipv4 = ipv4;
            this.sesiones = sesiones;
        }

        [JsonProperty("idUsuario")]
        private int idUsuario;

        [JsonProperty("nombre")]
        private string nombre;

        [JsonProperty("correo")]
        private string correo;

        [JsonProperty("contrasena")]
        private string contrasena;

        [JsonProperty("ipv4")]
        private string? ipv4;

        [JsonProperty("sesiones")]
        private List<Sesion>? sesiones;

        public int getIdUsuario() => this.idUsuario;

        public void setIdUsuario(int id) => this.idUsuario = id;

        public string getNombre() => this.nombre;

        public void setNombre(string nombre) => this.nombre = nombre;

        public string getContrasena() => this.contrasena;

        public void setContrasena(string contrasena) => this.contrasena = contrasena;

        public string getCorreo() => this.correo;

        public void setCorreo(string correo) => this.correo = correo;

        public string? getIpv4() => this.ipv4;

        public void setIpv4(string ipv4) => this.ipv4 = ipv4;

        public List<Sesion>? getSesiones() => this.sesiones;

        public void setSesiones(List<Sesion>? sesiones) => this.sesiones = sesiones;

    }

}
