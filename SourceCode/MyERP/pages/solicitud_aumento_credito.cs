namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class solicitud_aumento_credito
    {
        [Key]
        public int id_solicitud_aumento_credito { get; set; }

        public int? id_prestamo { get; set; }

        public int? id_usuario { get; set; }

        public DateTime? fecha { get; set; }

        public int? id_director { get; set; }

        public int? id_status_solicitud_aumento_credito { get; set; }

        public virtual prestamo prestamo { get; set; }

        public virtual status_solicitud_aumento_credito status_solicitud_aumento_credito { get; set; }
    }
}
