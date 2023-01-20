using System;

namespace Plataforma.Clases
{
    /// <summary>
    /// Clase que representa al inversionista
    /// JGG 2023-01-15:  Se anexaron los campos status
    /// Se cambio nombre porcentaje_interes_anual por UtilidadSugerida 
    /// Se anexa la fecha de registro
    /// </summary>
    public class Inversionista{

        #region Atributos
        public int IdInversionista
        { get; set; }
        public string Nombre
        { get; set; }
        public string RFC
        { get; set; }
        public bool Status
        { get; set; }
        public bool Eliminado
        { get; set; }
        public float PorcentajeUtilidadSugerida
        { get; set; }
        public string RazonSocial
        { get; set; }
        public DateTime FechaRegistro
        { get; set; }
        public string Accion
        { get; set; }
        #endregion

        #region Variables deprecadas
        //public string RazonSocial;
        //public float PorcentajeInteresAnual;
        #endregion
    }
}