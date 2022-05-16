'use strict';
let date = new Date();
let descargas = "RegistroDiagnostico_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '58';


class RegistroDiagnostico {

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

const registroDiagnostico = {


    init: () => {

        $('#alert-operacion-ok').hide();
        $('#alert-operacion-fail').hide();

        $('#panelTabla').show();
        $('#panelForm').hide();

        registroDiagnostico.idSeleccionado = -1;
        registroDiagnostico.idEquipoSeleccionado = -1;

        registroDiagnostico.cargarItems();
        registroDiagnostico.cargarComboMarcasRefacciones();

        datosEquipo.cargarComboMarcas();
        datosEquipo.cargarComboUbicaciones();
        datosEquipo.cargarComboUnidades();
        datosEquipo.cargarComboUnidadesMedida();

    },



    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/MantenimientoPanelDiagnostico.aspx/GetListaItems';



        utils.postData(url, parametros)
            .then(data => {
                //console.log(data); // JSON data parsed by `data.json()` call


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
                //var opcion = "";

                //for (var i = 0; i < items.length; i++) {
                //    var item = items[i];

                //    opcion += '<option value="' + item.Id_Marca + '">' + item.Nombre + '</option>';

                //}

                //$('#comboMarca').empty().append(opcion);


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

        registroDiagnostico.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },



    editar: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#divDocumentos').empty();

        $('#panelFormRefacciones').hide();
        $('#panelTablaRefacciones').hide();

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelDiagnostico.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                registroDiagnostico.idSeleccionado = item.IdRequisicion;
                registroDiagnostico.idEquipoSeleccionado = item.IdEquipo;

                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                $('#txtNombreEquipo').val(item.NombreEquipo);

                $('#txtGeneradoPor').val(item.NombreUsuario);

                $('#txtNombreOperador').val(item.NombreOperador);
                $('#txtTelefono').val(item.Telefono);



                $('#txtFechaCreacion').val(item.FechaCreacionFormateadaMx);
                $('#txtFechaCreacion').prop('fecha', item.FechaCreacionCanonical);

                $('#txtOrometro').val(item.Orometro);
                $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);

                $('#txtDescripcion').val(item.Descripcion);
                $('#txtDiagnostico').val(item.Diagnostico);
                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                //$('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
                $('#comboMarca').val(item.IdMarca);
                $('#comboUbicacion').val(item.IdUbicacion);
                datosEquipo.cargarComboModelos_(item.IdMarca, item.IdModelo);

                $('#txtPrioridadRojo').hide();
                $('#txtPrioridadAmarillo').hide();
                $('#txtPrioridadVerde').hide();

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

                registroDiagnostico.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);

                let parametros = {
                    path: window.location.hostname,
                    id_requisicion: item.IdRequisicion,
                    idUsuario: sessionStorage.getItem("idusuario"),
                    id_tipo: 1
                };



                utils.getDocumentos(parametros);

                registroPanelRefacciones.idRequisicionSeleccionada = item.IdRequisicion;

