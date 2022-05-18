using Newtonsoft.Json;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class CustomerTypes : System.Web.UI.Page
    {
        const string pagina = "7";

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
        public static List<TipoCliente> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoCliente> items = new List<TipoCliente>();


            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente as id, tipo_cliente ,prestamo_inicial_maximo, porcentaje_semanal, semanas_a_prestar, garantias_por_monto,
                     fechas_pago, cantidad_para_renovar, semana_extra
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
                        item.PrestamoInicialMaximo = float.Parse(ds.Tables[0].Rows[i]["prestamo_inicial_maximo"].ToString());
                        item.PorcentajeSemanal = float.Parse(ds.Tables[0].Rows[i]["porcentaje_semanal"].ToString());
                        item.SemanasAPrestar = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        item.GarantiasPorMonto = float.Parse(ds.Tables[0].Rows[i]["garantias_por_monto"].ToString());
                        item.FechasDePago = (ds.Tables[0].Rows[i]["fechas_pago"].ToString());
                        item.CantidadParaRenovar = float.Parse(ds.Tables[0].Rows[i]["cantidad_para_renovar"].ToString());
                        item.SemanasExtra = int.Parse(ds.Tables[0].Rows[i]["semana_extra"].ToString());

                        item.ActivoSemanaExtra = (item.SemanasExtra == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='tipoCliente.editar(" + item.IdTipoCliente + ")'  class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='tipoCliente.eliminar(" + item.IdTipoCliente + ")'   class='btn btn-outline-primary btn-sm'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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



        /**
         * Sin importar los permisos
         */
        [WebMethod]
        public static List<TipoCliente> GetListaItemsPublic(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoCliente> items = new List<TipoCliente>();


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente as id, tipo_cliente ,prestamo_inicial_maximo, porcentaje_semanal, semanas_a_prestar, garantias_por_monto,
                     fechas_pago, cantidad_para_renovar, semana_extra
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
                        item.PrestamoInicialMaximo = float.Parse(ds.Tables[0].Rows[i]["prestamo_inicial_maximo"].ToString());
                        item.PorcentajeSemanal = float.Parse(ds.Tables[0].Rows[i]["porcentaje_semanal"].ToString());
                        item.SemanasAPrestar = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        item.GarantiasPorMonto = float.Parse(ds.Tables[0].Rows[i]["garantias_por_monto"].ToString());
                        item.FechasDePago = (ds.Tables[0].Rows[i]["fechas_pago"].ToString());
                        item.CantidadParaRenovar = float.Parse(ds.Tables[0].Rows[i]["cantidad_para_renovar"].ToString());
                        item.SemanasExtra = int.Parse(ds.Tables[0].Rows[i]["semana_extra"].ToString());

                        item.ActivoSemanaExtra = (item.SemanasExtra == 1) ? "<span class='fa fa-check' aria-hidden='true'></span>" : "";


                        string botones = "<button  onclick='tipoCliente.editar(" + item.IdTipoCliente + ")'  class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Editar</button>";
                        botones += "&nbsp; <button  onclick='tipoCliente.eliminar(" + item.IdTipoCliente + ")'   class='btn btn-outline-primary btn-sm'> <span class='fa fa-remove mr-1'></span>Eliminar</button>";

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
        public static object Guardar(string path, TipoCliente item, string accion, string idUsuario)
        {

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = "";
                if (accion == "nuevo")
                {
                    sql = @" INSERT INTO tipo_cliente(tipo_cliente, prestamo_inicial_maximo, porcentaje_semanal, 
                    semanas_a_prestar, garantias_por_monto, fechas_pago, cantidad_para_renovar, semana_extra, eliminado) 
                    VALUES (@tipo_cliente, @prestamo_inicial_maximo,@porcentaje_semanal,@semanas_a_prestar,@garantias_por_monto,@fechas_pago
                    ,@cantidad_para_renovar,@semanas_extra, 0) ";
                }
                else
                {
                    sql = " UPDATE tipo_cliente " +
                          " SET tipo_cliente = @tipo_cliente, " +
                          "     prestamo_inicial_maximo = @prestamo_inicial_maximo, " +
                          "     porcentaje_semanal = @porcentaje_semanal,  " +
                          "     semanas_a_prestar = @semanas_a_prestar,  " +
                          "     garantias_por_monto = @garantias_por_monto,  " +
                          "     fechas_pago = @fechas_pago,  " +
                          "     cantidad_para_renovar = @cantidad_para_renovar,  " +
                          "     semana_extra = @semanas_extra  " +
                          " WHERE id_tipo_cliente = @id ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@tipo_cliente", item.NombreTipoCliente);
                cmd.Parameters.AddWithValue("@prestamo_inicial_maximo", item.PrestamoInicialMaximo);
                cmd.Parameters.AddWithValue("@porcentaje_semanal", item.PorcentajeSemanal);
                cmd.Parameters.AddWithValue("@semanas_a_prestar", item.SemanasAPrestar);
                cmd.Parameters.AddWithValue("@garantias_por_monto", item.GarantiasPorMonto);

                cmd.Parameters.AddWithValue("@fechas_pago", item.FechasDePago);

                cmd.Parameters.AddWithValue("@cantidad_para_renovar", item.CantidadParaRenovar);

                cmd.Parameters.AddWithValue("@semanas_extra", item.SemanasExtra);

                cmd.Parameters.AddWithValue("@id", item.IdTipoCliente);


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
        public static DatosSalida Eliminar(string path, string id, string idUsuario)
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

                string sql = @" UPDATE tipo_cliente SET eliminado = 1  
                                        WHERE id_tipo_cliente = @id ";

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

        [WebMethod]
        public static TipoCliente GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            TipoCliente item = new TipoCliente();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_cliente as id, tipo_cliente, prestamo_inicial_maximo, porcentaje_semanal, semanas_a_prestar, garantias_por_monto,
                     fechas_pago, cantidad_para_renovar, semana_extra
                     FROM tipo_cliente
                     WHERE id_tipo_cliente =  @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_puesto =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new TipoCliente();

                        item.IdTipoCliente = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.NombreTipoCliente = (ds.Tables[0].Rows[i]["tipo_cliente"].ToString());
                        item.PrestamoInicialMaximo = float.Parse(ds.Tables[0].Rows[i]["prestamo_inicial_maximo"].ToString());
                        item.PorcentajeSemanal = float.Parse(ds.Tables[0].Rows[i]["porcentaje_semanal"].ToString());
                        item.SemanasAPrestar = int.Parse(ds.Tables[0].Rows[i]["semanas_a_prestar"].ToString());
                        item.GarantiasPorMonto = float.Parse(ds.Tables[0].Rows[i]["garantias_por_monto"].ToString());
                        item.FechasDePago = (ds.Tables[0].Rows[i]["fechas_pago"].ToString());
                        item.CantidadParaRenovar = float.Parse(ds.Tables[0].Rows[i]["cantidad_para_renovar"].ToString());
                        item.SemanasExtra = int.Parse(ds.Tables[0].Rows[i]["semana_extra"].ToString());


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