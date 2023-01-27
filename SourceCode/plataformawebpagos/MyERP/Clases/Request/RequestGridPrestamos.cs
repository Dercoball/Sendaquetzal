using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada para el filtrado de inversionistas en el grid 
    /// </summary>
    public class RequestGridPrestamos
    {
        public int? NoPrestamoMinimo
        { get; set; }
        public int? NoPrestamoMaximo
        { get; set; }
        public string Nombre
        { get; set; }
        public int? MontoMinimo
        { get; set; }
        public int? MontoMaximo
        { get; set; }
        public DateTime? FechaPrimerSolicitudMinimo
        { get; set; }
        public DateTime? FechaPrimerSolicitudMaximo
        { get; set; }
        public DateTime? FechaUltimaSolicitudMinimo
        { get; set; }
        public DateTime? FechaUltimaSolicitudMaximo
        { get; set; }
        public int? RechazoMinimo
        { get; set; }
        public int? RechazosMaximo
        { get; set; }
        public int? AvalMinimo
        { get; set; }
        public int? AvalMaximo
        { get; set; }
        public int? Status
        { get; set; }
    }
}