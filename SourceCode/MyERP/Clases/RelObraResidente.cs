using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class RelObraResidente
    {


        public int Id;
        public int IdResidente;
        public int IdPrenomina;
        public int IdObra;
        public DateTime FechaAsignacion;
        public string FechaAsignacionStr;
        public string FechaInicio;
        public string FechaFin;

        public string FechaUltimaModificacion;
        public string FechaUltimaModificacionStr;

        public int Activo;
        
        public string ClaveObra;
        public string NombreObra;
        public string NombreEmpleado;
        public string Usuario;
        public string NombreStatus;
        public string ActivoStr;
        public string Accion;

    }
}