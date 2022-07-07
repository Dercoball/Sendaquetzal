namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class log_cambios
    {
        public int id { get; set; }

        public int? id_usuario { get; set; }

        [StringLength(1500)]
        public string descripcion { get; set; }

        public DateTime? fecha_hora { get; set; }

        [StringLength(50)]
        public string modulo { get; set; }
    }
}
