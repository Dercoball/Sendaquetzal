let date = new Date();
let descargas = "CombustiblesReporte_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '85';




const reporteCombustibles = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        reporteCombustibles.idSeleccionado = -1;
        reporteCombustibles.accion = '';

        reporteCombustibles.fechasHoy();
        reporteCombustibles.cargarComboStatus();
        reporteCombustibles.cargarComboEquipo();
        reporteCombustibles.cargarComboObra();
        reporteCombustibles.cargarComboSolicitadoPor();
        reporteCombustibles.cargarComboTipoCombustible();

        //cargarItems: (fechaInicial, fechaFinal, status, obra, combustible, usuarioSolicita, equipo) => {

        reporteCombustibles.cargarItems();




    },




    cargarItems: () => {

        const fechaInicial = $('#txtFechaInicial').val();
        let fechaFinal = $('#txtFechaFinal').val();

        let status = $('#comboStatus').val();
        let obra = $('#comboObra').val();
        let combustible = $('#comboCombustible').val();
        let usuarioSolicita = $('#comboSolicitadoPor').val();
        let equipo = $('#comboEquipo').val();

        if (fechaFinal == null || fechaFinal === '') {

            fechaFinal = reporteCombustibles.fecha();
            $('#txtFechaFinal').val(fechaFinal);
        }

        if (fechaInicial == null || fechaInicial === '') {

            fechaInicial = reporteCombustibles.fecha();
            $('#txtFechaInicial').val(fechaInicial);
        }

        status = status == null ? "-1" : status;
        obra = obra == null ? "-1" : obra;
        combustible = combustible == null ? "-1" : combustible;
        usuarioSolicita = usuarioSolicita == null ? "-1" : usuarioSolicita;
        equipo = equipo == null ? "-1" : equipo;

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        parametros.fechaInicial = fechaInicial;
        parametros.fechaFinal = fechaFinal;
        parametros.status = status;
        parametros.idObra = obra;
        parametros.idUsuarioSolicita = usuarioSolicita;
        parametros.idEquipo = equipo;
        parametros.idTipoCombustible = combustible;

        let url = '../pages/Combustibles_Reporte.aspx/GetListaItems';

        utils.postData(url, parametros)
            .then(data => {
                //console.log(data); // JSON data parsed by `data.json()` call


                data = data.d;



                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "pageLength": 20,
                    "order": [],
                    data: data,
                    columns: [

                        { data: 'NumeroEconomico' },
                        { data: 'FechaSolicitudFormateadaMx' },
                        { data: 'Orometro' },
                        { data: 'Cantidad' },
                        { data: 'NombreTipoCombustible' },
                        { data: 'NombreCentroCosto' },
                        { data: 'NombreStatus' },
                        { data: 'CintilloAnterior' },
                        { data: 'CintilloActual' },
                        { data: 'NombreUsuarioSolicita' },
                        { data: 'NombreUsuarioEntrega' },
                        { data: 'FechaEntregaFormateadaMx' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "orderable": false
                        },
                    ],
                    dom: 'fBrtipl',
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            title: descargas,
                            text: 'Xls'
                        },
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: 'Csv'
                        },

                        {
                            extend: 'pdfHtml5',
                            title: descargas,
                            text: 'Pdf'
                        }
                    ]

                });

            });



    },


    cargarComboStatus: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Reporte.aspx/GetListaEstatusSolicitudCombustibleReporte",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value="' + -1 + '">Todos</option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id + '">' + item.Nombre + '</option>';

                }

                $('#comboStatus').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },




    cargarComboObra: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Reporte.aspx/GetListaCentrosCostoReporte",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value="' + -1 + '">Todos</option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdCentroCosto + '">' + item.Nombre + '</option>';

                }

                $('#comboObra').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    cargarComboTipoCombustible: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Reporte.aspx/GetTiposCombustibleReporte",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value="' + -1 + '">Todos</option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdTipoCombustible + '">' + item.Nombre + '</option>';

                }

                $('#comboCombustible').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    cargarComboSolicitadoPor: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Reporte.aspx/GetUsuariosSolicitaReporte",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value="' + -1 + '">Todos</option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdUsuario + '">' + item.Nombre + '</option>';

                }

                $('#comboSolicitadoPor').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    cargarComboEquipo: () => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Combustibles_Reporte.aspx/GetListaEquiposReporte",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";
                opcion += '<option value="' + -1 + '">Todos</option>';

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdEquipo + '">' + item.NumeroEconomico + '</option>';

                }

                $('#comboEquipo').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    fechasHoy() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        $('#txtFechaInicial').val(`${today.getFullYear()}-${month}-${dayMonth}`);
        $('#txtFechaFinal').val(`${today.getFullYear()}-${month}-${dayMonth}`);

    },



    fecha() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;


    },


    accionesBotones: () => {


        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            reporteCombustibles.cargarItems();

        });


    }


}

window.addEventListener('load', () => {

    reporteCombustibles.init();

    reporteCombustibles.accionesBotones();

});


