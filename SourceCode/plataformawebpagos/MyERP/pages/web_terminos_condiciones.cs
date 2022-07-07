namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class web_terminos_condiciones
    {
        public int id { get; set; }

        [Column(TypeName = "text")]
        public string info { get; set; }
    }
}
