'use strict';
let date = new Date();
let descargas = "Empleados_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '8';

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
        params.path = window.location.hostname;
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
    delete: (id) => {
        employee.idSeleccionado = id;
        $('#panelEliminar').modal('show');
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

                $('#cboTipoBusqueda').html(opcion);
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

                $('#cboPlazaBusqueda').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },
    loadComboComision: () => {
        var params = {};
        params.path = window.location.hostname;
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
    }
}

window.addEventListener('load', () => {
    employee.init();
    employee.accionesBotones();
});


