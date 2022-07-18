using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class Customers : System.Web.UI.Page
    {
        const string pagina = "21";



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
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }





        [WebMethod]
        public static List<Cliente> GetItems(string path, string idUsuario, string idTipoUsuario, string idStatus)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            List<Cliente> items = new List<Cliente>();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();


                //  Filtro status 
                var sqlStatus = "";
                if (idStatus != "-1")
                {
                    sqlStatus = " AND c.id_status_cliente = '" + idStatus + "'";
                }


                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente,
                     concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                     c.telefono , c.curp, c.ocupacion,
                     IsNull(c.id_status_cliente, 2) id_status_cliente,
                     st.nombre nombre_status_cliente, st.color, p.id_prestamo
                     FROM cliente c 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     JOIN status_cliente st ON (st.id_status_cliente = c.id_status_cliente)                   
                    "
                    + sqlStatus
                    + @" AND p.id_prestamo =   
                      (SELECT TOP 1 pp.id_prestamo FROM prestamo pp WHERE pp.id_cliente = c.id_cliente 
                            ORDER BY pp.id_prestamo desc) 
                      ORDER BY c.id_cliente ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        Cliente item = new Cliente();

                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdStatusCliente = int.Parse(ds.Tables[0].Rows[i]["id_status_cliente"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();

                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.NombreStatus = ds.Tables[0].Rows[i]["nombre_status_cliente"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();

                        item.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        item.NombreStatus = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_cliente"].ToString() + "</span>";

                        string botones = "";

                        //  visualizar
                        botones += "<button onclick='customers.view(" + item.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-eye mr-1'></span>Visualizar</button>";

                        //  condonar
                        botones += "<button onclick='customers.condonate(" + item.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-ban mr-1'></span>Condonar</button>";

                        //  demanda
                        if (item.IdStatusCliente == Cliente.STATUS_VENCIDO)
                        {
                            botones += "<button onclick='customers.claim(" + item.IdPrestamo + ")'  class='btn btn-outline-primary'> <span class='fa fa-legal mr-1'></span>Demanda</button>";
                        }

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
        public static DatosSalida UpdateStatusCustomer(string path, string customerId, string userId, string statusId)
        {



            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, userId);
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

                string sql = @" UPDATE cliente SET id_status_cliente = @id_status_cliente
                                        WHERE id_cliente = @id  ";

                Utils.Log("\n-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                Utils.Log("customerId "+ customerId + "\n");
                Utils.Log("statusId"+ statusId + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_status_cliente", statusId);
                cmd.Parameters.AddWithValue("@id", customerId);

                int r = cmd.ExecuteNonQuery();

                Utils.Log("r = " + r);

                salida.MensajeError = null;
                salida.CodigoError = 0;

                return salida;
            }
            catch (Exception ex)
            {

                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo actualizar el registro.";

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

                string sql = @" UPDATE garantia_prestamo SET eliminado = 1  
                                        WHERE id_garantia_prestamo = @id ";

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



        [WebMethod]
        public static List<Status> GetListaStatus(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Status> items = new List<Status>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_status_cliente, nombre FROM  status_cliente ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Status item = new Status();
                        item.IdStatus = int.Parse(ds.Tables[0].Rows[i]["id_status_cliente"].ToString());
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



    }



}