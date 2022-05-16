<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelEquipos.aspx.cs" Inherits="Plataforma.pages.PanelEquipos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>Plataforma</title>
    <meta name="description" content="">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="robots" content="all,follow">
    <!-- Bootstrap CSS-->
    <link rel="stylesheet" href="../vendor/bootstrap/css/bootstrap.min.css">
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
                <div class="sidenav-header-logo"><a href="Index.aspx" class="brand-small text-center"><strong>C</strong><strong class="text-primary">B</strong></a></div>
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

            <div class="container-fluid">

                <header>
                    <h1 class="h3 display">Equipos</h1>

                </header>
                <div id="panelTabla">
                    <div class="table-responsive">

                        <table style="width: 100%!important;" class="table table-striped table-bordered table-hover " id="table">


                            <thead>
                                <th>No. económico</th>
                                <th>Equipo</th>
                                <th>Marca</th>
                                <th>Modelo</th>
                                <th>Operativo</th>
                                <th>Horometro/Odómetro último mantenimiento</th>                                                
                                <th>
                                    <button class="btn btn-outline btn-primary" id="btnNuevo"><i class="glyphicon glyphicon-plus"></i>&nbsp;Nuevo</button>
                                </th>

                            </thead>
                            <tbody>
                            </tbody>
                        </table>

                    </div>
                </div>

                <div class="panel-body">



                    <div id="panelForm">

                        <div class="modal-body form">
                            <form role="form" id="frm" name="frm">

                                <div class="form-body">
                                    <h3 class="text-left">
                                        <span id="spnTituloForm"></span>
                                        <hr />
                                    </h3>


                                    <div class="row">

                                        <div class="form-group col-md-8">
                                            <label for="txtNombre">
                                                Nombre
                                            </label>
                                            <input type="text" class="form-control" id="txtNombre" placeholder=""
                                                required data-required-error='Requerido' />
                                            <div class="help-block with-errors"></div>
                                        </div>

                                      

                                        <div class="form-group col-md-4">
                                            <label for="comboUltimaUbicacion">
                                                Obra/Ubicación
                                            </label>
                                            <select class="form-control" id="comboUltimaUbicacion" disabled
                                                required data-required-error='Requerido'>
                                            </select>
                                            <div class="help-block with-errors"></div>
                                        </div>


                                    </div>


                                    <div class="row">

                                        <div class="form-group col-md-4">
                                            <label for="comboMarca">Marca</label>
                                            <select class="form-control" id="comboMarca"
                                                required data-required-error='Requerido'>
                                            </select>
                                            <div class="help-block with-errors"></div>
                                        </div>



                                        <div class="form-group col-md-4">
                                            <label for="comboModelo">
                                                Modelo
                                            </label>
                                            <select class="form-control" id="comboModelo"
                                                required data-required-error='Requerido'>
                                            </select>
                                            <div class="help-block with-errors"></div>
                                        </div>


                                        <div class="form-group col-md-4">
                                            <label for="txtAnio">
                                                Año
                                            </label>
                                            <input type="text" class="form-control" id="txtAnio" placeholder=""
                                                required data-required-error='Requerido' />
                                            <div class="help-block with-errors"></div>
                                        </div>



                                    </div>

                                    <div class="row">

                                        <div class="form-group col-md-4">
                                            <label for="txtNumeroEconomico">Número económico</label>
                                            <input type="text" class="form-control" id="txtNumeroEconomico" placeholder=""
                                                required data-required-error='Requerido' />
                                            <div class="help-block with-errors"></div>
                                        </div>



                                        <div class="form-group col-md-4">
                                            <label for="txtNumeroSerie">
                                                Número de serie
                                            </label>
                                            <input type="text" class="form-control" id="txtNumeroSerie" placeholder=""
                                                required data-required-error='Requerido' />
                                            <div class="help-block with-errors"></div>
                                        </div>

                                        <%--  <div class="form-group col-md-4">
                                            <label for="txtValorComercial">
                                                Valor comercial
                                            </label>
                                            <input type="number" step="0.01" class="form-control" id="txtValorComercial" placeholder=""
                                                required data-required-error='Requerido' />
                                            <div class="help-block with-errors"></div>
                                        </div>--%>


                                        <div class="form-group col-md-4">
                                            <div class="form-group" id="divOperador">
                                                <label>Operador actual</label>
                                                <div id="comboOperador" class="form-control"></div>
                                            </div>
                                        </div>




                                    </div>

                                    <div class="row">



                                        <div class="form-group col-md-4">

                                            <input type="checkbox" id="chkOperativo" />
                                            <label for="chkOperativo">
                                                Operativo
                                            </label>
                                            <div class="help-block with-errors"></div>
                                        </div>

                                        <div class="form-group col-md-4">
                                            <label for="txtOdometro">
                                                Horometro/Odómetro
                                            </label>
                                            <input type="text" class="form-control" id="txtOdometro" disabled />
                                            <div class="help-block with-errors"></div>
                                        </div>

                                        <div class="form-group col-md-4">
                                            <label for="txtFechaOdometro">
                                                Último registro de Horometro/Odómetro
                                            </label>
                                            <input type="text" class="form-control" id="txtFechaOdometro" disabled />
                                            <div class="help-block with-errors"></div>
                                        </div>


                                    </div>

                                   <div class="row">

                                        <div class="form-group col-md-4">

                                            <label>
                                                Fotografía
                                            </label>

                                            <input type="file" class="form-control file-fotografia" />
                                            <div class="help-block with-errors"></div>
                                        </div>

                                         <div class="form-group col-md-4">
                                            <label for="comboUnidadMedidaOrometro">Unidad de medida de Horometro/Odómetro</label>
                                            <select class="form-control" id="comboUnidadMedidaOrometro" 
                                                required data-required-error='Requerido'>
                                            </select>
                                            <div class="help-block with-errors"></div>
                                        </div>

                                         <div class="form-group col-md-4">
                                            <label for="txtCapacidadTanque">
                                                Capacidad tanque
                                            </label>
                                            <input type="number" class="form-control" id="txtCapacidadTanque"/>
                                            <div class="help-block with-errors"></div>
                                        </div>


                                    </div>




                                </div>

                            </form>
                        </div>

                       <div class="card bg-light" style="width: 18rem;">
                            <div class="card-body text-center">
                                <a href="#" id="href_">
                                    <img src="../img/logo_small.jpg" id="img_" class="img-fluid" />
                                </a>

                            </div>
                        </div>


                        <div class="alert alert-success" role="alert" id="alert-operacion-ok">
                            <strong>¡Información!</strong>
                            <label id="lblMensajesOk"></label>
                        </div>

                        <div class="alert alert-danger" role="alert" id="alert-operacion-fail">
                            <strong>¡Atención!</strong>
                            <label id="lblMensajesFail"></label>
                        </div>

                        <div class="row mt-3 mb-3">

                            <div class=" col-md-6 text-left">
                                <button id="btnCancelar" class="btn btn-secondary"><i class="fa fa-arrow-circle-left"></i>&nbsp;Lista de Equipos</button>
                            </div>

                            <div class=" col-md-6 text-right">
                                <button id="btnGuardar" class="btn btn-primary"><i class="fa fa-save"></i>&nbsp;Guardar</button>
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



    <div id="panelMensajes" class="modal fade" role="dialog">
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

    <div id="panelEliminar" class="modal fade" role="dialog">
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

    <div class="modal fade" id="panelSeleccionarCliente" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <h3 class="modal-title">Seleccionar cliente ...</h3>
                </div>

                <div class="modal-body form">
                    <form role="form" id="frmSeleccionarCliente" name="frmSeleccionarCliente" data-toggle="validator">
                        <div class="form-body">

                            <table style="width: 100%!important;" class="table table-striped table-bordered table-hover" id="tablaClientes">

                                <thead>

                                    <th>Nombre</th>
                                    <th></th>


                                </thead>
                                <tbody>
                                </tbody>
                            </table>

                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Cancelar</button>
                </div>
            </div>

        </div>

    </div>


    <div class="modal fade" id="panelSeleccionarCita" role="dialog">
        <div class="modal-dialog">
            <div class="modal-content">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <h3 class="modal-title">Seleccionar cita ...</h3>
                </div>

                <div class="modal-body form">
                    <form role="form" id="frmSeleccionarCita" name="frmSeleccionarCita" data-toggle="validator">
                        <div class="form-body">

                            <table style="width: 100%!important;" class="table table-striped table-bordered table-hover"
                                id="tablaCitas">

                                <thead>

                                    <th></th>
                                    <th>Nombre</th>
                                    <th>Fecha</th>
                                    <th>Hora</th>
                                    <th></th>


                                </thead>
                                <tbody>
                                </tbody>
                            </table>

                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Cancelar</button>
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


    <div class="modal fade" id="panelRefacciones" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">

                <div class="modal-header">

                    <h3 class="modal-title">Refacciones</h3>
                    <span id="spnIdEquipo" style="display: none;"></span>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>


                </div>

                <div class="modal-body form">
                    <div class="form-body">

                        <div class="row">
                            <div class="form-group col-md-6">
                                <label for="listaRefacciones">
                                    Refacciones
                                </label>
                                <select class="form-control" id="listaRefacciones" multiple style="height: 300px;"></select>
                                <div class="help-block with-errors"></div>
                            </div>


                            <div class="form-group col-md-6">
                                <label>
                                    Refacciones del equipo <span class="spnNombreEquipo"></span>

                                </label>
                                <select class="form-control" id="listaRefaccionesSeleccionadas" multiple style="height: 300px;"></select>
                                <span></span>
                            </div>

                        </div>




                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>

                    <button type="button" id="btnGuardarRefacciones" class="btn btn-primary">Guardar</button>

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



    <script src="../vendor/jqwidgets/jqx-all.js"></script>
    <script src="../vendor/jqwidgets/jqxdropdownbutton.js"></script>
    <script src="../vendor/jqwidgets/jqxdropdownlist.js"></script>
    <script src="../vendor/jqwidgets/jqxbuttons.js"></script>
    <script src="../vendor/jqwidgets/jqxcore.js"></script>
    <script src="../vendor/jqwidgets/jqxscrollbar.js"></script>
    <script src="../vendor/jqwidgets/jqxlistbox.js"></script>

    <link href="../vendor/jqwidgets/styles/jqx.base.css" rel="stylesheet" />
    <link href="../vendor/jqwidgets/styles/jqx.bootstrap.css" rel="stylesheet" />

    <script src="../js/validator.js"></script>
    <script src="../js/cp/catalogos/equipos.js"></script>
    <script src="../js/cp/general.js"></script>
    <script src="../js/cp/mantenimiento/datos_equipo.js"></script>

    <!-- Toastr style -->
    <link href="../css/toastr.min.css" rel="stylesheet">
    <script src="../js/toastr.min.js"></script>

</body>
</html>
