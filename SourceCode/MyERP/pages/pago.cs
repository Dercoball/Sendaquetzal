namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("pago")]
    public partial class pago
    {
        [Key]
        public int id_pago { get; set; }

        public int? id_prestamo { get; set; }

        public double? monto { get; set; }

        public double? saldo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha { get; set; }

        public int? id_status_pago { get; set; }

        public int? id_usuario { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_registro_pago { get; set; }

        public int? numero_semana { get; set; }

        public double? pagado { get; set; }

        public int? semana_extra { get; set; }
    }
}
