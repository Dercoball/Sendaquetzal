
var accion = "";
var idSeleccionado = "-1";
var table;
var date = new Date();
var descargas = "Usuarios_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
var pagina = '9';




$(document).ready(function () {

    cargarItems();
    function cargarItems() {

        var parametros = new Object();
        parametros.path = window.location.hostname;        
        parametros.idUsuario = document.getElementById('txtIdUsuario').value;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/GetListaUsuarios",
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
                        { data: 'Login' },
                        { data: 'NombreTipoUsuario' },
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




    $('#panelTabla').show();
    $('#panelForm').hide();

    $('#btnNuevo').on('click', function (e) {
        e.preventDefault();

        nuevo();

    });


    $('#btnGuardar').click(function (e) {
        e.preventDefault();


        var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }

        if ($('#comboTipoUsuario').val() === '2') {

            if ($('#comboProveedor').val() == null) {
                toastr(mensajesAlertas.errorSeleccionarProveedorUsuario, 'error');

            }

        }


        let promesa = new Promise((resolve, reject) => {

            var usuario = new Object();
            usuario.Id_Usuario = idSeleccionado;
            usuario.Nombre = $('#txtNombre').val();
            usuario.Login = $('#txtLogin').val();
            usuario.Password = $('#txtPass').val();
            usuario.Email = $('#txtEmail').val();
            usuario.Telefono = $('#txtTelefono').val();
            usuario.IdTipoUsuario = $('#comboTipoUsuario').val();
            usuario.IdProveedor = $('#comboProveedor').val() === null ? '-1' : $('#comboProveedor').val();
            usuario.IdEmpleado = $('#comboEmpleado').val() === '' ? '-1' : $('#comboEmpleado').val();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.usuario = usuario;
            parametros.accion = accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Usuarios.aspx/GuardarUsuario",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var resultado = parseInt(msg.d);

                    if (resultado > 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');



                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        cargarItems();

                        resolve(mensajesAlertas.exitoGuardar);


                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'fail');

                        regect(mensajesAlertas.errorGuardar);
                    }


                    //ocultar panel flotante
                    $('#panelEdicion').modal('hide');


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    regect(mensajesAlertas.errorGuardar);
                    utils.toast(mensajesAlertas.errorGuardar, 'fail');

                }

            });

        });

        return promesa;




    });


    $('#btnGuardarPassword').click(function (e) {

        e.preventDefault();


        var hasErrors = $('form[name="frmUsuarioP"]').validator('validate').has('.has-error').length;


        if (!hasErrors) {
            var usuario = new Object();
            usuario.Id_Usuario = idSeleccionado;
            usuario.Password = $('#txtPassP').val();


            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.usuario = usuario;
            parametros.accion = accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "Usuarios.aspx/GuardarUsuarioP",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var resultado = parseInt(msg.d);

                    if (resultado > 0) {

                        utils.toast(mensajesAlertas.exitoPasswordModificada, 'ok');

                        table.destroy();
                        cargarItems();

                    } else {

                        utils.toast(mensajesAlertas.errorInesperado, 'fail');



                    }

                    $('#panelEdicionPass').modal('hide');


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


    $('#btnEliminarAceptar').click(function (e) {
        e.preventDefault();

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = idSeleccionado;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/EliminarUsuario",
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


    $('#comboTipoUsuario').on('change', (e) => {
        e.preventDefault();


        console.log('change');

        if ($('#comboTipoUsuario').val() === '2') { //si es de tipo proveedor
            $('#divProveedores').show();
            $('#divEmpleados').hide();
        } else {
            $('#divEmpleados').show();
            $('#divProveedores').hide();
        }

    });



    cargarTiposUsuarios();
    cargarProveedores();
    cargarEmpleados();

    function cargarTiposUsuarios() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/GetListaTiposUsuario",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var opcion = "";
                var valores = msg.d;
                for (var i = 0; i < valores.length; i++) {
                    var item = valores[i];

                    opcion += '<option value="' + item.IdTipoUsuario + '">' + item.Nombre + '</option>';
                }

                $("#comboTipoUsuario").empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    }


    function cargarProveedores() {

        var parametros = new Object();
        parametros.path = window.location.hostname;        
        parametros.idUsuario = document.getElementById('txtIdUsuario').value;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/GetListaProveedores",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var opcion = "";
                var valores = msg.d;
                for (var i = 0; i < valores.length; i++) {
                    var item = valores[i];

                    opcion += '<option value="' + item.IdProveedor + '">' + item.Nombre + '</option>';
                }

                $("#comboProveedor").empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    }

    function cargarEmpleados() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.like = '';
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/GetListaEmpleados",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                //console.log(msg.d);

                //var opcion = "";
                //var valores = msg.d;
                //for (var i = 0; i < valores.length; i++) {
                //    var item = valores[i];

                //    opcion += '<option value="' + item.IdEmpleado + '">' + item.NombreCompleto + '</option>';
                //}

                //$("#comboEmpleados").empty().append(opcion);

                var datos = JSON.parse(msg.d);
                var dataAdapter = new $.jqx.dataAdapter(datos);


                $("#comboEmpleado").jqxDropDownList({
                    source: dataAdapter, displayMember: "NombreCompleto", valueMember: "IdEmpleado", width: '350px',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboEmpleado").jqxDropDownList('clearSelection');



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });

    }
    $('#btnQuitar').click(function (e) {
        e.preventDefault();


        $("#listaPermisosSeleccionados option:selected").each(function () {

            var valor = $(this).val();
            var texto = $(this).text();

            console.log('permiso value = ' + valor);
            console.log('permiso text = ' + texto);

            var opcion = '<option value="' + valor + '">' + texto + '</option>';
            $('#listaPermisos').append(opcion);


            $("#listaPermisosSeleccionados option[value='" + valor + "']").remove();


        });



    });


    $('#btnAgregar').click(function (e) {
        e.preventDefault();


        $("#listaPermisos option:selected").each(function () {

            var valor = $(this).val();
            var texto = $(this).text();

            console.log('permiso value = ' + valor);
            console.log('permiso text = ' + texto);

            var opcion = '<option value="' + valor + '">' + texto + '</option>';
            $('#listaPermisosSeleccionados').append(opcion);


            $("#listaPermisos option[value='" + valor + "']").remove();


        });



    });


    $('#btnAgregarTodos').click(function (e) {
        e.preventDefault();

        var parametros = new Object();
        parametros.idUsuario = $('#spnIdUsuario').text();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);


        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/ObtenerTodosLosPermisos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdPermiso + '">' + item.Nombre + '</option>';

                }

                $('#listaPermisos').empty();
                $('#listaPermisosSeleccionados').empty().append(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });



    });


    $('#btnQuitarTodos').click(function (e) {
        e.preventDefault();

        var parametros = new Object();

        parametros.path = window.location.hostname;


        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/ObtenerTodosLosPermisos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdPermiso + '">' + item.Nombre + '</option>';

                }

                $('#listaPermisosSeleccionados').empty();
                $('#listaPermisos').empty().append(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });




    });




    $('#btnGuardarEquipos').click(function (e) {

        e.preventDefault();
        console.log('Boton btnGuardarPermisos');

        let listaEquipos = [];

        $("#listaEquiposSeleccionados option").each(function () {

            var item = {
                IdEquipo: $(this).val(),
                IdUsuario: $('#spnIdUsuario').text()
            }

            listaEquipos .push(item);
        });


        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.listaEquipos = listaEquipos;
        parametros.idUsuario = $('#spnIdUsuario').text();

        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "Usuarios.aspx/GuardarEquiposUsuario",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var resultado = parseInt(msg.d);

                if (resultado > 0) {

                    utils.toast(mensajesAlertas.exitoGuardar, 'ok');



                } else {
                    utils.toast(mensajesAlertas.errorGuardar, 'error');


                }

                $('#panelEquipos').modal('hide');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    });


    $("#listaEquipos").on("change", (e) => {
        e.preventDefault();

        let args = e.currentTarget.selectedOptions;

        if (args[0].value) {
            let value = args[0].value;
            let text = args[0].text;

            let item = '<option value="' + value + '">' + text + "</option>";

            $("#listaEquiposSeleccionados").append(item);
            $("#listaEquipos option[value='" + value + "']").remove();
        }
    });

    $("#listaEquiposSeleccionados").on("change", (e) => {
        e.preventDefault();

        let args = e.currentTarget.selectedOptions;

        if (args[0].value) {
            let value = args[0].value;
            let text = args[0].text;

            let item = '<option value="' + value + '">' + text + "</option>";

            $("#listaEquipos").append(item);
            $("#listaEquiposSeleccionados option[value='" + value + "']").remove();
        }
    });


}); // fin de ready


