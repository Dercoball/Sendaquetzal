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
        payments.login = "-1";
        payments.accion = "";

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
                        { data: 'Status' },
                        { data: 'Status' },
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



    loadComboStatusPrestamo: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/payments.aspx/GetListaStatusPrestamo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdStatusPrestamo}' > ${item.Nombre}</option > `;

                }

                $('#comboStatus').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },


    view(idPago) {

        console.log(idPago);


        //  traer datos del pago e historial del préstamo

        $('#panelTabla').hide();
        $('#panelForm').show();


        payments.historial(15);


    },

    historial: (numeroSemanas) => {

        console.log('Historial');

        payments.accion = "nuevo";
        payments.idEquipoSeleccionado = -1;


        let headers = '';
        headers += `<th scope="col"></th>`;

        let rows = '';
        rows += `<th scope="col"></th>`;


        for (var i = 1; i <= numeroSemanas; i++) {
            headers += `<th scope="col text-center">${i}</th>`;
            rows += `<th scope="col"></th>`;
        }

        const htmlTable = `
                <div class="responsive">
                <table style="width: 100%!important;" class="table table-bordered table-hover table-striped text-center"
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
        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;


        payments.fechaFinal = `${today.getFullYear()}-${month}-${dayMonth}`;

        //  fecha inicial
        let startWeekDay = new Date();
        startWeekDay.setDate(startWeekDay.getDate() - startWeekDay.getDay() + 1);

        let startDayMonth = startWeekDay.getDate();
        startDayMonth = startDayMonth.toString().length === 1 ? `0${startDayMonth}` : startDayMonth;

        let startMonth = (startWeekDay.getMonth() + 1);
        startMonth = startMonth.toString().length === 1 ? `0${startMonth}` : startMonth;
        let startYear = (startWeekDay.getFullYear());

        payments.fechaInicial = `${startYear}-${startMonth}-${startDayMonth}`;


    },



    accionesBotones: () => {

        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            payments.cargarItems();

        });




    }


}

window.addEventListener('load', () => {

    payments.init();

    payments.accionesBotones();

});


