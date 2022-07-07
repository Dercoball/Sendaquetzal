namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class historial_cliente
    {
        [Key]
        public int id_historial_cliente { get; set; }

        public int? id_cliente { get; set; }

        public int? id_semana { get; set; }

        public int? atiempo { get; set; }

        public int? abonado { get; set; }

        public int? falla { get; set; }
    }
}
