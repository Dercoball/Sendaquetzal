namespace Plataforma.Clases
{
    public class TipoCliente
    {
        public int IdTipoCliente 
        { 
            get; set;
        }
        public string NombreTipoCliente 
        {
            get; set; 
        }
        public float PrestamoInicialMaximo 
        { 
            get; set; 
        }
        public float PorcentajeSemanal 
        {
            get; set; 
        }
        public int SemanasAPrestar
        {
            get; set;
        }
        public float GarantiasPorMonto 
        {
            get; set; 
        }
        public string FechasDePago 
        {
            get; set; 
        }
        public int FechaPagoLunes 
        {
            get; set;
        }
        public int FechaPagoMartes 
        {
            get; set;
        }
        public int FechaPagoMiercoles 
        { 
            get; set;
        }
        public int FechaPagoJueves 
        {
            get; set;
        }
        public int FechaPagoViernes
        {
            get; set;
        }
        public int FechaPagoSabado 
        {
            get; set;
        }
        public int FechaPagoDomingo 
        {
            get; set;
        }
        public float CantidadParaRenovar 
        {
            get; set; 
        }
        public int SemanasExtra 
        {
            get; set;
        }  
        public string ActivoSemanaExtra 
        {
            get; set;
        }
        public string Accion 
        {
            get; set;
        }
    }
}