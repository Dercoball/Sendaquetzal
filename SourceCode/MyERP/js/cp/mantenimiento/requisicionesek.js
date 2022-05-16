'use strict';
let date = new Date();
let descargas = "RequisicionesEK_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '84';


const requisicionesEK = {


    init: () => {
        console.log('init');

        $('#alert-operacion-ok').hide();
        $('#alert-operacion-fail').hide();

        $('#panelTabla').show();
        $('#panelForm').hide();
        $('#panelTablaCerrados').hide();
        $('#panelTablaRefacciones').show();

        requisicionesEK.idProveedorSeleccionado = -1;
        requisicionesEK.idEquipoSeleccionado = -1;
        requisicionesEK.idSeleccionado = -1;
        requisicionesEK.accion = '';
        requisicionesEK.idUsuarioDiagnostico = -1;
        requisicionesEK.idRefaccionSeleccionada = -1;
        requisicionesEK.idRequisicionSeleccionada = -1;

        requisicionesEK.cargarItems(0);


  

    },



    cargarItems: (modo) => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idTipoUsuario = sessionStorage.getItem("idtipousuario");
        parametros.mostrarSoloFinalizados = 0;

        let url = '../pages/MantenimientoRequisicionesEK.aspx/GetListaItems';

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
                        { data: 'NombreEquipo' },
                        //{ data: 'Descripcion' },
                        { data: 'FechaCreacionFormateada' },
                        //{ data: 'TiempoTranscurrido' },
                        //{ data: 'FechaCierreFormateada' },
                        //{ data: 'NombrePrioridad' },
                        //{ data: 'NombreUsuarioDiagnostico' },
                        //{ data: 'NombreStatus' },
                        { data: 'Accion' },

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

                //  recargar grids
                if (modo === 1) {

                    $('#panelTabla').show();
                    $('#panelForm').hide();
                }
            });



    },


    capturar: (id) => {
        requisicionesEK.idRefaccionSeleccionada = id;

        $('#txtValorCaptura').val('');
        $('#msgCapturar').html('<p></p>');
        $('#panelCapturar').modal('show');
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
            url: "../pages/MantenimientoRequisicionesEK.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                var item = msg.d;

                requisicionesEK.idSeleccionado = item.IdRequisicion;
                requisicionesEK.idEquipoSeleccionado = item.IdEquipo;
                requisicionesEK.idProveedorSeleccionado = item.IdProveedor;

                $('#txtNumeroRequisicion').val(item.IdRequisicion);
                $('#txtNombreEquipo').val(item.NombreEquipo);


                $('#txtFechaCreacion').val(item.FechaCreacionFormateadaMx);
                $('#txtFechaCreacion').prop('fecha', item.FechaCreacion);

              
                $('#txtNumeroEconomico').val(item.NumeroEconomico);




                requisicionesEK.accion = "editar";
                $('#spnTituloForm').text('Datos orden de trabajo');

                //  refacciones
                requisicionesEK.idRequisicionSeleccionada = item.IdRequisicion;
                requisicionesEK.cargarItemsRefacciones(item.IdRequisicion);
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



    cargarItemsRefacciones: (idRequisicion) => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idRequisicion = idRequisicion;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoRequisicionesEK.aspx/GetItemsRefacciones",
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

    accionesBotones: () => {



        $('#btnCapturarAceptar_Refaccion').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frmCapturar"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }


            var parametros = {};
            parametros.path = window.location.hostname;
            parametros.IdRefaccion = requisicionesEK.idRefaccionSeleccionada;
            parametros.ValorCaptura = $('#txtValorCaptura').val();
            parametros = JSON.stringify(parametros);

            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoRequisicionesEK.aspx/GuardarValorCaptura",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;

                    if (valores.CodigoError == 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');
                        $('#panelCapturar').modal('hide');

                        requisicionesEK.cargarItemsRefacciones(requisicionesEK.idSeleccionado);

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



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();


            requisicionesEK.cargarItems(1);

            //$('#panelTabla').show();
            //$('#panelForm').hide();

        });


    }


}

window.addEventListener('load', () => {

    requisicionesEK.init();

    requisicionesEK.accionesBotones();

});


