using ServicioEnviaMsgsNuevoCredito;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicioEnviaMsgsNuevoCredito
{
    public class SQContext : DbContext
    {

        public SQContext() : base("sendaquetzalv1Entities")
        {
        }

        public DbSet<cliente> Customer { get; set; }
        public DbSet<empleado> Employee { get; set; }
        public DbSet<plantilla> TemplateMessage { get; set; }
        public DbSet<resumen_calculo_fallas> SummaryOperation { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
