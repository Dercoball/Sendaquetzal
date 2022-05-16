var accion = "";
var idSeleccionado = "-1";
var table;
var date = new Date();
var descargas = "Obras_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
var pagina = '55';


$(document).ready(function () {


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
            url: "../pages/Obras.aspx/GetListaItems",
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
                        { data: 'ActivoStr' },
                        { data: 'UltimaModificacionStr' },


                        { data: 'Accion' }

                    ],
                    "language": textosEsp,

                    "columnDefs": [
                        {
                            "targets": [-1], //para la ultima columna
                            "orderable": false //que no se ordene
                        }
                    ],
                    dom: 'fBrtip',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Exportar Csv', className: 'csvbtn'
                        },
                        {
                            extend: 'pdfHtml5',
                            title: descargas,
                            text: 'Xls', className: 'pdfbtn'
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
            $('#panelLoading').modal('show');


            var item = new Object();
            item.IdObra = idSeleccionado;
            item.Nombre = $('#txtNombre').val();
            item.Activo = $('#chkActivo').prop('checked') ? 1 : 0;



            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = accion;
            parametros.idUsuario = sessionStorage.getItem("idusuario");

            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Obras.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var resultado = parseInt(msg.d);

                    $("#panelLoading").show("fast", function () {
                        setTimeout(function () {
                            $('#panelLoading').modal('hide');

                        }, 500);
                    });

                    if (resultado > 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        faq.cargarItems();


                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'fail');


                    }

                    $('#panelEdicion').modal('hide');


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


        }

    });


    $('#btnNuevo').on('click', function (e) {
        e.preventDefault();

        idSeleccionado = -1;


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');

        $('#panelTabla').hide();
        $('#panelForm').show();
        accion = "nuevo";
        
    });

    $('#btnCancelar').on('click', function (e) {
        e.preventDefault();

        $('#panelTabla').show();
        $('#panelForm').hide();

    });

    $('#btnEliminarAceptar').on('click', function () {
    
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = idSeleccionado;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Obras.aspx/EliminarItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado > 0) {

                    utils.toast(mensajesAlertas.exitoEliminar, 'ok');

                    table.destroy();
                    cargarItems();

                } else {
                    utils.toast(mensajesAlertas.errorEliminar, 'fail');


                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    });




});


function eliminar(id) {
    idSeleccionado = id;
    $('#panelEliminar').modal('show');

}

function editar(id) {
   
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Obras.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

           
            var datosItem = msg.d;
            $('#txtNombre').val(datosItem.Nombre);
            $('#chkActivo').prop('checked', datosItem.Activo == 1 ? true : false);



            idSeleccionado = datosItem.IdObra;
            accion = "editar";
            $('#spnTituloForm').text('Editar');

            $('#panelTabla').hide();
            $('#panelForm').show();


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}

