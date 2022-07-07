namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class garantia_prestamo
    {
        [Key]
        public int id_garantia_prestamo { get; set; }

        public int? id_prestamo { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        [StringLength(100)]
        public string numero_serie { get; set; }

        public double? costo { get; set; }

        public string fotografia { get; set; }

        public DateTime? fecha_registro { get; set; }

        public int? aval { get; set; }

        public int? eliminado { get; set; }
    }
}
