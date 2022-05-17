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
    public partial class WTutoriales : System.Web.UI.Page
    {
        const string pagina = "6";

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
        public static List<Tutorial> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Tutorial> items = new List<Tutorial>();


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
                string query = @" SELECT id, titulo, url_video,
                     ISNull(activo, 1) activo 
                     FROM  web_tutorial
                     WHERE ISNull(eliminado, 0) = 0
                        ORDER BY id
                        ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Tutorial item = new Tutorial();
                        item.Id = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Titulo = ds.Tables[0].Rows[i]["titulo"].ToString();
                        item.UrlVideo = ds.Tables[0].Rows[i]["url_video"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='faq.editar(" + item.Id + ")'  class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='faq.eliminar(" + item.Id + ")'   class='btn btn-outline-primary btn-sm'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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



        /**
         * Sin importar los permisos
         */
        [WebMethod]
        public static List<Tutorial> GetListaItemsPublic(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Tutorial> items = new List<Tutorial>();


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id, titulo, url_video,
                     ISNull(activo, 1) activo 
                     FROM  web_tutorial
                     WHERE 
                        ISNull(eliminado, 0) = 0
                        AND
                        ISNull(activo, 1) = 1
                        ORDER BY id
                        ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Tutorial item = new Tutorial();
                        item.Id = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Titulo = ds.Tables[0].Rows[i]["titulo"].ToString();
                        item.UrlVideo = ds.Tables[0].Rows[i]["url_video"].ToString();

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
        public static object Guardar(string path, Tutorial item, string accion, string idUsuario)
        {

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = "";
                if (accion == "nuevo")
                {
                    sql = @" INSERT INTO web_tutorial(titulo, url_video, activo, eliminado) 
                                            VALUES (@titulo, @url_video, @activo, 0) ";
                }
                else
                {
                    sql = " UPDATE web_tutorial " +
                          " SET titulo = @titulo, url_video = @url_video, activo = @activo  " +
                          " WHERE id = @id ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@titulo", item.Titulo);
                cmd.Parameters.AddWithValue("@url_video", item.UrlVideo);
                cmd.Parameters.AddWithValue("@activo", item.Activo);
                cmd.Parameters.AddWithValue("@id", item.Id);


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
        public static DatosSalida Eliminar(string path, string id, string idUsuario)
        {



            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo eliminar el registro.";

                return salida;

            }

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            try
            {

                conn.Open();

                string sql = @" UPDATE web_tutorial SET eliminado = 1  
                                        WHERE id = @id ";

                Utils.Log("\n-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");


                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);

                int r = cmd.ExecuteNonQuery();

                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");

                salida.MensajeError = null;
                salida.CodigoError = 0;

                return salida;
            }
            catch (Exception ex)
            {

                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo eliminar el registro.";



                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return salida;
            }

            finally
            {
                conn.Close();
            }

        }

        [WebMethod]
        public static Tutorial GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Tutorial item = new Tutorial();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id, titulo, url_video,
                     ISNull(activo, 1) activo 
                     FROM  web_tutorial WHERE id =  @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Tutorial();

                        item.Id = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Titulo = ds.Tables[0].Rows[i]["titulo"].ToString();
                        item.UrlVideo = ds.Tables[0].Rows[i]["url_video"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";



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






    }



}