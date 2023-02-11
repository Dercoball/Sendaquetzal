<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PaymentsOverdue.aspx.cs" Inherits="Plataforma.pages.Loans.PaymentsOverdue" %>

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
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="../../vendor/bootstrap/css/bootstrap.min.css" />
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="../../vendor/font-awesome/css/font-awesome.min.css">
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="../../css/fontastic.css">
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700">
    <!-- jQuery Circle-->
    <link rel="stylesheet" href="../../css/grasp_mobile_progress_circle-1.0.0.min.css">
    <!-- Custom Scrollbar-->
    <link rel="stylesheet" href="../../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css">
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


    <style>

        #tableSolicitudes thead tr th{
            min-width: 40px;
        }
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
                <header>
                    <h1 class="h3 display" id="paginaName">Vencidas</h1>
                </header>

                <div id="panelTabla">
                    <div class="card">
                        <div class="card-header">
                            <div id="panelFiltro">
                                <div class="container-fluid">
                                    <div class="mt-2 mb-4">
                                        <div class="row align-items-end">
                                            <div class="col fv-row">
                                                <!--begin::Label-->
                                                <label class="d-flex align-items-center fs-6 fw-semibold form-label mb-2">
                                                    <span>PLAZA</span>
                                                </label>
                                                <!--end::Label-->
                                                <select class="form-control" id="cmbPlaza" name="cmbPlaza">
                                                    <option value="0">Todos</option>
                                                </select>
                                            </div>

                                            <div class="col fv-row">
                                                <!--begin::Label-->
                                                <label class="d-flex align-items-center fs-6 fw-semibold form-label mb-2">
                                                    <span>EJECUTIVO</span>
                                                </label>
                                                <!--end::Label-->
                                                <select class="form-control" id="cmbEjecutivo" name="cmbEjecutivo">
                                                    <option value="0">Todos</option>
                                                </select>
                                            </div>

                                            <div class="col fv-row">
                                                <!--begin::Label-->
                                                <label class="d-flex align-items-center fs-6 fw-semibold form-label mb-2">
                                                    <span>SUPERVISOR</span>
                                                </label>
                                                <!--end::Label-->
                                                <select class="form-control" id="cmbSupervisor" name="cmbSupervisor">
                                                    <option value="0">Todos</option>
                                                </select>
                                            </div>

                                            <div class="col fv-row">
                                                <!--begin::Label-->
                                                <label class="d-flex align-items-center fs-6 fw-semibold form-label mb-2">
                                                    <span>PROMOTOR</span>
                                                </label>
                                                <!--end::Label-->
                                                <select class="form-control" id="cmbPromotor" name="cmbPromotor">
                                                    <option value="0">Todos</option>
                                                </select>
                                            </div>

                                            <div class="col">
                                                <div class="d-flex align-items-center ">
                                                    <button class="btn btn-outline btn-primary" id="btnFiltrar"><i class="fa fa-search mr-1"></i>Filtrar</button></div>

                                            </div>
                                        </div>



                                    </div>
                                </div>
                                <hr />

                            </div>
                        </div>

                        <div class="card-body">
                            <div class="table-responsive">
                                <table style="width: 100%!important;" class="table table-bordered table-hover table-sm" id="table">
                                    <thead class="thead-light">
                                        <tr>
                                            <%--<th>No.</th>--%>
                                            <th>
                                                Nombre cliente<br />
                                                <input placeholder="Nombre" />
                                            </th>
                                            <th>
                                                Nombre aval<br />
                                                <input placeholder="Nombre" />
                                            </th>
                                            <th>
                                                Prestamo<br />
                                                <input id="pmax" type="number" placeholder="MAX" style="width:120px;" /><br />
                                                <input id="pmin" type="number" placeholder="MIN" style="width:120px;" />
                                            </th>
                                            <th>
                                                Fecha<br />
                                                <input id="fpmax" type="date" placeholder="MAX" /><br />
                                                <input id="fpmin" type="date" placeholder="MIN" />
                                            </th>
                                            <th>Fallas</th>
                                            <th>
                                                Pagos<br />
                                                <input id="fmax" type="number" placeholder="MAX" style="width:120px;" /><br />
                                                <input id="fmin" type="number" placeholder="MIN" style="width:120px;" />
                                            </th>
                                            <th>
                                                Monto<br />
                                                <input id="mmax" type="number" placeholder="MAX" style="width:120px;" /><br />
                                                <input id="mmin" type="number" placeholder="MIN" style="width:120px;" />
                                            </th>
                                            <th>
                                                Total<br />
                                                <input id="tmax" type="number" placeholder="MAX" style="width:120px;" /><br />
                                                <input id="tmin" type="number" placeholder="MIN" style="width:120px;" />
                                            </th>
                                            <th>
                                                Abonado<br />
                                                <input id="amax" type="number" placeholder="MAX" style="width:120px;" /><br />
                                                <input id="amin" type="number" placeholder="MIN" style="width:120px;" />
                                            </th>
                                            <th>
                                                Estatus<br />
                                                <select id="estatus">
                                                    <option value="">Todos</option>
                                                    <option value="Rechazado">Rechazado</option>
                                                    <option value="Aprobado">Aprobado</option>
                                                    <option value="Pagado">Pagado</option>
                                                    <option value="Condonado">Condonado</option>
                                                </select>
                                            </th>
                                            <th>
                                                <input type="checkbox" name="select_all" value="1" id="checked-select-all" checked="checked" />
                                                Select</th>
                                            <th>Acción</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                    </tbody>

                                    <tfoot class="thead-light">
                                        <tr>
                                            <th></th>
                                            <th></th>
                                            <th></th>
                                            <th></th>
                                            <th></th>
                                            <th></th>
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
                        </div>
                    </div>
                </div>

                <div id="panelForm">
                    <form role="form" id="frmPago" name="frmPago" data-toggle="validator">
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-12 col-md-8 col-lg-7">
                                        <div class="row">
                                            <div class="col-lg-6 col-12">
                                                <h4>DATOS CLIENTE</h4>
                                                <div class="form-group">
                                                    <label for="txtCliente">
                                                        Nombre Cliente
                                                    </label>
                                                    <input type="text" class="form-control campo-input" id="txtCliente"
                                                        disabled="disabled" />
                                                </div>

                                                <div class="row">
                                                    <div class="col-9">
                                                        <label for="txtCliente">
                                                            Calle
                                                        </label>
                                                        <input type="text" class="form-control campo-input" id="txtCalleCliente"
                                                            disabled="disabled" />
                                                    </div>
                                                    <div class="col-3">
                                                        <label for="txtCliente">
                                                            Número
                                                        </label>
                                                        <input type="text" class="form-control campo-input" id="txtNumeroCalleCliente"
                                                            disabled="disabled" />
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label for="txtCliente">
                                                        Teléfono
                                                    </label>
                                                    <input type="text" class="form-control campo-input" id="txtTelefonoCliente"
                                                        disabled="disabled" />
                                                </div>

                                                <div class="form-group">
                                                    <label for="txtCliente">
                                                        Notas
                                                    </label>
                                                    <textarea rows="5" class="form-control" id="txtNotasCliente" ></textarea>
                                                </div>
                                            </div>
                                            <div class="col-lg-6 col-12">
                                                <h4>DATOS AVAL</h4>
                                                <div class="form-group">
                                                    <label for="txtCliente">
                                                        Nombre Aval
                                                    </label>
                                                    <input type="text" class="form-control campo-input" id="txtAval"
                                                        disabled="disabled" />
                                                </div>

                                                <div class="row">
                                                    <div class="col-9">
                                                        <label for="txtCliente">
                                                            Calle
                                                        </label>
                                                        <input type="text" class="form-control campo-input" id="txtCalleAval"
                                                            disabled="disabled" />
                                                    </div>
                                                    <div class="col-3">
                                                        <label for="txtCliente">
                                                            Número
                                                        </label>
                                                        <input type="text" class="form-control campo-input" id="txtNumeroCalleAval"
                                                            disabled="disabled" />
                                                    </div>
                                                </div>

                                                <div class="form-group">
                                                    <label for="txtCliente">
                                                        Teléfono
                                                    </label>
                                                    <input type="text" class="form-control campo-input" id="txtTelefonoAval"
                                                        disabled="disabled" />
                                                </div>

                                                <div class="form-group">
                                                    <label for="txtCliente">
                                                        Notas
                                                    </label>
                                                    <textarea rows="5" class="form-control" id="txtNotasAval" ></textarea>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mt-3">
                                    <div class="col-12 col-md-8 col-lg-7">
                                        <div class="row">
                                            <div class="col-lg-6 col-12">
                                                <h5>FALLA</h5>
                                                <div class="row">
                                                    <div class="col-9">
                                                        <div class="form-group">
                                                            <label for="txtCliente">
                                                                Semanas falla
                                                            </label>
                                                            <input type="text" class="form-control campo-input" id="txtSemanasFallas"
                                                                disabled="disabled" />
                                                        </div>
                                                    </div>
                                                    <div class="col-3">
                                                        <div class="form-group">
                                                            <label for="txtCliente">
                                                                Pagos
                                                            </label>
                                                            <input type="text" class="form-control campo-input" id="txtPagos"
                                                                disabled="disabled" />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-6">
                                                        <div class="form-group">
                                                            <label for="txtCliente">
                                                                Monto
                                                            </label>
                                                            <input type="text" class="form-control campo-input" id="txtMonto"
                                                                disabled="disabled" />
                                                        </div>
                                                    </div>
                                                    <div class="col-6">
                                                        <div class="form-group">
                                                            <label for="txtCliente">
                                                                Total
                                                            </label>
                                                            <input type="text" class="form-control campo-input" id="txtTotal"
                                                                disabled="disabled" />
                                                        </div>
                                                    </div>
                                                    <div class="col-6">
                                                        <h5>PAGO</h5>
                                                        <div class="form-group">
                                                            <label for="txtCliente">
                                                                Monto Pago
                                                            </label>
                                                            <input type="text" class="form-control campo-input" id="txtMontoPago"
                                                                 required="required" data-required-error='Requerido' />
                                                            <div class="help-block with-errors"></div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-12 text-right mt-5">
                                            <button id="btnCancelar" class="btn btn-secondary cancelar deshabilitable mr-3"><i class="fa fa-arrow-circle-left mr-1"></i>Cancelar</button>
                                            <button id="btnCapturar" class="btn btn-primary deshabilitable mr-3"><i class="fa fa-save mr-1"></i>Guardar</button>
                                            <button id="btnRecibo" class="btn btn-warning" disabled="disabled"><i class="fa fa-save mr-1"></i>Recibo</button>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </form>



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

    <div id="panelMensajeControlado" class="modal fade" role="dialog" data-backdrop="static" style="margin-top: 200px;">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Información</h4>

                </div>
                <div class="modal-body">

                    <span id="spnMensajeControlado"></span>

                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" id="btnAceptarPanelMensajeControlado">Aceptar</button>
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


    <!-- JavaScript files-->
    <script src="../../vendor/jquery/jquery.min.js"></script>
    <script src="../../vendor/popper.js/umd/popper.min.js"> </script>
    <script src="../../vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="../../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>
    <%--<script src="../../js/front.js"></script>--%>
    <script src="../../vendor/momentjs/moment.min.js"></script>


    <!-- DataTables JavaScript -->
    <%--<script src="../../vendor/datatables-responsive/dataTables.responsive.js"></script>--%>
    <script src="../../vendor/datatables/1.13.1/js/jquery.dataTables.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/dataTables.bootstrap4.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/dataTables.buttons.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/buttons.bootstrap4.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/jszip.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/pdfmake.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/vfs_fonts.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/buttons.html5.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/buttons.print.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/buttons.colVis.min.js"></script>

    <!-- DataTables StyleSheet -->
    <link href="../../vendor/datatables/1.13.1/css/dataTables.bootstrap4.min.css" rel="stylesheet" />
    <link href="../../vendor/datatables/1.13.1/css/buttons.bootstrap4.min.css" rel="stylesheet" />

    <script src="../../js/validator.js"></script>
    <script src="../../js/app/loans/paymentsOverdue.js"></script>
    <script src="../../js/app/general.js"></script>

    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet">
    <script src="../../js/toastr.min.js"></script>


</body>
</html>

