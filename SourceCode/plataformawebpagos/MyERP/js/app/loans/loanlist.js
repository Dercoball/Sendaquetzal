'use strict';
let date = new Date();
let descargas = "Prestamo_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const loans = {
    init: () => {
        
    },
    createTableInvestments: (data) => {
        let table = $('#table').DataTable({
            footerCallback: function (row, data, start, end, display) {
                //var lf_TotalMontos = 0;;

                //$.each(data, function (index, oInversion) {
                //    lf_TotalMontos += oInversion.monto;
                //})

                //$("#thMontoTotalInversion").html("$" + lf_TotalMontos.toFixed(2));
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
                { data: 'plazo', className: 'text-center' },
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
    accionesBotones: () => {

    }
}

window.addEventListener('load', () => {
    loans.init();
    loans.accionesBotones();
});