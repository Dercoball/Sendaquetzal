using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Cliente{ 

        //public int Id_Cliente;
        public int IdCliente;
        public String Nombre;
        public String PrimerApellido;
        public String SegundoApellido;
       
        public String Curp;
        public String Ocupacion;
        public int IdTipoCliente;
        public String TipoCliente;
        public int IdPrestamo;
        public float Monto;
        public String FechaSolicitud;

        public String Telefono_1;
        public String Telefono_2;
        public String Celular;
        public String Correo_Electronico;
        public String NombreCompleto;

        public int Activo;

        public Direccion direccion;
        public Direccion direccionAval;


        //aval

        public String NombreAval;
        public String PrimerApellidoAval;
        public String SegundoApellidoAval;
        public String CurpAval;
        public String OcupacionAval;
        public String NombreCompletoAval;
        public String TelefonoAval;



        public string Accion;
       


    }
}