namespace ASPWeb_Demo2.Models
{

    [Serializable]
    public class Registro
    {

        private int idRegistro;
        private string texto;
        private string fecha;

        public Registro() { }

        public Registro(int idRegistro, string texto, string fecha)
        {
            this.idRegistro = idRegistro;
            this.texto = texto;
            this.fecha = fecha;
        }

        public int getIdRegistro() => this.idRegistro;

        public void setIdRegistro(int id)
        {
            this.idRegistro = id;
        }

        public string getTexto() => this.texto;

        public void setTexto(string texto) 
        { 
            this.texto = texto; 
        }

        public string getFecha() => this.fecha;

        public void setFecha(string fecha)
        {
            this.fecha = fecha;
        }

    }
}
