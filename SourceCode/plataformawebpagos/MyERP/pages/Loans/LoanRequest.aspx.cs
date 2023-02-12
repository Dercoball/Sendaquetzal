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

        [WebMethod]
        public static Cliente GetCustomerByCurp(string path, string curp)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Cliente item = new Cliente();
            SqlConnection conn = new SqlConnection(strConexion);


            try
            {
                conn.Open();


                item = GetCustomer(path, curp, conn);
                if (item != null)
                {
                    item.direccion = GetAddress(path, item.IdCliente.ToString(), 0, conn);
                    item.direccionAval = GetAddress(path, item.IdCliente.ToString(), 1, conn);
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

        [WebMethod]
        public static Cliente GetItemClient(string path, string idCliente, SqlConnection conn)
        {

            Cliente item = new Cliente();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente , c.nombre, c.primer_apellido, c.segundo_apellido, c.id_tipo_cliente,
                                concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                c.telefono , c.curp, c.ocupacion, c.activo, 
                                c.curp_aval, c.nombre_aval, c.primer_apellido_aval, c.segundo_apellido_aval, c.ocupacion_aval, c.telefono_aval,
                                tc.tipo_cliente, p.id_prestamo, p.monto, nota_fotografia, nota_fotografia_aval,
                                FORMAT(fecha_solicitud, 'yyyy-MM-dd') fecha_solicitud, IsNull(id_status_prestamo, 0) id_status_prestamo
                                FROM cliente c 
                                JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) 
                                JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                                WHERE c.id_cliente = @id
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_cliente =  " + idCliente);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idCliente);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Cliente();

                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Ocupacion = ds.Tables[0].Rows[i]["ocupacion"].ToString();

                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.CurpAval = ds.Tables[0].Rows[i]["curp_aval"].ToString();
                        item.NombreAval = ds.Tables[0].Rows[i]["nombre_aval"].ToString();
                        item.PrimerApellidoAval = ds.Tables[0].Rows[i]["primer_apellido_aval"].ToString();
                        item.SegundoApellidoAval = ds.Tables[0].Rows[i]["segundo_apellido_aval"].ToString();
                        item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();
                        item.OcupacionAval = ds.Tables[0].Rows[i]["ocupacion_aval"].ToString();
                        item.NotaFotografiaCliente = ds.Tables[0].Rows[i]["nota_fotografia"].ToString();
                        item.NotaFotografiaAval = ds.Tables[0].Rows[i]["nota_fotografia_aval"].ToString();

                    }
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

        [WebMethod]
        public static Cliente GetItemCustomer(string path, string id, SqlConnection conn)
        {

            Cliente item = new Cliente();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente , c.nombre, c.primer_apellido, c.segundo_apellido, c.id_tipo_cliente,
                                concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                c.telefono , c.curp, c.ocupacion, c.activo, 
                                c.curp_aval, c.nombre_aval, c.primer_apellido_aval, c.segundo_apellido_aval, c.ocupacion_aval, c.telefono_aval,
                                tc.tipo_cliente, p.id_prestamo, p.monto,
                                FORMAT(fecha_solicitud, 'yyyy-MM-dd') fecha_solicitud
                                FROM cliente c 
                                JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) 
                                JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                                WHERE c.id_cliente = @id
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_empleado =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Cliente();

                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Ocupacion = ds.Tables[0].Rows[i]["ocupacion"].ToString();

                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.CurpAval = ds.Tables[0].Rows[i]["curp_aval"].ToString();
                        item.NombreAval = ds.Tables[0].Rows[i]["nombre_aval"].ToString();
                        item.PrimerApellidoAval = ds.Tables[0].Rows[i]["primer_apellido_aval"].ToString();
                        item.SegundoApellidoAval = ds.Tables[0].Rows[i]["segundo_apellido_aval"].ToString();
                        item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();
                        item.OcupacionAval = ds.Tables[0].Rows[i]["ocupacion_aval"].ToString();

                    }
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

        [WebMethod]
        public static Direccion GetAddress(string path, string idCliente, int aval, SqlConnection conn)
        {

            Direccion item = new Direccion();

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT id_direccion, id_empleado, id_cliente, id_aval, calleyno, colonia, municipio, estado, 
                                    codigo_postal, id_municipio, id_estado, activo, ISNULL(aval, 0) aval, direccion_trabajo,
                                    ubicacion
                                    FROM direccion
                                    WHERE id_cliente =  @id_cliente AND aval = @aval
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_cliente =  " + idCliente);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_cliente", idCliente);
                adp.SelectCommand.Parameters.AddWithValue("@aval", aval);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Direccion();


                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());

                        item.Aval = int.Parse(ds.Tables[0].Rows[i]["aval"].ToString());
                        item.Calle = ds.Tables[0].Rows[i]["calleyno"].ToString();
                        item.Colonia = ds.Tables[0].Rows[i]["colonia"].ToString();
                        item.Municipio = ds.Tables[0].Rows[i]["municipio"].ToString();
                        item.Estado = ds.Tables[0].Rows[i]["estado"].ToString();
                        item.CodigoPostal = ds.Tables[0].Rows[i]["codigo_postal"].ToString();
                        item.DireccionTrabajo = ds.Tables[0].Rows[i]["direccion_trabajo"].ToString();
                        item.Ubicacion = ds.Tables[0].Rows[i]["ubicacion"].ToString();




                    }
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

        [WebMethod]
        public static Cliente GetCustomer(string path, string curp, SqlConnection conn)
        {
            //string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Cliente item = null;
            //SqlConnection conn = new SqlConnection(strConexion);


            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente, c.nombre, c.primer_apellido, c.segundo_apellido, c.id_tipo_cliente,
                                concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                c.telefono , c.curp, c.ocupacion, IsNull(c.activo, 1) activo, 
                                c.curp_aval, c.nombre_aval, c.primer_apellido_aval, c.segundo_apellido_aval, c.ocupacion_aval, c.telefono_aval,
                                tc.id_tipo_cliente, tc.tipo_cliente nombre_tipo_cliente 
                                FROM cliente c 
                                JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente)                                 
                                WHERE c.curp = @curp AND IsNull(c.eliminado, 0) <> 1 AND IsNull(c.activo, 1) = 1
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("curp =  " + curp);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp", curp);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Cliente();

                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Ocupacion = ds.Tables[0].Rows[i]["ocupacion"].ToString();

                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();


                        item.CurpAval = ds.Tables[0].Rows[i]["curp_aval"].ToString();
                        item.NombreAval = ds.Tables[0].Rows[i]["nombre_aval"].ToString();
                        item.PrimerApellidoAval = ds.Tables[0].Rows[i]["primer_apellido_aval"].ToString();
                        item.SegundoApellidoAval = ds.Tables[0].Rows[i]["segundo_apellido_aval"].ToString();
                        item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();
                        item.OcupacionAval = ds.Tables[0].Rows[i]["ocupacion_aval"].ToString();

                    }
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

        [WebMethod]
        public static List<RelPrestamoAprobacion> GetRelPrestamoAprobacion(string path, int idPrestamo, SqlConnection conn)
        {

            List<RelPrestamoAprobacion> items = new List<RelPrestamoAprobacion>();

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT r.id_historial_aprobacion, 
                                    IsNull(r.id_prestamo, 0) id_prestamo, 
                                    IsNull(r.id_usuario, 0) id_usuario, 
                                    IsNull(r.id_empleado, 0) id_empleado, 
                                    IsNull(r.id_supervisor, 0) id_supervisor, 
                                    IsNull(r.id_ejecutivo, 0) id_ejecutivo,
                                    IsNull(r.id_posicion, 0) id_posicion,
                                    r.notas_cliente, r.notas_aval, r.fecha, 
                                    r.notas_generales,
                                    r.status_aprobacion, p.nombre nombre_posicion
                                    FROM relacion_prestamo_aprobacion r
                                    JOIN posicion p ON (p.id_posicion = r.id_posicion)
                                    WHERE r.id_prestamo = @id_prestamo ORDER BY r.id_posicion DESC
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_prestamo =  " + idPrestamo);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_prestamo", idPrestamo);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        RelPrestamoAprobacion item = new RelPrestamoAprobacion();


                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.IdRelPrestamoAprobacion = int.Parse(ds.Tables[0].Rows[i]["id_historial_aprobacion"].ToString());
                        item.IdUsuario = int.Parse(ds.Tables[0].Rows[i]["id_usuario"].ToString());
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.IdSupervisor = int.Parse(ds.Tables[0].Rows[i]["id_supervisor"].ToString());
                        item.IdEjecutivo = int.Parse(ds.Tables[0].Rows[i]["id_ejecutivo"].ToString());
                        item.IdPosicion = int.Parse(ds.Tables[0].Rows[i]["id_posicion"].ToString());
                        item.NotaCliente = ds.Tables[0].Rows[i]["notas_cliente"].ToString();
                        item.NotaAval = ds.Tables[0].Rows[i]["notas_aval"].ToString();
                        item.NotasGenerales = ds.Tables[0].Rows[i]["notas_generales"].ToString();
                        item.StatusAprobacion = ds.Tables[0].Rows[i]["status_aprobacion"].ToString();
                        item.NombrePosicion = ds.Tables[0].Rows[i]["nombre_posicion"].ToString();

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