var accion = "";
var idSeleccionado = -1;
var idProyectoSeleccionado = -1;
var idProveedorSeleccionado = -1;
var idClienteSeleccionado = -1;
var idInsumoSeleccionado = -1;
var statusInsumoSeleccionado = -1;
var idStatusRechazo = -1;
var tablaSolicitudes;
var tablaClientes;
var tablaDestajos;
var tablaProveedores;
var date = new Date();
var descargas = "Insumos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
var pagina = '30';

var precios = [];
var sumatoriaTotal = [];
var subtotalRenglon = 0; // es el precio elegido (subtotal ó  cantidad * precioElegido )del insumo que se pasa a la siguiente tabla
var cantidadRenglon = 0;
var nombreInsumoSeleccionado = null;
var idTipoInsumoSeleccionado = null;
var importe;
var insumo;
$(document).ready(function () {





    console.log('Inicializando');
    $('#alert-operacion-ok').hide();
    $('#alert-operacion-fail').hide();



    //nuevo();

    var modo_insumos = sessionStorage.getItem("modo_insumos");
    console.log('modo_insumos=' + modo_insumos);

    var titulo = (modo_insumos === "1") ? "Insumos" : "Consulta de insumos";
    $('#spnTitulo').text(titulo);


    cargarComboTiposSolicitud();
    cargarComboEmpleados();
    cargarItemsProyectosParaSeleccionar();
    cargarItemsProveedores();

    cargarComboMarcas();
    cargarComboModelos();
    cargarComboTiposReparacion();
    cargarComboEstatus();
    cargarComboEmpleados();


    function cargarItemsProyectosParaSeleccionar() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.modoInsumos = sessionStorage.getItem("modo_insumos");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/GetListaProyectos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {



                table = $('#tablaProyectos').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'IdProyecto' },
                        { data: 'NumeroOrden' },
                        { data: 'Placas' },
                        { data: 'Color' },
                        { data: 'NombreMarca' },
                        { data: 'NombreModelo' },
                        { data: 'Anio' },
                        { data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1], //para la ultima columna
                            "orderable": false //que no se ordene
                        }
                    ],
                    dom: 'fBrtip',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Exportar Csv', className: 'csvbtn'
                        },
                        {
                            extend: 'pdfHtml5',
                            title: descargas,
                            text: 'Xls', className: 'pdfbtn'
                        }
                    ]

                });



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function cargarItemsProveedores() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/GetListaItemsProveedores",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {



                ////Se crea la tabla tipo bootstrap, msg.d es la lista de datos
                tablaProveedores = $('#tablaProveedores').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'Nombre' },
                        { data: 'Domicilio' },
                        { data: 'Telefono' },
                        { data: 'Celular' },
                        { data: 'Correo' },
                        { data: 'Accion' },

                    ],
                    "language": {
                        "sProcessing": "Procesando...",
                        "sLengthMenu": "Mostrar _MENU_ registros",
                        "sZeroRecords": "No se encontraron resultados",
                        "sEmptyTable": "Ningún dato disponible en esta tabla",
                        "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                        "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
                        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
                        "sInfoPostFix": "",
                        "sSearch": "Buscar:",
                        "sUrl": "",
                        "sInfoThousands": ",",
                        "sLoadingRecords": "Cargando...",
                        "oPaginate": {
                            "sFirst": "Primero",
                            "sLast": "Último",
                            "sNext": "Siguiente",
                            "sPrevious": "Anterior"
                        },
                        "oAria": {
                            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
                        }
                    },
                    "columnDefs": [
                        {
                            "targets": [-1], //para la ultima columna
                            "orderable": false, //que no se ordene
                        },
                    ],
                    dom: 'frtiplB'

                });



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }






    function cargarComboTiposSolicitud() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/GetListaTiposSolicitud",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                console.log('comboTipoInsumo = ' + msg.d);

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdTipoSolicitud + '">' + item.Nombre + '</option>';

                }

                $('#comboTipoInsumo').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function cargarComboEstatus() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaEstatus",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('status = ' + msg.d);

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdStatusProyecto + '">' + item.Nombre + '</option>';

                }

                $('#comboEstatus').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function cargarComboMarcas() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaMarcas",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('marcas = ' + msg.d);

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id_Marca + '">' + item.Nombre + '</option>';

                }

                $('#comboMarca').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function cargarComboModelos() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaModelos",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('modelos = ' + msg.d);

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.Id_Modelo + '">' + item.Nombre + '</option>';

                }

                $('#comboModelo').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }

    function cargarComboTiposReparacion() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaTiposReparacion",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('TiposReparacion = ' + msg.d);

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdTipoReparacion + '">' + item.Nombre + '</option>';

                }

                $('#comboTipoReparacion').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }


    function cargarComboEmpleados() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Proyectos.aspx/GetListaEmpleados",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                //console.log('empleados = ' + msg.d);

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdEmpleado + '">' + item.Nombre + '</option>';

                }

                $('#comboArmador').empty().append(opcion);
                $('#comboMontador').empty().append(opcion);
                $('#comboTecnico').empty().append(opcion);



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }


    $('#btnAbrirSeleccionarProyecto').click(function (e) {
        e.preventDefault();

        $('#panelSeleccionarProyecto').modal('show');


    });

    $('#btnAbrirSeleccionarProveedor').click(function (e) {
        e.preventDefault();

        $('#panelSeleccionarProveedor').modal('show');


    });

    $('#comboProveedorElegido').on('change', function (e) {

        console.log('cambio , posicion ' + $("#comboProveedorElegido :selected").index());
        console.log('cambio , precio ' + precios[$("#comboProveedorElegido :selected").index()]);
        var precio = precios[$("#comboProveedorElegido :selected").index()];

        if (precio !== -1) {
            $('#txtPrecioElegido').val(precio);
            $('#spnPrecioElegido').text(precio);

            $('.money').currency();
        } else {
            $('#txtPrecioElegido').val('');
            $('#spnPrecioElegido').text('');

        }

    });

    //Invocar un update para asociar el proveedor elegido para un insumo
    $('#btnGuardarElegirProveedor').click(function (e) {
        console.log('Boton proveedor');

        e.preventDefault();

        var hasErrors = $('form[name="frmElegirProveedor"]').validator('validate').has('.has-error').length;



        if (!hasErrors) {
            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idInsumo = $('#spnIdInsumoElegirProveedor').text();
            parametros.idProveedor = $('#comboProveedorElegido').val();
            parametros.precio = $('#spnPrecioElegido').text();
            parametros.observaciones = '';//$('#txtObservacionesFinales').val();
            parametros.numProveedor = 4;//el 4 es para el proveedor elegido
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Insumos.aspx/AsignarProveedor",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;

                    console.log('update = ' + valores[1]);

                    $('#panelElegirProveedor').modal('hide');


                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                    cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                    cargarCostosAdicionales(idProyectoSeleccionado);

                    cargarSumatoriaImportes(idProyectoSeleccionado);

                    cargarItemsDestajosArmador(idProyectoSeleccionado);

                    cargarItemsDestajosMontador(idProyectoSeleccionado);


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        }


    });



    //Asociar proveedor a un insumo, invoca un update
    $('#btnGuardarProveedor').click(function (e) {
        console.log('Boton proveedor');

        e.preventDefault();

        var hasErrors = $('form[name="frmProveedor"]').validator('validate').has('.has-error').length;


        if (idProveedorSeleccionado === '-1') {

            $('#spnMensajes').html('Debe seleccionar un proveedor ...');
            $('#panelMensajes').modal('show');




            $('#btnAbrirSeleccionarProveedor').focus();

            return; //fin de guardar, y ya no sigue hasta no seleccionar un proveedor
        }

        if (!hasErrors) {
            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idInsumo = $('#spnIdInsumo').text();
            parametros.idProveedor = idProveedorSeleccionado;
            parametros.precio = $('#txtPrecio').val();
            parametros.observaciones = $('#txtObservaciones').val();
            parametros.numProveedor = $('#spnNumProveedor').text();
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/Insumos.aspx/AsignarProveedor",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;

                    console.log('update = ' + valores[1]);

                    $('#panelEditarProveedor').modal('hide');


                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                    cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                    cargarCostosAdicionales(idProyectoSeleccionado);
                    cargarSumatoriaImportes(idProyectoSeleccionado);

                    cargarItemsDestajosArmador(idProyectoSeleccionado);

                    cargarItemsDestajosMontador(idProyectoSeleccionado);


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        }

    });


    //Guardar datos generales de un insumo
    $('#btnGuardarInsumo').click(function (e) {
        console.log('Boton guardar insumo');

        e.preventDefault();


        if (idProyectoSeleccionado === -1) {

            $('#spnMensajes').html('Debe seleccionar un proyecto ...');
            $('#panelMensajes').modal('show');




            $('#btnAbrirSeleccionarProyecto').focus();

            return; //fin de guardar, y ya no sigue hasta no seleccionar un proyecto
        }

        var hasErrors = $('form[name="frmInsumo"]').validator('validate').has('.has-error').length;


        if (!hasErrors) {


            console.log('idSeleccionado =' + idSeleccionado);

            // debugger;;
            var item = new Object();


            item.IdInsumo = idSeleccionado;
            item.IdProyecto = idProyectoSeleccionado;
            item.Fecha = $('#txtFecha').val();
            item.Hora = $('#txtHora').val();
            item.IdTipoSolicitud = $('#comboTipoInsumo').val();
            item.IdEmpleado = $('#comboTecnico').val();

            item.Cantidad = $('#txtCant').val();
            item.Descripcion = $('#txtDescripcion').val();
            item.IdStatusInsumo = 1;


            ////validar que la cantidad a usar no sobrepase la existencia en caso de ser aceite


            //var parametrosConsulta = new Object();
            //parametrosConsulta.path = window.location.hostname;
            //parametrosConsulta.idTipoSolicitud = item.IdTipoSolicitud;
            //parametrosConsulta = JSON.stringify(parametrosConsulta);
            //$.ajax({
            //    type: "POST",
            //    url: "../pages/Insumos.aspx/GetListaTipoSolicitud",
            //    data: parametrosConsulta,
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    async: true,
            //    success: function (msg) {
            //        var datosProducto = msg.d;

            //        if (datosProducto.IdCategoria === 2 && parseFloat(datosProducto.Existencia) < parseFloat(item.Cantidad)) {//categoria  = 2, son de categoria aceites


            //            $('#spnMensajes').html('La cantidad <strong>' + cantidadRenglon + '</strong>' + 
            //                ' sobrepasa a la existencia del producto/tipo de solicitud seleccionado.<br/>' +
            //                'Tipo de insumo = <strong>' + $('#comboTipoInsumo option:selected').text() + '</strong> Existencia = <strong>' + datosProducto.Existencia + '</strong>');
            //            $('#panelMensajes').modal('show');


            //        } else {
                        //todo bien, guardar

                        var parametros = new Object();
                        parametros.path = window.location.hostname;
                        parametros.insumo = item;
                        parametros.accion = accion;
                        parametros = JSON.stringify(parametros);
                        $.ajax({
                            type: "POST",
                            url: "../pages/Insumos.aspx/Guardar",
                            data: parametros,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            success: function (msg) {
                                var valores = msg.d;

                                var resultado = parseInt(valores[0]);//0 = el num de registros modificados o menos 1 hubo cuando error 
                                var mensaje = valores[1];//mensaje de error

                                console.log(mensaje);
                                $('#spnMensajes').html(mensaje);
                                $('#panelMensajes').modal('show');


                                if (resultado > 0) {

                                    $("#lblMensajesOk").text('Registro guardado correctamente...');
                                    $("#alert-operacion-ok").show("fast", function () {
                                        setTimeout(function () {
                                            $("#alert-operacion-ok").hide("fast");
                                        }, 3000);
                                    });


                                    //ocultar panel flotante
                                    $('#panelEditarInsumo').modal('hide');

                                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                                    cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                                    cargarCostosAdicionales(idProyectoSeleccionado);
                                    cargarSumatoriaImportes(idProyectoSeleccionado);

                                    cargarItemsDestajosArmador(idProyectoSeleccionado);

                                    cargarItemsDestajosMontador(idProyectoSeleccionado);



                                } else {

                                    $("#lblMensajesFail").text('No se pudo guardar el registro...');

                                    $("#alert-operacion-fail").show("fast", function () {
                                        setTimeout(function () {
                                            $("#alert-operacion-fail").hide("fast");
                                        }, 3000);
                                    });

                                }




                            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                                console.log(textStatus + ": " + XMLHttpRequest.responseText);
                            }

                        });



                    }




            //    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            //        console.log(textStatus + ": " + XMLHttpRequest.responseText);
            //    }

            //});






        //}

    });



    $('#btnAutorizarAceptar').click(function (e) {





        var monto25 = parseFloat($('#txtValor35MasCostoAdicional').attr('data-valor35mascosto'));
        console.log('monto25=' + monto25);

        var sumatoriaGeneral = sumatoriaTotal[1] + sumatoriaTotal[2] + sumatoriaTotal[3] + subtotalRenglon;
        //debugger;
        console.log(sumatoriaTotal);

        console.log('sumatoriaGeneral (acumulado de insumos) = ' + sumatoriaGeneral);

        if (sumatoriaGeneral > monto25) {
            $('#spnMensajes').html('El acumulado de costos  sobrepasa el 25% + costos adicionales. No se puede autorizar este insumo.');
            $('#panelMensajes').modal('show');

            return;
        }

       

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idInsumo = idInsumoSeleccionado;
        parametros.idStatusInsumo = 2;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/AutorizarInsumo",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado > 0) {



                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                    cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                    cargarCostosAdicionales(idProyectoSeleccionado);
                    cargarSumatoriaImportes(idProyectoSeleccionado);

                    cargarItemsDestajosArmador(idProyectoSeleccionado);

                    cargarItemsDestajosMontador(idProyectoSeleccionado);


                    $('#panelAutorizar').modal('hide');

                    $('#spnMensajes').html('Registro autorizado correctamente.');
                    $('#panelMensajes').modal('show');

                    idProveedorSeleccionado = -1;

                } else {
                    $('#spnMensajes').html('Error: No se pudo autorizar el registro.');
                    $('#panelMensajes').modal('show');

                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });












    });


    $('#btnEntregarAceptar').click(function (e) {

        var monto25 = parseFloat($('#txtValor35MasCostoAdicional').attr('data-valor35mascosto'));
        console.log('monto25=' + monto25);

        var sumatoriaGeneral = sumatoriaTotal[1] + sumatoriaTotal[2] + sumatoriaTotal[3];
        //debugger;
        console.log('sumatoriaGeneral= ' + sumatoriaGeneral);

        if (sumatoriaGeneral > monto25) {
            $('#spnMensajes').html('El acumulado de costos sobrepasa el 25% + costos adicionales. No se puede autorizar este insumo.');
            $('#panelMensajes').modal('show');

            return;
        }



        var parametrosConsulta = new Object();
        parametrosConsulta.path = window.location.hostname;
        parametrosConsulta.idTipoSolicitud = idTipoInsumoSeleccionado;
        parametrosConsulta = JSON.stringify(parametrosConsulta);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/GetListaTipoSolicitud",
            data: parametrosConsulta,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var datosProducto = msg.d;

                if (datosProducto.IdCategoria === 2 && parseFloat(datosProducto.Existencia) < parseFloat(cantidadRenglon)) {//categoria  = 2, son de categoria aceites


                    $('#spnMensajes').html('La cantidad <strong>' + cantidadRenglon + '</strong>' + 
                        ' sobrepasa a la existencia del producto/tipo de solicitud seleccionado.<br/>' +
                        'Tipo de insumo = <strong>' + nombreInsumoSeleccionado  + '</strong> Existencia = <strong>' + datosProducto.Existencia + '</strong>');
                    $('#panelMensajes').modal('show');


                } else {




                    var parametros = new Object();
                    parametros.path = window.location.hostname;
                    parametros.idInsumo = idInsumoSeleccionado;
                    parametros.idStatusInsumo = 3;
                    parametros = JSON.stringify(parametros);
                    $.ajax({
                        type: "POST",
                        url: "../pages/Insumos.aspx/EntregarInsumo",
                        data: parametros,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: true,
                        success: function (msg) {

                            var resultado = msg.d;
                            if (resultado.mensajeError === null) {

                                cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                                cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                                cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                                cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                                cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                                cargarCostosAdicionales(idProyectoSeleccionado);
                                cargarSumatoriaImportes(idProyectoSeleccionado);

                                cargarItemsDestajosArmador(idProyectoSeleccionado);

                                cargarItemsDestajosMontador(idProyectoSeleccionado);


                                $('#panelEntregar').modal('hide');

                                $('#spnMensajes').html('<br/>' + resultado.mensaje);
                                $('#panelMensajes').modal('show');

                            } else {
                                $('#spnMensajes').html('Error: No se pudo terminar la operación. <br/>' + resultado.mensajeError);
                                $('#panelMensajes').modal('show');

                            }


                        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                            console.log(textStatus + ": " + XMLHttpRequest.responseText);
                        }

                    });

                }

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });




    });

    $('#btnVerificarAceptar').click(function (e) {







        var hasErrors = $('form[name="frmVerificar"]').validator('validate').has('.has-error').length;



        if (hasErrors) {


            return;
        }







        var monto25 = parseFloat($('#txtValor35MasCostoAdicional').attr('data-valor35mascosto'));
        console.log('monto25=' + monto25);

        var nuevoPrecio = parseFloat($('#txtPrecioFinal').val());
        var nuevoSubtotalRenglon = parseFloat(cantidadRenglon) * nuevoPrecio;

        var sumatoriaGeneral = (sumatoriaTotal[1] + sumatoriaTotal[2] + sumatoriaTotal[3] - subtotalRenglon) + nuevoSubtotalRenglon;
      
        console.log('sumatoriaGeneral (acumulado de costos)= ' + sumatoriaGeneral);

        //debugger;
        if (sumatoriaGeneral > monto25) {
            $('#spnMensajes').html('La cantidad del insumo seleccionado es : <strong>' + cantidadRenglon +
                '</strong><p>El acumulado de costos de insumos (<strong>' + number_format(sumatoriaGeneral, 2) +
                '</strong>) sobrepasa el 25% + costos adicionales (<strong>' + number_format(monto25) + '</strong>).</p> ' +
                'No se puede autorizar este insumo.');
            $('#panelMensajes').modal('show');

            return;
        }


        ////TODO: TEST
        //return;
     


        var cantidadPrecioFinal = parseFloat($('#txtPrecioFinal').val());
        var cantidadPrecioFinalMasUno = insumo.PrecioElegido + 1;
        var cantidadPrecioFinalMenosUno = insumo.PrecioElegido - 1;
        console.log("subtotal_importe_insumo: " + insumo.PrecioElegido);



        console.log("cantidad precio final__: " + cantidadPrecioFinal);

        console.log("cantidad precio final mas uno: " + cantidadPrecioFinalMasUno);
        console.log("cantidad precio final menos uno: " + cantidadPrecioFinalMenosUno);

        if (cantidadPrecioFinal < cantidadPrecioFinalMenosUno || cantidadPrecioFinal > cantidadPrecioFinalMasUno) {
            $('#spnMensajes').html('Cantidad precio final sobrepasa el subtotal  + $1, ó es menor a el subtotal - $1');
            $('#panelMensajes').modal('show');

          
            return;

        }

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idInsumo = idInsumoSeleccionado;
        parametros.idStatusInsumo = 4;
        parametros.referencia = $('#txtReferencia').val();
        parametros.precioFinal = $('#txtPrecioFinal').val();
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/VerificarInsumo",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado > 0) {



                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                    cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                    cargarCostosAdicionales(idProyectoSeleccionado);
                    cargarSumatoriaImportes(idProyectoSeleccionado);

                    cargarItemsDestajosArmador(idProyectoSeleccionado);

                    cargarItemsDestajosMontador(idProyectoSeleccionado);


                    $('#panelVerificar').modal('hide');

                    $('#spnMensajes').html('Operación terminada correctamente.');
                    $('#panelMensajes').modal('show');

                } else {
                    $('#spnMensajes').html('Error: No se pudo terminar la operación.');
                    $('#panelMensajes').modal('show');

                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });




    });




    $('#btnEliminarAceptar').click(function (e) {

        //Ajax para eliminar el registro
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = idInsumoSeleccionado;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/EliminarInsumo",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado > 0) {



                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                    cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                    cargarCostosAdicionales(idProyectoSeleccionado);
                    cargarSumatoriaImportes(idProyectoSeleccionado);

                    cargarItemsDestajosArmador(idProyectoSeleccionado);

                    cargarItemsDestajosMontador(idProyectoSeleccionado);


                    $('#panelEliminar').modal('hide');

                    $('#spnMensajes').html('Registro eliminado correctamente.');
                    $('#panelMensajes').modal('show');




                } else {
                    $('#spnMensajes').html('Error: No se pudo eliminar el registro.');
                    $('#panelMensajes').modal('show');

                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });




    });





    $('#btnRechazarAceptar').click(function (e) {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idInsumo = idInsumoSeleccionado;
        parametros.idStatusInsumo = idStatusRechazo;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/Insumos.aspx/RechazarInsumo",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado > 0) {

                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
                    cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

                    cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

                    cargarCostosAdicionales(idProyectoSeleccionado);
                    cargarSumatoriaImportes(idProyectoSeleccionado);

                    cargarItemsDestajosArmador(idProyectoSeleccionado);

                    cargarItemsDestajosMontador(idProyectoSeleccionado);


                    $('#panelRechazar').modal('hide');

                    $('#spnMensajes').html('Registro enviado a etapa anterior correctamente.');
                    $('#panelMensajes').modal('show');


                } else {
                    $('#spnMensajes').html('Error: No se pudo actualizar el registro.');
                    $('#panelMensajes').modal('show');

                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });




    });



});//fin de ready

