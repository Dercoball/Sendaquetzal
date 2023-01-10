<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Plataforma.pages.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">



    <link rel="shortcut icon" href="../img/sq.jpg">




    <link rel="stylesheet" href="../css/custom.css">
    <link rel="stylesheet" href="../css/app/bootstrap.css">
    <link rel="stylesheet" href="../css/app/spinelli.css">


    <!-- Custom Fonts -->
    <link href="../vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css">

    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
    <meta name="theme-color" content="#7952b3">




    <title>Senda Quetzal</title>
</head>
<body>
    <header>
            <nav class="navbar navbar-default navbar-fixed-top">
            <div class="container">
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#myNavbar">
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                   <a href="Home.aspx"><img src="../img/sq2.png" 
                        class="img-responsive"
                        style="padding-right:50px"
                        <%--style="border-radius: 50%;--%> 
                        width: 70px; height: 70px;" 
                        />
                  </a>
                </div>
                <div class="collapse navbar-collapse" id="myNavbar">
                    <ul class="nav navbar-nav">
                        <li <%--class="active"--%>
                            ><a href="Home.aspx" style="font-size:150%"><strong>Inicio</strong></a></li>
                        <li><a href="../pages/Aboutus.aspx" style="font-size:150%"><strong>Nosotros</strong></a></li>
                    </ul>

                    <ul class="nav navbar-nav navbar-right links2">
                        <li class="active" ><a href="../pages/Login.aspx" style="font-size:150%"><strong>Iniciar sesión</strong></a></li>
                    </ul>

                    <ul class="nav navbar-nav navbar-right redes">
                        <li><a href="#">
                            <img src="../img/facebook-logo-button.png" class="img-social" /></a></li>
                        <li><a href="#">
                            <img src="../img/i-twitter.png" class="img-social" /></a></li>
                        <li><a href="#">
                            <img src="../img/instagram.png" class="img-social" /></a></li>
                        <li style="padding-right:50px"><a href="#">
                            <img src="../img/mail.png" class="img-social" /></a></li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>


    <img src="../img/brand2.png" class="img-responsive  center-block" style="margin-top: 70px;" />


    <div class="container">
        <div class="panel">
            <div class="panel-body col-md-6 col-md-offset-3">

                <div style="color: black; margin-left: auto; margin-right: auto; margin-top: 10px; background-color:#dcdcdc">

                    <h3 style="margin-top: 30px">Iniciar Sesión</h3>

                    <div class="panel-body">

                        <form class="form-signin" id="frmLogin" runat="server">

                            <asp:HiddenField ID="txtPath" runat="server"></asp:HiddenField>


                            <div>

                                <div class="text-center">
                                </div>

                                <div class="row">

                                    <div class="form-group col-md-12">
                                        <label>Usuario</label>
                                        <asp:TextBox ID="inputEmail" runat="server" class="form-control flex-fill"
                                            required="required" data-required-error='Requerido'
                                            placeholder="Ingrese su nombre de usuario"></asp:TextBox>
                                        <div class="help-block with-errors"></div>

                                    </div>
                                </div>

                                <div class="row">
                                    <div class="form-group col-md-12">
                                        <label>Constraseña</label>
                                        <asp:TextBox ID="inputPassword" runat="server" class="form-control" placeholder="Ingrese su contraseña"
                                            required="required" data-required-error='Requerido'
                                            TextMode="Password"></asp:TextBox>
                                        <div class="help-block with-errors"></div>

                                    </div>
                                </div>

                                <asp:Panel ID="panelError" runat="server" Visible="false">
                                    <div class="alert alert-warning alert-dismissible" role="alert">
                                        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <strong></strong>No se encontró el usuario y contraseña.
                                    </div>
                                </asp:Panel>

                                <asp:Panel ID="panelCamposVacios" runat="server" Visible="false">
                                    <div class="alert alert-danger alert-dismissible" role="alert">
                                        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                        <strong></strong>Debe ingresar su nombre de usuario y contraseña.
                                    </div>
                                </asp:Panel>

                                <div class="row text-right">
                                    <div class="col-md-12 d-flex">
                                        <asp:Button ID="Entrar" runat="server" class="btn btn-lg btn-primary"
                                            Text="Entrar" OnClick="Entrar_Click" UseSubmitBehavior="false" />
                                    </div>
                                </div>

                            </div>




                        </form>

                    </div>
                </div>

            </div>


        </div>
    </div>


    <footer>
       <div class="container">

            <div class="col-md-3 col-md-offset-3 text-center">
                
                <ul>
                    <li><a href="../pages/AboutUs.aspx"><span class="glyphicon glyphicon-play-circle margen-derecho" aria-hidden="true"></span>Acerca de nosotros</a></li>
                    <li><a href="../pages/TermsAndConditions.aspx"><span class="glyphicon glyphicon-play-circle margen-derecho" aria-hidden="true"></span>Términos y condiciones</a></li>
                </ul>
            </div>
            <div class="col-md-3 text-center">
                
                <ul>
                    <li><a href="../pages/NoticeOfPrivacy.aspx"><span class="glyphicon glyphicon-play-circle margen-derecho" aria-hidden="true"></span>Aviso de privacidad</a></li>
                    <li><a href="../pages/FAQS.aspx"><span class="glyphicon glyphicon-play-circle  margen-derecho" aria-hidden="true"></span>Preguntas frecuentes</a></li>
                    <li><a href="../pages/Tutorials.aspx"><span class="glyphicon glyphicon-play-circle  margen-derecho" aria-hidden="true"></span>Tutoriales</a></li>
                </ul>
            </div>
        </div>
    </footer>



    <script src="../js/app/jquery.min.js"></script>
    <script src="../js/app/bootstrap.min.js"></script>
    <script type="text/javascript" src="../js/app/formValidation.js"></script>



    <script src="../js/validator.js"></script>
    <script src="../js/app/login.js"></script>

</body>
</html>

