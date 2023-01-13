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
               <img src="../img/sq2.png" 
                    class="img-responsive"
                    style="padding-right:50px"
                    <%--style="border-radius: 50%;--%> 
                    width: 70px; height: 70px;" 
                    />
               
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

    <table>
        <tr>
            <%--<th>
                <h1 style="color:#FFFFFF">TABlE TEST</h1>
            </th>--%>
            <th>
                <img src="../img/brand2.png" class="img-responsive" style="margin-top: 70px;" />
            </th>
            <%--<th>
                <h1 style="color:#FFFFFF">TABlE TEST</h1>
            </th>--%>
            
            <th>
                <section class="pasos">
                <div class="container">
                    <%--<h1 style="color:#FFFFFF">TABlE TEST</h1>--%>

                    <div class="row">
                <div class="col-md-2">
                    <div class="text-center">
                        <img src="../img/registro.png"  width="50px">
                    </div>
                    <h1>Registro</h1>
                </div>

                <div class="col-md-2">
                    <div class="text-center">
                        <img src="../img/aprobacion.png" width="50px">
                    </div>
                    <h1>Aprobación</h1>
                </div>

                <div class="col-md-2">
                    <div class="text-center">
                        <img src="../img/listo.png" width="50px">
                    </div>
                    <h1>¡Listo!</h1>
                </div>
            </div>
                </div>
                    </section>

                 <section class="apprestar">
        <div class="container" >

            <div class="col-md-6" style="background-color:#dcdcdc">

                <form id="frm" runat="server">

                    <h1 style="text-align:center; font-size:200%" ><strong>Solicita un préstamo ahora</strong></h1>
                    <div class="form-group">
                        <div 
                            <%--class="input-group"--%>
                            >

                            <%--<span class="input-group-addon"><i class="glyphicon glyphicon-user"></i></span>--%>
                            <input type="text" class="form-control input" runat="server" name="txtNombre" id="txtNombre" placeholder="Nombre" required data-required-error=' ' nombre />
                            <div class="help-block with-errors"></div>
                            <%--<span class="input-group-addon"><i class="glyphicon glyphicon-envelope"></i></span>--%>
                            <input type="text" class="form-control input" runat="server" name="txtDireccionEmail" id="txtDireccionEmail" placeholder="Email" required data-required-error=' ' email />
                            <div class="help-block with-errors"></div>

                        </div>
                    </div>
                    <div class="form-group">


                        <textarea rows="10" id="txtContenidoEmail" runat="server" name="txtContenidoEmail" placeholder="Comentario" class="area" required data-required-error='Por favor ingrese el contenido de su mensaje.' /></textarea>
						<div class="help-block with-errors"></div>

                    </div>
                    <div class="text-center">
                       <%-- <button class="btnenviar" runat="server" OnClick="Button2_Click">Enviar</button>--%>
                        <asp:Button ID="Button2" Class="btnenviar" runat="server" Text="Enviar" OnClick="Button2_Click" />
                    </div>
                </form>

            </div>
        </div>



    </section>
                
            </th>
           
        </tr>
    </table>




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

