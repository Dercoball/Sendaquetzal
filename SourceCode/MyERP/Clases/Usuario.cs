using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Usuario
    {
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

        public const int TIPO_USUARIO_SUPER_ADMIN = 6;
        

    }

}