function seleccionarProveedor(id) {
    console.log('Desde la funcion seleccionarProveedor');
    console.log('Id ' + id);

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Proveedores.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var datosProveedor = msg.d;
            $('#txtNombreProveedor').val(datosProveedor.Nombre);
            $('#txtDomicilio').val(datosProveedor.Domicilio);



            idProveedorSeleccionado = datosProveedor.IdProveedor;

            $('#panelSeleccionarProveedor').modal('hide');


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}


function seleccionarProyecto(id) {
    console.log('Desde la funcion seleccionarProyecto');
    console.log('Id ' + id);

    //Ajax para traer todos los datos del registro
    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Insumos.aspx/GetItemProyecto",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var item = msg.d;
            idProyectoSeleccionado = item.IdProyecto;
            idClienteSeleccionado = item.IdCliente;
            $('#txtNumProyecto').val(item.IdProyecto);
            $('#txtFechaEntrada').val(item.FechaEntradaISO);
            $('#txtHoraEntrada').val(item.HoraEntrada);
            $('#txtNumOrden').val(item.NumeroOrden);
            $('#comboTipoReparacion').val(item.IdTipoReparacion);
            $('#comboMarca').val(item.IdMarca);
            $('#comboModelo').val(item.IdModelo);
            $('#txtColor').val(item.Color);
            $('#txtAnio').val(item.Anio);
            $('#txtKilometraje').val(item.Kilometraje);
            $('#txtPlacas').val(item.Placas);
            $('#txtDiagnostico').val(item.Diagnostico);
            $('#txtS01').val(item.Area);
          $('#comboArmador').val(item.IdArmador);
            $('#comboMontador').val(item.IdMontador);
            $('#txtSemanaAlta').val(item.NumeroSemanaAlta);
            $('#txtDiaAlta').val(item.NumeroDiaAlta);
            $('#txtValorAlta').val(item.ValorAlta);
            $('#comboEstatus').val(item.IdStatusProyecto);
            $('#txtDescripcionServicio1').val(item.DescripcionServicio1);
            $('#txtDescripcionServicio2').val(item.DescripcionServicio2);
            $('#txtDescripcionServicio3').val(item.DescripcionServicio3);
            $('#txtFechaProMesaEntrega').val(item.FechaPromesaEntregaISO);
            $('#txtHoraProMesaEntrega').val(item.HoraPromesaEntrega);
            $('#txtFechaPresupuesto').val(item.FechaPresupuestoISO);
            $('#txtValorVenta').val(item.ValorVenta);
            $('#txtValorCobro').val(item.ValorCobro);
            $('#txtValor35').val(item.Valor35);
            $('#txtValor35').attr('data-valor35', item.Valor35);


            //datos cliente
            $('#txtNombre').val(item.Nombre);
            $('#txtCalle_No').val(item.Calle_y_No);
            $('#txtColonia').val(item.Colonia);
            $('#txtTelefono_1').val(item.Telefono_1);
            $('#txtCelular').val(item.Celular);
            $('#txtCorreo_Electronico').val(item.Correo_Electronico);


            cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaSolicitudes', 1);
            cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaAutorizados', 2);
            cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaEntregados', 3);
            cargarItemsSolicitudes(idProyectoSeleccionado, 'tablaVerificados', 4);

            cargarItemsEliminados(idProyectoSeleccionado, 'tablaEliminados');

            //debugger;

            cargarCostosAdicionales(idProyectoSeleccionado);

            cargarSumatoriaImportes(idProyectoSeleccionado);

            cargarItemsDestajosArmador(idProyectoSeleccionado);

            cargarItemsDestajosMontador(idProyectoSeleccionado);


            $('.money').currency();


            $('#panelSeleccionarProyecto').modal('hide');


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}


