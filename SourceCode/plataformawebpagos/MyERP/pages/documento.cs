namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("documento")]
    public partial class documento
    {
        [Key]
        public int id_documento_colaborador { get; set; }

        [StringLength(250)]
        public string nombre { get; set; }

        public int? id_tipo_documento { get; set; }

        public string contenido { get; set; }

        public int? eliminado { get; set; }

        public int? id_empleado { get; set; }

        public int? id_aval { get; set; }

        [StringLength(150)]
        public string url { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fecha_ingreso { get; set; }

        public int? id_cliente { get; set; }

        [StringLength(50)]
        public string extension { get; set; }
    }
}
