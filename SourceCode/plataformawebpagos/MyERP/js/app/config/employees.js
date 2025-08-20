'use strict';

/*
  Archivo: /js/app/employee.js
  Página:  Employees.aspx (lista + formulario en la misma vista)
  Flujo:   Editar en la MISMA PÁGINA (sin navegar a NewEmployee.aspx)
*/

/* ========= Global usada por otros scripts (general.js) ========= */
var pagina = '8';

/* ========= Título para exportaciones ========= */
(function () {
    const d = new Date();
    const pad = n => String(n).padStart(2, '0');
    window.descargas = `Empleados_${d.getFullYear()}_${pad(d.getMonth() + 1)}_${pad(d.getDate())}_${d.getMilliseconds()}`;
})();

/* ========= Constantes de posiciones ========= */
const POSICION_DIRECTOR = 1;
const POSICION_COORDINADOR = 2;
const POSICION_EJECUTIVO = 3;
const POSICION_SUPERVISOR = 4;
const POSICION_PROMOTOR = 5;

/* ========= Endpoints EXACTOS según tu Employees.aspx.cs ========= */
const API = {
    lista: "../../pages/Config/Employees.aspx/GetListaItems",
    empleado: "../../pages/Config/Employees.aspx/GetDataEmployee",
    eliminar: "../../pages/Config/Employees.aspx/Delete",
    doc: "../../pages/Config/Employees.aspx/GetDocument",
    posiciones: "../../pages/Config/Employees.aspx/GetListaItemsPosiciones",
    plazas: "../../pages/Config/Employees.aspx/GetListaItemsPlazas",
    comisiones: "../../pages/Config/Employees.aspx/GetListaItemsComisiones"
};

/* ========= Clave de conexión (como venías usando) ========= */
const PATH_KEY = "connbd";

