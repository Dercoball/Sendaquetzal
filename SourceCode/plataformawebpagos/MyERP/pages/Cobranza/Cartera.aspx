<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cartera.aspx.cs" Inherits="Plataforma.pages.Cobranza.Cartera" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Cartera de Cobranza</title>
    <meta name="description" content="" />
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

    <style>
        #table thead tr th { min-width: 40px; }
        .badge-bucket-1 { background-color: #f0ad4e; color:#fff; }
        .badge-bucket-2 { background-color: #d9534f; color:#fff; }
        .badge-bucket-3 { background-color: #843534; color:#fff; }
    </style>
</head>


<body>


    <form class="form-signin" id="form1" runat="server">
        <asp:HiddenField ID="txtUsuario" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdTipoUsuario" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdUsuario" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdEmpleado" runat="server"></asp:HiddenField>
        <asp:HiddenField ID="txtIdPlaza" runat="server"></asp:HiddenField>
    </form>


    <!-- Side Navbar -->
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
                <ul id="side-main-menu" class="side-menu list-unstyled">
                </ul>
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
                    <h1 class="h3 display" id="paginaName">Cartera de Cobranza</h1>
                </header>

                <div id="panelTabla">
                    <div class="card">
                        <div class="card-header">
                            <div id="panelFiltro">
                                <div class="container-fluid">
                                    <div class="mt-2 mb-4">
                                        <div class="row align-items-end">
                                            <div class="col fv-row">
                                                <label class="d-flex align-items-center fs-6 fw-semibold form-label mb-2"><span>PLAZA</span></label>
                                                <select class="form-control" id="cmbPlaza" name="cmbPlaza">
                                                    <option value="0">Todos</option>
                                                </select>
                                            </div>
                                            <div class="col">
                                                <div class="d-flex align-items-center">
                                                    <button class="btn btn-outline btn-primary" id="btnFiltrar"><i class="fa fa-search mr-1"></i>Filtrar</button>
                                                </div>
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
                                            <th>Gestor</th>
                                            <th>Cliente</th>
                                            <th>Plaza</th>
                                            <th>Ejecutivo</th>
                                            <th>Supervisor</th>
                                            <th>Promotor</th>
                                            <th>Producto</th>
                                            <th>Fecha crédito</th>
                                            <th>Sem.</th>
                                            <th>Bucket</th>
                                            <th>Adeudo</th>
                                            <th>Multas</th>
                                            <th>Gasto 10%</th>
                                            <th>Total</th>
                                            <th>Saldo act.</th>
                                            <th>Acción</th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
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


    <!-- Modal detalle de cuenta -->
    <div id="panelDetalle" class="modal fade" role="dialog" data-backdrop="static">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Detalle de la cuenta</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-4">
                            <h6>CLIENTE</h6>
                            <p class="mb-1"><strong id="dCliente"></strong></p>
                            <p class="mb-1"><small id="dDirCliente"></small></p>
                            <p class="mb-1"><small id="dTelCliente"></small></p>
                        </div>
                        <div class="col-md-4">
                            <h6>AVAL 1</h6>
                            <p class="mb-1"><strong id="dAval1"></strong></p>
                            <p class="mb-1"><small id="dDirAval1"></small></p>
                            <p class="mb-1"><small id="dTelAval1"></small></p>
                        </div>
                        <div class="col-md-4">
                            <h6>AVAL 2</h6>
                            <p class="mb-1"><strong id="dAval2"></strong></p>
                            <p class="mb-1"><small id="dDirAval2"></small></p>
                            <p class="mb-1"><small id="dTelAval2"></small></p>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="col-md-6">
                            <p class="mb-1"><strong>Producto:</strong> <span id="dProducto"></span></p>
                            <p class="mb-1"><strong>Plaza:</strong> <span id="dPlaza"></span></p>
                            <p class="mb-1"><strong>Coordinador:</strong> <span id="dCoordinador"></span></p>
                            <p class="mb-1"><strong>Supervisor:</strong> <span id="dSupervisor"></span></p>
                            <p class="mb-1"><strong>Ejecutivo:</strong> <span id="dEjecutivo"></span></p>
                            <p class="mb-1"><strong>Promotor:</strong> <span id="dPromotor"></span></p>
                            <p class="mb-1"><strong>Fecha crédito:</strong> <span id="dFecha"></span></p>
                            <p class="mb-1"><strong>Pago semanal:</strong> <span id="dPagoSemanal"></span></p>
                        </div>
                        <div class="col-md-6">
                            <p class="mb-1"><strong>Semanas transcurridas:</strong> <span id="dSemanas"></span> (Bucket <span id="dBucket"></span>)</p>
                            <p class="mb-1"><strong>14. Adeudo:</strong> <span id="dAdeudo"></span></p>
                            <p class="mb-1"><strong>15. Multas:</strong> <span id="dMultas"></span> (<span id="dNumFallas"></span> fallas)</p>
                            <p class="mb-1"><strong>16. Subtotal:</strong> <span id="dSubtotal"></span></p>
                            <p class="mb-1"><strong>17. Gasto cobranza:</strong> <span id="dGasto"></span></p>
                            <p class="mb-1"><strong>18. Total:</strong> <span id="dTotal"></span></p>
                            <p class="mb-1"><strong>19. Saldo actualizado:</strong> <span id="dSaldo"></span></p>
                        </div>
                    </div>
                    <hr />
                    <div class="row" id="rowGestion">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Gestor asignado</label>
                                <select class="form-control" id="cmbGestor"><option value="0">Sin asignar</option></select>
                            </div>
                            <button class="btn btn-primary btn-sm" id="btnAsignar"><i class="fa fa-user-plus mr-1"></i>Asignar gestor</button>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Observaciones (sólo gerencia)</label>
                                <textarea rows="3" class="form-control" id="txtObservaciones"></textarea>
                            </div>
                            <button class="btn btn-secondary btn-sm" id="btnGuardarObs"><i class="fa fa-save mr-1"></i>Guardar observaciones</button>
                        </div>
                    </div>
                    <hr />
                    <div id="rowCobro">
                        <h6>REGISTRAR COBRO</h6>
                        <div class="row">
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label>Monto recuperado</label>
                                    <input type="number" step="0.01" class="form-control" id="txtMontoCobro" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label>Canal de pago</label>
                                    <select class="form-control" id="cmbCanal"></select>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label>Folio</label>
                                    <input type="text" class="form-control" id="txtFolio" list="listFolios" />
                                    <datalist id="listFolios"></datalist>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label>Fecha de cobro</label>
                                    <input type="date" class="form-control" id="txtFechaCobro" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-9">
                                <div class="form-group">
                                    <label>Observación del cobro</label>
                                    <input type="text" class="form-control" id="txtObsCobro" />
                                </div>
                            </div>
                            <div class="col-md-3 d-flex align-items-end">
                                <button class="btn btn-success btn-sm mb-3" id="btnRegistrarCobro"><i class="fa fa-money mr-1"></i>Registrar cobro</button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                </div>
            </div>
        </div>
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


    <!-- JavaScript files-->
    <script src="../../vendor/jquery/jquery.min.js"></script>
    <script src="../../vendor/popper.js/umd/popper.min.js"> </script>
    <script src="../../vendor/bootstrap/js/bootstrap.min.js"></script>
    <script src="../../vendor/malihu-custom-scrollbar-plugin/jquery.mCustomScrollbar.concat.min.js"></script>
    <script src="../../vendor/momentjs/moment.min.js"></script>

    <!-- DataTables JavaScript -->
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

    <link href="../../vendor/datatables/1.13.1/css/dataTables.bootstrap4.min.css" rel="stylesheet" />
    <link href="../../vendor/datatables/1.13.1/css/buttons.bootstrap4.min.css" rel="stylesheet" />

    <script src="../../js/validator.js"></script>
    <script src="../../js/app/cobranza/cartera.js"></script>
    <script src="../../js/app/general.js"></script>

    <link href="../../css/toastr.min.css" rel="stylesheet">
    <script src="../../js/toastr.min.js"></script>


</body>
</html>
