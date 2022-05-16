var accion = "";
var idSeleccionado = "-1";
var table;
var date = new Date();
var descargas = "Proveedores_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
var pagina = '70';

$(document).ready(function () {

    console.log('Inicializando');
    $('#alert-operacion-ok').hide();
    $('#alert-operacion-fail').hide();

    $('#panelTabla').show();
    $('#panelForm').hide();


    cargarItems();

    function cargarItems() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/ProveedoresCombustible.aspx/GetListaItems",
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

                        { data: 'Nombre' },
                        { data: 'NombreContacto' },
                        { data: 'Domicilio' },
                        { data: 'Telefono' },

                        { data: 'Accion' }

                    ],
                    "language": {
                        "sProcessing": "Procesando...",
                        "sLengthMenu": "Mostrar _MENU_ registros",
                        "sZeroRecords": "No se encontraron resultados",
                        "sEmptyTable": "Ningún dato disponible en esta tabla",
                        "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                        "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
                        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
                        "sInfoPostFix": "",
                        "sSearch": "Buscar:",
                        "sUrl": "",
                        "sInfoThousands": ",",
                        "sLoadingRecords": "Cargando...",
                        "oPaginate": {
                            "sFirst": "Primero",
                            "sLast": "Último",
                            "sNext": "Siguiente",
                            "sPrevious": "Anterior"
                        },
                        "oAria": {
                            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
                        }
                    },
                    "columnDefs": [
                        {
                            "targets": [-1], //para la ultima columna
                            "orderable": false, //que no se ordene
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



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }


    $('#btnGuardar').click(function (e) {

        e.preventDefault();


        var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


        if (!hasErrors) {


            var proveedor = new Object();
            proveedor.IdProveedor = idSeleccionado;
            proveedor.Nombre = $('#txtNombre').val();
            proveedor.Domicilio = $('#txtDomicilio').val();

            proveedor.Telefono = $('#txtTelefono').val();
            proveedor.Correo = $('#txtCorreo').val();
            proveedor.NombreContacto = $('#txtNombreContacto').val();
          



            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.proveedor = proveedor;
            parametros.accion = accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/ProveedoresCombustible.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var resultado = parseInt(msg.d);

                    if (resultado > 0) {

                        $("#lblMensajesOk").text('Registro guardado correctamente...');
                        $("#alert-operacion-ok").show("fast", function () {
                            setTimeout(function () {
                                $("#alert-operacion-ok").hide("fast");
                            }, 2000);
                        });


                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        cargarItems();


                    } else {

                        $("#lblMensajesFail").text('No se pudo guardar el registro...');

                        $("#alert-operacion-fail").show("fast", function () {
                            setTimeout(function () {
                                $("#alert-operacion-fail").hide("fast");
                            }, 2000);
                        });

                    }

                    $('#panelEdicion').modal('hide');


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        }

    });



    $('#btnCancelar').click(function (e) {
        e.preventDefault();

        $('#panelTabla').show();
        $('#panelForm').hide();

    });

    $('#btnNuevo').click(function (e) {
        e.preventDefault();

        nuevo();


    });
    $('#btnEliminarAceptar').click(function () {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = idSeleccionado;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/ProveedoresCombustible.aspx/EliminarProveedor",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado > 0) {

                    $("#lblMensajesOk").text('Registro eliminado correctamente...');
                    $("#alert-operacion-ok").show("fast", function () {
                        setTimeout(function () {
                            $("#alert-operacion-ok").hide("fast");
                        }, 2000);
                    });

                    table.destroy();
                    cargarItems();

                } else {

                    $("#lblMensajesFail").text('No se pudo eliminar el registro...');

                    $("#alert-operacion-fail").show("fast", function () {
                        setTimeout(function () {
                            $("#alert-operacion-fail").hide("fast");
                        }, 2000);
                    });

                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    });

});


function editar(id) {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/ProveedoresCombustible.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var datosProveedor = msg.d;
            $('#txtNombre').val(datosProveedor.Nombre);
            $('#txtNombreContacto').val(datosProveedor.NombreContacto);
            $('#txtDomicilio').val(datosProveedor.Domicilio);

            $('#txtTelefono').val(datosProveedor.Telefono);
            $('#txtCorreo').val(datosProveedor.Correo);
          




            idSeleccionado = datosProveedor.IdProveedor;
            accion = "editar";
            $('#spnTituloForm').text('Editar');

            $('#panelTabla').hide();
            $('#panelForm').show();


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}

function eliminar(id) {
    idSeleccionado = id;
    $('.modal-title').text('Confirmar');
    $('#panelEliminar').modal('show');

}

function nuevo() {
    console.log('Desde nuevo cliente');

    $('#frm')[0].reset();
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#spnTituloForm').text('Nuevo');




    $('#panelTabla').hide();
    $('#panelForm').show();
    accion = "nuevo";
    id = "-1";
}

