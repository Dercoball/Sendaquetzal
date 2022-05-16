let date = new Date();
let descargas = "SolicitudCombustible_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '69';


let solicitud = {
    idSolicitud: -1,
    idUsuario: -1,
    solicitud: {},
    arrayData: [],
    accion: '',
    idSeleccionado: -1,
    idDetalleSolicitud: -1,
    fecha: '',
    numero: -1
}


class SolicitudCombustible {

    constructor(Id_) {
        this.IdRegistroFalla = Id_;
        this.IdEquipo = null;
        this.FechaCreacion = null;

    }

}

const registroSolicitud = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        registroSolicitud.idSeleccionado = -1;
        registroSolicitud.accion = '';
        registroSolicitud.fecha = '';

        registroSolicitud.cargarItems();


    },




    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");

        let url = '../pages/Combustibles_Solicitud.aspx/GetListaItems';

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
                        { data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [0, 1, 2, 3],
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




    eliminar: (id) => {

        registroSolicitud.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },





    enviar: (id) => {
        registroSolicitud.idSeleccionado = id;
        $('#msgEnviar').html('<p>Se enviará a la siguiente etapa el registro seleccionado(No.' + id + '). ¿Desea continuar?</p>');
        $('#panelEnviar').modal('show');

    },

    cargarComboEquipo: (nombreControlReemplazo, nombreControl, index) => {

        let param_ = {};
        param_.path = window.location.hostname;
        param_ = JSON.stringify(param_);

        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_solicitud.aspx/GetListaEquipos",
            data: param_,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                var datos = msg.d;
                let html = `<select id="${nombreControl}" class="form-control" style="width: 200px;">`;
                html += `<option value="0">Seleccione...</option>`;

                for (let i = 0; i < datos.length; i++) {
                    let item = datos[i];
                    html += `<option data-tiposolicitud="${item.NombreTipoSolicitud}" value="${item.IdEquipo}">${item.NumeroEconomico}</option>`;
                }
                html += `</select>`;



                $(`#${nombreControlReemplazo}`).empty().append(html);

                $(`#${nombreControl}`).on('change', (e) => {
                    let idEquipo = e.currentTarget['value'];
                    //console.log(`${idEquipo}`);

                    //console.log('Loading cincho actual');

                    let paramCincho = {};
                    paramCincho.idEquipo = idEquipo;
                    paramCincho.path = window.location.hostname;
                    paramCincho = JSON.stringify(paramCincho);

                    $.ajax({
                        type: "POST",
                        url: "../pages/Combustibles_solicitud.aspx/GetCintilloActual",
                        data: paramCincho,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: true,
                        success: function (msg) {
                            const valorCincho = msg.d;
                            $(`#txtCintilloAnterior${index}`).val(valorCincho);


                        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                            console.log(textStatus + ": " + XMLHttpRequest.responseText);
                        }
                    });

                });

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });


    },

    cargarComboCentroCosto: (nombreControlReemplazo, nombreControl) => {

        let param_ = {};
        param_.path = window.location.hostname;
        param_ = JSON.stringify(param_);

        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_solicitud.aspx/GetListaCentrosCosto",
            data: param_,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                //console.log(msg.d);

                var datos = msg.d;
                let html = `<select id="${nombreControl}" class="form-control" style="width: 200px;">`;
                html += `<option value="0">Seleccione...</option>`;

                for (let i = 0; i < datos.length; i++) {
                    let item = datos[i];
                    html += `<option value="${item.IdCentroCosto}">${item.Nombre}</option>`;
                }
                html += `</select>`;


                $(`#${nombreControlReemplazo}`).empty().append(html);

                //  Si noes el primer combo de la lista
                if (nombreControl !== 'comboCentroCosto0') {
                    let valorPrimerCombo = $(`#comboCentroCosto0`).val();

                    $(`#${nombreControl}`).val(valorPrimerCombo);

                }

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });


    },

    cargarComboTipoCombustible: (nombreControlReemplazo, nombreControl) => {

        let param_ = {};
        param_.path = window.location.hostname;
        param_ = JSON.stringify(param_);

        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_solicitud.aspx/GetTiposCombustible",
            data: param_,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                //console.log(msg.d);

                var datos = msg.d;
                let html = `<select id="${nombreControl}" class="form-control" style="width: 200px;">`;
                html += `<option value="0">Seleccione...</option>`;

                for (let i = 0; i < datos.length; i++) {
                    let item = datos[i];
                    html += `<option value="${item.IdTipoCombustible}">${item.Nombre}</option>`;
                }
                html += `</select>`;

                $(`#${nombreControlReemplazo}`).empty().append(html);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });


    },

    editarDetalle: (id) => {


        $('.form-group').removeClass('has-error');
        $('.help-block').empty();




        $('#panelTabla').hide();
        $('#panelForm').show();
        registroSolicitud.accion = "editar";
        registroSolicitud.idSeleccionado = id;
        registroSolicitud.idEquipoSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);
        $('#tableSolicitudes tbody').empty();



        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idSolicitud = id;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Solicitud.aspx/AbrirSolicitud",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('tabla de Combustibles_Solicitud');

                var valores = msg.d;
                //console.log(`datos = ${JSON.stringify(valores)}`);

                $('#btnGuardar').hide();


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

                                    <th scope="col">Equipo</th>
                                    <th scope="col">Odómetro/horómetro</th>
                                    <th scope="col">Litros</th>
                                    <th scope="col">Tipo combustible</th>
                                    <th scope="col">Cincho anterior</th>
                                    <th scope="col">Cincho actual</th>
                                    <th scope="col">Obra</th>
                                    
                                    <th scope="col">Status</th>
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


    nuevasSolicitudes: () => {


        $('.form-group').removeClass('has-error');
        $('.help-block').empty();


        $('#btnGuardar').show();


        $('#panelTabla').hide();
        $('#panelForm').show();
        registroSolicitud.accion = "nuevo";
        registroSolicitud.idEquipoSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        $('#tableSolicitudes tbody').empty();


        $('#panelTabla').hide();
        $('#panelForm').show();

        const htmlTable = `
                <table style="width: 100%!important;" class="table table-bordered table-hover table-sm"
                    id="tableSolicitudes">

                    <thead class="thead-light">

                        <th scope="col">Equipo</th>
                        <th scope="col">Odómetro/horómetro</th>
                        <th scope="col">Lts</th>
                        <th scope="col">Tipo combustible</th>
                        <th scope="col">Cincho anterior</th>
                        <th scope="col">Cincho actual</th>
                        <th scope="col">Obra</th>
                        <th scope="col"><buttton id="btnAgregar" class="btn btn-primary">Agregar</button></th>


                    </thead>
                    <tbody>
                    </tbody>
                </table>
        `;


        $('#table_').empty().append(htmlTable);


        $('#btnAgregar').on('click', (e) => {
            e.preventDefault();
            //console.log('btnAgregar');


            solicitud.arrayData.push(solicitud.arrayData.length);
            //console.log(solicitud.arrayData);

            const nextIndex = solicitud.arrayData.length - 1;

            row = `<tr scope="row" id="tr${nextIndex}">
                <td><div class='comboEquipo${nextIndex}' id='divEquipo${nextIndex}' class='sm100 form-control'>Cargando...</div></td>
                <td><input type='number' value='' class='sm100 form-control-sm orometro'  id='txtOrometro${nextIndex}'></td>
                <td><input type='number' value='' class='sm100 form-control-sm litros'  id='txtLitros${nextIndex}'></td>
                <td><div class='comboTipoCombustible${nextIndex}' id='divCombustible${nextIndex}' class='sm100 form-control'>Cargando...</div></td>
                <td><input type='number' disabled value='' class='sm100 form-control-sm cintillo-anterior'  id='txtCintilloAnterior${nextIndex}'></td>
                <td><input type='number' disabled value='' class='sm100 form-control-sm cintillo-actual'  id='txtCintilloActual${nextIndex}'></td>

                <td><div class='comboCentroCosto${nextIndex}' id='divCentroCosto${nextIndex}' class='form-control'>Cargando...</div></td>

                <td><buttton data-index='${nextIndex}' class="btn btn-warning quitar-renglon">Quitar</button></td>
                </tr>`;

            $('#tableSolicitudes tbody').append(row);

            registroSolicitud.cargarComboEquipo(`divEquipo${nextIndex}`, `comboEquipo${nextIndex}`, nextIndex);
            registroSolicitud.cargarComboCentroCosto(`divCentroCosto${nextIndex}`, `comboCentroCosto${nextIndex}`);
            registroSolicitud.cargarComboTipoCombustible(`divCombustible${nextIndex}`, `comboTipoCombustible${nextIndex}`);

            $('.quitar-renglon').on('click', (e) => {
                e.preventDefault();
                //console.log('quitar-renglon');
                //debugger;

                let idTr = e.currentTarget.dataset["index"];

                $(`#tr${idTr}`).remove();

                let nuevoArray = [];
                for (let i = 0; i < solicitud.arrayData.length; i++) {
                    if (Number(solicitud.arrayData[i]) !== Number(idTr)) {
                        nuevoArray.push(solicitud.arrayData[i]);
                    }
                }

                solicitud.arrayData = nuevoArray;



            });


        });



        //  Agregar el primer row
        $('#btnAgregar').trigger('click');




    },

   

    subirImagenes: (id, fecha) => {


        registroSolicitud.idDetalleSolicitud = id;
        registroSolicitud.fecha = fecha;


        $('#panelImagenes').modal('show');

        imagenes.mostrarImagenes(id, null);

    },

    



    accionesBotones: () => {

        $('.file-fotografia1').on('change', function (e) {
            e.preventDefault();


            let file;
            if (file = this.files[0]) {

                imagenes.guardarImagenCombustible(registroSolicitud.idDetalleSolicitud, registroSolicitud.fecha, 1, file, 'fotografiaSolicitudCombustible1');

            }


        });


        $('.file-fotografia2').on('change', function (e) {
            e.preventDefault();


            let file;
            if (file = this.files[0]) {

                imagenes.guardarImagenCombustible(registroSolicitud.idDetalleSolicitud, registroSolicitud.fecha, 2, file, 'fotografiaSolicitudCombustible2');

            }


        });


        $('.file-fotografia3').on('change', function (e) {
            e.preventDefault();


            let file;
            if (file = this.files[0]) {

                imagenes.guardarImagenCombustible(registroSolicitud.idDetalleSolicitud, registroSolicitud.fecha, 3, file, 'fotografiaSolicitudCombustible3');

            }


        });


        $('#btnAbrirSeleccionarEquipo').on('click', (e) => {
            e.preventDefault();

            registroSolicitud.cargarItemsEquipos();
            $('#panelSeleccionarEquipo').modal('show');


        });



        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            solicitud.arrayData = [];
            registroSolicitud.nuevasSolicitudes();



        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();


            let valoresTabla = [];
            // recolectar los datos
            const numRows = $('#tableSolicitudes tbody tr').length;

            for (let index = 0; index < numRows; index++) {

                let newRow = {
                    Cantidad: $(`#txtLitros${index}`).val(),
                    IdEquipo: $(`#comboEquipo${index}`).val(),
                    IdTipoCombustible: $(`#comboTipoCombustible${index}`).val(),
                    Orometro: $(`#txtOrometro${index}`).val(),
                    CintilloAnterior: $(`#txtCintilloAnterior${index}`).val() != null ? $(`#txtCintilloAnterior${index}`).val() : '0',
                    CintilloActual: $(`#txtCintilloActual${index}`).val() != null ? $(`#txtCintilloActual${index}`).val() : '0',
                    IdObra: $(`#comboCentroCosto${index}`).val()
                };

                valoresTabla.push(newRow);

            }


            let error = false;
            for (let i = 0; i < valoresTabla.length; i++) {
                let item = valoresTabla[i];

                if (item.Cantidad === '' || parseInt(item.Cantidad) < 1) {
                    error = true;
                    break;
                }

                if (item.IdEquipo === '' || parseInt(item.IdEquipo) === 0) {
                    error = true;

                    break;
                }


                if (item.Orometro === '' || parseInt(item.Orometro) < 1) {
                    error = true;

                    break;
                }

                if (item.IdObra === '' || parseInt(item.IdObra) < 1) {
                    error = true;

                    break;
                }

                if (item.IdTipoCombustible === '' || parseInt(item.IdTipoCombustible) < 1) {
                    error = true;

                    break;
                }


            }
            if (error) {

                $('#spnMensajes').html(mensajesAlertas.errorSolicitudesCombustible);
                $('#panelMensajes').modal('show');
                return;
            }


            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.data = valoresTabla;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idEmpleado = sessionStorage.getItem("idempleado");
            parametros.accion = 'guardar';

            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Solicitud.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        registroSolicitud.idSeleccionado = valores.IdItem;

                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                        $('#panelMensajes').modal('show');
                        registroSolicitud.cargarItems();

                    } else {

                        $('#spnMensajes').html(valores.MensajeError);
                        $('#panelMensajes').modal('show');


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
                url: "../pages/Combustibles_Solicitud.aspx/EliminarRequisicion",
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
                url: "../pages/Combustibles_Solicitud.aspx/EnviarRequisicion",
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


