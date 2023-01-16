using Plataforma.Clases;
using Plataforma.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace Plataforma.pages
{
    public partial class Investments : System.Web.UI.Page
    {
        const string pagina = "52";

        public const int TIPO_MOVIMIENTO_INVERSION = 1;
        public const int TIPO_MOVIMIENTO_RETIRO = 2;
        public const int TIPO_MOVIMIENTO_UTILIDAD = 3;

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

        /// <summary>
        /// Mapea el objeto inversion
        /// </summary>
        /// <param name="prw_Inversion"></param>
        /// <returns></returns>
        static Inversion MapInversion(DataRow prw_Inversion)
        {
            var lo_Inversion = new Inversion
            {
                id_inversion = prw_Inversion["id_inversion"].ToString().ParseStringToInt(),
                id_inversionista = prw_Inversion["id_inversionista"].ToString().ParseStringToInt(),
                inversion_utilidad = prw_Inversion["inversion_utilidad"].ToString().ParseStringToFloat(),
                fecha = prw_Inversion["fecha"].ToString().ParseStringToDateTime(),
                monto = prw_Inversion["monto"].ToString().ParseStringToFloat(),
                plazo = prw_Inversion["plazo"].ToString().ParseStringToInt(),
                comprobante = prw_Inversion["comprobante"].ToString(),
                porcentaje_utilidad = prw_Inversion["porcentaje_utilidad"].ToString().ParseStringToFloat(),
                montoRetiro = prw_Inversion["monto_retiro"].ToString().ParseStringToFloat(),
                fechaRetiro = prw_Inversion["fecha_retiro"] == null
                ? prw_Inversion["fecha"].ToString().ParseStringToDateTime().AddDays(prw_Inversion["plazo"].ToString().ParseStringToInt())
                : prw_Inversion["fecha_retiro"].ToString().ParseStringToDateTime(),
                utilidad_pesos = prw_Inversion["utilidad_pesos"].ToString().ParseStringToFloat(),
                Estatus = new StatusInversion
                {
                    color = prw_Inversion["EstatusColor"].ToString(),
                    id_status_inversion = prw_Inversion["id_status_inversion"].ToString().ParseStringToInt(),
                    nombre = prw_Inversion["EstatusNombre"].ToString(),
                },
                Inversionista = new Inversionista
                {
                    Nombre = prw_Inversion["NombreInversionista"].ToString()
                }
            };

            lo_Inversion.fechaRetiro = lo_Inversion.fecha.AddDays(lo_Inversion.plazo);

            return lo_Inversion;
        }

        /// <summary>
        /// Busqueda de inversione spor filtros
        /// </summary>
        /// <param name="oRequest"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Inversion> Search(string path, InversionRequest oRequest)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var llst_Inversiones = new List<Inversion>();

            try
            {
                using (var conn = new SqlConnection(strConexion))
                {
                    conn.Open();
                    var ds = new DataSet();
                    string query = @" SELECT i.*,inv.nombre NombreInversionista ,s.id_status_inversion, s.nombre EstatusNombre, s.color EstatusColor
                        FROM inversion i  
                        INNER JOIN inversionista inv ON (inv.id_inversionista = i.id_inversionista)         
                        INNER JOIN status_inversion s ON s.id_status_inversion = i.id_status_inversion 
                        WHERE 
                        ISNull(i.eliminado, 0) = 0";

                    if (!string.IsNullOrWhiteSpace(oRequest.NombreInversionista))
                    {
                        query += $@" AND inv.nombre like '%{oRequest.NombreInversionista}%'";
                    }
                    if (oRequest.Estatus.HasValue)
                    {
                        query += $@" AND id_status_inversion = '{oRequest.Estatus.Value}'";
                    }
                    if (oRequest.MontoMinimo.HasValue)
                    {
                        query += $@" AND monto >= '{oRequest.MontoMinimo.Value}'";
                    }
                    if (oRequest.MontoMaximo.HasValue)
                    {
                        query += $@" AND monto <= '{oRequest.MontoMaximo.Value}'";
                    }
                    if (oRequest.UtilidadMinimo.HasValue)
                    {
                        query += $@" AND porcentaje_utilidad >= '{oRequest.UtilidadMinimo.Value}'";
                    }
                    if (oRequest.UtilidadMaximo.HasValue)
                    {
                        query += $@" AND  porcentaje_utilidad <= '{oRequest.UtilidadMaximo.Value}'";
                    }
                    if (oRequest.PlazoMinimo.HasValue)
                    {
                        query += $@" AND plazo >= '{oRequest.PlazoMinimo.Value}'";
                    }
                    if (oRequest.PlazoMaximo.HasValue)
                    {
                        query += $@" AND  plazo <= '{oRequest.PlazoMaximo.Value}'";
                    }
                    if (oRequest.RetiroMinimo.HasValue)
                    {
                        query += $@" AND Convert(Date,DATEADD(D, plazo,fecha)) >= '{oRequest.RetiroMinimo.Value.ToString("yyyy/MM/dd")}'";
                    }
                    if (oRequest.RetiroMaximo.HasValue)
                    {
                        query += $@" AND  Convert(Date,DATEADD(D, plazo,fecha)) <= '{oRequest.RetiroMaximo.Value.ToString("yyyy/MM/dd")}'";
                    }
                    if (oRequest.IngresoMinimo.HasValue)
                    {
                        query += $@" AND Convert(Date,fecha) >= '{oRequest.IngresoMinimo.Value.ToString("yyyy/MM/dd")}'";
                    }
                    if (oRequest.IngresoMaximo.HasValue)
                    {
                        query += $@" AND  Convert(Date,fecha) <= '{oRequest.IngresoMaximo.Value.ToString("yyyy/MM/dd")}'";
                    }

                    query += "ORDER BY id_inversionista DESC";
                    var adp = new SqlDataAdapter(query, conn);

                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                    adp.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            var lo_Inversionista = MapInversion(ds.Tables[0].Rows[i]);
                            var ls_botones = "<button  onclick='asset.edit(" + lo_Inversionista.id_inversion + ")'  class='btn btn-primary mr-1'><i class='fa fa-edit'></i></button>";
                            ls_botones += "<button  onclick='asset.delete(" + lo_Inversionista.id_inversion + ")'  class='btn btn-danger mr-1'><i class='fa fa-trash'></i></button>";
                            lo_Inversionista.Accion = ls_botones;
                            llst_Inversiones.Add(lo_Inversionista);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return llst_Inversiones;
        }

        /// <summary>
        /// Lisatdo de inversiones
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Inversion> GetListaItems(string path, string idUsuario)
        {
            var strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var conn = new SqlConnection(strConexion);
            var llst_Inversiones = new List<Inversion>();
            // verificar que tenga permisos para usar esta pagina
            var tienePermiso = Index.TienePermisoPagina(pagina, path, idUsuario);
            if (!tienePermiso)
            {
                return null;//No tiene permisos
            }

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                #region Codigo comentado
                //string query = @" SELECT i.id_inversion, i.id_inversionista, IsNull(i.monto, 0) monto, 
                //                         FORMAT(i.fecha, 'dd/MM/yyyy') fecha, i.utilidades,
                //                         inv.nombre, inv.porcentaje_interes_anual, 
                //                          (SELECT DATEPART(ISO_WEEK, GETDATE())) as numSemanaHoy,
                //                         (SELECT DATEPART(ISO_WEEK, i.fecha)) as numSemanaInversion,                                         
                //                         p.valor_periodo, IsNull(i.retiro_efectuado, 0) retiro_efectuado
                //                         FROM inversion i  
                //                         JOIN inversionista inv ON (inv.id_inversionista = i.id_inversionista)                      
                //                         JOIN periodo p ON (p.id_periodo = i.id_periodo)                      
                //                         WHERE 
                //                         ISNull(i.eliminado, 0) = 0
                //                         ORDER BY id_inversionista 
                //                ";
                #endregion

                string query = @" SELECT i.*,inv.nombre NombreInversionista ,s.id_status_inversion, s.nombre EstatusNombre, s.color EstatusColor
                                         FROM inversion i  
                                         INNER JOIN inversionista inv ON (inv.id_inversionista = i.id_inversionista)         
                                          INNER JOIN status_inversion s ON s.id_status_inversion = i.id_status_inversion
                                         WHERE 
                                         ISNull(i.eliminado, 0) = 0
                                         ORDER BY id_inversionista";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        var lo_Inversionista  = MapInversion(ds.Tables[0].Rows[i]);
                        var ls_botones = "<button  onclick='asset.edit(" + lo_Inversionista.id_inversion + ")'  class='btn btn-primary mr-1'><i class='fa fa-edit'></i></button>";
                        ls_botones += "<button  onclick='asset.delete(" + lo_Inversionista.id_inversion + ")'  class='btn btn-danger mr-1'><i class='fa fa-trash'></i></button>";
                        lo_Inversionista.Accion = ls_botones;
                        llst_Inversiones.Add(lo_Inversionista);
                        #region Codigo anterior
                        //Inversion item = new Inversion();
                        //item.Inversionista = new Inversionista();
                        //item.Inversionista.Nombre = (ds.Tables[0].Rows[i]["nombre"].ToString());
                        ////item.Inversionista.PorcentajeInteresAnual = float.Parse(ds.Tables[0].Rows[i]["porcentaje_interes_anual"].ToString());
                        //item.Inversionista.PorcentajeUtilidadSugerida = float.Parse(ds.Tables[0].Rows[i]["porcentaje_interes_anual"].ToString());
                        //item.IdInversion = int.Parse(ds.Tables[0].Rows[i]["id_inversion"].ToString());
                        //item.Utilidades = int.Parse(ds.Tables[0].Rows[i]["utilidades"].ToString());
                        //item.NumSemanaHoy = int.Parse(ds.Tables[0].Rows[i]["numSemanaHoy"].ToString());
                        //item.NumSemanaInversion = int.Parse(ds.Tables[0].Rows[i]["numSemanaInversion"].ToString());
                        //item.PeriodoSemanas = int.Parse(ds.Tables[0].Rows[i]["valor_periodo"].ToString());
                        //item.RetiroEfectuado = int.Parse(ds.Tables[0].Rows[i]["retiro_efectuado"].ToString());
                        //item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        //item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        //item.MontoMx = item.Monto.ToString("C2");
                        //item.Fecha = (ds.Tables[0].Rows[i]["fecha"].ToString());
                        ////var interes = (item.Inversionista.PorcentajeInteresAnual / 52) * item.PeriodoSemanas; //     52 semanas del año
                        //var interes = (item.Inversionista.PorcentajeUtilidadSugerida / 52) * item.PeriodoSemanas; //     52 semanas del año
                        //var montoRetiro = item.Monto + (interes * item.Monto / 100);
                        //string botones = "<button onclick='retirement.addRetiro(" + item.IdInversionista + "," + item.IdInversion + "," + montoRetiro + ")' class='btn btn-outline-primary btn-sm'> <span class='fa fa-edit mr-1'></span>Retiro</button>";

                        //if (item.Utilidades == 1 && item.RetiroEfectuado == 0)
                        //{
                        //    if ((item.NumSemanaHoy - item.NumSemanaInversion) > item.PeriodoSemanas)
                        //    {
                        //        item.Accion = botones;
                        //    }
                        //}
                        #endregion
                    }
                }

                return llst_Inversiones;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return llst_Inversiones;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene los datos de la inversion
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public static Inversionista GetDataInvestor(string path, string id)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var item = new Inversionista();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT i.id_inversionista, i.nombre, i.razon_social, i.rfc, i.porcentaje_utilidad_sugerida
                         FROM inversionista i                         
                         WHERE 
                         i.id_inversionista = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_comision =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Inversionista();

                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        item.RazonSocial = ds.Tables[0].Rows[i]["razon_social"].ToString();
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.RFC = ds.Tables[0].Rows[i]["rfc"].ToString();
                        item.PorcentajeUtilidadSugerida = float.Parse(ds.Tables[0].Rows[i]["porcentaje_utilidad_sugerida"].ToString());
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

        /// <summary>
        /// Guarda la inversion
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="accion"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [WebMethod]
        public static DatosSalida Save(string path, Inversion item, string accion, string idUsuario)
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
            SqlTransaction transaction = null;

            int r = 0;
            try
            {
                conn.Open();
                transaction = conn.BeginTransaction();
                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name);

                string sql = string.Empty;

                if (accion == "nuevo")
                {
                    sql = @" INSERT INTO inversion
                    OUTPUT INSERTED.id_inversion
                    VALUES (@id_inversionista, @id_status_inversion, @fecha, @monto, @porcentaje_utilidad,@utilidad_pesos,@plazo,@inversion_utilidad,null,null ,@comprobante,0) ";

                }
                else
                {
                    sql = @" UPDATE inversion
                    SET monto = @monto,
                           porcentaje_utilidad =  @porcentaje_utilidad ,
                           utilidad_pesos = @utilidad_pesos , 
                           plazo = @plazo  ,
                           inversion_utilidad = @inversion_utilidad
                    WHERE id_inversion = @id_inversion";
                }
                Utils.Log(sql + "\n");

                int idGenerado = 0;
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_inversion", item.id_inversion);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.id_inversionista);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@monto", item.monto);
                    cmd.Parameters.AddWithValue("@porcentaje_utilidad", item.porcentaje_utilidad);
                    cmd.Parameters.AddWithValue("@utilidad_pesos", item.utilidad_pesos);
                    cmd.Parameters.AddWithValue("@plazo", item.plazo);
                    cmd.Parameters.AddWithValue("@inversion_utilidad", item.inversion_utilidad);
                   
                    if(string.IsNullOrWhiteSpace(item.comprobante))
                        cmd.Parameters.AddWithValue("@comprobante",DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@comprobante", item.comprobante);

                    cmd.Transaction = transaction;

                    if (accion == "nuevo")
                    {
                        cmd.Parameters.AddWithValue("@id_status_inversion", (int)EStatusInversion.Vigente);
                        idGenerado  = (int)cmd.ExecuteScalar();
                    }
                    else
                    {
                        idGenerado = item.id_inversion;
                        cmd.ExecuteNonQuery();
                    }
                }

                #region Codigo anterior
                //float nuevoMontoTotalActual = GetNuevoMontoInversionTotalActual(item.IdInversionista.ToString(), item.Monto, transaction, conn, TIPO_MOVIMIENTO_INVERSION);
                //sql = @" INSERT INTO inversion_movimiento
                //        (fecha, id_inversion_total, id_tipo_movimiento_inversion, monto, id_usuario, balance) 
                //        SELECT @fecha, (SELECT id_inversion_total total FROM inversion_total WHERE id_inversionista = @id_inversionista) AS id_inversion_total,
                //               @id_tipo_movimiento_inversion, @monto, @id_usuario, @balance ";

                //Utils.Log(sql + "\n");

                //using (SqlCommand cmd = new SqlCommand(sql, conn))
                //{
                //    cmd.CommandType = CommandType.Text;
                //    cmd.Parameters.AddWithValue("@monto", item.Monto);
                //    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                //    cmd.Parameters.AddWithValue("@utilidades", item.Utilidades);
                //    cmd.Parameters.AddWithValue("@balance", nuevoMontoTotalActual);
                //    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                //    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_INVERSION);
                //    cmd.Parameters.AddWithValue("@id_periodo", item.IdPeriodo);
                //    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                //    cmd.Transaction = transaction;
                //    idGenerado = cmd.ExecuteNonQuery();
                //}

                //sql = @" UPDATE inversion_total 
                //            SET fecha_modificacion = @fecha_modificacion,
                //            monto_total = @nuevoMontoTotalActual
                //         WHERE id_inversionista = @id_inversionista
                //        ";
                //Utils.Log(sql + "\n");

                //using (SqlCommand cmd = new SqlCommand(sql, conn))
                //{
                //    cmd.CommandType = CommandType.Text;
                //    cmd.Parameters.AddWithValue("@nuevoMontoTotalActual", nuevoMontoTotalActual);
                //    cmd.Parameters.AddWithValue("@fecha_modificacion", fechaOperacion);
                //    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                //    cmd.Transaction = transaction;
                //    cmd.ExecuteNonQuery();
                //}
                #endregion

                Utils.Log("Guardado -> OK ");

                transaction.Commit();

                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idGenerado.ToString();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
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

        /// <summary>
        /// Obtiene la informaciond el ainversion
        /// </summary>
        /// <param name="idInversion"></param>
        /// <param name="transaction"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static Inversion GetInversion(string idInversion, SqlTransaction transaction, SqlConnection conn)
        {
            Inversion item = new Inversion();

            try
            {
                string query = @" SELECT *
                                         FROM inversion i                      
                                         WHERE  i.id_inversion = @id_inversion
                                ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_inversion", idInversion);
                adp.SelectCommand.Transaction = transaction;


                DataSet ds = new DataSet();
                adp.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = MapInversion(ds.Tables[0].Rows[i]);
                        #region Codigo anterior
                        //item.Inversionista = new Inversionista();
                        //item.IdInversion = int.Parse(ds.Tables[0].Rows[i]["id_inversion"].ToString());
                        //item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
                        //item.Monto = float.Parse(ds.Tables[0].Rows[i]["monto"].ToString());
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item;
        }

        /// <summary>
        /// Borrado logico de la inversion
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
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
                string sql = @" UPDATE inversion SET eliminado = 1  
                                        WHERE id_inversion = @id ";

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

        /// <summary>
        /// Obtiene la lista de inversores activos
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [WebMethod]
        public static List<Inversionista> GetListaItemsInvestors(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Inversionista> items = new List<Inversionista>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_inversionista, nombre FROM  inversionista WHERE ISNull(eliminado, 0) = 0";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Inversionista item = new Inversionista();
                        item.IdInversionista = int.Parse(ds.Tables[0].Rows[i]["id_inversionista"].ToString());
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

        /// <summary>
        /// Obtiene el detalle de la inversion
        /// </summary>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public static Inversion GetItem(string path, string id)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            var item = new Inversion();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT i.*,inv.nombre NombreInversionista ,s.id_status_inversion, s.nombre EstatusNombre, s.color EstatusColor
                         FROM inversion i           
                         INNER JOIN inversionista inv ON (inv.id_inversionista = i.id_inversionista)         
                         INNER JOIN status_inversion s ON s.id_status_inversion = i.id_status_inversion
                         WHERE 
                         i.id_inversion = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("id_inversion =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);
                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                        item = MapInversion(ds.Tables[0].Rows[0]);
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
        public static List<Periodo> GetListaItemsPeriodos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Periodo> items = new List<Periodo>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_periodo,  IsNull(valor_periodo, 0) valor_periodo, activo
                     FROM periodo
                     WHERE 
                     ISNull(eliminado, 0) = 0
                     ORDER BY id_periodo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Periodo item = new Periodo();
                        item.IdPeriodo = int.Parse(ds.Tables[0].Rows[i]["id_periodo"].ToString());
                        item.ValorPeriodo = int.Parse(ds.Tables[0].Rows[i]["valor_periodo"].ToString());


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
        public static float GetNuevoMontoInversionTotalActual(string idInversionista, float monto, SqlTransaction transaction, SqlConnection conn, int tipoMovimiento)
        {
            try
            {
                string query = @" SELECT 
	                                it.id_inversion_total, it.monto_total, it.id_inversionista
                                FROM inversion_total it                                                      
                                WHERE it.id_inversionista = @id_inversionista ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_inversionista", idInversionista);
                adp.SelectCommand.Transaction = transaction;

                DataSet ds = new DataSet();
                adp.Fill(ds);
                float montoTotalActual = float.Parse(ds.Tables[0].Rows[0]["monto_total"].ToString());
                Utils.Log("montoTotalActual : " + montoTotalActual);

                float nuevoMontoTotalActual = 0;
                if (tipoMovimiento == TIPO_MOVIMIENTO_INVERSION)
                {
                    nuevoMontoTotalActual = montoTotalActual + monto;
                }

                if (tipoMovimiento == TIPO_MOVIMIENTO_RETIRO)
                {
                    nuevoMontoTotalActual = montoTotalActual - monto;
                }


                Utils.Log("nuevoMontoTotalActual : " + nuevoMontoTotalActual);

                return nuevoMontoTotalActual;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public static DatosSalida SaveRetiro(string path, InversionRetiro item, string accion, string idUsuario)
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

            var fechaOperacion = DateTime.Now;
            //var fechaOperacion = "2022-07-01";

            SqlTransaction transaction = null;

            int r = 0;
            try
            {

                conn.Open();
                transaction = conn.BeginTransaction();


                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name);

                Inversion inversion = GetInversion(item.IdInversion.ToString(), transaction, conn);

                //
                //float utilidad = item.Monto - inversion.Monto;

                Utils.Log("\n Retiro " + item.Monto);
                //Utils.Log("\n Utilidad " + utilidad);
                //Utils.Log("\n Inversion " + inversion.Monto);


                string sql = "";
                sql = @" INSERT INTO inversion_retiro(fecha, id_inversion, id_tipo_movimiento_inversion, monto, id_usuario) 
                    OUTPUT INSERTED.id_inversion_retiro
                    VALUES (@fecha, @id_inversion, @id_tipo_movimiento_inversion, @monto, @id_usuario) ";

                Utils.Log(sql + "\n");

                int idGenerado = 0;
                int idInversionRetiro = 0;
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    // cmd.Parameters.AddWithValue("@monto", inversion.Monto);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    cmd.Parameters.AddWithValue("@id_inversion", item.IdInversion);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_RETIRO);


                    cmd.Parameters.AddWithValue("@id_usuario", item.IdUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = idInversionRetiro = (int)cmd.ExecuteScalar();

                }

                //
                //float nuevoMontoTotalActual = GetNuevoMontoInversionTotalActual(item.IdInversionista.ToString(), inversion.Monto, transaction, conn, TIPO_MOVIMIENTO_RETIRO);



                sql = @" INSERT INTO inversion_movimiento
                        (fecha, id_inversion_total, id_tipo_movimiento_inversion, monto, id_usuario, balance) 
                        SELECT @fecha, (SELECT id_inversion_total total FROM inversion_total WHERE id_inversionista = @id_inversionista) AS id_inversion_total,
                               @id_tipo_movimiento_inversion, @monto, @id_usuario, @balance ";

                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    //cmd.Parameters.AddWithValue("@monto", inversion.Monto);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    //cmd.Parameters.AddWithValue("@balance", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_RETIRO);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = cmd.ExecuteNonQuery();
                }


                // utilidad
                sql = @" INSERT INTO inversion_movimiento
                        (fecha, id_inversion_total, id_tipo_movimiento_inversion, monto, id_usuario, balance) 
                        SELECT @fecha, (SELECT id_inversion_total total FROM inversion_total WHERE id_inversionista = @id_inversionista) AS id_inversion_total,
                               @id_tipo_movimiento_inversion, @monto, @id_usuario, @balance ";

                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    // cmd.Parameters.AddWithValue("@monto", utilidad);
                    cmd.Parameters.AddWithValue("@fecha", fechaOperacion);
                    //cmd.Parameters.AddWithValue("@balance", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Parameters.AddWithValue("@id_tipo_movimiento_inversion", TIPO_MOVIMIENTO_UTILIDAD);
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmd.Transaction = transaction;
                    idGenerado = cmd.ExecuteNonQuery();
                }


                //
                sql = @" UPDATE inversion_total 
                            SET fecha_modificacion = @fecha_modificacion,
                            monto_total = @nuevoMontoTotalActual
                         WHERE id_inversionista = @id_inversionista
                        ";
                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    //  cmd.Parameters.AddWithValue("@nuevoMontoTotalActual", nuevoMontoTotalActual);
                    cmd.Parameters.AddWithValue("@fecha_modificacion", fechaOperacion);
                    cmd.Parameters.AddWithValue("@id_inversionista", item.IdInversionista);
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }

                // Marcar que la inversion ya se ha retirado mediante un retiro
                sql = @" UPDATE inversion
                            SET retiro_efectuado = 1
                         WHERE id_inversion = @id_inversion
                        ";
                Utils.Log(sql + "\n");

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_inversion", item.IdInversion);
                    cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                }
                //

                Utils.Log("Guardado -> OK ");

                transaction.Commit();

                salida.MensajeError = "Guardado correctamente";
                salida.CodigoError = 0;
                salida.IdItem = idInversionRetiro.ToString();


            }
            catch (Exception ex)
            {

                transaction.Rollback();

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
    }
}