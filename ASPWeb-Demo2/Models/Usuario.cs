using Newtonsoft.Json;

namespace ASPWeb_Demo2.Models
{

    [Serializable]
    public class Usuario
    {

        public Usuario() { }

        public Usuario(int idUsuario, string nombre, string correo, string contrasena, string? fechaSesion) 
        {
            this.idUsuario = idUsuario;
            this.nombre = nombre;
            this.correo = correo;
            this.contrasena = contrasena;
            this.fechaSesion = fechaSesion;
        }

        public int idUsuario {  get; set; }

        public string nombre { get; set; }

        public string correo { get; set; }

        public string contrasena { get; set; }
        public string? fechaSesion {  get; set; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

    }

}
