'use strict';
let date = new Date();
let descargas = "Prestamos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const POSICION_DIRECTOR = 1;
const POSICION_COORDINADOR = 2;
const POSICION_EJECUTIVO = 3;
const POSICION_SUPERVISOR = 4;
const POSICION_PROMOTOR = 5;

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

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();



    },

    cargarItems: () => {

        let status = $('#comboStatus').val();
        let fechaInicial = $('#txtFechaInicial').val();
        let fechaFinal = $('#txtFechaFinal').val();

        if (!fechaFinal) {

            fechaFinal = reporteCombustibles.fecha();
            $('#txtFechaFinal').val(fechaFinal);
        }

        if (!fechaInicial) {

            fechaInicial = reporteCombustibles.fecha();
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
                        { data: 'Monto' },
                        { data: 'FechaSolicitud' },
                        { data: 'NombreStatus' },                        
                        { data: 'Accion' }


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
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


    delete: (id) => {

        loansindex.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },

    view(idPrestamo) {
        console.log('Abir datos del préstamo' + idPrestamo);
    },


    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');
        $('#txtDescripcion').val('');

        $('#panelTabla').hide();
        $('#panelForm').show();
        loansindex.accion = "nuevo";
        loansindex.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();
        $('#txtPassword').prop('disabled', false);




    },

    testData() {
        $('.campo-combo').val(2);
        $('.campo-date').val('2022-01-01');
        $('.campo-input').val(7);

        //$('#txtFechaNacimiento').val('2000-01-01');

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



    fecha() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;


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

    loadComboTipoCliente: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Clients.aspx/GetListaItemsTipoCliente",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdTipoCliente}' > ${item.NombreTipoCliente}</option > `;

                } 

                $('#comboTipoCliente').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
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


