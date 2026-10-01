using System;



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