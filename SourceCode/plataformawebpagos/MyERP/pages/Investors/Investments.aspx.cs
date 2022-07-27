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
    public partial class Investments : System.Web.UI.Page
    {
        const string pagina = "52";

        public const int TIPO_MOVIMIENTO_INVERSION = 1;
        public const int TIPO_MOVIMIENTO_RETIRO= 2;
        public const int TIPO_MOVIMIENTO_UTILIDAD= 3;

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
        public static List<Inversion> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Inversion> items = new List<Inversion>();


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
                string query = @" SELECT i.id_inversion, i.id_inversionista, i.monto, 
                                         FORMAT(i.fecha, 'dd/MM/yyyy') fecha, 
                                         inv.nombre, inv.porcentaje_interes_anual
                         FROM inversion i  
                         JOIN inversionista inv ON (inv.id_inversionista = i.id_inversionista)                      
                         WHERE 
                         ISNull(i.eliminado, 0) = 0
                         ORDER BY id_inversionista ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Inversion item = new Inversion();
                        item.Inversionista = new Inversionista();
                        item.Inversionista.Nombre = (ds.Tables[0].Rows[i]["nombre"].ToString());
                        item.Inversionista.PorcentajeInteresAnual = float.Parse(ds.Tables[0].Rows[i]["porcentaje_interes_anual"].ToString());
                        item.IdInversion = int.Parse(ds.Tables[0].Rows[i]["id_inversion"].ToString());

                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());

                        item.Fecha = (ds.Tables[0].Rows[i]["fecha"].ToString());

                        string botones = "<button class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Retiro</button>";

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
        public static DatosSalida Save(string path, Inversion item, string accion, string idUsuario)
        {

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();


            SqlTransaction transaction = null;

            int r = 0;
            try
            {

                conn.Open();
                transaction = conn.BeginTransaction();


                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name);

                string sql = "";
                sql = @" INSERT INTO inversion(monto, fecha, id_inversionista, eliminado, id_periodo, utilidades) 
                    OUTPUT INSERTED.id_inversion
                    VALUES (@monto, @fecha, @id_inversionista, 0, @id_periodo, @utilidades) ";

                Utils.Log(sql + "\n");

                int idGenerado = 0;
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;                                        
                    cmd.Parameters.AddWithValue("@monto", item.Monto);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@utilidades", item.Utilidades);
                    cmd.Parameters.AddWithValue("@id_periodo", item.IdPeriodo);
                    cmd.Transaction = transaction;
                    idGenerado = (int)cmd.ExecuteScalar();
                }

                sql = @" INSERT INTO inversion_movimiento
                        (fecha, id_inversion_total, id_tipo_movimiento_inversion, monto, id_usuario) 
                        SELECT @fecha, (SELECT id_inversion_total total FROM inversion_total WHERE id_inversionista = @id_inversionista) AS id_inversion_total,
                               @id_tipo_movimiento_inversion, @monto, @id_usuario ";

                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monto", item.Monto);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@utilidades", item.Utilidades);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_INVERSION);
                    cmd.Parameters.AddWithValue("@id_periodo", item.IdPeriodo);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = cmd.ExecuteNonQuery();
                }

                sql = @" UPDATE inversion_total 
                            SET fecha_modificacion = @fecha_modificacion,
                            monto_total = monto_total + @monto
                         WHERE id_inversionista = @id_inversionista
                        ";
                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@monto", item.Monto);
                    cmd.Parameters.AddWithValue("@fecha_modificacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                Utils.Log("Guardado -> OK ");

                transaction.Commit();

                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idGenerado.ToString();


            }
            catch (Exception ex)
            {

                transaction.Rollback();

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
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
        public static List<Inversionista> GetListaItemsInvestors(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Inversionista> items = new List<Inversionista>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_inversionista, nombre FROM  inversionista WHERE ISNull(eliminado, 0) = 0";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Inversionista item = new Inversionista();
                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
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
        public static List<Periodo> GetListaItemsPeriodos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Periodo> items = new List<Periodo>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_periodo,  IsNull(valor_periodo, 0) valor_periodo, activo
                     FROM periodo
                     WHERE 
                     ISNull(eliminado, 0) = 0
                     ORDER BY id_periodo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Periodo item = new Periodo();
                        item.IdPeriodo = int.Parse(ds.Tables[0].Rows[i]["id_periodo"].ToString());
                        item.ValorPeriodo = int.Parse(ds.Tables[0].Rows[i]["valor_periodo"].ToString());


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