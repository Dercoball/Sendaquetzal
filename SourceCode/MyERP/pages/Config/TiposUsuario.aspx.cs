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
    public partial class TiposUsuario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {



        }

 



        [WebMethod]
        public static List<TipoUsuario> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoUsuario> items = new List<TipoUsuario>();


            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                string esSuperUser = idUsuario != "1" ? "   where  id_tipo_usuario <> @usuarioSuperAdmin AND ISNull(eliminado, 0) = 0 " 
                    : "  where ISNull(eliminado, 0) = 0 ";

                string query = " SELECT id_tipo_usuario , nombre, ISNull(activo, 1) activo FROM  tipo_usuario " 
                    + esSuperUser;

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@usuarioSuperAdmin", Usuario.TIPO_USUARIO_SUPER_ADMIN);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TipoUsuario item = new TipoUsuario();
                        item.IdTipoUsuario = int.Parse(ds.Tables[0].Rows[i]["id_tipo_usuario"].ToString());                        
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();                        

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='tiposUsuario.editar(" + item.IdTipoUsuario + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='tiposUsuario.eliminar(" + item.IdTipoUsuario + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove'></span>Eliminar</button>";
                        botones += "&nbsp; <button  onclick='tiposUsuario.permisos(" + item.IdTipoUsuario + ", \"" + item.Nombre  + "\")'   class='btn btn-outline-primary'> <span class='fa fa-key'></span> Permisos</button>";

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
        public static object GuardarPermisosUsuario(string path, List<PermisoUsuario> listaPermisos, string idTipoUsuario)
        {


            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);
            SqlTransaction transaccion = null;

            try
            {


                int r = 0;
                conn.Open();
                transaccion = conn.BeginTransaction();


                string sqlborrar = "";

                sqlborrar = " DELETE permisos_tipo_usuario " +
                        " WHERE id_tipo_usuario = @id ";



                SqlCommand cmd = new SqlCommand(sqlborrar, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", idTipoUsuario);
                cmd.Transaction = transaccion;


                r += cmd.ExecuteNonQuery();
                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");


                string sql = "";
                foreach (var item in listaPermisos)
                {
                    sql = " INSERT INTO permisos_tipo_usuario (id_permiso, id_tipo_usuario) VALUES (@id_permiso, @id_tipo_usuario )";



                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                    SqlCommand cmd2 = new SqlCommand(sql, conn);
                    cmd2.CommandType = CommandType.Text;

                    cmd2.Parameters.AddWithValue("@id_permiso", item.IdPermiso);
                    cmd2.Parameters.AddWithValue("@id_tipo_usuario", idTipoUsuario);

                    cmd2.Transaction = transaccion;

                    r += cmd2.ExecuteNonQuery();
                    Utils.Log("Guardado -> OK ");
                }

                transaccion.Commit();

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
        public static List<PermisoUsuario> ObtenerListaPermisos(string path, string idTipoUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<PermisoUsuario> items = new List<PermisoUsuario>();

            List<PermisoUsuario> listaPermisosUsuario = ObtenerListaPermisosPorTipoUsuario(path, idTipoUsuario);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_permiso,  nombre, nombre_interno, tipo_permiso" +
                    "  FROM permisos " +
                    "  WHERE IsNull(activo, 0) = 1 ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PermisoUsuario item = new PermisoUsuario();
                        item.IdPermiso = int.Parse(ds.Tables[0].Rows[i]["id_permiso"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreInterno = ds.Tables[0].Rows[i]["nombre_interno"].ToString();
                        item.TipoPermiso = ds.Tables[0].Rows[i]["tipo_permiso"].ToString();

                        items.Add(item);


                    }
                }



                HashSet<int> permisosIds = new HashSet<int>(listaPermisosUsuario.Select(x => x.IdPermiso));

                items.RemoveAll(x => permisosIds.Contains(x.IdPermiso));

             


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
        public static List<PermisoUsuario> ObtenerListaPermisosPorTipoUsuario(string path, string idTipoUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<PermisoUsuario> items = new List<PermisoUsuario>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "  SELECT p.nombre, p.id_permiso, p.nombre_recurso, p.nombre_interno, p.tipo_permiso " +
                    " from permisos_tipo_usuario rel_ptu join permisos p on (p.id_permiso = rel_ptu .id_permiso) " +
                    " where rel_ptu.id_tipo_usuario  = @id_tipo_usuario " +
                    " AND IsNull(p.activo, 0) = 1  ";


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("idtipoUsuario =  " + idTipoUsuario);
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_tipo_usuario", idTipoUsuario);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PermisoUsuario item = new PermisoUsuario();
                        item.IdPermiso = int.Parse(ds.Tables[0].Rows[i]["id_permiso"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreInterno = ds.Tables[0].Rows[i]["nombre_interno"].ToString();
                        item.TipoPermiso = ds.Tables[0].Rows[i]["tipo_permiso"].ToString();



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
        public static DatosSalida Guardar(string path, TipoUsuario item, string accion)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();

            int r = 0;
            try
            {


                conn.Open();
                string sql = "";
                if (accion == "nuevo")
                {
                    sql = " INSERT INTO tipo_usuario (nombre, activo, fecha_ultima_modificacion)    " +
                          "   VALUES                                                                " +
                          "   (@nombre, @activo, @fecha_ultima_modificacion)                 ";
                }
                else
                {
                    sql = " UPDATE tipo_usuario SET     " +
                          " nombre = @nombre,           " +
                          "    activo = @activo,        " +
                          "    fecha_ultima_modificacion = @fecha_ultima_modificacion " +
                          "    WHERE id_tipo_usuario = @id_tipo_usuario   ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha_ultima_modificacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@activo", item.Activo);
                cmd.Parameters.AddWithValue("@id_tipo_usuario", item.IdTipoUsuario);


                r = cmd.ExecuteNonQuery();

                Utils.Log("Guardado -> OK ");


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
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
        public static DatosSalida EliminarTipoUsuario(string path, string id)
        {
            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            
            try
            {


                conn.Open();


                string sql = "";

                sql = " UPDATE tipo_usuario set eliminado = 1" +
                        " WHERE id_tipo_usuario = @id_tipo_usuario ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_tipo_usuario", id);
                


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
                salida.MensajeError = "No se pudo eliminar el TipoUsuario.";



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
        public static TipoUsuario GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            TipoUsuario item = new TipoUsuario();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_tipo_usuario , nombre, ISNull(activo, 1) activo" +
                    "  FROM  tipo_usuario where id_tipo_usuario =  @id_tipo_usuario ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_tipo_usuario =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_tipo_usuario", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new TipoUsuario();

                        item.IdTipoUsuario = int.Parse(ds.Tables[0].Rows[i]["id_tipo_usuario"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();



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