'use strict';
let date = new Date();
let descargas = "INVERSIONES_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '52';

const asset = {
    init: () => {
        $('#panelTabla').show();
        $('#panelForm').hide();
        $("#nvMenuInversiones").hide();
        $("#lblFechaActual").html(moment(Date.now()).format("DD-MM-YYYY"));
        asset.idSeleccionado = -1;
        asset.accion = '';

        asset.loadContent();
        asset.loadComboInvestor();
        asset.loadComboPeriods();
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
        $("#txtUtilidad").val(lf_UtilidadPesos.toFixed(2));
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
                { data: 'plazo' },
                {
                    data: 'Estatus.id_status_inversion', className: 'text-center', render: function (datum, type, row) {
                        return "<h2><span class='" + row.Estatus.color + " p-2'>" + row.Estatus.nombre + "</span></h2>";
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
    loadComboPeriods: () => {
        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investments.aspx/GetListaItemsPeriodos",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];
                    opcion += `<option value = '${item.IdPeriodo}' > ${item.ValorPeriodo}</option > `;
                }
                $('#comboPeriodos').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    nuevo: () => {
        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#panelTabla').hide();
        $('#panelForm').show();
        $("#nvMenuInversiones").show();
        asset.accion = "nuevo";
        asset.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);
    },
    delete: (id) => {
        asset.idSeleccionado = id;
        $('#mensajeEliminar').text(`Esta seguro que dese eliminar la inversión ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');
    },
    edit: (id) => {
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#frm')[0].reset();
        $('#panelTabla').hide();
        $('#panelForm').show();
        $("#nvMenuInversiones").show();
        asset.accion = "edicion";
        asset.idSeleccionado = -1;

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
            success: function (msg) {
                let lo_Inversion = msg.d;
                asset.idSeleccionado = lo_Inversion.id_inversion;
                $("#comboInversionista").val(lo_Inversion.id_inversionista);
                $("#txtUtilidadInversion").val(lo_Inversion.inversion_utilidad);
                $("#txtMontoAInvertir").val(lo_Inversion.monto);
                $("#txtPlazo").val(lo_Inversion.plazo).keyup();
                $("#txtPorcentajeUtilidad").val(lo_Inversion.porcentaje_utilidad);
                $("#txtUtilidadPesos").val(lo_Inversion.utilidad_pesos);
                $("#txtUtilidad").val(lo_Inversion.utilidad_pesos);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    accionesBotones: () => {
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
            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.id = $(this).val();
            params = JSON.stringify(params);
            $("#txtUtilidadInversion").val('');

            $.ajax({
                type: "POST",
                url: "/pages/Investors/Investments.aspx/GetDataInvestor",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var lo_Inversor = msg.d;
                    $("#txtPorcentajeUtilidad").val(lo_Inversor.PorcentajeUtilidadSugerida);
                    asset.calculateUtilidadPesos();
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        })
        $("#btnLimpiar").click(function () {
            $("#txtNombreInversionistaBusqueda").val('');
            $("#txtMontoMinimoBusqueda").val('');
            $("#txtMontoMaximaBusqueda").val('');
            $("#txtUtilidadMinimaBusqueda").val('');
            $("#txtUtilidadMaximaBusqueda").val('');
            $("#txtPlazoMinimoBusqueda").val('');
            $("#txtPlazoMaximoBusqueda").val('');
            $("#dtpIngresoMinimoBusqueda").val('');
            $("#dtpIngresoMaximoBusqueda").val('');
            $("#dtpRetiroMinimoBusqueda").val('');
            $("#dtpRetiroMaximoBusqueda").val('');
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
                //  Objeto con los valores a enviar
                let lo_Inversion = {};
                var filecomprobante = $(".file-comprobante");

                if (filecomprobante.prop('files').length <= 0) {
                    var lo_reader = new FileReader();
                    lo_reader.readAsDataURL(filecomprobante.prop('files')[0]);
                    lo_reader.onload = function () {
                        lo_Inversion.comprobante = lo_reader.rresult;
                    };
                }

                lo_Inversion.id_inversion = asset.idSeleccionado;
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
                                    utils.sendFileEmployee(file, 'comprobanteInversion', valores.IdItem, 0, "comprobanteInversion");
                                }
                            });

                            $('#panelTabla').show();
                            $('#panelForm').hide();
                            $("#nvMenuInversiones").hide();
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
            $('#panelTabla').show();
            $('#panelForm').hide();
            $("#nvMenuInversiones").hide();
        });
    }
}

window.addEventListener('load', () => {
    asset.init();
    asset.accionesBotones();
});


