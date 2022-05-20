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
                    string idCliente = context.Request.Form[8];


                    System.IO.Stream fs = file.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                    string pattern = @"[^0-9a-zA-Z]+";  // para quitar espacios y caracteres raros
                    descripcionArchivo = descripcionArchivo.Replace(pattern, "") + "." + extension;

                    var url = Path.Combine("/pages/Uploads/") + idItem + "_" + tipo + "_" + descripcionArchivo;
                    var urlThumb = Path.Combine("/pages/Uploads/thumb_") + idItem + "_" + tipo + "_" + descripcionArchivo;

                    //for thumbnail
                    var thumb = "thumb_" + idItem + "_" + tipo;
                    SaveThumbnail(file, context, idItem + "_" + tipo, thumb);

                    if (nombreArchivo == "documento")
                    {
                        InsertDocument(path_server, idItem, idCliente, base64String, descripcionArchivo, tipo, urlThumb, extension);
                    }
                    else if (nombreArchivo == "update_document_employee")
                    {

                        int r = UpdateDocumentEmployee(path_server, idItem, base64String, descripcionArchivo, tipo, urlThumb, extension);

                        if (r == 0)
                        {
                            InsertDocument(path_server, idItem, idCliente, base64String, descripcionArchivo, tipo, urlThumb, extension);
                        }

                    }
                    else if (nombreArchivo == "update_document_customer")
                    {

                        int r = UpdateDocumentCustomer(path_server, idItem, base64String, descripcionArchivo, tipo, urlThumb, extension);

                        if (r == 0)
                        {
                            InsertDocument(path_server, idItem, idCliente, base64String, descripcionArchivo, tipo, urlThumb, extension);
                        }

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
            }
            catch (Exception ex)
            {
                Log(ex.Message + "\n");

            }

        }

        public int InsertDocument(string path, string idEmpleado, string idCliente, string b64Contenido, string nombre, string tipo, string urlThumbnail, string extension)
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
                sql = @" insert into documento 
                                     (contenido, nombre, fecha_ingreso, id_tipo_documento, url, id_empleado, id_cliente, extension) 
                                     values 
                                     (@contenido, @nombre, @fecha_ingreso, @id_tipo_documento, @url, @id_empleado, @id_cliente, @extension)  ";

                Log("sql = " + sql);

                SqlCommand cmdGrupo = new SqlCommand(sql, conn);
                cmdGrupo.CommandType = CommandType.Text;
                cmdGrupo.Parameters.AddWithValue("@id_empleado", idEmpleado);
                cmdGrupo.Parameters.AddWithValue("@id_cliente", idCliente);
                cmdGrupo.Parameters.AddWithValue("@fecha_ingreso", DateTime.Now);
                cmdGrupo.Parameters.AddWithValue("@contenido", b64Contenido);
                cmdGrupo.Parameters.AddWithValue("@nombre", nombre);
                cmdGrupo.Parameters.AddWithValue("@id_tipo_documento", tipo);
                cmdGrupo.Parameters.AddWithValue("@url", urlThumbnail);
                cmdGrupo.Parameters.AddWithValue("@extension", extension);


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

        public int UpdateDocumentEmployee(string path, string idEmpleado, string b64Contenido, string nombre, string tipo, string urlThumbnail, string extension)
        {

            Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);



            int r = 0;
            try
            {

                conn.Open();

                string sql = "";

                sql = @" UPDATE documento 
                                     SET contenido = @contenido, nombre = @nombre, fecha_ingreso = @fecha_ingreso, url = @url, extension = @extension
                                     WHERE 
                                     id_tipo_documento = @id_tipo_documento AND id_empleado = @id_empleado ";

                Log("sql = " + sql);

                SqlCommand cmdGrupo = new SqlCommand(sql, conn);
                cmdGrupo.CommandType = CommandType.Text;
                cmdGrupo.Parameters.AddWithValue("@id_empleado", idEmpleado);
                cmdGrupo.Parameters.AddWithValue("@fecha_ingreso", DateTime.Now);
                cmdGrupo.Parameters.AddWithValue("@contenido", b64Contenido);
                cmdGrupo.Parameters.AddWithValue("@nombre", nombre);
                cmdGrupo.Parameters.AddWithValue("@id_tipo_documento", tipo);
                cmdGrupo.Parameters.AddWithValue("@url", urlThumbnail);
                cmdGrupo.Parameters.AddWithValue("@extension", extension);


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


        public int UpdateDocumentCustomer(string path, string idCliente, string b64Contenido, string nombre, string tipo, string urlThumbnail, string extension)
        {

            Log("\n==>INICIANDO Método-> " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n");
            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);



            int r = 0;
            try
            {

                conn.Open();

                string sql = "";

                sql = @" UPDATE documento 
                                     SET contenido = @contenido, nombre = @nombre, fecha_ingreso = @fecha_ingreso, url = @url, extension = @extension
                                     WHERE 
                                     id_tipo_documento = @id_tipo_documento AND id_empleado = @id_empleado ";

                Log("sql = " + sql);

                SqlCommand cmdGrupo = new SqlCommand(sql, conn);
                cmdGrupo.CommandType = CommandType.Text;
                cmdGrupo.Parameters.AddWithValue("@id_cliente", idCliente);
                cmdGrupo.Parameters.AddWithValue("@fecha_ingreso", DateTime.Now);
                cmdGrupo.Parameters.AddWithValue("@contenido", b64Contenido);
                cmdGrupo.Parameters.AddWithValue("@nombre", nombre);
                cmdGrupo.Parameters.AddWithValue("@id_tipo_documento", tipo);
                cmdGrupo.Parameters.AddWithValue("@url", urlThumbnail);
                cmdGrupo.Parameters.AddWithValue("@extension", extension);


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


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}