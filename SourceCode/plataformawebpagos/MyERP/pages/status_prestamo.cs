namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class status_prestamo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public status_prestamo()
        {
            prestamo = new HashSet<prestamo>();
        }

        [Key]
        public int id_status_prestamo { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }

        [StringLength(25)]
        public string color { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<prestamo> prestamo { get; set; }
    }
}
