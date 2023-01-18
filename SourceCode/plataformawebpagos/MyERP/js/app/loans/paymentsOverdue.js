'use strict';
let date = new Date();
let descargas = "Pagos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '15';
let totalPrestamo = 0;
let totalFallas = 0;
let totalAbonado = 0;
let dataTable;


const payments = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();


        payments.idSeleccionado = "-1";
        payments.idTipoUsuario = "-1";
        payments.idPago = "-1";
        payments.accion = "";
        payments.idPrestamo = "-1";
        payments.idCliente = "-1";
        payments.numeroSemana = "-1";

        payments.fechaInicial = '';
        payments.fechaFinal = '';

        payments.fechasHoy();

        payments.loadComboPlaza();
        payments.cargarItems();
    },

    cargarItems: () => {

        let status = null;//document.querySelector('input[name="filtroPagos"]:checked').value

        status = status == null ? "0" : status;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.fechaInicial = payments.fechaInicial;
        params.fechaFinal = payments.fechaFinal;
        params.idStatus = status;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/PaymentOverdue.aspx/GetListaItems",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;

                //  si no tiene permisos
                if (data == null) {
                    window.location = "../../pages/Index.aspx";
                }

                dataTable = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    paging: false,
                    scrollY: '400px',
                    scrollCollapse: true,
                    data: data,
                    select: true,
                    columns: [
                        //{ data: 'IdPago' },
                        { data: 'NombreCliente' },
                        { data: 'NombreAval' },
                        { data: 'MontoPrestamo' },
                        {
                            data: 'Fecha',
                            render: function (data, type, full, meta) {
                                return moment(data).format('DD/MM/YYYY');
                            }
                        },
                        { data: 'SemanasFalla' },
                        {
                            data: 'SemanasFalla',
                            render: function (data, type, full, meta) {
                                var res = data.split(",");
                                return res.length;
                            }
                        },
                        { data: 'Monto' },
                        { data: 'TotalFalla' },
                        { data: 'Pagado' },
                        { data: 'Status' },
                        {
                            data: null,
                            searchable: false,
                            orderable: false,
                            className: 'dt-body-center',
                            render: function (data, type, full, meta) {
                                return '<input type="checkbox" name="id[]" value="true" checked="checked">';
                            }
                        },
                        {
                            data: null,
                            render: function (data, type, full, meta) {
                                console.log(data);
                                return `<button data-idcliente="${data.IdCliente}" data-idprestamo="${data.IdPrestamo}" onclick="payments.view(${data.IdPrestamo})" class="btn btn-outline-primary"> <span class="fa fa-folder-open mr-1"></span>Abrir</button>`;
                            }
                        }
                    ],
                    "language": textosEsp,
                    columnDefs: [{
                        orderable: false,
                        className: 'select-checkbox',
                        targets: 7
                    }],
                    dom: "rt<'row'<'col text-right mt-4'B>>ip",
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            title: descargas,
                            text: '&nbsp; Descargar Excel', className: 'csvbtn',
                            exportOptions: {
                                columns: [0,1,2,3,4,5,6,8],
                                modifier: {
                                    selected: true
                                }
                            }
                        },
                        {
                            extend: 'pdfHtml5',
                            text: 'Descargar PDF',
                            title: descargas,
                            orientation: 'landscape',
                            pageSize: 'LEGAL',
                            className: 'csvbtn ml-2',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6, 8],
                                rows: function (idx, data, node) {
                                    var checkbox = node.querySelector('td.select-checkbox > input[type="checkbox"]');
                                    return checkbox.checked;
                                },
                                modifier: {
                                    selected: true
                                }
                            }
                        }
                    ],
                    footerCallback: function (row, data, start, end, display) {
                        var api = this.api();

                        // Remove the formatting to get integer data for summation
                        var intVal = function (i) {
                            return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
                        };

                        // Total over all pages
                        totalPrestamo = api
                            .column(2)
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;

                        totalFallas = api
                            .column(6)
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;

                        totalAbonado = api
                            .column(7)
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;
                        // Update footer
                        $(api.column(2).footer()).html('$' + $.fn.dataTable.render.number(',', '.', 2, '').display(totalPrestamo));
                        $(api.column(6).footer()).html('$' + $.fn.dataTable.render.number(',', '.', 2, '').display(totalFallas));
                        $(api.column(7).footer()).html('$' + $.fn.dataTable.render.number(',', '.', 2, '').display(totalAbonado));
                    },


                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },

    view(id) {

        console.log(id);

        $('#frmPago')[0].reset();

        //  traer datos del pago e historial del préstamo
        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPrestamo = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/PaymentOverdue.aspx/GetPaymentByIdPrestamo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;
                $('#txtCliente').val(data.NombreCliente);
                $('#txtAval').val(data.NombreAval);
                $('#txtCliente').attr('data-idcliente', data.IdCliente);
                $('#txtCliente').attr('data-idprestamo', data.IdPrestamo);
                $('#txtCalleCliente').val(data.CalleCliente);
                $('#txtCalleAval').val(data.CalleAval);
                $('#txtTelefonoCliente').val(data.TelefonoCliente);
                $('#txtTelefonoAval').val(data.TelefonoAval);

                $('#txtSemanasFallas').val(data.SemanasFalla);
                var semanas = data.SemanasFalla.split(',');
                $('#txtPagos').val(semanas.length);
                $('#txtMonto').val(data.MontoFormateadoMx);
                $('#txtTotal').val(data.TotalFallaFormateadoMx);

                $('#panelTabla').hide();
                $('#panelForm').show();
                payments.idPago = data.IdPago;

                payments.idPrestamo = data.IdPrestamo;
                payments.idCliente = data.IdCliente;
                payments.numeroSemana = data.NumeroSemana;

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });

    },


    loadComboPlaza: () => {
        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/Customers.aspx/GetListaPlazas",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let selectEl = document.getElementById('cmbPlaza');
                //remueve las opciones del combo
                document.querySelectorAll('select[name="cmbPlaza"] option').forEach(option => option.remove());

                selectEl.add(new Option("Todos", "0", true, true));
                msg.d.forEach(item => {
                    const option = new Option(item.Nombre, item.IdPlaza, false, false);
                    selectEl.add(option);
                });

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    loadComboEjecutivo: () => {
        var params = {};
        params.path = window.location.hostname;
        params.idplaza = parseInt(document.getElementById('cmbPlaza').value);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/Customers.aspx/GetListaEjecutivo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let selectEl = document.getElementById('cmbEjecutivo');
                //remueve las opciones del combo
                document.querySelectorAll('select[name="cmbEjecutivo"] option').forEach(option => option.remove());

                selectEl.add(new Option("Todos", "0", true, true));
                msg.d.forEach(item => {
                    const option = new Option(`${item.Nombre} ${item.PrimerApellido} ${item.SegundoApellido}`, item.IdEmpleado, false, false);
                    selectEl.add(option);
                });

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    loadComboSupervisor: () => {
        var params = {};
        params.path = window.location.hostname;
        params.idejecutivo = parseInt(document.getElementById('cmbEjecutivo').value);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/Customers.aspx/GetListaSupervisor",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let selectEl = document.getElementById('cmbSupervisor');
                //remueve las opciones del combo
                document.querySelectorAll('select[name="cmbSupervisor"] option').forEach(option => option.remove());

                selectEl.add(new Option("Todos", "0", true, true));
                msg.d.forEach(item => {
                    const option = new Option(`${item.Nombre} ${item.PrimerApellido} ${item.SegundoApellido}`, item.IdEmpleado, false, false);
                    selectEl.add(option);
                });

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    loadComboPromotor: () => {
        var params = {};
        params.path = window.location.hostname;
        params.idsupervisor = parseInt(document.getElementById('cmbSupervisor').value);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/Customers.aspx/GetListaPromotor",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let selectEl = document.getElementById('cmbPromotor');
                //remueve las opciones del combo
                document.querySelectorAll('select[name="cmbPromotor"] option').forEach(option => option.remove());

                selectEl.add(new Option("Todos", "0", true, true));
                msg.d.forEach(item => {
                    const option = new Option(`${item.Nombre} ${item.PrimerApellido} ${item.SegundoApellido}`, item.IdEmpleado, false, false);
                    selectEl.add(option);
                });

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    fecha() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;


    },

    fechasHoy() {

        //  fecha hoy (final)
        let today = new Date();
        let month = (today.getMonth() + 1);

        month = month.toString().length === 1 ? `0${month}` : month;

        let endWeekDay = new Date();
        let end_ = endWeekDay.getDay() + 1

        endWeekDay.setDate(endWeekDay.getDate() + 7 - end_ + 1);

        let dayMonth = endWeekDay.getDate();
        month = (endWeekDay.getMonth() + 1);

        payments.fechaFinal = `${endWeekDay.getFullYear()}-${month}-${dayMonth}`;

        //  fecha inicial
        let startWeekDay = new Date();
        startWeekDay.setDate(startWeekDay.getDate() - startWeekDay.getDay() + 1);

        let startDayMonth = startWeekDay.getDate();
        startDayMonth = startDayMonth.toString().length === 1 ? `0${startDayMonth}` : startDayMonth;

        let startMonth = (startWeekDay.getMonth() + 1);
        startMonth = startMonth.toString().length === 1 ? `0${startMonth}` : startMonth;

        let startYear = (startWeekDay.getFullYear());

        payments.fechaInicial = `${startYear}-${startMonth}-${startDayMonth}`;


        console.log(`fechaInicial ${payments.fechaInicial}`);
        console.log(`fechaFinal ${payments.fechaFinal}`);

    },

    updatePayment(idPago, idStatus) {

        let params = {};
        params.path = window.location.hostname;
        params.idPago = idPago;
        params.idStatus = idStatus;
        params = JSON.stringify(params);


        $.ajax({
            type: "POST",
            url: `../../pages/Loans/PaymentOverdue.aspx/UpdateStatusPagoByPagoAndStatus`,
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let valores = msg.d;

                console.log(valores);
                payments.historial(payments.idPrestamo, payments.numeroSemana);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {

            }

        });

    },

    updatePendiente(idPago) {
        console.log(`updatePendiente`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_PENDIENTE);
    },

    updateFalla(idPago) {
        console.log(`updateFalla`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_FALLA);
    },

    updateAbonado(idPago) {
        console.log(`updateAbonado`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_ABONADO);
    },

    updatePagado(idPago) {
        console.log(`updatePagado`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_PAGADO);
    },

    accionesBotones: () => {

        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            payments.cargarItems();

        });

        $('#btnAceptarPanelMensajeControlado').on('click', (e) => {
            e.preventDefault();

            $('#panelMensajeControlado').modal('hide');

            $('#panelTabla').show();
            $('#panelForm').hide();



        });

        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();


            $('#panelTabla').show();
            $('#panelForm').hide();



        });

        $('#btnCapturar').on('click', (e) => {
            e.preventDefault();

            let hasErrors = $('form[name="frmPago"]').validator('validate').has('.has-error').length;

            if (hasErrors) {

                return;
            }

            //  

            $('.deshabilitable').prop('disabled', true);

            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idPosicion = document.getElementById('txtIdTipoUsuario').value
            params.idPrestamo = payments.idPrestamo;
            params.idCliente = payments.idCliente;
            params.abono = Number($('#txtMontoPago').val());
            params = JSON.stringify(params);


            $.ajax({
                type: "POST",
                url: `../../pages/Loans/PaymentOverdue.aspx/SavePayment`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;

                    $('.deshabilitable').prop('disabled', false);

                    if (parseInt(valores.CodigoError) === 0) {

                        $('#spnMensajeControlado').html(mensajesAlertas.pagoRegistradoExito);
                        $('#panelMensajeControlado').modal('show');

                        //payments.cargarItems();
                        window.location.reload();

                    } else {
                        $('#spnMensajes').html(valores.MensajeError);
                        $('#panelMensajes').modal('show');
                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('.deshabilitable').prop('disabled', false);

                    $('#spnMensajes').html(mensajesAlertas.errorInesperado);
                    $('#panelMensajes').modal('show');

                }

            });


        });

        $('#cmbPlaza').change(function () {
            payments.loadComboEjecutivo();
        });

        $('#cmbEjecutivo').change(function () {
            payments.loadComboSupervisor();
        });

        $('#cmbSupervisor').change(function () {
            payments.loadComboPromotor();
        });

        // Handle click on "Select all" control
        $('#checked-select-all').on('click', function () {
            // Get all rows with search applied
            var rows = dataTable.rows({ 'search': 'applied' }).nodes();
            // Check/uncheck checkboxes for all rows in the table
            $('input[type="checkbox"]', rows).prop('checked', this.checked);
        });

        $('#table tbody').on('change', 'input[type="checkbox"]', function () {
            // If checkbox is not checked
            if (!this.checked) {
                var el = $('#checked-select-all').get(0);
                // If "Select all" control is checked and has 'indeterminate' property
                if (el && el.checked && ('indeterminate' in el)) {
                    // Set visual state of "Select all" control
                    // as 'indeterminate'
                    el.indeterminate = true;
                }
            }
        });

    }


}

window.addEventListener('load', () => {

    payments.init();

    payments.accionesBotones();

});


