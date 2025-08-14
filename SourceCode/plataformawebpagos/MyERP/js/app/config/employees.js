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
        employee.loadComboPosicion();
        employee.loadComboPlaza();
        employee.loadComboComision();
        employee.cargarItems();
    },
    getFilter: () => {
        var oRequest =
        {
            Activo: $("#cboStatusBusqueda option:selected").val(),
            IdPlaza: $("#cboPlazaBusqueda option:selected").val(),
            IdTipo: $("#cboTipoBusqueda option:selected").val(),
            IdModulo: $("#cboModuloBusqueda option:selected").val(),
            NombreEjecutivo: $("#txtEjecutivoBusqueda").val(),
            NombreSupervisor: $("#txtSupervisorBusqueda").val(),
            NombreCompleto: $("#txtNombreBusqueda").val(),
            Usuario: $("#txtUsuarioBusqueda").val(),
            FechaIngreso: moment($("#dtpFechaIngresoBusqueda").val()).isValid()
                ? moment($("#dtpFechaIngresoBusqueda").val()).format('YYYY-MM-DD')
                : null
        };

        oRequest.IdPlaza = $.isNumeric(oRequest.IdPlaza) ? parseInt(oRequest.IdPlaza) :null;
        oRequest.IdTipo = $.isNumeric(oRequest.IdTipo) ? parseInt(oRequest.IdTipo)  : null;
        oRequest.IdModulo = $.isNumeric(oRequest.IdModulo) ? parseInt(oRequest.IdModulo)  :null ;
        oRequest.Activo = $.isNumeric(oRequest.Activo) ? parseInt(oRequest.Activo) :null ;

        return oRequest;
    },
    cargarItems: () => {
        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.Filtro = employee.getFilter();
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
                    destroy: true,
                    processing: true,
                    order: [],
                    searching: false,
                    pagingType: 'full_numbers',
                    bLengthChange: false,
                    data: data,
                    columns: [
                        { data: 'NombreCompleto' },
                        { data: 'Usuario', className:'text-center' },
                        { data: 'Modulo', className: 'text-center' },
                        { data: 'Tipo', className: 'text-center' },
                        { data: 'Plaza', className: 'text-center' },
                        { data: 'NombreSupervisor' },
                        { data: 'NombreEjecutivo' },
                        {
                            data: 'FechaIngreso', className: 'text-center', render: function (data, type, row) {
                                return moment(row.FechaIngreso).format('YYYY-MM-DD');
                            }
                        },
                        {
                            data: 'Activo', className: 'text-center', render: function (data, type, row) {
                                return row.Activo === 1
                                    ? "<span class='badge badge-success rounded p-2'>Activo</span>"
                                    : "<span class='badge badge-warning rounded p-2'>Baja</span>";
                            }
                        },
                        {
                            data: '', className: 'text-center', render: function (data, type, row) {
                                return "<a onclick='employee.edit(" + row.IdEmpleado + ")' class='text-white rounded btn btn-primary'> <span class='fa fa-edit mr-1'></span></a>" +
                                    " <a  onclick='employee.delete(" + row.IdEmpleado + ")'  class='text-white rounded btn btn-danger'> <span class='fa fa-remove mr-1'></span></a>";
                            }
                        }
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
    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();


        let params = {};
        params.path = "connbd";
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
                $('#txtCalle').val(item.Direccion.Calle);
                $('#txtColonia').val(item.Direccion.Colonia);
                $('#txtMunicipio').val(item.Direccion.Municipio);
                $('#txtEstado').val(item.Direccion.Estado);
                $('#txtCodigoPostal').val(item.Direccion.CodigoPostal);


                //datos aval
                $('#txtNombreAval').val(item.NombreAval);
                $('#txtPrimerApellidoAval').val(item.PrimerApellidoAval);
                $('#txtSegundoApellidoAval').val(item.SegundoApellidoAval);
                $('#txtCURPAval').val(item.CURPAval);
                $('#txtTelefonoAval').val(item.TelefonoAval);

                //  dirección aval
                $('#txtCalleAval').val(item.DireccionAval.Calle);
                $('#txtColoniaAval').val(item.DireccionAval.Colonia);
                $('#txtMunicipioAval').val(item.DireccionAval.Municipio);
                $('#txtEstadoAval').val(item.DireccionAval.Estado);
                $('#txtCodigoPostalAval').val(item.DireccionAval.CodigoPostal);


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
        params.path = "connbd";
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
    delete: (id) => {
        employee.idSeleccionado = id;
        $('#panelEliminar').modal('show');
    },
    loadComboPosicion: () => {
        var params = {};
        params.path = "connbd";
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

                $('#cboTipoBusqueda').html(opcion);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },
    loadComboPlaza: () => {

        var params = {};
        params.path = "connbd";
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

                $('#cboPlazaBusqueda').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },
    loadComboComision: () => {
        var params = {};
        params.path = "connbd";
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsComisiones",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdComision}' > ${item.Nombre}</option > `;

                }

                $('#cboModuloBusqueda').html(opcion);

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
    accionesBotones: () => {
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

        $('#btnLimpiar').on('click', (e) => {
            e.preventDefault();
            $('#frmFiltros')[0].reset();
        });

        $('#btnBuscar').on('click', (e) => {
            e.preventDefault();
            employee.cargarItems();
        });

        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = "connbd";
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
    }
}

window.addEventListener('load', () => {
    employee.init();
    employee.accionesBotones();
});


