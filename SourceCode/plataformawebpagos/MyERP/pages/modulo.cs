namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("modulo")]
    public partial class modulo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public modulo()
        {
            comision = new HashSet<comision>();
        }

        [Key]
        public int id_modulo { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comision> comision { get; set; }
    }
}
