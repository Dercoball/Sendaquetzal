'use strict';
let date = new Date();
let descargas = "Empleados_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '8';


const empleado = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        empleado.idSeleccionado = "-1";
        empleado.accion = "";


        empleado.loadComboPosicion();
        empleado.loadComboPlaza();
        empleado.loadComboModulo();
        empleado.loadComboEmployeesByPosicion(POSICION_EJECUTIVO, '#comboEjecutivo');
        empleado.loadComboEmployeesByPosicion(POSICION_SUPERVISOR, '#comboSupervisor');
        empleado.cargarItems();

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();

    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItems",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;

                //  si no tiene permisos
                if (data == null) {
                    window.location = "../../pages/Index.aspx";
                }

                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'IdEmpleado' },
                        { data: 'NombreCompleto' },
                        { data: 'NombreUsuario' },
                        { data: 'NombreModulo' },
                        { data: 'NombreTipoUsuario' },
                        { data: 'NombrePlaza' },
                        { data: 'FechaIngreso' },
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
            url: "../../pages/Config/Employees.aspx/GetItem",
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
                    url: "../../pages/Config/Employees.aspx/GetFoto",
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

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();


    },


    loadComboEmployeesByPosicion: (idTipoEmpleado, control) => {

        var params = {};
        params.path = window.location.hostname;
        params.idTipoEmpleado = idTipoEmpleado;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsEmpleadoByPosicion",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value='${item.IdEmpleado}'>${item.Nombre}</option>`;

                }

                $(`${control}`).html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    loadComboPosicion: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsPosiciones",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value='${item.IdPosicion}'>${item.Nombre}</option>`;

                }

                $('#comboPosicion').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    loadComboPlaza: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsPlazas",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value='${item.IdPlaza}'>${item.Nombre}</option>`;

                }

                $('#comboPlaza').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    loadComboModulo: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsModulos",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value='${item.IdModulo}'>${item.Nombre}</option>`;

                }

                $('#comboComisionInicial').html(opcion);

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

            let hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (hasErrors) {
                return;
            }

            let dataEmployee = {};
            dataEmployee.IdEmpleado = empleado.idSeleccionado;
            dataEmployee.FechaIngreso = $('#txtFechaIngreso').val();
            dataEmployee.IdPosicion = $('#comboPosicion').val();
            dataEmployee.IdSupervisor = $('#comboSupervisor').val();
            dataEmployee.IdEjecutivo = $('#comboEjecutivo').val();
            dataEmployee.CURP = $('#txtCURP').val();
            dataEmployee.FechaNacimiento = $('#txtFechaNacimiento').val();
            dataEmployee.PrimerApellido = $('#txtPrimerApellido').val();
            dataEmployee.SegundoApellido = $('#txtSegundoApellido').val();
            dataEmployee.Nombre = $('#txtNombre').val();

            dataEmployee.IdDepartamento = $('#comboDepartamento').val();
            dataEmployee.IdPlaza= $('#comboPlaza').val();
            
            dataEmployee.Clave = $('#txtClave').val();


            //let dataEmployee = {};
            //dataEmployee.IdEmpleado = empleado.idSeleccionado;
            //dataEmployee.Nombre = $('#txtNombre').val();
            //dataEmployee.IdDepartamento = $('#comboDepartamento').val();
            //dataEmployee.IdPosicion = $('#comboPosicion').val();
            //dataEmployee.IdPlaza = $('#comboPlaza').val();
            //dataEmployee.IdSupervisor = $('#comboSupervisor').val();
            //dataEmployee.IdEjecutivo = $('#comboEjecutivo').val();
            //dataEmployee.PrimerApellido = $('#txtPrimerApellido').val();
            //dataEmployee.SegundoApellido = $('#txtSegundoApellido').val();
            //dataEmployee.Clave = $('#txtClave').val();



            let params = {};
            params.path = window.location.hostname;
            params.item = dataEmployee;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.accion = empleado.acccion;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Employees.aspx/Save",
                data: params,
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
                            url: "../../pages/Config/Employees.aspx/GetFoto",
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

        $('#comboPosicion').on('change', (e) => {
            e.preventDefault();

            //console.log('Change comboPosicion');
            let value = $('#comboPosicion').val();
            //console.log(`Value = ${value}`);

            if (Number(value) === Number(POSICION_PROMOTOR)) {
                $('.combo-supervisor').show();
                $('.combo-ejecutivo').hide();
            }
            else if (Number(value) === Number(POSICION_SUPERVISOR)) {
                $('.combo-supervisor').hide();
                $('.combo-ejecutivo').show();
            } else {
                $('.combo-supervisor').hide();
                $('.combo-ejecutivo').hide();
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
                url: "../../pages/Config/Employees.aspx/Eliminar",
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


