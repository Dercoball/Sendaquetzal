using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Plataforma.pages
{
    public partial class ReglaEvaluacionModulo : DbContext
    {
        public ReglaEvaluacionModulo()
            : base("name=ConexionSettings")
        {
        }

        public virtual DbSet<abono_pago> abono_pago { get; set; }
        public virtual DbSet<calendario> calendario { get; set; }
        public virtual DbSet<categoria> categoria { get; set; }
        public virtual DbSet<cliente> cliente { get; set; }
        public virtual DbSet<comision> comision { get; set; }
        public virtual DbSet<configuracion> configuracion { get; set; }
        public virtual DbSet<dias_paro> dias_paro { get; set; }
        public virtual DbSet<direccion> direccion { get; set; }
        public virtual DbSet<documento> documento { get; set; }
        public virtual DbSet<forma_pago> forma_pago { get; set; }
        public virtual DbSet<garantia_prestamo> garantia_prestamo { get; set; }
        public virtual DbSet<historial_cliente> historial_cliente { get; set; }
        public virtual DbSet<log_cambios> log_cambios { get; set; }
        public virtual DbSet<modulo> modulo { get; set; }
        public virtual DbSet<pago> pago { get; set; }
        public virtual DbSet<periodo> periodo { get; set; }
        public virtual DbSet<permisos> permisos { get; set; }
        public virtual DbSet<permisos_tipo_usuario> permisos_tipo_usuario { get; set; }
        public virtual DbSet<plantilla> plantilla { get; set; }
        public virtual DbSet<plaza> plaza { get; set; }
        public virtual DbSet<prestamo> prestamo { get; set; }
        public virtual DbSet<regla_evaluacion_modulo> regla_evaluacion_modulo { get; set; }
        public virtual DbSet<relacion_prestamo_aprobacion> relacion_prestamo_aprobacion { get; set; }
        public virtual DbSet<solicitud_aumento_credito> solicitud_aumento_credito { get; set; }
        public virtual DbSet<status_cliente> status_cliente { get; set; }
        public virtual DbSet<status_pago> status_pago { get; set; }
        public virtual DbSet<status_prestamo> status_prestamo { get; set; }
        public virtual DbSet<status_solicitud_aumento_credito> status_solicitud_aumento_credito { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<tipo_cliente> tipo_cliente { get; set; }
        public virtual DbSet<tipo_documento> tipo_documento { get; set; }
        public virtual DbSet<tipo_paro> tipo_paro { get; set; }
        public virtual DbSet<usuario> usuario { get; set; }
        public virtual DbSet<web_pregunta_frecuente> web_pregunta_frecuente { get; set; }
        public virtual DbSet<empleado> empleado { get; set; }
        public virtual DbSet<frecuencia_envio_mensaje> frecuencia_envio_mensaje { get; set; }
        public virtual DbSet<posicion> posicion { get; set; }
        public virtual DbSet<tipo_plantilla> tipo_plantilla { get; set; }
        public virtual DbSet<web_acercade> web_acercade { get; set; }
        public virtual DbSet<web_politicas_privacidad> web_politicas_privacidad { get; set; }
        public virtual DbSet<web_terminos_condiciones> web_terminos_condiciones { get; set; }
        public virtual DbSet<web_tutorial> web_tutorial { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<calendario>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<categoria>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.curp)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.primer_apellido)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.segundo_apellido)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.ocupacion)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.telefono)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.curp_aval)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.nombre_aval)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.primer_apellido_aval)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.segundo_apellido_aval)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.ocupacion_aval)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.telefono_aval)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.nota_fotografia)
                .IsUnicode(false);

            modelBuilder.Entity<cliente>()
                .Property(e => e.nota_fotografia_aval)
                .IsUnicode(false);

            modelBuilder.Entity<comision>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<configuracion>()
                .Property(e => e.descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<configuracion>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<configuracion>()
                .Property(e => e.ValorCadena)
                .IsUnicode(false);

            modelBuilder.Entity<dias_paro>()
                .Property(e => e.nota)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.calleyno)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.colonia)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.municipio)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.estado)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.telefono)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.codigo_postal)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.direccion_trabajo)
                .IsUnicode(false);

            modelBuilder.Entity<direccion>()
                .Property(e => e.ubicacion)
                .IsUnicode(false);

            modelBuilder.Entity<documento>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<documento>()
                .Property(e => e.contenido)
                .IsUnicode(false);

            modelBuilder.Entity<documento>()
                .Property(e => e.url)
                .IsUnicode(false);

            modelBuilder.Entity<documento>()
                .Property(e => e.extension)
                .IsUnicode(false);

            modelBuilder.Entity<forma_pago>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<garantia_prestamo>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<garantia_prestamo>()
                .Property(e => e.numero_serie)
                .IsUnicode(false);

            modelBuilder.Entity<garantia_prestamo>()
                .Property(e => e.fotografia)
                .IsUnicode(false);

            modelBuilder.Entity<log_cambios>()
                .Property(e => e.descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<log_cambios>()
                .Property(e => e.modulo)
                .IsUnicode(false);

            modelBuilder.Entity<modulo>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<periodo>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<permisos>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<permisos>()
                .Property(e => e.nombre_interno)
                .IsUnicode(false);

            modelBuilder.Entity<permisos>()
                .Property(e => e.nombre_recurso)
                .IsUnicode(false);

            modelBuilder.Entity<plantilla>()
                .Property(e => e.contenido)
                .IsUnicode(false);

            modelBuilder.Entity<plantilla>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<plaza>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<prestamo>()
                .Property(e => e.notas_generales)
                .IsUnicode(false);

            modelBuilder.Entity<regla_evaluacion_modulo>()
                .Property(e => e.descripcion)
                .IsUnicode(false);

            modelBuilder.Entity<relacion_prestamo_aprobacion>()
                .Property(e => e.notas_aval)
                .IsUnicode(false);

            modelBuilder.Entity<relacion_prestamo_aprobacion>()
                .Property(e => e.notas_cliente)
                .IsUnicode(false);

            modelBuilder.Entity<relacion_prestamo_aprobacion>()
                .Property(e => e.status_aprobacion)
                .IsUnicode(false);

            modelBuilder.Entity<relacion_prestamo_aprobacion>()
                .Property(e => e.notas_generales)
                .IsUnicode(false);

            modelBuilder.Entity<status_cliente>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<status_pago>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<status_pago>()
                .Property(e => e.color)
                .IsUnicode(false);

            modelBuilder.Entity<status_prestamo>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<status_prestamo>()
                .Property(e => e.color)
                .IsUnicode(false);

            modelBuilder.Entity<status_solicitud_aumento_credito>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<tipo_cliente>()
                .Property(e => e.tipo_cliente1)
                .IsUnicode(false);

            modelBuilder.Entity<tipo_cliente>()
                .Property(e => e.fechas_pago)
                .IsUnicode(false);

            modelBuilder.Entity<tipo_documento>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<tipo_paro>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<usuario>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<usuario>()
                .Property(e => e.login)
                .IsUnicode(false);

            modelBuilder.Entity<usuario>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<usuario>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<usuario>()
                .Property(e => e.telefono)
                .IsUnicode(false);

            modelBuilder.Entity<web_pregunta_frecuente>()
                .Property(e => e.pregunta)
                .IsUnicode(false);

            modelBuilder.Entity<web_pregunta_frecuente>()
                .Property(e => e.respuesta)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.curp)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.primer_apellido)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.segundo_apellido)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.telefono)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.nombre_aval)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.primer_apellido_aval)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.segundo_apellido_aval)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.curp_aval)
                .IsUnicode(false);

            modelBuilder.Entity<empleado>()
                .Property(e => e.telefono_aval)
                .IsUnicode(false);

            modelBuilder.Entity<frecuencia_envio_mensaje>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<posicion>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<tipo_plantilla>()
                .Property(e => e.nombre)
                .IsUnicode(false);

            modelBuilder.Entity<web_acercade>()
                .Property(e => e.info)
                .IsUnicode(false);

            modelBuilder.Entity<web_politicas_privacidad>()
                .Property(e => e.info)
                .IsUnicode(false);

            modelBuilder.Entity<web_terminos_condiciones>()
                .Property(e => e.info)
                .IsUnicode(false);

            modelBuilder.Entity<web_tutorial>()
                .Property(e => e.titulo)
                .IsUnicode(false);

            modelBuilder.Entity<web_tutorial>()
                .Property(e => e.url_video)
                .IsUnicode(false);
        }
    }
}
