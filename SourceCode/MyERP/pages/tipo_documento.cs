namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tipo_documento
    {
        [Key]
        public int id_tipo_documento { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }
    }
}
