namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("comision")]
    public partial class comision
    {
        [Key]
        public int id_comision { get; set; }

        public int? id_modulo { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }

        public double? porcentaje { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }

        public virtual modulo modulo { get; set; }
    }
}
