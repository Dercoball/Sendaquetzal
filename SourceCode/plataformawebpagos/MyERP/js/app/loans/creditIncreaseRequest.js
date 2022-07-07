'use strict';
let date = new Date();
let descargas = "CreditIncreaseRequest_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '16';


const requests = {


    init: () => {


        requests.idSolicitud = -1;
        requests.idPrestamo = -1;

        requests.cargarItems();


    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.fechaInicial = requests.fechaInicial;
        params.fechaFinal = requests.fechaFinal;
        params.idStatus = status;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/CreditIncreaseRequest.aspx/GetListaItems",
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
                        { data: 'IdSolicitudAumentoCredito' },
                        { data: 'NombrePromotor' },
                        { data: 'NombreSupervisor' },
                        { data: 'LimiteCreditoActualMx' },
                        { data: 'LimiteCreditoRequeridoMx' },
                        { data: 'NombrePlaza' },
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




    approve(idSolicitud, idPrestamo) {

        console.log(idSolicitud);
        console.log(idPrestamo);
        requests.idSolicitud = idSolicitud;
        requests.idPrestamo = idPrestamo;

        $('#msgAprobar').html('¿Desea aprobar la solicitud?');
        $('#panelAprobar').modal('show');

    },


    reject(idSolicitud, idPrestamo) {

        console.log(idSolicitud);
        console.log(idPrestamo);
        requests.idSolicitud = idSolicitud;
        requests.idPrestamo = idPrestamo;

        $('#msgRechazar').html('¿Desea rechazar la solicitud?');
        $('#panelRechazar').modal('show');

    },


    accionesBotones: () => {

        

        $('#btnAprobarOk').on('click', (e) => {
            e.preventDefault();

            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idPrestamo = requests.idPrestamo;
            params.idSolicitud = requests.idSolicitud;
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
                        
                        requests.cargarItems();

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
            params.idPrestamo = requests.idPrestamo;
            params.idSolicitud = requests.idSolicitud;
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

                        requests.cargarItems();

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

    requests.init();

    requests.accionesBotones();

});


