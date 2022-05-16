'use strict';
let date = new Date();
let descargas = "Empleados_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '28';


class Empleado {

    

}

const empleado = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        empleado.idSeleccionado = "-1";
        empleado.accion = "";


        empleado.cargarComboDepartamentos();
        empleado.cargarComboPuestos();
        empleado.cargarItems();

    },

    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/Empleados.aspx/GetListaItems';

        
        utils.postData(url, parametros)
            .then(data => {
                

                data = data.d;



                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        //{ data: 'IdEmpleado' },
                        { data: 'Clave' },
                        { data: 'Nombre' },
                        { data: 'APaterno' },
                        { data: 'AMaterno' },
                        { data: 'NombrePuesto' },
                        { data: 'NombreDepartamento' },
                        { data: 'ActivoStr' },
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

            });







    },


    eliminar: (id) => {

        empleado.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },



    editar: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = id;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Empleados.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                empleado.idSeleccionado = item.IdEmpleado;

                console.log('.');
                $('#txtNombre').val(item.Nombre);
                $('#txtAPaterno').val(item.APaterno);
                $('#txtAMaterno').val(item.AMaterno);
                $('#comboDepartamento').val(item.IdDepartamento);
                $('#comboPuesto').val(item.IdPuesto);
                $('#txtClave').val(item.Clave);
                $('#chkActivo').prop('checked', item.Activo === 1);

                $('#panelTabla').hide();
                $('#panelForm').show();


                empleado.acccion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);
                $('#img_').attr('src', `../img/logo_small.jpg`);


                var parametros = new Object();
                parametros.path = window.location.hostname;
                parametros.idEmpleado = empleado.idSeleccionado;
                parametros = JSON.stringify(parametros);
                $.ajax({
                    type: "POST",
                    url: "../pages/Empleados.aspx/GetFoto",
                    data: parametros,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (foto) {

                        let strFoto = foto.d;
                        if (strFoto != '') {
                            $('#img_').attr('src', `data:image/jpg;base64,${strFoto}`);
                        }

                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },



    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');
        $('#txtDescripcion').val('');




        $('#panelTabla').hide();
        $('#panelForm').show();
        empleado.acccion = "nuevo";
        empleado.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFechaCreacion');
        $('#img_').attr('src', `../img/logo_small.jpg`);



    },

    cargarComboDepartamentos: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Empleados.aspx/GetListaItemsDepartamentos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdDepartamento + '">' + item.Nombre + '</option>';

                }

                $('#comboDepartamento').empty().append(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });




    
    },

    cargarComboPuestos: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Empleados.aspx/GetListaItemsPuestos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdPuesto + '">' + item.Nombre + '</option>';

                }

                $('#comboPuesto').empty().append(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    accionesBotones: () => {






        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            empleado.nuevo();

        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }




            var item = new Object();
            item.IdEmpleado = empleado.idSeleccionado;
            item.Activo = $('#chkActivo').prop('checked') == true ? 1 : 0;
            item.Nombre = $('#txtNombre').val();
            item.IdDepartamento = $('#comboDepartamento').val();
            item.IdPuesto = $('#comboPuesto').val();
            item.APaterno = $('#txtAPaterno').val();
            item.AMaterno= $('#txtAMaterno').val();
            item.Clave = $('#txtClave').val();




            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = empleado.acccion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Empleados.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        if (accion === 'nuevo') {
                            //guardar documentos
                            $('.file-fotografia').each(function (documento) {

                                let file;
                                if (file = this.files[0]) {

                                    utils.sendFile(file, 'fotografia_empleado', valores.IdItem, 'fotografia_empleado');

                                }

                            });
                        }


                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        empleado.cargarItems();

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });


        $('.file-fotografia').on('change', function (e) {
            e.preventDefault();

            if (empleado.idSeleccionado !== "-1") {


                //debugger;
                let file;
                if (file = this.files[0]) {

                    utils.sendFile(file, 'fotografia_empleado', empleado.idSeleccionado, 'fotografia_empleado');

                    setTimeout(function () {

                        //  Mostrar la imagen que se acaba de subir...
                        var parametros = new Object();
                        parametros.path = window.location.hostname;
                        parametros.idEmpleado = empleado.idSeleccionado;
                        parametros = JSON.stringify(parametros);
                        $.ajax({
                            type: "POST",
                            url: "../pages/Empleados.aspx/GetFoto",
                            data: parametros,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            success: function (foto) {

                                let strFoto = foto.d;
                                if (strFoto != '') {
                                    $('#img_').attr('src', `data:image/jpg;base64,${strFoto}`);
                                }

                            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                                console.log(textStatus + ": " + XMLHttpRequest.responseText);
                            }

                        });

                    }, 5000);

                }
            }


        });


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = empleado.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Empleados.aspx/Eliminar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        empleado.cargarItems();

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

    empleado.init();

    empleado.accionesBotones();

});