                registroPanelRefacciones.cargarItems(item.IdRequisicion);

                
                $('#panelTablaRefacciones').show();


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },



    finalizar: (id, idEquipo) => {
        registroDiagnostico.idSeleccionado = id;
        registroDiagnostico.idEquipoSeleccionado = idEquipo;
        $('#msgFinalizar').html('<p>Se dara por finalizada esta orden de trabajo  (No.' + id + '). ¿Desea continuar?</p>');
        $('#panelFinalizar').modal('show');

    },

    status_enatencion: (id, idEquipo) => {
        registroDiagnostico.idSeleccionado = id;
        registroDiagnostico.idEquipoSeleccionado = idEquipo;
        registroDiagnostico.idStatus = 5;
        $('#msgSetEnAtencion').html('<p>Se enviará esta orden a status <strong>En atención </strong>(No.' + id + '). ¿Desea continuar?</p>');
        $('#panelEnAtencion').modal('show');

    },

    status_proximoaatender: (id, idEquipo) => {
        registroDiagnostico.idSeleccionado = id;
        registroDiagnostico.idEquipoSeleccionado = idEquipo;
        registroDiagnostico.idStatus = 3;
        $('#msgSetStatus').html('<p>Se enviará esta orden a status <strong>Próximo a atender</strong>(No.' + id + '). ¿Desea continuar?</p>');
        $('#panelSetStatus').modal('show');

    },

    enviar: (id) => {
        registroDiagnostico.idSeleccionado = id;
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
            url: "../pages/MantenimientoPanelDiagnostico.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                registroDiagnostico.idSeleccionado = item.IdRequisicion;
                registroDiagnostico.idEquipoSeleccionado = item.IdEquipo;


                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                $('#txtNombreEquipo').val(item.NombreEquipo);
                $('#txtNombreProveedor').val(item.NombreProveedor);

                $('#txtFechaCreacion').val(item.FechaCreacionFormateada);
                $('#txtDescripcion').val(item.Descripcion);
                $('#txtNumeroEconomico').val(item.NumeroEconomico);
                //$('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
                $('#comboMarca').val(item.IdMarca);
                datosEquipo.cargarComboModelos_(item.IdMarca, item.IdModelo);


                registroDiagnostico.accion = "editar";
                $('#spnTituloForm').text('Datos requisición');

                $('.deshabilitable').prop('disabled', true);

                datosEquipo.odometro(item.IdEquipo);


                let parametros = {
                    path: window.location.hostname,
                    id_requisicion: item.IdRequisicion,
                    idUsuario: sessionStorage.getItem("idusuario"),
                    id_tipo: 1
                };

                datosEquipo.odometro(item.IdEquipo);


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
        $('#spnTituloForm').text('Reporte de fallas');
        $('#txtDescripcion').val('');




        $('#panelTabla').hide();
        $('#panelForm').show();
        registroDiagnostico.accion = "nuevo";
        registroDiagnostico.idSeleccionado = -1;
        registroDiagnostico.idEquipoSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFechaCreacion');



    },



    accionesBotones: () => {


        $('#btnSetEnAtencion').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = registroDiagnostico.idSeleccionado;
            parametros.idEquipo = registroDiagnostico.idEquipoSeleccionado;            
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.status = registroDiagnostico.idStatus;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelDiagnostico.aspx/setStatusEnAtencion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoCambiarStatus, 'ok');

                        registroDiagnostico.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    utils.toast(resultado.MensajeError, 'error');
                }

            });

        });


        $('#btnSetStatusAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = registroDiagnostico.idSeleccionado;
            parametros.idEquipo = registroDiagnostico.idEquipoSeleccionado;
            parametros.status = registroDiagnostico.idStatus;
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

                        registroDiagnostico.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');

                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    utils.toast(resultado.MensajeError, 'error');
                }

            });

        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }

            if (registroDiagnostico.idEquipoSeleccionado == -1) {
                utils.toast(mensajesAlertas.errorSeleccionarEquipo, 'error');

                return;
            }




            var item = new Object();
            item.IdRequisicion = registroDiagnostico.idSeleccionado;
            item.Diagnostico = $('#txtDiagnostico').val();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = registroDiagnostico.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelDiagnostico.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        registroDiagnostico.idSeleccionado = valores.IdItem;

                        $('#txtNumeroRequisicion').val(registroDiagnostico.idSeleccionado);

                        //guardar documentos
                        $('.file-documentos').each(function (documento) {


                            let file;
                            if (file = this.files[0]) {

                                utils.sendFile(file, 'documentos', valores.IdItem, 'documento');

                            }

                        });




                        $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                        $('#panelMensajes').modal('show');
                        registroDiagnostico.cargarItems();

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

            let htmlDoc = `<div class="form-group col-md-4">
                            <input type="file" class="form-control file-documentos" />
                            <div class="help-block with-errors"></div>
                            </div>`;

            $('#divDocumentos').append(htmlDoc);


        });


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnFinalizarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = registroDiagnostico.idSeleccionado;
            parametros.idEquipo = registroDiagnostico.idEquipoSeleccionado;
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


                        registroDiagnostico.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });


        $('#btnEnviarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = registroDiagnostico.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelDiagnostico.aspx/EnviarRequisicion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEnviar, 'ok');


                        registroDiagnostico.cargarItems();

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

    registroDiagnostico.init();

    registroDiagnostico.accionesBotones();

});


