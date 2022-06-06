'use strict';


class PanelSolicitudRefacciones {



}

const panelGuarantee = {


    init: () => {

        $('#panelFormGarantias').hide();
        $('#panelFormGarantiasAval').hide();

        panelGuarantee.accion = '';
        panelGuarantee.idPrestamo = -1;
        panelGuarantee.idSeleccionado = -1;


    },

    view(idPrestamo) {
        
        panelGuarantee.idPrestamo = idPrestamo;
        panelGuarantee.cargarItemsCustomer(idPrestamo);
        panelGuarantee.cargarItemsAval(idPrestamo);

        console.log('Asignar id del prestamo al paneeeel ' + idPrestamo);

    },


    cargarItemsCustomer: (idPrestamo) => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPrestamo = idPrestamo;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoansIndex.aspx/GetListGuaranteeCustomer",
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

                let table = $('#tableGarantias').DataTable({
                    "destroy": true,
                    "processing": false,
                    "order": [],
                    data: data,
                    columns: [

                        { data: 'Nombre' },
                        { data: 'NumeroSerie' },
                        { data: 'Costo' },
                        { data: 'Fecha' },
                        { data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: 'frBtipl',
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


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });



    },


    cargarItemsAval: (idPrestamo) => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idPrestamo = idPrestamo;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoansIndex.aspx/GetListGuaranteeAval",
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

                let table = $('#tableGarantiasAval').DataTable({
                    "destroy": true,
                    "processing": false,
                    "order": [],
                    data: data,
                    columns: [

                        { data: 'Nombre' },
                        { data: 'NumeroSerie' },
                        { data: 'Costo' },
                        { data: 'Fecha' },
                        { data: 'Accion' }

                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        },
                    ],
                    dom: 'frBtipl',
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


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });



    },

    delete: (id) => {

        panelGuarantee.idSeleccionado = id;

        $('#panelEliminar').modal('show');

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


        $('#btnEliminarAceptar').on('click', (e) => {

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.id = panelGuarantee.idSeleccionado;
            parametros.idUsuario = document.getElementById('txtIdUsuario').value;

            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../../pages/Loans/LoansIndex.aspx/Delete",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                   
                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        panelGuarantee.cargarItemsCustomer(panelGuarantee.idPrestamo);
                        panelGuarantee.cargarItemsAval(panelGuarantee.idPrestamo);

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

            

            e.preventDefault();

            var hasErrors = $('form[name="frmGarantias"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }


            var item = new Object();

            

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

                      

                        

                        

                        //guardar foto
                        $('.file-garantiacliente').each(function (documento) {

                            let file;
                            if (file = this.files[0]) {

                                
                                utils.sendFileEmployee(file, 'garantia', valores.IdItem, 0, "garantia");
                                
                            }

                        });
                        


                        panelGuarantee.cargarItemsCustomer(panelGuarantee.idPrestamo);

                        $('#panelFormGarantias').hide();
                        $('#panelTablaGarantias').show();
                        

                    } else {

                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                    }

                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);

                    utils.toast(mensajesAlertas.errorGuardar, 'error');
                }

            });

        });

        $('#btnGuardarGarantiaAval').on('click', (e) => {



            e.preventDefault();

            var hasErrors = $('form[name="frmGarantiasAval"]').validator('validate').has('.has-error').length;


            if (hasErrors) {
                return;
            }


            var item = new Object();

            

            item.IdGarantia = 0;
            item.Nombre = $('#txtNombreGarantiaAval').val();
            item.NumeroSerie = $('#txtNumeroSerieAval').val();
            item.Costo = $('#txtCostoAval').val();

            var parametros = new Object();
            parametros.path = window.location.hostname;
            parametros.item = item;
            parametros.idUsuario = document.getElementById('txtIdUsuario').value;
            parametros.idPrestamo = panelGuarantee.idPrestamo;



            parametros = JSON.stringify(parametros);
            $.ajax({
                type: "POST",
                url: "../../pages/Loans/LoansIndex.aspx/SaveAvalGuarantee",
                data: parametros,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    var valores = msg.d;

                    if (parseInt(valores.CodigoError) == 0) {

                        utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                        

                        //console.log("se guardo con el id" + valores.IdItem);



                        //guardar foto
                        $('.file-garantiaaval').each(function (documento) {

                            let file;
                            if (file = this.files[0]) {

                                console.log("guardar la foto aval" + valores.IdItem);

                                utils.sendFileEmployee(file, 'garantia', valores.IdItem, 0, "garantia");

                            }

                        });



                        panelGuarantee.cargarItemsAval(panelGuarantee.idPrestamo);

                        $('#panelFormGarantiasAval').hide();
                        $('#panelTablaGarantiasAval').show();



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


