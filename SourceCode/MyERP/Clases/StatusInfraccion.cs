using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class StatusInfraccion
    {


        public int idStatusInfraccion;
        public String nombre;


        public Boolean activo;
        public string activoStr;
        public Boolean eliminado;
        public string ultimaModificacionStr;



        public string accion;

        public const string RECIBIDA = "1";// "Recibida";
        public const string DESCARGO_PENDIENTE = "2";//"Descargo pendiente";
        public const string ABSUELTO = "3";//"Absuelto";
        public const string ENTIENDE_PAGO_VOLUNTARIO = "4";//"Extiende pago voluntario";
        public const string CONDENADO_PAGO_VOLUNTARIO = "5";//"Condenado a pago voluntario";
        public const string CONDENADO_PAGO_TOTAL = "6";//"Condenado a pago total";


    }
}