using Dapper;
using Newtonsoft.Json.Linq;
using Plataforma.Clases;
using Plataforma.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Services;
using System.Web.UI.WebControls;
using static iTextSharp.text.pdf.AcroFields;

namespace Plataforma.pages.Config
{
    public partial class NewEmployee : System.Web.UI.Page
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

        static Usuario GetUsuarioByLogin(string path, string login, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {
            Usuario item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 id_usuario, id_tipo_usuario,  nombre, login, password, email, telefono 
                                FROM usuario 
                                WHERE login = @login ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("login =  " + login);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@login", login);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    item = new Usuario();
                    item.Login = ds.Tables[0].Rows[0]["login"].ToString();
                }

                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return item;
            }
        }

        static void RegistrarUsuario(Usuario usuario, 
            SqlTransaction transaccion,
            SqlConnection conn)
        {

            //  Guardar usuario
            var sql = @"  INSERT INTO usuario
                            (id_empleado, login, password, id_tipo_usuario, eliminado)
                            VALUES
                            (@id_empleado, @login, @password, @id_tipo_usuario, 0);
                        ";


            SqlCommand cmdInsertUsuario = new SqlCommand(sql, conn);
            cmdInsertUsuario.CommandType = CommandType.Text;

            MD5 md5Hash = MD5.Create();
            string hash = Usuarios.GetMd5Hash(md5Hash, usuario.Password);

            cmdInsertUsuario.Parameters.AddWithValue("@id_empleado", usuario.IdEmpleado);
            cmdInsertUsuario.Parameters.AddWithValue("@id_tipo_usuario", usuario.IdTipoUsuario);
            cmdInsertUsuario.Parameters.AddWithValue("@login", usuario.Login);
            cmdInsertUsuario.Parameters.AddWithValue("@password", hash);
            cmdInsertUsuario.Transaction = transaccion;
            usuario .IdUsuario = cmdInsertUsuario.ExecuteNonQuery();
        }

        static void RegistrarEmpleado(Empleado oEmpleado, 
            SqlTransaction transaccion, 
            SqlConnection conn)
        {
            string sql = "";

            sql = @"  INSERT INTO empleado
                                OUTPUT INSERTED.id_empleado 
                                VALUES (@id_tipo_usuario, NULL, @id_posicion, @id_plaza, @curp, @nombre, @primer_apellido,
                                    @segundo_apellido, @telefono,0, NULL,1,@id_supervisor,@id_ejecutivo, @fecha_ingreso,
                                    NULL,NULL, NULL, NULL, NULL, NULL,NULL,
                                    @limite_venta_ejercicio,@limite_incremento_ejercicio,@fizcalizable,@ocupacion,@nota_foto)";


            Utils.Log("insert employee" + sql);

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@id_tipo_usuario", oEmpleado.IdPosicion);
            cmd.Parameters.AddWithValue("@id_posicion", oEmpleado.IdPosicion);    
            cmd.Parameters.AddWithValue("@id_plaza", oEmpleado.IdPlaza);
            cmd.Parameters.AddWithValue("@id_supervisor", oEmpleado.IdSupervisor);
            cmd.Parameters.AddWithValue("@id_ejecutivo",  oEmpleado.IdEjecutivo);
            cmd.Parameters.AddWithValue("@curp", oEmpleado.CURP);
            cmd.Parameters.AddWithValue("@nombre", oEmpleado.Nombre);
            cmd.Parameters.AddWithValue("@primer_apellido", oEmpleado.PrimerApellido);
            cmd.Parameters.AddWithValue("@segundo_apellido", oEmpleado.SegundoApellido);
            cmd.Parameters.AddWithValue("@limite_venta_ejercicio", oEmpleado.limite_incremento_ejercicio);
            cmd.Parameters.AddWithValue("@limite_incremento_ejercicio", oEmpleado.limite_incremento_ejercicio);
            cmd.Parameters.AddWithValue("@fizcalizable", oEmpleado.fizcalizable);
            cmd.Parameters.AddWithValue("@ocupacion", oEmpleado.Ocupacion);
            cmd.Parameters.AddWithValue("@nota_foto", oEmpleado.NotaFoto);
            cmd.Parameters.AddWithValue("@telefono", oEmpleado.Telefono);
            cmd.Parameters.AddWithValue("@fecha_ingreso", oEmpleado.FechaIngreso);
            cmd.Parameters.AddWithValue("@id_empleado", oEmpleado.IdEmpleado);
            
            cmd.Transaction = transaccion;
            oEmpleado.IdEmpleado= (int)cmd.ExecuteScalar();

            RegistrarDireccion(oEmpleado.Direccion, transaccion,conn);
        }

        static void RegistraClienteAval(Cliente oCliente, SqlTransaction transaccion, SqlConnection conn)
        {
            string sql = "";

            if (oCliente.IdCliente > 0)
            {
                sql = @"UPDATE cliente
                                SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                                segundo_apellido = @segundo_apellido,
                                ocupacion = @ocupacion, telefono = @telefono 
                          WHERE
                                id_cliente = @id_cliente ";
                Utils.Log("ACTUALIZAR CLIENTE " + sql);
            }
            else
            {
                sql = @" INSERT INTO cliente 
                            OUTPUT INSERTED.id_cliente
                    VALUES (@curp, @nombre, @primer_apellido, @segundo_apellido, @ocupacion, @telefono,NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,0,2,NULL,NULL, NULL) ";
                Utils.Log("INSERTAR CLIENTE " + sql);
            }

            var cmd = new SqlCommand(sql, conn);
            cmd.Transaction = transaccion;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@curp", oCliente.Curp);
            cmd.Parameters.AddWithValue("@nombre", oCliente.Nombre);
            cmd.Parameters.AddWithValue("@primer_apellido", oCliente.PrimerApellido);
            cmd.Parameters.AddWithValue("@segundo_apellido", oCliente.SegundoApellido);
            cmd.Parameters.AddWithValue("@ocupacion", oCliente.Ocupacion);
            cmd.Parameters.AddWithValue("@telefono", oCliente.Telefono);

            if (oCliente.IdCliente > 0)
            {
                cmd.Parameters.AddWithValue("@id_cliente",
                    oCliente.IdCliente);
                var rows = cmd.ExecuteNonQuery();
            }
            else
            {
                oCliente.IdCliente = cmd.ExecuteScalar().ToString().ParseStringToInt();
            }

            oCliente.direccion.IdCliente = oCliente.IdCliente;


            RegistrarDireccion(oCliente.direccion, transaccion, conn);
        }

        static void RegistrarDireccion(Direccion oDireccion, SqlTransaction transaccion, SqlConnection conn)
        {
            string sql = $"SELECT COUNT(*) FROM direccion WHERE id_cliente = {oDireccion.IdCliente}";

            var iExisteDireccion = conn.ExecuteScalar<int>(sql, transaction: transaccion);

            if (iExisteDireccion > 0)
            {
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, 
                                    colonia = @colonia, 
                                    municipio = @municipio, 
                                    estado = @estado,
                                   codigo_postal = @codigo_postal, 
                                   direccion_trabajo = @direccion_trabajo, 
                                   ubicacion = @ubicacion
                            WHERE id_empleado = @id_empleado
                        ";

                Utils.Log("ACTUALIZAR DIRECCIÓN " + sql);
            }
            else
            {
                sql = @" INSERT INTO direccion 
                      OUTPUT INSERTED.id_direccion
                    VALUES (@id_empleado, NULL,@calleyno, @colonia, @municipio, @estado,NULL, @codigo_postal,NULL,NULL,1,NULL, @direccion_trabajo,NULL,  @ubicacion);";
                Utils.Log("INSERTAR id_direccion " + sql);
            }

            var cmd = new SqlCommand(sql, conn);
            cmd.Transaction = transaccion;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@calleyno", oDireccion.Calle);
            cmd.Parameters.AddWithValue("@colonia", oDireccion.Colonia);
            cmd.Parameters.AddWithValue("@municipio", oDireccion.Municipio);
            cmd.Parameters.AddWithValue("@estado", oDireccion.Estado);
            cmd.Parameters.AddWithValue("@codigo_postal", oDireccion.CodigoPostal);
            cmd.Parameters.AddWithValue("@direccion_trabajo", oDireccion.DireccionTrabajo);
            cmd.Parameters.AddWithValue("@ubicacion", oDireccion.Ubicacion);
            cmd.Parameters.AddWithValue("@id_empleado", oDireccion.IdEmpleado);

            if (iExisteDireccion > 0)
            {
                var rows = cmd.ExecuteNonQuery();
            }
            else
            {
                oDireccion.idDireccion = cmd.ExecuteScalar().ToString().ParseStringToInt();
            }
        }

        [WebMethod]
        public static DatosSalida Save(string path, 
            string idUsuario,
            EmpleadoRequest oRequest)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;

            int r = 0;
            try
            {
                conn.Open();
                transaccion = conn.BeginTransaction();
                //  Validar que no exista un nombre de usuario-login 
                Usuario usuarioExists = GetUsuarioByLogin(path, oRequest.User.Login, conn, strConexion, transaccion);
                if (usuarioExists != null)
                {
                    salida.MensajeError = "Ya existe un usuario con el Nombre de usuario/Login " + oRequest.User.Login + " por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

                RegistrarEmpleado(oRequest.Colaborador, transaccion, conn);
                RegistrarUsuario(oRequest.User, transaccion, conn);
                RegistraClienteAval(oRequest.Aval, transaccion, conn);

                transaccion.Commit();
                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = oRequest.Colaborador.IdEmpleado.ToString();
            }
            catch (Exception ex)
            {
                try
                {
                    transaccion.Rollback();
                }
                catch (Exception exep)
                {
                    Utils.Log("Error ... " + exep.Message);
                    Utils.Log(exep.StackTrace);
                }

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;
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
        public static List<Empleado> GetListaItemsEmpleadoByPosicion(string path, string idTipoEmpleado)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_empleado,
                                    concat( nombre ,  ' ' , primer_apellido , ' ' ,  segundo_apellido) AS nombre_completo
                                    FROM empleado WHERE ISNull(activo, 1) = 1  
                                    AND ISNull(eliminado, 0) = 0
                                    AND id_posicion = @id_posicion ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_posicion", idTipoEmpleado);


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Empleado item = new Empleado();
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre_completo"].ToString();

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