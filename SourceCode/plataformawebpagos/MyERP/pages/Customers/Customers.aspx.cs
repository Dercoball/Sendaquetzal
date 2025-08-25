using Dapper;
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
        public static List<Cliente> GetItems(
    string path, string idUsuario, string idTipoUsuario, string idStatus,
    int idPlaza, int idEjecutivo, int idSupervisor, int idPromotor, string typeFilter)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // Permisos
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso) return null;

            var items = new List<Cliente>();

            using (var conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();

                    // 1) Trae empleados SOLO de plazas ACTIVAS (y opcional: empleados activos)
                    //    Si idPlaza > 0 se acota a esa plaza; si no, trae de todas las activas
                    var empleados = conn.Query<Empleado>(@"
                SELECT e.id_empleado   AS IdEmpleado,
                       e.id_plaza      AS IdPlaza,
                       e.id_posicion   AS IdPosicion,
                       e.id_supervisor AS IdSupervisor,
                       e.id_ejecutivo  AS IdEjecutivo
                FROM empleado e
                INNER JOIN plaza pl   ON pl.id_plaza = e.id_plaza
                WHERE pl.activo = 1 and pl.eliminado <> 1
                  /* Descomenta si tienes columna de empleado activo:
                  AND e.activo = 1 and e.eliminado <> 1
                  */
                  AND (@idPlaza = 0 OR e.id_plaza = @idPlaza);
            ", new { idPlaza }).ToList();

                    // 2) Aplica tu typeFilter sobre esa lista depurada
                    IEnumerable<Empleado> empleadosFiltrados = empleados;
                    switch (typeFilter?.ToLowerInvariant())
                    {
                        case "promotor":
                            empleadosFiltrados = empleados.Where(w => w.IdEmpleado == idPromotor);
                            break;

                        case "supervisor":
                            // promotores del supervisor
                            empleadosFiltrados = empleados.Where(w => w.IdSupervisor == idSupervisor && w.IdPosicion == 5);
                            break;

                        case "ejecutivo":
                            // supervisores del ejecutivo
                            var supervisoresIDs = empleados
                                .Where(w => w.IdEjecutivo == idEjecutivo && w.IdPosicion == 4)
                                .Select(s => s.IdEmpleado)
                                .ToHashSet();

                            // promotores de esos supervisores
                            empleadosFiltrados = empleados.Where(w => w.IdPosicion == 5 && supervisoresIDs.Contains(w.IdSupervisor));
                            break;
                    }

                    // 3) Construye el filtro IN de forma segura:
                    var idsEmpleados = empleadosFiltrados.Select(e => e.IdEmpleado).Distinct().ToList();

                    // Si no hay empleados válidos en plazas activas, no traigas nada
                    string filtroEmpleadosSql = (idsEmpleados.Count == 0)
                        ? " AND 1 = 0 "
                        : " AND e.id_empleado IN (" + string.Join(",", idsEmpleados) + ") ";

                    // 4) Query: último préstamo por cliente + plaza activa SIEMPRE
                    string query = @"
                WITH ult AS (
                    SELECT
                        p.id_prestamo,
                        p.id_cliente,
                        p.id_empleado,
                        p.monto,
                        ROW_NUMBER() OVER (PARTITION BY p.id_cliente ORDER BY p.id_prestamo DESC) AS rn
                    FROM prestamo p
                    inner join usuario u on u.id_usuario = p.id_empleado
                    inner JOIN empleado e ON e.id_empleado = u.id_empleado
                    inner JOIN plaza   pl ON pl.id_plaza   = e.id_plaza AND pl.activo = 1
                    WHERE 1 = 1
                    " + filtroEmpleadosSql + @"
                )
                SELECT
                    c.id_cliente,
                    CONCAT(c.nombre, ' ', c.primer_apellido, ' ', c.segundo_apellido) AS nombre_completo,
                    c.telefono, c.curp, c.ocupacion,
                    ISNULL(c.id_status_cliente, 2) AS id_status_cliente,
                    ISNULL(c.mensaje, 1) AS mensaje,
                    st.nombre AS nombre_status_cliente, st.color,
                    u.id_prestamo, u.monto,
                    d.calleyno, d.colonia, d.municipio, d.estado
                FROM ult u
                INNER JOIN cliente c       ON c.id_cliente = u.id_cliente
                INNER JOIN status_cliente st ON st.id_status_cliente = c.id_status_cliente
                LEFT JOIN direccion d      ON (d.id_cliente = c.id_cliente AND d.aval = 0)
                WHERE u.rn = 1
                ORDER BY c.id_cliente;";

                    var ds = new DataSet();
                    using (var adp = new SqlDataAdapter(query, conn))
                    {
                        Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                        adp.Fill(ds);
                    }

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            var item = new Cliente
                            {
                                IdCliente = Convert.ToInt32(r["id_cliente"]),
                                IdStatusCliente = Convert.ToInt32(r["id_status_cliente"]),
                                IdPrestamo = Convert.ToInt32(r["id_prestamo"]),
                                Curp = r["curp"]?.ToString(),
                                NombreCompleto = r["nombre_completo"]?.ToString(),
                                NombreStatus = r["nombre_status_cliente"]?.ToString(),
                                Telefono = r["telefono"]?.ToString(),
                                Monto = float.TryParse(r["monto"]?.ToString(), out var m) ? m : 0f,
                                direccion = new Direccion
                                {
                                    Calle = r["calleyno"]?.ToString(),
                                    Colonia = r["colonia"]?.ToString(),
                                    Municipio = r["municipio"]?.ToString(),
                                    Estado = r["estado"]?.ToString()
                                },
                                Color = r["color"]?.ToString(),
                                Mensaje = Convert.ToInt32(r["mensaje"])
                            };

                            // pinta status con color
                            item.NombreStatus = $"<span class='{item.Color}'>{item.NombreStatus}</span>";

                            // Botones
                            string botones = "";
                            botones += $"<button onclick='customers.view({item.IdPrestamo})' class='btn btn-outline-primary'><span class='fa fa-eye mr-1'></span>Visualizar</button>";

                            if (item.IdStatusCliente == Cliente.STATUS_ACTIVO ||
                                item.IdStatusCliente == Cliente.STATUS_INACTIVO ||
                                item.IdStatusCliente == Cliente.STATUS_VENCIDO)
                            {
                                botones += $"<button onclick='customers.condonate({item.IdCliente})' class='btn btn-outline-primary'><span class='fa fa-ban mr-1'></span>Condonar</button>";
                            }
                            if (item.IdStatusCliente == Cliente.STATUS_VENCIDO)
                            {
                                botones += $"<button onclick='customers.claim({item.IdCliente})' class='btn btn-outline-primary'><span class='fa fa-legal mr-1'></span>Demanda</button>";
                            }
                            if (item.IdStatusCliente == Cliente.STATUS_CONDONADO)
                            {
                                botones += $"<button onclick='customers.reactivate({item.IdCliente})' class='btn btn-outline-primary'><span class='fa fa-check-circle mr-1'></span>Reactivar</button>";
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
                string query = @" SELECT id_plaza, nombre FROM  plaza WHERE activo = 1 and eliminado <> 1";

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
        public static List<Empleado> GetListaSupervisor(string path, int idejecutivo, int idplaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_empleado, nombre, primer_apellido, segundo_apellido FROM  empleado WHERE id_ejecutivo = " + idejecutivo + " AND id_posicion = 4 AND id_plaza = " + idplaza;

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
        public static List<Empleado> GetListaPromotor(string path, int idsupervisor, int idplaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_empleado, nombre, primer_apellido, segundo_apellido FROM  empleado WHERE id_ejecutivo = " + idsupervisor + " AND id_posicion = 5 AND id_plaza = " + idplaza;

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