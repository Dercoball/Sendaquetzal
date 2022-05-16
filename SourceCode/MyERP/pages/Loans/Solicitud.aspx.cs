using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;

namespace Plataforma.pages
{
    public partial class Solicitud : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {



        }

        [WebMethod]
        public static List<SolicitudCombustible> GetListaItems(string path, string idUsuario, string idTipoUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<SolicitudCombustible> items = new List<SolicitudCombustible>();

            bool esSuperUser = idTipoUsuario == Usuario.TIPO_USUARIO_SUPER_ADMIN.ToString();
            string sqlTipoUsuario = esSuperUser ? "" : " AND id_usuario_solicita = @id_usuario_solicita ";

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT s.id_solicitud_combustible, s.fecha_creacion, s.hora_creacion, 
                                    s.id_proveedor_combustible, s.id_equipo, s.cantidad, 
                                    s.id_nivel_prioridad, s.id_usuario_solicita, s.status,
                                    s.id_usuario_entrega,
                                    u.nombre nombre_usuario_solicita
                                    FROM solicitud_combustible s                                     
                                    left JOIN usuario u ON (u.id_usuario = s.id_usuario_solicita)
                                        WHERE 
                                       IsNull(s.eliminado, 0) <> 1
                                       " + sqlTipoUsuario + @" 
                                       AND IsNull(s.status, 0) = 1
                                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_usuario_solicita", idUsuario);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SolicitudCombustible item = new SolicitudCombustible();

                        item.IdSolicitud = int.Parse(ds.Tables[0].Rows[i]["id_solicitud_combustible"].ToString());

                        var Fecha = DateTime.Parse(ds.Tables[0].Rows[i]["fecha_creacion"].ToString());
                        item.FechaFormateadaMx = Fecha.ToString("dd/MM/yyyy");
                        item.FechaCanonical = Fecha.ToString("yyyy-MM-dd");
                        item.NombreUsuarioSolicita = ds.Tables[0].Rows[i]["nombre_usuario_solicita"].ToString();

                        string botones = "";

                        //botones += " <button  title=\"Cancelar\" onclick='registroSolicitud.cancelar(" + item.IdSolicitud + ")'   class='btn btn-outline-primary btn-sm'> <span class='fa fa-times'></span> Cancelar</button>";
                        botones += " <button  title=\"Abrir\" onclick='registroSolicitud.editarDetalle(" + item.IdSolicitud + ")'   class='btn btn-outline-primary btn-sm'> <span class='fa fa-open'></span> Abrir</button>";


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
        public static DatosSalida Guardar(string path, List<SolicitudCombustible> data, string accion, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;

            Utils.Log("\nMétodo-> " +
                   System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");


            int r = 0;
            try
            {

                string sql = "";

                conn.Open();
                transaccion = conn.BeginTransaction();


                sql = @" INSERT INTO solicitud_combustible
                            (fecha_creacion, id_usuario_solicita,  status)
                                OUTPUT INSERTED.id_solicitud_combustible
                            VALUES(@fecha_creacion, @id_usuario_solicita, @status);
                          ";


                Utils.Log("\n" + sql + "\n");

                SqlCommand cmdSolicitud = new SqlCommand(sql, conn);
                cmdSolicitud.CommandType = CommandType.Text;
                cmdSolicitud.Transaction = transaccion;

                cmdSolicitud.Parameters.AddWithValue("@fecha_creacion", DateTime.Now);
                cmdSolicitud.Parameters.AddWithValue("@id_usuario_solicita", idUsuario);
                cmdSolicitud.Parameters.AddWithValue("@id_obra", DBNull.Value);
                cmdSolicitud.Parameters.AddWithValue("@status", 1); //Creado

                int idSolicitud = (int)cmdSolicitud.ExecuteScalar();


                if (accion == "guardar")
                {


                    foreach (var item in data)
                    {

                        //Validar que no haya una solicitud en proceso para este equipo
                        DataSet ds = new DataSet();

                        string query = @" SELECT DISTINCT e.numero_economico                              
                                        FROM detalle_solicitud_combustible d                                                                         
                                        JOIN solicitud_combustible s ON (d.id_solicitud_combustible = s.id_solicitud_combustible)
                                        JOIN equipo e ON (e.id_equipo = d.id_equipo)
                                        WHERE 
                                        IsNull(s.eliminado, 0) <> 1        
                                        AND IsNull(d.status, 0) <> 4
                                        AND IsNull(s.status, 0) <> 4
                                        AND IsNull(d.cancelado, 0) <> 1
                                        AND d.id_equipo = @id_equipo
                                    ";

                        //  status, 0) <> 4 = finalizado

                        SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                        adp.SelectCommand.Parameters.AddWithValue("id_equipo", item.IdEquipo);
                        adp.SelectCommand.Transaction = transaccion;
                        Utils.Log("\nMétodo-> " +
                        System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                        adp.Fill(ds);
                        List<string> equipoList = new List<string>();

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                SolicitudCombustible itemDetalleSol = new SolicitudCombustible();

                                equipoList.Add(ds.Tables[0].Rows[i]["numero_economico"].ToString());


                            }

                            string equipos = String.Join(", ", equipoList);

                            salida.MensajeError = "Los siguientes equipos tiene solicitudes en proceso: " + equipos + " Deben ser finalizadas para poder continuar.";
                            salida.CodigoError = 1;

                            try
                            {
                                transaccion.Rollback();
                            }
                            catch (Exception excep)
                            {

                            }

                            return salida;


                        }




                        sql = @" INSERT INTO detalle_solicitud_combustible
                            (fecha_creacion, id_solicitud_combustible, id_tipo_combustible,
                                id_equipo, cantidad, id_usuario_solicita, id_obra, status, orometro,
                                cintillo_anterior, cintillo_actual
                                )
                            VALUES(@fecha_creacion, @id_solicitud_combustible, @id_tipo_combustible,
                                @id_equipo, @cantidad, @id_usuario_solicita, @id_obra, @status, @orometro,
                                @cintillo_anterior, @cintillo_actual
                                );
                          ";



                        Utils.Log("\n" + sql + "\n");

                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.CommandType = CommandType.Text;
                        cmd.Transaction = transaccion;


                        cmd.Parameters.AddWithValue("@fecha_creacion", DateTime.Now);
                        cmd.Parameters.AddWithValue("@id_usuario_solicita", idUsuario);
                        cmd.Parameters.AddWithValue("@cantidad", item.Cantidad);
                        cmd.Parameters.AddWithValue("@id_equipo", item.IdEquipo);
                        cmd.Parameters.AddWithValue("@id_tipo_combustible", item.IdTipoCombustible);
                        cmd.Parameters.AddWithValue("@id_solicitud_combustible", idSolicitud);
                        cmd.Parameters.AddWithValue("@orometro", item.Orometro == null ? "" : item.Orometro);
                        cmd.Parameters.AddWithValue("@id_obra", item.IdObra);
                        cmd.Parameters.AddWithValue("@cintillo_anterior", item.CintilloAnterior);
                        cmd.Parameters.AddWithValue("@cintillo_actual", item.CintilloActual);
                        cmd.Parameters.AddWithValue("@status", 1); //Creado

                        r = cmd.ExecuteNonQuery();
                    }


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
        public static DatosSalida GuardarNuevaSolicitud(string path, string accion, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            DatosSalida salida = new DatosSalida();
            SqlTransaction transaccion = null;

            Utils.Log("\nMétodo-> " +
                   System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");

            int r = 0;
            try
            {

                string sql = "";

                conn.Open();

                sql = @" INSERT INTO solicitud_combustible
                            (fecha_creacion, id_usuario_solicita,  status)
                                OUTPUT INSERTED.id_solicitud_combustible
                            VALUES(@fecha_creacion, @id_usuario_solicita, @status);
                          ";



                Utils.Log("\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = transaccion;

                cmd.Parameters.AddWithValue("@fecha_creacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@id_usuario_solicita", idUsuario);
                cmd.Parameters.AddWithValue("@id_obra", DBNull.Value);
                cmd.Parameters.AddWithValue("@status", 1); //Creado

                r = (int)cmd.ExecuteScalar();

                Utils.Log("Guardado -> OK ");


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
        public static Requisicion GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Requisicion item = new Requisicion();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT r.id_requisicion, r.fecha_creacion, r.descripcion, r.diagnostico, IsNull(r.id_equipo, 0) id_equipo, " +
                               " IsNull(r.id_proveedor, 0) id_proveedor, r.id_status_requisicion, IsNull(r.activa, 0) activa, r.lat, r.long, " +
                               " IsNull(r.detiene_operacion, 0) detiene_operacion, r.orometro,  " +
                               " e.nombre nombre_equipo,  IsNull(e.id_modelo, 0) id_modelo, e.anio, e.numero_economico, e.numero_serie, e.id_marca,  " +
                               " p.nombre nombre_proveedor, " +
                               " st.nombre nombre_status_requisicion, " +
                               " IsNull(r.eliminado, 0) as eliminado" +
                               " FROM requisicion r " +
                               " left join equipo e on (e.id_equipo = r.id_equipo) " +
                               " left join proveedor p on (r.id_proveedor = p.id_proveedor) " +
                               " join status_requisicion st on (st.id_status_requisicion = r.id_status_requisicion) " +
                               " WHERE r.id_requisicion = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + id);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", id);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item = new Requisicion();

                        item.FechaCreacion = DateTime.Parse(ds.Tables[0].Rows[i]["fecha_creacion"].ToString());
                        item.FechaCreacionFormateadaMx = item.FechaCreacion.ToString("dd/MM/yyyy");
                        item.FechaCreacionCanonical = item.FechaCreacion.ToString("yyyy-MM-dd");
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.NombreEquipo = ds.Tables[0].Rows[i]["nombre_equipo"].ToString();
                        item.NumeroEconomico = ds.Tables[0].Rows[i]["numero_economico"].ToString();
                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.Orometro = ds.Tables[0].Rows[i]["orometro"].ToString();

                        item.NombreStatus = ds.Tables[0].Rows[i]["nombre_status_requisicion"].ToString();
                        item.Latitud = ds.Tables[0].Rows[i]["lat"].ToString();
                        item.Longitud = ds.Tables[0].Rows[i]["long"].ToString();

                        item.NombreProveedor = ds.Tables[0].Rows[i]["nombre_proveedor"].ToString();
                        item.Activa = int.Parse(ds.Tables[0].Rows[i]["activa"].ToString());
                        item.IdRequisicion = int.Parse(ds.Tables[0].Rows[i]["id_requisicion"].ToString());
                        item.DetieneOperacion = int.Parse(ds.Tables[0].Rows[i]["detiene_operacion"].ToString());

                        item.IdMarca = int.Parse(ds.Tables[0].Rows[i]["id_marca"].ToString());
                        item.IdModelo = int.Parse(ds.Tables[0].Rows[i]["id_modelo"].ToString());
                        item.IdProveedor = int.Parse(ds.Tables[0].Rows[i]["id_proveedor"].ToString());
                        item.IdEquipo = int.Parse(ds.Tables[0].Rows[i]["id_equipo"].ToString());

                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.Diagnostico = ds.Tables[0].Rows[i]["diagnostico"].ToString();



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



        /***
         * Trae el valor del cincho actual, que posterior se convertira en el anterior
         * 
         */
        [WebMethod]
        public static string GetCintilloActual(string path, string idEquipo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Requisicion item = new Requisicion();
            SqlConnection conn = new SqlConnection(strConexion);

            string cinchoActual = "0";
            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @"  
                                SELECT TOP 1 id_equipo, cintillo_actual  
                                FROM detalle_solicitud_combustible where id_equipo  = @id_equipo
                                order by fecha_hora_entrega DESC   
                            ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + idEquipo);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_equipo", idEquipo);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    cinchoActual = ds.Tables[0].Rows[0]["cintillo_actual"].ToString();

                }





                return cinchoActual;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);

                return cinchoActual;
            }

            finally
            {
                conn.Close();
            }

        }




        public class Data
        {
            public string Table;
            public SolicitudCombustible solicitud;
            public List<Equipo> Array = new List<Equipo>();
        }



        [WebMethod]
        public static Data AbrirSolicitud(string path, string idSolicitud)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<SolicitudCombustible> items = new List<SolicitudCombustible>();

            string table = "";
            Data data = new Data();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT s.id_solicitud_combustible,d.status, 
                                    IsNull(d.cantidad, 0) cantidad, d.orometro, IsNull(d.id_equipo, 0) id_equipo,d.id_detalle_solicitud_combustible, 
                                    e.numero_economico, e.nombre, IsNull(d.id_obra, 0) id_obra,
                                    d.cintillo_anterior, d.cintillo_actual, c.nombre nombre_centro_costo, 
                                    d.id_tipo_combustible, c.clave clave_centro_costo,
                                    tc.nombre nombre_tipo_combustible,
                                    st.nombre nombre_status,
                                    ts.id_tipo_solicitud_combustible,
                                    ts.nombre nombre_tipo_solicitud_combustible
                                    FROM solicitud_combustible s                                     
                                    left JOIN detalle_solicitud_combustible d ON (d.id_solicitud_combustible = s.id_solicitud_combustible)
                                    left JOIN equipo e ON (d.id_equipo = e.id_equipo)
                                    left JOIN centros_costo c ON (c.id = d.id_obra)
                                    left JOIN tipo_combustible tc ON (tc.id_tipo_combustible = d.id_tipo_combustible)
                                    left JOIN status_solicitud_combustible st ON (st.id = d.status)
                                    left JOIN tipo_solicitud_combustible ts ON (ts.id_tipo_solicitud_combustible = e.id_tipo_solicitud_combustible)
                                        WHERE s.id_solicitud_combustible = @id_solicitud_combustible 
                                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("id_solicitud_combustible", idSolicitud);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                int idStatus = 0;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SolicitudCombustible item = new SolicitudCombustible();

                        item.IdSolicitud = int.Parse(ds.Tables[0].Rows[i]["id_solicitud_combustible"].ToString());
                        idStatus = int.Parse(ds.Tables[0].Rows[i]["status"].ToString());


                        item.Orometro = ds.Tables[0].Rows[i]["orometro"].ToString();
                        item.NombreStatus = ds.Tables[0].Rows[i]["nombre_status"].ToString();
                        item.NumeroEconomico = ds.Tables[0].Rows[i]["numero_economico"].ToString();
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.CintilloAnterior = ds.Tables[0].Rows[i]["cintillo_anterior"].ToString();
                        item.CintilloActual = ds.Tables[0].Rows[i]["cintillo_actual"].ToString();
                        item.Cantidad = float.Parse(ds.Tables[0].Rows[i]["cantidad"].ToString());
                        item.IdEquipo = int.Parse(ds.Tables[0].Rows[i]["id_equipo"].ToString());
                        item.IdObra = int.Parse(ds.Tables[0].Rows[i]["id_obra"].ToString());
                        item.IdTipoCombustible = int.Parse(ds.Tables[0].Rows[i]["id_tipo_combustible"].ToString());
                        item.NombreTipoCombustible = ds.Tables[0].Rows[i]["nombre_tipo_combustible"].ToString();
                        item.NombreTipoSolicitudCombustible = ds.Tables[0].Rows[i]["nombre_tipo_solicitud_combustible"].ToString();


                        item.NombreCentroCosto = ds.Tables[0].Rows[i]["clave_centro_costo"].ToString() + " " + ds.Tables[0].Rows[i]["nombre_centro_costo"].ToString();
                        item.IdTipoSolicitudCombustible = int.Parse(ds.Tables[0].Rows[i]["id_tipo_solicitud_combustible"].ToString());
                        item.IdDetalleSolicitud = int.Parse(ds.Tables[0].Rows[i]["id_detalle_solicitud_combustible"].ToString());


                        string disabled = " disabled ";

                        table += "<tr>";
                        table += "<td>" + item.NumeroEconomico + " " + item.Nombre + "</td>";
                        table += "<td><input type='text' " + disabled + " value='" + item.Orometro + "' class='form-control-sm orometro sm100'  id='txtOrometro" + i + "'></td>";//orometro
                        table += "<td><input type='number' " + disabled + "data-idequipo='" + item.IdEquipo + "' data-idobra='" + item.IdObra + "' value='" + item.Cantidad + "' class='form-control-sm litros sm100'  id='txtLitros" + i + "'></td>";//litros                        
                        table += "<td>" + item.NombreTipoCombustible + "</td>";//tipo combustible
                        table += "<td><input type='number' " + disabled + "value='" + item.CintilloAnterior + "' class='form-control-sm cintillo-anterior sm100'  id='txtCintilloAnterior" + i + "'></td>";//
                        table += "<td><input type='number' " + disabled + "value='" + item.CintilloActual + "' class='form-control-sm cintillo-actual sm100'  id='txtCintilloActual" + i + "'></td>";//
                        table += "<td>" + item.NombreCentroCosto + "</td>";//
                                                                           //table += "<td>" + item.NombreTipoSolicitudCombustible + "</td>";//

                        if (idStatus == 2)
                        {
                            table += "<td class='aprobado'>" + item.NombreStatus + "</td>";//
                        }
                        else
                        {
                            table += "<td class='creado'>" + item.NombreStatus + "</td>";//
                        }

                        table += "<td>";
                        if (item.IdTipoSolicitudCombustible == 2)
                        {

                            table += " <button  title=\"Imágenes\" data-idtiposolicitud=" + item.IdDetalleSolicitud +
                                "  onclick= 'registroSolicitud.subirImagenes(" + item.IdDetalleSolicitud + ", \"" + item.FechaSolicitudFormateadaMx + "\")'  class='btn btn-outline-primary btn-sm'> <span class='fa fa-image'></span> Imágenes</button>";

                        }
                        table += "</td>";



                        table += "</tr>";

                        data.Array.Add(item);


                    }
                }

                SolicitudCombustible solicitud = new SolicitudCombustible();
                solicitud.IdSolicitud = int.Parse(idSolicitud);
                solicitud.IdStatus = idStatus;

                data.Table = table;
                data.solicitud = solicitud;

                return data;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return data;
            }

