namespace Plataforma.Clases
{
    /// <summary>
    /// Regla de comisión de cobranza por BUCKET (semanas de vencimiento).
    /// Bucket 1: sem 17-19 = 15% | Bucket 2: 20-23 = 20% | Bucket 3: 24+ = 25%
    /// (configurable en la tabla cobranza_regla_comision).
    /// </summary>
    public class CobranzaReglaComision
    {
        public int IdRegla;
        public int Bucket;
        public int SemanaDesde;
        public int SemanaHasta;   // 9999 = en adelante
        public float Porcentaje;  // fracción: 0.15 = 15%
        public int Activo;
    }
}
