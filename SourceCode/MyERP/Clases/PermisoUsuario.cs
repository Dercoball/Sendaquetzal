using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class PermisoUsuario
    {
        public int IdPermiso;
        public string Nombre;//Nombre completo
        public string NombreInterno;//nombre sin espacios
        public string TipoPermiso;
        public string IdUsuario;
        public string IdTipoUsuario;

        public string Accion;

        public const string TIPO_PERMISO_WEB = "4";
        public const string TIPO_PERMISO_CATALOGO = "2";
        public const string TIPO_PERMISO_BOTON = "1";
        public const string TIPO_PERMISO_PANEL = "3";

    }
}