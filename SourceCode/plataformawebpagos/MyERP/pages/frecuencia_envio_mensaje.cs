namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class frecuencia_envio_mensaje
    {
        [Key]
        public int id_frecuencia_envio_mensaje { get; set; }

        [StringLength(50)]
        public string nombre { get; set; }

        public int? eliminado { get; set; }
    }
}
