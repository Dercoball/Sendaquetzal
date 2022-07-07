namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("prestamo")]
    public partial class prestamo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public prestamo()
        {
            solicitud_aumento_credito = new HashSet<solicitud_aumento_credito>();
        }

        [Key]
        public int id_prestamo { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_solicitud { get; set; }

        public double? monto { get; set; }

        public int? id_status_prestamo { get; set; }

        public int? id_cliente { get; set; }

        public int? id_usuario { get; set; }

        [StringLength(500)]
        public string notas_generales { get; set; }

        public int? id_empleado { get; set; }

        public int? activo { get; set; }

        public virtual cliente cliente { get; set; }

        public virtual status_prestamo status_prestamo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<solicitud_aumento_credito> solicitud_aumento_credito { get; set; }
    }
}
