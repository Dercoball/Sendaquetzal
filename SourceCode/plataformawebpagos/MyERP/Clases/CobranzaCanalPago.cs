namespace Plataforma.Clases
{
    /// <summary>
    /// Canal por el que ingresa un cobro de cobranza
    /// (cobro en campo, fallo, recuperado, depósito, folio, oficina).
    /// </summary>
    public class CobranzaCanalPago
    {
        public int IdCanalPago;
        public string Nombre;
        public int NoTangible;   // 1 = fallo/depósito/oficina (no efectivo en mano del gestor)
    }
}
