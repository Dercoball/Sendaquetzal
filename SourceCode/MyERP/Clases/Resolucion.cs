using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Resolucion : Descargo
    {

        public int idResolucion;
        //public int idInfraccion;
        public int idStatusResolucion;
        public int idTipoResolucion;
        //public int idDescargo;
        public String texto1;
        public String texto2;
        public String texto3;
        //public Boolean activo;
        public string activoStr;
        public Boolean eliminado;
        //public string ultimaModificacionStr;
        //public string accion;
        public DateTime fechaPago;
        public string fechaPagoStr;

        public String statusResolucion;
        public String tipoResolucion;

        public const string STATUSRESOLUCION_ACEPTA_DESCARGO = "1";
        public const string STATUSRESOLUCION_EXTENDER_FECHA = "2";
        public const string STATUSRESOLUCION_RECHAZAR_DESCARGO = "3";
        public const string STATUSRESOLUCION_RECHAZAR_DESCARGO_ANULARPAGOVOLUNTARIO = "4";


    }
}