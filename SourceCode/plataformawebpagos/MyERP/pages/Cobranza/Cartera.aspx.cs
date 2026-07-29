using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace Plataforma.pages.Cobranza
{
    public partial class Cartera : System.Web.UI.Page
    {
        const string pagina = "90";

        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];
            var scope = UserVisibilityScope.GetByUser(path, idUsuario);
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NRAiBiAaIQQuGjN/V0Z+WE9EaFtGVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUViW39fc3RTQmFVWUR2");

            txtUsuario.Value = usuario;
            txtIdTipoUsuario.Value = idTipoUsuario;
            txtIdUsuario.Value = idUsuario;
            txtIdEmpleado.Value = scope.IdEmpleado > 0 ? scope.IdEmpleado.ToString() : string.Empty;
            txtIdPlaza.Value = (scope.IsSupervisor || scope.IsDirector) && scope.IdPlaza > 0
                ? scope.IdPlaza.ToString()
                : string.Empty;

            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }
        }

        /// <summary>
        /// Cartera de cobranza (Bloque A): préstamos que ya cayeron a cobranza.
        /// Una cuenta cae cuando las semanas transcurridas desde el inicio del
        /// crédito >= parámetro "semana_caida_cobranza" y aún tiene saldo.
        /// El vencimiento se calcula por FECHA (no depende del status manual "Falla").
        /// </summary>
        [WebMethod]
        public static List<CobranzaCuenta> GetCartera(string path, string idUsuario, string idTipoUsuario, int idPlaza)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;
            }

            List<CobranzaCuenta> items = new List<CobranzaCuenta>();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);

                // Parámetros de negocio (editables en cobranza_parametro)
                int semanaCaida = (int)GetParametro(conn, "semana_caida_cobranza", 17);
                double montoMulta = GetParametro(conn, "monto_multa", 50);
                double pctGasto = GetParametro(conn, "porcentaje_gasto_cobranza", 0.10);

                List<CobranzaReglaComision> reglas = GetReglasComision(conn);

                // Visibilidad: el Gestor (posición/tipo 7) sólo ve su cartera asignada;
                // gerencia/dirección ve según su alcance por plaza/jerarquía.
                string sqlScope;
                if (scope.IsGestor)
                {
                    sqlScope = " AND asg.id_gestor = " + scope.IdEmpleado + " ";
                }
                else
                {
                    sqlScope = UserVisibilityScope.BuildLoanEmployeeScopeSql(scope, "pre.id_empleado");
                }

                string sqlPlaza = idPlaza > 0 ? " AND e.id_plaza = " + idPlaza + " " : "";

                string query = @"
                    SELECT
                        pre.id_prestamo,
                        c.id_cliente,
                        CONVERT(VARCHAR(10), pre.fecha_solicitud, 103) AS fecha_credito,
                        DATEDIFF(DAY, pre.fecha_solicitud, GETDATE()) / 7 AS semanas_transcurridas,
                        LTRIM(RTRIM(CONCAT(c.nombre, ' ', c.primer_apellido, ' ', c.segundo_apellido))) AS nombre_cliente,
                        c.telefono AS telefono_cliente,
                        dc.calleyno AS calle_cliente,
                        dc.colonia  AS colonia_cliente,
                        LTRIM(RTRIM(CONCAT(a1.nombre, ' ', a1.primer_apellido, ' ', a1.segundo_apellido))) AS nombre_aval1,
                        a1.telefono AS telefono_aval1,
                        da1.calleyno AS calle_aval1,
                        da1.colonia  AS colonia_aval1,
                        LTRIM(RTRIM(CONCAT(a2.nombre, ' ', a2.primer_apellido, ' ', a2.segundo_apellido))) AS nombre_aval2,
                        a2.telefono AS telefono_aval2,
                        da2.calleyno AS calle_aval2,
                        da2.colonia  AS colonia_aval2,
                        tc.tipo_cliente AS producto,
                        pl.nombre AS plaza,
                        LTRIM(RTRIM(CONCAT(eco.nombre, ' ', eco.primer_apellido))) AS coordinador,
                        LTRIM(RTRIM(CONCAT(esu.nombre, ' ', esu.primer_apellido))) AS supervisor,
                        LTRIM(RTRIM(CONCAT(eje.nombre, ' ', eje.primer_apellido))) AS ejecutivo,
                        LTRIM(RTRIM(CONCAT(e.nombre, ' ', e.primer_apellido, ' ', e.segundo_apellido))) AS promotor,
                        ISNULL(asg.id_gestor, 0) AS id_gestor,
                        LTRIM(RTRIM(CONCAT(g.nombre, ' ', g.primer_apellido))) AS gestor,
                        asg.observaciones,
                        ISNULL((SELECT SUM(p.saldo) FROM pago p WHERE p.id_prestamo = pre.id_prestamo), 0) AS adeudo,
                        ISNULL((SELECT TOP 1 p.monto FROM pago p WHERE p.id_prestamo = pre.id_prestamo ORDER BY p.numero_semana), 0) AS pago_semanal,
                        ISNULL((
                            SELECT COUNT(*)
                            FROM pago p
                            WHERE p.id_prestamo = pre.id_prestamo
                              AND p.saldo > 0
                              AND p.fecha < CAST(GETDATE() AS DATE)
                        ), 0) AS numero_fallas
                    FROM prestamo pre
                        INNER JOIN cliente c        ON c.id_cliente = pre.id_cliente
                        LEFT JOIN tipo_cliente tc   ON tc.id_tipo_cliente = pre.id_tipo_cliente
                        LEFT JOIN direccion dc      ON dc.id_cliente = c.id_cliente AND ISNULL(dc.aval, 0) = 0
                        LEFT JOIN cliente a1        ON a1.id_cliente = pre.id_aval
                        LEFT JOIN direccion da1     ON da1.id_cliente = a1.id_cliente AND ISNULL(da1.aval, 0) = 0
                        LEFT JOIN cliente a2        ON a2.id_cliente = pre.id_aval2
                        LEFT JOIN direccion da2     ON da2.id_cliente = a2.id_cliente AND ISNULL(da2.aval, 0) = 0
                        LEFT JOIN empleado e        ON e.id_empleado = pre.id_empleado
                        LEFT JOIN plaza pl          ON pl.id_plaza = e.id_plaza
                        LEFT JOIN empleado eco      ON eco.id_empleado = e.id_coordinador
                        LEFT JOIN empleado esu      ON esu.id_empleado = e.id_supervisor
                        LEFT JOIN empleado eje      ON eje.id_empleado = e.id_ejecutivo
                        LEFT JOIN cobranza_asignacion asg ON asg.id_prestamo = pre.id_prestamo
                                                          AND ISNULL(asg.activo, 0) = 1 AND ISNULL(asg.eliminado, 0) = 0
                        LEFT JOIN empleado g        ON g.id_empleado = asg.id_gestor
                    WHERE pre.id_status_prestamo = 4
                      AND DATEDIFF(DAY, pre.fecha_solicitud, GETDATE()) / 7 >= " + semanaCaida + @"
                      AND EXISTS (SELECT 1 FROM pago p WHERE p.id_prestamo = pre.id_prestamo AND p.saldo > 0)
                      " + sqlPlaza + sqlScope + @"
                    ORDER BY semanas_transcurridas DESC, pre.id_prestamo DESC";

                Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow r = ds.Tables[0].Rows[i];

                        CobranzaCuenta item = new CobranzaCuenta();
                        item.IdPrestamo = ToInt(r["id_prestamo"]);
                        item.IdCliente = ToInt(r["id_cliente"]);
                        item.FechaCredito = r["fecha_credito"].ToString();
                        item.SemanasTranscurridas = ToInt(r["semanas_transcurridas"]);

                        item.NombreCliente = r["nombre_cliente"].ToString();
                        item.TelefonoCliente = r["telefono_cliente"].ToString();
                        item.CalleCliente = r["calle_cliente"].ToString();
                        item.ColoniaCliente = r["colonia_cliente"].ToString();

                        item.NombreAval1 = r["nombre_aval1"].ToString();
                        item.TelefonoAval1 = r["telefono_aval1"].ToString();
                        item.CalleAval1 = r["calle_aval1"].ToString();
                        item.ColoniaAval1 = r["colonia_aval1"].ToString();

                        item.NombreAval2 = r["nombre_aval2"].ToString();
                        item.TelefonoAval2 = r["telefono_aval2"].ToString();
                        item.CalleAval2 = r["calle_aval2"].ToString();
                        item.ColoniaAval2 = r["colonia_aval2"].ToString();

                        item.Producto = r["producto"].ToString();
                        item.Plaza = r["plaza"].ToString();
                        item.Coordinador = r["coordinador"].ToString();
                        item.Supervisor = r["supervisor"].ToString();
                        item.Ejecutivo = r["ejecutivo"].ToString();
                        item.Promotor = r["promotor"].ToString();

                        item.IdGestor = ToInt(r["id_gestor"]);
                        item.Gestor = r["gestor"].ToString();
                        item.Observaciones = r["observaciones"].ToString();

                        item.PagoSemanal = ToDouble(r["pago_semanal"]);
                        item.PagoSemanalMx = item.PagoSemanal.ToString("C2");

                        // --- Cálculos del diagrama (Bloque A) ---
                        item.Adeudo = ToDouble(r["adeudo"]);                       // 14
                        item.NumeroFallas = ToInt(r["numero_fallas"]);
                        item.Multas = item.NumeroFallas * montoMulta;              // 15
                        item.Subtotal = item.Adeudo + item.Multas;                // 16
                        item.GastoCobranza = item.Subtotal * pctGasto;            // 17 (10%)
                        item.TotalCobranza = item.Subtotal + item.GastoCobranza;  // 18 (POR CONFIRMAR: suma)
                        item.SaldoActualizado = item.TotalCobranza;               // 19
                        item.Bucket = CalcularBucket(item.SemanasTranscurridas, reglas);

                        item.AdeudoMx = item.Adeudo.ToString("C2");
                        item.MultasMx = item.Multas.ToString("C2");
                        item.SubtotalMx = item.Subtotal.ToString("C2");
                        item.GastoCobranzaMx = item.GastoCobranza.ToString("C2");
                        item.TotalCobranzaMx = item.TotalCobranza.ToString("C2");
                        item.SaldoActualizadoMx = item.SaldoActualizado.ToString("C2");

                        string botones = "<button data-idprestamo='" + item.IdPrestamo + "' onclick='cartera.ver(" + item.IdPrestamo + ")' class='btn btn-outline-primary btn-sm'><span class='fa fa-folder-open mr-1'></span>Ver</button>";
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

        /// <summary>
        /// Gestores de cobranza disponibles (empleados con posición Gestor = 7).
        /// </summary>
        [WebMethod]
        public static List<Empleado> GetGestores(string path, string idUsuario, int idPlaza)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;
            }

            List<Empleado> items = new List<Empleado>();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                string sqlPlaza = idPlaza > 0 ? " AND e.id_plaza = @id_plaza " : "";
                string query = @"
                    SELECT e.id_empleado,
                           LTRIM(RTRIM(CONCAT(e.nombre, ' ', e.primer_apellido, ' ', e.segundo_apellido))) AS nombre_completo
                    FROM empleado e
                    WHERE e.id_posicion = 7
                      AND ISNULL(e.eliminado, 0) = 0
                      AND ISNULL(e.activo, 1) = 1
                      " + sqlPlaza + @"
                    ORDER BY nombre_completo";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (idPlaza > 0)
                    {
                        cmd.Parameters.AddWithValue("@id_plaza", idPlaza);
                    }
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new Empleado
                            {
                                IdEmpleado = ToInt(dr["id_empleado"]),
                                NombreCompleto = dr["nombre_completo"].ToString()
                            });
                        }
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

        /// <summary>
        /// Asigna (o reasigna) una cuenta a un gestor. Sólo gerencia de cobranza.
        /// </summary>
        [WebMethod]
        public static DatosSalida AsignarGestor(string path, string idUsuario, int idPrestamo, int idGestor)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            DatosSalida response = new DatosSalida();
            SqlConnection conn = new SqlConnection(strConexion);
            SqlTransaction tran = null;

            try
            {
                conn.Open();
                var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);

                // Sólo gerencia de cobranza asigna cartera (Director/Coordinador/Superadmin).
                // POR CONFIRMAR: rol dedicado "Gerente de cobranza".
                if (!UserVisibilityScope.EsGerenciaCobranza(scope))
                {
                    response.CodigoError = 1;
                    response.MensajeError = "Sólo gerencia de cobranza puede asignar cartera.";
                    return response;
                }

                tran = conn.BeginTransaction();

                // Desactiva la asignación activa previa de esta cuenta
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE cobranza_asignacion SET activo = 0 WHERE id_prestamo = @id_prestamo AND ISNULL(activo,0) = 1", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO cobranza_asignacion (id_prestamo, id_gestor, id_usuario_asigna, fecha_asignacion, activo, eliminado)
                      VALUES (@id_prestamo, @id_gestor, @id_usuario, GETDATE(), 1, 0)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    cmd.Parameters.AddWithValue("@id_gestor", idGestor);
                    cmd.Parameters.AddWithValue("@id_usuario", scope.IdUsuario);
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                response.IdItem = idPrestamo.ToString();
                return response;
            }
            catch (Exception ex)
            {
                if (tran != null)
                {
                    try { tran.Rollback(); } catch { }
                }
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                response.CodigoError = 1;
                response.MensajeError = "No se pudo asignar la cuenta.";
                return response;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Guarda observaciones de cobranza sobre una cuenta.
        /// Sólo gerente o asistente de gerencia (Director/Coordinador/Superadmin).
        /// Requiere que la cuenta tenga un gestor asignado.
        /// </summary>
        [WebMethod]
        public static DatosSalida GuardarObservaciones(string path, string idUsuario, int idPrestamo, string observaciones)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            DatosSalida response = new DatosSalida();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);

                if (!UserVisibilityScope.EsGerenciaCobranza(scope))
                {
                    response.CodigoError = 1;
                    response.MensajeError = "Sólo gerencia puede editar observaciones.";
                    return response;
                }

                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE cobranza_asignacion SET observaciones = @obs WHERE id_prestamo = @id_prestamo AND ISNULL(activo,0) = 1", conn))
                {
                    cmd.Parameters.AddWithValue("@obs", (object)observaciones ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows < 1)
                    {
                        response.CodigoError = 1;
                        response.MensajeError = "Asigna un gestor a la cuenta antes de capturar observaciones.";
                        return response;
                    }
                }

                response.IdItem = idPrestamo.ToString();
                return response;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                response.CodigoError = 1;
                response.MensajeError = "No se pudieron guardar las observaciones.";
                return response;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Catálogo de canales de pago de cobranza.
        /// </summary>
        [WebMethod]
        public static List<CobranzaCanalPago> GetCanales(string path)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            List<CobranzaCanalPago> items = new List<CobranzaCanalPago>();
            using (SqlConnection conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT id_canal_pago, nombre, ISNULL(no_tangible,0) no_tangible FROM cobranza_canal_pago WHERE ISNULL(activo,0)=1 ORDER BY id_canal_pago", conn))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new CobranzaCanalPago
                            {
                                IdCanalPago = ToInt(dr["id_canal_pago"]),
                                Nombre = dr["nombre"].ToString(),
                                NoTangible = ToInt(dr["no_tangible"])
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log("Error ... " + ex.Message);
                }
            }
            return items;
        }

        /// <summary>
        /// Folios disponibles asignados a un gestor.
        /// </summary>
        [WebMethod]
        public static List<CobranzaFolio> GetFoliosDisponibles(string path, int idGestor)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            List<CobranzaFolio> items = new List<CobranzaFolio>();
            using (SqlConnection conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT id_folio, folio FROM cobranza_folio WHERE id_gestor = @g AND estatus = 1 AND ISNULL(eliminado,0)=0 ORDER BY folio", conn))
                    {
                        cmd.Parameters.AddWithValue("@g", idGestor);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                items.Add(new CobranzaFolio
                                {
                                    IdFolio = ToInt(dr["id_folio"]),
                                    Folio = dr["folio"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log("Error ... " + ex.Message);
                }
            }
            return items;
        }

        /// <summary>
        /// Registra un cobro/recuperación de cobranza (Bloque B).
        /// Aplica el monto a los pagos con saldo, calcula bucket y comisión,
        /// liga el folio y, si liquida el crédito, lo pasa a Pagado.
        /// </summary>
        [WebMethod]
        public static DatosSalida RegistrarCobro(string path, string idUsuario, int idPrestamo, int idGestor,
            double montoRecuperado, int idCanalPago, string folio, string fechaCobro, string observaciones)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            DatosSalida response = new DatosSalida();

            if (!Index.TienePermisoPagina(pagina, path, idUsuario))
            {
                response.CodigoError = 1;
                response.MensajeError = "No tiene permisos.";
                return response;
            }
            if (montoRecuperado <= 0)
            {
                response.CodigoError = 1;
                response.MensajeError = "El monto recuperado debe ser mayor a cero.";
                return response;
            }

            SqlConnection conn = new SqlConnection(strConexion);
            SqlTransaction tran = null;
            try
            {
                conn.Open();
                var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);

                DateTime fechaCobroDate;
                if (!DateTime.TryParse(fechaCobro, out fechaCobroDate))
                {
                    fechaCobroDate = DateTime.Today;
                }

                // Datos del crédito
                DateTime fechaSolicitud = DateTime.Today;
                int idCliente = 0;
                using (SqlCommand cmd = new SqlCommand("SELECT fecha_solicitud, id_cliente FROM prestamo WHERE id_prestamo = @p", conn))
                {
                    cmd.Parameters.AddWithValue("@p", idPrestamo);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            if (dr["fecha_solicitud"] != DBNull.Value) fechaSolicitud = Convert.ToDateTime(dr["fecha_solicitud"]);
                            idCliente = ToInt(dr["id_cliente"]);
                        }
                    }
                }

                int semanasVenc = (int)((fechaCobroDate - fechaSolicitud).TotalDays / 7);

                List<CobranzaReglaComision> reglas = GetReglasComision(conn);
                CobranzaReglaComision regla = null;
                foreach (var r in reglas)
                {
                    if (semanasVenc >= r.SemanaDesde && semanasVenc <= r.SemanaHasta) { regla = r; break; }
                }
                int bucket = regla != null ? regla.Bucket : 0;
                double pct = regla != null ? regla.Porcentaje : 0;
                double comision = montoRecuperado * pct;

                tran = conn.BeginTransaction();

                // 1) Aplicar la recuperación a los pagos con saldo (más antiguos primero)
                ApplyRecoveryToPagos(idPrestamo, montoRecuperado, scope.IdUsuario, conn, tran);

                // 2) Registrar el cobro
                int idCobro;
                using (SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO cobranza_cobro
                        (id_prestamo, id_gestor, id_usuario, fecha_cobro, monto_recuperado, id_canal_pago, id_folio,
                         semanas_vencimiento, bucket, porcentaje_comision, comision, observaciones, fecha_registro, eliminado)
                      VALUES
                        (@id_prestamo, @id_gestor, @id_usuario, @fecha_cobro, @monto, @id_canal, NULL,
                         @semanas, @bucket, @pct, @comision, @obs, GETDATE(), 0);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                    cmd.Parameters.AddWithValue("@id_gestor", idGestor > 0 ? (object)idGestor : DBNull.Value);
                    cmd.Parameters.AddWithValue("@id_usuario", scope.IdUsuario);
                    cmd.Parameters.AddWithValue("@fecha_cobro", fechaCobroDate);
                    cmd.Parameters.AddWithValue("@monto", montoRecuperado);
                    cmd.Parameters.AddWithValue("@id_canal", idCanalPago > 0 ? (object)idCanalPago : DBNull.Value);
                    cmd.Parameters.AddWithValue("@semanas", semanasVenc);
                    cmd.Parameters.AddWithValue("@bucket", bucket);
                    cmd.Parameters.AddWithValue("@pct", pct);
                    cmd.Parameters.AddWithValue("@comision", comision);
                    cmd.Parameters.AddWithValue("@obs", (object)observaciones ?? DBNull.Value);
                    idCobro = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 3) Ligar folio (si se capturó y existe disponible para el gestor)
                if (!string.IsNullOrEmpty(folio))
                {
                    using (SqlCommand cmd = new SqlCommand(
                        @"UPDATE cobranza_folio SET estatus = 2, fecha_uso = GETDATE(), id_cobranza_cobro = @id_cobro
                          WHERE folio = @folio AND id_gestor = @gestor AND estatus = 1", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@id_cobro", idCobro);
                        cmd.Parameters.AddWithValue("@folio", folio);
                        cmd.Parameters.AddWithValue("@gestor", idGestor);
                        cmd.ExecuteNonQuery();
                    }
                    // Guarda el folio también en el cobro (por si es texto libre)
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE cobranza_cobro SET id_folio = (SELECT TOP 1 id_folio FROM cobranza_folio WHERE folio=@folio AND id_gestor=@gestor) WHERE id_cobranza_cobro=@id_cobro", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@folio", folio);
                        cmd.Parameters.AddWithValue("@gestor", idGestor);
                        cmd.Parameters.AddWithValue("@id_cobro", idCobro);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 4) Si el crédito quedó sin saldo, pasarlo a Pagado y al cliente inactivo
                int pendientes;
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM pago WHERE id_prestamo = @p AND saldo > 0", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@p", idPrestamo);
                    pendientes = Convert.ToInt32(cmd.ExecuteScalar());
                }
                if (pendientes == 0)
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE prestamo SET id_status_prestamo = 5 WHERE id_prestamo = @p", conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@p", idPrestamo);
                        cmd.ExecuteNonQuery();
                    }
                    if (idCliente > 0)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE cliente SET id_status_cliente = 1 WHERE id_cliente = @c", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@c", idCliente);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                tran.Commit();
                response.IdItem = idCobro.ToString();
                return response;
            }
            catch (Exception ex)
            {
                if (tran != null) { try { tran.Rollback(); } catch { } }
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                response.CodigoError = 1;
                response.MensajeError = "No se pudo registrar el cobro.";
                return response;
            }
            finally
            {
                conn.Close();
            }
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        /// <summary>
        /// Aplica un monto recuperado a los pagos con saldo (semana más antigua
        /// primero), registra cada abono en abono_pago y liquida el pago si se cubre.
        /// </summary>
        private static void ApplyRecoveryToPagos(int idPrestamo, double monto, int idUsuario, SqlConnection conn, SqlTransaction tran)
        {
            if (monto <= 0) return;

            List<Pago> pagos = new List<Pago>();
            using (SqlCommand cmd = new SqlCommand(
                "SELECT id_pago, ISNULL(saldo,0) saldo FROM pago WHERE id_prestamo = @p AND saldo > 0 ORDER BY numero_semana", conn, tran))
            {
                cmd.Parameters.AddWithValue("@p", idPrestamo);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        pagos.Add(new Pago { IdPago = ToInt(dr["id_pago"]), Saldo = ToDouble(dr["saldo"]) });
                    }
                }
            }

            double restante = monto;
            foreach (var p in pagos)
            {
                if (restante <= 0) break;
                double aplicar = Math.Min(restante, p.Saldo);

                using (SqlCommand cmd = new SqlCommand(
                    @"UPDATE pago
                        SET pagado = ISNULL(pagado,0) + @a,
                            saldo = saldo - @a,
                            es_recuperado = 1,
                            fecha_registro_pago = GETDATE(),
                            id_status_pago = CASE WHEN (saldo - @a) <= 0 THEN 4 ELSE id_status_pago END
                      WHERE id_pago = @id", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@a", aplicar);
                    cmd.Parameters.AddWithValue("@id", p.IdPago);
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO abono_pago (id_pago, monto, fecha, id_usuario) VALUES (@id, @a, GETDATE(), @u)", conn, tran))
                {
                    cmd.Parameters.AddWithValue("@id", p.IdPago);
                    cmd.Parameters.AddWithValue("@a", aplicar);
                    cmd.Parameters.AddWithValue("@u", idUsuario);
                    cmd.ExecuteNonQuery();
                }

                restante -= aplicar;
            }
        }

        private static double GetParametro(SqlConnection conn, string nombre, double valorDefault)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT valor FROM cobranza_parametro WHERE nombre = @n", conn))
                {
                    cmd.Parameters.AddWithValue("@n", nombre);
                    object o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                    {
                        return Convert.ToDouble(o);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("GetParametro error ... " + ex.Message);
            }
            return valorDefault;
        }

        private static List<CobranzaReglaComision> GetReglasComision(SqlConnection conn)
        {
            List<CobranzaReglaComision> reglas = new List<CobranzaReglaComision>();
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT id_regla, bucket, semana_desde, semana_hasta, porcentaje FROM cobranza_regla_comision WHERE ISNULL(activo,0)=1 AND ISNULL(eliminado,0)=0 ORDER BY semana_desde", conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        reglas.Add(new CobranzaReglaComision
                        {
                            IdRegla = ToInt(dr["id_regla"]),
                            Bucket = ToInt(dr["bucket"]),
                            SemanaDesde = ToInt(dr["semana_desde"]),
                            SemanaHasta = ToInt(dr["semana_hasta"]),
                            Porcentaje = (float)ToDouble(dr["porcentaje"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Log("GetReglasComision error ... " + ex.Message);
            }
            return reglas;
        }

        private static int CalcularBucket(int semanas, List<CobranzaReglaComision> reglas)
        {
            foreach (var r in reglas)
            {
                if (semanas >= r.SemanaDesde && semanas <= r.SemanaHasta)
                {
                    return r.Bucket;
                }
            }
            return 0;
        }

        private static int ToInt(object o)
        {
            if (o == null || o == DBNull.Value) return 0;
            int v;
            return int.TryParse(o.ToString(), out v) ? v : 0;
        }

        private static double ToDouble(object o)
        {
            if (o == null || o == DBNull.Value) return 0;
            double v;
            return double.TryParse(o.ToString(), out v) ? v : 0;
        }
    }
}
