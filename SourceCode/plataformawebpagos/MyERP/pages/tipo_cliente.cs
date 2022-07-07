namespace Plataforma.pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tipo_cliente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tipo_cliente()
        {
            cliente = new HashSet<cliente>();
        }

        [Key]
        public int id_tipo_cliente { get; set; }

        [Column("tipo_cliente")]
        [Required]
        [StringLength(100)]
        public string tipo_cliente1 { get; set; }

        public double? prestamo_inicial_maximo { get; set; }

        public double? porcentaje_semanal { get; set; }

        public int? semanas_a_prestar { get; set; }

        public double? garantias_por_monto { get; set; }

        [StringLength(100)]
        public string fechas_pago { get; set; }

        public double? cantidad_para_renovar { get; set; }

        public int? semana_extra { get; set; }

        public int? eliminado { get; set; }

        public int? fecha_pago_lunes { get; set; }

        public int? fecha_pago_martes { get; set; }

        public int? fecha_pago_miercoles { get; set; }

        public int? fecha_pago_jueves { get; set; }

        public int? fecha_pago_viernes { get; set; }

        public int? fecha_pago_sabado { get; set; }

        public int? fecha_pago_domingo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cliente> cliente { get; set; }
    }
}
