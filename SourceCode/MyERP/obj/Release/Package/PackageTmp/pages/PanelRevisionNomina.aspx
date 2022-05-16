<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelRevisionNomina.aspx.cs" Inherits="Plataforma.pages.PanelRevisionNomina" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Plataforma</title>
    <meta name="description" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="robots" content="all,follow" />
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="../vendor/bootstrap/css/bootstrap.min.css" />
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
    <link rel="stylesheet" href="../css/style.default.css">
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="../css/custom.css">
    <!-- Favicon-->
    <link rel="shortcut icon" href="../img/favicon.ico">
    <!-- Tweaks for older IEs-->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script><![endif]-->
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
                <div class="sidenav-header-logo">
                    <a href="Index.aspx"
                        class="brand-small text-center"><strong>C</strong><strong class="text-primary">B</strong></a>
                </div>
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



        <section class="forms">

            <nav>
                <div class="nav nav-tabs" id="nav-tab" role="tablist">
                    <a class="nav-item nav-link active" id="nav-obras-tab" data-toggle="tab"
                        href="#nav-obras" role="tab" aria-controls="nav-obras" aria-selected="true">Obras</a>

                    <a class="nav-item nav-link" id="nav-empleados-tab" data-toggle="tab"
                        href="#nav-empleados" role="tab" aria-controls="nav-empleados" aria-selected="false">Revisión</a>
                </div>
            </nav>

            <div class="tab-content" id="nav-tabContent">



                <div class="tab-pane fade show active" id="nav-obras" role="tabpanel" aria-labelledby="nav-obras-tab">




                    <div class="container-fluid">
                        <header>
                            <h1 class="h3 display">Revisión Pre-Nómina</h1>
                        </header>




                        <div class="table-responsive">

                            <table style="width: 100%!important;"
                                class="table table-striped table-bordered table-hover"
                                id="tableObras">


                                <thead>

                                    <th>Clave</th>
                                    <th>Nombre Obra</th>
                                    <th>Creada por</th>
                                    <th>Status</th>
                                    <th></th>

                                </thead>
                                <tbody>
                                </tbody>
                            </table>

                        </div>

                        <div class="row mt-3 mb-3">

                            <div class=" col-md-6 text-left">
                            </div>

                            <div class=" col-md-6 text-right">
                                <button id="btnFinalizar" class="btn btn-primary"><i class="fa fa-send"></i>Generar csv y finalizar</button>
                            </div>

                        </div>


                    </div>


                </div>


                <div class="tab-pane fade" id="nav-empleados" role="tabpanel" aria-labelledby="nav-empleados-tab">
                    <div class="container-fluid">

                        <header>
                            <h1 class="h3 display">Revisión Nómina</h1>
                        </header>


                        <div class="table-responsive">

                            <table style="width: 100%!important; font-size: 11px;"
                                class="table table-striped table-bordered table-hover "
                                id="table">


                                <thead>
                                    <th>Clave</th>
                                    <th>Nombre</th>
                                    <th>Puesto</th>
                                    <th>Departamento</th>
                                    <th>Sem.</th>
                                    <th>J</th>
                                    <th>V</th>
                                    <th>S</th>
                                    <th>D</th>
                                    <th>L</th>
                                    <th>M</th>
                                    <th>M</th>
                                    <th>Días</th>
                                    <th>H.E.</th>
                                    <th>Detalle H.E.</th>
                                    <th>Viáticos</th>
                                    <th>Observaciones</th>

                                </thead>
                                <tbody>
                                </tbody>
                            </table>

                        </div>

                        <div class="row mt-3 mb-3">

                            <div class=" col-md-6 text-left">
                                <button id="btnCancelar" class="btn btn-secondary"><i class="fa fa-arrow-circle-left"></i>Volver a lista de obras</button>
                            </div>

                            <div class=" col-md-6 text-right">

                                <button id="btnAprobar" class="btn btn-primary"><i class="fa fa-send"></i>Aprobar</button>
                            </div>

                        </div>

                    </div>

                </div>
            </div>


        </section>



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



    <div id="panelMensajes" class="modal fade" role="dialog" data-backdrop="static">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Información</h4>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <p>
                        <span id="spnMensajes"></span>
                    </p>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" id="btnAceptarPanelMensajes" data-dismiss="modal">Aceptar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelEliminar" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p>
                        Se eliminará el registro seleccionado. ¿Desea continuar?
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnEliminarAceptar" class="btn btn-danger" data-dismiss="modal">Aceptar</button>
                    <button id="btnEliminarCancelar" class="btn btn-primary" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelAprobar" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p>
                        La prenomina seleccionada será aprobada. ¿Desea continuar?
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnAceptarAprobar" class="btn btn-danger" data-dismiss="modal">Aceptar</button>
                    <button class="btn btn-primary" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelFinalizar" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p>
                        La prenominas de la lista serán finalizadas y se generará el csv. ¿Desea continuar?
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnAceptarFinalizar" class="btn btn-danger" data-dismiss="modal">Aceptar</button>
                    <button class="btn btn-primary" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="panelLoading" tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">

                <div class="modal-body">
                    <div class="progress">
                        <div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar"
                            aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%">
                            Procesando...
                        </div>
                    </div>
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
    <script src="../vendor/jquery-validation/jquery.validate.min.js"></script>
    <script src="../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>



    <script src="../js/front.js"></script>

    <!-- DataTables JavaScript -->
    <script src="../vendor/datatables/js/jquery.dataTables.min.js"></script>
    <script src="../vendor/datatables-plugins/dataTables.bootstrap.min.js"></script>
    <script src="../vendor/datatables-responsive/dataTables.responsive.js"></script>
    <script src="../vendor/datatables/js/dataTables.bootstrap4.js"></script>

    <link href="../vendor/datatables-responsive/dataTables.responsive.css" rel="stylesheet" />
    <link href="../vendor/datatables/css/jquery.dataTables.css" rel="stylesheet" />
    <link href="../vendor/datatables/css/dataTables.bootstrap4.css" rel="stylesheet" />
    <link href="../vendor/datatables-plugins/dataTables.bootstrap.css" rel="stylesheet" />

    <script src="../vendor/datatables-plugins/Buttons-1.5.1/js/dataTables.buttons.min.js"></script>
    <script src="../vendor/datatables-plugins/Buttons-1.5.1/js/buttons.html5.min.js"></script>





    <script src="../js/validator.js"></script>
    <script src="../js/cp/nomina/panel_aprobar_nomina.js"></script>
    <script src="../js/cp/general.js"></script>

    <!-- Toastr style -->
    <link href="../css/toastr.min.css" rel="stylesheet">
    <script src="../js/toastr.min.js"></script>

</body>

</html>