function nuevo() {
    console.log('Desde nuevo');
    //e.preventDefault();

    $('#frmInsumo')[0].reset();
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#spnTituloForm').text('Nuevo insumo');
    $('#txtObservaciones').val('');
    $('#txtDescripcion').val('');




    $('#panelEditarInsumo').modal('show');
    accion = "nuevo";
    id = -1;
    idSeleccionado = -1;

    obtenerFechaHoraServidor();




}





function cargarItemsEliminados(idProyecto, tabla) {

    console.log("desde  cargarItemsEliminados");


    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idProyecto = idProyecto;
    parametros.idUsuario = sessionStorage.getItem("idusuario");
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Insumos.aspx/GetListaInsumosEliminadosPorProyecto",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            $('#' + tabla + '>tbody').empty();

            var elegido = 0;

            var fila = "";
            for (var i = 0; i < msg.d.length; i++) {
                fila = "";


                var item = msg.d[i];
                fila += '<tr>';



                fila += '<td>' + item.FechaEliminacionISO + ' ' + item.HoraEliminacion + '</td> ';

                fila += '<td>' + item.NombreTipoInsumo + '</td> ';
                fila += '<td>' + item.NombreEmpleado + '</td> ';
                fila += '<td>' + item.Cantidad + '</td> ';

                fila += '<td>' + item.Descripcion + '</td> ';
                fila += '<td>' + item.NombreProveedor1 + '</td> ';
                fila += '<td class="money">' + item.PrecioProveedor1 + '</td> ';
                fila += '<td>' + item.Observaciones1 + '</td> ';

                fila += '<td>' + item.NombreProveedor2 + '</td> ';
                fila += '<td class="money">' + item.PrecioProveedor2 + '</td> ';
                fila += '<td>' + item.Observaciones2 + '</td> ';

                fila += '<td>' + item.NombreProveedor3 + '</td> ';
                fila += '<td class="money">' + item.PrecioProveedor3 + '</td> ';
                fila += '<td>' + item.Observaciones3 + '</td> ';

                fila += '<td>' + item.NombreProveedorElegido + '</td> ';

                var importe = item.Cantidad * item.PrecioElegido;

                fila += '<td> <span class="money">' + item.PrecioElegido + '</span></td> ';

                fila += '<td> <span class="money">' + importe + '</span></td> ';




                elegido += parseFloat(importe);

                //fila += '<td>' + item.Accion + '</td> ';
                fila += '</tr>';

                $('#' + tabla + '>tbody').append(fila);

            }


            var columnaExtraSolicitudes = "";
            columnaExtraSolicitudes = '<td colspan="16" style="text-align:right;"><label class="label label-success form-control_">Total</label></td>';

            fila = "<tr>";
            fila += columnaExtraSolicitudes;
            fila += '<td class="money"><label class="label label-success">' + elegido + '</label></td>';
            fila += "</tr>";


            $('#' + tabla + '>tbody').append(fila);

            $('.money').currency();




        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

}



