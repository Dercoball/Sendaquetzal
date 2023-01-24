'use strict';
let date = new Date();
let descargas = "DIASDEPARO_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '49';




const dayOff = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        dayOff.idSeleccionado = -1;
        dayOff.accion = '';

        dayOff.loadComboPlaza();

        dayOff.loadContent();
        

    },

    loadContent() {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/DaysOff.aspx/GetListaItems",
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
                    ordering: false,
                    data: data,
                    paging: false,
                    scrollY: '400px',
                    scrollX: true,
                    columns: [

                        { data: 'IdDiaParo' },
                        { data: 'Nota' },
                        { data: 'FechaInicio' },
                        { data: 'FechaFin' },
                        { data: null, render: '' },
                        { data: null, render: '' },
                        { data: 'Accion' }
                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: "rt<'row'<'col text-right mt-4'B>>ip",
                    buttons: [
                        {
                            extend: 'excelHtml5',
                            title: descargas,
                            text: '&nbsp; Descargar Excel', className: 'csvbtn',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5],
                            }
                        },
                        {
                            extend: 'pdfHtml5',
                            text: 'Descargar PDF',
                            title: descargas,
                            orientation: 'landscape',
                            pageSize: 'LEGAL',
                            className: 'csvbtn ml-2',
                            exportOptions: {
                                columns: [0, 1, 2, 3, 4, 5],
                            }
                        }
                    ],
                    initComplete: function () {
                        let columnsSettings = this.api().settings().init().columns;

                        this.api()
                            .columns()
                            .every(function (idx) {
                                var column = this;
                                let dataHeader = columnsSettings[idx].data;

                                switch (dataHeader) {
                                    case 'MontoPrestamo':
                                    case 'SemanasFalla':
                                    case 'Monto':
                                    case 'TotalFalla':
                                    case 'Pagado':
                                        $('input', column.header()).on('keyup', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'NombreCliente':
                                    case 'NombreAval':
                                        $('input', column.header()).on('keyup change clear', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                    case 'Fecha':
                                        $('input', column.header()).on('change', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'Status':
                                        $('select', column.header()).on('change', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                }
                            });
                    }

                });


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
                let opcion = '<option value="">Seleccione...</option>';

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

    delete: (id) => {

        dayOff.idSeleccionado = id;


        $('#mensajeEliminar').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');

    },




    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        let params = {};
        params.path = window.location.hostname;
        params.id = id;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/DaysOff.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                dayOff.idSeleccionado = item.IdDiaParo;

                $('#txtNota').val(item.Nota);

                $('#txtFechaInicio').val(item.FechaInicio);
                $('#txtFechaFin').val(item.FechaFin);

                $('#panelTabla').hide();
                $('#panelForm').show();


                dayOff.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },


    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');




        $('#panelTabla').hide();
        $('#panelForm').show();
        dayOff.accion = "nuevo";
        dayOff.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            dayOff.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};
                item.IdDiaParo = dayOff.idSeleccionado;
                item.Nota = $('#txtNota').val();
                item.FechaInicio = document.getElementById('txtFechaInicio').value;
                item.FechaFin = document.getElementById('txtFechaFin').value;

                let tiposParo = $('#comboPlaza').val();

                
                if (tiposParo.length === 0) {
                    item.IdTipoParo = '2'
                } else {
                    item.IdTipoParo = '1'
                }

                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = dayOff.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/DaysOff.aspx/Save",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                            $('#panelTabla').show();
                            $('#panelForm').hide();

                            dayOff.loadContent();


                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }

                        $('#panelEdicion').modal('hide');


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });


        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = window.location.hostname;
            params.id = dayOff.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/DaysOff.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        dayOff.loadContent();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });


    }



}

window.addEventListener('load', () => {

    dayOff.init();

    dayOff.accionesBotones();

});


