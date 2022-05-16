'use strict';


class PanelSolicitudRefacciones {



}

const registroPanelRefacciones = {


    init: () => {

        $('#panelFormRefacciones').hide();
        $('#panelTablaRefacciones').hide();

        registroPanelRefacciones.accion = '';


        registroPanelRefacciones.cargarComboMarcasRefacciones();

        registroPanelRefacciones.idRequisicionSeleccionada = -1;
        registroPanelRefacciones.idRefaccionSeleccionada = -1;

    },



    cargarComboMarcasRefacciones: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaMarcas",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
             

                var dataAdapter = new $.jqx.dataAdapter(items);


                $("#comboMarcaProveedor").jqxDropDownList({
                    source: dataAdapter, displayMember: "Nombre", valueMember: "Id_Marca", width: '250px',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboMarcaProveedor").jqxDropDownList('clearSelection');



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },




    cargarItems: (idRequisicion) => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idRequisicion = idRequisicion;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelSolicitudRefacciones.aspx/GetItems",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {



                let tablaRefacciones = $('#tablaRefacciones').DataTable({
                    pageLength: 50,
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'FechaFormateadaMx' },
                        { data: 'NombreMarca' },
                        { data: 'Cantidad' },
                        { data: 'NumeroParte' },
                        { data: 'Descripcion' },
                        { data: 'ValorCaptura' },
                        { data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [0, 1, 2, 3, 4, 5],
                            "orderable": false
                        },
                    ],
                    dom: 't'

                });


                $('#tablaRefacciones').on('draw.dt', function () {


                });

                $('#tablaRefacciones').trigger('draw.dt');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    nuevo: () => {

        console.log('nuevo refaccion');

        $('#frmRefaccion')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        registroPanelRefacciones.accion = "nuevo";

        $('#panelTablaRefacciones').hide();
        $('#panelFormRefacciones').show();

        obtenerFechaHoraServidor('txtFecha');

    },

    cancelarRefacciones: (id) => {

        registroPanelRefacciones.idRefaccionSeleccionada = id;

        $('#msgCancelar').html(mensajesAlertas.confirmacionCancelarRefaccion.replace('{numrefaccion}', id));
        $('#panelCancelarRefaccion').modal('show');

    },

    accionesBotones: () => {


        $('#btnEliminarAceptarRefaccion').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = registroPanelRefacciones.idRefaccionSeleccionada;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelSolicitudRefacciones.aspx/CancelarRefaccion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    $('#panelCancelarRefaccion').modal('hide');

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoCancelarRefaccion, 'ok');


                        registroPanelRefacciones.cargarItems(registroPanelRefacciones.idRequisicionSeleccionada);

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();


            registroPanelRefacciones.nuevo();

        });


        $('#btnGuardarRefaccion').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frmRefaccion"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }

            if ($('#comboMarcaProveedor').val() == null || $('#comboMarcaProveedor').val() === '') {
                utils.toast(mensajesAlertas.errorSeleccionarMarca, 'error');

                return;

            }

            var item = new Object();
            item.IdRequisicion = registroPanelRefacciones.idRequisicionSeleccionada;
            item.Descripcion = $('#txtDescripcionRefaccion').val();
            item.NumeroParte = $('#txtNumeroParte').val();
            item.Cantidad = $('#txtCantidad').val();
            item.IdMarca = $('#comboMarcaProveedor').val() === null ? '-1' : $('#comboMarcaProveedor').val();;
            item.Fecha = $('#txtFecha').prop('fecha');
            item.IdUnidad = 1;
            item.Urgencia = 'urgente'; //   $("input[name='urgenteono']:checked").val();
            item.idUsuario = sessionStorage.getItem("idusuario");


            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = registroPanelRefacciones.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelSolicitudRefacciones.aspx/GuardarRefaccion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;

                    if (valores.CodigoError == 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        $('#panelFormRefacciones').hide();
                        $('#panelTablaRefacciones').show();

                        registroPanelRefacciones.cargarItems(registroPanelRefacciones.idRequisicionSeleccionada);

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);

                    utils.toast(mensajesAlertas.errorGuardar, 'error');
                }

            });

        });


        $('#btnCancelarRefaccion').on('click', (e) => {
            e.preventDefault();
            $('#panelFormRefacciones').hide();
            $('#panelTablaRefacciones').show();

        });


    }


}

window.addEventListener('load', () => {

    registroPanelRefacciones.init();

    registroPanelRefacciones.accionesBotones();

});


