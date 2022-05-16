using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Plataforma
{

    public class FileUploader : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            System.Diagnostics.Debug.Print("Respuesta desde FileUploader ");
            context.Response.ContentType = "text/plain";
            try
            {

                string str_image = "";

                foreach (string s in context.Request.Files)
                {
                    HttpPostedFile file = context.Request.Files[s];
                    string idItem = context.Request.Form[0];
                    string pagina = context.Request.Form[1];
                    string path_server = context.Request.Form[2];
                    string extension = context.Request.Form[3];
                    string descripcionArchivo = context.Request.Form[4];
                    string tipo = context.Request.Form[5];
                    string idUsuario = context.Request.Form[6];
                    string nombreArchivo = context.Request.Form[7];
                    
              
                    System.IO.Stream fs = file.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                    
                    //string uuid = Guid.NewGuid().ToString();
                    //descripcionArchivo = idItem + "_" +  uuid + "." +  extension;

                    var url = Path.Combine("/pages/Uploads/") + idItem + "_" + descripcionArchivo;
                    var urlThumb = Path.Combine("/pages/Uploads/thumb_") + idItem + "_" + descripcionArchivo;


                    if (tipo == "fotografia")
                    {
                        InsertarImagen(path_server, idItem, base64String, descripcionArchivo, "-1", url);

                    }
                    else if (tipo == "fotografiaSolicitudCombustible")
                    {
                        
                        InsertarImagenSolicitudCombustible(path_server, idItem, base64String, descripcionArchivo, tipo, url, idUsuario, nombreArchivo);
                    }
                    else if (tipo == "fotografia_empleado")
                    {
                        InsertarFotoEmpleado(path_server, idItem, base64String, descripcionArchivo, "-1", url);
                    }
                    else
                    {

                      
                        string id_tipo = (tipo == "documento") ? "1" : "0";

                        //for thumbnail
                        var thumb = "thumb_" + idItem + "_" + descripcionArchivo;

                        SaveThumbnail(file, context, idItem + "_" + descripcionArchivo, thumb);


                        InsertarDocumento(path_server, idItem, base64String, descripcionArchivo, id_tipo, urlThumb);
                    }

                    context.Response.Write(str_image);


                }

            }
            catch (Exception ac)
            {
                Log("Error " + ac.Message);
                Log(ac.StackTrace);
            }

        }

        public static void Log(string texto)
        {
            System.Diagnostics.Debug.Print(texto);



        }

        void SaveThumbnail(HttpPostedFile file, HttpContext context, string url, string urlThumb)
        {
            
            try
            {
                
                var path = Path.Combine(context.Server.MapPath("~/pages/Uploads"), url);                               
                file.SaveAs(path);

                //  thumbnail
                var pathThumb = Path.Combine(context.Server.MapPath("~/pages/Uploads"), urlThumb);
                Image image = Image.FromFile(path);
                Image thumb = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
                thumb.Save(pathThumb);


                //

                Log("Archivo guardado en Uploads correctamente ");                
            }catch(Exception ex)
            {
                Log(ex.Message  +  "\n");

            }

        }

        public int InsertarDocumento(string path, string id, string b64Contenido, string descripcion, string tipo, string urlThumbnail)
        {

            Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);



            int r = 0;
            try
            {

                conn.Open();

                string sql = "";

                //
                sql = " insert into documento_requisicion" +
                                    " (contenido_b64, id_requisicion, fecha, descripcion_documento, id_tipo_documento, url)" +
                                    " values (@contenido_b64, @id_requisicion, @fecha, @descripcion_documento, @id_tipo_documento, @url) " +
                                    "  ";

                Log("sql = " + sql);

                SqlCommand cmdGrupo = new SqlCommand(sql, conn);
                cmdGrupo.CommandType = CommandType.Text;
                cmdGrupo.Parameters.AddWithValue("@id_requisicion", id);
                cmdGrupo.Parameters.AddWithValue("@fecha", DateTime.Now);
                cmdGrupo.Parameters.AddWithValue("@contenido_b64", b64Contenido);
                cmdGrupo.Parameters.AddWithValue("@descripcion_documento", descripcion);
                cmdGrupo.Parameters.AddWithValue("@id_tipo_documento", tipo);
                cmdGrupo.Parameters.AddWithValue("@url", urlThumbnail);


                r = (int)cmdGrupo.ExecuteNonQuery();



                Log("Guardado -> OK " + r);



            }
            catch (Exception ex)
            {
                Log("Error " + ex.Message);
                Log(ex.StackTrace);

                r = -1;
            }

            finally
            {
                conn.Close();
            }
            return r;


        }


        public int InsertarImagen(string path, string id, string b64, string descripcionArchivo, string tipo, string urlThumbnail)
        {

            Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);



            int r = 0;
            try
            {

                conn.Open();

                string sql = "";

              
                sql = " UPDATE equipo " +
                                        " SET fotografia_b64 = @fotografia_b64 WHERE id_equipo =  @id";
               
                Log("sql = " + sql);


                SqlCommand cmdGrupo = new SqlCommand(sql, conn);
                cmdGrupo.CommandType = CommandType.Text;
                cmdGrupo.Parameters.AddWithValue("@id", id);
                cmdGrupo.Parameters.AddWithValue("@fotografia_b64", b64);


                r = (int)cmdGrupo.ExecuteNonQuery();



                Log("Guardado -> OK " + r);



            }
            catch (Exception ex)
            {
                Log("Error " + ex.Message);
                Log(ex.StackTrace);

                r = -1;
            }

            finally
            {
                conn.Close();
            }
            return r;


        }

        public int InsertarFotoEmpleado(string path, string id, string b64, string descripcionArchivo,
            string tipo, string urlThumbnail)
        {

            Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);



            int r = 0;
            try
            {

                conn.Open();

                string sql = "";


                sql = " UPDATE empleado " +
                                        " SET fotografia_b64 = @fotografia_b64 WHERE id_empleado =  @id";

                Log("sql = " + sql);


                SqlCommand cmdGrupo = new SqlCommand(sql, conn);
                cmdGrupo.CommandType = CommandType.Text;
                cmdGrupo.Parameters.AddWithValue("@id", id);
                cmdGrupo.Parameters.AddWithValue("@fotografia_b64", b64);


                r = (int)cmdGrupo.ExecuteNonQuery();



                Log("Guardado -> OK " + r);



            }
            catch (Exception ex)
            {
                Log("Error " + ex.Message);
                Log(ex.StackTrace);

                r = -1;
            }

            finally
            {
                conn.Close();
            }
            return r;


        }

        public int InsertarImagenSolicitudCombustible(string path, string id, string b64, string descripcionArchivo, 
            string tipo, string urlThumbnail, string idUsuario, string numeroImagen)
        {

            Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);
            


            int r = 0;
            try
            {

                conn.Open();

                string sql = "";


                // intentar hacer update si ya existe la imagen

                sql = @" UPDATE documento_solicitud_combustible  SET contenido_b64 = @contenido_b64, fecha = @fecha, 
                                                descripcion_documento = @descripcion_documento, id_usuario = @id_usuario, 
                                                url_documento = @url_documento
                                             WHERE id_tipo_documento = @id_tipo_documento AND
                                             id_detalle_solicitud_combustible = @id_detalle_solicitud_combustible ";
                                            

                Log("sql = " + sql);


                SqlCommand cmdUpdate = new SqlCommand(sql, conn);
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.Parameters.AddWithValue("@id_detalle_solicitud_combustible", id);
                cmdUpdate.Parameters.AddWithValue("@url_documento", urlThumbnail);
                cmdUpdate.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmdUpdate.Parameters.AddWithValue("@descripcion_documento", descripcionArchivo);
                cmdUpdate.Parameters.AddWithValue("@contenido_b64", b64);
                cmdUpdate.Parameters.AddWithValue("@id_tipo_documento", numeroImagen);
                cmdUpdate.Parameters.AddWithValue("@fecha", DateTime.Now);
                r = (int)cmdUpdate.ExecuteNonQuery();


                Log("r = " + r);

                //  si no se hizo el  update
                if (r == 0)
                {

                    sql = " INSERT INTO documento_solicitud_combustible (id_detalle_solicitud_combustible, url_documento, id_usuario, id_tipo_documento," +
                                                "contenido_b64, fecha, descripcion_documento) VALUES" +
                                                "(@id, @url_documento, @id_usuario, @id_tipo_documento, @contenido_b64, @fecha, @descripcion_documento)";

                    Log("sql = " + sql);


                    SqlCommand cmdGrupo = new SqlCommand(sql, conn);
                    cmdGrupo.CommandType = CommandType.Text;
                    cmdGrupo.Parameters.AddWithValue("@id", id);
                    cmdGrupo.Parameters.AddWithValue("@url_documento", urlThumbnail);
                    cmdGrupo.Parameters.AddWithValue("@id_usuario", idUsuario);
                    cmdGrupo.Parameters.AddWithValue("@id_tipo_documento", numeroImagen);
                    cmdGrupo.Parameters.AddWithValue("@descripcion_documento", descripcionArchivo);
                    cmdGrupo.Parameters.AddWithValue("@contenido_b64", b64);
                    cmdGrupo.Parameters.AddWithValue("@fecha", DateTime.Now);


                    r = (int)cmdGrupo.ExecuteNonQuery();

                }

                Log("Guardado -> OK " + r);



            }
            catch (Exception ex)
            {
                Log("Error " + ex.Message);
                Log(ex.StackTrace);

                r = -1;
            }

            finally
            {
                conn.Close();
            }
            return r;


        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}