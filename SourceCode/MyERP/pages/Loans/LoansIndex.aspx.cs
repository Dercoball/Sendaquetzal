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
    public partial class LoansIndex : System.Web.UI.Page
    {
        const string pagina = "13";



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
        public static List<Prestamo> GetListaItems(string path, string idUsuario, string idTipoUsuario, string idStatus,
                string fechaInicial, string fechaFinal)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }
            

            //  Lista de datos a devolver
            List<Prestamo> items = new List<Prestamo>();


            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                //  Traer datos del usuario para saber su id_empleado
                Usuario user = Usuarios.GetUsuario(path, idUsuario);


                //  Filtro status del préstamo
                var sqlStatus = "";
                if (idStatus != "-1")
                {
                    sqlStatus = " AND p.id_status_prestamo = '" + idStatus + "'";
                }


                var sqlUsuario = "";

                //  Si es superusuario que vea todos los datos de todos
                if (idTipoUsuario != Employees.SUPERUSUARIO.ToString())
                {

                    //  Filtro para que el promotor solo vea sus prestamos 
                    if (idTipoUsuario == Employees.POSICION_PROMOTOR.ToString())
                    {
                        sqlUsuario = " AND p.id_usuario = " + idUsuario;

                    }

                    //  Filtro para que el supervisor vea los prestamos hechos por sus promotores
                    else if (idTipoUsuario == Employees.POSICION_SUPERVISOR.ToString())
                    {

                            //  La subquery arroja todos los id_usuario, que son empleados que dependen del supervisor logueado
                        sqlUsuario = @" AND p.id_usuario IN   
                                        ( select u.id_usuario
		                                        from empleado e
                                                join empleado superv ON (e.id_supervisor = superv.id_empleado)
                                                JOIN usuario u ON (u.id_empleado = e.id_empleado)
		                                        WHERE superv.id_empleado = " + user.IdEmpleado + @" )

                            ";

                    }

                    //  Filtro para que el ejecutivo vea los prestamos asignados a sus supervisores
                    else if (idTipoUsuario == Employees.POSICION_EJECUTIVO.ToString())
                    {
                        sqlUsuario = @" AND p.id_usuario IN   
                                        ( select u.id_usuario
		                                    from empleado e
                                            join empleado ejec ON (e.id_ejecutivo = ejec.id_empleado)
                                            JOIN usuario u ON (u.id_empleado = e.id_empleado)
		                                    WHERE ejec.id_empleado = " + user.IdEmpleado + @" )

                            ";

                    }


                }



                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente,
                     concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                     c.telefono , c.curp, c.ocupacion, c.activo, p.id_prestamo, p.monto,
                     FORMAT(p.fecha_solicitud, 'dd/MM/yyyy') fecha_solicitud,
                     st.nombre nombre_status_prestamo, st.color
                     FROM cliente c 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     JOIN status_prestamo st ON (st.id_status_prestamo = p.id_status_prestamo) 
                     WHERE  "
                    + @" (p.fecha_solicitud >= '" + fechaInicial + @"' AND p.fecha_solicitud <= '" + fechaFinal + @"') "
                    + sqlStatus
                    + sqlUsuario
                    + " ORDER BY p.id_prestamo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        Prestamo item = new Prestamo();
                        item.Cliente = new Cliente();
                        item.Cliente.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.Cliente.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());

                        item.Cliente.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();

                        item.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        item.NombreStatus = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";


                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2");//moneda Mx -> $ 2.00
                        
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());


                        string botones = "<button disabled onclick='loansindex.view(" + item.Cliente.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-eye mr-1'></span>Ver</button>";



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
        public static List<StatusPrestamo> GetListaStatusPrestamo(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_status_prestamo, nombre FROM  status_prestamo 
                                    ORDER by id_status_prestamo";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        StatusPrestamo item = new StatusPrestamo();
                        item.IdStatusPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_status_prestamo"].ToString());
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