'use strict';
let date = new Date();
let descargas = "colaboracion_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '81';

let idRefaccionSeleccionada = -1;
let idRequisicionSeleccionada = -1;


const colaboracion = {


    init: () => {

        
        $('#panelForm').hide();
        $('#panelTabla').show();

        $('#btnDescargar').attr('disabled', true);
        $('#btnGuardarAsociacion').attr('disabled', true);


        colaboracion.cargarComboMarcas();
        colaboracion.cargarItems();
        //colaboracion.cargarComboModelos();


    },

    cargarItems: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoColaboracion.aspx/GetItemsColaboraciones",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let len = msg.d.length;

                if (len === 0) {
                    $('#btnNuevo').attr('disabled', false);
                } else {
                    $('#btnNuevo').attr('disabled', true);
                }

                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "pageLength": 10,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'IdRequisicion' },
                        { data: 'NumeroEconomico' },
                        { data: 'NombreEquipo' },
                        { data: 'Descripcion' },
                        { data: 'FechaCreacionFormateada' },                      
                        { data: 'Accion' },

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



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    finalizar_colaboracion: (id, idEquipo) => {
        colaboracion.idSeleccionado = id;
        colaboracion.idEquipoSeleccionado = idEquipo;
        $('#msgFinalizar').html('<p>Se dara por finalizada esta colaboración de trabajo  (No.' + id + ') Posteriormente se puede volver a asociar. ¿Desea continuar?</p>');
        $('#panelFinalizar').modal('show');

    },


    cargarComboModelos: (idMarca, idModelo) => {
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


    cargarComboMarcas: () => {
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
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoColaboracion.aspx/GetItemsRequisiciones",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                $('#panelSeleccionarRequisicion').modal('show');


                let tablaRequisiciones = $('#tablaRequisiciones').DataTable({
                    pageLength: 10,
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

                                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                                $('#txtNombreEquipo').val(item.NombreEquipo);
                                $('#txtNombreProveedor').val(item.NombreProveedor);
                                $('#txtOrometro').val(item.Orometro);
                                $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);

                                $('#txtFechaCreacion').val(item.FechaCreacionFormateadaMx);
                                $('#txtDescripcion').val(item.Descripcion);
                                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                                $('#txtDiagnostico').val(item.Diagnostico);
                                $('#comboMarca').val(item.IdMarca);
                                colaboracion.cargarComboModelos(item.IdMarca, item.IdModelo);


                                let parametros = {
                                    path: window.location.hostname,
                                    id_requisicion: item.IdRequisicion,
                                    idUsuario: sessionStorage.getItem("idusuario"),
                                    id_tipo: 1
                                };

                                utils.getDocumentos(parametros);

                                $('#panelSeleccionarRequisicion').modal('hide');

                                $('#panelTabla').show();

                                $('#btnDescargar').attr('disabled', false);
                                $('#btnGuardarAsociacion').attr('disabled', false);

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



    cancelar: (id) => {

        idRefaccionSeleccionada = id;

        $('#msgCancelar').html(mensajesAlertas.confirmacionCancelarRefaccion.replace('{numrefaccion}', id));
        $('#panelEliminar').modal('show');

    },

    accionesBotones: () => {

        $('#btnAbrirSeleccionarRequisicion').on('click', (e) => {
            e.preventDefault();

            $('#btnDescargar').attr('disabled', true);
            $('#btnGuardarAsociacion').attr('disabled', true);

            colaboracion.cargarItemsRequisiciones();


        });

        $('#btnDescargar').on('click', (e) => {
            e.preventDefault();
            
            window.open(`../pages/Download.ashx?path=${window.location.hostname}&id_requisicion=${idRequisicionSeleccionada}&tipo_descarga=3`);

        });

        $('#btnNuevo').click(function (e) {
            //console.log('Desde nuevo');
            e.preventDefault();

            $('#frm')[0].reset();
            $('.form-group').removeClass('has-error');
            $('.help-block').empty();



            $('#panelTabla').hide();
            $('#panelForm').show();
            accion = "nuevo";
            colaboracion.id = -1;

        });

        
        $('#btnFinalizarAceptar').on('click', (e) => {

            e.preventDefault();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = colaboracion.idSeleccionado;
            parametros.idEquipo = colaboracion.idEquipoSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoColaboracion.aspx/FinalizarColaboracion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoFinalizarColaboracion, 'ok');


                        colaboracion.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });




        });

        $('#btnGuardarAsociacion').on('click', (e) => {

            e.preventDefault();


            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idUsuarioColaborador = sessionStorage.getItem("idusuario");
            parametros.idRequisicion = idRequisicionSeleccionada;
            parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoListadoFallas.aspx/AsignarColaborador",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoAsignarColaboracion, 'ok');

                        $('#frm')[0].reset();

                        $('#panelTabla').show();
                        $('#panelForm').hide();

                    } else {

                        utils.toast(mensajesAlertas.errorInesperado, 'error');


                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();
            $('#panelForm').hide();
            $('#panelTabla').show();

        });


    }


}

window.addEventListener('load', () => {

    colaboracion.init();

    colaboracion.accionesBotones();

});


