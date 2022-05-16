using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Prenomina{

  
        public int Activo;
        public string Accion;
        public string NombreStatus;
        public string ActivoStr;

        public int IdPrenomina;
        public int IdEmpleadoRegistro;
        public int IdUsuarioRegistro;
        public int IdStatusPrenomina;
        public int IdEmpleado;//El residente o encargado del depto.
        public int IdObra;//El residente o encargado del depto.
        public String IdTipoPrenomina; // (1) normal ó (2)10-4
        public String NumeroSemana;
        
        public DateTime FechaCreacion;
        public string FechaCreacionStr;
        public DateTime FechModificacion;
        public string FechModificacionStr;

        public const int STATUS_PRENOMINA_PORGENERAR = 1;
        public const int STATUS_PRENOMINA_GENERADA = 2;
        public const int STATUS_PRENOMINA_ENVIADA = 3;
        public const int STATUS_PRENOMINA_APROBADA = 4;
        public const int STATUS_PRENOMINA_FINALIZADA = 5;
        

    }
}