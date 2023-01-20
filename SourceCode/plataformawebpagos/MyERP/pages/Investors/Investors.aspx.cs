using Plataforma.Clases;
using Plataforma.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace Plataforma.pages
{
    public partial class Investors : System.Web.UI.Page
    {
        const string pagina = "51";

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

        static Inversionista MapInversionista(DataRow prw_Inversionista)
        {
            return new Inversionista
            {
                IdInversionista = prw_Inversionista["id_inversionista"].ToString().ParseStringToInt(),
                Nombre = prw_Inversionista["nombre"].ToString(),
                RFC = prw_Inversionista["rfc"].ToString(),
                RazonSocial = prw_Inversionista["razon_social"].ToString(),
                Status = prw_Inversionista["status"].ToString().ParseStringToBoolean(),
                PorcentajeUtilidadSugerida = prw_Inversionista["porcentaje_utilidad_sugerida"].ToString().ParseStringToFloat(),
                FechaRegistro = prw_Inversionista["fecha_registro"].ToString().ParseStringToDateTime(),
                Eliminado = prw_Inversionista["status"].ToString().ParseStringToBoolean(),
            };
        }

        /// <summary>
        /// BUsqueda de inversionistas
        /// </summary>
        /// <param name="oRequest"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Inversionista> SearchInvestor(InversionistaRequest oRequest, string path)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var llst_Inversionistas = new List<Inversionista>();
            
            try
            {
                using (var conn = new SqlConnection(strConexion))
                {
                    conn.Open();
                    var ds = new DataSet();
                    string query = @" SELECT *
                         FROM inversionista i                          
                         WHERE 
                         ISNull(i.eliminado, 0) = 0";

                    if (!string.IsNullOrWhiteSpace(oRequest.NombreBusqueda)) {
                        query += $@" AND nombre like '%{oRequest.NombreBusqueda}%'";
                    }

                    if (!string.IsNullOrWhiteSpace(oRequest.RFCBusqueda))
                    {
                        query += $@" AND rfc = '{oRequest.RFCBusqueda}'";
                    }

                    if (oRequest.UtilidadMinimaBusqueda.HasValue) {
                        query += $@" AND porcentaje_utilidad_sugerida >= '{oRequest.UtilidadMinimaBusqueda.Value}'";
                    }

                    if (oRequest.UtilidadMaximaBusqueda.HasValue)
                    {
                        query += $@" AND porcentaje_utilidad_sugerida <= '{oRequest.UtilidadMaximaBusqueda.Value}'";
                    }

                    if (oRequest.FechaRegistroMinimaBusqueda.HasValue)
                    {
                        query += $@" AND Convert(Date,fecha_registro) >= '{oRequest.FechaRegistroMinimaBusqueda.Value.ToString("yyyy/MM/dd")}'";
                    }

                    if (oRequest.FechaRegistroMaximaBusqueda.HasValue)
                    {
                        query += $@" AND  Convert(Date,fecha_registro) <= '{oRequest.FechaRegistroMaximaBusqueda.Value.ToString("yyyy/MM/dd")}'";
                    }


                    query += "ORDER BY id_inversionista";
                    var adp = new SqlDataAdapter(query, conn);

                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var lo_Inversionista = MapInversionista(ds.Tables[0].Rows[i]);
                            var ls_botones = "<button  onclick='investor.edit(" + lo_Inversionista.IdInversionista + ")'  class='btn btn-primary mr-1'><i class='fa fa-edit'></i></button>";
                            ls_botones += "<button  onclick='investor.delete(" + lo_Inversionista.IdInversionista + ")'  class='btn btn-danger mr-1'><i class='fa fa-trash'></i></button>";
                            ls_botones += "<button  onclick='investor.redirectInversiones(" + lo_Inversionista.IdInversionista + ")'  class='btn btn-success'><i class='fa fa-external-link'></i></button>";
                            lo_Inversionista.Accion = ls_botones;

                            llst_Inversionistas.Add(lo_Inversionista);
                        }
                    }
                }

            }
            catch (Exception)
            {

            }

            return llst_Inversionistas;
        }

        [WebMethod]
        public static List<Inversionista> GetListaItems(string path, string idUsuario)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);
            var llst_Inversionistas = new List<Inversionista>();

            // verificar que tenga permisos para usar esta pagina
            var tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            try
            {
                conn.Open();
                var ds = new DataSet();
                string query = @" SELECT *
                         FROM inversionista i                          
                         WHERE 
                         ISNull(i.eliminado, 0) = 0
                         ORDER BY id_inversionista ";

                var adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var lo_Inversionista = MapInversionista(ds.Tables[0].Rows[i]);
                        var ls_botones = "<button  onclick='investor.edit(" + lo_Inversionista.IdInversionista + ")'  class='btn btn-primary mr-1'><i class='fa fa-edit'></i></button>";
                        ls_botones += "<button  onclick='investor.delete(" + lo_Inversionista.IdInversionista + ")'  class='btn btn-danger mr-1'><i class='fa fa-trash'></i></button>";
                        ls_botones += "<button  onclick='investor.redirectInversiones(" + lo_Inversionista.IdInversionista + ")'  class='btn btn-success'><i class='fa fa-external-link'></i></button>";
                        lo_Inversionista.Accion = ls_botones;

                        llst_Inversionistas.Add(lo_Inversionista);
                    }
                }

                return llst_Inversionistas;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return llst_Inversionistas;
            }

            finally
            {
                conn.Close();
            }
        }

        [WebMethod]
        public static Inversionista GetItem(string path, string id)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Inversionista item = new Inversionista();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT *
                         FROM inversionista i                         
                         WHERE 
                         i.id_inversionista = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_inversionista =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);
                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = MapInversionista(ds.Tables[0].Rows[i]);
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
        public static object Save(string path, Inversionista item, string accion, string idUsuario)
        {
            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            using (var conn = new SqlConnection(strConexion))
            {
                SqlTransaction transaction = null;
                int r = 0;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    string sql = "";
                    if (accion == "nuevo")
                    {
                        sql = @" INSERT INTO inversionista
                             OUTPUT INSERTED.id_inversionista
                                VALUES 
                            (@nombre, @razon_social, @rfc, 0 ,@porcentaje_utilidad_sugerida,GETDATE(),NULL,1) ";
                    }
                    else
                    {
                        sql = @" UPDATE inversionista
                          SET nombre = @nombre,
                              razon_social = @razon_social,
                              rfc = @rfc,
                              porcentaje_utilidad_sugerida = @porcentaje_utilidad_sugerida,
                              fecha_edicion = GETDATE()
                          WHERE 
                              id_inversionista = @id";

                    }

                    Utils.Log("\nMétodo-> " +
                   System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");
                    int idGenerado = 0;
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@nombre", item.Nombre);
                        cmd.Parameters.AddWithValue("@porcentaje_utilidad_sugerida", item.PorcentajeUtilidadSugerida);
                        cmd.Parameters.AddWithValue("@razon_social", item.RazonSocial);
                        cmd.Parameters.AddWithValue("@rfc", item.RFC);
                        cmd.Parameters.AddWithValue("@id", item.IdInversionista);
                        cmd.Transaction = transaction;

                        if (accion == "nuevo")
                            idGenerado = (int)cmd.ExecuteScalar();
                        else
                        {
                            idGenerado = item.IdInversionista;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    #region Codigo comentado
                    //if (accion == "nuevo")
                    //{
                    //    sql = @" INSERT INTO inversion_total 
                    //            (fecha_creacion, monto_total, id_inversionista) 
                    //            VALUES 
                    //            (@fecha_creacion, @monto_total, @id_inversionista) ";

                    //    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    //    {
                    //        cmd.CommandType = CommandType.Text;

                    //        cmd.Parameters.AddWithValue("@monto_total", 0);
                    //        cmd.Parameters.AddWithValue("@fecha_creacion", DateTime.Now);
                    //        cmd.Parameters.AddWithValue("@id_inversionista", idGenerado);
                    //        cmd.Transaction = transaction;
                    //        r += cmd.ExecuteNonQuery();

                    //    }
                    //}
                    #endregion

                    transaction.Commit();
                    Utils.Log("Guardado -> OK ");
                    return idGenerado;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                    return -1;
                }
            }
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
                string sql = @" UPDATE inversionista SET eliminado = 1  
                                        WHERE id_inversionista = @id ";

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
        public static DatosSalida Suspender(string path, 
            string id, 
            string idUsuario, 
            bool status)
        {
            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;

            // verificar que tenga permisos para usar esta pagina
            bool tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo suspender el inversionista.";
                return salida;
            }

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            using (var conn = new SqlConnection(strConexion))
            {
                try
                {
                    conn.Open();
                    string sql = @" UPDATE inversionista SET status = @status  
                                        WHERE id_inversionista = @id ";

                    Utils.Log("\n-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@status", status);
                    
                    int r = cmd.ExecuteNonQuery();
                    Utils.Log("r = " + r);
                    Utils.Log("Suspendido -> OK ");
                    salida.MensajeError = null;
                    salida.CodigoError = 0;

                    return salida;
                }
                catch (Exception ex)
                {
                    salida.CodigoError = -1;
                    salida.MensajeError = "No se pudo suspender el inversionista.";

                    Utils.Log("Error ... " + ex.Message);
                    Utils.Log(ex.StackTrace);
                    return salida;
                }
            }
        }
    }
}