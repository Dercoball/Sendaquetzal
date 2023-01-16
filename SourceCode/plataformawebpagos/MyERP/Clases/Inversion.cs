using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// JGC se genera nueva clase de inversion ya que de acuerdo al video proporcionado faltaban campos
    /// de iagul manera se comentaron las propiedades anteriores
    /// </summary>
    public class Inversion
    {
        public int id_inversion
        {
            get; set;
        }
        public int id_inversionista
        {
            get; set;
        }
        public int id_status_inversion
        { get; set; }
        public DateTime fecha
        {
            get; set;
        }
        public DateTime? fechaRetiro
        {
            get; set;
        }
        public DateTime fechaVencimiento
        {
            get; set;
        }
        public float monto
        {
            get; set;
        }
        public float? montoRetiro
        {
            get;set;
        }
        public float porcentaje_utilidad
        { 
            get; set; 
        }
        public float utilidad_pesos
        {
            get; set; 
        }
        public int plazo
        { 
            get; set; 
        }
        public float inversion_utilidad
        {
            get; set;
        }
        public string comprobante
        {
            get; set; 
        }
        public int eliminado
        { 
            get; set; 
        }
        public string Accion
        {
            get;
            set;
        }
        public Inversionista Inversionista 
        {
            get;set;
        }
        public StatusInversion Estatus
        {
            get; set;
        }
        #region Codigo comentado JGC
        /*
        public int IdInversion;
        public int IdInversionista;
        public int IdPeriodo;
        public int Utilidades;
        public int NumSemanaInversion;
        public int NumSemanaHoy;
        public int PeriodoSemanas;
        public int RetiroEfectuado;
        public float Monto;
        public string MontoMx;
        public string Fecha;
        public Inversionista Inversionista;
        public string Accion;
        */
        #endregion

    }
}