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

        public String Telefono;
        public String Telefono2;
        public String Celular;
        public String Correo_Electronico;
        public String NombreCompleto;

        public int Activo;
        public int ClienteExistente;

        public Direccion direccion;
        public Direccion direccionAval;

        public string NotaCliente;
        public string NotaAval;
        public string NotaFotografiaCliente;
        public string NotaFotografiaAval;

        //aval

        public String NombreAval;
        public String PrimerApellidoAval;
        public String SegundoApellidoAval;
        public String CurpAval;
        public String OcupacionAval;
        public String NombreCompletoAval;
        public String TelefonoAval;



        public string Accion;

        public const int STATUS_INACTIVO = 1;
        public const int STATUS_ACTIVO = 2;
        public const int STATUS_VENCIDO = 3;
        public const int STATUS_CONDONADO = 4;

    }
}