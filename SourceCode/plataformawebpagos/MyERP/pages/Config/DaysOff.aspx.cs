using Dapper;
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
    public partial class DaysOff : System.Web.UI.Page
    {
        const string pagina = "49";

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
        public static DiaDeParo GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            DiaDeParo item = new DiaDeParo();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                string query = @" SELECT 
                    id_dias_paro IdDiasParo, 
                    nota Nota, 
                    fecha_inicio FechaInicio,
                     fecha_fin FechaFin, 
                    id_tipo_paro IdTipoParo,
                    id_plaza IdPlaza
                     FROM dias_paro
                     WHERE  id_dias_paro =  @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_puesto =  " + id);

                item = conn.QueryFirstOrDefault<DiaDeParo>(query, new {id = id});

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
        public static object Save(string path, DiaDeParo item, string accion, string idUsuario)
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
                    sql = @" INSERT INTO dias_paro(nota, fecha_inicio, fecha_fin, id_tipo_paro, id_plaza, eliminado) 
                    VALUES (@nota, @fecha_inicio, @fecha_fin, @id_tipo_paro, @id_plaza, 0) ";
                }
                else
                {
                    sql = @" UPDATE dias_paro
                          SET nota = @nota,
                              fecha_inicio = @fecha_inicio,        
                              fecha_fin = @fecha_fin,
                              id_tipo_paro = @id_tipo_paro,
                              id_plaza = @id_plaza
                          WHERE 
                              id_dias_paro = @id";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@nota", item.Nota);
                cmd.Parameters.AddWithValue("@fecha_inicio", item.FechaInicio);
                cmd.Parameters.AddWithValue("@fecha_fin", item.FechaFin);
                cmd.Parameters.AddWithValue("@id_tipo_paro", item.IdTipoParo);
				cmd.Parameters.AddWithValue("@id_plaza", item.IdPlaza);

                cmd.Parameters.AddWithValue("@id", item.IdDiasParo);


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
        public static List<DiaDeParo> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<DiaDeParo> items = new List<DiaDeParo>();


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            try
            {
                conn.Open();
				string query = @"SELECT 
	                            dp.id_dias_paro IdDiasParo,
	                            dp.nota Nota,
	                            dp.fecha_inicio FechaInicio,
	                            dp.fecha_fin FechaFin,
	                            dp.id_tipo_paro IdTipoParo,
	                            dp.id_plaza IdPlaza,
	                            p.nombre Plaza
                            FROM
	                            dias_paro dp 
	                            LEFT JOIN plaza p ON (p.id_plaza = dp.id_plaza)
                            WHERE
	                            ISNULL(dp.eliminado, 0) = 0
                            ORDER BY dp.id_dias_paro";

                items = conn.Query<DiaDeParo>(query).ToList();

                items.ForEach(item =>
                {
                    item.Estatus = item.FechaFin.Date > DateTime.Now.Date ? "Programado" : "Realizado";
                });

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

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

                string sql = @" UPDATE dias_paro SET eliminado = 1  
                                        WHERE id_dias_paro = @id ";

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