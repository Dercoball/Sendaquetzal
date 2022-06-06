'use strict';
const loansEdit = {


    init: () => {

        loansEdit.idSeleccionado = "-1";
        loansEdit.idTipoUsuario = "-1";
        loansEdit.accion = "";



    },

    view(idPrestamo) {
        console.log('Abir datos del préstamo' + idPrestamo);

        loansEdit.edit(idPrestamo);

        panelGuarantee.view(idPrestamo);

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

            console.log('Your current position is:');
            console.log('Latitude : ' + crd.latitude);
            console.log('Longitude: ' + crd.longitude);
            console.log('More or less ' + crd.accuracy + ' meters.');



            $(`${control}`).val(`${crd.latitude}, ${crd.longitude}`);


        };

        function error(err) {
            console.warn('ERROR(' + err.code + '): ' + err.message);
        };


        navigator.geolocation.getCurrentPosition(success, error, options);


    },

    edit: (id) => {

        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        //$('#frm')[0].reset();


        let params = {};
        params.path = window.location.hostname;
        params.id = id;
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

                //  dirección aval
                $('#txtCalleAval').val(item.direccionAval.Calle);
                $('#txtColoniaAval').val(item.direccionAval.Colonia);
                $('#txtMunicipioAval').val(item.direccionAval.Municipio);
                $('#txtEstadoAval').val(item.direccionAval.Estado);
                $('#txtCodigoPostalAval').val(item.direccionAval.CodigoPostal);
                $('#txtDireccionTrabajoAval').val(item.direccionAval.DireccionTrabajo);

                loansEdit.getLocation('#txtUbicacion');



                $('#panelTabla').hide();
                $('#panelFiltro').hide();
                $('#btnNuevo').hide();

                $('#panelForm').show();
                loansEdit.accion = "editar";
                //$('#spnTituloForm').text('Editar');




                console.log(loansEdit.idSeleccionado);
                loansEdit.getDocument(loansEdit.idSeleccionado, 1, '#img_1');
                loansEdit.getDocument(loansEdit.idSeleccionado, 2, '#img_2');
                loansEdit.getDocument(loansEdit.idSeleccionado, 3, '#img_3');
                loansEdit.getDocument(loansEdit.idSeleccionado, 4, '#img_4');
                loansEdit.getDocument(loansEdit.idSeleccionado, 6, '#img_6');
                loansEdit.getDocument(loansEdit.idSeleccionado, 7, '#img_7');
                loansEdit.getDocument(loansEdit.idSeleccionado, 8, '#img_8');
                loansEdit.getDocument(loansEdit.idSeleccionado, 9, '#img_9');



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


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

            //let hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            //if (hasErrors) {

            //    $('#spnMensajes').html(mensajesAlertas.solicitudNoProcedenteCamposVacios);
            //    $('#panelMensajes').modal('show');

            //    return;
            //}

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
            params.accion = loansEdit.accion;
            params = JSON.stringify(params);


            $.ajax({
                type: "POST",
                url: `../../pages/Loans/LoanRequest.aspx/UpdateCustomer`,
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
            //e.preventDefault();

            //let hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            //if (hasErrors) {

            //    $('#spnMensajes').html(mensajesAlertas.solicitudNoProcedenteCamposVacios);
            //    $('#panelMensajes').modal('show');

            //    return;
            //}

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
            params.accion = loansEdit.accion;
            params = JSON.stringify(params);

           

            $.ajax({
                type: "POST",
                url: `../../pages/Loans/LoanRequest.aspx/UpdateCustomerAval`,
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


