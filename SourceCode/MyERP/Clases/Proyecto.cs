using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Proyecto : Requisicion
    {

        public int IdProyecto;
        public string FechaEntradaISO;// yyyy-MM-dd
   
        public string Diagnostico;
      
        public int IdStatusProyecto;
        public string IdStatusProyectoStr;
        public string DescripcionServicio1;
   

        public String NombreMarca;
        public String NombreModelo;


    }
}