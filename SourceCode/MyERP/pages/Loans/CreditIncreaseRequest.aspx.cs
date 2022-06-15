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
    public partial class CreditIncreaseRequest : System.Web.UI.Page
    {
        const string pagina = "16";



        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];



            txtUsuario.Value = usuario;//"promotor.colorado
            txtIdTipoUsuario.Value = idTipoUsuario;//5
            txtIdUsuario.Value = idUsuario;//69


            //  si no esta logueado
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }




        [WebMethod]
        public static List<SolicitudAumentoCredito> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            //  Lista de datos a devolver
            List<SolicitudAumentoCredito> items = new List<SolicitudAumentoCredito>();


            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();


                DataSet ds = new DataSet();
                string query = @" 
                    SELECT s.id_solicitud_aumento_credito, s.id_prestamo, s.fecha, s.id_status_solicitud_aumento_credito, 
		                    p.nombre nombre_plaza, IsNull(pre.monto, 0) monto, promotor.id_plaza, promotor.monto_limite_inicial,
		                    concat(promotor.nombre ,  ' ' , promotor.primer_apellido , ' ' , promotor.segundo_apellido) AS nombre_completo_promotor,
		                    concat(supervisor.nombre ,  ' ' , supervisor.primer_apellido , ' ' , supervisor.segundo_apellido) AS nombre_completo_supervisor
		                    FROM solicitud_aumento_credito s
		                    JOIN prestamo pre ON (s.id_prestamo = pre.id_prestamo)                                            
		                    JOIN empleado promotor ON (promotor.id_empleado = pre.id_empleado)
		                    JOIN empleado supervisor ON (supervisor.id_empleado = promotor.id_supervisor)
                            JOIN plaza p ON (p.id_plaza = promotor.id_plaza)
		                    WHERE s.id_status_solicitud_aumento_credito = 1 ORDER BY s.id_solicitud_aumento_credito ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SolicitudAumentoCredito item = new SolicitudAumentoCredito();
                        item.IdSolicitudAumentoCredito = int.Parse(ds.Tables[0].Rows[i]["id_solicitud_aumento_credito"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NombrePromotor = ds.Tables[0].Rows[i]["nombre_completo_promotor"].ToString();
                        item.NombreSupervisor = ds.Tables[0].Rows[i]["nombre_completo_supervisor"].ToString();

                        item.LimiteCreditoActual = float.Parse(ds.Tables[0].Rows[i]["monto_limite_inicial"].ToString());
                        item.LimiteCreditoActualMx = item.LimiteCreditoActual.ToString("C2");

                        item.LimiteCreditoRequerido = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.LimiteCreditoRequeridoMx = item.LimiteCreditoRequerido.ToString("C2");

                        item.NombrePlaza = ds.Tables[0].Rows[i]["nombre_plaza"].ToString();

                        string botones = "";

                        botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='requests.approve(" + item.IdSolicitudAumentoCredito + "," + item.IdPrestamo + ")'  class='btn btn-outline-success'> <span class='fa fa-check mr-1'></span>Aprobar</button>";
                        botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='requests.reject(" + item.IdSolicitudAumentoCredito + "," + item.IdPrestamo + ")'  class='btn btn-outline-danger ml-1'> <span class='fa fa-times mr-1'></span>Rechazar</button>";

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
        public static int ApproveRequest(string path, string idSolicitud, string idPrestamo, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlTransaction transaction = null;
            SqlConnection conn = new SqlConnection(strConexion);
            int response = 0;


            Utils.Log("\nMétodo-> " +
            System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();


                string nota = "Se aprueba la solicitud de aumento de crédito.";
                response = UpdateStatusPrestamo(idPrestamo, idUsuario, nota, Prestamo.STATUS_PENDIENTE_EJECUTIVO.ToString(), conn, transaction);
                response += UpdateStatuSolicitud(idSolicitud, idUsuario, SolicitudAumentoCredito.STATUS_SOLICITUD_APROBADA.ToString(), conn, transaction);

                transaction.Commit();

                Utils.Log("\nFinalizado correctamente");

            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex_)
                {
                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                }

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

            }

            return response;


        }



        [WebMethod]
        public static int RejectRequest(string path, string idSolicitud, string idPrestamo, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlTransaction transaction = null;
            SqlConnection conn = new SqlConnection(strConexion);
            int response = 0;


            Utils.Log("\nMétodo-> " +
            System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();


                string nota = "Se rechaza la solicitud de aumento de crédito.";
                response = UpdateStatusPrestamo(idPrestamo, idUsuario, nota, Prestamo.STATUS_RECHAZADO.ToString(), conn, transaction);
                response += UpdateStatuSolicitud(idSolicitud, idUsuario, SolicitudAumentoCredito.STATUS_SOLICITUD_RECHAZADA.ToString(), conn, transaction);

                transaction.Commit();

                Utils.Log("\nFinalizado correctamente");
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex_)
                {
                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                }

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

            }

            return response;


        }


        public static int UpdateStatuSolicitud(string idSolicitud, string idUsuario, string idStatus,
            SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {

                string sql = @"  UPDATE solicitud_aumento_credito
                            SET id_status_solicitud_aumento_credito = @id_status_solicitud_aumento_credito, id_usuario = @id_usuario
                            WHERE
                            id_solicitud_aumento_credito = @id_solicitud_aumento_credito ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");


                SqlCommand cmdUpdate = new SqlCommand(sql, conn);
                cmdUpdate.CommandType = CommandType.Text;

                cmdUpdate.Parameters.AddWithValue("@id_solicitud_aumento_credito", idSolicitud);
                cmdUpdate.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdUpdate.Parameters.AddWithValue("@id_status_solicitud_aumento_credito", idStatus);
                cmdUpdate.Transaction = transaction;

                r += cmdUpdate.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;

            }

            return r;

        }


        public static int UpdateStatusPrestamo(string idPrestamo, string idUsuario, string nota, string idStatusPrestamo,
           SqlConnection conn, SqlTransaction transaction)
        {

            int r = 0;
            try
            {



                string sql = @"  UPDATE prestamo
                            SET id_status_prestamo = @id_status_prestamo, notas_generales = @notas_generales, id_usuario = @id_usuario
                            WHERE
                            id_prestamo = @id_prestamo ";

                Utils.Log("\nMétodo-> " +
            System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");


                SqlCommand cmdUpdatePrestamo = new SqlCommand(sql, conn);
                cmdUpdatePrestamo.CommandType = CommandType.Text;

                cmdUpdatePrestamo.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmdUpdatePrestamo.Parameters.AddWithValue("@notas_generales", nota);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdUpdatePrestamo.Parameters.AddWithValue("@id_status_prestamo", idStatusPrestamo);
                cmdUpdatePrestamo.Transaction = transaction;

                r += cmdUpdatePrestamo.ExecuteNonQuery();

            }
            catch (Exception ex)
            {

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                throw ex;

            }


            return r;


        }



    }






}