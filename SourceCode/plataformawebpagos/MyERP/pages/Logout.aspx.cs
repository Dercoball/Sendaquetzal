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
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Session["usuario"] = null;
            Session["id_tipo_usuario"] = null;
            Session["id_usuario"] = null;
            Session["path"] = null;

            Response.Redirect("Login.aspx");

        }



    }
}