using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Pago
    {

        public int IdPago;
        public int IdPrestamo;
        public int IdUsuario;
        public int NumeroSemana;

        public float Monto;
        public float Saldo;
        public string MontoFormateadoMx;
        public string SaldoFormateadoMx;
        public int IdStatusPago;        
        public string FechaStr;
        public DateTime Fecha;
        public string FechaRegistroPago;

        public const int STATUS_PAGO_PENDIENTE = 1;

    }
}