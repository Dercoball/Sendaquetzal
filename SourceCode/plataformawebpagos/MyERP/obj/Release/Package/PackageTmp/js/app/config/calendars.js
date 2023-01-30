'use strict';
let date = new Date();
let descargas = "CALENDARIOS_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '46';




const calendar = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        calendar.idSeleccionado = -1;
        calendar.accion = '';

        calendar.loadContent();
        

    },

    loadContent() {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Calendars.aspx/GetListaItems",
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

                        { data: 'Id' },
                        { data: 'Nombre' },
                        { data: 'FechaMx' },
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

        calendar.idSeleccionado = id;


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
            url: "../../pages/Config/Calendars.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                calendar.idSeleccionado = item.Id;

                $('#txtNombre').val(item.Nombre);

                $('#txtFecha').val(item.Fecha);

                $('#panelTabla').hide();
                $('#panelForm').show();


                calendar.accion = "editar";
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
        calendar.accion = "nuevo";
        calendar.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            calendar.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};
                item.Id = calendar.idSeleccionado;
                item.Nombre = $('#txtNombre').val();
                item.Fecha = document.getElementById('txtFecha').value;

                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = calendar.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/Calendars.aspx/Save",
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

                            calendar.loadContent();


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
            params.id = calendar.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Calendars.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        calendar.loadContent();

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

    calendar.init();

    calendar.accionesBotones();

});


