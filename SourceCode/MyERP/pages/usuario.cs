namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("usuario")]
    public partial class usuario
    {
        [Key]
        public int id_usuario { get; set; }

        public int? id_tipo_usuario { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }

        [StringLength(50)]
        public string login { get; set; }

        [StringLength(50)]
        public string password { get; set; }

        [StringLength(50)]
        public string email { get; set; }

        [StringLength(50)]
        public string telefono { get; set; }

        public bool? eliminado { get; set; }

        public int? id_empleado { get; set; }
    }
}
