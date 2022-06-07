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
                Utils.Log("Núm de semanas  " + customerType.SemanasAPrestar);






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




        [WebMethod]
        public static DatosSalida UpdateCustomer(string path, Cliente item, Direccion itemAddress, string accion,
            string idUsuario, string idTipoUsuario, string idPrestamo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


            //verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;



            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();


                string sql = "";

                sql = @"  UPDATE cliente
                                SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                                segundo_apellido = @segundo_apellido, nota_fotografia = @nota_fotografia, 
                                ocupacion = @ocupacion, telefono = @telefono, id_tipo_cliente = @id_tipo_cliente 
                          WHERE
                                id_cliente = @id_cliente ";


                Utils.Log("ACTUALIZAR CLIENTE " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_tipo_cliente", item.IdTipoCliente);
                cmd.Parameters.AddWithValue("@curp", item.Curp);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@primer_apellido", item.PrimerApellido);
                cmd.Parameters.AddWithValue("@segundo_apellido", item.SegundoApellido);
                cmd.Parameters.AddWithValue("@ocupacion", item.Ocupacion);
                cmd.Parameters.AddWithValue("@telefono", item.Telefono);
                cmd.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmd.Parameters.AddWithValue("@nota_fotografia", item.NotaFotografiaCliente);
                //cmd.Parameters.AddWithValue("@nota_fotografia_aval", item.NotaFotografiaAval);
                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();

                //  Guardar direccion cliente
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                                codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo
                            WHERE id_cliente = @id_cliente AND ISNULL(aval, 0) = 0
                        ";


                Utils.Log("ACTUALIZAR DIRECCION CLIENTE " + sql);

                SqlCommand cmdAddressEmployee = new SqlCommand(sql, conn);
                cmdAddressEmployee.CommandType = CommandType.Text;

                cmdAddressEmployee.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmdAddressEmployee.Parameters.AddWithValue("@calleyno", itemAddress.Calle);
                cmdAddressEmployee.Parameters.AddWithValue("@colonia", itemAddress.Colonia);
                cmdAddressEmployee.Parameters.AddWithValue("@municipio", itemAddress.Municipio);
                cmdAddressEmployee.Parameters.AddWithValue("@estado", itemAddress.Estado);
                cmdAddressEmployee.Parameters.AddWithValue("@codigo_postal", itemAddress.CodigoPostal);
                cmdAddressEmployee.Parameters.AddWithValue("@direccion_trabajo", itemAddress.DireccionTrabajo);
                cmdAddressEmployee.Transaction = transaccion;

                r = cmdAddressEmployee.ExecuteNonQuery();

                Utils.Log("Guardado -> OK ");

                DatosSalida dataUpdateNotas = UpdateRelacionPrestamoAprobacion(path, idTipoUsuario, item.NotaCliente, item.NotaAval,
                    idUsuario, idPrestamo, conn, transaccion, 1);


                transaccion.Commit();


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = item.IdCliente.ToString();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



        [WebMethod]
        public static DatosSalida UpdateCustomerAval(string path, Cliente item, Direccion itemAddressAval, string accion, string idUsuario,
            string idTipoUsuario, string idPrestamo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


            //verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;


            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();


                string sql = "";

                sql = @"  UPDATE cliente
                                SET curp_aval = @curp_aval, nombre_aval = @nombre_aval, primer_apellido_aval = @primer_apellido_aval, 
                                segundo_apellido_aval = @segundo_apellido_aval, ocupacion_aval = @ocupacion_aval, nota_fotografia_aval = @nota_fotografia_aval, 
                                telefono_aval = @telefono_aval 
                                WHERE
                                id_cliente = @id_cliente ";


                Utils.Log("ACTUALIZAR CLIENTE " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;


                cmd.Parameters.AddWithValue("@curp_aval", item.CurpAval);
                cmd.Parameters.AddWithValue("@nombre_aval", item.NombreAval);
                cmd.Parameters.AddWithValue("@primer_apellido_aval", item.PrimerApellidoAval);
                cmd.Parameters.AddWithValue("@segundo_apellido_aval", item.SegundoApellidoAval);
                cmd.Parameters.AddWithValue("@telefono_aval", item.TelefonoAval);
                cmd.Parameters.AddWithValue("@ocupacion_aval", item.OcupacionAval);
                cmd.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmd.Parameters.AddWithValue("@nota_fotografia_aval", item.NotaFotografiaAval);

                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();

                //  Guardar direccion aval
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                                codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo
                            WHERE id_cliente = @id_cliente AND ISNULL(aval, 0) = 1
                        ";



                Utils.Log("update customer aval" + sql);

                SqlCommand cmdAddressEmployeeAval = new SqlCommand(sql, conn);
                cmdAddressEmployeeAval.CommandType = CommandType.Text;

                cmdAddressEmployeeAval.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@calleyno", itemAddressAval.Calle);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@colonia", itemAddressAval.Colonia);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@municipio", itemAddressAval.Municipio);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@estado", itemAddressAval.Estado);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@codigo_postal", itemAddressAval.CodigoPostal);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@direccion_trabajo", itemAddressAval.DireccionTrabajo);
                cmdAddressEmployeeAval.Transaction = transaccion;

                r = cmdAddressEmployeeAval.ExecuteNonQuery();

                Utils.Log("Guardado -> OK ");

                DatosSalida dataUpdateNotas = UpdateRelacionPrestamoAprobacion(path, idTipoUsuario, item.NotaCliente,
                    item.NotaAval, idUsuario, idPrestamo, conn, transaccion, 2);


                transaccion.Commit();


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = item.IdCliente.ToString();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="notas"></param>
        /// <param name="idPosicion">Es el puesto de la persona que hace el cambio, supervisor o coordinador</param>
        /// <returns></returns>
        [WebMethod]
        public static DatosSalida UpdateRelacionPrestamoAprobacion(string path, string idPosicion,
                string notaCliente, string notaAval, string idUsuario, string idPrestamo, SqlConnection conn, SqlTransaction transaction,
                int tipo)
        {
            //tipo 1 cliente, 2 aval

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            DatosSalida salida = new DatosSalida();

            int r = 0;
            try
            {

                string sqlActualizaPosicion = idPosicion == Employees.POSICION_SUPERVISOR.ToString() ? ", id_supervisor = @id_supervisor " : ", id_ejecutivo = @id_ejecutivo ";
                string sqlActualizaNotaCliente = tipo == 1 ? ", notas_cliente = @notas_cliente " : " ";
                string sqlActualizaNotaAval = tipo == 2 ? ", notas_aval = @notas_aval " : " ";

                string sql = "";

                sql = @"  UPDATE relacion_prestamo_aprobacion
                                SET fecha = @fecha "
                                + sqlActualizaPosicion
                                + sqlActualizaNotaCliente
                                + sqlActualizaNotaAval
                                + @"
                                WHERE id_prestamo = @id_prestamo AND
                                id_posicion = " + idPosicion + " ";


                Utils.Log("ACTUALIZAR RelacionPrestamoAprobacion " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@id_supervisor", idUsuario);
                cmd.Parameters.AddWithValue("@id_ejecutivo", idUsuario);
                cmd.Parameters.AddWithValue("@notas_cliente", notaCliente == null ? "" : notaCliente);
                cmd.Parameters.AddWithValue("@notas_aval", notaAval == null ? "" : notaAval);
                cmd.Transaction = transaction;

                r += cmd.ExecuteNonQuery();


                Utils.Log("Guardado -> OK ");



                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }


            return salida;


        }




    }



}