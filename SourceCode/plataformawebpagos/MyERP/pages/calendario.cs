namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("calendario")]
    public partial class calendario
    {
        public int id { get; set; }

        [StringLength(200)]
        public string nombre { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha { get; set; }

        public int? eliminado { get; set; }
    }
}
