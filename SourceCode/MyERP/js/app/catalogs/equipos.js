var accion = "";
var idSeleccionado = "-1";
var idClienteSeleccionado = "-1";
var idCitaSeleccionado = "-1";
var table;
var tablaCerrados;
var tablaClientes;
var tablaCitas;
var date = new Date();
var descargas = "Equipos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
var pagina = '54';

$(document).ready(function () {


    $('#alert-operacion-ok').hide();
    $('#alert-operacion-fail').hide();

    $('#panelTabla').show();
    $('#panelForm').hide();
    $('#panelTablaCerrados').hide();


    cargarItems();
    datosEquipo.cargarComboMarcas();
    datosEquipo.cargarComboUbicaciones();
    cargarOperadores();
    datosEquipo.cargarComboUnidadesMedida();





    function cargarItems() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/GetListaItems",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {



                table = $('#table').DataTable({
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'NumeroEconomico' },
                        { data: 'Nombre' },
                        { data: 'NombreMarca' },
                        { data: 'NombreModelo' },
                        { data: 'ActivoStr' },
                        { data: 'OrometroUltimoMantenimiento' },
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



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    }


    function cargarOperadores() {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/MantenimientoPanelRegistroFallas.aspx/GetListaOperadores",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {


                var datos = JSON.parse(msg.d);
                var dataAdapter = new $.jqx.dataAdapter(datos);


                $("#comboOperador").jqxDropDownList({
                    source: dataAdapter, displayMember: "Nombre", valueMember: "IdEmpleado", width: '93%',
                    height: '20px', placeHolder: "Seleccione:", filterable: true, searchMode: 'containsignorecase',
                    filterPlaceHolder: 'Buscar'
                });
                $("#comboOperador").jqxDropDownList('clearSelection');



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }
        });

    }




    $('#btnNuevo').click(function (e) {
        e.preventDefault();
        nuevo();
        //valoresPrueba();

    });


    $('#comboMarca').on('change', function (e) {

        e.preventDefault();
        datosEquipo.cargarComboModelos($('#comboMarca').val(), 0);


    });


    
    function valoresPrueba() {
        //$('#txtFechaEntrada').val('2018-07-15');
        //$('#txtHoraEntrada').val('10:20');
        $('#txtNumOrden').val('1');
        $('#txtNumProyecto').val(idSeleccionado);
        //$('#comboCliente').val('');
        //$('#comboTipoReparacion').val('');
        //$('#comboMarca').val('');
        //$('#comboModelo').val('');
        $('#txtColor').val('Negro');
        $('#txtAnio').val('2011');
        $('#txtKilometraje').val('192933');
        $('#txtPlacas').val('XXX911');
        $('#txtDiagnostico').val('Diagnostico ...');
        $('#txtS01').val('S01');//area
        //$('#comboArmador').val('');
        //$('#comboMontador').val('');
        $('#txtSemanaAlta').val('32');
        $('#txtDiaAlta').val('25');
        $('#txtValorAlta').val('100');
        //$('#comboEstatus').val('');
        $('#txtDescripcionServicio1').val('1');
        $('#txtDescripcionServicio2').val('2');
        $('#txtDescripcionServicio3').val('3');
        $('#txtFechaProMesaEntrega').val('2018-07-30');
        $('#txtHoraProMesaEntrega').val('20:00');
        $('#txtFechaPresupuesto').val('2018-07-01');
        $('#txtValorVenta').val('1200');
        $('#txtValorCobro').val('200');
        //$('#txtValor35').val('35');
    }

    $('#btnGuardar').click(function (e) {

        e.preventDefault();


        //console.log('Save...');

        var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

        if (hasErrors) {
            return;
        }


        if ($('#comboOperador').val() == null || $('#comboOperador').val() === '') {

            $('#spnMensajes').html(mensajesAlertas.errorSeleccionarOperador);
            $('#panelMensajes').modal('show');

            return;
        }




        var item = new Object();

        item.IdEquipo = idSeleccionado;
        item.Nombre = $('#txtNombre').val();
        item.Descripcion = '';
        item.NumeroEconomico = $('#txtNumeroEconomico').val();
        item.ValorComercial = 0;
        item.NumeroSerie = $('#txtNumeroSerie').val();
        item.IdUbicacion = $('#comboUbicacion').val();
        item.IdMarca = $('#comboMarca').val();
        item.IdUnidadMedidaOrometro = $('#comboUnidadMedidaOrometro').val();
        item.IdModelo = $('#comboModelo').val();
        item.IdOperador = $('#comboOperador').val();
        item.Anio = $('#txtAnio').val();
        item.Activo = ($('#chkOperativo').prop('checked')) == true ? 1 : 0;
        item.CapacidadTanque = $('#txtCapacidadTanque').val();
        item.IdUbicacion = 0;



        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.equipo = item;
        parametros.accion = accion;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/Guardar",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var valores = msg.d;

                var resultado = parseInt(valores[0]);// Este es el idGenerado
                var mensaje = valores[1];

                //console.log(mensaje);
                $('#spnMensajes').html(mensaje);
                $('#panelMensajes').modal('show');


                if (resultado > 0) {

                    if (accion === 'nuevo') {
                        //guardar documentos
                        $('.file-fotografia').each(function (documento) {

                            let file;
                            if (file = this.files[0]) {

                                utils.sendFile(file, 'fotografia', resultado, 'fotografia');

                            }

                        });
                    }


            



                    $('#panelTabla').show();
                    $('#panelForm').hide();

                    cargarItems();


                }



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);

                $("#lblMensajesFail").text('No se pudo guardar el registro...');

                $("#alert-operacion-fail").show("fast", function () {
                    setTimeout(function () {
                        $("#alert-operacion-fail").hide("fast");
                    }, 3000);
                });

            }

        });




    });


    $('.file-fotografia').on('change', function (e) {
        e.preventDefault();

        if (idSeleccionado !== "-1") {


            //debugger;
            let file;
            if (file = this.files[0]) {

                utils.sendFile(file, 'fotografia', idSeleccionado, 'fotografia');

                setTimeout(function () {

                    //  Mostrar la imagen que se acaba de subir...
                    var parametros = new Object();
                    parametros.path = window.location.hostname;
                    parametros.idEquipo = idSeleccionado;
                    parametros = JSON.stringify(parametros);
                    $.ajax({
                        type: "POST",
                        url: "../pages/PanelEquipos.aspx/GetFoto",
                        data: parametros,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: true,
                        success: function (foto) {
                            
                            let strFoto = foto.d;
                            if (strFoto != '') {
                                $('#img_').attr('src', `data:image/jpg;base64,${strFoto}`);
                            }

                        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                            console.log(textStatus + ": " + XMLHttpRequest.responseText);
                        }

                    });

                }, 5000);

            }
        }


    });

    $('#btnCancelar').click(function (e) {
        e.preventDefault();

        $('#panelTabla').show();
        $('#panelForm').hide();

    });


    $('#btnEliminarAceptar').click(function () {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.id = idSeleccionado;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/EliminarEquipo",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var resultado = msg.d;
                if (resultado.MensajeError === null) {

                    $('#spnMensajes').html('Registro eliminado correctamente...');
                    $('#panelMensajes').modal('show');

                    cargarItems();

                } else {

                    $('#spnMensajes').html(resultado.MensajeError);
                    $('#panelMensajes').modal('show');


                }


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    });

    $('#btnGuardarRefacciones').click(function (e) {

        e.preventDefault();
        console.log('Boton btnGuardarRefacciones');

        let listaRefacciones = [];

        $("#listaRefaccionesSeleccionadas option").each(function () {

            var item = {
                idCatalogoRefaccion: $(this).val(),
                IdEquipo: $('#spnIdEquipo').text()
            }

            listaRefacciones.push(item);
        });


        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.listaRefacciones = listaRefacciones;
        parametros.idEquipo = $('#spnIdEquipo').text();

        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "PanelEquipos.aspx/GuardarEquipoRefacciones",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var resultado = parseInt(msg.d);

                if (resultado > 0) {

                    utils.toast(mensajesAlertas.exitoGuardar, 'ok');



                } else {
                    utils.toast(mensajesAlertas.errorGuardar, 'error');


                }

                $('#panelRefacciones').modal('hide');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    });

    $("#listaRefacciones").on("change", (e) => {
        e.preventDefault();

        let args = e.currentTarget.selectedOptions;

        if (args[0].value) {
            let value = args[0].value;
            let text = args[0].text;

            let item = '<option value="' + value + '">' + text + "</option>";

            $("#listaRefaccionesSeleccionadas").append(item);
            $("#listaRefacciones option[value='" + value + "']").remove();
        }
    });

    $("#listaRefaccionesSeleccionadas").on("change", (e) => {
        e.preventDefault();

        let args = e.currentTarget.selectedOptions;

        if (args[0].value) {
            let value = args[0].value;
            let text = args[0].text;

            let item = '<option value="' + value + '">' + text + "</option>";

            $("#listaRefacciones").append(item);
            $("#listaRefaccionesSeleccionadas option[value='" + value + "']").remove();
        }
    });



});

