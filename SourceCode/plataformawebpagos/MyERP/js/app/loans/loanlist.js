'use strict';
let date = new Date();
let descargas = "Prestamo_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const loans = {
    init: () => {
        loans.loadPrestamos();
        loans.loadStatus();
    },
    getFilter: () => {
        var oRequest =
        {
            Status: $("#cboStatus option:selected").val(),
            Nombre: $("#txtNombreClienteBusqueda").val(),
            NoPrestamoMinimo: $("#txtNoPrestamoMinimoBusqueda").val(),
            NoPrestamoMaximo: $("#txtNoPrestamoMaximaBusqueda").val(),
            RechazoMinimo: $("#txtRechazosPrestamoMinimoBusqueda").val(),
            RechazosMaximo: $("#txtRechazosPrestamoMaximaBusqueda").val(),
            AvalMinimo: $("#txtAvalPrestamoMinimoBusqueda").val(),
            AvalMaximo: $("#txtAvalPrestamoMaximaBusqueda").val(),
            FechaPrimerSolicitudMinimo: moment($("#dtpFechaPrestamoMinimoBusqueda").val()).isValid()
                ? moment($("#dtpFechaPrestamoMinimoBusqueda").val()).format('YYYY-MM-DD')
                : null,
            FechaPrimerSolicitudMaximo: moment($("#dtpFechaPrestamoMaximaBusqueda").val()).isValid()
                ? moment($("#dtpFechaPrestamoMaximaBusqueda").val()).format('YYYY-MM-DD')
                : null,
            FechaUltimaSolicitudMaximo: moment($("#dtpFechaUltimaPrestamoMinimoBusqueda").val()).isValid()
                ? moment($("#dtpFechaUltimaPrestamoMinimoBusqueda").val()).format('YYYY-MM-DD')
                : null,
            FechaUltimaSolicitudMinimo: moment($("#dtpFechaUltimaPrestamoMaximaBusqueda").val()).isValid()
                ? moment($("#dtpFechaUltimaPrestamoMaximaBusqueda").val()).format('YYYY-MM-DD')
                : null
        };

        return oRequest;
    },
    loadStatus: () => {
        var params = {};
        params.path = "connbd";
        params = JSON.stringify(params);

        $('#cboStatus').html('');

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanRequest.aspx/GetStatus",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var llst_Status = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < llst_Status.length; i++) {
                    let oStatus = llst_Status[i];
                    opcion += `<option value = '${oStatus.IdStatusPrestamo}' > ${oStatus.Nombre}</option > `;
                }

                $('#cboStatus').html(opcion);
                    
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    loadPrestamos: () => {
        let params = {};
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.path = "connbd";

        $.ajax({
            type: "POST",
            url: "/pages/Loans/LoanRequest.aspx/GridPrestamos",
            data: JSON.stringify(params),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (resultado) {
                let data = resultado.d;
                if (data == null) {
                    window.location = "/pages/Index.aspx";
                }
                loans.createTableLoans(data);
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });
    },
    createTableLoans: (data) => {
        let table = $('#table').DataTable({
            footerCallback: function (row, data, start, end, display) {
                var lf_TotalMontos = 0;;

                $.each(data, function (index, oInversion) {
                    lf_TotalMontos += oInversion.monto;
                })

                $("#thMontoTotal").html("$" + lf_TotalMontos.toLocaleString('es-MX'));
            },
            destroy: true,
            processing: true,
            order: [],
            searching: false,
            pagingType: 'full_numbers',
            bLengthChange: false,
            data: data,
            columns: [
                { data: 'id_prestamo', className: 'text-center' },
                { data: 'NoPrestamos' ,className: 'text-center' },
                { data: 'nombreCliente' },
                {
                    data: 'monto', className: 'text-center', render: function (datum, type, row) {
                        return '$ ' + row.monto.toLocaleString('es-MX')
                    }
                },
                {
                    data: 'fecha_primera_solicitud', className: 'text-center', render: function (datum, type, row) {
                        return moment(row.fecha_primera_solicitud).format('DD-MM-YYYY');
                    }
                },
                {
                    data: 'fecha_ultima_solicitud', className: 'text-center', render: function (datum, type, row) {
                        return moment(row.fecha_ultima_solicitud).format('DD-MM-YYYY');
                    }
                },
                { data: 'NoRechazados', className: 'text-center' },
                { data: 'Aval', className: 'text-center' },
                {
                    data: 'Status', className: 'text-center', render: function (datam, type, row) {
                        return "<span class='" + row.ColorStatus +" rounded text-white p-2'>" + row.Status + "</span>";
                    }
                },
                {
                    data: '', className: 'text-center', render: function (datum, type, row) {
                        return "<a  class='btn btn-success rounded' href='/pages/Loans/LoanApprove.aspx?id=" + row.id_prestamo + "'><i class='fa fa-edit'></i></a>";
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
    accionesBotones: () => {
        $("#btnLimpiar").click(function () {
            $("#frmFiltros")[0].reset();
        });

        $("#btnBuscar").click(function (e) {
            e.preventDefault();
            let params = {};
            params.Filtro = loans.getFilter();
            params.path = "connbd";
            $.ajax({
                type: "POST",
                url: "/pages/Loans/LoanRequest.aspx/Search",
                data: JSON.stringify(params),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (resultado) {
                    let data = resultado.d;
                    if (data == null) {
                        window.location = "/pages/Index.aspx";
                    }
                    loans.createTableLoans(data);
                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }
            });
        });
    }
}

window.addEventListener('load', () => {
    loans.init();
    loans.accionesBotones();
});