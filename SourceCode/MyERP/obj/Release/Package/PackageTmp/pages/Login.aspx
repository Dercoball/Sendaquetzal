<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Plataforma.pages.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">



    <link rel="shortcut icon" href="../img/favicon.ico">




    <!-- Bootstrap Core CSS -->
    <link href="../vendor/bootstrap/css/bootstrap.min.css" rel="stylesheet">

    <!-- MetisMenu CSS -->
    <link href="../vendor/metisMenu/metisMenu.min.css" rel="stylesheet">


    <link rel="stylesheet" href="../css/custom.css">

    <!-- Custom Fonts -->
    <link href="../vendor/font-awesome/css/font-awesome.min.css" rel="stylesheet" type="text/css">

    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->




    <title>Plataforma</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

</head>
<body>


    <div class="container">
        <div class="row">
            <div style="width: 50%; margin-left: auto; margin-right: auto; margin-top: 100px;">
                <div class="login-panel panel panel-default">
                    <div class="panel-heading">
                        <h3 class="panel-title">Login</h3>
                    </div>
                    <div class="panel-body">

                        <form id="frmLogin" role="form" name="frmLogin" data-toggle="validator">

                            <div class="text-center">
                            </div>


                            <fieldset>

                                <div class="form-group">
                                    <label for="txtUsuario">Usuario</label>
                                    <input type="text" class="form-control" id="txtUsuario"
                                        placeholder="Nombre de usuario"
                                        required data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>

                                    <small id="emailHelp" class="form-text text-muted"></small>

                                </div>
                                <div class="form-group">
                                    <label for="txtPassword">Password</label>
                                    <input type="password" class="form-control" id="txtPassword" placeholder="Password"
                                        required data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>




                                <div class="form-group">
                                </div>
                                <div class="form-group">

                                    <!-- Change this to a button or input when using this as a form -->
                                    <button id="btnLogin" class="btn btn-lg btn-primary btn-block">
                                    <img src="../img/loading4.gif" style="width: 2em; height: 2em; display: none;"
                                        id="imgLoading" />

                                        Login</button>

                                    
                                </div>

                            </fieldset>
                        </form>

                        <div class="alert alert-dismissible" role="alert" id="alert-operacion-fail">
                            <strong>¡Información!</strong>
                            <label id="lblMensajes"></label>
                        </div>


                    </div>
                </div>
            </div>
        </div>
    </div>




    <!-- jQuery -->
    <script src="../vendor/jquery/jquery.min.js"></script>

    <!-- Bootstrap Core JavaScript -->
    <script src="../vendor/bootstrap/js/bootstrap.min.js"></script>

    <!-- Metis Menu Plugin JavaScript -->
    <script src="../vendor/metisMenu/metisMenu.min.js"></script>


    <script src="../js/validator.js"></script>
    <script src="../js/cp/login.js"></script>

</body>
</html>

