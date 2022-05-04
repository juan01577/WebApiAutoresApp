namespace WebApiAutores.DTOs
{
    public class DatosHATEOAS
    {
        public string Enlace { get; private set; } //private permite crear el dato y no modificarlo
        public string Descripcion { get; private set; }
        public string Metodo { get; private set; }

        public DatosHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Descripcion = descripcion;
            Metodo = metodo;
        }

        
    }
}
