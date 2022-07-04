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
        report.adelantoEntrada = '';
        report.adelantoSalida = '';
        report.totalDeEntregar = '';
        report.subtotal = '';
        report.totalRecuperacion = '';


        $('#txtFechaSemana').val(report.fechaHoy());

        report.loadComboPlaza();

        report.loadComboEjecutivosByPlaza("-1", '#comboEjecutivo');

        $('.secciones').hide();
        $('.reporteFalla').hide();
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


    getDataDebeEntregarYFalla(idPromotor, fechaInicial, fechaFinal) {


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
                report.subtotal = data[0].total - data[1].total;
                $(`#cell_subtotal`).text(number_format(report.subtotal, 2, '$'));


                report.getTotalByStatusPayment(idPromotor, 3, report.fechaInicial, report.fechaFinal, '#cell_totalRecuperacion');


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

                report.totalRecuperacion = data.total;

                $(`${htmlControl}`).text(data.totalStr);

                report.getAdelantoEntrada(idPromotor, report.fechaInicial, report.fechaFinal, '#cell_totalAdelantoEntrada');


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

                $('.reporteFalla').show();
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


    getTableFalla(idPromotor, idStatus, fechaInicial, fechaFinal) {


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
            url: "../../pages/Reports/Reports.aspx/GetPaymentsByStatusFallaAndPromotor",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;


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

                $('#tableFalla tbody').empty().append(html);




            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    getTableRecuperado(idPromotor, idStatus, fechaInicial, fechaFinal) {


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
            url: "../../pages/Reports/Reports.aspx/GetPaymentsByStatusFallaRecuperado",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;


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

                $('#tableRecuperado tbody').empty().append(html);




            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },

    getTableAdelantoEntrada(idPromotor, idStatus, fechaInicial, fechaFinal) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetPaymentsByStatusAdelantoEntrante",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;


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

                $('#tableAdelantoEntrante tbody').empty().append(html);




            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    getTableAdelantoSaliente(idPromotor, idStatus, fechaInicial, fechaFinal) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetPaymentsByStatusAdelantoSaliente",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;


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

                $('#tableAdelantoSaliente tbody').empty().append(html);




            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    getAdelantoEntrada(idPromotor, fechaInicial, fechaFinal, htmlControl) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetTotalByStatusAndPromotorAndSemanaEntrante",
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
                report.adelantoEntrada = data.total;


                report.getAdelantoSaliente(idPromotor, report.fechaInicial, report.fechaFinal, '#cell_totalAdelantoSalida');

                report.totalDeEntregar = report.subtotal + report.totalRecuperacion + report.adelantoEntrada - report.adelantoSalida;

                $('#cell_totalEntregar').text(number_format(report.totalDeEntregar, 2, '$'));


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    getAdelantoSaliente(idPromotor, fechaInicial, fechaFinal, htmlControl) {


        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPromotor = idPromotor;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Reports/Reports.aspx/GetTotalByStatusAndPromotorAndSemanaSaliente",
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
                report.adelantoSalida = data.total;





            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },

    accionesBotones: () => {

        function getPDF() {

            var HTML_Width = $("#divReporteFalla").width();
            var HTML_Height = $("#divReporteFalla").height();
            var top_left_margin = 15;
            var PDF_Width = HTML_Width + (top_left_margin * 2);
            var PDF_Height = (PDF_Width * 1.5) + (top_left_margin * 2);
            var canvas_image_width = HTML_Width;
            var canvas_image_height = HTML_Height;

            var totalPDFPages = Math.ceil(HTML_Height / PDF_Height) - 1;


            html2canvas($("#divReporteFalla")[0], { allowTaint: true }).then(function (canvas) {
                canvas.getContext('2d');

                console.log(canvas.height + "  " + canvas.width);


                var imgData = canvas.toDataURL("image/jpeg", 1.0);
                var pdf = new jsPDF('p', 'pt', [PDF_Width, PDF_Height]);
                pdf.addImage(imgData, 'JPG', top_left_margin, top_left_margin, canvas_image_width, canvas_image_height);


                for (var i = 1; i <= totalPDFPages; i++) {
                    pdf.addPage(PDF_Width, PDF_Height);
                    pdf.addImage(imgData, 'JPG', top_left_margin, -(PDF_Height * i) + (top_left_margin * 4), canvas_image_width, canvas_image_height);
                }

                pdf.save("HTML-Document.pdf");


                $('.deshabilitable').prop('disabled', false);
                $('#btnGuardar').html(`<i class="fa fa-save mr-1"></i>Guardar`);

                $('.reporteFalla').hide();
                $('#divLoading').hide();

            });



            


        };


        $('#btnGuardar').on('click', (e) => {
            e.preventDefault();

            $('.deshabilitable').prop('disabled', true);
            $('#btnGuardar').html(`<i class="fa fa-paper-plane mr-1"></i>Guardando...`);

            let params = {};
            params.path = window.location.hostname;
            params.fecha = $('#txtFechaSemana').val();
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);


            $.ajax({
                type: "POST",
                url: `../../pages/Reports/Reports.aspx/SaveReport`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;

                  

                    //  si no tiene permisos
                    if (valores == null) {
                        window.location = "../../pages/Index.aspx";
                    }

                    if (parseInt(valores.CodigoError) === 0) {

                        $('#txtFolio').val(valores.IdItem);

                        let time_ = 5000;
                        setTimeout(function () {

                            setTimeout(function () {

                                getPDF();

                            }, time_);

                        }, time_);



                    } else {

                        $('.deshabilitable').prop('disabled', false);

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                        return;

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {

                    utils.toast(mensajesAlertas.errorGuardar, 'error');

                }

            });

        });



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



            //let idPromotor = $('#comboPromotor').val();
            let idPromotor = 33;    //  TODO: Solo para test

            if (!idPromotor) {

                utils.toast(mensajesAlertas.errorSeleccionarPromotor, 'error');
                return;
            }

            $('.reporteFalla').hide();
            $('#divLoading').show();

            $('#txtEjecutivo').val($('#comboEjecutivo option:selected').text());
            $('#txtPromotor').val($('#comboPromotor option:selected').text());

            console.log(`dia =  ${$('#txtFechaSemana').val().split('-')[2]}`);

            let formatedDate = `${$('#txtFechaSemana').val().split('-')[2]}/${$('#txtFechaSemana').val().split('-')[1]}/${$('#txtFechaSemana').val().split('-')[0]}`;

            report.fechasHoy(`${$('#txtFechaSemana').val().split('-')[0]}`, `${$('#txtFechaSemana').val().split('-')[1]}`, `${$('#txtFechaSemana').val().split('-')[2]}`);

            $('#txtFecha').val(formatedDate);

            report.getDataDebeEntregarYFalla(idPromotor, report.fechaInicial, report.fechaFinal);

            report.GetTotalLoanByPromotor(idPromotor, report.fechaInicial, report.fechaFinal, '#cell_totalVenta');


            report.getTableSemanaExtra(idPromotor, utils.STATUS_PAGO_PAGADO, report.fechaInicial, report.fechaFinal);

            report.getTableFalla(idPromotor, utils.STATUS_PAGO_PAGADO, report.fechaInicial, report.fechaFinal);

            report.getTableRecuperado(idPromotor, utils.STATUS_PAGO_ABONADO, report.fechaInicial, report.fechaFinal);

            report.getTableAdelantoEntrada(idPromotor, utils.STATUS_PAGO_ABONADO, report.fechaInicial, report.fechaFinal);

            report.getTableAdelantoSaliente(idPromotor, utils.STATUS_PAGO_ABONADO, report.fechaInicial, report.fechaFinal);




        });

    }


}

window.addEventListener('load', () => {

    report.init();

    report.accionesBotones();

});


