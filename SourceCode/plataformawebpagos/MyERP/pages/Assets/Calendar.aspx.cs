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
                DataSet ds = new DataSet();
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

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Calendario item = new Calendario();
                        item.Id = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.FechaMx = ds.Tables[0].Rows[i]["fecha_mx"].ToString();
                        //item.Fecha = ds.Tables[0].Rows[i]["fecha"].ToString();
                        item.FechaFinal = ds.Tables[0].Rows[i]["fecha_final"].ToString();
                        item.Tipo = ds.Tables[0].Rows[i]["tipo"].ToString();

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
        public static List<Calendario> GetListaItemsFechasByMonth(string path, string idUsuario, string month)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Calendario> items = new List<Calendario>();

            int monthInt = int.Parse(month) + 1;

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" 
                                SELECT c.id, c.nombre, 
                                    FORMAT(c.fecha, 'dd/MM/yyyy') fecha_mx,
                                    FORMAT(c.fecha, 'yyyy-MM-dd') fecha,                       
                                    FORMAT(c.fecha, 'yyyy-MM-dd') fecha_final,
                                    c.fecha fecha_orden,
                                    'calendario' as tipo
                                FROM calendario c
                                  WHERE ISNull(c.eliminado, 0) = 0
                                    AND month(c.fecha) = @month
                                UNION 
                                SELECT c.id_dias_paro id, concat('Día de paro por ', c.nota) as nombre, 
                                    FORMAT(c.fecha_inicio, 'dd/MM/yyyy') fecha_mx,
                                    FORMAT(c.fecha_inicio, 'yyyy-MM-dd') fecha,                       
                                    FORMAT(c.fecha_fin, 'yyyy-MM-dd') fecha_final,
                                    c.fecha_inicio fecha_orden,
                                    'paro' as tipo
                                FROM dias_paro c
                                  WHERE ISNull(c.eliminado, 0) = 0
                                    AND @month BETWEEN month(c.fecha_inicio) AND month(c.fecha_fin)
                                ORDER BY fecha_orden


                                ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@month", monthInt);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Calendario item = new Calendario();
                        item.Id = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.FechaMx = ds.Tables[0].Rows[i]["fecha_mx"].ToString();
                        //item.Fecha = ds.Tables[0].Rows[i]["fecha"].ToString();
                        item.FechaLarga = DateTime.Parse(ds.Tables[0].Rows[i]["fecha"].ToString()).ToLongDateString();
                        item.FechaFinal = ds.Tables[0].Rows[i]["fecha_final"].ToString();
                        item.Tipo = ds.Tables[0].Rows[i]["tipo"].ToString();

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


    }


}