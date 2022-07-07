namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class abono_pago
    {
        [Key]
        public int id_abono_pago { get; set; }

        public int? id_pago { get; set; }

        public double? monto { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha { get; set; }

        public int? id_usuario { get; set; }
    }
}
