using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
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
        public float CantidadParaRenovar;
        public int SemanasExtra;
        public string ActivoSemanaExtra;


        public string Accion;


    }
}