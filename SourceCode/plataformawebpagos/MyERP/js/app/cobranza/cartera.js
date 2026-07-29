'use strict';

// Página de permiso del módulo de Cobranza / Cartera
let pagina = '90';
let dataTable;
let carteraData = [];

const cartera = {

    idPrestamoSel: 0,

    init: () => {
        cartera.loadComboPlaza().finally(() => cartera.cargarItems());

        $('#btnFiltrar').on('click', function (e) {
            e.preventDefault();
            cartera.cargarItems();
        });

        $('#btnAsignar').on('click', function (e) {
            e.preventDefault();
            cartera.asignar();
        });

        $('#btnGuardarObs').on('click', function (e) {
            e.preventDefault();
            cartera.guardarObs();
        });

        $('#btnRegistrarCobro').on('click', function (e) {
            e.preventDefault();
            cartera.registrarCobro();
        });

        cartera.loadCanales();

        // Sólo gerencia (Director=1, Coordinador=2, Superadmin=6) gestiona.
        const tipo = Number(document.getElementById('txtIdTipoUsuario').value || -1);
        const esGerencia = (tipo === 1 || tipo === 2 || tipo === 6);
        if (!esGerencia) {
            $('#rowGestion').hide();
        }
    },

    loadComboPlaza: () => {
        return new Promise((resolve, reject) => {
            const params = JSON.stringify({ path: 'connbd' });
            $.ajax({
                type: 'POST',
                url: '../../pages/Customers/Customers.aspx/GetListaPlazas',
                data: params,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (msg) {
                    const selectEl = document.getElementById('cmbPlaza');
                    document.querySelectorAll('select[name="cmbPlaza"] option').forEach(o => o.remove());
                    selectEl.add(new Option('Todos', '0', true, true));
                    (msg.d || []).forEach(item => selectEl.add(new Option(item.Nombre, item.IdPlaza, false, false)));
                    resolve();
                },
                error: function (xhr, textStatus, errorThrown) {
                    console.log(textStatus + ': ' + xhr.responseText);
                    reject(errorThrown || textStatus);
                }
            });
        });
    },

    cargarItems: () => {
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idTipoUsuario: document.getElementById('txtIdTipoUsuario').value,
            idPlaza: parseInt(document.getElementById('cmbPlaza').value || '0', 10)
        });

        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Cartera.aspx/GetCartera',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const data = msg.d;
                if (data == null) {
                    cartera.mostrarMensaje('No tiene permisos para consultar la cartera de cobranza.');
                    return;
                }
                carteraData = data;
                cartera.pintarTabla(data);
            },
            error: function (xhr, textStatus) {
                console.log(textStatus + ': ' + xhr.responseText);
                cartera.mostrarMensaje('Ocurrió un error al cargar la cartera.');
            }
        });
    },

    pintarTabla: (data) => {
        if ($.fn.dataTable.isDataTable('#table')) {
            dataTable.clear().destroy();
        }

        const rows = data.map(item => {
            const gestor = item.Gestor && item.Gestor.trim() !== '' ? item.Gestor : '<span class="text-muted">Sin asignar</span>';
            const badge = 'badge-bucket-' + item.Bucket;
            return [
                gestor,
                item.NombreCliente,
                item.Plaza,
                item.Ejecutivo,
                item.Supervisor,
                item.Promotor,
                item.Producto,
                item.FechaCredito,
                item.SemanasTranscurridas,
                '<span class="badge ' + badge + '">' + item.Bucket + '</span>',
                item.AdeudoMx,
                item.MultasMx,
                item.GastoCobranzaMx,
                item.TotalCobranzaMx,
                item.SaldoActualizadoMx,
                item.Accion
            ];
        });

        dataTable = $('#table').DataTable({
            data: rows,
            language: (typeof textosEsp !== 'undefined') ? textosEsp : undefined,
            dom: 'Bfrtip',
            buttons: ['copy', 'excel', 'pdf', 'print', 'colvis'],
            order: [[8, 'desc']],
            columnDefs: [{ orderable: false, targets: [15] }]
        });
    },

    ver: (idPrestamo) => {
        const item = carteraData.find(x => x.IdPrestamo === idPrestamo);
        if (!item) return;
        cartera.idPrestamoSel = idPrestamo;

        const set = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val || ''; };

        set('dCliente', item.NombreCliente);
        set('dDirCliente', [item.CalleCliente, item.ColoniaCliente].filter(Boolean).join(', '));
        set('dTelCliente', item.TelefonoCliente);
        set('dAval1', item.NombreAval1);
        set('dDirAval1', [item.CalleAval1, item.ColoniaAval1].filter(Boolean).join(', '));
        set('dTelAval1', item.TelefonoAval1);
        set('dAval2', item.NombreAval2);
        set('dDirAval2', [item.CalleAval2, item.ColoniaAval2].filter(Boolean).join(', '));
        set('dTelAval2', item.TelefonoAval2);

        set('dProducto', item.Producto);
        set('dPlaza', item.Plaza);
        set('dCoordinador', item.Coordinador);
        set('dSupervisor', item.Supervisor);
        set('dEjecutivo', item.Ejecutivo);
        set('dPromotor', item.Promotor);
        set('dFecha', item.FechaCredito);
        set('dPagoSemanal', item.PagoSemanalMx);

        set('dSemanas', item.SemanasTranscurridas);
        set('dBucket', item.Bucket);
        set('dAdeudo', item.AdeudoMx);
        set('dMultas', item.MultasMx);
        set('dNumFallas', item.NumeroFallas);
        set('dSubtotal', item.SubtotalMx);
        set('dGasto', item.GastoCobranzaMx);
        set('dTotal', item.TotalCobranzaMx);
        set('dSaldo', item.SaldoActualizadoMx);

        document.getElementById('txtObservaciones').value = item.Observaciones || '';

        // Captura de cobro
        document.getElementById('txtMontoCobro').value = '';
        document.getElementById('txtFolio').value = '';
        document.getElementById('txtObsCobro').value = '';
        document.getElementById('txtFechaCobro').value = new Date().toISOString().slice(0, 10);
        cartera.loadFolios(item.IdGestor);

        cartera.loadGestores(item.IdGestor);
        $('#panelDetalle').modal('show');
    },

    loadCanales: () => {
        const params = JSON.stringify({ path: 'connbd' });
        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Cartera.aspx/GetCanales',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const selectEl = document.getElementById('cmbCanal');
                document.querySelectorAll('#cmbCanal option').forEach(o => o.remove());
                (msg.d || []).forEach(c => selectEl.add(new Option(c.Nombre, c.IdCanalPago, false, false)));
            }
        });
    },

    loadFolios: (idGestor) => {
        const dl = document.getElementById('listFolios');
        dl.innerHTML = '';
        if (!idGestor || idGestor <= 0) return;
        const params = JSON.stringify({ path: 'connbd', idGestor: idGestor });
        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Cartera.aspx/GetFoliosDisponibles',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                (msg.d || []).forEach(f => {
                    const opt = document.createElement('option');
                    opt.value = f.Folio;
                    dl.appendChild(opt);
                });
            }
        });
    },

    registrarCobro: () => {
        const monto = parseFloat(document.getElementById('txtMontoCobro').value || '0');
        if (!(monto > 0)) {
            cartera.mostrarMensaje('Capture un monto recuperado válido.');
            return;
        }
        const item = carteraData.find(x => x.IdPrestamo === cartera.idPrestamoSel);
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idPrestamo: cartera.idPrestamoSel,
            idGestor: item ? item.IdGestor : 0,
            montoRecuperado: monto,
            idCanalPago: parseInt(document.getElementById('cmbCanal').value || '0', 10),
            folio: document.getElementById('txtFolio').value,
            fechaCobro: document.getElementById('txtFechaCobro').value,
            observaciones: document.getElementById('txtObsCobro').value
        });

        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Cartera.aspx/RegistrarCobro',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const res = msg.d;
                if (res && res.CodigoError === 0) {
                    $('#panelDetalle').modal('hide');
                    cartera.cargarItems();
                    if (typeof toastr !== 'undefined') toastr.success('Cobro registrado correctamente.');
                } else {
                    cartera.mostrarMensaje((res && res.MensajeError) || 'No se pudo registrar el cobro.');
                }
            },
            error: function (xhr, textStatus) {
                console.log(textStatus + ': ' + xhr.responseText);
                cartera.mostrarMensaje('Ocurrió un error al registrar el cobro.');
            }
        });
    },

    loadGestores: (idGestorSel) => {
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idPlaza: parseInt(document.getElementById('cmbPlaza').value || '0', 10)
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
                selectEl.add(new Option('Sin asignar', '0', false, false));
                (msg.d || []).forEach(g => {
                    const opt = new Option(g.NombreCompleto, g.IdEmpleado, false, g.IdEmpleado === idGestorSel);
                    selectEl.add(opt);
                });
                selectEl.value = idGestorSel || 0;
            }
        });
    },

    asignar: () => {
        const idGestor = parseInt(document.getElementById('cmbGestor').value || '0', 10);
        if (idGestor <= 0) {
            cartera.mostrarMensaje('Seleccione un gestor.');
            return;
        }
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idPrestamo: cartera.idPrestamoSel,
            idGestor: idGestor
        });

        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Cartera.aspx/AsignarGestor',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const res = msg.d;
                if (res && res.CodigoError === 0) {
                    $('#panelDetalle').modal('hide');
                    cartera.cargarItems();
                    if (typeof toastr !== 'undefined') toastr.success('Gestor asignado correctamente.');
                } else {
                    cartera.mostrarMensaje((res && res.MensajeError) || 'No se pudo asignar.');
                }
            },
            error: function (xhr, textStatus) {
                console.log(textStatus + ': ' + xhr.responseText);
                cartera.mostrarMensaje('Ocurrió un error al asignar.');
            }
        });
    },

    guardarObs: () => {
        const obs = document.getElementById('txtObservaciones').value;
        const params = JSON.stringify({
            path: 'connbd',
            idUsuario: document.getElementById('txtIdUsuario').value,
            idPrestamo: cartera.idPrestamoSel,
            observaciones: obs
        });

        $.ajax({
            type: 'POST',
            url: '../../pages/Cobranza/Cartera.aspx/GuardarObservaciones',
            data: params,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (msg) {
                const res = msg.d;
                if (res && res.CodigoError === 0) {
                    cartera.cargarItems();
                    if (typeof toastr !== 'undefined') toastr.success('Observaciones guardadas.');
                } else {
                    cartera.mostrarMensaje((res && res.MensajeError) || 'No se pudo guardar.');
                }
            },
            error: function (xhr, textStatus) {
                console.log(textStatus + ': ' + xhr.responseText);
                cartera.mostrarMensaje('Ocurrió un error al guardar.');
            }
        });
    },

    mostrarMensaje: (texto) => {
        document.getElementById('spnMensajes').textContent = texto;
        $('#panelMensajes').modal('show');
    }
};

window.addEventListener('load', () => {
    cartera.init();
});
