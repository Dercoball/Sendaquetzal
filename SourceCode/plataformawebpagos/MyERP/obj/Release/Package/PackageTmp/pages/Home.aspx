<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Plataforma.pages.Home" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Senda Quetzal</title>
    
    <link rel="shortcut icon" href="../img/sq.jpg" />
    <meta name="theme-color" content="#7952b3" />

    <link rel="stylesheet" href="../css/app/bootstrap.css" />
    <link rel="stylesheet" href="../css/app/spinelli.css" />
    <link href="../vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />

    <link rel="stylesheet" href="../css/custom.css" />

    </head>
<body>
    <form id="frm" runat="server">
        <nav class="navbar navbar-default navbar-fixed-top">
            <div class="container">
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#myNavbar">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <a class="navbar-brand" href="Home.aspx">
                        <img src="../img/sq2.png" alt="Senda Quetzal Logo" style="height: 50px; margin-top: -15px;" />
                    </a>
                </div>
                <div class="collapse navbar-collapse" id="myNavbar">
                    <ul class="nav navbar-nav">
                        <li class="active"><a href="Home.aspx" style="font-size:150%"><strong>Inicio</strong></a></li>
                        <li><a href="../pages/Aboutus.aspx" style="font-size:150%"><strong>Nosotros</strong></a></li>
                    </ul>
                    <ul class="nav navbar-nav navbar-right">
                        <li><a href="../pages/Login.aspx" style="font-size:150%"><strong>Iniciar sesión</strong></a></li>
                    </ul>
                    <ul class="nav navbar-nav navbar-right">
                        <li><a href="#"><img src="../img/facebook-logo-button.png" class="img-social" /></a></li>
                        <li><a href="#"><img src="../img/i-twitter.png" class="img-social" /></a></li>
                        <li><a href="#"><img src="../img/instagram.png" class="img-social" /></a></li>
                        <li><a href="#"><img src="../img/mail.png" class="img-social" /></a></li>
                    </ul>
                </div>
            </div>
        </nav>

        <main style="padding-top: 70px;">
            <section class="banner text-center">
                <div class="container">
                    <img src="../img/brand2.png" class="img-responsive" style="display: inline-block;" />
                </div>
            </section>

            <section class="pasos" style="padding: 20px 0;">
                <div class="container">
                    <div class="row text-center">
                        <div class="col-md-4">
                            <img src="../img/registro.png" width="50px" />
                            <h3>Registro</h3>
                        </div>
                        <div class="col-md-4">
                            <img src="../img/aprobacion.png" width="50px" />
                            <h3>Aprobación</h3>
                        </div>
                        <div class="col-md-4">
                            <img src="../img/listo.png" width="50px" />
                            <h3>¡Listo!</h3>
                        </div>
                    </div>
                </div>
            </section>

            <section class="apprestar" style="padding: 40px 0; background-color: #f7f7f7;">
                <div class="container">
                    <div class="row">
                        <div class="col-md-8 col-md-offset-2" style="background-color:#fff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">
                            <h2 class="text-center" style="margin-bottom: 30px;"><strong>Solicita un préstamo ahora</strong></h2>
                            <div class="form-group">
                                <input type="text" class="form-control" runat="server" id="txtNombre" placeholder="Nombre completo" required="required" />
                            </div>
                            <div class="form-group">
                                <input type="text" class="form-control" runat="server" id="txtDireccionEmail" placeholder="Correo electrónico" required="required" />
                            </div>
                            <div class="form-group">
                                <textarea rows="5" id="txtContenidoEmail" runat="server" placeholder="Comentario" class="form-control" required="required"></textarea>
                            </div>
                            <div class="text-center">
                                <asp:Button ID="Button2" CssClass="btn btn-primary btn-lg" runat="server" Text="Enviar Solicitud" OnClick="Button2_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </main>

        <div class="modal fade" id="panelMensajes" tabindex="-1" role="dialog">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Información</h4>
                    </div>
                    <div class="modal-body">
                        <p id="lblMensajeInfo"></p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">ACEPTAR</button>
                    </div>
                </div>
            </div>
        </div>

        <footer style="padding: 20px 0;">
            <div class="container">
                <div class="row">
                    <div class="col-md-4 col-md-offset-2 text-center">
                        <ul class="list-unstyled">
                            <li><a href="../pages/AboutUs.aspx">Acerca de nosotros</a></li>
                            <li><a href="../pages/TermsAndConditions.aspx">Términos y condiciones</a></li>
                        </ul>
                    </div>
                    <div class="col-md-4 text-center">
                        <ul class="list-unstyled">
                            <li><a href="../pages/NoticeOfPrivacy.aspx">Aviso de privacidad</a></li>
                            <li><a href="../pages/FAQS.aspx">Preguntas frecuentes</a></li>
                            <li><a href="../pages/Tutorials.aspx">Tutoriales</a></li>
                        </ul>
                    </div>
                </div>
            </div>
        </footer>

        <script src="../js/app/jquery.min.js"></script>
        <script src="../js/app/bootstrap.min.js"></script>
        <script type="text/javascript" src="../js/app/formValidation.js"></script>
        <script src="../js/validator.js"></script>
    </form>
</body>
</html>