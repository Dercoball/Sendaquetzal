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
    public partial class PanelEquipos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {



        }

        public class FechaHora
        {
            public string Fecha;
            public string Hora;
        }


        /// <summary>
        /// Obtiene fecha  - hora en uso horario -05
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [WebMethod]
        public static FechaHora GetFechaHora(string path)
        {

            FechaHora fecha = new FechaHora();
            fecha.Fecha = DateTime.Now.ToString("yyy-MM-dd");

            string hora = DateTime.Now.ToString("HH:mm");

            //el server esta atrasado una hora,  por lo tanto sumar una
            string[] valoresHOra = hora.Split(':');
            int hh = int.Parse(valoresHOra[0]);
            if (hh < 23)
                hh++;//aca se suma 1

            string cero = (hh < 10) ? "0" : "";

            fecha.Hora = cero + hh + ":" + valoresHOra[1];


            return fecha;


        }

        public class DatosOdometro
        {
            public string odometro;
            public string IdUbicacion;
            public string fechaFormateadaMx;
        }

        [WebMethod]
        public static DatosOdometro GetUltimoOdometroReportado(string path, string idEquipo)
        {
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);
            DatosOdometro odometro = new DatosOdometro();

            try
            {
                conn.Open();

                DataSet ds = new DataSet();
                string query = @" SELECT r.id_requisicion, r.fecha_creacion, r.descripcion, IsNull(r.id_equipo, 0) id_equipo,                                
                                IsNull(r.detiene_operacion, 0) detiene_operacion, IsNull(r.id_ubicacion, 0) id_ubicacion,
                                r.orometro
                                FROM requisicion r 
                                join equipo e on (e.id_equipo = r.id_equipo) 
                                where IsNull(r.eliminado, 0) <> 1                                   
                                AND r.id_equipo =  @id_equipo
                                ORDER BY r.id_requisicion DESC, r.fecha_creacion DESC
                                    ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_equipo", idEquipo);

                Utils.Log("\n traer los reportes de falla de este equipo " + query + "\n");

                Utils.Log("\nidEquipo-> " + idEquipo);


                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    odometro.odometro = ds.Tables[0].Rows[0]["orometro"].ToString();

                    var fecha = DateTime.Parse(ds.Tables[0].Rows[0]["fecha_creacion"].ToString());
                    odometro.fechaFormateadaMx = fecha.ToString("dd/MM/yyyy");
                    odometro.IdUbicacion = ds.Tables[0].Rows[0]["id_ubicacion"].ToString();


                }


            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return null;
            }

            finally
            {
                conn.Close();
            }


            return odometro;

        }



        [WebMethod]
        public static List<Equipo> GetListaItems(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Equipo> items = new List<Equipo>();

            List<PermisoUsuario> listaPermisos = Usuarios.ObtenerListaPermisosPorUsuario(path, idUsuario);
            PermisoUsuario permisoEditar = listaPermisos.Find(x => x.IdPermiso == 7);
            PermisoUsuario permisoEliminar = listaPermisos.Find(x => x.IdPermiso == 6);


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT p.id_equipo, p.nombre, p.descripcion, p.numero_economico, p.numero_serie, IsNull(p.id_ubicacion, 0) id_ubicacion,  
                                IsNull(p.id_modelo, 0) id_modelo, p.id_marca, p.anio, IsNull(p.activo, 0) activo                     
                                ,m.nombre nombre_marca, mo.nombre nombre_modelo, u.nombre nombre_ubicacion, 
                                IsNull(p.valor_comercial, 0) valor_comercial, IsNull(p.orometro_ultimo_mantenimiento , 0) orometro_ultimo_mantenimiento
                                FROM equipo p 
                                left JOIN ubicacion u ON (u.id_ubicacion = p.id_ubicacion)  
                                JOIN marca m ON (m.id_marca = p.id_marca)  
                                left JOIN modelo mo ON (mo.id_modelo = p.id_modelo)  
                                where isNull(p.eliminado, 0) =  0 
                                ORDER BY p.id_equipo ";

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Equipo item = new Equipo();


                        item.IdEquipo = int.Parse(ds.Tables[0].Rows[i]["id_equipo"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.ValorComercial = float.Parse(ds.Tables[0].Rows[i]["valor_comercial"].ToString());
                        //item.IdCategoria = int.Parse(ds.Tables[0].Rows[i]["id_categoria"].ToString());

                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.NumeroEconomico = ds.Tables[0].Rows[i]["numero_economico"].ToString();
                        item.OrometroUltimoMantenimiento = ds.Tables[0].Rows[i]["orometro_ultimo_mantenimiento"].ToString();
                        item.IdMarca = int.Parse(ds.Tables[0].Rows[i]["id_marca"].ToString());
                        item.IdModelo = int.Parse(ds.Tables[0].Rows[i]["id_modelo"].ToString());

                        item.Anio = ds.Tables[0].Rows[i]["anio"].ToString();

                        item.NombreMarca = ds.Tables[0].Rows[i]["nombre_marca"].ToString();
                        item.NombreModelo = ds.Tables[0].Rows[i]["nombre_modelo"].ToString();
                        //item.NombreCategoria = ds.Tables[0].Rows[i]["nombre_categoria"].ToString();
                        item.NombreUbicacion = ds.Tables[0].Rows[i]["nombre_ubicacion"].ToString();

                        string operativo = (ds.Tables[0].Rows[i]["activo"].ToString());

                        if (operativo == "1")
                        {
                            item.ActivoStr = "<span class=\"badge badge-success\">Operativo</span>";
                        }
                        else
                        {
                            item.ActivoStr = "<span class=\"badge badge-warning\">NO operativo</span>";
                        }

                        string botones = "<button  title=\"Editar\" onclick='editar(" + item.IdEquipo + ")'  class='btn btn-outline-primary sm200'> <span class='fa fa-edit'></span>Editar&nbsp;</button>";
                        botones += " <button  title=\"Eliminar\" onclick='eliminar(" + item.IdEquipo + ")'   class='btn btn-outline-primary sm200'> <span class='fa fa-remove'></span>Eliminar&nbsp;</button>";
                        botones += " <button  title=\"Asignar refacciones\" onclick='asignarRefacciones(" + item.IdEquipo + ", \"" + item.Nombre + "\")'   class='btn btn-outline-primary sm200'> <span class='fa fa-car mr-1'></span>&nbsp;Refacciones</button>";

                        //botones += "&nbsp; <button  title=\"Descargar orden de trabajo\" onclick='descargarFormato(" + item.IdEquipo + ")'   class='btn btn-outline-primary'> <span class='fa fa-file-pdf-o'></span> Descargar</button>";


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
        public static List<Marca> GetListaMarcas(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Marca> items = new List<Marca>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_marca, nombre FROM marca ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                //string salida = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Marca item = new Marca();
                        item.Id_Marca = int.Parse(ds.Tables[0].Rows[i]["id_marca"].ToString());

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
        public static List<Unidad> GetListaUnidadesMedidaOrometro(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Unidad> items = new List<Unidad>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_unidad_medida, nombre FROM  unidad_medida WHERE ISNULL(activo, 1) <> 0 ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Unidad item = new Unidad();
                        item.IdUnidad = int.Parse(ds.Tables[0].Rows[i]["id_unidad_medida"].ToString());

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
        public static List<Modelo> GetListaModelos(string path, string idMarca)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Modelo> items = new List<Modelo>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_modelo, nombre FROM modelo WHERE id_marca = @id_marca ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_marca", idMarca);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Modelo item = new Modelo();
                        item.Id_Modelo = int.Parse(ds.Tables[0].Rows[i]["id_modelo"].ToString());

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
        public static List<Categoria> GetListaCategorias(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Categoria> items = new List<Categoria>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_categoria, nombre  FROM categoria ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                //string salida = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Categoria item = new Categoria();
                        item.IdCategoria = int.Parse(ds.Tables[0].Rows[i]["id_categoria"].ToString());

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
        public static List<Ubicacion> GetListaUbicaciones(string path)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Ubicacion> items = new List<Ubicacion>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = "SELECT id_ubicacion, nombre  FROM ubicacion ";


                SqlDataAdapter adp = new SqlDataAdapter(query, conn);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");

                adp.Fill(ds);
                //string salida = "";

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Ubicacion item = new Ubicacion();
                        item.IdUbicacion = int.Parse(ds.Tables[0].Rows[i]["id_ubicacion"].ToString());

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
        public static object Guardar(string path, Equipo equipo, string accion)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            string[] valores = new string[2];
            int r = 0;
            try
            {


                conn.Open();
                string sql = "";
                if (accion == "nuevo")
                {
                    sql = @" INSERT INTO equipo (nombre, descripcion, numero_economico, numero_serie,                         
                             id_categoria, id_marca, id_modelo, anio, activo, id_operador,
                                id_unidad_medida_orometro, capacidad_tanque)  
                             OUTPUT INSERTED.id_equipo
                             VALUES     
                             (@nombre, @descripcion, @numero_economico, @numero_serie,                                   
                             @id_categoria, @id_marca, @id_modelo, @anio, @activo, @id_operador, 
                                @id_unidad_medida_orometro, @capacidad_tanque) ";
                }
                else
                {
                    sql = @" UPDATE equipo SET nombre = @nombre, descripcion = @descripcion,               
                               numero_economico = @numero_economico, numero_serie = @numero_serie,         
                              id_marca = @id_marca, id_modelo = @id_modelo,   
                              id_categoria = @id_categoria, anio = @anio, activo = @activo, 
                              id_unidad_medida_orometro = @id_unidad_medida_orometro,
                              capacidad_tanque = @capacidad_tanque,
                              id_operador = @id_operador
                              WHERE id_equipo = @id_equipo  ";

                }

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@nombre", equipo.Nombre);
                cmd.Parameters.AddWithValue("@numero_economico", equipo.NumeroEconomico); 
                cmd.Parameters.AddWithValue("@id_unidad_medida_orometro", equipo.IdUnidadMedidaOrometro);
                
                cmd.Parameters.AddWithValue("@numero_serie", equipo.NumeroSerie);
                cmd.Parameters.AddWithValue("@descripcion", equipo.Descripcion);
                cmd.Parameters.AddWithValue("@id_categoria", 1);
                cmd.Parameters.AddWithValue("@id_marca", equipo.IdMarca);
                cmd.Parameters.AddWithValue("@id_modelo", equipo.IdModelo);

                cmd.Parameters.AddWithValue("@anio", equipo.Anio);
                cmd.Parameters.AddWithValue("@id_equipo", equipo.IdEquipo);
                cmd.Parameters.AddWithValue("@activo", equipo.Activo);
                cmd.Parameters.AddWithValue("@valor_comercial", equipo.ValorComercial);
                cmd.Parameters.AddWithValue("@id_operador", equipo.IdOperador);
                cmd.Parameters.AddWithValue("@capacidad_tanque", equipo.CapacidadTanque);

                int idGenerado = -1;
                if (accion == "nuevo")
                {
                    idGenerado = (int)cmd.ExecuteScalar();
                }
                else
                {
                    cmd.ExecuteNonQuery();

                    idGenerado = equipo.IdEquipo;
                }

                Utils.Log("Guardado -> OK ");

                valores[0] = idGenerado.ToString();
                valores[1] = "Guardado correctamente";

            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                r = -1;
                valores[1] = "Se ha generado un error <br/>" + ex.Message + " ... " + ex.StackTrace.ToString();

            }

            finally
            {
                conn.Close();
            }

            return valores;


        }

        public class DatosSalida
        {
            public int CodigoError;
            public string MensajeError;
        }

        [WebMethod]
        public static DatosSalida EliminarEquipo(string path, string id)
        {
            DatosSalida salida = new DatosSalida();
            salida.CodigoError = 0;
            salida.MensajeError = null;

            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);


            SqlTransaction transaccion = null;


            try
            {


                conn.Open();

                transaccion = conn.BeginTransaction();



                string sql = "";

                sql = " UPDATE equipo set eliminado = 1" +
                        " WHERE id_equipo = @id ";



                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Transaction = transaccion;


                int r = cmd.ExecuteNonQuery();
                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");

                transaccion.Commit();



                return salida;
            }
            catch (Exception ex)
            {

                salida.CodigoError = -1;
                salida.MensajeError = "No se pudo eliminar el proyecto.";


                try
                {
                    transaccion.Rollback();
                }
                catch (Exception ex2)
                {
                    Utils.Log("Error ... " + ex2.Message);
                    Utils.Log(ex2.StackTrace);
                }

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
        public static Equipo GetItem(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Equipo item = new Equipo();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT p.id_equipo, p.nombre, p.descripcion, p.numero_economico, p.numero_serie, IsNull(p.id_ubicacion, 0) id_ubicacion,  
                                IsNull(p.id_modelo, 0) id_modelo, p.id_marca, p.anio, IsNull(p.activo, 0) activo, 
                                m.nombre nombre_marca, mo.nombre nombre_modelo, u.nombre nombre_ubicacion, IsNull(p.valor_comercial, 0) valor_comercial, 
                                IsNull(p.id_operador, 0) id_operador,  
                                IsNull(p.id_unidad_medida_orometro, 2) id_unidad_medida_orometro,
                                capacidad_tanque
                                FROM equipo p                                
                                LEFT JOIN ubicacion u ON (u.id_ubicacion = p.id_ubicacion)  
                                JOIN marca m ON (m.id_marca = p.id_marca)  
                                left JOIN modelo mo ON (mo.id_modelo = p.id_modelo)  
                                WHERE p.id_equipo = @id ";

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
                        item = new Equipo();

                        item.IdEquipo = int.Parse(ds.Tables[0].Rows[i]["id_equipo"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.NumeroSerie = ds.Tables[0].Rows[i]["numero_serie"].ToString();
                        item.NumeroEconomico = ds.Tables[0].Rows[i]["numero_economico"].ToString();
                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.IdUbicacion = int.Parse(ds.Tables[0].Rows[i]["id_ubicacion"].ToString());
                        item.IdOperador = int.Parse(ds.Tables[0].Rows[i]["id_operador"].ToString());
                        item.IdUnidadMedidaOrometro = int.Parse(ds.Tables[0].Rows[i]["id_unidad_medida_orometro"].ToString());
                        //item.IdCategoria = int.Parse(ds.Tables[0].Rows[i]["id_categoria"].ToString());
                        //item.ValorComercial = float.Parse(ds.Tables[0].Rows[i]["valor_comercial"].ToString());


                        item.IdMarca = int.Parse(ds.Tables[0].Rows[i]["id_marca"].ToString());
                        item.IdModelo = int.Parse(ds.Tables[0].Rows[i]["id_modelo"].ToString());

                        item.Anio = ds.Tables[0].Rows[i]["anio"].ToString();
                        item.CapacidadTanque = ds.Tables[0].Rows[i]["capacidad_tanque"].ToString();

                        item.NombreMarca = ds.Tables[0].Rows[i]["nombre_marca"].ToString();
                        item.NombreModelo = ds.Tables[0].Rows[i]["nombre_modelo"].ToString();
                        //item.NombreCategoria = ds.Tables[0].Rows[i]["nombre_categoria"].ToString();
                        item.NombreUbicacion = ds.Tables[0].Rows[i]["nombre_ubicacion"].ToString();
                        item.Activo = int.Parse(ds.Tables[0].Rows[i]["activo"].ToString());

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
        public static string GetFoto(string path, string idEquipo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            string item = "";
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" SELECT fotografia_b64 FROM equipo WHERE id_equipo = @id ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("Id =  " + idEquipo);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id", idEquipo);

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
        public static List<CatalogoRefaccion> GetListaRefaccionesPorEquipo(string path, string idEquipo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<CatalogoRefaccion> items = new List<CatalogoRefaccion>();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" 
                                     SELECT r.id_catalogo_refacciones, r.numero_parte, r.descripcion, r.id_marca
                                     FROM catalogo_refacciones r
                                     JOIN relacion_equipo_refacciones rel ON  (rel.id_catalogo_refacciones = r.id_catalogo_refacciones) 
                                     WHERE rel.id_equipo = @id_equipo
                                ";

                Utils.Log("idtipoUsidUsuariouario =  " + idEquipo);
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_equipo", idEquipo);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CatalogoRefaccion item = new CatalogoRefaccion();

                        item.IdCatalogoRefaccion = int.Parse(ds.Tables[0].Rows[i]["id_catalogo_refacciones"].ToString());
                        item.NumeroParte = ds.Tables[0].Rows[i]["numero_parte"].ToString();
                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.Id_Marca = ds.Tables[0].Rows[i]["id_marca"].ToString();


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
        public static List<CatalogoRefaccion> GetListaRefacciones(string path, string idEquipo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<CatalogoRefaccion> items = new List<CatalogoRefaccion>();

            List<CatalogoRefaccion> listaRefaccionesEquipo = GetListaRefaccionesPorEquipo(path, idEquipo);


            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = @" 
                                     SELECT r.id_catalogo_refacciones, r.numero_parte, r.descripcion, r.id_marca
                                     FROM catalogo_refacciones r
                                ";

                Utils.Log("idtipoUsidUsuariouario =  " + idEquipo);
                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@id_equipo", idEquipo);

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");



                adp.Fill(ds);


                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        CatalogoRefaccion item = new CatalogoRefaccion();

                        item.IdCatalogoRefaccion = int.Parse(ds.Tables[0].Rows[i]["id_catalogo_refacciones"].ToString());
                        item.NumeroParte = ds.Tables[0].Rows[i]["numero_parte"].ToString();
                        item.Descripcion = ds.Tables[0].Rows[i]["descripcion"].ToString();
                        item.Id_Marca = ds.Tables[0].Rows[i]["id_marca"].ToString();


                        items.Add(item);

                    }
                }

                //  quitar de la lista total de equipos los equipos que ya tiene asignados
                HashSet<int> equiposIds = new HashSet<int>(listaRefaccionesEquipo.Select(x => x.IdCatalogoRefaccion));
                items.RemoveAll(x => equiposIds.Contains(x.IdCatalogoRefaccion));



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
        public static object GuardarEquipoRefacciones(string path, List<EquipoCatalogoRefaccion> listaRefacciones, string idEquipo)
        {


            Utils.Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            SqlTransaction transaccion = null;

            try
            {

                int r = 0;

                conn.Open();
                transaccion = conn.BeginTransaction();


                string sqlborrar = "";

                sqlborrar = @" DELETE relacion_equipo_refacciones  WHERE id_equipo =  @id ";



                SqlCommand cmd = new SqlCommand(sqlborrar, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@id", idEquipo);
                cmd.Transaction = transaccion;


                r += cmd.ExecuteNonQuery();
                Utils.Log("r = " + r);
                Utils.Log("Eliminado -> OK ");


                string sql = "";
                foreach (var item in listaRefacciones)
                {
                    sql = " INSERT INTO relacion_equipo_refacciones (id_equipo, id_catalogo_refacciones) " +
                          "VALUES (@id_equipo, @id_catalogo_refacciones )";

                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                    SqlCommand cmd2 = new SqlCommand(sql, conn);
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Parameters.AddWithValue("@id_catalogo_refacciones", item.idCatalogoRefaccion);
                    cmd2.Parameters.AddWithValue("@id_equipo", item.IdEquipo);
                    cmd2.Transaction = transaccion;


                    r += cmd2.ExecuteNonQuery();
                    Utils.Log("Guardado -> OK ");
                }

                transaccion.Commit();

                return r;
            }
            catch (Exception ex)
            {

                try
                {
                    transaccion.Rollback();
                }
                catch (Exception ex2)
                {
                    Utils.Log("Error ... " + ex2.Message);
                    Utils.Log(ex2.StackTrace);
                }

                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }

            finally
            {
                conn.Close();
            }


        }




    }



}