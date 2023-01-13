using EASendMail;
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
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows;
using Twilio.TwiML.Voice;
using static QRCoder.PayloadGenerator;

namespace Plataforma.pages
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void OnGet()
        {

        }

        //public void OnPost()
        //{
        //    var nombre = Request.Form["txtNombre"];
        //    var email = Request.Form["txtDireccionEmail"];
        //    var contenidoEmail = Request.Form["txtContenidoEmail"];

        //    EnviarEmail(nombre, email, contenidoEmail);


        //}

        ///MEDIO FUNCIONAL
        //SmtpServer getSmtp(int Intento)
        //{
        //    SmtpServer mySmtp;
        //    string MailEmisor, pwd;
        //    pwd = "i3Q7nSUwwfk-0w";
        //    if (Intento == 1)
        //    {
        //        MailEmisor = "cfdi2020@nanocode.mx";
        //        var Domain = "nanocode.mx";
        //        var User = "bpgkvdnqrvke";
        //        mySmtp = new SmtpServer("smtp1.s.ipzmarketing.com", 10587);
        //        mySmtp.User = User;
        //        mySmtp.Password = pwd;
        //        mySmtp.ConnectType = SmtpConnectType.ConnectTryTLS;
        //        //mySmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        //mySmtp.UseDefaultCredentials = false;
        //    }
        //    else
        //    {
        //        var User = "cfdi2020@nanocode.mx";
        //        mySmtp = new SmtpServer("mail.smtp2go.com", 2525);
        //        mySmtp.User = User;
        //        mySmtp.Password = pwd;
        //    }
        //    return mySmtp;
        //}
        //public bool EnviaFacturaMail(int NumeroIntento)
        //{

        //    //string cmd = "Select [Nombre Comercial] FROM Emisor";
        //    string NomComp = txtNombre.Value;


        //    string Asunto = "Prestamo SQ";
        //    //string Body = "Hola, "+NomComp+" le env�o una "+doc+":<br>" +
        //    //            "<b>Gracias por su atenci�n</b>";
        //    string Body = "<H4 Comentario: "+txtContenidoEmail.Value+"</H5>";

        //    //string MailEmisor = NumeroIntento == 1 ? "CFDI@nanocode.mx" : "nanocode@outlook.com";
        //    string MailEmisor = "cfdi@nanocode.mx";

        //    SmtpMail m = new SmtpMail("TRYIT");
        //    m.From = MailEmisor;
        //    m.To = "j.sillas@nanocode.mx";//txtDireccionEmail.Value;
        //    //m.To = "dlopez@nanocode.mx";//txtDireccionEmail.Value;
        //    m.Subject = Asunto;
        //    m.HtmlBody = Body;
        //    //m.AddAttachment(FFiscalLb.Text + ".pdf");
        //    //if (TipoRd.EditValue.ToString() != "3")
        //    //{
        //    //    m.AddAttachment(FFiscalLb.Text + ".xml");
        //    //}

        //    EASendMail.SmtpClient smtp = new EASendMail.SmtpClient();
        //    try
        //    {
        //        smtp.SendMail(getSmtp(NumeroIntento), m);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (NumeroIntento == 2)
        //            MessageBox.Show(ex.ToString());
        //            //Console.Write(ex.ToString());
        //        return false;
        //    }
        //    return true;
        //}

        //public bool EnviarEmail(string nombre, string direccionEmail, string contenidoEmail)
        //{


        //    return true;


        //}

        SmtpServer getSmtp(int Intento)
        {
            SmtpServer mySmtp;
            string MailEmisor, pwd;
            pwd = "i3Q7nSUwwfk-0w";
            if (Intento == 1)
            {
                MailEmisor = "cfdi2020@nanocode.mx";
                var Domain = "nanocode.mx";
                var User = "bpgkvdnqrvke";
                mySmtp = new SmtpServer("smtp1.s.ipzmarketing.com", 10587);
                mySmtp.User = User;
                mySmtp.Password = pwd;
                mySmtp.ConnectType = SmtpConnectType.ConnectTryTLS;
                //mySmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //mySmtp.UseDefaultCredentials = false;
            }
            else
            {
                var User = "cfdi2020@nanocode.mx";
                mySmtp = new SmtpServer("mail.smtp2go.com", 2525);
                mySmtp.User = User;
                mySmtp.Password = pwd;
            }
            return mySmtp;
        }
        public bool EnviaFacturaMail(int NumeroIntento)
        {

            string cmd = "Select [Nombre Comercial] FROM Emisor";
            string NomComp = txtNombre.Value; //"Juan de Dios";
            //string doc = "factura";
            //switch (TipoRd.EditValue.ToString())
            //{
            //    case "1":
            //        doc = "factura";
            //        break;
            //    case "2":
            //        doc = "nota de cr�dito";
            //        break;
            //    case "3":
            //        doc = "cotizaci�n";
            //        break;
            //        //case "4":
            //        //    doc = "Carta Porte";
            //        //    break;
            //        //default:
            //        //    break;
            //}
            string Asunto = NomComp + "ha solicitado un préstamo en SQ";
            //string Body = "Hola, "+NomComp+" le env�o una "+doc+":<br>" +
            //            "<b>Gracias por su atenci�n</b>";
            string Body = "<H4 align=\"center\"><b>Hola:</b><br><br> " +
                            "" + NomComp + " te ha hecho solicitud de préstamo a traves de " +
                            "<b><font color=\"#2B66A6\">Formulario SQ</font></b> " +
                            "de <font color=\"#2B66A6\"><b>NanoCode</b></font><br> " +
                            //"Si lo deseas, t� tambi�n podr�s hacerlo tan f�cil como " + NomComp + ".<br><br> " +
                            //"<HR width=65% align=\"center\"> <br>" +
                            //"Informate como es en:<br>" +
                            //"<a href=\"http://www.nanocode.mx\">nanocode.mx</a> <br>" +
                            //"<a href=\"https://www.facebook.com/SoftwareNanoCode\">Facebook/SoftwareNanoCode</a> <br>" +
                            //"<a href=\"https://www.youtube.com/watch?v=92AtK9cveEo\">www.youtube.com/nanofactura</a> <br>" +
                            //"<b><font color=\"#2B66A6\">Telefonos: (33) 1258-0618 y (33) 3607-8422</font></b></H4>" +
                            //"<HR width=65% align=\"center\"> <br>" +
                            
                            "<H5 align=\"center\">Este es un servicio de envio de correo automático <b><font color=\"#2B66A6\">NanoCode</font></b> favor de no responder a esta dirección</H5>"
                            +"El mensaje que ha escrito :"+ NomComp+ " es : " + txtContenidoEmail.Value+ "" + " y su dirección de correo es : " + txtDireccionEmail.Value;

            //string MailEmisor = NumeroIntento == 1 ? "CFDI@nanocode.mx" : "nanocode@outlook.com";
            string MailEmisor = "cfdi@nanocode.mx";

            SmtpMail m = new SmtpMail("TRYIT");
            m.From = MailEmisor;
            m.To = "j.sillas@nanocode.mx";
            //m.To = "dlopez@nanocode.mx";
            //m.To = "jalvarez@nanocode.mx";
            m.Subject = Asunto;
            m.HtmlBody = Body;
            //m.AddAttachment(FFiscalLb.Text + ".pdf");
            //if (TipoRd.EditValue.ToString() != "3")
            //{
            //    m.AddAttachment(FFiscalLb.Text + ".xml");
            //}

            EASendMail.SmtpClient smtp = new EASendMail.SmtpClient();
            try
            {
                smtp.SendMail(getSmtp(NumeroIntento), m);
            }
            catch (Exception ex)
            {
                if (NumeroIntento == 2)
                    MessageBox.Show(ex.ToString());
                return false;
            }
            return true;
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            //string nombre = Request.Form["txtNombre"];
            //string correo = Request.Form["txtDireccionEmail"];
            //string contenidoCorreo = Request.Form["txtContenidoEmail"];
            string nombre = txtNombre.Value;
            string correo = txtDireccionEmail.Value;
            string contenidoCorreo = txtContenidoEmail.Value;



            //EnviarEmail(nombre, correo, contenidoCorreo);
            EnviaFacturaMail(1);

            txtNombre.Value = "";
            txtDireccionEmail.Value = "";
            txtContenidoEmail.Value = "";

        }
    }
}
