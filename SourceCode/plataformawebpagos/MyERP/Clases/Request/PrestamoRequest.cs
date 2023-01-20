using System.Collections.Generic;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada registrar uin prestamo
    /// </summary>
    public class PrestamoRequest
    {
        public List<Documento> DocumentosAval 
        { get; set; }
        public List<Documento> DocumentosCliente
        { get; set; }
        public Prestamo Prestamo
        { get; set; }
        public Cliente Cliente
        { get; set; }
        public Cliente Aval
        { get; set; }
    }
}