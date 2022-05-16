<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MantenimientoListadoFallas.aspx.cs" Inherits="Plataforma.pages.MantenimientoListadoFallas" %>

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



    <link href="https://printjs-4de6.kxcdn.com/print.min.css" rel="stylesheet" type="text/css">
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
                    <h1 class="h3 display">OT's en Curso</h1>

                </header>

                <div id="panelTabla">
                    <div class="table-responsive">

                        <table style="width: 100%!important;" class="table table-striped table-bordered table-hover " id="table">


                            <thead>

                                <th>No.</th>
                                <th>No. económico</th>
                                <th>Acciones</th>
                                <th>Equipo</th>
                                <th>Descripción</th>
                                <th>Fecha</th>
                                <th>Días transcurridos</th>
                                <%--<th>Fecha finalización</th>--%>
                                <th>Prioridad</th>
                                <th>Usuario-Diagnóstico/actual</th>
                                <th>Status</th>

                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <!-- /.table-responsive -->
                    </div>
                </div>




                <div id="panelForm">

                    <div class="modal-body form">
                        <form role="form" id="frm" name="frm">

                            <div class="form-body">
                                <h3 class="text-left">
                                    <span id="spnTituloForm"></span>
                                    <hr />
                                </h3>





                                <div class="row">

                                    <div class="form-group col-md-4">
                                        <label for="txtFechaCreacion">
                                            Fecha
                                        </label>
                                        <input type="text" class="form-control" id="txtFechaCreacion" disabled />
                                    </div>


                                    <div class="form-group col-md-4">
                                        <label for="txtNumeroRequisicion">
                                            No. de orden
                                        </label>
                                        <input type="text" class="form-control" id="txtNumeroRequisicion" disabled />
                                    </div>


                                    <div class="form-group col-md-4">
                                        <label for="txtGeneradoPor">
                                            Generado por
                                        </label>
                                        <input type="text" class="form-control" id="txtGeneradoPor" disabled />
                                    </div>



                                </div>


                                <div class="mt-2">
                                    <h4 class="text-center ">Datos equipo
                                    </h4>
                                </div>

                                <hr />

                                <div class="row">



                                    <div class="form-group col-md-8">
                                        <label for="txtNombreEquipo">
                                            Equipo
                                        </label>
                                        <input type="text" class="form-control" id="txtNombreEquipo" disabled />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <label for="comboUbicacion">
                                            Obra/Ubicación
                                        </label>
                                        <select class="form-control" id="comboUbicacion" disabled
                                            required data-required-error='Requerido'>
                                        </select>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>




                                <div class="row">


                                    <div class="form-group col-md-4">
                                        <label for="txtNumeroEconomico">
                                            Número económico
                                        </label>
                                        <input type="text" class="form-control" id="txtNumeroEconomico" disabled
                                            required data-required-error='Requerido' />
                                        <div class="help-block with-errors"></div>
                                    </div>


                                    <div class="form-group col-md-4">
                                        <label for="comboMarca">Marca</label>
                                        <select class="form-control" id="comboMarca" disabled
                                            required data-required-error='Requerido'>
                                        </select>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <label for="comboModelo">
                                            Modelo
                                        </label>
                                        <select class="form-control" id="comboModelo" disabled
                                            required data-required-error='Requerido'>
                                        </select>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>


                                <div class="row">



                                    <div class="form-group col-md-4">
                                        <label for="txtOrometro">
                                            Horometro/Odómetro
                                        </label>
                                        <input type="text" class="form-control" id="txtOrometro" disabled
                                            required data-required-error='Requerido' />
                                        <div class="help-block with-errors"></div>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <label for="txtNombreOperador">
                                            Operador
                                        </label>
                                        <input type="text" class="form-control" id="txtNombreOperador" disabled />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <label for="txtTelefono">
                                            Teléfono
                                        </label>
                                        <input type="text" class="form-control" id="txtTelefono" disabled />
                                    </div>

                                </div>

                                <div class="row">

                                    <div class="form-group col-md-4">
                                        <label for="comboUnidadMedidaOrometro">Unidad de medida de Horometro/Odómetro</label>
                                        <select class="form-control" id="comboUnidadMedidaOrometro" disabled
                                            required data-required-error='Requerido'>
                                        </select>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>



                                <%--Documentos--%>
                                <div class="mt-2">
                                    <h4 class="text-center ">Datos de la falla
                                    </h4>
                                </div>

                                <hr />


                                <div class="row">



                                    <div class="form-group col-md-12">
                                        <label for="txtDescripcion">
                                            Descripción problema
                                        </label>
                                        <textarea style="text-transform: uppercase" class="form-control deshabilitable" id="txtDescripcion"
                                            required data-required-error='Requerido' rows="4">
											
											</textarea>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>

                                <div class="row">


                                    <div class="form-group col-md-4">

                                        <input type="checkbox" id="chkDetieneOperacion" disabled class="deshabilitable" />
                                        <label for="chkDetieneOperacion">
                                            Detiene la operación
                                        </label>
                                        <div class="help-block with-errors"></div>
                                    </div>


                                    <div class="form-group col-md-4">
                                        <label for="comboPrioridad">
                                            Nivel de prioridad
                                        </label>
                                        <select class="form-control deshabilitable" id="comboPrioridad"
                                            required data-required-error='Requerido'>
                                            <option value="1">1 - Rojo</option>
                                            <option value="2">2 - Amarillo</option>
                                            <option value="3">3 - Verde </option>
                                        </select>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                    <%--             <div class="form-group col-md-4">
                                        <label for="comboPrioridad">
                                            Prioridad actual
                                        </label>
                                        <input id="txtPrioridadRojo" type="text" style="display: none; width: 100%; background-color: #C31F1F" />
                                        <input id="txtPrioridadAmarillo" type="text" style="display: none; width: 100%; background-color: #E5DA00" />
                                        <input id="txtPrioridadVerde" type="text" style="display: none; width: 100%; background-color: #32AD02" />

                                    </div>--%>
                                </div>

                                <%--Documentos--%>
                                <div class="mt-2">
                                    <h4 class="text-center ">Diagnóstico
                                    </h4>
                                </div>

                                <hr />






                                <div id="divDiagnosticos">
                                </div>

                                <div class="row">

                                    <div class="form-group col-md-12">
                                        <label for="txtDiagnostico" id="lblUsuarioDiagnosticoActual">
                                            Diagnóstico
                                        </label>
                                        <textarea style="text-transform: uppercase" class="form-control deshabilitable" id="txtDiagnostico"
                                            required data-required-error='Requerido' rows="4">
											
											</textarea>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>


                                <div class="row">

                                    <div class="form-group col-md-12">
                                        <div class="form-group" id="divEmpleados">
                                            <label>Empleado para diagnóstico</label>
                                            <span class="form-control deshabilitable" id="comboEmpleado" />

                                        </div>
                                    </div>
                                </div>



                                <%--Documentos--%>
                                <div class="mt-2">
                                    <h4 class="text-center ">Documentos
                                    </h4>
                                </div>

                                <hr />



                                <div class="row">
                                    <div class="col-md-12">

                                        <div class="form-group" id="divDocumentosExistentes">
                                        </div>
                                    </div>

                                </div>


                                <div class="row">

                                    <div class="col-md-4">
                                        <button id="btnAgregarOtroDocumento" class="btn btn-primary btn-sm form-control boton-agregar deshabilitable">Agregar documento</button>
                                    </div>

                                    <div class="col-md-8">

                                        <div class="form-group col-md-12" id="divDocumentos">
                                        </div>

                                    </div>


                                </div>

                                <%--End documentos--%>
                            </div>
                        </form>


                        <div class="mt-4">
                            <h4 class="text-center ">Refacciones
                            </h4>
                        </div>
                        <hr />

                        <div class="row mt-3 mb-3">

                            <div class=" col-md-4 text-right">
                            </div>

                            <div class=" col-md-4 text-right">
                                <button class="btn btn-outline btn-primary deshabilitable" id="btnCargarRefacciones"><i class="fa fa-plus mr-1"></i>PM1</button>
                            </div>

                            <div class=" col-md-4 text-right">
                                <button class="btn btn-outline btn-primary deshabilitable" id="btnNuevo"><i class="fa fa-plus mr-1"></i>Agregar registro de refacción</button>
                            </div>

                        </div>

                        <div id="panelTablaRefacciones">

                            <div class="table-responsive">

                                <table style="width: 100%!important;" class="table table-striped table-bordered table-hover " id="tablaRefacciones">


                                    <thead>
                                        <th>Fecha solicitud</th>
                                        <th>Marca/Proveedor</th>
                                        <th>Cantidad</th>
                                        <th>Número de parte</th>
                                        <th>Descripción</th>
                                        <th>Requisición EK</th>
                                        <th></th>

                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>

                            </div>
                        </div>

                        <div id="panelFormRefacciones">

                            <div class="modal-body form">

                                <form role="form" id="frmRefaccion" name="frmRefaccion">

                                    <div class="form-body">
                                        <h3 class="text-left">
                                            <hr />
                                        </h3>

                                        <div class="row">

                                            <div class="form-group col-md-4">
                                                <label for="txtFechaCreacion">
                                                    Fecha solicitud
                                                </label>
                                                <input type="text" class="form-control" id="txtFecha" disabled
                                                    required data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>



                                        </div>


                                        <div class="row">


                                            <div class="form-group col-md-6">
                                                <label for="txtCantidad">
                                                    Cantidad
                                                </label>
                                                <input type="number" class="form-control" id="txtCantidad" step="0.1"
                                                    required data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>

                                            <div class="form-group col-md-6">
                                                <div class="form-group">
                                                    <label>Marca</label>
                                                    <div id="comboMarcaProveedor" class="form-control"></div>
                                                </div>
                                            </div>



                                        </div>

                                        <div class="row">

                                            <div class="form-group col-md-4">
                                                <label for="txtNumeroParte">
                                                    Número de parte
                                                </label>
                                                <input type="text" class="form-control" id="txtNumeroParte"
                                                    required data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>

                                            </div>


                                            <div class="form-group col-md-8">
                                                <label for="txtDescripcionRefaccion">
                                                    Descripción
                                                </label>
                                                <input type="text" class="form-control" id="txtDescripcionRefaccion"
                                                    required data-required-error='Requerido' />
                                                <div class="help-block with-errors"></div>
                                            </div>


                                        </div>



                                        <div class="row mt-3 mb-3">

                                            <div class=" col-md-6 text-left">
                                                <button id="btnCancelarRefaccion" class="btn btn-secondary"><i class="fa fa-arrow-circle-left"></i>&nbsp;Cancelar</button>
                                            </div>

                                            <div class=" col-md-6 text-right">
                                                <button id="btnGuardarRefaccion" class="btn btn-primary"><i class="fa fa-save"></i>&nbsp;Guardar datos de refacción</button>
                                            </div>

                                        </div>
                                    </div>
                                </form>

                            </div>
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
                            <button id="btnCancelar" class="btn btn-secondary"><i class="fa fa-arrow-circle-left"></i>&nbsp;Regresar al listado</button>
                        </div>

                        <div class=" col-md-6 text-right">
                            <button id="btnDescargar" class="btn btn-primary"><i class="fa fa-file-pdf-o"></i>&nbsp;Descargar</button>
                            <button id="btnGuardar" class="btn btn-primary deshabilitable"><i class="fa fa-save"></i>&nbsp;Guardar</button>
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
                    <span id="spnMensajes"></span>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" id="btnAceptarPanelMensajes" data-dismiss="modal">Aceptar</button>
                </div>
            </div>
        </div>
    </div>


    <div id="panelReAbrir" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p id="msgReAbrir">
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnReAbrirAceptar" class="btn btn-primary" data-dismiss="modal">Aceptar</button>
                    <button class="btn btn-default" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelCancelarRefaccion" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p id="msgCancelar">
                        Se eliminará el registro seleccionado. ¿Desea continuar?                                                   
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnEliminarAceptarRefaccion" class="btn btn-danger" data-dismiss="modal">Aceptar</button>
                    <button id="btnEliminarCancelaRefaccionr" class="btn btn-primary" data-dismiss="modal">Cancelar</button>
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
                    <p id="msgEliminar">
                        Se eliminará el registro seleccionado. ¿Desea continuar?                                                   
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnEliminarAceptar" class="btn btn-danger" data-dismiss="modal">Aceptar</button>
                    <button id="btnEliminarCancela" class="btn btn-primary" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelEliminarImagen" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p id="msgEliminarImagen">
                        Se eliminará el registro seleccionado. ¿Desea continuar?                                                   
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnEliminarAceptarImagen" class="btn btn-danger" data-dismiss="modal">Aceptar</button>
                    <button id="btnEliminarCancelarImagen" class="btn btn-primary" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelFinalizar" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p id="msgFinalizar">
                        ¿Desea continuar?                                                   
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnFinalizarAceptar" class="btn btn-primary" data-dismiss="modal">Aceptar</button>
                    <button class="btn btn-default" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="panelSetStatus" class="modal fade" role="dialog" data-backdrop="static">
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
                    <p id="msgSetStatus">
                    </p>
                </div>
                <div class="modal-footer">
                    <button id="btnSetStatusAceptar" class="btn btn-primary" data-dismiss="modal">Aceptar</button>
                    <button class="btn btn-default" data-dismiss="modal">Cancelar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="panelSeleccionarEquipo" role="dialog" data-backdrop="static">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h3 class="modal-title">Seleccionar equipo ...</h3>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                </div>

                <div class="modal-body form">
                    <form role="form" id="frmSeleccionarEquipo" name="frmSeleccionarEquipo" data-toggle="validator">
                        <div class="form-body">
                            <div class="table-responsive">

                                <table style="width: 100%!important;" class="table table-striped table-bordered table-hover" id="tablaEquipos">

                                    <thead>

                                        <th>No. económico</th>
                                        <th>Equipo</th>
                                        <th>Marca</th>
                                        <th></th>


                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>
                            </div>

                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Cancelar</button>
                </div>
            </div>

        </div>

    </div>


    <div class="modal fade" id="panelSeleccionarProveedor" role="dialog" data-backdrop="static">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h3 class="modal-title">Seleccionar proveedor ...</h3>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                </div>

                <div class="modal-body form">
                    <form role="form" id="frmSeleccionarProveedor" name="frmSeleccionarProveedor" data-toggle="validator">
                        <div class="form-body">
                            <div class="table-responsive">

                                <table style="width: 100%!important;" class="table table-striped table-bordered table-hover"
                                    id="tablaProveedores">

                                    <thead>

                                        <th>#</th>
                                        <th>Nombre</th>
                                        <th></th>


                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>

                            </div>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Cancelar</button>
                </div>
            </div>

        </div>

    </div>

    <div class="modal fade" id="panelAsignarColaborador" role="dialog" data-backdrop="static">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">

                <div class="modal-header">
                    <h3 class="modal-title">Asignar colaborador, Ot <span id="idRequisicionSpan"></span></h3>
                </div>

                <div class="modal-body form">

                    <div class="row">

                        <div class="form-group col-md-8">
                            Colaboradores actuales
                        </div>
                        <span id="listaColaboradores"></span>

                    </div>

                    <div class="row">

                        <div class="form-group col-md-8 mt-4">
                            <label for="comboAsignarColaborador">
                                Seleccione el nuevo colaborador
                            </label>

                            <span class="form-control" id="comboAsignarColaborador" />

                            <div class="help-block with-errors"></div>
                        </div>

                    </div>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" id="btnAsignarColaborador">Guardar</button>
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Cerrar</button>
                </div>
            </div>

        </div>

    </div>


    <div id="panelImagen" class="modal fade" role="dialog" data-backdrop="static">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Dcumentos</h4>

                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <span id="spanImagen">
                        <img id="imgDoc" class="img-fluid" /></span>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary" data-dismiss="modal">Cerrar</button>
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
    <script src="../js/cp/mantenimiento/ots_encurso.min.js"></script>
    <script src="../js/cp/general.js"></script>
    <script src="../js/cp/mantenimiento/datos_equipo.js"></script>
    <script src="../js/cp/mantenimiento/panel_refacciones.js"></script>

    <!-- Toastr style -->
    <link href="../css/toastr.min.css" rel="stylesheet">
    <script src="../js/toastr.min.js"></script>

    <script src="../js/jspdf.min.js"></script>
    <script src="../js/html2canvas.min.js"></script>


</body>
</html>
