'use strict';
let date = new Date();
let descargas = "Mensajes_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '11';



const faq = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        faq.idSeleccionado = -1;
        faq.accion = '';
        faq.cargarItems();

    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Messsages.aspx/GetListaItems",
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

                        { data: 'IdPlantilla' },
                        { data: 'Nombre' },
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

        faq.idSeleccionado = id;

        
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
            url: "../../pages/Config/Messages.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                faq.idSeleccionado = item.Id;

                $('#txtPregunta').val(item.Pregunta);
                $('#txtRespuesta').val(item.Respuesta);
                $('#chkActivo').prop('checked', item.Activo === 1);

                $('#panelTabla').hide();
                $('#panelForm').show();


                faq.accion = "editar";
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
        faq.accion = "nuevo";
        faq.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            faq.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                let item =  {};
                item.IdPlantilla = faq.idSeleccionado;
                item.Nombre = $('#txtNombre').val();                
                item.Contenido = tinymce.get("contenido").getContent();


                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = faq.accion;
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

                            faq.cargarItems();


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
            params.id = faq.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Messages.aspxEliminar",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        faq.cargarItems();

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

    faq.init();

    faq.accionesBotones();

});


