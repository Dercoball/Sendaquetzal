namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class regla_evaluacion_modulo
    {
        [Key]
        public int id_regla_evaluacion_modulo { get; set; }

        [StringLength(150)]
        public string descripcion { get; set; }

        public int? ponderacion { get; set; }
    }
}
