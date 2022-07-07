namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class status_solicitud_aumento_credito
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public status_solicitud_aumento_credito()
        {
            solicitud_aumento_credito = new HashSet<solicitud_aumento_credito>();
        }

        [Key]
        public int id_status_solicitud_aumento_credito { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<solicitud_aumento_credito> solicitud_aumento_credito { get; set; }
    }
}
