using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerifyStatusPaymentsService
{
    public class Infraccion
    {

        public int idInfraccion;
        public int idStatusInfraccion;
        public String apellidos;
        public String nombre;
        public String cuit;
        public String patente;
        public String numeroNotificacion;
        public String dominio;//patente o placas del carro
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