'use strict';
let date = new Date();
let descargas = "Empleados_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '8';


const POSICION_DIRECTOR = 1;
const POSICION_COORDINADOR = 2;
const POSICION_EJECUTIVO = 3;
const POSICION_SUPERVISOR = 4;
const POSICION_PROMOTOR = 5;

const employee = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        employee.idSeleccionado = "-1";
        employee.idTipoUsuario = "-1";
        employee.login = "-1";
        employee.accion = "";


        employee.loadComboPosicion();
        employee.loadComboPlaza();
        employee.loadComboModulo();
        employee.loadComboEmployeesByPosicion(POSICION_EJECUTIVO, '#comboEjecutivo');
        employee.loadComboEmployeesByPosicion(POSICION_SUPERVISOR, '#comboSupervisor');
        employee.loadComboEmployeesByPosicion(POSICION_COORDINADOR, '#comboCoordinador');
        employee.cargarItems();

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();
        $('.combo-coordinador').hide();

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
                        { data: 'FechaIngresoMx' },
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


    delete: (id) => {

        employee.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },



    /**
     * 
     * @param {any} idEmpleado
     * @param {any} idPosicion  - Tipo usuario / Puesto
     * @param {any} login       - Nombre de usuario
     */
    changePassword: (idEmpleado, idPosicion, login) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        $('#txtLoginP').val(login);
        $('#txtPassP').val('');
        employee.idSeleccionado = idEmpleado;
        employee.idTipoUsuario = idPosicion;
        employee.login = login;
        employee.accion = "changePassword";

        $('#panelEdicionPass').modal('show');

    },


    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#frm')[0].reset();


        let params = {};
        params.path = window.location.hostname;
        params.id = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetDataEmployee",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                //console.log(`${JSON.stringify(item.IdPlaza)}`);

                employee.idSeleccionado = item.IdEmpleado;

                //  empleado
                $('#txtNombre').val(item.Nombre);
                $('#txtPrimerApellido').val(item.PrimerApellido);
                $('#txtSegundoApellido').val(item.SegundoApellido);
                $('#txtCURP').val(item.CURP);
                $('#txtFechaIngreso').val(item.FechaIngreso);
                $('#txtFechaNacimiento').val(item.FechaNacimiento);
                $('#txtTelefono').val(item.Telefono);
                $('#txtMontoLimiteInicial').val(item.MontoLimiteInicial);
                $('#comboComisionInicial').val(item.IdComisionInicial);
                $('#comboPosicion').val(item.IdPosicion);
                $('#comboPlaza').val(item.IdPlaza);
                $('#comboEjecutivo').val(item.IdEjecutivo);
                $('#comboSupervisor').val(item.IdSupervisor);
                $('#comboCoordinador').val(item.IdCoordinador);

                //  dirección empleado
                $('#txtCalle').val(item.direccion.Calle);
                $('#txtColonia').val(item.direccion.Colonia);
                $('#txtMunicipio').val(item.direccion.Municipio);
                $('#txtEstado').val(item.direccion.Estado);
                $('#txtCodigoPostal').val(item.direccion.CodigoPostal);


                //datos aval
                $('#txtNombreAval').val(item.NombreAval);
                $('#txtPrimerApellidoAval').val(item.PrimerApellidoAval);
                $('#txtSegundoApellidoAval').val(item.SegundoApellidoAval);
                $('#txtCURPAval').val(item.CURPAval);
                $('#txtTelefonoAval').val(item.TelefonoAval);

                //  dirección aval
                $('#txtCalleAval').val(item.direccionAval.Calle);
                $('#txtColoniaAval').val(item.direccionAval.Colonia);
                $('#txtMunicipioAval').val(item.direccionAval.Municipio);
                $('#txtEstadoAval').val(item.direccionAval.Estado);
                $('#txtCodigoPostalAval').val(item.direccionAval.CodigoPostal);


                $('#txtNombreUsuario').val(item.usuario.Login);
                $('#txtPassword').val("0000000000");
                $('#txtPassword').prop('disabled', true);

                $('#panelTabla').hide();
                $('#panelForm').show();


                employee.accion = "editar";
                $('#spnTituloForm').text('Editar');


                if (Number(item.IdPosicion) === Number(POSICION_PROMOTOR)) {
                    $('.combo-supervisor').show();
                    $('.combo-ejecutivo').hide();
                    $('.combo-coordinador').hide();
                }
                else if (Number(item.IdPosicion) === Number(POSICION_SUPERVISOR)) {
                    $('.combo-ejecutivo').show();
                    $('.combo-supervisor').hide();
                    $('.combo-coordinador').hide();
                }
                else if (Number(item.IdPosicion) === Number(POSICION_EJECUTIVO)) {
                    $('.combo-coordinador').show();
                    $('.combo-ejecutivo').hide();
                    $('.combo-supervisor').hide();
                } else {
                    $('.combo-coordinador').hide();
                    $('.combo-supervisor').hide();
                    $('.combo-ejecutivo').hide();
                }


                employee.getDocument(employee.idSeleccionado, 1, '#img_1');
                employee.getDocument(employee.idSeleccionado, 2, '#img_2');
                employee.getDocument(employee.idSeleccionado, 3, '#img_3');
                employee.getDocument(employee.idSeleccionado, 4, '#img_4');
                employee.getDocument(employee.idSeleccionado, 5, '#img_5');
                employee.getDocument(employee.idSeleccionado, 6, '#img_6');
                employee.getDocument(employee.idSeleccionado, 7, '#img_7');
                employee.getDocument(employee.idSeleccionado, 8, '#img_8');



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },

    getDocument(idEmpleado, idTipoDocumento, idControl) {

        let params = {};
        params.path = window.location.hostname;
        params.idEmpleado = idEmpleado;
        params.idTipoDocumento = idTipoDocumento;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetDocument",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (foto) {

                let doc = foto.d;

                if (doc.IdDocumento) {
                    if (doc.Extension === 'png' || doc.Extension === 'jpg' || doc.Extension === 'jpeg' || doc.Extension === 'bmp') {
                        $(`${idControl}`).attr('src', `data:image/jpg;base64,${doc.Contenido}`);
                    } else if (doc.Extension === 'pdf') {
                        $(`${idControl}`).attr('src', '../../img/ico_pdf.png');
                    } else {
                        $(`${idControl}`).attr('src', '../../img/ico_doc.png');
                    }

                    $(`#href_${idTipoDocumento}`).css('cursor', 'pointer');
                } else {
                    $(`#href_${idTipoDocumento}`).css('cursor', 'default');
                }

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
        employee.accion = "nuevo";
        employee.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();
        $('.combo-coordinador').hide();
        $('#txtPassword').prop('disabled', false);

        //employee.testData();


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

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

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

                    opcion += `<option value = '${item.IdPosicion}' > ${item.Nombre}</option > `;

                }

                console.log('loadComboPosicion');

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

                    opcion += `<option value = '${item.IdPlaza}' > ${item.Nombre}</option > `;

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

                    opcion += `<option value = '${item.IdModulo}' > ${item.Nombre}</option > `;

                }

                $('#comboComisionInicial').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    accionesBotones: () => {

        $('.img-document').on('click', function (e) {
            e.preventDefault();

            let idTipoDocumento = e.currentTarget.dataset['tipo'];
            //let disabled = $(`#${e.currentTarget.id}`).prop('disabled');

            if (employee.accion !== 'nuevo') {

                let params = {};
                params.path = window.location.hostname;
                params.idEmpleado = employee.idSeleccionado;
                params.idTipoDocumento = idTipoDocumento;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/Employees.aspx/GetDocument",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (foto) {

                        let doc = foto.d;

                        if (doc.Contenido) {

                            let blob = null;
                            if (doc.Extension === 'png' || doc.Extension === 'jpg' || doc.Extension === 'jpeg' || doc.Extension === 'bmp') {
                                blob = utils.b64toBlob(doc.Contenido, 'data:image/png', 512);
                            } else if (doc.Extension === 'pdf') {
                                blob = utils.b64toBlob(doc.Contenido, 'data:document/pdf', 512);
                            }
                            else {
                                blob = utils.b64toBlob(doc.Contenido, '', 512);
                            }
                            if (blob) {
                                const blobUrl = URL.createObjectURL(blob);
                                const link = document.createElement('a');
                                link.href = blobUrl;
                                link.setAttribute('download', doc.Nombre + "." + doc.Extension);
                                document.body.appendChild(link);
                                link.click();
                                document.body.removeChild(link);
                            }

                        }
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });
            }

        });

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

            if (Number(valuePosition) === Number(POSICION_EJECUTIVO)) {
                let id = $('#comboCoordinador').val();
                if (!id) {
                    utils.toast(mensajesAlertas.errorSeleccionarCoordinador, 'error');

                    return;
                }
            }

            let dataEmployee = {};
            dataEmployee.IdEmpleado = employee.idSeleccionado;
            dataEmployee.FechaIngreso = $('#txtFechaIngreso').val();
            dataEmployee.IdPosicion = $('#comboPosicion').val();
            dataEmployee.IdSupervisor = $('#comboSupervisor').val() == null ? 0 : $('#comboSupervisor').val();
            dataEmployee.IdEjecutivo = $('#comboEjecutivo').val() == null ? 0 : $('#comboEjecutivo').val();
            dataEmployee.IdCoordinador = $('#comboCoordinador').val() == null ? 0 : $('#comboCoordinador').val();
            dataEmployee.CURP = $('#txtCURP').val();
            dataEmployee.FechaNacimiento = $('#txtFechaNacimiento').val();
            dataEmployee.PrimerApellido = $('#txtPrimerApellido').val();
            dataEmployee.SegundoApellido = $('#txtSegundoApellido').val();
            dataEmployee.Nombre = $('#txtNombre').val();

            dataEmployee.CURPAval = $('#txtCURPAval').val();
            dataEmployee.PrimerApellidoAval = $('#txtPrimerApellidoAval').val();
            dataEmployee.SegundoApellidoAval = $('#txtSegundoApellidoAval').val();
            dataEmployee.NombreAval = $('#txtNombreAval').val();
            dataEmployee.TelefonoAval = $('#txtTelefonoAval').val();

            if (!dataEmployee.IdSupervisor) dataEmployee.IdSupervisor = 0;
            if (!dataEmployee.IdEjecutivo) dataEmployee.IdEjecutivo = 0;

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
            dataAddressEmployee.CodigoPostal = $('#txtCodigoPostal').val();

            let dataAddressAval = {};
            dataAddressAval.IdEmpleado = employee.idSeleccionado;
            dataAddressAval.Calle = $('#txtCalleAval').val();
            dataAddressAval.Aval = 1;
            dataAddressAval.Colonia = $('#txtColoniaAval').val();
            dataAddressAval.Municipio = $('#txtMunicipioAval').val();
            dataAddressAval.Estado = $('#txtEstadoAval').val();
            dataAddressAval.CodigoPostal = $('#txtCodigoPostalAval').val();

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
            params.accion = employee.accion;
            params = JSON.stringify(params);

            let urlService = (employee.accion === 'nuevo') ? "Save" : "Update";

            $.ajax({
                type: "POST",
                url: `../../pages/Config/Employees.aspx/${urlService}`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;


                    //debugger;
                    if (valores.CodigoError == 0) {

                        if (employee.accion === 'nuevo') {

                            //guardar documentos
                            $('.campo-imagen').each(function (i, item) {

                                let idTipoDocumento = item.dataset['tipo'];

                                let file;
                                if (file = this.files[0]) {

                                    utils.sendFileEmployee(file, 'documento', valores.IdItem, idTipoDocumento);

                                }

                            });


                        }

                        let time_ = employee.accion === 'nuevo' ? 5000 : 100;

                        setTimeout(function () {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                            employee.init();


                        }, time_);


                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });


        $('.campo-imagen').on('change', function (e) {
            e.preventDefault();

            let idTipoDocumento = e.currentTarget.dataset['tipo'];

            if (employee.idSeleccionado !== "-1" && employee.accion !== 'nuevo') {


                let file;
                if (file = this.files[0]) {

                    utils.sendFileEmployee(file, 'update_document_employee', employee.idSeleccionado, idTipoDocumento);

                    setTimeout(function () {

                        //  Mostrar la imagen que se acaba de subir...
                        employee.getDocument(employee.idSeleccionado, idTipoDocumento, `#img_${idTipoDocumento}`);

                    }, 5000);

                }
            }


        });

        $('#comboPosicion').on('change', (e) => {
            e.preventDefault();

            //console.log('Change comboPosicion');
            let value = $('#comboPosicion').val();
            //console.log(`Value = ${ value } `);

            if (Number(value) === Number(POSICION_PROMOTOR)) {
                $('.combo-supervisor').show();
                $('.combo-ejecutivo').hide();
                $('.combo-coordinador').hide();
            }
            else if (Number(value) === Number(POSICION_SUPERVISOR)) {
                $('.combo-ejecutivo').show();
                $('.combo-supervisor').hide();
                $('.combo-coordinador').hide();
            }
            else if (Number(value) === Number(POSICION_EJECUTIVO)) {
                $('.combo-coordinador').show();
                $('.combo-ejecutivo').hide();
                $('.combo-supervisor').hide();
            } else {
                $('.combo-coordinador').hide();
                $('.combo-supervisor').hide();
                $('.combo-ejecutivo').hide();
            }

        });




        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $(`.documentos`).attr('src', '../../img/upload.png');


            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = window.location.hostname;
            params.id = employee.idSeleccionado;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Employees.aspx/Delete",
                data: params,
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


        $('#btnGuardarPassword').click(function (e) {
            e.preventDefault();


            let hasErrors = $('form[name="frmUsuarioP"]').validator('validate').has('.has-error').length;


            if (!hasErrors) {


                let params = {};
                params.path = window.location.hostname;
                params.newPassword = $('#txtPassP').val();
                params.idEmpleado = employee.idSeleccionado;
                params.login = employee.login;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params.accion = accion;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/Employees.aspx/ChangePassword",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        var resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoPasswordModificada, 'ok');

                            employee.cargarItems();

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


    }


}

window.addEventListener('load', () => {

    employee.init();

    employee.accionesBotones();

});