const asignarRefacciones = (idEquipo, nombre) => {
    //tiposUsuario.idSeleccionado = idUsuario;
    $('.spnNombreEquipo').text(nombre);
    $('#spnIdEquipo').text(idEquipo);


    refaccionesEquipo.cargarListaRefacciones(idEquipo);
    refaccionesEquipo.cargarListaRefaccionesUsuario(idEquipo);

    $('#panelRefacciones').modal('show');

}


const refaccionesEquipo = {

    cargarListaRefacciones: (idEquipo) => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idEquipo = idEquipo;
        parametros = JSON.stringify(parametros);

        $('#listaRefacciones').empty();

        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/GetListaRefacciones",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdCatalogoRefaccion + '">' + item.NumeroParte + ' - ' + item.Descripcion + '</option>';


                }

                $('#listaRefacciones').empty().append(opcion);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },

    cargarListaRefaccionesUsuario: (idEquipo) => {

        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idEquipo = idEquipo;
        parametros = JSON.stringify(parametros);

        $('#listaRefaccionesSeleccionadas').empty();

        $.ajax({
            type: "POST",
            url: "../pages/PanelEquipos.aspx/GetListaRefaccionesPorEquipo",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {
                var items = msg.d;
                var opcion = "";

                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    opcion += '<option value="' + item.IdCatalogoRefaccion + '">' + item.NumeroParte + ' - ' + item.Descripcion + '</option>';

                }

                $('#listaRefaccionesSeleccionadas').empty().append(opcion);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    }


}


function descargarFormato(id) {
    window.open("../pages/Download.ashx?path=" + window.location.hostname + "&id_proyecto=" + id + "&tipo=1");


}


function eliminar(id) {

    idSeleccionado = id;

    $('#panelEliminar').modal('show');

}



function editar(id) {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#frm')[0].reset();

    accion = "editar";

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/PanelEquipos.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var item = msg.d;
            idSeleccionado = item.IdEquipo;

            //console.log(item.IdUnidadMedidaOrometro);

            $('#txtNombre').val(item.Nombre);
            //$('#txtDescripcion').val('');
            $('#txtNumeroEconomico').val(item.NumeroEconomico);
            $('#txtNumeroSerie').val(item.NumeroSerie);
            $('#comboUbicacion').val(item.IdUbicacion);
            $('#txtValorComercial').val(item.ValorComercial);
            $('#comboMarca').val(item.IdMarca);
            $('#chkOperativo').prop('checked', item.Activo == 1);
            $('#comboUnidadMedidaOrometro').val(item.IdUnidadMedidaOrometro);
            $('#txtCapacidadTanque').val(item.CapacidadTanque);
            
            $("#comboOperador").val(item.IdOperador);

            datosEquipo.cargarComboModelos_(item.IdMarca, item.IdModelo);
            $('#txtAnio').val(item.Anio);


            accion = "editar";
            $('#spnTituloForm').text('Editar');

            datosEquipo.odometro(id);


            $('#img_').attr('src', `../img/logo_small.jpg`);

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.idEquipo = id;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/PanelEquipos.aspx/GetFoto",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (foto) {

                    let strFoto = foto.d;
                    if (strFoto != '') {
                        $('#img_').attr('src', `data:image/jpg;base64,${strFoto}`);
                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });


            

            $('#panelTabla').hide();
            $('#panelForm').show();


        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }

    });


}



function nuevo() {


    $('#frm')[0].reset();
    $('.form-group').removeClass('has-error');
    $('.help-block').empty();
    $('#spnTituloForm').text('Nuevo');




    $('#panelTabla').hide();
    $('#panelForm').show();
    accion = "nuevo";
    id = "-1";
    idSeleccionado = "-1";
    idClienteSeleccionado = "-1";
    $("#comboOperador").jqxDropDownList('clearSelection');
    $('#img_').attr('src', `../img/logo_small.jpg`);


    //2011-10-05T14:48:00.000Z
    var fechaHoy = new Date();

    //obtenerFechaHoraServidor();



    $('#txtNumProyecto').prop('disabled', true);



}
