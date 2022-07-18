'use strict';
let date = new Date();
let descargas = "Clientes_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '21';


const customers = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();


        customers.idTipoUsuario = "-1";
        customers.accion = "";

        customers.loadComboStatus();
        customers.selectedCustomerId = '';
        customers.cargarItems();



    },

    cargarItems: () => {

        let status = $('#comboStatus').val();

        status = status == null ? "-1" : status;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.idStatus = status;
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
                    data: data,
                    columns: [
                        { data: 'IdCliente' },
                        { data: 'NombreCompleto' },
                        { data: 'Curp' },
                        { data: 'Telefono' },
                        { data: 'NombreStatus' },
                        { data: 'Accion' }


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        }
                    ],
                    dom: 'fBrtipl',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Csv', className: 'csvbtn'
                        },
                        {
                            extend: 'excelHtml5',
                            title: descargas,
                            text: 'Xls', className: 'excelbtn'
                        },
                        {
                            extend: 'pdfHtml5',
                            title: descargas,
                            text: 'Pdf', className: 'pdfbtn'
                        }
                    ]


                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },

    condonate(customerId) {

        $('#condonatePanelMsg').html(`¿Desea pasar a condonado el préstamo actual del cliente seleccionado. (No. ${customerId}) ?`);
        $('#condonatePanel').modal('show');

        customers.selectedCustomerId = customerId;

    },

    //  demanda
    claim(customerId) {

        $('#claimPanelMsg').html(`¿Desea pasar a demanda al cliente seleccionado. (No. ${customerId}) ?`);
        $('#claimPanel').modal('show');

        customers.selectedCustomerId = customerId;

    },

    view(loadId) {
        console.log('Abir datos del idPrestamo ' + loadId);


        window.location = `../Loans/LoanApprove.aspx?id=${loadId}&idf=${pagina}`;


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


    }


}

window.addEventListener('load', () => {

    customers.init();

    customers.accionesBotones();

});


