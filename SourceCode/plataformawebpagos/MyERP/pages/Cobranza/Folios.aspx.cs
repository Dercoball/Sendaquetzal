using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace Plataforma.pages.Cobranza
{
    public partial class Folios : System.Web.UI.Page
    {
        const string pagina = "92";

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
        /// Folios registrados (opcionalmente filtrados por gestor).
        /// </summary>
        [WebMethod]
        public static List<CobranzaFolio> GetFolios(string path, string idUsuario, int idGestor)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            if (!Index.TienePermisoPagina(pagina, path, idUsuario))
            {
                return null;
            }

            List<CobranzaFolio> items = new List<CobranzaFolio>();
            using (SqlConnection conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();
                    string sqlGestor = idGestor > 0 ? " AND f.id_gestor = @g " : "";
                    string query = @"
                        SELECT f.id_folio, f.folio, ISNULL(f.id_gestor,0) id_gestor,
                               LTRIM(RTRIM(CONCAT(e.nombre, ' ', e.primer_apellido))) AS gestor,
                               ISNULL(f.estatus,1) estatus,
                               CONVERT(VARCHAR(16), f.fecha_asignacion, 120) AS fecha_asignacion,
                               CONVERT(VARCHAR(16), f.fecha_uso, 120) AS fecha_uso
                        FROM cobranza_folio f
                        LEFT JOIN empleado e ON e.id_empleado = f.id_gestor
                        WHERE ISNULL(f.eliminado,0) = 0 " + sqlGestor + @"
                        ORDER BY f.estatus, f.folio";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (idGestor > 0) cmd.Parameters.AddWithValue("@g", idGestor);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                CobranzaFolio f = new CobranzaFolio
                                {
                                    IdFolio = ToInt(dr["id_folio"]),
                                    Folio = dr["folio"].ToString(),
                                    IdGestor = ToInt(dr["id_gestor"]),
                                    Gestor = dr["gestor"].ToString(),
                                    Estatus = ToInt(dr["estatus"]),
                                    FechaAsignacion = dr["fecha_asignacion"].ToString(),
                                    FechaUso = dr["fecha_uso"].ToString()
                                };
                                f.EstatusStr = f.Estatus == CobranzaFolio.ESTATUS_USADO ? "Usado"
                                             : f.Estatus == CobranzaFolio.ESTATUS_CANCELADO ? "Cancelado" : "Disponible";
                                items.Add(f);
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
        /// Asigna un block de folios a un gestor: genera prefijo+n para n en [desde, hasta].
        /// Sólo gerencia de cobranza.
        /// </summary>
        [WebMethod]
        public static DatosSalida AsignarFolios(string path, string idUsuario, int idGestor, string prefijo, int desde, int hasta)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            DatosSalida response = new DatosSalida();

            if (!Index.TienePermisoPagina(pagina, path, idUsuario))
            {
                response.CodigoError = 1;
                response.MensajeError = "No tiene permisos.";
                return response;
            }
            if (idGestor <= 0 || hasta < desde || (hasta - desde) > 1000)
            {
                response.CodigoError = 1;
                response.MensajeError = "Rango de folios inválido (máximo 1000 por block).";
                return response;
            }

            using (SqlConnection conn = new SqlConnection(strConexion))
            {
                SqlTransaction tran = null;
                try
                {
                    conn.Open();
                    var scope = UserVisibilityScope.GetByUser(path, idUsuario, conn);
                    if (!UserVisibilityScope.EsGerenciaCobranza(scope))
                    {
                        response.CodigoError = 1;
                        response.MensajeError = "Sólo gerencia de cobranza puede asignar folios.";
                        return response;
                    }

                    tran = conn.BeginTransaction();
                    int creados = 0;
                    for (int n = desde; n <= hasta; n++)
                    {
                        string folio = (prefijo ?? string.Empty) + n;
                        using (SqlCommand cmd = new SqlCommand(
                            @"IF NOT EXISTS (SELECT 1 FROM cobranza_folio WHERE folio = @folio AND ISNULL(eliminado,0)=0)
                              INSERT INTO cobranza_folio (folio, id_gestor, id_usuario_asigna, estatus, fecha_asignacion, eliminado)
                              VALUES (@folio, @gestor, @usuario, 1, GETDATE(), 0)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@folio", folio);
                            cmd.Parameters.AddWithValue("@gestor", idGestor);
                            cmd.Parameters.AddWithValue("@usuario", scope.IdUsuario);
                            creados += cmd.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                    response.IdItem = creados.ToString();
                    return response;
                }
                catch (Exception ex)
                {
                    if (tran != null) { try { tran.Rollback(); } catch { } }
                    Utils.Log("Error ... " + ex.Message);
                    response.CodigoError = 1;
                    response.MensajeError = "No se pudieron asignar los folios.";
                    return response;
                }
            }
        }

        /// <summary>
        /// Gestores de cobranza (posición 7) para el combo de asignación.
        /// </summary>
        [WebMethod]
        public static List<Empleado> GetGestores(string path, string idUsuario)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            if (!Index.TienePermisoPagina(pagina, path, idUsuario))
            {
                return null;
            }

            List<Empleado> items = new List<Empleado>();
            using (SqlConnection conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT e.id_empleado,
                               LTRIM(RTRIM(CONCAT(e.nombre, ' ', e.primer_apellido, ' ', e.segundo_apellido))) AS nombre_completo
                        FROM empleado e
                        WHERE e.id_posicion = 7 AND ISNULL(e.eliminado,0)=0 AND ISNULL(e.activo,1)=1
                        ORDER BY nombre_completo";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
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
                catch (Exception ex)
                {
                    Utils.Log("Error ... " + ex.Message);
                }
            }
            return items;
        }

        private static int ToInt(object o)
        {
            if (o == null || o == DBNull.Value) return 0;
            int v;
            return int.TryParse(o.ToString(), out v) ? v : 0;
        }
    }
}
