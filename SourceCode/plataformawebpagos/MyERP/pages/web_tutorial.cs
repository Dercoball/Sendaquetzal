namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class web_tutorial
    {
        public int id { get; set; }

        [StringLength(100)]
        public string titulo { get; set; }

        [StringLength(250)]
        public string url_video { get; set; }

        public int? activo { get; set; }

        public int? eliminado { get; set; }
    }
}
