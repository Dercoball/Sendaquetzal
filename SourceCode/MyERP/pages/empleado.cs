namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("empleado")]
    public partial class empleado
    {
        [Key]
        public int id_empleado { get; set; }

        public int? id_tipo_usuario { get; set; }

        public int? id_comision_inicial { get; set; }

        public int? id_posicion { get; set; }

        public int? id_plaza { get; set; }

        [StringLength(20)]
        public string curp { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        [StringLength(100)]
        public string primer_apellido { get; set; }

        [StringLength(100)]
        public string segundo_apellido { get; set; }

        [StringLength(50)]
        public string telefono { get; set; }

        public int? eliminado { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_nacimiento { get; set; }

        public int? activo { get; set; }

        public int? id_supervisor { get; set; }

        public int? id_ejecutivo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_ingreso { get; set; }

        [StringLength(100)]
        public string nombre_aval { get; set; }

        [StringLength(100)]
        public string primer_apellido_aval { get; set; }

        [StringLength(100)]
        public string segundo_apellido_aval { get; set; }

        [StringLength(100)]
        public string curp_aval { get; set; }

        [StringLength(50)]
        public string telefono_aval { get; set; }

        public double? monto_limite_inicial { get; set; }

        public int? id_coordinador { get; set; }
    }
}
