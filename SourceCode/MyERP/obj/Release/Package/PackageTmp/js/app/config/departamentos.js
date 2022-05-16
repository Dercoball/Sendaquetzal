'use strict';
let date = new Date();
let descargas = "Departamento_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '61';



const departamento = {


    init: () => {
        departamento.idSeleccionado = "-1";
        departamento.accion = "";

        $('#panelTabla').show();
        $('#panelForm').hide();


        departamento.cargarItems();

    },

    cargarItems: () => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/Departamentos.aspx/GetListaItems';

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
                        { data: 'IdDepartamento' },
                        { data: 'Nombre' },
                        { data: 'Clave' },
                        { data: 'NombreResidente' },
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

        departamento.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },

    asignar: (idObra, idResidente) => {
        departamento.idSeleccionado = idObra;

        console.log('Asignar');
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/Departamentos.aspx/GetListaResidentes';

        utils.postData(url, parametros)
            .then(data => {

                //console.log(data);

                var datos = JSON.parse(data.d);
                var dataAdapter = new $.jqx.dataAdapter(datos);


                $("#comboResidente").jqxDropDownList({
                    source: dataAdapter, displayMember: "NombreCompleto", valueMember: "IdEmpleado", width: '350px',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboResidente").jqxDropDownList('clearSelection');

                if (idResidente) {
                    $("#comboResidente").val(idResidente);
                } else {
                    $("#comboResidente").val(-1);

                }



                //data = data.d;
                //let op = '';
                //for (let i = 0; i < data.length; i++) {
                //    let item = data[i];

                //    op += '<option value = "' + item.IdEmpleado + '">' + item.NombreCompleto + '</option>';
                //}

                //$('#comboResidente').empty().append(op);

                $('#panelSeleccionarResidente').modal('show');

            });



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
            url: "../pages/Departamentos.aspx/GetItem",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                departamento.idSeleccionado = item.IdDepartamento;

                console.log('.');
                $('#txtNombre').val(item.Nombre);
                $('#txtClave').val(item.Clave);
                $('#chkActivo').prop('checked', item.Activo === 1);

                $('#panelTabla').hide();
                $('#panelForm').show();


                departamento.accion = "editar";
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
        departamento.accion = "nuevo";
        departamento.idSeleccionado = "-1";

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFechaCreacion');



    },



    accionesBotones: () => {






        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            departamento.nuevo();

        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }




            var item = new Object();
            item.IdDepartamento = departamento.idSeleccionado;
            item.Activo = $('#chkActivo').prop('checked') == true ? 1 : 0;
            item.Nombre = $('#txtNombre').val();
            item.Clave = $('#txtClave').val();




            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.accion = departamento.accion;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Departamentos.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError == 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        departamento.cargarItems();

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });

        $('#btnAceptarSeleccionResidente').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idObra = departamento.idSeleccionado;
            parametros.idResidente = $('#comboResidente').val();
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Departamentos.aspx/UpdateResidente",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                        departamento.cargarItems();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });



        $('#btnEliminarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = departamento.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Departamentos.aspx/Eliminar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        departamento.cargarItems();

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

    departamento.init();

    departamento.accionesBotones();

});


