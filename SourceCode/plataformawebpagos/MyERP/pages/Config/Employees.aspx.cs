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
        [WebMethod]
        public static Empleado GetDataEmployee(string path, string id)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Empleado item = new Empleado();
            SqlConnection conn = new SqlConnection(strConexion);


            try
            {
                conn.Open();


                item = GetItemEmployee(path, id, conn, strConexion);
                item.Direccion = GetAddress(path, id, 0, conn, strConexion);
                item.DireccionAval = GetAddress(path, id, 1, conn, strConexion);
                item.usuario = GetUser(path, id, conn, strConexion);

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
        public static Empleado GetItemEmployee(string path, string id, SqlConnection conn, string strconexion)
        {

            Empleado item = new Empleado();

            try
            {

                DataSet ds = new DataSet();
                string query = @" SELECT id_empleado, id_tipo_usuario, id_comision_inicial, id_posicion, id_plaza, curp, email, 
                                    nombre, primer_apellido, segundo_apellido, telefono, eliminado, 
                                    activo, IsNull(id_supervisor, 0) id_supervisor, IsNull(id_ejecutivo, 0) id_ejecutivo, 
                                    IsNull(id_coordinador, 0) id_coordinador, FORMAT(fecha_ingreso, 'yyyy-MM-dd') fecha_ingreso, 
                                    FORMAT(fecha_nacimiento, 'yyyy-MM-dd') fecha_nacimiento,
                                    monto_limite_inicial,
                                    nombre_aval, primer_apellido_aval, segundo_apellido_aval, curp_aval, telefono_aval,
                                    concat(nombre ,  ' ' , primer_apellido , ' ' , segundo_apellido) AS nombre_completo,
                                    concat(nombre_aval ,  ' ' , primer_apellido_aval , ' ' , segundo_apellido_aval) AS nombre_completo_aval
                                FROM empleado
                                WHERE id_empleado =  @id_empleado 
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_empleado =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_empleado", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Empleado();

                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.IdPosicion = int.Parse(ds.Tables[0].Rows[i]["id_posicion"].ToString());
                        item.IdPlaza = int.Parse(ds.Tables[0].Rows[i]["id_plaza"].ToString());
                        item.IdComisionInicial = int.Parse(ds.Tables[0].Rows[i]["id_comision_inicial"].ToString());

                        item.IdSupervisor = int.Parse(ds.Tables[0].Rows[i]["id_supervisor"].ToString());
                        item.IdEjecutivo = int.Parse(ds.Tables[0].Rows[i]["id_ejecutivo"].ToString());
                        item.IdCoordinador = int.Parse(ds.Tables[0].Rows[i]["id_coordinador"].ToString());

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.CURP = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.MontoLimiteInicial = float.Parse(ds.Tables[0].Rows[i]["monto_limite_inicial"].ToString());
                        item.FechaIngreso = DateTime.TryParse(ds.Tables[0].Rows[i]["fecha_ingreso"].ToString(), out DateTime fechaIngreso) ? fechaIngreso : default(DateTime);
                        item.FechaNacimiento = ds.Tables[0].Rows[i]["fecha_nacimiento"].ToString();
                        item.CURPAval = ds.Tables[0].Rows[i]["curp_aval"].ToString();
                        item.NombreAval = ds.Tables[0].Rows[i]["nombre_aval"].ToString();
                        item.NombreCompletoAval = ds.Tables[0].Rows[i]["nombre_completo_aval"].ToString();
                        item.PrimerApellidoAval = ds.Tables[0].Rows[i]["primer_apellido_aval"].ToString();
                        item.SegundoApellidoAval = ds.Tables[0].Rows[i]["segundo_apellido_aval"].ToString();
                        item.TelefonoAval = ds.Tables[0].Rows[i]["telefono_aval"].ToString();



                        //item.NombreTipoUsuario = ds.Tables[0].Rows[i]["nombre_tipo_usuario"].ToString();
                        //item.NombrePlaza = ds.Tables[0].Rows[i]["nombre_plaza"].ToString();



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
        public static Direccion GetAddress(string path, string idEmpleado, int aval, SqlConnection conn, string strconexion)
        {

            Direccion item = new Direccion();

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT id_direccion, id_empleado, id_aval, calleyno, colonia, municipio, estado, 
                                    codigo_postal, id_municipio, id_estado, activo, ISNULL(aval, 0) aval
                                    FROM direccion
                                    WHERE id_empleado =  @id_empleado AND aval = @aval
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_empleado =  " + idEmpleado);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_empleado", idEmpleado);
                adp.SelectCommand.Parameters.AddWithValue("@aval", aval);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Direccion();

                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.Aval = int.Parse(ds.Tables[0].Rows[i]["aval"].ToString());
                        item.Calle = ds.Tables[0].Rows[i]["calleyno"].ToString();
                        item.Colonia = ds.Tables[0].Rows[i]["colonia"].ToString();
                        item.Municipio = ds.Tables[0].Rows[i]["municipio"].ToString();
                        item.Estado = ds.Tables[0].Rows[i]["estado"].ToString();
                        item.CodigoPostal = ds.Tables[0].Rows[i]["codigo_postal"].ToString();




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

            // Reemplaza la asignación de FechaIngreso en el método GetItemEmployee por un parseo seguro a DateTime
        }
        [WebMethod]
        public static Usuario GetUser(string path, string id, SqlConnection conn, string strconexion)
        {

            Usuario item = new Usuario();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_usuario, id_tipo_usuario,
                                IsNull(id_empleado, 0) id_empleado, nombre, login, password, email, telefono 
                                FROM usuario 
                                WHERE id_empleado = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item.IdUsuario = int.Parse(ds.Tables[0].Rows[i]["id_usuario"].ToString());
                        item.IdTipoUsuario = int.Parse(ds.Tables[0].Rows[i]["id_tipo_usuario"].ToString());

                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Login = ds.Tables[0].Rows[i]["login"].ToString();
                        item.Password = ds.Tables[0].Rows[i]["password"].ToString();
                        item.Email = ds.Tables[0].Rows[i]["email"].ToString();




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
        public static Documento GetDocument(string path, string idEmpleado, string idTipoDocumento)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Documento item = new Documento();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_documento_colaborador, contenido, extension, nombre 
                                  FROM documento    
                                  WHERE id_empleado = @id AND id_tipo_documento = @id_tipo_documento ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("idEmpleado =  " + idEmpleado);
                Utils.Log("idTipoDocumento =  " + idTipoDocumento);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idEmpleado);
                adp.SelectCommand.Parameters.AddWithValue("@id_tipo_documento", idTipoDocumento);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    item.IdDocumento = int.Parse(ds.Tables[0].Rows[0]["id_documento_colaborador"].ToString());
                    item.Contenido = ds.Tables[0].Rows[0]["contenido"].ToString();
                    item.Extension = ds.Tables[0].Rows[0]["extension"].ToString();
                    item.Nombre = ds.Tables[0].Rows[0]["nombre"].ToString();
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
    }
}