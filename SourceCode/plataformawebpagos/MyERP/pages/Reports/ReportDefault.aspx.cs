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
    public partial class ReportDefault : System.Web.UI.Page
    {
        const string pagina = "19";



        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string idTipoUsuario = (string)Session["id_tipo_usuario"];
            string idUsuario = (string)Session["id_usuario"];
            string path = (string)Session["path"];



            txtUsuario.Value = usuario;//"promotor.colorado
            txtIdTipoUsuario.Value = idTipoUsuario;//5
            txtIdUsuario.Value = idUsuario;//69

            //  si no esta logueado
            if (usuario == string.Empty)
            {
                Response.Redirect("Login.aspx");
            }


        }

        public class Total
        {
            public float total;
            public string totalStr;
        }

        public class LineaDeterminacion
        {
            public string Promotor;
            public float Comision;
            public string ComisionFormateado;
            public float DebeEntregar;
            public float Falla;
            public float Efectivo;
            public float Recuperado;
            public float AbonoEntrante;
            public float Total;
            public float AbonoSaliente;
            public float Total2;
            public float PorcentajeFalla;

            public string DebeEntregarFormateadoMx;
            public string FallaFormateadoMx;
            public string EfectivoFormateadoMx;
            public string RecuperadoFormateadoMx;
            public string AbonoEntranteFormateadoMx;
            public string TotalFormateadoMx;
            public string AbonoSalienteFormateadoMx;
            public string Total2FormateadoMx;
            public string PorcentajeFallaFormateadoMx;

            public float Venta;
            public string VentaFormateadoMx;

            public float Comisiones;
            public string ComisionesFormateadoMx;

            public float TotalFinal;
            public string TotalFinalFormateadoMx;


        }

        /// <summary>
        /// Debe entregar y falla
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idPlaza"></param>
        /// <param name="idEjecutivo"></param>
        /// <param name="idPromotor"></param>
        /// <param name="idSupervisor"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Total> GetTotals(string path, string idUsuario, string idPromotor,
            string fechaInicial, string fechaFinal)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            List<Total> items = new List<Total>();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                //debe entregar
                string query = @"   SELECT IsNull(SUM(p.monto) , 0)  total
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)       
                                    JOIN cliente cc ON (cc.id_cliente = pre.id_cliente AND IsNull(cc.id_status_cliente, 2) <>" + Cliente.STATUS_CONDONADO + @" )                                                                                       
                                    WHERE "
                                    + @"(p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')
                                        AND pre.id_empleado = " + idPromotor + @" 
                                        AND p.id_status_pago = " + 1 + "";

                using (SqlDataAdapter adp = new SqlDataAdapter(query, conn))
                {

                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                    adp.Fill(ds);

                    Total item = new Total();
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        item.total = float.Parse(ds.Tables[0].Rows[0]["total"].ToString());
                        item.totalStr = item.total.ToString("C2");
                    }
                    items.Add(item);

                }


                //falla
                query = @"   SELECT IsNull(SUM(p.monto) , 0)  total
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                                                                       
                                    WHERE "
                                    + @"(p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')
                                        AND pre.id_empleado = " + idPromotor + @" 
                                        AND p.id_status_pago = " + 2 + "";



                using (SqlDataAdapter adapterFalla = new SqlDataAdapter(query, conn))
                {
                    DataSet dataseFalla = new DataSet();

                    Utils.Log("\n" + query + "\n");

                    adapterFalla.Fill(dataseFalla);

                    Total item = new Total();
                    if (dataseFalla.Tables[0].Rows.Count > 0)
                    {

                        item.total = float.Parse(dataseFalla.Tables[0].Rows[0]["total"].ToString());
                        item.totalStr = item.total.ToString("C2");
                    }
                    items.Add(item);
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
        public static Total GetTotalByPaymentStatus(string path, string idUsuario, string idPromotor,
           string fechaInicial, string fechaFinal, string idStatusPago)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            Total item = new Total();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                string query = @"   SELECT IsNull(SUM(p.monto) , 0)  total
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                                                                       
                                    WHERE "
                                    + @"(p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')
                                        AND pre.id_empleado = " + idPromotor + @" 
                                        AND p.id_status_pago = " + idStatusPago + "";

                using (SqlDataAdapter adp = new SqlDataAdapter(query, conn))
                {

                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        item.total = float.Parse(ds.Tables[0].Rows[0]["total"].ToString());
                        item.totalStr = item.total.ToString("C2");
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
        public static Total GetTotalByStatusAndPromotorAndSemanaEntrante(string path, string idUsuario,
            string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Total item = new Total();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                DataSet ds = new DataSet();
                string query = @" SELECT IsNull(SUM(p.monto), 0) total
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha_registro_pago >= '" + fechaInicial + @"' AND p.fecha_registro_pago <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                    + " AND IsNull(p.pagado_con_adelanto, 0) = 1 "
                                    + " AND p.id_status_pago = " + Pago.STATUS_PAGO_PAGADO
                                    + "  ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {

                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        item.total = float.Parse(ds.Tables[0].Rows[0]["total"].ToString());
                        item.totalStr = item.total.ToString("C2");
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
        public static Total GetTotalByStatusAndPromotorAndSemanaSaliente(string path, string idUsuario,
          string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            Total item = new Total();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                DataSet ds = new DataSet();
                string query = @" SELECT IsNull(SUM(p.monto), 0) total
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                    + " AND IsNull(p.pagado_con_adelanto, 0) = 1 "
                                    + " AND p.id_status_pago = " + Pago.STATUS_PAGO_PAGADO
                                    + "  ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {

                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        item.total = float.Parse(ds.Tables[0].Rows[0]["total"].ToString());
                        item.totalStr = item.total.ToString("C2");
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
        public static List<Empleado> GetListaEjecutivosByPlaza(string path,
        string idPlaza)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            var sqlPlaza = "";
            if (idPlaza != "" && idPlaza != "-1")
            {
                sqlPlaza = " AND id_plaza = '" + idPlaza + "'";
            }





            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado,
                                    concat( e.nombre ,  ' ' , e.primer_apellido , ' ' ,  e.segundo_apellido) AS nombre_completo
                                    FROM empleado e
                                    WHERE ISNull(e.activo, 1) = 1  
                                    AND ISNull(e.eliminado, 0) = 0
                                    AND e.id_posicion =  " + Employees.POSICION_EJECUTIVO +
                                    sqlPlaza
                                    ;

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
        public static List<Empleado> GetListaSupervisoresByEjecutivo(string path,
            string idEjecutivo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();

            var sqlEjecutivo = "";
            if (idEjecutivo != "" && idEjecutivo != "-1")
            {
                sqlEjecutivo = " AND e.id_ejecutivo = '" + idEjecutivo + "'";
            }


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado,
                                    concat( e.nombre ,  ' ' , e.primer_apellido , ' ' ,  e.segundo_apellido) AS nombre_completo
                                    FROM empleado e
                                    WHERE ISNull(e.activo, 1) = 1  
                                    AND ISNull(e.eliminado, 0) = 0
                                    AND e.id_posicion =  " + Employees.POSICION_SUPERVISOR +
                                    sqlEjecutivo
                                    ;

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


        /// <summary>
        /// TRaer los datos de prestamos que han sido aprobados la semana que se indica, préstamos del promotor.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idTipoUsuario"></param>
        /// <param name="idStatus"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>

        [WebMethod]
        public static Total GetTotalLoanByPromotor(string path, string idUsuario,
                string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            //  Lista de datos a devolver
            Total item = new Total();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();

                DataSet ds = new DataSet();
                string query = @" SELECT IsNull(SUM(p.monto), 0) total
                     FROM prestamo p
                     WHERE  "
                    + @" (p.fecha_aprobacion >= '" + fechaInicial + @"' AND p.fecha_aprobacion <= '" + fechaFinal + @"') "
                    + " AND p.id_status_prestamo = '" + Prestamo.STATUS_APROBADO + "'"
                    + " AND p.id_empleado = '" + idPromotor + "'";


                Utils.Log("\nMétodo-> " +
                 System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                using (SqlDataAdapter adp = new SqlDataAdapter(query, conn))
                {


                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        item.total = float.Parse(ds.Tables[0].Rows[0]["total"].ToString());
                        item.totalStr = item.total.ToString("C2");
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
        public static List<Empleado> GetListaPromotoresBySupervisor(string path,
            string idSupervisor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Empleado> items = new List<Empleado>();


            var sqlSupervisor = "";
            if (idSupervisor != "" && idSupervisor != "-1")
            {
                sqlSupervisor = " AND e.id_supervisor = '" + idSupervisor + "'";
            }


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado, e.id_comision_inicial,
                                    concat( e.nombre ,  ' ' , e.primer_apellido , ' ' ,  e.segundo_apellido) AS nombre_completo
                                    FROM empleado e
                                    WHERE ISNull(e.activo, 1) = 1  
                                    AND ISNull(e.eliminado, 0) = 0
                                    AND e.id_posicion =  " + Employees.POSICION_PROMOTOR +
                                    sqlSupervisor
                                    ;

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
                        item.Nombre = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.IdComisionInicial = int.Parse(ds.Tables[0].Rows[i]["id_comision_inicial"].ToString());

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
        public static Empleado GetPromotorDataById(string path,
            string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            Empleado item = new Empleado();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_empleado, e.id_comision_inicial, IsNull(c.porcentaje, 0) porcentaje,
                                    concat( e.nombre ,  ' ' , e.primer_apellido , ' ' ,  e.segundo_apellido)
                                        AS nombre_completo
                                    FROM empleado e
                                    JOIN comision c ON (e.id_comision_inicial = c.id_comision)
                                    WHERE ISNull(e.activo, 1) = 1  
                                    AND ISNull(e.eliminado, 0) = 0
                                    AND e.id_empleado =  " + id;

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);


                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.IdComisionInicial = int.Parse(ds.Tables[0].Rows[i]["id_comision_inicial"].ToString());
                        item.PorcentajeComision = float.Parse(ds.Tables[0].Rows[i]["porcentaje"].ToString());

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
        public static List<Pago> GetPaymentsByStatusAndPromotor(string path, string idUsuario, string idStatus,
                string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;



            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();


            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                //  Filtro status del pago
                var sqlStatus = "";
                if (idStatus != "0")    //  todos
                {
                    sqlStatus = " AND p.id_status_pago = '" + idStatus + "'";
                }

                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                    FORMAT(p.fecha, 'dd/MM/yyyy') fechastr
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                    + sqlStatus
                                    + " ORDER BY p.id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();
                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00

                        //item.TotalFalla = float.Parse(ds.Tables[0].Rows[i]["total_falla"].ToString());
                        //item.TotalFallaFormateadoMx = item.TotalFalla.ToString("C2");

                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();

                        //item.SemanasFalla = ds.Tables[0].Rows[i]["semanas_falla"].ToString();


                        //item.Color = ds.Tables[0].Rows[i]["color"].ToString();
                        //item.Status = "<span class='" + item.Color + "'>" + ds.Tables[0].Rows[i]["nombre_status_pago"].ToString() + "</span>";


                        //string botones = "";

                        //botones += "<button data-idprestamo = " + item.IdPrestamo + " onclick='payments.view(" + item.IdPago + ")'  class='btn btn-outline-primary'> <span class='fa fa-folder-open mr-1'></span>Abrir</button>";

                        //item.Accion = botones;

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
        public static List<Pago> GetPaymentsByStatusAndPromotorAndSemanaExtra(string path, string idUsuario, string idStatus,
              string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                //  Filtro status del pago
                var sqlStatus = "";
                if (idStatus != "0")    //  todos
                {
                    sqlStatus = " AND p.id_status_pago = '" + idStatus + "'";
                }

                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                    FORMAT(p.fecha, 'dd/MM/yyyy') fechastr
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                    + sqlStatus
                                    + " AND IsNull(p.semana_extra, 0) = 1 " +
                                    "ORDER BY p.id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();
                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00

                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();

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
        public static List<Pago> GetPaymentsByStatusFallaAndPromotor(string path,
              string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                    FORMAT(p.fecha, 'dd/MM/yyyy') fechastr
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                    + " AND p.id_status_pago  =  " + Pago.STATUS_PAGO_FALLA + "  "
                                    + " AND IsNull(p.semana_extra, 0) = 0 " +
                                    " ORDER BY p.id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();
                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00

                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();

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
        public static List<Pago> GetPaymentsByStatusFallaRecuperado(string path,
             string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                    FORMAT(p.fecha, 'dd/MM/yyyy') fechastr
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                    + " AND p.id_status_pago  =  " + Pago.STATUS_PAGO_ABONADO + "  "
                                    + " AND IsNull(p.semana_extra, 0) = 0 "
                                    + " AND IsNull(p.es_recuperado, 0) = 1 "
                                    + " ORDER BY p.id_pago ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();
                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00

                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();

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
        public static List<Pago> GetPaymentsByStatusAdelantoEntrante(string path,
           string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                    FORMAT(p.fecha_registro_pago, 'dd/MM/yyyy') fechastr
                                    FROM pago p
                                    JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                  + @" WHERE (p.fecha_registro_pago >= '" + fechaInicial + @"' AND p.fecha_registro_pago <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                  + " AND IsNull(p.pagado_con_adelanto, 0) = 1 "
                                  + " AND p.id_status_pago = " + Pago.STATUS_PAGO_PAGADO;


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();
                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2");

                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();

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
        public static List<Pago> GetPaymentsByStatusAdelantoSaliente(string path,
           string fechaInicial, string fechaFinal, string idPromotor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            //  Lista de datos a devolver
            List<Pago> items = new List<Pago>();

            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                    FORMAT(p.fecha, 'dd/MM/yyyy') fechastr
                                    FROM pago p
                                     JOIN prestamo pre ON (p.id_prestamo = pre.id_prestamo)                                            
                                    JOIN cliente c ON (c.id_cliente = pre.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"')                                 
                                        AND pre.id_empleado = " + idPromotor + "  "
                                    + " AND IsNull(p.pagado_con_adelanto, 0) = 1 "
                                    + " AND p.id_status_pago = " + Pago.STATUS_PAGO_PAGADO
                                    + "  ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Pago item = new Pago();
                        item.IdPago = int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NumeroSemana = int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2");

                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();

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
        public static DatosSalida SaveReport(string path, string idUsuario, string fecha, string idTipoReporte)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }



            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;

            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();

                string sql = "";

                sql = @"  INSERT INTO reporte
                                (id_usuario, fecha, id_tipo_reporte)                             
                                OUTPUT INSERTED.id_reporte                                
                                VALUES (@id_usuario, @fecha, @id_tipo_reporte)";


                Utils.Log("insert reporte " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@id_tipo_reporte", idTipoReporte);
                cmd.Transaction = transaccion;

                int idGenerado = (int)cmd.ExecuteScalar();

                Utils.Log("Guardado -> OK ");


                transaccion.Commit();

                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idGenerado.ToString();

            }
            catch (Exception ex)
            {
                try
                {
                    transaccion.Rollback();
                }
                catch (Exception exRollBack)
                {
                    Utils.Log("Error ... " + exRollBack.Message);
                }

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                salida.MensajeError = "Se ha generado un error.";
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



        [WebMethod]
        public static List<LineaDeterminacion> getTablePrincipalFondos(string path, string idUsuario, string idSupervisor,
           string fechaInicial, string fechaFinal)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            List<LineaDeterminacion> items = new List<LineaDeterminacion>();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();

                //debe entregar
                string query = @" 
                    SELECT concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido) AS promotor,
 						c.porcentaje comision,                     

                         (SELECT IsNull(SUM(pp.monto) , 0)  total
                                    FROM pago pp
                                    JOIN prestamo pre2 ON (pp.id_prestamo = pre2.id_prestamo)                                                                                       
                                    JOIN cliente cc ON (cc.id_cliente = pre2.id_cliente AND IsNull(cc.id_status_cliente, 2) <>" + Cliente.STATUS_CONDONADO + @" )                                                                                       
                                    WHERE 
                                        (pp.fecha >= '" + fechaInicial + @"' AND pp.fecha <= '" + fechaFinal + @"')                                            
                                        AND pre2.id_empleado = e.id_empleado
                                        AND pp.id_status_pago = " + Pago.STATUS_PAGO_PENDIENTE + @")                total_debe_entregar,

                            (SELECT IsNull(SUM(nuevosPrestamos.monto), 0)  total                                    
                                    FROM prestamo nuevosPrestamos                                                                                       
                                    WHERE 
                                        (nuevosPrestamos.fecha_aprobacion >= '" + fechaInicial +
                                        @"' AND nuevosPrestamos.fecha_aprobacion <= '" + fechaFinal + @"')                                            
                                        AND nuevosPrestamos.id_empleado = e.id_empleado
                                        AND nuevosPrestamos.id_status_prestamo = " + Prestamo.STATUS_APROBADO + @")                    
                                                                                                                    total_venta,
                     
                         (SELECT IsNull(SUM(pp.monto) , 0)  total
                                    FROM pago pp
                                    JOIN prestamo pre2 ON (pp.id_prestamo = pre2.id_prestamo)                                                                                       
                                    WHERE 
                                        (pp.fecha >= '" + fechaInicial + @"' AND pp.fecha <= '" + fechaFinal + @"')                                            
                                        AND pre2.id_empleado = e.id_empleado
                                        AND pp.id_status_pago = " + Pago.STATUS_PAGO_FALLA + @")                    total_falla,
                                                                                
                         (SELECT IsNull(SUM(p3.monto) , 0)  total
                                    FROM pago p3
                                    JOIN prestamo pre3 ON (p3.id_prestamo = pre3.id_prestamo)                                                                                       
                                    WHERE 
                                        (p3.fecha >= '" + fechaInicial + @"' AND p3.fecha <= '" + fechaFinal + @"')
                                        AND pre3.id_empleado = e.id_empleado
                                        AND p3.id_status_pago = " + Pago.STATUS_PAGO_ABONADO + @")                  total_recuperado,
                                        
                         (SELECT IsNull(SUM(pagoEntrante.monto) , 0)  total
                                    FROM pago pagoEntrante
                                    JOIN prestamo preEntrante ON (pagoEntrante.id_prestamo = preEntrante.id_prestamo)                                                                                       
                                    WHERE
                                        (pagoEntrante.fecha_registro_pago >= '" + fechaInicial + @"' AND pagoEntrante.fecha_registro_pago <= '" + fechaFinal + @"')
                                        AND preEntrante.id_empleado = e.id_empleado
                                        AND IsNull(pagoEntrante.pagado_con_adelanto, 0) = 1
                         				AND pagoEntrante.id_status_pago = " + Pago.STATUS_PAGO_PAGADO + @")         total_abono_entrante,
                                                                                
                         (SELECT IsNull(SUM(pagoSaliente.monto) , 0)  total
                                    FROM pago pagoSaliente
                                    JOIN prestamo preSaliente ON (pagoSaliente.id_prestamo = preSaliente.id_prestamo)                                                                                       
                                    WHERE 
                                        (pagoSaliente.fecha >= '" + fechaInicial + @"' AND pagoSaliente.fecha <= '" + fechaFinal + @"')
                                        AND preSaliente.id_empleado = e.id_empleado
                                        AND IsNull(pagoSaliente.pagado_con_adelanto, 0) = 1
                         				AND pagoSaliente.id_status_pago = " + Pago.STATUS_PAGO_PAGADO + @")         total_abono_saliente
                                        
                                    FROM empleado e 
                                    JOIN comision c ON (c.id_comision = e.id_comision_inicial)  
                                    WHERE e.id_supervisor = " + idSupervisor + @" 
                                  	GROUP BY 
                                    e.id_empleado,
                                    concat(e.nombre ,  ' ' , e.primer_apellido , ' ' , e.segundo_apellido), 
                                    c.porcentaje ";

                using (SqlDataAdapter adp = new SqlDataAdapter(query, conn))
                {

                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {

                            LineaDeterminacion item = new LineaDeterminacion();
                            item.Promotor = ds.Tables[0].Rows[i]["promotor"].ToString();

                            item.Comision = float.Parse(ds.Tables[0].Rows[i]["comision"].ToString());
                            item.ComisionFormateado = item.Comision + "%";

                            item.DebeEntregar = float.Parse(ds.Tables[0].Rows[i]["total_debe_entregar"].ToString());
                            item.DebeEntregarFormateadoMx = item.DebeEntregar.ToString("C2");

                            item.Falla = float.Parse(ds.Tables[0].Rows[i]["total_falla"].ToString());
                            item.FallaFormateadoMx = item.Falla.ToString("C2");

                            item.Efectivo = item.DebeEntregar - item.Falla;
                            item.EfectivoFormateadoMx = item.Efectivo.ToString("C2");

                            item.Recuperado = float.Parse(ds.Tables[0].Rows[i]["total_recuperado"].ToString());
                            item.RecuperadoFormateadoMx = item.Recuperado.ToString("C2");

                            item.AbonoEntrante = float.Parse(ds.Tables[0].Rows[i]["total_abono_entrante"].ToString());
                            item.AbonoEntranteFormateadoMx = item.AbonoEntrante.ToString("C2");


                            item.AbonoSaliente = float.Parse(ds.Tables[0].Rows[i]["total_abono_saliente"].ToString());
                            item.AbonoSalienteFormateadoMx = item.AbonoSaliente.ToString("C2");

                            item.Total2 = item.Efectivo + item.Recuperado + item.AbonoEntrante - item.AbonoSaliente;
                            item.Total2FormateadoMx = item.Total2.ToString("C2");

                            item.Venta = float.Parse(ds.Tables[0].Rows[i]["total_venta"].ToString());
                            item.VentaFormateadoMx = item.Venta.ToString("C2");

                            item.Comisiones = (item.Venta * item.Comision) / 100;
                            item.ComisionesFormateadoMx = item.Comisiones.ToString("C2");

                            item.TotalFinal = item.Venta + item.Comisiones;
                            item.TotalFinalFormateadoMx = item.TotalFinal.ToString("C2");

                            item.Total = item.Venta - item.Total2;
                            item.TotalFormateadoMx = item.Total.ToString("C2");


                            if (item.Falla > 0)
                            {
                                item.PorcentajeFalla = item.DebeEntregar / item.Falla;
                                item.PorcentajeFallaFormateadoMx = item.PorcentajeFalla.ToString("#.##") + "%";
                            }
                            else
                            {
                                item.PorcentajeFalla = 0;
                                item.PorcentajeFallaFormateadoMx = "0%";
                            }


                            items.Add(item);

                        }
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
        public static List<Gasto> GetItemsGastos(string path, string idSupervisor, string fechaInicial, string fechaFinal)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Gasto> items = new List<Gasto>();


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id, concepto, monto, id_usuario, id_empleado,
                                FORMAT(fecha, 'dd/MM/yyyy') fecha 
                                FROM gasto " +
                                 @" WHERE (fecha >= '" + fechaInicial + @"' AND fecha <= '" + fechaFinal + @"')   
                                AND id_empleado = " + idSupervisor + @"
                                ORDER BY id ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Gasto item = new Gasto();
                        item.IdGasto = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2");
                        item.Concepto = (ds.Tables[0].Rows[i]["concepto"].ToString());
                        item.Fecha = ds.Tables[0].Rows[i]["fecha"].ToString();


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





