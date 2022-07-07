'use strict';
let date = new Date();
let descargas = "TIPOCLIENTE_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '7';



const tipoCliente = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        tipoCliente.idSeleccionado = -1;
        tipoCliente.accion = '';
        tipoCliente.cargarItems();

    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/CustomerTypes.aspx/GetListaItems",
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

                        { data: 'NombreTipoCliente' },
                        { data: 'PrestamoInicialMaximo' },
                        { data: 'PorcentajeSemanal' },
                        { data: 'SemanasAPrestar' },
                        { data: 'GarantiasPorMonto' },
                        //{ data: 'FechasDePago' },
                        { data: 'CantidadParaRenovar' },
                        { data: 'ActivoSemanaExtra' },
                        { data: 'Accion' }


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: 'frBtipl',
                    buttons: [
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


    eliminar: (id) => {

        tipoCliente.idSeleccionado = id;


        $('#mensajeEliminar').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');

    },




    editar: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        let params = {};
        params.path = window.location.hostname;
        params.id = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/CustomerTypes.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                tipoCliente.idSeleccionado = item.IdTipoCliente;

                $('#txtTipoCliente').val(item.NombreTipoCliente);
                $('#txtPrestamoInicialMaximo').val(item.PrestamoInicialMaximo);
                $('#txtPorcentajeSemanal').val(item.PorcentajeSemanal);
                $('#txtSemanasAPrestar').val(item.SemanasAPrestar);
                $('#txtGarantiasPorMonto').val(item.GarantiasPorMonto);

                $('#chkLunes').prop('checked', item.FechaPagoLunes === 1);
                $('#chkMartes').prop('checked', item.FechaPagoMartes === 1);
                $('#chkMiercoles').prop('checked', item.FechaPagoMiercoles === 1);
                $('#chkJueves').prop('checked', item.FechaPagoJueves === 1);
                $('#chkViernes').prop('checked', item.FechaPagoViernes === 1);
                $('#chkSabado').prop('checked', item.FechaPagoSabado === 1);
                $('#chkDomingo').prop('checked', item.FechaPagoDomingo === 1);

                $('#txtCantidadRenovar').val(item.CantidadParaRenovar);

                $('#chkSemanaExtra').prop('checked', item.SemanasExtra === 1);

                $('#panelTabla').hide();
                $('#panelForm').show();


                tipoCliente.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },


    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');




        $('#panelTabla').hide();
        $('#panelForm').show();
        tipoCliente.accion = "nuevo";
        tipoCliente.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();


            tipoCliente.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};
                item.IdTipoCliente = tipoCliente.idSeleccionado;
                item.NombreTipoCliente = $('#txtTipoCliente').val();
                item.PrestamoInicialMaximo = $('#txtPrestamoInicialMaximo').val();
                item.PorcentajeSemanal = $('#txtPorcentajeSemanal').val();
                item.SemanasAPrestar = $('#txtSemanasAPrestar').val();
                item.GarantiasPorMonto = $('#txtGarantiasPorMonto').val();

                item.FechaPagoLunes = $('#chkLunes').prop('checked') ? 1 : 0;
                item.FechaPagoMartes = $('#chkMartes').prop('checked') ? 1 : 0;
                item.FechaPagoMiercoles = $('#chkMiercoles').prop('checked') ? 1 : 0;
                item.FechaPagoJueves = $('#chkJueves').prop('checked') ? 1 : 0;
                item.FechaPagoViernes = $('#chkViernes').prop('checked') ? 1 : 0;
                item.FechaPagoSabado = $('#chkSabado').prop('checked') ? 1 : 0;
                item.FechaPagoDomingo = $('#chkDomingo').prop('checked') ? 1 : 0;

                item.CantidadParaRenovar = $('#txtCantidadRenovar').val();

                item.SemanasExtra = $('#chkSemanaExtra').prop('checked') ? 1 : 0;

                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = tipoCliente.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/CustomerTypes.aspx/Save",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                            $('#panelTabla').show();
                            $('#panelForm').hide();

                            tipoCliente.cargarItems();


                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }

                        $('#panelEdicion').modal('hide');


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = window.location.hostname;
            params.id = tipoCliente.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/CustomerTypes.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        tipoCliente.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });


    }


}

window.addEventListener('load', () => {

    tipoCliente.init();

    tipoCliente.accionesBotones();

});


