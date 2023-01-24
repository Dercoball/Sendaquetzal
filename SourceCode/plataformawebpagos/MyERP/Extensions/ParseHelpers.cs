using System;

namespace Plataforma.Extensions
{
    public static class ParseHelpers
    {
        /// <summary>
        /// Convertidor de starin a Datetime
        /// </summary>
        /// <param name="ps_valor"></param>
        /// <returns></returns>
        public static DateTime ParseStringToDateTime(this string ps_valor)
        {
            var ldt_valor = DateTime.Now;
            return !DateTime.TryParse(ps_valor, out ldt_valor)
                    ? DateTime.Now
                    : ldt_valor;
        }

        /// <summary>
        /// Convertidor de string a float
        /// </summary>
        /// <param name="ps_valor"></param>
        /// <returns></returns>
        public static float  ParseStringToFloat(this string ps_valor) 
        {
            var lf_valor = 0f;
            return !float.TryParse(ps_valor, out lf_valor)
                    ? 0
                    : lf_valor;
        }

        /// <summary>
        /// Convertidor de  string a int 
        /// </summary>
        /// <param name="ps_valor"></param>
        /// <returns></returns>
        public static int ParseStringToInt(this string ps_valor)
        {
            var li_valor = 0;
            return !int.TryParse(ps_valor, out li_valor)
                    ? 0
                    : li_valor;
        }

        /// <summary>
        /// Convertido de string a boolean
        /// </summary>
        /// <param name="ps_valor"></param>
        /// <returns></returns>
        public static bool ParseStringToBoolean(this string ps_valor)
        {
            var lb_valor = false;
            return !bool.TryParse(ps_valor, out lb_valor)
                    ? false
                    : lb_valor;
        }
    }
}