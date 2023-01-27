<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoanRequest.aspx.cs" Inherits="Plataforma.pages.LoanRequest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title></title>
    <meta name="description" content="" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <meta name="robots" content="all,follow" />
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="/vendor/bootstrap/css/bootstrap.min.css" />
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="/vendor/font-awesome/css/font-awesome.min.css" />
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="/css/fontastic.css" />
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700" />
    <!-- jQuery Circle-->
    <link rel="stylesheet" href="/css/grasp_mobile_progress_circle-1.0.0.min.css" />
    <!-- Custom Scrollbar-->
    <link rel="stylesheet" href="/vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css" />
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="/css/style.sea.css" />
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="/css/custom.css" />
    <!-- Favicon-->
    <link rel="shortcut icon" href="/img/sq.jpg" />
    <!-- Tweaks for older IEs-->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script><![endif]-->
</head>
<body>
    <form class="form-signin" id="form1" runat="server">
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
                <div id="panelTabla">
                    <!-- Inicio Titulo -->
                    <div class="row mt-4 mb-3 border-bottom col-12">
                        <h3>PRESTAMOS</h3>
                    </div>
                    <!-- Fin titulo -->

                    <div class="row">
                        <div class="col-12">
                            <!-- Inicio botones -->
                            <div class="row mb-4 col-12">
                                <a href="/pages/Loans/LoanApprove.aspx" class="btn btn-primary mr-1 rounded" id="btnNuevo">
                                    <i class="fa fa-money mr-1"></i>NUEVO PRESTAMO
                                </a>
                                <button class="btn btn-primary rounded mr-2" id="btnBuscar">
                                    <i class="fa fa-search mr-1"></i>BUSCAR
                                </button>
                                <button class="btn btn-secondary rounded" id="btnLimpiar">
                                    <i class="fa fa-eraser mr-1"></i>LIMPIAR
                                </button>
                            </div>
                            <!-- Fin botones -->

                            <!-- Inicio tabla de prestamos -->
                            <form id="frmFiltros">
                                <div class="table-responsive">
                                    <table class="table table-striped table-bordered table-hover w-100" id="table">
                                        <thead>
                                            <tr>
                                                <td></td>
                                                <td>
                                                    <input id="txtNoPrestamoMaximaBusqueda" type="number" class="form-control w-100" placeholder="No. prestamos max." />
                                                    <input id="txtNoPrestamoMinimoBusqueda" type="number" class="form-control w-100 mt-2" placeholder="No. prestamo min." />
                                                </td>
                                                <td class="p-1">
                                                    <input id="txtNombreClienteBusqueda" class="form-control w-100 mt-2" placeholder="Nombre" />
                                                </td>
                                                <td>
                                                    <input id="txtMontoPrestamoMaximaBusqueda" type="number" class="form-control w-100" placeholder="Monto max." />
                                                    <input id="txtMontoPrestamoMinimoBusqueda" type="number" class="form-control w-100 mt-2" placeholder="Monto min." />
                                                </td>
                                                <td>
                                                    <input id="dtpFechaPrestamoMaximaBusqueda" type="date" class="form-control w-100" />
                                                    <input id="dtpFechaPrestamoMinimoBusqueda" type="date" class="form-control w-100 mt-2" />
                                                </td>
                                                <td>
                                                    <input id="dtpFechaUltimaPrestamoMaximaBusqueda" type="date" class="form-control w-100" />
                                                    <input id="dtpFechaUltimaPrestamoMinimoBusqueda" type="date" class="form-control w-100 mt-2" />
                                                </td>
                                                <td>
                                                    <input id="txtRechazosPrestamoMaximaBusqueda" type="number" class="form-control w-100" placeholder="Rechazos max." />
                                                    <input id="txtRechazosPrestamoMinimoBusqueda" type="number" class="form-control w-100 mt-2" placeholder="Rechazo min." />
                                                </td>
                                                 <td>
                                                    <input id="txtAvalPrestamoMaximaBusqueda" type="number" class="form-control w-100" placeholder="Aval max." />
                                                    <input id="txtAvalPrestamoMinimoBusqueda" type="number" class="form-control w-100 mt-2" placeholder="Aval min." />
                                                </td>
                                              <td>
                                                <select class="form-control w-100" id="cboStatus">
                                                </select>
                                            </td>
                                            </tr>
                                            <tr>
                                                <th>Folio</th>
                                                <th>No prestamos</th>
                                                <th>Nombre del cliente</th>
                                                <th>Monto</th>
                                                <th>Primera solicitud</th>
                                                <th>Ultima solicitud</th>
                                                <th>Rechazo</th>
                                                <th>Aval</th>
                                                <th>Status</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                        <tfoot>
                                            <tr>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td id="thMontoTotal"></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                                <td></td>
                                            </tr>
                                        </tfoot>
                                    </table>
                                </div>
                                <!-- Fin tabla de prestamos -->
                            </form>
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


    <div id="panelMensajeControlado" class="modal fade" role="dialog" data-backdrop="static" style="margin-top: 200px;">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Información</h4>
                </div>
                <div class="modal-body">
                    <p>
                        <span id="spnMensajeControlado"></span>
                    </p>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" id="btnAceptarPanelMensajeControlado">Aceptar</button>
                </div>
            </div>
        </div>
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

    <div class="modal fade" id="panelLoading" tabindex="-1" role="dialog" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">

                <div class="modal-body">
                    <div class="progress">
                        <div class="progress-bar progress-bar-striped progress-bar-animated"
                            role="progressbar" aria-valuenow="100" aria-valuemin="0"
                            aria-valuemax="100" style="width: 100%">
                            Procesando...
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- JavaScript files-->
    <script src="/vendor/jquery/jquery.min.js"></script>
    <script src="/vendor/bootstrap/js/bootstrap.min.js"></script>


    <!-- DataTables JavaScript -->
    <script src="/vendor/datatables/js/jquery.dataTables.min.js"></script>
    <script src="/vendor/datatables-plugins/dataTables.bootstrap.min.js"></script>
    <script src="/vendor/datatables-responsive/dataTables.responsive.js"></script>
    <script src="/vendor/datatables/js/dataTables.bootstrap4.js"></script>

    <link href="/vendor/datatables-responsive/dataTables.responsive.css" rel="stylesheet" />
    <link href="/vendor/datatables/css/jquery.dataTables.css" rel="stylesheet" />
    <link href="/vendor/datatables/css/dataTables.bootstrap4.css" rel="stylesheet" />
    <link href="/vendor/datatables-plugins/dataTables.bootstrap.css" rel="stylesheet" />

    <script src="/vendor/datatables-plugins/Buttons-1.5.1/js/dataTables.buttons.min.js"></script>
    <script src="/vendor/datatables-plugins/Buttons-1.5.1/js/buttons.html5.min.js"></script>



    <script src="/js/validator.js"></script>
    <script src="/js/app/loans/loanlist.js"></script>
    <script src="/js/app/general.js"></script>


    <!-- Toastr style -->
    <link href="/css/toastr.min.css" rel="stylesheet" />
    <script src="/js/toastr.min.js"></script>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>

</body>
</html>
