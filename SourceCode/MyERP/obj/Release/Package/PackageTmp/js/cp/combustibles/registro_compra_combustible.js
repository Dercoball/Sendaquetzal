'use strict';
let date = new Date();
let descargas = "ComprasCombustible_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '71';


class SolicitudCombustible {

    constructor(Id_) {
        this.IdRegistroFalla = Id_;
        this.IdEquipo = null;
        this.FechaCreacion = null;

    }

}

const registroSolicitud = {


    init: () => {

        $('#alert-operacion-ok').hide();
        $('#alert-operacion-fail').hide();

        $('#panelTabla').show();
        $('#panelForm').hide();
        $('#panelTablaCerrados').hide();

        registroSolicitud.idProveedorSeleccionado = -1;
        registroSolicitud.idSeleccionado = -1;
        registroSolicitud.accion = '';

        registroSolicitud.cargarItems();

    },




    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/PanelCompraCombustible.aspx/GetListaItems';

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
                        { data: 'IdCompraCombustible' },
                        { data: 'FechaFormateadaMx' },
                        { data: 'Cantidad' },
                        { data: 'NombreProveedor' }
                        //{ data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: 'fBrtip',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Exportar Csv', className: 'csvbtn'
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

            });



    },


    cargarItemsProveedores: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/ProveedoresCombustible.aspx/GetListaProveedores",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                let tablaProveedores = $('#tablaProveedores').DataTable({
                    pageLength: 5,
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'IdProveedor' },
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
                    dom: 'frtipl'

                });



                $('#tablaProveedores').on('draw.dt', function () {



                    $('.boton-seleccionar').on('click', function (e) {
                        e.preventDefault();

                        registroSolicitud.idProveedorSeleccionado = $(this).attr('data-idproveedor');


                        var parametros = new Object();
                        parametros.path = window.location.hostname;
                        parametros.id = registroSolicitud.idProveedorSeleccionado;
                        parametros = JSON.stringify(parametros);
                        $.ajax({
                            type: "POST",
                            url: "../pages/ProveedoresCombustible.aspx/GetItem",
                            data: parametros,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            success: function (msg) {


                                var datos = msg.d;

                                $('#txtNombreProveedor').val(datos.Nombre);


                                $('#panelSeleccionarProveedor').modal('hide');


                            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                                console.log(textStatus + ": " + XMLHttpRequest.responseText);
                            }

                        });




                    });



                });

                $('#tablaProveedores').trigger('draw.dt');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    eliminar: (id) => {

        registroSolicitud.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },


  

    editar: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#divDocumentos').empty();

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelCompraCombustible.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
               
                $('#txtFecha').val(item.FechaFormateadaMx);
                $('#txtCantidad').val(item.Cantidad);
                $('#txtFecha').prop('fecha', item.FechaCanonical);


                registroSolicitud.accion = "editar";
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
        $('#spnTituloForm').text('Reporte de fallas');




        $('#panelTabla').hide();
        $('#panelForm').show();
        registroSolicitud.accion = "nuevo";
        registroSolicitud.idSeleccionado = -1;
        registroSolicitud.idEquipoSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFecha');



    },



    accionesBotones: () => {


        $('#btnAbrirSeleccionarProveedor').on('click', (e) => {
            e.preventDefault();

            registroSolicitud.cargarItemsProveedores();
            $('#panelSeleccionarProveedor').modal('show');


        });



        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            registroSolicitud.nuevo();

        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }

            if (registroSolicitud.idProveedorSeleccionado == -1) {
                utils.toast(mensajesAlertas.errorSeleccionarProveedor, 'error');

                return;
            }



            var item = new Object();
            item.IdCompraCombustible = registroSolicitud.idSeleccionado;
            item.Fecha = $('#txtFecha').prop('fecha');
            item.idProveedor = registroSolicitud.idProveedorSeleccionado;
            item.Cantidad = $('#txtCantidad').val();
            item.IdUsuario = sessionStorage.getItem("idusuario");

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = registroSolicitud.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelCompraCombustible.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        registroSolicitud.idSeleccionado = valores.IdItem;

            
                        $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                        $('#panelMensajes').modal('show');
                        registroSolicitud.cargarItems();

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);



                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });


        $('#btnAceptarPanelMensajes').on('click', (e) => {

            e.preventDefault();
            $('#panelMensajes').modal('hide');

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnAgregarOtroDocumento').on('click', (e) => {
            e.preventDefault();

            let htmlDoc = '<div class="form-group col-md-4">';
            htmlDoc += '<input type="file" class="form-control file-documentos" />';
            htmlDoc += '<div class="help-block with-errors"></div>';
            htmlDoc += '</div>';

            $('#divDocumentos').append(htmlDoc);


        });


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = registroSolicitud.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelCompraCombustible.aspx/EliminarRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        registroSolicitud.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });

        $('#btnEnviarAceptar').on('click', (e) => {
            e.preventDefault();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = registroSolicitud.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelCompraCombustible.aspx/EnviarRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEnviar, 'ok');


                        registroSolicitud.cargarItems();

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

    registroSolicitud.init();

    registroSolicitud.accionesBotones();

});


