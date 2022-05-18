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
    public partial class Departamentos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {



        }

 



        [WebMethod]
        public static List<Departamento> GetListaItems(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Departamento> items = new List<Departamento>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT ISNull(d.id_residente, 1) id_residente,  " +
                    "   d.id_departamento, d.nombre, d.clave, ISNull(d.activo, 1) activo, " +
                    "  e.nombre + ' ' + e.apellido_paterno + ' '  + e.apellido_materno as nombre_residente " +
                    "  FROM departamento d " +
                    "  LEFT JOIN empleado e ON (d.id_residente = e.id_empleado)" +
                    " WHERE ISNULL(d.eliminado, 0) != 1 ";

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
                        item.IdResidente = int.Parse(ds.Tables[0].Rows[i]["id_residente"].ToString());                        
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();                        
                        item.NombreResidente = ds.Tables[0].Rows[i]["nombre_residente"].ToString();
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();                        

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='departamento.editar(" + item.IdDepartamento + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='departamento.eliminar(" + item.IdDepartamento + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove'></span>Eliminar</button>";
                        botones += "&nbsp; <button  onclick='departamento.asignar(" + item.IdDepartamento + " , " + item.IdResidente + ")'   class='btn btn-outline-primary'> <span class='fa fa-user'></span>Asignar residente</button>";

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
        public static string GetListaResidentes(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();
            string salida = "";

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_empleado , nombre, apellido_materno, apellido_paterno, id_puesto, id_departamento, " +
                    " clave + ' ' +  nombre + ' ' +  apellido_paterno + ' ' +  apellido_materno as nombre_completo,  " +
                    " clave, ISNull(activo, 1) activo " +
                    " FROM  empleado WHERE  ISNull(activo, 1) = 1 "; // residente de obra

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
                        item.PrimerApellido = ds.Tables[0].Rows[i]["apellido_paterno"].ToString();
                        item.SegundoApellido  = ds.Tables[0].Rows[i]["apellido_materno"].ToString();
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();
                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";



                        items.Add(item);



                    }
                    salida = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);

                }


                return salida;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return "";
            }

            finally
            {
                conn.Close();
            }

        }




        [WebMethod]
        public static DatosSalida UpdateResidente(string path, string idResidente, string idObra)
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

                sql = " UPDATE departamento set id_residente = @id_residente  " +
                        " WHERE id_departamento = @id_departamento ";

                Utils.Log("sql = " + sql);
                Utils.Log("idObra = " + idObra);
                Utils.Log("idresidente  = " + idResidente);


                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_departamento", idObra);
                cmd.Parameters.AddWithValue("@id_residente", idResidente);

                int r = cmd.ExecuteNonQuery();

                Utils.Log("r = " + r);
                Utils.Log("Actualizado -> OK ");

                salida.MensajeError = null;
                salida.CodigoError = 0;

                return salida;
            }
            catch (Exception ex)
            {

                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo actualizar.";



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
        public static DatosSalida Guardar(string path, Departamento item, string accion)
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
                    sql = " INSERT INTO departamento (nombre, clave, activo)    " +
                          "   VALUES                                                                " +
                          "   (@nombre, @clave, @activo)                 ";
                }
                else
                {
                    sql = " UPDATE departamento SET     " +
                          " nombre = @nombre,           " +
                          " clave = @clave,           " +
                          "    activo = @activo        " +
                          "    WHERE id_departamento = @id_departamento   ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha_ultima_modificacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@clave", item.Clave);
                cmd.Parameters.AddWithValue("@activo", item.Activo);
                cmd.Parameters.AddWithValue("@id_departamento", item.IdDepartamento);


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

                sql = " UPDATE departamento set eliminado = 1" +
                        " WHERE id_departamento = @id_departamento ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_departamento", id);
                


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
                salida.MensajeError = "No se pudo eliminar el Departamento.";



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
        public static Departamento GetItemByClave(string path, string clave)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Departamento item = new Departamento();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_departamento , nombre, clave, ISNull(activo, 1) activo " +
                    "  FROM  departamento where clave =  @clave ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("clave depto =  " + clave);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@clave", clave);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Departamento();

                        item.IdDepartamento = int.Parse(ds.Tables[0].Rows[i]["id_departamento"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
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


        [WebMethod]
        public static Departamento GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Departamento item = new Departamento();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_departamento , nombre, clave, ISNull(activo, 1) activo " +
                    "  FROM  departamento where id_departamento =  @id_departamento ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_departamento =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_departamento", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Departamento();

                        item.IdDepartamento = int.Parse(ds.Tables[0].Rows[i]["id_departamento"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
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