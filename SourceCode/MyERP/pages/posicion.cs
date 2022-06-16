namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("posicion")]
    public partial class posicion
    {
        [Key]
        public int id_posicion { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }
    }
}
