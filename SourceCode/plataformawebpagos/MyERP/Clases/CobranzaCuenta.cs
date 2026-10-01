using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Renglón de la Cartera de Cobranza (Bloque A del diagrama).
    /// Una "cuenta" es un préstamo que ya cayó a cobranza
    /// (semanas transcurridas desde el inicio >= parámetro semana_caida_cobranza
    /// y con saldo pendiente).
    /// </summary>
    public class CobranzaCuenta
    {
        public int IdPrestamo;
        public int IdCliente;

        // 8.- Fecha en que se otorgó el crédito
        public string FechaCredito;

        // 1..7 datos de plaza / jerarquía / producto
        public int IdGestor;
        public string Gestor;          // 1.- gestor asignado
        public string Plaza;           // 2.- plaza
        public string Coordinador;     // 3.- coordinador de plaza
        public string Supervisor;      // 4.- supervisor de plaza
        public string Ejecutivo;       // 5.- ejecutivo de la plaza
        public string Promotor;        // promotor que colocó el crédito (prestamo.id_empleado)
        public string Producto;        // 6.- producto (tipo_cliente)

        // 9.- datos del cliente
        public string NombreCliente;
        public string CalleCliente;
        public string ColoniaCliente;
        public string TelefonoCliente;

        // 10.- primer aval
        public string NombreAval1;
        public string CalleAval1;
        public string ColoniaAval1;
        public string TelefonoAval1;

        // 11.- segundo aval
        public string NombreAval2;
        public string CalleAval2;
        public string ColoniaAval2;
        public string TelefonoAval2;

        // 12.- monto del pago semanal
        public double PagoSemanal;
        public string PagoSemanalMx;

        // vencimiento / bucket
        public int SemanasTranscurridas;   // desde el inicio del crédito
        public int Bucket;                 // 1, 2, 3 (segun cobranza_regla_comision)

        // 14.- suma de pagos pendientes (falla completa/parcial)
        public double Adeudo;
        public string AdeudoMx;

        // 15.- suma de multas
        public int NumeroFallas;
        public double Multas;
        public string MultasMx;

        // 16.- suma de 14 + 15
        public double Subtotal;
        public string SubtotalMx;

        // 17.- 10% de gasto de cobranza sobre el punto 16
        public double GastoCobranza;
        public string GastoCobranzaMx;

        // 18.- resultado final (14 + 15 + 17)  (POR CONFIRMAR: suma vs resta)
        public double TotalCobranza;
        public string TotalCobranzaMx;

        // 19.- saldo actualizado (refleja pagos hechos a partir de la caída)
        public double SaldoActualizado;
        public string SaldoActualizadoMx;

        // 20.- observaciones (editable solo por gerencia)
        public string Observaciones;

        public string Accion;
    }
}
