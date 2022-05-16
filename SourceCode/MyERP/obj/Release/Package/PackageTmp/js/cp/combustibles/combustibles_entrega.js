'use strict';
let date = new Date();
let descargas = "EntregaCombustible" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '72';



const entregasCombustible = {


    init: () => {

        $('#alert-operacion-ok').hide();
        $('#alert-operacion-fail').hide();

        $('#panelTabla').show();
        $('#panelForm').hide();
        $('#panelTablaCerrados').hide();

        entregasCombustible.idEquipoSeleccionado = -1;
        entregasCombustible.idDetalleSeleccionado = -1;
        entregasCombustible.idSolicitud = -1;
        entregasCombustible.idObra = -1;
        entregasCombustible.data = {};

        entregasCombustible.accion = '';

        entregasCombustible.cargarItems();

    },




    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/Combustibles_Entrega.aspx/GetListaCentrosCosto';

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
                        { data: 'Clave' },
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



    abrir: (id) => {

        console.log('abrir');
        entregasCombustible.idObra = id;


        let parametros = {};
        parametros.path = window.location.hostname;
        parametros.idObra = id;
        parametros.modo = 'edicion';
        parametros.status = 2;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Entrega.aspx/AbrirSolicitudesIndividuales",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var valores = msg.d;
                //console.log(`datos = ${JSON.stringify(valores.solicitud)}`);

                //const idStatussolicitud = valores.solicitud.IdStatus;
                //console.log(`datos = ${JSON.stringify(solicitud)}`);


                const htmlTable = `

                            <label>CC: ${valores.solicitud.NombreCentroCosto} </label>

                            <table style="width: 100%!important;" class="table table-bordered table-hover table-sm"
                                id="tableSolicitudes">

                                <thead class="thead-light">

                                    <th scope="col">#</th>
                                    <th scope="col">Equipo</th>
                                    <th scope="col">Odóm./horóm.</th>
                                    <th scope="col">Lts</th>
                                    <th scope="col">Tipo combustible</th>
                                    <th scope="col">Cincho anterior</th>
                                    <th scope="col">Cincho actual</th>
                                    <th scope="col">Litros surtidos</th>
                                    <th scope="col">Odóm./horóm. en entrega</th>
                                  
                                    <th scope="col"></th>


                                </thead>
                                <tbody>
                                </tbody>
                            </table>
                    `;

                $('#table_').empty().append(htmlTable);

                $('#tableSolicitudes tbody').empty().append(valores.Table);


                $('#panelTabla').hide();
                $('#panelForm').show();


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });



    },

    cancelar: (idSolicitud, idDetalleSolicitud) => {

        entregasCombustible.idSolicitud = idSolicitud;
        entregasCombustible.idDetalleSeleccionado = idDetalleSolicitud;

        $('#msgCancelar').html('Se marcará como cancelada la solicitud de combustible seleccionada (No.' + idDetalleSolicitud + '). ¿Desea continuar?');
        $('#panelCancelar').modal('show');

    },


    entregar: (idSolicitud, idDetalleSolicitud) => {
        console.log('entregar');

        // recolectar los datos
        const numRows = $('#tableSolicitudes tbody tr').length;
        let cintilloActual = 0;
        let litrosSurtidos = 0;
        let horometroEntrega = '';
        for (let index = 0; index < numRows; index++) {

            let idDetalle = $(`#txtCintilloActual${index}`).attr('data-iddetalle');

            if (Number(idDetalle) === Number(idDetalleSolicitud)) {
                cintilloActual = $(`#txtCintilloActual${index}`).val() != null ? $(`#txtCintilloActual${index}`).val() : '0';
                litrosSurtidos = $(`#txtLitrosSurtidos${index}`).val() != null ? $(`#txtLitrosSurtidos${index}`).val() : '0';
                horometroEntrega = $(`#txtOrometroEntrega${index}`).val() != null ? $(`#txtOrometroEntrega${index}`).val() : '0';
            }

        }
        console.table('litrosSurtidos');
        console.table(litrosSurtidos);


        if (cintilloActual === 0 || cintilloActual === '' || Number(cintilloActual) < 1) {

            $('#spnMensajes').html(mensajesAlertas.errorSolicitudesCombustibleCinchoActual);
            $('#panelMensajes').modal('show');
            return;
        }

        if (litrosSurtidos === 0 || litrosSurtidos === '' || Number(litrosSurtidos) < 1) {

            $('#spnMensajes').html(mensajesAlertas.errorSolicitudesLitrosSurtidos);
            $('#panelMensajes').modal('show');
            return;
        }

        if (horometroEntrega === 0 || horometroEntrega === '' || Number(horometroEntrega) < 1) {

            $('#spnMensajes').html(mensajesAlertas.errorSolicitudesHorometroEntrega);
            $('#panelMensajes').modal('show');
            return;
        }



        entregasCombustible.idSolicitud = idSolicitud;
        entregasCombustible.idDetalleSeleccionado = idDetalleSolicitud;

        entregasCombustible.data = {
            cintilloActual: cintilloActual,
            cantidadSurtida: litrosSurtidos,
            horometroEntrega: horometroEntrega
        };

        $('#msgEntregar').html('<p>Se marcará como entregado la solicitud de combustible seleccionada (No.' + idDetalleSolicitud + '). ¿Desea continuar?</p>');
        $('#panelEntregar').modal('show');
    },




    accionesBotones: () => {

        //  Al pulsar el boton de Aceptar la cancelacion
        $('#btnCancelarAceptar').on('click', (e) => {
            e.preventDefault();
            $('.btn-entregar').attr('disabled', true);

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idSolicitud = entregasCombustible.idSolicitud;
            parametros.idDetalleSolicitud = entregasCombustible.idDetalleSeleccionado;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Entrega.aspx/CancelarEntrega",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {


                    var resultado = msg.d;
                    //console.log(resultado);

                    $('.btn-entregar').attr('disabled', false);

                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoCancelarSolicitudCombustible, 'ok');


                        entregasCombustible.abrir(entregasCombustible.idObra);

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        });

        $('#btnEntregarAceptar').on('click', (e) => {

            console.log('entregar item');

            $('.btn-entregar').attr('disabled', true);

            let parametros = {};
            parametros.path = window.location.hostname;
            parametros.idSolicitud = entregasCombustible.idSolicitud;
            parametros.idDetalleSolicitud = entregasCombustible.idDetalleSeleccionado;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.cinchoActual = entregasCombustible.data.cintilloActual;
            parametros.cantidadSurtida = entregasCombustible.data.cantidadSurtida;
            parametros.horometroEntrega = entregasCombustible.data.horometroEntrega;
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/Combustibles_Entrega.aspx/Entregar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    console.log(resultado);
                    $('.btn-entregar').attr('disabled', false);

                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEntregar, 'ok');


                        entregasCombustible.abrir(entregasCombustible.idObra);

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

        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            entregasCombustible.cargarItems();

            $('#panelTabla').show();
            $('#panelForm').hide();


        });


    }


}

window.addEventListener('load', () => {

    entregasCombustible.init();

    entregasCombustible.accionesBotones();

});


