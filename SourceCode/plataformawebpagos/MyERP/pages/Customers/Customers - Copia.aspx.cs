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
        public static List<Cliente> GetItems(string path, string idUsuario, string idTipoUsuario, string idStatus, int idPlaza, int idEjecutivo, int idSupervisor, int idPromotor, string typeFilter)
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
                var sqlPlaza = "";
                if (idPlaza > 0)
                {
                    var sqlEmpleado = "";
					switch (typeFilter)
					{
                        case "promotor":
                            sqlEmpleado = " AND id_empleado = " + idPromotor;
                            break;
                        case "supervisor":
                            sqlEmpleado = " AND id_supervisor = " + idSupervisor;
                            break;
                        case "ejecutivo":
                            sqlEmpleado = " AND id_ejecutivo = " + idEjecutivo;
                            break;
					}
                    sqlPlaza = " AND p.id_empleado IN (SELECT id_empleado FROM empleado WHERE id_plaza = " + idPlaza + sqlEmpleado + ") ";
                }


                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente,
                     concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                     c.telefono , c.curp, c.ocupacion,
                     IsNull(c.id_status_cliente, 2) id_status_cliente,
                     st.nombre nombre_status_cliente, st.color, p.id_prestamo, p.monto, d.calleyno, d.colonia, d.municipio, d.estado
                     FROM cliente c 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     JOIN status_cliente st ON (st.id_status_cliente = c.id_status_cliente)   
                     LEFT JOIN direccion d ON (d.id_cliente = c.id_cliente)
                    WHERE 1=1 
                    "
                    + sqlPlaza 
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
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.direccion = new Direccion
                        {
                            Calle = ds.Tables[0].Rows[i]["calleyno"].ToString(),
                            Colonia = ds.Tables[0].Rows[i]["colonia"].ToString(),
                            Municipio = ds.Tables[0].Rows[i]["municipio"].ToString(),
                            Estado = ds.Tables[0].Rows[i]["estado"].ToString()
                        };

                        item.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        item.NombreStatus = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_cliente"].ToString() + "</span>";

                        string botones = "";

                        //  visualizar
                        botones += "<button onclick='customers.view(" + item.IdPrestamo  + ")'  class='btn btn-outline-primary'> <span class='fa fa-eye mr-1'></span>Visualizar</button>";

                        //  condonar
                        if (item.IdStatusCliente == Cliente.STATUS_ACTIVO || item.IdStatusCliente == Cliente.STATUS_INACTIVO || item.IdStatusCliente == Cliente.STATUS_VENCIDO)
                        {
                            botones += "<button onclick='customers.condonate(" + item.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-ban mr-1'></span>Condonar</button>";
                        }

                        //  demanda
                        if (item.IdStatusCliente == Cliente.STATUS_VENCIDO)
                        {
                            botones += "<button onclick='customers.claim(" + item.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-legal mr-1'></span>Demanda</button>";
                        }

                        if (item.IdStatusCliente == Cliente.STATUS_CONDONADO)
                        {
                            botones += "<button onclick='customers.reactivate(" + item.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-check-circle mr-1'></span>Reactivar</button>";
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


                //  TODO: si se condona al cliente, pasar a status condonado los pagos


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

        [WebMethod]
        public static List<Plaza> GetListaPlazas(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Plaza> items = new List<Plaza>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_plaza, nombre FROM  plaza WHERE activo = 1";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Plaza item = new Plaza();
                        item.IdPlaza = int.Parse(ds.Tables[0].Rows[i]["id_plaza"].ToString());
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
        public static List<Empleado> GetListaEjecutivo(string path, int idplaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_empleado, nombre, primer_apellido, segundo_apellido FROM  empleado WHERE id_plaza = " +  idplaza +" AND id_posicion = 3";

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
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
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
        public static List<Empleado> GetListaSupervisor(string path, int idejecutivo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_empleado, nombre, primer_apellido, segundo_apellido FROM  empleado WHERE id_ejecutivo = " + idejecutivo + " AND id_posicion = 4";

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
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
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
        public static List<Empleado> GetListaPromotor(string path, int idsupervisor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_empleado, nombre, primer_apellido, segundo_apellido FROM  empleado WHERE id_ejecutivo = " + idsupervisor + " AND id_posicion = 5";

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
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
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