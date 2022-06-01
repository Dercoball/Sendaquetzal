using Plataforma.Clases;
using Plataforma.pages.Loans;
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
    public partial class CustomerHistory : System.Web.UI.Page
    {
        const string pagina = "14";



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
        public static List<TipoCliente> GetItemsCustomerTypes(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoCliente> items = new List<TipoCliente>();




            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente as id, tipo_cliente ,prestamo_inicial_maximo,
                        porcentaje_semanal, semanas_a_prestar, garantias_por_monto,
                        cantidad_para_renovar, semana_extra
                     FROM tipo_cliente
                     WHERE 
                     ISNull(eliminado, 0) = 0
                     ORDER BY id ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TipoCliente item = new TipoCliente();
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.NombreTipoCliente = (ds.Tables[0].Rows[i]["tipo_cliente"].ToString());
                        item.SemanasAPrestar = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());


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
        public static TipoCliente GetItemCustomerType(string path, string customerTypeId)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            TipoCliente item = null;




            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente as id, tipo_cliente ,prestamo_inicial_maximo,
                        porcentaje_semanal, semanas_a_prestar, garantias_por_monto,
                        cantidad_para_renovar, semana_extra
                     FROM tipo_cliente
                     WHERE  id_tipo_cliente = @id_tipo_cliente
                     ORDER BY id ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_tipo_cliente", customerTypeId);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new TipoCliente();
                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.NombreTipoCliente = (ds.Tables[0].Rows[i]["tipo_cliente"].ToString());
                        item.SemanasAPrestar = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());



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


        public class Data
        {
            public string Table;
            public ItemHistorial solicitud;
            //public List<Equipo> Array = new List<Equipo>();
        }







        [WebMethod]
        public static DatosSalida Save(string path, List<ItemHistorial> data, string accion, string idUsuario, string curpCliente,
                string curpAval, string idTipoCliente)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;

            Utils.Log("\nMétodo-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            LoanValidation validations = new LoanValidation();


            int r = 0;
            try
            {

                string sql = "";

                conn.Open();
                transaccion = conn.BeginTransaction();

                //  Validar que el nuevo cliente no sea el mismo que el aval mediante su curp
                if (curpCliente == curpAval)
                {
                    salida.MensajeError = "La CURP del cliente y del aval no debe ser la misma.";
                    salida.CodigoError = 1;
                    return salida;

                }

                Cliente customerExists = validations.GetClienteByCURP(path, curpCliente, conn, strConexion, transaccion);
                if (customerExists != null)
                {
                    salida.MensajeError = "Ya existe el cliente con CURP " + curpCliente + " por favor verifique e intente de nuevo.";
                    salida.CodigoError = 1;
                    return salida;
                }


                sql = @"  INSERT INTO cliente
                                (curp, id_tipo_cliente, curp_aval, activo, eliminado)                             
                                OUTPUT INSERTED.id_cliente                                
                                VALUES 
                                (@curp, @id_tipo_cliente, @curp_aval, 1, 0)";


                Utils.Log("insert client" + sql);

                SqlCommand cmdCliente = new SqlCommand(sql, conn);
                cmdCliente.CommandType = CommandType.Text;

                cmdCliente.Parameters.AddWithValue("@id_tipo_cliente", idTipoCliente);
                cmdCliente.Parameters.AddWithValue("@curp", curpCliente);
                cmdCliente.Parameters.AddWithValue("@curp_aval", curpAval);
                cmdCliente.Transaction = transaccion;


                int idCliente = (int)cmdCliente.ExecuteScalar();


                //  Guardar direccion cliente
                sql = @"  INSERT INTO direccion
                            (id_cliente, aval, activo)
                            VALUES
                                (@id_cliente, 0, 1);
                        ";


                Utils.Log("insert direccion client" + sql);

                SqlCommand cmdAddressCustomer = new SqlCommand(sql, conn);
                cmdAddressCustomer.CommandType = CommandType.Text;
                cmdAddressCustomer.Parameters.AddWithValue("@id_cliente", idCliente);
                cmdAddressCustomer.Transaction = transaccion;
                r = cmdAddressCustomer.ExecuteNonQuery();


                //  Guardar direccion aval
                sql = @"  INSERT INTO direccion
                            (activo, aval, id_cliente)
                            VALUES
                                (1, 1, @id_cliente);
                        ";


                Utils.Log("insert direccion aval" + sql);

                SqlCommand cmdAddressCustomerAval = new SqlCommand(sql, conn);
                cmdAddressCustomerAval.CommandType = CommandType.Text;
                cmdAddressCustomerAval.Parameters.AddWithValue("@id_cliente", idCliente);
                cmdAddressCustomerAval.Transaction = transaccion;
                r += cmdAddressCustomerAval.ExecuteNonQuery();



                var index = 0;
                foreach (var item in data)
                {
                    sql = @" INSERT INTO historial_cliente 
                                (id_cliente, id_semana, atiempo, abonado, falla )
                            VALUES
                                (@id_cliente, @id_semana, @atiempo, @abonado, @falla);
                          ";



                    Utils.Log("\n" + sql + "\n");

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Transaction = transaccion;

                    cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                    cmd.Parameters.AddWithValue("@id_semana", index++);
                    cmd.Parameters.AddWithValue("@atiempo", item.ATiempo);
                    cmd.Parameters.AddWithValue("@abonado", item.Abonado);
                    cmd.Parameters.AddWithValue("@falla", item.Falla);
                    r = cmd.ExecuteNonQuery();
                }
                           

                transaccion.Commit();


                Utils.Log("Guardado -> OK ");


                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;

            }
            catch (Exception ex)
            {
                try
                {
                    transaccion.Rollback();
                }
                catch (Exception excep)
                {

                }

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                salida.MensajeError = "No se pudo completar la operación <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();
                salida.CodigoError = 1;
            }

            finally
            {
                conn.Close();
            }

            return salida;


        }



    }



}