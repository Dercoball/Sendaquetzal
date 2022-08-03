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
    public partial class Utilities : System.Web.UI.Page
    {
        const string pagina = "53";

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
        public static object GetListaItems(string path, string idUsuario, string idInversionista)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Inversionista> items = new List<Inversionista>();


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
                string query = @" SELECT 
	                                it.id_inversion_total, it.monto_total, it.id_inversionista, 
                                    FORMAT(it.monto_total, 'C2') monto_totalmx,
                                    FORMAT(im.fecha, 'dd/MM/yyyy') fecha, im.id_retiro, im.id_tipo_movimiento_inversion, im.monto,
                                    FORMAT(im.monto, 'C2') montomx,
                                    FORMAT(im.balance, 'C2') balancemx,
                                    tipo.nombre nombre_tipo_movimiento_inversion
                                FROM inversion_total it
                                JOIN inversion_movimiento im ON (im.id_inversion_total = it.id_inversion_total)
                                JOIN tipo_movimiento_inversion tipo ON (tipo.id_tipo_movimiento_inversion = im.id_tipo_movimiento_inversion)                                                       
                                WHERE it.id_inversionista = @id_inversionista                          
                                ORDER BY im.fecha  ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_inversionista", idInversionista);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
               

                var json = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
                return json;
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




    }



}