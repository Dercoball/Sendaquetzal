using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        static string GetIP()
        {
            string Str = "";
            Str = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(Str);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();

        }

        [WebMethod]
        public static Usuario GetLogin(string path, string login, string password, string valor)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Usuario item = new Usuario();
            SqlConnection conn = new SqlConnection(strConexion);

            //Usuario u = Usuarios.GetUsuario();

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_usuario, id_tipo_usuario,  nombre, login, password, " +
                    " email, telefono, Isnull(id_proveedor, 0) id_proveedor, " +
                    " Isnull(id_empleado, 0) id_empleado " +
                               " FROM usuario " +
                               " WHERE login = @login and password = @password " +
                               " and IsNull(eliminado, 0) <> 1 ";

                Utils.Log("\nMétodo-> " +
                System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + query + "\n");
                Utils.Log("login =  " + login);

                SqlDataAdapter adp = new SqlDataAdapter(query, conn);
                adp.SelectCommand.Parameters.AddWithValue("@login", login);
                adp.SelectCommand.Parameters.AddWithValue("@password", password);

                adp.Fill(ds);

                bool found = false;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        item.Id_Usuario = int.Parse(ds.Tables[0].Rows[i]["id_usuario"].ToString());
                        item.IdTipoUsuario = int.Parse(ds.Tables[0].Rows[i]["id_tipo_usuario"].ToString());
                        item.IdProveedor = int.Parse(ds.Tables[0].Rows[i]["id_proveedor"].ToString());
                        item.IdEmpleado = int.Parse(ds.Tables[0].Rows[i]["id_empleado"].ToString());
                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Login = ds.Tables[0].Rows[i]["login"].ToString();
                        item.Password = ds.Tables[0].Rows[i]["password"].ToString();
                        item.Email = ds.Tables[0].Rows[i]["email"].ToString();
                        item.Telefono = ds.Tables[0].Rows[i]["telefono"].ToString();

                        found = true;

                    }
                }

                string uuid = Guid.NewGuid().ToString();
                Utils.Log("uuid =  " + uuid);
                string hashedData = ComputeSha256Hash(uuid);
                Utils.Log("hashedData =  " + hashedData);

                item.Token = hashedData;

                if (found)
                {
                    DateTime now = DateTime.Now;

                    string sql = "";
                    sql = " INSERT INTO bitacora_login ( id_usuario, fecha_hora, comentario, ip) " +
                          " VALUES ( @id_usuario, @fecha_hora, @comentario, @ip ) ";
                    Utils.Log("\nMétodo-> " +
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                    string IP = GetIP();


                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@id_usuario", item.Id_Usuario);
                    cmd.Parameters.AddWithValue("@fecha_hora", now);
                    cmd.Parameters.AddWithValue("@ip",  IP);
                    cmd.Parameters.AddWithValue("@comentario",  "Usuario : " + item.Login + " -> Login, Os ->" + valor);

                    int r = cmd.ExecuteNonQuery();
                    Utils.Log("Login Guardado -> OK ");

                }

                return item;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                item.Msg = ex.Message + ex.StackTrace;
                return item;
            }

            finally
            {
                conn.Close();
            }

        }


        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }

        }


    }
}