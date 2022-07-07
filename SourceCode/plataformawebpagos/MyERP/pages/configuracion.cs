namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("configuracion")]
    public partial class configuracion
    {
        [Key]
        public int id_configuracion { get; set; }

        [StringLength(250)]
        public string descripcion { get; set; }

        public bool? eliminado { get; set; }

        public int? id_usuario { get; set; }

        [StringLength(100)]
        public string nombre { get; set; }

        public DateTime? fecha_ultima_modificacion { get; set; }

        public double? valor { get; set; }

        [StringLength(250)]
        public string ValorCadena { get; set; }
    }
}
