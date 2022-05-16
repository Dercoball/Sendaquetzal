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
    public partial class Marcas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static List<Marca> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Marca> items = new List<Marca>();

            List<PermisoUsuario> listaPermisos = Usuarios.ObtenerListaPermisosPorUsuario(path, idUsuario);
            PermisoUsuario permisoEditar = listaPermisos.Find(x => x.IdPermiso == 7);
            PermisoUsuario permisoEliminar = listaPermisos.Find(x => x.IdPermiso == 6);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_marca, nombre FROM marca ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                //string salida = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Marca item = new Marca();
                        item.Id_Marca = int.Parse(ds.Tables[0].Rows[i]["id_marca"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();



                        string botones = "<button  title=\"Editar\" onclick='editar(" + item.Id_Marca + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit'></span>Editar</button>";
                        botones += " &nbsp; <button  title=\"Eliminar\" onclick='eliminar(" + item.Id_Marca + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove'></span>Eliminar</button>";

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
        public static object Guardar(string path, Marca marca, string accion)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = "";
                if (accion == "nuevo")
                {
                    sql = " INSERT INTO marca(nombre) " +
                                           " VALUES (@nombre);";
                }
                else
                {
                    sql = " UPDATE marca " +
                          " SET nombre = @nombre " +
                          " WHERE id_marca = @id ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@nombre", SqlDbType.VarChar).Value = marca.Nombre;



                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = marca.Id_Marca;


                int r = cmd.ExecuteNonQuery();
                Utils.Log("Guardado -> OK ");



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1; //Retornamos menos uno cuando se dió por alguna razón un error
            }

            finally
            {
                conn.Close();
            }

        }


        [WebMethod]
        public static object EliminarMarca(string path, string id)
        {

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = "";

                sql = " DELETE marca" +
                        " WHERE id_marca = @id ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;


                int r = cmd.ExecuteNonQuery();
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

        [WebMethod]
        public static Marca GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Marca item = new Marca();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_marca,nombre " +
                               " FROM marca" +
                               " WHERE id_marca= @id ";

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
                        item.Id_Marca = int.Parse(ds.Tables[0].Rows[i]["id_marca"].ToString());

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