using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class CompraCombustible: Equipo
    {

        public int IdCompraCombustible;
        public DateTime Fecha;
        public string FechaFormateada;//
        public string FechaCanonical;//
        public string FechaFormateadaMx;//
        public string HoraCreacion;
        public float Cantidad;
        public string NombreProveedor;
        public string IdProveedor;



        public int IdEquipo;
        public int IdCliente;
        public int IdUsuario;
        public int IdUsuarioSolicita;
        public int IdUsuarioEntrega;



    }
}