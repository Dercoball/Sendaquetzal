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
    public partial class FAQS : System.Web.UI.Page
    {
        public List<PreguntaFrecuente> itemsFaqs;

        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = (string)Session["usuario"];
            string path = Dns.GetHostName();

            //string host = Dns.GetHostByAddress;
            string host = Request.UserHostAddress;


            itemsFaqs = FAQ.GetListaItemsPublic(host);

        }




    }
}