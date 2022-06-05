'use strict';


class PanelSolicitudRefacciones {



}

const panelGuarantee = {


    init: () => {

        $('#panelFormGarantias').hide();
        $('#panelFormGarantiasAval').hide();

        panelGuarantee.accion = '';
        panelGuarantee.idPrestamo = -1;


    },

    view(idPrestamo) {
        //console.log('Asignar id del prestamo al panel ' + idPrestamo);

        panelGuarantee.idPrestamo = idPrestamo;
        panelGuarantee.cargarItemsCustomer(idPrestamo);
        console.log('Asignar id del prestamo al paneeeel ' + idPrestamo);

    },


    cargarItemsCustomer: (idPrestamo) => {
        var parametros = new Object();
        parametros.path = window.location.hostname;
        parametros.idUsuario = sessionStorage.getItem("idusuario");
        parametros.idPrestamo = idPrestamo;
        parametros = JSON.stringify(parametros);
        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoansIndex.aspx/GetListGuaranteeCustomer",
            data: parametros,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {



                let tablaRefacciones = $('#tableGarantias').DataTable({
                    pageLength: 50,
                    "destroy": true,
                    "processing": true,
                    "order": [],
                    data: msg.d,
                    columns: [
                        { data: 'Nombre' },
                        { data: 'NumeroSerie' },
                        { data: 'Costo' },
                        { data: 'Fecha' },
                        { data: 'Accion' },

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [0, 1, 2, 3, 4],
                            "orderable": false
                        },
                    ],
                    dom: 't'

                });


                $('#tableGarantias').on('draw.dt', function () {


                });

                $('#tableGarantias').trigger('draw.dt');


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    nuevoGarantiaCliente: () => {

       

        $('#frmGarantias')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        panelGuarantee.accion = "nuevo";

        $('#panelTablaGarantias').hide();
        $('#panelFormGarantias').show();

        

    },


    nuevoGarantiaAval: () => {

        console.log('nuevo garantia aval');

        $('#frmGarantias')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();

        panelGuarantee.accion = "nuevo";

        $('#panelTablaGarantiasAval').hide();
        $('#panelFormGarantiasAval').show();



    },

    cancelarRefacciones: (id) => {

        panelGuarantee.idRefaccionSeleccionada = id;

        $('#msgCancelar').html(mensajesAlertas.confirmacionCancelarRefaccion.replace('{numrefaccion}', id));
        $('#panelCancelarRefaccion').modal('show');

    },

    accionesBotones: () => {


        $('#btnEliminarAceptarRefaccion').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = panelGuarantee.idRefaccionSeleccionada;
            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../pages/MantenimientoPanelSolicitudRefacciones.aspx/CancelarRefaccion",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    $('#panelCancelarRefaccion').modal('hide');

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoCancelarRefaccion, 'ok');


                        //panelGuarantee.cargarItems(panelGuarantee.idPrestamo)

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });

        $('#btnNuevaGarantia').on('click', (e) => {
            e.preventDefault();


            panelGuarantee.nuevoGarantiaCliente();

        });

        $('#btnNuevaGarantiaAval').on('click', (e) => {
            e.preventDefault();


            panelGuarantee.nuevoGarantiaAval();

        });


        $('#btnGuardarGarantiaCliente').on('click', (e) => {

            console.log("llego")

            e.preventDefault();

            var hasErrors = $('form[name="frmRefaccion"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }


            var item = new Object();

            var archivo = $('#imgGarantee').prop("files")[0];

            item.IdGarantia = 0;
            item.Nombre = $('#txtNombreGarantia').val();
            item.NumeroSerie = $('#txtNumeroSerie').val();
            item.Costo = $('#txtCosto').val();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.idUsuario = document.getElementById('txtIdUsuario').value;
            parametros.idPrestamo = panelGuarantee.idPrestamo;

            console.log("idPrestamo = " + panelGuarantee.idPrestamo)

            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../../pages/Loans/LoansIndex.aspx/SaveCustomerGuarantee",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;

                    if (parseInt(valores.CodigoError) == 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        $('#panelTablaGarantias').show();
                        $('#panelFormGarantias').hide();

                        

                        //guardar foto
                        $('.file-fotografia').each(function (documento) {

                            let file;
                            if (file = this.files[0]) {

                                utils.sendFileEmployee(file, 'garantia', valores.IdItem, 0, "garantia");

                            }

                        });
                        


                        //panelGuarantee.cargarItems(panelGuarantee.idPrestamo)


                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);

                    utils.toast(mensajesAlertas.errorGuardar, 'error');
                }

            });

        });


        $('#btnCancelarGarantíaCliente').on('click', (e) => {
            e.preventDefault();
            $('#panelTablaGarantias').show();
            $('#panelFormGarantias').hide();
        });

        $('#btnCancelarGarantíaAval').on('click', (e) => {
            e.preventDefault();
            $('#panelTablaGarantiasAval').show();
            $('#panelFormGarantiasAval').hide();
        });


    }


}

window.addEventListener('load', () => {

    panelGuarantee.init();

    panelGuarantee.accionesBotones();

});


