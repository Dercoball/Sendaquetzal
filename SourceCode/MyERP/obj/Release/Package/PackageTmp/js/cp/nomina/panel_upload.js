'use strict';
let date = new Date();
let descargas = "Uploads_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '62';

const upload = {


    init: () => {

        $('#panelTabla').hide();
        $('#panelForm').show();

        upload.idSeleccionado = -1;
        upload.accion = '';


    },

    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/uploads.aspx/GetListaItems';

   
        utils.postData(url, parametros)
            .then(data => {
                console.log(data); // JSON data parsed by `data.json()` call


                data = data.d;



                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'Idupload' },
                        { data: 'Nombre' },
                        { data: 'Clave' },
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

        upload.idSeleccionado = id;

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
            url: "../pages/uploads.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                upload.idSeleccionado = item.Idupload;

                console.log('.');
                $('#txtNombre').val(item.Nombre);
                $('#txtClave').val(item.Clave);
                $('#chkActivo').prop('checked', item.Activo === 1);

                $('#panelTabla').hide();
                $('#panelForm').show();


                upload.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);





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
        upload.accion = "nuevo";
        upload.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFechaCreacion');



    },

    sendFile: (file, nombreArchivo) => {


        var files = file;
        var fileName = files.name;
        var extension = utils.getFileExtension(fileName);
        console.log(`extension = ${extension}`);
        if (extension !== 'csv' && extension !== 'txt') {
            console.log('archivo incorrecto');
            
            utils.toast(mensajesAlertas.errorSubirCsv, 'error');
            return;
        }
        $('#panelLoading').modal('show');

        var formData = new FormData();
        formData.append('file', file);
        formData.append('pagina', window.location.pathname);
        formData.append('path', window.location.hostname);
        formData.append('extension', extension);
        formData.append('descripcion', fileName);
        formData.append('tipo', 'csv');
        formData.append('usuario', sessionStorage.getItem("idusuario"));


        $.ajax({
            type: 'post',
            url: '../pages/FileUploaderCSV.ashx',
            data: formData,
            success: function (status) {

                if (status != 'error') {
                    var valores = JSON.parse(status);


                    if (valores === -1) {

                        setTimeout(function () {
                            $('#panelLoading').modal('hide');

                            $('#spnTipoMensaje').text('Información');
                            $('#spnContenidoMensaje').html('Errores en el proceso.<br/><ul>' +
                                '<li>No existe la configuración correcta.' +
                                '</ul>');

                            $('#panelMensajes').modal('show');


                        }, 500);

                    }
                    else
                        if (valores === null) {

                            setTimeout(function () {
                                $('#panelLoading').modal('hide');

                                $('#spnTipoMensaje').text('Información');
                                $('#spnContenidoMensaje').html('Errores en el proceso.<br/>Posibles problemas:<ul>' +
                                    '<li>No ha generado la tabla de detalle.' +
                                    '<li>El archivo que seleccionó no es del formato correcto.' +
                                    '</ul>');

                                $('#panelMensajes').modal('show');


                            }, 500);



                        } else {


                            $('#tableResultados >tbody').empty();

                            for (var i = 0; i < valores.length; i++) {
                                var tr = "<tr>";

                                var ok = valores[i].valido;

                                var msgOk = (ok === true) ? '<span class="badge badge-success">Correcto</span>' : '<span class="badge badge-danger">Incorrecto</span>';

                                tr += `<td>${msgOk}</td>`;

                                var linea = valores[i].parts;
                                var estiloCss = valores[i].estilosCss;

                                for (var j = 0; j < linea.length; j++) {

                                    var item = linea[j];
                                    tr += `<td class='${estiloCss[j + 1]}'>${item}</td>`;
                                }

                                tr += `</tr>`;

                                $('#tableResultados >tbody').append(tr);

                            }



                            $("#panelLoading").show("fast", function () {
                                setTimeout(function () {
                                    $('#panelLoading').modal('hide');

                                    $('#spnTipoMensaje').text('Información');
                                    $('#spnMensajes').html('Operación finalizada.<br/>' + valores[0].mensajeError);


                                    $('#panelMensajes').modal('show');

                                    upload.generarTabla(upload.idSeleccionado);


                                }, 500);
                            });

                        }


                }
            },
            processData: false,
            contentType: false,
            error: function () {
                $('#panelLoading').modal('hide');

            }
        });
    },


     generarTabla : (id) => {
        //Traer lista campos

        //var parametros = new Object();
        //parametros.path = window.location.hostname;
        //parametros.idImpuesto = id;
        //parametros = JSON.stringify(parametros);
        //$.ajax({
        //    type: "POST",
        //    url: "../pages/Impuestos.aspx/GetListaItemsCampos_Reales",
        //    data: parametros,
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        //    async: true,
        //    success: function (msg) {

                //console.log(JSON.stringify(msg.d));

         var items = ['clave empleado', 'nombre',
             'apellido  paterno', 'apellido materno',
             'clave depto.', 'depto.', 'clave area', 'area', 'clave puesto', 'puesto',
                    'fecha_alta', 'salario_mens', 'id. reg. pat.', 'reg. pat.', 'reg. pat.']

                var opcion = "";
                var opcion2 = "";

                opcion2 += '<th>Status</th>';


                for (var i = 0; i < items.length; i++) {
                    var item = items[i].toUpperCase();

                    opcion += '<th>' + item + '</th>';

                    //if (i > 0)
                        opcion2 += '<th>' + item + '</th>';

                }

                $('#tableResultados >thead').empty().append(opcion2);


                //$('#tableRegistros >thead').empty().append(opcion);




                //Traer los registros existentes
                //var parametros = new Object();
                //parametros.path = window.location.hostname;
                //parametros.idImpuesto = id;
                //parametros = JSON.stringify(parametros);
                //$.ajax({
                //    type: "POST",
                //    url: "../pages/Impuestos.aspx/GetListaValoresTabla",
                //    data: parametros,
                //    contentType: "application/json; charset=utf-8",
                //    dataType: "json",
                //    async: true,
                //    success: function (msg) {
                //        var html_trs = msg.d;

                //        //console.log(JSON.stringify(html_trs));

                //        $('#tableRegistros >tbody').empty().append(html_trs);



                //    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                //        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                //    }

                //});



        //    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        console.log(textStatus + ": " + XMLHttpRequest.responseText);
        //    }

        //});

    },

    accionesBotones: () => {



        $('#txtFileCSV').on('change', (e) => {
            e.preventDefault();

            console.log('txtFileCSV subir archivo csv');
            let file;
            if (file = $('#txtFileCSV')[0].files[0]) {
                upload.sendFile(file, 'csv');
            }
        });

        $('#btnUpload').on('click', (e) => {
            e.preventDefault();

            console.log('txtFileCSV subir archivo csv');
            let file;
            if (file = $('#txtFileCSV')[0].files[0]) {
                upload.sendFile(file, 'csv');
            } else {

                utils.toast(mensajesAlertas.errorSubirCsv, 'error');

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
            parametros.id = upload.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/uploads.aspx/Eliminar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        upload.cargarItems();

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

    upload.init();

    upload.accionesBotones();

});


