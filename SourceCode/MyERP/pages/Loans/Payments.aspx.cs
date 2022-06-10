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
    public partial class Payments : System.Web.UI.Page
    {
        const string pagina = "15";



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
        public static DatosSalida Paid(string path, string idPrestamo, string idUsuario, string idPosicion, string nota)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<StatusPrestamo> items = new List<StatusPrestamo>();
            DatosSalida response = new DatosSalida();


            SqlTransaction transaccion = null;

            int r = 0;
            try
            {

                conn.Open();
                transaccion = conn.BeginTransaction();



                //  Actualizar relacion de aprobaciones
                string sqlActualizaPosicion = idPosicion == Employees.POSICION_SUPERVISOR.ToString() ? ", id_supervisor = @id_supervisor " : ", id_ejecutivo = @id_ejecutivo ";

               string sql = @"  UPDATE relacion_prestamo_aprobacion
                                SET fecha = @fecha, notas_generales = @notas_generales, status_aprobacion = @status_aprobacion  "
                                + sqlActualizaPosicion
                                + @"WHERE id_prestamo = @id_prestamo AND
                                id_posicion = " + idPosicion + " ";


                Utils.Log("ACTUALIZAR RelacionPrestamoAprobacion " + sql);

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_prestamo", idPrestamo);
                cmd.Parameters.AddWithValue("@notas_generales", nota);
                cmd.Parameters.AddWithValue("@id_supervisor", idUsuario);
                cmd.Parameters.AddWithValue("@id_ejecutivo", idUsuario);
                cmd.Parameters.AddWithValue("@status_aprobacion", "Aprobado");
                cmd.Transaction = transaccion;

                r += cmd.ExecuteNonQuery();


                //
                transaccion.Commit();




                return response;
            }
            catch (Exception ex)
            {

                transaccion.Rollback();

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                response.MensajeError = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                response.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return response;


        }



        [WebMethod]
        public static List<Pago> GetListaItems(string path, string idUsuario, string idTipoUsuario, string idStatus,
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
            List<Pago> items = new List<Pago>();


            SqlConnection conn = new SqlConnection(strConexion);

            try
            {

                conn.Open();

                //  Traer datos del usuario para saber su id_empleado
                Usuario user = Usuarios.GetUsuario(path, idUsuario);


                //  Filtro status del pago
                var sqlStatus = "";
                if (idStatus != "0")    //  todos
                {
                    sqlStatus = " AND p.id_status_pago = '" + idStatus + "'";
                }

                


                DataSet ds = new DataSet();
                string query = @" SELECT p.id_pago, p.id_prestamo, p.monto, p.saldo, p.fecha, p.id_status_pago, p.id_usuario, p.numero_semana,
                                    concat(c.nombre ,  ' ' , c.primer_apellido , ' ' , c.segundo_apellido) AS nombre_completo,
                                     FORMAT(p.fecha, 'dd/MM/yyyy') fechastr,
            
                                    st.nombre nombre_status_pago
                                    FROM pago p
                                    JOIN prestamo prestamo ON (p.id_prestamo = prestamo.id_prestamo)                                            
                                    JOIN status_pago st ON (st.id_status_pago = p.id_status_pago)                                            
                                    JOIN cliente c ON (c.id_cliente = prestamo.id_cliente) "
                                    + @" WHERE (p.fecha >= '" + fechaInicial + @"' AND p.fecha <= '" + fechaFinal + @"') "

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
                        item.IdPago= int.Parse(ds.Tables[0].Rows[i]["id_pago"].ToString());                        
                        item.IdPrestamo = int.Parse(ds.Tables[0].Rows[i]["id_prestamo"].ToString());
                        item.NumeroSemana= int.Parse(ds.Tables[0].Rows[i]["numero_semana"].ToString());
                        item.NombreCliente = ds.Tables[0].Rows[i]["nombre_completo"].ToString();
                        item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        item.MontoFormateadoMx = item.Monto.ToString("C2"); //moneda Mx -> $ 2,233.00
                        item.FechaStr = ds.Tables[0].Rows[i]["fechastr"].ToString();
                        item.Status = ds.Tables[0].Rows[i]["nombre_status_pago"].ToString();
                                              
                        string botones = "";

                        botones += "<button onclick='payments.view(" + item.IdPrestamo + ")'  class='btn btn-outline-primary'> <span class='fa fa-folder-open mr-1'></span>Ver</button>";
                        
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


    }



}