            finally
            {
                conn.Close();
            }

        }






        [WebMethod]
        public static List<EquipoMini> GetListaEquipos(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<EquipoMini> items = new List<EquipoMini>();


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT e.id_equipo, e.nombre, e.descripcion, e.numero_economico, 
                                         t.nombre nombre_tipo_solicitud
                                FROM equipo e
                                JOIN tipo_solicitud_combustible t ON (e.id_tipo_solicitud_combustible = t.id_tipo_solicitud_combustible)
                                where isNull(e.eliminado, 0) =  0                                                               
                                ORDER BY e.numero_economico, e.id_equipo  ";

                //@" SELECT p.id_equipo, p.nombre, p.descripcion, p.numero_economico
                //                FROM equipo p 
                //                where isNull(p.eliminado, 0) =  0                                 
                //                AND NOT p.numero_economico LIKE '%PU%'
                //                AND NOT p.numero_economico LIKE '%TR%'
                //                AND NOT p.numero_economico LIKE '%EP%'
                //                AND NOT p.numero_economico LIKE '%SE%'


                //                ORDER BY p.id_equipo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        EquipoMini item = new EquipoMini();

                        item.IdEquipo = int.Parse(ds.Tables[0].Rows[i]["id_equipo"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NumeroEconomico = ds.Tables[0].Rows[i]["numero_economico"].ToString() + " " + item.Nombre;
                        item.NombreTipoSolicitud = ds.Tables[0].Rows[i]["nombre_tipo_solicitud"].ToString();

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
        public static List<CentroCosto> GetListaCentrosCosto(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<CentroCosto> items = new List<CentroCosto>();


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT c.id, c.nombre, c.descripcion_corta, c.clave
                                FROM centros_costo c 
                                where isNull(c.eliminado, 0) =  0                                 
                                ORDER BY c.clave ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CentroCosto item = new CentroCosto();

                        item.IdCentroCosto = int.Parse(ds.Tables[0].Rows[i]["id"].ToString());
                        item.Clave = ds.Tables[0].Rows[i]["clave"].ToString();
                        item.Nombre = item.Clave + " " + ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.DescripcionCorta = ds.Tables[0].Rows[i]["descripcion_corta"].ToString();

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
        public static List<TipoCombustible> GetTiposCombustible(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<TipoCombustible> items = new List<TipoCombustible>();


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT id_tipo_combustible, nombre
                                FROM tipo_combustible
                                where isNull(activo, 0) =  1  ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        TipoCombustible item = new TipoCombustible();

                        item.IdTipoCombustible = int.Parse(ds.Tables[0].Rows[i]["id_tipo_combustible"].ToString());
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
        public static string GetFoto(string path, string idDetalleSolicitud, string tipo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            string item = "";
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT contenido_b64 FROM documento_solicitud_combustible 
                                    WHERE id_detalle_solicitud_combustible = @id_detalle_solicitud_combustible 
                                    AND id_tipo_documento = @id_tipo_documento
                                ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + idDetalleSolicitud);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_detalle_solicitud_combustible", idDetalleSolicitud);
                adp.SelectCommand.Parameters.AddWithValue("@id_tipo_documento", tipo);

                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    item = ds.Tables[0].Rows[0]["contenido_b64"].ToString();
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