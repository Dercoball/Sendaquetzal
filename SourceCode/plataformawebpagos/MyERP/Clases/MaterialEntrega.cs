using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class MaterialEntrega
    {

        public int IdMaterialEntrega;
        public string MaterialEntregado;
        public float Cantidad;
        public DateTime Fecha;
        public float Costo;
        public string CostoMx;
        public int Eliminado;
        public int IdEmpleado;
        public int IdCategoria;
        public string Colaborador;
        public string NombreCategoria;

        public Categoria Categoria;
        public Empleado Empleado;

        public string Accion;



    }
}