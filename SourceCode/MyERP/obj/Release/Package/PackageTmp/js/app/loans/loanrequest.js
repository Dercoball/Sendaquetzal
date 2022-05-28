'use strict';
let date = new Date();
let descargas = "Prestamos" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '12';


const POSICION_DIRECTOR = 1;
const POSICION_COORDINADOR = 2;
const POSICION_EJECUTIVO = 3;
const POSICION_SUPERVISOR = 4;
const POSICION_PROMOTOR = 5;

const client = {


    init: () => {

        $('#panelTabla').hide();
        $('#panelForm').show();

        client.idSeleccionado = "-1";
        client.idTipoUsuario = "-1";

        client.accion = "nuevo";


        client.loadComboTipoCliente();

        //client.cargarItems();
        client.fechaHoy();
        client.testData();

        $('.combo-supervisor').hide();
        $('.combo-ejecutivo').hide();



    },

    cargarItems: () => {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoanRequest.aspx/GetListaItems",
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
                        { data: 'IdCliente' },
                        { data: 'NombreCompleto' },
                        { data: 'TipoCliente' },
                        { data: 'Telefono_1' },
                        { data: 'Curp' },
                        { data: 'Ocupacion' },
                        { data: 'Monto' },
                        { data: 'FechaSolicitud' },
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


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },


    delete: (id) => {

        client.idSeleccionado = id;

        $('#panelEliminar').modal('show');

    },


    fechaHoy() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        $('#txtFechaSolicitud').val(`${today.getFullYear()}-${month}-${dayMonth}`);

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
            url: "../../pages/Loans/LoanRequest.aspx/GetDataClient",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                var item = msg.d;


                client.idSeleccionado = item.IdCliente;

                //  cliente
                $('#txtNombre').val(item.Nombre);
                $('#txtPrimerApellido').val(item.PrimerApellido);
                $('#txtSegundoApellido').val(item.SegundoApellido);
                $('#txtCURP').val(item.Curp);
                $('#txtFechaSolicitud').val(item.FechaSolicitud);
                $('#txtTelefono').val(item.Telefono_1);
                $('#txtCantidadPrestamo').val(item.Monto);

                $('#txtOcupacion').val(item.Ocupacion);
                $('#comboTipoCliente').val(item.IdTipoCliente);

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



                $('#panelTabla').hide();
                $('#panelForm').show();


                client.accion = "editar";
                $('#spnTituloForm').text('Editar');




                console.log(client.idSeleccionado);
                client.getDocument(client.idSeleccionado, 2, '#img_1');
                client.getDocument(client.idSeleccionado, 3, '#img_2');
                client.getDocument(client.idSeleccionado, 4, '#img_3');
                client.getDocument(client.idSeleccionado, 6, '#img_6');
                client.getDocument(client.idSeleccionado, 7, '#img_7');
                client.getDocument(client.idSeleccionado, 8, '#img_8');



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
                    if (doc.Extension === 'png' || doc.Extension === 'jpg' || doc.Extension === 'jpeg' || doc.Extension === 'bmp') {
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


    nuevo: () => {


        $('#frm')[0].reset();
        $('.form-group').removeClass('has-error');
        $('.help-block').empty();
        $('#spnTituloForm').text('Nuevo');
        $('#txtDescripcion').val('');

        $('#panelTabla').hide();
        $('#panelForm').show();
        client.accion = "nuevo";
        client.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },

    testData() {
        $('.campo-combo').val(2);
        //$('.campo-date').val(client.fechaHoy());
        $('.campo-input').val(7);
        $('.campo-input').val(200 + parseInt(Math.random() * 1000 + 1));
        $('#txtCantidadPrestamo').val(1000 + parseInt(Math.random() * 1000 + 1));

        //$('#txtFechaNacimiento').val('2000-01-01');

    },





    loadComboTipoCliente: () => {

        var params = {};
        params.path = window.location.hostname;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Loans/LoanRequest.aspx/GetListaItemsTipoCliente",
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

        $('.img-document').on('click', function (e) {
            e.preventDefault();

            let idTipoDocumento = e.currentTarget.dataset['tipo'];

            if (client.accion !== 'nuevo') {

                let params = {};
                params.path = window.location.hostname;
                params.idEmpleado = client.idSeleccionado;
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

                        if (doc.Contenido) {

                            let blob = null;
                            if (doc.Extension === 'png' || doc.Extension === 'jpg' || doc.Extension === 'jpeg' || doc.Extension === 'bmp') {
                                blob = utils.b64toBlob(doc.Contenido, 'data:image/png', 512);
                            } else if (doc.Extension === 'pdf') {
                                blob = utils.b64toBlob(doc.Contenido, 'data:document/pdf', 512);
                            }
                            else {
                                blob = utils.b64toBlob(doc.Contenido, '', 512);
                            }
                            if (blob) {
                                const blobUrl = URL.createObjectURL(blob);
                                const link = document.createElement('a');
                                link.href = blobUrl;
                                link.setAttribute('download', doc.Nombre + "." + doc.Extension);
                                document.body.appendChild(link);
                                link.click();
                                document.body.removeChild(link);
                            }

                        }
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });
            }

        });

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            client.nuevo();

        });



        $('#btnGuardar').on('click', (e) => {
            e.preventDefault();

            let hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (hasErrors) {

                $('#spnMensajes').html(mensajesAlertas.solicitudNoProcedenteCamposVacios);
                $('#panelMensajes').modal('show');

                return;
            }

            //  Deshabilitamos boton de guardar, para que no vaya a enviar otro request de guardar mietnras se esta guardando
            $('.deshabilitable').prop('disabled', true);
            $('#btnGuardar').html(`<i class="fa fa-paper-plane mr-1"></i>Guardando...`);


            let dataClient = {};
            dataClient.IdCliente = client.idSeleccionado;



            dataClient.FechaSolicitud = $('#txtFechaSolicitud').val();
            dataClient.Monto = $('#txtCantidadPrestamo').val();
            dataClient.IdTipoCliente = $('#comboTipoCliente').val() == null ? 0 : $('#comboTipoCliente').val();
            dataClient.Curp = $('#txtCURP').val();
            dataClient.PrimerApellido = $('#txtPrimerApellido').val();
            dataClient.SegundoApellido = $('#txtSegundoApellido').val();
            dataClient.Nombre = $('#txtNombre').val();
            dataClient.Telefono_1 = $('#txtTelefono').val();
            dataClient.Ocupacion = $('#txtOcupacion').val();
            dataClient.OcupacionAval = $('#txtOcupacionAval').val();


            dataClient.CurpAval = $('#txtCURPAval').val();
            dataClient.PrimerApellidoAval = $('#txtPrimerApellidoAval').val();
            dataClient.SegundoApellidoAval = $('#txtSegundoApellidoAval').val();
            dataClient.NombreAval = $('#txtNombreAval').val();
            dataClient.TelefonoAval = $('#txtTelefonoAval').val();






            let dataAddressClient = {};
            dataAddressClient.IdCliente = client.idSeleccionado;
            dataAddressClient.Calle = $('#txtCalle').val();
            dataAddressClient.Aval = 0;
            dataAddressClient.Colonia = $('#txtColonia').val();
            dataAddressClient.Municipio = $('#txtMunicipio').val();
            dataAddressClient.Estado = $('#txtEstado').val();
            dataAddressClient.CodigoPostal = $('#txtCodigoPostal').val();
            dataAddressClient.DireccionTrabajo = $('#txtDireccionTrabajo').val();

            let dataAddressClientAval = {};
            dataAddressClientAval.IdCliente = client.idSeleccionado;
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
            params.itemAddress = dataAddressClient;
            params.itemAddressAval = dataAddressClientAval;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params.accion = client.accion;
            params = JSON.stringify(params);

            let urlService = "Save";

            //console.log(urlService);

            $.ajax({
                type: "POST",
                url: `../../pages/Loans/LoanRequest.aspx/${urlService}`,
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

                    //debugger;
                    if (parseInt(valores.CodigoError) === 0) {


                        //guardar documentos
                        $('.campo-imagen').each(function (i, item) {

                            let idTipoDocumento = item.dataset['tipo'];

                            let file;
                            if (file = this.files[0]) {

                                utils.sendFileEmployee(file, 'documento', valores.IdItem, idTipoDocumento, "cliente");

                            }

                        });


                        $('#btnGuardar').html(`<i class="fa fa-save mr-1"></i>Guardar`);


                        let time_ = 10000;
                        setTimeout(function () {                         

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');

                            $('#spnMensajeControlado').html(mensajesAlertas.solicitudPrestamoEnviada);
                            $('#panelMensajeControlado').modal('show');

                        }, time_);


                        $('.deshabilitable').prop('disabled', false);
                        $('#btnGuardar').html(`<i class="fa fa-save mr-1"></i>Guardar`);

                    } else {

                        $('.deshabilitable').prop('disabled', false);
                        $('#btnGuardar').html(`<i class="fa fa-save mr-1"></i>Guardar`);

                        $('#spnMensajes').html(valores.MensajeError);
                        $('#panelMensajes').modal('show');


                        utils.toast(mensajesAlertas.errorGuardar, 'error');

                        return;

                    }



                }, error: function (XMLHttpRequest, textStatus, errorThrown) {


                    utils.toast(mensajesAlertas.errorGuardar, 'error');




                }

            });

        });


        $('.campo-imagen').on('change', function (e) {
            e.preventDefault();

            let idTipoDocumento = e.currentTarget.dataset['tipo'];

            if (client.idSeleccionado !== "-1" && client.accion !== 'nuevo') {


                let file;
                if (file = this.files[0]) {

                    utils.sendFileEmployee(file, 'update_document_employee', client.idSeleccionado, idTipoDocumento, "cliente");

                    setTimeout(function () {

                        //  Mostrar la imagen que se acaba de subir...
                        client.getDocument(client.idSeleccionado, idTipoDocumento, `#img_${idTipoDocumento}`);

                    }, 5000);

                }
            }


        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $(`.documentos`).attr('src', '../../img/upload.png');

            window.location = "LoansIndex.aspx";


        });


        $('#btnAceptarPanelMensajeControlado').on('click', (e) => {
            e.preventDefault();


            setTimeout(function () {
                console.log(`Retornnado a lista de prestamos`);
                window.location = "LoansIndex.aspx";


            }, 2000);


        });





        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = window.location.hostname;
            params.id = client.idSeleccionado;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Loans/LoanRequest.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        client.cargarItems();

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

    client.init();

    client.accionesBotones();

});


