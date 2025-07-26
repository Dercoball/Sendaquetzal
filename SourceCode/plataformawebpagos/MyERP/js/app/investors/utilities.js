'use strict';
let date = new Date();
let descargas = "INVERSIONISTAS_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '51';




const utility = {

    utilidades: [],
    utilidades_fechas: [],

    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        utility.idSeleccionado = -1;
        utility.accion = '';

        utility.loadComboInvestor();
        utility.loadContent();

    },

    loadContent() {

        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idInversionista = document.getElementById('comboInversionista').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/Utilities.aspx/GetListaItems",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;
                //console.log(data);

                //  si no tiene permisos
                if (data == null) {
                    window.location = "../../pages/Index.aspx";
                }

                data = JSON.parse(data);

                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [

                        { data: 'nombre_tipo_movimiento_inversion' },
                        { data: 'fecha' },
                        { data: 'montomx' },
                        { data: 'balancemx' },

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

                if (data && data.length > 0) {
                    utility.generateChart(data);
                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });



    },

    generateChart: (data) => {


        utility.utilidades = data.map((x) => {

            if (x.id_tipo_movimiento_inversion === 3) {
                return x.monto
            }
        });

        //console.log(` utility.utilidades ${JSON.stringify(utility.utilidades)}`);
        utility.utilidades = utility.utilidades.filter(x => x != null);


        utility.utilidades_fechas = data.map((x) => {

            if (x.id_tipo_movimiento_inversion === 3) {
                return x.fecha
            }
        });
        utility.utilidades_fechas = utility.utilidades_fechas.filter(x => x != null);
        //console.log(` utility.utilidades_fechas ${JSON.stringify(utility.utilidades_fechas)}`);


        Highcharts.chart('container_grafica_Utilidades', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Utilidades por inversionista'
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: utility.utilidades_fechas
            },
            yAxis: {
                title: {
                    text: 'Monto'
                }
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: false
                }
            },
            series: [{
                name: 'Utilidades',
                data: utility.utilidades
            }
            ]
        });


    },

    loadComboInvestor: () => {

        var params = {};
        params.path = "connbd";
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









    accionesBotones: () => {

        $('#comboInversionista').on('change', (e) => {
            e.preventDefault();

            utility.loadContent();

        });





    }





}

window.addEventListener('load', () => {

    utility.init();

    utility.accionesBotones();

});


