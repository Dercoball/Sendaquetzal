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
    public partial class Calendar : System.Web.UI.Page
    {
        const string pagina = "58";

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
        public static List<Calendario> GetListaItemsFechas(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Calendario> items = new List<Calendario>();

            try
            {
                conn.Open();
                string query = @" SELECT c.id, c.nombre, 
                                    FORMAT(c.fecha, 'dd/MM/yyyy') fecha_mx,
                                    FORMAT(c.fecha, 'yyyy-MM-dd') fecha,                       
                                    FORMAT(c.fecha, 'yyyy-MM-dd') fecha_final,
                                    c.fecha fecha_orden,
                                    'calendario' as tipo
                                FROM calendario c
                                  WHERE ISNull(eliminado, 0) = 0
                                UNION 
                                SELECT c.id_dias_paro id, concat('Día de paro por ', c.nota) as nombre, 
                                    FORMAT(c.fecha_inicio, 'dd/MM/yyyy') fecha_mx,
                                    FORMAT(c.fecha_inicio, 'yyyy-MM-dd') fecha,                       
                                    FORMAT(c.fecha_fin, 'yyyy-MM-dd') fecha_final,
                                    c.fecha_inicio fecha_orden,
                                    'paro' as tipo
                                FROM dias_paro c
                                  WHERE ISNull(c.eliminado, 0) = 0
                                ORDER BY fecha_orden
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
				items = conn.Query<Calendario>(query).ToList();

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
        public static List<Calendario> GetListaItemsFechasByMonth(string path, string idUsuario, int month, int year, int plaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Calendario> items = new List<Calendario>();

            try
            {
                string conditionPlaza = "";
                if(plaza > 0)
                {
                    conditionPlaza = " AND c.id_plaza = @plaza ";
                }

                conn.Open();
                string query = @" 
                                SELECT c.id Id, 
	                                c.nombre Nombre,
	                                c.fecha Fecha,
	                                c.fecha FechaFinal,
	                                c.es_laboral EsLaboral,
                                    'calendario' as Tipo
                                FROM calendario c
                                    WHERE ISNull(c.eliminado, 0) = 0
                                    AND year(c.fecha) = @year AND month(c.fecha) = @month
                                UNION 
                                SELECT c.id_dias_paro Id, 
	                                c.nota as Nombre, 
                                    c.fecha_inicio Fecha,
	                                c.fecha_fin FechaFinal,
	                                0 EsLaboral,
                                    'paro' as Tipo
                                FROM dias_paro c
                                    WHERE ISNull(c.eliminado, 0) = 0
                                    " + conditionPlaza + @"
                                    AND year(c.fecha_inicio) = @year AND month(c.fecha_inicio) = @month
                                ORDER BY 3
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

				items = conn.Query<Calendario>(query, new { month = (month + 1), year, plaza}).ToList();

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


    }


}