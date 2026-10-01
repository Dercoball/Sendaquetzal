'use strict';

let pagina = '93';
let dataTable;
let dataTableResumen;

const reporte = {

    init: () => {
        reporte.loadComboPlaza();
        reporte.loadGestores();

        $('#btnFiltrar').on('click', function (e) {
            e.preventDefault();
            reporte.cargar();
        });

        reporte.cargar();
    },

    loadComboPlaza: () => {
        const params = JSON.stringify({ path: 'connbd' });
        $.ajax({
            type: 'POST',
            url: '../../pages/Customers/Customers.aspx/GetListaPlazas',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const selectEl = document.getElementById('cmbPlaza');
                document.querySelectorAll('#cmbPlaza option').forEach(o => o.remove());
                selectEl.add(new Option('Todos', '0', true, true));
                (msg.d || []).forEach(item => selectEl.add(new Option(item.Nombre, item.IdPlaza, false, false)));
            }
        });
    },

    loadGestores: () => {
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idPlaza: 0
        });
        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Cartera.aspx/GetGestores',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const selectEl = document.getElementById('cmbGestor');
                document.querySelectorAll('#cmbGestor option').forEach(o => o.remove());
                selectEl.add(new Option('Todos', '0', true, true));
                (msg.d || []).forEach(g => selectEl.add(new Option(g.NombreCompleto, g.IdEmpleado, false, false)));
            }
        });
    },

    cargar: () => {
        const base = {
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idPlaza: parseInt(document.getElementById('cmbPlaza').value || '0', 10),
            idGestor: parseInt(document.getElementById('cmbGestor').value || '0', 10),
            fechaInicial: document.getElementById('txtDesde').value,
            fechaFinal: document.getElementById('txtHasta').value
        };

        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Reporte.aspx/GetReporteSemanal',
            data: JSON.stringify(base),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const data = msg.d;
                if (data == null) { reporte.mostrarMensaje('No tiene permisos.'); return; }
                reporte.pintarDetalle(data);
            }
        });

        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Reporte.aspx/GetResumenEconomico',
            data: JSON.stringify(base),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                reporte.pintarResumen(msg.d || []);
            }
        });
    },

    pintarDetalle: (data) => {
        if ($.fn.dataTable.isDataTable('#table')) {
            dataTable.clear().destroy();
        }
        const rows = data.map(c => [
            c.PeriodoReporte,
            c.Gestor,
            c.UsuarioGestor,
            c.FechaInicioCredito,
            c.FechaCobro,
            c.Plaza,
            c.NombreCliente,
            c.MontoRecuperadoMx,
            c.CanalPago,
            c.Folio,
            c.SemanasVencimiento,
            c.Bucket,
            c.PorcentajeComisionStr,
            c.ComisionMx,
            c.Observaciones
        ]);
        dataTable = $('#table').DataTable({
            data: rows,
            language: (typeof textosEsp !== 'undefined') ? textosEsp : undefined,
            dom: 'Bfrtip',
            buttons: ['copy', 'excel', 'print'],
            order: [[4, 'asc']]
        });
    },

    pintarResumen: (data) => {
        if ($.fn.dataTable.isDataTable('#tableResumen')) {
            dataTableResumen.clear().destroy();
        }
        const rows = data.map(r => [
            r.Gestor,
            r.TotalCobradoMx,
            r.NoTangibleMx,
            r.ComisionesMx,
            r.DiferenciaMx
        ]);
        dataTableResumen = $('#tableResumen').DataTable({
            data: rows,
            language: (typeof textosEsp !== 'undefined') ? textosEsp : undefined,
            paging: false,
            searching: false,
            info: false
        });
    },

    mostrarMensaje: (texto) => {
        document.getElementById('spnMensajes').textContent = texto;
        $('#panelMensajes').modal('show');
    }
};

window.addEventListener('load', () => {
    reporte.init();
});
