let date = new Date();
let descargas = "AprobarNomina_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '65';

let prenomina = {
    idPrenomina: -1,
    idObra: -1,
    prenomina: {},
    arrayData: [],
    accion: '',
    idSeleccionado: -1

}

const panel = {


    init: () => {

        panel.listaPrenominas = [];
        prenomina.idSeleccionado = -1;
        prenomina.accion = '';

        panel.cargarItems();

    },

    //obras 
    cargarItems: () => {

        console.log(`id del usuario logueado = ${sessionStorage.getItem("idusuario")}`);
        console.log(`id del empleado logueado = ${sessionStorage.getItem("idempleado")}`);

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idEmpleado = sessionStorage.getItem("idempleado");
        parametros.idUsuario = sessionStorage.getItem("idusuario");

        let url = '../pages/PanelRevisionNomina.aspx/GetListaObrasPorResidente';


        utils.postData(url, parametros)
            .then(data => {
                console.log(data);

                panel.listaPrenominas = data.d;

                data = data.d;



                let tableObras = $('#tableObras').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'ClaveObra' },
                        { data: 'NombreObra' },
                        { data: 'NombreEmpleado' },
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
                    dom: 'frtiplB',
                    buttons: [
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


            });


    },

    eliminar: (id) => {

        prenomina.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },

    //abrir para ver los empleados de una obra
    seleccionar: (id, idPrenomina) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        prenomina.idObra = id;
        prenomina.idPrenomina = idPrenomina;

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idObra = prenomina.idObra;
        parametros.idPrenomina = prenomina.idPrenomina;
        parametros = JSON.stringify(parametros);

        $.ajax({
            type: "POST",
            url: "../pages/PanelRegistroAsistencia.aspx/GetListaItemsAsistencia",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('tabla de prenomina');

                var valores = msg.d;
                //console.log(`datos prenomina = ${JSON.stringify(valores.prenomina)}`);

                if (valores.table === '') {
                    console.log('No se pudo generar la tabla de prenomina');

                } else {
                    //console.log(item);
                    $('#table tbody').empty().append(valores.Table);

                    prenomina.arrayData = valores.Array;
                    prenomina.prenomina = valores.prenomina;

                    let disabled = prenomina.prenomina.IdStatusPrenomina === 1 ||
                        prenomina.prenomina.IdStatusPrenomina === 2 ||
                        prenomina.prenomina.IdStatusPrenomina === 3;


                    $('#btnAprobar').attr('disabled', !disabled);




                    $('#table').on('change', 'textarea', (e) => {
                        e.preventDefault();
                        e.stopImmediatePropagation();

                        let id_ = e.currentTarget.id;
                        let value = $('#' + id_).val();

                        let esDetalleHorasExtra = $('#' + id_).hasClass('detalle-horas-extra');
                        let esObservaciones = $('#' + id_).hasClass('observaciones');

                        //console.log(`id_ = ${id_}`);


                        //console.log(`esDetalleHorasExtra = ${esDetalleHorasExtra}`);
                        //console.log(`esObservaciones = ${esObservaciones}`);



                        if (esDetalleHorasExtra === true) {
                            prenomina.arrayData[row_index].DetalleHorasExtra = value;
                        }

                        if (esObservaciones === true) {
                            prenomina.arrayData[row_index].Observaciones = value;
                        }


                    });


                    $('#table').on('change', 'input', (e) => {
                        e.preventDefault();
                        e.stopImmediatePropagation();

                        let id_ = e.currentTarget.id;
                        let value = $('#' + id_).val();

                        let esNumeroHorasExtra = $('#' + id_).hasClass('numero-horas-extra');
                        let esMontoViaticos = $('#' + id_).hasClass('monto-viaticos');

                        //console.log(`id_ = ${id_}`);


                        //console.log(`esNumeroHorasExtra = ${esNumeroHorasExtra}`);
                        //console.log(`esMontoViaticos = ${esMontoViaticos}`);

                        if (esNumeroHorasExtra === true) {
                            prenomina.arrayData[row_index].NumeroHorasExtra = value;
                        }

                        if (esMontoViaticos === true) {
                            prenomina.arrayData[row_index].MontoViaticos = value;
                        }


                    });


                    let row_index = -1;

                    $("#table").on("click", "tbody tr", function () {
                        row_index = $(this).index();
                        if ($(this).hasClass('selected')) {
                            $(this).removeClass('selected');
                        }
                        else {
                            table.$('tr.selected').removeClass('selected');
                            $(this).addClass('selected');
                        }


                    });





                    $('#table').on('change', 'select', (e) => {
                        e.preventDefault();
                        e.stopImmediatePropagation();

                        var id_ = e.currentTarget.id;


                        //console.log('change combo id = ' + id_);
                        //console.log('change val = ', $('#' + id_).val())

                        let claveEmpleado = $('#' + id_).attr('data-empleado');

                        //console.log('clave empleado = ', claveEmpleado)
                        //let lengthCombos = $('.' + claveEmpleado).length;
                        //console.log('lengthCombos = ', lengthCombos)


                        let numDiasEmpleado = 6;
                        $('.clave_' + claveEmpleado).each(function (i, obj) {
                            //console.log('... index = ', i);
                            //console.log('... obj = ', obj);

                            let idItem = $(obj).attr('id');
                            let dia = $(obj).attr('data-dia');

                            //console.log('... item = ', idItem);

                            let value = $('#' + idItem).val();

                            //console.log('... value = ', value);

                            //console.log('clave empleado = ', value)
                            if (value !== '1' && value !== '10') {
                                numDiasEmpleado--;
                            }


                            if (dia === 'jue') {
                                prenomina.arrayData[row_index].ValorJue = value;
                            }
                            if (dia === 'vie') {
                                prenomina.arrayData[row_index].ValorVie = value;
                            }
                            if (dia === 'sab') {
                                prenomina.arrayData[row_index].ValorSab = value;
                            }
                            if (dia === 'dom') {
                                prenomina.arrayData[row_index].ValorDom = value;
                            }
                            if (dia === 'lun') {
                                prenomina.arrayData[row_index].ValorLun = value;
                            }
                            if (dia === 'mar') {
                                prenomina.arrayData[row_index].ValorMar = value;
                            }
                            if (dia === 'mie') {
                                prenomina.arrayData[row_index].ValorMie = value;
                            }


                            prenomina.arrayData[row_index].NumeroDias = numDiasEmpleado;


                        });
                        $('#txtNumDias_' + claveEmpleado).val(numDiasEmpleado);



                    });


                    let descargas = "Asistencia_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();


                    let table = $('#table').DataTable({
                        "destroy": true,
                        pageLength: 25,
                        "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                        "processing": true,
                        "order": [],


                        //],
                        "language": textosEsp,
                        "columnDefs": [
                            {
                                "targets": [-1],
                                "orderable": false
                            },
                        ],
                        "columnDefs": [
                            {
                                "targets": [-1], //para la ultima columna
                                "orderable": false, //que no se ordene
                            },
                        ],

                        dom: 'fBrtipl',


                        buttons: [
                            {
                                extend: 'csvHtml5',
                                title: descargas,
                                text: '&nbsp;Exportar Csv', className: 'csvbtn'
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







                    $('#nav-tab a[href="#nav-empleados"]').tab('show');
                }


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
        $('#txtDescripcion').val('');



        prenomina.acccion = "nuevo";
        prenomina.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        obtenerFechaHoraServidor('txtFechaCreacion');



    },



    accionesBotones: () => {


        //Evita que las tabs sean clicables
        $('#nav-tab a').on('click', function (e) {
            e.preventDefault();
            e.stopImmediatePropagation();

        });




        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            panel.nuevo();

        });

        $('#btnSeleccionarSemana').on('click', (e) => {
            e.preventDefault();
            console.log('asignar semana');

            let numSemana = $('#txtNumSemana').val();
            $('.num-semana').text(numSemana);


        });


        $('#btnGuardar').on('click', (e) => {

            e.preventDefault();


            $('#panelLoading').modal('show');



            let parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.data = prenomina.arrayData;
            parametros.idObra = prenomina.idSeleccionado;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idEmpleado = sessionStorage.getItem("idempleado");
            parametros.accion = 'guardar';
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelRevisionNomina.aspx/Guardar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;


                    setTimeout(() => {

                        $('#panelLoading').modal('hide');


                    }, 2000);

                    if (valores.CodigoError === 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        $('#nav-tab a[href="#nav-obras"]').tab('show');


                        panel.cargarItems();

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {

                    $('#panelLoading').modal('hide');

                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });



        $('#btnAceptarAprobar').on('click', (e) => {

            e.preventDefault();
            $('#panelLoading').modal('show');


            let parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.data = prenomina.arrayData;
            parametros.idObra = prenomina.idObra;
            parametros.idPrenomina = prenomina.idPrenomina;
            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idEmpleado = sessionStorage.getItem("idempleado");
            parametros.accion = 'enviar';
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelRevisionNomina.aspx/Enviar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;

                    setTimeout(() => {

                        $('#panelLoading').modal('hide');

                        $('#nav-tab a[href="#nav-obras"]').tab('show');

                    }, 2000);

                    if (valores.CodigoError === 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');



                        panel.cargarItems();

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {

                    $('#panelLoading').modal('show');

                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });


        });

        $('#btnAceptarFinalizar').on('click', (e) => {

            e.preventDefault();
            console.table('finalizar');

            let listaPrenominas = panel.listaPrenominas.map((item) => {
                return item.IdPrenomina;
            });

            let parametros = new Object();
            parametros.data = listaPrenominas;
            parametros.path = window.location.hostname;

            parametros.idUsuario = sessionStorage.getItem("idusuario");
            parametros.idEmpleado = sessionStorage.getItem("idempleado");

            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelRevisionNomina.aspx/Finalizar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;



                    if (valores.CodigoError === 0) {
                        window.open("../pages/Download.ashx?path=" + window.location.hostname + "&id_documento=-1&tipo_descarga=2");

                        utils.toast(mensajesAlertas.exitoOperacion, 'ok');



                        panel.cargarItems();

                    } else {

                        utils.toast(valores.MensajeError, 'error');

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });
        });

        $('#btnAprobar').on('click', (e) => {

            e.preventDefault();


            $('#panelAprobar').modal('show');

        })


        $('#btnFinalizar').on('click', (e) => {

            e.preventDefault();




            $('#panelFinalizar').modal('show');

        });

        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#nav-tab a[href="#nav-obras"]').tab('show');


        });


        $('#btnEliminarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = prenomina.idSeleccionado;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelRevisionNomina.aspx/Eliminar",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        panel.cargarItems();

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

    panel.init();

    panel.accionesBotones();

});


