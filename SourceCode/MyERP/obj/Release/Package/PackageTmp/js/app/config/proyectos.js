var accion = "";
var idSeleccionado = "-1";
var idEquipoSeleccionado = "-1";
var idCitaSeleccionado = "-1";
var table;
var tablaCerrados;
var tablaClientes;
var tablaCitas;
var date = new Date();
var descargas = "Proyectos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
var pagina = '15';

$(document).ready(function () {


    $('#alert-operacion-ok').hide();
    $('#alert-operacion-fail').hide();

    $('#panelTabla').show();
    $('#panelForm').hide();
    $('#panelTablaCerrados').hide();


    cargarItems();
    cargarComboMarcas();
    cargarComboTiposReparacion();
    cargarComboEstatus();


    function cargarComboMarcas() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/GetListaMarcas",
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
    }



    function cargarItems() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idProveedor = sessionStorage.getItem("idproveedor");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaItems",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {



                table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'IdProyecto' },
                        { data: 'NumeroEconomico' },
                        { data: 'NombreEquipo' },
                        { data: 'NombreStatusProyecto' },
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



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }


    function cargarItemsEquipos() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaEquipos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                tablaClientes = $('#tablaEquipos').DataTable({
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

                function cargarComboModelos(idMarca, idModelo) {
                    var parametros = new Object();
                    parametros.path = window.location.hostname;
                    parametros.idMarca = idMarca;
                    parametros = JSON.stringify(parametros);
                    $.ajax({
                        type: "POST",
                        url: "../pages/PanelEquipos.aspx/GetListaModelos",
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
                }



                $('#tablaEquipos').on('draw.dt', function () {



                    $('.boton-seleccionar-equipo').on('click', function (e) {
                        e.preventDefault();

                        idEquipoSeleccionado = $(this).attr('data-idequipo');
                        $('#txtNombreEquipo').val(idEquipoSeleccionado);


                        var parametros = new Object();
                        parametros.path = window.location.hostname;
                        parametros.id = idEquipoSeleccionado;
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
                                idEquipoSeleccionado = datos.IdEquipo;

                                $('#txtNombreEquipo').val(datos.Nombre);
                                $('#txtNumeroEconomico').val(datos.NumeroEconomico);
                                $('#txtNumeroSerie').val(datos.NumeroSerie);
                                $('#txtAnio').val(datos.Anio);
                                $('#txtNombreEquipo').val(datos.Nombre);
                                $('#comboMarca').val(datos.IdMarca);


                                cargarComboModelos(datos.IdMarca, datos.IdModelo);


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
    }



    function cargarComboMarcas() {
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
    }

    function cargarComboTiposReparacion() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaTiposReparacion",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdTipoReparacion + '">' + item.Nombre + '</option>';

                }

                $('#comboTipoReparacion').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function cargarComboEstatus() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaEstatus",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdStatusProyecto + '">' + item.Nombre + '</option>';

                }

                $('#comboEstatus').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }




    $('#btnAbrirSeleccionarEquipo').click(function (e) {
        e.preventDefault();

        cargarItemsEquipos();
        $('#panelSeleccionarEquipo').modal('show');


    });

    $('#btnAbrirSeleccionarcliente').click(function (e) {
        e.preventDefault();

        cargarItemsClientes();
        $('#panelSeleccionarCliente').modal('show');


    });



    $('#comboMarca').on('change', function (e) {

        e.preventDefault();
        cargarComboModelos($('#comboMarca').val(), 0);


    });


    function cargarComboModelos(idMarca, idModelo) {
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
    }




    $('#btnAceptarPanelMensajes').on('click', (e) => {

        e.preventDefault();
        $('#panelMensajes').modal('hide');

        $('#panelTabla').show();
        $('#panelForm').hide();

    });

    $('#btnGuardar').click(function (e) {

        e.preventDefault();




        var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


        if (hasErrors) {
            return;
        }

        var item = new Object();

        item.IdProyecto = idSeleccionado;
        item.FechaCreacion = $('#txtFecha').prop('fecha');
        item.Diagnostico = $('#txtDiagnostico').val();
        item.DescripcionServicio1 = $('#txtDescripcionServicio1').val();
        item.IdRequisicion = idSeleccionado;


        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.proyecto = item;
        parametros.accion = accion;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/Guardar",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var valores = msg.d;


                if (valores.CodigoError == 0) {

                    $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                    $('#panelMensajes').modal('show');
                    cargarItems();

                }

                else {

                    utils.toast(mensajesAlertas.errorGuardar, 'error');

                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


                utils.toast(mensajesAlertas.errorGuardar, 'error');


            }

        });




    });

    $('#btnCancelar').click(function (e) {
        e.preventDefault();

        $('#panelTabla').show();
        $('#panelForm').hide();

    });


    $('#btnEliminarAceptar').click(function () {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = idSeleccionado;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/EliminarProyecto",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado.MensajeError === null) {

                    $('#spnMensajes').html('Registro eliminado correctamente...');
                    $('#panelMensajes').modal('show');

                    cargarItems();

                } else {

                    $('#spnMensajes').html(resultado.MensajeError);
                    $('#panelMensajes').modal('show');


                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    });



});


//function descargarFormato(id) {
//    window.open("../pages/Download.ashx?path=" + window.location.hostname + "&id_proyecto=" + id + "&tipo=1");


//}


