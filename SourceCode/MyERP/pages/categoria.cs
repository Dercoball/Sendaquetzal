namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("categoria")]
    public partial class categoria
    {
        public int id { get; set; }

        [StringLength(200)]
        public string nombre { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }
    }
}
