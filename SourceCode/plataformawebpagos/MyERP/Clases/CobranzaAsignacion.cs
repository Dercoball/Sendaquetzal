using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Asignación de una cuenta (préstamo) a un gestor de cobranza.
    /// La asignación la realiza únicamente gerencia de cobranza.
    /// </summary>
    public class CobranzaAsignacion
    {
        public int IdCobranzaAsignacion;
        public int IdPrestamo;
        public int IdGestor;
        public int? IdUsuarioAsigna;
        public DateTime? FechaAsignacion;
        public string Observaciones;
        public int Activo;
        public int Eliminado;
    }
}
