'use strict';
let date = new Date();
let descargas = "EmployeeEvaluation" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '18';


const employeeEvaluation = {


    init: () => {


        employeeEvaluation.idSolicitud = -1;
        employeeEvaluation.idPrestamo = -1;
        employeeEvaluation.loadComboPlaza();
        employeeEvaluation.loadComboEmployeesByPosicion(utils.POSICION_PROMOTOR, '#comboPromotor');
        employeeEvaluation.loadComboEmployeesByPosicion(utils.POSICION_EJECUTIVO, '#comboEjecutivo');
        employeeEvaluation.loadComboEmployeesByPosicion(utils.POSICION_SUPERVISOR, '#comboSupervisor');

        employeeEvaluation.cargarItems();


    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.fechaInicial = employeeEvaluation.fechaInicial;
        params.fechaFinal = employeeEvaluation.fechaFinal;
        params.idStatus = status;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Commissions/EmployeeEvaluation.aspx/GetListaItems",
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
                        { data: 'NombreCompleto' },
                        { data: 'NombreCompletoSupervisor' },
                        { data: 'NombreCompletoEjecutivo' },
                        { data: 'NombrePlaza' },
                        { data: 'FechaIngresoMx' },
                        { data: 'NombreComision' },
                        { data: 'NombreCompleto' },
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
                        }
                    ]


                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    loadComboEmployeesByPosicion: (idTipoEmpleado, control) => {

        var params = {};
        params.path = window.location.hostname;
        params.idTipoEmpleado = idTipoEmpleado;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsEmpleadoByPosicion",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $(`${control}`).html(opcion);

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
            url: "../../pages/Config/Employees.aspx/GetListaItemsPlazas",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdPlaza}' > ${item.Nombre}</option > `;

                }

                $('#comboPlaza').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    approve(idSolicitud, idPrestamo) {

        console.log(idSolicitud);
        console.log(idPrestamo);
        employeeEvaluation.idSolicitud = idSolicitud;
        employeeEvaluation.idPrestamo = idPrestamo;

        $('#msgAprobar').html('¿Desea aprobar la solicitud?');
        $('#panelAprobar').modal('show');

    },


    reject(idSolicitud, idPrestamo) {

        console.log(idSolicitud);
        console.log(idPrestamo);
        employeeEvaluation.idSolicitud = idSolicitud;
        employeeEvaluation.idPrestamo = idPrestamo;

        $('#msgRechazar').html('¿Desea rechazar la solicitud?');
        $('#panelRechazar').modal('show');

    },


    accionesBotones: () => {

        

        $('#btnAprobarOk').on('click', (e) => {
            e.preventDefault();

            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idPrestamo = employeeEvaluation.idPrestamo;
            params.idSolicitud = employeeEvaluation.idSolicitud;
            params = JSON.stringify(params);

            $('.deshabilitable').prop('disabled', true);

            $.ajax({
                type: "POST",
                url: `../../pages/Loans/CreditIncreaseRequest.aspx/ApproveRequest`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;
                    $('#panelAprobar').modal('hide');

                    $('.deshabilitable').prop('disabled', false);

                    if (parseInt(valores)> 0) {

                        utils.toast(mensajesAlertas.solicitidAumentoAprobada, 'ok');
                        
                        employeeEvaluation.cargarItems();

                    } else {
                        util.toast(mensajesAlertas.errorInesperado, 'error');
                        

                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('.deshabilitable').prop('disabled', false);

                    utils.toast(mensajesAlertas.errorInesperado, 'error');


                }

            });


        });


        $('#btnRechazarOk').on('click', (e) => {
            e.preventDefault();

            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idPrestamo = employeeEvaluation.idPrestamo;
            params.idSolicitud = employeeEvaluation.idSolicitud;
            params = JSON.stringify(params);

            $('.deshabilitable').prop('disabled', true);

            $.ajax({
                type: "POST",
                url: `../../pages/Loans/CreditIncreaseRequest.aspx/RejectRequest`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;
                    $('#panelRechazar').modal('hide');

                    $('.deshabilitable').prop('disabled', false);

                    if (parseInt(valores) > 0) {

                        $('#spnMensajes').html(mensajesAlertas.solicitidAumentoRechazada);
                        $('#panelMensajes').modal('show');

                        employeeEvaluation.cargarItems();

                    } else {
                        $('#spnMensajes').html(mensajesAlertas.errorInesperado);
                        $('#panelMensajes').modal('show');

                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('.deshabilitable').prop('disabled', false);

                    $('#spnMensajes').html(mensajesAlertas.errorInesperado);
                    $('#panelMensajes').modal('show');

                }

            });


        });

    }


}

window.addEventListener('load', () => {

    employeeEvaluation.init();

    employeeEvaluation.accionesBotones();

});


