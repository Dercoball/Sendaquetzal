using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class DiaDeParo
    {

        [Column("id_dias_paro")]
        public int IdDiaParo;

        [Column("nota")]
        public string Nota;

        [Column("fecha_inicio")]
        public DateTime FechaInicio;

        [Column("fecha_fin")]
        public DateTime FechaFin;

        public string Plaza;

        public string Estatus;

        [Column("id_tipo_paro")]
        public int IdTipoParo;
    }
}