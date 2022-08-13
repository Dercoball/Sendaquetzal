using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerifyStatusPaymentsService
{
    public class TipoCliente
    {
        public int IdTipoCliente;
        public string NombreTipoCliente;
        public float PrestamoInicialMaximo;
        public float PorcentajeSemanal;
        public int SemanasAPrestar;
        public float GarantiasPorMonto;
        public string FechasDePago;
        public int FechaPagoLunes;
        public int FechaPagoMartes;
        public int FechaPagoMiercoles;
        public int FechaPagoJueves;
        public int FechaPagoViernes;
        public int FechaPagoSabado;
        public int FechaPagoDomingo;
        public float CantidadParaRenovar;
        public int SemanasExtra;
        public string ActivoSemanaExtra;


        public string Accion;


    }
}