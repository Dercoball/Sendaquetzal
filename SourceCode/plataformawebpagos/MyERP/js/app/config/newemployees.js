'use strict';

/* ======= Globales ======= */
let pagina = '8';

// Título para exportaciones
(function () {
    const d = new Date();
    const pad = n => String(n).padStart(2, '0');
    window.descargas = `Empleados_${d.getFullYear()}_${pad(d.getMonth() + 1)}_${pad(d.getDate())}_${d.getMilliseconds()}`;
})();

/* ======= Constantes de posiciones ======= */
const POSICION_DIRECTOR = 1;
const POSICION_COORDINADOR = 2;
const POSICION_EJECUTIVO = 3;
const POSICION_SUPERVISOR = 4;
const POSICION_PROMOTOR = 5;

const employee = {
    idSeleccionado: -1,

    // usa el hidden si existe; si no, cae a 'connbd'
    getPath: () => document.getElementById('txtPath')?.value || 'connbd',

    /* ========== INIT ========== */
    init: () => {
        // Detecta ?id=123 para modo edición
        employee.idSeleccionado = employee.getQueryParamInt('id', -1);

        // 1) Cargar catálogos del formulario
        const p1 = employee.loadComboPosicion();
        const p2 = employee.loadComboPlaza();
        const p3 = employee.loadComboEmployeesByPosicion(POSICION_EJECUTIVO, '#cboEjecutivo');
        const p4 = employee.loadComboEmployeesByPosicion(POSICION_SUPERVISOR, '#cboSupervisor');

        $.when(p1, p2, p3, p4).then(() => {
            if (employee.idSeleccionado > 0) {
                employee.loadEmpleado(employee.idSeleccionado);
            } else {
                $('#dtpFechaIngreso').val(moment().format('YYYY-MM-DD'));
            }
        });
    },

    /* ========== Utils ========== */
    getQueryParamInt: (name, def) => {
        const url = new URL(window.location.href);
        const val = url.searchParams.get(name);
        return $.isNumeric(val) ? parseInt(val, 10) : def;
    },

    fmtFecha: (d) => {
        if (!d) return '';
        // ISO simple
        if (/^\d{4}-\d{2}-\d{2}/.test(d)) return moment(d).format('YYYY-MM-DD');
        // .NET /Date(…)/ 
        const m = /Date\((\d+)\)/.exec(d);
        if (m) return moment(Number(m[1])).format('YYYY-MM-DD');
        // Fallback
        try { return moment(d).format('YYYY-MM-DD'); } catch { return ''; }
    },

    /* ========== CARGA EDICIÓN ========== */
    loadEmpleado: (id) => {
        const path = employee.getPath();
        const params = JSON.stringify({ path: path, idEmpleado: id });

        // helpers locales
        const ensureOptionSelected = (selector, id, label) => {
            const $sel = $(selector);
            if (!$.isNumeric(id) || Number(id) <= 0) return;
            if ($sel.find(`option[value="${id}"]`).length === 0) {
                $sel.append(`<option value="${id}">${label || '[No listado]'}</option>`);
            }
            $sel.val(String(id)).trigger('change');
        };

        const fmt = employee.fmtFecha;


        $.ajax({
            type: 'POST',
            url: '/pages/Config/NewEmployee.aspx/GetEmpleadoById',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: params
        }).done((res) => {
            console.log('GetEmpleadoById result:', res);

            const d = res && res.d ? res.d : null;
            if (!d || !d.Colaborador) return;

            const emp = d.Colaborador;
            const user = d.User || {};
            const aval = d.Aval || {};
            const dir = emp.Direccion || {};
            const dirA = (aval && aval.direccion) ? aval.direccion : {};

            // ----- Cabecera -----
            $('#dtpFechaIngreso').val(fmt(emp.FechaIngreso));
            $('#dtpFechaBaja').val(fmt(emp.FechaBaja || emp.fecha_baja));

            // Fallbacks para IDs que a veces cambian de nombre en el backend
            const idPosicion =
                emp.IdPosicion ?? emp.id_posicion ?? emp.IdPuesto ?? emp.id_puesto ?? 0;

            const idPlaza =
                emp.IdPlaza ?? emp.id_plaza ?? emp.IdSucursal ?? emp.id_sucursal ?? 0;

            const idSup = emp.IdSupervisor ?? emp.id_supervisor ?? 0;
            const idEje = emp.IdEjecutivo ?? emp.id_ejecutivo ?? 0;

            console.log('[LOAD EMP] IDs del empleado:', { idPosicion, idPlaza, idSup, idEje });

            // Asignar selects (usa tu helper global setSelectValue)
            setSelectValue('#cboPuesto', idPosicion);
            setSelectValue('#cboPlaza', idPlaza);
            setSelectValue('#cboSupervisor', idSup);
            setSelectValue('#cboEjecutivo', idEje);

            // 300ms después, si no quedó seleccionado (porque el combo no trae ese ID), inyecta opción “[No listado]”
            setTimeout(() => {
                const curPos = Number($('#cboPuesto').val() || 0);
                const curPla = Number($('#cboPlaza').val() || 0);

                if ((!curPos || curPos === 0) && idPosicion > 0) {
                    console.warn('[PUESTO] ID del empleado no existe en el combo. Inyectando opción temporal.');
                    ensureOptionSelected('#cboPuesto', idPosicion, '[No listado]');
                }
                if ((!curPla || curPla === 0) && idPlaza > 0) {
                    console.warn('[PLAZA] ID del empleado no existe en el combo. Inyectando opción temporal.');
                    ensureOptionSelected('#cboPlaza', idPlaza, '[No listado]');
                }

                // “tipo de usuario = puesto”
                const puestoVal = Number($('#cboPuesto').val() || 0);
                if (puestoVal > 0) {
                    $('#txtIdTipoUsuario').val(String(puestoVal));
                }
            }, 300);

            $('#txtLimiteVentaPorEjercicio').val(emp.limite_venta_ejercicio || '');
            $('#txtLimiteIncrementoPorEjercicio').val(emp.limite_incremento_ejercicio || '');
            $('#cboFiscalzable').val(emp.fizcalizable ? '1' : '2');
            $('#txtNotaFoto').val(emp.NotaFoto || '');

            // ----- Usuario -----
            $('#txtNombreUsuario').val(user.Login || '');
            $('#txtPassword').val('').prop('placeholder', 'Dejar vacío para no cambiar');

            // Si el backend guarda tipo usuario en User, úsalo también
            if ($.isNumeric(user.IdTipoUsuario)) {
                $('#txtIdTipoUsuario').val(String(user.IdTipoUsuario));
            }

            // ----- Colaborador (UcColaborador) -----
            employee.setDatosPersona({
                Curp: emp.CURP || '',
                Nombre: emp.Nombre || '',
                PrimerApellido: emp.PrimerApellido || '',
                SegundoApellido: emp.SegundoApellido || '',
                Telefono: emp.Telefono || '',
                Ocupacion: emp.Ocupacion || '',
                direccion: dir
            }, 'UcColaborador');

            // ----- Aval (UcAval) -----
            employee.setDatosPersona({
                Curp: aval.Curp || '',
                Nombre: aval.Nombre || '',
                PrimerApellido: aval.PrimerApellido || '',
                SegundoApellido: aval.SegundoApellido || '',
                Telefono: aval.Telefono || '',
                Ocupacion: aval.Ocupacion || '',
                direccion: dirA
            }, 'UcAval');

            // ----- Evidencias (si aplica) -----
            for (let i = 1; i <= 8; i++) {
                employee.getDocumento(emp.IdEmpleado, i, `#img_${i}`, path);
            }
        }).fail((xhr) => {
            console.error('GetEmpleadoById error:', xhr.responseText || xhr);
            if (window.utils?.toast) utils.toast('Error al cargar el colaborador', 'error');
        }).always(() => {
            $('#panelLoading').modal('hide');
        });
    },


    /* ========== DOCUMENTOS (solo lectura para previews) ========== */
    getDocumento: (idEmpleado, idTipoDocumento, imgSelector, path) => {
        $.ajax({
            type: 'POST',
            url: '/pages/Config/Employees.aspx/GetDocument', // está en Employees.aspx.cs
            data: JSON.stringify({ path, idEmpleado: String(idEmpleado), idTipoDocumento: String(idTipoDocumento) }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        }).done((foto) => {
            const doc = foto.d || {};
            const $img = $(imgSelector);
            if (!doc.IdDocumento) return;

            const ext = (doc.Extension || '').toLowerCase().replace(/^\./, '');
            if (['png', 'jpg', 'jpeg', 'bmp', 'webp'].includes(ext)) {
                $img.attr('src', `data:image/${ext || 'jpg'};base64,${doc.Contenido}`);
            } else if (ext === 'pdf') {
                $img.attr('src', '/img/ico_pdf.png');
            } else {
                $img.attr('src', '/img/ico_doc.png');
            }
        });
    },

    /* ========== MAPEO PERSONA/DIRECCIÓN ========== */
    setDatosPersona: (o, pref) => {
        $('#' + pref + '_txtCURP').val(o.Curp || '');
        $('#' + pref + '_txtNombre').val(o.Nombre || '');
        $('#' + pref + '_txtPrimerApellido').val(o.PrimerApellido || '');
        $('#' + pref + '_txtSegundoApellido').val(o.SegundoApellido || '');
        $('#' + pref + '_txtTelefono').val(o.Telefono || '');
        $('#' + pref + '_txtOcupacion').val(o.Ocupacion || '');
        employee.setDireccion(o.direccion || {}, pref);
    },

    setDireccion: (dir, pref) => {
        $('#' + pref + '_txtEstado').val(dir.Estado || '');
        $('#' + pref + '_txtCalle').val(dir.Calle || dir.calleyno || '');
        $('#' + pref + '_txtColonia').val(dir.Colonia || '');
        $('#' + pref + '_txtMunicipio').val(dir.Municipio || '');
        $('#' + pref + '_txtCodigoPostal').val(dir.CodigoPostal || '');
        $('#' + pref + '_txtUbicacion').val(dir.Ubicacion || '');
        $('#' + pref + '_txtDireccionTrabajo').val(dir.DireccionTrabajo || '');
    },

    getDireccion: (pref) => {
        return {
            Calle: $('#' + pref + '_txtCalle').val(),
            Colonia: $('#' + pref + '_txtColonia').val(),
            Municipio: $('#' + pref + '_txtMunicipio').val(),
            Estado: $('#' + pref + '_txtEstado').val(),
            CodigoPostal: $('#' + pref + '_txtCodigoPostal').val(),
            DireccionTrabajo: $('#' + pref + '_txtDireccionTrabajo').val(),
            Ubicacion: $('#' + pref + '_txtUbicacion').val()
        };
    },

    /* ========== BUILDERS GUARDAR ========== */
    // tipo de usuario = puesto
    getUser: () => {
        const puestoVal = $('#cboPuesto').val();
        const idTipoUsuario = $.isNumeric(puestoVal) ? parseInt(puestoVal, 10) : 0;

        const base = {
            IdEmpleado: employee.idSeleccionado,
            Login: $('#txtNombreUsuario').val(),
            Password: $('#txtPassword').val(),
            IdTipoUsuario: idTipoUsuario
        };

        // duplicado snake_case por compatibilidad con backend
        return {
            ...base,
            id_tipo_usuario: base.IdTipoUsuario
        };
    },

    getColaborador: () => {
        const fIngreso = $('#dtpFechaIngreso').val();
        const fBaja = $('#dtpFechaBaja').val();

        const IdPosicion = $.isNumeric($('#cboPuesto').val()) ? parseInt($('#cboPuesto').val(), 10) : 0;
        const IdSupervisor = $.isNumeric($('#cboSupervisor').val()) ? parseInt($('#cboSupervisor').val(), 10) : 0;
        const IdEjecutivo = $.isNumeric($('#cboEjecutivo').val()) ? parseInt($('#cboEjecutivo').val(), 10) : 0;
        const IdPlaza = $.isNumeric($('#cboPlaza').val()) ? parseInt($('#cboPlaza').val(), 10) : 0;

        const base = {
            IdEmpleado: employee.idSeleccionado,
            FechaIngreso: moment(fIngreso).isValid() ? moment(fIngreso).format('YYYY-MM-DD') : null,
            // envío ambos por compatibilidad
            FechaBaja: moment(fBaja).isValid() ? moment(fBaja).format('YYYY-MM-DD') : null,
            fecha_baja: moment(fBaja).isValid() ? moment(fBaja).format('YYYY-MM-DD') : null,

            IdPosicion,
            IdSupervisor,
            IdEjecutivo,
            IdPlaza,

            CURP: $('#UcColaborador_txtCURP').val(),
            Telefono: $('#UcColaborador_txtTelefono').val(),
            Ocupacion: $('#UcColaborador_txtOcupacion').val(),
            PrimerApellido: $('#UcColaborador_txtPrimerApellido').val(),
            SegundoApellido: $('#UcColaborador_txtSegundoApellido').val(),
            Nombre: $('#UcColaborador_txtNombre').val(),
            Direccion: employee.getDireccion('UcColaborador'),
            limite_venta_ejercicio: $('#txtLimiteVentaPorEjercicio').val(),
            limite_incremento_ejercicio: $('#txtLimiteIncrementoPorEjercicio').val(),
            fizcalizable: $('#cboFiscalzable').val() === '1',
            NotaFoto: $('#txtNotaFoto').val()
        };

        // Duplicados snake_case de los IDs críticos
        return {
            ...base,
            id_posicion: base.IdPosicion,
            id_supervisor: base.IdSupervisor,
            id_ejecutivo: base.IdEjecutivo,
            id_plaza: base.IdPlaza
        };
    },

    getDatosPersona: (pref) => {
        return {
            Curp: $('#' + pref + '_txtCURP').val(),
            Nombre: $('#' + pref + '_txtNombre').val(),
            PrimerApellido: $('#' + pref + '_txtPrimerApellido').val(),
            SegundoApellido: $('#' + pref + '_txtSegundoApellido').val(),
            Telefono: $('#' + pref + '_txtTelefono').val(),
            Ocupacion: $('#' + pref + '_txtOcupacion').val(),
            direccion: employee.getDireccion(pref)
        };
    },

    /* ========== CARGA DE COMBOS (WebMethods en NewEmployee.aspx.cs) ========== */
    loadComboEmployeesByPosicion: (idTipoEmpleado, selector) => {
        return $.ajax({
            type: 'POST',
            url: '/pages/Config/NewEmployee.aspx/GetListaItemsEmpleadoByPosicion',
            data: JSON.stringify({ path: employee.getPath(), idTipoEmpleado: String(idTipoEmpleado) }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        }).done((msg) => {
            const items = msg.d || [];
            console.log(`[COMBO EMP BY POS ${idTipoEmpleado}] items`, items);
            let html = '<option value="">Seleccione...</option>';
            for (const it of items) html += `<option value="${it.IdEmpleado}">${it.Nombre}</option>`;
            $(selector).html(html);
        }).fail((xhr) => console.log('loadComboEmployeesByPosicion:', xhr.responseText || xhr));
    },

    loadComboPosicion: () => {
        return $.ajax({
            type: 'POST',
            url: '/pages/Config/NewEmployee.aspx/GetListaItemsPosiciones',
            data: JSON.stringify({ path: employee.getPath() }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        }).done((msg) => {
            const items = msg.d || [];
            console.log('[COMBO POSICION] items', items);

            let html = '<option value="">Seleccione...</option>';
            let nonZero = 0;

            for (const it of items) {
                const val = extractId(it, 'posicion'); // <- usa múltiples llaves posibles
                if (val > 0) nonZero++;
                html += `<option value="${val}">${it.Nombre}</option>`;
            }

            if (nonZero === 0 && items.length > 0) {
                console.warn('[COMBO POSICION] Todas las opciones traen ID=0. Revisa el WebMethod/DTO.');
            }

            $('#cboPuesto').html(html);
        }).fail((xhr) => console.log('loadComboPosicion:', xhr.responseText || xhr));
    },

    loadComboPlaza: () => {
        return $.ajax({
            type: 'POST',
            url: '/pages/Config/NewEmployee.aspx/GetListaItemsPlazas',
            data: JSON.stringify({ path: employee.getPath() }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json'
        }).done((msg) => {
            const items = msg.d || [];
            console.log('[COMBO PLAZA] items', items);

            let html = '<option value="">Seleccione...</option>';
            let nonZero = 0;

            for (const it of items) {
                const val = extractId(it, 'plaza'); // <- usa múltiples llaves posibles
                if (val > 0) nonZero++;
                html += `<option value="${val}">${it.Nombre}</option>`;
            }

            if (nonZero === 0 && items.length > 0) {
                console.warn('[COMBO PLAZA] Todas las opciones traen ID=0. Revisa el WebMethod/DTO.');
            }

            $('#cboPlaza').html(html);
        }).fail((xhr) => console.log('loadComboPlaza:', xhr.responseText || xhr));
    },


    /* ========== UI / HANDLERS ========== */
    loadPreviewImg: (upload, fn) => {
        if (upload.files && upload.files[0]) {
            const reader = new FileReader();
            reader.onload = fn;
            reader.readAsDataURL(upload.files[0]);
        } else {
            if (window.utils?.toast) utils.toast('Debe cargar la imagen', 'info');
        }
    },

    getLocation: (control) => {
        const options = { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 };
        navigator.geolocation.getCurrentPosition(
            (pos) => $('#' + control).val(`${pos.coords.latitude}, ${pos.coords.longitude}`),
            (err) => console.warn('GEO ERR(' + err.code + '): ' + err.message),
            options
        );
    },

    accionesBotones: () => {
        // sincroniza hidden de tipo usuario cuando cambia el puesto
        $(document).on('change', '#cboPuesto', function () {
            const v = $(this).val();
            $('#txtIdTipoUsuario').val($.isNumeric(v) ? String(parseInt(v, 10)) : '');
        });

        $(document).on('click', '.btnReloadLocation', function () {
            const sControl = $(this).attr('id').split('_')[0];
            employee.getLocation(sControl + '_txtUbicacion');
        });

        // helper de validación simple
        function getIntOrZero($el) {
            const v = $el.val();
            return $.isNumeric(v) ? parseInt(v, 10) : 0;
        }

        $('#btnGuardar').off('click').on('click', (e) => {
            e.preventDefault();

            // --- Validaciones duras antes de armar params ---
            const vPuesto = getIntOrZero($('#cboPuesto'));
            const vPlaza = getIntOrZero($('#cboPlaza'));

            if (vPuesto <= 0) {
                window.utils?.toast?.('Selecciona un Puesto válido.', 'warning');
                $('#cboPuesto').focus();
                return;
            }
            if (vPlaza <= 0) {
                window.utils?.toast?.('Selecciona una Plaza válida.', 'warning');
                $('#cboPlaza').focus();
                return;
            }

            // sincroniza hidden por si lo usas en server
            $('#txtIdTipoUsuario').val(String(vPuesto));

            // construye objetos
            const oCol = employee.getColaborador();
            const oUsr = employee.getUser();

            // trazas previas
            console.log('[PRE-SAVE] Puesto=', vPuesto, 'Plaza=', vPlaza);
            console.log('[PRE-SAVE] Colaborador IDs:', {
                IdPosicion: oCol.IdPosicion, id_posicion: oCol.id_posicion,
                IdPlaza: oCol.IdPlaza, id_plaza: oCol.id_plaza
            });
            console.log('[PRE-SAVE] User:', {
                IdTipoUsuario: oUsr.IdTipoUsuario, id_tipo_usuario: oUsr.id_tipo_usuario
            });

            const params = {
                path: employee.getPath(),
                idUsuario: document.getElementById('txtIdUsuario').value,
                oRequest: {
                    Colaborador: oCol,
                    User: oUsr,
                    Aval: employee.getDatosPersona('UcAval'),
                    DocumentosAval: employee.getDocumentos?.('UcDocumentacionAval') || [],
                    DocumentosColaborador: employee.getDocumentos?.('UcDocumentacionColaborador') || []
                }
            };

            // más trazas
            console.log('[SAVE params]', params);

            // muestra loading y protege con try/finally
            $('#panelLoading').modal('show');

            try {
                $.ajax({
                    type: 'POST',
                    url: '/pages/Config/NewEmployee.aspx/Save',
                    data: JSON.stringify(params),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    async: true,
                    timeout: 30000 // 30s para no colgarse
                }).done((msg) => {
                    const r = msg?.d;
                    console.log('[SAVE response]', r);

                    if (!r) {
                        window.utils?.toast?.('Respuesta vacía del servidor.', 'error');
                        return;
                    }
                    if (r.CodigoError === 1) {
                        window.utils?.toast?.(r.MensajeError || 'Error al guardar', 'error');
                    } else {
                        window.location = '/pages/Config/Employees.aspx';
                    }
                }).fail((xhr, status, err) => {
                    console.error('[SAVE fail]', { status, err, resp: xhr?.responseText });
                    const msg = (status === 'timeout')
                        ? 'El servidor tardó demasiado en responder.'
                        : (xhr?.responseText || 'Error al guardar');
                    window.utils?.toast?.(msg, 'error');
                }).always(() => {
                    $('#panelLoading').modal('hide');
                });
            } catch (ex) {
                console.error('[SAVE exception previa a $.ajax]', ex);
                window.utils?.toast?.('Error en el script antes de enviar.', 'error');
                $('#panelLoading').modal('hide');
            }
        });

        // --- PREVIEWS COLABORADOR ---
        $('#UcDocumentacionColaborador_filefotografia').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionColaborador_imgDocumento1').attr('src', e.target.result));
        });
        $('#UcDocumentacionColaborador_fileidentificacionfrente').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionColaborador_imgDocumento2').attr('src', e.target.result));
        });
        $('#UcDocumentacionColaborador_fileidentificacionreverso').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionColaborador_imgDocumento3').attr('src', e.target.result));
        });
        $('#UcDocumentacionColaborador_filecomprobantedomicilio').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionColaborador_imgDocumento4').attr('src', e.target.result));
        });

        // --- PREVIEWS AVAL ---
        $('#UcDocumentacionAval_filefotografia').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionAval_imgDocumento1').attr('src', e.target.result));
        });
        $('#UcDocumentacionAval_fileidentificacionfrente').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionAval_imgDocumento2').attr('src', e.target.result));
        });
        $('#UcDocumentacionAval_fileidentificacionreverso').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionAval_imgDocumento3').attr('src', e.target.result));
        });
        $('#UcDocumentacionAval_filecomprobantedomicilio').change(function () {
            employee.loadPreviewImg(this, (e) => $('#UcDocumentacionAval_imgDocumento4').attr('src', e.target.result));
        });
    }
};

