using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Descargo : Infraccion
    {

        public int idDescargo;
        public int idStatusDescargo;
        public int idTipoDescargo;
        public bool activo;
        public String descripcionDescargo;
        public string tipoDescargo;
        public string statusDescargo;
        public string ultimaModificacionStr;


        public const string PENDIENTE = "1";
        public const string CANCELADO = "2";
        public const string FINALIZADO = "3";

    }
}