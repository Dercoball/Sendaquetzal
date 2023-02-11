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
        public int IdCliente;
        public int IdUsuario;
        public int NumeroSemana;
        public int NumeroSemanas;
        public int SemanaExtra;
        public int Mensaje;

        public double MontoPrestamo;
        public double Monto;
        public float Pagado; // Lo actual abonado
        public double Saldo;
        public string MontoPrestamoFormateadoMx;
        public string MontoFormateadoMx;
        public string SaldoFormateadoMx;
        public int IdStatusPago;
        public string FechaStr;
        public DateTime Fecha;
        public string FechaRegistroPago;
        public Nullable<DateTime> FechaUltimoPago;

        public string NombreCliente;
        public string CalleCliente;
        public string TelefonoCliente;
        public string NombreAval;
        public string TelefonoAval;
        public string CalleAval;
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