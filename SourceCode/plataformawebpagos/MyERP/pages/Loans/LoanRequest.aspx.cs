using Dapper;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Services;
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
        public static object Search(RequestGridPrestamos Filtro, string path) {

            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var llst_Prestamos= new List<ResponseGridPrestamos>();
            var conn = new SqlConnection(strConexion);

            try
            {
                var sql = @"SELECT *  FROM (SELECT id_prestamo , 
	                            c.nombre nombreCliente,
	                            monto,
	                            (select min(fecha_solicitud) from prestamo  where id_cliente = c.id_cliente) fecha_primera_solicitud,
	                            (select max(fecha_solicitud) from prestamo  where id_cliente = c.id_cliente) fecha_ultima_solicitud,
	                            (select count(*)  from prestamo  where id_cliente = c.id_cliente) NoPrestamos,
	                            (select count(*)  from prestamo  where id_cliente = c.id_cliente and id_status_prestamo = 3) Rechazados,
	                            (select count(*)  from prestamo  where id_aval = c.id_cliente) Aval,
	                            sp.nombre Status ,
	                            sp.color ColorStatus,
                                sp.id_status_prestamo,
                                p.activo
	                    FROM prestamo p
	                    INNER JOIN cliente  c on c.id_cliente  = p.id_cliente
	                    INNER JOIN cliente  av on av.id_cliente  = p.id_aval
	                    INNER JOIN status_prestamo sp on sp.id_status_prestamo = p.id_status_prestamo ) gp
                        WHERE gp.activo = 1
                        ";

                if (!string.IsNullOrWhiteSpace(Filtro.Nombre))
                {
                    sql += $@" AND gp.nombreCliente like '%{Filtro.Nombre}%'";
                }

                if (Filtro.NoPrestamoMinimo.HasValue)
                {
                    sql += $@" AND gp.NoPrestamos >= {Filtro.NoPrestamoMinimo.Value}";
                }

                if (Filtro.NoPrestamoMaximo.HasValue)
                {
                    sql += $@" AND gp.NoPrestamos <= {Filtro.NoPrestamoMaximo.Value}";
                }

                if (Filtro.AvalMinimo.HasValue)
                {
                    sql += $@" AND gp.Aval >= {Filtro.AvalMinimo.Value}";
                }

                if (Filtro.AvalMaximo.HasValue)
                {
                    sql += $@" AND gp.Aval <= {Filtro.AvalMaximo.Value}";
                }

                if (Filtro.RechazoMinimo.HasValue)
                {
                    sql += $@" AND gp.Rechazados >= {Filtro.RechazoMinimo.Value}";
                }

                if (Filtro.RechazosMaximo.HasValue)
                {
                    sql += $@" AND gp.Rechazados <= {Filtro.RechazosMaximo.Value}";
                }

                if (Filtro.MontoMinimo.HasValue)
                {
                    sql += $@" AND gp.monto >= {Filtro.MontoMinimo.Value}";
                }

                if (Filtro.MontoMaximo.HasValue)
                {
                    sql += $@" AND gp.montp <= {Filtro.MontoMaximo.Value}";
                }

                if (Filtro.Status.HasValue)
                {
                    sql += $@" AND gp.id_status_prestamo = {Filtro.Status}";
                }

                if (Filtro.FechaPrimerSolicitudMinimo.HasValue)
                {
                    sql += $@" AND Convert(Date,fecha_primera_solicitud) >= '{Filtro.FechaPrimerSolicitudMinimo.Value.ToString("yyyy/MM/dd")}'";
                }
                if (Filtro.FechaPrimerSolicitudMaximo.HasValue)
                {
                    sql += $@" AND  Convert(Date,fecha_primera_solicitud) <= '{Filtro.FechaPrimerSolicitudMaximo.Value.ToString("yyyy/MM/dd")}'";
                }

                if (Filtro.FechaUltimaSolicitudMinimo.HasValue)
                {
                    sql += $@" AND Convert(Date,fecha_ultima_solicitud) >= '{Filtro.FechaUltimaSolicitudMinimo.Value.ToString("yyyy/MM/dd")}'";
                }
                if (Filtro.FechaUltimaSolicitudMaximo.HasValue)
                {
                    sql += $@" AND  Convert(Date,fecha_ultima_solicitud) <= '{Filtro.FechaUltimaSolicitudMaximo.Value.ToString("yyyy/MM/dd")}'";
                }

                llst_Prestamos = conn.Query<ResponseGridPrestamos>(sql)
                .ToList() ?? new List<ResponseGridPrestamos>();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }

            return llst_Prestamos;
        }
        
        [WebMethod]
        public static object GetStatus(string path)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);
            var items = new List<StatusPrestamo>();

            try
            {
                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name);

                items = conn.Query<StatusPrestamo>($@"SELECT id_status_prestamo {nameof(StatusPrestamo.IdStatusPrestamo)},
                    nombre {nameof(StatusPrestamo.Nombre)}
                    FROM status_prestamo")
                    .ToList() ?? new List<StatusPrestamo>();
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }
            finally
            {
            }

            return items;
        }

        [WebMethod]
        public static object GridPrestamos(string path, string idUsuario)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            var conn = new SqlConnection(strConexion);
            var items = new List<ResponseGridPrestamos>();

            try
            {
                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n spGridPrestamos\n");

                items = conn.Query<ResponseGridPrestamos>("spGridPrestamos",  
                    commandType: CommandType.StoredProcedure)
                    .ToList() ?? new List<ResponseGridPrestamos>();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
            }

            finally
            {
            }

            return items;
        }

        [WebMethod]
        public static Prestamo GetDataPrestamo(string path, string idPrestamo)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Prestamo item = new Prestamo();
            SqlConnection conn = new SqlConnection(strConexion);


            try
            {
                conn.Open();

                item = GetPrestamoById(path, idPrestamo, conn);

                if (item != null)
                {
                    item.Cliente = GetItemClient(path, item.IdCliente, conn);
                    item.Cliente.direccion = GetAddress(path, item.IdCliente, 0, conn);
                    item.Cliente.direccionAval = GetAddress(path, item.IdCliente, 1, conn);
                    item.listaRelPrestamoAprobacion = GetRelPrestamoAprobacion(path, item.IdPrestamo, conn);
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


        public static Prestamo GetPrestamoById(string path, string idPrestamo, SqlConnection conn)
        {

            Prestamo prestamoData = null;

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT p.monto, p.id_prestamo, IsNull(p.id_empleado, 0) id_empleado,
                                         FORMAT(p.fecha_solicitud, 'dd/MM/yyyy') fecha_solicitud, fecha_solicitud fecha_solicitud_date,
                                         p.id_status_prestamo, p.id_cliente,
                                         st.nombre nombre_status_prestamo, st.color
                                    FROM prestamo p 
                                    JOIN status_prestamo st ON (st.id_status_prestamo = p.id_status_prestamo) 
                                    WHERE P.id_prestamo = @id_prestamo
                                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_prestamo", idPrestamo);

                Utils.Log("\nMétodo-> " +
                        System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        prestamoData = new Prestamo();
                        prestamoData.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        prestamoData.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        prestamoData.IdCliente = ds.Tables[0].Rows[i]["id_cliente"].ToString();
                        prestamoData.IdStatusPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_status_prestamo"].ToString());
                        prestamoData.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        prestamoData.NombreStatus = "<span class='" + prestamoData.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";
                        prestamoData.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();
                        prestamoData.FechaSolicitudDate = DateTime.Parse(ds.Tables[0].Rows[i]["fecha_solicitud_date"].ToString());
                        prestamoData.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        prestamoData.MontoFormateadoMx = prestamoData.Monto.ToString("C2");


                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return prestamoData;
            }

            return prestamoData;

        }
    }
}