
let date = new Date();
let descargas = "Requisiciones_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '57';

let idProveedorSeleccionado = -1;
let idEquipoSeleccionado = -1;


$(document).ready(function () {


    $('#alert-operacion-ok').hide();
    $('#alert-operacion-fail').hide();

    $('#panelTabla').show();
    $('#panelForm').hide();
    $('#panelTablaCerrados').hide();


    cargarItems();
    cargarComboMarcas();

    cargarComboEstatus();


    function cargarComboMarcas() {
        var parametros = new Object();
        parametros.path = "connbd";
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelRequisiciones.aspx/GetListaMarcas",
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
        parametros.path = "connbd";
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelRequisiciones.aspx/GetListaItems",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                console.log('');

                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'IdRequisicion' },
                        { data: 'NumeroEconomico' },
                        { data: 'NombreEquipo' },
                        { data: 'NombreProveedor' },
                        { data: 'FechaCreacionFormateada' },
                        { data: 'NombrePrioridad' },
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
        parametros.path = "connbd";
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelRequisiciones.aspx/GetListaEquiposSinRequisicion",
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
                    parametros.path = "connbd";
                    parametros.idMarca = idMarca;
                    parametros = JSON.stringify(parametros);
                    $.ajax({
                        type: "POST",
                        url: "../pages/PanelRequisiciones.aspx/GetListaModelos",
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
                        parametros.path = "connbd";
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


    function cargarItemsProveedores() {
        var parametros = new Object();
        parametros.path = "connbd";
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelRequisiciones.aspx/GetListaItemsProveedores",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                let tablaProveedores = $('#tablaProveedores').DataTable({
                    pageLength: 5,
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [

                        { data: 'IdProveedor' },
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
                    dom: 'frtipl'

                });



                $('#tablaProveedores').on('draw.dt', function () {



                    $('.boton-seleccionar-proveedor').on('click', function (e) {
                        e.preventDefault();

                        idProveedorSeleccionado = $(this).attr('data-idproveedor');
                        $('#btnAbrirSeleccionarProveedor').prop('data-idproveedor', idProveedorSeleccionado);

                        $('#txtNombreProveedor').val(idProveedorSeleccionado + ' - ' + $(this).attr('data-nombreproveedor'));




                        $('#panelSeleccionarProveedor').modal('hide');




                    });



                });

                $('#tablaProveedores').trigger('draw.dt');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }



    function cargarComboMarcas() {
        var parametros = new Object();
        parametros.path = "connbd";
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


    function cargarComboEstatus() {
        var parametros = new Object();
        parametros.path = "connbd";
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


    $('#btnEnviarAceptar').on('click', (e) => {
        e.preventDefault();

        var parametros = new Object();
        parametros.path = "connbd";
        parametros.id = idSeleccionado;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelRequisiciones.aspx/EnviarRequisicion",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado.MensajeError === null) {

                    utils.toast(mensajesAlertas.exitoEnviar, 'ok');


                    cargarItems();

                } else {

                    utils.toast(resultado.MensajeError, 'error');


                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    });


    $('#btnAbrirSeleccionarEquipo').click(function (e) {
        e.preventDefault();

        cargarItemsEquipos();
        $('#panelSeleccionarEquipo').modal('show');


    });

    $('#btnAbrirSeleccionarProveedor').click(function (e) {
        e.preventDefault();

        cargarItemsProveedores();
        $('#panelSeleccionarProveedor').modal('show');


    });

    $('#btnNuevo').click(function (e) {
        e.preventDefault();
        nuevo();

    });



    function cargarComboModelos(idMarca, idModelo) {
        var parametros = new Object();
        parametros.path = "connbd";
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


    $('#btnGuardar').on('click', function (e) {

        e.preventDefault();

        var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


        if (hasErrors) {
            return;
        }

        if (idEquipoSeleccionado == -1) {
            utils.toast(mensajesAlertas.errorSeleccionarEquipo, 'error');

            return;
        }

        if (idProveedorSeleccionado == -1 || idProveedorSeleccionado == 0) {
            utils.toast(mensajesAlertas.errorSeleccionarProveedor, 'error');

            return;
        }

        var item = new Object();
        item.IdRequisicion = idSeleccionado;
        item.IdEquipo = idEquipoSeleccionado;
        item.IdProveedor = idProveedorSeleccionado;
        item.Descripcion = $('#txtDescripcion').val();
        item.Diagnostico = $('#txtDiagnostico').val();
        item.Coordenadas = $('#txtCoordenadas').val();



        var parametros = new Object();
        parametros.path = "connbd";
        parametros.item = item;
        parametros.accion = accion;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelRequisiciones.aspx/Guardar",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var valores = msg.d;



                if (valores.CodigoError == 0) {

                    idSeleccionado = valores.IdItem;

                    $('#txtNumeroRequisicion').val(idSeleccionado);

                    //guardar documentos
                    $('.file-documentos').each(function (documento) {


                        let file;
                        if (file = this.files[0]) {

                            utils.sendFile(file, 'documentos', valores.IdItem, 'documento');

                        }

                    });



                    $('#spnMensajes').html(mensajesAlertas.exitoGuardar);
                    $('#panelMensajes').modal('show');
                    cargarItems();

                } else {

                    utils.toast(mensajesAlertas.errorGuardar, 'error');

                }



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);

                utils.toast(mensajesAlertas.errorGuardar, 'error');




            }

        });

    });


    $('#btnAceptarPanelMensajes').on('click',  (e) => {

        e.preventDefault();
        $('#panelMensajes').modal('hide');

        $('#panelTabla').show();
        $('#panelForm').hide();

    });


    $('#btnAgregarOtroDocumento').on('click', function (e) {
        e.preventDefault();

        let htmlDoc = '<div class="form-group col-md-4">';
        htmlDoc += '<input type="file" class="form-control file-documentos" />';
        htmlDoc += '<div class="help-block with-errors"></div>';
        htmlDoc += '</div>';

        $('#divDocumentos').append(htmlDoc);


    });


    $('#btnCancelar').on('click', function (e) {
        e.preventDefault();

        $('#panelTabla').show();
        $('#panelForm').hide();

    });


    $('#btnEliminarAceptar').on('click', function () {

        var parametros = new Object();
        parametros.path = "connbd";
        parametros.id = idSeleccionado;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelRequisiciones.aspx/EliminarRequisicion",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado.MensajeError === null) {

                    utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                    cargarItems();

                } else {

                    utils.toast(resultado.MensajeError, 'error');


                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    });

    
});//ready


