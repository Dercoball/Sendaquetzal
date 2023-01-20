'use strict';
let date = new Date();
let descargas = "INVERSIONES_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '52';

const asset = {
    init: () => {
        $("#dvFechaInversion").hide();
        $('#panelTabla').show();
        $('#panelForm').hide();
        $("#nvMenuInversiones").hide();
        $("#lblFechaActual").html(moment(Date.now()).format("DD-MM-YYYY"));
        $("#dvCamposRetiro").hide();

        asset.idInversionista = -1;
        asset.idStatus = 1;
        asset.idSeleccionado = -1;
        asset.idTabSeleccionado = 1;
        asset.loadComboInvestor();
        asset.loadContent();
        asset.loadComboStatus();
    },
    calculateUtilidadPesos: () => {
        var lf_montoInvertir = parseFloat($("#txtMontoAInvertir").val());
        var lf_porcentajeUtilidad = parseFloat($("#txtPorcentajeUtilidad").val());
        var lf_UtilidadPesos = 0;
        var lf_UtilidadInversion = 0;

        lf_montoInvertir = isNaN(lf_montoInvertir) ? 0 : lf_montoInvertir;
        lf_porcentajeUtilidad = isNaN(lf_porcentajeUtilidad) ? 0 : lf_porcentajeUtilidad / 100;
        lf_UtilidadPesos = lf_montoInvertir * lf_porcentajeUtilidad;
        lf_UtilidadInversion = (lf_montoInvertir + lf_UtilidadPesos).toFixed(2);

        $("#txtUtilidadPesos").val(lf_UtilidadPesos.toFixed(2));
        $("#txtUtilidadInversion").val(lf_UtilidadInversion);
    },
    loadContent() {
        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investments.aspx/GetListaItems",
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
                asset.createTableInvestments(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    getFilter: () => {
        var oRequest =
        {
            Estatus: $("#cboStatus").val(),
            NombreInversionista: $("#txtNombreInversionistaBusqueda").val(),
            MontoMinimo: $("#txtMontoMinimoBusqueda").val(),
            MontoMaximo: $("#txtMontoMaximaBusqueda").val(),
            UtilidadMinimo: $("#txtUtilidadMinimaBusqueda").val(),
            UtilidadMaximo: $("#txtUtilidadMaximaBusqueda").val(),
            PlazoMinimo: $("#txtPlazoMinimoBusqueda").val(),
            PlazoMaximo: $("#txtPlazoMaximoBusqueda").val(),
            IngresoMinimo: moment($("#dtpIngresoMinimoBusqueda").val()).isValid()
                ? moment($("#dtpIngresoMinimoBusqueda").val()).format('YYYY-MM-DD')
                : null,
            IngresoMaximo: moment($("#dtpIngresoMaximoBusqueda").val()).isValid()
                ? moment($("#dtpIngresoMaximoBusqueda").val()).format('YYYY-MM-DD')
                : null,
            RetiroMinimo: moment($("#dtpRetiroMinimoBusqueda").val()).isValid()
                ? moment($("#dtpRetiroMinimoBusqueda").val()).format('YYYY-MM-DD')
                : null,
            RetiroMaximo: moment($("#dtpRetiroMaximoBusqueda").val()).isValid()
                ? moment($("#dtpRetiroMaximoBusqueda").val()).format('YYYY-MM-DD')
                : null
        };

        return oRequest;
    },
    createTableInvestments: (data) => {
        let table = $('#table').DataTable({
            footerCallback: function (row, data, start, end, display) {
                var lf_TotalMontos = 0;;

                $.each(data, function (index, oInversion) {
                    lf_TotalMontos += oInversion.monto;
                })

                $("#thMontoTotalInversion").html("$" + lf_TotalMontos.toFixed(2));
            },
            destroy: true,
            processing: true,
            order: [],
            searching: false,
            pagingType: 'full_numbers',
            bLengthChange: false,
            data: data,
            columns: [
                { data: 'Inversionista.Nombre' },
                {
                    data: 'monto', className: 'text-center', render: function (datum, type, row) {
                        return '$' + row.monto.toFixed(2);
                    }
                },
                {
                    data: 'porcentaje_utilidad', className: 'text-center', render: function (datum, type, row) {
                        return (row.porcentaje_utilidad / 100).toFixed(2) + " %";
                    }
                },
                { data: 'plazo', className: 'text-center'},
                {
                    data: 'Estatus.id_status_inversion', className: 'text-center', render: function (datum, type, row) {
                        return "<h2><span class='rounded text-white " + row.Estatus.color + " p-2'>" + row.Estatus.nombre + "</span></h2>";
                    }
                },
                {
                    data: 'fecha', className: 'text-center', render: function (datum, type, row) {
                        return moment(row.fecha).format('DD-MM-YYYY');
                    }
                },
                {
                    data: 'fechaRetiro', className: 'text-center', render: function (datum, type, row) {
                        return moment(row.fechaRetiro).format('DD-MM-YYYY');
                    }
                },
                { data: 'Accion', className: 'text-center' }
            ],
            "language": textosEsp,
            "columnDefs": [
                {
                    "targets": [-1],
                    "orderable": false
                },
            ],
            dom: 'frBtipl',
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
    },
    loadComboInvestor: () => {
        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investments.aspx/GetListaItemsInvestors",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];
                    opcion += `<option value = '${item.IdInversionista}' > ${item.Nombre}</option > `;
                }

                $('#comboInversionista').html(opcion);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadComboStatus: () => {
        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investments.aspx/GetListaStatusInversion",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];
                    opcion += `<option value = '${item.id_status_inversion}' > ${item.nombre}</option > `;
                }

                $('#cboStatus').html(opcion);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadInversiones: () => {
        var params = {};
        var Request = {};
        Request.IdInversionista = asset.idInversionista;
        Request.Estatus = 1;
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.oRequest = Request;
        params = JSON.stringify(params);

        $('#cboFechaInversionesRetiro').html('');

        $.ajax({
            type: "POST",
            url: "/pages/Investors/Investments.aspx/Search",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var llst_Inversiones = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < llst_Inversiones.length; i++) {
                    let lo_Inversion = llst_Inversiones[i];
                    opcion += `<option value = '${lo_Inversion.id_inversion}' > ${moment(lo_Inversion.fecha).format('YYYY-MM-DD')}</option > `;
                }

                $('#cboFechaInversionesRetiro').html(opcion);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadInversionista: (id, funcion) => {
        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.id = id;
        params = JSON.stringify(params);
     
        $.ajax({
            type: "POST",
            url: "/pages/Investors/Investments.aspx/GetDataInvestor",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: funcion,
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadInversion: (id, funcion) =>
    {
        let params = {};
        params.path = window.location.hostname;
        params.id = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investments.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: funcion,
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    cargaInversionDetalle: (inversion) => {
        $("#comboInversionista").val(inversion.id_inversionista);
        $("#txtUtilidadInversion").val(inversion.inversion_utilidad);
        $("#txtMontoAInvertir").val(inversion.monto);
        $("#txtPlazo").val(inversion.plazo).keyup();
        $("#txtPorcentajeUtilidad").val(inversion.porcentaje_utilidad);
        $("#txtUtilidadPesos").val(inversion.utilidad_pesos);

        if (inversion.id_status_inversion === 2) {
            asset.bloquearCampos(true);
        }
    },
    cancelarGuardadoResgistro: () => {
        asset.idSeleccionado = -1;
        $('#frm')[0].reset();
        $('#panelTabla').show();
        $('#panelForm').hide();
        $("#nvMenuInversiones").hide();
        asset.idInversionista = -1;
        asset.idStatus = -1;
        $('#tabInvertir').tab('show');
        $("#cboFechaInversionesRetiro").html("");
        $("#comboInversionista").attr("disabled", true);
    },
    bloquearCampos: (enabled) => {
        $("#txtPlazo").attr("readonly", enabled);
        $("#txtMontoAInvertir").attr("readonly", enabled);
        $("#txtPorcentajeUtilidad").attr("readonly", enabled);
    },
    limpiarNuevoResgistro: () => {
        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#panelTabla').hide();
        $('#panelForm').show();
        $("#nvMenuInversiones").show();
        asset.idSeleccionado = -1;
        asset.idInversionista = -1;
        asset.idStatus = -1;       
    },
    nuevo: () => {
        asset.limpiarNuevoResgistro();
        asset.idStatus = 1;
        $('#tabInvertir').tab('show').click();
        $("#comboInversionista").attr("disabled", false);
        $('.deshabilitable').prop('disabled', false);
    },
    delete: (id) => {
        asset.idSeleccionado = id;
        $('#mensajeEliminar').text(`Esta seguro que dese eliminar la inversión ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');
    },
    edit: (id) => {
        asset.limpiarNuevoResgistro();
        asset.loadInversion(id, function (resultado) {
            let lo_Inversion = resultado.d;
            asset.idSeleccionado = lo_Inversion.id_inversion;
            asset.idInversionista = lo_Inversion.id_inversionista;
            asset.idStatus = lo_Inversion.id_status_inversion; 
            asset.cargaInversionDetalle(lo_Inversion);
            $('#tabInvertir').tab('show').click();
            $("#comboInversionista").attr("disabled", true);
        })
    },
    accionesBotones: () => {
        $("#cboFechaInversionesRetiro").change(function () {
            asset.loadInversion($(this).val(), function (resultado) {
                let lo_Inversion = resultado.d;
                asset.cargaInversionDetalle(lo_Inversion);
                $("#txtUtilidadAcumulada").val(lo_Inversion.utilidad_acumulada);
                $("#txtRetiro").val(lo_Inversion.montoRetiro);
            })
        })

        $(".tabAccion").click(function () {
            asset.idTabSeleccionado = parseInt($(this).attr('data-id'));
            $('.tabInvertir').tab('show');
            $("#lblFechaVencimiento").html('');
            $("#dvCamposRetiro").hide();
            $("#dvFechaInversion").hide();
            $("#dvFechaActual").hide();
            asset.idStatus = asset.idStatus > 0 ? asset.idStatus : 1;

            if (asset.idTabSeleccionado === 1) {     
                $("#dvFechaActual").show();
            } else if (asset.idTabSeleccionado === 2) {
                $("#dvCamposRetiro").show();
                $("#dvFechaInversion").show();
            }

            $("#comboInversionista").val(asset.idInversionista).change();

            if (asset.idSeleccionado > 0) {
                $("#txtPlazo").keyup();
            }

            asset.bloquearCampos(asset.idTabSeleccionado === 2 || asset.idStatus === 2);
        })

        $("#btnEliminarAceptar").click(function () {
            let params = {};
            params.path = window.location.hostname;
            params.id = asset.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Investors/Investments.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {
                        asset.loadContent();
                    } else {
                        utils.toast(resultado.MensajeError, 'error');
                    }
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        })

        $("#txtPorcentajeUtilidad").keyup(function () {
            asset.calculateUtilidadPesos();
        })

        $("#txtMontoAInvertir").keyup(function () {
            asset.calculateUtilidadPesos();
        })

        $("#txtPlazo").keyup(function () {
            var iPlazo = parseInt($("#txtPlazo").val());
            iPlazo = isNaN(iPlazo) ? 0 : iPlazo;
            var ldt_FechaTermino = moment(Date.now()).add(iPlazo, 'days').format("DD-MM-YYYY");
            $("#lblFechaVencimiento").html(ldt_FechaTermino);
        })

        $("#comboInversionista").change(function () {
            asset.idInversionista = parseInt($("#comboInversionista").val());
            asset.idInversionista = isNaN(asset.idInversionista) ? -1 : asset.idInversionista;

            if (asset.idTabSeleccionado === 1) {
                asset.loadInversionista($(this).val(), function (resultado) {
                    var lo_Inversor = resultado.d;
                    $("#txtPorcentajeUtilidad").val(lo_Inversor.PorcentajeUtilidadSugerida);
                    asset.calculateUtilidadPesos();
                });
            }
            else if (asset.idTabSeleccionado === 2) {
                asset.loadInversiones();
            }
        })

        $("#btnLimpiar").click(function () {
            $('#txtNombreInversionistaBusqueda').val('');
            $('#txtMontoMaximaBusqueda').val('');
            $('#txtMontoMinimoBusqueda').val('');
            $('#txtUtilidadMaximaBusqueda').val('');
            $('#txtUtilidadMinimaBusqueda').val('');
            $('#txtPlazoMaximoBusqueda').val('');
            $('#txtPlazoMinimoBusqueda').val('');
            $('#dtpIngresoMaximoBusqueda').val('');
            $('#dtpIngresoMinimoBusqueda').val('');
            $('#dtpRetiroMaximoBusqueda').val('');
            $('#dtpRetiroMinimoBusqueda').val('');
        });

        $("#btnBuscar").click(function (e) {
            e.preventDefault();
            let params = {};
            params.oRequest = asset.getFilter();
            params.path = window.location.hostname;
            $.ajax({
                type: "POST",
                url: "/pages/Investors/Investments.aspx/Search",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (resultado) {
                    let data = resultado.d;
                    if (data == null) {
                        window.location = "../../pages/Index.aspx";
                    }
                    asset.createTableInvestments(data);
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();
            asset.nuevo();
        });

        $('#btnGuardar').click(function (e) {
            e.preventDefault();
            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;
            if (!hasErrors) {
                let lo_Inversion = {};
                lo_Inversion.id_status_inversion = asset.idStatus;
                lo_Inversion.id_inversion = asset.idTabSeleccionado === 2
                    ? parseInt($("#cboFechaInversionesRetiro").val())
                    : asset.idSeleccionado;
                lo_Inversion.id_inversionista = $("#comboInversionista").val();
                lo_Inversion.inversion_utilidad = $("#txtUtilidadInversion").val();
                lo_Inversion.monto = $("#txtMontoAInvertir").val();
                lo_Inversion.plazo = $("#txtPlazo").val();
                lo_Inversion.porcentaje_utilidad = $("#txtPorcentajeUtilidad").val();
                lo_Inversion.utilidad_pesos = $("#txtUtilidadPesos").val();

                let params = {};
                params.path = window.location.hostname;
                params.item = lo_Inversion;
                params.accion = asset.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Investors/Investments.aspx/Save",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        var valores = msg.d;
                        if (parseInt(valores.CodigoError) == 0) {
                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                            $('.file-comprobante').each(function (documento) {
                                let file;
                                if (file = this.files[0]) {
                                    var ls_TipoComprobante = asset.idTabSeleccionado === 2
                                        ? 'comprobanteRetiro' : 'comprobanteInversion';  
                                    utils.sendFileEmployee(file,
                                        ls_TipoComprobante,
                                        valores.IdItem,
                                        0,
                                        ls_TipoComprobante);
                                }
                            });

                            asset.cancelarGuardadoResgistro();
                            asset.loadContent();
                        } else {
                            utils.toast(mensajesAlertas.errorGuardar, 'fail');
                        }

                        $('#panelEdicion').modal('hide');
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }
                });
            }
        });

        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();
            asset.cancelarGuardadoResgistro();
        });
    }
}

window.addEventListener('load', () => {
    asset.init();
    asset.accionesBotones();
});