function editar(id) {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();

    accion = "nuevo";
    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Usuarios.aspx/GetUsuario",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var datosUsuario = msg.d;
            $('#txtNombre').val(datosUsuario.Nombre);
            $('#txtLogin').val(datosUsuario.Login);
            $('#txtEmail').val(datosUsuario.Email);
            $('#txtTelefono').val(datosUsuario.Telefono);
            $('#txtPass').val(datosUsuario.Password);
            $('#comboTipoUsuario').val(datosUsuario.IdTipoUsuario);

            $("#comboEmpleado").jqxDropDownList('clearSelection');


            $("#comboEmpleado").val(datosUsuario.IdEmpleado);
            $("#comboProveedor").val(datosUsuario.IdProveedor);

            idSeleccionado = datosUsuario.Id_Usuario;
            accion = "editar";
            $('#spnTituloForm').text('Editar');

            if ($('#comboTipoUsuario').val() === '2') { //si es de tipo proveedor
                $('#divProveedores').show();
                $('#divEmpleados').hide();
            } else {
                $('#divEmpleados').show();
                $('#divProveedores').hide();
            }



            $('#panelTabla').hide();
            $('#panelForm').show();


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}


function editarP(id) {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();

    accion = "editar";
    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "Usuarios.aspx/GetUsuario",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var datosUsuario = msg.d;

            $('#txtLoginP').val(datosUsuario.Login);
            $('#txtPassP').val('');


            idSeleccionado = datosUsuario.Id_Usuario;
            accion = "editar";

            $('#panelEdicionPass').modal('show');


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}
function eliminar(id) {

    idSeleccionado = id;
    $('#panelEliminar').modal('show');

}