function eliminar(id) {

    idSeleccionado = id;

    $('#panelEliminar').modal('show');

}


function cargarComboModelos_(idMarca, idModelo) {
    var parametros = new Object();
    parametros.path = "connbd";
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


const enviar = (id) => {


    var parametros = new Object();
    parametros.path = "connbd";
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/PanelRequisiciones.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var item = msg.d;
            idSeleccionado = item.IdRequisicion;
            idEquipoSeleccionado = item.IdEquipo;
            idProveedorSeleccionado = item.IdProveedor;


            accion = "editar";

            if (idProveedorSeleccionado === 0 || idProveedorSeleccionado === -1) {

                $('#spnMensajes').html(mensajesAlertas.errorSeleccionarProveedorAlEnviar);
                $('#panelMensajes').modal('show');
                    
            } else {

                $('#msgEnviar').html('<p>Se enviará al proveedor, el registro seleccionado(No.' + id + '). ¿Desea continuar?</p>');
                $('#panelEnviar').modal('show');

            }


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

  
}


const editar = (id) => {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#frm')[0].reset();
    $('#divDocumentos').empty();

    var parametros = new Object();
    parametros.path = "connbd";
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/PanelRequisiciones.aspx/GetItem",
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
            $('#txtOrometro').val(item.Orometro);
            $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);

            $('#txtFechaCreacion').val(item.FechaCreacionFormateada);
            $('#txtDescripcion').val(item.Descripcion);
            $('#txtNumeroEconomico').val(item.NumeroEconomico);
            $('#txtDiagnostico').val(item.Diagnostico);
            $('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
            $('#comboMarca').val(item.IdMarca);
            cargarComboModelos_($('#comboMarca').val(), item.IdModelo);


            accion = "editar";
            $('#spnTituloForm').text('Editar requisición');
            $('.deshabilitable').prop('disabled', false);

            let parametros = {
                path: "connbd",
                id_requisicion : item.IdRequisicion,
                idUsuario : sessionStorage.getItem("idusuario"),
                id_tipo : 1
            };





            utils.getDocumentos(parametros);



         


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}


function abrir(id) {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#frm')[0].reset();
    $('#divDocumentos').empty();

    var parametros = new Object();
    parametros.path = "connbd";
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/PanelRequisiciones.aspx/GetItem",
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
            $('#txtDiagnostico').val(item.Diagnostico);
            $('#txtFechaCreacion').val(item.FechaCreacionFormateada);
            $('#txtDescripcion').val(item.Descripcion);
            $('#txtNumeroEconomico').val(item.NumeroEconomico);
            $('#txtCoordenadas').val(item.Latitud + ', ' + item.Longitud);
            $('#comboMarca').val(item.IdMarca);
            cargarComboModelos_($('#comboMarca').val(), item.IdModelo);
            $('#txtOrometro').val(item.Orometro);
            $('#chkDetieneOperacion').prop('checked', item.DetieneOperacion == 1);


            accion = "editar";
            $('#spnTituloForm').text('Datos requisición');

            $('.deshabilitable').prop('disabled' , true);


            let parametros = {
                path: "connbd",
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


}


function nuevo() {


    $('#frm')[0].reset();
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#spnTituloForm').text('Nueva requisición');
    $('#txtDescripcion').val('');




    $('#panelTabla').hide();
    $('#panelForm').show();
    accion = "nuevo";
    id = -1;
    idSeleccionado = -1;
    idEquipoSeleccionado = -1;
    idProveedorSeleccionado = -1;

    $('.deshabilitable').prop('disabled', false);

    obtenerFechaHoraServidor('txtFechaCreacion');



}

