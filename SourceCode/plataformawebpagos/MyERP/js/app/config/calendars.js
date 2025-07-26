'use strict';
let date = new Date();
let descargas = "CALENDARIOS_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '46';




const calendar = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        calendar.idSeleccionado = -1;
        calendar.accion = '';

        calendar.loadContent();
        
        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var min = $('#finicial').val();
                var max = $('#ffinal').val();
                var createdAt = data[2] || 0; // Our date column in the table

                if (min != "" && max == "") {
                    min = moment(min, 'YYY-MM-DD');
                    return moment(createdAt, 'DD/MM/YYY').isSameOrAfter(min)
                }
                else if (min != "" && max != "") {
                    min = moment(min, 'YYY-MM-DD');
                    max = moment(max, 'YYY-MM-DD');
                    return (moment(createdAt, 'DD/MM/YYY').isSameOrAfter(min) && moment(createdAt, 'DD/MM/YYY').isSameOrBefore(max))
                }
                else if (min == "" && max != "") {
                    max = moment(max, 'YYY-MM-DD');
                    return moment(createdAt, 'DD/MM/YYY').isSameOrBefore(max)
                }
                else
                    return true;
            }
        );

        $('#table input, #table select').click(function (e) {
            e.stopPropagation();
        });
    },

    loadContent() {

        let params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Config/Calendars.aspx/GetListaItems",
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
                    ordering: true,
                    paging: false,
                    scrollY: '400px',
                    scrollX: true,
                    data: data,
                    columns: [

                        { data: 'Id' },
                        { data: 'Nombre' },
                        {
                            data: 'Fecha',
                            type: 'date',
                            render: function (data, type, full, meta) {
                                return moment(data).format('DD/MM/YYYY');
                            }
                        },
                        {
                            data: 'EsLaboral',
                            className: 'dt-body-center',
                            render: function (data) {
                                if (data)
                                    return 'Si';
                                else
                                    return 'No';
                            }
                        },
                        {
                            data: 'Estatus',
                            name: 'Estatus',
                            render: function (data, type, full, meta) {
                                if (data === 'Programado')
                                    return `<button type="button" class='btn btn-success btn-sm'>${data}</button>`;
                                else
                                    return `<button type="button" class='btn btn-warning btn-sm'>${data}</button>`;
                            }
                        },
                        {
                            data: null,
                            className: 'dt-body-center',
                            render: function (data, type, full, meta) {
                                return `<button type="button" onclick="calendar.edit(${full.Id})" class='btn btn-primary btn-sm'> <span class='fa fa-edit mr-1'></span></button>
                                        <button type="button" onclick="calendar.delete(${full.Id})" class='btn btn-danger btn-sm'> <span class='fa fa-remove mr-1'></span></button>`;
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
                                columns: [0, 1, 2, 3, 4],
                                format: {
                                    header: function (data, index, row) {
                                        var name;
                                        switch (index) {
                                            case 1:
                                                name = "Evento";
                                                break;
                                            case 2:
                                                name = "Fecha";
                                                break;
                                            case 3:
                                                name = "Laboral";
                                                break;
                                            case 4:
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
                                columns: [0, 1, 2, 3, 4],
                                format: {
                                    header: function (data, index, row) {
                                        var name;
                                        switch (index) {
                                            case 1:
                                                name = "Evento";
                                                break;
                                            case 2:
                                                name = "Fecha";
                                                break;
                                            case 3:
                                                name = "Laboral";
                                                break;
                                            case 4:
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
                                    case 'Nombre':
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
                                    case 'EsLaboral':
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

    delete: (id) => {

        calendar.idSeleccionado = id;


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
            url: "../../pages/Config/Calendars.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;
                calendar.idSeleccionado = item.Id;

                $('#txtNombre').val(item.Nombre);

                $('#txtFecha').val(moment(item.Fecha).format('YYYY-MM-DD'));
                $('#cmbLaboral').val(item.EsLaboral ? 1 : 0);
                $('#panelTabla').hide();
                $('#panelForm').show();


                calendar.accion = "editar";
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
        calendar.accion = "nuevo";
        calendar.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },


    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            calendar.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};
                item.Id = calendar.idSeleccionado;
                item.Nombre = $('#txtNombre').val();
                item.Fecha = document.getElementById('txtFecha').value;
                item.EsLaboral = Number(document.getElementById('cmbLaboral').value) == 1 ? true : false;

                let params = {};
                params.path = "connbd";
                params.item = item;
                params.accion = calendar.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Config/Calendars.aspx/Save",
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

                            calendar.loadContent();


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
            params.id = calendar.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Config/Calendars.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        calendar.loadContent();

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

    calendar.init();

    calendar.accionesBotones();

});