function cargarCostosAdicionales(idProyecto) {

    console.log("desde  cargarCostosAdicionales");


    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idProyecto = idProyecto;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/CostosAdicionales.aspx/GetSumatoriaCostos_ItemsPorProyecto",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var total = msg.d;


            $('#txtSumaCostoAdicional').val(total);


            var valor35 = $('#txtValor35').attr('data-valor35');
            //debugger;
            var txtValor35MasCostoAdicional = parseFloat(total) + parseFloat(valor35);

            $('#txtValor35MasCostoAdicional').val(txtValor35MasCostoAdicional);
            $('#txtValor35MasCostoAdicional').attr('data-valor35mascosto', txtValor35MasCostoAdicional);

            $('.money').currency();

        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

}


function cargarItemsSolicitudes(idProyecto, tabla, idStatus) {

    //console.log("desde cargar items solicitudes");


    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idProyecto = idProyecto;
    parametros.idStatus = idStatus;
    parametros.idUsuario = sessionStorage.getItem("idusuario");
    parametros.modoInsumos = sessionStorage.getItem("modo_insumos");
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Insumos.aspx/GetListaInsumosPorProyectoYStatus",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            $('#' + tabla + '>tbody').empty();

            var elegido = 0;

            var fila = "";
            for (var i = 0; i < msg.d.length; i++) {
                fila = "";


                var item = msg.d[i];
                fila += '<tr class=\'clickable-row\' >';


                fila += '<td>' + item.FechaISO + ' ' + item.Hora + '</td> ';

                console.log('item.IdStatusInsumo = ' + item.IdStatusInsumo);

                if (idStatus >= 2) {//si es autorizacion hacia arriba mostrar esta columna

                    fila += '<td>' + item.FechaAutorizacionISO + ' ' + item.HoraAutorizacion + '</td> ';
                }
                if (idStatus >= 3) {//si es autorizacion hacia arriba mostrar esta columna
                    fila += '<td>' + item.FechaEntregaISO + ' ' + item.HoraEntrega + '</td> ';
                }
                if (idStatus >= 4) {//si es verificacion hacia arriba mostrar esta columna

                    fila += '<td>' + item.FechaVerificacionISO + ' ' + item.HoraVerificacion + '</td> ';
                    fila += '<td>' + item.Referencia + '</td> ';

                }


                fila += '<td>' + item.NombreTipoInsumo + '</td> ';
                fila += '<td>' + item.NombreEmpleado + '</td> ';
                fila += '<td>' + item.Cantidad + '</td> ';

                fila += '<td>' + item.Descripcion + '</td> ';
                fila += '<td>' + item.NombreProveedor1 + '</td> ';
                fila += '<td class="money">' + item.PrecioProveedor1 + '</td> ';
                fila += '<td>' + item.Observaciones1 + '</td> ';

                fila += '<td>' + item.NombreProveedor2 + '</td> ';
                fila += '<td class="money">' + item.PrecioProveedor2 + '</td> ';
                fila += '<td>' + item.Observaciones2 + '</td> ';

                fila += '<td>' + item.NombreProveedor3 + '</td> ';
                fila += '<td class="money">' + item.PrecioProveedor3 + '</td> ';
                fila += '<td>' + item.Observaciones3 + '</td> ';

                fila += '<td>' + item.NombreProveedorElegido + '</td> ';

                importe = item.Cantidad * item.PrecioElegido;

                fila += '<td> <span class="money">' + item.PrecioElegido + '</span></td> ';
                //fila += '<td> <span class="sumar">' + item.PrecioElegido + '</span></td> ';
                //fila += '<td>' + item.ObservacionesFinales + '</td> ';
                // fila += '<td>' + item.Referencia + '</td> ';

                fila += '<td> <span class="money">' + importe + '</span></td> ';

                

               


                elegido += parseFloat(importe);

                fila += '<td>' + item.Accion + '</td> ';
                fila += '</tr>';

                $('#' + tabla + '>tbody').append(fila);

            }

            sumatoriaTotal[idStatus - 1] = elegido;//sumatoria[0]}=sumatoriasolicitud, sumatoria[1]=sumatoriaautorizacion,...

            var columnaExtraSolicitudes = "";
            if (idStatus === 1) {
                columnaExtraSolicitudes = '<td colspan="16" style="text-align:right;"><label class="label label-success form-control_">Total</label></td>';
            }
            if (idStatus === 2) {
                columnaExtraSolicitudes = '<td colspan="17"style="text-align:right;"><label class="label label-success form-control_">Total</label></td>';
            }
            if (idStatus === 3) {
                columnaExtraSolicitudes = '<td colspan="18"style="text-align:right;"><label class="label label-success form-control_">Total</label></td>';
            }

            if (idStatus === 4) {
                columnaExtraSolicitudes = '<td colspan="20"style="text-align:right;"><label class="label label-success form-control_">Total</label></td>';
            }
            console.log("la suma es: " + sumatoriaTotal[idStatus - 1]);
            fila = "<tr>";
            fila += columnaExtraSolicitudes;
            fila += '<td class="money"><label class="label label-success">' + elegido + '</label></td>';
            fila += "</tr>";


            $('#' + tabla + '>tbody').append(fila);

            $('.money').currency();




            $('.boton-agregar-proveedor').click(function (e) {
                e.preventDefault();
                console.log('agregar proveedor');

                idProveedorSeleccionado = '-1';
                $('#frmProveedor')[0].reset();

                $('#txtObservaciones').val('');
                $('#txtDescripcion').val('');

                var numProveedor = $(this).attr('data-numproveedor');
                var idInsumo = $(this).attr('data-idinsumo');

                $('#spnNumProveedor').html('<strong>' + numProveedor + '</strong>');
                $('#spnIdInsumo').text(idInsumo);


                $('#panelEditarProveedor').modal('show');


            });


            $('.boton-elegir-proveedor').click(function (e) {
                e.preventDefault();
                console.log('elegir proveedor');

                $('#frmElegirProveedor')[0].reset();
                $('#txtObservaciones').val('');
                $('#txtDescripcion').val('');

                var idInsumo = $(this).attr('data-idinsumo');

                $('#spnIdInsumoElegirProveedor').text(idInsumo);

                //cargar combo de proveedores elegidos

                var parametros = new Object();
                parametros.path = window.location.hostname;
                parametros.idInsumo = idInsumo;
                parametros = JSON.stringify(parametros);
                $.ajax({
                    type: "POST",
                    url: "../pages/Insumos.aspx/GetProveedoresAsociadosAInsumo",
                    data: parametros,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        console.log('comboProveedorElegido = ' + msg.d);

                        precios = [];

                        var items = msg.d;
                        var opcion = "";
                        opcion += '<option value="">Seleccione...</option>';
                        precios.push(-1);

                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];

                            opcion += '<option value="' + item.IdProveedor + '">' + item.Nombre + '</option>';
                            precios.push(item.Precio);
                        }

                        $('#comboProveedorElegido').empty().append(opcion);



                        $('#panelElegirProveedor').modal('show');


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            });


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

}


