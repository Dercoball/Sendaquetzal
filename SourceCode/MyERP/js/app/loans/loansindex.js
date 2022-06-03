'use strict';
let date = new Date();
let descargas = "Prestamos_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '13';


const loansindex = {


    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        loansindex.idSeleccionado = "-1";
        loansindex.idTipoUsuario = "-1";
        loansindex.login = "-1";
        loansindex.accion = "";
        //loansindex.lat = '';
        //loansindex.lng = '';


        loansindex.fechasHoy();
        loansindex.loadComboStatusPrestamo();

        loansindex.cargarItems();

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();



    },

    cargarItems: () => {

        let status = $('#comboStatus').val();
        let fechaInicial = $('#txtFechaInicial').val();
        let fechaFinal = $('#txtFechaFinal').val();

        if (!fechaFinal) {

            fechaFinal = loansindex.fecha();
            $('#txtFechaFinal').val(fechaFinal);
        }

        if (!fechaInicial) {

            fechaInicial = loansindex.fecha();
            $('#txtFechaInicial').val(fechaInicial);
        }

        status = status == null ? "-1" : status;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idTipoUsuario = document.getElementById('txtIdTipoUsuario').value;
        params.fechaInicial = fechaInicial;
        params.fechaFinal = fechaFinal;
        params.idStatus = status;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoansIndex.aspx/GetListaItems",
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
                    "order": [],
                    data: data,
                    columns: [
                        { data: 'IdPrestamo' },
                        { data: 'Cliente.NombreCompleto' },
                        { data: 'Cliente.Curp' },
                        { data: 'MontoFormateadoMx' },
                        { data: 'FechaSolicitud' },
                        { data: 'NombreStatus' },
                        { data: 'Accion' }


                    ],
                    "language": textosEsp,
                    "columnDefs": [
                        {
                            "targets": [-1],
                            "orderable": false
                        }
                    ],
                    dom: 'fBrtipl',
                    buttons: [
                        {
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Csv', className: 'csvbtn'
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


    },


    delete: (id) => {

        loansindex.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },

    view(idPrestamo) {
        console.log('Abir datos del préstamo' + idPrestamo);

        loansindex.edit(idPrestamo);

    },


    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');
        $('#txtDescripcion').val('');

        $('#panelTabla').hide();
        $('#panelForm').show();
        loansindex.accion = "nuevo";
        loansindex.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();
        $('#txtPassword').prop('disabled', false);




    },

    testData() {
        $('.campo-combo').val(2);
        $('.campo-date').val('2022-01-01');
        $('.campo-input').val(7);

        //$('#txtFechaNacimiento').val('2000-01-01');

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
        $('#frm')[0].reset();


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



                loansindex.idSeleccionado = item.IdCliente;
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

                loansindex.getLocation('#txtUbicacion');



                $('#panelTabla').hide();
                $('#panelFiltro').hide();
                $('#btnNuevo').hide();

                $('#panelForm').show();
                loansindex.accion = "editar";
                //$('#spnTituloForm').text('Editar');




                console.log(loansindex.idSeleccionado);
                loansindex.getDocument(loansindex.idSeleccionado, 1, '#img_1');
                loansindex.getDocument(loansindex.idSeleccionado, 2, '#img_2');
                loansindex.getDocument(loansindex.idSeleccionado, 3, '#img_3');
                loansindex.getDocument(loansindex.idSeleccionado, 4, '#img_4');
                loansindex.getDocument(loansindex.idSeleccionado, 6, '#img_6');
                loansindex.getDocument(loansindex.idSeleccionado, 7, '#img_7');
                loansindex.getDocument(loansindex.idSeleccionado, 8, '#img_8');
                loansindex.getDocument(loansindex.idSeleccionado, 9, '#img_9');



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




    loadComboStatusPrestamo: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoansIndex.aspx/GetListaStatusPrestamo",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="-1">Todos</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdStatusPrestamo}' > ${item.Nombre}</option > `;

                }

                $('#comboStatus').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },



    fecha() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        return `${today.getFullYear()}-${month}-${dayMonth}`;


    },

    fechasHoy() {

        //  fecha hoy (final)
        let today = new Date();
        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        $('#txtFechaFinal').val(`${today.getFullYear()}-${month}-${dayMonth}`);


        //  fecha inicial
        let startWeekDay = new Date();
        startWeekDay.setDate(startWeekDay.getDate() - startWeekDay.getDay() + 1);

        let startDayMonth = startWeekDay.getDate();
        startDayMonth = startDayMonth.toString().length === 1 ? `0${startDayMonth}` : startDayMonth;

        let startMonth = (startWeekDay.getMonth() + 1);
        startMonth = startMonth.toString().length === 1 ? `0${startMonth}` : startMonth;
        let startYear = (startWeekDay.getFullYear());

        $('#txtFechaInicial').val(`${startYear}-${startMonth}-${startDayMonth}`);


    },

    loadComboTipoCliente: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Clients.aspx/GetListaItemsTipoCliente",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdTipoCliente}' > ${item.NombreTipoCliente}</option > `;

                }

                $('#comboTipoCliente').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },





    accionesBotones: () => {

        $('#btnFiltrar').on('click', (e) => {
            e.preventDefault();

            loansindex.cargarItems();

        });


        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $(`.documentos`).attr('src', '../../img/upload.png');

            window.location = "LoansIndex.aspx";


        });

        $('#btnCancelarAval').on('click', (e) => {
            e.preventDefault();

            $(`.documentos`).attr('src', '../../img/upload.png');

            window.location = "LoansIndex.aspx";


        });

        $('#btnCancelarAprobacion').on('click', (e) => {
            e.preventDefault();

            $(`.documentos`).attr('src', '../../img/upload.png');

            window.location = "LoansIndex.aspx";


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
            dataClient.IdCliente = loansindex.idSeleccionado;


            dataClient.Curp = $('#txtCURP').val();
            dataClient.PrimerApellido = $('#txtPrimerApellido').val();
            dataClient.SegundoApellido = $('#txtSegundoApellido').val();
            dataClient.Nombre = $('#txtNombre').val();
            dataClient.Telefono = $('#txtTelefono').val();
            dataClient.Ocupacion = $('#txtOcupacion').val();
            dataClient.CurpAval = $('#txtCURPAval').val();

            let dataAddressClient = {};
            dataAddressClient.IdCliente = loansindex.idSeleccionado;
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
            params.accion = loansindex.accion;
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
                    loansindex.edit(loansindex.idSeleccionado);



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
            dataClient.IdCliente = loansindex.idSeleccionado;



            dataClient.OcupacionAval = $('#txtOcupacionAval').val();
            dataClient.CurpAval = $('#txtCURPAval').val();
            dataClient.PrimerApellidoAval = $('#txtPrimerApellidoAval').val();
            dataClient.SegundoApellidoAval = $('#txtSegundoApellidoAval').val();
            dataClient.NombreAval = $('#txtNombreAval').val();
            dataClient.TelefonoAval = $('#txtTelefonoAval').val();


            let dataAddressClientAval = {};
            dataAddressClientAval.IdCliente = loansindex.idSeleccionado;
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
            params.accion = loansindex.accion;
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
                    loansindex.edit(loansindex.idSeleccionado);




                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });







        $('#btnReloadLocation').on('click', (e) => {
            e.preventDefault();

            loansindex.getLocation('#txtUbicacion');

        });


    }


}

window.addEventListener('load', () => {

    loansindex.init();

    loansindex.accionesBotones();

});


