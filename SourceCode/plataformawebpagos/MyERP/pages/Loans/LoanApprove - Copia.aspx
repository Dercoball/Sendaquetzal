<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoanApprove - Copia.aspx.cs" Inherits="Plataforma.pages.LoanApproveCopia" %>

<%@ Register Src="~/pages/Controles/UcCliente.ascx" TagPrefix="uc1" TagName="UcCliente" %>
<%@ Register Src="~/pages/Controles/UcDocumentacion.ascx" TagPrefix="uc1" TagName="UcDocumentacion" %>



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
    <link rel="stylesheet" href="../../vendor/font-awesome/css/font-awesome.min.css">
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
    <style>
        .nav-tabs .nav-link.active, .nav-tabs .nav-item.show .nav-link {
            color: #FFF;
            background-color: #018F6D;
        }

        i.fa-camera, i.fa-folder {
            cursor: pointer;
            font-size: 25px;
        }

        i:hover {
            opacity: 0.6;
        }

        input[type=file] {
            display: none;
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
                <!-- Inicio Titulo -->
                <div class="row mt-4 mb-3 border-bottom col-12">
                    <h3>DATOS DE PRESTAMO</h3>
                </div>
                <!-- Fin titulo -->

                <!-- Inicio de campos de prestamos -->
                <div class="card rounded">
                    <div class="card-body">
                        <div class="row mb-2">
                            <div class="col-lg-3">
                                <label class="form-label">Fecha solicitud</label>
                                <label class="form-control"></label>
                            </div>
                            <div class="col-lg-3">
                                <label class="form-label">Tipo de cliente</label>
                                <select class="form-control campo-combo" id="cboTipoCliente"
                                    required="required"
                                    data-required-error='Requerido'>
                                </select>
                                <div class="help-block with-errors"></div>
                            </div>
                            <div class="col-lg-3">
                                <label class="form-label">Cantidad de prestamo</label>
                                <input type="number"
                                    class="form-control campo-input"
                                    id="txtCantidadPrestamo"
                                    required="required" data-required-error='Requerido' />
                                <div class="help-block with-errors"></div>
                            </div>
                            <div class="col-lg-3">
                                <label class="form-label">Máximo por renovación</label>
                                <input type="number"
                                    class="form-control campo-input"
                                    id="txMaximoPorRenovacion"
                                    required="required" data-required-error='Requerido' />
                                <div class="help-block with-errors"></div>
                            </div>
                        </div>

                        <div class="row mb-4">
                            <div class="col-3"></div>
                            <div class="col-lg-3">
                                <label class="form-label">Veces como aval</label>
                                <input class="form-control campo-input"
                                    id="txtVecesComoAval"
                                    readonly="true" />
                            </div>
                            <div class="col-lg-3">
                                <label class="form-label">Prestamos completados</label>
                                <input class="form-control campo-input"
                                    id="txtPrestamosCompletados"
                                    readonly="true" />
                            </div>
                            <div class="col-lg-3">
                                <label class="form-label">Prestamos rechazados</label>
                                <input class="form-control campo-input"
                                    id="txtPrestamosRechazados"
                                    readonly="true" />
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Fin de campos de prestamos -->

                <div id="panelForm" class="mt-4">
                    <nav>
                        <div class="nav nav-tabs text-center" id="nav-tab" role="tablist">
                            <a class="nav-item nav-link active" style="width: 133px;" id="nav-client-tab" data-toggle="tab" href="#nav-client" role="tab" aria-controls="nav-client" aria-selected="true">Cliente</a>
                            <a class="nav-item nav-link" id="nav-aval-tab" style="width: 133px;" data-toggle="tab" href="#nav-aval" role="tab" aria-controls="nav-aval" aria-selected="false">Aval</a>
                            <a class="nav-item nav-link" id="nav-aprobacion-tab" style="width: 133px;" data-toggle="tab" href="#nav-aprobacion" role="tab" aria-controls="nav-aprovacion" aria-selected="false">Aprobación</a>
                        </div>
                    </nav>

                    <div class="tab-content" id="nav-tabContent">
                        <!--Inicio  Datos del cliente -->
                        <div class="tab-pane fade show active" id="nav-client" role="tabpanel" aria-labelledby="nav-client-tab">
                            <form role="form" id="frmCustomer" name="frmCustomer" data-toggle="validator">
                                <div class="row">
                                    <div class="col-7">
                                        <div class="card rounded p-3">
                                            <div class="card-body">
                                                <uc1:UcCliente runat="server" id="UcCliente" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-5 text-center">
                                        <div class="card rounded p-3">
                                            <div class="card-body p-3">
                                                <uc1:UcDocumentacion runat="server" id="UcDocumentacionCliente" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <!--Fin Datos del cliente -->

                        <!-- Inicio Datos aval-->
                        <div class="tab-pane fade" id="nav-aval" role="tabpanel" aria-labelledby="nav-aval-tab">
                            <form role="form" id="frmAval" name="frmAval" data-toggle="validator">
                                <div class="row">
                                    <div class="col-7">
                                        <div class="card p-3">
                                            <div class="card-body">
                                                <uc1:UcCliente runat="server" id="UcAval" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-5 text-center">
                                        <div class="card p-3">
                                            <div class="card-body p-3">
                                                <uc1:UcDocumentacion runat="server" id="UcDocumentacionAval" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <!-- Fin Datos aval-->

                         <!-- Inicio aprobacion de ejecutivo-->
                        <div class="tab-pane fade" id="nav-aprobacion" role="tabpanel" aria-labelledby="nav-aprobacion-tab">
                            <div class="card">
                                <div class="card-body">
                                    <form role="form" id="frmAprobacion" name="frmAprobacion" data-toggle="validator">
                                        <div class="row mb-5">
                                            <div class="form-group col-md-12">
                                                <label for="txtNotaAprobacion">
                                                    Notas
                                                </label>
                                                <textarea class="form-control campo-textarea" id="txtNotaAprobacion" rows="6"
                                                    required="required" data-required-error='Requerido'></textarea>
                                                <div class="help-block with-errors"></div>

                                            </div>

                                        </div>



                                        <div id="panelTableAprobadores mt-5">
                                            <div class="table-responsive">

                                                <table style="width: 100%!important;" class="table table-striped table-bordered table-hover table-sm" id="tableAprobadores">


                                                    <thead>
                                                        <tr>
                                                            <th>Aprobadores</th>
                                                            <th>Estatus</th>
                                                            <th>Notas</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>





                                    </form>

                                </div>


                            </div>


                            <div class="row mt-5 mb-3">

                                <div class=" col-md-6 text-center">
                                    <button id="btnRechazar" class="btn btn-danger deshabilitable  boton-ocultable"><i class="fa fa-times mr-1"></i>Rechazar</button>
                                </div>


                                <div class=" col-md-6 text-center">
                                    <button id="btnAprobar" class="btn btn-primary deshabilitable boton-ocultable"><i class="fa fa-save mr-1"></i>Aprobar</button>
                                </div>

                            </div>

                        </div>
                         <!-- Fin aprobacion de ejecutivo-->
                    </div>

                    <div class="row mt-3 mb-3">
                        <div class=" col-md-12 text-center">
                            <button class="btn btn-secondary cancelar"><i class="fa fa-arrow-circle-left mr-1"></i>Regresar</button>
                            <button class="btn btn-primary" id="btnGuardarCliente"><i class="fa fa-save mr-1"></i>Guadar</button>
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


    <div id="panelFormGarantias" class="modal fade" role="dialog" data-backdrop="static">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Garantía cliente</h4>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body form">

                    <form role="form" id="frmGarantias" name="frmGarantias" data-toggle="validator">

                        <div class="form-body">

                            <div class="row">


                                <div class="form-group col-md-6">
                                    <label for="txtNombreGarantia">
                                        Nombre
                                    </label>
                                    <input type="text" class="form-control campo-input" id="txtNombreGarantia"
                                        required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>


                                <div class="form-group col-md-6">
                                    <label for="txtNumeroSerie">
                                        Número de serie
                                    </label>
                                    <input type="text" class="form-control" id="txtNumeroSerie"
                                        required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>


                            </div>

                            <div class="row">

                                <div class="form-group col-md-6">
                                    <label for="txtCosto">
                                        Costo
                                    </label>
                                    <input type="number" class="form-control" id="txtCosto" step="0.1"
                                        required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>


                                <div class="form-group col-md-6">

                                    <label>
                                        Fotografía
                                    </label>

                                    <input type="file" class="form-control file-garantiacliente" id="imgGarantee"
                                        required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>

                                    <div class="card">
                                        <a href="#" class="img-document" data-tipo="1" id="href_10">
                                            <img src="../../img/upload.png" id="img_10" class="img-fluid documentos" />
                                        </a>
                                    </div>

                                </div>


                            </div>



                            <div class="row mt-3 mb-3">

                                <div class=" col-md-6 text-left">
                                    <button id="btnCancelarGarantíaCliente" data-dismiss="modal" class="btn btn-secondary"><i class="fa fa-arrow-circle-left"></i>&nbsp;Cancelar</button>
                                </div>

                                <div class=" col-md-6 text-right">
                                    <button id="btnGuardarGarantiaCliente" class="btn btn-primary boton-ocultable"><i class="fa fa-save"></i>&nbsp;Guardar</button>
                                </div>

                            </div>
                        </div>

                    </form>
                </div>

            </div>
        </div>
    </div>

    <div id="panelFormGarantiasAval" class="modal fade" role="dialog" data-backdrop="static">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Garantía aval</h4>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>

                <div class="modal-body form">


                    <div class="form-body">

                        <form role="form" id="frmGarantiasAval" name="frmGarantiasAval" data-toggle="validator">

                            <div class="row">


                                <div class="form-group col-md-6">
                                    <label for="txtNombreGarantiaAval">
                                        Nombre
                                    </label>
                                    <input type="text" class="form-control campo-input" id="txtNombreGarantiaAval"
                                        required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>


                                <div class="form-group col-md-6">
                                    <label for="txtNumeroSerieAval">
                                        Número de serie
                                    </label>
                                    <input type="text" class="form-control" id="txtNumeroSerieAval"
                                        required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>


                            </div>

                            <div class="row">

                                <div class="form-group col-md-6">
                                    <label for="txtCostoAval">
                                        Costo
                                    </label>
                                    <input type="number" class="form-control" id="txtCostoAval" step="0.1"
                                        required="required" data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>


                                <div class="form-group col-md-6">

                                    <label>
                                        Fotografía
                                    </label>

                                    <input type="file" class="form-control file-garantiaaval" id="imgGaranteeAval"
                                        required="required" data-required-error='Requerido' />

                                    <div class="help-block with-errors"></div>

                                    <div class="card">
                                        <a href="#" class="img-document" data-tipo="1" id="href_11">
                                            <img src="../../img/upload.png" id="img_11" class="img-fluid documentos" />
                                        </a>
                                    </div>

                                </div>


                            </div>



                            <div class="row mt-3 mb-3">

                                <div class=" col-md-6 text-left">
                                    <button id="btnCancelarGarantíaAval" data-dismiss="modal" class="btn btn-secondary"><i class="fa fa-arrow-circle-left"></i>&nbsp;Cancelar</button>
                                </div>

                                <div class=" col-md-6 text-right">
                                    <button id="btnGuardarGarantiaAval" class="btn btn-primary boton-ocultable"><i class="fa fa-save"></i>&nbsp;Guardar</button>
                                </div>

                            </div>

                        </form>

                    </div>

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
    <script src="/js/app/loans/loans.js"></script>
    <script src="../../js/app/general.js"></script>


    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet" />
    <script src="../../js/toastr.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
</body>
</html>
