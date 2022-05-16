using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class SolicitudCombustible: Equipo
    {

        public int IdSolicitud;
        public int IdDetalleSolicitud;
        //public DateTime Fecha;
        //public DateTime FechaHoraEntrega;
        public string FechaFormateada;//
        public string FechaCanonical;//
        public string FechaFormateadaMx;//
        
        public string FechaSolicitudFormateadaMx;//
        public string FechaEntregaFormateadaMx;//

        public string HoraCreacion;
        public string NombreUsuarioSolicita;
        public string NombreUsuarioEntrega;
        public string CintilloAnterior;
        public string CintilloActual;
        public float Cantidad;
        public float CantidadSurtida;
        public string NombreStatus;
        public string NombreCentroCosto;
        public string NombreProveedor;



        public int Cancelado;
        public int IdStatus;
        public int IdObra;
        public int IdTipoCombustible;
        public int IdTipoSolicitudCombustible; //   1 general, 2 individual
        public string NombreTipoCombustible;
        public string NombreTipoSolicitudCombustible;
        public int IdCliente;
        public int IdUsuarioSolicita;
        public int IdUsuarioEntrega;
        public int Entregado;
        public string EntregadoStr;



    }
}