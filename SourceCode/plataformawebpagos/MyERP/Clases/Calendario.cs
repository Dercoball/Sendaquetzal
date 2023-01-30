using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Calendario
    {

        public int Id;
        public string Nombre;

        public DateTime Fecha;
        public DateTime FechaFinal;

        public bool EsLaboral;
		public string Estatus;
        public string FechaMx;
        public string FechaLarga;
        
        //public string FechaFinal;
        public string Tipo;

        public string Dia;
        public string Mes;
        public string Anio;

        public string Accion;

    }
}