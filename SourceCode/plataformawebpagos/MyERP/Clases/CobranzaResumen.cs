namespace Plataforma.Clases
{
    /// <summary>
    /// Resumen económico de cobranza por gestor / semana (Bloque C del diagrama).
    /// </summary>
    public class CobranzaResumen
    {
        public int IdGestor;
        public string Gestor;

        // 1.- suma total de lo cobrado por el gestor
        public double TotalCobrado;
        public string TotalCobradoMx;

        // 2.- cobranza no tangible (fallos, depósitos, pagos en oficina)
        public double NoTangible;
        public string NoTangibleMx;

        // 3.- suma total de comisiones de la semana
        public double Comisiones;
        public string ComisionesMx;

        // 4.- diferencia = total cobrado - no tangible - comisiones (puede ser +/-)
        public double Diferencia;
        public string DiferenciaMx;
    }
}
