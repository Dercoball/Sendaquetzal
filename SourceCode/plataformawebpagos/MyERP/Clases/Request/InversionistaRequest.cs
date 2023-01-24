using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada para el filtrado de inversionistas en el grid 
    /// </summary>
    public class InversionistaRequest
    {
        public string NombreBusqueda
        { get; set; }
        public string RFCBusqueda
        { get; set; }
        public float? UtilidadMaximaBusqueda
        { get; set; }
        public DateTime? FechaRegistroMaximaBusqueda
        { get; set; }
        public float? UtilidadMinimaBusqueda
        { get; set; }
        public DateTime? FechaRegistroMinimaBusqueda
        { get; set; }
    }
}