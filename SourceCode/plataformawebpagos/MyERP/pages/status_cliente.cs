namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class status_cliente
    {
        [Key]
        public int id_status_cliente { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }
    }
}
