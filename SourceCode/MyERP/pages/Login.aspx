<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Plataforma.pages.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">



    <link rel="shortcut icon" href="../img/favicon.ico">




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




    <title></title>

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
                    <img src="../img/sq.jpg" class="img-responsive" style="border-radius: 50%; width: 70px; height: 70px;" />
                </div>
                <div class="collapse navbar-collapse" id="myNavbar">
                    <ul class="nav navbar-nav">
                        <li><a href="Home.aspx">Inicio</a></li>
                        <li><a href="Aboutus.aspx">Nosotros</a></li>
                    </ul>

                    <ul class="nav navbar-nav navbar-right links2">
                        <li class="active"><a href="Login.aspx">Iniciar sesión</a></li>
                    </ul>

                    <ul class="nav navbar-nav navbar-right redes">
                        <li><a href="#">
                            <img src="../img/facebook-logo-button.png" class="img-social" /></a></li>
                        <li><a href="#">
                            <img src="../img/i-twitter.png" class="img-social" /></a></li>
                        <li><a href="#">
                            <img src="../img/instagram.png" class="img-social" /></a></li>
                        <li><a href="#">
                            <img src="../img/mail.png" class="img-social" /></a></li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>


    <img src="../img/brand.png" class="img-responsive  center-block" style="margin-top: 70px;" />


    <section class="nosotros">
        <div class="container">
            <div class="row">
                <div>

                    <div class="panel panel-success" style="width: 400px; margin-left: auto; margin-right: auto;">

                        <h3>Login</h3>

                        <div class="panel-body">

                            <form class="form-signin" id="form1" runat="server">

                                <asp:HiddenField ID="txtPath" runat="server" ></asp:HiddenField>


                                <div>

                                    <div class="text-center mb-4">
                                    </div>

                                    <div class="form-label-group mb-1">
                                        <asp:TextBox ID="inputEmail" runat="server" class="form-control"
                                            placeholder="Usuario" required autofocus></asp:TextBox>
                                    </div>

                                    <div class="form-label-group mb-1">
                                        <asp:TextBox ID="inputPassword" runat="server" class="form-control" placeholder="Contraseña" 
                                             required TextMode="Password"></asp:TextBox>
                                    </div>

                                    <asp:Button ID="Entrar" runat="server" class="btn btn-lg btn-primary btn-block btnAzu" 
                                        Text="Entrar" OnClick="Entrar_Click" UseSubmitBehavior="false" />


                                </div>

                                <div class="alert alert-warning alert-dismissible" role="alert" id="alert-login-fail">
                                    <span id="lblMensajes"></span>

                                </div>

                            </form>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <footer>
        <div class="container">

            <div class="col-md-3 col-md-offset-3 text-center">
                <h2>Acerca de...</h2>
                <ul>

                    <li><a href="apprestar.aspx"><span class="glyphicon glyphicon-play-circle " aria-hidden="true"></span>Acerca de...</a></li>
                    <li><a href="login.aspx"><span class="glyphicon glyphicon-play-circle " aria-hidden="true"></span>Iniciar sesión</a></li>
                </ul>
            </div>
            <div class="col-md-3 text-center">
                <h2>Ayuda</h2>
                <ul>

                    <li><a href="faqs.aspx"><span class="glyphicon glyphicon-play-circle " aria-hidden="true"></span>Preguntas frecuentes</a></li>
                    <li><a href="terminosycondiciones.aspx"><span class="glyphicon glyphicon-play-circle " aria-hidden="true"></span>Términos y condiciones</a></li>
                    <li><a href="avisodeprivacidad.aspx"><span class="glyphicon glyphicon-play-circle " aria-hidden="true"></span>Aviso de privacidad</a></li>
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

