using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada para el filtrado de inversionistas en el grid 
    /// </summary>
    public class ResponseGridEmpleados
    {
        public string NombreCompleto
        { get; set; }
        public string Usuario
        { get; set; }
        public int? IdModulo
        { get; set; }
        public int? IdTipo
        { get; set; }
        public int? IdPlaza
        { get; set; }
        public int? Activo
        { get; set; }
        public DateTime? FechaIngreso
        { get; set; }
        public string NombreEjecutivo
        { get; set; }
        public string NombreSupervisor
        { get; set; }
    }
}