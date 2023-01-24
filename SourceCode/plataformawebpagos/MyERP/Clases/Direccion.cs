using System;

namespace Plataforma.Clases
{
    public class Direccion
    {
        public int idDireccion
        { get; set; }
        public int? IdEmpleado
        { get; set; }
        public int? IdCliente
        { get; set; }

        public int? Aval
        { get; set; }
        public String Calle
        { get; set; }
        public String Colonia
        { get; set; }
        public String Municipio
        { get; set; }
        public String Estado
        { get; set; }

        public String CodigoPostal
        { get; set; }
        public int Activo { get; set; }
        public string Accion { get; set; }
        public string ActivoStr { get; set; }
        public string DireccionTrabajo { get; set; }
        public string Ubicacion { get; set; }
    }
}