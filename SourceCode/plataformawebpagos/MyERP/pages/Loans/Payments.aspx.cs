using Dapper;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class Payments : System.Web.UI.Page
    {
        const string pagina = "15";



        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];



            txtUsuario.Value = usuario;//"promotor.colorado
            txtIdTipoUsuario.Value = idTipoUsuario;//5
            txtIdUsuario.Value = idUsuario;//69


            //  FASE DE PRUEBAS, QUITAR AL FINAL
            //if (usuario == string.Empty)
            //{
            //    txtUsuario.Value = "promotor.colorado";
            //    txtIdTipoUsuario.Value = "5";
            //    txtIdUsuario.Value = "69";
            //}
            //


            //  si no esta logueado
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }


        /// <summary>
        /// Capturar los datos del pago
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idPrestamo"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idPosicion"></param>
        /// <param name="nota"></param>
        /// <returns></returns>
        [WebMethod]
        public static DatosSalida SavePayment(string path, string idPago, string idUsuario, float abono, double recuperado)
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

                //1.  Traer datos del pago
                Pago itemPago = new Pago();

                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr, tc.semanas_a_prestar,            
                                    st.nombre nombre_status_pago, c.id_cliente
                                    FROM pago p
                                    JOIN prestamo prestamo ON (p.id_prestamo = prestamo.id_prestamo)                                            
                                    JOIN status_pago st ON (st.id_status_pago = p.id_status_pago)                                            
                                    JOIN cliente c ON (c.id_cliente = prestamo.id_cliente) 
                                    JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) "
                                    + @" WHERE p.id_pago = @id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_pago", idPago);
                adp.SelectCommand.Transaction = transaccion;

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        itemPago.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        itemPago.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        itemPago.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        itemPago.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        itemPago.NumeroSemanas = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        itemPago.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        itemPago.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        itemPago.Saldo = float.Parse(ds.Tables[0].Rows[i]["saldo"].ToString());
                        itemPago.MontoFormateadoMx = itemPago.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00
                        itemPago.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();
                        itemPago.Status = ds.Tables[0].Rows[i]["nombre_status_pago"].ToString();
                    }
                }


                //2.  Actualizar saldo y status del pago
                string sqlUstatus = " id_status_pago =  " + Pago.STATUS_PAGO_ABONADO + ", saldo = saldo - @abono, pagado = pagado + @abono ";
                if (itemPago.Saldo <= abono)
                {
                    sqlUstatus = " id_status_pago =  " + Pago.STATUS_PAGO_PAGADO + ", pagado = monto, saldo = 0 ";
                }

                string sql = @"  UPDATE pago
                                 SET fecha_registro_pago = @fecha_registro_pago, " +
                                        sqlUstatus +
                                 @" WHERE id_pago =  " + idPago + " ";


                Utils.Log("ACTUALIZAR pago " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@fecha_registro_pago", DateTime.Now);
                cmd.Parameters.AddWithValue("@abono", abono);
                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();


                #region Abono de saldo recuperado
                if (recuperado > 0)
                {
                    //3.  Abonar lo recuperado a el o los pagos en status abono hasta completar lo abonado
                    //DataSet ds = new DataSet();
                    List<Pago> pagosFalla = GetPaymentsByIdPrestamoAndStatus(itemPago.IdPrestamo.ToString(), new int[] { Pago.STATUS_PAGO_FALLA }, conn, transaccion);
                    //  Acomodar el monto recuperado en los registros de fallas
                    if (pagosFalla.Count > 0 && recuperado > 0)
                    {
                        int i = 0;

                        Utils.Log("recuperado ... " + recuperado);

                        while (recuperado > 0 && i < pagosFalla.Count)
                        {
                            Pago pago = pagosFalla[i];
                            Utils.Log("idPago ... " + pago.IdPago);
                            Utils.Log("Monto ... " + pago.Monto);
                            Utils.Log("Saldo ... " + pago.Saldo);

                            double montoAAbonar = recuperado;
							bool completo = false;
							if (recuperado >= pago.Saldo)
                            {
                                montoAAbonar = pago.Saldo;
								completo = true;
							}
                            Utils.Log("montoAAbonar ... " + montoAAbonar);

                            int rowsUpdateds = UpdatePago(pago.IdPago, montoAAbonar, true, completo, conn, transaccion);

                            Utils.Log("rowsAffected de pago " + pago.IdPago + " ... " + rowsUpdateds);

                            recuperado -= montoAAbonar;

                            Utils.Log("recuperado restamte ... " + recuperado);

                            i++;
                        }

                        //  Revisar que si uno de los pagos de la lista de los que estaban en falla, pasan se cubren en su totalidad
                        //  Cambiarlos de status a abonado
                        foreach (var item in pagosFalla)
                        {
                            Utils.Log("Revisar status y actualizar de pago " + item.IdPago);

                            int rowsAfcected = UpdateStatusPago(item.IdPago, conn, transaccion);

                            Utils.Log("rowsAfcected UpdateStatusPago ... " + rowsAfcected);
                        }

                    }

                } 
                #endregion

                //  Revisar que si todos los pagos  han sido realizados, abonado o pagado normal pasar el status del prestamo a pagado.                    
                List<Pago> pagosDistintosAFalla = GetPaymentsByIdPrestamoAndStatus(itemPago.IdPrestamo.ToString(),
                        new int[] { Pago.STATUS_PAGO_FALLA, Pago.STATUS_PAGO_PENDIENTE }, conn, transaccion);

                if (pagosDistintosAFalla.Count < 1)
                {
                    Utils.Log("Actualizar status de préstamo a Pagado");

                    string nota = "Todos los pagos del préstamo se encuentran cubiertos, el préstamo se pasa a status pagado." + DateTime.Now.ToString("g");

                    int rowsAffectedPrestamo = UpdateStatusPrestamo(itemPago.IdPrestamo.ToString(), idUsuario.ToString(), nota, conn, transaccion);

                    Utils.Log("rowsAffectedPrestamo  ... " + rowsAffectedPrestamo);

                    //  Pasar el status del cliente a inactivo
                    rowsAffectedPrestamo = UpdateStatusCustomer(itemPago.IdCliente.ToString(), idUsuario.ToString(), Cliente.STATUS_INACTIVO, conn, transaccion);
                    Utils.Log("rowsAffectedCliente ... " + rowsAffectedPrestamo);


                }

                //
                transaccion.Commit();




                return response;
            }
            catch (Exception ex)
            {
                try
                {
                    transaccion.Rollback();
                }
                catch (Exception ex_)
                {

                    r = -1;
                    response.MensajeError = "Se ha generado un error, no se pudo finalizar la operación.";
                    response.CodigoError = 1;
                }


                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                response.MensajeError = "Se ha generado un error, no se pudo finalizar la operación.";
                response.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return response;


        }


        [WebMethod]
        public static int UpdatePago(int idPago, double abono, bool esRecuperacion, bool esCompleto, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {
                string sqlEsRecuperacion = "";
                string sql = "";
                if (esRecuperacion)
                {
                    sqlEsRecuperacion += " ,fecha_registro_pago = @fecha_registro_pago,es_recuperado = 1 ";
                }
				if (esCompleto)
				{
					sql = @" UPDATE pago SET pagado = monto, saldo = 0 " + sqlEsRecuperacion +
							 @" WHERE id_pago = @id_pago ";
				}
				else
				{
					sql = @" UPDATE pago SET pagado = pagado+@abono, saldo = saldo-@abono " + sqlEsRecuperacion +
							 @" WHERE id_pago = @id_pago ";
				}

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@abono", abono);
                cmd.Parameters.AddWithValue("@id_pago", idPago);
				cmd.Parameters.AddWithValue("@fecha_registro_pago", DateTime.Now);
                cmd.Transaction = transaction;

                r = cmd.ExecuteNonQuery();


                Utils.Log("Pago actualizado -> OK ");


            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;

            }


            return r;

        }


        [WebMethod]
        public static int UpdateStatusPago(int idPago, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @" UPDATE pago SET saldo = 0, pagado = monto, id_status_pago = @id_status_pago
                            WHERE id_pago = @id_pago AND pagado >= monto ";


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_pago", idPago);
                cmd.Parameters.AddWithValue("@id_status_pago", Pago.STATUS_PAGO_ABONADO);
                cmd.Transaction = transaction;

                r = cmd.ExecuteNonQuery();


                Utils.Log("Status Pago actualizado  " + (r > 0).ToString());


            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;

            }


            return r;

        }


        [WebMethod]
        public static int UpdateStatusPagoByPagoAndStatus(string path, int idPago, int idStatus)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            int r = 0;
            try
            {
                conn.Open();

                string sqlPendiente = "";
                string sqlFalla = "";
                string sqlAbonado= "";
                string sqlPagado = "";

                if (idStatus == Pago.STATUS_PAGO_PENDIENTE)
                {
                    sqlPendiente = " id_status_pago = 1, saldo = monto, pagado = 0 ";
                }

                if (idStatus == Pago.STATUS_PAGO_FALLA)
                {
                    sqlFalla = " id_status_pago = 2, saldo = monto, pagado = 0 ";
                }

                if (idStatus == Pago.STATUS_PAGO_ABONADO)
                {
                    sqlAbonado = " id_status_pago = 3, saldo = 100, pagado = monto - 100 ";
                }

                if (idStatus == Pago.STATUS_PAGO_PAGADO)
                {
                    sqlPagado = " id_status_pago = 4, saldo = 0, pagado = monto ";
                }

                string sql = @" UPDATE pago SET " 
                                + sqlPendiente
                                + sqlFalla
                                + sqlAbonado
                                + sqlPagado
                            + @" WHERE id_pago = @id_pago ";


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_pago", idPago);
                r = cmd.ExecuteNonQuery();

                Utils.Log("Status Pago actualizado  "  + idPago + " ... " +  (r > 0).ToString());

            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;

            }
            finally
            {
                conn.Close();
            }

            return r;

        }


        public static List<Pago> GetPaymentsByIdPrestamoAndStatus(string idPrestamo, int[] idsStatus, SqlConnection conn, SqlTransaction transaction)
        {

            List<Pago> items = new List<Pago>();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, IsNull(p.monto, 0) monto, IsNull(p.saldo, 0) saldo, 
                                    IsNull(p.pagado, 0) pagado,
                                    p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr   
                                    FROM pago p
                                    WHERE p.id_prestamo = @id_prestamo
                                        AND p.id_status_pago IN (" + string.Join(",", idsStatus) + @")
                                    ORDER BY p.numero_semana  ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idPrestamo);
                adp.SelectCommand.Transaction = transaction;

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();

                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.IdStatusPago = int.Parse(ds.Tables[0].Rows[i]["id_status_pago"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.Saldo = float.Parse(ds.Tables[0].Rows[i]["saldo"].ToString());
                        item.Pagado = float.Parse(ds.Tables[0].Rows[i]["pagado"].ToString());


                        items.Add(item);

                    }
                }


            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;
            }



            return items;


        }



        [WebMethod]
        public static List<Pago> GetListaItems(
    string path, string idUsuario, string idTipoUsuario, string idStatus,
    string fechaInicial, string fechaFinal, int idPlaza, int idEjecutivo,
    int idSupervisor, int idPromotor, string typeFilter)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // Permisos
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso) return null;

            var items = new List<Pago>();
            using (var conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();

                    // Datos del usuario
                    Usuario user = Usuarios.GetUsuario(path, idUsuario);
                    string sqlUser = "";
                    if (idTipoUsuario != Usuario.TIPO_USUARIO_SUPER_ADMIN.ToString() && idTipoUsuario != Usuario.TIPO_USUARIO_DIRECTOR.ToString())
                    {
                        // Si necesitas restringir por el empleado dueño del préstamo, ajusta aquí.
                        // Ojo: esta condición aplica sobre el préstamo, no sobre el pago.
                        sqlUser = "  AND pre.id_empleado = " + user.IdUsuario + "  ";
                    }

                    // ---- Filtro por Plaza / Árbol de empleados (para aplicar sobre los pagos en el APPLY) ----
                    string sqlPlazaApply = "";
                    if (idPlaza > 0)
                    {
                        var empleados = conn.Query<Empleado>(
                            "SELECT u.id_usuario IdEmpleado, id_plaza IdPlaza, id_posicion IdPosicion, id_supervisor IdSupervisor, id_ejecutivo IdEjecutivo FROM empleado e inner join usuario u on u.id_empleado = e.id_empleado WHERE id_plaza = @plz",
                            new { plz = idPlaza }).ToList();
                        sqlUser = "";
                        List<Empleado> empleadosFiltrados = new List<Empleado>();
                        switch ((typeFilter ?? "").ToLowerInvariant())
                        {
                            case "promotor":
                                empleadosFiltrados = empleados.Where(w => w.IdEmpleado == idPromotor).ToList();
                                break;
                            case "supervisor":
                                var promotores = empleados.Where(w => w.IdSupervisor == idSupervisor && w.IdPosicion == 5).ToList();
                                empleadosFiltrados.AddRange(promotores);
                                break;
                            case "ejecutivo":
                                var supervisores = empleados.Where(w => w.IdEjecutivo == idEjecutivo && w.IdPosicion == 4).ToList();
                                var supIds = supervisores.Select(s => s.IdEmpleado).ToList();
                                var promotoresEjecutivo = empleados.Where(w => supIds.Contains(w.IdSupervisor) && w.IdPosicion == 5).ToList();
                                empleadosFiltrados.AddRange(promotoresEjecutivo);
                                break;
                        }

                        List<int> lista = (empleadosFiltrados.Count > 0 ? empleadosFiltrados : empleados)
                                          .Select(s => s.IdEmpleado).Distinct().ToList();

                        if (lista.Count > 0)
                        {
                            sqlPlazaApply = " AND p.id_usuario IN (" + string.Join(",", lista) + ") ";
                        }
                    }

                    // ---- Rango de fechas (parametrizado) ----
                    DateTime desde;
                    DateTime hasta;
                    if (!DateTime.TryParse(fechaInicial, out desde)) desde = new DateTime(2022, 06, 16);
                    if (!DateTime.TryParse(fechaFinal, out hasta)) hasta = DateTime.Today;

                    // ===== Query sin duplicados =====
                    string query = @"
                        SELECT
                            pre.id_prestamo,
                            CONCAT(c.nombre , ' ', c.primer_apellido , ' ', c.segundo_apellido) AS nombre_completo,
                            c.id_cliente,
                            ISNULL(c.mensaje, 1) AS mensaje,
                            pre.monto,
                            pre.fecha_solicitud,
                            pre.fecha_aprobacion,
                            pre.id_status_prestamo,
                            st.color,
                            st.nombre AS nombre_status_prestamo,

                            -- Semanas en falla (concat)
                            sem.semanas_falla,

                            -- Total en falla
                            ISNULL(falla.total_falla, 0) AS total_falla,

                            -- Último pago registrado (cualquier fecha)
                            up.fecha_ultimo_pago,

                            -- Pago representativo en rango: último por fecha
                            pago_ult.id_pago,
                            pago_ult.numero_semana,
                            pago_ult.montopago
                        FROM prestamo pre
                        INNER JOIN cliente c           ON c.id_cliente = pre.id_cliente
                        INNER JOIN status_prestamo st  ON st.id_status_prestamo = pre.id_status_prestamo

                        -- ÚLTIMO PAGO EN RANGO (elimina duplicados)
                        OUTER APPLY (
                            SELECT TOP (1)
                                p.id_pago,
                                p.numero_semana,
                                p.monto AS montopago
                            FROM pago p
                            WHERE p.id_prestamo = pre.id_prestamo
                              AND p.fecha >= @desde
                              AND p.fecha <  DATEADD(DAY, 1, @hasta)
                              /*** filtro por plaza/arbol (si aplica) ***/
                              " + sqlPlazaApply + @"
                            ORDER BY p.fecha DESC, p.id_pago DESC
                        ) AS pago_ult

                        -- FECHA DEL ÚLTIMO PAGO REGISTRADO (global al préstamo)
                        OUTER APPLY (
                            SELECT MAX(p3.fecha_registro_pago) AS fecha_ultimo_pago
                            FROM pago p3
                            WHERE p3.id_prestamo = pre.id_prestamo
                        ) AS up

                        -- TOTAL EN FALLA
                        OUTER APPLY (
                            SELECT SUM(CASE WHEN f.id_status_pago = 2 THEN f.saldo ELSE 0 END) AS total_falla
                            FROM pago f
                            WHERE f.id_prestamo = pre.id_prestamo
                        ) AS falla

                        -- LISTA DE SEMANAS EN FALLA
                        OUTER APPLY (
                            SELECT
                                STUFF((
                                    SELECT ', ' + CAST(p2.numero_semana AS VARCHAR(5))
                                    FROM pago p2
                                    WHERE p2.id_prestamo = pre.id_prestamo
                                      AND p2.id_status_pago = 2
                                      AND p2.saldo > 0
                                    FOR XML PATH(''), TYPE
                                ).value('.','nvarchar(max)'), 1, 2, '') AS semanas_falla
                        ) AS sem

                        WHERE pre.id_status_prestamo = 4
                        " + sqlUser + @"
                        ";

                    var adp = new SqlDataAdapter(query, conn);
                    adp.SelectCommand.Parameters.AddWithValue("@desde", desde);
                    adp.SelectCommand.Parameters.AddWithValue("@hasta", hasta);

                    DataSet ds = new DataSet();

                    Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name +
                              $"\nQuery:\n{query}\n@desde={desde:yyyy-MM-dd} @hasta={hasta:yyyy-MM-dd}\n");

                    adp.Fill(ds);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var t = ds.Tables[0];
                        foreach (DataRow r in t.Rows)
                        {
                            var item = new Pago();

                            item.IdPrestamo = Convert.ToInt32(r["id_prestamo"]);
                            item.IdCliente = Convert.ToInt32(r["id_cliente"]);
                            item.NombreCliente = Convert.ToString(r["nombre_completo"]);
                            item.Mensaje = Convert.ToInt32(r["mensaje"]);

                            item.MontoPrestamo = Convert.ToSingle(r["monto"]);
                            item.Fecha = Convert.ToDateTime(r["fecha_solicitud"]);
                            item.FechaStr = Convert.ToString(r["fecha_solicitud"]);

                            // Pago representativo (puede venir null)
                            item.IdPago = r["id_pago"] == DBNull.Value ? 0 : Convert.ToInt32(r["id_pago"]);
                            item.NumeroSemana = r["numero_semana"] == DBNull.Value ? 0 : Convert.ToInt32(r["numero_semana"]);
                            item.Monto = r["montopago"] == DBNull.Value ? 0f : Convert.ToSingle(r["montopago"]);
                            item.MontoFormateadoMx = item.Monto.ToString("C2");

                            // Falla
                            item.TotalFalla = r["total_falla"] == DBNull.Value ? 0f : Convert.ToSingle(r["total_falla"]);
                            item.TotalFallaFormateadoMx = item.TotalFalla.ToString("C2");
                            item.SemanasFalla = Convert.ToString(r["semanas_falla"]);

                            // Último pago registrado (puede venir null)
                            if (r["fecha_ultimo_pago"] == DBNull.Value)
                                item.FechaUltimoPago = null;
                            else
                                item.FechaUltimoPago = Convert.ToDateTime(r["fecha_ultimo_pago"]);

                            // Status / color   
                            item.Color = Convert.ToString(r["color"]);
                            item.Status = "<span class='" + item.Color + "'>" +
                                          Convert.ToString(r["nombre_status_prestamo"]) + "</span>";

                            // Botón
                            string botones = "<button data-idcliente='" + item.IdCliente + "' " +
                                             "data-idprestamo='" + item.IdPrestamo + "' " +
                                             "onclick='payments.view(" + item.IdPago + ")' " +
                                             "class='btn btn-outline-primary'>" +
                                             "<span class='fa fa-folder-open mr-1'></span>Abrir</button>";
                            item.Accion = botones;

                            items.Add(item);
                        }
                    }

                    return items;
                }
                catch (Exception ex)
                {
                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                    return items;
                }
            }
        }





        [WebMethod]
        public static Pago GetPayment(string path, string idPago, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            Pago item = new Pago();


            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr, tc.semanas_a_prestar, prestamo.id_cliente, prestamo.monto montoprestamo, 
                                     IsNull( (SELECT SUM(f.saldo) FROM pago f WHERE f.id_prestamo = prestamo.id_prestamo AND f.numero_semana <= p.numero_semana) , 0)  saldopendiente,
                                    st.nombre nombre_status_pago
                                    FROM pago p
                                    JOIN prestamo prestamo ON (p.id_prestamo = prestamo.id_prestamo)                                            
                                    JOIN status_pago st ON (st.id_status_pago = p.id_status_pago)                                            
                                    JOIN cliente c ON (c.id_cliente = prestamo.id_cliente) 
                                    JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) "
									+ @" WHERE p.id_pago = @id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_pago", idPago);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.IdStatusPago = int.Parse(ds.Tables[0].Rows[i]["id_status_pago"].ToString());
                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NumeroSemanas = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.MontoPrestamo = Math.Round(float.Parse(ds.Tables[0].Rows[i]["montoprestamo"].ToString()), 2);
						item.MontoPrestamoFormateadoMx = item.MontoPrestamo.ToString("C2");
						item.Monto = Math.Round(float.Parse(ds.Tables[0].Rows[i]["monto"].ToString()), 2);
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00
						item.Saldo = float.Parse(ds.Tables[0].Rows[i]["saldo"].ToString());
                        item.SaldoFormateadoMx = item.Saldo.ToString("C2");
						item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();
                        item.Status = ds.Tables[0].Rows[i]["nombre_status_pago"].ToString();



                    }
                }


            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }

            finally
            {
                conn.Close();
            }

            return item;


        }


        [WebMethod]
        public static List<Pago> GetPaymentsByIdPrestamo(string path, string idPrestamo, string idUsuario, int numeroSemanaActual,
                string idTipoUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            List<Pago> items = new List<Pago>();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, IsNull(p.monto, 0) monto, IsNull(p.saldo, 0) saldo, IsNull(p.pagado, 0) pagado,
                                    p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr, tc.semanas_a_prestar,            
                                    st.nombre nombre_status_pago
                                    FROM pago p
                                    JOIN prestamo prestamo ON (p.id_prestamo = prestamo.id_prestamo)                                            
                                    JOIN status_pago st ON (st.id_status_pago = p.id_status_pago)                                            
                                    JOIN cliente c ON (c.id_cliente = prestamo.id_cliente) 
                                    JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) "
                                    + @" WHERE prestamo.id_prestamo = @id_prestamo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idPrestamo);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();

                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.IdStatusPago = int.Parse(ds.Tables[0].Rows[i]["id_status_pago"].ToString());
                        
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.Saldo = float.Parse(ds.Tables[0].Rows[i]["saldo"].ToString());
                        item.Pagado = float.Parse(ds.Tables[0].Rows[i]["pagado"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());

                        if (item.IdStatusPago == Pago.STATUS_PAGO_PENDIENTE)//Este aun no se muestra en historial
                        {
							item.SaldoFormateadoMx = "-";
							item.Color = "transparent";         //   tono gris

                        }

                        if (item.IdStatusPago == Pago.STATUS_PAGO_FALLA)         //  Mostrar lo pagado actualmente
                        {
                            item.SaldoFormateadoMx = item.Pagado.ToString("C2");
                            item.Color = "#F1948A";         // tono Rojo
                        }

                        if (item.IdStatusPago == Pago.STATUS_PAGO_ABONADO)//status Abonado esta todo pagado, por tanto se muestra el monto total del pago
                        {
                            item.Saldo = item.Monto;
                            item.SaldoFormateadoMx = item.Saldo.ToString("C2");
                            item.Color = "#00A2FF";         //   tono azul
                        }

                        if (item.IdStatusPago == Pago.STATUS_PAGO_PAGADO)//Mostrar el monto total que fue pagado de manera correcta
                        {
                            item.SaldoFormateadoMx = item.Monto.ToString("C2");
                            item.Color = "#ABEBC6";         // tono verde
                        }



                        string botones = "";
                        //if (idTipoUsuario == Usuario.TIPO_USUARIO_SUPER_ADMIN.ToString())    //superuser
                        //{
                        //    botones += "<div class=\"btn-group\" role=\"group\" aria-label=\"Acciones\">";
                        //    botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='payments.updatePendiente(" + item.IdPago + ")'  class='btn btn-secondary btn-sm'> <span class='fa fa-check-circle mr-1'></span>Pe</button>";
                        //    botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='payments.updateFalla(" + item.IdPago + ")'  class='btn btn-secondary btn-sm'> <span class='fa fa-check-circle mr-1'></span>Fa</button>";
                        //    botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='payments.updateAbonado(" + item.IdPago + ")'  class='btn btn-secondary btn-sm'> <span class='fa fa-check-circle mr-1'></span>Ab</button>";
                        //    botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='payments.updatePagado(" + item.IdPago + ")'  class='btn btn-secondary btn-sm'> <span class='fa fa-check-circle mr-1'></span>Pa</button>";
                        //    botones += "</div>";
                        //}

                        item.Accion = botones;

                        items.Add(item);

                    }
                }


            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }

            finally
            {
                conn.Close();
            }

            return items;


        }


        public static int UpdateStatusPrestamo(string idPrestamo, string idUsuario, string nota,
            SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @"  UPDATE prestamo
                            SET id_status_prestamo = @id_status_prestamo, notas_generales = @notas_generales, id_usuario = @id_usuario
                            WHERE
                            id_prestamo = @id_prestamo ";


                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmdUpdatePrestamo.Parameters.AddWithValue("@notas_generales", nota);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PAGADO);
                cmdUpdatePrestamo.Transaction = transaction;

                r += cmdUpdatePrestamo.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;

            }


            return r;


        }


        public static int UpdateStatusCustomer(string idCliente, string idUsuario, int idStatus,
            SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @"  UPDATE cliente
                            SET id_status_cliente = @id_status_cliente, id_usuario = @id_usuario
                            WHERE
                                id_cliente = @id_cliente ";


                using (SqlCommand cmdUpdate = new SqlCommand(sql, conn))
                {
                    cmdUpdate.CommandType = CommandType.Text;

                    cmdUpdate.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmdUpdate.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdUpdate.Parameters.AddWithValue("@id_status_cliente", idStatus);
                    cmdUpdate.Transaction = transaction;

                    r += cmdUpdate.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;

            }


            return r;


        }


    }






}