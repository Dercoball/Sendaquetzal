var accion = "";
var idSeleccionado = "-1";
var table;
var date = new Date();
var descargas = "StatusProyectos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
var pagina = '23';


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
            url: "../pages/StatusProyectos.aspx/GetListaItems",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                console.log(msg.d);



                ////Se crea la tabla tipo bootstrap, msg.d es la lista de datos
                table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'Nombre' },


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


    $('#btnNuevo').click(function (e) {
        //console.log('Desde nuevo');
        e.preventDefault();

        nuevo();

    });


    $('#btnGuardar').click(function (e) {
        //console.log('Boton guardar');

        e.preventDefault();


        var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


        if (!hasErrors) {


            //console.log('idSeleccionado =' + idSeleccionado);

            ////debugger;;
            var statusProyecto= new Object();
            statusProyecto.IdStatusProyecto = idSeleccionado;
            statusProyecto.Nombre = $('#txtNombre').val();



            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.statusProyecto = statusProyecto;
            parametros.accion = accion;
            var parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/StatusProyectos.aspx/Guardar",
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

                    //ocultar panel flotante
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

    $('#btnEliminarAceptar').click(function () {
        //console.log('Desde la funcion eliminar');
        //console.log('Id ' + id);

        //Ajax para eliminar el registro
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = idSeleccionado;
        var parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/StatusProyectos.aspx/EliminarStatus",
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


                    //Refrescamos la tabla
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



//Funcion para invocar el borrado de un registro
function eliminar(id) {
    //console.log('Desde la funcion editar');
    //console.log('Id ' + id);
    idSeleccionado = id;
    //$('.modal-title').text('Confirmar');
    //mostrar confirmación de eliminar
    $('#panelEliminar').modal('show');

}

function editar(id) {
    console.log('Desde la funcion editar');
    console.log('Id ' + id);

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();

    accion = "nuevo";
    //Ajax para traer todos los datos del registro
    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    var parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/StatusProyectos.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            //debugger;;
            //Una vez que se tienen los datos, se  pintan en los controles
            var datosStatus = msg.d;
            $('#txtNombre').val(datosStatus.Nombre);



            idSeleccionado = datosStatus.IdStatusProyecto;
            accion = "editar";
            $('#spnTituloForm').text('Editar');

            $('#panelTabla').hide();
            $('#panelForm').show();


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}

function nuevo() {
    //console.log('Desde nuevo');
    //e.preventDefault();

    $('#frm')[0].reset();
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#spnTituloForm').text('Nuevo');




    $('#panelTabla').hide();
    $('#panelForm').show();
    accion = "nuevo";
    id = "-1";
}
