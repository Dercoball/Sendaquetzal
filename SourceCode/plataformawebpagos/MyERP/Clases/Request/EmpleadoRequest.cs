using System;
using System.Collections.Generic;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase proporcionada para la creacion de nuevos colaboradores
    /// </summary>
    public class EmpleadoRequest
    {
        public Empleado Colaborador
        { get; set; }
        public Cliente Aval
        { get; set; }
        public List<Documento> DocumentosAval
        { get; set; }
        public List<Documento> DocumentosColaborador
        { get; set; }
        public Usuario User
        { get; set; }
    }
}