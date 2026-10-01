using Dapper;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Plataforma.Clases
{
    public class Utils
    {

        public static void Log(string texto)
        {
            System.Diagnostics.Debug.Print(texto);

        }


    }

    public class UserVisibilityContext
    {
        public int IdUsuario { get; set; }
        public int IdTipoUsuario { get; set; }
        public int IdEmpleado { get; set; }
        public int IdPlaza { get; set; }
        public int IdSupervisor { get; set; }
        public int IdEjecutivo { get; set; }

        public bool IsSuperAdmin => IdTipoUsuario == Usuario.TIPO_USUARIO_SUPER_ADMIN;
        public bool IsDirector => IdTipoUsuario == Usuario.TIPO_USUARIO_DIRECTOR;
        public bool IsEjecutivo => IdTipoUsuario == 3;
        public bool IsSupervisor => IdTipoUsuario == 4;
        public bool IsPromotor => IdTipoUsuario == 5;
        public bool IsCapturista => IdTipoUsuario == Usuario.TIPO_USUARIO_CAPTURISTA;
        public bool IsGestor => IdTipoUsuario == Usuario.TIPO_USUARIO_GESTOR;
        public bool IsGerenteCobranza => IdTipoUsuario == Usuario.TIPO_USUARIO_GERENTE_COBRANZA;
    }

    public static class UserVisibilityScope
    {
        public static UserVisibilityContext GetByUser(string path, string idUsuario, SqlConnection existingConnection = null)
        {
            if (!int.TryParse(idUsuario, out var userId) || userId <= 0)
            {
                return new UserVisibilityContext();
            }

            var ownsConnection = existingConnection == null;
            var conn = existingConnection ?? new SqlConnection(ConfigurationManager.ConnectionStrings[path].ConnectionString);
            var shouldClose = false;

            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                    shouldClose = true;
                }

                const string sql = @"
                    SELECT TOP 1
                        ISNULL(u.id_usuario, 0) AS IdUsuario,
                        ISNULL(u.id_tipo_usuario, 0) AS IdTipoUsuario,
                        ISNULL(u.id_empleado, 0) AS IdEmpleado,
                        ISNULL(e.id_plaza, 0) AS IdPlaza,
                        ISNULL(e.id_supervisor, 0) AS IdSupervisor,
                        ISNULL(e.id_ejecutivo, 0) AS IdEjecutivo
                    FROM usuario u
                    LEFT JOIN empleado e ON e.id_empleado = u.id_empleado
                    WHERE u.id_usuario = @userId";

                return conn.QueryFirstOrDefault<UserVisibilityContext>(sql, new { userId })
                    ?? new UserVisibilityContext();
            }
            finally
            {
                if (shouldClose)
                {
                    conn.Close();
                }

                if (ownsConnection)
                {
                    conn.Dispose();
                }
            }
        }

        public static UserVisibilityContext GetCurrent(string path, SqlConnection existingConnection = null)
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var idUsuario = Convert.ToString(session != null ? session["id_usuario"] : string.Empty);
            return GetByUser(path, idUsuario, existingConnection);
        }

        public static int GetFixedPlaza(UserVisibilityContext context, int requestedPlaza)
        {
            if (context == null)
            {
                return requestedPlaza;
            }

            //  Sólo se fuerza la plaza cuando el director realmente tiene una.
            return context.IsDirector && context.IdPlaza > 0 ? context.IdPlaza : requestedPlaza;
        }

        /// <summary>
        /// ¿El usuario pertenece a gerencia de cobranza? (puede asignar cartera y folios)
        /// </summary>
        public static bool EsGerenciaCobranza(UserVisibilityContext context)
        {
            return context != null && (context.IsGerenteCobranza || context.IsDirector);
        }

        /// <summary>
        /// Deriva la columna de id_prestamo a partir de la columna de empleado del
        /// préstamo (p.id_empleado -> p.id_prestamo), para filtrar por cartera asignada.
        /// </summary>
        private static string LoanIdColumnFrom(string loanEmployeeColumn)
        {
            if (string.IsNullOrEmpty(loanEmployeeColumn))
            {
                return "id_prestamo";
            }

            var idx = loanEmployeeColumn.LastIndexOf('.');
            return idx > 0
                ? loanEmployeeColumn.Substring(0, idx) + ".id_prestamo"
                : "id_prestamo";
        }

        /// <summary>
        /// Filtro SQL: sólo los préstamos asignados a este gestor de cobranza.
        /// </summary>
        private static string BuildGestorCarteraSql(UserVisibilityContext context, string loanIdColumn)
        {
            if (context.IdEmpleado <= 0)
            {
                return " AND 1 = 0 ";
            }

            return @" AND EXISTS (
                        SELECT 1
                        FROM cobranza_asignacion ca_scope
                        WHERE ca_scope.id_prestamo = " + loanIdColumn + @"
                          AND ca_scope.id_gestor = " + context.IdEmpleado + @"
                          AND ISNULL(ca_scope.activo, 0) = 1
                          AND ISNULL(ca_scope.eliminado, 0) = 0
                    ) ";
        }

        public static string BuildLoanEmployeeScopeSql(UserVisibilityContext context, string loanEmployeeColumn)
        {
            if (context == null)
            {
                return " AND 1 = 0 ";
            }

            //  Sin un usuario resuelto (sesión caída o llamada sin autenticar) no se
            //  expone información: el filtro debe cerrar, no abrir.
            if (context.IdUsuario <= 0)
            {
                return " AND 1 = 0 ";
            }

            if (context.IsSuperAdmin)
            {
                return string.Empty;
            }

            //  Gerente de cobranza: ve todas las plazas (filtra con el combo de plaza).
            if (context.IsGerenteCobranza)
            {
                return string.Empty;
            }

            //  Gestor de cobranza: sólo las cuentas que gerencia le asignó.
            if (context.IsGestor)
            {
                return BuildGestorCarteraSql(context, LoanIdColumnFrom(loanEmployeeColumn));
            }

            //  Director: si tiene plaza se acota a ella; si NO tiene plaza
            //  (administrativo de oficina) ve la información de todas las plazas.
            if (context.IsDirector)
            {
                return context.IdPlaza > 0
                    ? $" AND EXISTS (SELECT 1 FROM empleado e_scope WHERE e_scope.id_empleado = {loanEmployeeColumn} AND e_scope.id_plaza = {context.IdPlaza}) "
                    : string.Empty;
            }

            if (context.IsPromotor)
            {
                return context.IdEmpleado > 0
                    ? $" AND {loanEmployeeColumn} = {context.IdEmpleado} "
                    : " AND 1 = 0 ";
            }

            if (context.IsSupervisor)
            {
                return context.IdEmpleado > 0
                    ? $" AND {loanEmployeeColumn} IN (SELECT e_scope.id_empleado FROM empleado e_scope WHERE e_scope.id_supervisor = {context.IdEmpleado}) "
                    : " AND 1 = 0 ";
            }

            if (context.IsEjecutivo)
            {
                return context.IdEmpleado > 0
                    ? @" AND " + loanEmployeeColumn + @" IN (
                            SELECT e_scope.id_empleado
                            FROM empleado e_scope
                            WHERE e_scope.id_supervisor IN (
                                SELECT s.id_empleado
                                FROM empleado s
                                WHERE s.id_ejecutivo = " + context.IdEmpleado + @"
                            )
                        ) "
                    : " AND 1 = 0 ";
            }

            return string.Empty;
        }

        public static string BuildEmployeeScopeSql(UserVisibilityContext context, string employeeColumn)
        {
            if (context == null)
            {
                return " AND 1 = 0 ";
            }

            //  Sin un usuario resuelto no se expone información.
            if (context.IdUsuario <= 0)
            {
                return " AND 1 = 0 ";
            }

            if (context.IsSuperAdmin)
            {
                return string.Empty;
            }

            //  Gerente de cobranza: ve todas las plazas.
            if (context.IsGerenteCobranza)
            {
                return string.Empty;
            }

            //  Gestor: sólo los empleados dueños de los créditos de su cartera.
            if (context.IsGestor)
            {
                return context.IdEmpleado > 0
                    ? @" AND EXISTS (
                            SELECT 1
                            FROM cobranza_asignacion ca_scope
                            INNER JOIN prestamo p_scope ON p_scope.id_prestamo = ca_scope.id_prestamo
                            WHERE ca_scope.id_gestor = " + context.IdEmpleado + @"
                              AND ISNULL(ca_scope.activo, 0) = 1
                              AND ISNULL(ca_scope.eliminado, 0) = 0
                              AND p_scope.id_empleado = " + employeeColumn + @"
                        ) "
                    : " AND 1 = 0 ";
            }

            //  Director sin plaza (administrativo de oficina): ve todas las plazas.
            if (context.IsDirector)
            {
                return context.IdPlaza > 0
                    ? $" AND EXISTS (SELECT 1 FROM empleado e_scope WHERE e_scope.id_empleado = {employeeColumn} AND e_scope.id_plaza = {context.IdPlaza}) "
                    : string.Empty;
            }

            if (context.IsEjecutivo)
            {
                return context.IdEmpleado > 0
                    ? @" AND (
                            " + employeeColumn + @" = " + context.IdEmpleado + @"
                            OR EXISTS (
                                SELECT 1
                                FROM empleado e_scope
                                WHERE e_scope.id_empleado = " + employeeColumn + @"
                                  AND e_scope.id_ejecutivo = " + context.IdEmpleado + @"
                            )
                            OR EXISTS (
                                SELECT 1
                                FROM empleado e_scope
                                WHERE e_scope.id_empleado = " + employeeColumn + @"
                                  AND e_scope.id_supervisor IN (
                                      SELECT s.id_empleado
                                      FROM empleado s
                                      WHERE s.id_ejecutivo = " + context.IdEmpleado + @"
                                  )
                            )
                        ) "
                    : " AND 1 = 0 ";
            }

            if (context.IsSupervisor)
            {
                return context.IdEmpleado > 0
                    ? @" AND (
                            " + employeeColumn + @" = " + context.IdEmpleado
                            + (context.IdEjecutivo > 0 ? @" OR " + employeeColumn + @" = " + context.IdEjecutivo : string.Empty)
                            + @" OR EXISTS (
                                SELECT 1
                                FROM empleado e_scope
                                WHERE e_scope.id_empleado = " + employeeColumn + @"
                                  AND e_scope.id_supervisor = " + context.IdEmpleado + @"
                            )
                        ) "
                    : " AND 1 = 0 ";
            }

            if (context.IsPromotor)
            {
                return context.IdEmpleado > 0
                    ? @" AND (
                            " + employeeColumn + @" = " + context.IdEmpleado
                            + (context.IdSupervisor > 0 ? @" OR " + employeeColumn + @" = " + context.IdSupervisor : string.Empty)
                            + (context.IdEjecutivo > 0 ? @" OR " + employeeColumn + @" = " + context.IdEjecutivo : string.Empty)
                            + @" ) "
                    : " AND 1 = 0 ";
            }

            return string.Empty;
        }

        public static bool CanAccessEmployee(UserVisibilityContext context, SqlConnection conn, int idEmpleado, int? requiredPosition = null)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM empleado e
                WHERE e.id_empleado = @idEmpleado"
                + (requiredPosition.HasValue ? " AND e.id_posicion = @requiredPosition " : string.Empty)
                + BuildEmployeeScopeSql(context, "e.id_empleado");

            return conn.ExecuteScalar<int>(sql, new { idEmpleado, requiredPosition }) > 0;
        }

        public static bool CanAccessPrestamo(UserVisibilityContext context, SqlConnection conn, int idPrestamo)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM prestamo p
                WHERE p.id_prestamo = @idPrestamo"
                + BuildLoanEmployeeScopeSql(context, "p.id_empleado");

            return conn.ExecuteScalar<int>(sql, new { idPrestamo }) > 0;
        }

        public static bool CanAccessPago(UserVisibilityContext context, SqlConnection conn, int idPago)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM pago pg
                INNER JOIN prestamo p ON p.id_prestamo = pg.id_prestamo
                WHERE pg.id_pago = @idPago"
                + BuildLoanEmployeeScopeSql(context, "p.id_empleado");

            return conn.ExecuteScalar<int>(sql, new { idPago }) > 0;
        }

        public static bool CanAccessCliente(UserVisibilityContext context, SqlConnection conn, int idCliente)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM prestamo p
                WHERE p.id_cliente = @idCliente"
                + BuildLoanEmployeeScopeSql(context, "p.id_empleado");

            return conn.ExecuteScalar<int>(sql, new { idCliente }) > 0;
        }

        public static bool CanAccessClienteByCurp(UserVisibilityContext context, SqlConnection conn, string curp)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM prestamo p
                INNER JOIN cliente c ON c.id_cliente = p.id_cliente
                WHERE c.curp = @curp"
                + BuildLoanEmployeeScopeSql(context, "p.id_empleado");

            return conn.ExecuteScalar<int>(sql, new { curp }) > 0;
        }
    }
}
