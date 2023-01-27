<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewEmployee.aspx.cs" Inherits="Plataforma.pages.Config.NewEmployee" %>

<%@ Register Src="~/pages/Controles/UcCliente.ascx" TagPrefix="uc1" TagName="UcCliente" %>
<%@ Register Src="~/pages/Controles/UcDocumentacion.ascx" TagPrefix="uc1" TagName="UcDocumentacion" %>

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
    <link rel="stylesheet" href="/vendor/bootstrap/css/bootstrap.min.css" />
    <!-- Font Awesome CSS-->
    <link rel="stylesheet" href="/vendor/font-awesome/css/font-awesome.min.css">
    <!-- Fontastic Custom icon font-->
    <link rel="stylesheet" href="/css/fontastic.css">
    <!-- Google fonts - Roboto -->
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700">
    <!-- jQuery Circle-->
    <link rel="stylesheet" href="/css/grasp_mobile_progress_circle-1.0.0.min.css">
    <!-- Custom Scrollbar-->
    <link rel="stylesheet" href="/vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css">
    <!-- theme stylesheet-->
    <link rel="stylesheet" href="/css/style.sea.css">
    <!-- Custom stylesheet - for your changes-->
    <link rel="stylesheet" href="/css/custom.css">
    <!-- Favicon-->
    <link rel="shortcut icon" href="/img/sq.jpg">
    <!-- Tweaks for older IEs-->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
        <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script><![endif]-->
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

        #flImagenGarantia {
            display: inline;
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
                    <h3>REGISTRO DE COLABORADOR</h3>
                </div>
                <!-- Fin titulo -->

                <!-- Inicio de campos de alta de colaboradores -->
                <div class="card rounded">
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-lg-4">
                                <label class="form-label">Fecha ingreso</label>
                                <input class="form-control" id="dtpFechaIngreso" type="date" />
                            </div>
                            <div class="col-lg-4">
                                <label class="form-label">Puesto</label>
                                <select class="form-control campo-combo" id="cboPuesto"
                                    required="required"
                                    data-required-error='Requerido'>
                                </select>
                                <div class="help-block with-errors"></div>
                            </div>
                            <div class="col-lg-4">
                                <label class="form-label">Plaza</label>
                                <select class="form-control campo-combo" id="cboPlaza"
                                    required="required"
                                    data-required-error='Requerido'>
                                </select>
                                <div class="help-block with-errors"></div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <div class="col-lg-4">
                                <label class="form-label">Fecha baja</label>
                                <input class="form-control" id="dtpFechaBaja" type="date" />
                            </div>
                            <div class="col-lg-4">
                                <label class="form-label">Supervisor</label>
                                <select class="form-control campo-combo" id="cboSupervisor"
                                    required="required"
                                    data-required-error='Requerido'>
                                </select>
                                <div class="help-block with-errors"></div>
                            </div>
                            <div class="col-lg-4">
                                <label class="form-label">Ejecutivo</label>
                                <select class="form-control campo-combo" id="cboEjecutivo"
                                    required="required"
                                    data-required-error='Requerido'>
                                </select>
                                <div class="help-block with-errors"></div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-lg-4">
                                <label class="form-label">Limite de venta por ejercicio</label>
                                <input class="form-control" id="txtLimiteVentaPorEjercicio" type="number" />
                            </div>
                            <div class="col-lg-4">
                                <label class="form-label">Limite de incremento por ejercicio</label>
                                <input class="form-control" id="txtLimiteIncrementoPorEjercicio" type="number" />
                            </div>
                            <div class="col-lg-4">
                                <label class="form-label">Fiscalizable</label>
                                <select class="form-control campo-combo" id="cboFiscalzable"
                                    required="required"
                                    data-required-error='Requerido'>
                                    <option value="">Seleccione</option>
                                    <option value="1">Sí</option>
                                    <option value="2">No</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Fin de campos de alta de colaboradores -->

                <div id="panelForm" class="mt-4">
                    <nav>
                        <div class="nav nav-tabs text-center" id="nav-tab-prestamos" role="tablist">
                            <a class="nav-item nav-link active"
                                style="width: 133px;"
                                id="nav-client-tab"
                                data-toggle="tab"
                                data-id="1"
                                href="#nav-client"
                                role="tab"
                                aria-controls="nav-client"
                                aria-selected="true">Colaborador</a>
                            <a class="nav-item nav-link"
                                id="nav-aval-tab"
                                style="width: 133px;"
                                data-toggle="tab"
                                href="#nav-aval"
                                role="tab"
                                data-id="2"
                                aria-controls="nav-aval"
                                aria-selected="false">Aval</a>
                        </div>
                    </nav>

                    <div class="tab-content" id="nav-tabContent">
                        <!--Inicio  Datos del colaborador -->
                        <div class="tab-pane fade show active" id="nav-client" role="tabpanel" aria-labelledby="nav-client-tab">
                            <form role="form" id="frmColaborador" name="frmColaborador" data-toggle="validator">
                                <div class="row">
                                    <div class="col-7">
                                        <div class="card rounded p-3">
                                            <div class="card-body">
                                                <uc1:UcCliente runat="server" id="UcColaborador" />

                                                <div class="row mt-3 mb-3">
                                                    <label class="form-label w-100">Nota de foto</label>
                                                    <textarea class="form-control" id="txtNotaFoto" required="required"
                                                        data-required-error='Requerido'></textarea>
                                                    <div class="help-block with-errors"></div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="form-label w-100">Nombre de usuario</label>
                                                    <input type="text" class="form-control" id="txtNombreUsuario" 
                                                        required="required" data-required-error='Requerido' />
                                                    <div class="help-block with-errors"></div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="form-label w-100">Contraseña</label>
                                                    <input type="password" class="form-control" id="txtPassword" 
                                                        required="required" data-required-error='Requerido' />
                                                    <div class="help-block with-errors"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-5 text-center">
                                        <div class="card rounded p-3">
                                            <div class="card-body p-3">
                                                <uc1:UcDocumentacion runat="server" id="UcDocumentacionColaborador" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <!--Fin Datos del colaborador -->

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
                    </div>

                       <div class="row mt-3 mb-3">
                        <div class=" col-md-12 text-center">
                            <a class="btn btn-secondary rounded cancelar" href="/pages/Config/Employees.aspx"><i class="fa fa-arrow-circle-left mr-1"></i>REGRESAR</a>
                            <button class="btn btn-primary rounded" id="btnGuardar"><i class="fa fa-save mr-1"></i>GUARDAR</button>
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
    <script src="/vendor/popper.js/umd/popper.min.js"> </script>
    <script src="/vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="/js/grasp_mobile_progress_circle-1.0.0.min.js"></script>
    <script src="/vendor/jquery.cookie/jquery.cookie.js"> </script>
    <script src="/vendor/jquery-validation/jquery.validate.min.js"></script>
    <script src="/vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>


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
    <script src="/js/app/config/newemployees.js"></script>
    <script src="/js/app/general.js"></script>

    <!-- Toastr style -->
    <link href="/css/toastr.min.css" rel="stylesheet" />
    <script src="/js/toastr.min.js"></script>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.4/moment.min.js"></script>
</body>
</html>
