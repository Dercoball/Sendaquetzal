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
    public partial class ConfigRules : System.Web.UI.Page
    {
        const string pagina = "17";

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


        /// <summary>
        /// Obtener comisiones o módulos
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Comision> GetItemsCommissions(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Comision> items = new List<Comision>();


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
                string query = @" SELECT c.id_comision, c.porcentaje, c.activo, c.nombre 
                         FROM comision c                          
                         WHERE 
                         ISNull(c.eliminado, 0) = 0
                         ORDER BY id_comision ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Comision item = new Comision();
                        item.IdComision = int.Parse(ds.Tables[0].Rows[i]["id_comision"].ToString());
                        item.Porcentaje = float.Parse(ds.Tables[0].Rows[i]["porcentaje"].ToString());
                        item.Nombre = (ds.Tables[0].Rows[i]["nombre"].ToString());


                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='configComissions.open(" + item.IdComision + ", \"" + item.Nombre + "\")'  class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Configurar</button>";

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
        public static List<ReglaEvaluacionModulo> GetItemsRulesByCommission(string path, string idUsuario, string idComision)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<ReglaEvaluacionModulo> items = new List<ReglaEvaluacionModulo>();


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
                string query = @" SELECT r.id_regla_evaluacion_modulo, r.descripcion, IsNull(r.ponderacion, 0) ponderacion, 
                                    IsNull(r.id_comision, 0) id_comision, c.nombre
                                    FROM regla_evaluacion_modulo r
                                    JOIN comision c ON (c.id_comision = r.id_comision)
                                    WHERE r.id_comision = @id_comision ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_comision", idComision);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        ReglaEvaluacionModulo item = new ReglaEvaluacionModulo();
                        item.IdReglaEvaluacionModulo = int.Parse(ds.Tables[0].Rows[i]["id_regla_evaluacion_modulo"].ToString());
                        item.Ponderacion = int.Parse(ds.Tables[0].Rows[i]["ponderacion"].ToString());
                        item.PonderacionStr = item.Ponderacion + "%";
                        item.Descripcion = (ds.Tables[0].Rows[i]["descripcion"].ToString());
                        item.NombreComision = (ds.Tables[0].Rows[i]["nombre"].ToString());

                        string botones = "";
                        botones += "<button  onclick='configComissions.deleteRule(" + item.IdReglaEvaluacionModulo + ")'   class='btn btn-outline-primary btn-sm ml-1'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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
        public static ReglaEvaluacionModulo GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            ReglaEvaluacionModulo item = new ReglaEvaluacionModulo();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @"  SELECT r.id_regla_evaluacion_modulo, r.descripcion, IsNull(r.ponderacion, 0) ponderacion, 
                                    IsNull(r.id_comision, 0) id_comision, c.nombre
                                    FROM regla_evaluacion_modulo r
                                    WHERE r.id_regla_evaluacion_modulo = @id_regla_evaluacion_modulo ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_comision =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_regla_evaluacion_modulo", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new ReglaEvaluacionModulo();

                        item.IdReglaEvaluacionModulo = int.Parse(ds.Tables[0].Rows[i]["id_regla_evaluacion_modulo"].ToString());
                        item.Ponderacion = int.Parse(ds.Tables[0].Rows[i]["ponderacion"].ToString());
                        item.Descripcion = (ds.Tables[0].Rows[i]["descripcion"].ToString());
                        item.NombreComision = (ds.Tables[0].Rows[i]["nombre"].ToString());



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
        public static object Save(string path, ReglaEvaluacionModulo item, string accion, string idUsuario)
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
                    sql = @" INSERT INTO regla_evaluacion_modulo(descripcion, id_comision, ponderacion) 
                    VALUES (@descripcion, @id_comision, @ponderacion) ";
                }
                else
                {
                    sql = @" UPDATE regla_evaluacion_modulo
                          SET descripcion = @descripcion,
                              id_comision = @id_comision,
                              ponderacion = @ponderacion
                          WHERE 
                              id_regla_evaluacion_modulo = @id";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@descripcion", item.Descripcion);
                cmd.Parameters.AddWithValue("@ponderacion", item.Ponderacion);
                cmd.Parameters.AddWithValue("@id_comision", item.IdComision);
                cmd.Parameters.AddWithValue("@id", item.IdReglaEvaluacionModulo);


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
        public static DatosSalida DeleteRegla(string path, string id, string idUsuario)
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

                string sql = @" DELETE FROM regla_evaluacion_modulo 
                                        WHERE id_regla_evaluacion_modulo = @id ";

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