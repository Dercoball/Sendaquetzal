'use strict';
let date = new Date();
let descargas = "ACTIVOS_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '57';
let totalCosto = 0;
let totalPiezas = 0;

const deliveryMaterial = {



    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        deliveryMaterial.idSeleccionado = -1;
        deliveryMaterial.accion = '';

        deliveryMaterial.loadContent();
        deliveryMaterial.loadComboEmpleado();
        deliveryMaterial.loadComboTipo();
        deliveryMaterial.fechas();

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#pmin').val(), 10);
            var max = parseInt($('#pmax').val(), 10);
            var prestamo = parseFloat(data[2]) || 0;
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

        $.fn.dataTable.ext.search.push(function (settings, data, dataIndex) {
            var min = parseInt($('#cmin').val(), 10);
            var max = parseInt($('#cmax').val(), 10);
            var prestamo = parseFloat(data[4]) || 0;
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

        $('#table input, #table select').click(function (e) {
            e.stopPropagation();
        });

    },

    loadContent() {

        let fechaInicial = $('#txtFiltroFechaInicial').val();
        let fechaFinal = $('#txtFiltroFechaFinal').val();

        if (!fechaInicial) {

            fechaInicial = deliveryMaterial.fecha();
            $('#txtFiltroFechaInicial').val(fechaInicial);
        }

        if (!fechaFinal) {

            fechaFinal = deliveryMaterial.fecha();
            $('#txtFiltroFechaFinal').val(fechaFinal);
        }

        let idColaborador = $('#comboFiltroEmpleado').val();
        idColaborador = idColaborador == null ? "-1" : status;

        let idCategoria= $('#comboFiltroCategoria').val();
        idCategoria = idCategoria == null ? "-1" : idCategoria;


        let costoDesde = $('#txtFiltroCostoInicial').val();
        let costoHasta = $('#txtFiltroCostoFinal').val();
        


        let params = {};
        params.path = "connbd";
        console.log(params.idUsuario = document.getElementById('txtIdUsuario').value);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetListaItems",
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
                        { data: 'MaterialEntregado' },
                        { data: 'Cantidad' },
                        { data: 'Colaborador' },
                        {
                            data: 'Costo',
                            render: $.fn.dataTable.render.number(',', '.', 2, '$')
                        },
                        {
                            data: 'Fecha',
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
                                return `<button type="button" onclick="deliveryMaterial.edit(${full.IdMaterialEntrega})" class='btn btn-primary btn-sm'> <span class='fa fa-edit mr-1'></span></button>
                                        <button type="button" onclick="deliveryMaterial.delete(${full.IdMaterialEntrega})" class='btn btn-danger btn-sm'> <span class='fa fa-remove mr-1'></span></button>`;
                            }
                        }
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
                                            case 0:
                                                name = "Categoría";
                                                break;
                                            case 1:
                                                name = "Material";
                                                break;
                                            case 2:
                                                name = "Piezas";
                                                break;
                                            case 3:
                                                name = "Colaborador";
                                                break;
                                            case 4:
                                                name = "Costo Total";
                                                break;
                                            case 5:
                                                name = "Fecha";
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
                                            case 0:
                                                name = "Categoría";
                                                break;
                                            case 1:
                                                name = "Material";
                                                break;
                                            case 2:
                                                name = "Piezas";
                                                break;
                                            case 3:
                                                name = "Colaborador";
                                                break;
                                            case 4:
                                                name = "Costo Total";
                                                break;
                                            case 5:
                                                name = "Fecha";
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
                            .column(4, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;

                        totalPiezas = api
                            .column(2, { page: 'current' })
                            .data()
                            .reduce(function (a, b) {
                                return intVal(a) + intVal(b);
                            }, 0);
                        ;

                        // Update footer
                        $(api.column(2).footer()).html($.fn.dataTable.render.number(',', '.', 2, '').display(totalPiezas));
                        $(api.column(4).footer()).html('$' + $.fn.dataTable.render.number(',', '.', 2, '').display(totalCosto));
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
                                    case 'MaterialEntregado':
                                    case 'Colaborador':
                                        $('input', column.header()).on('keyup change clear', function () {
                                            if (column.search() !== this.value) {
                                                column.search(this.value).draw();
                                            }
                                        });
                                        break;
                                    case 'Costo':
                                    case 'Cantidad':
                                        $('input', column.header()).on('keyup', function () {
                                            column.draw();
                                        });
                                        break;
                                    case 'Fecha':
                                        $('input', column.header()).on('change', function () {
                                            column.draw();
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

        deliveryMaterial.idSeleccionado = id;


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
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;
                deliveryMaterial.idSeleccionado = item.IdMaterialEntrega;

                $('#txtMaterial').val(item.MaterialEntregado);
                $('#comboCategoria').val(item.IdCategoria);
                $('#comboEmpleado').val(item.IdEmpleado);
                $('#txtCantidad').val(item.Cantidad);
                $('#txtCosto').val(item.Costo);
                $('#txtFecha').val(moment(item.Fecha).format('YYYY-MM-DD'));

                $('#panelTabla').hide();
                $('#panelForm').show();


                deliveryMaterial.accion = "editar";
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
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetListaItemsEmpleados",
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
                $('#comboFiltroEmpleado').html(opcion);

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
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetListaItemsCategorias",
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
                $('#comboFiltroCategoria').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    generateChart: (data) => {

        deliveryMaterial.empleados = data.map((x) => {
            return {
                id_empleado: x.Empleado.IdEmpleado,
                material: x.MaterialEntregado,
                nombre: x.Empleado.Nombre
            }
        });

        deliveryMaterial.nombres_empleados = deliveryMaterial.empleados.map(x => { return x.nombre });
        let nombresDistintos = [...new Set(deliveryMaterial.nombres_empleados)];  //  quitar repetidos


        let series = [];
        for (let i in deliveryMaterial.empleados) {

            let current = deliveryMaterial.empleados[i];

            let serie = {
                nombre: current.nombre,
                material: current.material,
                montos: data.map((x) => {

                    if (x.Empleado.IdEmpleado == current.id_empleado) {
                        return x.Costo;
                    }
                })
            }

            let exists = series.find(item => item.nombre === current.nombre);


            if (!exists) {
                series.push(serie);
            }

        }


        //  Quitar nulls y convertir a ceros
        let cleanSeries = [];
        for (let i in series) {
            let s = series[i];

            let newSerie = {
                'name': s.nombre,
                data: s.montos.map((item) => {

                    if (typeof item !== 'undefined') {
                        return item
                    } else {
                        return 0
                    }
                })

            };

            cleanSeries.push(newSerie);

        }



        Highcharts.chart('container_grafica', {
            chart: {
                type: 'column'
            },
            title: {
                text: 'Monto total'
            },
            xAxis: {
                categories: deliveryMaterial.empleados.map(x => { return x.material })
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                }
            },
            legend: {
                reversed: true
            },
            plotOptions: {
                series: {
                    stacking: 'normal'
                }
            },
            series: cleanSeries
        });


    },


    fechas() {

        //  fecha hoy (final)
        let today = new Date();
        let month = (today.getMonth() + 1);

        month = month.toString().length === 1 ? `0${month}` : month;

        let endWeekDay = new Date();
        let end_ = endWeekDay.getDay() + 1

        endWeekDay.setDate(endWeekDay.getDate() + 7 - end_ + 1);

        let dayMonth = endWeekDay.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;


        month = (endWeekDay.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        let fechaFinal = `${endWeekDay.getFullYear()}-${month}-${dayMonth}`;

        //  fecha inicial
        let startWeekDay = new Date();
        startWeekDay.setDate(startWeekDay.getDate() - startWeekDay.getDay() + 1);

        let startDayMonth = startWeekDay.getDate();
        startDayMonth = startDayMonth.toString().length === 1 ? `0${startDayMonth}` : startDayMonth;

        let startMonth = (startWeekDay.getMonth() + 1);
        startMonth = startMonth.toString().length === 1 ? `0${startMonth}` : startMonth;

        let startYear = (startWeekDay.getFullYear());

        let fechaInicial = `${startYear}-${startMonth}-${startDayMonth}`;


        console.log(`fechaInicial ${fechaInicial}`);
        console.log(`fechaFinal ${fechaFinal}`);


        $('#txtFiltroFechaFinal').val(fechaFinal);
        $('#txtFiltroFechaInicial').val(fechaInicial);


    },

    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');




        $('#panelTabla').hide();
        $('#panelForm').show();
        deliveryMaterial.accion = "nuevo";
        deliveryMaterial.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);
        
        $('#txtFecha').val(deliveryMaterial.fecha());


    },

    fecha() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;


    },



    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();


            deliveryMaterial.nuevo();

        });

        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            deliveryMaterial.loadContent();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};


                item.IdMaterialEntrega = deliveryMaterial.idSeleccionado;
                item.MaterialEntregado = $('#txtMaterial').val();
                item.IdCategoria = $('#comboCategoria').val();
                item.IdEmpleado = $('#comboEmpleado').val();
                item.Cantidad = $('#txtCantidad').val();
                item.Costo = $('#txtCosto').val();
                item.Fecha = $('#txtFecha').val();



                let params = {};
                params.path = "connbd";
                params.item = item;
                params.accion = deliveryMaterial.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Assets/DeliveryMaterials.aspx/Save",

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

                            deliveryMaterial.loadContent();


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
            params.id = deliveryMaterial.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Assets/DeliveryMaterials.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        deliveryMaterial.loadContent();

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

    deliveryMaterial.init();

    deliveryMaterial.accionesBotones();

});


