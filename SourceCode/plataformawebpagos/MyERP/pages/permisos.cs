namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class permisos
    {
        [Key]
        public int id_permiso { get; set; }

        [StringLength(200)]
        public string nombre { get; set; }

        public int? tipo_permiso { get; set; }

        public bool? activo { get; set; }

        [StringLength(200)]
        public string nombre_interno { get; set; }

        [StringLength(100)]
        public string nombre_recurso { get; set; }

        public int? id_tipo_usuario { get; set; }
    }
}