function nuevo() {

    $('#frm')[0].reset();
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#spnTituloForm').text('Nuevo');


    idSelecccionado = -1;

    $('#panelTabla').hide();
    $('#panelForm').show();
    accion = "nuevo";
    id = "-1";

    $("#divProveedores").hide();

    $("#comboEmpleado").jqxDropDownList('clearSelection');


}

const asignarEquipos = (idUsuario, nombre) => {
    //tiposUsuario.idSeleccionado = idUsuario;
    $('.spnNombreUsuario').text(nombre);
    $('#spnIdUsuario').text(idUsuario);


    equiposUsuario.cargarListaEquipos(idUsuario);
    equiposUsuario.cargarListaPermisosUsuario(idUsuario);

    $('#panelEquipos').modal('show');

}

const equiposUsuario = {

    cargarListaEquipos: (idUsuario) => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = idUsuario;
        parametros = JSON.stringify(parametros);

        $('#listaEquipos').empty();

        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/GetListaEquipos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdEquipo + '">' + item.NumeroEconomico + ' - ' + item.Nombre + '</option>';

                }

                $('#listaEquipos').empty().append(opcion);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },

    cargarListaPermisosUsuario: (idUsuario) => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = idUsuario;
        parametros = JSON.stringify(parametros);

        $('#listaEquiposSeleccionados').empty();

        $.ajax({
            type: "POST",
            url: "../pages/Usuarios.aspx/GetListaEquiposPorUsuario",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdEquipo + '">' + item.NumeroEconomico + ' - ' + item.Nombre + '</option>';

                }

                $('#listaEquiposSeleccionados').empty().append(opcion);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    }


}