/* ======= Bootstrap ======= */
window.addEventListener('load', () => {
    employee.init();
    employee.accionesBotones();
});

function setSelectValue(selector, value, intentos = 10) {
    if (value === undefined || value === null || value === '') return;

    const $sel = $(selector);
    if ($sel.length === 0) return;

    const v = String(value).trim();

    // si aún no tiene opciones, reintenta
    if ($sel.find('option').length === 0) {
        if (intentos <= 0) return;
        return setTimeout(() => setSelectValue(selector, value, intentos - 1), 100);
    }

    // intenta match exacto por value
    let $opt = $sel.find(`option[value="${v}"]`);
    if ($opt.length === 0) {
        // intenta por trim (por si vienen espacios) o por texto visible
        $opt = $sel.find('option').filter(function () {
            const val = String($(this).val()).trim();
            const txt = String($(this).text()).trim();
            return val === v || txt === v;
        });
    }

    if ($opt.length > 0) {
        $sel.val($opt.first().val()).trigger('change');
    } else {
        // como último recurso, asigna directamente y dispara change
        $sel.val(v).trigger('change');
    }
    // === helpers nuevos ===
    

}
function extractId(obj, kind) {
    // kind: 'posicion' | 'plaza'
    const common = ['Id', 'ID', 'Value', 'Valor', 'Key', 'Clave', 'pk', 'PK'];
    const posKeys = ['IdPosicion', 'id_posicion', 'IdPuesto', 'id_puesto', 'PosicionId', 'POSICION_ID'];
    const plaKeys = ['IdPlaza', 'id_plaza', 'IdSucursal', 'id_sucursal', 'PlazaId', 'SUCURSAL_ID'];

    const keys = (kind === 'posicion') ? [...posKeys, ...common] : [...plaKeys, ...common];

    for (const k of keys) {
        if (k in obj && $.isNumeric(obj[k])) {
            const n = Number(obj[k]);
            if (n > 0) return n;
        }
    }
    return 0;
}

function ensureOptionSelected(selector, id, label) {
    const $sel = $(selector);
    if (!$.isNumeric(id) || Number(id) <= 0) return;
    if ($sel.find(`option[value="${id}"]`).length === 0) {
        $sel.append(`<option value="${id}">${label || '[No listado]'}</option>`);
    }
    $sel.val(String(id)).trigger('change');
}