namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cliente")]
    public partial class cliente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public cliente()
        {
            prestamo = new HashSet<prestamo>();
        }

        [Key]
        public int id_cliente { get; set; }

        [StringLength(100)]
        public string curp { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        [StringLength(100)]
        public string primer_apellido { get; set; }

        [StringLength(100)]
        public string segundo_apellido { get; set; }

        [StringLength(200)]
        public string ocupacion { get; set; }

        [StringLength(100)]
        public string telefono { get; set; }

        public int? id_tipo_cliente { get; set; }

        [StringLength(100)]
        public string curp_aval { get; set; }

        [StringLength(100)]
        public string nombre_aval { get; set; }

        [StringLength(100)]
        public string primer_apellido_aval { get; set; }

        [StringLength(100)]
        public string segundo_apellido_aval { get; set; }

        [StringLength(200)]
        public string ocupacion_aval { get; set; }

        [StringLength(100)]
        public string telefono_aval { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }

        public int? id_status_cliente { get; set; }

        [StringLength(500)]
        public string nota_fotografia { get; set; }

        [StringLength(500)]
        public string nota_fotografia_aval { get; set; }

        public virtual tipo_cliente tipo_cliente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<prestamo> prestamo { get; set; }
    }
}
