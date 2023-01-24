<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Investors.aspx.cs" Inherits="Plataforma.pages.Investors" %>

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
    <link rel="stylesheet" href="../../vendor/bootstrap/css/bootstrap.min.css" />
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="../../vendor/font-awesome/css/font-awesome.min.css" />
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="../../css/fontastic.css" />
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700" />
    <!-- jQuery Circle-->
    <link rel="stylesheet" href="../../css/grasp_mobile_progress_circle-1.0.0.min.css" />
    <!-- Custom Scrollbar-->
    <link rel="stylesheet" href="../../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css" />
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="../../css/style.sea.css" />
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="../../css/custom.css" />
    <!-- Favicon-->
    <link rel="shortcut icon" href="../../img/sq.jpg" />
    <!-- Tweaks for older IEs-->
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
                <div class="sidenav-header-logo"><a href="#" class="brand-small text-center"><strong>S</strong><strong class="text-primary">Q</strong></a></div>
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
                            <h3>INVERSIONISTAS</h3>
                        </div>
                        <!-- Fin titulo -->
                 
                        <!-- Inicio Boton de nuevo inversionista -->
                        <div class="row mb-4 col-12">
                                <button class="btn btn-primary mr-1 rounded" id="btnNuevo">
                                    <i class="fa fa-user-plus mr-1"></i>NUEVO INVERSIONISTA
                                </button>
                                    <button class="btn btn-secondary rounded" id="btnBuscar">
                                    <i class="fa fa-search mr-1"></i>BUSCAR
                                </button>
                        </div>
                        <!-- Fin Boton de nuevo inversionista -->

                        <!-- Inicio filtrado listado de inverisonistas -->
                        <div class="row mb-3">
                            <div class="col-lg-3">
                                <label class="form-label">Nombre</label>
                                <input id="txtNombreBusqueda" class="form-control" placeholder="Nombre" />
                            </div>
                            <div class="col-lg-3">
                                <label class="form-label">RFC</label>
                                <input id="txtRFCBusqueda" class="form-control" placeholder="RFC" />
                            </div>
                            <div class="col-lg-3">
                                <label class="form-label">Utilidad max.</label>
                                <input id="txtUtilidadMaximaBusqueda" type="number" class="form-control" placeholder="Utilidad max." />
                            </div>
                                <div class="col-lg-3">
                                <label class="form-label">Registro max.</label>
                                <input id="txtFechaRegistroMaximaBusqueda" type="date" class="form-control" placeholder="Registro max." />
                            </div>
                        </div>
                        <div class="row mb-4">
                                <div class="col-lg-3">
                            </div>
                            <div class="col-lg-3">
                            </div>
                                <div class="col-lg-3">
                                <label class="form-label">Utilidad min.</label>
                                <input id="txtUtilidadMinimaBusqueda" type="number" class="form-control" placeholder="Utilidad min." />
                            </div>
                                <div class="col-lg-3">
                                <label class="form-label">Registro min.</label>
                                <input id="txtFechaRegistroMinimaBusqueda" type="date" class="form-control" placeholder="Registro min." />
                            </div>
                        </div>
                        <!-- Fin filtrado listado de inverisonistas -->

                        <!-- Inicio listado de inverisonistas -->            
                        <div class="table-responsive">
                            <table class="table table-striped table-bordered table-hover w-100" id="table">
                                <thead>
                                    <tr>
                                        <th>No.</th>
                                        <th>Nombre</th>
                                        <th>RFC</th>
                                        <th class="text-center">Utilidad sugerida</th>
                                        <th class="text-center">Estatus</th> 
                                        <th class="text-center">Registro</th> 
                                        <th class="text-center">Acciones</th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                        </div>
                        <!-- fin listado de inverisonistas -->
                </div>

                <div id="panelForm">
                    <form role="form" id="frm" name="frm">
                        <div class="form-body">
                            <!-- Inicio Titulo -->
                            <div class="row mt-4 mb-3 border-bottom col-12">
                                <h3  id="spnTituloForm"></h3>
                            </div>
                            <!-- Fin titulo -->

                            <!-- Inicio Nombre corto -->
                            <div class="row mb-3">
                                <div class="col-5">
                                      <label for="txtNombre">Nombre corto</label>
                                    <input type="text" class="form-control" id="txtNombre" required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>
                            </div>
                            <!-- Fin Nombre corto -->

                            <!-- Inicio Razon social -->
                            <div class="row mb-3">
                                  <div class="col-5">
                                      <label for="txtRazonSocial"> Razon Social</label>
                                     <input type="text" class="form-control" id="txtRazonSocial" required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>
                            </div>
                            <!-- Fin Razon social -->

                            <!-- Inicio RFC -->
                            <div class="row mb-3">
                                  <div class="col-5">
                                      <label for="txtRFC"> RFC</label>
                                       <input type="text" class="form-control" id="txtRFC" title="txtRFC" required="required" data-required-error='Requerido' 
                                        pattern="[A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?""  
                                        data-pattern-error="Debe ingresar un RFC valido."/>
                                    <div class="help-block with-errors"></div>
                                </div>
                            </div>
                            <!-- Inicio RFC -->

                            <!-- Inicio % Interes anual-->
                             <div class="row mb-3">
                                  <div class="col-5">
                                       <label for="txtPorcentajeUtilidadSugerida">% Utilidad sugerida </label>
                                        <input type="number" step="any" class="form-control" id="txtPorcentajeUtilidadSugerida" required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>
                            </div>
                            <!-- Fin % Interes anual- -->
                        </div>
                    </form>
                    
                    <!-- Inicio acciones-->
                    <div class="row mt-3 text-center">
                        <div class="col-5">
                            <button id="btnCancelar" class="btn btn-secondary rounded"><i class="fa fa-ban mr-1"></i>CANCELAR</button>
                            <button id="btnGuardar" class="btn btn-success deshabilitable rounded"><i class="fa fa-save mr-1"></i>REGISTRAR</button>
                            <button id="btnModalSuspender" class="btn btn-warning deshabilitable text-white rounded"><i class="fa fa-lock mr-1"></i>SUSPENDER</button>
                        </div>
                    </div>
                    <!-- Fin acciones-->
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
                    <span id="mensajeEliminar"></span>
                </div>
                <div class="modal-footer">
                    <button id="btnEliminarAceptar" class="btn btn-warning" data-dismiss="modal">Eliminar</button>
                    <button id="btnEliminarCancelar" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelSuspender" class="modal fade" role="dialog" data-backdrop="static">
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
                    <span id="mensajeSuspender"></span>
                </div>
                <div class="modal-footer">
                    <button id="btnSuspenderAceptar" class="btn btn-warning" data-dismiss="modal">Suspender</button>
                    <button id="btnSuspenderCancelar" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
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
    <script src="../../js/app/investors/investors.js"></script>
    <script src="../../js/app/general.js"></script>

    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet" />
    <script src="../../js/toastr.min.js"></script>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
    
</body>
</html>
