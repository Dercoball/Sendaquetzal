using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class PermisoUsuario
    {
        public int IdPermiso;
        public string Nombre;//Nombre completo todo el path
        public string NombreInterno;//todo el path + extension
        public string NombreRecurso;//Solo el nombre
        public string TipoPermiso;
        public string IdUsuario;
        public string IdTipoUsuario;

        public string Accion;

        public const string TIPO_PERMISO_WEB = "4";
        public const string TIPO_PERMISO_LIBRE = "6";//libre
        public const string TIPO_PERMISO_PRESTAMOS = "7";        
        public const string TIPO_PERMISO_COMISIONES = "8";
        public const string TIPO_PERMISO_REPORTES = "10";
        public const string TIPO_PERMISO_CONFIGURACION = "2";
        public const string TIPO_PERMISO_BOTON = "1";
        public const string TIPO_PERMISO_PANEL = "3";
        public const string TIPO_PERMISO_INVERSIONISTAS = "11";
        public const string TIPO_PERMISO_ACTIVOS = "12";



    }
}