using Dapper;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
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
                // Determinar alcance según rol
                var idUsuario = Convert.ToString(HttpContext.Current?.Session["id_usuario"] ?? "0");
                var idTipoUsuario = Convert.ToString(HttpContext.Current?.Session["id_tipo_usuario"] ?? "0");

                var usuarioActual = Usuarios.GetUsuario(path, idUsuario);
                var tipoActual = int.TryParse(idTipoUsuario, out var t) ? t : 0;
                var idEmpleadoActual = usuarioActual?.IdEmpleado ?? 0;

                // Empleados activos por plaza
                var empleados = conn.Query<Empleado>(@"
                    SELECT e.id_empleado   AS IdEmpleado,
                           e.id_posicion   AS IdPosicion,
                           e.id_supervisor AS IdSupervisor,
                           e.id_ejecutivo  AS IdEjecutivo,
                           e.id_plaza      AS IdPlaza
                    FROM empleado e
                    INNER JOIN plaza pl ON pl.id_plaza = e.id_plaza
                    WHERE pl.activo = 1 AND ISNULL(pl.eliminado,0) = 0
                          AND ISNULL(e.eliminado,0) = 0
                          AND ISNULL(e.activo,1) = 1
                ").ToList();

                IEnumerable<Empleado> promotoresAutorizados = empleados.Where(e => e.IdPosicion == Employees.POSICION_PROMOTOR);

                if (tipoActual == Employees.POSICION_PROMOTOR)
                {
                    promotoresAutorizados = promotoresAutorizados.Where(p => p.IdEmpleado == idEmpleadoActual);
                }
                else if (tipoActual == Employees.POSICION_SUPERVISOR)
                {
                    promotoresAutorizados = promotoresAutorizados.Where(p => p.IdSupervisor == idEmpleadoActual);
                }
                else if (tipoActual == Employees.POSICION_EJECUTIVO)
                {
                    var supervisores = empleados.Where(s => s.IdPosicion == Employees.POSICION_SUPERVISOR &&
                                                            s.IdEjecutivo == idEmpleadoActual)
                                                .Select(s => s.IdEmpleado)
                                                .ToHashSet();

                    promotoresAutorizados = promotoresAutorizados.Where(p =>
                        p.IdEjecutivo == idEmpleadoActual || supervisores.Contains(p.IdSupervisor));
                }
                // Otros roles (director/superadmin) ven todo por defecto

                var promotoresIds = promotoresAutorizados.Select(p => p.IdEmpleado).Distinct().ToList();
                // Si no hay promotores asignados, avisar claramente y no aplicar filtro bloqueante
                if (promotoresIds.Count == 0)
                {
                    return new List<ResponseGridPrestamos>
                    {
                        new ResponseGridPrestamos { Mensaje = "No tiene promotores asignados" }
                    };
                }

                var filtroPromotoresSql = " AND p.id_empleado IN (" + string.Join(",", promotoresIds) + ") ";

                var sql = @"SELECT *  FROM (SELECT p.id_prestamo , 
                            c.id_cliente AS IdCliente,
                            c.nombre nombreCliente,
                            p.monto,
                            (select min(fecha_solicitud) from prestamo  where id_cliente = c.id_cliente AND ISNULL(activo,1)=1) fecha_primera_solicitud,
                            (select max(fecha_solicitud) from prestamo  where id_cliente = c.id_cliente AND ISNULL(activo,1)=1) fecha_ultima_solicitud,
	                            (select count(*)  from prestamo  where id_cliente = c.id_cliente AND ISNULL(activo,1)=1) NoPrestamos,
	                            (select count(*)  from prestamo  where id_cliente = c.id_cliente and id_status_prestamo = 3 AND ISNULL(activo,1)=1) Rechazados,
	                            (select count(*)  from prestamo  where id_aval = c.id_cliente AND ISNULL(activo,1)=1) Aval,
	                            sp.nombre Status ,
	                            sp.color ColorStatus,
                                sp.id_status_prestamo,
                                p.activo
	                    FROM prestamo p
	                    INNER JOIN cliente  c on c.id_cliente  = p.id_cliente
	                    INNER JOIN cliente  av on av.id_cliente  = p.id_aval
	                    INNER JOIN status_prestamo sp on sp.id_status_prestamo = p.id_status_prestamo 
                        WHERE ISNULL(c.eliminado,0) = 0 AND ISNULL(c.activo,1) = 1 AND ISNULL(p.activo,1) = 1
                          AND EXISTS (SELECT 1 FROM cliente cx WHERE cx.id_cliente = p.id_cliente AND ISNULL(cx.eliminado,0)=0 AND ISNULL(cx.activo,1)=1)
                          " + filtroPromotoresSql + @"
                        ) gp
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

        // =============================================================================
        //  ALTA / ACTUALIZACIÓN DE PRÉSTAMOS
        //  Estas operaciones guardan id_empleado correctamente (tomado del usuario logueado).
        // =============================================================================

        [WebMethod]
        public static List<Cliente> GetListaItems(string path, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso) return null;

            using (var conn = new SqlConnection(strConexion))
            {
                conn.Open();
                var ds = new DataSet();
                const string query = @"
                     SELECT c.id_cliente , c.nombre, c.primer_apellido, c.segundo_apellido, 
                            concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                            c.telefono , c.curp, c.ocupacion, c.activo, tc.id_tipo_cliente, tc.tipo_cliente,
                            p.id_prestamo, p.monto, FORMAT(p.fecha_solicitud, 'dd/MM/yyyy') fecha_solicitud
                     FROM cliente c 
                     JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     WHERE isnull(c.eliminado, 0) != 1 
                     ORDER BY id_cliente";

                var adp = new SqlDataAdapter(query, conn);
                adp.Fill(ds);

                var items = new List<Cliente>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        var item = new Cliente
                        {
                            IdCliente = Convert.ToInt32(r["id_cliente"]),
                            IdPrestamo = Convert.ToInt32(r["id_prestamo"]),
                            PrimerApellido = r["primer_apellido"].ToString(),
                            Telefono = r["telefono"].ToString(),
                            Curp = r["curp"].ToString(),
                            Ocupacion = r["ocupacion"].ToString(),
                            IdTipoCliente = Convert.ToInt32(r["id_tipo_cliente"]),
                            TipoCliente = r["tipo_cliente"].ToString(),
                            NombreCompleto = r["nombre_completo"].ToString(),
                            SegundoApellido = r["segundo_apellido"].ToString(),
                            Monto = float.Parse(r["monto"].ToString()),
                            FechaSolicitud = r["fecha_solicitud"].ToString(),
                            Activo = Convert.ToInt32(r["activo"])
                        };

                        var botones = "<button  onclick='client.edit(" + item.IdCliente + ")'  class='btn btn-outline-primary'><span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='client.delete(" + item.IdCliente + ")'   class='btn btn-outline-primary'><span class='fa fa-remove mr-1'></span>Eliminar</button>";
                        item.Accion = botones;

                        items.Add(item);
                    }
                }

                return items;
            }
        }

        [WebMethod]
        public static DatosSalida Save(string path, Cliente item, Direccion itemAddress, Direccion itemAddressAval, string accion, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso) return null;

            var user = Usuarios.GetUsuario(path, idUsuario);
            var validations = new LoanValidation();
            var salida = new DatosSalida();

            using (var conn = new SqlConnection(strConexion))
            {
                conn.Open();
                var tx = conn.BeginTransaction();
                try
                {
                    // Validaciones principales (CURP, aval, historial, etc.)
                    if (validations.GetClienteByCURP(path, item.Curp, conn, strConexion, tx) != null)
                        return new DatosSalida { CodigoError = 1, MensajeError = "Ya existe el cliente con CURP " + item.Curp };

                    if (item.Curp == item.CurpAval)
                        return new DatosSalida { CodigoError = 1, MensajeError = "La CURP del cliente y del aval no debe ser la misma." };

                    if (validations.GetClienteByCURPAvalCliente(path, item.Curp, conn, strConexion, tx) != null)
                        return new DatosSalida { CodigoError = 1, MensajeError = "El cliente se encuentra como aval de otro préstamo." };

                    if (validations.GetClienteByCURPAvalCliente3Veces(path, item.CurpAval, conn, strConexion, tx) > 2)
                        return new DatosSalida { CodigoError = 1, MensajeError = "El aval ya está registrado más de 2 veces en otros préstamos." };

                    if (validations.GetPrestamoByCURP(path, item.Curp, conn, strConexion, tx) != null)
                        return new DatosSalida { CodigoError = 1, MensajeError = "El cliente ya cuenta con un préstamo en proceso." };

                    if (validations.GetHistorialFallaOAbonadoByCustomerCurp(path, item.Curp, conn, strConexion, tx))
                        return new DatosSalida { CodigoError = 1, MensajeError = "El cliente tiene historial de falla o abonado." };

                    // Cliente
                    // Cliente nuevo
                    const string sqlCliente = @"
                        INSERT INTO cliente
                            (curp, nombre, primer_apellido, segundo_apellido, ocupacion, telefono, id_tipo_cliente, 
                             curp_aval, nombre_aval, primer_apellido_aval, segundo_apellido_aval, ocupacion_aval, telefono_aval, 
                             activo, eliminado)
                        OUTPUT INSERTED.id_cliente
                        VALUES (@curp, @nombre, @primer_apellido, @segundo_apellido, @ocupacion, @telefono, @id_tipo_cliente,
                                @curp_aval, @nombre_aval, @primer_apellido_aval, @segundo_apellido_aval, @ocupacion_aval, @telefono_aval,
                                1, 0)";

                    var cmdCliente = new SqlCommand(sqlCliente, conn, tx);
                    cmdCliente.CommandType = CommandType.Text;
                    cmdCliente.Parameters.AddWithValue("@id_tipo_cliente", item.IdTipoCliente);
                    cmdCliente.Parameters.AddWithValue("@curp", item.Curp);
                    cmdCliente.Parameters.AddWithValue("@nombre", item.Nombre);
                    cmdCliente.Parameters.AddWithValue("@primer_apellido", item.PrimerApellido);
                    cmdCliente.Parameters.AddWithValue("@segundo_apellido", item.SegundoApellido);
                    cmdCliente.Parameters.AddWithValue("@ocupacion", item.Ocupacion);
                    cmdCliente.Parameters.AddWithValue("@telefono", item.Telefono);
                    cmdCliente.Parameters.AddWithValue("@curp_aval", item.CurpAval);
                    cmdCliente.Parameters.AddWithValue("@nombre_aval", item.NombreAval);
                    cmdCliente.Parameters.AddWithValue("@primer_apellido_aval", item.PrimerApellidoAval);
                    cmdCliente.Parameters.AddWithValue("@segundo_apellido_aval", item.SegundoApellidoAval);
                    cmdCliente.Parameters.AddWithValue("@telefono_aval", item.TelefonoAval);
                    cmdCliente.Parameters.AddWithValue("@ocupacion_aval", item.OcupacionAval);
                    var idCliente = (int)cmdCliente.ExecuteScalar();

                    // Dirección cliente
                    const string sqlDirCliente = @"
                        INSERT INTO direccion (calleyno, colonia, municipio, estado, codigo_postal, activo, aval, direccion_trabajo, id_cliente)
                        VALUES (@calleyno, @colonia, @municipio, @estado, @codigo_postal, 1, 0, @direccion_trabajo, @id_cliente);";

                    var cmdDir = new SqlCommand(sqlDirCliente, conn, tx);
                    cmdDir.CommandType = CommandType.Text;
                    cmdDir.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmdDir.Parameters.AddWithValue("@calleyno", itemAddress.Calle);
                    cmdDir.Parameters.AddWithValue("@colonia", itemAddress.Colonia);
                    cmdDir.Parameters.AddWithValue("@municipio", itemAddress.Municipio);
                    cmdDir.Parameters.AddWithValue("@estado", itemAddress.Estado);
                    cmdDir.Parameters.AddWithValue("@codigo_postal", itemAddress.CodigoPostal);
                    cmdDir.Parameters.AddWithValue("@direccion_trabajo", itemAddress.DireccionTrabajo);
                    cmdDir.ExecuteNonQuery();

                    // Dirección aval
                    const string sqlDirAval = @"
                        INSERT INTO direccion (calleyno, colonia, municipio, estado, codigo_postal, activo, aval, direccion_trabajo, id_cliente)
                        VALUES (@calleyno, @colonia, @municipio, @estado, @codigo_postal, 1, 1, @direccion_trabajo, @id_cliente);";

                    var cmdDirAval = new SqlCommand(sqlDirAval, conn, tx);
                    cmdDirAval.CommandType = CommandType.Text;
                    cmdDirAval.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmdDirAval.Parameters.AddWithValue("@calleyno", itemAddressAval.Calle);
                    cmdDirAval.Parameters.AddWithValue("@colonia", itemAddressAval.Colonia);
                    cmdDirAval.Parameters.AddWithValue("@municipio", itemAddressAval.Municipio);
                    cmdDirAval.Parameters.AddWithValue("@estado", itemAddressAval.Estado);
                    cmdDirAval.Parameters.AddWithValue("@codigo_postal", itemAddressAval.CodigoPostal);
                    cmdDirAval.Parameters.AddWithValue("@direccion_trabajo", itemAddressAval.DireccionTrabajo);
                    cmdDirAval.ExecuteNonQuery();

                    // Préstamo: guardar id_empleado asociado al usuario
                    const string sqlPrestamo = @"
                        INSERT INTO prestamo (fecha_solicitud, monto, id_cliente, id_usuario, id_status_prestamo, id_empleado)
                        OUTPUT INSERTED.id_prestamo
                        VALUES (@fecha_solicitud, @monto, @id_cliente, @id_usuario, @id_status_prestamo, @id_empleado);";

                    var cmdPrestamo = new SqlCommand(sqlPrestamo, conn, tx);
                    cmdPrestamo.CommandType = CommandType.Text;
                    cmdPrestamo.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmdPrestamo.Parameters.AddWithValue("@fecha_solicitud", item.FechaSolicitud);
                    cmdPrestamo.Parameters.AddWithValue("@monto", item.Monto);
                    cmdPrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdPrestamo.Parameters.AddWithValue("@id_empleado", user.IdEmpleado);
                    cmdPrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PENDIENTE);
                    var idPrestamo = (int)cmdPrestamo.ExecuteScalar();

                    // Rastro de aprobación (supervisor y ejecutivo)
                    const string sqlRel = @"
                        INSERT INTO relacion_prestamo_aprobacion (id_prestamo, id_posicion, id_usuario)
                        VALUES (@id_prestamo, @id_posicion, @id_usuario);";

                    var cmdRelSup = new SqlCommand(sqlRel, conn, tx);
                    cmdRelSup.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    cmdRelSup.Parameters.AddWithValue("@id_posicion", Employees.POSICION_SUPERVISOR);
                    cmdRelSup.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdRelSup.ExecuteNonQuery();

                    var cmdRelEje = new SqlCommand(sqlRel, conn, tx);
                    cmdRelEje.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    cmdRelEje.Parameters.AddWithValue("@id_posicion", Employees.POSICION_EJECUTIVO);
                    cmdRelEje.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdRelEje.ExecuteNonQuery();

                    tx.Commit();
                    salida.CodigoError = 0;
                    salida.MensajeError = "Guardado correctamente";
                    salida.IdItem = idCliente.ToString();
                }
                catch (Exception ex)
                {
                    tx?.Rollback();
                    Utils.Log("Error Save prestamo ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                    salida.CodigoError = 1;
                    salida.MensajeError = "Error al guardar el préstamo.";
                }
            }

            return salida;
        }

        [WebMethod]
        public static DatosSalida SaveLoanUpdateCustomer(string path, Cliente item, Direccion itemAddress, Direccion itemAddressAval, string accion, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            //verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso) return null;

            var user = Usuarios.GetUsuario(path, idUsuario);
            var salida = new DatosSalida();
            var validations = new LoanValidation();

            using (var conn = new SqlConnection(strConexion))
            {
                conn.Open();
                var tx = conn.BeginTransaction();
                try
                {
                    // Validaciones principales
                    var customer = validations.GetClienteByCURP(path, item.Curp, conn, strConexion, tx);
                    if (customer != null && (customer.IdStatusCliente == Cliente.STATUS_CONDONADO || customer.IdStatusCliente == Cliente.STATUS_VENCIDO))
                        return new DatosSalida { CodigoError = 1, MensajeError = "Cliente con status vencido/condonado, no es posible continuar." };

                    var avalCurp = validations.GetClienteByCURP(path, item.CurpAval, conn, strConexion, tx);
                    if (avalCurp != null && (avalCurp.IdStatusCliente == Cliente.STATUS_CONDONADO || avalCurp.IdStatusCliente == Cliente.STATUS_VENCIDO))
                        return new DatosSalida { CodigoError = 1, MensajeError = "El aval pertenece a un cliente vencido/condonado." };

                    if (validations.GetClienteByCURPAvalCliente(path, item.Curp, conn, strConexion, tx) != null)
                        return new DatosSalida { CodigoError = 1, MensajeError = "El cliente se encuentra como aval de otro préstamo." };

                    if (item.Curp == item.CurpAval)
                        return new DatosSalida { CodigoError = 1, MensajeError = "La CURP del cliente y del aval no debe ser la misma." };

                    if (validations.GetClienteByCURPAvalCliente3Veces(path, item.CurpAval, conn, strConexion, tx) > 2)
                        return new DatosSalida { CodigoError = 1, MensajeError = "El aval ya está registrado más de 2 veces en otros préstamos." };

                    if (validations.GetPrestamoByCURP(path, item.Curp, conn, strConexion, tx) != null)
                        return new DatosSalida { CodigoError = 1, MensajeError = "El cliente ya cuenta con un préstamo en proceso." };

                    if (validations.GetHistorialFallaOAbonadoByCustomerId(path, item.IdCliente.ToString(), conn, strConexion, tx))
                        return new DatosSalida { CodigoError = 1, MensajeError = "El cliente tiene historial de falla o abonado." };

                    // Update cliente
                    const string sqlCliente = @"
                        UPDATE cliente
                        SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                            segundo_apellido = @segundo_apellido, ocupacion = @ocupacion, telefono = @telefono,
                            id_tipo_cliente = @id_tipo_cliente, curp_aval = @curp_aval, nombre_aval = @nombre_aval,
                            primer_apellido_aval = @primer_apellido_aval, segundo_apellido_aval = @segundo_apellido_aval,
                            ocupacion_aval = @ocupacion_aval, telefono_aval = @telefono_aval
                        WHERE id_cliente = @id_cliente";

                    var cmdCliente = new SqlCommand(sqlCliente, conn, tx);
                    cmdCliente.Parameters.AddWithValue("@id_tipo_cliente", item.IdTipoCliente);
                    cmdCliente.Parameters.AddWithValue("@curp", item.Curp);
                    cmdCliente.Parameters.AddWithValue("@nombre", item.Nombre);
                    cmdCliente.Parameters.AddWithValue("@primer_apellido", item.PrimerApellido);
                    cmdCliente.Parameters.AddWithValue("@segundo_apellido", item.SegundoApellido);
                    cmdCliente.Parameters.AddWithValue("@ocupacion", item.Ocupacion);
                    cmdCliente.Parameters.AddWithValue("@telefono", item.Telefono);
                    cmdCliente.Parameters.AddWithValue("@curp_aval", item.CurpAval);
                    cmdCliente.Parameters.AddWithValue("@nombre_aval", item.NombreAval);
                    cmdCliente.Parameters.AddWithValue("@primer_apellido_aval", item.PrimerApellidoAval);
                    cmdCliente.Parameters.AddWithValue("@segundo_apellido_aval", item.SegundoApellidoAval);
                    cmdCliente.Parameters.AddWithValue("@ocupacion_aval", item.OcupacionAval);
                    cmdCliente.Parameters.AddWithValue("@telefono_aval", item.TelefonoAval);
                    cmdCliente.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                    cmdCliente.ExecuteNonQuery();

                    // Update dirección cliente
                    const string sqlDirCliente = @"
                        UPDATE direccion
                        SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                            codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo
                        WHERE id_cliente = @id_cliente AND ISNULL(aval,0)=0";

                    var cmdDirCli = new SqlCommand(sqlDirCliente, conn, tx);
                    cmdDirCli.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                    cmdDirCli.Parameters.AddWithValue("@calleyno", itemAddress.Calle);
                    cmdDirCli.Parameters.AddWithValue("@colonia", itemAddress.Colonia);
                    cmdDirCli.Parameters.AddWithValue("@municipio", itemAddress.Municipio);
                    cmdDirCli.Parameters.AddWithValue("@estado", itemAddress.Estado);
                    cmdDirCli.Parameters.AddWithValue("@codigo_postal", itemAddress.CodigoPostal);
                    cmdDirCli.Parameters.AddWithValue("@direccion_trabajo", itemAddress.DireccionTrabajo);
                    cmdDirCli.ExecuteNonQuery();

                    // Update dirección aval
                    const string sqlDirAval = @"
                        UPDATE direccion
                        SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                            codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo
                        WHERE id_cliente = @id_cliente AND ISNULL(aval,0)=1";

                    var cmdDirAv = new SqlCommand(sqlDirAval, conn, tx);
                    cmdDirAv.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                    cmdDirAv.Parameters.AddWithValue("@calleyno", itemAddressAval.Calle);
                    cmdDirAv.Parameters.AddWithValue("@colonia", itemAddressAval.Colonia);
                    cmdDirAv.Parameters.AddWithValue("@municipio", itemAddressAval.Municipio);
                    cmdDirAv.Parameters.AddWithValue("@estado", itemAddressAval.Estado);
                    cmdDirAv.Parameters.AddWithValue("@codigo_postal", itemAddressAval.CodigoPostal);
                    cmdDirAv.Parameters.AddWithValue("@direccion_trabajo", itemAddressAval.DireccionTrabajo);
                    cmdDirAv.ExecuteNonQuery();

                    // Nuevo préstamo (con id_empleado)
                    const string sqlPrestamo = @"
                        INSERT INTO prestamo (fecha_solicitud, monto, id_cliente, id_usuario, id_status_prestamo, id_empleado)
                        OUTPUT INSERTED.id_prestamo
                        VALUES (@fecha_solicitud, @monto, @id_cliente, @id_usuario, @id_status_prestamo, @id_empleado);";

                    var cmdPrestamo = new SqlCommand(sqlPrestamo, conn, tx);
                    cmdPrestamo.CommandType = CommandType.Text;
                    cmdPrestamo.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                    cmdPrestamo.Parameters.AddWithValue("@fecha_solicitud", item.FechaSolicitud);
                    cmdPrestamo.Parameters.AddWithValue("@monto", item.Monto);
                    cmdPrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdPrestamo.Parameters.AddWithValue("@id_empleado", user.IdEmpleado);
                    cmdPrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PENDIENTE);
                    var idPrestamo = (int)cmdPrestamo.ExecuteScalar();

                    // Rastro de aprobación
                    const string sqlRel = @"
                        INSERT INTO relacion_prestamo_aprobacion (id_prestamo, id_posicion, id_usuario)
                        VALUES (@id_prestamo, @id_posicion, @id_usuario);";

                    var cmdRelSup = new SqlCommand(sqlRel, conn, tx);
                    cmdRelSup.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    cmdRelSup.Parameters.AddWithValue("@id_posicion", Employees.POSICION_SUPERVISOR);
                    cmdRelSup.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdRelSup.ExecuteNonQuery();

                    var cmdRelEje = new SqlCommand(sqlRel, conn, tx);
                    cmdRelEje.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    cmdRelEje.Parameters.AddWithValue("@id_posicion", Employees.POSICION_EJECUTIVO);
                    cmdRelEje.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdRelEje.ExecuteNonQuery();

                    tx.Commit();
                    salida.CodigoError = 0;
                    salida.MensajeError = "Guardado correctamente";
                    salida.IdItem = item.IdCliente.ToString();
                }
                catch (Exception ex)
                {
                    tx?.Rollback();
                    Utils.Log("Error SaveLoanUpdateCustomer ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                    salida.CodigoError = 1;
                    salida.MensajeError = "Error al guardar el préstamo.";
                }
            }

            return salida;
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
                                    WHERE id_cliente =  @id_cliente
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
