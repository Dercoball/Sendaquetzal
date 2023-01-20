using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Cliente{ 

        //public int Id_Cliente;
        public int IdCliente 
        { get; set; }
        public String Nombre
        { get; set; }
        public String PrimerApellido
        { get; set; }
        public String SegundoApellido
        { get; set; }

        public String Curp
        { get; set; }
        public String Ocupacion
        { get; set; }
        public int IdTipoCliente
        { get; set; }
        public String TipoCliente
        { get; set; }
        public int IdPrestamo
        { get; set; }
        public float Monto
        { get; set; }
        public String FechaSolicitud
        { get; set; }

        public String Telefono
        { get; set; }
        public String Telefono2
        { get; set; }
        public String Celular
        { get; set; }
        public String Correo_Electronico
        { get; set; }
        public String NombreCompleto
        { get; set; }
        public String NombreStatus
        { get; set; }
        public String Color
        { get; set; }
        public int IdStatusCliente
        { get; set; }

        public int Activo
        { get; set; }
        public int ClienteExistente
        { get; set; }

        public Direccion direccion
        { get; set; }
        public Direccion direccionAval
        { get; set; }

        public string NotaCliente
        { get; set; }
        public string NotaAval
        { get; set; }
        public string NotaEjecutivoCliente;
        public string NotaEjecutivoAval;
        public string NotaFotografiaCliente
        { get; set; }
        public string NotaFotografiaAval
            { get; set; }

        //aval

        public String NombreAval;
        public String PrimerApellidoAval;
        public String SegundoApellidoAval;
        public String CurpAval;
        public String OcupacionAval;
        public String NombreCompletoAval;
        public String TelefonoAval;


		public int Mensaje;
		public string Accion;

        public const int STATUS_INACTIVO = 1;
        public const int STATUS_ACTIVO = 2;
        public const int STATUS_VENCIDO = 3;
        public const int STATUS_CONDONADO = 4;
        public const int STATUS_DEMANDA= 5;

    }
}