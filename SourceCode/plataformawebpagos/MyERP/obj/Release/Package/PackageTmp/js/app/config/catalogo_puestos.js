'use strict';
let date = new Date();
let descargas = "Puesto_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '60';


class Puesto {



}

const puesto = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();
        puesto.accion = '';
        puesto.idSeleccionado = -1;
        puesto.cargarItems();

    },

    cargarItems: () => {

        var parametros = new Object();
        parametros.path = "connbd";
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/Puestos.aspx/GetListaItems';

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
                        { data: 'IdPuesto' },
                        { data: 'Nombre' },
                        { data: 'Clave' },
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

        puesto.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },



    editar: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        var parametros = new Object();
        parametros.path = "connbd";
        parametros.id = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Puestos.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                puesto.idSeleccionado = item.IdPuesto;

                console.log('.');
                $('#txtNombre').val(item.Nombre);
                $('#txtClave').val(item.Clave);
                $('#chkActivo').prop('checked', item.Activo === 1);

                $('#panelTabla').hide();
                $('#panelForm').show();


                puesto.accion = "editar";
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
        puesto.accion = "nuevo";
        puesto.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFechaCreacion');



    },



    accionesBotones: () => {






        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            puesto.nuevo();

        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }




            var item = new Object();
            item.IdPuesto = puesto.idSeleccionado;
            item.Activo = $('#chkActivo').prop('checked') == true ? 1 : 0;
            item.Nombre = $('#txtNombre').val();
            item.Clave = $('#txtClave').val();




            var parametros = new Object();
            parametros.path = "connbd";
            parametros.item = item;
            parametros.accion = puesto.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Puestos.aspx/Guardar",
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

                        puesto.cargarItems();

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
            parametros.path = "connbd";
            parametros.id = puesto.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Puestos.aspx/Eliminar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        puesto.cargarItems();

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

    puesto.init();

    puesto.accionesBotones();

});


