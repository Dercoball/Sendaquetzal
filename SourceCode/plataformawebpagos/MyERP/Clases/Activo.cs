using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class Activo
    {

        public int IdActivo;
        public string Tipo;
        public string Descripcion;
        public string NumeroSerie;
        public float Costo;
        public string Comentarios;
        public int eliminado;
        public int id_empleado;

        public Categoria Categoria;
        public Empleado Empleado;

        public string Accion;



    }
}