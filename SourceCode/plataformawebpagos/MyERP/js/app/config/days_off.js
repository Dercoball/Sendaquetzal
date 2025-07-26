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

        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var min = $('#finicial').val();
                var createdAt = data[2] || 0; // Our date column in the table

                if (min != "") {
                    min = moment(min, 'YYY-MM-DD');
                    return moment(createdAt, 'DD/MM/YYY').isSameOrAfter(min)
                }
                else
                    return true;
            }
        );

        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var max = $('#ffinal').val();
                var createdAt = data[3] || 0; // Our date column in the table

                if (max != "") {
                    max = moment(max, 'YYY-MM-DD');
                    return moment(createdAt, 'DD/MM/YYY').isSameOrBefore(max)
                }
                else
                    return true;
            }
        );

    },

    loadContent() {

        let params = {};
        params.path = "connbd";
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

                        { data: 'IdDiasParo' },
                        { data: 'Nota' },
                        {
                            data: 'FechaInicio',
                            type: 'date',
                            render: function (data, type, full, meta) {
                                return moment(data).format('DD/MM/YYYY');
                            }
                        },
                        {
                            data: 'FechaFin',
                            type: 'date',
                            render: function (data, type, full, meta) {
                                return moment(data).format('DD/MM/YYYY');
                            }
                        },
                        {
                            data: 'Plaza'
                        },
                        {
                            data: 'Estatus',
                            name: 'Estatus',
                            render: function (data, type, full, meta) {
                                if(data === 'Programado')
                                    return `<button type="button" class='btn btn-success btn-sm'>${data}</button>`;
                                else
                                    return `<button type="button" class='btn btn-warning btn-sm'>${data}</button>`;
                            }
                        },
                        {
                            data: null,
                            className: 'dt-body-center',
                            render: function (data, type, full, meta) {
                                return `<button type="button" onclick="dayOff.edit(${full.IdDiasParo})" class='btn btn-primary btn-sm'> <span class='fa fa-edit mr-1'></span></button>
                                        <button type="button" onclick="dayOff.delete(${full.IdDiasParo})" class='btn btn-danger btn-sm'> <span class='fa fa-remove mr-1'></span></button>`;
                            }
                        },
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
                                format: {
                                    header: function (data, index, row) {
                                        var name;
                                        switch (index) {
                                            case 1:
                                                name = "Evento";
                                                break;
                                            case 2:
                                                name = "Fecha Inicial";
                                                break;
                                            case 3:
                                                name = "Fecha Final";
                                                break;
                                            case 4:
                                                name = "Plaza";
                                                break;
                                            case 5:
                                                name = "Estatus";
                                                break;
                                            default:
                                                name = data;
                                                break;
                                        }

                                        return name
                                    }
                                }
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
                                format: {
                                    header: function (data, index, row) {
                                        var name;
                                        switch (index) {
                                            case 1:
                                                name = "Evento";
                                                break;
                                            case 2:
                                                name = "Fecha Inicial";
                                                break;
                                            case 3:
                                                name = "Fecha Final";
                                                break;
                                            case 4:
                                                name = "Plaza";
                                                break;
                                            case 5:
                                                name = "Estatus";
                                                break;
                                            default:
                                                name = data;
                                                break;
                                        }

                                        return name
                                    }
                                }
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
                                    case 'Nota':
                                        $('input', column.header()).on('keyup change clear', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                    case 'FechaInicio':
                                    case 'FechaFin':
                                        $('input', column.header()).on('change', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'Plaza':
                                    case 'Estatus':
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
        params.path = "connbd";
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
                opcion += '<option value="0">Todos</option>';

                let selectEl = document.getElementById('cmbPlaza');
                //remueve las opciones del combo
                document.querySelectorAll('select[name="cmbPlaza"] option').forEach(option => option.remove());

                selectEl.add(new Option("Todos", "", true, true));



                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdPlaza}' > ${item.Nombre}</option > `;
                    const option = new Option(item.Nombre, item.Nombre, false, false);
                    selectEl.add(option);

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
        params.path = "connbd";
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
                dayOff.idSeleccionado = item.IdDiasParo;

                $('#txtNota').val(item.Nota);
                $('#txtFechaInicio').val(moment(item.FechaInicio).format('YYYY-MM-DD'));
                $('#txtFechaFin').val(moment(item.FechaFin).format('YYYY-MM-DD'));
                if (item.IdPlaza > 0) {
                    $('#comboPlaza').val(item.IdPlaza);
                }

                $('#panelTabla').hide();
                $('#panelForm').show();


                dayOff.accion = "editar";
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
                item.IdDiasParo = dayOff.idSeleccionado;
                item.Nota = $('#txtNota').val();
                item.FechaInicio = document.getElementById('txtFechaInicio').value;
                item.FechaFin = document.getElementById('txtFechaFin').value;
                item.IdPlaza = Number($('#comboPlaza').val());

                let tiposParo = $('#comboPlaza').val();

                
                if (tiposParo.length === 0) {
                    item.IdTipoParo = '2'
                } else {
                    item.IdTipoParo = '1'
                }

                let params = {};
                params.path = "connbd";
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
            params.path = "connbd";
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


