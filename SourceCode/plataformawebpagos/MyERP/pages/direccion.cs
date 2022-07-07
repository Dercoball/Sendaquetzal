namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("direccion")]
    public partial class direccion
    {
        [Key]
        public int id_direccion { get; set; }

        public int? id_empleado { get; set; }

        public int? id_aval { get; set; }

        [StringLength(100)]
        public string calleyno { get; set; }

        [StringLength(100)]
        public string colonia { get; set; }

        [StringLength(100)]
        public string municipio { get; set; }

        [StringLength(100)]
        public string estado { get; set; }

        [StringLength(50)]
        public string telefono { get; set; }

        [StringLength(50)]
        public string codigo_postal { get; set; }

        public int? id_municipio { get; set; }

        public int? id_estado { get; set; }

        public int? activo { get; set; }

        public int? aval { get; set; }

        [StringLength(250)]
        public string direccion_trabajo { get; set; }

        public int? id_cliente { get; set; }

        [StringLength(100)]
        public string ubicacion { get; set; }
    }
}
