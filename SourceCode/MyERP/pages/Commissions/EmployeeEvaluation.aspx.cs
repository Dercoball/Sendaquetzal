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
    public partial class EmployeeEvaluation : System.Web.UI.Page
    {
        const string pagina = "18";



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
        public static List<Empleado> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado , e.nombre, e.primer_apellido, e.segundo_apellido, 
                     concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) AS nombre_completo,
                     concat(superv.nombre ,  ' ' , superv.primer_apellido , ' ' , superv.segundo_apellido) AS nombre_completo_supervisor,
                     concat(ejec.nombre ,  ' ' , ejec.primer_apellido , ' ' , ejec.segundo_apellido) AS nombre_completo_ejecutivo,
                     FORMAT(e.fecha_ingreso, 'dd/MM/yyyy') fecha_ingreso,
                     plaza.nombre nombre_plaza, comis.nombre nombre_comision
                     FROM empleado e 
                     join empleado superv ON (e.id_supervisor = superv.id_empleado)
                     join empleado ejec ON (superv.id_ejecutivo = ejec.id_empleado)
                     JOIN plaza plaza ON (plaza.id_plaza = e.id_plaza) 
                     JOIN comision comis ON (comis.id_comision = e.id_comision_inicial) 
                     WHERE isnull(e.eliminado, 0) != 1 
                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Empleado item = new Empleado();
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.NombreCompletoSupervisor = ds.Tables[0].Rows[i]["nombre_completo_supervisor"].ToString();
                        item.NombreCompletoEjecutivo = ds.Tables[0].Rows[i]["nombre_completo_ejecutivo"].ToString();
                        item.NombrePlaza = ds.Tables[0].Rows[i]["nombre_plaza"].ToString();
                        item.NombreComision= ds.Tables[0].Rows[i]["nombre_comision"].ToString();
                        item.FechaIngresoMx = ds.Tables[0].Rows[i]["fecha_ingreso"].ToString();

                        string botones = "";
                        botones += "<button  onclick='employee.delete(" + item.IdEmpleado + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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