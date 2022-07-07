<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportDefault.aspx.cs" Inherits="Plataforma.pages.ReportDefault" %>

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
                                    <label for="txtFechaSemana">
                                        Semana
                                    </label>
                                    <input type="date" class="form-control" id="txtFechaSemana" />
                                </div>

                                <div class="form-group col-md-2">
                                    <label for="comboPlaza">
                                        Plaza
                                    </label>
                                    <select id="comboPlaza" class="form-control">
                                    </select>
                                </div>

                                <div class="form-group col-md-2">
                                    <label for="comboEjecutivo">
                                        Ejecutivo
                                    </label>
                                    <select id="comboEjecutivo" class="form-control">
                                    </select>
                                </div>

                                <div class="form-group col-md-2">
                                    <label for="comboSupervisor">
                                        Supervisor
                                    </label>
                                    <select id="comboSupervisor" class="form-control">
                                        <option value="">Seleccione</option>
                                    </select>
                                </div>

                                <div class="form-group col-md-2">
                                    <label for="comboPromotor">
                                        Promotor
                                    </label>
                                    <select id="comboPromotor" class="form-control">
                                        <option value="">Seleccione</option>
                                    </select>
                                </div>


                            </div>


                            <div class="row mt-3 mb-3">

                                <div class=" col-md-3">
                                    <button class="btn btn-outline btn-primary" id="btnReporteFalla"><i class="fa fa-search mr-1"></i>Reporte de falla</button>
                                </div>
                                <div class=" col-md-3">
                                    <button class="btn btn-outline btn-primary" id="btnReporteDeterminacion"><i class="fa fa-search mr-1"></i>Reporte determinación</button>
                                </div>

                            </div>
                        </div>

                    </div>

                    <hr />

                    <div id="divLoading">
                        <div class="d-flex justify-content-center mt-5">
                            <div class="spinner-border" style="width: 3rem; height: 3rem;" role="status">
                                <span class="sr-only">Loading...</span>
                            </div>
                        </div>
                    </div>


                    <div class="reporteDeterminacion" id="divReporteDeterminacion">

                        <div>


                            <h1 class="h3 display text-center">Determinación de fondos</h1>



                            <div class="row mt-3 mb-3">



                                <div class="form-group col-md-4">
                                    <label for="txtPlaza">
                                        Plaza
                                    </label>
                                    <input type="text" disabled="disabled" id="txtPlaza" class="form-control form-control-sm" />
                                </div>

                                <div class="form-group col-md-4">
                                    <label for="txtSupervisor">
                                        Supervisor
                                    </label>
                                    <input type="text" disabled="disabled" id="txtSupervisor" class="form-control form-control-sm" />
                                </div>

                                <div class="form-group col-md-2">
                                    <label for="txtSemana">
                                        Semana
                                    </label>
                                    <input type="text" disabled="disabled" id="txtSemana" class="form-control form-control-sm" />
                                </div>

                                <div class="form-group col-md-2">
                                    <label for="txtFechaFondos">
                                        Fecha
                                    </label>
                                    <input type="text" disabled="disabled" id="txtFechaFondos" class="form-control form-control-sm" />
                                </div>




                            </div>

                            <div class="row mt-5 mb-3">

                                <div class="col-md-12">

                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm" id="tablePrincipal">

                                            <thead>
                                                <tr>
                                                    <th>Promotor</th>
                                                    <th>Comisión</th>
                                                    <th>Debe entregar</th>
                                                    <th>Falla</th>
                                                    <th>Efectivo</th>
                                                    <th>Recuperado</th>
                                                    <th>Abono entrante</th>
                                                    <%--<th>Total 1</th>--%>
                                                    <th>Abono saliente</th>
                                                    <th>Total</th>
                                                    <th>% Falla</th>
                                                    <th>Venta</th>
                                                    <th>Comisiones</th>
                                                    <th>Resultado venta</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                            </div>


                            <div class="row mt-5">
                                <div class="col-md-6">
                                    <div class="table-responsive">

                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm" id="tablePromotoraTotal">

                                            <thead>
                                                <tr>
                                                    <th>Promotor</th>
                                                    <th>Total final</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                                <div class="col-md-6">
                                    <div class="table-responsive">

                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm" id="tableGastos">

                                            <thead>
                                                <tr>
                                                    <th>Concepto</th>
                                                    <th>Gasto</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                            </div>


                            <div class="row mt-5">
                                <div class="col-md-6">
                                    <div class="table-responsive">

                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm" id="tableConcentrado">

                                            <thead>
                                                <tr>
                                                    <th>Concentrado</th>
                                                    <th>Monto</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                                <tr>
                                                    <th class="text-right">Gastos</th>
                                                    <th class="text-right" id="cell_ConcentradoGastos"></th>
                                                </tr>
                                                <tr>
                                                    <th class="text-right">Fondo</th>
                                                    <th class="text-right" id="cell_ConcentradoFondo"></th>
                                                </tr>
                                                <tr>
                                                    <th class="text-right">Debe entregar</th>
                                                    <th class="text-right" id="cell_ConcentradoDebeEntregar"></th>
                                                </tr>
                                                <tr>
                                                    <th class="text-right">Total</th>
                                                    <th class="text-right" id="cell_ConcentradoTotal"></th>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="row mt-3">
                            </div>
                            <div class="row mt-5">

                                <div class="col-md-3 text-center"></div>
                                <div class="col-md-2 text-center">
                                    <hr />
                                </div>
                                <div class="col-md-2 text-center">
                                </div>
                                <div class="col-md-2 text-center">
                                    <hr />
                                </div>
                                <div class="col-md-3 text-center"></div>

                            </div>

                            <div class="row mb-5">

                                <div class="col-md-3 text-center"></div>
                                <div class="col-md-2 text-center">Firma de entregado</div>
                                <div class="col-md-2 text-center"></div>
                                <div class="col-md-2 text-center">Firma de recibido</div>
                                <div class="col-md-3 text-center"></div>

                            </div>


                        </div>



                    </div>

                    <div class="row mt-3 mb-3 reporteDeterminacion">


                        <div class=" col-md-6 text-center">
                            <button id="btnCancelar" class="btn btn-secondary volver"><i class="fa fa-arrow-circle-left mr-1"></i>Volver</button>
                        </div>

                        <div class=" col-md-6 text-center">
                            <button class="btn btn-outline btn-primary deshabilitable" id="btnGuardarDeterminacion"><i class="fa fa-save mr-1"></i>Guardar e imprimir</button>
                        </div>

                    </div>


                    <div class="reporteFalla" id="divReporteFalla">

                        <div>

                            <div class="row mt-3 text-center">

                                <div class="col-md-3">
                                </div>

                                <div class="col-md-6">
                                    <img src="../../img/brand.png" class="img-responsive" style="width: 90%; height: 90%;" />
                                </div>

                                <div class="col-md-3">
                                </div>
                            </div>

                            <div class="row mb-4">



                                <div class="form-group col-md-4">
                                    <label for="txtEjecutivo">
                                        Ejecutivo
                                    </label>
                                    <input type="text" disabled="disabled" id="txtEjecutivo" class="form-control form-control-sm" />
                                </div>

                                <div class="col-md-4">
                                    <label for="txtPromotor">
                                        Promotor
                                    </label>
                                    <input type="text" disabled="disabled" id="txtPromotor" class="form-control form-control-sm" />
                                </div>



                                <div class="col-md-2">
                                    <label for="txtFecha">
                                        Fecha
                                    </label>
                                    <input type="text" disabled="disabled" id="txtFecha" class="form-control form-control-sm" />
                                </div>

                                <div class="col-md-2">
                                    <label for="txtFolio">
                                        Folio
                                    </label>
                                    <input type="text" disabled="disabled" id="txtFolio" class="form-control form-control-sm" />
                                </div>


                            </div>

                            <div class="row mt-5 mb-3">

                                <div class="col-md-8">

                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-bordered table-striped table-sm mb-3">

                                            <tbody>
                                                <tr>
                                                    <th class="text-right">Debe entregar</th>
                                                    <th class="text-right mr-3" id="cell_totalDebeEntregar"></th>
                                                    <th class="text-right"></th>
                                                    <th class="text-right">Adelanto entrada</th>
                                                    <th class="text-right mr-3" id="cell_totalAdelantoEntrada"></th>
                                                    <th class="text-center">+</th>
                                                </tr>

                                                <tr>
                                                    <th class="text-right">Falla</th>
                                                    <th class="text-right mr-3" id="cell_totalFalla"></th>
                                                    <th class="text-center">-</th>
                                                    <th class="text-right">Adelanto salida</th>
                                                    <th class="text-right mr-3" id="cell_totalAdelantoSalida"></th>
                                                    <th class="text-center">-</th>
                                                </tr>

                                                <tr>
                                                    <th class="text-right">Subtotal</th>
                                                    <th class="text-right mr-3" id="cell_subtotal"></th>
                                                    <th class="text-center">=</th>
                                                    <th></th>
                                                    <th></th>
                                                    <th></th>
                                                </tr>

                                                <tr>
                                                    <th class="text-right">Recuperación</th>
                                                    <th class="text-right mr-3" id="cell_totalRecuperacion"></th>
                                                    <th class="text-center">+</th>
                                                    <th class="text-right">Total de entregar</th>
                                                    <th class="text-right mr-3" id="cell_totalEntregar"></th>
                                                    <th class="text-center">=</th>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                </div>

                                <div class="col-md-1">
                                </div>

                                <div class="col-md-3">

                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-bordered bg-light table-sm mb-3">

                                            <tbody>
                                                <tr>
                                                    <th class="text-right">Venta</th>
                                                    <th class="text-right mr-3" id="cell_totalVenta"></th>
                                                </tr>

                                                <tr>
                                                    <th class="text-right">Comisión %</th>
                                                    <th class="text-right mr-3" id="cell_porcentajeComision"></th>
                                                </tr>

                                                <tr>
                                                    <th>&nbsp;</th>
                                                    <th></th>
                                                </tr>

                                                <tr>
                                                    <th class="text-right">Comisión</th>
                                                    <th class="text-right mr-3" id="cell_totalComision"></th>
                                                </tr>
                                            </tbody>
                                        </table>

                                    </div>
                                </div>

                            </div>

                        </div>


                        <div>

                            <div class="row">
                                <div class="col-md-6">

                                    <h1 class="h3 display">Semana extra</h1>
                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm mb-3" id="tableSemanaExtra">

                                            <thead>
                                                <tr>
                                                    <th>Fecha de crédito</th>
                                                    <th>Nombre</th>
                                                    <th class="text-right">Monto</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                                <div class="col-md-6">

                                    <h1 class="h3 display">Recuperado</h1>
                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm mb-3" id="tableRecuperado">

                                            <thead>
                                                <tr>
                                                    <th>Fecha de crédito</th>
                                                    <th>Nombre</th>
                                                    <th>Monto</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>

                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6">

                                    <h1 class="h3 display">Falla</h1>
                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm mb-3" id="tableFalla">

                                            <thead>
                                                <tr>
                                                    <th>Fecha de crédito</th>
                                                    <th>Nombre</th>
                                                    <th class="text-right">Monto</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>

                                <div class="col-md-6">

                                    <h1 class="h3 display">Adelanto entrante</h1>
                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm" id="tableAdelantoEntrante">

                                            <thead>
                                                <tr>
                                                    <th>Fecha de crédito</th>
                                                    <th>Nombre</th>
                                                    <th>Monto</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6">
                                    <h1 class="h3 display">Adelanto saliente</h1>
                                    <div class="table-responsive">
                                        <table style="width: 100%!important;" class="table table-striped table-bordered table-sm" id="tableAdelantoSaliente">

                                            <thead>
                                                <tr>
                                                    <th>Fecha de crédito</th>
                                                    <th>Nombre</th>
                                                    <th>Monto</th>
                                                </tr>
                                            </thead>

                                            <tbody>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row mt-3">
                        </div>
                        <div class="row mt-5">

                            <div class="col-md-3 text-center"></div>
                            <div class="col-md-2 text-center">
                                <hr />
                            </div>
                            <div class="col-md-2 text-center">
                                <hr />
                            </div>
                            <div class="col-md-2 text-center">
                                <hr />
                            </div>
                            <div class="col-md-3 text-center"></div>

                        </div>

                        <div class="row mb-5">

                            <div class="col-md-3 text-center"></div>
                            <div class="col-md-2 text-center">Firma ejecutivo</div>
                            <div class="col-md-2 text-center">Firma supervisor</div>
                            <div class="col-md-2 text-center">Firma promotor</div>
                            <div class="col-md-3 text-center"></div>

                        </div>


                    </div>

                    <div class="row mt-3 mb-3 reporteFalla">


                        <div class=" col-md-6 text-center">
                            <button class="btn btn-secondary volver"><i class="fa fa-arrow-circle-left mr-1"></i>Volver</button>
                        </div>

                        <div class=" col-md-6 text-center">
                            <button class="btn btn-outline btn-primary deshabilitable" id="btnGuardar"><i class="fa fa-save mr-1"></i>Guardar e imprimir</button>
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
    <script src="../../js/app/reports/fault_report.js"></script>
    <script src="../../js/app/general.js"></script>

    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jspdf/1.3.3/jspdf.min.js"></script>
    <script src="https://html2canvas.hertzen.com/dist/html2canvas.js"></script>

    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet" />
    <script src="../../js/toastr.min.js"></script>


</body>
</html>
