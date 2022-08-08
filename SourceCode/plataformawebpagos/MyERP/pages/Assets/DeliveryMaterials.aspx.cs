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
    public partial class DeliveryMaterials : System.Web.UI.Page
    {
        const string pagina = "57";

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
        public static List<MaterialEntrega> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<MaterialEntrega> items = new List<MaterialEntrega>();


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
                string query = @"  SELECT m.id_material_entrega, m.material_entregado, m.cantidad, m.costo, FORMAT(m.fecha, 'dd/MM/yyyy') fecha, 
                                         concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) AS nombre_empleado,
                                         c.nombre as tipo
                                         FROM material_entrega m
                                         JOIN empleado e ON (e.id_empleado = m.id_empleado)
                                         JOIN categoria c ON (c.id = m.id_categoria)
                                         WHERE 
                                            e.id_plaza = 
                                	        (SELECT id_plaza FROM empleado e JOIN usuario u ON (u.id_empleado = e.id_empleado)
                                		    WHERE u.id_usuario = @id_usuario)
                                         AND ISNull(m.eliminado, 0) = 0      
                                         ORDER BY m.id_material_entrega
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
                        MaterialEntrega item = new MaterialEntrega();
                        item.Categoria = new Categoria();
                        item.Empleado = new Empleado();

                        item.IdMaterialEntrega = int.Parse(ds.Tables[0].Rows[i]["id_material_entrega"].ToString());
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.MaterialEntregado = ds.Tables[0].Rows[i]["material_entregado"].ToString();
                        item.Cantidad = float.Parse(ds.Tables[0].Rows[i]["cantidad"].ToString());
                        item.Fecha = ds.Tables[0].Rows[i]["fecha"].ToString();
                        item.Categoria.Nombre = ds.Tables[0].Rows[i]["tipo"].ToString();
                        item.Empleado.Nombre = ds.Tables[0].Rows[i]["nombre_empleado"].ToString();

                        string botones = "<button  onclick='deliveryMaterial.edit(" + item.IdMaterialEntrega + ")'  class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='deliveryMaterial.delete(" + item.IdMaterialEntrega + ")'   class='btn btn-outline-primary btn-sm'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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
        public static MaterialEntrega GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            MaterialEntrega item = new MaterialEntrega();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT m.id_material_entrega, m.material_entregado, m.cantidad, m.costo, FORMAT(fecha, 'yyyy-MM-dd') fecha, m.id_empleado, m.id_categoria, 
                                         concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) AS nombre_empleado,
                                         c.nombre as tipo
                                         FROM material_entrega m
                                         JOIN empleado e ON (e.id_empleado = m.id_empleado)
                                         JOIN categoria c ON (c.id = m.id_categoria)
          
                                         WHERE 
                                         m.id_material_entrega = @id ";

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
                        item = new MaterialEntrega();

                        item.Categoria = new Categoria();
                        item.Empleado = new Empleado();

                        item.IdMaterialEntrega = int.Parse(ds.Tables[0].Rows[i]["id_material_entrega"].ToString());
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.MaterialEntregado = ds.Tables[0].Rows[i]["material_entregado"].ToString();
                        item.Cantidad = float.Parse(ds.Tables[0].Rows[i]["cantidad"].ToString());
                        item.Fecha = ds.Tables[0].Rows[i]["fecha"].ToString();
                        item.Categoria.Nombre = ds.Tables[0].Rows[i]["tipo"].ToString();
                        item.Empleado.Nombre = ds.Tables[0].Rows[i]["nombre_empleado"].ToString();
                        item.Categoria.Id = int.Parse(ds.Tables[0].Rows[i]["id_categoria"].ToString());
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



        [WebMethod]
        public static object Save(string path, MaterialEntrega item, string accion, string idUsuario)
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
                    sql = @" INSERT INTO material_entrega(material_entregado, cantidad, costo, fecha, eliminado, id_empleado, id_categoria) 
                    VALUES (@material_entregado, @cantidad, @costo, @fecha, 0, @id_empleado, @id_categoria) ";
                }
                else
                {
                    sql = @" UPDATE material_entrega
                          SET material_entregado = @material_entregado,
                              cantidad = @cantidad,
                              costo = @costo,
                              id_empleado = @id_empleado,
                              id_categoria = @id_categoria
                          WHERE 
                              id_material_entrega = @id";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@cantidad", item.Cantidad);
                cmd.Parameters.AddWithValue("@material_entregado", item.MaterialEntregado);
                cmd.Parameters.AddWithValue("@costo", item.Costo);
                cmd.Parameters.AddWithValue("@fecha", DateTime.Today);


                cmd.Parameters.AddWithValue("@id_empleado", item.IdEmpleado);
                cmd.Parameters.AddWithValue("@id_categoria", item.IdCategoria);

                cmd.Parameters.AddWithValue("@id", item.IdMaterialEntrega);



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
        public static List<Empleado> GetListaItemsEmpleados(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT em.id_empleado, 
                                        concat(em.nombre ,  ' ' , em.primer_apellido , ' ' , em.segundo_apellido) AS nombre_completo
                                        FROM empleado em 
                                	    WHERE em.id_plaza = 
                                	    (SELECT id_plaza FROM empleado e JOIN usuario u ON (u.id_empleado = e.id_empleado)
                                		WHERE u.id_usuario = @id) 
                                        AND ISNull(eliminado, 0) = 0";

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
                        item.Nombre = ds.Tables[0].Rows[i]["nombre_completo"].ToString();

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

                string sql = @" UPDATE material_entrega SET eliminado = 1  
                                        WHERE id_material_entrega = @id ";

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