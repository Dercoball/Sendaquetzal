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
    public partial class Assets : System.Web.UI.Page
    {
        const string pagina = "54";

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
        public static List<Activo> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Activo> items = new List<Activo>();


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
                string query = @" SELECT a.id_activo, a.descripcion, a.numero_serie, a.costo, a.comentarios, e.nombre as nombre_empleado, c.nombre as tipo
                                         FROM activo a   
                                         JOIN empleado e ON (e.id_empleado = a.id_empleado)
                                         JOIN categoria c ON (c.id = a.id_categoria)
                                         WHERE 
                                         ISNull(a.eliminado, 0) = 0      
                                         ORDER BY a.id_activo
                                ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                adp.SelectCommand.Parameters.AddWithValue("@id_usuario", idUsuario);
                

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Activo item = new Activo();
                        item.Categoria = new Categoria();
                        item.Empleado = new Empleado();

                        item.IdActivo = int.Parse(ds.Tables[0].Rows[i]["id_activo"].ToString());
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.Comentarios = ds.Tables[0].Rows[i]["comentarios"].ToString();
                        item.Categoria.Nombre = ds.Tables[0].Rows[i]["tipo"].ToString();
                        item.Empleado.Nombre = ds.Tables[0].Rows[i]["nombre_empleado"].ToString();

                        string botones = "<button  onclick='asset.edit(" + item.IdActivo + ")'  class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='asset.delete(" + item.IdActivo + ")'   class='btn btn-outline-primary btn-sm'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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
        public static Activo GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Activo item = new Activo();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT a.id_activo, a.descripcion, a.numero_serie, a.costo, a.comentarios, e.nombre as nombre_empleado, e.id_empleado,
                                         c.nombre as tipo, c.id as id_categoria
                                         FROM activo a   
                                         JOIN empleado e ON (e.id_empleado = a.id_empleado)
                                         JOIN categoria c ON (c.id = a.id_categoria)
                                                       
                                         WHERE 
                                         a.id_activo = @id ";

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
                        item = new Activo();

                        item.Categoria = new Categoria();
                        item.Empleado = new Empleado();

                        item.IdActivo = int.Parse(ds.Tables[0].Rows[i]["id_activo"].ToString());
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.Comentarios = ds.Tables[0].Rows[i]["comentarios"].ToString();
                        item.Categoria.Nombre = ds.Tables[0].Rows[i]["tipo"].ToString();
                        item.Categoria.Id = int.Parse(ds.Tables[0].Rows[i]["id_categoria"].ToString());
                        item.Empleado.Nombre = ds.Tables[0].Rows[i]["nombre_empleado"].ToString();
                        item.Empleado.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());




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
        public static object Save(string path, Activo item, string accion, string idUsuario)
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
                    sql = @" INSERT INTO activo(descripcion, numero_serie,costo, comentarios, eliminado, id_empleado, id_categoria) 
                    VALUES (@descripcion, @numero_serie, @costo, @comentarios, 0, @id_empleado, @id_categoria) ";
                }
                else
                {
                    sql = @" UPDATE activo
                          SET descripcion = @descripcion,
                              numero_serie = @numero_serie,
                              costo = @costo,
                              comentarios = @comentarios,
                              id_empleado = @id_empleado,
                              id_categoria = @id_categoria
                          WHERE 
                              id_activo = @id";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@descripcion", item.Descripcion);
                cmd.Parameters.AddWithValue("@numero_serie", item.NumeroSerie);
                cmd.Parameters.AddWithValue("@costo", item.Costo);
                cmd.Parameters.AddWithValue("@comentarios", item.Comentarios);


                cmd.Parameters.AddWithValue("@id_empleado", item.IdEmpleado);
                cmd.Parameters.AddWithValue("@id_categoria", item.IdCategoria);

                cmd.Parameters.AddWithValue("@id", item.IdActivo);



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
        public static List<Empleado> GetListaItemsEmpleados(string path, string idUsuario )
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT em.id_empleado, em.nombre FROM empleado em 
                                	    WHERE id_plaza = 
                                	    (SELECT id_plaza FROM empleado e JOIN usuario u ON (u.id_empleado = e.id_empleado)
                                		WHERE u.id_usuario = @id) AND ISNull(eliminado, 0) = 0";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idUsuario);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Empleado item = new Empleado();
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
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
        public static List<Categoria> GetListaItemsCategorias(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Categoria> items = new List<Categoria>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT c.id, c.nombre FROM categoria c
                                  WHERE ISNull(eliminado, 0) = 0
                                ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Categoria item = new Categoria();
                        item.Id = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
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
        public static DatosSalida Delete(string path, string id, string idUsuario)
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

                string sql = @" UPDATE activo SET eliminado = 1  
                                        WHERE id_activo = @id ";

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




    }



}