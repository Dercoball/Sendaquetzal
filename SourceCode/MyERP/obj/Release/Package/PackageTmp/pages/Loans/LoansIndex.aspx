<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoansIndex.aspx.cs" Inherits="Plataforma.pages.LoansIndex" %>

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
                    <h1 class="h3 display" id="paginaName">Préstamos</h1>

                </header>
                <div class="text-right">
                    <a href="LoanRequest.aspx" class="btn btn-outline btn-success" id="btnNuevo"><i class="glyphicon glyphicon-plus mr-1"></i>Nuevo prestamo</a>
                </div>


                <div id="panelFiltro">

                    <div class="mt-2 mb-4">



                        <div class="row mt-3 mb-3">

                            <div class="form-group col-md-2">
                                <label for="comboStatus">
                                    Status
                                </label>
                                <select id="comboStatus" class="form-control">
                                </select>
                            </div>


                            <div class="form-group col-md-3">
                                <label for="txtFechaInicial">
                                    Fecha inicial
                                </label>
                                <input type="date" class="form-control" id="txtFechaInicial" />
                            </div>


                            <div class="form-group col-md-3">
                                <label for="txtFechaFinal">
                                    Fecha final
                                </label>
                                <input type="date" class="form-control" id="txtFechaFinal" />
                            </div>

                            <div class=" col-md-3">
                                <button class="btn btn-outline btn-primary" id="btnFiltrar"><i class="fa fa-search mr-1"></i>Filtrar</button>
                            </div>

                        </div>

                    </div>
                </div>



                <div id="panelTabla">
                    <div class="table-responsive">

                        <table style="width: 100%!important;" class="table table-striped table-bordered table-hover table-sm" id="table">

                            <thead>
                                <tr>
                                    <th>No.</th>
                                    <%--<th></th>--%>
                                    <th>Nombre</th>
                                    <th>CURP</th>
                                    <th>Monto préstamo</th>
                                    <th>Fecha de solicitud</th>
                                    <th>Status</th>
                                    <th></th>
                                </tr>
                            </thead>

                            <tbody>
                            </tbody>
                        </table>

                    </div>
                </div>




                <div id="panelForm" style="overflow-y: auto;">
                    <form role="form" id="frm" name="frm">


                        <nav>
                            <div class="nav nav-tabs" id="nav-tab" role="tablist">
                                <a class="nav-item nav-link active" id="nav-client-tab" data-toggle="tab" href="#nav-client" role="tab" aria-controls="nav-client" aria-selected="true">Cliente</a>
                                <a class="nav-item nav-link" id="nav-aval-tab" data-toggle="tab" href="#nav-aval" role="tab" aria-controls="nav-aval" aria-selected="false">Aval</a>
                                <a class="nav-item nav-link" id="nav-aprobacion-tab" data-toggle="tab" href="#nav-aprobacion" role="tab" aria-controls="nav-aprovacion" aria-selected="false">Aprobación</a>

                            </div>
                        </nav>

                        <div class="tab-content" id="nav-tabContent">

                            <div class="tab-pane fade show active" id="nav-client" role="tabpanel" aria-labelledby="nav-client-tab">
                                <div class="card">

                                    <div class="card-header">
                                        Cliente
                                    </div>

                                    <div class="card-body">
                                        <div class="row">



                                            <div class="form-group col-md-3">
                                                <label for="txtNombre">
                                                    Nombre(s)
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtNombre"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>



                                            <div class="form-group col-md-3">

                                                <label>
                                                    Identificación frente
                                                </label>


                                                <div class="card">
                                                    <a href="#" class="img-document" data-tipo="1" id="href_1">
                                                        <img src="../../img/upload.png" id="img_1" class="img-fluid documentos" />
                                                    </a>
                                                </div>


                                            </div>


                                            <div class="form-group col-md-3">

                                                <label>
                                                    Identificación reverso
                                                </label>


                                                <div class="card">
                                                    <a href="#" class="img-document" data-tipo="2" id="href_2">
                                                        <img src="../../img/upload.png" id="img_2" class="img-fluid documentos" />
                                                    </a>
                                                </div>

                                            </div>


                                            <div class="form-group col-md-3">

                                                <label>
                                                    Comprobante de domicilio
                                                </label>


                                                <div class="card">
                                                    <a href="#" class="img-document" data-tipo="3" id="href_3">
                                                        <img src="../../img/upload.png" id="img_3" class="img-fluid documentos" />
                                                    </a>
                                                </div>


                                            </div>


                                        </div>



                                        <div class="row">


                                            <div class="form-group col-md-3">
                                                <label for="txtPrimerApellido">
                                                    Primer apellido
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtPrimerApellido"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>

                                            </div>


                                        </div>


                                        <div class="row">



                                            <div class="form-group col-md-6">
                                                <label for="txtSegundoApellido">
                                                    Segundo apellido
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtSegundoApellido" />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtCalle">
                                                    Calle y número
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtCalle"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                        </div>


                                        <div class="row">

                                            <div class="form-group col-md-6">
                                                <label for="txtColonia">
                                                    Colonia
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtColonia"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtMunicipio">
                                                    Municipio
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtMunicipio"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                        </div>


                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtEstado">
                                                    Estado
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtEstado"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtCodigoPostal">
                                                    Código postal
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtCodigoPostal"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                        </div>

                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtTelefono">
                                                    Teléfono
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtTelefono"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                            <div class="form-group col-md-6">
                                                <label for="txtCURP">
                                                    CURP
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtCURP" title=""
                                                    required="required" data-required-error='Requerido'
                                                    pattern="([A-Z][AEIOUX][A-Z]{2}\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])[HM](?:AS|B[CS]|C[CLMSH]|D[FG]|G[TR]|HG|JC|M[CNS]|N[ETL]|OC|PL|Q[TR]|S[PLR]|T[CSL]|VZ|YN|ZS)[B-DF-HJ-NP-TV-Z]{3}[A-Z\d])(\d)"
                                                    data-pattern-error="Debe ingresar una CURP válida." />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                        </div>

                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtOcupacion">
                                                    Ocupación
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtOcupacion"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtDireccionTrabajo">
                                                    Dirección de trabajo
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtDireccionTrabajo"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                        </div>

                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtUbicacion">
                                                    Ubicacion
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtUbicacion"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                        </div>


                                    </div>
                                </div>


                                <div class="card">

                                    <div class="card-header">
                                        Garantías
                                    </div>

                                    <div class="card-body">


                                        <div class="row">



                                            <div class="form-group col-md-6">

                                                <label>
                                                    Foto de cliente
                                                </label>


                                                <div class="col-md-6 text-center">
                                                    <div class="card">
                                                        <a href="#" class="img-document" data-tipo="4" id="href_4">
                                                            <img src="../../img/upload.png" id="img_4" class="img-fluid documentos" />
                                                        </a>
                                                    </div>
                                                </div>


                                            </div>

                                            <div class="form-group col-md-6">


                                                <label for="txtNotaDeFoto">
                                                    Nota de la foto
                                                </label>
                                                <textarea class="form-control" id="txtNotaDeFoto" disabled
                                                    required data-required-error='Requerido' rows="4">
                                            
                                            </textarea>
                                                <div class="help-block with-errors"></div>

                                            </div>

                                        </div>


                                        <div id="panelTablaGarantias">
                                            <div class="table-responsive">

                                                <table style="width: 100%!important;" class="table table-striped table-bordered table-hover table-sm" id="tableGarantias">


                                                    <thead>

                                                        <th>Nombre</th>

                                                        <th>No. Serie</th>
                                                        <th>Costo</th>
                                                        <th>Fotografía</th>


                                                    </thead>
                                                    <tbody>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>



                                        <div class="row">


                                            <div class="form-group col-md-6">


                                                <label for="txtNotaSupervisor">
                                                    Notas supervisor
                                                </label>

                                                <textarea class="form-control" id="txtNotaSupervisor"
                                                    rows="4"> 
                                                </textarea>

                                                <div class="help-block with-errors"></div>

                                            </div>

                                        </div>


                                    </div>

                                </div>

                            </div>

                            <!-- PESTAÑA AVAL-->

                            <div class="tab-pane fade" id="nav-aval" role="tabpanel" aria-labelledby="nav-aval-tab">

                                <div class="card">

                                    <div class="card-header">
                                        Aval
                                    </div>

                                    <div class="card-body">
                                        <div class="row">



                                            <div class="form-group col-md-3">
                                                <label for="txtNombreAval">
                                                    Nombre(s)
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtNombreAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>



                                            <div class="form-group col-md-3">

                                                <label>
                                                    Identificación frente
                                                </label>


                                                <div class="card">
                                                    <a href="#" class="img-document" data-tipo="5" id="href_5">
                                                        <img src="../../img/upload.png" id="img_5" class="img-fluid documentos" />
                                                    </a>
                                                </div>


                                            </div>


                                            <div class="form-group col-md-3">

                                                <label>
                                                    Identificación reverso
                                                </label>


                                                <div class="card">
                                                    <a href="#" class="img-document" data-tipo="6" id="href_6">
                                                        <img src="../../img/upload.png" id="img_6" class="img-fluid documentos" />
                                                    </a>
                                                </div>

                                            </div>


                                            <div class="form-group col-md-3">

                                                <label>
                                                    Comprobante de domicilio
                                                </label>


                                                <div class="card">
                                                    <a href="#" class="img-document" data-tipo="7" id="href_7">
                                                        <img src="../../img/upload.png" id="img_7" class="img-fluid documentos" />
                                                    </a>
                                                </div>


                                            </div>


                                        </div>



                                        <div class="row">


                                            <div class="form-group col-md-3">
                                                <label for="txtPrimerApellidoAval">
                                                    Primer apellido
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtPrimerApellidoAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>

                                            </div>


                                        </div>


                                        <div class="row">



                                            <div class="form-group col-md-6">
                                                <label for="txtSegundoApellidoAval">
                                                    Segundo apellido
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtSegundoApellidoAval" />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtCalleAval">
                                                    Calle y número
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtCalleAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                        </div>


                                        <div class="row">

                                            <div class="form-group col-md-6">
                                                <label for="txtColoniaAval">
                                                    Colonia
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtColoniaAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtMunicipioAval">
                                                    Municipio
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtMunicipioAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                        </div>


                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtEstadoAval">
                                                    Estado
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtEstadoAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtCodigoPostalAval">
                                                    Código postal
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtCodigoPostalAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                        </div>

                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtTelefonoAval">
                                                    Teléfono
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtTelefonoAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                            <div class="form-group col-md-6">
                                                <label for="txtCURPAval">
                                                    CURP
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtCURPAval" title=""
                                                    required="required" data-required-error='Requerido'
                                                    pattern="([A-Z][AEIOUX][A-Z]{2}\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])[HM](?:AS|B[CS]|C[CLMSH]|D[FG]|G[TR]|HG|JC|M[CNS]|N[ETL]|OC|PL|Q[TR]|S[PLR]|T[CSL]|VZ|YN|ZS)[B-DF-HJ-NP-TV-Z]{3}[A-Z\d])(\d)"
                                                    data-pattern-error="Debe ingresar una CURP válida." />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                        </div>

                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtOcupacionAval">
                                                    Ocupación
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtOcupacionAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <label for="txtDireccionTrabajoAval">
                                                    Dirección de trabajo
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtDireccionTrabajoAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                        </div>

                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtUbicacionAval">
                                                    Ubicacion
                                                </label>
                                                <input type="text" class="form-control campo-input" id="txtUbicacionAval"
                                                    required="required" data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                        </div>


                                    </div>
                                </div>


                                <div class="card">

                                    <div class="card-header">
                                        Garantías
                                    </div>

                                    <div class="card-body">


                                        <div class="row">



                                            <div class="form-group col-md-6">

                                                <label>
                                                    Foto de aval
                                                </label>


                                                <div class="col-md-6 text-center">
                                                    <div class="card">
                                                        <a href="#" class="img-document" data-tipo="8" id="href_8">
                                                            <img src="../../img/upload.png" id="img_8" class="img-fluid documentos" />
                                                        </a>
                                                    </div>
                                                </div>


                                            </div>

                                            <div class="form-group col-md-6">


                                                <label for="txtNotaDeFotoAval">
                                                    Nota de la foto
                                                </label>
                                                <textarea class="form-control" id="txtNotaDeFotoAval" disabled
                                                    required data-required-error='Requerido' rows="4">
                                            
                                            </textarea>
                                                <div class="help-block with-errors"></div>

                                            </div>

                                        </div>


                                        <div id="panelTablaGarantiasAval">
                                            <div class="table-responsive">

                                                <table style="width: 100%!important;" class="table table-striped table-bordered table-hover table-sm" id="tableGarantiasAval">


                                                    <thead>

                                                        <th>Nombre</th>

                                                        <th>No. Serie</th>
                                                        <th>Costo</th>
                                                        <th>Fotografía</th>


                                                    </thead>
                                                    <tbody>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>



                                        <div class="row">


                                            <div class="form-group col-md-6">


                                                <label for="txtNotaSupervisorAval">
                                                    Notas supervisor
                                                </label>

                                                <textarea class="form-control" id="txtNotaSupervisorAval" rows="4"> 
                                                </textarea>

                                                <div class="help-block with-errors"></div>

                                            </div>

                                        </div>


                                    </div>

                                </div>

                            </div>


                             <!-- PESTAÑA APROBACION-->
                            <div class="tab-pane fade" id="nav-aprobacion" role="tabpanel" aria-labelledby="nav-aprobacion-tab">

                                <div class="card">

                                    <div class="card-header">
                                        Aprobación
                                    </div>

                                    <div class="card-body">

                                        <div class="form-group col-md-12">


                                            <label for="txtNotaAprobacion">
                                                Notas
                                            </label>
                                            <textarea class="form-control" id="txtNotaAprobacion" rows="6">
                                            
                                            </textarea>
                                            <div class="help-block with-errors"></div>

                                        </div>


                                        <div class="row mt-3 mb-3">

                                            <div class=" col-md-6 text-right">
                                                <button id="btnAceptar" class="btn btn-secondary"><i class=""></i>Aceptar</button>
                                            </div>

                                            <div class=" col-md-6 ">
                                                <button id="btnRechazar" class="btn btn-primary deshabilitable"><i class=""></i>Rechazar</button>
                                            </div>

                                        </div>



                                        <div id="panelTableAprobadores">
                                            <div class="table-responsive">

                                                <table style="width: 100%!important;" class="table table-striped table-bordered table-hover table-sm" id="tableAprobadores">


                                                    <thead>

                                                        <th>Aprobadores</th>
                                                        <th>Estatus</th>
                                                        <th>Notas</th>

                                                    </thead>
                                                    <tbody>
                                                    </tbody>
                                                </table>

                                            </div>
                                        </div>

                                    </div>

                                </div>



                            </div>


                        </div>






                        <div class="row mt-3 mb-3">

                            <div class=" col-md-6 text-left">
                                <button id="btnCancelar" class="btn btn-secondary"><i class="fa fa-arrow-circle-left mr-1"></i>Listado</button>
                            </div>

                            <div class=" col-md-6 text-right">
                                <button id="btnGuardar" class="btn btn-primary deshabilitable"><i class="fa fa-save mr-1"></i>Guardar</button>
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
    <%--<script src="../../vendor/popper.js/umd/popper.min.js"> </script>--%>
    <script src="../../vendor/bootstrap/js/bootstrap.min.js"></script>
    <%--<script src="../../js/grasp_mobile_progress_circle-1.0.0.min.js"></script>--%>
    <%--<script src="../../vendor/jquery.cookie/jquery.cookie.js"> </script>--%>
    <%--<script src="../../vendor/jquery-validation/jquery.validate.min.js"></script>--%>
    <script src="../../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>


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
    <script src="../../js/app/loans/loansindex.js"></script>
    <script src="../../js/app/general.js"></script>


    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet">
    <script src="../../js/toastr.min.js"></script>


</body>
</html>
