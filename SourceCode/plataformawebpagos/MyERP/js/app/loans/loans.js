'use strict';

let date = new Date();
let descargas = "Prestamo_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';

/* ===================== Helpers ===================== */

// Convierte una ruta relativa (o ~/) en URL navegable (respeta http/https).
function toWebUrl(p) {
    let s = String(p || '');
    if (!s) return '';
    if (/^https?:\/\//i.test(s)) return s;
    s = s.replace(/^~\//, '').replace(/^\/+/, '');
    return (window.APP_ROOT || '/') + s;
}

// Si backend regresa solo ID de foto para garantías, construye URL de handler (opcional).
function urlGarantiaFromId(idFoto, idPrestamo) {
    return (window.APP_ROOT || '/') + 'ImgGarantia.ashx?id=' +
        encodeURIComponent(idFoto) + '&pid=' + encodeURIComponent(idPrestamo);
}

/* ===================== Módulo principal ===================== */

const loans = {
    init: () => {
        loans.idPrestamo = parseInt($("#hfIdPrestamo").val());
        loans.idPrestamo = isNaN(loans.idPrestamo) ? 0 : loans.idPrestamo;
        loans.idAval = -1;
        loans.idCliente = -1;
        loans.arrDocumentosCliente = {};
        loans.arrDocumentosAval = {};
        loans.arrGarantias = {};

        $("#nav-aprobacion-supervisor-tab").hide();
        $("#nav-aprobacion-ejecutivo-tab").hide();
        $("#btnRechazar").hide();
        $("#btnAprobar").hide();
        $("#dvBotonAgregarGarantia").hide();

        const today = moment().format('YYYY-MM-DD');
        $("#txtFechaSolicitud").val(today);

        loans.loadTipoClientes(function (result) {
            loans.cargaComboClientes(result);
            if (loans.idPrestamo > 0) {
                loans.detail(loans.idPrestamo);
            }
        });
    },

    getFechaSolicitud: () => $("#txtFechaSolicitud").val(),

    obtenerContadores: (IdCliente) => {
        var params = { path: "connbd", IdCliente: IdCliente };

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/ObtenrContadoresCliente",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                $("#txtVecesComoAval").val(oResponse.iContadorVecesAval);
                $("#txtPrestamosCompletados").val(oResponse.iContadorCompletados);
                $("#txtPrestamosRechazados").val(oResponse.iContadorRechazados);
            },
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    cargaComboClientes(msg) {
        var llst_TipoClientes = msg.d;
        let opcion = '<option value="">Seleccione...</option>';
        for (let i = 0; i < llst_TipoClientes.length; i++) {
            let lo_TipoCliente = llst_TipoClientes[i];
            opcion += `<option value='${lo_TipoCliente.IdTipoCliente}'>${lo_TipoCliente.NombreTipoCliente}</option>`;
        }
        $('#cboTipoCliente').html(opcion);
    },

    loadTipoClientes: (funcion) => {
        var params = { path: "connbd", idUsuario: document.getElementById('txtIdUsuario').value };
        $('#cboTipoCliente').html('');

        $.ajax({
            type: "POST",
            url: "/pages/Config/CustomerTypes.aspx/GetListaItems",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: funcion,
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    loadPreviewImg: (upload, funcion) => {
        if (upload.files && upload.files[0]) {
            var reader = new FileReader();
            reader.onload = funcion;
            reader.readAsDataURL(upload.files[0]);
        } else {
            utils.toast('Debe cargar la imagen', 'info');
        }
    },

    obtenerGarantias: (funcion) => {
        var params = { path: "connbd", idPrestamo: loans.idPrestamo };

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/ListadoGarantias",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: funcion,
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    loadTableGarantias: (data) => {
        $('#tableGarantias').DataTable({
            footerCallback: function (_row, data) {
                var lf_TotalMontos = 0;
                $.each(data, function (_i, o) { lf_TotalMontos += Number(o.costo || 0); });
                $("#thMontoTotal").html("$" + Number(lf_TotalMontos || 0).toLocaleString('es-MX'));
            },
            destroy: true,
            processing: true,
            order: [],
            searching: false,
            pagingType: 'full_numbers',
            bLengthChange: false,
            data: data,
            columns: [
                { data: 'nombre' },
                { data: 'numero_serie', className: 'text-center' },
                {
                    data: 'costo', className: 'text-center',
                    render: function (_d, _t, row) {
                        return '$ ' + Number(row.costo || 0).toLocaleString('es-MX');
                    }
                },
                {
                    data: 'fotografia', className: 'text-center',
                    render: function (_d, _t, row) {
                        const f = row.fotografia;
                        if (!f) return '';

                        // 1) base64
                        if (/^data:image\//i.test(f)) {
                            return "<img class='rounded' src='" + f + "' style='width:170px'/>";
                        }
                        // 2) ruta relativa /Uploads/...
                        if (/[\\/]/.test(f)) {
                            const url = toWebUrl(f);
                            return "<img class='rounded' src='" + url + "' style='width:170px'/>";
                        }
                        // 3) solo ID (handler)
                        if (/^\d+$/.test(String(f))) {
                            const url = urlGarantiaFromId(f, row.id_prestamo);
                            return "<img class='rounded' src='" + url + "' style='width:170px'/>";
                        }
                        // 4) nombre de archivo en carpeta del préstamo
                        const url2 = (window.APP_ROOT || '/') + 'Uploads/Prestamos/' +
                            encodeURIComponent(row.id_prestamo) + '/' + encodeURIComponent(f);
                        return "<img class='rounded' src='" + url2 + "' style='width:170px'/>";
                    }
                },
                {
                    data: '', className: 'text-center',
                    render: function (_d, _t, row) {
                        return "<a class='rounded btn btn-danger text-white eliminarGarantia' data-id='" +
                            row.id_garantia_prestamo + "'><i class='fa fa-trash'></i></a>";
                    }
                }
            ],
            language: textosEsp,
            columnDefs: [{ targets: [-1], orderable: false }],
            dom: 'frBtipl',
            buttons: [
                { extend: 'excelHtml5', title: descargas, text: 'Xls', className: 'excelbtn' },
                { extend: 'pdfHtml5', title: descargas, text: 'Pdf', className: 'pdfbtn' }
            ]
        });
    },

    setDireccion: (oDireccion, control) => {
        $("#" + control + "_txtEstado").val(oDireccion.Estado);
        $("#" + control + "_txtCalle").val(oDireccion.Calle);
        $("#" + control + "_txtColonia").val(oDireccion.Colonia);
        $("#" + control + "_txtMunicipio").val(oDireccion.Municipio);
        $("#" + control + "_txtCodigoPostal").val(oDireccion.CodigoPostal);
        $("#" + control + "_txtUbicacion").val(oDireccion.Ubicacion);
        $("#" + control + "_txtDireccionTrabajo").val(oDireccion.DireccionTrabajo);
    },

    getDireccion: (control) => {
        return {
            Calle: $("#" + control + "_txtCalle").val(),
            Colonia: $("#" + control + "_txtColonia").val(),
            Municipio: $("#" + control + "_txtMunicipio").val(),
            Estado: $("#" + control + "_txtEstado").val(),
            CodigoPostal: $("#" + control + "_txtCodigoPostal").val(),
            DireccionTrabajo: $("#" + control + "_txtDireccionTrabajo").val(),
            Ubicacion: $("#" + control + "_txtUbicacion").val()
        };
    },

    setPrestamo: (oPrestamo) => {
        loans.idPrestamo = oPrestamo.IdPrestamo;
        $("#lblFechaSolicitud").html(moment(oPrestamo.FechaSolicitud).format('YYYY-MM-DD'));
        $("#txtCantidadPrestamo").val(oPrestamo.Monto);
        $("#txMaximoPorRenovacion").val(oPrestamo.MontoPorRenovacion);
        $("#cboTipoCliente").val(oPrestamo.IdTipoCliente);
        $("#txtNotaAprobacion").val(oPrestamo.NotasGenerales);
        $("#txtUbicacionReconfirmar").val(oPrestamo.IdTipoCliente);
        $("#txtNotaAprobacionEjecutivo").val(oPrestamo.NotasEjecutivo);
    },

    getPrestamo: () => {
        return {
            IdPrestamo: loans.idPrestamo,
            FechaSolicitud: moment($("#lblFechaSolicitud").html()).format('YYYY-MM-DD'),
            IdTipoCliente: $("#cboTipoCliente").val(),
            Monto: $("#txtCantidadPrestamo").val(),
            MontoPorRenovacion: $("#txMaximoPorRenovacion").val()
        };
    },

    setDatosPersona: (oDatosPersona, control) => {
        $("#" + control + "_txtCURP").val(oDatosPersona.Curp);
        $("#" + control + "_txtNombre").val(oDatosPersona.Nombre);
        $("#" + control + "_txtPrimerApellido").val(oDatosPersona.PrimerApellido);
        $("#" + control + "_txtSegundoApellido").val(oDatosPersona.SegundoApellido);
        $("#" + control + "_txtTelefono").val(oDatosPersona.Telefono);
        $("#" + control + "_txtOcupacion").val(oDatosPersona.Ocupacion);
        loans.setDireccion(oDatosPersona.direccion, control);
    },

    getDatosPersona: (control) => {
        return {
            Curp: $("#" + control + "_txtCURP").val(),
            Nombre: $("#" + control + "_txtNombre").val(),
            PrimerApellido: $("#" + control + "_txtPrimerApellido").val(),
            SegundoApellido: $("#" + control + "_txtSegundoApellido").val(),
            Telefono: $("#" + control + "_txtTelefono").val(),
            Ocupacion: $("#" + control + "_txtOcupacion").val(),
            direccion: loans.getDireccion(control)
        };
    },

    getDocumentos: (control) => {
        var arrDocumentos = [];
        var filefotografia = $("#" + control + "_filefotografia");
        var fileidentificacionfrente = $("#" + control + "_fileidentificacionfrente");
        var fileidentificacionreverso = $("#" + control + "_fileidentificacionreverso");
        var filecomprobantedomicilio = $("#" + control + "_filecomprobantedomicilio");

        var imgFotografia = $("#" + control + "_imgDocumento1");
        var imgFrente = $("#" + control + "_imgDocumento2");
        var imgReverso = $("#" + control + "_imgDocumento3");
        var imgDomicilio = $("#" + control + "_imgDocumento4");

        var hfIdDocumento1 = $("#" + control + "_hfIdDocumento1");
        var hfIdDocumento2 = $("#" + control + "_hfIdDocumento2");
        var hfIdDocumento3 = $("#" + control + "_hfIdDocumento3");
        var hfIdDocumento4 = $("#" + control + "_hfIdDocumento4");

        if (filefotografia[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 1,
                IdDocumento: $.isNumeric(hfIdDocumento1.val()) ? parseInt(hfIdDocumento1.val()) : 0,
                Contenido: imgFotografia.attr('src'),
                Nombre: filefotografia[0].files[0].name
            });
        }

        if (fileidentificacionfrente[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 2,
                IdDocumento: $.isNumeric(hfIdDocumento2.val()) ? parseInt(hfIdDocumento2.val()) : 0,
                Contenido: imgFrente.attr('src'),
                Nombre: fileidentificacionfrente[0].files[0].name
            });
        }

        if (fileidentificacionreverso[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 3,
                IdDocumento: $.isNumeric(hfIdDocumento3.val()) ? parseInt(hfIdDocumento3.val()) : 0,
                Contenido: imgReverso.attr('src'),
                Nombre: fileidentificacionreverso[0].files[0].name
            });
        }

        if (filecomprobantedomicilio[0].files.length > 0) {
            arrDocumentos.push({
                IdTipoDocumento: 4,
                IdDocumento: $.isNumeric(hfIdDocumento4.val()) ? parseInt(hfIdDocumento4.val()) : 0,
                Contenido: imgDomicilio.attr('src'),
                Nombre: filecomprobantedomicilio[0].files[0].name
            });
        }

        return arrDocumentos;
    },

    detail: (idPrestamo) => {
        var params = {
            path: "connbd",
            Id: idPrestamo,
            idUsuario: document.getElementById('txtIdUsuario').value
        };

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/DetallePrestamo",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var lo_Prestamo = msg.d;

                loans.idAval = lo_Prestamo.Prestamo.IdAval;
                loans.idCliente = lo_Prestamo.Prestamo.IdCliente;

                loans.obtenerContadores(loans.idCliente);
                loans.setDatosPersona(lo_Prestamo.Cliente, 'UcCliente');
                loans.setDatosPersona(lo_Prestamo.Aval, 'UcAval');
                loans.setPrestamo(lo_Prestamo.Prestamo);

                loans.obtenerGarantias(function (msg) {
                    loans.arrGarantias = msg.d;
                    loans.loadTableGarantias(loans.arrGarantias);

                    /* ===== Rutea documentos por IdCliente/IdAval ===== */
                    var docsAll = []
                        .concat(lo_Prestamo.DocumentosCliente || [])
                        .concat(lo_Prestamo.DocumentosAval || []);

                    var idCliente = loans.idCliente;
                    var idAval = loans.idAval;

                    var docsCliente = docsAll.filter(d => String(d.IdCliente) === String(idCliente));
                    var docsAval = docsAll.filter(d => String(d.IdCliente) === String(idAval));

                    function putDoc(controlPrefix, d) {
                        var tipo = d.IdTipoDocumento; // 1..4
                        var hid = '#' + controlPrefix + '_hfIdDocumento' + tipo;
                        var img = '#' + controlPrefix + '_imgDocumento' + tipo;

                        $(hid).val(d.IdDocumento).removeAttr('src');

                        var url = toWebUrl(d.Contenido || d.Url);
                        if (url) {
                            $(img).attr('src', url).removeClass('border border-danger').attr('alt', '');
                        } else {
                            console.warn('Documento sin ruta', d);
                            $(img).removeAttr('src').addClass('border border-danger').attr('alt', 'Sin imagen');
                        }
                    }

                    docsCliente.forEach(d => putDoc('UcDocumentacionCliente', d));
                    docsAval.forEach(d => putDoc('UcDocumentacionAval', d));

                    /* ===== Fin ruteo documentos ===== */

                    if (lo_Prestamo.Prestamo.IdStatusPrestamo === 1) {
                        $("#frmAval input").prop("disabled", true);
                        $("#frmCustomer input").prop("disabled", true);
                        $("#nav-aprobacion-supervisor-tab").show();
                        $("#dvBotonAgregarGarantia").show();
                        $("#btnRechazar").show();
                        $("#btnAprobar").show();
                    }
                    else if (lo_Prestamo.Prestamo.IdStatusPrestamo === 2) {
                        $("#frmAval input").prop("disabled", true);
                        $("#frmCustomer input").prop("disabled", true);
                        $("#nav-aprobacion-ejecutivo-tab").show();
                        $("#btnRechazar").show();
                        $("#btnAprobar").show();
                        $("#tableGarantias tbody tr a").prop("style", "display:none");
                    }
                    else {
                        $("#frmAval input").prop("disabled", true);
                        $("#frmCustomer input").prop("disabled", true);
                        $("#frmAprobacion input").prop("disabled", true);
                        $("#frmAprobacionEjecutivo input").prop("disabled", true);
                        $("#frmAprobacion textarea").prop("disabled", true);
                        $("#frmAprobacionEjecutivo textarea").prop("disabled", true);
                        $("#btnGuardarCliente").hide();
                        $("#nav-aprobacion-supervisor-tab").show();
                        $("#nav-aprobacion-ejecutivo-tab").show();
                        $("#tableGarantias tbody tr a").prop("style", "display:none");
                    }
                });
            },
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    getLocation: (control) => {
        var options = { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 };

        function success(pos) {
            var crd = pos.coords;
            $("#" + control).val(`${crd.latitude}, ${crd.longitude}`);
        }
        function error(err) {
            console.warn('ERROR(' + err.code + '): ' + err.message);
        }
        navigator.geolocation.getCurrentPosition(success, error, options);
    },

    guardaRechazo: () => {
        var params = {
            oPrestamo: {
                IdPrestamo: loans.idPrestamo,
                NotasGenerales: $("#txtNotaAprobacion").val(),
                UbicacionConfirmada: $("#txtUbicacionReconfirmar").val()
            },
            path: "connbd"
        };

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/RechazoPrestamo",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                if (oResponse.CodigoError <= 0) {
                    utils.toast('El prestamo fue rechazado correctamente', 'ok');
                    setTimeout(function () {
                        window.location = '/pages/Loans/LoanRequest.aspx';
                    }, 500);
                } else {
                    utils.toast(oResponse.MensajeError, 'error');
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    guardarAprobacionSupervisor: () => {
        var params = {
            oPrestamo: {
                IdPrestamo: loans.idPrestamo,
                IdTipoCliente: $("#cboTipoCliente").val(),
                NotasGenerales: $("#txtNotaAprobacion").val(),
                UbicacionConfirmada: $("#txtUbicacionReconfirmar").val()
            },
            fMontoGarantia: 0,
            path: "connbd"
        };

        $.each(loans.arrGarantias, function (_i, g) {
            params.fMontoGarantia += Number(g.costo || 0);
        });

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/AprobacionSupervisor",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                if (oResponse.CodigoError <= 0) {
                    utils.toast('El prestamo fue aprobado por el supervisor correctamente', 'ok');
                    setTimeout(function () {
                        window.location = '/pages/Loans/LoanRequest.aspx';
                    }, 500);
                } else {
                    utils.toast(oResponse.MensajeError, 'error');
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    guardaAprobacionEjecutivo: () => {
        var frmAprobacionEjecutivo = $("#frmAprobacionEjecutivo");
        if (frmAprobacionEjecutivo.valid()) {
            var params = {
                path: "connbd",
                IdPrestamo: loans.idPrestamo,
                sNotaEjecutivo: $("#txtNotaAprobacionEjecutivo").val()
            };

            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanApprove.aspx/AprobacionEjecutivo",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var oResponse = msg.d;
                    if (oResponse.CodigoError <= 0) {
                        utils.toast('El prestamo fue aprobado por el ejecutivo', 'ok');
                        setTimeout(function () {
                            window.location = '/pages/Loans/LoanRequest.aspx';
                        }, 500);
                    } else {
                        utils.toast(oResponse.MensajeError, 'error');
                    }
                },
                error: function (XMLHttpRequest, textStatus) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        }
    },

    guardarClienteAval: () => {
        var params = { Request: {} };
        params.Request.Prestamo = loans.getPrestamo();
        params.Request.DocumentosAval = loans.getDocumentos('UcDocumentacionAval');
        params.Request.DocumentosCliente = loans.getDocumentos('UcDocumentacionCliente');
        params.Request.Cliente = loans.getDatosPersona('UcCliente');
        params.Request.Aval = loans.getDatosPersona('UcAval');
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.Request.Cliente.idCliente = loans.idCliente;
        params.Request.Aval.idCliente = loans.idAval;

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanApprove.aspx/SaveCustomerOrAval",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var oResponse = msg.d;
                if (oResponse.CodigoError <= 0) {
                    utils.toast('Los datos del prestamo fue actualizado correctamente', 'ok');
                    setTimeout(function () {
                        window.location = '/pages/Loans/LoanRequest.aspx';
                    }, 500);
                } else {
                    utils.toast(oResponse.MensajeError, 'error');
                }
            },
            error: function (XMLHttpRequest, textStatus) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },

    accionesBotones: () => {
        $(document).on("click", ".btnReloadLocation", function () {
            var sControl = $(this).attr('id').substring(0, $(this).attr('id').indexOf('_'));
            loans.getLocation(sControl + '_txtUbicacion');
        });

        $("#btnConfirmarUbicacion").click(function () {
            loans.getLocation('txtUbicacionReconfirmar');
        });

        $(document).on('click', '.eliminarGarantia', function () {
            var params = { path: "connbd", Id: $(this).attr('data-id') };

            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanApprove.aspx/DeleteGarantia",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var lo_Salida = msg.d;
                    if (lo_Salida.CodigoError <= 0) {
                        loans.obtenerGarantias(function (msg) {
                            var listaGarantias = msg.d;
                            $("#frmGarantias")[0].reset();
                            $("#imgImagenGarantia").attr('src', '');
                            loans.arrGarantias = listaGarantias;
                            loans.loadTableGarantias(listaGarantias);
                        });
                    }
                },
                error: function (XMLHttpRequest, textStatus) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });

        $("#btnAgregarGarantia").click(function () {
            var frmGarantias = $("#frmGarantias");

            if (frmGarantias.valid()) {
                var params = {
                    path: "connbd",
                    idUsuario: document.getElementById('txtIdUsuario').value,
                    oGarantia: {
                        id_prestamo: loans.idPrestamo,
                        nombre: $("#txtNombreGarantia").val(),
                        numero_serie: $("#txtNumeroSerie").val(),
                        costo: $("#txtCosto").val(),
                        fotografia: $("#imgImagenGarantia").attr('src') // dataURL o ruta
                    }
                };

                $.ajax({
                    type: "POST",
                    url: "/pages/Loans/LoanApprove.aspx/SaveGarantia",
                    data: JSON.stringify(params),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        var lo_Salida = msg.d;
                        if (lo_Salida.CodigoError <= 0) {
                            loans.obtenerGarantias(function (msg) {
                                var listaGarantias = msg.d;
                                $("#frmGarantias")[0].reset();
                                $("#imgImagenGarantia").attr('src', '');
                                loans.arrGarantias = listaGarantias;
                                loans.loadTableGarantias(listaGarantias);
                            });
                        }
                    },
                    error: function (XMLHttpRequest, textStatus) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }
                });
            }
        });

        $('#cboTipoCliente').change(function () {
            var params = { path: "connbd", id: $(this).val() };

            $("#txtCantidadPrestamo").val('');
            $("#txMaximoPorRenovacion").val('');

            $.ajax({
                type: "POST",
                url: "/pages/Config/CustomerTypes.aspx/GetItem",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var lo_TipoCliente = msg.d;
                    $("#txtCantidadPrestamo").val(lo_TipoCliente.PrestamoInicialMaximo);
                    $("#txMaximoPorRenovacion").val(lo_TipoCliente.CantidadParaRenovar);
                },
                error: function (XMLHttpRequest, textStatus) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });

        $("#flImagenGarantia").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#imgImagenGarantia').attr('src', e.target.result);
            });
        });

        // Previews Cliente
        $("#UcDocumentacionCliente_filefotografia").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionCliente_imgDocumento1').attr('src', e.target.result);
            });
        });
        $("#UcDocumentacionCliente_fileidentificacionfrente").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionCliente_imgDocumento2').attr('src', e.target.result);
            });
        });
        $("#UcDocumentacionCliente_fileidentificacionreverso").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionCliente_imgDocumento3').attr('src', e.target.result);
            });
        });
        $("#UcDocumentacionCliente_filecomprobantedomicilio").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionCliente_imgDocumento4').attr('src', e.target.result);
            });
        });

        // Previews Aval
        $("#UcDocumentacionAval_filefotografia").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionAval_imgDocumento1').attr('src', e.target.result);
            });
        });
        $("#UcDocumentacionAval_fileidentificacionfrente").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionAval_imgDocumento2').attr('src', e.target.result);
            });
        });
        $("#UcDocumentacionAval_fileidentificacionreverso").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionAval_imgDocumento3').attr('src', e.target.result);
            });
        });
        $("#UcDocumentacionAval_filecomprobantedomicilio").change(function () {
            loans.loadPreviewImg(this, function (e) {
                $('#UcDocumentacionAval_imgDocumento4').attr('src', e.target.result);
            });
        });

        $("#btnAprobar").click(function (e) {
            e.preventDefault();
            var li_tab_index = $('#nav-tab-prestamos .active').attr('data-id');

            switch (li_tab_index) {
                case "3":
                    loans.guardarAprobacionSupervisor();
                    break;
                case "4":
                    loans.guardaAprobacionEjecutivo();
                    break;
            }
        });

        $("#btnRechazar").click(function (e) {
            e.preventDefault();
            loans.guardaRechazo();
        });

        $("#btnGuardarCliente").click(function (_e) {
            loans.guardarClienteAval();
        });

        $(document).on("keyup", ".curp-persona", function () {
            var txtCurp = $(this);
            txtCurp.val(txtCurp.val().toUpperCase());
        });

        $(document).on("change", ".curp-persona", function () {
            var sControl = $(this).attr('id').substring(0, $(this).attr('id').indexOf('_'));
            var params = { path: "connbd", sCURP: $(this).val() };

            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanApprove.aspx/BuscarClientePorCURP",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var oCliente = msg.d;
                    if (oCliente.IdCliente > 0) {
                        if (sControl === 'UcCliente') { loans.idCliente = oCliente.IdCliente; }
                        if (sControl === 'UcAval') { loans.idAval = oCliente.IdCliente; }
                        if (loans.idCliente > 0) {
                            loans.obtenerContadores(loans.idCliente);
                        }
                        utils.toast('El CURP fue encontrado', 'ok');
                        loans.setDatosPersona(oCliente, sControl);
                    }
                },
                error: function (XMLHttpRequest, textStatus) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });
    }
};

window.addEventListener('load', () => {
    loans.init();
    loans.accionesBotones();
});
