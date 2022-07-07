namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tipo_plantilla
    {
        [Key]
        public int id_tipo_plantilla { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        public int? eliminado { get; set; }
    }
}
