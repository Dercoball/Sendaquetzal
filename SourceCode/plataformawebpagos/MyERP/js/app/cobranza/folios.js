'use strict';

let pagina = '92';
let dataTable;

const folios = {

    init: () => {
        folios.loadGestores();
        folios.cargar();

        $('#btnAsignar').on('click', function (e) {
            e.preventDefault();
            folios.asignar();
        });

        $('#btnFiltrar').on('click', function (e) {
            e.preventDefault();
            folios.cargar();
        });
    },

    loadGestores: () => {
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value
        });
        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Folios.aspx/GetGestores',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const sel = document.getElementById('cmbGestor');
                const selF = document.getElementById('cmbGestorFiltro');
                sel.innerHTML = '';
                document.querySelectorAll('#cmbGestorFiltro option').forEach(o => o.remove());
                selF.add(new Option('Todos', '0', true, true));
                (msg.d || []).forEach(g => {
                    sel.add(new Option(g.NombreCompleto, g.IdEmpleado, false, false));
                    selF.add(new Option(g.NombreCompleto, g.IdEmpleado, false, false));
                });
            }
        });
    },

    cargar: () => {
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idGestor: parseInt(document.getElementById('cmbGestorFiltro').value || '0', 10)
        });
        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Folios.aspx/GetFolios',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const data = msg.d;
                if (data == null) {
                    folios.mostrarMensaje('No tiene permisos.');
                    return;
                }
                folios.pintar(data);
            }
        });
    },

    pintar: (data) => {
        if ($.fn.dataTable.isDataTable('#table')) {
            dataTable.clear().destroy();
        }
        const rows = data.map(f => {
            const badge = f.Estatus === 2 ? 'badge-success' : (f.Estatus === 3 ? 'badge-danger' : 'badge-info');
            return [
                f.Folio,
                f.Gestor,
                '<span class="badge ' + badge + '">' + f.EstatusStr + '</span>',
                f.FechaAsignacion,
                f.FechaUso
            ];
        });
        dataTable = $('#table').DataTable({
            data: rows,
            language: (typeof textosEsp !== 'undefined') ? textosEsp : undefined,
            order: [[0, 'asc']]
        });
    },

    asignar: () => {
        const idGestor = parseInt(document.getElementById('cmbGestor').value || '0', 10);
        const desde = parseInt(document.getElementById('txtDesde').value || '0', 10);
        const hasta = parseInt(document.getElementById('txtHasta').value || '0', 10);
        if (idGestor <= 0) { folios.mostrarMensaje('Seleccione un gestor.'); return; }
        if (!(hasta >= desde) || desde <= 0) { folios.mostrarMensaje('Rango de folios inválido.'); return; }

        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idGestor: idGestor,
            prefijo: document.getElementById('txtPrefijo').value,
            desde: desde,
            hasta: hasta
        });
        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Folios.aspx/AsignarFolios',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const res = msg.d;
                if (res && res.CodigoError === 0) {
                    folios.cargar();
                    if (typeof toastr !== 'undefined') toastr.success(res.IdItem + ' folios asignados.');
                } else {
                    folios.mostrarMensaje((res && res.MensajeError) || 'No se pudo asignar.');
                }
            },
            error: function (xhr, textStatus) {
                console.log(textStatus + ': ' + xhr.responseText);
                folios.mostrarMensaje('Ocurrió un error al asignar folios.');
            }
        });
    },

    mostrarMensaje: (texto) => {
        document.getElementById('spnMensajes').textContent = texto;
        $('#panelMensajes').modal('show');
    }
};

window.addEventListener('load', () => {
    folios.init();
});
