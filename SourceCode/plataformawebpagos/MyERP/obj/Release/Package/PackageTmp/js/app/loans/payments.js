'use strict';
let date = new Date();
let descargas = "Pagos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '15';


const payments = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();


        payments.idSeleccionado = "-1";
        payments.idTipoUsuario = "-1";
        payments.idPago = "-1";
        payments.accion = "";
        payments.idPrestamo = "-1";
        payments.numeroSemana = "-1";

        payments.fechaInicial = '';
        payments.fechaFinal = '';

        payments.fechasHoy();

        payments.cargarItems();


    },

    cargarItems: () => {

        let status = document.querySelector('input[name="filtroPagos"]:checked').value

        status = status == null ? "-1" : status;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.fechaInicial = payments.fechaInicial;
        params.fechaFinal = payments.fechaFinal;
        params.idStatus = status;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/Payments.aspx/GetListaItems",
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

                let table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'IdPago' },
                        { data: 'NumeroSemana' },
                        { data: 'NombreCliente' },
                        { data: 'MontoFormateadoMx' },
                        { data: 'FechaStr' },
                        { data: 'TotalFallaFormateadoMx' },
                        { data: 'SemanasFalla' },
                        { data: 'Status' },
                        { data: 'Accion' }


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        }
                    ],
                    dom: 'fBrtipl',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Csv', className: 'csvbtn'
                        },
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


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },




    view(idPago) {

        console.log(idPago);

        $('#frmPago')[0].reset();

        //  traer datos del pago e historial del préstamo
        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPago = idPago;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/Payments.aspx/GetPayment",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;
                console.log(data);

                $('#txtCliente').val(data.NombreCliente);
                $('#txtCliente').attr('data-idcliente', data.IdCliente);
                $('#txtCliente').attr('data-idprestamo', data.IdPrestamo);

                $('#txtSaldo').val(data.MontoFormateadoMx);
                $('#txtAbono').val(data.Monto);

                $('#panelTabla').hide();
                $('#panelForm').show();
                payments.idPago = data.IdPago;

                payments.idPrestamo = data.IdPrestamo;
                payments.numeroSemana = data.NumeroSemana;

                payments.historial(data.IdPrestamo, data.NumeroSemana);

                if (Number(data.IdStatusPago) === 1) {
                    $('#btnCapturar').show();
                } else {
                    $('#btnCapturar').hide();
                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });

    },

    historial: (idPrestamo, numeroSemanaActual) => {

        console.log(`Historial  idPrestamo ${idPrestamo}`);

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.idPrestamo = idPrestamo;
        params.numeroSemanaActual = numeroSemanaActual;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/Payments.aspx/GetPaymentsByIdPrestamo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let data = msg.d;

                //console.log(data);
                let headers = '';

                let rows = '';


                for (var i = 0; i < data.length; i++) {
                    headers += `<th scope="col text-center">${(i + 1)}</th>`;
                    let pago = data[i];
                    rows += `<th scope="col" data-idpago="${pago.IdPago}" style="background-color: ${pago.Color}">
                                ${pago.SaldoFormateadoMx}
                                ${pago.Accion}
                            </th>`;

                }

                const htmlTable = `
                
                Semana actual: ${numeroSemanaActual}
                <br/><br/>
                Historial

                <div class="table-responsive">                
                    <table class="table table-bordered table-hover table-striped text-center"
                        id="tableSolicitudes">

                        <thead class="thead-light">

                            ${headers}
                       
                        </thead>
                        <tbody>
                            <tr>
                            ${rows}
                            </tr>
                        
                        </tbody>
                    </table>
              </div>
        `;


                $('#table_').html(htmlTable);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });



    },


    fecha() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;


    },

    fechasHoy() {

        //  fecha hoy (final)
        let today = new Date();
        let month = (today.getMonth() + 1);

        month = month.toString().length === 1 ? `0${month}` : month;

        let endWeekDay = new Date();
        let end_ = endWeekDay.getDay() + 1

        endWeekDay.setDate(endWeekDay.getDate() + 7 - end_ + 1);

        let dayMonth = endWeekDay.getDate();
        month = (endWeekDay.getMonth() + 1);

        payments.fechaFinal = `${endWeekDay.getFullYear()}-${month}-${dayMonth}`;

        //  fecha inicial
        let startWeekDay = new Date();
        startWeekDay.setDate(startWeekDay.getDate() - startWeekDay.getDay() + 1);

        let startDayMonth = startWeekDay.getDate();
        startDayMonth = startDayMonth.toString().length === 1 ? `0${startDayMonth}` : startDayMonth;

        let startMonth = (startWeekDay.getMonth() + 1);
        startMonth = startMonth.toString().length === 1 ? `0${startMonth}` : startMonth;

        let startYear = (startWeekDay.getFullYear());

        payments.fechaInicial = `${startYear}-${startMonth}-${startDayMonth}`;


        console.log(`fechaInicial ${payments.fechaInicial}`);
        console.log(`fechaFinal ${payments.fechaFinal}`);

    },

    updatePayment(idPago, idStatus) {

        let params = {};
        params.path = window.location.hostname;
        params.idPago = idPago;
        params.idStatus = idStatus;
        params = JSON.stringify(params);


        $.ajax({
            type: "POST",
            url: `../../pages/Loans/Payments.aspx/UpdateStatusPagoByPagoAndStatus`,
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                let valores = msg.d;

                console.log(valores);
                payments.historial(payments.idPrestamo, payments.numeroSemana);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {

            }

        });

    },

    updatePendiente(idPago) {
        console.log(`updatePendiente`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_PENDIENTE);
    },

    updateFalla(idPago) {
        console.log(`updateFalla`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_FALLA);
    },

    updateAbonado(idPago) {
        console.log(`updateAbonado`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_ABONADO);
    },

    updatePagado(idPago) {
        console.log(`updatePagado`);

        payments.updatePayment(idPago, utils.STATUS_PAGO_PAGADO);
    },



    accionesBotones: () => {

        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            payments.cargarItems();

        });



        $('#btnAceptarPanelMensajeControlado').on('click', (e) => {
            e.preventDefault();

            $('#panelMensajeControlado').modal('hide');

            $('#panelTabla').show();
            $('#panelForm').hide();



        });


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();


            $('#panelTabla').show();
            $('#panelForm').hide();



        });

        $('#btnCapturar').on('click', (e) => {
            e.preventDefault();

            let hasErrors = $('form[name="frmPago"]').validator('validate').has('.has-error').length;

            if (hasErrors) {

                return;
            }

            //  

            $('.deshabilitable').prop('disabled', true);

            let params = {};
            params.path = window.location.hostname;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idPosicion = document.getElementById('txtIdTipoUsuario').value
            params.idPago = payments.idPago;
            params.abono = $('#txtAbono').val();
            params.recuperado = $('#txtRecuperado').val() === '' ? 0 : $('#txtRecuperado').val();
            params = JSON.stringify(params);


            $.ajax({
                type: "POST",
                url: `../../pages/Loans/Payments.aspx/SavePayment`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;

                    $('.deshabilitable').prop('disabled', false);

                    if (parseInt(valores.CodigoError) === 0) {

                        $('#spnMensajeControlado').html(mensajesAlertas.pagoRegistradoExito);
                        $('#panelMensajeControlado').modal('show');

                        payments.cargarItems();

                    } else {
                        $('#spnMensajes').html(valores.MensajeError);
                        $('#panelMensajes').modal('show');
                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    $('.deshabilitable').prop('disabled', false);

                    $('#spnMensajes').html(mensajesAlertas.errorInesperado);
                    $('#panelMensajes').modal('show');

                }

            });


        });

    }


}

window.addEventListener('load', () => {

    payments.init();

    payments.accionesBotones();

});


