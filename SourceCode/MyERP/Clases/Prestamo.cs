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
        public List<RelPrestamoAprobacion> listaRelPrestamoAprobacion;

        public string FechaSolicitud;
        public DateTime FechaSolicitudDate;
        public float Monto;
        public string MontoFormateadoMx;

        public int IdStatusPrestamo;

        public Cliente Cliente;

        public int Activo;
        public string ActivoStr;
        public string Color;    //
        public string NombreStatus;



        public string Accion;

        public const int STATUS_PENDIENTE = 1;
        public const int STATUS_PENDIENTE_EJECUTIVO = 2;
        public const int STATUS_RECHAZADO = 3;
        public const int STATUS_APROBADO = 4;
        public const int STATUS_PAGADO = 5;

    }
}