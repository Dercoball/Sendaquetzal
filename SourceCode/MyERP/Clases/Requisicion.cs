using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Requisicion
    {

        public int IdRequisicion;
        public DateTime FechaCreacion;
        public DateTime FechaModificacion;
        public string FechaCreacionCanonical;//
        public string FechaCreacionISO;//
        public string FechaCreacionFormateada;//
        public string FechaCreacionFormateadaMx;//
        public string HoraCreacion;

        public DateTime FechaCierre;
        public string FechaCierreFormateada;//


        public string TiempoTranscurrido;
        
        
        public int IdCliente;
        public int IdUsuario;
        public int IdOperador;
        public int IdUnidadMedidaOrometro;


        public int IdProveedor;
        public int IdStatusRequisicion;
        public int IdUbicacion;
        public int IdEquipo;
        public int IdMarca;
        public int IdNivelPrioridad;
        public int IdUsuarioDiagnostico;
        public int IdUsuarioDiagnosticoAnterior;
        public string NombreUsuarioDiagnostico;
        public int IdModelo;
        public int Activa;
        public int StatusFinalizada;

        public string NombrePrioridad;
        public string Descripcion;
        public string Diagnostico;
        public string Orometro;
        public string OrometroUltimoMantenimiento;
        public int DetieneOperacion;

        public string Latitud;
        public string Longitud;
        public string Coordenadas;


        public string NombreStatus;
        public string NombreUbicacion;
        public string NombreStatusProyecto;

        public string NumeroSerie;
        public string NumeroEconomico;
        public string Anio;

        public string NombreEquipo;
        public string NombreProveedor;
        public string NombreUsuario;

        public string NombreOperador;
        public string Telefono;
        public string Tipo;//asignacion, sin asignacion, colaboracion
        public string TotalRequisiciones;


        public string Accion;

        //public const int STATUS_REQUISICION_REGISTROFALLA = 1;
        //public const int STATUS_REQUISICION_REGISTRODIAGNOSTICO = 2;
        //public const int STATUS_REQUISICION_CREADA = 3;
        //public const int STATUS_REQUISICION_ENVIADA = 4;
        //public const int STATUS_REQUISICION_ENPROCESO = 5;
        //public const int STATUS_REQUISICION_FINALIZADA = 6;


        public const int STATUS_OT_ENREVISION = 1;           //  Cuando recien se crea la OT y se puede editar 
        public const int STATUS_OT_PARAASIGNAR = 2;          //  Al ser enviada para asignarle un empleado  desde el botón Enviar ( en la lista de ordenes)
        public const int STATUS_OT_PROXIMOATENDER = 3;       //  Al asignarle un empleado desde Ots en curso       
        public const int STATUS_OT_ENESPERADEREFACCIONES= 4; //  Manual con un botón desde Ots en curso, mediante un permiso
        public const int STATUS_OT_ENATENCION = 5;           //  Manual con un botón desde Ots en curso, mediante un permiso    
        public const int STATUS_OT_FINALIZADA = 6;           //  Desde el panel de diagnostico por quien esta atendiendo o manual con un botón desde Ots en curso, mediante un permiso  

    }
}