'use strict';
let date = new Date();
let descargas = "INVERSIONES_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '55';




const investmentDashboard = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        investmentDashboard.idSeleccionado = -1;
        investmentDashboard.accion = '';

        investmentDashboard.loadContent();

    },

    loadContent() {

        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Investors/InvestmentsDashboard.aspx/GetListaItems",
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

                var series_ = [];

                for (var i = 0; i < data.length; i++) {

                    series_.push({
                        name: data[i].Inversionista.Nombre,
                        y: data[i].Monto
                    });

                }

                window.chartColors = {
                    red: 'rgb(255, 99, 132)',
                    orange: 'rgb(255, 159, 64)',
                    yellow: 'rgb(255, 205, 86)',
                    green: 'rgb(75, 192, 192)',
                    blue: 'rgb(54, 162, 235)',
                    purple: 'rgb(153, 102, 255)',
                    grey: 'rgb(201, 203, 207)'
                };


                console.log(`${JSON.stringify(series_)}`);

                var meses_ = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'];

                Highcharts.chart('container_grafica_InversionDisponible', {
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: window.chartColors.blue,
                        plotShadow: false,
                        type: 'pie'
                    },
                    title: {
                        text: ''
                    },

                    tooltip: {
                        pointFormat: '<b>{series.name}</b>'
                    },
                    credits: {
                        enabled: false
                    },
                    plotOptions: {
                        pie: {
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: true,
                                format: '{point.y}'
                            },
                            showInLegend: true
                        }
                    },
                    series: [{
                        name: 'Inversión total',
                        colorByPoint: true,
                        data: series_


                    }]
                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });



    },


    accionesBotones: () => {




    }


}

window.addEventListener('load', () => {

    investmentDashboard.init();

    investmentDashboard.accionesBotones();

});


