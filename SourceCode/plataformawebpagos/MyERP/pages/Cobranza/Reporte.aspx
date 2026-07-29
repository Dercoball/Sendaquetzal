<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reporte.aspx.cs" Inherits="Plataforma.pages.Cobranza.Reporte" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Reporte semanal de Cobranza</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="robots" content="all,follow" />
    <link rel="stylesheet" href="../../vendor/bootstrap/css/bootstrap.min.css" />
    <link rel="stylesheet" href="../../vendor/font-awesome/css/font-awesome.min.css">
    <link rel="stylesheet" href="../../css/fontastic.css">
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Roboto:300,400,500,700">
    <link rel="stylesheet" href="../../css/grasp_mobile_progress_circle-1.0.0.min.css">
    <link rel="stylesheet" href="../../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.css">
    <link rel="stylesheet" href="../../css/style.sea.css">
    <link rel="stylesheet" href="../../css/custom.css">
    <link rel="shortcut icon" href="../../img/sq.jpg">
</head>


<body>


    <form class="form-signin" id="form1" runat="server">
        <asp:HiddenField ID="txtUsuario" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdTipoUsuario" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdUsuario" runat="server"></asp:HiddenField>
    </form>


    <nav class="side-navbar">
        <div class="side-navbar-wrapper">
            <div class="sidenav-header d-flex align-items-center justify-content-center">
                <div class="sidenav-header-inner text-center">
                    <i class="fa fa-user-o fa-4x"></i>
                    <h3 class="h5" id="nombreUsuario"></h3>
                    <span id="nombreTipoUsuario"></span>
                </div>
                <div class="sidenav-header-logo"><a href="Index.aspx" class="brand-small text-center"><strong class="text-primary">F</strong></a></div>
            </div>
            <div class="main-menu">
                <h5 class="sidenav-heading">MENÚ</h5>
                <ul id="side-main-menu" class="side-menu list-unstyled"></ul>
            </div>
        </div>
    </nav>

    <div class="page">
        <header class="header">
            <nav class="navbar">
                <div class="container-fluid">
                    <div class="navbar-header">
                        <a id="toggle-btn" href="#" class="menu-btn"><i class="icon-bars"></i></a>
                        <a href="Index.aspx" class="navbar-brand">
                            <div class="brand-text d-none d-md-inline-block"><span></span><strong class="text-primary"><span id="systemName" /></strong></div>
                        </a>
                    </div>
                    <ul class="nav-menu list-unstyled d-flex flex-md-row">
                        <li class="nav-item"><a href="#" class="nav-link logout" onclick="window.top.location.href = '/pages/Logout.aspx'">
                            <span class="d-none d-sm-inline-block">Salir</span><i class="fa fa-sign-out"></i></a></li>
                    </ul>
                </div>
            </nav>
        </header>


        <section class="forms">
            <div class="container-fluid">
                <header><h1 class="h3 display" id="paginaName">Reporte semanal de Cobranza</h1></header>

                <div class="card">
                    <div class="card-header">
                        <div class="row align-items-end">
                            <div class="col-md-3">
                                <label>PLAZA</label>
                                <select class="form-control" id="cmbPlaza"><option value="0">Todos</option></select>
                            </div>
                            <div class="col-md-3">
                                <label>GESTOR</label>
                                <select class="form-control" id="cmbGestor"><option value="0">Todos</option></select>
                            </div>
                            <div class="col-md-2">
                                <label>Desde (lunes)</label>
                                <input type="date" class="form-control" id="txtDesde" />
                            </div>
                            <div class="col-md-2">
                                <label>Hasta (domingo)</label>
                                <input type="date" class="form-control" id="txtHasta" />
                            </div>
                            <div class="col-md-2">
                                <button class="btn btn-outline btn-primary" id="btnFiltrar"><i class="fa fa-search mr-1"></i>Filtrar</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card mt-3">
                    <div class="card-header"><h5>Detalle de cobranza (comisiones)</h5></div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <table style="width: 100%!important;" class="table table-bordered table-hover table-sm" id="table">
                                <thead class="thead-light">
                                    <tr>
                                        <th>Semana</th>
                                        <th>Gestor</th>
                                        <th>Usuario</th>
                                        <th>Inicio crédito</th>
                                        <th>Fecha cobro</th>
                                        <th>Plaza</th>
                                        <th>Cliente</th>
                                        <th>Recuperado</th>
                                        <th>Canal</th>
                                        <th>Folio</th>
                                        <th>Sem. venc.</th>
                                        <th>Bucket</th>
                                        <th>% Com.</th>
                                        <th>Comisión</th>
                                        <th>Observación</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <div class="card mt-3">
                    <div class="card-header"><h5>Resumen económico por gestor</h5></div>
                    <div class="card-body">
                        <div class="table-responsive">
                            <table style="width: 100%!important;" class="table table-bordered table-sm" id="tableResumen">
                                <thead class="thead-light">
                                    <tr>
                                        <th>Gestor</th>
                                        <th>Total cobrado</th>
                                        <th>No tangible</th>
                                        <th>Comisiones</th>
                                        <th>Diferencia</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <footer class="main-footer">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-sm-6"><p class='nombre-empresa'></p></div>
                    <div class="col-sm-6 text-right"><p class="empresa-copy"><a href="#" class="external"></a></p></div>
                </div>
            </div>
        </footer>
    </div>

    <div id="panelMensajes" class="modal fade" role="dialog" data-backdrop="static" style="margin-top: 200px;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title text-center">Información</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body"><p><span id="spnMensajes"></span></p></div>
                <div class="modal-footer"><button class="btn btn-primary" data-dismiss="modal">Aceptar</button></div>
            </div>
        </div>
    </div>


    <script src="../../vendor/jquery/jquery.min.js"></script>
    <script src="../../vendor/popper.js/umd/popper.min.js"> </script>
    <script src="../../vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="../../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>
    <script src="../../vendor/momentjs/moment.min.js"></script>

    <script src="../../vendor/datatables/1.13.1/js/jquery.dataTables.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/dataTables.bootstrap4.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/dataTables.buttons.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/buttons.bootstrap4.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/jszip.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/buttons.html5.min.js"></script>
    <script src="../../vendor/datatables/1.13.1/js/buttons.print.min.js"></script>

    <link href="../../vendor/datatables/1.13.1/css/dataTables.bootstrap4.min.css" rel="stylesheet" />
    <link href="../../vendor/datatables/1.13.1/css/buttons.bootstrap4.min.css" rel="stylesheet" />

    <script src="../../js/app/cobranza/reporte.js"></script>
    <script src="../../js/app/general.js"></script>

    <link href="../../css/toastr.min.css" rel="stylesheet">
    <script src="../../js/toastr.min.js"></script>


</body>
</html>
