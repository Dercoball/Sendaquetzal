namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class status_pago
    {
        [Key]
        public int id_status_pago { get; set; }

        [StringLength(250)]
        public string nombre { get; set; }

        [StringLength(20)]
        public string color { get; set; }
    }
}
