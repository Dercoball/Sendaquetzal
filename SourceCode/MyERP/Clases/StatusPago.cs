using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class StatusPago
    {


        public int IdStatusPago;
        public String Nombre;


        public Boolean Activo;
        public string ActivoStr;
        public Boolean Eliminado;
        public string UltimaModificacionStr;



        public string Accion;

        public const string STATUSPAGO_PENDIENTE = "1";
        public const string STATUSPAGO_PAGADO = "2";
        public const string STATUSPAGO_BAJA = "3";
        public const string STATUSPAGO_ABSUELTO = "4";
        public const string STATUSPAGO_CONDENADO = "5";



    }
}