const employee = {
    idSeleccionado: -1,
    accion: null,

    /* ====== Init ====== */
    init: () => {
        employee.loadComboPosicion();   // combos de BÚSQUEDA
        employee.loadComboPlaza();      // combos de BÚSQUEDA
        employee.loadComboComision();   // combos de BÚSQUEDA
        employee.cargarItems();         // DataTable
    },

    /* ====== Filtros de búsqueda (coinciden con ResponseGridEmpleados) ====== */
    getFilter: () => {
        const fechaStr = $("#dtpFechaIngresoBusqueda").val();
        const fechaValida = moment(fechaStr).isValid();

        const o = {
            Activo: $("#cboStatusBusqueda option:selected").val(),
            IdPlaza: $("#cboPlazaBusqueda option:selected").val(),
            IdTipo: $("#cboTipoBusqueda option:selected").val(),
            IdModulo: $("#cboModuloBusqueda option:selected").val(),
            NombreEjecutivo: $("#txtEjecutivoBusqueda").val(),
            NombreSupervisor: $("#txtSupervisorBusqueda").val(),
            NombreCompleto: $("#txtNombreBusqueda").val(),
            Usuario: $("#txtUsuarioBusqueda").val(),
            // ASP.NET puede parsear "YYYY-MM-DD" a DateTime
            FechaIngreso: fechaValida ? moment(fechaStr).format('YYYY-MM-DD') : null
        };

        o.IdPlaza = $.isNumeric(o.IdPlaza) ? parseInt(o.IdPlaza, 10) : null;
        o.IdTipo = $.isNumeric(o.IdTipo) ? parseInt(o.IdTipo, 10) : null;
        o.IdModulo = $.isNumeric(o.IdModulo) ? parseInt(o.IdModulo, 10) : null;
        o.Activo = $.isNumeric(o.Activo) ? parseInt(o.Activo, 10) : null;

        return o;
    },

    /* ====== DataTable ====== */
    cargarItems: () => {
        const idUsuario = document.getElementById('txtIdUsuario')?.value || "";
        const payload = JSON.stringify({
            path: PATH_KEY,
            idUsuario: idUsuario,
            Filtro: employee.getFilter()
        });

        $.ajax({
            type: "POST",
            url: API.lista,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((msg) => {
            const data = msg.d;

            // si no tiene permisos (server retorna null)
            if (!data) {
                window.location = "../../pages/Index.aspx";
                return;
            }

            $('#table').DataTable({
                destroy: true,
                processing: true,
                order: [],
                searching: false,
                pagingType: 'full_numbers',
                bLengthChange: false,
                data: data,
                columns: [
                    { data: 'NombreCompleto' },
                    { data: 'Usuario', className: 'text-center' },
                    { data: 'Modulo', className: 'text-center' },
                    { data: 'Tipo', className: 'text-center' },
                    { data: 'Plaza', className: 'text-center' },
                    { data: 'NombreSupervisor' },
                    { data: 'NombreEjecutivo' },
                    {
                        data: 'FechaIngreso', className: 'text-center',
                        render: (d, t, row) => moment(row.FechaIngreso).format('YYYY-MM-DD')
                    },
                    {
                        data: 'Activo', className: 'text-center',
                        render: (d, t, row) => row.Activo === 1
                            ? "<span class='badge badge-success rounded p-2'>Activo</span>"
                            : "<span class='badge badge-warning rounded p-2'>Baja</span>"
                    },
                    {
                        data: '', className: 'text-center',
                        render: (d, t, row) => `
              <a onclick="employee.edit(${row.IdEmpleado})" class="text-white rounded btn btn-primary">
                <span class="fa fa-edit mr-1"></span>
              </a>
              <a onclick="employee.delete(${row.IdEmpleado})" class="text-white rounded btn btn-danger">
                <span class="fa fa-remove mr-1"></span>
              </a>`
                    }
                ],
                language: textosEsp,
                columnDefs: [{ targets: [-1], orderable: false }],
                dom: 'frtiplB',
                buttons: [
                    { extend: 'excelHtml5', title: window.descargas, text: 'Xls', className: 'excelbtn' },
                    { extend: 'pdfHtml5', title: window.descargas, text: 'Pdf', className: 'pdfbtn' }
                ]
            });
        }).fail((xhr, textStatus) => {
            console.log(textStatus + ": " + xhr.responseText);
        });
    },

    /* ====== Editar EN LA MISMA PÁGINA ====== */
    edit: (id) => {
        if (!id) return;
        window.location.href = '/pages/Config/NewEmployee.aspx?id=' + encodeURIComponent(id);
    },

    /* ====== Mostrar/ocultar combos por posición ====== */
    toggleCombosPorPosicion: (idPos) => {
        idPos = Number(idPos);
        if (idPos === POSICION_PROMOTOR) {
            $('.combo-supervisor').show(); $('.combo-ejecutivo, .combo-coordinador').hide();
        } else if (idPos === POSICION_SUPERVISOR) {
            $('.combo-ejecutivo').show(); $('.combo-supervisor, .combo-coordinador').hide();
        } else if (idPos === POSICION_EJECUTIVO) {
            $('.combo-coordinador').show(); $('.combo-ejecutivo, .combo-supervisor').hide();
        } else {
            $('.combo-coordinador, .combo-supervisor, .combo-ejecutivo').hide();
        }
    },

    /* ====== Documento / Imagen ====== */
    getDocument: (idEmpleado, idTipoDocumento, idControl) => {
        const payload = JSON.stringify({
            path: PATH_KEY, idEmpleado: String(idEmpleado), idTipoDocumento: String(idTipoDocumento)
        });

        $.ajax({
            type: "POST",
            url: API.doc,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((foto) => {
            const doc = foto.d || {};
            const $img = $(idControl);
            const $href = $(`#href_${idTipoDocumento}`);

            if (!doc.IdDocumento) {
                $href.css('cursor', 'default');
                return;
            }

            const ext = (doc.Extension || '').toLowerCase().replace(/^\./, '');
            if (['png', 'jpg', 'jpeg', 'bmp', 'webp'].includes(ext)) {
                $img.attr('src', `data:image/${ext || 'jpg'};base64,${doc.Contenido}`);
            } else if (ext === 'pdf') {
                $img.attr('src', '../../img/ico_pdf.png');
            } else {
                $img.attr('src', '../../img/ico_doc.png');
            }
            $href.css('cursor', 'pointer');
        }).fail((xhr, textStatus) => {
            console.log(textStatus + ": " + xhr.responseText);
        });
    },

    /* ====== Eliminar ====== */
    delete: (id) => {
        employee.idSeleccionado = id;
        $('#panelEliminar').modal('show');
    },

    /* ====== Combos BÚSQUEDA ====== */
    loadComboPosicion: () => {
        const payload = JSON.stringify({ path: PATH_KEY });

        return $.ajax({
            type: "POST",
            url: API.posiciones,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((msg) => {
            const items = msg.d || [];
            let html = '<option value="">Seleccione...</option>';
            for (const it of items) html += `<option value='${it.IdPosicion}'>${it.Nombre}</option>`;
            $('#cboTipoBusqueda').html(html);
        }).fail((xhr, t) => console.log(t + ": " + xhr.responseText));
    },

    loadComboPlaza: () => {
        const payload = JSON.stringify({ path: PATH_KEY });

        return $.ajax({
            type: "POST",
            url: API.plazas,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((msg) => {
            const items = msg.d || [];
            let html = '<option value="">Seleccione...</option>';
            for (const it of items) html += `<option value='${it.IdPlaza}'>${it.Nombre}</option>`;
            $('#cboPlazaBusqueda').html(html);
        }).fail((xhr, t) => console.log(t + ": " + xhr.responseText));
    },

    loadComboComision: () => {
        const payload = JSON.stringify({ path: PATH_KEY });

        return $.ajax({
            type: "POST",
            url: API.comisiones,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((msg) => {
            const items = msg.d || [];
            let html = '<option value="">Seleccione...</option>';
            for (const it of items) html += `<option value='${it.IdComision}'>${it.Nombre}</option>`;
            $('#cboModuloBusqueda').html(html);
        }).fail((xhr, t) => console.log(t + ": " + xhr.responseText));
    },

    /* ====== Combos del FORM (mismos WebMethods) ====== */
    loadFormComboPosicion: (selector) => {
        const payload = JSON.stringify({ path: PATH_KEY });

        return $.ajax({
            type: "POST",
            url: API.posiciones,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((msg) => {
            const items = msg.d || [];
            let html = '<option value="">Seleccione...</option>';
            for (const it of items) html += `<option value='${it.IdPosicion}'>${it.Nombre}</option>`;
            $(selector).html(html);
        }).fail(() => $(selector).html('<option value="">Seleccione...</option>'));
    },

    loadFormComboPlaza: (selector) => {
        const payload = JSON.stringify({ path: PATH_KEY });

        return $.ajax({
            type: "POST",
            url: API.plazas,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((msg) => {
            const items = msg.d || [];
            let html = '<option value="">Seleccione...</option>';
            for (const it of items) html += `<option value='${it.IdPlaza}'>${it.Nombre}</option>`;
            $(selector).html(html);
        }).fail(() => $(selector).html('<option value="">Seleccione...</option>'));
    },

    loadFormComboComision: (selector) => {
        const payload = JSON.stringify({ path: PATH_KEY });

        return $.ajax({
            type: "POST",
            url: API.comisiones,
            data: payload,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true
        }).done((msg) => {
            const items = msg.d || [];
            let html = '<option value="">Seleccione...</option>';
            for (const it of items) html += `<option value='${it.IdComision}'>${it.Nombre}</option>`;
            $(selector).html(html);
        }).fail(() => $(selector).html('<option value="">Seleccione...</option>'));
    },

    /* ====== Botones ====== */
    accionesBotones: () => {
        $('#comboPosicion').on('change', () => {
            employee.toggleCombosPorPosicion($('#comboPosicion').val());
        });

        $('#btnLimpiar').on('click', (e) => {
            e.preventDefault();
            $('#frmFiltros')[0]?.reset();
        });

        $('#btnBuscar').on('click', (e) => {
            e.preventDefault();
            employee.cargarItems();
        });

        $('#btnEliminarAceptar').on('click', () => {
            const payload = JSON.stringify({ path: PATH_KEY, id: String(employee.idSeleccionado) });

            $.ajax({
                type: "POST",
                url: API.eliminar,
                data: payload,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true
            }).done((msg) => {
                const r = msg.d;
                if (!r.MensajeError) {
                    utils.toast(mensajesAlertas.exitoEliminar, 'ok');
                    employee.cargarItems();
                } else {
                    utils.toast(r.MensajeError, 'error');
                }
            }).fail((xhr, t) => console.log(t + ": " + xhr.responseText));
        });
    }
};

/* ====== Bootstrap ====== */
window.addEventListener('load', () => {
    employee.init();
    employee.accionesBotones();
});
