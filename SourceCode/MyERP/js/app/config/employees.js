'use strict';
let date = new Date();
let descargas = "Empleados_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '8';


const employee = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        employee.idSeleccionado = "-1";
        employee.accion = "";


        employee.loadComboPosicion();
        employee.loadComboPlaza();
        employee.loadComboModulo();
        employee.loadComboEmployeesByPosicion(POSICION_EJECUTIVO, '#comboEjecutivo');
        employee.loadComboEmployeesByPosicion(POSICION_SUPERVISOR, '#comboSupervisor');
        employee.cargarItems();

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
                        { data: 'Login' },
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

        employee.idSeleccionado = id;

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
                employee.idSeleccionado = item.IdEmpleado;

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


                employee.acccion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);
                $('#img_').attr('src', `../img/logo_small.jpg`);


                var parametros = new Object();
                parametros.path = window.location.hostname;
                parametros.idEmpleado = employee.idSeleccionado;
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
        employee.acccion = "nuevo";
        employee.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();

        employee.testData();


    },

    testData() {
        $('.campo-combo').val(2);
        $('.campo-date').val('2022-01-01');
        $('.campo-input').val(7);

        //$('#txtFechaNacimiento').val('2000-01-01');

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

            employee.nuevo();

        });


        $('#btnGuardar').on('click', (e) => {
            e.preventDefault();

            let hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (hasErrors) {
                return;
            }


            let valuePosition = $('#comboPosicion').val();
            if (Number(valuePosition) === Number(POSICION_PROMOTOR)) {
                let id = $('#comboSupervisor').val();
                if (!id) {
                    utils.toast(mensajesAlertas.errorSeleccionarSupervisor, 'error');

                    return;
                }

            }

            if (Number(valuePosition) === Number(POSICION_SUPERVISOR)) {
                let id = $('#comboEjecutivo').val();
                if (!id) {
                    utils.toast(mensajesAlertas.errorSeleccionarEjecutivo, 'error');

                    return;
                }
            }

            let dataEmployee = {};
            dataEmployee.IdEmpleado = employee.idSeleccionado;
            dataEmployee.FechaIngreso = $('#txtFechaIngreso').val();
            dataEmployee.IdPosicion = $('#comboPosicion').val();
            dataEmployee.IdSupervisor = $('#comboSupervisor').val() == null ? 0 : $('#comboSupervisor').val();
            dataEmployee.IdEjecutivo = $('#comboEjecutivo').val() == null ? 0 : $('#comboEjecutivo').val();
            dataEmployee.CURP = $('#txtCURP').val();
            dataEmployee.FechaNacimiento = $('#txtFechaNacimiento').val();
            dataEmployee.PrimerApellido = $('#txtPrimerApellido').val();                       
            dataEmployee.SegundoApellido = $('#txtSegundoApellido').val();
            dataEmployee.Nombre = $('#txtNombre').val();

            dataEmployee.CURPAval = $('#txtCURPAval').val();
            dataEmployee.PrimerApellidoAval = $('#txtPrimerApellidoAval').val();
            dataEmployee.SegundoApellidoAval = $('#txtSegundoApellidoAval').val();
            dataEmployee.NombreAval = $('#txtNombreAval').val();


            dataEmployee.IdPlaza = $('#comboPlaza').val();
            dataEmployee.IdComisionInicial = $('#comboComisionInicial').val();
            dataEmployee.Telefono = $('#txtTelefono').val();
            dataEmployee.MontoLimiteInicial = $('#txtMontoLimiteInicial').val();


            let dataAddressEmployee = {};
            dataAddressEmployee.IdEmpleado = employee.idSeleccionado;
            dataAddressEmployee.Calle = $('#txtCalle').val();
            dataAddressEmployee.Aval = 0;
            dataAddressEmployee.Colonia = $('#txtColonia').val();
            dataAddressEmployee.Municipio = $('#txtMunicipio').val();
            dataAddressEmployee.Estado = $('#txtEstado').val();
            dataAddressEmployee.CP = $('#txtCP').val();

            let dataAddressAval = {};
            dataAddressAval.IdEmpleado = employee.idSeleccionado;
            dataAddressAval.Calle = $('#txtCalleAval').val();
            dataAddressAval.Aval = 1;
            dataAddressAval.Colonia = $('#txtColoniaAval').val();
            dataAddressAval.Municipio = $('#txtMunicipioAval').val();
            dataAddressAval.Estado = $('#txtEstadoAval').val();
            dataAddressAval.CP = $('#txtCPAval').val();

            let dataUser = {};
            dataUser.IdEmpleado = employee.idSeleccionado;
            dataUser.Login = $('#txtNombreUsuario').val();
            dataUser.Password = $('#txtPassword').val();
            dataUser.IdTipoUsuario = $('#comboPosicion').val();


            let params = {};
            params.path = window.location.hostname;
            params.item = dataEmployee;
            params.itemAddress = dataAddressEmployee;
            params.itemAddressAval = dataAddressAval;
            params.itemUser = dataUser;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.accion = employee.acccion;
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
                            //$('.file-fotografia').each(function (documento) {

                            //    let file;
                            //    if (file = this.files[0]) {

                            //        utils.sendFile(file, 'fotografia_empleado', valores.IdItem, 'fotografia_empleado');

                            //    }

                            //});
                        }


                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                        $('#panelTabla').show();
                        $('#panelForm').hide();

                        employee.cargarItems();

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

            if (employee.idSeleccionado !== "-1") {


                //debugger;
                let file;
                if (file = this.files[0]) {

                    utils.sendFile(file, 'fotografia_empleado', employee.idSeleccionado, 'fotografia_empleado');

                    setTimeout(function () {

                        //  Mostrar la imagen que se acaba de subir...
                        var parametros = new Object();
                        parametros.path = window.location.hostname;
                        parametros.idEmpleado = employee.idSeleccionado;
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
            parametros.id = employee.idSeleccionado;
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


                        employee.cargarItems();

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

    employee.init();

    employee.accionesBotones();

});