function cargarSumatoriaImportes(idProyecto) {

    //console.log("desde cargar items cargarSumatoriaImportes");


    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idProyecto = idProyecto;

    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Insumos.aspx/GetSumatoriaImportesInsumosPorProyecto",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var total = msg.d;

            $('#txtTotalInsumos').val(total);
            $('#txtTotalInsumos').attr('data-totalinsumos', total);

            console.log('sumatoria total importes = ' + total);
            $('.money').currency();


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

}



function autorizarInsumo(id) {


    //console.log('Desde la funcion editar');
    console.log('Id insumo ' + id);
    console.log('status insumo ' + status);
    idInsumoSeleccionado = id;
    //debugger;
    subtotalRenglon = 0;
    cantidadRenglon = 0;

    //traer de base de datos el insumo
    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idInsumo = idInsumoSeleccionado;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Insumos.aspx/GetInsumoPorId",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var insumoEncontrado = msg.d;


            subtotalRenglon = insumoEncontrado.Subtotal;//subtotalRenglon = precioElegido * cantidad
            cantidadRenglon = insumoEncontrado.Cantidad;

            console.log('cantidad renglon = ' + insumoEncontrado.Cantidad);
            console.log('subtotalRenglon  = ' + insumoEncontrado.Subtotal);


            //Ver si ya tiene un proveedor elegido
            if (insumoEncontrado.IdProveedorElegido === null || insumoEncontrado.IdProveedorElegido === 0) {

                $('#spnMensajes').html('Error: Para apoder autorizar, primero debe elegir un proveedor.');
                $('#panelMensajes').modal('show');

                return;
            }

            if (cantidadRenglon !== null) {


                $('#panelAutorizar').modal('show');
            } else {
                console.log('no se pudo traer datos de insumo');

            }

        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });




}

