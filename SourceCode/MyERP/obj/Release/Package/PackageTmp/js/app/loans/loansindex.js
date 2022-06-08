'use strict';
let date = new Date();
let descargas = "Prestamos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const loansindex = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        loansindex.idSeleccionado = "-1";
        loansindex.idTipoUsuario = "-1";
        loansindex.login = "-1";
        loansindex.accion = "";

        loansindex.fechasHoy();
        loansindex.loadComboStatusPrestamo();

        loansindex.cargarItems();


    },

    cargarItems: () => {

        let status = $('#comboStatus').val();
        let fechaInicial = $('#txtFechaInicial').val();
        let fechaFinal = $('#txtFechaFinal').val();

        if (!fechaFinal) {

            fechaFinal = loansindex.fecha();
            $('#txtFechaFinal').val(fechaFinal);
        }

        if (!fechaInicial) {

            fechaInicial = loansindex.fecha();
            $('#txtFechaInicial').val(fechaInicial);
        }

        status = status == null ? "-1" : status;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params.idStatus = status;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoansIndex.aspx/GetListaItems",
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
                        { data: 'IdPrestamo' },
                        { data: 'Cliente.NombreCompleto' },
                        { data: 'Cliente.Curp' },
                        { data: 'MontoFormateadoMx' },
                        { data: 'FechaSolicitud' },
                        { data: 'NombreStatus' },
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
            url: "../../pages/Loans/LoansIndex.aspx/GetListaStatusPrestamo",
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


    view(idCliente) {
        console.log('Abir datos del cliente ' + idCliente);

        //loansEdit.edit(idCliente);

        window.location = "LoanApprove.aspx?id=" + idCliente;

        panelGuarantee.view(idCliente);

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

        $('#txtFechaFinal').val(`${today.getFullYear()}-${month}-${dayMonth}`);


        //  fecha inicial
        let startWeekDay = new Date();
        startWeekDay.setDate(startWeekDay.getDate() - startWeekDay.getDay() + 1);

        let startDayMonth = startWeekDay.getDate();
        startDayMonth = startDayMonth.toString().length === 1 ? `0${startDayMonth}` : startDayMonth;

        let startMonth = (startWeekDay.getMonth() + 1);
        startMonth = startMonth.toString().length === 1 ? `0${startMonth}` : startMonth;
        let startYear = (startWeekDay.getFullYear());

        $('#txtFechaInicial').val(`${startYear}-${startMonth}-${startDayMonth}`);


    },

 

    accionesBotones: () => {

        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            loansindex.cargarItems();

        });




    }


}

window.addEventListener('load', () => {

    loansindex.init();

    loansindex.accionesBotones();

});


