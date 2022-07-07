<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="Plataforma.pages.Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />



    <link rel="shortcut icon" href="../img/sq.jpg" />




    <link rel="stylesheet" href="../css/custom.css" />
    <link rel="stylesheet" href="../css/app/bootstrap.css" />
    <link rel="stylesheet" href="../css/app/spinelli.css" />


    <!-- Custom Fonts -->
    <link href="../vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css" />

    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
    <meta name="theme-color" content="#7952b3" />




    <title>Senda Quetzal</title>

    <style>
      
    </style>

</head>
<body>

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
                    <li class="active"><a href="Home.aspx">Inicio</a></li>
                    <li><a href="../pages/Aboutus.aspx">Nosotros</a></li>
                </ul>

                <ul class="nav navbar-nav navbar-right links2">
                    <li><a href="../pages/Login.aspx">Iniciar sesión</a></li>
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


    <img src="../img/brand.png" class="img-responsive  center-block" style="margin-top: 70px;" />


    <%--
    <div id="myCarousel" class="carousel slide carousel-fade" data-ride="carousel" data-interval="100000">

        <div class="carousel-inner">
            <div class="item active">
            </div>




        </div>

    </div>



    <section class="opcion">
        <div class="container">

            <div class="col-md-12 text-center">
                <img src="../img/logo_apprestar.svg" width="300px">
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div id="main-slider" class="owl-carousel owl-theme">
                        <div class="item">
                            <div style="text-align: right!important">
                                <h2>Somos tu mejor opción</h2>
                            </div>
                            <img src="../img/img1.jpg" alt="" class="img-fluid">
                        </div>
                        <div class="item">
                            <h2>Calcula tu préstamo</h2>
                            <img src="../img/img2.jpg" alt="" class="img-fluid">
                        </div>
                        <div class="item">
                            <h2>Tu préstamo a un clic</h2>
                            <img src="../img/img3.jpg" alt="" class="img-fluid">
                        </div>
                        <div class="item">
                            <h2>Somos tu mejor opción</h2>
                            <img src="../img/img1.jpg" alt="" class="img-fluid">
                        </div>
                        <div class="item">
                            <h2>Calcula tu préstamo</h2>
                            <img src="../img/img2.jpg" alt="" class="img-fluid">
                        </div>
                        <div class="item">
                            <h2>Tu préstamo a un clic</h2>
                            <img src="../img/img3.jpg" alt="" class="img-fluid">
                        </div>
                    </div>
                    <!-- /#main-slider-->
                </div>
            </div>


        </div>
    </section>--%>


    <section class="pasos">
        <div class="container">
            <%--    <div class="col-md-12">
                <h2>Tres sencillos pasos para obtener tu préstamo</h2>
            </div>--%>
            <div class="row">
                <div class="col-md-4">
                    <div class="text-center">
                        <img src="../img/registro.jpg">
                    </div>
                    <h1>Registro</h1>
                </div>

                <div class="col-md-4">
                    <div class="text-center">
                        <img src="../img/aprobacion.jpg">
                    </div>
                    <h1>Aprobación</h1>
                </div>

                <div class="col-md-4">
                    <div class="text-center">
                        <img src="../img/registro.jpg">
                    </div>
                    <h1>¡Listo!</h1>
                </div>
            </div>

        </div>
    </section>

    <section class="apprestar">
        <div class="container">

            <div class="col-md-12">

                <form role="form" id="frm" name="frm" data-toggle="validator">

                    <h1>Contáctanos</h1>
                    <div class="form-group">
                        <div class="input-group">

                            <span class="input-group-addon"><i class="glyphicon glyphicon-envelope"></i></span>
                            <input type="text" class="form-control input" name="txtDireccionEmail" id="txtDireccionEmail" placeholder="Email" required data-required-error=' ' email />
                            <div class="help-block with-errors"></div>

                        </div>
                    </div>
                    <div class="form-group">


                        <textarea rows="10" id="txtContenidoEmail" name="txtContenidoEmail" placeholder="Contenido" class="area" required data-required-error='Por favor ingrese el contenido de su mensaje.' /></textarea>
						<div class="help-block with-errors"></div>

                    </div>
                    <div>
                        <button class="btnenviar" id="btnEnviar">Enviar</button>
                    </div>
                </form>

            </div>
        </div>



    </section>



    <div class="modal fade" id="panelMensajes" tabindex="-1" role="dialog">
        <div class="modal-dialog" role="document">
            <div class="modal-content">

                <div class="modal-header">
                    <p>Información</p>
                </div>
                <div class="modal-body">
                    <span id="lblMensajeInfo" style="color: black;"></span>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">ACEPTAR</button>
                </div>
            </div>
        </div>
    </div>

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


    <!-- jQuery -->
    <script src="../js/app/jquery.min.js"></script>
    <script src="../js/app/bootstrap.min.js"></script>
    <script type="text/javascript" src="../js/app/formValidation.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <%--<script src="../vendor/bootstrap5/js/bootstrap.js"></script>--%>

    <!-- Metis Menu Plugin JavaScript -->
    <%--<script src="../vendor/metisMenu/metisMenu.min.js"></script>--%>


    <script src="../js/validator.js"></script>
    <%--<script src="../js/cp/login.js"></script>--%>
</body>
</html>

