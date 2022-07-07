namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class dias_paro
    {
        [Key]
        public int id_dias_paro { get; set; }

        [StringLength(100)]
        public string nota { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_inicio { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_fin { get; set; }

        public int? id_tipo_paro { get; set; }

        public int? eliminado { get; set; }

        public virtual tipo_paro tipo_paro { get; set; }
    }
}
