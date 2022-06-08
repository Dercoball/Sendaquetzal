<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermsAndConditions.aspx.cs" Inherits="Plataforma.pages.TermsAndConditions" %>

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
                    <img src="../img/sq.jpg" class="img-responsive" style="border-radius: 50%; width: 70px; height: 70px;" />
                </div>
                <div class="collapse navbar-collapse" id="myNavbar">
                    <ul class="nav navbar-nav">
                        <li><a href="Home.aspx">Inicio</a></li>
                        <li class="active"><a href="Aboutus.aspx">Nosotros</a></li>
                    </ul>

                    <ul class="nav navbar-nav navbar-right links2">
                        <li><a href="Login.aspx">Iniciar sesión</a></li>
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



    <section class="nosotros">
        <div class="container">
            <div class="offset-2">

                <h3>Términos y condiciones</h3>


                <span id="loadItems"></span>

                <%--   <% foreach (var item in itemsFaqs)
                    { %>
                <div>
                    <div style="margin-top: 20px;">
                        <strong>
                            <%= item.Pregunta %>
                        </strong>
                    </div>
                    <div>
                        <%= item.Respuesta %>
                    </div>

                    <% } %>
                    <!--End the for loop -->--%>
            </div>
        </div>
    </section>

    <footer>
        <div class="container">

            <div class="col-md-3 col-md-offset-3 text-center">
                <h2>Acerca de...</h2>
                <ul>
                    <li><a href="../pages/AboutUs.aspx"><span class="glyphicon glyphicon-play-circle margen-derecho" aria-hidden="true"></span>Acerca de...</a></li>
                    <li><a href="../pages/Login.aspx"><span class="glyphicon glyphicon-play-circle margen-derecho" aria-hidden="true"></span>Iniciar sesión</a></li>
                </ul>
            </div>
            <div class="col-md-3 text-center">
                <h2>Ayuda</h2>
                <ul>

                    <li><a href="../pages/FAQS.aspx"><span class="glyphicon glyphicon-play-circle  margen-derecho" aria-hidden="true"></span>Preguntas frecuentes</a></li>
                    <li><a href="../pages/TermsAndConditions.aspx"><span class="glyphicon glyphicon-play-circle margen-derecho" aria-hidden="true"></span>Términos y condiciones</a></li>
                    <li><a href="../pages/NoticeOfPrivacy.aspx"><span class="glyphicon glyphicon-play-circle margen-derecho" aria-hidden="true"></span>Aviso de privacidad</a></li>
                </ul>
            </div>
        </div>
    </footer>


    <script src="../js/app/jquery.min.js"></script>
    <script src="../js/app/bootstrap.min.js"></script>
    <script type="text/javascript" src="../js/app/formValidation.js"></script>



    <script src="../js/validator.js"></script>
    <script src="../js/app/termsandconditions.js"></script>

</body>
</html>

