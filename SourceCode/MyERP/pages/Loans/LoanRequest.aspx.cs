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
        public static List<Cliente> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            SqlConnection conn = new SqlConnection(strConexion);
            List<Cliente> items = new List<Cliente>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente , c.nombre, c.primer_apellido, c.segundo_apellido, 
                     concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                     c.telefono , c.curp, c.ocupacion, c.activo, tc.id_tipo_cliente, tc.tipo_cliente, p.id_prestamo, p.monto,
                     FORMAT(p.fecha_solicitud, 'dd/MM/yyyy') fecha_solicitud
                     FROM cliente c 
                     JOIN tipo_cliente tc ON (tc.id_tipo_cliente = c.id_tipo_cliente) 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     WHERE isnull(c.eliminado, 0) != 1 
                     ORDER BY id_cliente ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Cliente item = new Cliente();
                        //..
                        //
                        item.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.PrimerApellido = ds.Tables[0].Rows[i]["primer_apellido"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();
                        item.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.Ocupacion = ds.Tables[0].Rows[i]["ocupacion"].ToString();
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.TipoCliente = (ds.Tables[0].Rows[i]["tipo_cliente"].ToString());

                        item.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();


                        item.SegundoApellido = ds.Tables[0].Rows[i]["segundo_apellido"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());


                        string botones = "<button  onclick='client.edit(" + item.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='client.delete(" + item.IdCliente + ")'   class='btn btn-outline-primary'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";


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
        public static DatosSalida Save(string path, Cliente item, Direccion itemAddress, Direccion itemAddressAval,
                     string accion, string idUsuario)
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


                //  Validar que no exista un cliente con la misma CURP
                Cliente customerExists = GetClienteByCURP(path, item.Curp, conn, strConexion, transaccion);
                if (customerExists != null)
                {
                    salida.MensajeError = "Ya existe el cliente con CURP " + item.Curp + " por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }


                //  Validar que el nuevo cliente no sea el mismo que el aval mediante su curp
                if (item.Curp == item.CurpAval)
                {
                    salida.MensajeError = "La CURP del cliente y del aval no debe ser la misma.";
                    salida.CodigoError = 1;
                    return salida;

                }


                //  Validar que el nuevo cliente no sea AVAL de otro préstamo en tabla de clientes
                Cliente customerAval = GetClienteByCURPAvalCliente(path, item.Curp, conn, strConexion, transaccion);
                if (customerAval != null)
                {
                    salida.MensajeError = "El cliente se encuentra como aval del otro préstamo, por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }

                //TODO: Asegurar que la siguiente validacion sea correcta o se quita
                ////  Validar que el nuevo cliente no sea AVAL de otro préstamo en tabla de empleados
                //Empleado employeeAval = GetClienteByCURPAvalEmpleado(path, item.Curp, conn, strConexion, transaccion);
                //if (employeeAval != null)
                //{
                //    salida.MensajeError = "El cliente se encuentra como aval del otro préstamo,  por favor verifique e intente de nuevo.";
                //    salida.CodigoError = 1;
                //    return salida;
                //}


                //  Validar que el nuevo aval no sea AVAL 3 o mas veces en la tabla de clientes
                int customerAval3Times = GetClienteByCURPAvalCliente3Veces(path, item.CurpAval, conn, strConexion, transaccion);
                if (customerAval3Times > 2)
                {
                    salida.MensajeError = "El aval ya existe " + customerAval3Times + " veces como aval del otros préstamos. Lo máximo permitido son 2,  por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }

                //  Validar que no tenga un prestamo en status Pendiente ... != aprobado y != rechazado
                Prestamo prestamoExists = GetPrestamoByCURP(path, item.Curp, conn, strConexion, transaccion);
                if (prestamoExists != null)
                {
                    salida.MensajeError = "El cliente con curp " + item.Curp + " ya cuenta con un préstamo en proceso de aprobación, por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }



                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


                string sql = "";

                sql = @"  INSERT INTO cliente
                                (curp, nombre, primer_apellido, segundo_apellido, ocupacion, telefono, id_tipo_cliente, 
                                    curp_aval, nombre_aval, primer_apellido_aval, segundo_apellido_aval, ocupacion_aval, telefono_aval, 
                                    activo, eliminado)
                             
                                OUTPUT INSERTED.id_cliente
                                
                                VALUES (@curp, @nombre, @primer_apellido,
                                    @segundo_apellido, @ocupacion, @telefono, @id_tipo_cliente,
                                    @curp_aval, @nombre_aval, @primer_apellido_aval, @segundo_apellido_aval, @ocupacion_aval, @telefono_aval,
                                    1, 0)";


                Utils.Log("insert client" + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_tipo_cliente", item.IdTipoCliente);


                cmd.Parameters.AddWithValue("@curp", item.Curp);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@primer_apellido", item.PrimerApellido);
                cmd.Parameters.AddWithValue("@segundo_apellido", item.SegundoApellido);
                cmd.Parameters.AddWithValue("@ocupacion", item.Ocupacion);

                cmd.Parameters.AddWithValue("@telefono", item.Telefono);


                cmd.Parameters.AddWithValue("@curp_aval", item.CurpAval);
                cmd.Parameters.AddWithValue("@nombre_aval", item.NombreAval);
                cmd.Parameters.AddWithValue("@primer_apellido_aval", item.PrimerApellidoAval);
                cmd.Parameters.AddWithValue("@segundo_apellido_aval", item.SegundoApellidoAval);
                cmd.Parameters.AddWithValue("@telefono_aval", item.TelefonoAval);
                cmd.Parameters.AddWithValue("@ocupacion_aval", item.OcupacionAval);
                cmd.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmd.Transaction = transaccion;

                int idGenerado = (int)cmd.ExecuteScalar();

                //  Guardar direccion cliente
                sql = @"  INSERT INTO direccion
                            (calleyno, colonia, municipio, estado,
                                codigo_postal, activo, aval, direccion_trabajo, id_cliente)
                            VALUES
                                (@calleyno, @colonia, @municipio, @estado,
                                @codigo_postal, 1, 0, @direccion_trabajo, @id_cliente);
                        ";


                Utils.Log("insert direccion client" + sql);

                SqlCommand cmdAddressEmployee = new SqlCommand(sql, conn);
                cmdAddressEmployee.CommandType = CommandType.Text;

                cmdAddressEmployee.Parameters.AddWithValue("@id_cliente", idGenerado);
                cmdAddressEmployee.Parameters.AddWithValue("@calleyno", itemAddress.Calle);
                cmdAddressEmployee.Parameters.AddWithValue("@colonia", itemAddress.Colonia);
                cmdAddressEmployee.Parameters.AddWithValue("@municipio", itemAddress.Municipio);
                cmdAddressEmployee.Parameters.AddWithValue("@estado", itemAddress.Estado);
                cmdAddressEmployee.Parameters.AddWithValue("@codigo_postal", itemAddress.CodigoPostal);
                cmdAddressEmployee.Parameters.AddWithValue("@direccion_trabajo", itemAddress.DireccionTrabajo);

                cmdAddressEmployee.Transaction = transaccion;

                r = cmdAddressEmployee.ExecuteNonQuery();


                //  Guardar direccion aval
                sql = @"  INSERT INTO direccion
                            (calleyno, colonia, municipio, estado,
                                codigo_postal, activo, aval, direccion_trabajo, id_cliente)
                            VALUES
                                (@calleyno, @colonia, @municipio, @estado,
                                @codigo_postal, 1, 1, @direccion_trabajo, @id_cliente);
                        ";


                Utils.Log("insert direccion aval" + sql);

                SqlCommand cmdAddressEmployeeAval = new SqlCommand(sql, conn);
                cmdAddressEmployeeAval.CommandType = CommandType.Text;

                cmdAddressEmployeeAval.Parameters.AddWithValue("@id_cliente", idGenerado);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@calleyno", itemAddressAval.Calle);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@colonia", itemAddressAval.Colonia);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@municipio", itemAddressAval.Municipio);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@estado", itemAddressAval.Estado);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@codigo_postal", itemAddressAval.CodigoPostal);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@direccion_trabajo", itemAddressAval.DireccionTrabajo);
                cmdAddressEmployeeAval.Transaction = transaccion;


                r += cmdAddressEmployeeAval.ExecuteNonQuery();

                //  Guardar prestamo
                sql = @"  INSERT INTO prestamo
                            (fecha_solicitud, monto, id_cliente, id_usuario, id_status_prestamo)
                            VALUES
                            (@fecha_solicitud, @monto, @id_cliente, @id_usuario, @id_status_prestamo);
                        ";

                // un nuevo prestamo nace con status 1 = PENDIENTE

                Utils.Log("insert prestamo " + sql);

                SqlCommand cmdInsertPrestamo = new SqlCommand(sql, conn);
                cmdInsertPrestamo.CommandType = CommandType.Text;



                cmdInsertPrestamo.Parameters.AddWithValue("@id_cliente", idGenerado);
                cmdInsertPrestamo.Parameters.AddWithValue("@fecha_solicitud", item.FechaSolicitud);
                cmdInsertPrestamo.Parameters.AddWithValue("@monto", item.Monto);
                cmdInsertPrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdInsertPrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PENDIENTE);

                cmdInsertPrestamo.Transaction = transaccion;

                r += cmdInsertPrestamo.ExecuteNonQuery();


                Utils.Log("Guardado -> OK ");


                transaccion.Commit();


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idGenerado.ToString();

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
        public static DatosSalida SaveLoanUpdateCustomer(string path, Cliente item, Direccion itemAddress, Direccion itemAddressAval, string accion, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            //verificar que tenga permisos para usar esta pagina
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

                //  Validar que el nuevo cliente no sea AVAL de otro préstamo en tabla de clientes
                Cliente customerAval = GetClienteByCURPAvalCliente(path, item.Curp, conn, strConexion, transaccion);
                if (customerAval != null)
                {
                    salida.MensajeError = "El cliente se encuentra como aval del otro préstamo, por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }

                //  Validar que el nuevo cliente no sea el mismo que el aval mediante su curp
                if (item.Curp == item.CurpAval)
                {
                    salida.MensajeError = "La CURP del cliente y del aval no debe ser la misma.";
                    salida.CodigoError = 1;
                    return salida;

                }


                //  Validar que el nuevo aval no sea AVAL 3 o mas veces en la tabla de clientes
                int customerAval3Times = GetClienteByCURPAvalCliente3Veces(path, item.CurpAval, conn, strConexion, transaccion);
                if (customerAval3Times > 2)
                {
                    salida.MensajeError = "El aval ya existe " + customerAval3Times + " veces como aval del otros préstamos. Lo máximo permitido son 2,  por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }


                //  Validar que no tenga un prestamo en status Pendiente ... != aprobado y != rechazado
                Prestamo prestamoExists = GetPrestamoByCURP(path, item.Curp, conn, strConexion, transaccion);
                if (prestamoExists != null)
                {
                    salida.MensajeError = "El cliente con curp " + item.Curp + " ya cuenta con un préstamo en proceso de aprobación, por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }


                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


                string sql = "";

                sql = @"  UPDATE cliente
                                SET curp = @curp, nombre = @nombre, primer_apellido = @primer_apellido,
                                segundo_apellido = @segundo_apellido, 
                                ocupacion = @ocupacion, telefono = @telefono, id_tipo_cliente = @id_tipo_cliente, 
                                curp_aval = @curp_aval, nombre_aval = @nombre_aval, primer_apellido_aval = @primer_apellido_aval, 
                                segundo_apellido_aval = @segundo_apellido_aval, ocupacion_aval = @ocupacion_aval, 
                                telefono_aval = @telefono_aval 
                                WHERE
                                id_cliente = @id_cliente ";


                Utils.Log("ACTUALIZAR CLIENTE " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_tipo_cliente", item.IdTipoCliente);
                cmd.Parameters.AddWithValue("@curp", item.Curp);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@primer_apellido", item.PrimerApellido);
                cmd.Parameters.AddWithValue("@segundo_apellido", item.SegundoApellido);
                cmd.Parameters.AddWithValue("@ocupacion", item.Ocupacion);
                cmd.Parameters.AddWithValue("@telefono", item.Telefono);
                cmd.Parameters.AddWithValue("@curp_aval", item.CurpAval);
                cmd.Parameters.AddWithValue("@nombre_aval", item.NombreAval);
                cmd.Parameters.AddWithValue("@primer_apellido_aval", item.PrimerApellidoAval);
                cmd.Parameters.AddWithValue("@segundo_apellido_aval", item.SegundoApellidoAval);
                cmd.Parameters.AddWithValue("@telefono_aval", item.TelefonoAval);
                cmd.Parameters.AddWithValue("@ocupacion_aval", item.OcupacionAval);
                cmd.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();

                //  Guardar direccion cliente
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                                codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo
                            WHERE id_cliente = @id_cliente AND ISNULL(aval, 0) = 0
                        ";


                Utils.Log("ACTUALIZAR DIRECCION CLIENTE " + sql);

                SqlCommand cmdAddressEmployee = new SqlCommand(sql, conn);
                cmdAddressEmployee.CommandType = CommandType.Text;

                cmdAddressEmployee.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmdAddressEmployee.Parameters.AddWithValue("@calleyno", itemAddress.Calle);
                cmdAddressEmployee.Parameters.AddWithValue("@colonia", itemAddress.Colonia);
                cmdAddressEmployee.Parameters.AddWithValue("@municipio", itemAddress.Municipio);
                cmdAddressEmployee.Parameters.AddWithValue("@estado", itemAddress.Estado);
                cmdAddressEmployee.Parameters.AddWithValue("@codigo_postal", itemAddress.CodigoPostal);
                cmdAddressEmployee.Parameters.AddWithValue("@direccion_trabajo", itemAddress.DireccionTrabajo);
                cmdAddressEmployee.Transaction = transaccion;

                r = cmdAddressEmployee.ExecuteNonQuery();


                //  Guardar direccion aval
                sql = @"  UPDATE direccion
                             SET calleyno = @calleyno, colonia = @colonia, municipio = @municipio, estado = @estado,
                                codigo_postal = @codigo_postal, direccion_trabajo = @direccion_trabajo
                            WHERE id_cliente = @id_cliente AND ISNULL(aval, 0) = 1
                        ";



                Utils.Log("update customer aval" + sql);

                SqlCommand cmdAddressEmployeeAval = new SqlCommand(sql, conn);
                cmdAddressEmployeeAval.CommandType = CommandType.Text;

                cmdAddressEmployeeAval.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@calleyno", itemAddressAval.Calle);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@colonia", itemAddressAval.Colonia);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@municipio", itemAddressAval.Municipio);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@estado", itemAddressAval.Estado);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@codigo_postal", itemAddressAval.CodigoPostal);
                cmdAddressEmployeeAval.Parameters.AddWithValue("@direccion_trabajo", itemAddressAval.DireccionTrabajo);
                cmdAddressEmployeeAval.Transaction = transaccion;

                r += cmdAddressEmployeeAval.ExecuteNonQuery();


                //  Guardar prestamo
                sql = @"  INSERT INTO prestamo
                            (fecha_solicitud, monto, id_cliente, id_usuario, id_status_prestamo)
                            VALUES
                            (@fecha_solicitud, @monto, @id_cliente, @id_usuario, @id_status_prestamo);
                        ";

                // un nuevo prestamo nace con status 1 = PENDIENTE

                Utils.Log("GUARDAR NUEVO PRESTAMO " + sql);

                SqlCommand cmdInsertPrestamo = new SqlCommand(sql, conn);
                cmdInsertPrestamo.CommandType = CommandType.Text;



                cmdInsertPrestamo.Parameters.AddWithValue("@id_cliente", item.IdCliente);
                cmdInsertPrestamo.Parameters.AddWithValue("@fecha_solicitud", item.FechaSolicitud);
                cmdInsertPrestamo.Parameters.AddWithValue("@monto", item.Monto);
                cmdInsertPrestamo.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdInsertPrestamo.Parameters.AddWithValue("@id_status_prestamo", Prestamo.STATUS_PENDIENTE);
                cmdInsertPrestamo.Transaction = transaccion;

                r += cmdInsertPrestamo.ExecuteNonQuery();


                Utils.Log("Guardado -> OK ");


                transaccion.Commit();


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = item.IdCliente.ToString();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



        public static Cliente GetClienteByCURP(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Cliente item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 id_cliente, curp
                                FROM cliente 
                                WHERE curp = @curp";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Cliente();
                    item.IdCliente = int.Parse(ds.Tables[0].Rows[0]["id_cliente"].ToString());

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


        //  Buscar una persona Aval, mediante su curp en tabla de cliente
        public static Cliente GetClienteByCURPAvalCliente(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Cliente item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 id_cliente, curp
                                FROM cliente 
                                WHERE curp_aval = @curp_aval";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP Aval =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp_aval", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Cliente();
                    item.IdCliente = int.Parse(ds.Tables[0].Rows[0]["id_cliente"].ToString());

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


        //  Buscar una persona Aval, mediante su curp en tabla de empleado
        public static Empleado GetClienteByCURPAvalEmpleado(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Empleado item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 id_empleado, curp
                                FROM empleado 
                                WHERE curp_aval = @curp_aval";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP Aval =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp_aval", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Empleado();
                    item.IdEmpleado = int.Parse(ds.Tables[0].Rows[0]["id_empleado"].ToString());

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


        //  Buscar una persona Aval, cuantas veces esta como aval una curp en la tabla de cliente
        public static int GetClienteByCURPAvalCliente3Veces(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            int num = 0;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT id_cliente
                                FROM cliente 
                                WHERE curp_aval = @curp_aval";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                Utils.Log("CURP Aval =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp_aval", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);

                num = ds.Tables[0].Rows.Count + 1;

                return num;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }



        }



        //  Buscar prestamo por curp del cliente, PRESTAMO con status pendiente 2 y 3 actualmente
        public static Prestamo GetPrestamoByCURP(string path, string CURP, SqlConnection conn, string strConexion, SqlTransaction transaction)
        {

            Prestamo item = null;

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT TOP 1 p.id_prestamo, c.curp
                                FROM prestamo p JOIN cliente c ON (c.id_cliente = p.id_cliente)
                                WHERE c.curp = @curp AND p.id_status_prestamo NOT IN (3, 4) ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("CURP =  " + CURP);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@curp", CURP);
                adp.SelectCommand.Transaction = transaction;
                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {

                    item = new Prestamo();
                    item.IdPrestamo = int.Parse(ds.Tables[0].Rows[0]["id_prestamo"].ToString());

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



        [WebMethod]
        public static Documento GetDocument(string path, string idCliente, string idTipoDocumento)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Documento item = new Documento();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_documento_colaborador, contenido, extension, nombre, id_cliente
                                  FROM documento    
                                  WHERE id_cliente = @id AND id_tipo_documento = @id_tipo_documento ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("idCliente =  " + idCliente);
                Utils.Log("idTipoDocumento =  " + idTipoDocumento);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idCliente);
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




        [WebMethod]
        public static List<TipoCliente> GetListaItemsTipoCliente(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoCliente> items = new List<TipoCliente>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente, tipo_cliente FROM  tipo_cliente WHERE  ISNull(eliminado, 0) = 0   ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TipoCliente item = new TipoCliente();
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id_tipo_cliente"].ToString());
                        item.NombreTipoCliente = ds.Tables[0].Rows[i]["tipo_cliente"].ToString();

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
        public static Cliente GetDataPrestamo(string path, string id)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Cliente item = new Cliente();
            SqlConnection conn = new SqlConnection(strConexion);


            try
            {
                conn.Open();


                item = GetItemClient(path, id, conn, strConexion);
                item.direccion = GetAddress(path, id, 0, conn, strConexion);
                item.direccionAval = GetAddress(path, id, 1, conn, strConexion);


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
        public static Cliente GetCustomerByCurp(string path, string curp)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Cliente item = new Cliente();
            SqlConnection conn = new SqlConnection(strConexion);


            try
            {
                conn.Open();


                item = GetCustomer(path, curp, conn, strConexion);
                if (item != null)
                {
                    item.direccion = GetAddress(path, item.IdCliente.ToString(), 0, conn, strConexion);
                    item.direccionAval = GetAddress(path, item.IdCliente.ToString(), 1, conn, strConexion);
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
        public static Cliente GetItemClient(string path, string id, SqlConnection conn, string strconexion)
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
        public static Cliente GetItemCustomer(string path, string id, SqlConnection conn, string strconexion)
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
        public static Direccion GetAddress(string path, string idCliente, int aval, SqlConnection conn, string strconexion)
        {

            Direccion item = new Direccion();

            try
            {
                DataSet ds = new DataSet();
                string query = @" SELECT id_direccion, id_empleado, id_cliente, id_aval, calleyno, colonia, municipio, estado, 
                                    codigo_postal, id_municipio, id_estado, activo, ISNULL(aval, 0) aval, direccion_trabajo
                                    FROM direccion
                                    WHERE id_cliente =  @id_cliente AND aval = @aval
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
        public static Cliente GetCustomer(string path, string curp, SqlConnection conn, string strconexion)
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




    }



}