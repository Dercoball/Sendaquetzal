using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerifyStatusPaymentsService
{
    public class Pago
    {

        public int IdPago;
        public int IdPrestamo;
        public int IdCliente;
        public int IdUsuario;
        public int NumeroSemana;
        public int NumeroSemanas;
        public Cliente cliente;
        public double Monto;
        public float Pagado; // Lo actual abonado
        public double Saldo;
        public string MontoFormateadoMx;
        public string SaldoFormateadoMx;
        public int IdStatusPago;
        public string FechaStr;
        public DateTime Fecha;
        public string FechaRegistroPago;

        public string NombreCliente;
        public string Status;
        public string Color;
        public string SemanasFalla; //  concatenadas -> 3,4
        public string Accion;

        public float TotalFalla;
        public string TotalFallaFormateadoMx;


        public const int STATUS_PAGO_PENDIENTE = 1;
        public const int STATUS_PAGO_FALLA = 2;
        public const int STATUS_PAGO_ABONADO = 3;
        public const int STATUS_PAGO_PAGADO = 4;

    }
}