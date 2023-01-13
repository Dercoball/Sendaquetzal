'use strict';
let date = new Date();
let descargas = "Clientes_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '21';
let total = 0;
let pageTotal = 0;


const customers = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();


        customers.idTipoUsuario = "-1";
        customers.accion = "";

        customers.loadComboPlaza();
        customers.selectedCustomerId = '';
        customers.cargarItems();

        $('#cmbPlaza').change(function () {
            customers.loadComboEjecutivo();
        });

        $('#cmbEjecutivo').change(function () {
            customers.loadComboSupervisor();
        });

        $('#cmbSupervisor').change(function () {
            customers.loadComboPromotor();
        });

    },

    cargarItems: () => {

        let status = $('#comboStatus').val();

        status = status == null ? "-1" : status;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.idStatus = status;
        params.idPlaza = parseInt(document.getElementById("cmbPlaza").value);
        params.idEmpleado = parseInt(document.getElementById("cmbPromotor").value);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/Customers.aspx/GetItems",
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

                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    columnDefs: [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                        { type: "num-fmt", render: $.fn.dataTable.render.number(',', '.', 2, ''), targets: 4 }
                    ],
                    data: data,
                    columns: [
                        { data: 'IdCliente' },
                        { data: 'NombreCompleto' },
                        { data: 'Curp' },
                        { data: 'Telefono' },
                        { data: 'Monto' },
                        {
                            data: 'Direccion', render: function (data, type, row) {

                                return `${row.direccion.Calle}, ${row.direccion.Colonia}, ${row.direccion.Municipio}, ${row.direccion.Estado}`;
                            }
                        },
                        { data: 'NombreStatus' },
                        { data: 'Accion' }


                    ],

                    "language": textosEsp,
                    dom: "rt<'row'<'col text-right mt-4'B>>ip",
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp; Descargar CSV', className: 'csvbtn'
                        },
                        //{
                        //    extend: 'pdfHtml5',
                        //    title: descargas,
                        //    text: 'Pdf', className: 'pdfbtn'
                        //}
                        {
                            extend: 'pdfHtml5',
                            text: 'Descargar PDF',
                            title: descargas,
                            orientation: 'landscape',
                            pageSize: 'LEGAL',
                            className: 'csvbtn ml-2'
                        }
                    ],
                    footerCallback: function (row, data, start, end, display) {
                        var api = this.api();

                        // Remove the formatting to get integer data for summation
                        var intVal = function (i) {
                            return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
                        };

                        // Total over all pages
                        total = api
                            .column(4)
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);

                        // Total over this page
                        pageTotal = api
                            .column(4, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);

                        // Update footer
                        $(api.column(4).footer()).html('$' + $.fn.dataTable.render.number(',', '.', 2, '').display(pageTotal) + ' ( $' + $.fn.dataTable.render.number(',', '.', 2, '').display(total) + ' total)');
                    },

                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },

    condonate(customerId) {

        $('#condonatePanelMsg').html(`¿Desea pasar a status Condonado al cliente seleccionado. (No. ${customerId}) ?`);
        $('#condonatePanel').modal('show');

        customers.selectedCustomerId = customerId;

    },

    //  demanda
    claim(customerId) {

        $('#claimPanelMsg').html(`¿Desea pasar a status Demanda al cliente seleccionado. (No. ${customerId}) ?`);
        $('#claimPanel').modal('show');

        customers.selectedCustomerId = customerId;

    },

    //  reactivar
    reactivate(customerId) {

        $('#reactivatePanelMsg').html(`¿Desea pasar a status Activo al cliente seleccionado. (No. ${customerId}) ?`);
        $('#reactivatePanel').modal('show');

        customers.selectedCustomerId = customerId;

    },

    view(loanId) {
        console.log('Abir datos del idPrestamo ' + loanId);


        window.location = `../Loans/LoanApprove.aspx?id=${loanId}&idf=${pagina}`;


    },

    loadComboStatus: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/Customers.aspx/GetListaStatus",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdStatus}' > ${item.Nombre}</option > `;

                }

                $('#comboStatus').html(opcion);

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



    accionesBotones: () => {

        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            customers.cargarItems();

        });


        $('#btnOkCondonate').on('click', (e) => {
            e.preventDefault();

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.userId = document.getElementById('txtIdUsuario').value;
            parametros.statusId = utils.cliente.STATUS_CONDONADO;
            parametros.customerId = customers.selectedCustomerId;
            parametros = JSON.stringify(parametros);


            $.ajax({
                type: "POST",
                url: "../../pages/Customers/Customers.aspx/UpdateStatusCustomer",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;
                    console.log(valores);

                    if (valores.CodigoError === 0) {
                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        $('#condonatePanel').modal('hide');

                        customers.cargarItems();

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);

                    utils.toast(mensajesAlertas.errorGuardar, 'error');

                }

            });


        });


        $('#btnOkClaim').on('click', (e) => {
            e.preventDefault();

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.userId = document.getElementById('txtIdUsuario').value;
            parametros.statusId = utils.cliente.STATUS_DEMANDA;
            parametros.customerId = customers.selectedCustomerId;
            parametros = JSON.stringify(parametros);


            $.ajax({
                type: "POST",
                url: "../../pages/Customers/Customers.aspx/UpdateStatusCustomer",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;
                    console.log(valores);

                    if (valores.CodigoError === 0) {
                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        $('#claimPanel').modal('hide');

                        customers.cargarItems();

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);

                    utils.toast(mensajesAlertas.errorGuardar, 'error');

                }

            });


        });


        $('#btnOkReactivate').on('click', (e) => {
            e.preventDefault();

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.userId = document.getElementById('txtIdUsuario').value;
            parametros.statusId = utils.cliente.STATUS_INACTIVO;
            parametros.customerId = customers.selectedCustomerId;
            parametros = JSON.stringify(parametros);


            $.ajax({
                type: "POST",
                url: "../../pages/Customers/Customers.aspx/UpdateStatusCustomer",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;
                    console.log(valores);

                    if (valores.CodigoError === 0) {
                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        $('#reactivatePanel').modal('hide');

                        customers.cargarItems();

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);

                    utils.toast(mensajesAlertas.errorGuardar, 'error');

                }

            });


        });


    }


}

window.addEventListener('load', () => {

    customers.init();

    customers.accionesBotones();

});


