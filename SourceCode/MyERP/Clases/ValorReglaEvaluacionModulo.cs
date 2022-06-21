using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class ValorReglaEvaluacionModulo:EvaluacionModulo
    {

        public int IdValorReglaEvaluacionModulo;
        public int IdReglaEvaluacionModulo;
        
        public string Descripcion;
        public int Ponderacion;
        public int Calificacion;
        public string PonderacionStr;
        public string CalificacionStr;
        public int Completado;

    }
}