function entregarInsumo(id) {


    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idInsumo = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Insumos.aspx/GetInsumoPorId",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var insumo = msg.d;
            cantidadRenglon = insumo.Cantidad;
            nombreInsumoSeleccionado = insumo.NombreTipoInsumo;
            idTipoInsumoSeleccionado = insumo.IdTipoSolicitud;
                
            console.log('cantidad renglon = ' + insumo.Cantidad);

            idInsumoSeleccionado = id;

            $('#panelEntregar').modal('show');


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

    

}

function verificarInsumo(id) {


    subtotalRenglon = 0;
    cantidadRenglon = 0;



    //traer de base de datos el insumo
    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idInsumo = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/Insumos.aspx/GetInsumoPorId",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

             insumo = msg.d;


            subtotalRenglon = insumo.Subtotal;//subtotalRenglon = precioElegido * cantidad
            cantidadRenglon = insumo.Cantidad;

            console.log('cantidad renglon = ' + insumo.Cantidad);
            console.log('subtotalRenglon  = ' + insumo.Subtotal);


            idInsumoSeleccionado = id;

            if (cantidadRenglon !== null) {
                $('#frmVerificar')[0].reset();
                $('#txtCantidadRenglon').val(cantidadRenglon);

                $('#panelVerificar').modal('show');
            } else {
                console.log('no se pudo traer datos de insumo');

            }

        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });





}

