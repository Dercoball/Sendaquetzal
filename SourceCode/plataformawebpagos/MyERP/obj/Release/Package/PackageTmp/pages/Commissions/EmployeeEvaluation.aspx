<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmployeeEvaluation.aspx.cs" Inherits="Plataforma.pages.EmployeeEvaluation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title></title>
    <meta name="description" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="robots" content="all,follow" />
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="../../vendor/bootstrap/css/bootstrap.min.css" />
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="../../vendor/font-awesome/css/font-awesome.min.css">
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="../../css/fontastic.css">
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700">
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="../../css/style.sea.css">
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="../../css/custom.css">
    <!-- Favicon-->
    <link rel="shortcut icon" href="../../img/sq.jpg">
    <!-- Tweaks for older IEs-->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script><![endif]-->
</head>


<body>


    <form class="form-signin" id="form1" runat="server" action="EmployeeEvaluation.aspx">
        <asp:HiddenField ID="txtUsuario" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdTipoUsuario" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdUsuario" runat="server"></asp:HiddenField>


    </form>


    <!-- Side Navbar -->
    <nav class="side-navbar">
        <div class="side-navbar-wrapper">
            <!-- Sidebar Header    -->
            <div class="sidenav-header d-flex align-items-center justify-content-center">
                <!-- User Info-->
                <div class="sidenav-header-inner text-center">
                    <i class="fa fa-user-o fa-4x"></i>
                    <h3 class="h5" id="nombreUsuario"></h3>
                    <span id="nombreTipoUsuario"></span>
                </div>
                <!-- Small Brand information, appears on minimized sidebar-->
                <div class="sidenav-header-logo"><a href="Index.aspx" class="brand-small text-center"><strong>S</strong><strong class="text-primary">Q</strong></a></div>
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

                    <div class="navbar-header">
                        <a id="toggle-btn" href="#" class="menu-btn"><i class="icon-bars"></i></a>
                        <a href="Index.aspx" class="navbar-brand">
                            <div class="brand-text d-none d-md-inline-block">
                                <span></span><strong class="text-primary"><span id="systemName" /></strong>
                            </div>
                        </a>
                    </div>
                    <ul class="nav-menu list-unstyled d-flex flex-md-row">
                        <li class="nav-item"><a href="#" class="nav-link logout" onclick="window.top.location.href = '/pages/Logout.aspx'">
                            <span class="d-none d-sm-inline-block">Salir</span><i class="fa fa-sign-out"></i></a>
                        </li>
                    </ul>


                </div>
            </nav>
        </header>


        <section class="forms">

            <div class="container-fluid">

                <div id="panelTabla" class="secciones">


                    <header>
                        <h1 class="h3 display" id="paginaName"></h1>

                    </header>

                    <div id="panelFiltro">

                        <div class="mt-2 mb-4">

                            <div class="row mt-3 mb-3">

                                <div class="form-group col-md-3">
                                    <label for="comboPlaza">
                                        Plaza
                                    </label>
                                    <select id="comboPlaza" class="form-control">
                                    </select>
                                </div>

                                <div class="form-group col-md-3">
                                    <label for="comboEjecutivo">
                                        Ejecutivo
                                    </label>
                                    <select id="comboEjecutivo" class="form-control">
                                    </select>
                                </div>

                                <div class="form-group col-md-3">
                                    <label for="comboSupervisor">
                                        Supervisor
                                    </label>
                                    <select id="comboSupervisor" class="form-control">
                                    </select>
                                </div>

                                <div class="form-group col-md-3">
                                    <label for="comboPromotor">
                                        Promotor
                                    </label>
                                    <select id="comboPromotor" class="form-control">
                                    </select>
                                </div>


                            </div>


                            <div class="row mt-3 mb-3">

                                <div class=" col-md-3">
                                    <button class="btn btn-outline btn-primary" id="btnFiltrar"><i class="fa fa-search mr-1"></i>Filtrar</button>
                                </div>

                            </div>
                        </div>

                    </div>



                    <div class="table-responsive">

                        <table style="width: 100%!important;" class="table table-striped table-bordered table-hover table-sm" id="table">

                            <thead>
                                <tr>
                                    <th>Promotor</th>
                                    <th>Supervisor</th>
                                    <th>Ejecutivo</th>
                                    <th>Plaza</th>
                                    <th>Fecha de ingreso</th>
                                    <th>Comisión</th>
                                    <th>% de nivel</th>
                                    <th></th>
                                </tr>
                            </thead>

                            <tbody>
                            </tbody>
                        </table>

                    </div>
                </div>

                <div id="panelForm" class="secciones">


                      <header>
                        <span class="h3 display" id="nombreEmpleado"></span>
                    </header>

                    <div class="table-responsive">

                        <table style="width: 100%!important;" class="table table-striped table-bordered table-hover " id="tableReglas">


                            <thead>
                                <tr>
                                    <th id="nombreComision"></th>
                                    <th>Ponderación</th>
                                    <th>Calificación</th>
                                    <th>Completado</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>

                    </div>

                    <div class="row mt-3 mb-3">

                        <div class=" col-md-6 text-left">
                            <button id="btnCancelar" class="btn btn-secondary"><i class="fa fa-arrow-circle-left mr-1"></i>Volver</button>
                        </div>

                        <div class=" col-md-6 text-right">
                            <button id="btnGuardar" class="btn btn-primary deshabilitable"><i class="fa fa-save mr-1"></i>Guardar</button>
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



    <div id="panelMensajes" class="modal fade" role="dialog" data-backdrop="static" style="margin-top: 200px;">
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
                    <button class="btn btn-primary" data-dismiss="modal">Aceptar</button>
                </div>
            </div>
        </div>
    </div>




    <!-- JavaScript files-->
    <script src="../../vendor/jquery/jquery.min.js"></script>
    <script src="../../vendor/bootstrap/js/bootstrap.min.js"></script>


    <!-- DataTables JavaScript -->
    <script src="../../vendor/datatables/js/jquery.dataTables.min.js"></script>
    <script src="../../vendor/datatables-plugins/dataTables.bootstrap.min.js"></script>
    <script src="../../vendor/datatables-responsive/dataTables.responsive.js"></script>
    <script src="../../vendor/datatables/js/dataTables.bootstrap4.js"></script>

    <link href="../../vendor/datatables-responsive/dataTables.responsive.css" rel="stylesheet" />
    <link href="../../vendor/datatables/css/jquery.dataTables.css" rel="stylesheet" />
    <link href="../../vendor/datatables/css/dataTables.bootstrap4.css" rel="stylesheet" />
    <link href="../../vendor/datatables-plugins/dataTables.bootstrap.css" rel="stylesheet" />

    <script src="../../vendor/datatables-plugins/Buttons-1.5.1/js/dataTables.buttons.min.js"></script>
    <script src="../../vendor/datatables-plugins/Buttons-1.5.1/js/buttons.html5.min.js"></script>


    <script src="../../js/validator.js"></script>
    <script src="../../js/app/commissions/employee_evaluation.js"></script>
    <script src="../../js/app/general.js"></script>



    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet" />
    <script src="../../js/toastr.min.js"></script>


</body>
</html>
