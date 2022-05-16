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
    public partial class Empleados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {



        }

 



        [WebMethod]
        public static List<Empleado> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT e.id_empleado , e.nombre, e.apellido_materno, e.apellido_paterno, e.clave, " +
                    " ISNull(e.activo, 1) activo, " +
                    " d.nombre nombre_departamento,  " +
                    " p.nombre nombre_puesto  " +
                    " FROM  empleado e " +
                    " JOIN puesto p ON (p.id_puesto = e.id_puesto) " +
                    " JOIN departamento d ON (d.id_departamento= e.id_departamento)" +
                    " where isnull(e.eliminado, 0) != 1 ";

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
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();                        
                        item.APaterno = ds.Tables[0].Rows[i]["apellido_paterno"].ToString();                        
                        item.AMaterno    = ds.Tables[0].Rows[i]["apellido_materno"].ToString();                        
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();                        
                        item.NombreDepartamento = ds.Tables[0].Rows[i]["nombre_departamento"].ToString();                        
                        item.NombrePuesto     = ds.Tables[0].Rows[i]["nombre_puesto"].ToString();                        

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='empleado.editar(" + item.IdEmpleado + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='empleado.eliminar(" + item.IdEmpleado + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove'></span>Eliminar</button>";

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
        public static DatosSalida Guardar(string path, Empleado item, string accion)
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
                    sql = " INSERT INTO empleado (nombre, apellido_paterno, apellido_materno, " +
                        " clave, activo, id_departamento, id_puesto)    " +
                          "   VALUES                                                                " +
                          "   (@nombre, @apellido_paterno, @apellido_materno, @clave, @activo, " +
                          " @id_departamento, @id_puesto)                 ";
                }
                else
                {
                    sql = " UPDATE empleado SET     " +
                          " nombre = @nombre,           " +
                          " apellido_paterno = @apellido_paterno,           " +
                          " apellido_materno = @apellido_materno,           " +
                          " id_departamento = @id_departamento,           " +
                          " id_puesto = @id_puesto,           " +
                          " clave = @clave,           " +
                          "    activo = @activo        " +
                          "    WHERE id_empleado = @id_empleado   ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha_ultima_modificacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@apellido_paterno", item.APaterno);
                cmd.Parameters.AddWithValue("@apellido_materno", item.AMaterno);
                cmd.Parameters.AddWithValue("@id_departamento", item.IdDepartamento);
                cmd.Parameters.AddWithValue("@id_puesto", item.IdPuesto);
                cmd.Parameters.AddWithValue("@clave", item.Clave);
                cmd.Parameters.AddWithValue("@activo", item.Activo);
                cmd.Parameters.AddWithValue("@id_empleado", item.IdEmpleado);


                r = cmd.ExecuteNonQuery();

                Utils.Log("Guardado -> OK ");


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = r.ToString ();

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
        public static string GetFoto(string path, string idEmpleado)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            string item = "";
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT fotografia_b64 FROM empleado WHERE id_empleado = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + idEmpleado);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idEmpleado);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    item = ds.Tables[0].Rows[0]["fotografia_b64"].ToString();
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
        public static string GetFotoByIdUsuario(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            string item = "";
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.fotografia_b64 
                            FROM empleado e
                            LEFT JOIN usuario u ON e.id_empleado = u.id_empleado
                            WHERE u.id_usuario = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + idUsuario);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idUsuario);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    item = ds.Tables[0].Rows[0]["fotografia_b64"].ToString();
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
        public static DatosSalida Eliminar(string path, string id)
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

                sql = " UPDATE empleado set eliminado = 1" +
                        " WHERE id_empleado = @id_empleado ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_empleado", id);
                


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
                salida.MensajeError = "No se pudo eliminar el Empleado.";



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
        public static List<Departamento> GetListaItemsDepartamentos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Departamento> items = new List<Departamento>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_departamento , nombre, clave, ISNull(activo, 1) activo" +
                    " FROM  departamento ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Departamento item = new Departamento();
                        item.IdDepartamento = int.Parse(ds.Tables[0].Rows[i]["id_departamento"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


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
        public static List<Puesto> GetListaItemsPuestos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Puesto> items = new List<Puesto>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_puesto , nombre, clave, ISNull(activo, 1) activo FROM  puesto ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Puesto item = new Puesto();
                        item.IdPuesto = int.Parse(ds.Tables[0].Rows[i]["id_puesto"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


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
        public static Empleado GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Empleado item = new Empleado();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_empleado , nombre, apellido_materno," +
                    " apellido_paterno,  clave, ISNull(activo, 1) activo, id_departamento, id_puesto " +
                    "  FROM  empleado where id_empleado =  @id_empleado ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_empleado =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_empleado", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Empleado();

                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.IdPuesto = int.Parse(ds.Tables[0].Rows[i]["id_puesto"].ToString());
                        item.IdDepartamento = int.Parse(ds.Tables[0].Rows[i]["id_departamento"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();

                        item.APaterno = ds.Tables[0].Rows[i]["apellido_paterno"].ToString();
                        item.AMaterno = ds.Tables[0].Rows[i]["apellido_materno"].ToString();
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();



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