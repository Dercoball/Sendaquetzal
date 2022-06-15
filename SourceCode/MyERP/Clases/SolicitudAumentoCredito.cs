using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class SolicitudAumentoCredito
    {

        public int IdSolicitudAumentoCredito;
        public int IdStatusSolicitudAumentoCredito;
        public int IdPrestamo;
        public int IdUsuario;
        public int IdDirector; //   id de empleado del director que aprueba/rechaza

        public string FechaFormateadaMx;

        public string NombrePromotor;
        public string NombreSupervisor;
        
        public float LimiteCreditoActual;
        public string LimiteCreditoActualMx;

        public float LimiteCreditoRequerido;
        public string LimiteCreditoRequeridoMx;

        public string NombrePlaza;

        public string Accion;

        public const int STATUS_SOLICITUD_CREADA = 1;
        public const int STATUS_SOLICITUD_RECHAZADA = 2;
        public const int STATUS_SOLICITUD_APROBADA = 3;

    }
}