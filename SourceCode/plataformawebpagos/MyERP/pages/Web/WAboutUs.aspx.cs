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
    public partial class WAboutUs : System.Web.UI.Page
    {
        const string pagina = "3";

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




        private static string GetTableName(string idTable)
        {

            string tableName = "";
            switch (idTable)
            {
                case "1": tableName = "web_acercade"; break;
                case "2": tableName = "web_terminos_condiciones"; break;
                case "3": tableName = "web_politicas_privacidad"; break;
            }

            return tableName;

        }


        [WebMethod]
        public static object Save(string path, string contenido, string idUsuario, string idTabla)
        {

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            string tableName = GetTableName(idTabla);

            try
            {

                conn.Open();
                string sql = "";

                sql = @" UPDATE " +  tableName + @" 
                           SET info = @info 
                           WHERE id = 1 ";

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@info", contenido);


                int r = cmd.ExecuteNonQuery();
                Utils.Log("Guardado -> OK ");



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }

            finally
            {
                conn.Close();
            }

        }




        [WebMethod]
        public static string LoadContent(string path, string idTabla, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            PreguntaFrecuente item = new PreguntaFrecuente();
            SqlConnection conn = new SqlConnection(strConexion);
            string content = "";

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            string tableName = GetTableName(idTabla);


            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                string query = @" SELECT id, info
                     FROM  " + tableName + " WHERE id =  1 ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {

                    content = ds.Tables[0].Rows[0]["info"].ToString();

                }


                return content;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return content;
            }

            finally
            {
                conn.Close();
            }

        }



        [WebMethod]
        public static string LoadContentPublic(string path, string idTabla)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            PreguntaFrecuente item = new PreguntaFrecuente();
            SqlConnection conn = new SqlConnection(strConexion);
            string content = "";
                 

            string tableName = GetTableName(idTabla);


            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                string query = @" SELECT id, info
                     FROM  " + tableName + " WHERE id =  1 ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {

                    content = ds.Tables[0].Rows[0]["info"].ToString();

                }


                return content;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return content;
            }

            finally
            {
                conn.Close();
            }

        }





    }



}