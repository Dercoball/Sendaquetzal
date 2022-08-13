using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerifyStatusPaymentsService
{
    public class Impuesto
    {

        public int id;
        public int idImpuesto;
        public int idStatusImpuesto;
        public String apellidos;
        public String nombre;
        public String nombreTablaDetalle;
        public String cuit;
        public String patente;
        public String numeroNotificacion;
        public String dominio;//patente o placas del carro

        public String numeroPago;
        public String contribuyente;
        public String anio;
        public String cuota;
        public float importe1;
        public String vencimiento1Str;
        public DateTime vencimiento1;
        public String vencimiento;//fecha vencimiento actual, puede ser la 1 o la 2
        public float importe2;
        public float importe;
        public DateTime vencimiento2;
        public String vencimiento2Str;
        public float punitorios;
        public float importeActualizado;

        public string accion;
        public float importeV1;
        public string importeV1Moneda;
        public float importeV2;
        public string importeV2Moneda;
        public DateTime infraccionVto1;
        public string infraccionVto1Str = null;
        public string infraccionVto1Canonical = null;
        public DateTime infraccionVto2;
        public string infraccionVto2Str = null;
        public string infraccionVto2Canonical = null;


    }
}