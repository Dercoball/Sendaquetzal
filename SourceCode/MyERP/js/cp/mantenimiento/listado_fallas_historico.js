'use strict';
let date = new Date();
let descargas = "HistoricosOT_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '74';


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

const listadoGeneral = {


    init: () => {

        $('#alert-operacion-ok').hide();
        $('#alert-operacion-fail').hide();

        $('#panelTabla').show();
        $('#panelForm').hide();
        $('#panelTablaCerrados').hide();

        listadoGeneral.idProveedorSeleccionado = -1;
        listadoGeneral.idEquipoSeleccionado = -1;
        listadoGeneral.idSeleccionado = -1;
        listadoGeneral.accion = '';
        listadoGeneral.idUsuarioDiagnostico = -1;

        listadoGeneral.cargarItems();
        datosEquipo.cargarComboMarcas();

        datosEquipo.cargarComboEstatus();
        listadoGeneral.cargarEmpleados();
        datosEquipo.cargarComboUnidadesMedida();

    },


    cargarComboUbicaciones: (idUbicacion) => {

        console.log('cargarComboUbicaciones');
        console.log(`idUbicacion = ${idUbicacion}`);

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/GetListaUbicaciones",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdUbicacion + '">' + item.Nombre + '</option>';

                }

                $('#comboUbicacion').html(opcion).promise().done(function () {
                    $('#comboUbicacion').val(idUbicacion);
                });



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },




    cargarItemsRefacciones: (idRequisicion) => {
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


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [0, 1, 2, 3, 4],
                            "orderable": false
                        },
                    ],
                    dom: 't'

                });




            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },




    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
        parametros.mostrarSoloFinalizados = 1;

        let url = '../pages/MantenimientoListadoFallas.aspx/GetListaItems';

        utils.postData(url, parametros)
            .then(data => {
                //console.log(data); // JSON data parsed by `data.json()` call


                data = data.d;



                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "pageLength": 10,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'IdRequisicion' },
                        { data: 'NumeroEconomico' },
                        { data: 'Accion' },
                        { data: 'NombreEquipo' },
                        { data: 'Descripcion' },
                        { data: 'FechaCreacionFormateada' },
                        { data: 'TiempoTranscurrido' },
                        { data: 'FechaCierreFormateada' },
                        { data: 'NombrePrioridad' },
                        { data: 'NombreUsuarioDiagnostico' },
                        { data: 'NombreStatus' },

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],

                    dom: 'fBrtipl',
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




                var datos = JSON.parse(msg.d);
                var dataAdapter = new $.jqx.dataAdapter(datos);


                $("#comboEmpleado").jqxDropDownList({
                    source: dataAdapter, displayMember: "Nombre", valueMember: "IdEmpleado", width: '350px',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboEmpleado").jqxDropDownList('clearSelection');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });

    },


    cargarItemsUsuarios: (idRequisicion) => {
        //console.log(`idRequisicion  = ${idRequisicion}`);

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idRequisicion = idRequisicion;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoListadoFallas.aspx/GetItemsUsuarios",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                //console.log(`${JSON.stringify(msg)}`); 


                let items = msg.d;



                //Generar seccion de diagnosticos anteriores
                let html = ``;

                for (var i = 0; i < items.length - 1; i++) {
                    var item = items[i];

                    html += `
							 <div class="row">



									<div class="form-group col-md-12">
										<label>
											${item.NombreUsuario}, ${item.FechaFormateadaMx}
										</label>
										<textarea class="form-control" rows="4" disabled="disabled">${item.Diagnostico}</textarea>
									</div>

								</div>
						`;

                }
                $('#divDiagnosticos').empty().append(html);

                if (items[items.length - 1] != null && items[items.length - 1].NombreUsuario != null) {
                    $('#lblUsuarioDiagnosticoActual').text(`${items[items.length - 1].NombreUsuario} (ACTUAL), ${items[items.length - 1].FechaFormateadaMx}`);
                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    reAbrir: (id) => {
        listadoGeneral.idSeleccionado = id;
        $('#msgReAbrir').html('<p>Se va a re-abrir esta orden de trabajo y pasarla a status "En diagnostico" (No.' + id + '). ¿Desea continuar?</p>');
        $('#panelReAbrir').modal('show');

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
            url: "../pages/MantenimientoListadoFallas.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                var item = msg.d;

                listadoGeneral.idSeleccionado = item.IdRequisicion;
                listadoGeneral.idEquipoSeleccionado = item.IdEquipo;
                listadoGeneral.idProveedorSeleccionado = item.IdProveedor;

                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                $('#txtNombreEquipo').val(item.NombreEquipo);
                $('#txtNombreProveedor').val(item.NombreProveedor);

                $('#txtGeneradoPor').val(item.NombreUsuario);

                $('#txtNombreOperador').val(item.NombreOperador);
                $('#txtTelefono').val(item.Telefono);
                $('#comboUnidadMedidaOrometro').val(item.IdUnidadMedidaOrometro);

                listadoGeneral.cargarComboUbicaciones(item.IdUbicacion);

                $('#txtFechaCreacion').val(item.FechaCreacionFormateadaMx);
                $('#txtFechaCreacion').prop('fecha', item.FechaCreacion);

                $('#txtOrometro').val(item.Orometro);
                $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);

                $('#txtDescripcion').val(item.Descripcion);
                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                $('#comboMarca').val(item.IdMarca);
                $('#comboPrioridad').val(item.IdNivelPrioridad);
                $('#txtDiagnostico').val(item.Diagnostico);

                $("#comboEmpleado").val(item.IdUsuarioDiagnostico);
                $("#comboEmpleado").jqxDropDownList({ disabled: true });

                datosEquipo.cargarComboModelos_(item.IdMarca, item.IdModelo);

                $('#txtPrioridadRojo').hide();
                $('#txtPrioridadAmarillo').hide();
                $('#txtPrioridadVerde').hide();

                //console.log(item.IdNivelPrioridad);

                switch (item.IdNivelPrioridad) {
                    case 1:
                        {

                            $('#txtPrioridadRojo').show();

                            break;
                        }
                    case 2:
                        {
                            $('#txtPrioridadAmarillo').show();

                            break;
                        }
                    case 3:
                        {
                            $('#txtPrioridadVerde').show();

                            break;
                        }

                }

                listadoGeneral.accion = "editar";
                $('#spnTituloForm').text('Datos orden de trabajo');

                $('.deshabilitable').prop('disabled', true);


                datosEquipo.odometro(item.IdEquipo);


                let parametros = {
                    path: window.location.hostname,
                    id_requisicion: item.IdRequisicion,
                    idUsuario: sessionStorage.getItem("idusuario"),
                    id_tipo: 1
                };

                utils.getDocumentos(parametros);
                listadoGeneral.cargarItemsUsuarios(id);
                listadoGeneral.cargarItemsRefacciones(id);

                $('#panelTabla').hide();
                $('#panelForm').show();


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },


    accionesBotones: () => {

        $('#btnDescargar').on('click', (e) => {
            e.preventDefault();

            window.open(`../pages/Download.ashx?path=${window.location.hostname}&id_requisicion=${listadoGeneral.idSeleccionado}&tipo_descarga=3`);

        });


        $('#btnReAbrirAceptar').on('click', (e) => {
            e.preventDefault();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = listadoGeneral.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoListadoFallas.aspx/ReAbrirRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoOperacion, 'ok');


                        listadoGeneral.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });

        $('#btnAbrirSeleccionarEquipo').on('click', (e) => {
            e.preventDefault();

            listadoGeneral.cargarItemsEquipos();
            $('#panelSeleccionarEquipo').modal('show');


        });



        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            listadoGeneral.nuevo();

        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }

            if (listadoGeneral.idEquipoSeleccionado == -1) {
                utils.toast(mensajesAlertas.errorSeleccionarEquipo, 'error');

                return;
            }




            var item = new Object();
            item.IdRequisicion = listadoGeneral.idSeleccionado;
            item.IdEquipo = listadoGeneral.idEquipoSeleccionado;
            item.Descripcion = $('#txtDescripcion').val();
            item.Orometro = $('#txtOrometro').val();
            item.Diagnostico = $('#txtDiagnostico').val();
            item.DetieneOperacion = $('#chkDetieneOperacion').prop('checked') == true ? 1 : 0;
            item.IdNivelPrioridad = $('#comboPrioridad').val();
            item.IdUsuarioDiagnostico = $('#comboEmpleado').val() === null ? '-1' : $('#comboEmpleado').val();
            item.IdUsuarioDiagnosticoAnterior = listadoGeneral.IdUsuarioDiagnostico

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = listadoGeneral.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoListadoFallas.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        listadoGeneral.idSeleccionado = valores.IdItem;

                        $('#txtNumeroRequisicion').val(listadoGeneral.idSeleccionado);

                        //guardar documentos
                        $('.file-documentos').each(function (documento) {


                            let file;
                            if (file = this.files[0]) {

                                utils.sendFile(file, 'documentos', valores.IdItem, 'documento');

                            }

                        });




                        $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                        $('#panelMensajes').modal('show');
                        listadoGeneral.cargarItems();

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


        //$('#btnAgregarOtroDocumento').on('click', (e) => {
        //	e.preventDefault();

        //	let htmlDoc = '<div class="form-group col-md-4">';
        //	htmlDoc += '<input type="file" class="form-control file-documentos" />';
        //	htmlDoc += '<div class="help-block with-errors"></div>';
        //	htmlDoc += '</div>';

        //	$('#divDocumentos').append(htmlDoc);


        //});


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = listadoGeneral.idSeleccionado;
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


                        listadoGeneral.cargarItems();

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
            parametros.id = listadoGeneral.idSeleccionado;
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


                        listadoGeneral.cargarItems();

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

    listadoGeneral.init();

    listadoGeneral.accionesBotones();

});


