'use strict';
let date = new Date();
let descargas = "Reportes" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '19';


const report = {


    init: () => {


        report.idSolicitud = -1;
        report.idEmpleado = -1;
        report.idComision = -1;
        report.NombreComision = '';
        report.NombreEmpleado = '';
        report.fechaInicial = '';
        report.fechaFinal = '';

        report.totalDebeEntregar = '';
        report.totalFalla = '';
        report.porcentajeComision = '';
        report.comision = '';
        report.venta = '';

        $('#txtFechaSemana').val(report.fechaHoy());

        report.loadComboPlaza();

        report.loadComboEjecutivosByPlaza("-1", '#comboEjecutivo');

        $('.secciones').hide();
        $('#divReporteFalla').hide();
        $('#divLoading').hide();
        $('#panelTabla').show();

    },

    fechaHoy() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;

    },

    fechasHoy(yearSelected, monthSelected, daySelected) {
        console.log('fechasHoy');
        monthSelected--;

        //  fecha (final)

        let endWeekDay = new Date(yearSelected, monthSelected, daySelected);
        let end_ = endWeekDay.getDay() + 1

        endWeekDay.setDate(endWeekDay.getDate() + 7 - end_ + 1);

        let dayMonth = endWeekDay.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;

        let month = (endWeekDay.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        report.fechaFinal = `${endWeekDay.getFullYear()}-${month}-${dayMonth}`;

        //  fecha inicial
        let startWeekDay = new Date(yearSelected, monthSelected, daySelected);
        startWeekDay.setDate(startWeekDay.getDate() - startWeekDay.getDay() + 1);

        let startDayMonth = startWeekDay.getDate();
        startDayMonth = startDayMonth.toString().length === 1 ? `0${startDayMonth}` : startDayMonth;

        let startMonth = (startWeekDay.getMonth() + 1);
        startMonth = startMonth.toString().length === 1 ? `0${startMonth}` : startMonth;

        let startYear = (startWeekDay.getFullYear());

        report.fechaInicial = `${startYear}-${startMonth}-${startDayMonth}`;


        console.log(`fechaInicial ${report.fechaInicial}`);
        console.log(`fechaFinal ${report.fechaFinal}`);

    },

    loadComboEjecutivosByPlaza: (idPlaza, control) => {

        var params = {};
        params.path = window.location.hostname;
        params.idPlaza = idPlaza;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetListaEjecutivosByPlaza",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $(`${control}`).html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    loadComboSupervisoresByEjecutivo: (idEjecutivo, control) => {

        var params = {};
        params.path = window.location.hostname;
        params.idEjecutivo = idEjecutivo;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetListaSupervisoresByEjecutivo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $(`${control}`).html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    loadComboPromotoresBySupervisor: (idSupervisor, control) => {

        var params = {};
        params.path = window.location.hostname;
        params.idSupervisor = idSupervisor;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetListaPromotoresBySupervisor",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $(`${control}`).html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    loadComboPlaza: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Employees.aspx/GetListaItemsPlazas",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdPlaza}' > ${item.Nombre}</option > `;

                }

                $('#comboPlaza').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    getData(idPromotor, fechaInicial, fechaFinal) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetTotals",
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

                console.log(data);

                $(`#cell_totalDebeEntregar`).text(data[0].totalStr);
                $(`#cell_totalFalla`).text(data[1].totalStr);

                //  calcular subtotal
                $(`#cell_subtotal`).text(number_format(data[0].total - data[1].total, 2, '$'));



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },



    getTotalByStatusPayment(idPromotor, idStatusPago, fechaInicial, fechaFinal, htmlControl) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.idStatusPago = idStatusPago;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetTotalByPaymentStatus",
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

                console.log(data);

                $(`${htmlControl}`).text(data.totalStr);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },



    GetTotalLoanByPromotor(idPromotor, fechaInicial, fechaFinal, htmlControl) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetTotalLoanByPromotor",
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



                $(`${htmlControl}`).text(data.totalStr);
                report.venta = data.total;

                //  Datos de la comision, necesarios en este punto para calcular la comisión a pagar
                report.getPromotorWithCommissionData(idPromotor);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    getPromotorWithCommissionData: (id) => {

        let params = {};
        params.path = window.location.hostname;
        params.id = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetPromotorDataById",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;

                //console.log(`Promotor data = ${item}`);

                $(`#cell_porcentajeComision`).text(`${item.PorcentajeComision}%`);
                report.porcentajeComision = item.PorcentajeComision;

                report.comision = report.venta * item.PorcentajeComision / 100;

                $(`#cell_totalComision`).text(`${number_format(report.comision, 2, '$')}`);


                //  final de generacion del reporte
                $('#divReporteFalla').show();
                $('#divLoading').hide();


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },

    getSubtotal(totalEntregar, totalFalla, htmlControl) {


        let params = {};
        params.path = window.location.hostname;
        params.totalEntregar = totalEntregar;
        params.totalFalla = totalFalla;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetSubtotal",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;

                $(`${htmlControl}`).text(data.totalStr);

                return data.total;


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    getTableSemanaExtra(idPromotor, idStatus, fechaInicial, fechaFinal) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.idStatus = idStatus;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetPaymentsByStatusAndPromotorAndSemanaExtra",
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

                console.log(data);

                let html = '';
                let total = 0;
                data.forEach((item, i) => {
                    html += `<tr>`;
                    html += `<td>${item.FechaStr}</td>`;
                    html += `<td>${item.NombreCliente}</td>`;
                    html += `<td>${item.MontoFormateadoMx}</td>`;
                    html += `</tr>`;

                    total += item.Monto;
                });

                html += `<tr>`;
                html += `<th></th>`;
                html += `<th>Total</th>`;
                html += `<th>${number_format(total, 2, '$')}</th>`;
                html += `</tr>`;

                $('#tableSemanaExtra tbody').empty().append(html);




            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },

    accionesBotones: () => {


        $('#comboPlaza').on('change', (e) => {
            e.preventDefault();

            report.loadComboEjecutivosByPlaza($('#comboPlaza').val(), '#comboEjecutivo');

        });

        $('#comboEjecutivo').on('change', (e) => {
            e.preventDefault();

            report.loadComboSupervisoresByEjecutivo($('#comboEjecutivo').val(), '#comboSupervisor');

        });

        $('#comboSupervisor').on('change', (e) => {
            e.preventDefault();

            report.loadComboPromotoresBySupervisor($('#comboSupervisor').val(), '#comboPromotor');

        });
        $('#btnReporteFalla').on('click', async (e) => {
            e.preventDefault();

            console.log('btnReporteFalla');

            $('#divReporteFalla').hide();
            $('#divLoading').show();

            //let idPromotor = $('#comboPromotor').val();
            let idPromotor = 33;    //  TODO: Solo para test

            if (!idPromotor) {

                utils.toast(mensajesAlertas.errorSeleccionarPromotor, 'error');
                return;
            }

            $('#txtEjecutivo').val($('#comboEjecutivo option:selected').text());
            $('#txtPromotor').val($('#comboPromotor option:selected').text());

            let formatedDate = `${$('#txtFechaSemana').val().split('-')[2]}/${$('#txtFechaSemana').val().split('-')[1]}/${$('#txtFechaSemana').val().split('-')[0]}`;

            let date = $('#txtFechaSemana').val();
            report.fechasHoy(`${$('#txtFechaSemana').val().split('-')[0]}`, `${$('#txtFechaSemana').val().split('-')[1]}`, `${$('#txtFechaSemana').val().split('-')[2]}`);

            $('#txtFecha').val(formatedDate);

            report.getData(idPromotor, report.fechaInicial, report.fechaFinal);
            report.getTotalByStatusPayment(idPromotor, 3, report.fechaInicial, report.fechaFinal, '#cell_totalRecuperacion');
            report.GetTotalLoanByPromotor(idPromotor, report.fechaInicial, report.fechaFinal, '#cell_totalVenta');
            report.getTableSemanaExtra(idPromotor, utils.STATUS_PAGO_PAGADO, report.fechaInicial, report.fechaFinal, '#cell_totalVenta');

            return;

            var element = document.getElementById('divReporteFalla');
            html2pdf(element);

        });

    }


}

window.addEventListener('load', () => {

    report.init();

    report.accionesBotones();

});


