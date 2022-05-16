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
    public partial class Ubicaciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        [WebMethod]
        public static List<Ubicacion> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Ubicacion> items = new List<Ubicacion>();



            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_ubicacion, nombre, IsNull(activo, 0) activo,  " +
                    " IsNull(eliminado, 0) eliminado, ultima_modificacion FROM ubicacion" +
                    " WHERE eliminado = 0 ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Ubicacion item = new Ubicacion();
                        item.IdUbicacion = int.Parse(ds.Tables[0].Rows[i]["id_ubicacion"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        string checkActivo = (item.Activo == 1) ? "checked" : "";
                        item.ActivoStr = "<input type='checkbox' disabled " + checkActivo + " >";

                        item.Eliminado = int.Parse(ds.Tables[0].Rows[i]["eliminado"].ToString());
                        item.UltimaModificacionStr = DateTime.Parse(ds.Tables[0].Rows[i]["ultima_modificacion"].ToString()).ToString("dd/MM/yyyy");



                        string botones = "<button  onclick='editar(" + item.IdUbicacion + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='eliminar(" + item.IdUbicacion + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove'></span>Eliminar</button>";

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


        public static object RegistrarLogCambios(string path, int idUsuario, string descripcion)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = " INSERT INTO log_cambios(id_usuario, descripcion, fecha_hora) " +
                                           " VALUES (@id_usuario, @descripcion, @fecha_hora)";

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@fecha_hora", DateTime.Now);


                int r = cmd.ExecuteNonQuery();



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
        public static object Guardar(string path, Ubicacion item, string accion, int idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = "";
                string descripcionLog = "";

                if (accion == "nuevo")
                {
                    sql = " INSERT INTO ubicacion(nombre, activo, ultima_modificacion, eliminado) " +
                                           " VALUES (@nombre, @activo, @ultima_modificacion, 0)";
                    descripcionLog = "Nuevo registro. " +
                        " Valores Nombre=" + item.Nombre + " Activo=" + item.Activo + " Eliminado=" + item.Eliminado;
                }
                else
                {
                    sql = " UPDATE Ubicacion " +
                          " SET  nombre = @nombre, activo=@activo, ultima_modificacion=@ultima_modificacion " +
                          " WHERE id_ubicacion = @id_ubicacion ";

                    Ubicacion itemAnterior = GetItem(path, item.IdUbicacion.ToString());

                    descripcionLog = "Edición de registro." +
                        " Valores anteriores Nombre=" + itemAnterior.Nombre + " Activo=" + itemAnterior.Activo + " Eliminado=" + itemAnterior.Eliminado +
                        " Valores nuevos Nombre=" + item.Nombre + " Activo=" + item.Activo + " Eliminado=" + item.Eliminado;

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@activo", item.Activo);
                cmd.Parameters.AddWithValue("@ultima_modificacion", DateTime.Now);

                cmd.Parameters.AddWithValue("@id_ubicacion", item.IdUbicacion);


                int r = cmd.ExecuteNonQuery();

                if (r > 0)
                {
                    RegistrarLogCambios(path, idUsuario, descripcionLog);
                }

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
        public static Ubicacion GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Ubicacion item = new Ubicacion();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_ubicacion, nombre, IsNull(activo, 0) activo, IsNull(eliminado, 0) eliminado, ultima_modificacion " +
                               " FROM ubicacion " +
                               " WHERE id_ubicacion = @id_ubicacion ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_ubicacion", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item.IdUbicacion = int.Parse(ds.Tables[0].Rows[i]["id_ubicacion"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Eliminado = int.Parse(ds.Tables[0].Rows[i]["eliminado"].ToString());
                        item.UltimaModificacionStr = DateTime.Parse(ds.Tables[0].Rows[i]["ultima_modificacion"].ToString()).ToString("dd/MM/yyyy");


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
        public static object EliminarItem(string path, string id, int idUsuario)
        {

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();
                string sql = "";

                sql = " UPDATE ubicacion SET eliminado = 1 " +
                        " WHERE id_ubicacion = @id_ubicacion ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@id_ubicacion", SqlDbType.VarChar).Value = id;


                int r = cmd.ExecuteNonQuery();

                if (r > 0)
                {
                    string descripcionLog = "Eliminado de registro de ubicacion. ID = " + id + 
                        " Nuevo valor para eliminado=true ";

                    RegistrarLogCambios(path, idUsuario, descripcionLog);
                }

                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");



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


    }
}