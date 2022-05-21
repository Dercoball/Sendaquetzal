'use strict';
let date = new Date();
let descargas = "TiposUsuario_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '10';

let idEquipoSeleccionado = -1;


const tiposUsuario = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        tiposUsuario.idSeleccionado = -1;
        tiposUsuario.accion = '';
        tiposUsuario.cargarItems();

    },

    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = document.getElementById('txtIdUsuario').value;

        let url = '../../pages/Config/Positions.aspx/GetListaItems';
        
        utils.postData(url, parametros)
            .then(data => {
                console.log(data); // JSON data parsed by `data.json()` call


                data = data.d;



                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'IdPosicion' },
                        { data: 'Nombre' },
                        { data: 'ActivoStr' },
                        { data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: 'frtiplB',
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

            });







    },


    eliminar: (id) => {

        tiposUsuario.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },



    editar: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../../pages/Config/Positions.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                tiposUsuario.idSeleccionado = item.IdTipoUsuario;

                console.log('.');
                $('#txtNombre').val(item.Nombre);
                $('#chkActivo').prop('checked', item.Activo === 1);

                $('#panelTabla').hide();
                $('#panelForm').show();


                tiposUsuario.accion = "editar";
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
        $('#txtDescripcion').val('');




        $('#panelTabla').hide();
        $('#panelForm').show();
        tiposUsuario.accion = "nuevo";
        tiposUsuario.idSeleccionado = -1;
        idEquipoSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFechaCreacion');



    },



    permisos: (id, nombre) => {

        tiposUsuario.idSeleccionado = id;
        $('.spnNombreTipoUsuario').text(nombre);
        


        tiposUsuario.cargarListaPermisos(id);
        tiposUsuario.cargarListaPermisosUsuario(id);

        $('#panelPermisos').modal('show');

    },

    cargarListaPermisos: (idTipoUsuario) => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idTipoUsuario = idTipoUsuario;

        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../../pages/Config/Positions.aspx/ObtenerListaPermisos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdPermiso + '">' + item.Nombre + '</option>';

                }

                $('#listaPermisos').empty().append(opcion);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },

    cargarListaPermisosUsuario: (id) => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idTipoUsuario = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../../pages/Config/Positions.aspx/ObtenerListaPermisosPorTipoUsuario",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdPermiso + '">' + item.Nombre + '</option>';

                }

                $('#listaPermisosSeleccionados').empty().append(opcion);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },


    accionesBotones: () => {


        $("#listaPermisos").on("change",  (e) => {
            e.preventDefault();

            let args = e.currentTarget.selectedOptions;

            if (args[0].value) {
                let value = args[0].value;
                let text = args[0].text;

                let item = '<option value="' + value + '">' + text + "</option>";

                $("#listaPermisosSeleccionados").append(item);
                $("#listaPermisos option[value='" + value + "']").remove();
            }
        });

        $("#listaPermisosSeleccionados").on("change", (e) =>  {
            e.preventDefault();

            let args = e.currentTarget.selectedOptions;

            if (args[0].value) {
                let value = args[0].value;
                let text = args[0].text;

                let item = '<option value="' + value + '">' + text + "</option>";

                $("#listaPermisos").append(item);
                $("#listaPermisosSeleccionados option[value='" + value + "']").remove();
            }
        });


        $('#btnGuardarPermisos').on('click', (e) => {
            console.log('Boton guardarP');

            e.preventDefault();



            var listaP = [];

            $("#listaPermisosSeleccionados option").each(function () {

                var item = new Object();
                item.IdPermiso = $(this).val();
                item.IdTipoUsuario = tiposUsuario.idSeleccionado;

                listaP.push(item);
            });


            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.listaPermisos = listaP;
            parametros.idTipoUsuario = tiposUsuario.idSeleccionado;

            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "Positions.aspx/GuardarPermisosUsuario",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var resultado = parseInt(msg.d);

                    if (resultado > 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');



                    } else {
                        utils.toast(mensajesAlertas.errorGuardar, 'ok');


                    }
                    $('#listaPermisosSeleccionados').empty();

                    $('#panelPermisos').modal('hide');


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });




        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            tiposUsuario.nuevo();

        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }




            var item = new Object();
            item.IdTipoUsuario = tiposUsuario.idSeleccionado;
            item.Activo = $('#chkActivo').prop('checked') == true ? 1 : 0;
            item.NOmbre = $('#txtNombre').val();




            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = tiposUsuario.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../../pages/Config/Positions.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        tiposUsuario.cargarItems();

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = tiposUsuario.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../../pages/Config/Positions.aspx/EliminarTipoUsuario",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        tiposUsuario.cargarItems();

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

    tiposUsuario.init();

    tiposUsuario.accionesBotones();

});


