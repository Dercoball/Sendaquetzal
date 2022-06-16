namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class relacion_prestamo_aprobacion
    {
        [Key]
        public int id_historial_aprobacion { get; set; }

        public int? id_prestamo { get; set; }

        public int? id_usuario { get; set; }

        public int? id_empleado { get; set; }

        public DateTime? fecha { get; set; }

        public int? id_supervisor { get; set; }

        public int? id_ejecutivo { get; set; }

        [StringLength(500)]
        public string notas_aval { get; set; }

        [StringLength(500)]
        public string notas_cliente { get; set; }

        public int? id_posicion { get; set; }

        [StringLength(50)]
        public string status_aprobacion { get; set; }

        [StringLength(500)]
        public string notas_generales { get; set; }
    }
}
