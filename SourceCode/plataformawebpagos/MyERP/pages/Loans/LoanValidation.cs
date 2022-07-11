using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plataforma.pages
{
    /**
     * Validación de solicitud de préstamo
     */
    public class LoanValidation
    {

        /// <summary>
        ///Buscar un cliente por su curp 
        /// </summary>
        /// <returns>Cliente</returns>
        public Cliente GetClienteByCURP(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Cliente item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 id_cliente, curp, IsNull(id_status_cliente, 0) id_status_cliente 
                                FROM cliente 
                                WHERE curp = @curp";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Cliente();
                    item.IdCliente = int.Parse(ds.Tables[0].Rows[0]["id_cliente"].ToString());
                    item.IdStatusCliente = int.Parse(ds.Tables[0].Rows[0]["id_status_cliente"].ToString());

                }



                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }



        }


        /// <summary>
        ///Buscar una persona Aval, mediante su curp en tabla de cliente 
        /// </summary>
        /// <returns></returns>
        public Cliente GetClienteByCURPAvalCliente(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Cliente item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 id_cliente, curp
                                FROM cliente 
                                WHERE curp_aval = @curp_aval";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP Aval =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp_aval", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Cliente();
                    item.IdCliente = int.Parse(ds.Tables[0].Rows[0]["id_cliente"].ToString());

                }



                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }



        }


        //  Buscar una persona Aval, mediante su curp en tabla de empleado
        public Empleado GetClienteByCURPAvalEmpleado(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Empleado item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 id_empleado, curp
                                FROM empleado 
                                WHERE curp_aval = @curp_aval";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP Aval =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp_aval", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Empleado();
                    item.IdEmpleado = int.Parse(ds.Tables[0].Rows[0]["id_empleado"].ToString());

                }



                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }



        }


        //  Buscar una persona Aval, cuantas veces esta como aval una curp en la tabla de cliente
        public int GetClienteByCURPAvalCliente3Veces(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            int num = 0;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT id_cliente
                                FROM cliente 
                                WHERE curp_aval = @curp_aval";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP Aval =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp_aval", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);

                num = ds.Tables[0].Rows.Count + 1;

                return num;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }



        }



        //  Buscar prestamo por curp del cliente, PRESTAMO con status pendiente 1, 2 actualmente
        public Prestamo GetPrestamoByCURP(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Prestamo item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 p.id_prestamo, c.curp
                                FROM prestamo p JOIN cliente c ON (c.id_cliente = p.id_cliente)
                                WHERE c.curp = @curp AND p.id_status_prestamo IN (1, 2) ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("CURP =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Prestamo();
                    item.IdPrestamo = int.Parse(ds.Tables[0].Rows[0]["id_prestamo"].ToString());

                }



                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }



        }

        //  Buscar prestamo por curp del cliente, PRESTAMO con status activo 4 
        public Prestamo GetPrestamoActiveByCURP(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Prestamo item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 p.id_prestamo, c.curp
                                FROM prestamo p JOIN cliente c ON (c.id_cliente = p.id_cliente)
                                WHERE c.curp = @curp AND p.id_status_prestamo IN (1, 2) ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("CURP =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Prestamo();
                    item.IdPrestamo = int.Parse(ds.Tables[0].Rows[0]["id_prestamo"].ToString());

                }



                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }



        }


        //  Historial con falla o abonado por curp
        public Boolean GetHistorialFallaOAbonadoByCustomerCurp(string path, string curp, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Boolean historialConFallaOAbonado = false;

            try
            {
                DataSet ds = new DataSet();
                string query = @"  SELECT h.id_cliente, h.id_semana, h.atiempo, h.abonado, h.falla
                                FROM historial_cliente h 
                                JOIN cliente c ON (c.id_cliente = h.id_cliente)
                                WHERE c.curp = @curp AND ( IsNull(h.abonado, 0) = 1  OR IsNull(h.falla, 0) = 1 ) ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("curp =  " + curp);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp", curp);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                historialConFallaOAbonado = ds.Tables[0].Rows.Count > 0;    //  true / false

                return historialConFallaOAbonado;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return historialConFallaOAbonado;
            }



        }


        //  Historial con falla o abonado por id cliente
        public Boolean GetHistorialFallaOAbonadoByCustomerId(string path, string idCliente, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Boolean historialConFallaOAbonado = false;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT id_cliente, id_semana, atiempo, abonado, falla
                                FROM historial_cliente  
                                WHERE id_cliente = @id_cliente AND(IsNull(abonado, 0) = 1  OR IsNull(falla, 0) = 1 )";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("idCliente =  " + idCliente);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_cliente", idCliente);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                historialConFallaOAbonado = ds.Tables[0].Rows.Count > 0;    //  true / false

                return historialConFallaOAbonado;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return historialConFallaOAbonado;
            }



        }

        public class WeekData
        {
            public string fechaInicial;
            public string fechaFinal;
        }

        public WeekData GetFechas()
        {

            //  fechas
            DateTime endWeekDate = DateTime.Now;
            var numDayOfweek = (int)endWeekDate.DayOfWeek;
            endWeekDate = new DateTime(endWeekDate.Year, endWeekDate.Month, endWeekDate.Day);
            endWeekDate = endWeekDate.AddDays(7);
            endWeekDate = endWeekDate.AddDays(-numDayOfweek);
            Utils.Log("Fecha final de la semana: " + endWeekDate);

            DateTime startWeekDate = DateTime.Now;
            var numDayOfweek2 = (int)startWeekDate.DayOfWeek;
            startWeekDate = startWeekDate.AddDays(-numDayOfweek2);
            Utils.Log("Fecha inicial de la semana : " + startWeekDate);


            string startDateStr = startWeekDate.Year.ToString() + "-" + startWeekDate.Month.ToString() + "-" + startWeekDate.Day.ToString();
            string endDateStr = endWeekDate.Year.ToString() + "-" + endWeekDate.Month.ToString() + "-" + endWeekDate.Day.ToString();

            WeekData week = new WeekData();
            week.fechaInicial = startDateStr;
            week.fechaFinal = endDateStr;

            return week;

        }



    }
}
