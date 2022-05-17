using Newtonsoft.Json;
using Plataforma.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Plataforma.pages
{
    public partial class Index : System.Web.UI.Page
    {
        const string pagina = "0";

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

        public class ElementosInterfaz
        {
            public string BarraLateral;
            public string BarraSuperior;


        }

        [WebMethod]
        public static bool TienePermisoPagina(string pagina, string path, string idUsuario)
        {
            List<PermisoUsuario> listaPermisos = Usuarios.ObtenerListaPermisosPorTipoUsuario(path, idUsuario);

            if (pagina != "0")
            {
                int idPermiso = -1;
                try
                {
                    idPermiso = int.Parse(pagina);
                }
                catch (Exception ex)
                {
                    Utils.Log("Error " + ex.Message);

                }
                PermisoUsuario permisoPaginaActual = listaPermisos.Find(x => x.IdPermiso == idPermiso);

                return (permisoPaginaActual != null);

            }
            return false;

        }

        [WebMethod]
        public static object GetPermisoPantallaActual(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Cliente> items = new List<Cliente>();



            List<PermisoUsuario> listaPermisos = Usuarios.ObtenerListaPermisosPorTipoUsuario(path, idUsuario);
            PermisoUsuario permisoNuevo = listaPermisos.Find(x => x.IdPermiso == 18);
            string botonNuevo = (permisoNuevo == null) ? "" : "<button class=\"btn btn-outline btn-success\" id=\"btnNuevo\" ><i class=\"glyphicon glyphicon-plus\" ></i>&nbsp;Nuevo</button>";



            string[] botones = new String[] { botonNuevo };

            return botones;



        }

        [WebMethod]
        public static Configuracion GetItemConfiguracion(string path, string id)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            Configuracion item = new Configuracion();
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {
                conn.Open();
                DataSet ds = new DataSet();
                string query = " SELECT id_configuracion, nombre, IsNull(valor, 0) valor, valorCadena " +
                               " FROM configuracion " +
                               " WHERE id_configuracion = @id ";

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
                        item.IdConfiguracion = int.Parse(ds.Tables[0].Rows[i]["id_configuracion"].ToString());

                        item.Nombre = ds.Tables[0].Rows[i]["nombre"].ToString();
                        item.Valor = float.Parse(ds.Tables[0].Rows[i]["valor"].ToString());
                        item.ValorCadena = ds.Tables[0].Rows[i]["ValorCadena"].ToString();

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
        public static object GetPermisoNuevo(string path, string idUsuario)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;

            SqlConnection conn = new SqlConnection(strConexion);
            List<Cliente> items = new List<Cliente>();



            List<PermisoUsuario> listaPermisos = Usuarios.ObtenerListaPermisosPorUsuario(path, idUsuario);
            PermisoUsuario permisoNuevo = listaPermisos.Find(x => x.IdPermiso == 18);



            string botonNuevo = (permisoNuevo == null) ? "" : "<button class=\"btn btn-outline btn-success\" id=\"btnNuevo\" ><i class=\"glyphicon glyphicon-plus\" ></i>&nbsp;Nuevo</button>";



            string[] botones = new String[] { botonNuevo };

            return botones;



        }


        [WebMethod]
        public static ElementosInterfaz GetNav(string path, string pagina, string idUsuario, string idTipoUsuario, string nombreUsuario)
        {

            ElementosInterfaz elementos = new ElementosInterfaz();


            List<PermisoUsuario> listaPermisos = Usuarios.ObtenerListaPermisosPorTipoUsuario(path, idTipoUsuario);


            if (pagina != "0")
            {
                int idPermiso = -1;
                try
                {
                    idPermiso = int.Parse(pagina);
                }
                catch (Exception ex)
                {
                    Utils.Log("Error " + ex.Message);

                }
                PermisoUsuario permisoPaginaActual = listaPermisos.Find(x => x.IdPermiso == idPermiso);
                if (idPermiso == -1 || permisoPaginaActual == null)
                {
                    //no tiene permisos para usar esta pagina

                    return null;

                }
            }






            string nav = "";


            List<string> paginaHome = new List<string>
                {
                    "83"
                };
            var esPagina_Home = paginaHome.Find(x => x == pagina);
            string active_Home = esPagina_Home != null ? "class = 'active' " : "";

            nav += "<li " + active_Home +
              "  id='liHome'><a href=\"" + HttpContext.Current.Server.UrlPathEncode("/pages/") + "Index.aspx\" aria-expanded=\"false\"> " +
              " <i class=\"icon-interface-windows\"></i>Home</a>   ";


            nav += "</li> ";

            //........MENU WEB
            //Agregar los numeros de paginas o permisos , de los tipo WEB
            List<string> paginasWeb = new List<string>
                {
                    "2",//faq                                  
                };


            var esPagina_Web = paginasWeb.Find(x => x == pagina);
            string active_Web = esPagina_Web != null ? "class = 'active' " : "";

            string htmlItemsWeb = "";
            foreach (var permiso in listaPermisos)
            {

                if (permiso.TipoPermiso == PermisoUsuario.TIPO_PERMISO_WEB)//4
                {
                    permiso.NombreInterno = HttpContext.Current.Server.UrlPathEncode("/pages/") + permiso.NombreInterno;

                    htmlItemsWeb += "<li> " +
                           " <a href=\"" + permiso.NombreInterno + "\"><i class=\"icon-picture\"></i>" + permiso.Nombre + "</a> " +
                           " </li> ";

                }

            }

            if (htmlItemsWeb != "")
            {

                nav += "<li " + active_Web + "><a href=\"#liWeb\" aria-expanded=\"false\" data-toggle=\"collapse\"> " +
                    " <i class=\"icon-interface-windows\"></i>Web</a> " +
                "  <ul id=\"liWeb\" class=\"collapse list-unstyled\"> ";


                nav += htmlItemsWeb;

                nav += "</ul> ";
                nav += "</li> ";
            }
            //

            //........MENU Configuracion
            //Agregar los numeros de paginas o permisos , de los tipo Configuracion
            List<string> paginasConfiguracion = new List<string>
                {
                    "7",//tipos de clientes                                  
                };


            var esPagina_Configuracion = paginasConfiguracion.Find(x => x == pagina);
            string active_Configuracion = esPagina_Configuracion != null ? "class = 'active' " : "";

            string htmlItemsConfiguracion = "";
            foreach (var permiso in listaPermisos)
            {

                if (permiso.TipoPermiso == PermisoUsuario.TIPO_PERMISO_CONFIGURACION)//2
                {
                    permiso.NombreInterno = HttpContext.Current.Server.UrlPathEncode("/config/") + permiso.NombreInterno;

                    htmlItemsConfiguracion += "<li> " +
                           " <a href=\"" + permiso.NombreInterno + "\"><i class=\"icon-picture\"></i>" + permiso.Nombre + "</a> " +
                           " </li> ";

                }

            }

            if (htmlItemsConfiguracion != "")
            {

                nav += "<li " + active_Configuracion + "><a href=\"#liConfiguracion\" aria-expanded=\"false\" data-toggle=\"collapse\"> " +
                    " <i class=\"icon-interface-windows\"></i>Configuracion</a> " +
                "  <ul id=\"liConfiguracion\" class=\"collapse list-unstyled\"> ";


                nav += htmlItemsConfiguracion;

                nav += "</ul> ";
                nav += "</li> ";
            }
            //


            List<string> paginaCliente = new List<string>
                {
                    "83"
                };
            var esPagina_Cliente = paginaCliente.Find(x => x == pagina);
            string active_Cliente = esPagina_Cliente != null ? "class = 'active' " : "";


            nav += "<li " + active_Cliente +
                " id='liHome'><a href=\"" + HttpContext.Current.Server.UrlPathEncode("/pages/") + "Customers.aspx\" aria-expanded=\"false\"> " +
                " <i class=\"icon-interface-windows\"></i>Clientes</a>   ";

            nav += "</li> ";



            //.........FIN DE CONFIGURACION





            Configuracion nombreEmpresa = new Configuracion();


            nombreEmpresa = GetItemConfiguracion(path, "2");//nombre de la empresa



            string barraSuperior = "" +
            " <div class=\"navbar-holder d-flex align-items-center justify-content-between\"> " +
            "  <div class=\"navbar-header\"><a id=\"toggle-btn\" href=\"#\" class=\"menu-btn\"><i class=\"icon-bars\"> </i></a><a href=\"Index.aspx\" class=\"navbar-brand\">  " +
            "      <div class=\"brand-text d-none d-md-inline-block\"><span></span><strong class=\"text-primary\">" + nombreEmpresa.ValorCadena + "</strong></div></a></div> " +

            "                <ul class=\"nav-menu list-unstyled d-flex flex-md-row\"> " +


            "                           <li class=\"nav-item\"><a href=\"#\" class=\"nav-link logout\" onclick=\"sessionStorage.removeItem('idusuario');    " +
            "                                   sessionStorage.removeItem('nombreusuario'); " + " window.top.location.href = '" +
                                                        HttpContext.Current.Server.UrlPathEncode("/pages/") +  "Login.aspx'\"><span class=\"d-none d-sm-inline-block\">Salir</span><i class=\"fa fa-sign-out\"></i></a></li>" +

            "                                   </li>   " +
            "                 </ul>   " +

            "             </div>   " +
            "         </div>   " +


            "       </div>   " +
            "   </div>   ";


            elementos.BarraLateral = nav;

            elementos.BarraSuperior = barraSuperior;

            return elementos;
        }



        public static object RegistrarLogCambios(string path, string idUsuario, string descripcion, string modulo)
        {

            string strConexion = System.Configuration.ConfigurationManager.ConnectionStrings[path].ConnectionString;
            SqlConnection conn = new SqlConnection(strConexion);

            try
            {


                conn.Open();
                string sql = " INSERT INTO log_cambios(id_usuario, descripcion, fecha_hora, modulo) " +
                                           " VALUES (@id_usuario, @descripcion, @fecha_hora, @modulo)";

                Utils.Log("\nMétodo-> " +
               System.Reflection.MethodBase.GetCurrentMethod().Name + "\n" + sql + "\n");

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@fecha_hora", DateTime.Now);
                cmd.Parameters.AddWithValue("@modulo", modulo);


                int r = cmd.ExecuteNonQuery();



                return r;
            }
            catch (Exception ex)
            {
                Utils.Log("Error ... " + ex.Message);
                Utils.Log(ex.StackTrace);
                return -1;
            }

            finally
            {
                conn.Close();
            }

        }


        public class Totales
        {
            public string descripcion;
            public string valorFormateado;
        }



        public class DatosApp
        {

            public string AppID;
            public string Url;
            public string Id_Infracciones;
        }




    }
}