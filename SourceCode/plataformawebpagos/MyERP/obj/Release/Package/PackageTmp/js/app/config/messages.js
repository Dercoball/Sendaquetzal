'use strict';
let date = new Date();
let descargas = "Mensajes_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '11';



const messages = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();
        messages.loadComboTipoPlantilla();
        messages.loadComboFrecuenciaEnvio();
        messages.idSeleccionado = -1;
        messages.accion = '';
        messages.cargarItems();

    },


    loadComboTipoPlantilla: () => {

        let params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Messages.aspx/GetListaItemsTipoPlantilla",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdTipoPlantilla}' > ${item.Nombre}</option > `;

                }

                //console.log('loadComboPosicion');

                $('#comboTipoPlantilla').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    loadComboFrecuenciaEnvio: () => {

        let params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Messages.aspx/GetListaItemsFrecuenciaEnvio",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdFrecuenciaEnvio}' > ${item.Nombre}</option > `;

                }

                //console.log('loadComboPosicion');

                $('#comboFrecuenciaEnvio').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Messages.aspx/GetItems",
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

                        { data: 'IdMensaje' },
                        { data: 'Nombre' },
                        { data: 'NombreTipoPlantilla' },
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


    delete: (id) => {

        messages.idSeleccionado = id;


        $('#mensajeEliminar').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');

    },




    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        let params = {};
        params.path = window.location.hostname;
        params.id = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Messages.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;
                messages.idSeleccionado = item.IdMensaje;

                $('#txtNombre').val(item.Nombre);
                $('#comboTipoPlantilla').val(item.IdTipoPlantilla);
                $('#comboFrecuenciaEnvio').val(item.IdFrecuenciaEnvio);
                tinymce.get("contenido").setContent(item.Contenido);

                $('#panelTabla').hide();
                $('#panelForm').show();

                messages.accion = "editar";
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
        messages.accion = "nuevo";
        messages.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            messages.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                let item = {};
                item.IdMensaje = messages.idSeleccionado;
                item.Nombre = $('#txtNombre').val();
                item.Contenido = tinymce.get("contenido").getContent();
                item.IdTipoPlantilla = $('#comboTipoPlantilla').val();
                item.IdFrecuenciaEnvio = $('#comboFrecuenciaEnvio').val();


                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = messages.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/Messages.aspx/Save",
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

                            messages.cargarItems();


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
            params.id = messages.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Messages.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        messages.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });


        $('#btnEnviarS').on('click', (e) => {
            e.preventDefault();

            console.log('Enviar sms');

            let params = {};
            params.path = window.location.hostname;
            params.id = messages.idSeleccionado;
            params.msg = tinymce.get("contenido").getContent().replace(/<[^>]*>?/gm, '');
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Messages.aspx/SendMessageSMS",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    console.log(msg);


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });

        $('#btnEnviarW').on('click', (e) => {
            e.preventDefault();

            console.log('Enviar SendMessageWhatsapp ');

            let numCel = $('#txtCelular').val();
            if (!numCel) {
                utils.toast(mensajesAlertas.errorCelular, 'error');

                return;
            }

            if (numCel.length < 10) {
                utils.toast(mensajesAlertas.errorCelular, 'error');

                return;
            }

            let params = {};
            params.path = window.location.hostname;
            params.id = messages.idSeleccionado;
            params.celular = numCel;
            params.nombre = "Juan Perez";
            //params.msg = tinymce.get("contenido").getContent();
            params.msg = tinymce.activeEditor.getContent({ format: 'text' });
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Messages.aspx/SendMessageWhatsapp",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    console.log(msg);
                    if (msg.d !== '') {

                        utils.toast(mensajesAlertas.exitoEnviarMsgWthats, 'ok');

                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });


    }


}

window.addEventListener('load', () => {

    messages.init();

    messages.accionesBotones();

});