function eliminarInsumo(id) {
    //console.log('Desde la funcion editar');
    //console.log('Id ' + id);
    idInsumoSeleccionado = id;
    //$('.modal-title').text('Confirmar');
    ////mostrar confirmación de eliminar
    $('#panelEliminar').modal('show');

}



function rechazarInsumo(id, idStatusRechazo_) {
    //console.log('Desde la funcion rechazar');
    //console.log('Id ' + id);
    idInsumoSeleccionado = id;
    idStatusRechazo = idStatusRechazo_;
    //$('.modal-title').text('Confirmar');
    ////mostrar confirmación de eliminar
    $('#panelRechazar').modal('show');

}






function cargarItemsDestajosArmador(idProyecto) {

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idProyecto = idProyecto;
    parametros.idUsuario = sessionStorage.getItem("idusuario");
    parametros.tipoTecnico = 0;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/DestajoTecnico.aspx/GetItemsDestajosPorProyectoArmador",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            $('#tablaItemsDestajosArmador >tbody').empty();

            var valoresDestajos = msg.d;

            var datos = valoresDestajos.listaItems;


            //debugger;
            for (var i = 0; i < datos.length; i++) {
                var fila = "";


                var item = datos[i];
                fila += '<tr>';


                fila += '<td>' + item.IdDestajoTecnicoStr + '</td> ';
                fila += '<td>' + item.FechaStr + ' ' + item.Hora + '</td> ';

                fila += '<td>' + item.Descripcion + '</td> ';
                fila += '<td>' + item.TipoEmpleadoStr + '</td> ';


                fila += '<td style="text-align:right;"><span class="money">' + item.Cantidad + '</span></td> ';


                fila += '<td>' + item.NumeroSemanaStr + '</td> ';

                

                fila += '</tr>';

                $('#tablaItemsDestajosArmador >tbody').append(fila);

            }


            fila = '<tr>';

            fila += '<td colspan="4" style="text-align:right;"><label class="label label-success form-control_">Total</td> ';
            fila += '<td style="text-align:right;"><span class="money">' + valoresDestajos.Totalizado + '</span></td> ';


            fila += '<td></td> ';

            fila += '</tr>';


            $('#tablaItemsDestajosArmador >tbody').append(fila);

            $('.money').currency();




        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

}


