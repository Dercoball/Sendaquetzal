using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Services;

namespace Plataforma.pages.Cobranza
{
    public partial class Reporte : System.Web.UI.Page
    {
        const string pagina = "93";

        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NRAiBiAaIQQuGjN/V0Z+WE9EaFtGVmJLYVB3WmpQdldgdVRMZVVbQX9PIiBoS35RdUViW39fc3RTQmFVWUR2");

            txtUsuario.Value = usuario;
            txtIdTipoUsuario.Value = idTipoUsuario;
            txtIdUsuario.Value = idUsuario;

            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }
        }

        /// <summary>
        /// Reporte semanal de cobranza (Bloque B). Corte de lunes a domingo.
        /// Si no se envían fechas, usa la semana actual.
        /// </summary>
        [WebMethod]
        public static List<CobranzaCobro> GetReporteSemanal(string path, string idUsuario, int idPlaza, int idGestor,
            string fechaInicial, string fechaFinal)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            if (!Index.TienePermisoPagina(pagina, path, idUsuario))
            {
                return null;
            }

            DateTime ini, fin;
            ResolverSemana(fechaInicial, fechaFinal, out ini, out fin);

            List<CobranzaCobro> items = new List<CobranzaCobro>();
            using (SqlConnection conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();
                    var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);

                    string sqlScope = scope.IsGestor ? " AND cc.id_gestor = " + scope.IdEmpleado + " " : "";
                    string sqlPlaza = idPlaza > 0 ? " AND ep.id_plaza = " + idPlaza + " " : "";
                    string sqlGestor = idGestor > 0 ? " AND cc.id_gestor = " + idGestor + " " : "";

                    string query = @"
                        SELECT
                            cc.id_cobranza_cobro,
                            cc.id_prestamo,
                            CONVERT(VARCHAR(10), cc.fecha_cobro, 103) AS fecha_cobro,
                            DATEPART(ISO_WEEK, cc.fecha_cobro) AS numero_semana,
                            ISNULL(cc.id_gestor, 0) AS id_gestor,
                            LTRIM(RTRIM(CONCAT(g.nombre, ' ', g.primer_apellido, ' ', g.segundo_apellido))) AS gestor,
                            ISNULL(ug.login, '') AS usuario_gestor,
                            CONVERT(VARCHAR(10), pre.fecha_solicitud, 103) AS fecha_inicio,
                            pl.nombre AS plaza,
                            LTRIM(RTRIM(CONCAT(c.nombre, ' ', c.primer_apellido, ' ', c.segundo_apellido))) AS cliente,
                            ISNULL(cc.monto_recuperado, 0) AS monto,
                            cc.observaciones,
                            ISNULL(cc.id_canal_pago, 0) AS id_canal,
                            ca.nombre AS canal,
                            f.folio,
                            ISNULL(cc.porcentaje_comision, 0) AS pct,
                            ISNULL(cc.bucket, 0) AS bucket,
                            ISNULL(cc.semanas_vencimiento, 0) AS semanas,
                            ISNULL(cc.comision, 0) AS comision
                        FROM cobranza_cobro cc
                            LEFT JOIN prestamo pre ON pre.id_prestamo = cc.id_prestamo
                            LEFT JOIN cliente c    ON c.id_cliente = pre.id_cliente
                            LEFT JOIN empleado g   ON g.id_empleado = cc.id_gestor
                            LEFT JOIN empleado ep  ON ep.id_empleado = pre.id_empleado
                            LEFT JOIN plaza pl     ON pl.id_plaza = ep.id_plaza
                            LEFT JOIN usuario ug   ON ug.id_empleado = cc.id_gestor
                            LEFT JOIN cobranza_canal_pago ca ON ca.id_canal_pago = cc.id_canal_pago
                            LEFT JOIN cobranza_folio f ON f.id_folio = cc.id_folio
                        WHERE ISNULL(cc.eliminado, 0) = 0
                          AND cc.fecha_cobro BETWEEN @ini AND @fin
                          " + sqlPlaza + sqlGestor + sqlScope + @"
                        ORDER BY cc.fecha_cobro, gestor";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ini", ini);
                        cmd.Parameters.AddWithValue("@fin", fin);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                CobranzaCobro item = new CobranzaCobro();
                                item.IdCobranzaCobro = ToInt(dr["id_cobranza_cobro"]);
                                item.IdPrestamo = ToInt(dr["id_prestamo"]);
                                item.FechaCobro = dr["fecha_cobro"].ToString();
                                item.NumeroSemanaReporte = ToInt(dr["numero_semana"]);
                                item.PeriodoReporte = "Semana " + item.NumeroSemanaReporte;
                                item.IdGestor = ToInt(dr["id_gestor"]);
                                item.Gestor = dr["gestor"].ToString();
                                item.UsuarioGestor = dr["usuario_gestor"].ToString();
                                item.FechaInicioCredito = dr["fecha_inicio"].ToString();
                                item.Plaza = dr["plaza"].ToString();
                                item.Grupo = string.Empty; // POR CONFIRMAR: no existe entidad "grupo"
                                item.NombreCliente = dr["cliente"].ToString();
                                item.MontoRecuperado = ToDouble(dr["monto"]);
                                item.MontoRecuperadoMx = item.MontoRecuperado.ToString("C2");
                                item.Observaciones = dr["observaciones"].ToString();
                                item.IdCanalPago = ToInt(dr["id_canal"]);
                                item.CanalPago = dr["canal"].ToString();
                                item.Folio = dr["folio"].ToString();
                                item.PorcentajeComision = ToDouble(dr["pct"]);
                                item.PorcentajeComisionStr = (item.PorcentajeComision * 100).ToString("0.#") + "%";
                                item.Bucket = ToInt(dr["bucket"]);
                                item.SemanasVencimiento = ToInt(dr["semanas"]);
                                item.Comision = ToDouble(dr["comision"]);
                                item.ComisionMx = item.Comision.ToString("C2");
                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                }
            }
            return items;
        }

        /// <summary>
        /// Resumen económico de cobranza por gestor (Bloque C).
        /// </summary>
        [WebMethod]
        public static List<CobranzaResumen> GetResumenEconomico(string path, string idUsuario, int idPlaza, int idGestor,
            string fechaInicial, string fechaFinal)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            if (!Index.TienePermisoPagina(pagina, path, idUsuario))
            {
                return null;
            }

            DateTime ini, fin;
            ResolverSemana(fechaInicial, fechaFinal, out ini, out fin);

            List<CobranzaResumen> items = new List<CobranzaResumen>();
            using (SqlConnection conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();
                    var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);

                    string sqlScope = scope.IsGestor ? " AND cc.id_gestor = " + scope.IdEmpleado + " " : "";
                    string sqlPlaza = idPlaza > 0 ? " AND ep.id_plaza = " + idPlaza + " " : "";
                    string sqlGestor = idGestor > 0 ? " AND cc.id_gestor = " + idGestor + " " : "";

                    string query = @"
                        SELECT
                            ISNULL(cc.id_gestor, 0) AS id_gestor,
                            LTRIM(RTRIM(CONCAT(g.nombre, ' ', g.primer_apellido, ' ', g.segundo_apellido))) AS gestor,
                            SUM(ISNULL(cc.monto_recuperado, 0)) AS total,
                            SUM(CASE WHEN ISNULL(ca.no_tangible, 0) = 1 THEN ISNULL(cc.monto_recuperado, 0) ELSE 0 END) AS no_tangible,
                            SUM(ISNULL(cc.comision, 0)) AS comisiones
                        FROM cobranza_cobro cc
                            LEFT JOIN empleado g  ON g.id_empleado = cc.id_gestor
                            LEFT JOIN prestamo pre ON pre.id_prestamo = cc.id_prestamo
                            LEFT JOIN empleado ep ON ep.id_empleado = pre.id_empleado
                            LEFT JOIN cobranza_canal_pago ca ON ca.id_canal_pago = cc.id_canal_pago
                        WHERE ISNULL(cc.eliminado, 0) = 0
                          AND cc.fecha_cobro BETWEEN @ini AND @fin
                          " + sqlPlaza + sqlGestor + sqlScope + @"
                        GROUP BY cc.id_gestor, LTRIM(RTRIM(CONCAT(g.nombre, ' ', g.primer_apellido, ' ', g.segundo_apellido)))
                        ORDER BY gestor";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ini", ini);
                        cmd.Parameters.AddWithValue("@fin", fin);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                CobranzaResumen item = new CobranzaResumen();
                                item.IdGestor = ToInt(dr["id_gestor"]);
                                item.Gestor = dr["gestor"].ToString();
                                item.TotalCobrado = ToDouble(dr["total"]);
                                item.NoTangible = ToDouble(dr["no_tangible"]);
                                item.Comisiones = ToDouble(dr["comisiones"]);
                                item.Diferencia = item.TotalCobrado - item.NoTangible - item.Comisiones;

                                item.TotalCobradoMx = item.TotalCobrado.ToString("C2");
                                item.NoTangibleMx = item.NoTangible.ToString("C2");
                                item.ComisionesMx = item.Comisiones.ToString("C2");
                                item.DiferenciaMx = item.Diferencia.ToString("C2");
                                items.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                }
            }
            return items;
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------

        /// <summary>
        /// Resuelve el rango de la semana (lunes-domingo). Si no vienen fechas
        /// válidas, toma la semana actual.
        /// </summary>
        private static void ResolverSemana(string fechaInicial, string fechaFinal, out DateTime ini, out DateTime fin)
        {
            DateTime i, f;
            bool okI = DateTime.TryParse(fechaInicial, out i);
            bool okF = DateTime.TryParse(fechaFinal, out f);

            if (okI && okF)
            {
                ini = i.Date;
                fin = f.Date;
                return;
            }

            DateTime hoy = DateTime.Today;
            int diff = (7 + (int)hoy.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            ini = hoy.AddDays(-diff).Date;      // lunes
            fin = ini.AddDays(6);               // domingo
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
