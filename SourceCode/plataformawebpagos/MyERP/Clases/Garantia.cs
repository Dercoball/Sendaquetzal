using System;

namespace Plataforma.Clases
{
    public class Garantia
    {
        public int id_garantia_prestamo
        { get; set; }
        public int id_prestamo
        { get; set; }
        public string nombre
        { get; set; }
        public string numero_serie
        { get; set; }
        public float costo
        { get; set; }
        public string fotografia
        { get; set; }
        public DateTime fecha_registro
        { get; set; }
        public int eliminado
        { get; set; }
        public int id_usuario
        { get; set; }
        //public int IdGarantia;
        //public string Nombre;

        //public string NumeroSerie;
        //public float Costo;
        //public string CostoFormateadoMx;
        //public string Fotografia;
        //public string Fecha;
        //public string Imagen;


        //public string Accion;

    }
}