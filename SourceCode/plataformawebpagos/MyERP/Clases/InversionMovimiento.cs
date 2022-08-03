using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plataforma.Clases
{
    public class InversionMovimiento
    {

        public int IdInversion;
        public int IdInversionista;
        public int IdPeriodo;
        public int Utilidades;
        public int NumSemanaInversion;
        public int NumSemanaHoy;
        public int PeriodoSemanas;
        public int RetiroEfectuado;

        public float Monto;
        public string MontoMx;
        public string Fecha;

        public Inversionista Inversionista;



        public string Accion;

    }
}