const eliminar = (id) => {

    idSeleccionado = id;

    $('#panelEliminar').modal('show');

}


const cargarComboModelos_ = (idMarca, idModelo) => {
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
}


const aprobar = (id) => {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();

    $('#frm')[0].reset();
    $('#txtDiagnostico').val('');
    $('#txtDescripcionServicion1').val('');

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros.idProveedor = sessionStorage.getItem("idproveedor");
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Proyectos.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var item = msg.d;
            idSeleccionado = item.IdRequisicion;
            idEquipoSeleccionado = item.IdEquipo;
            idProveedorSeleccionado = item.IdProveedor;

            console.log('.');
            $('#txtNumeroRequisicion').val(item.IdRequisicion);
            $('#txtNombreEquipo').val(item.NombreEquipo);
            $('#txtNombreProveedor').val(item.NombreProveedor);

            $('#txtFechaCreacion').val(item.FechaCreacionFormateada);
            $('#txtDescripcion').val(item.Descripcion);
            $('#txtNumeroEconomico').val(item.NumeroEconomico);
            $('#txtNumeroSerie').val(item.NumeroSerie);
            $('#txtAnio').val(item.Anio);
            $('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
            $('#comboMarca').val(item.IdMarca);
            cargarComboModelos_($('#comboMarca').val(), item.IdModelo);

            $('.deshabilitable').prop('disabled', false);


            accion = "nuevo";
            $('#spnTituloForm').text('Aprobar requisición');

            let parametros = {
                path: window.location.hostname,
                id_requisicion: item.IdRequisicion,
                idUsuario: sessionStorage.getItem("idusuario"),
                id_tipo: 1
            };


            $.ajax({
                type: "POST",
                url: "../pages/PanelRequisiciones.aspx/GetDocumentos",
                data: JSON.stringify(parametros),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    let documentos = msg.d;

                    console.log(msg.d);

                    let htmlDoc = '';

                    documentos.forEach(function (documento) {

                        htmlDoc += '<div>';
                        htmlDoc += '<a href = "../pages/Download.ashx?path=' + window.location.hostname + '&id_documento=' + documento.IdDocumento + '" target="blank" >';
                        htmlDoc += documento.Descripcion;
                        htmlDoc += '</a>';
                        htmlDoc += '</div>';

                        $('#divDocumentos').append(htmlDoc);

                    });

                    $('#divDocumentos').empty().append(htmlDoc);


                    $('#panelTabla').hide();
                    $('#panelForm').show();


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


            obtenerFechaHoraServidor('txtFecha');

            $('#panelTabla').hide();
            $('#panelForm').show();



        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}


const abrir = (id) => {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#frm')[0].reset();
    $('#txtDiagnostico').val('');
    $('#txtDescripcionServicion1').val('');

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros.idProveedor = sessionStorage.getItem("idproveedor");
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Proyectos.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var item = msg.d;
            idSeleccionado = item.IdRequisicion;
            idEquipoSeleccionado = item.IdEquipo;
            idProveedorSeleccionado = item.IdProveedor;

            console.log('.');
            $('#txtNumeroRequisicion').val(item.IdRequisicion);
            $('#txtNombreEquipo').val(item.NombreEquipo);
            $('#txtNombreProveedor').val(item.NombreProveedor);

            $('#txtFechaCreacion').val(item.FechaCreacionFormateada);

            $('#txtNumeroEconomico').val(item.NumeroEconomico);
            $('#txtNumeroSerie').val(item.NumeroSerie);
            $('#txtAnio').val(item.Anio);
            $('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
            $('#txtDiagnostico').val(item.Latitud);
            $('#txtDescripcionServicion1').val(item.Latitud);
            $('#comboMarca').val(item.IdMarca);
            cargarComboModelos_($('#comboMarca').val(), item.IdModelo);

            $('.deshabilitable').prop('disabled', true);


            accion = "nuevo";
            $('#spnTituloForm').text('Aprobar requisición');

            let parametros = {
                path: window.location.hostname,
                id_requisicion: item.IdRequisicion,
                idUsuario: sessionStorage.getItem("idusuario"),
                id_tipo: 1
            };


            $.ajax({
                type: "POST",
                url: "../pages/PanelRequisiciones.aspx/GetDocumentos",
                data: JSON.stringify(parametros),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    let documentos = msg.d;

                    console.log(msg.d);

                    let htmlDoc = '';

                    documentos.forEach(function (documento) {

                        htmlDoc += '<div>';
                        htmlDoc += '<a href = "../pages/Download.ashx?path=' + window.location.hostname + '&id_documento=' + documento.IdDocumento + '" target="blank" >';
                        htmlDoc += documento.Descripcion;
                        htmlDoc += '</a>';
                        htmlDoc += '</div>';

                        $('#divDocumentos').append(htmlDoc);

                    });

                    $('#divDocumentos').empty().append(htmlDoc);


                    $('#panelTabla').hide();
                    $('#panelForm').show();


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


            obtenerFechaHoraServidor('txtFecha');

            $('#panelTabla').hide();
            $('#panelForm').show();



        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}


const nuevo = () => {


    $('#frm')[0].reset();
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#spnTituloForm').text('Nuevo');




    $('#panelTabla').hide();
    $('#panelForm').show();
    accion = "nuevo";
    id = "-1";
    idSeleccionado = "-1";
    idClienteSeleccionado = "-1";


    obtenerFechaHoraServidor('txtFechaCreacion');


}


