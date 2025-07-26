'use strict';
let date = new Date();
let descargas = "ACTIVOS_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '52';
let totalCosto = 0;

const asset = {

    

    init: () => {

        moment.locale('es-mx');
        $('#panelTabla').show();
        $('#panelForm').hide();

        asset.idSeleccionado = -1;
        asset.accion = '';

        asset.loadContent();
        asset.loadComboEmpleado();
        asset.loadComboTipo();

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#cmin').val(), 10);
            var max = parseInt($('#cmax').val(), 10);
            var prestamo = parseFloat(data[3]) || 0;
            if (
                (isNaN(min) && isNaN(max)) ||
                (isNaN(min) && prestamo <= max) ||
                (min <= prestamo && isNaN(max)) ||
                (min <= prestamo && prestamo <= max)
            ) {
                return true;
            }
            return false;
        });

        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var min = $('#finicial').val();
                var max = $('#ffinal').val();
                var createdAt = data[5] || 0; // Our date column in the table

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

        $.fn.dataTable.ext.search.push(
            function (settings, data, dataIndex) {
                var min = $('#finicialb').val();
                var max = $('#ffinalb').val();
                var createdAt = data[7] || 0; // Our date column in the table

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
        console.log(params.idUsuario = document.getElementById('txtIdUsuario').value);
        
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetListaItems",
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

                        { data: 'NombreCategoria' },
                        { data: 'Descripcion' },
                        { data: 'NumeroSerie' },
                        {
                            data: 'Costo',
                            render: $.fn.dataTable.render.number(',', '.', 2, '$')
                        },
                        { data: 'Asignado' },
                        {
                            data: 'FechaIngreso',
                            type: 'date',
                            render: function (data, type, full, meta) {
                                if (data == null)
                                    return 'N/A'
                                return moment(data).format('DD/MM/YYYY');
                            }
                        },
                        {
                            data: 'Estatus',
                            name: 'Estatus',
                            render: function (data, type, full, meta) {
                                if (data === 'Activo')
                                    return `<button type="button" class='btn btn-success btn-sm'>${data}</button>`;
                                else
                                    return `<button type="button" class='btn btn-danger btn-sm'>${data}</button>`;
                            }
                        },
                        {
                            data: 'FechaBaja',
                            type: 'date',
                            render: function (data, type, full, meta) {
                                if (data == null)
                                    return 'N/A'
                                return moment(data).format('DD/MM/YYYY');
                            }
                        },
                        {
                            data: null,
                            width: '150px',
                            className: 'dt-body-center',
                            render: function (data, type, full, meta) {
                                return `<button type="button" onclick="asset.edit(${full.IdActivo})" class='btn btn-primary btn-sm'> <span class='fa fa-edit mr-1'></span></button>
                                        <button type="button" onclick="asset.delete(${full.IdActivo})" class='btn btn-danger btn-sm'> <span class='fa fa-remove mr-1'></span></button>`;
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
                                columns: [0, 1, 2, 3, 4, 5, 6, 7],
                                format: {
                                    header: function (data, index, row) {
                                        var name;
                                        switch (index) {
                                            case 0:
                                                name = "Categoría";
                                                break;
                                            case 1:
                                                name = "Descripción";
                                                break;
                                            case 2:
                                                name = "No. Serie";
                                                break;
                                            case 3:
                                                name = "Costo";
                                                break;
                                            case 4:
                                                name = "Asignación";
                                                break;
                                            case 5:
                                                name = "Ingreso";
                                                break;
                                            case 6:
                                                name = "Estatus";
                                                break;
                                            case 7:
                                                name = "Baja";
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
                                columns: [0, 1, 2, 3, 4, 5, 6, 7],
                                format: {
                                    header: function (data, index, row) {
                                        var name;
                                        switch (index) {
                                            case 0:
                                                name = "Categoría";
                                                break;
                                            case 1:
                                                name = "Descripción";
                                                break;
                                            case 2:
                                                name = "No. Serie";
                                                break;
                                            case 3:
                                                name = "Costo";
                                                break;
                                            case 4:
                                                name = "Asignación";
                                                break;
                                            case 5:
                                                name = "Ingreso";
                                                break;
                                            case 6:
                                                name = "Estatus";
                                                break;
                                            case 7:
                                                name = "Baja";
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
                    footerCallback: function (row, data, start, end, display) {
                        var api = this.api();

                        // Remove the formatting to get integer data for summation
                        var intVal = function (i) {
                            return typeof i === 'string' ? i.replace(/[\$,]/g, '') * 1 : typeof i === 'number' ? i : 0;
                        };


                        // Total over all pages
                        totalCosto = api
                            .column(3, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;

                        // Update footer
                        $(api.column(3).footer()).html('$' + $.fn.dataTable.render.number(',', '.', 2, '').display(totalCosto));
                    },
                    initComplete: function () {
                        let columnsSettings = this.api().settings().init().columns;

                        this.api()
                            .columns()
                            .every(function (idx) {
                                var column = this;
                                let dataHeader = columnsSettings[idx].data;
                                switch (dataHeader) {
                                    case 'NombreCategoria':
                                    case 'Descripcion':
                                    case 'NumeroSerie':
                                    case 'Asignado':
                                        $('input', column.header()).on('keyup change clear', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                    case 'Costo':
                                        $('input', column.header()).on('keyup', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'FechaIngreso':
                                    case 'FechaBaja':
                                        $('input', column.header()).on('change', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'Estatus':
                                        $('select', column.header()).on('change', function () {
                                            if (column.search() !== this.value) {
                                                if (this.value !== '') {
                                                    var searchTerm = '^' + this.value.replace('(', '\\(').replace(')', '\\)').trim() + '$';
                                                    column.search(searchTerm, true, false).draw();
                                                } else {
                                                    column.search(this.value).draw();
                                                }
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

        asset.idSeleccionado = id;


        $('#mensajeEliminar').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');

    },




    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#frm')[0].reset();

        let params = {};
        params.path = "connbd";
        params.id = id;
        console.log(id);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;
                asset.idSeleccionado = item.IdActivo;

                $('#txtDescripcion').val(item.Descripcion);
                $('#comboCategoria').val(item.IdCategoria);
                $('#comboEmpleado').val(item.IdEmpleado);

                $('#txtNumeroSerie').val(item.NumeroSerie);
                $('#txtCosto').val(item.Costo);
                $('#txtComentarios').val(item.Comentarios);

                $('#txtFechaIngreso').val(moment(item.FechaIngreso).format('YYYY-MM-DD'));
                $('#txtFechaBaja').val(moment(item.FechaBaja).format('YYYY-MM-DD'));


                $('#panelTabla').hide();
                $('#panelForm').show();


                asset.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },



   

    loadComboEmpleado: () => {

        var params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetListaItemsEmpleados",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $('#comboEmpleado').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    loadComboTipo: () => {

        var params = {};
        params.path = "connbd";
        params.idUsuario = document.getElementById('txtIdUsuario').value;

        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/Assets.aspx/GetListaItemsCategorias",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.Id}' > ${item.Nombre}</option > `;

                }

                $('#comboCategoria').html(opcion);

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
        asset.accion = "nuevo";
        asset.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },



    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            asset.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};

                item.IdActivo = asset.idSeleccionado;
                item.Descripcion = $('#txtDescripcion').val();
                item.IdCategoria = $('#comboCategoria').val();
                item.IdEmpleado = $('#comboEmpleado').val();
                item.NumeroSerie = $('#txtNumeroSerie').val();
                item.Costo = $('#txtCosto').val();
                item.Comentarios = $('#txtComentarios').val();
                item.FechaIngreso = document.getElementById('txtFechaIngreso').value;
                item.FechaBaja = document.getElementById('txtFechaBaja').value;


                let params = {};
                params.path = "connbd";
                params.item = item;
                params.accion = asset.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Assets/Assets.aspx/Save",
                    
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

                            asset.loadContent();


                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }



                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });

        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = "connbd";
            params.id = asset.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Assets/Assets.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        asset.loadContent();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });




        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });

    }


}

window.addEventListener('load', () => {

    asset.init();

    asset.accionesBotones();

});


