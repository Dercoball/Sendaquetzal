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
    public partial class Messages : System.Web.UI.Page
    {
        const string pagina = "11";

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
        public static List<Mensaje> GetItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            SqlConnection conn = new SqlConnection(strConexion);
            List<Mensaje> items = new List<Mensaje>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT  p.id, p.nombre, p.id_tipo_plantilla, t.nombre nombre_tipo_plantilla
                                    FROM plantilla p 
                                    JOIN tipo_plantilla t ON (t.id_tipo_plantilla = p.id_tipo_plantilla)
                                    WHERE isnull(p.eliminado, 0) != 1  

                                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Mensaje item = new Mensaje();

                        item.IdMensaje = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreTipoPlantilla = ds.Tables[0].Rows[i]["nombre_tipo_plantilla"].ToString();



                        string botones = "<button  onclick='messages.edit(" + item.IdMensaje + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='messages.delete(" + item.IdMensaje + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";


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
        public static object Save(string path, Mensaje item, string accion, string idUsuario)
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
                    sql = @" INSERT INTO plantilla(nombre, contenido, id_tipo_plantilla, id_frecuencia_envio_mensaje, eliminado) 
                                            VALUES (@nombre, @contenido, @id_tipo_plantilla, @id_frecuencia_envio_mensaje, 0) ";
                }
                else
                {
                    sql = @" UPDATE plantilla 
                           SET nombre = @nombre, contenido = @contenido, 
                            id_tipo_plantilla = @id_tipo_plantilla, id_frecuencia_envio_mensaje = @id_frecuencia_envio_mensaje
                           WHERE id = @id ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@contenido", item.Contenido);
                cmd.Parameters.AddWithValue("@id_tipo_plantilla", item.IdTipoPlantilla);
                cmd.Parameters.AddWithValue("@id_frecuencia_envio_mensaje", item.IdFrecuenciaEnvio);
                cmd.Parameters.AddWithValue("@id", item.IdMensaje);


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
        public static DatosSalida Delete(string path, string id, string idUsuario)
        {
            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            try
            {

                conn.Open();

                string sql = "";

                sql = @" UPDATE plantilla set eliminado = 1 WHERE id = @id ";

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
        public static List<TipoPlantilla> GetListaItemsTipoPlantilla(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoPlantilla> items = new List<TipoPlantilla>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_plantilla, nombre FROM  tipo_plantilla WHERE  ISNull(eliminado, 0) = 0   ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TipoPlantilla item = new TipoPlantilla();
                        item.IdTipoPlantilla = int.Parse(ds.Tables[0].Rows[i]["id_tipo_plantilla"].ToString());
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
        public static List<FrecuenciaEnvio> GetListaItemsFrecuenciaEnvio(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<FrecuenciaEnvio> items = new List<FrecuenciaEnvio>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_frecuencia_envio_mensaje, nombre FROM  frecuencia_envio_mensaje WHERE  ISNull(eliminado, 0) = 0   ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        FrecuenciaEnvio item = new FrecuenciaEnvio();
                        item.IdFrecuenciaEnvio = int.Parse(ds.Tables[0].Rows[i]["id_frecuencia_envio_mensaje"].ToString());
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
        public static Mensaje GetItem(string path, string id)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            Mensaje item = new Mensaje();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT  id, nombre, contenido, id_tipo_plantilla, id_frecuencia_envio_mensaje FROM plantilla WHERE id =  @id ";

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

                        item.IdMensaje = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Contenido = ds.Tables[0].Rows[i]["contenido"].ToString();
                        item.IdTipoPlantilla = int.Parse(ds.Tables[0].Rows[i]["id_tipo_plantilla"].ToString());
                        item.IdFrecuenciaEnvio = int.Parse(ds.Tables[0].Rows[i]["id_frecuencia_envio_mensaje"].ToString());

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