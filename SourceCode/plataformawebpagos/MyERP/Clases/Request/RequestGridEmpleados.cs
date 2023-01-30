using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada para el filtrado de empleados en el grid 
    /// </summary>
    public class RequestGridEmpleados
    {
        public int IdEmpleado
        { get; set; }
        public string NombreCompleto
        { get; set; }
        public string Usuario
        { get; set; }
        public string Modulo
        { get; set; }
        public string Tipo
        { get; set; }
        public string Plaza
        { get; set; }
        public int Activo
        { get; set; }
        public DateTime FechaIngreso
        { get; set; }
        public string NombreEjecutivo
        { get; set; }
        public string NombreSupervisor
        { get; set; }
    }
}