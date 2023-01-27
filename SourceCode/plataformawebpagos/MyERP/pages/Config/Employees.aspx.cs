using Dapper;
using Newtonsoft.Json.Linq;
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
    public partial class Employees : System.Web.UI.Page
    {
        const string pagina = "8";

        public const int POSICION_DIRECTOR = 1;
        public const int POSICION_COORDINADOR = 2;
        public const int POSICION_EJECUTIVO = 3;
        public const int POSICION_SUPERVISOR = 4;
        public const int POSICION_PROMOTOR = 5;
        public const int SUPERUSUARIO = 6;


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
        public static List<RequestGridEmpleados> GetListaItems(string path, string idUsuario, 
            ResponseGridEmpleados Filtro)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            var conn = new SqlConnection(strConexion);
            var items = new List<RequestGridEmpleados>();

            try
            {
                string query = @" SELECT e.id_empleado IdEmpleado,
	                concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) NombreCompleto,
	                ISNull(e.activo, 1) Activo,
	                u.login Usuario,
	                e.fecha_ingreso FechaIngreso,
                     e.id_posicion,
	                m.nombre Modulo,  
	                pos.nombre Tipo,
	                p.nombre Plaza , 
	                IsNull(concat(sup.nombre ,  ' ' , sup.primer_apellido , ' ' , sup.segundo_apellido),'No asignado') NombreSupervisor,
	                IsNull(concat(eje.nombre ,  ' ' , eje.primer_apellido , ' ' , eje.segundo_apellido),'No asignado') NombreEjecutivo,
                    m.id_comision,
	                p.id_plaza,
	                pos.id_posicion
                FROM empleado e 
                JOIN usuario u ON (u.id_empleado = e.id_empleado) 
                JOIN comision m ON (m.id_comision = e.id_comision_inicial) 
                JOIN plaza p ON (p.id_plaza = e.id_plaza) 
                JOIN posicion pos ON (pos.id_posicion = e.id_posicion)
                LEFT JOIN empleado sup ON (sup.id_empleado = e.id_supervisor)
                LEFT JOIN empleado eje ON (eje.id_empleado  = e.id_ejecutivo)
                    ";

                query += "WHERE isnull(e.eliminado, 0) != 1 ";

                if (!string.IsNullOrWhiteSpace(Filtro.NombreCompleto))
                {
                    query += $@" AND  concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) like '%{Filtro.NombreCompleto}%'";
                }

                if (!string.IsNullOrWhiteSpace(Filtro.Usuario))
                {
                    query += $@" AND u.login  like '%{Filtro.Usuario}%'";
                }

                if (!string.IsNullOrWhiteSpace(Filtro.NombreEjecutivo))
                {
                    query += $@" AND concat(eje.nombre ,  ' ' , eje.primer_apellido , ' ' , eje.segundo_apellido) like '%{Filtro.NombreEjecutivo}%'";
                }

                if (!string.IsNullOrWhiteSpace(Filtro.NombreSupervisor))
                {
                    query += $@" AND  concat(sup.nombre ,  ' ' , sup.primer_apellido , ' ' , sup.segundo_apellido) like '%{Filtro.NombreSupervisor}%'";
                }

                if (Filtro.IdTipo.HasValue)
                {
                    query += $@" AND  pos.id_posicion = {Filtro.IdTipo.Value}";
                }

                if (Filtro.IdPlaza.HasValue)
                {
                    query += $@" AND  p.id_plaza = {Filtro.IdPlaza.Value}";
                }

                if (Filtro.IdModulo.HasValue)
                {
                    query += $@" AND m.id_comision = {Filtro.IdModulo.Value}";
                }
                
                if (Filtro.Activo.HasValue)
                {
                    query += $@" AND  e.activo = {Filtro.Activo.Value}";
                }

                if (Filtro.FechaIngreso.HasValue)
                {
                    query += $@" AND   Convert(Date,e.fecha_ingreso) = '{Filtro.FechaIngreso.Value.ToString("yyyy/MM/dd")}'";
                }

                items = conn.Query<RequestGridEmpleados>(query).ToList() ?? new List<RequestGridEmpleados>();

                return items;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return items;
            }
        }

        [WebMethod]
        public static List<Posicion> GetListaItemsPosiciones(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Posicion> items = new List<Posicion>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_posicion, nombre FROM  posicion WHERE ISNull(eliminado, 0) = 0 AND id_posicion <> 6  ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Posicion item = new Posicion();
                        item.IdPosicion = int.Parse(ds.Tables[0].Rows[i]["id_posicion"].ToString());
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
        public static List<Plaza> GetListaItemsPlazas(string path)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Plaza> items = new List<Plaza>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_plaza, nombre FROM  plaza WHERE IsNull(activo, 1) = 1  AND ISNull(eliminado, 0) = 0   ";

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
        public static List<Comision> GetListaItemsComisiones(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Comision> items = new List<Comision>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_comision, nombre FROM comision WHERE IsNull(activo, 1) = 1  AND ISNull(eliminado, 0) = 0   ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Comision item = new Comision();
                        item.IdComision = int.Parse(ds.Tables[0].Rows[i]["id_comision"].ToString());
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
        public static DatosSalida Delete(string path, string id)
        {
            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                string sql = "";
                sql = @" UPDATE empleado set eliminado = 1 WHERE id_empleado = @id_empleado ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_empleado", id);

                int r = cmd.ExecuteNonQuery();
                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");

                salida.MensajeError = null;
                salida.CodigoError = 0;

                return salida;
            }
            catch (Exception ex)
            {

                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo eliminar el Empleado.";

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return salida;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}