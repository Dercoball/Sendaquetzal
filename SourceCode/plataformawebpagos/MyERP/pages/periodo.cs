namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("periodo")]
    public partial class periodo
    {
        [Key]
        public int id_periodo { get; set; }

        [Required]
        [StringLength(200)]
        public string nombre { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }

        public int? valor_periodo { get; set; }
    }
}
