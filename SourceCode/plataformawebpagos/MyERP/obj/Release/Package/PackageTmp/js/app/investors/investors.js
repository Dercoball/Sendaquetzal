'use strict';
let date = new Date();
let descargas = "INVERSIONISTAS_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '51';

const investor = {
    init: () => {
        $('#panelTabla').show();
        $('#panelForm').hide();

        investor.idSeleccionado = -1;
        investor.accion = '';
        investor.loadContent();
    },
    loadContent() {
        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investors.aspx/GetListaItems",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let data = msg.d;
                if (data == null) {
                    window.location = "../../pages/Index.aspx";
                }
                investor.createInvestorsList(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    createInvestorsList: (data) => {
        let table = $('#table').DataTable({
            "destroy": true,
            "processing": true,
            "order": [],
            searching: false,
            pagingType: 'full_numbers',
            bLengthChange: false,
            data: data,
            columns: [
                { data: 'IdInversionista' },
                { data: 'Nombre' },
                { data: 'RFC' },
                {
                    data: 'PorcentajeUtilidadSugerida', className: 'text-center', render: function (datum, type, row) {
                        return row.PorcentajeUtilidadSugerida + ' %';
                    }
                },
                {
                    data: 'Status', className: 'text-center', render: function (datum, type, row) {
                        return row.Status ? "<h2><span class='badge badge-primary p-2'>Activo</span></h2>"
                            : "<h2><span class='badge badge-warning p-2'>Inactivo</span></h2>";
                    }
                },
                {
                    data: 'FechaRegistro', className: 'text-center', render: function (datum, type, row) {
                        return moment(row.FechaRegistro).format('DD-MM-YYYY');
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
    suspender: () => {
        let params = {};
        params.path = "connbd";
        params.id = investor.idSeleccionado;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.status = false; 
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investors.aspx/Suspender",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var resultado = msg.d;
                if (resultado.MensajeError === null) {
                    utils.toast('El inversionista fue suspendido correctamente', 'ok');
                    $('#panelTabla').show();
                    $('#panelForm').hide();
                    investor.loadContent();
                } else {
                    utils.toast(resultado.MensajeError, 'error');
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    delete: (id) => {
        investor.idSeleccionado = id;
        $('#mensajeEliminar').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');
    },
    edit: (id) => {
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#frm')[0].reset();
        $("#btnSuspender").show();

        let params = {};
        params.path = "connbd";
        params.id = id;
        console.log(id);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Investors.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let lo_Inversionista = msg.d;
                investor.idSeleccionado = lo_Inversionista.IdInversionista;
                $('#txtNombre').val(lo_Inversionista.Nombre);
                $('#txtRazonSocial').val(lo_Inversionista.RazonSocial);
                $('#txtRFC').val(lo_Inversionista.RFC);
                $('#txtPorcentajeUtilidadSugerida').val(lo_Inversionista.PorcentajeUtilidadSugerida);

                $('#panelTabla').hide();
                $('#panelForm').show();

                investor.accion = "editar";
                $('#spnTituloForm').text('EDITAR INVERSIONISTA');
                $('.deshabilitable').prop('disabled', false);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    nuevo: () => {
        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('REGISTRAR INVERSIONISTA');
        $('#panelTabla').hide();
        $('#panelForm').show();
        $("#btnSuspender").hide();
        investor.accion = "nuevo";
        investor.idSeleccionado = -1;
        $('.deshabilitable').prop('disabled', false);
    },
    accionesBotones: () => {
        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();
            investor.nuevo();
        });

        $("#btnModalSuspender").click(function (e) {
            e.preventDefault();
            $('#mensajeSuspender').text('Se suspendera el inversionista ' + $("#txtNombre").val() + '. ¿Desea continuar ?');
            $('#panelSuspender').modal('show');
        })

        $('#btnSuspenderAceptar').on('click', (e) => {
            e.preventDefault();
            investor.suspender();
        });

        $('#btnGuardar').click(function (e) {
            e.preventDefault();
            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;
            if (!hasErrors) {
                //  Objeto con los valores a enviar
                let lo_Inversionista = {};
                lo_Inversionista.IdInversionista = investor.idSeleccionado;
                lo_Inversionista.Nombre = $('#txtNombre').val();
                lo_Inversionista.RazonSocial = $('#txtRazonSocial').val();
                lo_Inversionista.RFC = $('#txtRFC').val();
                lo_Inversionista.Status = true;
                lo_Inversionista.PorcentajeUtilidadSugerida = $('#txtPorcentajeUtilidadSugerida').val();

                let params = {};
                params.path = "connbd";
                params.item = lo_Inversionista;
                params.accion = investor.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Investors/Investors.aspx/Save",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);
                        if (resultado > 0) {
                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');
                            $('#panelTabla').show();
                            $('#panelForm').hide();
                            investor.loadContent();
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
        });

        $('#btnEliminarAceptar').on('click', (e) => {
            let params = {};
            params.path = "connbd";
            params.id = investor.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Investors/Investors.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {
                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');
                        investor.loadContent();
                    } else {
                        utils.toast(resultado.MensajeError, 'error');
                    }
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });

        $("#btnBuscar").click(function (e) {
            e.preventDefault();
            var oRequest =
            {
                NombreBusqueda: $("#txtNombreBusqueda").val(),
                RFCBusqueda: $("#txtRFCBusqueda").val(),
                UtilidadMaximaBusqueda: $("#txtUtilidadMaximaBusqueda").val(),
                UtilidadMinimaBusqueda: $("#txtUtilidadMinimaBusqueda").val(),
                FechaRegistroMaximaBusqueda: moment($("#txtFechaRegistroMaximaBusqueda").val()).isValid()
                    ? moment($("#txtFechaRegistroMaximaBusqueda").val()).format('YYYY-MM-DD')
                    : null,
                FechaRegistroMinimaBusqueda: moment($("#txtFechaRegistroMinimaBusqueda").val()).isValid()
                    ? moment($("#txtFechaRegistroMinimaBusqueda").val()).format('YYYY-MM-DD')
                    : null
            };

            let params = {};
            params.oRequest = oRequest;
            params.path = "connbd";
            $.ajax({
                type: "POST",
                url: "/pages/Investors/Investors.aspx/SearchInvestor",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (resultado) {
                    let data = resultado.d;
                    if (data == null) {
                        window.location = "../../pages/Index.aspx";
                    }
                    investor.createInvestorsList(data);
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });
    }
}

window.addEventListener('load', () => {
    investor.init();
    investor.accionesBotones();
});