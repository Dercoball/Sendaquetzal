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
    public partial class LoanRequest : System.Web.UI.Page
    {
        const string pagina = "12";

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
        public static object GridPrestamos(string path, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            SqlConnection conn = new SqlConnection(strConexion);
            List<Cliente> items = new List<Cliente>();

            try
            {
                var ds = new DataSet();
                var cmdGridPrestamos = new SqlCommand("spGridPrestamos", conn);
                cmdGridPrestamos.CommandType = CommandType.StoredProcedure;
                cmdGridPrestamos.Connection = conn;

                var adp = new SqlDataAdapter(cmdGridPrestamos);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n spGridPrestamos\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var item = new RequestGridPrestamos
                        {
                            Aval = ds.Tables[0].Rows[i]["Aval"].ToString(),
                            fecha_primera_solicitud = (DateTime?)ds.Tables[0].Rows[i]["fecha_primera_solicitud"],
                            fecha_ultima_solicitud = (DateTime?)ds.Tables[0].Rows[i]["fecha_ultima_solicitud"],
                            id_prestamo = (int)ds.Tables[0].Rows[i]["id_prestamo"],
                            monto = (float)ds.Tables[0].Rows[i]["monto"],
                            nombreCliente = ds.Tables[0].Rows[i]["nombreCliente"].ToString(),
                            NoRechazados = (int)ds.Tables[0].Rows[i]["NoRechazados"],
                            Status = ds.Tables[0].Rows[i]["Status"].ToString(),
                        };

                        items.Add(new );
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
            }
        }


        [WebMethod]
        public static List<Cliente> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            SqlConnection conn = new SqlConnection(strConexion);
            List<Cliente> items = new List<Cliente>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente , c.nombre, c.primer_apellido, c.segundo_apellido, 
                     concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                     c.telefono , c.curp, c.ocupacion, c.activo, tc.id_tipo_cliente, tc.tipo_cliente, p.id_prestamo, p.monto,
                     FORMAT(p.fecha_solicitud, 'dd/MM/yyyy') fecha_solicitud
                     FROM cliente c 
                     JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     WHERE isnull(c.eliminado, 0) != 1 
                     ORDER BY id_cliente ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Cliente item = new Cliente();
                        //..
                        //
                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Ocupacion = ds.Tables[0].Rows[i]["ocupacion"].ToString();
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.TipoCliente = (ds.Tables[0].Rows[i]["tipo_cliente"].ToString());

                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();


                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());


                        string botones = "<button  onclick='client.edit(" + item.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='client.delete(" + item.IdCliente + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";


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
    }
}