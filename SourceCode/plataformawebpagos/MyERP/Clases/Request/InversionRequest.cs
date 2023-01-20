using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada para el filtrado de inversionistas en el grid 
    /// </summary>
    public class InversionRequest
    {
        public int? IdInversionista
        { get; set; }
        public string NombreInversionista
        { get; set; }
        public float? MontoMinimo
        { get; set; }
        public float? MontoMaximo
        { get; set; }
        public float? UtilidadMinimo
        { get; set; }
        public float? UtilidadMaximo
        { get; set; }
        public int? Estatus
        { get; set; }
        public int? PlazoMinimo
        { get; set; }
        public int? PlazoMaximo
        { get; set; }
        public DateTime? IngresoMinimo
        { get; set; }
        public DateTime? IngresoMaximo
        { get; set; }
        public DateTime? RetiroMinimo
        { get; set; }
        public DateTime? RetiroMaximo
        { get; set; }
    }
}