namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("plantilla")]
    public partial class plantilla
    {
        public int id { get; set; }

        [Column(TypeName = "text")]
        public string contenido { get; set; }

        public int? id_tipo_plantilla { get; set; }

        public int? id_frecuencia_envio_mensaje { get; set; }

        public int? eliminado { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }
    }
}
