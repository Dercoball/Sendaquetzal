using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class DiaDeParo
    {

        public int IdDiasParo;

        public string Nota;

        public DateTime FechaInicio;

        public DateTime FechaFin;

        public int IdPlaza;

        public string Plaza;

        public string Estatus;

        public int IdTipoParo;
    }
}