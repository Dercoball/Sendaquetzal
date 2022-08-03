using Newtonsoft.Json;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class Investments : System.Web.UI.Page
    {
        const string pagina = "52";

        public const int TIPO_MOVIMIENTO_INVERSION = 1;
        public const int TIPO_MOVIMIENTO_RETIRO = 2;
        public const int TIPO_MOVIMIENTO_UTILIDAD = 3;

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
            if (usuario == "")
            {
                Response.Redirect("Login.aspx");
            }

        }


        [WebMethod]
        public static List<Inversion> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Inversion> items = new List<Inversion>();


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT i.id_inversion, i.id_inversionista, IsNull(i.monto, 0) monto, 
                                         FORMAT(i.fecha, 'dd/MM/yyyy') fecha, i.utilidades,
                                         inv.nombre, inv.porcentaje_interes_anual, 
                                          (SELECT DATEPART(ISO_WEEK, GETDATE())) as numSemanaHoy,
                                         (SELECT DATEPART(ISO_WEEK, i.fecha)) as numSemanaInversion,                                         
                                         p.valor_periodo, IsNull(i.retiro_efectuado, 0) retiro_efectuado
                                         FROM inversion i  
                                         JOIN inversionista inv ON (inv.id_inversionista = i.id_inversionista)                      
                                         JOIN periodo p ON (p.id_periodo = i.id_periodo)                      
                                         WHERE 
                                         ISNull(i.eliminado, 0) = 0
                                         ORDER BY id_inversionista 
                                ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Inversion item = new Inversion();
                        item.Inversionista = new Inversionista();
                        item.Inversionista.Nombre = (ds.Tables[0].Rows[i]["nombre"].ToString());
                        item.Inversionista.PorcentajeInteresAnual = float.Parse(ds.Tables[0].Rows[i]["porcentaje_interes_anual"].ToString());
                        item.IdInversion = int.Parse(ds.Tables[0].Rows[i]["id_inversion"].ToString());
                        item.Utilidades = int.Parse(ds.Tables[0].Rows[i]["utilidades"].ToString());
                        item.NumSemanaHoy = int.Parse(ds.Tables[0].Rows[i]["numSemanaHoy"].ToString());
                        item.NumSemanaInversion = int.Parse(ds.Tables[0].Rows[i]["numSemanaInversion"].ToString());
                        item.PeriodoSemanas = int.Parse(ds.Tables[0].Rows[i]["valor_periodo"].ToString());
                        item.RetiroEfectuado = int.Parse(ds.Tables[0].Rows[i]["retiro_efectuado"].ToString());

                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoMx = item.Monto.ToString("C2");

                        item.Fecha = (ds.Tables[0].Rows[i]["fecha"].ToString());

                        var interes = (item.Inversionista.PorcentajeInteresAnual / 52) * item.PeriodoSemanas; //     52 semanas del año
                        var montoRetiro = item.Monto + (interes * item.Monto / 100);

                        string botones = "<button onclick='retirement.addRetiro(" + item.IdInversionista + "," + item.IdInversion + "," + montoRetiro + ")' class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Retiro</button>";

                        if (item.Utilidades == 1 && item.RetiroEfectuado == 0)
                        {
                            if ((item.NumSemanaHoy - item.NumSemanaInversion) > item.PeriodoSemanas)
                            {
                                item.Accion = botones;
                            }
                        }

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
        public static Inversionista GetDataInvestor(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Inversionista item = new Inversionista();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT i.id_inversionista, i.nombre, i.razon_social, i.rfc, i.porcentaje_interes_anual
                         FROM inversionista i                         
                         WHERE 
                         i.id_inversionista = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_comision =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Inversionista();

                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        item.RazonSocial = ds.Tables[0].Rows[i]["razon_social"].ToString();
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.RFC = ds.Tables[0].Rows[i]["rfc"].ToString();
                        item.PorcentajeInteresAnual = float.Parse(ds.Tables[0].Rows[i]["porcentaje_interes_anual"].ToString());



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


        // Guardar nueva inversión
        [WebMethod]
        public static DatosSalida Save(string path, Inversion item, string accion, string idUsuario)
        {

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();


            SqlTransaction transaction = null;

            var fechaOperacion = DateTime.Now;
            //var fechaOperacion = "2022-05-01";

            int r = 0;
            try
            {

                conn.Open();
                transaction = conn.BeginTransaction();


                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name);

                string sql = "";
                sql = @" INSERT INTO inversion(monto, fecha, id_inversionista, eliminado, id_periodo, utilidades) 
                    OUTPUT INSERTED.id_inversion
                    VALUES (@monto, @fecha, @id_inversionista, 0, @id_periodo, @utilidades) ";

                Utils.Log(sql + "\n");

                int idGenerado = 0;
                int idInversion = 0;
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monto", item.Monto);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@utilidades", item.Utilidades);
                    cmd.Parameters.AddWithValue("@id_periodo", item.IdPeriodo);
                    cmd.Transaction = transaction;
                    idGenerado = idInversion = (int)cmd.ExecuteScalar();
                }

                //

                float nuevoMontoTotalActual = GetNuevoMontoInversionTotalActual(item.IdInversionista.ToString(), item.Monto, transaction, conn, TIPO_MOVIMIENTO_INVERSION);


                //
                sql = @" INSERT INTO inversion_movimiento
                        (fecha, id_inversion_total, id_tipo_movimiento_inversion, monto, id_usuario, balance) 
                        SELECT @fecha, (SELECT id_inversion_total total FROM inversion_total WHERE id_inversionista = @id_inversionista) AS id_inversion_total,
                               @id_tipo_movimiento_inversion, @monto, @id_usuario, @balance ";

                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monto", item.Monto);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    cmd.Parameters.AddWithValue("@utilidades", item.Utilidades);
                    cmd.Parameters.AddWithValue("@balance", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_INVERSION);
                    cmd.Parameters.AddWithValue("@id_periodo", item.IdPeriodo);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = cmd.ExecuteNonQuery();
                }
                //


                //

                sql = @" UPDATE inversion_total 
                            SET fecha_modificacion = @fecha_modificacion,
                            monto_total = @nuevoMontoTotalActual
                         WHERE id_inversionista = @id_inversionista
                        ";
                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@nuevoMontoTotalActual", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@fecha_modificacion", fechaOperacion);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                Utils.Log("Guardado -> OK ");

                transaction.Commit();

                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idInversion.ToString();


            }
            catch (Exception ex)
            {

                transaction.Rollback();

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                salida.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;

        }


        public static Inversion GetInversion(string idInversion, SqlTransaction transaction, SqlConnection conn)
        {
            Inversion item = new Inversion();

            try
            {
                string query = @" SELECT i.id_inversion, i.id_inversionista, IsNull(i.monto, 0) monto, 
                                         FORMAT(i.fecha, 'dd/MM/yyyy') fecha, i.utilidades
                                         FROM inversion i                      
                                         WHERE  i.id_inversion = @id_inversion
                                ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_inversion", idInversion);
                adp.SelectCommand.Transaction = transaction;


                DataSet ds = new DataSet();
                adp.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item.Inversionista = new Inversionista();
                        item.IdInversion = int.Parse(ds.Tables[0].Rows[i]["id_inversion"].ToString());
                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item;

        }

        public static float GetNuevoMontoInversionTotalActual(string idInversionista, float monto, SqlTransaction transaction, SqlConnection conn, int tipoMovimiento)
        {
            try
            {
                string query = @" SELECT 
	                                it.id_inversion_total, it.monto_total, it.id_inversionista
                                FROM inversion_total it                                                      
                                WHERE it.id_inversionista = @id_inversionista ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_inversionista", idInversionista);
                adp.SelectCommand.Transaction = transaction;

                DataSet ds = new DataSet();
                adp.Fill(ds);
                float montoTotalActual = float.Parse(ds.Tables[0].Rows[0]["monto_total"].ToString());
                Utils.Log("montoTotalActual : " + montoTotalActual);

                float nuevoMontoTotalActual = 0;
                if (tipoMovimiento == TIPO_MOVIMIENTO_INVERSION)
                {
                     nuevoMontoTotalActual = montoTotalActual + monto;
                }

                if (tipoMovimiento == TIPO_MOVIMIENTO_RETIRO)
                {
                    nuevoMontoTotalActual = montoTotalActual - monto;
                }


                Utils.Log("nuevoMontoTotalActual : " + nuevoMontoTotalActual);

                return nuevoMontoTotalActual;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public static DatosSalida SaveRetiro(string path, InversionRetiro item, string accion, string idUsuario)
        {

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();

            var fechaOperacion = DateTime.Now;
            //var fechaOperacion = "2022-07-01";

            SqlTransaction transaction = null;

            int r = 0;
            try
            {

                conn.Open();
                transaction = conn.BeginTransaction();


                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name);

                Inversion inversion = GetInversion(item.IdInversion.ToString(), transaction, conn);

                //
                float utilidad = item.Monto - inversion.Monto;

                Utils.Log("\n Retiro " + item.Monto);
                Utils.Log("\n Utilidad " + utilidad);
                Utils.Log("\n Inversion " + inversion.Monto);


                string sql = "";
                sql = @" INSERT INTO inversion_retiro(fecha, id_inversion, id_tipo_movimiento_inversion, monto, id_usuario) 
                    OUTPUT INSERTED.id_inversion_retiro
                    VALUES (@fecha, @id_inversion, @id_tipo_movimiento_inversion, @monto, @id_usuario) ";

                Utils.Log(sql + "\n");

                int idGenerado = 0;
                int idInversionRetiro = 0;
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monto", inversion.Monto);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    cmd.Parameters.AddWithValue("@id_inversion", item.IdInversion);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_RETIRO);


                    cmd.Parameters.AddWithValue("@id_usuario", item.IdUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = idInversionRetiro = (int)cmd.ExecuteScalar();

                }

                //
                float nuevoMontoTotalActual = GetNuevoMontoInversionTotalActual(item.IdInversionista.ToString(), inversion.Monto, transaction, conn, TIPO_MOVIMIENTO_RETIRO);



                sql = @" INSERT INTO inversion_movimiento
                        (fecha, id_inversion_total, id_tipo_movimiento_inversion, monto, id_usuario, balance) 
                        SELECT @fecha, (SELECT id_inversion_total total FROM inversion_total WHERE id_inversionista = @id_inversionista) AS id_inversion_total,
                               @id_tipo_movimiento_inversion, @monto, @id_usuario, @balance ";

                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monto", inversion.Monto);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    cmd.Parameters.AddWithValue("@balance", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_RETIRO);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = cmd.ExecuteNonQuery();
                }


                // utilidad
                sql = @" INSERT INTO inversion_movimiento
                        (fecha, id_inversion_total, id_tipo_movimiento_inversion, monto, id_usuario, balance) 
                        SELECT @fecha, (SELECT id_inversion_total total FROM inversion_total WHERE id_inversionista = @id_inversionista) AS id_inversion_total,
                               @id_tipo_movimiento_inversion, @monto, @id_usuario, @balance ";

                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monto", utilidad);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    cmd.Parameters.AddWithValue("@balance", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_UTILIDAD);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = cmd.ExecuteNonQuery();
                }


                //
                sql = @" UPDATE inversion_total 
                            SET fecha_modificacion = @fecha_modificacion,
                            monto_total = @nuevoMontoTotalActual
                         WHERE id_inversionista = @id_inversionista
                        ";
                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@nuevoMontoTotalActual", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@fecha_modificacion", fechaOperacion);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                // Marcar que la inversion ya se ha retirado mediante un retiro
                sql = @" UPDATE inversion
                            SET retiro_efectuado = 1
                         WHERE id_inversion = @id_inversion
                        ";
                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_inversion", item.IdInversion);
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }
                //

                Utils.Log("Guardado -> OK ");

                transaction.Commit();

                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idInversionRetiro.ToString();


            }
            catch (Exception ex)
            {

                transaction.Rollback();

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                salida.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;

        }

        [WebMethod]
        public static List<Inversionista> GetListaItemsInvestors(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Inversionista> items = new List<Inversionista>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_inversionista, nombre FROM  inversionista WHERE ISNull(eliminado, 0) = 0";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Inversionista item = new Inversionista();
                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();

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
        public static List<Periodo> GetListaItemsPeriodos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Periodo> items = new List<Periodo>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_periodo,  IsNull(valor_periodo, 0) valor_periodo, activo
                     FROM periodo
                     WHERE 
                     ISNull(eliminado, 0) = 0
                     ORDER BY id_periodo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Periodo item = new Periodo();
                        item.IdPeriodo = int.Parse(ds.Tables[0].Rows[i]["id_periodo"].ToString());
                        item.ValorPeriodo = int.Parse(ds.Tables[0].Rows[i]["valor_periodo"].ToString());


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





    }



}