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
                string sqlUstatus = " id_status_pago =  " + Pago.STATUS_PAGO_ABONADO + ", saldo = saldo - @abono ";
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


                //3.  Abonar lo recuperado a el o los pagos en status abono hasta completar lo abonado
                //DataSet ds = new DataSet();
                List<Pago> pagosFalla = GetPaymentsByIdPrestamoAndStatus(itemPago.IdPrestamo.ToString(), new int[] { Pago.STATUS_PAGO_FALLA }, conn, transaccion);


                //  Acomodar el monto recuperado en los registros de fallas
                if (pagosFalla.Count > 0 && recuperado > 0)
                {
                    double abonoCompleto = recuperado;
                    int i = 0;

                    Utils.Log("recuperado ... " + recuperado);

                    while (recuperado > 0 && i < pagosFalla.Count)
                    {
                        Pago pago = pagosFalla[i];
                        Utils.Log("idPago ... " + pago.IdPago);
                        Utils.Log("Monto ... " + pago.Monto);
                        Utils.Log("Saldo ... " + pago.Saldo);

                        double montoAAbonar = recuperado;
                        if (recuperado >= pago.Saldo)
                        {
                            montoAAbonar = pago.Saldo;
                        }
                        Utils.Log("montoAAbonar ... " + montoAAbonar);

                        int rowsUpdateds = UpdatePago(pago.IdPago, montoAAbonar,true, conn, transaccion);

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



                //  Revisar que si todos los pagos  han sido realizados, abonado o pagado normal pasar el status del prestamo a pagado.                    
                List<Pago> pagosDistintosAFalla = GetPaymentsByIdPrestamoAndStatus(itemPago.IdPrestamo.ToString(),
                        new int[] { Pago.STATUS_PAGO_FALLA, Pago.STATUS_PAGO_PENDIENTE }, conn, transaccion);

                if (pagosDistintosAFalla.Count < 1)
                {
                    Utils.Log("Actualizar status de préstamo a Pagado");

                    string nota = "Todos los pagos del préstamo se encuentran cubiertos, el préstamo se pasa a status pagado." + DateTime.Now.ToString("g");

                    int rowsAffectedPrestamo = UpdateStatusPrestamo(itemPago.IdPrestamo.ToString(),  idUsuario.ToString(), nota, conn, transaccion);

                    Utils.Log("rowsAffectedPrestamo  ... " + rowsAffectedPrestamo);

                    //  Pasar el status del cliente a inactivo
                    rowsAffectedPrestamo  = UpdateStatusCustomer(itemPago.IdCliente.ToString(), idUsuario.ToString(), Cliente.STATUS_INACTIVO, conn, transaccion);
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
        public static int UpdatePago(int idPago, double abono, bool esRecuperacion, SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {
                string sqlEsRecuperacion = "";
                if (esRecuperacion)
                {
                    sqlEsRecuperacion += " ,es_recuperado = 1 ";
                }

                string sql = @" UPDATE pago SET pagado = pagado+@abono, saldo = saldo-@abono " + sqlEsRecuperacion + 
                             @" WHERE id_pago = @id_pago ";


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@abono", abono);
                cmd.Parameters.AddWithValue("@id_pago", idPago);
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
        public static List<Pago> GetListaItems(string path, string idUsuario, string idTipoUsuario, string idStatus,
                string fechaInicial, string fechaFinal)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();


            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                //  Traer datos del usuario para saber su id_empleado
                Usuario user = Usuarios.GetUsuario(path, idUsuario);


                //  Filtro status del pago
                var sqlStatus = "";
                if (idStatus != "0")    //  todos
                {
                    sqlStatus = " AND p.id_status_pago = '" + idStatus + "'";
                }


                int idStatusPagoFalla = Pago.STATUS_PAGO_FALLA;    // PARA EL subquery de totalizado de fallas


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr,                                
                                    st.nombre nombre_status_pago, st.color, 

                                    IsNull( (SELECT SUM(f.saldo) FROM pago f WHERE p.id_prestamo = f.id_prestamo 
                                            AND  f.id_status_pago = " + idStatusPagoFalla + @" ) , 0)  total_falla,

                                      (SELECT SUBSTRING( 
                                               (
                                                SELECT ', ' + CAST(p2.numero_semana AS VARCHAR(5)) AS 'data()'
                                                FROM pago p2 WHERE p.id_prestamo = p2.id_prestamo 
                                                AND  p2.id_status_pago = " + idStatusPagoFalla + @" 
                                                FOR XML PATH('') 
                                              ), 2 , 9999)
                                    				) As semanas_falla

                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN status_pago st ON (st.id_status_pago = p.id_status_pago)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + user.IdEmpleado + "  "                                      
                                    + sqlStatus
                                    + " ORDER BY p.id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

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
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00

                        item.TotalFalla = float.Parse(ds.Tables[0].Rows[i]["total_falla"].ToString());
                        item.TotalFallaFormateadoMx = item.TotalFalla.ToString("C2");

                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();

                        item.SemanasFalla = ds.Tables[0].Rows[i]["semanas_falla"].ToString();


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
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return items;
            }

            finally
            {
                conn.Close();
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
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr, tc.semanas_a_prestar, prestamo.id_cliente,            
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
                        item.Monto = Math.Round(float.Parse(ds.Tables[0].Rows[i]["monto"].ToString()), 2);
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00
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
        public static List<Pago> GetPaymentsByIdPrestamo(string path, string idPrestamo, string idUsuario, int numeroSemanaActual)
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
                        //item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        //item.NumeroSemanas = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        //item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.Saldo = float.Parse(ds.Tables[0].Rows[i]["saldo"].ToString());
                        item.Pagado = float.Parse(ds.Tables[0].Rows[i]["pagado"].ToString());

                        if (item.IdStatusPago == 1)//Este aun no se muestra en historial
                        {
                            if (i + 1 <= numeroSemanaActual)
                            {
                                item.SaldoFormateadoMx = item.Monto.ToString("C2");
                            }
                            else
                            {
                                item.SaldoFormateadoMx = "";
                            }
                            item.Color = "#D3D3D3";         //   tono gris

                        }
                        if (item.IdStatusPago == 2)         //  Mostrar lo pagado actualmente
                        {
                            item.SaldoFormateadoMx = item.Pagado.ToString("C2");
                            item.Color = "#F1948A";         // tono Rojo
                        }

                        if (item.IdStatusPago == 3)//status Abonado esta todo pagado, por tanto se muestra el monto total del pago
                        {
                            item.Saldo = item.Monto;
                            item.SaldoFormateadoMx = item.Saldo.ToString("C2");
                            item.Color = "#00A2FF";         //   tono azul
                        }

                        if (item.IdStatusPago == 4)//Mostrar el monto total que fue pagado de manera correcta
                        {
                            item.SaldoFormateadoMx = item.Monto.ToString("C2");
                            item.Color = "#ABEBC6";         // tono verde
                        }

                        //item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();
                        //item.Status = ds.Tables[0].Rows[i]["nombre_status_pago"].ToString();


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