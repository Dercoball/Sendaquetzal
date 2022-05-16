'use strict';
let date = new Date();
let descargas = "ResumenRequisicion_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '59';

let idRefaccionSeleccionada = -1;
let idRequisicionSeleccionada = -1;
class ResumenRequisicion {

    constructor(IdRegistroFalla) {
        this.IdRegistroFalla = IdRegistroFalla;
        this.IdEqúipo = null;
        this.FechaCreacion = null;
        this.Descripcion = '';
        this.Diagnostico = '';
        this.Orometro = '';
        this.DetieneOperacion = false;

    }

}

const resumenRequisicion = {


    init: () => {

        $('#alert-operacion-ok').hide();
        $('#alert-operacion-fail').hide();
        $('#panelForm').hide();
        $('#panelTabla').hide();
        


        resumenRequisicion.cargarComboMarcas();


    },



    cargarComboMarcas: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/ResumenRequisicion.aspx/GetListaMarcas",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value=""></option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id_Marca + '">' + item.Nombre + '</option>';

                }

                $('#comboMarca').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },




    cargarItemsRequisiciones: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/ResumenRequisicion.aspx/GetListaRequisicionesCreadas",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                let tablaRequisiciones = $('#tablaRequisiciones').DataTable({
                    pageLength: 5,
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'IdRequisicion' },
                        { data: 'NumeroEconomico' },
                        { data: 'NombreEquipo' },
                        { data: 'NombreStatus' },

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

                $('#panelSeleccionarRequisicion').modal('show');


                $('#tablaRequisiciones').on('draw.dt', function () {



                    $('.boton-seleccionar-requisicion').on('click', function (e) {
                        e.preventDefault();

                        idRequisicionSeleccionada = $(this).attr('data-idrequisicion');


                        var parametros = new Object();
                        parametros.path = window.location.hostname;
                        parametros.id = idRequisicionSeleccionada;
                        parametros = JSON.stringify(parametros);
                        $.ajax({
                            type: "POST",
                            url: "../pages/MantenimientoListadoFallas.aspx/GetItem",
                            data: parametros,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            success: function (msg) {

                                var item = msg.d;
                                idRequisicionSeleccionada = item.IdRequisicion;

                                $('#txtRequisicion').val(item.IdRequisicion);
                                $('#txtNombreEquipo').val(item.NombreEquipo);
                                $('#txtNombreProveedor').val(item.NombreProveedor);
                                $('#txtOrometro').val(item.Orometro);
                                $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);

                                $('#txtFechaCreacion').val(item.FechaCreacionFormateadaMx);
                                $('#txtDescripcion').val(item.Descripcion);
                                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                                $('#txtDiagnostico').val(item.Diagnostico);
                                $('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
                                $('#comboMarca').val(item.IdMarca);
                                cargarComboModelos($('#comboMarca').val(), item.IdModelo);


                                let parametros = {
                                    path: window.location.hostname,
                                    id_requisicion: item.IdRequisicion,
                                    idUsuario: sessionStorage.getItem("idusuario"),
                                    id_tipo: 1
                                };

                                $('#panelSeleccionarRequisicion').modal('hide');

                                $('#panelTabla').show();
                                resumenRequisicion.cargarItems(idRequisicionSeleccionada);


                                let parametros = {
                                    path: window.location.hostname,
                                    id_requisicion: item.IdRequisicion,
                                    idUsuario: sessionStorage.getItem("idusuario"),
                                    id_tipo: 1
                                };

                                utils.getDocumentos(parametros);

                            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                                console.log(textStatus + ": " + XMLHttpRequest.responseText);
                            }

                        });




                    });



                });

                $('#tablaRequisiciones').trigger('draw.dt');


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
            url: "../pages/ResumenRequisicion.aspx/GetItems",
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
                        { data: 'IdRefaccion' },
                        { data: 'NumeroParte' },
                        { data: 'FechaFormateadaMx' },
                        { data: 'Descripcion' },
                        { data: 'Cantidad' },
                        { data: 'Urgencia' },
                        

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: 'rti'

                });


                $('#tablaRefacciones').on('draw.dt', function () {


                });

                $('#tablaRefacciones').trigger('draw.dt');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    cargarComboModelos_: (idMarca, idModelo) => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idMarca = idMarca;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaModelos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value=""></option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id_Modelo + '">' + item.Nombre + '</option>';

                }

                $('#comboModelo').empty().append(opcion);

                if (idModelo !== 0) {
                    $('#comboModelo').val(idModelo);
                }
                //

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    cancelar: (id) => {

        idRefaccionSeleccionada = id;

        $('#msgCancelar').html(mensajesAlertas.confirmacionCancelarRefaccion.replace('{numrefaccion}', id));
        $('#panelEliminar').modal('show');

    },

    accionesBotones: () => {

        $('#btnAbrirSeleccionarRequisicion').on('click', (e) => {
            e.preventDefault();

            resumenRequisicion.cargarItemsRequisiciones();
            $('#panelSeleccionarRequisicion').modal('show');


        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();
            $('#panelForm').hide();
            $('#panelTabla').show();

        });


    }


}

window.addEventListener('load', () => {

    resumenRequisicion.init();

    resumenRequisicion.accionesBotones();

});


