<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TiposResolucion.aspx.cs"
    Inherits="Plataforma.pages.TiposResolucion" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title></title>
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
                    <img src="../img/generic.png" alt="person" class="img-fluid rounded-circle">
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
                    <h1 class="h3 display">Tipo resolución</h1>

                </header>

                <div id="panelTabla">
                    <div class="table-responsive">

                        <table style="width: 100%!important;" class="table table-striped table-bordered table-hover" id="table">

                            <thead>

                                <th>Nombre</th>
                                <th>Activo</th>
                                <th>Ultima modificación</th>
                                <th>
                                    <button class="btn btn-outline btn-primary" id="btnNuevo"><i class="glyphicon glyphicon-plus"></i>&nbsp;Nuevo</button>
                                </th>


                            </thead>
                            <tbody>
                            </tbody>
                        </table>
                        <!-- /.table-responsive -->
                    </div>
                </div>

                <div id="panelForm">

                    <div class="modal-body form">
                        <form role="form" id="frm" name="frm" data-toggle="validator">

                            <div class="form-body">
                                <h3 class="text-center">
                                    <small><span id="spnTituloForm"></span></small>
                                </h3>
                                <div class="form-group">
                                    <label for="txtNombre">Nombre</label>
                                    <input type="text" class="form-control" id="txtNombre"
                                        required data-required-error='Requerido' />
                                    <div class="help-block with-errors"></div>
                                </div>
                                
                                <div class="form-group">

                                    <label for="txtTexto1">Texto 1</label>
                                    <textarea id="txtTexto1" class="form-control texto-grande" required data-required-error='Requerido'>
                                    </textarea>
                                    <div class="help-block with-errors"></div>
                                </div>

                                <div class="form-group">

                                    <label for="txtTexto2">Texto 2</label>
                                    <textarea id="txtTexto2" class="form-control texto-grande" required data-required-error='Requerido'>
                                    </textarea>
                                    <div class="help-block with-errors"></div>
                                </div>

                                <div class="form-group">

                                    <label for="txtTexto3">Texto 3</label>
                                    <textarea id="txtTexto3" class="form-control texto-grande" required data-required-error='Requerido'>
                                    </textarea>
                                    <div class="help-block with-errors"></div>
                                </div>

                                <div class="form-control-sm">
                                    <input id="chkActivo" checked class="form-control-sm" type="checkbox" />
                                    <label for="chkActivo">Activo</label>

                                </div>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button id="btnGuardar" class="btn btn-primary">Guardar</button>
                        <button id="btnCancelar" class="btn btn-danger">Cancelar</button>
                    </div>

                </div>

            </div>
        </section>

        <footer class="main-footer">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-6">
                        <p>Mi empresa &copy; 2019</p>
                    </div>
                    <div class="col-sm-6 text-right">
                        <p><a href="https://bootstrapious.com/p/bootstrap-4-dashboard" class="external">@c</a></p>
                        <!-- Please do not remove the backlink to us unless you support further theme's development at https://bootstrapious.com/donate. It is part of the license conditions and it helps me to run Bootstrapious. Thank you for understanding :)-->
                    </div>
                </div>
            </div>
        </footer>

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


    <script src="../js/validator.js"></script>
    <script src="../js/cp/catalogos/tiposresolucion.js"></script>
    <script src="../js/cp/general.js"></script>


    <!-- Toastr style -->
    <link href="../css/toastr.min.css" rel="stylesheet">
    <script src="../js/toastr.min.js"></script>

</body>
</html>
