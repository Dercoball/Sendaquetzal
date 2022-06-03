using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class LoanApprove : System.Web.UI.Page
    {
        const string pagina = "13";



        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];



            txtUsuario.Value = usuario;
            txtIdTipoUsuario.Value = idTipoUsuario;
            txtIdUsuario.Value = idUsuario;

            //  si no esta logueado
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }
        [WebMethod]
        public static DatosSalida Approve(string path, string idPrestamo, string idUsuario, string idEmpleado)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();
            DatosSalida response = new DatosSalida();


            SqlTransaction transaccion = null;

            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();

                //  Validaciones



                //  Actualizar status del préstamo
                string sql = @"  UPDATE prestamo
                            SET id_status_prestamo = @id_status_prestamo
                            WHERE
                            id_prestamo = @id_prestamo ";

                Utils.Log("Actualizar NUEVO PRESTAMO " + sql);

                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_ACEPTADO);
                cmdUpdatePrestamo.Transaction = transaccion;
                r += cmdUpdatePrestamo.ExecuteNonQuery();


                //  Traer los datos del préstamo y cliente
                Cliente prestamoData = GetLoanDataByCustomerId(path, idPrestamo, conn, transaccion);
                //prestamoData.IdCliente
                //prestamoData.IdTipoCliente

                //  Tipo de cliente
                TipoCliente customerType = GetCustomerTypeById(path, prestamoData.TipoCliente, conn, transaccion);


                //  Generar calendario de pagos de acuerdo al num. de semanas del tipo de cliente
                Utils.Log( "Núm de semanas  " +  customerType.SemanasAPrestar);






                return response;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                response.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                response.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return response;


        }


        [WebMethod]
        public static TipoCliente GetCustomerTypeById(string path, string id, SqlConnection conn, SqlTransaction transaction)
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

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id=  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

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
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }

            finally
            {
                conn.Close();
            }


        }

        public static Prestamo GetPrestamoById(string path, string idCliente, SqlConnection conn, SqlTransaction transaction)
        {

            Prestamo prestamoData = null;

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_prestamo, fecha_solicitud, monto, 
                                    id_status_prestamo, id_cliente, id_usuario
                                    FROM prestamo WHERE id_prestamo = @id_prestamo
                                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idCliente);
                adp.SelectCommand.Transaction = transaction;

                Utils.Log("\nMétodo-> " +
                        System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        prestamoData = new Prestamo();
                        prestamoData.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        prestamoData.IdStatusPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_status_prestamo"].ToString());
                        prestamoData.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        prestamoData.NombreStatus = "<span class='" + prestamoData.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";
                        prestamoData.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();
                        prestamoData.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        prestamoData.MontoFormateadoMx = prestamoData.Monto.ToString("C2");


                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return prestamoData;
            }

            return prestamoData;

        }

        [WebMethod]
        public static Cliente GetLoanDataByCustomerId(string path, string idCliente, SqlConnection conn, SqlTransaction transaction)
        {

            Cliente item = new Cliente();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente , c.nombre, c.primer_apellido, c.segundo_apellido, c.id_tipo_cliente,
                                concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                c.telefono , c.curp, c.ocupacion, c.activo, 
                                c.curp_aval, c.nombre_aval, c.primer_apellido_aval, c.segundo_apellido_aval, c.ocupacion_aval, c.telefono_aval,
                                tc.tipo_cliente, p.id_prestamo, p.monto,
                                FORMAT(fecha_solicitud, 'yyyy-MM-dd') fecha_solicitud
                                FROM cliente c 
                                JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) 
                                JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                                WHERE c.id_cliente = @id
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_empleado =  " + idCliente);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_cliente", idCliente);
                adp.SelectCommand.Transaction = transaction;

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Cliente();

                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Ocupacion = ds.Tables[0].Rows[i]["ocupacion"].ToString();

                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.CurpAval = ds.Tables[0].Rows[i]["curp_aval"].ToString();
                        item.NombreAval = ds.Tables[0].Rows[i]["nombre_aval"].ToString();
                        item.PrimerApellidoAval = ds.Tables[0].Rows[i]["primer_apellido_aval"].ToString();
                        item.SegundoApellidoAval = ds.Tables[0].Rows[i]["segundo_apellido_aval"].ToString();
                        item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();
                        item.OcupacionAval = ds.Tables[0].Rows[i]["ocupacion_aval"].ToString();

                    }
                }





                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }

            finally
            {
                conn.Close();
            }

        }

        [WebMethod]
        public static DatosSalida ApproveBySupervisor(string path, string idPrestamos, string idUsuario, string idEmpleado)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();
            DatosSalida response = new DatosSalida();


            SqlTransaction transaccion = null;

            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();

                //  Guardar prestamo
                string sql = @"  UPDATE prestamo
                            SET id_status_prestamo = @id_status_prestamo
                            WHERE
                            id_prestamo = @id_prestamo ";

                Utils.Log("Actualizar NUEVO PRESTAMO " + sql);

                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamos);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PENDIENTE_EJECUTIVO);
                cmdUpdatePrestamo.Transaction = transaccion;
                cmdUpdatePrestamo.ExecuteNonQuery();


                return response;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                response.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                response.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return response;


        }


    }



}