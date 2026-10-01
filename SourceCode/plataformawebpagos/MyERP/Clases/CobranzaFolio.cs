namespace Plataforma.Clases
{
    /// <summary>
    /// Folio de cobranza. Los folios se asignan por blocks desde gerencia
    /// y se ligan a un cobro cuando se usan.
    /// </summary>
    public class CobranzaFolio
    {
        public int IdFolio;
        public string Folio;
        public int IdGestor;
        public string Gestor;
        public int Estatus;          // 1 disponible, 2 usado, 3 cancelado
        public string EstatusStr;
        public string FechaAsignacion;
        public string FechaUso;

        public const int ESTATUS_DISPONIBLE = 1;
        public const int ESTATUS_USADO = 2;
        public const int ESTATUS_CANCELADO = 3;
    }
}
