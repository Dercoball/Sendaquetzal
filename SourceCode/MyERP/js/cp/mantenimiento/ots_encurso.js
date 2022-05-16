'use strict';
let date = new Date();
let descargas = "ListadoGeneral_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '67';

/**
 * Controlar valores para OT's en curso
 * */
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
        $('#panelTablaRefacciones').show();

        listadoGeneral.idProveedorSeleccionado = -1;
        listadoGeneral.idEquipoSeleccionado = -1;
        listadoGeneral.idSeleccionado = -1;
        listadoGeneral.accion = '';
        listadoGeneral.idUsuarioDiagnostico = -1;

        listadoGeneral.cargarItems();
        listadoGeneral.cargarComboMarcas();

        datosEquipo.cargarComboEstatus();
        listadoGeneral.cargarEmpleados();
        datosEquipo.cargarComboUnidadesMedida();


        //  intentar cargar el id de Ot traido desde GET
        let urlParams = utils.parseURLParams(window.location.href);
        //console.log(`urlParams = ${urlParams['id']}`);
        if (urlParams != null && urlParams['id'] !== undefined) {

            let idOt = urlParams['id'][0];
            //console.log(`idOt = ${idOt}`);
            listadoGeneral.abrir(idOt);
        }

    },




    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
        parametros.mostrarSoloFinalizados = 0;

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
                        //{ data: 'FechaCierreFormateada' },
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


                $("#comboAsignarColaborador").jqxDropDownList({
                    source: dataAdapter, displayMember: "Nombre", valueMember: "IdEmpleado", width: '350px',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboAsignarColaborador").jqxDropDownList('clearSelection');


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


                console.log(`items`); 


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






    eliminar: (id) => {
        listadoGeneral.idSeleccionado = id;
        $('#msgEliminar').html('<p>Se va a eliminar esta orden de trabajo  (No.' + id + '). ¿Desea continuar?</p>');
        $('#panelEliminar').modal('show');

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

                $('#txtGeneradoPor').val(item.NombreUsuario);
                //debugger;
                $('#txtNombreOperador').val(item.NombreOperador);
                $('#txtTelefono').val(item.Telefono);
                $('#comboUnidadMedidaOrometro').val(item.IdUnidadMedidaOrometro);


                datosEquipo.cargarComboUbicaciones(item.IdUbicacion);


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

                datosEquipo.cargarComboModelos(item.IdMarca, item.IdModelo);

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



                datosEquipo.odometro(item.IdEquipo);


                let parametros = {
                    path: window.location.hostname,
                    id_requisicion: item.IdRequisicion,
                    idUsuario: sessionStorage.getItem("idusuario"),
                    id_tipo: 1
                };

                utils.getDocumentos(parametros);
                listadoGeneral.cargarItemsUsuarios(id);



                //  refacciones
                registroPanelRefacciones.idRequisicionSeleccionada = item.IdRequisicion;
                registroPanelRefacciones.cargarItems(item.IdRequisicion);
                $('#panelTablaRefacciones').show();
                //



                $('#panelTabla').hide();
                $('#panelForm').show();
                $('.deshabilitable').prop('disabled', true);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


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
            url: "../pages/MantenimientoListadoFallas.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                var item = msg.d;
                //debugger;

                listadoGeneral.idSeleccionado = item.IdRequisicion;
                listadoGeneral.idEquipoSeleccionado = item.IdEquipo;

                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                $('#txtNombreEquipo').val(item.NombreEquipo);

                $('#txtGeneradoPor').val(item.NombreUsuario);

                $('#txtNombreOperador').val(item.NombreOperador);
                $('#txtTelefono').val(item.Telefono);
                $('#comboUnidadMedidaOrometro').val(item.IdUnidadMedidaOrometro);

                datosEquipo.cargarComboUbicaciones(item.IdUbicacion);

                $('#txtFechaCreacion').val(item.FechaCreacionFormateadaMx);
                $('#txtFechaCreacion').prop('fecha', item.FechaCreacion);

                $('#txtOrometro').val(item.Orometro);
                $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);

                $('#txtDescripcion').val(item.Descripcion);
                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                $('#comboMarca').val(item.IdMarca);
                $('#comboPrioridad').val(item.IdNivelPrioridad);
                $('#txtDiagnostico').val(item.Diagnostico);


                datosEquipo.cargarComboModelos($('#comboMarca').val(), item.IdModelo);


                $("#comboEmpleado").jqxDropDownList('clearSelection');


                $("#comboEmpleado").val(item.IdUsuarioDiagnostico);
                $("#comboEmpleado").jqxDropDownList({ disabled: false });


                listadoGeneral.accion = "editar";
                listadoGeneral.IdUsuarioDiagnostico = item.IdUsuarioDiagnostico;

                $('#spnTituloForm').text('Editar');


                let parametros = {
                    path: window.location.hostname,
                    id_requisicion: item.IdRequisicion,
                    idUsuario: sessionStorage.getItem("idusuario"),
                    id_tipo: 1
                };

                datosEquipo.odometro(item.IdEquipo);


                utils.getDocumentos(parametros);
                listadoGeneral.cargarItemsUsuarios(id);

                $('#panelTabla').hide();
                $('#panelForm').show();


                //  refacciones
                registroPanelRefacciones.idRequisicionSeleccionada = item.IdRequisicion;
                registroPanelRefacciones.cargarItems(item.IdRequisicion);
                $('#panelTablaRefacciones').show();
                //

                $('.deshabilitable').prop('disabled', false);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },

    finalizar: (id, idEquipo) => {
        listadoGeneral.idSeleccionado = id;
        listadoGeneral.idEquipoSeleccionado = idEquipo;
        $('#msgFinalizar').html('<p>Se dara por finalizada esta orden de trabajo  (No.' + id + '). ¿Desea continuar?</p>');
        $('#panelFinalizar').modal('show');

    },

    status_espera: (id, idEquipo) => {
        listadoGeneral.idSeleccionado = id;
        listadoGeneral.idEquipoSeleccionado = idEquipo;
        listadoGeneral.idStatus = 4;
        $('#msgSetStatus').html('<p>Se enviará esta orden de trabajo a status  <strong> En espera de refacciones </strong>(No.' + id + '). ¿Desea continuar?</p>');
        $('#panelSetStatus').modal('show');

    },

    status_enatencion: (id, idEquipo) => {
        listadoGeneral.idSeleccionado = id;
        listadoGeneral.idEquipoSeleccionado = idEquipo;
        listadoGeneral.idStatus = 5;
        $('#msgSetStatus').html('<p>Se enviará esta orden a status <strong>En atención </strong>(No.' + id + '). ¿Desea continuar?</p>');
        $('#panelSetStatus').modal('show');

    },

    asignar_colaborador: (id, idEquipo) => {
        listadoGeneral.idSeleccionado = id;
        listadoGeneral.idEquipoSeleccionado = idEquipo;

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idRequisicion = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoListadoFallas.aspx/GetItemsColaboraciones",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                let items = msg.d;
                console.log(items);


                let html = `
                         

                            <div class="form-group col-md-12">
                    <ul>`;

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    html += `
						
										<li>
											${item.NombreUsuario}, ${item.FechaFormateadaMx}
										</li>
									

								
						`;

                }
                html += ` 
                    <ul> </div>`;

                $('#listaColaboradores').empty().append(html);

                $('#idRequisicionSpan').empty().append(id);



                $('#panelAsignarColaborador').modal('show');


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


        // PM1
        $('#btnCargarRefacciones').on('click', (e) => {
            e.preventDefault();
            console.log('btnCargarRefacciones PM1');


            var parametros = {};
            parametros.path = window.location.hostname;
            parametros.idEquipo = listadoGeneral.idEquipoSeleccionado;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idRequisicion = listadoGeneral.idSeleccionado;
            parametros = JSON.stringify(parametros);


            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoListadoFallas.aspx/GuardarListaRefaccionesPorEquipoPM1",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var items = msg.d;
                    console.log(items);

                    registroPanelRefacciones.idRequisicionSeleccionada = listadoGeneral.idSeleccionado;
                    registroPanelRefacciones.cargarItems(registroPanelRefacciones.idRequisicionSeleccionada);

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });



        $('#btnFinalizarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = listadoGeneral.idSeleccionado;
            parametros.idEquipo = listadoGeneral.idEquipoSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelDiagnostico.aspx/FinalizarRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoFinalizar, 'ok');


                        listadoGeneral.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });

        $('#btnSetStatusAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = listadoGeneral.idSeleccionado;
            parametros.idEquipo = listadoGeneral.idEquipoSeleccionado;
            parametros.status = listadoGeneral.idStatus;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoListadoFallas.aspx/setStatusRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoCambiarStatus, 'ok');

                        listadoGeneral.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    utils.toast(resultado.MensajeError, 'error');
                }

            });

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




            var item = {};
            item.IdRequisicion = listadoGeneral.idSeleccionado;
            item.IdEquipo = listadoGeneral.idEquipoSeleccionado;
            item.Descripcion = $('#txtDescripcion').val();
            item.Orometro = $('#txtOrometro').val();
            item.Diagnostico = $('#txtDiagnostico').val();
            item.DetieneOperacion = $('#chkDetieneOperacion').prop('checked') == true ? 1 : 0;
            item.IdNivelPrioridad = $('#comboPrioridad').val();
            item.IdUsuarioDiagnostico = $('#comboEmpleado').val() === null || $('#comboEmpleado').val() === ''
                ? '-1'
                : $('#comboEmpleado').val();
            item.IdUsuarioDiagnosticoAnterior = listadoGeneral.IdUsuarioDiagnostico;
            item.IdUsuario = sessionStorage.getItem("idusuario");

            var parametros = {};
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = listadoGeneral.accion;
            parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
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
            e.preventDefault();

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

        $('#btnEliminarAceptarImagen').on('click', (e) => {
            e.preventDefault();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = utils.idDoc;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoListadoFallas.aspx/EliminarImagen",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');

                        let parametros = {
                            path: window.location.hostname,
                            id_requisicion: listadoGeneral.idSeleccionado,
                            idUsuario: sessionStorage.getItem("idusuario"),
                            id_tipo: 1
                        };

                        utils.getDocumentos(parametros);


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


