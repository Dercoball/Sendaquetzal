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

        const userType = Number(document.getElementById('txtIdTipoUsuario').value || -1);
        const isPromotor = userType === utils.POSICION_PROMOTOR;
        const isSupervisor = userType === utils.POSICION_SUPERVISOR;

        customers.loadComboPlaza().then(() => {
            customers.selectedCustomerId = '';
            customers.cargarItems();
        });

        $('#cmbPlaza').change(function () {
            customers.loadComboEjecutivo();
        });

        $('#cmbEjecutivo').change(function () {
            customers.loadComboSupervisor();
        });

        $('#cmbSupervisor').change(function () {
            customers.loadComboPromotor();
        });

        // Bloquear filtros para promotor (visual y forzar valor)
        if (isPromotor) {
            $('#cmbPlaza').val(0).prop('disabled', true);
            $('#cmbEjecutivo').val(0).prop('disabled', true);
            $('#cmbSupervisor').val(0).prop('disabled', true);
            $('#cmbPromotor').val(0).prop('disabled', true);
            $('#btnFiltrar').prop('disabled', true);
        }

        // Bloquear solo plaza para supervisor (se fija a su plaza actual)
        if (isSupervisor) {
            const hiddenPlazaEl = document.getElementById('txtIdPlaza');
            const plazaActual = hiddenPlazaEl ? parseInt(hiddenPlazaEl.value || '0') : 0;
            if (plazaActual > 0) {
                $('#cmbPlaza').val(plazaActual).prop('disabled', true).trigger('change');
            } else {
                $('#cmbPlaza').prop('disabled', true); // se fijará tras cargar combo
            }
        }

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#pmin').val(), 10);
            var max = parseInt($('#pmax').val(), 10);
            var prestamo = parseFloat(data[4]) || 0;
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

    },

    cargarItems: () => {

        let status = $('#comboStatus').val();

        status = status == null ? "-1" : status;

        //Se define el tipo de filtro
        var typeFilter = "";
        if (parseInt(document.getElementById("cmbPromotor").value) > 0) typeFilter = "promotor";
        else if (parseInt(document.getElementById("cmbSupervisor").value) > 0) typeFilter = "supervisor";
        else if (parseInt(document.getElementById("cmbEjecutivo").value) > 0) typeFilter = "ejecutivo";
        else typeFilter = "plaza";

        // Forzar filtro promotor si es promotor logueado
        const userType = Number(document.getElementById('txtIdTipoUsuario').value || -1);
        const isPromotor = userType === utils.POSICION_PROMOTOR;
        if (isPromotor) {
            typeFilter = "promotor";
        }


        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.idStatus = status;
        params.idPlaza = parseInt(document.getElementById("cmbPlaza").value);
        params.typeFilter = typeFilter;
        params.idEjecutivo = parseInt(document.getElementById("cmbEjecutivo").value);
        params.idSupervisor = parseInt(document.getElementById("cmbSupervisor").value);
        params.idPromotor = parseInt(document.getElementById("cmbPromotor").value);

        if (isPromotor) {
            params.idPromotor = parseInt(document.getElementById('txtIdEmpleado').value || '0');
            params.idPlaza = 0;
            params.idEjecutivo = 0;
            params.idSupervisor = 0;
        }
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
                    ordering: false,
                    //"order": [],
                    paging: false,
                    scrollY: '400px',
                    scrollX: true,
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
                        {
                            data: 'Monto',
                            render: $.fn.dataTable.render.number(',', '.', 2, '$')
                        },
                        {
                            data: 'Direccion', render: function (data, type, row) {

                                return `${row.direccion.Calle}, ${row.direccion.Colonia}, ${row.direccion.Municipio}, ${row.direccion.Estado}`;
                            }
                        },
                        { data: 'NombreStatus' },
                        {
                            data: 'Mensaje',
                            type: 'unknownType',
                            className: 'dt-body-center',
                            render: function (data, type, full, meta) {
                                if (data == 1) {
                                    return '<input type="checkbox" name="mensaje[]" value="1" checked="checked" disabled="disabled">';
                                } else {
                                    return '<input type="checkbox" name="mensaje[]" value="0" disabled="disabled">';
                                }
                            }
                        },
                        { data: 'Accion' }


                    ],

                    "language": textosEsp,
                    dom: "rt<'row'<'col text-right mt-4'B>>ip",
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            title: descargas,
                            text: '&nbsp; Descargar Excel', className: 'csvbtn',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5, 6],
                                //modifier: {
                                //    selected: true
                                //}
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
                                columns: [0, 1, 2, 3, 4, 5, 6],
                                //rows: function (idx, data, node) {
                                //    var checkbox = node.querySelector('td.select-checkbox > input[type="checkbox"]');
                                //    return checkbox.checked;
                                //},
                                //modifier: {
                                //    selected: true
                                //}
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
                        total = api
                            .column(4, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
;
                        // Update footer
                        $(api.column(4).footer()).html('$' + $.fn.dataTable.render.number(',', '.', 2, '').display(total));
                    },
                    initComplete: function () {
                        let columnsSettings = this.api().settings().init().columns;

                        this.api()
                            .columns()
                            .every(function (idx) {
                                var column = this;
                                let dataHeader = columnsSettings[idx].data;

                                switch (dataHeader) {
                                    case 'Monto':
                                        $('input', column.header()).on('keyup', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'NombreCompleto':
                                    case 'Curp':
                                    case 'Telefono':
                                    case 'Direccion':
                                        $('input', column.header()).on('keyup change clear', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                    case 'NombreStatus':
                                        $('select', column.header()).on('change', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                    case 'Mensaje':
                                        $('select', column.header()).on('change', function () {
                                            column.search(this.value).draw();
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
        params.path = "connbd";
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

    deleteCustomer: (idCliente) => {
        if (!confirm('¿Desea eliminar al cliente y sus préstamos asociados?')) return;

        var params = {
            path: "connbd",
            idCliente: idCliente,
            idUsuario: document.getElementById('txtIdUsuario').value
        };

        $.ajax({
            type: "POST",
            url: "../../pages/Customers/Customers.aspx/DeleteCliente",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                if (oResponse && parseInt(oResponse.CodigoError) === 0) {
                    utils.toast('Cliente eliminado correctamente', 'ok');
                    // Si necesitas refrescar manual, hazlo desde la UI; evitamos recargar para poder ver mensajes/errores
                } else {
                    utils.toast(oResponse ? oResponse.MensajeError : 'Error al eliminar', 'error');
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
                utils.toast('Error al eliminar cliente', 'error');
            }
        });
    },

    loadComboPlaza: () => {
        return new Promise((resolve) => {
            var params = {};
            params.path = "connbd";
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

                    // Si es supervisor, seleccionar y bloquear su plaza actual
                    const idTipoUsuario = parseInt(document.getElementById('txtIdTipoUsuario').value);
                    if (idTipoUsuario === utils.POSICION_SUPERVISOR) {
                        const hiddenPlazaEl = document.getElementById('txtIdPlaza');
                        const plazaSes = hiddenPlazaEl ? parseInt(hiddenPlazaEl.value || '0') : 0;
                        const aplicarPlaza = (plazaFija) => {
                            if (plazaFija > 0) {
                                selectEl.value = plazaFija.toString();
                                $('#cmbPlaza').trigger('change');
                            }
                            $('#cmbPlaza').prop('disabled', true);
                            resolve();
                        };

                        if (plazaSes > 0) {
                            aplicarPlaza(plazaSes);
                        } else {
                            customers.getPlazaActual().then(plazaActual => {
                                aplicarPlaza(plazaActual);
                            });
                        }
                    } else {
                        resolve();
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    resolve();
                }
            });
        });
    },

    loadComboEjecutivo: () => {
        var params = {};
        params.path = "connbd";
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
        params.path = "connbd";
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
        params.path = "connbd";
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

    getPlazaActual: () => {
        return new Promise((resolve) => {
            const params = {
                path: "connbd",
                idUsuario: document.getElementById('txtIdUsuario').value
            };

            $.ajax({
                type: "POST",
                url: "../../pages/Customers/Customers.aspx/GetPlazaActual",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    const data = msg.d;
                    const plaza = data && data.IdPlaza ? parseInt(data.IdPlaza) : 0;
                    resolve(isNaN(plaza) ? 0 : plaza);
                },
                error: function () {
                    resolve(0);
                }
            });
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
            parametros.path = "connbd";
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
            parametros.path = "connbd";
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
            parametros.path = "connbd";
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

// Expone customers globalmente para onclick inline
window.customers = customers;

window.addEventListener('load', () => {

    customers.init();

    customers.accionesBotones();

});


