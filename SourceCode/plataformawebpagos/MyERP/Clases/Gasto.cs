using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Gasto
    {

        public int IdGasto;
        public int IdUsuario;
        public int IdEmpleado;
        
        public string Concepto;
        public string Fecha;
        public DateTime FechaDate;
        public string FechaFormateadaMx;
        public float Monto;
        public string MontoFormateadoMx;


        public string Accion;



    }
}