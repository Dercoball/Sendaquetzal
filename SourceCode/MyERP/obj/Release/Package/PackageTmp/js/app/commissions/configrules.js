'use strict';
let date = new Date();
let descargas = "Reglas_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '17';




const configComissions = {


    init: () => {

        $('.secciones').hide();
        $('#panelTabla').show();

        configComissions.idReglaSeleccionada = -1;
        configComissions.accion = '';
        configComissions.idComision = -1;
        configComissions.NombreComision = '';

        configComissions.loadItems();


    },

    loadItems() {

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Commissions/ConfigRules.aspx/GetItemsCommissions",
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
                        { data: 'Nombre' },
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
                            extend: 'csvHtml5',
                            title: descargas,
                            text: '&nbsp;Csv', className: 'csvbtn'
                        },
                    ]

                });


            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });



    },




    open(idComision, nombreComision) {

        //console.log(`idComision ${idComision}`);
        configComissions.idComision = idComision;
        configComissions.nombreComision = nombreComision;

        let params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params.idComision = idComision;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Commissions/ConfigRules.aspx/GetItemsRulesByCommission",
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

                let html = '';
                let total = 0;
                data.forEach((item, i) => {
                    html += `<tr>`;
                    html += `<td>${item.Descripcion}</td>`;
                    html += `<td>${item.PonderacionStr}</td>`;
                    html += `<td>${item.Accion}</td>`;
                    html += `</tr>`;

                    total += item.Ponderacion;
                });

                html += `<tr>`;
                html += `<th>Total</th>`;
                html += `<th>${total}%</th>`;
                html += `<th></th>`;
                html += `</tr>`;

                $('#tableReglas tbody').empty().append(html);



                $('#nombreComision').html(configComissions.nombreComision);

                $('.secciones').hide();
                $('#panelTablaReglas').show();



            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);


            }

        });


    },

    deleteRule: (id) => {

        configComissions.idReglaSeleccionada = id;


        $('#mensajeEliminar').text(`Se eliminará el registro seleccionado (No. ${id}). ¿Desea continuar ?`);
        $('#panelEliminar').modal('show');

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
            url: "../../pages/Config/Commissions.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;
                configComissions.idSeleccionado = item.IdComision;

                $('#txtNombre').val(item.Nombre);

                $('#txtPorcentaje').val(item.Porcentaje);


                $('#chkActivo').prop('checked', item.Activo === 1);

                $('.secciones').hide();
                $('#panelTabla').show();


                configComissions.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);

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


        $('#txtComision').val(configComissions.nombreComision);


        $('.secciones').hide();
        $('#panelForm').show();

        configComissions.accion = "nuevo";
        configComissions.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);




    },



    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();

            configComissions.nuevo();

        });


        $('#btnGuardar').on('click', function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};
                item.IdReglaEvaluacionModulo = configComissions.idReglaSeleccionada;
                item.IdComision = configComissions.idComision;
                item.Descripcion = $('#txtNombre').val();
                item.Ponderacion = $('#txtPonderacion').val();


                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = configComissions.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Commissions/ConfigRules.aspx/Save",
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                            $('.secciones').hide();
                            $('#panelTabla').show();

                            configComissions.open(configComissions.idComision, configComissions.nombreComision);



                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }

                        $('#panelEdicion').modal('hide');


                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });



        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('.secciones').hide();
            $('#panelTablaReglas').show();

        });


        $('#btnVolver').on('click', (e) => {
            e.preventDefault();

            $('.secciones').hide();
            $('#panelTabla').show();

        });

        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = window.location.hostname;
            params.id = configComissions.idReglaSeleccionada;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Commissions/ConfigRules.aspx/DeleteRegla",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        configComissions.open(configComissions.idComision, configComissions.nombreComision);

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

    configComissions.init();

    configComissions.accionesBotones();

});


