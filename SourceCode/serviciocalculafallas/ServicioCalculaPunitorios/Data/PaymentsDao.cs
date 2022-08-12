using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VerifyStatusPaymentsService.App_Data
{
    public class PaymentsDao
    {


        public List<Pago> GetPayments(string idStatus,
               string fechaInicial, string fechaFinal, SqlConnection conn, SqlTransaction transaction)
        {

            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();


            try
            {

                //  Filtro status del pago
                var sqlStatus = " AND p.id_status_pago = '" + idStatus + "'";
                

                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr,  prestamo.id_cliente, c.id_tipo_cliente,
                                    st.nombre nombre_status_pago, st.color

                                    FROM pago p
                                    JOIN prestamo prestamo ON (p.id_prestamo = prestamo.id_prestamo AND ISNULL(prestamo.activo, 1) = 1)                                            
                                    JOIN status_pago st ON (st.id_status_pago = p.id_status_pago)                                            
                                    JOIN cliente c ON (c.id_cliente = prestamo.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"') "
                                    + sqlStatus
                                    + " ORDER BY p.id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Transaction = transaction;

                Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();
                        item.cliente = new Cliente();

                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());

                        item.cliente.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.cliente.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.cliente.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();

                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00
                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();
                        item.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        item.Status = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_pago"].ToString() + "</span>";


                        string botones = "";

                        botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='payments.view(" + item.IdPago + ")'  class='btn btn-outline-primary'> <span class='fa fa-folder-open mr-1'></span>Abrir</button>";

                        item.Accion = botones;

                        items.Add(item);


                    }
                }


                return items;
            }
            catch (Exception ex)
            {
                Log("Error ... " + ex.Message);
                Log(ex.StackTrace);
                throw ex;

            }


        }

        public TipoCliente GetCustomerTypeById(string customerTypeId, SqlConnection conn, SqlTransaction transaction)
        {

            TipoCliente item = new TipoCliente();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente as id, tipo_cliente, IsNull(prestamo_inicial_maximo, 0) prestamo_inicial_maximo, 
                                    IsNull(porcentaje_semanal, 0) porcentaje_semanal, IsNull(semanas_a_prestar, 0) semanas_a_prestar, 
                                    IsNull(garantias_por_monto, 0) garantias_por_monto,
                                    fechas_pago, IsNull(cantidad_para_renovar, 0) cantidad_para_renovar, 
                                    IsNull(semana_extra, 0) semana_extra, IsNull(fecha_pago_lunes, 0) fecha_pago_lunes, 
                                    IsNull(fecha_pago_martes, 0) fecha_pago_martes, IsNull(fecha_pago_miercoles, 0) fecha_pago_miercoles, 
                                    IsNull(fecha_pago_jueves, 0) fecha_pago_jueves, IsNull(fecha_pago_viernes, 0) fecha_pago_viernes, 
                                    IsNull(fecha_pago_sabado, 0) fecha_pago_sabado, IsNull(fecha_pago_domingo, 0) fecha_pago_domingo
                                FROM tipo_cliente
                                WHERE id_tipo_cliente =  @id ";

                Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Log("customerTypeId =  " + customerTypeId);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", customerTypeId);
                adp.SelectCommand.Transaction = transaction;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new TipoCliente();

                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.NombreTipoCliente = (ds.Tables[0].Rows[i]["tipo_cliente"].ToString());
                        item.PrestamoInicialMaximo = float.Parse(ds.Tables[0].Rows[i]["prestamo_inicial_maximo"].ToString());
                        item.PorcentajeSemanal = float.Parse(ds.Tables[0].Rows[i]["porcentaje_semanal"].ToString());
                        item.SemanasAPrestar = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        item.GarantiasPorMonto = float.Parse(ds.Tables[0].Rows[i]["garantias_por_monto"].ToString());
                        item.FechasDePago = (ds.Tables[0].Rows[i]["fechas_pago"].ToString());
                        item.CantidadParaRenovar = float.Parse(ds.Tables[0].Rows[i]["cantidad_para_renovar"].ToString());
                        item.SemanasExtra = int.Parse(ds.Tables[0].Rows[i]["semana_extra"].ToString());

                        item.FechaPagoLunes = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_lunes"].ToString());
                        item.FechaPagoMartes = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_martes"].ToString());
                        item.FechaPagoMiercoles = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_miercoles"].ToString());
                        item.FechaPagoJueves = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_jueves"].ToString());
                        item.FechaPagoViernes = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_viernes"].ToString());
                        item.FechaPagoSabado = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_sabado"].ToString());
                        item.FechaPagoDomingo = int.Parse(ds.Tables[0].Rows[i]["fecha_pago_domingo"].ToString());


                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                Log("Error ... " + ex.Message);
                Log(ex.StackTrace);
                throw ex;

            }


        }

        public int InsertLog(string observations, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @" INSERT INTO resumen_calculo_fallas (observaciones, fecha)
                            VALUES (@observaciones, getdate()) ";

                Log("\nMétodo-> " +
         System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@observaciones", observations);
                cmd.Transaction = transaction;

                r = cmd.ExecuteNonQuery();


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return r;

        }

        public int UpdatePymentStatus(int idPago, int idStatus, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @" UPDATE pago SET id_status_pago = @id_status_pago
                            WHERE id_pago = @id_pago ";


                Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_pago", idPago);
                cmd.Parameters.AddWithValue("@id_status_pago", idStatus);
                cmd.Transaction = transaction;

                r = cmd.ExecuteNonQuery();


                Log("Status Pago actualizado  " + (r > 0).ToString());


            }
            catch (Exception ex)
            {
                throw ex;
            }


            return r;

        }

        public object RegistrarLogCambios(string path, int idUsuario, string descripcion, string modulo, SqlConnection conn)
        {

            
            try
            {


                string sql = " INSERT INTO log_cambios(id_usuario, descripcion, fecha_hora, modulo) " +
                                           " VALUES (@id_usuario, @descripcion, @fecha_hora, @modulo)";

                Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@fecha_hora", DateTime.Now);
                cmd.Parameters.AddWithValue("@modulo", modulo);


                int r = cmd.ExecuteNonQuery();



                return r;
            }
            catch (Exception ex)
            {
                Log("Error ... " + ex.Message);
                Log(ex.StackTrace);
                return -1;
            }

    

        }

        public static void Log(string msg)
        {
            System.Diagnostics.Debug.Print(msg);
            Console.WriteLine(msg);

        }


    }

}