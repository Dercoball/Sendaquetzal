using System;

namespace Plataforma.Clases
{
    public class Prestamo
    {
        public int IdTipoCliente
        { get; set; }
        public int IdPrestamo
        { get; set; }
        public int IdAval
        { get; set; }
        public string IdCliente
        { get; set; }
        public int? IdEmpleado
        { get; set; }
        public string FechaSolicitud 
        { get; set; }
        public DateTime FechaSolicitudDate
        { get; set; }
        public float Monto
        { get; set; }
        public float MontoPorRenovacion
        { get; set; }
        public int IdStatusPrestamo
        { get; set; }
        public Cliente Cliente
        { get; set; }
        public int Activo
        { get; set; }
        public string ActivoStr
        { get; set; }
        public string NotasGenerales
        { get; set; }
        public string NotasEjecutivo
        { get; set; }
        public string Color
        { get; set; }
        public string NombreStatus
        { get; set; }
        public string UbicacionConfirmada
        { get; set; }
        public int? idUsuario
        { get; set; }
        public string Accion
        { get; set; }

        public string MontoFormateadoMx;
        public float Saldo;             //Deuda actual de este prestamo
        public string SaldoFormateadoMx;

        public const int STATUS_PENDIENTE = 1;
        public const int STATUS_PENDIENTE_EJECUTIVO = 2;
        public const int STATUS_RECHAZADO = 3;
        public const int STATUS_APROBADO = 4;
        public const int STATUS_PAGADO = 5;
    }
}