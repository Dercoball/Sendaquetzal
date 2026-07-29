namespace Plataforma.Clases
{
    /// <summary>
    /// Renglón del Reporte semanal de cobranza (Bloque B del diagrama).
    /// Cada registro es un cobro/recuperación hecho por un gestor.
    /// </summary>
    public class CobranzaCobro
    {
        public int IdCobranzaCobro;
        public int IdPrestamo;

        // 1.- periodo / semana de reporte (corte lunes-domingo)
        public int NumeroSemanaReporte;
        public string PeriodoReporte;

        // 2-3.- gestor
        public int IdGestor;
        public string Gestor;
        public string UsuarioGestor;

        // 4.- fecha de inicio del crédito
        public string FechaInicioCredito;

        // 5.- fecha en que se realiza el pago a cobranza
        public string FechaCobro;

        // 6.- plaza y grupo
        public string Plaza;
        public string Grupo;   // POR CONFIRMAR: no existe entidad "grupo" en el modelo

        // 7.- cliente
        public string NombreCliente;

        // 8.- cantidad recuperada
        public double MontoRecuperado;
        public string MontoRecuperadoMx;

        // 9.- observación
        public string Observaciones;

        // 10.- canal de pago
        public int IdCanalPago;
        public string CanalPago;

        // 11.- folio de cobranza
        public string Folio;

        // 12-13.- % comisión y bucket
        public double PorcentajeComision;
        public string PorcentajeComisionStr;
        public int Bucket;

        // 14.- semanas en vencimiento desde el inicio
        public int SemanasVencimiento;

        // 15.- comisión económica
        public double Comision;
        public string ComisionMx;
    }
}
