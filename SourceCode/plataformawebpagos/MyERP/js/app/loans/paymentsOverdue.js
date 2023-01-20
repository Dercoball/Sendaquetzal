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

        //Filtros personalizados en datatable
        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#pmin').val(), 10);
            var max = parseInt($('#pmax').val(), 10);
            var prestamo = parseFloat(data[2]) || 0;
            if (
                (isNaN(min) && isNaN(max)) ||
                (isNaN(min) && prestamo <= max) ||
                (min <= prestamo && isNaN(max)) ||
                (min <= prestamo && prestamo <= max)
            ) {
                return true;
            }
            return false;
        });

        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var min = $('#fpmin').val();
                var max = $('#fpmax').val();
                var createdAt = data[3] || 0; // Our date column in the table

                if (min != "" && max == "") {
                    min = moment(min, 'YYY-MM-DD');
                    return moment(createdAt, 'DD/MM/YYY').isSameOrAfter(min)
                }
                else if (min != "" && max != "") {
                    min = moment(min, 'YYY-MM-DD');
                    max = moment(max, 'YYY-MM-DD');
                    return (moment(createdAt, 'DD/MM/YYY').isSameOrAfter(min) && moment(createdAt, 'DD/MM/YYY').isSameOrBefore(max))
                }
                else if (min == "" && max != "") {
                    max = moment(max, 'YYY-MM-DD');
                    return moment(createdAt, 'DD/MM/YYY').isSameOrBefore(max)
                }
                else
                    return true;
            }
        );

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#fmin').val(), 10);
            var max = parseInt($('#fmax').val(), 10);
            var semana = parseFloat(data[5]) || 0;
            if (
                (isNaN(min) && isNaN(max)) ||
                (isNaN(min) && semana <= max) ||
                (min <= semana && isNaN(max)) ||
                (min <= semana && semana <= max)
            ) {
                return true;
            }
            return false;
        });

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#mmin').val(), 10);
            var max = parseInt($('#mmax').val(), 10);
            var semana = parseFloat(data[6]) || 0;
            if (
                (isNaN(min) && isNaN(max)) ||
                (isNaN(min) && semana <= max) ||
                (min <= semana && isNaN(max)) ||
                (min <= semana && semana <= max)
            ) {
                return true;
            }
            return false;
        });

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#tmin').val(), 10);
            var max = parseInt($('#tmax').val(), 10);
            var semana = parseFloat(data[7]) || 0;
            if (
                (isNaN(min) && isNaN(max)) ||
                (isNaN(min) && semana <= max) ||
                (min <= semana && isNaN(max)) ||
                (min <= semana && semana <= max)
            ) {
                return true;
            }
            return false;
        });

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#amin').val(), 10);
            var max = parseInt($('#amax').val(), 10);
            var semana = parseFloat(data[8]) || 0;
            if (
                (isNaN(min) && isNaN(max)) ||
                (isNaN(min) && semana <= max) ||
                (min <= semana && isNaN(max)) ||
                (min <= semana && semana <= max)
            ) {
                return true;
            }
            return false;
        });
    },

    cargarItems: () => {

        let status = null;//document.querySelector('input[name="filtroPagos"]:checked').value

        status = status == null ? "0" : status;
        //Se define el tipo de filtro
        var typeFilter = "";
        if (parseInt(document.getElementById("cmbPromotor").value) > 0) typeFilter = "promotor";
        else if (parseInt(document.getElementById("cmbSupervisor").value) > 0) typeFilter = "supervisor";
        else if (parseInt(document.getElementById("cmbEjecutivo").value) > 0) typeFilter = "ejecutivo";
        else typeFilter = "plaza";

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.fechaInicial = payments.fechaInicial;
        params.fechaFinal = payments.fechaFinal;
        params.idStatus = status;
        params.idPlaza = parseInt(document.getElementById("cmbPlaza").value);
        params.typeFilter = typeFilter;
        params.idEjecutivo = parseInt(document.getElementById("cmbEjecutivo").value);
        params.idSupervisor = parseInt(document.getElementById("cmbSupervisor").value);
        params.idPromotor = parseInt(document.getElementById("cmbPromotor").value);
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
                    ordering: false,
                    paging: false,
                    scrollY: '400px',
                    scrollX: true,
                    data: data,
                    select: true,
                    columns: [
                        //{ data: 'IdPago' },
                        { data: 'NombreCliente' },
                        { data: 'NombreAval' },
                        {
                            data: 'MontoPrestamo',
                            render: $.fn.dataTable.render.number(',', '.', 2, '$')
                        },
                        {
                            data: 'Fecha',
                            type: 'date',
                            render: function (data, type, full, meta) {
                                return moment(data).format('DD/MM/YYYY');
                            }
                        },
                        { data: 'SemanasFalla' },
                        {
                            data: 'SemanasFalla',
                            type: 'date',
                            render: function (data, type, full, meta) {
                                var res = data.split(",");
                                return res.length;
                            }
                        },
                        {
                            data: 'Monto',
                            render: $.fn.dataTable.render.number(',', '.', 2, '$')
                        },
                        {
                            data: 'TotalFalla',
                            render: $.fn.dataTable.render.number(',', '.', 2, '$')
                        },
                        {
                            data: 'Pagado',
                            render: $.fn.dataTable.render.number(',', '.', 2, '$')
                        },
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
                                return `<button data-idcliente="${data.IdCliente}" data-idprestamo="${data.IdPrestamo}" onclick="payments.view(${data.IdPrestamo})" class="btn btn-outline-primary"> <span class="fa fa-folder-open mr-1"></span>Abrir</button>`;
                            }
                        }
                    ],
                    "language": textosEsp,
                    columnDefs: [{
                        orderable: false,
                        className: 'select-checkbox',
                        targets: 10
                    }],
                    dom: "rt<'row'<'col text-right mt-4'B>>ip",
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            title: descargas,
                            text: '&nbsp; Descargar Excel', className: 'csvbtn',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6, 8, 9],
                                rows: function (idx, data, node) {
                                    var checkbox = node.querySelector('td.select-checkbox > input[type="checkbox"]');
                                    return checkbox.checked;
                                },
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
                                columns: [0, 1, 2, 3, 4, 5, 6, 8, 9],
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
                            .column(2, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;

                        totalFallas = api
                            .column(6, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;

                        totalAbonado = api
                            .column(7, { page: 'current' })
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
                    initComplete: function () {
                        let columnsSettings = this.api().settings().init().columns;

                        this.api()
                            .columns()
                            .every(function (idx) {
                                var column = this;
                                let dataHeader = columnsSettings[idx].data;

                                switch (dataHeader) {
                                    case 'MontoPrestamo':
                                    case 'SemanasFalla':
                                    case 'Monto':
                                    case 'TotalFalla':
                                    case 'Pagado':
                                        $('input', column.header()).on('keyup', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'NombreCliente':
                                    case 'NombreAval':
                                        $('input', column.header()).on('keyup change clear', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                    case 'Fecha':
                                        $('input', column.header()).on('change', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'Status':
                                        $('select', column.header()).on('change', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                }
                            });
                    }

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
                $('#txtMonto').attr('data-monto', data.Monto);
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
        params.idplaza = parseInt(document.getElementById('cmbPlaza').value);
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
        params.idplaza = parseInt(document.getElementById('cmbPlaza').value);
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

            //$('#panelForm').hide();
            //$('#panelTabla').show();
            //payments.cargarItems();

        });

        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            var pago = parseFloat($('#btnRecibo').data('pago'));
            $('#btnRecibo').attr('data-pago', 0);
            $('#btnRecibo').removeClass("deshabilitable");
            $('#btnRecibo').prop('disabled', true);

            $('#panelForm').hide();
            $('#panelTabla').show();
            
            if (pago > 0) {
                console.log('cargaitems');
                payments.cargarItems();
            }

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
            params.notaCliente = $('#txtNotasCliente').val();
            params.notaAval = $('#txtNotasAval').val();
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

                        $('#btnRecibo').attr('data-pago', valores.IdItem);
                        $('#btnRecibo').addClass("deshabilitable");
                        $('#btnRecibo').prop('disabled', false);
                        payments.cargarItems();
                        //window.location.reload();

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

        $('#btnRecibo').on('click', (e) => {
            e.preventDefault();

            let hasErrors = $('form[name="frmPago"]').validator('validate').has('.has-error').length;

            if (hasErrors) {

                return;
            }

            $('.deshabilitable').prop('disabled', true);

            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = Number(document.getElementById('txtIdUsuario').value);
            params.idPosicion = Number(document.getElementById('txtIdTipoUsuario').value);
            params.idPrestamo = payments.idPrestamo;
            params.idCliente = payments.idCliente;
            params.abono = Number($('#btnRecibo').data('pago'));
            params.abonoPactado = Number($('#txtMonto').data('monto'));
            params.semanas = $('#txtSemanasFallas').val();
            params = JSON.stringify(params);

            //fetch(`../../pages/Loans/PaymentOverdue.aspx/GenerateReport`, {
            //    body: params,
            //    method: 'POST',
            //    headers: {
            //        'Content-Type': 'application/json; charset=utf-8'
            //    },
            //})
            //    .then(response => response.json())
            //    .then(response => {
            //        $('.deshabilitable').prop('disabled', false);
            //        console.log(d);
            //        //const blob = new Blob([response], { type: 'application/pdf' });
            //        //const downloadUrl = URL.createObjectURL(blob);
            //        const a = document.createElement("a");
            //        a.href = response.d;
            //        a.download = "reporte.pdf";
            //        document.body.appendChild(a);
            //        a.click();
            //        $('.deshabilitable').prop('disabled', false);
            //    })
            //    .catch(() => {
            //        $('.deshabilitable').prop('disabled', false);
            //    });





            $.ajax({
                type: "POST",
                url: `../../pages/Loans/PaymentOverdue.aspx/GenerateReport`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;
                    const a = document.createElement("a");
                    a.href = valores;
                    a.download = "reporte.pdf";
                    document.body.appendChild(a);
                    a.click();
                    $('.deshabilitable').prop('disabled', false);
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('.deshabilitable').prop('disabled', false);

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


