using System;
using System.Runtime.CompilerServices;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada para el filtrado de inversionistas en el grid 
    /// </summary>
    public class RequestGridPrestamos
    {
        public int id_prestamo
        { get; set; }
        public string nombreCliente
        { get; set; }
        public float monto
        { get; set; }
        public DateTime? fecha_primera_solicitud
        { get; set; }
        public DateTime? fecha_ultima_solicitud
        { get; set; }
        public int NoRechazados 
        { get; set; }
        public string Aval
        { get; set; }
        public string Status
        { get; set; }
    }
}