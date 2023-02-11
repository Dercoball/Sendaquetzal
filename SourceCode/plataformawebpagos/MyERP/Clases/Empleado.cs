using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Empleado{ 

        public int IdEmpleado;
        public String CURP;
        public String Nombre;
        public String PrimerApellido;
        public String SegundoApellido;

        public String CURPAval;
        public String NombreAval;
        public String PrimerApellidoAval;
        public String SegundoApellidoAval;
        public String TelefonoAval;


        public String AMaterno;
        public String Login;
        
        public String APaterno;
        public String NombreCompleto;
        public String NombreCompletoSupervisor;
        public String NombreCompletoEjecutivo;
        public String NombreCompletoAval;
        public DateTime FechaIngreso;
        public String FechaNacimiento;
        
        //  fechas dd/mm/aaaa
        public String FechaIngresoMx;
        public String FechaNacimientoMx;

        public String Ocupacion;
        public String Telefono;
        //public String NombreModulo;
        public String NombreComision;
        public String NombrePlaza;
        public String NombreUsuario;
        public String Password;
        public String NombreTipoUsuario;
        //public int IdModulo;
        public int IdPosicion;
        public int IdPlaza;
        public int IdDepartamento;
        public int IdPuesto;
        public int IdTipoUsuario;
        public int IdSupervisor;
        public int IdEjecutivo;
        public int IdCoordinador;
        public int IdComisionInicial;   //modulo
        public string SalarioMensual;
        public float MontoLimiteInicial;
        public float PorcentajeComision;
        public int Activo;
        public string Accion;
        public string ActivoStr;

        public float limite_venta_ejercicio;
        public float limite_incremento_ejercicio;
        public bool? fizcalizable;
        public string NotaFoto;
        public Direccion Direccion;
        public Direccion DireccionAval;
        public Usuario usuario;
        public Comision nivelNomision;// comision o modulo
    }
}