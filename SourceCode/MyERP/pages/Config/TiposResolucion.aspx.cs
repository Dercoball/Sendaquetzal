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
    public partial class TiposResolucion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static List<TipoResolucion> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoResolucion> items = new List<TipoResolucion>();



            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id, nombre, IsNull(activo, 0) activo,  " +
                    " IsNull(eliminado, 0) eliminado, ultima_modificacion, texto1, texto2, texto3 " +
                    " FROM tipo_resolucion" +
                    " WHERE eliminado = 0 ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TipoResolucion item = new TipoResolucion();
                        item.idTipoResolucion = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.texto1 = ds.Tables[0].Rows[i]["texto1"].ToString();
                        item.texto2 = ds.Tables[0].Rows[i]["texto2"].ToString();
                        item.texto3 = ds.Tables[0].Rows[i]["texto3"].ToString();
                        item.activo = Boolean.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        string checkActivo = (item.activo) ? "checked" : "";
                        item.activoStr = "<input type='checkbox' disabled " + checkActivo + " >";

                        item.eliminado = Boolean.Parse(ds.Tables[0].Rows[i]["eliminado"].ToString());
                        item.ultimaModificacionStr = DateTime.Parse(ds.Tables[0].Rows[i]["ultima_modificacion"].ToString()).ToString("dd/MM/yyyy");



                        string botones = "<button  onclick='tipoResolucion.editar(" + item.idTipoResolucion + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='tipoResolucion.invocarEliminar(" + item.idTipoResolucion + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove'></span>Eliminar</button>";

                        item.accion = botones;

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
        public static object Guardar(string path, TipoResolucion item, string accion, int idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string descripcionLog = "";
                string sql = "";

                if (accion == "nuevo")
                {
                    sql = " INSERT INTO tipo_resolucion(nombre, activo, ultima_modificacion, eliminado,  texto1, texto2, texto3) " +
                                           " VALUES (@nombre, @activo, @ultima_modificacion, 0, @texto1, @texto2, @texto3)";
                    descripcionLog = "Nuevo registro. " +
                        " Valores Nombre=" + item.nombre + " Activo=" + item.activo + " Eliminado=" + item.eliminado;
                }
                else
                {
                    sql = " UPDATE tipo_resolucion " +
                          " SET  nombre = @nombre, activo=@activo, ultima_modificacion=@ultima_modificacion, " +
                          " texto1 = @texto1, texto2 = @texto2, texto3 = @texto3 " +
                          " WHERE id = @id ";

                    TipoResolucion itemAnterior = GetItem(path, item.idTipoResolucion.ToString());

                    descripcionLog = "Edición de registro." +
                        " Valores anteriores Nombre=" + itemAnterior.nombre + " Activo=" + itemAnterior.activo + " Eliminado=" + itemAnterior.eliminado +
                        " texto1 = " + itemAnterior.texto1 + " texto2 = " + itemAnterior.texto2 + " texto3 = " + itemAnterior.texto3 +
                        " Valores nuevos Nombre=" + item.nombre + " Activo=" + item.activo + " Eliminado=" + item.eliminado +
                        " texto1 = " + item.texto1 + " texto2 = " + item.texto2 + " texto3 = " + item.texto3;


                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@nombre", item.nombre);
                cmd.Parameters.AddWithValue("@texto1", item.texto1);
                cmd.Parameters.AddWithValue("@texto2", item.texto2);
                cmd.Parameters.AddWithValue("@texto3", item.texto3);
                cmd.Parameters.AddWithValue("@activo", item.activo);
                cmd.Parameters.AddWithValue("@ultima_modificacion", DateTime.Now);

                cmd.Parameters.AddWithValue("@id", item.idTipoResolucion);


                int r = cmd.ExecuteNonQuery();
                if (r > 0)
                {
                    RegistrarLogCambios(path, idUsuario, descripcionLog);
                }



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
        public static object EliminarItem(string path, string id, int idUsuario)
        {

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = "";

                sql = " UPDATE tipo_resolucion SET eliminado = 1 " +
                            " WHERE id = @id ";


                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;


                int r = cmd.ExecuteNonQuery();

                if (r > 0)
                {
                    string descripcionLog = "Eliminado de registro. " +
                        "Nuevo valor para eliminado=true ";

                    RegistrarLogCambios(path, idUsuario, descripcionLog);
                }



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
        public static TipoResolucion GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            TipoResolucion item = new TipoResolucion();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id, nombre, IsNull(activo, 0) activo, IsNull(eliminado, 0) eliminado, ultima_modificacion" +
                               " , texto1, texto2, texto3 " +
                               " FROM tipo_resolucion " +
                               " WHERE id = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item.idTipoResolucion = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.texto1 = ds.Tables[0].Rows[i]["texto1"].ToString();
                        item.texto2 = ds.Tables[0].Rows[i]["texto2"].ToString();
                        item.texto3 = ds.Tables[0].Rows[i]["texto3"].ToString();
                        item.activo = Boolean.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.eliminado = Boolean.Parse(ds.Tables[0].Rows[i]["eliminado"].ToString());
                        item.ultimaModificacionStr = DateTime.Parse(ds.Tables[0].Rows[i]["ultima_modificacion"].ToString()).ToString("dd/MM/yyyy");

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