function cargarItemsDestajosMontador(idProyecto) {

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.idProyecto = idProyecto;
    parametros.idUsuario = sessionStorage.getItem("idusuario");
    parametros.tipoTecnico = 0;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/DestajoTecnico.aspx/GetItemsDestajosPorProyectoMontador",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            $('#tablaItemsDestajosMontador >tbody').empty();

            var valoresDestajos = msg.d;

            var datos = valoresDestajos.listaItems;


            //debugger;
            for (var i = 0; i < datos.length; i++) {
                var fila = "";


                var item = datos[i];
                fila += '<tr>';


                fila += '<td>' + item.IdDestajoTecnicoStr + '</td> ';
                fila += '<td>' + item.FechaStr + ' ' + item.Hora + '</td> ';

                fila += '<td>' + item.Descripcion + '</td> ';
                fila += '<td>' + item.TipoEmpleadoStr + '</td> ';


                fila += '<td style="text-align:right;"><span class="money">' + item.Cantidad + '</span></td> ';


                fila += '<td>' + item.NumeroSemanaStr + '</td> ';



                fila += '</tr>';

                $('#tablaItemsDestajosMontador >tbody').append(fila);

            }


            fila = '<tr>';

            fila += '<td colspan="4" style="text-align:right;"><label class="label label-success form-control_">Total</td> ';
            fila += '<td style="text-align:right;"><span class="money">' + valoresDestajos.Totalizado + '</span></td> ';


            fila += '<td></td> ';

            fila += '</tr>';


            $('#tablaItemsDestajosMontador >tbody').append(fila);

            $('.money').currency();





        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });

}

