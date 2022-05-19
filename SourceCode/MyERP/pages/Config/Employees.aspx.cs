using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class Employees : System.Web.UI.Page
    {
        const string pagina = "8";


        const int POSICION_DIRECTOR = 1;
        const int POSICION_COORDINADOR = 2;
        const int POSICION_EJECUTIVO = 3;
        const int POSICION_SUPERVISOR = 4;
        const int POSICION_PROMOTOR = 5;


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
            if (usuario == "")
            {
                Response.Redirect("Login.aspx");
            }


        }





        [WebMethod]
        public static List<Empleado> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado , e.nombre, e.primer_apellido, e.segundo_apellido, 
                     concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) AS nombre_completo,
                     ISNull(e.activo, 1) activo, FORMAT(e.fecha_ingreso, 'dd/MM/yyyy') fecha_ingreso,
                     u.login ,
                     m.nombre nombre_modulo,  
                     t.nombre nombre_tipo_usuario,
                     p.nombre nombre_plaza
                     FROM empleado e 
                     JOIN usuario u ON (u.id_empleado = e.id_empleado) 
                     JOIN modulo m ON (m.id_modulo = e.id_comision_inicial) 
                     JOIN plaza p ON (p.id_plaza = e.id_plaza) 
                     JOIN tipo_usuario t ON (t.id_tipo_usuario = e.id_tipo_usuario)
                     WHERE isnull(e.eliminado, 0) != 1 
                    ";

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
                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        item.Login = ds.Tables[0].Rows[i]["login"].ToString();
                        item.Nombre = ds.Tables[0].Rows[i]["nombre_modulo"].ToString();
                        item.NombreTipoUsuario = ds.Tables[0].Rows[i]["nombre_tipo_usuario"].ToString();
                        item.NombreModulo= ds.Tables[0].Rows[i]["nombre_modulo"].ToString();
                        item.NombrePlaza = ds.Tables[0].Rows[i]["nombre_plaza"].ToString();
                        item.FechaIngreso = ds.Tables[0].Rows[i]["fecha_ingreso"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

                        item.ActivoStr = (item.Activo == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='empleado.editar(" + item.IdEmpleado + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='empleado.eliminar(" + item.IdEmpleado + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove'></span>Eliminar</button>";

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
        public static DatosSalida Save(string path, Empleado item, Direccion itemAddress, Direccion itemAddressAval, 
                    Usuario itemUser, string accion, string idUsuario)
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


                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name  + "\n");


                string sql = "";

                sql = @"  INSERT INTO empleado
                                (id_tipo_usuario, id_comision_inicial, id_posicion, id_plaza, curp, nombre, primer_apellido,
                                    segundo_apellido, telefono, fecha_nacimiento, fecha_ingreso,
                                    id_supervisor, id_ejecutivo, 
                                    curp_aval, nombre_aval, primer_apellido_aval, segundo_apellido_aval,
                                    eliminado, activo)
                             
                                OUTPUT INSERTED.id_empleado
                                
                                VALUES (@id_tipo_usuario, @id_comision_inicial, @id_posicion, @id_plaza, @curp, @nombre, @primer_apellido,
                                    @segundo_apellido, @telefono, @fecha_nacimiento, @fecha_ingreso,
                                    @id_supervisor, @id_ejecutivo,
                                    @curp_aval, @nombre_aval, @primer_apellido_aval, @segundo_apellido_aval,
                                    0, 1)";


                Utils.Log("insert employee" + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_tipo_usuario", item.IdPosicion);   // resolver si tipo empleado sera lo mismo que tipo usuario
                cmd.Parameters.AddWithValue("@id_comision_inicial", item.IdComisionInicial);
                cmd.Parameters.AddWithValue("@id_posicion", item.IdPosicion);       //  puesto
                cmd.Parameters.AddWithValue("@id_plaza", item.IdPlaza);
                

                cmd.Parameters.AddWithValue("@id_supervisor", item.IdPosicion == POSICION_PROMOTOR ?  item.IdSupervisor : 0);
                cmd.Parameters.AddWithValue("@id_ejecutivo", item.IdPosicion == POSICION_SUPERVISOR ?  item.IdEjecutivo : 0);

                cmd.Parameters.AddWithValue("@curp", item.CURP);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@primer_apellido", item.PrimerApellido);
                cmd.Parameters.AddWithValue("@segundo_apellido", item.SegundoApellido);

                cmd.Parameters.AddWithValue("@curp_aval", item.CURPAval);
                cmd.Parameters.AddWithValue("@nombre_aval", item.NombreAval);
                cmd.Parameters.AddWithValue("@primer_apellido_aval", item.PrimerApellidoAval);
                cmd.Parameters.AddWithValue("@segundo_apellido_aval", item.SegundoApellidoAval);

                cmd.Parameters.AddWithValue("@telefono", item.Telefono);
                cmd.Parameters.AddWithValue("@fecha_nacimiento", item.FechaNacimiento);
                cmd.Parameters.AddWithValue("@fecha_ingreso", item.FechaIngreso);
                cmd.Parameters.AddWithValue("@id_empleado", item.IdEmpleado);
                cmd.Transaction = transaccion;

                int idGenerado = (int)cmd.ExecuteScalar();

                //  Guardar direccion empleado
                sql = @"  INSERT INTO direccion
                            (id_empleado, calleyno, colonia, municipio, estado,
                                codigo_postal, activo, aval)
                            VALUES
                                (@id_empleado, @calleyno, @colonia, @municipio, @estado,
                                @codigo_postal, 1, 0);
                        ";


                Utils.Log("insert employee" + sql);

                SqlCommand cmdAddressEmployee = new SqlCommand(sql, conn);
                cmdAddressEmployee.CommandType = CommandType.Text;

                cmdAddressEmployee.Parameters.AddWithValue("@id_empleado", idGenerado);   
                cmdAddressEmployee.Parameters.AddWithValue("@calleyno", itemAddress.Calle);
                cmdAddressEmployee.Parameters.AddWithValue("@colonia", itemAddress.Colonia);       
                cmdAddressEmployee.Parameters.AddWithValue("@municipio", itemAddress.Municipio);
                cmdAddressEmployee.Parameters.AddWithValue("@estado", itemAddress.Estado);
                cmdAddressEmployee.Parameters.AddWithValue("@codigo_postal", itemAddress.CP);
                cmdAddressEmployee.Transaction = transaccion;

                r = cmdAddressEmployee.ExecuteNonQuery();


                //  Guardar direccion aval
                sql = @"  INSERT INTO direccion
                            (id_empleado, calleyno, colonia, municipio, estado,
                                codigo_postal, activo, aval)
                            VALUES
                                (@id_empleado, @calleyno, @colonia, @municipio, @estado,
                                @codigo_postal, 1, 1);
                        ";


                Utils.Log("insert employee aval" + sql);

                SqlCommand cmdAddressEmployeeAval = new SqlCommand(sql, conn);
                cmdAddressEmployeeAval.CommandType = CommandType.Text;

                cmdAddressEmployeeAval.Parameters.AddWithValue("@id_empleado", idGenerado);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@calleyno", itemAddressAval.Calle);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@colonia", itemAddressAval.Colonia);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@municipio", itemAddressAval.Municipio);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@estado", itemAddressAval.Estado);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@codigo_postal", itemAddressAval.CP);
                cmdAddressEmployeeAval.Transaction = transaccion;

                r += cmdAddressEmployeeAval.ExecuteNonQuery();


                //  Guardar usuario
                sql = @"  INSERT INTO usuario
                            (id_empleado, login, password, id_tipo_usuario, eliminado)
                            VALUES
                            (@id_empleado, @login, @password, @id_tipo_usuario, 1);
                        ";


                Utils.Log("insert usuario " + sql);

                SqlCommand cmdInsertUsuario = new SqlCommand(sql, conn);
                cmdInsertUsuario.CommandType = CommandType.Text;

                MD5 md5Hash = MD5.Create();
                string hash = Usuarios.GetMd5Hash(md5Hash, itemUser.Password);

                cmdInsertUsuario.Parameters.AddWithValue("@id_empleado", idGenerado);
                cmdInsertUsuario.Parameters.AddWithValue("@id_tipo_usuario", itemUser.IdTipoUsuario);
                cmdInsertUsuario.Parameters.AddWithValue("@login", itemUser.Login);
                cmdInsertUsuario.Parameters.AddWithValue("@password", hash);
                cmdInsertUsuario.Transaction = transaccion;

                r += cmdInsertUsuario.ExecuteNonQuery();


                Utils.Log("Guardado -> OK ");


                transaccion.Commit();


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = r.ToString();

            }
            catch (Exception ex)
            {
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
        public static string GetFoto(string path, string idEmpleado)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            string item = "";
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT fotografia_b64 FROM empleado WHERE id_empleado = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + idEmpleado);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idEmpleado);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    item = ds.Tables[0].Rows[0]["fotografia_b64"].ToString();
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
        public static string GetFotoByIdUsuario(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            string item = "";
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.fotografia_b64 
                            FROM empleado e
                            LEFT JOIN usuario u ON e.id_empleado = u.id_empleado
                            WHERE u.id_usuario = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + idUsuario);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idUsuario);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    item = ds.Tables[0].Rows[0]["fotografia_b64"].ToString();
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
        public static DatosSalida Eliminar(string path, string id)
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

                sql = " UPDATE empleado set eliminado = 1" +
                        " WHERE id_empleado = @id_empleado ";



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
        public static List<Posicion> GetListaItemsPosiciones(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Posicion> items = new List<Posicion>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_posicion, nombre FROM  posicion WHERE ISNull(activo, 1) = 1  ";

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
                                    FROM empleado WHERE ISNull(activo, 1) = 1  AND id_posicion = @id_posicion ";

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
                        item.IdPosicion = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
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
                string query = @" SELECT id_plaza, nombre FROM  plaza WHERE ISNull(activo, 1) = 1  ";

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
        public static List<Modulo> GetListaItemsModulos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Modulo> items = new List<Modulo>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_modulo, nombre FROM  modulo WHERE ISNull(activo, 1) = 1  ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Modulo item = new Modulo();
                        item.IdModulo = int.Parse(ds.Tables[0].Rows[i]["id_modulo"].ToString());
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
        public static Empleado GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Empleado item = new Empleado();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_empleado , nombre, segundo_apellido," +
                    " primer_apellido,  clave, ISNull(activo, 1) activo, id_departamento, id_puesto " +
                    "  FROM  empleado where id_empleado =  @id_empleado ";

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
                        item.IdPosicion = int.Parse(ds.Tables[0].Rows[i]["id_puesto"].ToString());
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();

                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        //item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();



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








    }



}