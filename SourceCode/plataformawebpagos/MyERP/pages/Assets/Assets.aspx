<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Assets.aspx.cs" Inherits="Plataforma.pages.Assets" %>

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

                <header>
                    <h1 class="h3 display">Activos</h1>
                </header>

                <div id="panelTabla">
                    <div class="card">
                        <div class="card-body">
                            <button class="btn btn-outline btn-primary" id="btnNuevo"><i class="fa fa-file mr-1"></i>Agregar</button>
                            <div class="table-responsive">
                                <table class="table table-bordered table-hover table-sm w-100" id="table">
                                    <thead>
                                        <tr>
                                            <th>Categoría<br /><input placeholder="Categoría" /></th>
                                            <th>Descripción<br /><input placeholder="Descripción" /></th>
                                            <th>No. Serie<br /><input placeholder="No. Serie" /></th>
                                            <th>
                                                Costo<br />
                                                <input id="cmax" type="number" placeholder="MAX" /><br />
                                                <input id="cmin" type="number" placeholder="MIN" />
                                            </th>
                                            <th>Asignación<br /><input placeholder="ASignación" /></th>
                                            <th>
                                                Ingreso<br />
                                                <input id="finicial" type="date" /><br />
                                                <input id="ffinal" type="date" />
                                            </th>
                                            <th>
                                                Estatus<br />
                                                <select>
                                                    <option value="">Todos</option>
                                                    <option value="Programado">Programado</option>
                                                    <option value="Realizado">Realizado</option>
                                                </select>
                                            </th>
                                            <th>
                                                Baja<br />
                                                <input id="finicialb" type="date" /><br />
                                                <input id="ffinalb" type="date" />
                                            </th>
                                            <th class="text-center">Acción</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                    <tfoot>
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
                                        </tr>
                                    </tfoot>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="panelForm">

                    <div class="modal-body form">
                        <form role="form" id="frm" name="frm">

                            <div class="form-body">
                                <h3 class="text-left">
                                    <span id="spnTituloForm"></span>
                                </h3>
                                <hr />





                                <div class="row">

                                    <div class="form-group col-md-6">
                                        <label for="comboCategoria">
                                            Tipo
                                        </label>
                                        <select class="form-control campo-combo" id="comboCategoria" required="required" data-required-error='Requerido'>
                                        </select>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                    <div class="form-group col-md-6">
                                        <label for="comboEmpleado">
                                            Empleado asignado
                                        </label>
                                        <select class="form-control campo-combo" id="comboEmpleado" required="required" data-required-error='Requerido'>
                                        </select>
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>

                                <div class="row">


                                    <div class="form-group col-md-6">
                                        <label for="txtDescripcion">
                                            Descripción
                                        </label>
                                        <input type="text" class="form-control" id="txtDescripcion" required="required" data-required-error='Requerido' />
                                        <div class="help-block with-errors"></div>
                                    </div>

                                    <div class="form-group col-md-6">
                                        <label for="txtNumeroSerie">
                                            Numero de serie
                                        </label>
                                        <input type="text" class="form-control" id="txtNumeroSerie" required="required" data-required-error='Requerido' />
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>

                                <div class="row">


                                    <div class="form-group col-md-6">
                                        <label for="txtCosto">
                                            Costo  
                                        </label>
                                        <input type="number" step="any" class="form-control" id="txtCosto" required="required" data-required-error='Requerido' />
                                        <div class="help-block with-errors"></div>
                                    </div>

                                    <div class="form-group col-md-6">
                                        <label for="txtComentarios">
                                            Comentarios
                                        </label>
                                        <input type="text" class="form-control" id="txtComentarios" required="required" data-required-error='Requerido' />
                                        <div class="help-block with-errors"></div>
                                    </div>

                                </div>


                               
                            </div>
                                
                        </form>
                    </div>



                    <div class="row mt-3 mb-3">

                        <div class=" col-md-6 text-left">
                            <button id="btnCancelar" class="btn btn-secondary"><i class="fa fa-arrow-circle-left mr-1"></i>Volver</button>
                        </div>

                        <div class=" col-md-6 text-right">
                            <button id="btnGuardar" class="btn btn-primary deshabilitable"><i class="fa fa-save mr-1"></i>Guardar</button>
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
    <script src="../../vendor/jquery-validation/jquery.validate.min.js"></script>
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


    <%--<script src="../../js/front.js"></script>--%>


    <script src="../../js/validator.js"></script>
    <script src="../../js/app/assets/assets.js"></script>
    <script src="../../js/app/general.js"></script>

    <!-- Toastr style -->
    <link href="../../css/toastr.min.css" rel="stylesheet">
    <script src="../../js/toastr.min.js"></script>

</body>
</html>
