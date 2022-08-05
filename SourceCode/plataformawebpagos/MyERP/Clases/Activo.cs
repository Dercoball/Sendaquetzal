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
        public int Eliminado;
        public int IdEmpleado;
        public int IdCategoria;

        public Categoria Categoria;
        public Empleado Empleado;

        public string Accion;



    }
}