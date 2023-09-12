using Newtonsoft.Json;

namespace ASPWeb_Demo2.Models
{
    [Serializable]
    public class Contacto
    {

        public Contacto() { }
        public Contacto(int idcontacto, string nombre, string correo) 
        {
            this.idcontacto = idcontacto;
            this.nombre = nombre;
            this.correo = correo;
        }

        public int idcontacto { get; set; }

        public string nombre { get; set; }

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
