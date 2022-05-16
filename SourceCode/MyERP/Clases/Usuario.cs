using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Usuario
    {
        public int Id_Usuario;
        public int IdUsuario;
        public int IdTipoUsuario;
        public int IdProveedor;        
        public int IdEmpleado;
        public string Nombre;
        public string NombreTipoUsuario;
        public string Login;
        public string Password;
        public string Email;
        public string Telefono;
        public string Accion;
        public string Msg;
        public string Token;

        public float TotalDias;
        public float Factor;

        public const int TIPO_USUARIO_SUPER_ADMIN = 1;
        public const int TIPO_USUARIO_PROVEEDOR = 2;
        public const int TIPO_USUARIO_ADMIN = 3;
        public const int TIPO_USUARIO_DEPTO_OPERACIONES = 4;
        public const int TIPO_USUARIO_DEPTO_MANTENIMIENTO = 5;


    }

}