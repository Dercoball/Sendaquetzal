using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class LineaPrenomina : Empleado{ 

        //public int IdEmpleado;
        //public String Nombre;
        //public String Clave;
        //public String ClavePuesto;
        //public String ClaveDepartamento;
        //public String APaterno;
        //public String NombreCompleto;
        //public String FechaAlta;
        //public String SalarioMensual;
        //public String AMaterno;
        //public String NombreDepartamento;
        //public String NombrePuesto;
        //public int IdDepartamento;
        //public int IdArea;
        //public int IdPuesto;
        //public int Activo;
        //public string Accion;
        //public string ActivoStr;

        public String IdTipoPrenomina; // normal ó 10-4
        public String NumeroSemana;
        public String ValorJue;
        public String ValorVie;
        public String ValorSab;
        public String ValorDom;
        public String ValorLun;
        public String ValorMar;
        public String ValorMie;
        public String NumeroDias;
        public String NumeroHorasExtra;
        public int Viaticos;
        public int DiasViaticos;
        public string MontoViaticos;
        public float MontoTotalViaticos;    //  dato fijo que viene de la tabla nivel_puesto
        public String DetalleHorasExtra;
        public String Observaciones;


    }
}