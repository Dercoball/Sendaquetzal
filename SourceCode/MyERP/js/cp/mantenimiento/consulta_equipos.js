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
var pagina = '66';

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
            url: "../pages/MantenimientoConsultaEquipos.aspx/GetListaItems",
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
            url: "../pages/MantenimientoConsultaEquipos.aspx/EliminarEquipo",
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



});


function descargarFormato(id) {
    window.open("../pages/Download.ashx?path=" + window.location.hostname + "&id_proyecto=" + id + "&tipo=1");


}


function eliminar(id) {

    idSeleccionado = id;

    $('#panelEliminar').modal('show');

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
                height: '20px', placeHolder: "", filterable: true, searchMode: 'containsignorecase',
                filterPlaceHolder: 'Buscar'
            });
            $("#comboOperador").jqxDropDownList('clearSelection');
            $("#comboOperador").jqxDropDownList({ disabled: true });



        }, error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(textStatus + ": " + XMLHttpRequest.responseText);
        }
    });

}


function abrir(id) {

    $('.form-group').removeClass('has-error');
    $('.help-block').empty();

    accion = "nuevo";

    var parametros = new Object();
    parametros.path = window.location.hostname;
    parametros.id = id;
    parametros = JSON.stringify(parametros);
    $.ajax({
        type: "POST",
        url: "../pages/MantenimientoConsultaEquipos.aspx/GetItem",
        data: parametros,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: true,
        success: function (msg) {

            var item = msg.d;
            idSeleccionado = item.IdEquipo;


            $('#txtNombre').val(item.Nombre);
            $('#txtNumeroEconomico').val(item.NumeroEconomico);
            $('#txtNumeroSerie').val(item.NumeroSerie);
            $('#comboUbicacion').val(item.IdUbicacion);
            $('#txtValorComercial').val(item.ValorComercial);
            $('#comboMarca').val(item.IdMarca);
            $('#chkOperativo').prop('checked', item.Activo == 1);

            $("#comboOperador").val(item.IdOperador);
            $('#comboUnidadMedidaOrometro').val(item.IdUnidadMedidaOrometro);


            datosEquipo.cargarComboModelos(item.IdMarca, item.IdModelo);
            $('#txtAnio').val(item.Anio);


            accion = "editar";
            $('#spnTituloForm').text('Datos del equipo');


            datosEquipo.odometro(id);

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
                    console.log(`Foto...`);
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


    //2011-10-05T14:48:00.000Z
    var fechaHoy = new Date();

    //obtenerFechaHoraServidor();



    $('#txtNumProyecto').prop('disabled', true);



}
