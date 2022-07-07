namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class web_pregunta_frecuente
    {
        public int id { get; set; }

        [StringLength(250)]
        public string pregunta { get; set; }

        [StringLength(500)]
        public string respuesta { get; set; }

        public DateTime? ultima_modificacion { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }
    }
}
