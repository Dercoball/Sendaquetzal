'use strict';
let date = new Date();
let descargas = "AprobacionCombustible_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '82';


let solicitud = {
    idSolicitud: -1,
    idUsuario: -1,
    solicitud: {},
    arrayData: [],
    accion: '',
    idSeleccionado: -1
}


class SolicitudCombustible {

    constructor(Id_) {
        this.IdRegistroFalla = Id_;
        this.IdEquipo = null;
        this.FechaCreacion = null;

    }

}

const registroAprobacion = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        registroAprobacion.idSeleccionado = -1;
        registroAprobacion.idSolicitud = -1;
        registroAprobacion.idDetalleSolicitud = -1;
        registroAprobacion.idDetalleSeleccionado = -1;
        registroAprobacion.idTipoSolicitud = -1;
        registroAprobacion.accion = '';

        registroAprobacion.cargarComboProveedoresCombustible();
        registroAprobacion.cargarItems();
        registroAprobacion.fecha = '';


    },


    cargarComboProveedoresCombustible: () => {

        let param_ = {};
        param_.path = window.location.hostname;
        param_ = JSON.stringify(param_);

        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Aprobacion.aspx/GetListaProveedoresCombustible",
            data: param_,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let datos = msg.d;
                let html = `<option value="0">Seleccione...</option>`;

                for (let i = 0; i < datos.length; i++) {
                    let item = datos[i];
                    html += `<option value="${item.IdProveedor}">${item.Nombre}</option>`;
                }

                $('#comboProveedorCombustible').empty().append(html);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });


    },

    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/Combustibles_Aprobacion.aspx/GetListaItems';

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
                        { data: 'IdSolicitud' },
                        { data: 'FechaFormateadaMx' },
                        { data: 'NombreUsuarioSolicita' },
                        { data: 'NombreStatus' },
                        { data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [0, 1, 2, 3],
                            "orderable": false
                        },
                    ],
                    //dom: 'fBrtip',
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




    eliminar: (id) => {

        registroAprobacion.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },



    cancelar: (idSolicitud, idDetalleSolicitud) => {

        registroAprobacion.idSolicitud = idSolicitud;
        registroAprobacion.idDetalleSeleccionado = idDetalleSolicitud;

        $('#msgCancelarSolicitud').html('Se marcará como cancelada la solicitud de combustible seleccionada (No.' + idDetalleSolicitud + '). ¿Desea continuar?');
        $('#panelCancelar').modal('show');

    },


    finalizar: (id) => {
        registroAprobacion.idSeleccionado = id;
        $('#msgFinalizar').html('Se dará por concluida esta solicitud y se envía a histórico (No.' + id + '). ¿Desea continuar?');
        $('#panelFinalizar').modal('show');
    },

    finalizarSolicitudIndividual: (idSolicitud, idDetalle) => {
        console.log('finalizarSolicitudIndividual');

        registroAprobacion.idSolicitud = idSolicitud;
        registroAprobacion.idDetalleSeleccionado = idDetalle;
        $('#msgFinalizarIndividual').html('Se dará por concluida esta solicitud individual y se envía a histórico (No.' + idDetalle + '). ¿Desea continuar?');
        $('#panelFinalizarIndividual').modal('show');
    },



    aprobarSolicitudIndividual: (idSolicitud, idDetalleSolicitud, idTipoSolicitud) => {
        console.log('aprobarSolicitudIndividual');
        $('#comboProveedorCombustible').val(0);

        //console.log(`${idSolicitud}`);
        //console.log(`${idDetalleSolicitud}`);
        //console.log(`${idTipoSolicitud}`);

        // recolectar los datos
        const numRows = $('#tableSolicitudes tbody tr').length;
        let odometro = 0;
        let litros = 0;
        for (let index = 0; index < numRows; index++) {

            let idDetalle = $(`#txtLitros${index}`).attr('data-iddetalle');

            if (Number(idDetalle) === Number(idDetalleSolicitud)) {
                odometro = $(`#txtOrometro${index}`).val() != null ? $(`#txtOrometro${index}`).val() : '0';
                litros = $(`#txtLitros${index}`).val() != null ? $(`#txtLitros${index}`).val() : '0';
            }

        }
        //console.table('litrosSurtidos');
        //console.table(litros);


        if (odometro === 0 || odometro === '' || Number(odometro) < 1) {

            $('#spnMensajes').html(mensajesAlertas.errorSolicitudesCombustibleOrometroAprobacion);
            $('#panelMensajes').modal('show');
            return;
        }

        if (litros === 0 || litros === '' || Number(litros) < 1) {

            $('#spnMensajes').html(mensajesAlertas.errorSolicitudesCombustibleLitrosAprobacion);
            $('#panelMensajes').modal('show');
            return;
        }

        registroAprobacion.idSolicitud = idSolicitud;
        registroAprobacion.idDetalleSolicitud = idDetalleSolicitud;
        registroAprobacion.idTipoSolicitud = idTipoSolicitud;

        registroAprobacion.data = {
            odometro: odometro,
            litros: litros
        };


        registroAprobacion.idSolicitud = idSolicitud;
        registroAprobacion.idDetalleSolicitud = idDetalleSolicitud;

        registroAprobacion.data = {
            odometro: odometro,
            litros: litros
        };


        if (idTipoSolicitud === 2) {

            $('#msgAprobarIndividual').html('Se marcará como aprobada la solicitud de combustible seleccionada (No.' + idDetalleSolicitud + '). ¿Desea continuar?');
            $('#panelAprobarIndividuales').modal('show');

        } else {

            $('#msgAprobar').html('Se marcará como aprobada la solicitud de combustible seleccionada (No.' + idDetalleSolicitud + '). ¿Desea continuar?');
            $('#panelAprobarGeneral').modal('show');
        }

    },


    editarDetalle: (id) => {


        $('.form-group').removeClass('has-error');
        $('.help-block').empty();




        $('#panelTabla').hide();
        $('#panelForm').show();
        registroAprobacion.accion = "editar";
        registroAprobacion.idSeleccionado = id;
        registroAprobacion.idEquipoSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);
        $('#tableSolicitudes tbody').empty();



        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idSolicitud = id;
        parametros.modo = 'edicion';
        parametros.status = 2;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Aprobacion.aspx/AbrirSolicitud",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('tabla de Combustibles_Solicitud');

                var valores = msg.d;
                //console.log(`datos = ${JSON.stringify(valores.solicitud)}`);


                if (valores.table === '') {
                    console.log('No se pudo generar la tabla de solicitudes');

                } else {


                    $('#panelTabla').hide();
                    $('#panelForm').show();

                    const htmlTable = `

                            <label>Solicitud: ${valores.solicitud.IdSolicitud} </label>

                            <table style="width: 100%!important;" class="table table-bordered table-hover table-sm"
                                id="tableSolicitudes">

                                <thead class="thead-light">

                                    <th scope="col">#</th>
                                    <th scope="col">Equipo</th>
                                    <th scope="col">Odómetro/horómetro</th>
                                    <th scope="col">Lts</th>
                                    <th scope="col">Tipo combustible</th>
                                    <th scope="col">Cincho anterior</th>
                                    <th scope="col">Cincho actual</th>
                                    <th scope="col">Obra</th>
                                    <th scope="col">Litros surtidos</th>
                                    <th scope="col"></th>
                                    <th scope="col"></th>


                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                    `;

                    $('#table_').empty().append(htmlTable);

                    $('#tableSolicitudes tbody').empty().append(valores.Table);

                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },



    subirImagenes: (id, fecha) => {


        registroAprobacion.idDetalleSolicitud = id;
        registroAprobacion.fecha = fecha;


        $('#panelImagenes').modal('show');

        imagenes.mostrarImagenes(id, null);

    },


    accionesBotones: () => {

        $('.file-fotografia1').on('change', function (e) {
            e.preventDefault();


            let file;
            if (file = this.files[0]) {

                imagenes.guardarImagenCombustible(registroAprobacion.idDetalleSolicitud, registroAprobacion.fecha, 1, file, 'fotografiaSolicitudCombustible1');

            }


        });


        $('.file-fotografia2').on('change', function (e) {
            e.preventDefault();


            let file;
            if (file = this.files[0]) {

                imagenes.guardarImagenCombustible(registroAprobacion.idDetalleSolicitud, registroAprobacion.fecha, 2, file, 'fotografiaSolicitudCombustible2');

            }


        });


        $('.file-fotografia3').on('change', function (e) {
            e.preventDefault();


            let file;
            if (file = this.files[0]) {

                imagenes.guardarImagenCombustible(registroAprobacion.idDetalleSolicitud, registroAprobacion.fecha, 3, file, 'fotografiaSolicitudCombustible3');

            }


        });

        //  Al pulsar el boton de Aceptar la cancelacion
        $('#btnCancelarAceptar').on('click', (e) => {
            e.preventDefault();
            $('.btn-aprobar').attr('disabled', true);
            $('.btn-cancelar').attr('disabled', true);

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idSolicitud = registroAprobacion.idSolicitud;
            parametros.idDetalleSolicitud = registroAprobacion.idDetalleSeleccionado;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Aprobacion.aspx/CancelarEntrega",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {


                    var resultado = msg.d;
                    //console.log(resultado);

                    $('.btn-aprobar').attr('disabled', false);
                    $('.btn-cancelar').attr('disabled', false);

                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoCancelarSolicitudCombustible, 'ok');


                        registroAprobacion.editarDetalle(registroAprobacion.idSolicitud);

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });


        $('#btnFinalizarAceptar').on('click', (e) => {

            e.preventDefault();

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idSolicitud = registroAprobacion.idSeleccionado;
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Aprobacion.aspx/Finalizar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {


                        $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                        $('#panelMensajes').modal('show');

                        registroAprobacion.cargarItems();

                    } else {

                        utils.toast(valores.MensajeError, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);



                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });

        $('#btnFinalizarIndividualAceptar').on('click', (e) => {

            e.preventDefault();

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idSolicitud = registroAprobacion.idSolicitud;
            parametros.idDetalleSolicitud = registroAprobacion.idDetalleSeleccionado;
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Aprobacion.aspx/FinalizarIndividual",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {


                        $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                        $('#panelMensajes').modal('show');

                        registroAprobacion.cargarItems();


                        $('#panelTabla').show();
                        $('#panelForm').hide();


                    } else {

                        utils.toast(valores.MensajeError, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);



                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });

        $('.btnAprobarAceptar').on('click', (e) => {
            e.preventDefault();

            console.log('aprobar item');

            $('.btn-aprobar').attr('disabled', true);

            let idProveedorCombustible = -1;
            if (parseInt(registroAprobacion.idTipoSolicitud) === 2) {
                idProveedorCombustible = $('#comboProveedorCombustible').val();
                if (idProveedorCombustible == null || parseInt(idProveedorCombustible) === 0) {

                    utils.toast(mensajesAlertas.errorSeleccionarProveedor, 'error');

                    return;
                }

            }

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idSolicitud = registroAprobacion.idSolicitud;
            parametros.idDetalleSolicitud = registroAprobacion.idDetalleSolicitud;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.odometro = registroAprobacion.data.odometro;
            parametros.litros = registroAprobacion.data.litros;
            parametros.horometroEntrega = registroAprobacion.data.horometroEntrega;
            parametros.idProveedorCombustible = idProveedorCombustible;
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Aprobacion.aspx/Aprobar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    console.log(resultado);
                    $('.btn-aprobar').attr('disabled', false);

                    $('#panelAprobarIndividuales').modal('hide');

                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoAprobar, 'ok');
                        registroAprobacion.cargarItems();

                        registroAprobacion.editarDetalle(registroAprobacion.idSolicitud);

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    utils.toast(mensajesAlertas.errorGuardar, 'error');
                    $('.btn-entregar').attr('disabled', false);
                }

            });

        });


        //$('#btnAceptarPanelMensajes').on('click', (e) => {

        //    e.preventDefault();
        //    $('#panelMensajes').modal('hide');

        //    $('#panelTabla').show();
        //    $('#panelForm').hide();

        //});


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();
            registroAprobacion.cargarItems();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


    }


}

window.addEventListener('load', () => {

    registroAprobacion.init();

    registroAprobacion.accionesBotones();

});


