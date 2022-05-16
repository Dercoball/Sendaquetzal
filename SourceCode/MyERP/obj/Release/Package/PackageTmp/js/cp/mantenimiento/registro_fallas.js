'use strict';
let date = new Date();
let descargas = "OrdenTrabajo_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '53';


class RegistroFalla {

    constructor(IdRegistroFalla) {
        this.IdRegistroFalla = IdRegistroFalla;
        this.IdEqúipo = null;
        this.FechaCreacion = null;
        this.Descripcion = '';
        this.Orometro = '';
        this.DetieneOperacion = false;

    }

}

const registroFallas = {


    init: () => {

        $('#alert-operacion-ok').hide();
        $('#alert-operacion-fail').hide();

        $('#panelTabla').show();
        $('#panelForm').hide();
        $('#panelTablaCerrados').hide();

        registroFallas.idProveedorSeleccionado = -1;
        registroFallas.idEquipoSeleccionado = -1;
        registroFallas.IdUnidadMedidaOrometro = -1;
        registroFallas.NumeroEconomico = -1;
        registroFallas.idSeleccionado = -1;
        registroFallas.accion = '';

        registroFallas.cargarItems();
        datosEquipo.cargarComboMarcas();

        datosEquipo.cargarComboEstatus();
        //registroFallas.cargarEmpleados();
        datosEquipo.cargarComboUnidadesMedida();
        datosEquipo.cargarComboUbicaciones();



    },



    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/MantenimientoPanelRegistroFallas.aspx/GetListaItems';

        utils.postData(url, parametros)
            .then(data => {


                data = data.d;



                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'IdRequisicion' },
                        { data: 'NumeroEconomico' },
                        { data: 'NombreEquipo' },
                        { data: 'FechaCreacionFormateada' },
                        { data: 'NombrePrioridad' },
                        { data: 'NombreStatus' },
                        { data: 'NombreUsuario' },
                        { data: 'Accion' }

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


    cargarItemsEquipos: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelRegistroFallas.aspx/GetListaEquiposSinRequisicion",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                let tablaClientes = $('#tablaEquipos').DataTable({
                    pageLength: 5,
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'NumeroEconomico' },
                        { data: 'Nombre' },
                        { data: 'NombreMarca' },

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

               


                $('#tablaEquipos').on('draw.dt', function () {



                    $('.boton-seleccionar-equipo').on('click', function (e) {
                        e.preventDefault();
                        e.stopImmediatePropagation();

                        registroFallas.idEquipoSeleccionado = $(this).attr('data-idequipo');
                        $('#txtNombreEquipo').val(registroFallas.idEquipoSeleccionado);


                        var parametros = new Object();
                        parametros.path = window.location.hostname;
                        parametros.id = registroFallas.idEquipoSeleccionado;
                        parametros = JSON.stringify(parametros);
                        $.ajax({
                            type: "POST",
                            url: "../pages/Proyectos.aspx/GetItemEquipo",
                            data: parametros,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            success: function (msg) {


                                var datos = msg.d;
                                registroFallas.idEquipoSeleccionado = datos.IdEquipo;
                                registroFallas.IdUnidadMedidaOrometro = datos.IdUnidadMedidaOrometro;
                                registroFallas.NumeroEconomico = datos.NumeroEconomico;

                                $('#txtNombreEquipo').val(datos.Nombre);
                                $('#txtNumeroEconomico').val(datos.NumeroEconomico);

                                $('#txtNombreEquipo').val(datos.Nombre);
                                $('#txtNombreOperador').val(datos.NombreOperador);
                                $('#txtTelefono').val(datos.Telefono);
                                $('#comboMarca').val(datos.IdMarca);
                                $('#comboUnidadMedidaOrometro').val(datos.IdUnidadMedidaOrometro);


                                datosEquipo.cargarComboModelos(datos.IdMarca, datos.IdModelo);


                                $('#panelSeleccionarEquipo').modal('hide');


                            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                                console.log(textStatus + ": " + XMLHttpRequest.responseText);
                            }

                        });




                    });



                });

                $('#tablaEquipos').trigger('draw.dt');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    cargarEmpleados: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.like = '';
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelRegistroFallas.aspx/GetListaEmpleados",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                //console.log(msg.d);



                var datos = JSON.parse(msg.d);
                var dataAdapter = new $.jqx.dataAdapter(datos);


                $("#comboEmpleado").jqxDropDownList({
                    source: dataAdapter, displayMember: "Nombre", valueMember: "IdEmpleado", width: '300px',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboEmpleado").jqxDropDownList('clearSelection');

                $("#comboEmpleado").jqxDropDownList({ disabled: true });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });

    },

    cargarOperadores: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelRegistroFallas.aspx/GetListaOperadores",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                var datos = JSON.parse(msg.d);
                var dataAdapter = new $.jqx.dataAdapter(datos);


                $("#comboOperador").jqxDropDownList({
                    source: dataAdapter, displayMember: "Nombre", valueMember: "IdEmpleado", width: '300px',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboOperador").jqxDropDownList('clearSelection');



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });

    },



    eliminar: (id) => {

        registroFallas.idSeleccionado = id;

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
            url: "../pages/MantenimientoPanelRegistroFallas.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                registroFallas.idSeleccionado = item.IdRequisicion;
                registroFallas.idEquipoSeleccionado = item.IdEquipo;
                registroFallas.IdUnidadMedidaOrometro = item.IdUnidadMedidaOrometro;
                registroFallas.NumeroEconomico = item.NumeroEconomico;

                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                $('#txtNombreEquipo').val(item.NombreEquipo);

                $('#txtNombreOperador').val(item.NombreOperador);
                $('#txtTelefono').val(item.Telefono);

                $('#txtFechaCreacion').val(item.FechaCreacionFormateadaMx);
                $('#txtFechaCreacion').prop('fecha', item.FechaCreacionCanonical);

                $('#txtOrometro').val(item.Orometro);

                $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);

                $('#txtDescripcion').val(item.Descripcion);
                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                $('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
                $('#comboMarca').val(item.IdMarca);
                $('#comboPrioridad').val(item.IdNivelPrioridad);
                $('#comboUbicacion').val(item.IdUbicacion);

                datosEquipo.cargarComboModelos(item.IdMarca, item.IdModelo);

                $("#comboEmpleado").jqxDropDownList('clearSelection');
                $("#comboEmpleado").jqxDropDownList({ disabled: true });
                $('#comboUnidadMedidaOrometro').val(item.IdUnidadMedidaOrometro);


                $("#comboEmpleado").val(item.IdUsuarioDiagnostico);

                registroFallas.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);
                $('#txtOrometro').prop('disabled', true);


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


    },




    enviar: (id) => {
        registroFallas.idSeleccionado = id;
        $('#msgEnviar').html('<p>Se enviará a la siguiente etapa el registro seleccionado(No.' + id + '). ¿Desea continuar?</p>');
        $('#panelEnviar').modal('show');

    },


    abrir: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelRegistroFallas.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;

                console.log(item);

                registroFallas.idSeleccionado = item.IdRequisicion;
                registroFallas.idEquipoSeleccionado = item.IdEquipo;
                registroFallas.IdUnidadMedidaOrometro = item.IdUnidadMedidaOrometro;
                registroFallas.NumeroEconomico = item.NumeroEconomico;

                console.log('.');
                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                $('#txtNombreEquipo').val(item.NombreEquipo);

                $('#txtNombreOperador').val(item.NombreOperador);
                $('#txtTelefono').val(item.Telefono);

                $('#txtFechaCreacion').val(item.FechaCreacionFormateada);
                $('#txtDescripcion').val(item.Descripcion);
                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                $('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
                $('#comboMarca').val(item.IdMarca);
                $('#comboUnidadMedidaOrometro').val(item.IdUnidadMedidaOrometro);
                $('#comboPrioridad').val(item.IdNivelPrioridad);

                datosEquipo.cargarComboModelos(item.IdMarca, item.IdModelo);


                registroFallas.accion = "editar";
                $('#spnTituloForm').text('Datos orden de trabajo');

                $('.deshabilitable').prop('disabled', true);




                let parametros = {
                    path: window.location.hostname,
                    id_requisicion: item.IdRequisicion,
                    idUsuario: sessionStorage.getItem("idusuario"),
                    id_tipo: 1
                };

                utils.getDocumentos(parametros);


                $('#panelTabla').hide();
                $('#panelForm').show();


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },


    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#txtDescripcion').val('');

        $('#txtOrometro').prop('disabled', false);



        $('#panelTabla').hide();
        $('#panelForm').show();
        registroFallas.accion = "nuevo";
        registroFallas.idSeleccionado = -1;
        registroFallas.idEquipoSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);
        $("#comboEmpleado").jqxDropDownList({ disabled: true });

        obtenerFechaHoraServidor('txtFechaCreacion');

        $("#comboEmpleado").jqxDropDownList('clearSelection');

        $('#divDocumentosExistentes').empty();

    },



    accionesBotones: () => {


        $('#btnAbrirSeleccionarEquipo').on('click', (e) => {
            e.preventDefault();

            registroFallas.cargarItemsEquipos();
            $('#panelSeleccionarEquipo').modal('show');


        });



        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            registroFallas.nuevo();

        });



        //$('#txtOrometro').on('blur', (e) => {
        //    e.preventDefault();
        //    console.log('change value orometro');
        //    let valor = $('#txtOrometro').jqxMaskedInput('value');
        //    console.log(`Orometro = ${valor}`);
        //    let valorClean = valor.replaceAll('_', '0');
        //    console.log(`valorClean = ${valorClean}`);
        //    let valorOrometro = Number(valorClean);
        //    console.log(`valorClean = ${valorOrometro}`);
        //    $('#txtOrometro').jqxMaskedInput('value', valorClean);

        //});


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }

            //let valorOrometro = $('#txtOrometro').jqxMaskedInput('value');
            //let valorClean = valorOrometro.replaceAll('_', '0');
            //let valorNumericoOrometro = Number(valorClean);

            //if (valorNumericoOrometro <= 0) {
            //    console.log(`debe indicar el otrometro`);
            //    utils.toast(mensajesAlertas.errorValorOrometro, 'error');

            //    return;
            //}

            if (registroFallas.idEquipoSeleccionado == -1) {
                utils.toast(mensajesAlertas.errorSeleccionarEquipo, 'error');

                return;
            }

            const idUbicacion = $('#comboUbicacion').val();
            if (idUbicacion === '') {
                utils.toast(mensajesAlertas.errorSeleccionarUbicacion, 'error');

                return;
            }


            var item = new Object();
            item.IdRequisicion = registroFallas.idSeleccionado;
            item.FechaCreacion = $('#txtFechaCreacion').prop('fecha');
            item.IdEquipo = registroFallas.idEquipoSeleccionado;
            item.Descripcion = $('#txtDescripcion').val();

            item.Orometro = $("#txtOrometro").val();
            item.IdUnidadMedidaOrometro = registroFallas.IdUnidadMedidaOrometro;
            item.NumeroEconomico = registroFallas.NumeroEconomico;

            item.IdUbicacion = idUbicacion;
            item.DetieneOperacion = $('#chkDetieneOperacion').prop('checked') == true ? 1 : 0;
            item.IdUsuario = sessionStorage.getItem("idusuario");

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = registroFallas.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelRegistroFallas.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        registroFallas.idSeleccionado = valores.IdItem;

                        $('#txtNumeroRequisicion').val(registroFallas.idSeleccionado);

                        //guardar documentos
                        $('.file-documentos').each(function (documento) {


                            let file;
                            if (file = this.files[0]) {

                                utils.sendFile(file, 'documentos', valores.IdItem, 'documento');

                            }

                        });




                        $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                        $('#panelMensajes').modal('show');
                        registroFallas.cargarItems();

                    } else {

                        utils.toast(valores.MensajeError, 'error');

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
            parametros.id = registroFallas.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelRegistroFallas.aspx/EliminarRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        registroFallas.cargarItems();

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
            parametros.id = registroFallas.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelRegistroFallas.aspx/EnviarRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEnviar, 'ok');


                        registroFallas.cargarItems();

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

    registroFallas.init();

    registroFallas.accionesBotones();

});


