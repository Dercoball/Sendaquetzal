<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MantenimientoDashboard.aspx.cs" Inherits="Plataforma.pages.MantenimientoDashboardResumen" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>Plataforma</title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="robots" content="all,follow">
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="../vendor/bootstrap/css/bootstrap.min.css">
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="../vendor/font-awesome/css/font-awesome.min.css">
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="../css/fontastic.css">
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700">
    <!-- jQuery Circle-->
    <link rel="stylesheet" href="../css/grasp_mobile_progress_circle-1.0.0.min.css">
    <!-- Custom Scrollbar-->
    <link rel="stylesheet" href="../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css">
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="../css/style.default.css" id="theme-stylesheet">
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="../css/custom.css">
    <!-- Favicon-->
    <link rel="shortcut icon" href="../img/favicon.ico">
    <!-- Tweaks for older IEs-->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script><![endif]-->

    <style>
        .bd-placeholder-img {
            font-size: 1.125rem;
            text-anchor: middle;
            -webkit-user-select: none;
            -moz-user-select: none;
            user-select: none;
        }

        @media (min-width: 768px) {
            .bd-placeholder-img-lg {
                font-size: 3.5rem;
            }
        }

    </style>


</head>
<body>
    <!-- Side Navbar -->
    <nav class="side-navbar">
        <div class="side-navbar-wrapper">
            <!-- Sidebar Header    -->
            <div class="sidenav-header d-flex align-items-center justify-content-center">
                <!-- User Info-->
                <div class="sidenav-header-inner text-center">
                    <i class="fa fa-user-o fa-4x"></i>
                    <h2 class="h5" id="nombreUsuario"></h2>
                    <span></span>
                </div>
                <!-- Small Brand information, appears on minimized sidebar-->
                <div class="sidenav-header-logo"><a href="Index.aspx" class="brand-small text-center"><strong>S</strong><strong class="text-primary">G</strong></a></div>
            </div>
            <!-- Sidebar Navigation Menus-->
            <div class="main-menu">
                <h5 class="sidenav-heading">MENÚ</h5>
                <ul id="side-main-menu" class="side-menu list-unstyled">
                </ul>
            </div>
        </div>
    </nav>
    <div class="page">
        <!-- navbar-->
        <header class="header">





            <nav class="navbar">
                <div class="container-fluid">


                    <span id="spnMenuSuperior"></span>


                    <!-- Languages dropdown    -->

                </div>
            </nav>
        </header>




        <div class="container-fluid mt-3">


            <div class="row">

                <div class="col-md-8">

                    <div class="wrapper recent-updated">
                        <div id="new-updates_" class="card-header d-flex justify-content-between align-items-center">
                            <h2 class="h5 display"><a data-toggle="collapse" data-parent="#accordion" href="#upadtes-box" aria-expanded="true"
                                aria-controls="upadtes-box">Trabajos en atención</a></h2>
                            <i class="fa fa-angle-down"></i>
                        </div>
                        <div class="card-body" id="contenedorTrabajosEnAtencion">
                            <div class="row" id="trabajosEnAtencion">
                            </div>

                        </div>
                    </div>


                    <%--                    <div class="card">

                        <div class="wrapper recent-updated">

                            <h2 class="h5 display"><a data-toggle="collapse" data-parent="#accordion" href="#upadtes-box" aria-expanded="true"
                                aria-controls="upadtes-box">Trabajos en atención</a></h2>

                        </div>

                        <div class="card body">
                           
                        </div>
                    </div>--%>
                </div>

                <div class="col-md-4">

                    <!-- Recent Updates            -->
                    <%--        <div class="wrapper recent-updated">
                            <div id="new-updates" class="card-header d-flex justify-content-between align-items-center">
                                <h2 class="h5 display"><a data-toggle="collapse" data-parent="#accordion" href="#upadtes-box" aria-expanded="true"
                                    aria-controls="upadtes-box">Trabajos finalizados (mes actual)</a></h2>
                                <i class="fa fa-angle-down"></i>
                            </div>
                            <div id="upadtes-box" role="tabpanel" class="collapse show">
                                <ul class="news list-unstyled" id="itemsTotales">
                                    <!-- Item-->
                                </ul>
                            </div>
                        </div>--%>

                    <div class="wrapper recent-updated">
                        <div id="new-update" class="card-header d-flex justify-content-between align-items-center">
                            <h2 class="h5 display"><a data-toggle="collapse" data-parent="#accordion" href="#upadtes-box" aria-expanded="true"
                                aria-controls="upadtes-box">Ot´s por hacer</a></h2>
                            <i class="fa fa-angle-down"></i>
                        </div>
                        <div class="card-body" style="height: 600px; overflow-y: scroll;" id="contenedorItemsPorHacer">
                            <div class="row" id="itemsOtsPorHacer">
                                <!-- Item-->
                            </div>

                        </div>

                    </div>


                </div>

            </div>

            <div class="row">

                <div class="col-md-8">

                    <%--     <div class="wrapper recent-updated">
                        <div class="card-header d-flex justify-content-between align-items-center">
                            <h2 class="h5 display"><a data-toggle="collapse" data-parent="#accordion" href="#upadtes-box" aria-expanded="true"
                                aria-controls="upadtes-box">Colaboraciones</a></h2>
                            <i class="fa fa-angle-down"></i>
                        </div>
                        <div class="card-body">
                            <div class="row" id="trabajosEnColaboracion">
                            </div>

                        </div>
                    </div>--%>
                </div>

                <%--Reloj--%>
                <div class="col-md-4">

                    <div class="card fondo-negro">
                        <div class="card-body">
                            <div class="row">
                                <div class="ml-auto mr-auto fondo-negro">
                                    <span id="fechaHora"></span>
                                </div>
                            </div>
                        </div>


                    </div>
                </div>


            </div>
        </div>


        <footer class="main-footer">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-6">
                        <p class='nombre-empresa'></p>
                    </div>
                    <div class="col-sm-6 text-right">
                        <p class="empresa-copy"><a href="#" class="external"></a></p>
                    </div>
                </div>
            </div>
        </footer>

    </div>

    <div id="paneResetStatus" class="modal fade" role="dialog" data-backdrop="static">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Confirmación</h4>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <p id="msgResetStatus">
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnResetStatusAceptar" class="btn btn-primary" data-dismiss="modal">Aceptar</button>
                    <button class="btn btn-default" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>


    <!-- JavaScript files-->
    <script src="../vendor/jquery/jquery.min.js"></script>
    <script src="../vendor/popper.js/umd/popper.min.js"> </script>
    <script src="../vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="../js/grasp_mobile_progress_circle-1.0.0.min.js"></script>
    <script src="../vendor/jquery.cookie/jquery.cookie.js"> </script>
    <%--<script src="../vendor/chart.js/Chart.min.js"></script>--%>
    <script src="../vendor/jquery-validation/jquery.validate.min.js"></script>
    <script src="../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>
    <%--<script src="../js/charts-home.js"></script>--%>

    <script src="../js/front.js"></script>


    <script src="../js/validator.js"></script>
    <script src="../js/cp/general.js"></script>
    <script src="../js/cp/mantenimiento/dashboard_resumen.min.js"></script>

    <!-- Toastr style -->
    <link href="../css/toastr.min.css" rel="stylesheet">
    <script src="../js/toastr.min.js"></script>
</body>
</html>
