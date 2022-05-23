using Newtonsoft.Json;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class Usuarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// Obtiene la lista de todos los usuarios existentes
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns></returns>
        [WebMethod]
        public static List<Usuario> GetListaUsuarios(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Usuario> items = new List<Usuario>();

          

            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                string esSuperUser = idUsuario != "1" ? "   AND  u.id_tipo_usuario <> @usuarioSuperAdmin  " : "   ";

                string query = "SELECT u.id_usuario, u.id_tipo_usuario, u.nombre, u.login," +
                    " u.password, u.email, u.telefono, tp.nombre nombre_tipo_usuario " +
                    " FROM usuario u " +
                    " JOIN tipo_usuario tp ON (u.id_tipo_usuario = tp.id_tipo_usuario)" +
                    " WHERE IsNull(u.eliminado, 0) = 0  " + esSuperUser;


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@usuarioSuperAdmin", Usuario.TIPO_USUARIO_SUPER_ADMIN);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Usuario item = new Usuario();
                        item.IdUsuario = int.Parse(ds.Tables[0].Rows[i]["id_usuario"].ToString());
                        item.IdTipoUsuario = int.Parse(ds.Tables[0].Rows[i]["id_tipo_usuario"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreTipoUsuario = ds.Tables[0].Rows[i]["nombre_tipo_usuario"].ToString();
                        item.Login = ds.Tables[0].Rows[i]["login"].ToString();
                        item.Password = ds.Tables[0].Rows[i]["password"].ToString();
                        item.Email = ds.Tables[0].Rows[i]["email"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();

                        string botones = "<button  title=\"Editar\" onclick='editar(" + item.IdUsuario + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += " &nbsp; <button  title=\"Eliminar\" onclick='eliminar(" + item.IdUsuario + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";
                        botones += " &nbsp; <button  title=\"Asignar contraseña\" onclick='editarP(" + item.IdUsuario + ")'   class='btn btn-outline-primary'> <span class='fa fa-lock mr-1'></span>Contraseña</button>";
                        botones += " &nbsp; <button  title=\"Asignar equipos\" onclick='asignarEquipos(" + item.IdUsuario + ", \"" + item.Nombre + "\")'   class='btn btn-outline-primary'> <span class='fa fa-car mr-1'></span>Equipos</button>";
                        //botones += " &nbsp; <button  title=\"Asignar permisos\" onclick='permisos(" + item.Id_Usuario + ",\"" + item.Nombre + "\")'   class='btn btn-outline-primary'> <span class='fa fa-key'></span>Permisos</button>";

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
       [WebMethod]
        public static string GetListaEmpleados(string path, string idUsuario, string like)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();



            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_empleado, nombre, apellido_paterno, apellido_materno, clave " +
                               " FROM empleado where IsNull(activo, 1) <> 0 order by id_empleado ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                string salida = "";
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Empleado item = new Empleado();
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.APaterno = ds.Tables[0].Rows[i]["apellido_paterno"].ToString();
                        item.AMaterno = ds.Tables[0].Rows[i]["apellido_materno"].ToString();
                        //item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();
                        //item.NombreCompleto = item.Clave + " - " + item.Nombre + " " + item.APaterno + " " + item.AMaterno;



                        items.Add(item);


                    }
                }

                if (ds.Tables[0].Rows.Count > 0)
                {

                    salida = JsonConvert.SerializeObject(items, Newtonsoft.Json.Formatting.Indented);

                }
                return salida;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return "";
            }

            finally
            {
                conn.Close();
            }

        }


        [WebMethod]
        public static object GuardarUsuarioP(string path, Usuario usuario, string accion)
        {

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                MD5 md5Hash = MD5.Create();
                string hash = GetMd5Hash(md5Hash, usuario.Password);


                conn.Open();
                string sql = "";

                sql = " UPDATE usuario " +
                      " SET password = @password  " +
                      " WHERE id_usuario = @id ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = hash;
                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = usuario.IdUsuario;



                int r = cmd.ExecuteNonQuery();
                Utils.Log("Guardado -> OK ");



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1; //Retornamos menos uno cuando se dió por alguna razón un error
            }

            finally
            {
                conn.Close();
            }

        }

    
        [WebMethod]
        public static Usuario GetUsuario(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Usuario item = new Usuario();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_usuario, id_tipo_usuario," +
                               " IsNull(id_empleado, 0) id_empleado, nombre, login, password, email, telefono " +
                               " FROM usuario " +
                               " WHERE id_usuario = @id ";

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
                        item.IdEmpleado= int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Login = ds.Tables[0].Rows[i]["login"].ToString();
                        item.Password = ds.Tables[0].Rows[i]["password"].ToString();
                        item.Email = ds.Tables[0].Rows[i]["email"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();



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

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="usuario"></param>
        /// <param name="accion"></param>
        /// <returns></returns>
        [WebMethod]
        public static object GuardarUsuario(string path, Usuario usuario, string accion)
        {

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {



                conn.Open();
                string sql = "";
                if (accion == "nuevo")
                {
                    sql = " INSERT INTO usuario (id_tipo_usuario, nombre, login, email, telefono, id_empleado) " +
                                       " VALUES ( @id_tipo_usuario, @nombre, @login, @email, @telefono, @id_empleado)";
                }
                else
                {
                    string proveedorSql = " , id_empleado = @id_empleado ";

                    sql = " UPDATE usuario " +
                          " SET id_tipo_usuario = @id_tipo_usuario, nombre = @nombre, login = @login, " +
                          "email = @email, telefono = @telefono  " + proveedorSql + 
                          " WHERE id_usuario = @id ";

                }

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id_tipo_usuario",  usuario.IdTipoUsuario);
                cmd.Parameters.AddWithValue("@nombre",  usuario.Nombre);
                cmd.Parameters.AddWithValue("@login",  usuario.Login);
                cmd.Parameters.AddWithValue("@email",  usuario.Email);
                cmd.Parameters.AddWithValue("@telefono",  usuario.Telefono);
                cmd.Parameters.AddWithValue("@id",  usuario.IdUsuario);

          
                cmd.Parameters.AddWithValue("@id_empleado",usuario.IdEmpleado);
                



                int r = cmd.ExecuteNonQuery();
                Utils.Log("Guardado -> OK ");



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1; //Retornamos menos uno cuando se dió por alguna razón un error
            }

            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// Webmethod que elimina un registro
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="id">Id del cliente a eliminar</param>
        /// <returns>Retorna el número de registros manipulados, ó -1 cuando no se pudo eliminar el registro</returns>
        [WebMethod]
        public static object EliminarUsuario(string path, string id)
        {

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = "";

                sql = " UPDATE usuario " +
                        " SET eliminado = 1 WHERE id_usuario = @id ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;


                int r = cmd.ExecuteNonQuery();
                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }

            finally
            {
                conn.Close();
            }

        }

        /// <summary>
        /// Obtiene la lista de los tipos de usuario 
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns></returns>
        [WebMethod]
        public static List<TipoUsuario> GetListaTiposUsuario(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoUsuario> items = new List<TipoUsuario>();





            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_tipo_usuario, nombre FROM tipo_usuario where id_tipo_usuario <> @usuarioSuperAdmin order by nombre ";

                //TipoUsuario item0 = new TipoUsuario();
                //item0.IdTipoUsuario = 0;
                //item0.Nombre = "Seleccione...";
                //items.Add(item0);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                adp.SelectCommand.Parameters.AddWithValue("@usuarioSuperAdmin", Usuario.TIPO_USUARIO_SUPER_ADMIN);
            
                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                //string salida = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TipoUsuario item = new TipoUsuario();
                        item.IdTipoUsuario = int.Parse(ds.Tables[0].Rows[i]["id_tipo_usuario"].ToString());
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
        public static List<PermisoUsuario> ObtenerListaPermisosPorTipoUsuario(string path, string idTipoUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<PermisoUsuario> items = new List<PermisoUsuario>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @"  SELECT p.nombre, p.id_permiso, p.nombre_recurso, p.nombre_interno, p.tipo_permiso 
                                     from permisos_tipo_usuario rel_ptu 
                                     join permisos p on (p.id_permiso = rel_ptu .id_permiso) 
                                        where rel_ptu.id_tipo_usuario  = @id_tipo_usuario 
                                        AND IsNull(p.activo, 0) = 1 ORDER BY p.nombre ";


                Utils.Log("idtipoUsuario =  " + idTipoUsuario);
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_tipo_usuario", idTipoUsuario);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PermisoUsuario item = new PermisoUsuario();
                        item.IdPermiso = int.Parse(ds.Tables[0].Rows[i]["id_permiso"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreInterno = ds.Tables[0].Rows[i]["nombre_interno"].ToString();
                        item.NombreRecurso = ds.Tables[0].Rows[i]["nombre_recurso"].ToString();
                        item.TipoPermiso = ds.Tables[0].Rows[i]["tipo_permiso"].ToString();



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
        public static List<PermisoUsuario> ObtenerListaPermisosPorUsuario(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<PermisoUsuario> items = new List<PermisoUsuario>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT distinct p.nombre, p.id_permiso, p.tipo_permiso, p.nombre_interno " +
                    " from permisos as p " +
                    " join permisos_usuario as pr on(p.id_permiso = pr.id_permiso)" +
                    " where pr.id_usuario = @idUsuario AND IsNull(p.activo, 0) = 1 ";


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("IdUsuario =  " + idUsuario);
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@idUsuario", idUsuario);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PermisoUsuario item = new PermisoUsuario();
                        item.IdPermiso = int.Parse(ds.Tables[0].Rows[i]["id_permiso"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreInterno = ds.Tables[0].Rows[i]["nombre_interno"].ToString();
                        item.TipoPermiso = ds.Tables[0].Rows[i]["tipo_permiso"].ToString();



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
        public static PermisoUsuario ObtenerPermisoPagina(string path, string idUsuario, string idPermiso)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            PermisoUsuario item = null;


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT distinct p.nombre, p.id_permiso, p.tipo_permiso " +
                    " from permisos as p " +
                    " join permisos_usuario as pr on(p.id_permiso = pr.id_permiso)" +
                    " where pr.id_usuario = @idUsuario AND  p.id_permiso = @idPermiso ";


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("IdUsuario =  " + idUsuario);
                Utils.Log("idPermiso =  " + idPermiso);
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@idUsuario", idUsuario);
                adp.SelectCommand.Parameters.AddWithValue("@idPermiso", idPermiso);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new PermisoUsuario();

                        item.IdPermiso = int.Parse(ds.Tables[0].Rows[i]["id_permiso"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.TipoPermiso = ds.Tables[0].Rows[i]["tipo_permiso"].ToString();



                    }
                }



            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

            }

            finally
            {
                conn.Close();
            }

            return item;

        }


        [WebMethod]
        public static List<PermisoUsuario> ObtenerListaPermisos(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<PermisoUsuario> items = new List<PermisoUsuario>();

            List<PermisoUsuario> listaPermisosUsuario = ObtenerListaPermisosPorUsuario(path, idUsuario);

            try
            {
                conn.Open();



                DataSet ds = new DataSet();
                string query = "SELECT id_permiso,  nombre, nombre_interno, tipo_permiso" +
                    "  FROM permisos " +
                    "  WHERE IsNull(activo, 0) = 1 ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                //string salida = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PermisoUsuario item = new PermisoUsuario();
                        item.IdPermiso = int.Parse(ds.Tables[0].Rows[i]["id_permiso"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreInterno = ds.Tables[0].Rows[i]["nombre_interno"].ToString();
                        item.TipoPermiso = ds.Tables[0].Rows[i]["tipo_permiso"].ToString();

                        items.Add(item);


                    }
                }


                //void cargarPermisos()
                //{
                //    try
                //    {
                //        int claveOperativo = (int)(comboOperativo.SelectedValue);
                //        listaPermisosAsignados = servicio.GetListaPermisosPorUsuario(claveOperativo);


                //        listaPermisosDisponibles = servicio.GetListaPermisos();

                HashSet<int> permisosIds = new HashSet<int>(listaPermisosUsuario.Select(x => x.IdPermiso));

                items.RemoveAll(x => permisosIds.Contains(x.IdPermiso));

                //        PoblarListas();
                //    }
                //    catch (Exception ex)
                //    {

                //    }
                //}


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

        //[WebMethod]
        //public static object GuardarEquiposUsuario(string path, List<EquipoUsuario> listaEquipos, string idUsuario)
        //{


        //    Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
        //    string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
        //    SqlConnection conn = new SqlConnection(strConexion);

        //    SqlTransaction transaccion = null;

        //    try
        //    {

        //        int r = 0;

        //        conn.Open();
        //        transaccion = conn.BeginTransaction();


        //        string sqlborrar = "";

        //        sqlborrar = @" DELETE equipos_usuario  WHERE id_usuario =  @id ";



        //        SqlCommand cmd = new SqlCommand(sqlborrar, conn);
        //        cmd.CommandType = CommandType.Text;
        //        cmd.Parameters.AddWithValue("@id", idUsuario);
        //        cmd.Transaction = transaccion;


        //        r += cmd.ExecuteNonQuery();
        //        Utils.Log("r = " + r);
        //        Utils.Log("Eliminado -> OK ");


        //        string sql = "";
        //        foreach (var item in listaEquipos)
        //        {
        //            sql = " INSERT INTO equipos_usuario (id_equipo, id_usuario) VALUES (@id_equipo, @id_usuario )";

        //            Utils.Log("\nMétodo-> " +
        //            System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

        //            SqlCommand cmd2 = new SqlCommand(sql, conn);
        //            cmd2.CommandType = CommandType.Text;
        //            cmd2.Parameters.AddWithValue("@id_equipo", item.IdEquipo);
        //            cmd2.Parameters.AddWithValue("@id_usuario", item.IdUsuario);
        //            cmd2.Transaction = transaccion;


        //            r += cmd2.ExecuteNonQuery();
        //            Utils.Log("Guardado -> OK ");
        //        }

        //        transaccion.Commit();

        //        return r;
        //    }
        //    catch (Exception ex)
        //    {

        //        try
        //        {
        //            transaccion.Rollback();
        //        }
        //        catch (Exception ex2)
        //        {
        //            Utils.Log("Error ... " + ex2.Message);
        //            Utils.Log(ex2.StackTrace);
        //        }

        //        Utils.Log("Error ... " + ex.Message);
        //        Utils.Log(ex.StackTrace);
        //        return -1;
        //    }

        //    finally
        //    {
        //        conn.Close();
        //    }


        //}

        [WebMethod]
        public static object GuardarPermisosUsuario(string path, List<PermisoUsuario> listaPermisos, string idUsuario)
        {


            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                int r = 0;
                conn.Open();


                string sqlborrar = "";

                sqlborrar = " DELETE permisos_usuario " +
                        " WHERE id_usuario= @id ";



                SqlCommand cmd = new SqlCommand(sqlborrar, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", idUsuario);


                r += cmd.ExecuteNonQuery();
                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");


                string sql = "";
                foreach (var item in listaPermisos)
                {
                    sql = " INSERT INTO permisos_usuario (id_permiso, id_usuario) VALUES (@id_permiso,@id_usuario )";



                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                    SqlCommand cmd2 = new SqlCommand(sql, conn);
                    cmd2.CommandType = CommandType.Text;

                    cmd2.Parameters.AddWithValue("@id_permiso", item.IdPermiso);



                    cmd2.Parameters.AddWithValue("@id_usuario", item.IdUsuario);


                    r += cmd2.ExecuteNonQuery();
                    Utils.Log("Guardado -> OK ");
                }


                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }

            finally
            {
                conn.Close();
            }


        }


        [WebMethod]
        public static List<PermisoUsuario> ObtenerTodosLosPermisos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<PermisoUsuario> items = new List<PermisoUsuario>();



            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_permiso,  nombre, nombre_interno, tipo_permiso" +
                    " FROM permisos " +
                    " WHERE IsNull(activo, 0) = 1 ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                //string salida = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        PermisoUsuario item = new PermisoUsuario();
                        item.IdPermiso = int.Parse(ds.Tables[0].Rows[i]["id_permiso"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NombreInterno = ds.Tables[0].Rows[i]["nombre_interno"].ToString();
                        item.TipoPermiso = ds.Tables[0].Rows[i]["tipo_permiso"].ToString();

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


        //[WebMethod]
        //public static List<Equipo> GetListaEquiposPorUsuario(string path, string idUsuario)
        //{

        //    string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

        //    SqlConnection conn = new SqlConnection(strConexion);
        //    List<Equipo> items = new List<Equipo>();

        //    try
        //    {
        //        conn.Open();
        //        DataSet ds = new DataSet();
        //        string query = @" 
        //                             SELECT e.id_equipo, e.nombre, e.numero_economico
        //                             FROM equipo e
        //                             JOIN equipos_usuario rel ON  (rel.id_equipo = e.id_equipo) 
        //                             WHERE rel.id_usuario = @id_usuario
        //                        ";

        //        Utils.Log("idtipoUsidUsuariouario =  " + idUsuario);
        //        SqlDataAdapter adp = new SqlDataAdapter(query, conn);
        //        adp.SelectCommand.Parameters.AddWithValue("@id_usuario", idUsuario);

        //        Utils.Log("\nMétodo-> " +
        //        System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



        //        adp.Fill(ds);


        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                Equipo item = new Equipo();

        //                item.IdEquipo = int.Parse(ds.Tables[0].Rows[i]["id_equipo"].ToString());
        //                item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
        //                item.NumeroEconomico = ds.Tables[0].Rows[i]["numero_economico"].ToString();

        //                items.Add(item);

        //            }
        //        }


        //        return items;
        //    }
        //    catch (Exception ex)
        //    {
        //        Utils.Log("Error ... " + ex.Message);
        //        Utils.Log(ex.StackTrace);
        //        return items;
        //    }

        //    finally
        //    {
        //        conn.Close();
        //    }


        //}


        //[WebMethod]
        //public static List<Equipo> GetListaEquipos(string path, string idUsuario)
        //{

        //    string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

        //    SqlConnection conn = new SqlConnection(strConexion);
        //    List<Equipo> items = new List<Equipo>();

        //    List<Equipo> listaEquiposUsuario = GetListaEquiposPorUsuario(path, idUsuario);


        //    try
        //    {
        //        conn.Open();
        //        DataSet ds = new DataSet();
        //        string query = @" 
        //                             SELECT e.id_equipo, e.nombre, e.numero_economico
        //                             FROM equipo e
        //                             WHERE isNull(e.eliminado, 0) =  0 
        //                        ";

        //        Utils.Log("idtipoUsidUsuariouario =  " + idUsuario);
        //        SqlDataAdapter adp = new SqlDataAdapter(query, conn);
        //        adp.SelectCommand.Parameters.AddWithValue("@id_usuario", idUsuario);

        //        Utils.Log("\nMétodo-> " +
        //        System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



        //        adp.Fill(ds);


        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                Equipo item = new Equipo();

        //                item.IdEquipo = int.Parse(ds.Tables[0].Rows[i]["id_equipo"].ToString());
        //                item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
        //                item.NumeroEconomico = ds.Tables[0].Rows[i]["numero_economico"].ToString();

        //                items.Add(item);

        //            }
        //        }

        //        //  quitar de la lista total de equipos los equipos que ya tiene asignados
        //        HashSet<int> equiposIds = new HashSet<int>(listaEquiposUsuario.Select(x => x.IdEquipo));
        //        items.RemoveAll(x => equiposIds.Contains(x.IdEquipo));



        //        return items;
        //    }
        //    catch (Exception ex)
        //    {
        //        Utils.Log("Error ... " + ex.Message);
        //        Utils.Log(ex.StackTrace);
        //        return items;
        //    }

        //    finally
        //    {
        //        conn.Close();
        //    }


        //}



    }
}