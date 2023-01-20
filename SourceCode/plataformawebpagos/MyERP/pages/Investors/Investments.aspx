<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Investments.aspx.cs" Inherits="Plataforma.pages.Investments" %>

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
                        <h3>INVERSIONES</h3>
                    </div>
                    <!-- Fin titulo -->

                    <div class="row">
                        <div class="col-12">
                            <!-- Inicio botones -->
                            <div class="row mb-4 col-12">
                                <button class="btn btn-primary mr-1 rounded" id="btnNuevo">
                                    <i class="fa fa-money mr-1"></i>NUEVA INVERSIÓN
                                </button>
                                <button class="btn btn-primary rounded mr-2" id="btnBuscar">
                                    <i class="fa fa-search mr-1"></i>BUSCAR
                                </button>
                                <button class="btn btn-secondary rounded" id="btnLimpiar">
                                    <i class="fa fa-eraser mr-1"></i>LIMPIAR
                                </button>
                            </div>
                            <!-- Fin botones -->

                            <!-- Inicio tabla de inversiones -->
                            <div class="table-responsive">
                                <table class="table table-striped table-bordered table-hover w-100" id="table">
                                    <thead>
                                        <tr>
                                            <td class="p-1">
                                                <input id="txtNombreInversionistaBusqueda" class="form-control w-100 mt-2" placeholder="Nombre" />
                                            </td>
                                            <td style="width: 170px">
                                                <input id="txtMontoMaximaBusqueda" type="number" class="form-control w-100" placeholder="Monto max." />
                                                <input id="txtMontoMinimoBusqueda" type="number" class="form-control w-100 mt-2" placeholder="Monto min." />
                                            </td>
                                            <td style="width: 120px">
                                                <input id="txtUtilidadMaximaBusqueda" type="number" class="form-control w-100" placeholder="Utilidad max." />
                                                <input id="txtUtilidadMinimaBusqueda" type="number" class="form-control w-100 mt-2" placeholder="Utilidad min." />
                                            </td>
                                            <td style="width: 100px">
                                                <input id="txtPlazoMaximoBusqueda" type="number" class="form-control w-100" placeholder="Plazo max." />
                                                <input id="txtPlazoMinimoBusqueda" type="number" class="form-control w-100 mt-2" placeholder="Plazo min." />
                                            </td>
                                            <td>
                                                <select class="form-control campo-combo" id="cboStatus">
                                                </select>
                                            </td>
                                            <td style="width: 110px">
                                                <input id="dtpIngresoMaximoBusqueda" type="date" class="form-control w-100" placeholder="Ingreso max." />
                                                <input id="dtpIngresoMinimoBusqueda" type="date" class="form-control w-100 mt-2" placeholder="Ingreso min." />
                                            </td>
                                            <td style="width: 110px">
                                                <input id="dtpRetiroMaximoBusqueda" type="date" class="form-control w-100" placeholder="Retiro max." />
                                                <input id="dtpRetiroMinimoBusqueda" type="date" class="form-control w-100 mt-2" placeholder="Retiro min." />
                                            </td>
                                            <td></td>
                                        </tr>
                                        <tr>
                                            <th>Nombre inversionista</th>
                                            <th>Monto inversión</th>
                                            <th>Utilidad</th>
                                            <th>Plazo</th>
                                            <th>Estatus</th>
                                            <th>Ingreso</th>
                                            <th>Retiro</th>
                                            <th>Acciones</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                    <tfoot>
                                        <tr>
                                            <th></th>
                                            <th class="text-center" id="thMontoTotalInversion"></th>
                                            <th></th>
                                            <th></th>
                                            <th></th>
                                            <th></th>
                                            <th></th>
                                            <th></th>
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                            <!-- Fin tabla de inversiones -->
                        </div>
                    </div>
                </div>


                <!--Inicio menu de inversiones -->
                <nav id="nvMenuInversiones" class="row mt-4">
                    <!-- Inicio Titulo -->
                    <div class="row mt-4 mb-3 border-bottom col-12">
                        <h3>REGISTRO DE INVERSIONES</h3>
                    </div>
                    <!-- Fin titulo -->

                    <div class="nav nav-tabs" id="nav-tab" role="tablist">
                        <a class="nav-item nav-link active tabAccion" id="tabInvertir" data-id="1" data-toggle="tab" href="#pnAccion" role="tab" aria-controls="nav-client" aria-selected="true">Invertir</a>
                        <a class="nav-item nav-link tabAccion" id="tabRetirar" data-toggle="tab" data-id="2" href="#pnAccion" role="tab" aria-controls="nav-aval" aria-selected="false">Retirar</a>
                    </div>
                </nav>
                <!--Fin menu de inversiones -->

                <div class="tab-content">
                    <div class="tab-pane fade show active" id="pnAccion" role="tabpanel" data-id="1">
                        <!--Inicio panel de Inversiones -->

                        <div id="panelForm">
                            <form role="form" id="frm" name="frm" class="mt-4">
                                <div>
                                    <!-- Inicio Inversionista y Fecha Actual-->
                                    <div class="row mb-3">
                                        <div class="col-4">
                                            <label class="form-label">Inversionista</label>
                                            <select class="form-control campo-combo" id="comboInversionista" required="required" data-required-error='Requerido'>
                                            </select>
                                            <div class="help-block with-errors"></div>
                                        </div>
                                        <div class="col-4" id="dvFechaActual">
                                            <label class="form-label">Fecha Actual</label>
                                            <label class="form-control" id="lblFechaActual"></label>
                                            <div class="help-block with-errors"></div>
                                        </div>
                                        <div class="col-4" id="dvFechaInversion">
                                            <label class="form-label">Fecha Inversion</label>
                                            <select class="form-control campo-combo" id="cboFechaInversionesRetiro" required="required" data-required-error='Requerido'>
                                            </select>
                                            <div class="help-block with-errors"></div>
                                        </div>
                                    </div>
                                    <!-- Fin Inversionista-->

                                    <!-- Inicio Monto inversión-->
                                    <div class="row mb-3">
                                        <div class="col-4">
                                            <label class="form-label">Monto inversión  </label>
                                            <input type="number" step="any" class="form-control"
                                                id="txtMontoAInvertir"
                                                required="required"
                                                data-required-error='Requerido'
                                                placeholder="$" />
                                            <div class="help-block with-errors"></div>
                                        </div>
                                    </div>
                                    <!-- Fin % Monto inversión-->

                                    <!-- Inicio % Utilidad y % Utilidad (Pesos)-->
                                    <div class="row mb-3">
                                        <div class="col-4">
                                            <label class="form-label">% Utilidad</label>
                                            <div class="input-group">
                                                <div class="input-group-prepend">
                                                    <span class="input-group-text">%</span>
                                                </div>
                                                <input type="number" placeholder="Porcentaje" step="any" class="form-control" id="txtPorcentajeUtilidad" required="required" data-required-error='Requerido' />
                                            </div>
                                            <div class="help-block with-errors"></div>
                                        </div>

                                        <div class="col-4">
                                            <label class="form-label">Utilidad (Pesos)</label>
                                            <div class="input-group">
                                                <div class="input-group-prepend">
                                                    <span class="input-group-text">$</span>
                                                </div>
                                                <input type="number" placeholder="$" step="any" class="form-control" id="txtUtilidadPesos" readonly="true" required="required" data-required-error='Requerido' />
                                            </div>
                                            <div class="help-block with-errors"></div>
                                        </div>
                                    </div>
                                    <!-- Inicio % Utilidad y % Utilidad (Pesos)-->

                                    <!-- Inicio Plazo y Fecha para retiro-->
                                    <div class="row mb-3">
                                        <div class="col-4">
                                            <label class="form-label">Plazo (días)</label>
                                            <input type="number" placeholder="Plazo" step="any" class="form-control" id="txtPlazo" required="required" data-required-error='Requerido' />
                                            <div class="help-block with-errors"></div>
                                        </div>

                                        <div class="col-4">
                                            <label class="form-label">Fecha para retiro</label>
                                            <label class="form-control" id="lblFechaVencimiento"></label>
                                        </div>
                                    </div>
                                    <!-- Fin Plazo y Fecha para retiro-->

                                    <!-- Inicio Inversión + Utilidad -->
                                    <div class="row mb-3">
                                        <div class="col-4">
                                            <label class="form-label">Inversión + Utilidad</label>
                                            <div class="input-group">
                                                <div class="input-group-prepend">
                                                    <span class="input-group-text">$</span>
                                                </div>
                                                <input type="number" placeholder="$" step="any" class="form-control" readonly="true" id="txtUtilidadInversion" required="required" data-required-error='Requerido' />
                                            </div>
                                            <div class="help-block with-errors"></div>
                                        </div>
                                    </div>
                                    <!-- Fin Utilidad -->

                                    <!-- Inicio  Utilidad  acumulada y total a retirar-->
                                    <div class="row mb-3" id="dvCamposRetiro">
                                        <div class="col-4">
                                            <label class="form-label">Utilidad Acumulada</label>
                                            <div class="input-group">
                                                <div class="input-group-prepend">
                                                    <span class="input-group-text">$</span>
                                                </div>
                                                <input type="number" placeholder="$" step="any" class="form-control" readonly="true" id="txtUtilidadAcumulada" required="required" data-required-error='Requerido' />
                                            </div>
                                            <div class="help-block with-errors"></div>
                                        </div>
                                        <div class="col-4">
                                            <label class="form-label">Total a retirar</label>
                                            <div class="input-group">
                                                <div class="input-group-prepend">
                                                    <span class="input-group-text">$</span>
                                                </div>
                                                <input type="number" placeholder="$" step="any" class="form-control" readonly="true" id="txtRetiro" required="required" data-required-error='Requerido' />
                                            </div>
                                            <div class="help-block with-errors"></div>
                                        </div>
                                    </div>
                                    <!-- Fin Utilidad -->

                                    <!-- Inicio Comprobante de pago -->
                                    <div class="row mb-4">
                                        <div class="col-4">
                                            <label class="form-label">Comprobante de pago</label>
                                            <div class="input-group">
                                                <div class="input-group-prepend">
                                                    <span class="input-group-text" id="inputGroupFileAddon01">Archivo</span>
                                                </div>
                                                <div class="custom-file">
                                                    <input type="file" class="custom-file-input file-comprobante" data-tipo="1"
                                                        required="required" data-required-error='Requerido' />
                                                    <label class="custom-file-label">Selecciona archivo</label>
                                                    <div class="help-block with-errors"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <!-- Fin Comprobante de pago -->
                                </div>
                            </form>
                            <!-- Inicio acciones-->
                            <div class="row mt-4 text-center">
                                <div class="col-8">
                                    <button id="btnCancelar" class="btn btn-secondary rounded"><i class="fa fa-ban mr-1"></i>CANCELAR</button>
                                    <button id="btnGuardar" class="btn btn-success deshabilitable rounded"><i class="fa fa-save mr-1"></i>REGISTRAR</button>
                                </div>
                            </div>
                            <!-- Fin acciones-->
                        </div>

                        <!--Fin panel de inversiones -->
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

    <div id="panelFiltros" class="modal fade" role="dialog" data-backdrop="static">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Filtros</h4>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" id="btnAplicarFiltros" data-dismiss="modal">Aceptar</button>
                </div>
            </div>
        </div>
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
    <script src="../../js/app/investors/investments.js"></script>
    <script src="../../js/app/general.js"></script>

    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet" />
    <script src="../../js/toastr.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
</body>
</html>
