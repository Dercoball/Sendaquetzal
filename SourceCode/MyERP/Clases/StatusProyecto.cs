using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class StatusProyecto
    {

        public const int STATUS_ACTIVO = 1; 
        public const int STATUS_CERRADO = 2; 
        public const int STATUS_ENTREGADO = 3;

        public int IdStatusProyecto;
        public String Nombre;
        

        public string Accion;



    }
}