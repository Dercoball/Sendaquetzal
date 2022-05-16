using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Infraccion
    {

        public int idInfraccion;
        public int idStatusInfraccion;
        public String apellidos;
        public String nombre;
        public String cuit;
        public String patente;
        public String dominio;
        public String numero_notificacion;
        public string accion;
        public DateTime infraccionVto1;
        public string infraccionVto1Str = null;
        public string infraccionVto1Canonical = null;
        public DateTime infraccionVto2;
        public string infraccionVto2Str = null;
        public string infraccionVto2Canonical = null;


    }
}