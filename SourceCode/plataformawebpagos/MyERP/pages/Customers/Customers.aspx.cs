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
    public partial class Customers : System.Web.UI.Page
    {
        const string pagina = "21";

        // Información mínima del empleado para contextualizar filtros por usuario.
        public class EmpleadoLigero
        {
            public int IdEmpleado { get; set; }
            public int IdPlaza { get; set; }
            public int IdPosicion { get; set; }
            public int IdSupervisor { get; set; }
            public int IdEjecutivo { get; set; }
        }

        private static UserVisibilityContext GetCurrentScope(string path, SqlConnection conn)
        {
            return UserVisibilityScope.GetCurrent(path, conn) ?? new UserVisibilityContext();
        }

        private static int GetScopedPlaza(UserVisibilityContext scope, int requestedPlaza)
        {
            //  Sólo se fuerza la plaza cuando el usuario realmente tiene una asignada.
            //  Un administrativo sin plaza (director de oficina) ve todas.
            if (scope != null && (scope.IsDirector || scope.IsSupervisor) && scope.IdPlaza > 0)
            {
                return scope.IdPlaza;
            }

            return requestedPlaza;
        }



        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];
            string idPlaza = "";



            txtUsuario.Value = usuario;
            txtIdTipoUsuario.Value = idTipoUsuario;
            txtIdUsuario.Value = idUsuario;
            var scope = UserVisibilityScope.GetByUser(path, idUsuario);
            txtIdEmpleado.Value = scope.IdEmpleado > 0 ? scope.IdEmpleado.ToString() : string.Empty;

            // Plaza fija para roles con visibilidad acotada por plaza.
            if ((scope.IsSupervisor || scope.IsDirector) && scope.IdPlaza > 0)
            {
                idPlaza = scope.IdPlaza.ToString();
            }
            // Asegurar el hidden de plaza siempre exista y asigne valor (puede ser vacío)
            if (txtIdPlaza == null)
            {
                txtIdPlaza = new HiddenField { ID = "txtIdPlaza" };
                form1.Controls.Add(txtIdPlaza);
            }
            txtIdPlaza.Value = idPlaza;

            //  si no esta logueado
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }

        [WebMethod]
        public static DatosSalida DeleteCliente(string path, int idCliente, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // Permisos
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso) return null;

            var salida = new DatosSalida();
            using (var conn = new SqlConnection(strConexion))
            {
                conn.Open();
                var tx = conn.BeginTransaction();
                try
                {
                    var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);
                    if (!UserVisibilityScope.CanAccessCliente(scope, conn, idCliente))
                    {
                        tx.Rollback();
                        return new DatosSalida
                        {
                            CodigoError = 1,
                            MensajeError = "No tienes permiso para eliminar un cliente fuera de tu plaza."
                        };
                    }

                    // Prestamos del cliente -> inactivos/rechazados
                    var sqlPrestamo = @"UPDATE prestamo
                                        SET activo = 0, id_status_prestamo = @status
                                        WHERE id_cliente = @id_cliente";
                    conn.Execute(sqlPrestamo, new { id_cliente = idCliente, status = Prestamo.STATUS_RECHAZADO }, tx);

                    // Cliente inactivo/eliminado
                    var sqlCliente = @"UPDATE cliente
                                       SET activo = 0, eliminado = 1, id_status_cliente = @status
                                       WHERE id_cliente = @id_cliente";
                    conn.Execute(sqlCliente, new { id_cliente = idCliente, status = Cliente.STATUS_INACTIVO }, tx);

                    tx.Commit();
                    salida.CodigoError = 0;
                    salida.MensajeError = "Cliente eliminado lógicamente";
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    Utils.Log("Error DeleteCliente ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                    salida.CodigoError = 1;
                    salida.MensajeError = "No se pudo eliminar el cliente";
                }
            }

            return salida;
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

                    // Enforce scope based on the logged-in user to avoid cross-plaza leakage.
                    var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);
                    var tipoActual = scope.IdTipoUsuario;
                    var idEmpleadoActual = scope.IdEmpleado;

                    if (scope.IsDirector)
                    {
                        //  Con plaza asignada se acota a ella; sin plaza (administrativo
                        //  de oficina) idPlaza queda en 0 = todas las plazas.
                        idPlaza = UserVisibilityScope.GetFixedPlaza(scope, idPlaza);
                    }

                    if (tipoActual == Employees.POSICION_PROMOTOR)
                    {
                        // Un promotor solo ve sus clientes
                        idPromotor = idEmpleadoActual;
                        typeFilter = "promotor";
                    }
                    else if (tipoActual == Employees.POSICION_SUPERVISOR)
                    {
                        // Un supervisor solo ve los clientes de sus promotores
                        idSupervisor = idEmpleadoActual;
                        typeFilter = "supervisor";
                    }
                    else if (tipoActual == Employees.POSICION_EJECUTIVO)
                    {
                        // Un ejecutivo solo ve los supervisores bajo él
                        if (idEjecutivo <= 0) idEjecutivo = idEmpleadoActual;
                        if (string.IsNullOrWhiteSpace(typeFilter) || typeFilter == "plaza")
                            typeFilter = "ejecutivo";
                    }

                    // 1) Trae empleados SOLO de plazas ACTIVAS (y opcional: empleados activos)
                    //    Si idPlaza > 0 se acota a esa plaza; si no, trae de todas las activas
                    var empleados = conn.Query<Empleado>(@"
                    SELECT e.id_empleado AS IdEmpleado,
                       e.id_plaza      AS IdPlaza,
                       e.id_posicion   AS IdPosicion,
                       e.id_supervisor AS IdSupervisor,
                       e.id_ejecutivo  AS IdEjecutivo,
                       u.id_usuario    AS IdUsuario
                FROM empleado e
                INNER JOIN plaza pl   ON pl.id_plaza = e.id_plaza
                inner join usuario u on u.id_empleado = e.id_empleado
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
                            // Solo ese promotor
                            if (idPromotor > 0)
                                empleadosFiltrados = empleados.Where(w => w.IdPosicion == 5 && w.IdEmpleado == idPromotor);
                            else
                                empleadosFiltrados = empleados.Where(w => w.IdPosicion == 5);
                            break;

                        case "supervisor":
                            // Promotores directamente asignados a ese supervisor
                            empleadosFiltrados = empleados.Where(w =>
                                w.IdPosicion == 5 && w.IdSupervisor == idSupervisor);
                            break;

                        case "ejecutivo":
                            // Supervisores asignados a ese ejecutivo
                            empleadosFiltrados = empleados.Where(w =>
                                w.IdPosicion == 4 && w.IdEjecutivo == idEjecutivo);
                            break;
                    }


                    // 3) Construye el filtro IN de forma segura:
                    var idsEmpleados = empleadosFiltrados.Select(e => e.IdEmpleado).Distinct().ToList();

                    // Si no hay empleados válidos en plazas activas, no traigas nada
                    string filtroEmpleadosSql = (idsEmpleados.Count == 0)
                        ? " AND 1 = 0 "
                        : " AND e.id_empleado IN (" + string.Join(",", idsEmpleados) + ") ";

                    // Gestor de cobranza: sólo los clientes de la cartera que le asignaron.
                    if (scope.IsGestor)
                    {
                        filtroEmpleadosSql = scope.IdEmpleado > 0
                            ? @" AND EXISTS (
                                    SELECT 1
                                    FROM cobranza_asignacion ca_scope
                                    WHERE ca_scope.id_prestamo = p.id_prestamo
                                      AND ca_scope.id_gestor = " + scope.IdEmpleado + @"
                                      AND ISNULL(ca_scope.activo, 0) = 1
                                      AND ISNULL(ca_scope.eliminado, 0) = 0
                                ) "
                            : " AND 1 = 0 ";
                    }

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
                    inner join usuario u on u.id_empleado = p.id_empleado
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
                  AND ISNULL(c.eliminado, 0) = 0
                  AND ISNULL(c.activo, 1) = 1
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

                            botones += $"<button type='button' onclick='customers.deleteCustomer({item.IdCliente}); return false;' class='btn btn-outline-danger ml-2'><span class='fa fa-trash mr-1'></span>Eliminar</button>";

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

                Utils.Log("customerId " + customerId + "\n");
                Utils.Log("statusId" + statusId + "\n");

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

        [WebMethod(EnableSession = true)]
        public static List<Plaza> GetListaPlazas(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Plaza> items = new List<Plaza>();

            try
            {
                conn.Open();
                var scope = GetCurrentScope(path, conn);
                var idPlaza = GetScopedPlaza(scope, 0);

                if (scope.IsSupervisor && idPlaza <= 0)
                {
                    return items;
                }

                DataSet ds = new DataSet();
                string query = @" SELECT id_plaza, nombre
                                  FROM plaza
                                  WHERE activo = 1
                                    AND eliminado <> 1";

                if (idPlaza > 0)
                {
                    query += " AND id_plaza = " + idPlaza;
                }

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

        [WebMethod(EnableSession = true)]
        public static List<Empleado> GetListaEjecutivo(string path, int idplaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                var scope = GetCurrentScope(path, conn);
                idplaza = GetScopedPlaza(scope, idplaza);

                if (scope.IsSupervisor && idplaza <= 0)
                {
                    return items;
                }

                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado, e.nombre, e.primer_apellido, e.segundo_apellido
                                  FROM empleado e
                                  WHERE e.id_plaza = " + idplaza + @"
                                    AND e.id_posicion = 3"
                                  + UserVisibilityScope.BuildEmployeeScopeSql(scope, "e.id_empleado");

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

        [WebMethod(EnableSession = true)]
        public static List<Empleado> GetListaSupervisor(string path, int idejecutivo, int idplaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                var scope = GetCurrentScope(path, conn);
                idplaza = GetScopedPlaza(scope, idplaza);

                if (scope.IsSupervisor && idplaza <= 0)
                {
                    return items;
                }

                if (idejecutivo > 0 && !UserVisibilityScope.CanAccessEmployee(scope, conn, idejecutivo, Employees.POSICION_EJECUTIVO))
                {
                    return items;
                }

                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado, e.nombre, e.primer_apellido, e.segundo_apellido
                                  FROM empleado e
                                  WHERE e.id_ejecutivo = " + idejecutivo + @"
                                    AND e.id_posicion = 4
                                    AND e.id_plaza = " + idplaza
                                  + UserVisibilityScope.BuildEmployeeScopeSql(scope, "e.id_empleado");

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

        [WebMethod(EnableSession = true)]
        public static List<Empleado> GetListaPromotor(string path, int idsupervisor, int idplaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                var scope = GetCurrentScope(path, conn);
                idplaza = GetScopedPlaza(scope, idplaza);

                if (scope.IsSupervisor && idplaza <= 0)
                {
                    return items;
                }

                if (idsupervisor > 0 && !UserVisibilityScope.CanAccessEmployee(scope, conn, idsupervisor, Employees.POSICION_SUPERVISOR))
                {
                    return items;
                }

                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado, e.nombre, e.primer_apellido, e.segundo_apellido
                                  FROM empleado e
                                  WHERE e.id_supervisor = " + idsupervisor + @"
                                    AND e.id_posicion = 5
                                    AND e.id_plaza = " + idplaza
                                  + UserVisibilityScope.BuildEmployeeScopeSql(scope, "e.id_empleado");

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
        public static EmpleadoLigero GetPlazaActual(string path, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // Permisos
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso) return null;

            using (var conn = new SqlConnection(strConexion))
            {
                try
                {
                    const string query = @"
                        SELECT TOP 1
                            e.id_empleado   AS IdEmpleado,
                            e.id_plaza      AS IdPlaza,
                            e.id_posicion   AS IdPosicion,
                            e.id_supervisor AS IdSupervisor,
                            e.id_ejecutivo  AS IdEjecutivo
                        FROM usuario u
                        INNER JOIN empleado e ON e.id_empleado = u.id_empleado
                        WHERE u.id_usuario = @idUsuario;";

                    return conn.Query<EmpleadoLigero>(query, new { idUsuario }).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                    return null;
                }
            }
        }



    }



}
