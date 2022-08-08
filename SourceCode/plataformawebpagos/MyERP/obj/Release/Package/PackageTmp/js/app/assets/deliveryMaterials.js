'use strict';
let date = new Date();
let descargas = "ACTIVOS_" + date.getFullYear() + "_" + date.getMonth() + "_" + date.getUTCDay() + "_" + date.getMilliseconds();
let pagina = '57';




const deliveryMaterial = {

    

    init: () => {

        $('#panelTabla').show();
        $('#panelForm').hide();

        deliveryMaterial.idSeleccionado = -1;
        deliveryMaterial.accion = '';

        deliveryMaterial.loadContent();
        deliveryMaterial.loadComboEmpleado();
        deliveryMaterial.loadComboTipo();

    },

    loadContent() {

        let params = {};
        params.path = window.location.hostname;
        console.log(params.idUsuario = document.getElementById('txtIdUsuario').value);
        
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetListaItems",
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

                        { data: 'Empleado.Nombre' },
                        { data: 'Categoria.Nombre' },
                        { data: 'MaterialEntregado' },
                        { data: 'Cantidad' },
                        { data: 'CostoMx' },
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

        deliveryMaterial.idSeleccionado = id;


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
        console.log(id);
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetItem",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let item = msg.d;
                deliveryMaterial.idSeleccionado = item.IdMaterialEntrega;

                $('#txtMaterial').val(item.MaterialEntregado);
                $('#comboCategoria').val(item.Categoria.Id);
                $('#comboEmpleado').val(item.Empleado.IdEmpleado);

                $('#txtCantidad').val(item.Cantidad);
                $('#txtCosto').val(item.Costo);
                $('#txtFecha').val(item.Fecha);



                $('#panelTabla').hide();
                $('#panelForm').show();


                deliveryMaterial.accion = "editar";
                $('#spnTituloForm').text('Editar');
                $('.deshabilitable').prop('disabled', false);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });


    },



   

    loadComboEmpleado: () => {

        var params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;
        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetListaItemsEmpleados",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.IdEmpleado}' > ${item.Nombre}</option > `;

                }

                $('#comboEmpleado').html(opcion);

            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(textStatus + ": " + XMLHttpRequest.responseText);
            }

        });
    },

    loadComboTipo: () => {

        var params = {};
        params.path = window.location.hostname;
        params.idUsuario = document.getElementById('txtIdUsuario').value;

        params = JSON.stringify(params);

        $.ajax({
            type: "POST",
            url: "../../pages/Assets/DeliveryMaterials.aspx/GetListaItemsCategorias",
            data: params,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (msg) {

                let items = msg.d;
                let opcion = '<option value="">Seleccione...</option>';

                for (let i = 0; i < items.length; i++) {
                    let item = items[i];

                    opcion += `<option value = '${item.Id}' > ${item.Nombre}</option > `;

                }

                $('#comboCategoria').html(opcion);

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




        $('#panelTabla').hide();
        $('#panelForm').show();
        deliveryMaterial.accion = "nuevo";
        deliveryMaterial.idSeleccionado = -1;

        $('.deshabilitable').prop('disabled', false);
        deliveryMaterial.fechaHoy();



    },


    fechaHoy() {
        let today = new Date();

        let dayMonth = today.getDate();
        dayMonth = dayMonth.toString().length === 1 ? `0${dayMonth}` : dayMonth;
        let month = (today.getMonth() + 1);
        month = month.toString().length === 1 ? `0${month}` : month;

        $('#txtFecha').val(`${today.getFullYear()}-${month}-${dayMonth}`);

    },

    accionesBotones: () => {

        $('#btnNuevo').on('click', (e) => {
            e.preventDefault();


            deliveryMaterial.nuevo();

        });


        $('#btnGuardar').click(function (e) {

            e.preventDefault();

            var hasErrors = $('form[name="frm"]').validator('validate').has('.has-error').length;

            if (!hasErrors) {

                //  Objeto con los valores a enviar
                let item = {};


                item.IdMaterialEntrega = deliveryMaterial.idSeleccionado;
                item.MaterialEntregado = $('#txtMaterial').val();
                item.IdCategoria = $('#comboCategoria').val();
                item.IdEmpleado = $('#comboEmpleado').val();
                item.Cantidad = $('#txtCantidad').val();
                item.Costo = $('#txtCosto').val();
                //item.Fecha = $('#txtFecha').val();



                let params = {};
                params.path = window.location.hostname;
                params.item = item;
                params.accion = deliveryMaterial.accion;
                params.idUsuario = document.getElementById('txtIdUsuario').value;
                params = JSON.stringify(params);

                $.ajax({
                    type: "POST",
                    url: "../../pages/Assets/DeliveryMaterials.aspx/Save",
                    
                    data: params,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: true,
                    success: function (msg) {
                        let resultado = parseInt(msg.d);

                        if (resultado > 0) {

                            utils.toast(mensajesAlertas.exitoGuardar, 'ok');


                            $('#panelTabla').show();
                            $('#panelForm').hide();

                            deliveryMaterial.loadContent();


                        } else {

                            utils.toast(mensajesAlertas.errorGuardar, 'fail');


                        }



                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(textStatus + ": " + XMLHttpRequest.responseText);
                    }

                });


            }

        });

        $('#btnEliminarAceptar').on('click', (e) => {

            let params = {};
            params.path = window.location.hostname;
            params.id = deliveryMaterial.idSeleccionado;
            params.idUsuario = document.getElementById('txtIdUsuario').value;
            params = JSON.stringify(params);

            $.ajax({
                type: "POST",
                url: "../../pages/Assets/DeliveryMaterials.aspx/Delete",
                data: params,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: true,
                success: function (msg) {

                    var resultado = msg.d;
                    if (resultado.MensajeError === null) {

                        utils.toast(mensajesAlertas.exitoEliminar, 'ok');


                        deliveryMaterial.loadContent();

                    } else {

                        utils.toast(resultado.MensajeError, 'error');


                    }


                }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(textStatus + ": " + XMLHttpRequest.responseText);
                }

            });

        });




        $('#btnCancelar').on('click', (e) => {
            e.preventDefault();

            $('#panelTabla').show();
            $('#panelForm').hide();

        });

    }


}

window.addEventListener('load', () => {

    deliveryMaterial.init();

    deliveryMaterial.accionesBotones();

});


