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
    public partial class LoansIndex : System.Web.UI.Page
    {
        const string pagina = "13";



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
        public static List<Prestamo> GetListaItems(string path, string idUsuario, string idTipoUsuario, string idStatus,
                string fechaInicial, string fechaFinal)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }
            

            //  Lista de datos a devolver
            List<Prestamo> items = new List<Prestamo>();


            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                //  Traer datos del usuario para saber su id_empleado
                Usuario user = Usuarios.GetUsuario(path, idUsuario);


                //  Filtro status del préstamo
                var sqlStatus = "";
                if (idStatus != "-1")
                {
                    sqlStatus = " AND p.id_status_prestamo = '" + idStatus + "'";
                }


                var sqlUsuario = "";

                //  Si es superusuario que vea todos los datos de todos
                if (idTipoUsuario != Employees.SUPERUSUARIO.ToString())
                {

                    //  Filtro para que el promotor solo vea sus prestamos 
                    if (idTipoUsuario == Employees.POSICION_PROMOTOR.ToString())
                    {
                        sqlUsuario = " AND p.id_usuario = " + idUsuario;

                    }

                    //  Filtro para que el supervisor vea los prestamos hechos por sus promotores
                    else if (idTipoUsuario == Employees.POSICION_SUPERVISOR.ToString())
                    {

                            //  La subquery arroja todos los id_usuario, que son empleados que dependen del supervisor logueado
                        sqlUsuario = @" AND p.id_usuario IN   
                                        ( select u.id_usuario
		                                        from empleado e
                                                join empleado superv ON (e.id_supervisor = superv.id_empleado)
                                                JOIN usuario u ON (u.id_empleado = e.id_empleado)
		                                        WHERE superv.id_empleado = " + user.IdEmpleado + @" )

                            ";

                    }

                    //  Filtro para que el ejecutivo vea los prestamos asignados a sus supervisores
                    else if (idTipoUsuario == Employees.POSICION_EJECUTIVO.ToString())
                    {
                        sqlUsuario = @" AND p.id_usuario IN   
                                        ( select u.id_usuario
		                                    from empleado e
                                            join empleado ejec ON (e.id_ejecutivo = ejec.id_empleado)
                                            JOIN usuario u ON (u.id_empleado = e.id_empleado)
		                                    WHERE ejec.id_empleado = " + user.IdEmpleado + @" )

                            ";

                    }


                }



                DataSet ds = new DataSet();
                string query = @" SELECT c.id_cliente,
                     concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                     c.telefono , c.curp, c.ocupacion, c.activo, p.id_prestamo, p.monto,
                     FORMAT(p.fecha_solicitud, 'dd/MM/yyyy') fecha_solicitud,
                     st.nombre nombre_status_prestamo, st.color
                     FROM cliente c 
                     JOIN prestamo p ON (p.id_cliente = c.id_cliente) 
                     JOIN status_prestamo st ON (st.id_status_prestamo = p.id_status_prestamo) 
                     WHERE  "
                    + @" (p.fecha_solicitud >= '" + fechaInicial + @"' AND p.fecha_solicitud <= '" + fechaFinal + @"') "
                    + sqlStatus
                    + sqlUsuario
                    + " ORDER BY p.id_prestamo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        Prestamo item = new Prestamo();
                        item.Cliente = new Cliente();
                        item.Cliente.IdCliente = int.Parse(ds.Tables[0].Rows[i]["id_cliente"].ToString());
                        item.Cliente.Curp = ds.Tables[0].Rows[i]["curp"].ToString();
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());

                        item.Cliente.NombreCompleto = ds.Tables[0].Rows[i]["nombre_completo"].ToString();

                        item.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        item.NombreStatus = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_prestamo"].ToString() + "</span>";


                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2");//moneda Mx -> $ 2.00
                        
                        item.FechaSolicitud = ds.Tables[0].Rows[i]["fecha_solicitud"].ToString();

                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());


                        string botones = "";

                        //if (idTipoUsuario != Employees.SUPERUSUARIO.ToString())
                        //{
                         
                        //    botones += "<button disabled onclick='loansEdit.view(" + item.Cliente.IdCliente + ")'  class='btn btn-outline-primary'> <span class='fa fa-eye mr-1'></span>Ver</button>";
                        //}
                        //else
                        //{

                            botones += "<button onclick='loansindex.view(" + item.IdPrestamo + ")'  class='btn btn-outline-primary'> <span class='fa fa-eye mr-1'></span>Ver</button>";
                        //}


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
        public static Garantia getGuaranteePhoto(string path, string idGarantia)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Garantia item = new Garantia();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_garantia_prestamo, fotografia 
                                  FROM garantia_prestamo  
                                  WHERE id_garantia_prestamo = @id_garantia_prestamo ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("idGarantia =  " + idGarantia);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_garantia_prestamo", idGarantia);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    item.IdGarantia = int.Parse(ds.Tables[0].Rows[0]["id_garantia_prestamo"].ToString());
                    item.Fotografia = ds.Tables[0].Rows[0]["fotografia"].ToString();
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
        public static List<StatusPrestamo> GetListaStatusPrestamo(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_status_prestamo, nombre FROM  status_prestamo 
                                    ORDER by id_status_prestamo";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        StatusPrestamo item = new StatusPrestamo();
                        item.IdStatusPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_status_prestamo"].ToString());
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
        public static List<Garantia> GetListGuaranteeCustomer(string path, string idUsuario, string idPrestamo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Garantia> items = new List<Garantia>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_garantia_prestamo, nombre, numero_serie, costo, fotografia, 
                            FORMAT(fecha_registro, 'dd/MM/yyyy') fecha_registro
                            FROM  garantia_prestamo 
                            WHERE id_prestamo = @id_prestamo AND
                            aval = 0 AND eliminado = 0 
                            ORDER by id_garantia_prestamo";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.SelectCommand.Parameters.AddWithValue("@id_prestamo", idPrestamo);


                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Garantia item = new Garantia();
                        item.IdGarantia = int.Parse(ds.Tables[0].Rows[i]["id_garantia_prestamo"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.Fecha = ds.Tables[0].Rows[i]["fecha_registro"].ToString();
                        item.Imagen = "<img src='../../img/upload.png' class='img-fluid garantias' id='img_garantia_" + item.IdGarantia + "' data-idgarantia='" + item.IdGarantia + "' />";

                        string botones = "&nbsp; <button  onclick='panelGuarantee.delete(" + item.IdGarantia + ")'   class='btn btn-outline-primary boton-ocultable'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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
        public static List<Garantia> GetListGuaranteeAval(string path, string idUsuario, string idPrestamo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Garantia> items = new List<Garantia>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_garantia_prestamo, nombre, numero_serie, costo, fotografia, 
                            FORMAT(fecha_registro, 'dd/MM/yyyy') fecha_registro
                            FROM  garantia_prestamo
                            WHERE id_prestamo = @id_prestamo AND
                            aval = 1 AND eliminado = 0 
                            ORDER by id_garantia_prestamo";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.SelectCommand.Parameters.AddWithValue("@id_prestamo", idPrestamo);


                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Garantia item = new Garantia();
                        item.IdGarantia = int.Parse(ds.Tables[0].Rows[i]["id_garantia_prestamo"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.Costo = float.Parse(ds.Tables[0].Rows[i]["costo"].ToString());
                        item.Fecha = ds.Tables[0].Rows[i]["fecha_registro"].ToString();
                        item.Imagen = "<img src='../../img/upload.png' class='img-fluid garantias' id='img_garantia_" + item.IdGarantia + "' data-idgarantia='" + item.IdGarantia + "' />";

                        string botones = "&nbsp; <button  onclick='panelGuarantee.delete(" + item.IdGarantia + ")'   class='btn btn-outline-primary boton-ocultable'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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
        public static DatosSalida SaveCustomerGuarantee(string path, Garantia item, string idUsuario, string idPrestamo)
        {


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            DatosSalida salida = new DatosSalida();

            try
            {


                conn.Open();

                string sql = "";


                sql = @" INSERT INTO garantia_prestamo (id_prestamo, nombre, numero_serie, costo, fecha_registro, 
                    aval, eliminado) 

                    OUTPUT INSERTED.id_garantia_prestamo

                    VALUES (@id_prestamo, @nombre, @numero_serie, @costo, @fecha_registro, 0, 0) ";



                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@numero_serie", item.NumeroSerie);
                cmd.Parameters.AddWithValue("@costo", item.Costo);
                cmd.Parameters.AddWithValue("@fecha_registro", DateTime.Now);
                cmd.Parameters.AddWithValue("@id", item.IdGarantia);



                

                int idGenerado = (int)cmd.ExecuteScalar();
                
                Utils.Log("Guardado -> OK ");

                

                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idGenerado.ToString();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
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
        public static DatosSalida SaveAvalGuarantee(string path, Garantia item, string idUsuario, string idPrestamo)
        {


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            DatosSalida salida = new DatosSalida();

            try
            {


                conn.Open();

                string sql = "";


                sql = @" INSERT INTO garantia_prestamo (id_prestamo, nombre, numero_serie, costo, fecha_registro, 
                    aval, eliminado) 

                    OUTPUT INSERTED.id_garantia_prestamo

                    VALUES (@id_prestamo, @nombre, @numero_serie, @costo, @fecha_registro, 1, 0) ";



                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                cmd.Parameters.AddWithValue("@numero_serie", item.NumeroSerie);
                cmd.Parameters.AddWithValue("@costo", item.Costo);
                cmd.Parameters.AddWithValue("@fecha_registro", DateTime.Now);
                cmd.Parameters.AddWithValue("@id", item.IdGarantia);





                int idGenerado = (int)cmd.ExecuteScalar();

                Utils.Log("Guardado -> OK ");



                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idGenerado.ToString();

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
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
        public static DatosSalida Delete(string path, string id, string idUsuario)
        {



            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo eliminar el registro.";

                return salida;

            }

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            try
            {

                conn.Open();

                string sql = @" UPDATE garantia_prestamo SET eliminado = 1  
                                        WHERE id_garantia_prestamo = @id ";

                Utils.Log("\n-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");


                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);

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
                salida.MensajeError = "No se pudo eliminar el registro.";



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