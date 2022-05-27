using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Prestamo
    {

        public int IdPrestamo;
        public string IdCliente;

        public string FechaSolicitud;
        public float Monto;

        public int IdStatusPrestamo;

        public Cliente Cliente;

        public int Activo;
        public string ActivoStr;


        public string Accion;

    }
}