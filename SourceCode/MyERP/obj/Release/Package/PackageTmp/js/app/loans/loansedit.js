'use strict';
let date = new Date();
let descargas = "Prestamo_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const loansEdit = {


    init: () => {

        loansEdit.idSeleccionado = "-1";
        loansEdit.idTipoUsuario = "-1";
        loansEdit.accion = "";
        loansEdit.idPrestamo = "-1";

        //  Cargar el id del cliente via GET
        let urlParams = utils.parseURLParams(window.location.href);
        //console.log(`urlParams = ${urlParams['id']}`);
        if (urlParams && urlParams['id'] !== undefined) {

            let id = urlParams['id'][0];
            //console.log(`id = ${id}`);
            loansEdit.edit(id);
        }


    },



    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');
        $('#txtDescripcion').val('');

        $('#panelTabla').hide();
        $('#panelForm').show();
        loansEdit.accion = "nuevo";
        loansEdit.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();
        $('#txtPassword').prop('disabled', false);




    },



    getLocation(control) {

        var options = {
            enableHighAccuracy: true,
            timeout: 5000,
            maximumAge: 0
        };


        function success(pos) {
            var crd = pos.coords;

            //console.log('Your current position is:');
            //console.log('Latitude : ' + crd.latitude);
            //console.log('Longitude: ' + crd.longitude);
            //console.log('More or less ' + crd.accuracy + ' meters.');



            $(`${control}`).val(`${crd.latitude}, ${crd.longitude}`);


        };

        function error(err) {
            console.warn('ERROR(' + err.code + '): ' + err.message);
        };


        navigator.geolocation.getCurrentPosition(success, error, options);


    },

    edit: (idCliente) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#frmCustomer')[0].reset();
        $('#frmAval')[0].reset();


        let params = {};
        params.path = window.location.hostname;
        params.idCliente = idCliente;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoanRequest.aspx/GetDataPrestamo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;



                loansEdit.idSeleccionado = item.IdCliente;
                //  cliente
                $('#txtNombre').val(item.Nombre);
                $('#txtPrimerApellido').val(item.PrimerApellido);
                $('#txtSegundoApellido').val(item.SegundoApellido);
                $('#txtCURP').val(item.Curp);
                $('#txtTelefono').val(item.Telefono);

                $('#txtOcupacion').val(item.Ocupacion);
                $('#txtNotaDeFoto').val(item.NotaFotografiaCliente);

                //  dirección cliente
                $('#txtCalle').val(item.direccion.Calle);
                $('#txtColonia').val(item.direccion.Colonia);
                $('#txtMunicipio').val(item.direccion.Municipio);
                $('#txtEstado').val(item.direccion.Estado);
                $('#txtCodigoPostal').val(item.direccion.CodigoPostal);
                $('#txtDireccionTrabajo').val(item.direccion.DireccionTrabajo);


                //datos aval
                $('#txtNombreAval').val(item.NombreAval);
                $('#txtPrimerApellidoAval').val(item.PrimerApellidoAval);
                $('#txtSegundoApellidoAval').val(item.SegundoApellidoAval);
                $('#txtCURPAval').val(item.CurpAval);
                $('#txtTelefonoAval').val(item.TelefonoAval);
                $('#txtOcupacionAval').val(item.OcupacionAval);
                $('#txtNotaDeFotoAval').val(item.NotaFotografiaAval);

                //  dirección aval
                $('#txtCalleAval').val(item.direccionAval.Calle);
                $('#txtColoniaAval').val(item.direccionAval.Colonia);
                $('#txtMunicipioAval').val(item.direccionAval.Municipio);
                $('#txtEstadoAval').val(item.direccionAval.Estado);
                $('#txtCodigoPostalAval').val(item.direccionAval.CodigoPostal);
                $('#txtDireccionTrabajoAval').val(item.direccionAval.DireccionTrabajo);

                loansEdit.getLocation('#txtUbicacion');
                loansEdit.getLocation('#txtUbicacionAval');

                let relPrestamoAprobacion1 = item.listaRelPrestamoAprobacion[0];
                let relPrestamoAprobacionAval2 = item.listaRelPrestamoAprobacion[1];

                console.log(`NotaFotografiaCliente ${JSON.stringify(item.NotaFotografiaCliente)}`);
                console.log(`NotaFotografiaAval ${JSON.stringify(item.NotaFotografiaAval)}`);


                console.log(`NotaCliente ${JSON.stringify(relPrestamoAprobacion1.NotaCliente)}`);
                console.log(`NotaAval ${JSON.stringify(relPrestamoAprobacion1.NotaAval)}`);

                $('#txtNotaSupervisor').val(relPrestamoAprobacion1.NotaCliente);
                $('#txtNotaSupervisorAval').val(relPrestamoAprobacion1.NotaAval);



                $('#panelTabla').hide();
                $('#panelFiltro').hide();
                $('#btnNuevo').hide();

                $('#panelForm').show();
                loansEdit.accion = "editar";

                loansEdit.idPrestamo = item.IdPrestamo;


                //console.log(loansEdit.idSeleccionado);
                loansEdit.getDocument(loansEdit.idSeleccionado, 1, '#img_1');
                loansEdit.getDocument(loansEdit.idSeleccionado, 2, '#img_2');
                loansEdit.getDocument(loansEdit.idSeleccionado, 3, '#img_3');
                loansEdit.getDocument(loansEdit.idSeleccionado, 4, '#img_4');
                loansEdit.getDocument(loansEdit.idSeleccionado, 6, '#img_6');
                loansEdit.getDocument(loansEdit.idSeleccionado, 7, '#img_7');
                loansEdit.getDocument(loansEdit.idSeleccionado, 8, '#img_8');
                loansEdit.getDocument(loansEdit.idSeleccionado, 9, '#img_9');


                //  Deshabilitar y ocultar campos de acuerdo al tipo de usuario

                let userTypeId = document.getElementById('txtIdTipoUsuario').value;
                let disabled = (Number(userTypeId) === utils.POSICION_PROMOTOR);   //  true/ false
                loansEdit.disableFields(disabled);

                panelGuarantee.view(item.IdPrestamo, disabled);


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },

    disableFields(disabled) {

        $('.campo-combo').prop('disabled', disabled);
        $('.campo-date').prop('disabled', disabled);
        $('.campo-input').prop('disabled', disabled);
        $('.campo-imagen').prop('disabled', disabled);
        $('.campo-textarea').prop('disabled', disabled);
        if (disabled) {
            $('.campo-imagen').hide();
            $('.boton-ocultable').hide();
        } else {
            $('.campo-imagen').show();
            $('.boton-ocultable').show()

        }
    },


    getDocument(idCliente, idTipoDocumento, idControl) {

        let params = {};
        params.path = window.location.hostname;
        params.idCliente = idCliente;
        params.idTipoDocumento = idTipoDocumento;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoanRequest.aspx/GetDocument",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (foto) {

                let doc = foto.d;

                if (doc.IdDocumento) {
                    if (doc.Extension === 'png' || doc.Extension === 'PNG' || doc.Extension === 'jpg' || doc.Extension === 'jpeg' || doc.Extension === 'bmp') {
                        $(`${idControl}`).attr('src', `data:image/jpg;base64,${doc.Contenido}`);
                    } else if (doc.Extension === 'pdf') {
                        $(`${idControl}`).attr('src', '../../img/ico_pdf.png');
                    } else {
                        $(`${idControl}`).attr('src', '../../img/ico_doc.png');
                    }

                    $(`#href_${idTipoDocumento}`).css('cursor', 'pointer');
                } else {
                    $(`#href_${idTipoDocumento}`).css('cursor', 'default');
                }

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });

    },




    accionesBotones: () => {



        $('.cancelar').on('click', (e) => {
            e.preventDefault();

            $(`.documentos`).attr('src', '../../img/upload.png');

            window.location = "loansIndex.aspx";


        });


        $('#btnGuardarCliente').on('click', (e) => {
            e.preventDefault();

            let hasErrors = $('form[name="frmCustomer"]').validator('validate').has('.has-error').length;

            if (hasErrors) {

                $('#spnMensajes').html(mensajesAlertas.solicitudCamposVacios);
                $('#panelMensajes').modal('show');

                return;
            }

            //  Deshabilitamos boton de guardar, para que no vaya a enviar otro request de guardar mietnras se esta guardando
            $('.deshabilitable').prop('disabled', true);
            $('#btnGuardar').html(`<i class="fa fa-paper-plane mr-1"></i>Guardando...`);


            let dataClient = {};
            dataClient.IdCliente = loansEdit.idSeleccionado;


            dataClient.Curp = $('#txtCURP').val();
            dataClient.PrimerApellido = $('#txtPrimerApellido').val();
            dataClient.SegundoApellido = $('#txtSegundoApellido').val();
            dataClient.Nombre = $('#txtNombre').val();
            dataClient.Telefono = $('#txtTelefono').val();
            dataClient.Ocupacion = $('#txtOcupacion').val();
            dataClient.CurpAval = $('#txtCURPAval').val();
            dataClient.NotaCliente = $('#txtNotaSupervisor').val();
            dataClient.NotaFotografiaCliente = $('#txtNotaDeFoto').val();

            let dataAddressClient = {};
            dataAddressClient.IdCliente = loansEdit.idSeleccionado;
            dataAddressClient.Calle = $('#txtCalle').val();
            dataAddressClient.Aval = 0;
            dataAddressClient.Colonia = $('#txtColonia').val();
            dataAddressClient.Municipio = $('#txtMunicipio').val();
            dataAddressClient.Estado = $('#txtEstado').val();
            dataAddressClient.CodigoPostal = $('#txtCodigoPostal').val();
            dataAddressClient.DireccionTrabajo = $('#txtDireccionTrabajo').val();






            let params = {};
            params.path = window.location.hostname;
            params.item = dataClient;
            params.itemAddress = dataAddressClient;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
            params.idPrestamo = loansEdit.idPrestamo;
            params.accion = loansEdit.accion;
            params = JSON.stringify(params);


            $.ajax({
                type: "POST",
                url: `../../pages/Loans/LoanApprove.aspx/UpdateCustomer`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;

                    //  si no tiene permisos
                    if (valores == null) {
                        window.location = "../../pages/Index.aspx";
                    }

                    if (parseInt(valores.CodigoError) === 0) {


                        //guardar documentos
                        $('.campo-imagen').each(function (i, item) {

                            let idTipoDocumento = item.dataset['tipo'];

                            //console.log(`idTipoDocumento  = ${idTipoDocumento}`);

                            let file;
                            if (file = this.files[0]) {

                                console.log("id del cliente: " + valores.IdItem);



                                utils.sendFileEmployee(file, 'update_document_customer', valores.IdItem, idTipoDocumento, "cliente");




                            }

                        });

                        let time_ = 10000;
                        setTimeout(function () {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                            $('#spnMensajeControlado').html(mensajesAlertas.solicitudPrestamoEnviada);
                            $('#panelMensajeControlado').modal('show');

                            $('.deshabilitable').prop('disabled', false);
                            $('#btnGuardarCliente').html(`<i class="fa fa-save mr-1"></i>Guardar`);

                        }, time_);




                    } else {

                        $('.deshabilitable').prop('disabled', false);
                        $('#btnGuardar').html(`<i class="fa fa-save mr-1"></i>Guardar`);

                        $('#spnMensajes').html(valores.MensajeError);
                        $('#panelMensajes').modal('show');


                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                        return;

                    }


                    // VUELVE A CARGAR LOS DATOS
                    loansEdit.edit(loansEdit.idSeleccionado);



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });

        $('#btnGuardarAval').on('click', (e) => {
            e.preventDefault();

            let hasErrors = $('form[name="frmAval"]').validator('validate').has('.has-error').length;

            if (hasErrors) {

                $('#spnMensajes').html(mensajesAlertas.solicitudCamposVacios);
                $('#panelMensajes').modal('show');

                return;
            }

            //  Deshabilitamos boton de guardar, para que no vaya a enviar otro request de guardar mietnras se esta guardando
            $('.deshabilitable').prop('disabled', true);
            $('#btnGuardar').html(`<i class="fa fa-paper-plane mr-1"></i>Guardando...`);


            let dataClient = {};
            dataClient.IdCliente = loansEdit.idSeleccionado;



            dataClient.OcupacionAval = $('#txtOcupacionAval').val();
            dataClient.CurpAval = $('#txtCURPAval').val();
            dataClient.PrimerApellidoAval = $('#txtPrimerApellidoAval').val();
            dataClient.SegundoApellidoAval = $('#txtSegundoApellidoAval').val();
            dataClient.NombreAval = $('#txtNombreAval').val();
            dataClient.TelefonoAval = $('#txtTelefonoAval').val();
            dataClient.NotaAval = $('#txtNotaSupervisorAval').val();
            dataClient.NotaFotografiaAval = $('#txtNotaDeFotoAval').val();


            let dataAddressClientAval = {};
            dataAddressClientAval.IdCliente = loansEdit.idSeleccionado;
            dataAddressClientAval.Calle = $('#txtCalleAval').val();
            dataAddressClientAval.Aval = 1;
            dataAddressClientAval.Colonia = $('#txtColoniaAval').val();
            dataAddressClientAval.Municipio = $('#txtMunicipioAval').val();
            dataAddressClientAval.Estado = $('#txtEstadoAval').val();
            dataAddressClientAval.CodigoPostal = $('#txtCodigoPostalAval').val();
            dataAddressClientAval.DireccionTrabajo = $('#txtDireccionTrabajoAval').val();




            let params = {};
            params.path = window.location.hostname;
            params.item = dataClient;
            params.itemAddressAval = dataAddressClientAval;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
            params.idPrestamo = loansEdit.idPrestamo;
            params.accion = loansEdit.accion;
            params = JSON.stringify(params);



            $.ajax({
                type: "POST",
                url: `../../pages/Loans/LoanApprove.aspx/UpdateCustomerAval`,
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {
                    let valores = msg.d;

                    //  si no tiene permisos
                    if (valores == null) {
                        window.location = "../../pages/Index.aspx";
                    }

                    if (parseInt(valores.CodigoError) === 0) {


                        //guardar documentos
                        $('.campo-imagen').each(function (i, item) {

                            let idTipoDocumento = item.dataset['tipo'];

                            //console.log(`idTipoDocumento  = ${idTipoDocumento}`);

                            let file;
                            if (file = this.files[0]) {

                                utils.sendFileEmployee(file, 'update_document_customer', valores.IdItem, idTipoDocumento, "cliente");

                            }

                        });

                        let time_ = 10000;
                        setTimeout(function () {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                            $('#spnMensajeControlado').html(mensajesAlertas.solicitudPrestamoEnviada);
                            $('#panelMensajeControlado').modal('show');

                            $('.deshabilitable').prop('disabled', false);
                            $('#btnGuardar').html(`<i class="fa fa-save mr-1"></i>Guardar`);

                        }, time_);




                    } else {

                        $('.deshabilitable').prop('disabled', false);
                        $('#btnGuardar').html(`<i class="fa fa-save mr-1"></i>Guardar`);

                        $('#spnMensajes').html(valores.MensajeError);
                        $('#panelMensajes').modal('show');


                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                        return;

                    }


                    // VUELVE A CARGAR LOS DATOS
                    loansEdit.edit(loansEdit.idSeleccionado);




                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });



        $('#btnReloadLocation').on('click', (e) => {
            e.preventDefault();

            loansEdit.getLocation('#txtUbicacion');

        });


    }


}

window.addEventListener('load', () => {

    loansEdit.init();

    loansEdit.accionesBotones();

});


